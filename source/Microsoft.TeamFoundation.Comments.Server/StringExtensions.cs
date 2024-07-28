// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.StringExtensions
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public static class StringExtensions
  {
    public static string ReplaceStringWithChar(
      this string input,
      string pattern,
      char replaceValue,
      StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
      ArgumentUtility.CheckForNull<string>(input, nameof (input));
      ArgumentUtility.CheckStringForNullOrEmpty(pattern, nameof (pattern));
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      while (true)
      {
        int num = input.IndexOf(pattern, startIndex, comparison);
        if (num >= startIndex)
        {
          int length = num - startIndex;
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(input.Length + (int) byte.MaxValue);
          if (length > 0)
            stringBuilder.Append(input.Substring(startIndex, length));
          stringBuilder.Append(replaceValue);
          startIndex = num + pattern.Length;
        }
        else
          break;
      }
      if (stringBuilder == null)
        return input;
      if (startIndex < input.Length)
        stringBuilder.Append(input.Substring(startIndex));
      return stringBuilder.ToString();
    }

    public static string ReplaceCharWithString(this string input, char signalChar, string newValue)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      while (true)
      {
        int num = input.IndexOf(signalChar, startIndex);
        if (num >= startIndex)
        {
          int length = num - startIndex;
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(input.Length + (int) byte.MaxValue);
          if (length > 0)
            stringBuilder.Append(input.Substring(startIndex, length));
          stringBuilder.Append(newValue);
          startIndex = num + 1;
        }
        else
          break;
      }
      if (stringBuilder == null)
        return input;
      if (startIndex < input.Length)
        stringBuilder.Append(input.Substring(startIndex));
      return stringBuilder.ToString();
    }
  }
}
