// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.SegmentedMemoryStream
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class SegmentedMemoryStream : Stream
  {
    private readonly object readWriteLock = new object();
    private int m_length;
    private int m_position;
    private bool m_readOnly;
    private readonly int m_segmentSize;
    private readonly List<byte[]> m_segments = new List<byte[]>();

    public SegmentedMemoryStream(int segmentSize = 81920) => this.m_segmentSize = segmentSize;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => !this.m_readOnly;

    public override long Length => (long) this.m_length;

    public override long Position
    {
      get => (long) this.m_position;
      set
      {
        if (value < 0L)
          throw new ArgumentOutOfRangeException(string.Format("Cannot seek before the beginning of the stream. Attempted to set Position to {0}.", (object) value));
        lock (this.readWriteLock)
          this.m_position = (int) value;
      }
    }

    protected override void Dispose(bool disposing) => this.m_segments.Clear();

    public override void Flush()
    {
    }

    public void MakeReadOnly() => this.m_readOnly = true;

    public override int Read(byte[] buffer, int offset, int count)
    {
      lock (this.readWriteLock)
      {
        ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
        ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
        ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
        if (count > buffer.Length - offset)
          throw new ArgumentOutOfRangeException(nameof (count));
        if (this.m_position >= this.m_length)
          return 0;
        count = Math.Min(count, this.m_length - this.m_position);
        int length1 = count;
        int position = (int) this.Position;
        int index = position / this.m_segmentSize;
        int sourceIndex;
        int length2;
        for (sourceIndex = position % this.m_segmentSize; (length2 = this.m_segments[index].Length - sourceIndex) <= length1; length1 -= length2)
        {
          Array.Copy((Array) this.m_segments[index], sourceIndex, (Array) buffer, offset, length2);
          ++index;
          sourceIndex = 0;
          if (length1 == length2)
          {
            this.m_position += count;
            return count;
          }
          offset += length2;
        }
        Array.Copy((Array) this.m_segments[index], sourceIndex, (Array) buffer, offset, length1);
        this.m_position += count;
        return count;
      }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      lock (this.readWriteLock)
      {
        switch (origin)
        {
          case SeekOrigin.Begin:
            return this.Position = offset;
          case SeekOrigin.Current:
            return checked (this.Position += offset);
          case SeekOrigin.End:
            return this.Position = checked (this.Length + offset);
          default:
            throw new NotImplementedException();
        }
      }
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      lock (this.readWriteLock)
      {
        if (this.m_readOnly)
          throw new NotSupportedException();
        int position = checked ((int) this.Position);
        int num = (position + count) / this.m_segmentSize;
        if (this.m_segments.Count <= num)
        {
          for (int count1 = this.m_segments.Count; count1 <= num; ++count1)
            this.m_segments.Add(new byte[this.m_segmentSize]);
        }
        int length1 = count;
        int index = position / this.m_segmentSize;
        int destinationIndex;
        int length2;
        for (destinationIndex = position % this.m_segmentSize; (length2 = this.m_segments[index].Length - destinationIndex) <= length1; length1 -= length2)
        {
          Array.Copy((Array) buffer, offset, (Array) this.m_segments[index], destinationIndex, length2);
          ++index;
          destinationIndex = 0;
          if (length1 == length2)
          {
            this.m_position += count;
            this.m_length = Math.Max(this.m_length, this.m_position);
            return;
          }
          offset += length2;
        }
        Array.Copy((Array) buffer, offset, (Array) this.m_segments[index], destinationIndex, length1);
        this.m_position += count;
        this.m_length = Math.Max(this.m_length, this.m_position);
      }
    }
  }
}
