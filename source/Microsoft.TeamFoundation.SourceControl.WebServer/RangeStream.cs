// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.RangeStream
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal class RangeStream : Stream
  {
    private const long BufferSize = 4096;
    private Lazy<Stream> m_streamFactory;
    private readonly long m_startPosition;
    private readonly int m_maxLength;
    private int m_readSoFar;
    private bool m_truncated;
    private bool m_seekRequired;
    private bool m_disposed;

    public RangeStream(Lazy<Stream> streamFactory, long startPosition, int maxLength)
    {
      ArgumentUtility.CheckForNull<Lazy<Stream>>(streamFactory, nameof (streamFactory));
      ArgumentUtility.CheckForOutOfRange(startPosition, nameof (startPosition), 0L);
      ArgumentUtility.CheckForOutOfRange(maxLength, nameof (maxLength), 0);
      this.m_streamFactory = streamFactory;
      this.m_startPosition = startPosition;
      this.m_maxLength = maxLength;
      this.m_seekRequired = this.m_startPosition > 0L;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      Stream stream = this.m_streamFactory.Value;
      if (this.m_seekRequired)
      {
        if (stream.CanSeek)
        {
          stream.Seek(this.m_startPosition, SeekOrigin.Begin);
        }
        else
        {
          int num1 = 0;
          int num2 = 1;
          byte[] buffer1 = new byte[new IntPtr(4096)];
          for (; (long) num1 < this.m_startPosition && num2 > 0; num1 += num2)
            num2 = stream.Read(buffer1, 0, (int) Math.Min(4096L, this.m_startPosition - (long) num1));
        }
        this.m_seekRequired = false;
      }
      int num = 0;
      if (this.m_readSoFar < this.m_maxLength)
      {
        num = stream.Read(buffer, offset, Math.Min(count, this.m_maxLength - this.m_readSoFar));
        this.m_readSoFar += num;
        if (this.m_readSoFar == this.m_maxLength)
          this.m_truncated = stream.ReadByte() > -1;
      }
      return num;
    }

    public bool Truncated => this.m_truncated;

    public int BytesRead => this.m_readSoFar;

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.m_disposed)
      {
        if (this.m_streamFactory.IsValueCreated)
          this.m_streamFactory.Value.Dispose();
        this.m_disposed = true;
      }
      base.Dispose(disposing);
    }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => this.m_startPosition + (long) this.m_readSoFar;
      set => throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Flush() => throw new NotSupportedException();
  }
}
