using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Spark;

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
            _types = _typeDiscoveryService.GetTypes(typeof(object), true) as IEnumerable<Type>;
        }

        public TypeNavigator(IEnumerable<Type> types)
            : this()
        {
            if (types == null) throw new ArgumentNullException("types");
            _types = types;
        }

        private TypeNavigator()
        {
            _commonFlags = Flags.Public | Flags.ExcludeBackingMembers | Flags.TrimExplicitlyImplemented | Flags.PartialNameMatch;
        }

        public IEnumerable<Type> Types
        {
            get { return _types ?? (_types = _typeDiscoveryService.GetTypes(typeof (object), true) as IEnumerable<Type>); }
        }

        public IEnumerable<MemberInfo> GetStaticMembers()
        {
            return Types.SelectMany(t => t.Members(_commonFlags | Flags.Static));
        }

        public IEnumerable<MemberInfo> GetInstanceMembers()
        {
            var members = Types.SelectMany(t => t.Members(_commonFlags | Flags.Instance));
            return members;
        }

        public IEnumerable<MethodInfo> GetMethodByName(string methodName)
        {
            return Types.SelectMany(t => t.Methods(_commonFlags | Flags.Static | Flags.Instance, new[] { methodName }));
        }

        public IEnumerable<PropertyInfo> GetPropertyByName(string propertyName)
        {
            return Types.SelectMany(t => t.Properties(_commonFlags | Flags.Static | Flags.Instance, new[] { propertyName }));            
        }

        public IEnumerable<Type> GetTriggerTypes()
        {
            var types = Types.Where(t => (t.IsPublic || t.IsNestedPublic) && t.Members(_commonFlags | Flags.Static | Flags.Instance).Count > 0);
            return types;
        }

        public IEnumerable<MemberInfo> GetMembers(PropertyInfo property)
        {
            var members = property.Type().Members(_commonFlags | Flags.Static | Flags.Instance);
            return members;
        }

        public bool TryResolveType(string codeSnippit, out Type resolvedType, out string remainingCode)
        {
            resolvedType = null;
            remainingCode = codeSnippit;

            var fragments = codeSnippit.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Reverse();

            foreach (var fragment in fragments)
            {
                resolvedType = GetResolvedTypeByInstanceKeyword(fragment) ?? GetResolvedTypeByName(fragment);
                if (resolvedType != null)
                    break;
            }

            if (resolvedType != null) remainingCode = GetRemainingCode(fragments, resolvedType);

            return resolvedType != null;
        }

        private string GetRemainingCode(IEnumerable<string> fragments, Type resolvedType)
        {
            string typeName = resolvedType.Name;
            string remainingCode = string.Join(".", fragments.TakeWhile(s => s != typeName && s != "this").Reverse());
            return remainingCode;
        }

        private Type GetResolvedTypeByInstanceKeyword(string fragment)
        {
            Type resolvedType = null;
            if (fragment == "this")
            {
                IEnumerable<Type> resolvedTypes = Types.Where(t => typeof (SparkViewBase).IsAssignableFrom(t));
                //BUG: This will potentially return the wrong Type if there are more than one inheritor of SparkViewBase
                resolvedType = resolvedTypes.FirstOrDefault(t => t.Name != typeof(SparkViewBase).Name);
            }
            return resolvedType;
        }

        private Type GetResolvedTypeByName(string fragment)
        {
            Type resolvedType = null;
            IEnumerable<Type> resolvedTypes = Types.Where(t => t.Name.Contains(fragment));
            if (resolvedTypes.Count() == 1)
            {
                resolvedType = resolvedTypes.First();
            }
            if (resolvedTypes.Count(t => t.Name == fragment) == 1)
            {
                resolvedType = resolvedTypes.First(t => t.Name == fragment);
            }
            return resolvedType;
        }
    }
}