// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.StringUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class StringUtils
  {
    public static string TrimEnd(
      this string input,
      string suffixToRemove,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
      return input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType) ? input.Substring(0, input.Length - suffixToRemove.Length) : input;
    }

    public static string TrimStart(
      this string input,
      string prefixToRemove,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
      return input != null && prefixToRemove != null && input.StartsWith(prefixToRemove, comparisonType) ? input.Substring(prefixToRemove.Length, input.Length - prefixToRemove.Length) : input;
    }
  }
}
