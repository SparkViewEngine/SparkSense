using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Fasterflect;
using Microsoft.VisualStudio.Language.Intellisense;
using SparkSense.Parsing;

namespace SparkSense.Parser
{
    public static class CompletionBuilderExtensions
    {
        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<MethodInfo> methods)
        {
            IEnumerable<Completion> completions = methods.Select(
                x => new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Method), null));
            return completions;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<PropertyInfo> properties)
        {
            IEnumerable<Completion> completions = properties.Select(
                x => new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Property), null));
            return completions;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<MemberInfo> members)
        {
            IEnumerable<Completion> completions = members.Select(
                x =>
                {
                    switch (x.MemberType)
                    {
                        case MemberTypes.Property:
                            return new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Property), null);
                        case MemberTypes.Method:
                            return new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Method), null);
                        case MemberTypes.Field:
                            return new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Field), null);
                        case MemberTypes.NestedType:
                        case MemberTypes.TypeInfo:
                            return new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Class), null);
                        case MemberTypes.Event:
                            return new Completion(GetDisplayText(x), GetInsertionText(x), GetMemberDescription(x), GetIcon(Constants.ICON_Event), null);
                        case MemberTypes.Constructor:
                            return null;
                        case MemberTypes.Custom:
                        case MemberTypes.All:
                        default:
                            return new Completion(GetDisplayText(x));
                    }
                });
            return completions.Where(x => x != null);
        }

        private static string GetDisplayText(MemberInfo member)
        {
            string name = member.Name;
            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    break;
                case MemberTypes.Event:
                    break;
                case MemberTypes.Field:
                    break;
                case MemberTypes.Method:
                    break;
                case MemberTypes.Property:
                    break;
                case MemberTypes.TypeInfo:
                    var type = (Type)member;
                    if (type.IsGenericTypeDefinition)
                        name = string.Format("{0}<>", name.Split('`').First());
                    break;
                case MemberTypes.Custom:
                    break;
                case MemberTypes.NestedType:
                    break;
                case MemberTypes.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string displayText = string.Format("{0}", name);
            return displayText;
        }

        private static string GetInsertionText(MemberInfo member)
        {
            string name = member.Name;
            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    break;
                case MemberTypes.Event:
                    break;
                case MemberTypes.Field:
                    break;
                case MemberTypes.Method:
                    break;
                case MemberTypes.Property:
                    break;
                case MemberTypes.TypeInfo:
                    var type = (Type)member;
                    if (type.IsGenericTypeDefinition)
                        name = string.Format("{0}<>", name.Split('`').First());
                    break;
                case MemberTypes.Custom:
                    break;
                case MemberTypes.NestedType:
                    break;
                case MemberTypes.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string insertionText = string.Format("{0}", name);
            return insertionText;
        }

        private static string GetMemberDescription(MemberInfo member)
        {
            string name = member.Name;
            string membertype = member.MemberType.ToString();
            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    break;
                case MemberTypes.Event:
                    break;
                case MemberTypes.Field:
                    break;
                case MemberTypes.Method:
                    var method = (MethodInfo)member;
                    membertype = method.ReturnType.Name;
                    var methodArgs = method.Parameters().Select(x => string.Format("{0} {1}", x.ParameterType, x.Name));
                    name += string.Format("({0})", string.Join(", ", methodArgs));
                    break;
                case MemberTypes.Property:
                    var property = ((PropertyInfo)member).Type();
                    membertype = property.IsGenericType
                        ? GetGenericFormatting(property)
                        : property.FullName;
                    break;
                case MemberTypes.TypeInfo:
                    membertype = "Class";
                    var type = (Type)member;
                    if (type.IsGenericTypeDefinition)
                        name = GetGenericFormatting(type);
                    break;
                case MemberTypes.Custom:
                    break;
                case MemberTypes.NestedType:
                    break;
                case MemberTypes.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            string description = string.Format("{0} {1}", membertype, name);
            return description;
        }

        public static IEnumerable<Completion> ToCompletionList(this IEnumerable<string> elements)
        {
            IEnumerable<Completion> completions =
                elements.Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith("<")).Distinct().Select(
                    x => new Completion(x, x, string.Format("{0} Namespace", x), GetIcon(Constants.ICON_Namespace), null));
            return completions;
        }

        private static BitmapImage GetIcon(string iconName)
        {
            BitmapImage icon;
            try
            {
                icon = new BitmapImage(new Uri(String.Format("pack://application:,,,/SparkSense;component/Resources/{0}.png", iconName), UriKind.Absolute));
            }
            catch (UriFormatException ex)
            {
                icon = new BitmapImage();
            }
            return icon;
        }

        private static string GetGenericFormatting(Type type)
        {
            string name;
            string args = string.Join(", ", type.GetGenericArguments().Select(x => x.Name));
            name = string.Format("{0}<{1}>", type.FullName.Split('`').First(), args);
            return name;
        }
    }
}