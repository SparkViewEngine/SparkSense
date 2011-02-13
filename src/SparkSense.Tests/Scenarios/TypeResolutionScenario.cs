using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rhino.Mocks;
using SparkSense.Parsing;

namespace SparkSense.Tests.Scenarios
{
    public class TypeResolutionScenario : Scenario
    {
        private readonly ITypeDiscoveryService _typeDiscoveryService;
        private TypeResolver _typeResolver;

        public TypeResolutionScenario()
        {
            _typeDiscoveryService = MockRepository.GenerateMock<ITypeDiscoveryService>();
        }

        protected IEnumerable<MemberInfo> TheResolvedMembers { get; private set; }

        protected void GivenReferencedTypes(ICollection types)
        {
            _typeResolver = new TypeResolver(_typeDiscoveryService);
            _typeDiscoveryService.Stub(x => x.GetTypes(typeof (object), true)).Return(types);
        }

        protected void WhenLookingUpStaticMembers()
        {
            TheResolvedMembers = _typeResolver.GetStaticMembers();
        }

        protected void WhenLookingUpInstanceMembers()
        {
            TheResolvedMembers = _typeResolver.GetInstanceMembers();
        }

        protected void WhenLookingUpSomeCode(string codeSnippit)
        {
        }

        public class StubType
        {
            public static string StubStaticField;

            public string StubInstanceField;

            public static string StubStaticProperty { get; set; }

            public string StubInstanceProperty { get; set; }

            public static string StubStaticMethod()
            {
                return string.Empty;
            }

            public string StubInstanceMethod()
            {
                return string.Empty;
            }
        }
    }
}