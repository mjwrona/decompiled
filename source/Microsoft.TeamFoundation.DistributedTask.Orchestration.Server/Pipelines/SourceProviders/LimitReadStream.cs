// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.LimitReadStream
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class LimitReadStream : Stream
  {
    private long m_bytesRead;
    private readonly Stream m_stream;
    private readonly long m_maxSize;

    public LimitReadStream(Stream stream, long maxSize)
    {
      this.m_stream = stream;
      this.m_maxSize = maxSize;
    }

    public override bool CanRead => true;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => false;

    public override void Flush() => this.m_stream.Flush();

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int bytesRead = this.m_stream.Read(buffer, offset, count);
      this.m_bytesRead += (long) this.CheckBytesRead(bytesRead);
      return bytesRead;
    }

    public override int ReadByte()
    {
      this.m_bytesRead += (long) this.CheckBytesRead(1);
      return base.ReadByte();
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      int bytesRead = await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      this.m_bytesRead += (long) this.CheckBytesRead(bytesRead);
      return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    private int CheckBytesRead(int bytesRead)
    {
      if (this.m_bytesRead + (long) bytesRead > this.m_maxSize)
        throw new DistributedTaskException(string.Format("The maximum file size of {0} bytes has been exceeded", (object) this.m_maxSize));
      return bytesRead;
    }
  }
}
