// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Internal.ExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ExtensionMethods
  {
    public static string Replace(
      this string s,
      string oldValue,
      string newValue,
      StringComparison comparisonType)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      while (true)
      {
        int num = s.IndexOf(oldValue, startIndex, comparisonType);
        if (num >= startIndex)
        {
          int length = num - startIndex;
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(s.Length + (int) byte.MaxValue);
          if (length > 0)
            stringBuilder.Append(s.Substring(startIndex, length));
          stringBuilder.Append(newValue);
          startIndex = num + oldValue.Length;
        }
        else
          break;
      }
      if (stringBuilder == null)
        return s;
      if (startIndex < s.Length)
        stringBuilder.Append(s.Substring(startIndex));
      return stringBuilder.ToString();
    }
  }
}
