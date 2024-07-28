// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.BaseHeaderParser`1
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal abstract class BaseHeaderParser<T> : HttpHeaderParser<T>
  {
    protected BaseHeaderParser(bool supportsMultipleValues)
      : base(supportsMultipleValues)
    {
    }

    public override sealed bool TryParseValue(string value, ref int index, out T parsedValue)
    {
      parsedValue = default (T);
      if (string.IsNullOrEmpty(value) || index == value.Length)
        return this.SupportsMultipleValues;
      bool separatorFound = false;
      int orWhitespaceIndex1 = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, index, this.SupportsMultipleValues, out separatorFound);
      if (separatorFound && !this.SupportsMultipleValues)
        return false;
      if (orWhitespaceIndex1 == value.Length)
      {
        if (this.SupportsMultipleValues)
          index = orWhitespaceIndex1;
        return this.SupportsMultipleValues;
      }
      T parsedValue1;
      int parsedValueLength = this.GetParsedValueLength(value, orWhitespaceIndex1, out parsedValue1);
      if (parsedValueLength == 0)
        return false;
      int startIndex = orWhitespaceIndex1 + parsedValueLength;
      int orWhitespaceIndex2 = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, startIndex, this.SupportsMultipleValues, out separatorFound);
      if (separatorFound && !this.SupportsMultipleValues || !separatorFound && orWhitespaceIndex2 < value.Length)
        return false;
      index = orWhitespaceIndex2;
      parsedValue = parsedValue1;
      return true;
    }

    protected abstract int GetParsedValueLength(string value, int startIndex, out T parsedValue);
  }
}
