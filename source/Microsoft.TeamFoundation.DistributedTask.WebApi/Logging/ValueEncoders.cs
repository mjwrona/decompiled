// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Logging.ValueEncoders
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Logging
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ValueEncoders
  {
    public static string ExpressionStringEscape(string value) => ExpressionUtil.StringEscape(value);

    public static string JsonStringEscape(string value)
    {
      string str = JsonConvert.ToString(value);
      return str.Substring(1, str.Length - 2);
    }

    public static string BackslashEscape(string value) => value.Replace("\\\\", "\\").Replace("\\'", "'").Replace("\\\"", "\"").Replace("\\t", "\t");

    public static string UriDataEscape(string value) => ValueEncoders.UriDataEscape(value, 65519);

    internal static string UriDataEscape(string value, int maxSegmentSize)
    {
      if (value.Length <= maxSegmentSize)
        return Uri.EscapeDataString(value);
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      do
      {
        int length = Math.Min(value.Length - startIndex, maxSegmentSize);
        if (char.IsHighSurrogate(value[startIndex + length - 1]) && length > 1)
          --length;
        stringBuilder.Append(Uri.EscapeDataString(value.Substring(startIndex, length)));
        startIndex += length;
      }
      while (startIndex < value.Length);
      return stringBuilder.ToString();
    }
  }
}
