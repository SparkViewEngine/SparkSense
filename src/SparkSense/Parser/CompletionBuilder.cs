using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SparkSense.Parser
{
    public class CompletionBuilder
    {
        public IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types)
        {
            var typeCompletions = types
                .Distinct()
                .Select(type => new Completion(type.Name));

            var firstNamespaceElements = types.Select(t => t.Namespace != null ? t.Namespace.Split('.').First() : string.Empty);
            var namespaceCompletions = firstNamespaceElements.Distinct().Select(ns => new Completion(ns));

            return typeCompletions.Union(namespaceCompletions);
        }

        public IEnumerable<Completion> ToCompletionList(IEnumerable<Type> types, string startingPoint)
        {
            var namespaceElements = types.Select(
                t => t.Namespace != null && t.Namespace.StartsWith(startingPoint)
                         ? t.Namespace.Remove(0, startingPoint.Length).Split('.').First()
                         : string.Empty);

            return namespaceElements.Distinct()
                .Where(ns => !string.IsNullOrEmpty(ns))
                .Select(ns => new Completion(ns));
        }
    }
}