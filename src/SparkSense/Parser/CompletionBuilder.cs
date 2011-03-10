using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Spark;
using SparkSense.Parsing;

namespace SparkSense.Parser
{
    public class CompletionBuilder
    {
        public IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types, string startingPoint)
        {
            if (string.IsNullOrEmpty(startingPoint)) return ToCompletionList(types);

            IEnumerable<Completion> namespaceCompletions = GetNamespaceCompletions(types, startingPoint);
            IEnumerable<Completion> memberCompletions = GetStaticMemberCompletions(types, startingPoint);

            return namespaceCompletions.Union(memberCompletions).Distinct();
        }


        private static IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types)
        {
            IEnumerable<Completion> typeCompletions = types.Distinct().Select(type => new Completion(type.Name));
            IEnumerable<Completion> namespaceCompletions = GetNamespaceCompletions(types, string.Empty);
            IEnumerable<Completion> viewMemberCompletions = GetViewInstanceMemberCompletions(types);

            return typeCompletions.Union(namespaceCompletions).Union(viewMemberCompletions).Distinct();
        }

        private static IEnumerable<Completion> GetStaticMemberCompletions(IEnumerable<Type> types, string startingPoint)
        {
            var typeNavigator = new TypeNavigator(types.Where(t => startingPoint.TrimEnd('.').EndsWith(t.Name)));
            return typeNavigator.GetStaticMembers().Distinct()
                .Select(m => new Completion(m.Name));
        }

        private static IEnumerable<Completion> GetViewInstanceMemberCompletions(IEnumerable<Type> types)
        {
            var viewTypeNavigator = new TypeNavigator(types.Where(t => typeof(ISparkView).IsAssignableFrom(t)));
            return viewTypeNavigator.GetInstanceMembers().Distinct()
                .Select(m => new Completion(m.Name));
        }

        private static IEnumerable<Completion> GetNamespaceCompletions(IEnumerable<Type> types, string startingPoint)
        {
            IEnumerable<string> namespaceElements = types.Select(t => GetMatchingNamespaceElements(t, startingPoint));
            return namespaceElements.Distinct().Where(ns => !string.IsNullOrEmpty(ns)).Select(ns => new Completion(ns));
        }

        private static string GetMatchingNamespaceElements(Type type, string startingPoint)
        {
            string fullName = !string.IsNullOrEmpty(type.FullName) ? type.FullName.Replace('+', '.') : string.Empty;
            return fullName.Contains(startingPoint)
                       ? fullName.Remove(0, fullName.IndexOf(startingPoint) + startingPoint.Length).Split('.').First()
                       : string.Empty;
        }
    }
}