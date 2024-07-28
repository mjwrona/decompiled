// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.HttpRuleParser
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal static class HttpRuleParser
  {
    internal const char CR = '\r';
    internal const char LF = '\n';
    internal const char SP = ' ';
    internal const char Tab = '\t';
    internal const int MaxInt64Digits = 19;
    internal const int MaxInt32Digits = 10;
    private const int MaxNestedCount = 5;
    private static readonly bool[] TokenChars = HttpRuleParser.CreateTokenChars();

    internal static bool IsTokenChar(char character) => character <= '\u007F' && HttpRuleParser.TokenChars[(int) character];

    internal static int GetTokenLength(string input, int startIndex)
    {
      if (startIndex >= input.Length)
        return 0;
      for (int index = startIndex; index < input.Length; ++index)
      {
        if (!HttpRuleParser.IsTokenChar(input[index]))
          return index - startIndex;
      }
      return input.Length - startIndex;
    }

    internal static int GetWhitespaceLength(string input, int startIndex)
    {
      if (startIndex >= input.Length)
        return 0;
      int index = startIndex;
      while (index < input.Length)
      {
        switch (input[index])
        {
          case '\t':
          case ' ':
            ++index;
            continue;
          case '\r':
            if (index + 2 < input.Length && input[index + 1] == '\n')
            {
              switch (input[index + 2])
              {
                case '\t':
                case ' ':
                  index += 3;
                  continue;
              }
            }
            else
              break;
            break;
        }
        return index - startIndex;
      }
      return input.Length - startIndex;
    }

    internal static HttpParseResult GetQuotedStringLength(
      string input,
      int startIndex,
      out int length)
    {
      int nestedCount = 0;
      return HttpRuleParser.GetExpressionLength(input, startIndex, '"', '"', false, ref nestedCount, out length);
    }

    internal static HttpParseResult GetQuotedPairLength(
      string input,
      int startIndex,
      out int length)
    {
      length = 0;
      if (input[startIndex] != '\\')
        return HttpParseResult.NotParsed;
      if (startIndex + 2 > input.Length || input[startIndex + 1] > '\u007F')
        return HttpParseResult.InvalidFormat;
      length = 2;
      return HttpParseResult.Parsed;
    }

    private static bool[] CreateTokenChars()
    {
      bool[] tokenChars = new bool[128];
      for (int index = 33; index < (int) sbyte.MaxValue; ++index)
        tokenChars[index] = true;
      tokenChars[40] = false;
      tokenChars[41] = false;
      tokenChars[60] = false;
      tokenChars[62] = false;
      tokenChars[64] = false;
      tokenChars[44] = false;
      tokenChars[59] = false;
      tokenChars[58] = false;
      tokenChars[92] = false;
      tokenChars[34] = false;
      tokenChars[47] = false;
      tokenChars[91] = false;
      tokenChars[93] = false;
      tokenChars[63] = false;
      tokenChars[61] = false;
      tokenChars[123] = false;
      tokenChars[125] = false;
      return tokenChars;
    }

    private static HttpParseResult GetExpressionLength(
      string input,
      int startIndex,
      char openChar,
      char closeChar,
      bool supportsNesting,
      ref int nestedCount,
      out int length)
    {
      length = 0;
      if ((int) input[startIndex] != (int) openChar)
        return HttpParseResult.NotParsed;
      int num = startIndex + 1;
      while (num < input.Length)
      {
        int length1 = 0;
        if (num + 2 < input.Length && HttpRuleParser.GetQuotedPairLength(input, num, out length1) == HttpParseResult.Parsed)
        {
          num += length1;
        }
        else
        {
          if (supportsNesting && (int) input[num] == (int) openChar)
          {
            ++nestedCount;
            try
            {
              if (nestedCount > 5)
                return HttpParseResult.InvalidFormat;
              int length2 = 0;
              switch (HttpRuleParser.GetExpressionLength(input, num, openChar, closeChar, supportsNesting, ref nestedCount, out length2))
              {
                case HttpParseResult.Parsed:
                  num += length2;
                  break;
                case HttpParseResult.InvalidFormat:
                  return HttpParseResult.InvalidFormat;
              }
            }
            finally
            {
              --nestedCount;
            }
          }
          if ((int) input[num] == (int) closeChar)
          {
            length = num - startIndex + 1;
            return HttpParseResult.Parsed;
          }
          ++num;
        }
      }
      return HttpParseResult.InvalidFormat;
    }
  }
}
