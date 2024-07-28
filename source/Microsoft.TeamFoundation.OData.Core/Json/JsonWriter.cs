// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.OData.Json
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This class does not own the underlying stream/writer and thus should never dispose it.")]
  internal sealed class JsonWriter : IJsonStreamWriter, IJsonWriter, IDisposable
  {
    private readonly TextWriterWrapper writer;
    private readonly Stack<JsonWriter.Scope> scopes;
    private readonly bool isIeee754Compatible;
    private readonly ODataStringEscapeOption stringEscapeOption;
    private char[] buffer;
    private Stream binaryValueStream;
    private string currentContentType;

    internal JsonWriter(TextWriter writer, bool isIeee754Compatible)
      : this(writer, isIeee754Compatible, ODataStringEscapeOption.EscapeNonAscii)
    {
    }

    internal JsonWriter(
      TextWriter writer,
      bool isIeee754Compatible,
      ODataStringEscapeOption stringEscapeOption)
    {
      this.writer = (TextWriterWrapper) new NonIndentedTextWriter(writer);
      this.scopes = new Stack<JsonWriter.Scope>();
      this.isIeee754Compatible = isIeee754Compatible;
      this.stringEscapeOption = stringEscapeOption;
    }

    public ICharArrayPool ArrayPool { get; set; }

    private bool IsWritingJson => string.IsNullOrEmpty(this.currentContentType) || this.currentContentType.StartsWith("application/json", StringComparison.Ordinal);

    public void StartPaddingFunctionScope() => this.StartScope(JsonWriter.ScopeType.Padding);

    public void EndPaddingFunctionScope()
    {
      this.writer.WriteLine();
      this.writer.DecreaseIndentation();
      this.writer.Write(this.scopes.Pop().EndString);
    }

    public void StartObjectScope() => this.StartScope(JsonWriter.ScopeType.Object);

    public void EndObjectScope()
    {
      this.writer.WriteLine();
      this.writer.DecreaseIndentation();
      this.writer.Write(this.scopes.Pop().EndString);
    }

    public void StartArrayScope() => this.StartScope(JsonWriter.ScopeType.Array);

    public void EndArrayScope()
    {
      this.writer.WriteLine();
      this.writer.DecreaseIndentation();
      this.writer.Write(this.scopes.Pop().EndString);
    }

    public void WriteName(string name)
    {
      JsonWriter.Scope scope = this.scopes.Peek();
      if (scope.ObjectCount != 0)
        this.writer.Write(",");
      ++scope.ObjectCount;
      JsonValueUtils.WriteEscapedJsonString((TextWriter) this.writer, name, this.stringEscapeOption, ref this.buffer);
      this.writer.Write(":");
    }

    public void WritePaddingFunctionName(string functionName) => this.writer.Write(functionName);

    public void WriteValue(bool value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(int value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(float value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(short value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(long value)
    {
      this.WriteValueSeparator();
      if (this.isIeee754Compatible)
        JsonValueUtils.WriteValue((TextWriter) this.writer, value.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.stringEscapeOption, ref this.buffer);
      else
        JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(double value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(Guid value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(Decimal value)
    {
      this.WriteValueSeparator();
      if (this.isIeee754Compatible)
        JsonValueUtils.WriteValue((TextWriter) this.writer, value.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.stringEscapeOption, ref this.buffer);
      else
        JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(DateTimeOffset value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value, ODataJsonDateTimeFormat.ISO8601DateTime);
    }

    public void WriteValue(TimeSpan value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(TimeOfDay value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(Date value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(byte value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(sbyte value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value);
    }

    public void WriteValue(string value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value, this.stringEscapeOption, ref this.buffer);
    }

    public void WriteValue(byte[] value)
    {
      this.WriteValueSeparator();
      JsonValueUtils.WriteValue((TextWriter) this.writer, value, ref this.buffer, this.ArrayPool);
    }

    public void WriteRawValue(string rawValue)
    {
      this.WriteValueSeparator();
      this.writer.Write(rawValue);
    }

    public void Flush() => this.writer.Flush();

    public Stream StartStreamValueScope()
    {
      this.WriteValueSeparator();
      this.writer.Write('"');
      this.writer.Flush();
      this.binaryValueStream = (Stream) new ODataBinaryStreamWriter((TextWriter) this.writer, ref this.buffer, this.ArrayPool);
      return this.binaryValueStream;
    }

    public void EndStreamValueScope()
    {
      this.binaryValueStream.Flush();
      this.binaryValueStream.Dispose();
      this.binaryValueStream = (Stream) null;
      this.writer.Flush();
      this.writer.Write('"');
    }

    public TextWriter StartTextWriterValueScope(string contentType)
    {
      this.WriteValueSeparator();
      this.currentContentType = contentType;
      if (!this.IsWritingJson)
      {
        this.writer.Write('"');
        this.writer.Flush();
        return (TextWriter) new ODataJsonTextWriter((TextWriter) this.writer, ref this.buffer, this.ArrayPool);
      }
      this.writer.Flush();
      return (TextWriter) this.writer;
    }

    public void EndTextWriterValueScope()
    {
      if (this.IsWritingJson)
        return;
      this.writer.Write('"');
    }

    void IDisposable.Dispose()
    {
      if (this.binaryValueStream == null)
        return;
      try
      {
        this.binaryValueStream.Dispose();
      }
      finally
      {
        this.binaryValueStream = (Stream) null;
      }
    }

    public void Dispose()
    {
      if (this.ArrayPool == null || this.buffer == null)
        return;
      BufferUtils.ReturnToBuffer(this.ArrayPool, this.buffer);
      this.ArrayPool = (ICharArrayPool) null;
      this.buffer = (char[]) null;
    }

    private void WriteValueSeparator()
    {
      if (this.scopes.Count == 0)
        return;
      JsonWriter.Scope scope = this.scopes.Peek();
      if (scope.Type != JsonWriter.ScopeType.Array)
        return;
      if (scope.ObjectCount != 0)
        this.writer.Write(",");
      ++scope.ObjectCount;
    }

    private void StartScope(JsonWriter.ScopeType type)
    {
      if (this.scopes.Count != 0 && this.scopes.Peek().Type != JsonWriter.ScopeType.Padding)
      {
        JsonWriter.Scope scope = this.scopes.Peek();
        if (scope.Type == JsonWriter.ScopeType.Array && scope.ObjectCount != 0)
          this.writer.Write(",");
        ++scope.ObjectCount;
      }
      JsonWriter.Scope scope1 = new JsonWriter.Scope(type);
      this.scopes.Push(scope1);
      this.writer.Write(scope1.StartString);
      this.writer.IncreaseIndentation();
      this.writer.WriteLine();
    }

    internal enum ScopeType
    {
      Array,
      Object,
      Padding,
    }

    private sealed class Scope
    {
      private readonly JsonWriter.ScopeType type;

      public Scope(JsonWriter.ScopeType type)
      {
        this.type = type;
        switch (type)
        {
          case JsonWriter.ScopeType.Array:
            this.StartString = "[";
            this.EndString = "]";
            break;
          case JsonWriter.ScopeType.Object:
            this.StartString = "{";
            this.EndString = "}";
            break;
          case JsonWriter.ScopeType.Padding:
            this.StartString = "(";
            this.EndString = ")";
            break;
        }
      }

      public string StartString { get; private set; }

      public string EndString { get; private set; }

      public int ObjectCount { get; set; }

      public JsonWriter.ScopeType Type => this.type;
    }
  }
}
