// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LengthCountingStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class LengthCountingStream : Stream
  {
    private readonly Stream m_stream;
    private readonly bool m_leaveOpen;
    private long m_bytesProcessed;
    private long? m_expectedLength;
    private bool m_isReadable;
    private bool m_closed;

    public static LengthCountingStream CreateForRead(
      Stream stream,
      bool leaveOpen = false,
      long? expectedLength = null)
    {
      return new LengthCountingStream(stream, leaveOpen, expectedLength, true);
    }

    public static LengthCountingStream CreateForWrite(Stream stream, bool leaveOpen = false) => new LengthCountingStream(stream, leaveOpen, new long?(), false);

    private LengthCountingStream(
      Stream stream,
      bool leaveOpen,
      long? expectedLength,
      bool isReadable)
    {
      this.m_stream = stream;
      this.m_leaveOpen = leaveOpen;
      this.m_expectedLength = expectedLength;
      this.m_isReadable = isReadable;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (this.m_isReadable && this.m_stream is IZlibInflateStream stream && stream.LeaveBaseOpen && stream.AvailableInputBytes > 0)
        {
          if (stream.BaseStream.CanSeek)
          {
            stream.BaseStream.Seek((long) -stream.AvailableInputBytes, SeekOrigin.Current);
          }
          else
          {
            if (!(stream.BaseStream is IRewindableStream baseStream))
              throw new InvalidOperationException("Seekable or rewindable stream required.");
            baseStream.Rewind(stream.AvailableInputBytes);
          }
        }
        if (!this.m_leaveOpen)
          this.m_stream.Close();
      }
      base.Close();
    }

    public override bool CanRead => this.m_isReadable && this.m_stream.CanRead;

    public override bool CanWrite => !this.m_isReadable;

    public override long Length => !this.m_isReadable ? this.m_stream.Length : this.m_expectedLength ?? this.m_stream.Length;

    public override void Flush() => this.m_stream.Flush();

    public override long Position
    {
      get => this.m_bytesProcessed;
      set => throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.m_isReadable)
        throw new InvalidOperationException("Cannot write to readable stream.");
      this.m_stream.Write(buffer, offset, count);
      this.m_bytesProcessed += (long) count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (!this.m_isReadable)
        throw new InvalidOperationException("Cannot read from writable stream.");
      int num = this.m_stream.Read(buffer, offset, count);
      this.m_bytesProcessed += (long) num;
      return num;
    }

    public override bool CanSeek => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();
  }
}
