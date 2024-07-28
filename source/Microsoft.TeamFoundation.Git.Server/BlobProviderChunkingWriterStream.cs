// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BlobProviderChunkingWriterStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class BlobProviderChunkingWriterStream : Stream
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly ITfsGitBlobProvider m_blobProvider;
    private readonly OdbId m_odbId;
    private readonly string m_resourceId;
    private long m_outputPosition;
    private bool m_closed;

    public BlobProviderChunkingWriterStream(
      IVssRequestContext requestContext,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId)
    {
      this.m_requestContext = requestContext;
      this.m_blobProvider = blobProvider;
      this.m_odbId = odbId;
      this.m_resourceId = resourceId;
      this.m_outputPosition = 0L;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.m_blobProvider.PutChunk(this.m_requestContext, this.m_odbId, this.m_resourceId, Array.Empty<byte>(), 0, this.m_outputPosition, this.m_outputPosition, true);
      }
      base.Close();
    }

    public override void Flush()
    {
    }

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
      byte[] numArray = buffer;
      if (offset != 0)
      {
        numArray = new byte[count];
        Array.Copy((Array) buffer, offset, (Array) numArray, 0, count);
      }
      this.m_blobProvider.PutChunk(this.m_requestContext, this.m_odbId, this.m_resourceId, numArray, count, long.MaxValue, this.m_outputPosition, false);
      this.m_outputPosition += (long) count;
    }

    public override long Position
    {
      get => this.m_outputPosition;
      set => throw new NotImplementedException();
    }

    public override bool CanSeek => false;

    public override bool CanRead => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Length => throw new NotImplementedException();
  }
}
