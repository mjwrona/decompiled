// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CircularBufferLog
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class CircularBufferLog
  {
    private readonly byte[] m_log;
    private int m_count;
    private int m_position;

    public CircularBufferLog(int capacity) => this.m_log = new byte[capacity];

    public void Reset()
    {
      this.m_count = 0;
      this.m_position = 0;
    }

    public int Capacity => this.m_log.Length;

    public int Count => this.m_count;

    public int Read(byte[] buffer, int offset, int rewindOffset, int count)
    {
      ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0, buffer.Length);
      ArgumentUtility.CheckForOutOfRange(rewindOffset, nameof (rewindOffset), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, buffer.Length - offset);
      if (rewindOffset > this.m_count)
        throw new ArgumentOutOfRangeException(nameof (rewindOffset));
      int sourceIndex = count <= rewindOffset ? this.GetReadOffset(rewindOffset) : throw new ArgumentOutOfRangeException(nameof (count));
      int length1 = Math.Min(count, this.m_log.Length - sourceIndex);
      Array.Copy((Array) this.m_log, sourceIndex, (Array) buffer, offset, length1);
      int length2 = count - length1;
      if (length2 > 0)
        Array.Copy((Array) this.m_log, 0, (Array) buffer, offset + length1, length2);
      return count;
    }

    public void Erase(int count)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, this.m_count);
      this.m_position = this.GetReadOffset(count);
      this.m_count -= count;
      if (this.m_count != 0)
        return;
      this.m_position = 0;
    }

    private int GetReadOffset(int rewindOffset)
    {
      int readOffset = this.m_position - rewindOffset;
      if (readOffset < 0)
        readOffset += this.m_log.Length;
      return readOffset;
    }

    public void Flush()
    {
      if (this.m_count > 0)
        this.Displace(this.m_count);
      this.m_count = 0;
      this.m_position = 0;
    }

    private void Displace(int bytesDisplaced)
    {
      CircularBufferLog.BytesDisplacedEventHandler bytesDisplaced1 = this.BytesDisplaced;
      if (bytesDisplaced1 == null)
        return;
      int readOffset = this.GetReadOffset(this.m_count);
      int count1 = Math.Min(bytesDisplaced, this.m_log.Length - readOffset);
      bytesDisplaced1((object) this, this.m_log, readOffset, count1);
      int count2 = bytesDisplaced - count1;
      if (count2 <= 0)
        return;
      bytesDisplaced1((object) this, this.m_log, 0, count2);
    }

    public void Write(byte[] buffer, int offset, int count)
    {
      ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0, buffer.Length);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, buffer.Length - offset);
      CircularBufferLog.BytesDisplacedEventHandler bytesDisplaced1 = this.BytesDisplaced;
      int val1 = Math.Min(this.m_log.Length, count);
      if (bytesDisplaced1 != null)
      {
        int bytesDisplaced2 = Math.Max(val1 - (this.m_log.Length - this.m_count), 0);
        if (bytesDisplaced2 > 0)
          this.Displace(bytesDisplaced2);
        int count1 = count - val1;
        if (count1 > 0)
          bytesDisplaced1((object) this, buffer, offset, count1);
      }
      if (val1 == this.m_log.Length)
        this.m_position = 0;
      int sourceIndex = Math.Max(offset + count - this.m_log.Length, offset);
      int length1 = Math.Min(val1, this.m_log.Length - this.m_position);
      Array.Copy((Array) buffer, sourceIndex, (Array) this.m_log, this.m_position, length1);
      int length2 = val1 - length1;
      if (length2 > 0)
        Array.Copy((Array) buffer, sourceIndex + length1, (Array) this.m_log, 0, length2);
      this.m_position = (this.m_position + val1) % this.m_log.Length;
      if (this.m_count >= this.m_log.Length)
        return;
      this.m_count = Math.Min(this.m_log.Length, this.m_count + val1);
    }

    private void FireBytesDisplaced(int offset, int count)
    {
      CircularBufferLog.BytesDisplacedEventHandler bytesDisplaced = this.BytesDisplaced;
      if (bytesDisplaced == null)
        return;
      bytesDisplaced((object) this, this.m_log, offset, count);
    }

    public event CircularBufferLog.BytesDisplacedEventHandler BytesDisplaced;

    public delegate void BytesDisplacedEventHandler(
      object sender,
      byte[] buffer,
      int offset,
      int count);
  }
}
