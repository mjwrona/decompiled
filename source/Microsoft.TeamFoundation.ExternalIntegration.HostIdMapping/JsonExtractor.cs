// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.JsonExtractor
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using System;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public static class JsonExtractor
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
