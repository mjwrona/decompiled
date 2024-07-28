// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.WriteBufferStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class WriteBufferStream : Stream
  {
    private readonly Stream m_stream;
    private readonly bool m_leaveOpen;
    private ByteArray m_buffer;
    private bool m_closed;
    private int m_position;

    public WriteBufferStream(Stream stream, int capacity = 65536, bool leaveOpen = false)
    {
      this.m_stream = stream;
      this.m_leaveOpen = leaveOpen;
      this.m_buffer = new ByteArray(capacity);
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.Flush();
        if (!this.m_leaveOpen)
          this.m_stream.Close();
        if (this.m_buffer != null)
        {
          this.m_buffer.Dispose();
          this.m_buffer = (ByteArray) null;
        }
      }
      base.Close();
    }

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
      while (count > 0)
      {
        int length = Math.Min(this.m_buffer.SizeRequested - this.m_position, count);
        Array.Copy((Array) buffer, offset, (Array) this.m_buffer.Bytes, this.m_position, length);
        this.m_position += length;
        if (this.m_position == this.m_buffer.SizeRequested)
        {
          this.m_stream.Write(this.m_buffer.Bytes, 0, this.m_position);
          this.m_position = 0;
        }
        count -= length;
        offset += length;
      }
    }

    public override void Flush()
    {
      if (this.m_position > 0)
      {
        this.m_stream.Write(this.m_buffer.Bytes, 0, this.m_position);
        this.m_position = 0;
      }
      this.m_stream.Flush();
    }

    public override bool CanSeek => false;

    public override bool CanRead => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Length => throw new NotImplementedException();
  }
}
