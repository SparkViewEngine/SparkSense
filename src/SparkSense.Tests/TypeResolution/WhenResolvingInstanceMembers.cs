using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeResolution
{
    public class WhenResolvingInstanceMembers : TypeResolutionScenario
    {
        public WhenResolvingInstanceMembers()
        {
            GivenReferencedTypes(new[] {typeof (StubType)});
            WhenLookingUpInstanceMembers();
        }

        [Test]
        public void ShouldFilterOutBackingAndStaticMembers()
        {
            TheResolvedMembers
                .ShouldNotContain(m => m.Name == "get_StubStaticProperty")
                .ShouldNotContain(m => m.Name == "StubStaticProperty")
                .ShouldNotContain(m => m.Name == "StubStaticMethod");
        }

        [Test]
        public void ShouldResolvePublicInstanceField()
        {
            TheResolvedMembers
                .ShouldContain(m => m.Name == "StubInstanceField");
        }

        [Test]
        public void ShouldResolvePublicInstanceMethod()
        {
            TheResolvedMembers
                .ShouldContain(m => m.Name == "StubInstanceMethod");
        }

        [Test]
        public void ShouldResolvePublicInstanceProperty()
        {
            TheResolvedMembers
                .ShouldContain(m => m.Name == "StubInstanceProperty");
        }
    }
}