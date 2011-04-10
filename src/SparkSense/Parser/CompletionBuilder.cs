using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.Language.Intellisense;
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

        public IEnumerable<Completion> ToCompletionList(string codeSnippit)
        {
            Type resolvedType;
            string remainingCode;
            if (_typeNavigator.TryResolveType(codeSnippit, out resolvedType, out remainingCode))
            {
                if (string.IsNullOrEmpty(remainingCode))
                    return ResolveTypeMemberCompletions(resolvedType);
                return ResolveInstanceMemberCompletions(resolvedType, remainingCode);
            }

            IEnumerable<string> namespaceElements =
                _typeNavigator.Types.Select(t => _typeNavigator.GetMatchingNamespaceElements(t, codeSnippit));
            if (string.IsNullOrEmpty(codeSnippit))
            {
                IEnumerable<Type> triggerTypes = _typeNavigator.GetTriggerTypes();
                IEnumerable<MemberInfo> viewMembers = _typeNavigator.GetViewInstanceMembers(triggerTypes);
                return triggerTypes
                    .Union(viewMembers).ToCompletionList()
                    .Union(namespaceElements.ToCompletionList());
            }
            return namespaceElements.ToCompletionList();
        }

        private IEnumerable<Completion> ResolveInstanceMemberCompletions(Type resolvedType, string remainingCode)
        {
            string[] fragments = remainingCode.Split(new[] {".", "(", ")"}, StringSplitOptions.RemoveEmptyEntries);
            var flags = Flags.Instance | Flags.Static;
            MemberInfo resolvedMember = _typeNavigator.GetResolvedMember(resolvedType, fragments.First(), flags);

            IEnumerable<string> remainingFragments = fragments.Skip(1);
            while (resolvedMember != null)
            {
                resolvedType = _typeNavigator.GetResolvedType(resolvedMember) ?? resolvedType;
                resolvedMember = _typeNavigator.GetResolvedMember(resolvedType, remainingFragments.FirstOrDefault(), flags);
                remainingFragments = remainingFragments.Skip(1);
            }

            IEnumerable<MemberInfo> instanceMembers = _typeNavigator.GetMembers(resolvedType, flags);
            return instanceMembers.ToCompletionList();
        }

        private IEnumerable<Completion> ResolveTypeMemberCompletions(Type resolvedType)
        {
            IEnumerable<MemberInfo> members = _typeNavigator.GetMembers(resolvedType, Flags.Static | Flags.Instance);
            IEnumerable<Completion> memberCompletions = members.ToCompletionList();
            return memberCompletions;
        }
    }
}