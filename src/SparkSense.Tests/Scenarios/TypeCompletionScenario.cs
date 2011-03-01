using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;
using SparkSense.Parser;

namespace SparkSense.Tests.Scenarios
{
    public class TypeCompletionScenario : TypeResolutionScenario
    {
        protected IEnumerable<Completion> TheCompletionList { get; private set; }

        protected void WhenTriggeringAnInitialCompletion()
        {
            var completionBuilder = new CompletionBuilder();
            WhenLookingUpTriggerTypes();
            TheCompletionList = completionBuilder.ToCompletionList(TheResolvedTriggerTypes);
        }

        protected void WhenTriggeringACompletionFromAPoint(string continuation)
        {
            var completionBuilder = new CompletionBuilder();
            WhenLookingUpTriggerTypes();
            TheCompletionList = completionBuilder.ToCompletionList(TheResolvedTriggerTypes, continuation);            
        }
    }
}