using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class Text
    {
        public static string SpaceCapitalisedVariableName(string variableName)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < variableName.Length; i++)
            {
                char c = variableName[i];
                if ((i > 0) && char.IsUpper(c))
                {
                    result.Append(" ");
                }
                result.Append(c);
            }
            return result.ToString();
        }
    }
}
