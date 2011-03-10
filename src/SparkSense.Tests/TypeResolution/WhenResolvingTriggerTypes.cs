using System;
using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeResolution
{
    public class WhenResolvingTriggerTypes : TypeResolutionScenario
    {
        public WhenResolvingTriggerTypes()
        {
            GivenReferencedTypes(new[] { typeof(StubType), typeof(String), typeof(StubTypeWithNoStatics), typeof(StubPrivateType) });
            WhenLookingUpTriggerTypes();
        }

        [Test]
        public void ShouldOnlyResolveTypesThatArePublic()
        {
            TheResolvedTriggerTypes
                .ShouldHaveCount(3)
                .ShouldNotContain(t => t.Name == "StubPrivateType")
                .ShouldContain(t => t.Name == "StubType");

            TheResolvedTriggerTypes
                .ShouldContain(t => t.Name == "StubTypeWithNoStatics");

            TheResolvedTriggerTypes
                .ShouldContain(t => t.Name == "String");
        }
    }
}