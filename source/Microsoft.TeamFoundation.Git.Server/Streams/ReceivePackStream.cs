// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Streams.ReceivePackStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Streams
{
  internal class ReceivePackStream : FileBufferedStreamBase
  {
    private readonly string m_filePath;
    private readonly FileStream m_stream;
    private bool m_closed;
    private bool m_disposed;
    private bool m_read;

    public ReceivePackStream(FileStream fs)
      : base(long.MaxValue)
    {
      this.m_stream = fs;
      this.m_filePath = fs.Name;
    }

    public ReceivePackStream(string tempDir, int bufferSize = 4096)
      : base(long.MaxValue)
    {
      this.m_filePath = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
      this.m_stream = FileBufferedStreamBase.CreateFileWithTemporaryAttribute(this.m_filePath, bufferSize, FileAccess.ReadWrite);
    }

    public override bool BufferingComplete => true;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    public override Exception Exception => (Exception) null;

    public override long Length => this.m_stream.Length;

    public override string Name
    {
      get
      {
        this.MarkAsReadFrom();
        return this.m_filePath;
      }
    }

    public override long Position
    {
      get => this.m_stream.Position;
      set => this.m_stream.Position = value;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.m_stream.Close();
      }
      base.Close();
    }

    public override void Flush()
    {
      this.CheckNotReadFrom();
      this.m_stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.MarkAsReadFrom();
      return this.m_stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

    public override void SetLength(long value)
    {
      this.CheckNotReadFrom();
      this.m_stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.CheckNotReadFrom();
      this.m_stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || this.m_disposed)
        return;
      this.m_disposed = true;
      this.m_stream.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckNotReadFrom()
    {
      if (this.m_read)
        throw new InvalidOperationException("This operation is not supported after the Stream has been read from.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MarkAsReadFrom()
    {
      if (this.m_read)
        return;
      this.Flush();
      this.m_read = true;
    }
  }
}
