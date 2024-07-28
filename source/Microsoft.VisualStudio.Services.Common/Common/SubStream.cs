// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SubStream
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SubStream : Stream
  {
    private readonly long m_length;
    private readonly long m_startingPosition;
    private readonly Stream m_stream;

    public SubStream(Stream stream, int maxStreamSize)
    {
      this.m_startingPosition = stream.Position;
      long val2 = stream.Length - this.m_startingPosition;
      this.m_length = Math.Min((long) maxStreamSize, val2);
      this.m_stream = stream;
    }

    public override bool CanRead => this.m_stream.CanRead && this.m_stream.Position <= this.EndingPostionOnOuterStream;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => false;

    public override void Flush() => throw new NotImplementedException();

    public override long Length => this.m_length;

    public override long Position
    {
      get => this.m_stream.Position - this.m_startingPosition;
      set
      {
        if (value >= this.m_length)
          throw new EndOfStreamException();
        this.m_stream.Position = this.m_startingPosition + value;
      }
    }

    public long StartingPostionOnOuterStream => this.m_startingPosition;

    public long EndingPostionOnOuterStream => this.m_startingPosition + this.m_length - 1L;

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      count = this.EnsureLessThanOrEqualToRemainingBytes(count);
      return this.m_stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      count = this.EnsureLessThanOrEqualToRemainingBytes(count);
      return this.m_stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (origin == SeekOrigin.Begin && 0L <= offset && offset < this.m_length)
        return this.m_stream.Seek(offset + this.m_startingPosition, origin);
      if (origin == SeekOrigin.End && 0L >= offset && offset > -this.m_length)
        return this.m_stream.Seek(offset - (this.m_stream.Length - 1L - this.EndingPostionOnOuterStream), origin);
      if (origin == SeekOrigin.Current && offset + this.m_stream.Position >= this.StartingPostionOnOuterStream && offset + this.m_stream.Position < this.EndingPostionOnOuterStream)
        return this.m_stream.Seek(offset, origin);
      throw new ArgumentException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    private int EnsureLessThanOrEqualToRemainingBytes(int numBytes)
    {
      long num = this.m_length - this.Position;
      if ((long) numBytes > num)
        numBytes = Convert.ToInt32(num);
      return numBytes;
    }
  }
}
