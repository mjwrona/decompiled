// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataJsonTextWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataJsonTextWriter : TextWriter
  {
    private readonly TextWriter textWriter;
    private readonly ODataStringEscapeOption escapeOption = ODataStringEscapeOption.EscapeOnlyControls;
    private char[] streamingBuffer;
    private ICharArrayPool bufferPool;

    internal ODataJsonTextWriter(
      TextWriter textWriter,
      ref char[] buffer,
      ICharArrayPool bufferPool)
      : base((IFormatProvider) CultureInfo.InvariantCulture)
    {
      ExceptionUtils.CheckArgumentNotNull<TextWriter>(textWriter, nameof (textWriter));
      this.textWriter = textWriter;
      this.streamingBuffer = buffer;
      this.bufferPool = bufferPool;
    }

    public override Encoding Encoding => this.textWriter.Encoding;

    public override string NewLine
    {
      get => this.textWriter.NewLine;
      set => this.textWriter.NewLine = value;
    }

    public override IFormatProvider FormatProvider => this.textWriter.FormatProvider;

    public override void Flush() => this.textWriter.Flush();

    public override int GetHashCode() => this.textWriter.GetHashCode();

    public override string ToString() => this.textWriter.ToString();

    public override void Write(char value) => this.WriteEscapedCharValue(value);

    public override void Write(bool value) => this.textWriter.Write(value);

    public override void Write(string value) => this.WriteEscapedStringValue(value);

    public override void Write(char[] buffer) => this.WriteEscapedCharArray(buffer, 0, buffer.Length);

    public override void Write(char[] buffer, int index, int count) => this.WriteEscapedCharArray(buffer, index, count);

    public override void Write(string format, params object[] arg) => this.WriteEscapedStringValue(string.Format(this.FormatProvider, format, arg));

    public override void Write(Decimal value) => this.textWriter.Write(value);

    public override void Write(object value) => this.textWriter.Write(value);

    public override void Write(double value) => this.textWriter.Write(value);

    public override void Write(float value) => this.textWriter.Write(value);

    public override void Write(int value) => this.textWriter.Write(value);

    public override void Write(long value) => this.textWriter.Write(value);

    public override void Write(uint value) => this.textWriter.Write(value);

    public override void Write(ulong value) => this.textWriter.Write(value);

    public override void WriteLine() => this.textWriter.WriteLine();

    public override void WriteLine(bool value) => this.textWriter.WriteLine(value);

    public override void WriteLine(char value)
    {
      this.Write(value);
      this.textWriter.WriteLine();
    }

    public override void WriteLine(char[] buffer)
    {
      this.Write(buffer);
      this.textWriter.WriteLine();
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
      this.Write(buffer, index, count);
      this.textWriter.WriteLine();
    }

    public override void WriteLine(Decimal value) => this.textWriter.WriteLine(value);

    public override void WriteLine(double value) => this.textWriter.WriteLine(value);

    public override void WriteLine(float value) => this.textWriter.WriteLine(value);

    public override void WriteLine(int value) => this.textWriter.WriteLine(value);

    public override void WriteLine(long value) => this.textWriter.WriteLine(value);

    public override void WriteLine(object value) => this.textWriter.WriteLine(value);

    public override void WriteLine(string format, params object[] arg)
    {
      this.Write(format, arg);
      this.textWriter.WriteLine();
    }

    public override void WriteLine(string value)
    {
      this.Write(value);
      this.textWriter.WriteLine();
    }

    public override void WriteLine(uint value) => this.textWriter.WriteLine(value);

    public override void WriteLine(ulong value) => this.textWriter.WriteLine(value);

    public override Task FlushAsync() => this.textWriter.FlushAsync();

    public override Task WriteAsync(char value) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.Write(value)));

    public override Task WriteAsync(char[] buffer, int index, int count) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.Write(buffer, index, count)));

    public override Task WriteAsync(string value) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.Write(value)));

    public override Task WriteLineAsync() => this.textWriter.WriteLineAsync();

    public override Task WriteLineAsync(char value) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteLine(value)));

    public override Task WriteLineAsync(char[] buffer, int index, int count) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteLine(buffer, index, count)));

    public override Task WriteLineAsync(string value) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteLine(value)));

    protected override void Dispose(bool disposing)
    {
      this.textWriter.Dispose();
      base.Dispose(disposing);
    }

    private void WriteEscapedCharValue(char value) => JsonValueUtils.WriteValue(this.textWriter, value, this.escapeOption);

    private void WriteEscapedStringValue(string value) => JsonValueUtils.WriteEscapedJsonStringValue(this.textWriter, value, this.escapeOption, ref this.streamingBuffer, this.bufferPool);

    private void WriteEscapedCharArray(char[] value, int offset, int count) => JsonValueUtils.WriteEscapedCharArray(this.textWriter, value, offset, count, this.escapeOption, ref this.streamingBuffer, this.bufferPool);
  }
}
