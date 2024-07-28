// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.StringExtensions
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class StringExtensions
  {
    public static string FirstLetterToLowerCase(this string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return value;
      char[] charArray = value.ToCharArray();
      charArray[0] = char.ToLowerInvariant(charArray[0]);
      return new string(charArray);
    }
  }
}
