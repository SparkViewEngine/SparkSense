using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace SparkSense.Parsing
{
    public class TypeNavigator
    {
        private readonly Flags _commonFlags;
        private readonly ITypeDiscoveryService _typeDiscoveryService;
        private IEnumerable<Type> _types;

        public TypeNavigator(ITypeDiscoveryService typeDiscoveryService)
            : this()
        {
            if (typeDiscoveryService == null) throw new ArgumentNullException("typeDiscoveryService");
            _typeDiscoveryService = typeDiscoveryService;
            _types = _typeDiscoveryService.GetTypes(typeof (object), true) as IEnumerable<Type>;
        }

        public TypeNavigator(IEnumerable<Type> types)
            : this()
        {
            if (types == null) throw new ArgumentNullException("types");
            _types = types;
        }

        private TypeNavigator()
        {
            _commonFlags = Flags.Public | Flags.ExcludeBackingMembers | Flags.TrimExplicitlyImplemented;
        }

        public IEnumerable<Type> Types
        {
            get { return _types ?? (_types = _typeDiscoveryService.GetTypes(typeof (object), true) as IEnumerable<Type>); }
        }

        public IEnumerable<MemberInfo> GetStaticMembers()
        {
            return Types.SelectMany(t => t.Members(_commonFlags | Flags.StaticAnyVisibility));
        }

        public IEnumerable<MemberInfo> GetInstanceMembers()
        {
            return Types.SelectMany(t => t.Members(_commonFlags | Flags.InstanceAnyVisibility));
        }

        public IEnumerable<MethodInfo> GetMethodByName(string methodName)
        {
            return Types.SelectMany(t => t.Methods(_commonFlags | Flags.StaticInstanceAnyVisibility, new[] {methodName}));
        }

        public IEnumerable<Type> GetTriggerTypes()
        {
            return Types.Where(t => t.Members(_commonFlags | Flags.StaticAnyVisibility).Count > 0);
        }
    }
}