// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.SidebandStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal class SidebandStream : Stream
  {
    private readonly Stream m_stream;
    private readonly bool m_leaveOpen;
    private readonly byte[] m_buffer;
    private bool m_closed;
    private const int c_maxBytesPerLine = 65515;

    public SidebandStream(Stream stream, SidebandChannel channel, bool leaveOpen = false)
    {
      this.m_stream = stream;
      this.m_leaveOpen = leaveOpen;
      if (channel == SidebandChannel.None)
        return;
      this.m_buffer = new byte[5];
      this.m_buffer[4] = (byte) channel;
    }

    public int MaxLineSize => 65515;

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        this.Flush();
        if (!this.m_leaveOpen)
          this.m_stream.Close();
      }
      base.Close();
    }

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
      int count1;
      for (; count > 0; count -= count1)
      {
        count1 = Math.Min(count, 65515);
        if (this.m_buffer != null)
        {
          string s = (this.m_buffer.Length + count1).ToString("x4");
          GitEncodingUtil.SafeUtf8NoBom.GetBytes(s, 0, 4, this.m_buffer, 0);
          this.m_stream.Write(this.m_buffer, 0, this.m_buffer.Length);
        }
        this.m_stream.Write(buffer, offset, count1);
        offset += count1;
      }
    }

    public override void Flush() => this.m_stream.Flush();

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
