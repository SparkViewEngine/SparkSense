using System;
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
        private TypeNavigator _typeNavigator;

        public TypeResolutionScenario()
        {
            _typeDiscoveryService = MockRepository.GenerateMock<ITypeDiscoveryService>();
        }

        protected Type TheResolvedType { get; private set; }

        protected IEnumerable<Type> TheResolvedTriggerTypes { get; private set; }

        protected IEnumerable<MemberInfo> TheResolvedMembers { get; private set; }

        protected IEnumerable<MethodInfo> TheResolvedMethods { get; private set; }

        protected string TheRemainingCode { get; private set; }

        public TypeNavigator TheTypeNavigator
        {
            get { return _typeNavigator; }
        }

        protected void GivenReferencedTypes(ICollection types)
        {
            _typeNavigator = new TypeNavigator(_typeDiscoveryService);
            _typeDiscoveryService.Stub(x => x.GetTypes(typeof (object), true)).Return(types);
        }

        protected void WhenLookingUpTriggerTypes()
        {
            TheResolvedTriggerTypes = TheTypeNavigator.GetTriggerTypes();
        }

        protected void WhenLookingUpStaticMembers()
        {
            TheResolvedMembers = TheTypeNavigator.GetStaticMembers();
        }

        protected void WhenLookingUpInstanceMembers()
        {
            TheResolvedMembers = TheTypeNavigator.GetInstanceMembers();
        }

        protected void WhenLookingUpMethods(string methodName)
        {
            TheResolvedMethods = TheTypeNavigator.GetMethodByName(methodName);
        }

        protected void WhenLookingUpSomeCode(string codeSnippit)
        {
            Type resolvedType;
            string remainingCode;
            if (_typeNavigator.TryResolveType(codeSnippit, out resolvedType, out remainingCode))
                TheResolvedType = resolvedType;
            TheRemainingCode = remainingCode;
        }

        protected class StubPrivateType
        {
            public string StubInstanceField;

            public string StubInstanceProperty { get; set; }

            public string StubInstanceMethod()
            {
                return string.Empty;
            }
        }

        public class StubTypeWithNoStatics
        {
            public string StubInstanceField;

            public string StubInstanceProperty { get; set; }

            public string StubInstanceMethod()
            {
                return string.Empty;
            }
        }

        public class StubType
        {
            public static string StubStaticField;

            public string StubInstanceField;

            public static string StubStaticProperty { get; set; }

            public string StubInstanceProperty { get; set; }

            public StubType StubTypeInstanceProperty { get; set; }

            public static string StubStaticMethod()
            {
                return string.Empty;
            }

            public string StubInstanceMethod()
            {
                return string.Empty;
            }

            public string StubMethodWithParameters(string param1)
            {
                return string.Empty;
            }

            public string StubMethodWithParameters(string param1, int param2)
            {
                return string.Empty;
            }

            public string StubMethodWithParameters(string param1, int param2, StubType param3)
            {
                return string.Empty;
            }
        }
    }
}