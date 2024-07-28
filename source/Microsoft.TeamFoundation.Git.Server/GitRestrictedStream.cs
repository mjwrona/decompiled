// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRestrictedStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRestrictedStream : Stream
  {
    private Stream m_stream;
    private readonly long m_offsetWithinFile;
    private readonly long m_length;
    private readonly bool m_leaveOpen;
    private Stack<Action> m_onDispose;

    public GitRestrictedStream(Stream stream, long offset, long length, bool leaveOpen)
    {
      this.m_stream = stream;
      this.m_offsetWithinFile = offset;
      this.m_stream.Seek(offset, SeekOrigin.Begin);
      this.m_length = length;
      this.m_leaveOpen = leaveOpen;
    }

    public void PushActionOnDispose(Action dispose)
    {
      this.EnsureNotDisposed();
      this.m_onDispose = this.m_onDispose ?? new Stack<Action>();
      this.m_onDispose.Push(dispose);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.m_stream != null)
      {
        while (this.m_onDispose != null && this.m_onDispose.Count != 0)
          this.m_onDispose.Pop()();
        if (!this.m_leaveOpen)
          this.m_stream.Dispose();
        this.m_stream = (Stream) null;
      }
      base.Dispose(disposing);
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override long Length => this.m_length;

    public override long Position
    {
      get => this.m_stream.Position - this.m_offsetWithinFile;
      set
      {
        if (value < 0L)
          throw new IOException(Resources.Get("CannotSeekBeforeZero"));
        this.m_stream.Position = value + this.m_offsetWithinFile;
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.Position + (long) count > this.Length)
      {
        count = (int) (this.Length - this.Position);
        if (count < 0)
          count = 0;
      }
      return this.m_stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          return this.Position = offset;
        case SeekOrigin.Current:
          return this.Position += offset;
        case SeekOrigin.End:
          return this.Position = this.m_length + offset;
        default:
          throw new NotImplementedException();
      }
    }

    public override bool CanWrite => false;

    public override void Flush() => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    private void EnsureNotDisposed()
    {
      if (this.m_stream == null)
        throw new ObjectDisposedException(nameof (GitRestrictedStream));
    }
  }
}
