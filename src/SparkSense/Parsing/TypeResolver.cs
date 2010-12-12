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

        public TypeResolver(ITypeDiscoveryService typeDiscoveryService)
        {
            _typeDiscoveryService = typeDiscoveryService;
        }

        public IEnumerable<Type> Resolve()
        {
            return _typeDiscoveryService.GetTypes(typeof (object), true) as IEnumerable<Type>;
        }

        public IEnumerable<MemberInfo> Resolve(string searchString, Flags flags)
        {
            IEnumerable<Type> types = Resolve();

            IEnumerable<MemberInfo> members = types.SelectMany(
                type => type.Members(flags | Flags.ExcludeBackingMembers));
            return members;
        }
    }
}