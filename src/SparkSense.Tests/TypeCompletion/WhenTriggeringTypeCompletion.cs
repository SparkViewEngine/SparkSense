using System;
using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeCompletion
{
    public class WhenTriggeringTypeCompletion : TypeCompletionScenario
    {
        public WhenTriggeringTypeCompletion()
        {
            GivenReferencedTypes(new[] {typeof (StubType), typeof(StubTypeWithNoStatics), typeof (String), typeof (Int32)});
            WhenTriggeringAnInitialCompletion();
        }

        [Test]
        public void ShouldIncludeTypesWithStaticMembersInTheCompletionList()
        {
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "StubTypeWithNoStatics");

            TheCompletionList.ShouldContain(c => c.DisplayText == "StubType");
            TheCompletionList.ShouldContain(c => c.DisplayText == "String");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Int32");
        }

        [Test]
        public void ShouldIncludeInitialNamespacesOfIncludedTypesInTheCompletionList()
        {
            TheCompletionList.ShouldContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldContain(c => c.DisplayText == "System");
        }

        [Test]
        public void ShouldNotContainDuplicatesInTheCompletionList()
        {
            TheCompletionList.ShouldHaveCount(1, c => c.DisplayText == "System");
        }

    }
}