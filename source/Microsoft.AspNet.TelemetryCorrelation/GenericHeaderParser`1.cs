// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.GenericHeaderParser`1
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

using System;

namespace Microsoft.AspNet.TelemetryCorrelation
{
  internal sealed class GenericHeaderParser<T> : BaseHeaderParser<T>
  {
    private GenericHeaderParser<T>.GetParsedValueLengthDelegate getParsedValueLength;

    internal GenericHeaderParser(
      bool supportsMultipleValues,
      GenericHeaderParser<T>.GetParsedValueLengthDelegate getParsedValueLength)
      : base(supportsMultipleValues)
    {
      this.getParsedValueLength = getParsedValueLength != null ? getParsedValueLength : throw new ArgumentNullException(nameof (getParsedValueLength));
    }

    protected override int GetParsedValueLength(string value, int startIndex, out T parsedValue) => this.getParsedValueLength(value, startIndex, out parsedValue);

    internal delegate int GetParsedValueLengthDelegate(
      string value,
      int startIndex,
      out T parsedValue);
  }
}
