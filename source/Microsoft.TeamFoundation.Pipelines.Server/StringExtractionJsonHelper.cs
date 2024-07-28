// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.StringExtractionJsonHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class StringExtractionJsonHelper
  {
    public static string GetFragmentExcludingTokens(
      string content,
      string fragmentStartToken,
      string fragmentEndToken)
    {
      if (!string.IsNullOrEmpty(content))
      {
        int num1 = content.IndexOf(fragmentStartToken, StringComparison.Ordinal);
        if (num1 >= 0)
        {
          int startIndex = num1 + fragmentStartToken.Length;
          int num2 = content.IndexOf(fragmentEndToken, startIndex, StringComparison.Ordinal);
          return num2 >= 0 ? content.Substring(startIndex, num2 - startIndex) : content.Substring(startIndex);
        }
      }
      return string.Empty;
    }
  }
}
