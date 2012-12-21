using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Extensions
    {
        public static Regex _assemblyQualifiedNameWithoutVersionRegex = new Regex(@", (?:(?:Version|Culture|PublicKeyToken)=[^,\]]+|mscorlib)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Dictionary<Type, String> _assemblyQualifiedNameWithoutVersionLookup = new Dictionary<Type, string>();

        public static string AssemblyQualifiedNameWithoutVersion(this Type type)
        {
            string result;
            _assemblyQualifiedNameWithoutVersionLookup.TryGetValue(type, out result);
            if (result != null)
                return result;

            string name = type.AssemblyQualifiedName;
            if (false)
            {
                name = _assemblyQualifiedNameWithoutVersionRegex.Replace(name, "");
            }
            else
            {
                name = RemoveText(name, ", Version=");
                name = RemoveText(name, ", Culture=");
                name = RemoveText(name, ", PublicKeyToken=");
                name = RemoveText(name, ", mscorlib");
            }
            _assemblyQualifiedNameWithoutVersionLookup[type] = name;
            return name;
        }

        private static string RemoveText(string text, string find)
        {
            int index = text.IndexOf(find, StringComparison.CurrentCultureIgnoreCase);
            while (index >= 0)
            {
                int endIndex = text.IndexOfAny(new char[] { ',', ']' }, index + find.Length);
                if (endIndex >= 0)
                {
                    text = text.Substring(0, index) + text.Substring(endIndex);
                    index = text.IndexOf(find, index, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    text = text.Substring(0, index);
                    break;
                }
            }
            return text;
        }

    }
}
