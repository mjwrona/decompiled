// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RewindableStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RewindableStream : Stream, IRewindableStream
  {
    private readonly Stream m_stream;
    private readonly CircularBufferLog m_log;
    private readonly bool m_leaveOpen;
    private long m_position;
    private int m_rewindDistance;
    private bool m_closed;

    public RewindableStream(Stream stream, int rewindCapacity = 4096, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForOutOfRange(rewindCapacity, nameof (rewindCapacity), 0);
      this.m_stream = stream;
      this.m_log = new CircularBufferLog(rewindCapacity);
      this.m_leaveOpen = leaveOpen;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (!this.m_leaveOpen)
          this.m_stream.Close();
      }
      base.Close();
    }

    public override bool CanRead => true;

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_position;
      set => throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num1 = 0;
      int count1 = Math.Min(this.m_rewindDistance, count);
      int count2 = count - count1;
      if (count1 > 0)
      {
        int num2 = this.m_log.Read(buffer, offset, this.m_rewindDistance, count1);
        this.m_rewindDistance -= count1;
        num1 += num2;
      }
      if (count2 > 0)
      {
        int count3 = this.m_stream.Read(buffer, offset + count1, count2);
        this.m_log.Write(buffer, offset + count1, count3);
        num1 += count3;
      }
      this.m_position += (long) num1;
      return num1;
    }

    public int RewindCapacity => this.m_log.Capacity;

    public long Rewind(int positiveOffset)
    {
      ArgumentUtility.CheckForOutOfRange(positiveOffset, nameof (positiveOffset), 0, this.m_log.Count);
      int num = this.m_rewindDistance + positiveOffset;
      if (num > this.m_log.Count)
        throw new ArgumentOutOfRangeException("offset");
      this.m_rewindDistance = num;
      this.m_position -= (long) positiveOffset;
      return this.m_position;
    }

    public override bool CanWrite => false;

    public override bool CanSeek => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void Flush() => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
