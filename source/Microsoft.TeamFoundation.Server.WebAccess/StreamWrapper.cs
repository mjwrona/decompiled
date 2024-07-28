// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.StreamWrapper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class StreamWrapper : Stream
  {
    private Stream m_stream;
    private bool m_ownStream;

    public StreamWrapper(Stream stream)
      : this(stream, true)
    {
    }

    public StreamWrapper(Stream stream, bool ownStream)
    {
      this.m_stream = stream != null ? stream : throw new ArgumentNullException(nameof (stream));
      this.m_ownStream = ownStream;
    }

    public override bool CanRead => this.m_stream.CanRead;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => this.m_stream.CanWrite;

    public override void Flush() => this.m_stream.Flush();

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set => this.m_stream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count) => this.m_stream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

    public override void SetLength(long value) => this.m_stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.m_stream.Write(buffer, offset, count);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.m_ownStream && this.m_stream != null)
        this.m_stream.Close();
      base.Dispose(disposing);
    }
  }
}
