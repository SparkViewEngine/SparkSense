using System;
using System.Collections.Generic;

namespace SparkSense.Parsing
{
    public interface IViewExplorer
    {
        IList<string> GetRelatedPartials();
        IList<string> GetPossibleMasterLayouts();
        IList<string> GetPossiblePartialDefaults(string partialName);
        IList<string> GetGlobalVariables();
        IList<string> GetLocalVariables();
        IList<string> GetContentNames();
        IList<string> GetLocalMacros();
        IList<string> GetMacroParameters(string macroName);
        IEnumerable<Type> GetTriggerTypes();
        IList<string> GetMembers();
        void InvalidateView(string newContent);
    }
}
