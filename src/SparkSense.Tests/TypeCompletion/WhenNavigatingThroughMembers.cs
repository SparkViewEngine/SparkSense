using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeCompletion
{
    public class WhenNavigatingThroughMembers : TypeCompletionScenario
    {
        public WhenNavigatingThroughMembers()
        {
            GivenReferencedTypes(new[] { typeof(StubType) });
        }

        [Test]
        public void ShouldFindInitialObjectMembers()
        {
            WhenTriggeringACompletion("StubType.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticProperty");
        }

        [Test]
        public void ShouldFindInitialObjectMembersWithPartialText()
        {
            WhenTriggeringACompletion("StubType.St");
            TheCompletionList.ShouldContain(c => c.DisplayText == "StubStaticProperty");
        }

        [Test]
        public void ShouldDrillDownToObjectMembers()
        {
            WhenTriggeringACompletion("StubType.StubStaticProperty.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "ToLower");
        }

        [Test]
        public void ShouldDrillDownToObjectMembersWithPartialText()
        {
            WhenTriggeringACompletion("StubType.StubStaticProperty.Spl");
            TheCompletionList.ShouldContain(c => c.DisplayText == "ToLower");
        }

        [Test]
        public void ShouldDrillDownThroughObjectLayers()
        {
            WhenTriggeringACompletion("StubType.StubTypeInstanceProperty.StubInstanceProperty.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "ToLower");
        }

        [Test]
        public void ShouldDrillDownThroughObjectMethods()
        {
            WhenTriggeringACompletion("StubType.StubTypeInstanceProperty.StubTypeInstanceProperty.StubInstanceProperty.ToLower().");
            TheCompletionList.ShouldContain(c => c.DisplayText == "Split");
        }
    }
}