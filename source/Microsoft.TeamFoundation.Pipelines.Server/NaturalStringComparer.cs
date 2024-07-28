// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.NaturalStringComparer
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class NaturalStringComparer : IComparer<string>
  {
    private readonly StringComparison m_stringComparison;

    public NaturalStringComparer(StringComparison stringComparison) => this.m_stringComparison = stringComparison;

    public int Compare(string x, string y)
    {
      if (string.Equals(x, y, this.m_stringComparison))
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      string[] strArray1 = Regex.Split(x, "(\\d+)");
      string[] strArray2 = Regex.Split(y, "(\\d+)");
      for (int index = 0; index < strArray1.Length && index < strArray2.Length; ++index)
      {
        int num = this.CompareParts(strArray1[index], strArray2[index]);
        if (num != 0)
          return num;
      }
      if (strArray1.Length > strArray2.Length)
        return 1;
      return strArray1.Length < strArray2.Length ? -1 : 0;
    }

    private int CompareParts(string x, string y)
    {
      int result1;
      int result2;
      return int.TryParse(x, out result1) && int.TryParse(y, out result2) ? result1.CompareTo(result2) : string.Compare(x, y, this.m_stringComparison);
    }
  }
}
