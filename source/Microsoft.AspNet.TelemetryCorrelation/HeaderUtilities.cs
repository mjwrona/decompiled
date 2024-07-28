// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.HeaderUtilities
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal static class HeaderUtilities
  {
    internal static int GetNextNonEmptyOrWhitespaceIndex(
      string input,
      int startIndex,
      bool skipEmptyValues,
      out bool separatorFound)
    {
      separatorFound = false;
      int index1 = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
      if (index1 == input.Length || input[index1] != ',')
        return index1;
      separatorFound = true;
      int startIndex1 = index1 + 1;
      int index2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      int startIndex2;
      if (skipEmptyValues)
      {
        for (; index2 < input.Length && input[index2] == ','; index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2))
          startIndex2 = index2 + 1;
      }
      return index2;
    }
  }
}
