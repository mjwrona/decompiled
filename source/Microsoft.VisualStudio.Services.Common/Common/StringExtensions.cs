// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.StringExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  internal static class StringExtensions
  {
    public static string Replace(
      this string input,
      string oldValue,
      string newValue,
      StringComparison comparison)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(oldValue, nameof (oldValue));
      ArgumentUtility.CheckForNull<string>(newValue, nameof (newValue));
      if (string.IsNullOrEmpty(input))
        return input;
      int startIndex = 0;
      StringBuilder stringBuilder = (StringBuilder) null;
      int length;
      for (; (length = input.IndexOf(oldValue, startIndex, comparison)) >= 0; startIndex = length + oldValue.Length)
      {
        if (stringBuilder == null)
        {
          if (length == 0 && input.Length == oldValue.Length)
          {
            input = newValue;
            break;
          }
          stringBuilder = new StringBuilder(input.Substring(0, length));
        }
        else if (length > startIndex)
          stringBuilder.Append(input.Substring(startIndex, length - startIndex));
        stringBuilder.Append(newValue);
      }
      if (stringBuilder == null)
        return input;
      if (startIndex < input.Length)
        stringBuilder.Append(input.Substring(startIndex));
      return stringBuilder.ToString();
    }

    public static string UnescapeXml(this string source)
    {
      if (string.IsNullOrEmpty(source) || !source.Contains<char>('&'))
        return source;
      StringBuilder stringBuilder = new StringBuilder(source);
      stringBuilder.Replace("&lt;", "<");
      stringBuilder.Replace("&gt;", ">");
      stringBuilder.Replace("&amp;", "&");
      stringBuilder.Replace("&apos;", "'");
      stringBuilder.Replace("&quot;", "\"");
      return stringBuilder.ToString();
    }

    public static IEnumerable<string> SplitByNumberOfChars(this string input, int chunkSize)
    {
      if (input != null)
      {
        ArgumentUtility.CheckGreaterThanZero((float) chunkSize, nameof (chunkSize));
        int start = 0;
        while (start <= input.Length - 1)
        {
          int num1 = start + chunkSize;
          int num2 = num1 > input.Length - 1 ? input.Length : num1;
          string str = input.Substring(start, num2 - start);
          start = num2;
          yield return str;
        }
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsSubdomainOf(this string domain, string parentDomain) => UriUtility.IsSubdomainOf(domain, parentDomain);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Uri AsUri(this string uri) => new Uri(uri);
  }
}
