using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.Language.Intellisense;
using Spark;
using SparkSense.Parsing;

namespace SparkSense.Parser
{
    public class CompletionBuilder
    {
        private readonly TypeNavigator _typeNavigator;

        public CompletionBuilder(TypeNavigator typeNavigator)
        {
            _typeNavigator = typeNavigator;
        }

        //Obsolete
        public CompletionBuilder()
        {
        }

        public IEnumerable<Completion> ToCompletionList(string codeSnippit)
        {
            Type resolvedType;
            string remainingCode;
            if (_typeNavigator.TryResolveType(codeSnippit, out resolvedType, out remainingCode))
            {
                
            }




            if (string.IsNullOrEmpty(codeSnippit))
                return ToCompletionList(_typeNavigator.GetTriggerTypes());
            return ToCompletionList(_typeNavigator.Types, codeSnippit);
        }

        public IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types, string codeSnippit)
        {
            if (string.IsNullOrEmpty(codeSnippit)) return ToCompletionList(types);

            IEnumerable<Completion> namespaceCompletions = GetNamespaceCompletions(types, codeSnippit);
            IEnumerable<Completion> typeStaticMemberCompletions = GetTypeStaticMemberCompletions(types, codeSnippit);
            IEnumerable<Completion> objectMemberCompletions = GetObjectMemberCompletions(types, codeSnippit);

            return namespaceCompletions
                .Union(typeStaticMemberCompletions)
                .Union(objectMemberCompletions)
                .Distinct();
        }

        private static IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types)
        {
            IEnumerable<Completion> typeCompletions = types.Distinct().Select(type => new Completion(type.Name));
            IEnumerable<Completion> namespaceCompletions = GetNamespaceCompletions(types, string.Empty);
            IEnumerable<Completion> viewMemberCompletions = GetViewInstanceMemberCompletions(types);

            return typeCompletions.Union(namespaceCompletions).Union(viewMemberCompletions).Distinct();
        }

        private static IEnumerable<Completion> GetObjectMemberCompletions(IEnumerable<Type> types, string codeSnippit)
        {
            var parentMemberName = codeSnippit.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
            var typeNavigator = new TypeNavigator(types);
            var property = typeNavigator.GetPropertyByName(parentMemberName).FirstOrDefault();
            if (property == null)
            {
                return new List<Completion>();
            }
            IEnumerable<MemberInfo> members = typeNavigator.GetMembers(property);
            IEnumerable<Completion> memberCompletions = members.Select(m => new Completion(m.Name));
            return memberCompletions;
        }

        private static IEnumerable<Completion> GetTypeStaticMemberCompletions(IEnumerable<Type> types, string codeSnippit)
        {
            var typeNavigator = new TypeNavigator(types.Where(t => codeSnippit.TrimEnd('.').EndsWith(t.Name)));

            IEnumerable<MemberInfo> staticMembersFromAllTypes = typeNavigator.GetStaticMembers();

            return staticMembersFromAllTypes.Distinct()
                .Select(m => new Completion(m.Name));
        }

        private static IEnumerable<Completion> GetViewInstanceMemberCompletions(IEnumerable<Type> types)
        {
            var viewTypeNavigator = new TypeNavigator(types.Where(t => typeof(SparkViewBase).IsAssignableFrom(t)));
            return viewTypeNavigator.GetInstanceMembers().Distinct()
                .Select(m => new Completion(m.Name));
        }

        private static IEnumerable<Completion> GetNamespaceCompletions(IEnumerable<Type> types, string codeSnippit)
        {
            IEnumerable<string> namespaceElements = types.Select(t => GetMatchingNamespaceElements(t, codeSnippit));
            return namespaceElements.Distinct().Where(ns => !string.IsNullOrEmpty(ns)).Select(ns => new Completion(ns));
        }

        private static string GetMatchingNamespaceElements(Type type, string codeSnippit)
        {
            string fullName = !string.IsNullOrEmpty(type.FullName) ? type.FullName.Replace('+', '.') : string.Empty;
            return fullName.Contains(codeSnippit)
                       ? fullName.Remove(0, fullName.IndexOf(codeSnippit) + codeSnippit.Length).Split('.').First()
                       : string.Empty;
        }
    }
}