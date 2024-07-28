// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataNotificationReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataNotificationReader : TextReader
  {
    private readonly TextReader textReader;
    private IODataStreamListener listener;

    internal ODataNotificationReader(TextReader textReader, IODataStreamListener listener)
    {
      this.textReader = textReader;
      this.listener = listener;
    }

    public override int GetHashCode() => this.textReader.GetHashCode();

    public override string ToString() => this.textReader.ToString();

    public override int Peek() => this.textReader.Peek();

    public override int Read() => this.textReader.Read();

    public override int Read(char[] buffer, int index, int count) => this.textReader.Read(buffer, index, count);

    public override int ReadBlock(char[] buffer, int index, int count) => this.textReader.ReadBlock(buffer, index, count);

    public override string ReadLine() => this.textReader.ReadLine();

    public override string ReadToEnd() => this.textReader.ReadToEnd();

    public override Task<int> ReadAsync(char[] buffer, int index, int count) => this.textReader.ReadAsync(buffer, index, count);

    public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) => this.ReadBlockAsync(buffer, index, count);

    public override Task<string> ReadLineAsync() => this.textReader.ReadLineAsync();

    public override Task<string> ReadToEndAsync() => this.textReader.ReadToEndAsync();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.listener != null)
      {
        this.listener.StreamDisposed();
        this.listener = (IODataStreamListener) null;
      }
      this.textReader.Dispose();
      base.Dispose(disposing);
    }
  }
}
