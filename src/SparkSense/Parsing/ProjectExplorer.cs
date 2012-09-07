using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Spark.FileSystem;

namespace SparkSense.Parsing
{
    public class ProjectExplorer : IProjectExplorer
    {
        private readonly ISparkServiceProvider _services;
        private ITypeDiscoveryService _discovery;
        private IVsHierarchy _hier;
        private IEnumerable<Type> _projectReferencedTypes;
        private CachingViewFolder _projectViewFolder;

        public ProjectExplorer(ISparkServiceProvider services)
        {
            if (services == null) throw new ArgumentNullException("services", "services is null.");
            _services = services;
            InitTypes();
        }

        private void InitTypes()
        {
            InitTypeDiscoveryService();
            GetProjectReferencedTypes();
        }

        private CachingViewFolder ProjectViewFolder
        {
            get
            {
                string activeDocumentPath;
                if (!TryGetActiveDocumentPath(out activeDocumentPath)) return null;

                if (_projectViewFolder == null || _projectViewFolder.BasePath != GetViewRoot(activeDocumentPath))
                {
                    _hier = null;
                    _discovery = null;
                    _projectReferencedTypes = null;
                    _projectViewFolder = new CachingViewFolder(GetViewRoot(activeDocumentPath) ?? string.Empty);
                    InitTypes();
                    BuildViewMapFromProjectEnvironment();
                }
                return _projectViewFolder;
            }
        }

        #region IProjectExplorer Members

        public bool ViewFolderExists()
        {
            string activeDocumentPath;
            if (!TryGetActiveDocumentPath(out activeDocumentPath)) return false;

            int viewsLocationStart = activeDocumentPath.LastIndexOf("Views");
            return viewsLocationStart != -1;
        }

        public IViewFolder GetViewFolder()
        {
            string activeDocumentPath;
            if (!TryGetActiveDocumentPath(out activeDocumentPath)) return null;

            return ProjectViewFolder;
        }

        public IViewExplorer GetViewExplorer(ITextBuffer textBuffer)
        {
            IViewExplorer viewExplorer;
            if (textBuffer.Properties.TryGetProperty(typeof (ViewExplorer), out viewExplorer))
                return viewExplorer;

            viewExplorer = new ViewExplorer(this, GetCurrentViewPath(textBuffer));
            textBuffer.Properties.AddProperty(typeof (ViewExplorer), viewExplorer);
            return viewExplorer;
        }

        public string GetCurrentViewPath(ITextBuffer textBuffer)
        {
            var adapter = _services.AdaptersFactoryService.GetBufferAdapter(textBuffer) as IPersistFileFormat;
            if (adapter == null) return string.Empty;

            string filename;
            uint format;
            adapter.GetCurFile(out filename, out format);
        	return GetViewPath(filename);
        }

        public void SetViewContent(string viewPath, string content)
        {
            ProjectViewFolder.SetViewSource(viewPath, content);
        }

        public IEnumerable<Type> GetProjectReferencedTypes()
        {
            if (_projectReferencedTypes != null) return _projectReferencedTypes;
            try
            {
                _projectReferencedTypes = _discovery.GetTypes(typeof (object), true) as IEnumerable<Type>;
            }
            catch (NullReferenceException)
            {
                //Type Discovery service isn't ready yet.
            }
            catch (Exception)
            {
                throw;
            }
            return _projectReferencedTypes;
        }

        #endregion

        public string GetCurrentViewPath()
        {
            string activeDocumentPath;
            if (!TryGetActiveDocumentPath(out activeDocumentPath)) return null;
        	return GetViewPath(activeDocumentPath);
        }

        public bool HasView(string viewPath)
        {
            return ProjectViewFolder.HasView(viewPath);
        }

        private bool TryGetActiveDocumentPath(out string activeDocumentPath)
        {
            activeDocumentPath = _services.VsEnvironment.ActiveDocument != null
                                     ? _services.VsEnvironment.ActiveDocument.FullName
                                     : string.Empty;
            return !String.IsNullOrEmpty(activeDocumentPath);
        }

        private void BuildViewMapFromProjectEnvironment()
        {
            if (_services.VsEnvironment.ActiveDocument == null) return;
            Project currentProject = _services.VsEnvironment.ActiveDocument.ProjectItem.ContainingProject;
            foreach (ProjectItem projectItem in currentProject.ProjectItems)
                ScanProjectItemForViews(projectItem);
        }

        private void ScanProjectItemForViews(ProjectItem projectItem)
        {
            if (projectItem.Name.EndsWith(".spark"))
            {
                string projectItemMap = GetProjectItemMap(projectItem);
                if (!string.IsNullOrEmpty(projectItemMap))
                    ProjectViewFolder.Add(projectItemMap);
            }

            if (projectItem.ProjectItems != null)
                foreach (ProjectItem child in projectItem.ProjectItems)
                    ScanProjectItemForViews(child);
        }

        private static string GetProjectItemMap(ProjectItem projectItem)
        {
            if (projectItem.Properties == null) return null;

            string fullPath = projectItem.Properties.Item("FullPath").Value.ToString();

            var foundView = GetViewPath(fullPath);

        	return foundView;
        }

    	private static string GetViewPath(string fullPath) {
    		string viewRoot = GetViewRoot(fullPath);
    		return fullPath.Replace(viewRoot, string.Empty).TrimStart('\\');
    	}

    	private static string GetViewRoot(string activeDocumentPath)
        {
            int viewsLocationStart = activeDocumentPath.LastIndexOf("Views");
            return viewsLocationStart != -1 ? activeDocumentPath.Substring(0, viewsLocationStart + 5) : null;
        }

        private IVsHierarchy GetHierarchy()
        {
            if (_hier == null)
            {
                var sln = _services.GetService<IVsSolution>();
                if (_services.VsEnvironment.ActiveDocument != null)
                {
                    string projectName = _services.VsEnvironment.ActiveDocument.ProjectItem.ContainingProject.UniqueName;
                    sln.GetProjectOfUniqueName(projectName, out _hier);
                }
            }
            return _hier;
        }

        private void InitTypeDiscoveryService()
        {
            if (_discovery != null) return;
            DynamicTypeService typeService = _services.TypeService;
            if (typeService != null)
                _discovery = typeService.GetTypeDiscoveryService(GetHierarchy());
        }
    }
}