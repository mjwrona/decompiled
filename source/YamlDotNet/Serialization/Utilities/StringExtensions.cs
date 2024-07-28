// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Utilities.StringExtensions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Serialization.Utilities
{
  internal static class StringExtensions
  {
    private static string ToCamelOrPascalCase(string str, Func<char, char> firstLetterTransform)
    {
      string str1 = Regex.Replace(str, "([_\\-])(?<char>[a-z])", (MatchEvaluator) (match => match.Groups["char"].Value.ToUpperInvariant()), RegexOptions.IgnoreCase);
      return firstLetterTransform(str1[0]).ToString() + str1.Substring(1);
    }

    public static string ToCamelCase(this string str) => StringExtensions.ToCamelOrPascalCase(str, new Func<char, char>(char.ToLowerInvariant));

    public static string ToPascalCase(this string str) => StringExtensions.ToCamelOrPascalCase(str, new Func<char, char>(char.ToUpperInvariant));

    public static string FromCamelCase(this string str, string separator)
    {
      str = char.ToLower(str[0]).ToString() + str.Substring(1);
      str = Regex.Replace(str.ToCamelCase(), "(?<char>[A-Z])", (MatchEvaluator) (match => separator + match.Groups["char"].Value.ToLowerInvariant()));
      return str;
    }
  }
}
