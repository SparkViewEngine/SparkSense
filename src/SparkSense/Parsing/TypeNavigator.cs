using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Spark;
using Spark.Compiler;

namespace SparkSense.Parsing
{
    public class TypeNavigator
    {
        private readonly Flags _commonFlags;
        private readonly ITypeDiscoveryService _typeDiscoveryService;
        private IEnumerable<Type> _types;

        public TypeNavigator(IEnumerable<Type> types)
            : this()
        {
            if (types == null) throw new ArgumentNullException("types");
            _types = types;
        }

        private TypeNavigator()
        {
            _commonFlags = Flags.Public | Flags.ExcludeBackingMembers | Flags.TrimExplicitlyImplemented;// | Flags.PartialNameMatch;
        }

        public IEnumerable<Type> Types
        {
            get { return _types ?? (_types = _typeDiscoveryService.GetTypes(typeof (object), true) as IEnumerable<Type>); }
        }

        public IEnumerable<MemberInfo> GetStaticMembers()
        {
            return Types.SelectMany(t => t.Members(_commonFlags | Flags.Static));
        }

        public IEnumerable<MemberInfo> GetViewInstanceMembers(IEnumerable<Type> types)
        {
            return GetInstanceMembers(types.Where(t => typeof (SparkViewBase).IsAssignableFrom(t)));
        }

        public IEnumerable<MemberInfo> GetInstanceMembers(IEnumerable<Type> types)
        {
            var members = types.SelectMany(t => t.Members(_commonFlags | Flags.Instance));
            return members;
        }

        public IEnumerable<MemberInfo> GetInstanceMembers()
        {
            return GetInstanceMembers(Types);
        }

        public MemberInfo GetMemberByName(Type type, string memberName, Flags flags)
        {
            if (memberName == null) return null;
            var member = type.Member(memberName, _commonFlags | flags);
            return member;
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
            var types = Types.Where(t => (t.IsPublic || t.IsNestedPublic));
            return types;
        }

        public IEnumerable<MemberInfo> GetMembers(Type type, Flags flags)
        {
            var members = type.Members(_commonFlags | flags);
            return members;
        }

        public IEnumerable<MethodInfo> GetMethods(Type type)
        {
            var methods = type.Methods(_commonFlags | Flags.Static | Flags.Instance);
            return methods;
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            var properties = type.Properties(_commonFlags | Flags.Static | Flags.Instance);
            return properties;
        }

        public IEnumerable<FieldInfo> GetFields(Type type)
        {
            var fields = type.Fields(_commonFlags | Flags.Static | Flags.Instance);
            return fields;
        }

        public bool TryResolveType(IViewExplorer viewExplorer, string codeSnippit, out Type resolvedType, out string remainingCode)
        {
            resolvedType = null;
            remainingCode = codeSnippit;

            var fragments = codeSnippit.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Reverse();

            string instanceName = string.Empty;
            foreach (var fragment in fragments)
            {
                resolvedType = GetResolvedTypeByInstance(viewExplorer, fragment, out instanceName) ?? GetResolvedTypeByName(fragment);
                if (resolvedType != null)
                    break;
            }

            if (resolvedType != null) remainingCode = GetRemainingCode(fragments, resolvedType, instanceName);

            return resolvedType != null;
        }

        public MemberInfo GetResolvedMember(Type resolvedType, string fragment, Flags flags)
        {
            if (String.IsNullOrEmpty(fragment)) return null;
            MemberInfo member = GetMemberByName(resolvedType, fragment, flags);
            return member == null || member.Name.Length != fragment.Length 
                ? null 
                : member;
        }

        public Type GetResolvedType(MemberInfo resolvedMember)
        {
            if (resolvedMember == null) return null;

            return resolvedMember.IsInvokable()
                       ? resolvedMember.DeclaringType.Methods(resolvedMember.Name).First().ReturnType
                       : resolvedMember.Type();
        }

        public string GetMatchingNamespaceElements(Type type, string codeSnippit)
        {
            string fullName = !String.IsNullOrEmpty(type.FullName) ? type.FullName.Replace('+', '.') : String.Empty;
            return fullName.Contains(codeSnippit)
                       ? fullName.Remove(0, fullName.IndexOf(codeSnippit) + codeSnippit.Length).Split('.').First()
                       : String.Empty;
        }

        private string GetRemainingCode(IEnumerable<string> fragments, Type resolvedType, string instanceName)
        {
            string typeName = resolvedType.Name;
            string remainingCode = String.Join(".", fragments.TakeWhile(s => s != typeName && s != instanceName).Reverse());
            return remainingCode;
        }

        private Type GetResolvedTypeByInstance(IViewExplorer viewExplorer, string fragment, out string instanceName)
        {
            instanceName = string.Empty;
            Type resolvedType = null;
            if (fragment == "this")
            {
                instanceName = "this";
                IEnumerable<Type> resolvedTypes = Types.Where(t => typeof(SparkViewBase).IsAssignableFrom(t));
                //BUG: This will potentially return the wrong Type if there is more than one inheritor of SparkViewBase
                resolvedType = resolvedTypes.FirstOrDefault(t => t.Name != typeof(SparkViewBase).Name);
            }
            else
            {
                resolvedType = GetResolvedTypeFromViewVariables(fragment, viewExplorer);
                instanceName = resolvedType != null ? fragment : string.Empty;
            }

            return resolvedType;
        }

        private Type GetResolvedTypeFromViewVariables(string fragment, IViewExplorer viewExplorer)
        {
            Type resolvedType = null;
            var locals = viewExplorer.GetLocalVariableChunks();
            var viewData = viewExplorer.GetViewDataVariableChunks();
            if (locals != null)
            {
                var localFound = locals.Where(x => x.Value == fragment).FirstOrDefault();
                if (localFound != null) resolvedType = Type.GetType(localFound.Type, false);
            }
            if (viewData != null)
            {
                var viewDataFound = viewData.Where(x => x.Key == fragment).FirstOrDefault();
                if (resolvedType == null && viewDataFound != null) resolvedType = Type.GetType(viewDataFound.Type, false);
            }

            return resolvedType;
        }

        private Type GetResolvedTypeByName(string fragment)
        {
            IEnumerable<Type> resolvedTypes = Types.Where(t => t.Name.Equals(fragment));
            return resolvedTypes.FirstOrDefault();
        }
    }
}