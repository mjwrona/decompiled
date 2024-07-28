// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Streams.ConcatMemoryStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Streams
{
  internal sealed class ConcatMemoryStream : Stream
  {
    private readonly byte[][] m_shards;
    private readonly int m_length;
    private int m_iShard;
    private int m_shardPos;
    private int m_position;

    public ConcatMemoryStream(ConcatMemoryStream.Shards shards)
    {
      ArgumentUtility.CheckForNull<ConcatMemoryStream.Shards>(shards, nameof (shards));
      this.m_shards = shards.Buffers;
      this.m_length = shards.Length;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => (long) this.m_length;

    public override long Position
    {
      get => (long) this.m_position;
      set
      {
        int num1 = value >= 0L ? checked ((int) value) : throw new ArgumentOutOfRangeException("Cannot seek before the beginning of the stream.");
        int num2 = num1 - this.m_position;
        this.m_shardPos += num2;
        if (num2 > 0)
        {
          for (; this.m_iShard < this.m_shards.Length && this.m_shardPos >= this.m_shards[this.m_iShard].Length; ++this.m_iShard)
            this.m_shardPos -= this.m_shards[this.m_iShard].Length;
        }
        else
        {
          for (; this.m_shardPos < 0; this.m_shardPos += this.m_shards[this.m_iShard].Length)
            --this.m_iShard;
        }
        this.m_position = num1;
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
      if (count > buffer.Length - offset)
        throw new ArgumentException("count > buffer.Length - offset");
      if (this.m_position >= this.m_length)
        return 0;
      count = Math.Min(count, this.m_length - this.m_position);
      int length1;
      int length2;
      for (length1 = count; (length2 = this.m_shards[this.m_iShard].Length - this.m_shardPos) <= length1; length1 -= length2)
      {
        Array.Copy((Array) this.m_shards[this.m_iShard], this.m_shardPos, (Array) buffer, offset, length2);
        ++this.m_iShard;
        this.m_shardPos = 0;
        if (length1 != length2)
          offset += length2;
        else
          goto label_9;
      }
      Array.Copy((Array) this.m_shards[this.m_iShard], this.m_shardPos, (Array) buffer, offset, length1);
      this.m_shardPos += length1;
label_9:
      this.m_position += count;
      return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
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

    public override void Flush() => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public sealed class Shards
    {
      public Shards(byte[][] buffers)
      {
        this.Buffers = buffers;
        this.Length = ((IEnumerable<byte[]>) buffers).Sum<byte[]>((Func<byte[], int>) (s => s.Length));
      }

      public static bool TryShardNonLoh(Stream source, out ConcatMemoryStream.Shards shards) => ConcatMemoryStream.Shards.TryShard(source, 81920, out shards);

      public static bool TryShard(
        Stream source,
        int bufferSize,
        out ConcatMemoryStream.Shards shards)
      {
        ArgumentUtility.CheckForNull<Stream>(source, nameof (source));
        ArgumentUtility.CheckForOutOfRange(bufferSize, nameof (bufferSize), 1);
        int result;
        int num = Math.DivRem(checked ((int) (source.Length - source.Position)), bufferSize, out result);
        switch (num)
        {
          case 0:
            shards = (ConcatMemoryStream.Shards) null;
            return false;
          case 1:
            if (result != 0)
              break;
            goto case 0;
        }
        bool flag = result > 0;
        List<byte[]> numArrayList = new List<byte[]>(checked (num + flag ? 1 : 0));
        for (int index = 0; index < num; ++index)
        {
          byte[] buf = new byte[bufferSize];
          GitStreamUtil.ReadGreedy(source, buf, 0, buf.Length);
          numArrayList.Add(buf);
        }
        if (flag)
        {
          byte[] buf = new byte[result];
          GitStreamUtil.ReadGreedy(source, buf, 0, buf.Length);
          numArrayList.Add(buf);
        }
        shards = new ConcatMemoryStream.Shards(numArrayList.ToArray());
        return true;
      }

      internal byte[][] Buffers { get; }

      public int Length { get; }
    }
  }
}
