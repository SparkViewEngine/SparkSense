using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using NUnit.Framework;
using SparkSense.Tests.Scenarios;

namespace SparkSense.Tests.TypeResolution
{
    public class WhenResolvingMethods : TypeResolutionScenario
    {
        public WhenResolvingMethods()
        {
            GivenReferencedTypes(new[] {typeof (StubType)});
            WhenLookingUpMethods("StubMethodWithParameters");
        }

        [Test]
        public void ShouldResolveMethodParameters()
        {
            IList<ParameterInfo> parameters = TheResolvedMethods
                .Where(m => m.Parameters().Count == 3).FirstOrDefault().Parameters();

            parameters.ShouldHaveCount(3);
            parameters[0].Name.ShouldBe("param1");
            parameters[0].ParameterType.Name.ShouldBe("String");
            parameters[1].Name.ShouldBe("param2");
            parameters[1].ParameterType.Name.ShouldBe("Int32");
            parameters[2].Name.ShouldBe("param3");
            parameters[2].ParameterType.Name.ShouldBe("StubType");
        }

        [Test]
        public void ShouldResolveMultipleMethodsWithSameName()
        {
            TheResolvedMethods
                .ShouldHaveCount(3);
        }
    }
}