using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.Language.Intellisense;
using Spark.Compiler;
using SparkSense.Parsing;

namespace SparkSense.Parser
{
    public class CompletionBuilder
    {
        private readonly TypeNavigator _typeNavigator;
        private readonly IViewExplorer _viewExplorer;

        public CompletionBuilder(IViewExplorer viewExplorer)
        {
            if (viewExplorer == null) throw new ArgumentNullException("viewExplorer");
            _viewExplorer = viewExplorer;
            _typeNavigator = viewExplorer.GetTypeNavigator();
        }

        public IEnumerable<Completion> ToCompletionList(string codeSnippit)
        {
            Type resolvedType;
            string remainingCode;
            if (_typeNavigator.TryResolveType(_viewExplorer, codeSnippit, out resolvedType, out remainingCode))
            {
                if (string.IsNullOrEmpty(remainingCode))
                    return ResolveTypeMemberCompletions(resolvedType);
                return ResolveInstanceMemberCompletions(resolvedType, remainingCode);
            }

            IEnumerable<string> namespaceElements =
                _typeNavigator.Types.Select(t => _typeNavigator.GetMatchingNamespaceElements(t, codeSnippit));
            IEnumerable<ViewDataChunk> viewDataElements = _viewExplorer.GetViewDataVariableChunks();
            IEnumerable<LocalVariableChunk> viewLocalElements = _viewExplorer.GetLocalVariableChunks();
            if (string.IsNullOrEmpty(codeSnippit))
            {
                IEnumerable<Type> triggerTypes = _typeNavigator.GetTriggerTypes();
                IEnumerable<MemberInfo> viewMembers = _typeNavigator.GetViewInstanceMembers(triggerTypes);
                return triggerTypes
                    .Union(viewMembers).ToCompletionList()
                    .Union(namespaceElements.ToCompletionList())
                    .Union(viewDataElements.ToCompletionList())
                    .Union(viewLocalElements.ToCompletionList());
            }
            return namespaceElements.ToCompletionList();
        }

        private IEnumerable<Completion> ResolveInstanceMemberCompletions(Type resolvedType, string remainingCode)
        {
            if (remainingCode.EndsWith("(")) return null;
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