// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.ClrSafeFunctions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class ClrSafeFunctions
  {
    public static string SubstringStart(string str, int startIndex)
    {
      if (startIndex < 0)
        startIndex = 0;
      return startIndex > str.Length ? string.Empty : str.Substring(startIndex);
    }

    public static string SubstringStartAndLength(string str, int startIndex, int length)
    {
      if (startIndex < 0)
        startIndex = 0;
      int length1 = str.Length;
      if (startIndex > length1)
        return string.Empty;
      length = Math.Min(length, length1 - startIndex);
      return length < 0 ? string.Empty : str.Substring(startIndex, length);
    }
  }
}
