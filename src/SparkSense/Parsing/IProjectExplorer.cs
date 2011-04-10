using System;
using System.Collections.Generic;
using Spark.FileSystem;
using Microsoft.VisualStudio.Text;

namespace SparkSense.Parsing
{
    public interface IProjectExplorer
    {
        bool ViewFolderExists();
        IViewFolder GetViewFolder();
        IViewExplorer GetViewExplorer(ITextBuffer textBuffer);
        string GetCurrentViewPath(ITextBuffer textBuffer);
        void SetViewContent(string viewPath, string content);
        IEnumerable<Type> GetProjectReferencedTypes();
    }
}
