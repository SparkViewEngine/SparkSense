using System;
using NUnit.Framework;
using Spark;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeResolution
{
    public class WhenResolvingTypes : TypeResolutionScenario
    {
        public WhenResolvingTypes()
        {
            GivenReferencedTypes(new[] { typeof(StubType), typeof(String), typeof(SparkViewBase), typeof(AbstractSparkView)});
        }

        [Test]
        public void ShouldNotResolveTypeByNamespaceOnly()
        {
            WhenLookingUpSomeCode("SparkSense.Tests.Scenarios.");
            TheResolvedType
                .ShouldBe(null);
            TheRemainingCode
             .ShouldBe("SparkSense.Tests.Scenarios.");
        }

        [Test]
        public void ShouldTryResolveTypeByName()
        {
            WhenLookingUpSomeCode("StubType.");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("StubType");
            TheRemainingCode
              .ShouldBe(string.Empty);
        }

        [Test]
        public void ShouldTryResolveTypeByFullName()
        {
            WhenLookingUpSomeCode("SparkSense.Tests.Scenarios.TypeResolutionScenario.StubType.");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("StubType");
            TheRemainingCode
              .ShouldBe(string.Empty);
        }

        [Test]
        public void ShouldTryResolveTypeByPartialFullName()
        {
            WhenLookingUpSomeCode("Scenarios.TypeResolutionScenario.StubType.");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("StubType");
            TheRemainingCode
             .ShouldBe(string.Empty);
        }

        [Test]
        public void ShouldTryResolveTypeByNameAndMember()
        {
            WhenLookingUpSomeCode("StubType.StubInstanceProperty.");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("StubType");
            TheRemainingCode
                .ShouldBe("StubInstanceProperty");

            WhenLookingUpSomeCode("String.ToLower(");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("String");
            TheRemainingCode
                .ShouldBe("ToLower(");

            WhenLookingUpSomeCode("String.ToLower().ToUpper(");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("String");
            TheRemainingCode
                .ShouldBe("ToLower().ToUpper(");
        }

        [Test]
        public void ShouldTryResolveTypeByKeyword()
        {
            WhenLookingUpSomeCode("this.");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("AbstractSparkView");
            TheRemainingCode
                .ShouldBe(string.Empty);

            WhenLookingUpSomeCode("this.OutputScope(");
            TheResolvedType
                .ShouldNotBeNull()
                .Name.ShouldBe("AbstractSparkView");
            TheRemainingCode
                .ShouldBe("OutputScope(");
        }
    }
}