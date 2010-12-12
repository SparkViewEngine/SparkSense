using System;
using System.ComponentModel.Design;
using System.Linq;
using Fasterflect;
using NUnit.Framework;
using Rhino.Mocks;
using SparkSense.Parsing;

namespace SparkSense.Tests.Parsing
{
    [TestFixture]
    public class TypeResolutionTests
    {
        private ITypeDiscoveryService _typeDiscoveryService;
        private Type[] _referencedTypes;

        [SetUp]
        public void Setup()
        {
            _referencedTypes = this.GetType().GetNestedTypes();
            _typeDiscoveryService = MockRepository.GenerateMock<ITypeDiscoveryService>();
            _typeDiscoveryService.Expect(x => x.GetTypes(typeof(object), true)).Return(_referencedTypes);
        }

        [TearDown]
        public void TearDown()
        {
            _typeDiscoveryService.VerifyAllExpectations();
            _typeDiscoveryService = null;
        }

        [Test]
        public void ShouldGetAvailableTypes()
        {
            var typeResolver = new TypeResolver(_typeDiscoveryService);
            var types = typeResolver.Resolve();

            Assert.AreEqual(1, types.ToList().Count);
            Assert.Contains(typeof(SomeType), types.ToList());
        }

        [Test]
        public void ShouldGetAvailablePublicStaticMembers()
        {
            var typeResolver = new TypeResolver(_typeDiscoveryService);
            var members = typeResolver.Resolve("SomeType", Flags.StaticAnyVisibility);
            
            Assert.AreEqual(1, members.ToList().Count);
        }

        [Test]
        public void ShouldGetAvailablePublicInstanceMembers()
        {
            var typeResolver = new TypeResolver(_typeDiscoveryService);
            var members = typeResolver.Resolve("SomeType", Flags.InstanceAnyVisibility);
            
            Assert.AreEqual(3, members.ToList().Count);
        }

        public class SomeType
        {
            public static string SomeStaticString;

            public SomeType(){}

            public string SomeMethod()
            {
                return string.Empty;
            }

            public string SomeProp { get; set; }
        }
    }
}