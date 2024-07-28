// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BufferStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class BufferStream : Stream
  {
    private readonly Stream m_stream;
    private readonly MemoryMappedFile m_memoryMappedFile;
    private readonly long m_capacity;
    private bool m_closed;

    public BufferStream(IVssRequestContext requestContext, Guid repositoryId, long capacity)
      : this(GitServerUtils.GetCacheDirectory(requestContext, repositoryId), capacity)
    {
    }

    public BufferStream(
      string tempDir,
      long capacity,
      int maxMemoryStreamBytes = 65536,
      int maxMemoryMappedFileBytes = 67108864)
    {
      ArgumentUtility.CheckForOutOfRange(capacity, nameof (capacity), 0L);
      if (capacity < (long) maxMemoryStreamBytes)
        this.m_stream = (Stream) new MemoryStream((int) capacity);
      else if (capacity < (long) maxMemoryMappedFileBytes)
      {
        this.m_memoryMappedFile = MemoryMappedFile.CreateNew((string) null, capacity);
        this.m_stream = (Stream) this.m_memoryMappedFile.CreateViewStream(0L, capacity);
      }
      else
        this.m_stream = (Stream) new FileStream(Path.Combine(tempDir, Guid.NewGuid().ToString("N")), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Delete, 4096, FileOptions.DeleteOnClose);
      this.m_capacity = capacity;
    }

    public BufferStream(
      IVssRequestContext requestContext,
      Guid repositoryId,
      Stream source,
      long sourceLength)
      : this(requestContext, repositoryId, sourceLength)
    {
      if (this.m_stream is MemoryStream stream && sourceLength <= (long) stream.Capacity)
      {
        stream.SetLength(sourceLength);
        byte[] buffer = stream.GetBuffer();
        GitStreamUtil.ReadGreedy(source, buffer, 0, (int) sourceLength);
      }
      else
        source.CopyTo(this.m_stream);
      this.Seek(0L, SeekOrigin.Begin);
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.m_stream.Dispose();
        if (this.m_memoryMappedFile != null)
          this.m_memoryMappedFile.Dispose();
      }
      base.Close();
    }

    public override bool CanRead => this.m_stream.CanRead;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => this.m_stream.CanWrite;

    public override void Flush() => this.m_stream.Flush();

    public override long Length => this.m_capacity;

    public override long Position
    {
      get => this.m_stream.Position;
      set => this.m_stream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count) => this.m_stream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

    public override void SetLength(long value) => this.m_stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.m_stream.Write(buffer, offset, count);
  }
}
