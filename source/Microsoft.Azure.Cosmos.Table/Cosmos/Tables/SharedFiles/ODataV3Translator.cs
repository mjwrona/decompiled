// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.ODataV3Translator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class ODataV3Translator
  {
    private const string GuidPrefix = "guid'";
    private const string DateTimeOffSetPrefix = "datetime'";
    private const string BinaryPrefix = "X'";
    private static readonly char[] filterSplit = new char[1]
    {
      ' '
    };
    private static readonly char[] terminatingChar = new char[1]
    {
      '\''
    };
    private const string terminatingCharString = "'";

    public static string TranslateFilter(string odataFilter, bool useUtcTicks = true)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filterToken1 in odataFilter.Split(ODataV3Translator.filterSplit, StringSplitOptions.None))
      {
        string restOfToken = (string) null;
        if (filterToken1.StartsWith("guid'", StringComparison.Ordinal))
          filterToken1 = ODataV3Translator.GetGuid(ODataV3Translator.PrepareFilterContent(filterToken1, out restOfToken));
        else if (filterToken1.StartsWith("X'", StringComparison.Ordinal))
          filterToken1 = ODataV3Translator.GetBinaryData(ODataV3Translator.PrepareFilterContent(filterToken1, out restOfToken));
        else if (filterToken1.StartsWith("datetime'", StringComparison.Ordinal))
        {
          string filterToken2 = ODataV3Translator.PrepareFilterContent(filterToken1, out restOfToken);
          filterToken1 = !useUtcTicks ? ODataV3Translator.removeDateTimePrefix(filterToken2) : ODataV3Translator.GetDateTimeOffset(filterToken2);
        }
        stringBuilder.Append(filterToken1);
        if (!string.IsNullOrEmpty(restOfToken))
          stringBuilder.Append(restOfToken);
        stringBuilder.Append(" ");
      }
      return stringBuilder.ToString();
    }

    private static string PrepareFilterContent(string filterToken, out string restOfToken)
    {
      restOfToken = (string) null;
      string str = filterToken;
      if (!str.EndsWith("'", StringComparison.Ordinal))
      {
        int num = str.LastIndexOf("'", StringComparison.Ordinal) + 1;
        str = str.IndexOf("'", StringComparison.Ordinal) + 1 != num ? filterToken.Substring(0, num) : throw new InvalidFilterException("Filter token had one single quote instead of two", filterToken);
        restOfToken = filterToken.Substring(num);
      }
      return str;
    }

    private static string GetGuid(string filterToken) => filterToken.Substring("guid'".Length, filterToken.Length - "guid'".Length - 1);

    private static string GetBinaryData(string filterToken)
    {
      string hex = filterToken.Substring("X'".Length, filterToken.Length - "X'".Length - 1);
      byte[] bytes;
      try
      {
        bytes = HexUtilities.HexToBytes(hex);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidFilterException("The binary filter token is invalid.", filterToken);
      }
      return string.Format("binary'{0}'", (object) Convert.ToBase64String(bytes));
    }

    private static string GetDateTimeOffset(string filterToken) => filterToken.Substring("datetime'".Length, filterToken.Length - "datetime'".Length - 1);

    private static string removeDateTimePrefix(string filterToken)
    {
      int length = filterToken.Length - "datetime'".Length - 1;
      return filterToken.Substring("datetime'".Length, length);
    }
  }
}
