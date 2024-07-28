// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.NameValueHeaderValue
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal class NameValueHeaderValue
  {
    private static readonly HttpHeaderParser<NameValueHeaderValue> SingleValueParser = (HttpHeaderParser<NameValueHeaderValue>) new GenericHeaderParser<NameValueHeaderValue>(false, new GenericHeaderParser<NameValueHeaderValue>.GetParsedValueLengthDelegate(NameValueHeaderValue.GetNameValueLength));
    private string name;
    private string value;

    private NameValueHeaderValue()
    {
    }

    public string Name => this.name;

    public string Value => this.value;

    public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
    {
      int index = 0;
      return NameValueHeaderValue.SingleValueParser.TryParseValue(input, ref index, out parsedValue);
    }

    internal static int GetValueLength(string input, int startIndex)
    {
      if (startIndex >= input.Length)
        return 0;
      int length = HttpRuleParser.GetTokenLength(input, startIndex);
      return length == 0 && HttpRuleParser.GetQuotedStringLength(input, startIndex, out length) != HttpParseResult.Parsed ? 0 : length;
    }

    private static int GetNameValueLength(
      string input,
      int startIndex,
      out NameValueHeaderValue parsedValue)
    {
      parsedValue = (NameValueHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      string str = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int num = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (num == input.Length || input[num] != '=')
      {
        parsedValue = new NameValueHeaderValue();
        parsedValue.name = str;
        return num + HttpRuleParser.GetWhitespaceLength(input, num) - startIndex;
      }
      int startIndex2 = num + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex3);
      parsedValue = new NameValueHeaderValue();
      parsedValue.name = str;
      parsedValue.value = input.Substring(startIndex3, valueLength);
      int startIndex4 = startIndex3 + valueLength;
      return startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4) - startIndex;
    }
  }
}
