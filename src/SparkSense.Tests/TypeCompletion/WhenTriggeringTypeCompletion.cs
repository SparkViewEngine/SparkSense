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
        }

        [Test]
        public void ShouldIncludeTypesWithPublicMembersInTheCompletionList()
        {
            WhenTriggeringAnInitialCompletion();
            TheCompletionList.ShouldNotContain(c => c.DisplayText == "StubPrivateType");

            TheCompletionList.ShouldContain(c => c.DisplayText == "StubTypeWithNoStatics");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubType");
            TheCompletionList.ShouldContain(c => c.DisplayText == "String");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Int32");
        }

        [Test]
        public void ShouldIncludeInitialNamespacesOfIncludedTypesInTheCompletionList()
        {
            WhenTriggeringAnInitialCompletion();
            TheCompletionList.ShouldContain(c => c.DisplayText == "SparkSense");
            TheCompletionList.ShouldContain(c => c.DisplayText == "System");
        }

        [Test]
        public void ShouldNotContainDuplicatesInTheCompletionList()
        {
            WhenTriggeringAnInitialCompletion();
            TheCompletionList.ShouldHaveCount(1, c => c.DisplayText == "System");
        }

        [Test]
        public void ShouldContainStaticMembersInTheCompletionList()
        { 
            WhenTriggeringACompletion("SparkSense.Tests.Scenarios.TypeResolutionScenario.StubType.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticField");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticMethod");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticProperty");

            WhenTriggeringACompletion("Scenarios.TypeResolutionScenario.StubType.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticField");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticMethod");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticProperty");

            WhenTriggeringACompletion("StubType.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticField");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticMethod");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticProperty");
        }
    }
}