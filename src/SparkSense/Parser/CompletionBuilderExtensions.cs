using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SparkSense.Parser
{
    public static class CompletionBuilderExtensions
    {
        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<MethodInfo> methods)
        {
            IEnumerable<Completion> completions = methods.Select(x => new Completion(x.Name));
            return completions;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<PropertyInfo> properties)
        {
            IEnumerable<Completion> completions = properties.Select(x => new Completion(x.Name));
            return completions;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<MemberInfo> members)
        {
            IEnumerable<Completion> completions = members.Select(x => new Completion(x.Name));
            return completions;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<string> elements)
        {
            IEnumerable<Completion> completions =
                elements.Where(x => !string.IsNullOrEmpty(x)).Distinct().Select(x => new Completion(x));
            return completions;
        }
    }
}
