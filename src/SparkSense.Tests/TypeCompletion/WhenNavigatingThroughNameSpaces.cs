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
        public void ShouldIncludeNameSpaceItemsInTheCompletionList()
        {
            WhenTriggeringAnInitialCompletion();
            TheCompletionList.ShouldContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldContain(c => c.DisplayText == "SparkSense");
        }

        [Test]
        public void ShouldNotIncludeTypesThemselvesInTheCompletionList()
        {
            WhenTriggeringACompletionFromAPoint("SparkSense.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "StubType");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "String");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Int32");
        }

        [Test]
        public void ShouldNotIncludeAnyEmptyItemsInTheCompletionList()
        {
            WhenTriggeringACompletionFromAPoint("SparkSense.Tests.Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);
        }

        [Test]
        public void ShouldBeAbleToDigDownThroughNamespaceLayers()
        {
            WhenTriggeringACompletionFromAPoint("SparkSense.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Tests");

            WhenTriggeringACompletionFromAPoint("SparkSense.Tests.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Scenarios");

            WhenTriggeringACompletionFromAPoint("SparkSense.Tests.Scenarios.");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "System");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Tests");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "Scenarios");
            TheCompletionList.ShouldNotContain(c => c.DisplayText == string.Empty);
        }
    }
}