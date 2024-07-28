// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.KeyValueParser
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.AspNet.OData.Routing
{
  internal static class KeyValueParser
  {
    private static readonly Regex _stringLiteralRegex = new Regex("^'([^']|'')*'$", RegexOptions.Compiled);

    public static Dictionary<string, string> ParseKeys(string segment)
    {
      Dictionary<string, string> keys = new Dictionary<string, string>();
      int index = 0;
      int startIndex = 0;
      for (; index < segment.Length; ++index)
      {
        if (segment[index] == '=')
        {
          string key = segment.Substring(startIndex, index - startIndex);
          if (string.IsNullOrWhiteSpace(key))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NoKeyNameFoundInSegment, (object) startIndex, (object) segment));
          if (key.Contains("'"))
          {
            if (keys.Count != 0)
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NoKeyNameFoundInSegment, (object) startIndex, (object) segment));
            KeyValueParser.CheckSingleQuote(segment, segment);
            keys.Add(string.Empty, segment);
            return keys;
          }
          ++index;
          startIndex = index;
          for (; index <= segment.Length; ++index)
          {
            if (index == segment.Length || segment[index] == ',')
            {
              string str = segment.Substring(startIndex, index - startIndex);
              if (string.IsNullOrWhiteSpace(str))
                throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NoValueLiteralFoundInSegment, (object) key, (object) startIndex, (object) segment));
              if (keys.ContainsKey(key))
                throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.DuplicateKeyInSegment, (object) key, (object) segment));
              KeyValueParser.CheckSingleQuote(str, segment);
              keys.Add(key, str);
              startIndex = index + 1;
              break;
            }
            if (segment[index] == '\'')
            {
              for (++index; index <= segment.Length; ++index)
              {
                if (index == segment.Length)
                  throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.UnterminatedStringLiteral, (object) startIndex, (object) segment));
                if (segment[index] == '\'')
                {
                  if (index + 1 != segment.Length && segment[index + 1] == '\'')
                    ++index;
                  else
                    break;
                }
              }
            }
          }
        }
      }
      if (keys.Count == 0 && !string.IsNullOrWhiteSpace(segment))
      {
        KeyValueParser.CheckSingleQuote(segment, segment);
        keys.Add(string.Empty, segment);
      }
      return keys;
    }

    private static void CheckSingleQuote(string value, string segment)
    {
      if (value.StartsWith("'", StringComparison.Ordinal))
      {
        if (!KeyValueParser._stringLiteralRegex.IsMatch(value))
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.LiteralHasABadFormat, (object) value, (object) segment));
      }
      else
      {
        int num = value.Count<char>((Func<char, bool>) (c => c == '\''));
        switch (num)
        {
          case 0:
          case 2:
            if (num == 0 || value.EndsWith("'", StringComparison.Ordinal))
              break;
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.LiteralHasABadFormat, (object) value, (object) segment));
          default:
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidSingleQuoteCountForNonStringLiteral, (object) value, (object) segment));
        }
      }
    }
  }
}
