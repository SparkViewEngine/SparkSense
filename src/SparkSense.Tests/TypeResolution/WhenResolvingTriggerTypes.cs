using System;
using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeResolution
{
    public class WhenResolvingTriggerTypes : TypeResolutionScenario
    {
        public WhenResolvingTriggerTypes()
        {
            GivenReferencedTypes(new[] { typeof(StubType), typeof(String), typeof(StubTypeWithNoStatics) });
            WhenLookingUpTriggerTypes();
        }

        [Test]
        public void ShouldOnlyResolveTypesThatHaveStaticMembers()
        {
            TheResolvedTriggerTypes
                .ShouldHaveCount(2)
                .ShouldContain(t => t.Name == "StubType");

            TheResolvedTriggerTypes
                .ShouldContain(t => t.Name == "String");
        }
    }
}