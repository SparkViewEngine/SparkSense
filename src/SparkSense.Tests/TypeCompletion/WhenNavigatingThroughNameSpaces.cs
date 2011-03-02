using System;
using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeCompletion
{
    public class WhenNavigatingThroughNameSpaces : TypeCompletionScenario
    {
        public WhenNavigatingThroughNameSpaces()
        {
            GivenReferencedTypes(new[] {typeof (StubType), typeof (String), typeof (Int32)});
        }

        [Test]
        public void ShouldBeAbleToDigDownThroughNamespaceLayers()
        {
            WhenTriggeringACompletion("SparkSense.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Tests");

            WhenTriggeringACompletion("SparkSense.Tests.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Scenarios");

            WhenTriggeringACompletion("SparkSense.Tests.Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Scenarios");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);
        }

        [Test]
        public void ShouldBeAbleToDigDownToTheActualTypeAtTheEndOfTheNamespace()
        {
            WhenTriggeringACompletion("System.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "String");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Int32");

            WhenTriggeringACompletion("SparkSense.Tests.Scenarios.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "TypeResolutionScenario");

            WhenTriggeringACompletion("SparkSense.Tests.Scenarios.TypeResolutionScenario.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubType");
        }

        [Test]
        public void ShouldBeAbleToStartFromPartialyThroughNamespaces()
        {
            WhenTriggeringACompletion("Tests.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Scenarios");

            WhenTriggeringACompletion("Tests.Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Scenarios");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);

            WhenTriggeringACompletion("Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Scenarios");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);
        }

        [Test]
        public void ShouldIncludeNameSpaceItemsInTheCompletionList()
        {
            WhenTriggeringAnInitialCompletion();
            TheCompletionList.ShouldContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldContain(c => c.DisplayText == "SparkSense");
        }

        [Test]
        public void ShouldNotIncludeAnyEmptyItemsInTheCompletionList()
        {
            WhenTriggeringACompletion("SparkSense.Tests.Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);
        }

        [Test]
        public void ShouldNotIncludeTypesThemselvesInTheCompletionList()
        {
            WhenTriggeringACompletion("SparkSense.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "StubType");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "String");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Int32");
        }
    }
}