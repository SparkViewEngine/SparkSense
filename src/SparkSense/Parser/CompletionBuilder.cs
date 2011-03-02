using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using SparkSense.Parsing;

namespace SparkSense.Parser
{
    public class CompletionBuilder
    {
        public IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types, string startingPoint)
        {
            if (string.IsNullOrEmpty(startingPoint)) return ToCompletionList(types);

            IEnumerable<string> namespaceElements = types.Select(t => GetMatchingNamespaceElements(t, startingPoint));
            IEnumerable<Completion> namespaceCompletions = namespaceElements.Distinct()
                .Where(ns => !string.IsNullOrEmpty(ns))
                .Select(ns => new Completion(ns));

            var typeNavigator = new TypeNavigator(types.Where(t => startingPoint.TrimEnd('.').EndsWith(t.Name)));
            IEnumerable<Completion> memberCompletions = typeNavigator.GetStaticMembers().Distinct()
                .Select(m => new Completion(m.Name));

            return namespaceCompletions.Union(memberCompletions).Distinct();
        }

        private static IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types)
        {
            IEnumerable<Completion> typeCompletions = types
                .Distinct()
                .Select(type => new Completion(type.Name));

            IEnumerable<string> namespaceElements = types.Select(t => GetMatchingNamespaceElements(t, string.Empty));
            IEnumerable<Completion> namespaceCompletions = namespaceElements.Distinct().Select(ns => new Completion(ns));

            return typeCompletions.Union(namespaceCompletions);
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