using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using SparkSense.Parsing;

namespace SparkSense
{
    public interface ISparkServiceProvider
    {
        IProjectExplorer ProjectExplorer { get; }
        DTE VsEnvironment { get; }
        IVsEditorAdaptersFactoryService AdaptersFactoryService { get; }
        DynamicTypeService TypeService { get; }
        T GetService<T>();
        T GetService<T>(Type serviceType);
    }

    [Export(typeof (ISparkServiceProvider))]
    public class SparkServiceProvider : ISparkServiceProvider
    {
        [Import] private IVsEditorAdaptersFactoryService _adaptersFactoryService;
        private IProjectExplorer _projectExplorer;
        [Import(typeof (SVsServiceProvider))] private IServiceProvider _serviceProvider;
        private DynamicTypeService _typeService;
        private DTE _vsEnvironment;

        #region ISparkServiceProvider Members

        public T GetService<T>()
        {
            return (T) _serviceProvider.GetService(typeof (T));
        }

        public T GetService<T>(Type serviceType)
        {
            return (T) _serviceProvider.GetService(serviceType);
        }

        public IVsEditorAdaptersFactoryService AdaptersFactoryService
        {
            get { return _adaptersFactoryService; }
        }

        public IProjectExplorer ProjectExplorer
        {
            get
            {
                if (_projectExplorer == null)
                    _projectExplorer = VsEnvironment != null ? new ProjectExplorer(this) : null;
                return _projectExplorer;
            }
        }

        public DTE VsEnvironment
        {
            get
            {
                if (_vsEnvironment == null)
                    _vsEnvironment = GetService<DTE>();
                return _vsEnvironment;
            }
        }

        public DynamicTypeService TypeService
        {
            get
            {
                if (_typeService == null)
                {
                    _typeService = GetService<DynamicTypeService>();
                    Debug.Assert(_typeService != null, "No dynamic type service available.");
                }
                return _typeService;
            }
        }

        #endregion
    }
}