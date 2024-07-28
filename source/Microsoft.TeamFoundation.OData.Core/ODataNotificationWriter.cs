// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataNotificationWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataNotificationWriter : TextWriter
  {
    private readonly TextWriter textWriter;
    private IODataStreamListener listener;

    internal ODataNotificationWriter(TextWriter textWriter, IODataStreamListener listener)
      : base((IFormatProvider) CultureInfo.InvariantCulture)
    {
      this.textWriter = textWriter;
      this.listener = listener;
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

    public override void Write(char value) => this.textWriter.Write(value);

    public override void Write(bool value) => this.textWriter.Write(value);

    public override void Write(string value) => this.textWriter.Write(value);

    public override void Write(char[] buffer) => this.textWriter.Write(buffer);

    public override void Write(char[] buffer, int index, int count) => this.textWriter.Write(buffer, index, count);

    public override void Write(string format, params object[] arg) => this.textWriter.Write(format, arg);

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

    public override void WriteLine(char value) => this.textWriter.WriteLine(value);

    public override void WriteLine(char[] buffer) => this.textWriter.WriteLine(buffer);

    public override void WriteLine(char[] buffer, int index, int count) => this.textWriter.WriteLine(buffer, index, count);

    public override void WriteLine(Decimal value) => this.textWriter.WriteLine(value);

    public override void WriteLine(double value) => this.textWriter.WriteLine(value);

    public override void WriteLine(float value) => this.textWriter.WriteLine(value);

    public override void WriteLine(int value) => this.textWriter.WriteLine(value);

    public override void WriteLine(long value) => this.textWriter.WriteLine(value);

    public override void WriteLine(object value) => this.textWriter.WriteLine(value);

    public override void WriteLine(string format, params object[] arg) => this.textWriter.WriteLine(format, arg);

    public override void WriteLine(string value) => this.textWriter.WriteLine(value);

    public override void WriteLine(uint value) => this.textWriter.WriteLine(value);

    public override void WriteLine(ulong value) => this.textWriter.WriteLine(value);

    public override Task FlushAsync() => this.textWriter.FlushAsync();

    public override Task WriteAsync(char value) => this.textWriter.WriteAsync(value);

    public override Task WriteAsync(char[] buffer, int index, int count) => this.textWriter.WriteAsync(buffer, index, count);

    public override Task WriteAsync(string value) => this.textWriter.WriteAsync(value);

    public override Task WriteLineAsync() => this.textWriter.WriteLineAsync();

    public override Task WriteLineAsync(char value) => this.textWriter.WriteLineAsync(value);

    public override Task WriteLineAsync(char[] buffer, int index, int count) => this.textWriter.WriteLineAsync(buffer, index, count);

    public override Task WriteLineAsync(string value) => this.textWriter.WriteLineAsync(value);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.listener != null)
      {
        this.listener.StreamDisposed();
        this.listener = (IODataStreamListener) null;
      }
      this.textWriter.Dispose();
      base.Dispose(disposing);
    }
  }
}
