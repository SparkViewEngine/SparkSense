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
        public void ShouldBeAbleToDigDownToObjectMembers()
        {
            WhenTriggeringACompletion("StubType.StubStaticProperty.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "ToLower");
        }

        [Test]
        public void ShouldBeAbleToDigDownThroughObjectLayers()
        {
            WhenTriggeringACompletion("StubType.StubTypeInstanceProperty.StubStaticProperty.");
            TheCompletionList.ShouldContain(c => c.DisplayText == "ToLower");
        }
    }
}