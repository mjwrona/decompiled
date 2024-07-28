// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.AssetStream
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using System.IO;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public class AssetStream : Stream
  {
    private HttpResponseMessage m_responseMessage;
    private Stream m_stream;

    public AssetStream(HttpResponseMessage responseMessage)
    {
      this.m_responseMessage = responseMessage;
      this.m_stream = this.m_responseMessage.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
    }

    public override bool CanRead => this.m_stream.CanRead;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => this.m_stream.CanWrite;

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set => this.m_stream.Position = value;
    }

    public override void Flush() => this.m_stream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => this.m_stream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

    public override void SetLength(long value) => this.m_stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.m_stream.Write(buffer, offset, count);

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.m_stream != null)
      {
        this.m_stream.Dispose();
        this.m_stream = (Stream) null;
      }
      if (this.m_responseMessage == null)
        return;
      this.m_responseMessage.Dispose();
      this.m_responseMessage = (HttpResponseMessage) null;
    }
  }
}
