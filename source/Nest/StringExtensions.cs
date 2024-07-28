// Decompiled with JetBrains decompiler
// Type: Nest.StringExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  internal static class StringExtensions
  {
    internal static string ToCamelCase(this string s)
    {
      if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
        return s;
      string camelCase = char.ToLowerInvariant(s[0]).ToString();
      if (s.Length > 1)
        camelCase += s.Substring(1);
      return camelCase;
    }
  }
}
