// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.RawValueWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using Microsoft.Spatial;
using System;
using System.IO;
using System.Text;

namespace Microsoft.OData
{
  internal sealed class RawValueWriter : IDisposable
  {
    private readonly ODataMessageWriterSettings settings;
    private readonly Stream stream;
    private readonly Encoding encoding;
    private TextWriter textWriter;
    private JsonWriter jsonWriter;

    internal RawValueWriter(ODataMessageWriterSettings settings, Stream stream, Encoding encoding)
    {
      this.settings = settings;
      this.stream = stream;
      this.encoding = encoding;
      this.InitializeTextWriter();
    }

    internal TextWriter TextWriter => this.textWriter;

    internal JsonWriter JsonWriter => this.jsonWriter;

    public void Dispose()
    {
      this.textWriter.Dispose();
      this.textWriter = (TextWriter) null;
    }

    internal void Start()
    {
      if (!this.settings.HasJsonPaddingFunction())
        return;
      this.textWriter.Write(this.settings.JsonPCallback);
      this.textWriter.Write("(");
    }

    internal void End()
    {
      if (!this.settings.HasJsonPaddingFunction())
        return;
      this.textWriter.Write(")");
    }

    internal void WriteRawValue(object value)
    {
      switch (value)
      {
        case ODataEnumValue odataEnumValue:
          this.textWriter.Write(odataEnumValue.Value);
          break;
        case Geometry _:
        case Geography _:
          PrimitiveConverter.Instance.WriteJsonLight(value, (IJsonWriter) this.jsonWriter);
          break;
        default:
          string result;
          if (!ODataRawValueUtils.TryConvertPrimitiveToString(value, out result))
            throw new ODataException(Strings.ODataUtils_CannotConvertValueToRawString((object) value.GetType().FullName));
          this.textWriter.Write(result);
          break;
      }
    }

    internal void Flush()
    {
      if (this.TextWriter == null)
        return;
      this.TextWriter.Flush();
    }

    private void InitializeTextWriter()
    {
      this.textWriter = (TextWriter) new StreamWriter(MessageStreamWrapper.IsNonDisposingStream(this.stream) || this.stream is AsyncBufferedStream ? this.stream : MessageStreamWrapper.CreateNonDisposingStream(this.stream), this.encoding);
      this.jsonWriter = new JsonWriter(this.textWriter, false);
    }
  }
}
