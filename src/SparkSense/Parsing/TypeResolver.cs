using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace SparkSense.Parsing
{
    public class TypeResolver
    {
        private readonly ITypeDiscoveryService _typeDiscoveryService;
        private IEnumerable<Type> _types;
        private readonly Flags _commonFlags;

        public TypeResolver(ITypeDiscoveryService typeDiscoveryService)
        {
            if (typeDiscoveryService == null) throw new ArgumentNullException("typeDiscoveryService");
            _typeDiscoveryService = typeDiscoveryService;
            _types = _typeDiscoveryService.GetTypes(typeof(object), true) as IEnumerable<Type>;
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
    }
}