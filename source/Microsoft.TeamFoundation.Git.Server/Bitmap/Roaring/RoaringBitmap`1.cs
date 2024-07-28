// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.RoaringBitmap`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Riff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class RoaringBitmap<T> : AbstractBitmap<T>
  {
    private int? m_count;
    private List<ushort> m_highBits;
    private bool m_highBitsSorted;
    private Dictionary<ushort, IChunk> m_chunks;

    public RoaringBitmap(ITwoWayReadOnlyList<T> objectList)
      : base(objectList)
    {
      this.m_highBits = new List<ushort>();
      this.m_chunks = new Dictionary<ushort, IChunk>();
    }

    public RoaringBitmap(
      ITwoWayReadOnlyList<T> objectList,
      Dictionary<ushort, IChunk> chunks,
      bool readOnly = false)
      : this(objectList)
    {
      foreach (KeyValuePair<ushort, IChunk> chunk in chunks)
      {
        this.m_highBits.Add(chunk.Key);
        this.m_chunks[chunk.Key] = chunk.Value;
      }
      if (readOnly)
        base.MakeReadOnly();
      this.m_highBits.Sort();
    }

    public override void MakeReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.MakeReadOnly();
      this.OptimizeAsReadOnly();
    }

    public void OptimizeAsReadOnly()
    {
      this.EnsureHighBitsSorted();
      for (int index = 0; index < this.m_highBits.Count; ++index)
      {
        ushort highBit = this.m_highBits[index];
        IChunk chunk = this.m_chunks[highBit];
        this.m_chunks[highBit] = ChunkOptimizer.OptimizeAsReadOnly(chunk);
      }
    }

    public IChunk GetChunk(ushort chunkId)
    {
      IChunk chunk;
      return this.m_chunks.TryGetValue(chunkId, out chunk) ? chunk : (IChunk) null;
    }

    public void SetChunk(ushort chunkId, IChunk chunk)
    {
      this.CheckForReadOnly();
      if (!this.m_chunks.ContainsKey(chunkId))
      {
        this.m_highBits.Add(chunkId);
        this.m_highBitsSorted = false;
      }
      this.m_chunks[chunkId] = chunk;
      this.m_count = new int?();
    }

    public override int Count
    {
      get
      {
        this.EnsureCountComputed();
        return this.m_count.Value;
      }
    }

    public override bool ContainsIndex(int index)
    {
      if (index < 0 || index >= this.FullObjectList.Count)
        throw new ArgumentOutOfRangeException(nameof (index));
      ushort highBits;
      ushort lowBits;
      BitUtils.Split(index, out highBits, out lowBits);
      IChunk chunk;
      return this.m_chunks.TryGetValue(highBits, out chunk) && chunk.Contains(lowBits);
    }

    public override IEnumerable<int> Indices
    {
      get
      {
        this.EnsureHighBitsSorted();
        for (int i = 0; i < this.m_highBits.Count; ++i)
        {
          ushort highBits = this.m_highBits[i];
          foreach (ushort lowBits in (IEnumerable<ushort>) this.m_chunks[highBits])
            yield return BitUtils.Join(highBits, lowBits);
        }
      }
    }

    public override bool AddIndex(int index)
    {
      this.CheckForReadOnly();
      if (index < 0 || index >= this.FullObjectList.Count)
        throw new ArgumentOutOfRangeException(nameof (index));
      ushort highBits;
      ushort lowBits;
      BitUtils.Split(index, out highBits, out lowBits);
      IChunk chunk1;
      if (!this.m_chunks.TryGetValue(highBits, out chunk1))
      {
        chunk1 = this.FullObjectList.Count >= ((int) highBits + 1) * 65536 ? (IChunk) new RawChunk() : (IChunk) new RawChunk(new ulong[(this.FullObjectList.Count - (int) highBits * 65536 + 63) / 64]);
        this.m_chunks[highBits] = chunk1;
        this.m_highBits.Add(highBits);
        this.m_highBitsSorted = false;
      }
      this.EnsureCountComputed();
      int num = chunk1.Add(lowBits) ? 1 : 0;
      if (num == 0)
        return num != 0;
      int? count = this.m_count;
      this.m_count = count.HasValue ? new int?(count.GetValueOrDefault() + 1) : new int?();
      IChunk chunk2 = chunk1.Optimize();
      if (chunk2 == chunk1)
        return num != 0;
      this.m_chunks[highBits] = chunk2;
      return num != 0;
    }

    public void AddIndexRange(int fromIndex, int toIndex)
    {
      this.CheckForReadOnly();
      if (toIndex <= fromIndex)
        return;
      if (fromIndex < 0)
        throw new IndexOutOfRangeException(nameof (fromIndex));
      ushort highBits1;
      ushort lowBits1;
      BitUtils.Split(fromIndex, out highBits1, out lowBits1);
      ushort highBits2;
      ushort lowBits2;
      BitUtils.Split(toIndex, out highBits2, out lowBits2);
      if ((int) highBits1 == (int) highBits2)
      {
        this.ReplaceOrAddRangeToChunk(highBits1, lowBits1, (ushort) ((uint) lowBits2 - 1U));
      }
      else
      {
        this.ReplaceOrAddRangeToChunk(highBits1, lowBits1, ushort.MaxValue);
        for (int chunkId = (int) highBits1 + 1; chunkId < (int) highBits2; ++chunkId)
          this.SetChunk((ushort) chunkId, (IChunk) new RunChunk(new ushort[2]
          {
            (ushort) 0,
            ushort.MaxValue
          }));
        if (lowBits2 <= (ushort) 0)
          return;
        this.ReplaceOrAddRangeToChunk(highBits2, (ushort) 0, (ushort) ((uint) lowBits2 - 1U));
      }
    }

    private void ReplaceOrAddRangeToChunk(ushort highBits, ushort lowStart, ushort lowEnd)
    {
      if (this.GetChunk(highBits) == null)
      {
        this.SetChunk(highBits, (IChunk) new RunChunk(new ushort[2]
        {
          lowStart,
          (ushort) ((uint) lowEnd - (uint) lowStart)
        }));
      }
      else
      {
        for (ushort lowBits = lowStart; (int) lowBits <= (int) lowEnd; ++lowBits)
          this.AddIndex(BitUtils.Join(highBits, lowBits));
      }
    }

    public IReadOnlyList<ushort> ChunkIds
    {
      get
      {
        this.EnsureHighBitsSorted();
        return (IReadOnlyList<ushort>) this.m_highBits;
      }
    }

    public override int GetSize()
    {
      int size = CacheUtil.ObjectOverhead + this.m_highBits.Capacity * 2 + 2 * this.m_chunks.Keys.Count * IntPtr.Size;
      for (int index = 0; index < this.m_highBits.Count; ++index)
        size += this.m_chunks[this.m_highBits[index]].GetSize();
      return size;
    }

    private void EnsureHighBitsSorted()
    {
      if (this.m_highBitsSorted)
        return;
      this.m_highBits.Sort();
      this.m_highBitsSorted = true;
    }

    private void EnsureCountComputed()
    {
      if (this.m_count.HasValue)
        return;
      int num = 0;
      foreach (IChunk chunk in this.m_chunks.Values)
        num += chunk.Count;
      this.m_count = new int?(num);
    }

    internal class M118ArrayChunkFormat : RoaringBitmap<T>.IM118ChunkFormat
    {
      public int GetBytesToWrite(IChunk chunk) => checked (4 + ((AbstractChunk) chunk).Count * 2);

      public void WriteChunk(IChunk chunk, Stream stream, byte[] buffer)
      {
        ArrayChunk arrayChunk = (ArrayChunk) chunk;
        GitStreamUtil.WriteArray<int>(stream, new int[1]
        {
          arrayChunk.Count
        }, 4, 0, 1, buffer);
        GitStreamUtil.WriteArray<ushort>(stream, arrayChunk.Values, 2, 0, chunk.Count, buffer);
      }

      public IChunk ReadChunk(Stream stream, byte[] buffer)
      {
        int[] numArray = GitStreamUtil.ReadArray<int>(stream, 4, 1, buffer);
        return (IChunk) new ArrayChunk((IEnumerable<ushort>) GitStreamUtil.ReadArray<ushort>(stream, 2, numArray[0], buffer), new int?(numArray[0]), true);
      }
    }

    internal sealed class M118Format : IRoaringBitmapReader<T>, IRoaringBitmapWriter<T>
    {
      public const byte ChunkTypeRaw = 1;
      public const byte ChunkTypeRun = 2;
      public const byte ChunkTypeArray = 3;
      private static readonly Dictionary<Type, byte> s_typeToByte;
      private static readonly Dictionary<byte, RoaringBitmap<T>.IM118ChunkFormat> s_byteToFormat = new Dictionary<byte, RoaringBitmap<T>.IM118ChunkFormat>();

      static M118Format()
      {
        RoaringBitmap<T>.M118Format.s_byteToFormat[(byte) 3] = (RoaringBitmap<T>.IM118ChunkFormat) new RoaringBitmap<T>.M118ArrayChunkFormat();
        RoaringBitmap<T>.M118Format.s_byteToFormat[(byte) 2] = (RoaringBitmap<T>.IM118ChunkFormat) new RoaringBitmap<T>.M118RunChunkFormat();
        RoaringBitmap<T>.M118Format.s_byteToFormat[(byte) 1] = (RoaringBitmap<T>.IM118ChunkFormat) new RoaringBitmap<T>.M118RawChunkFormat();
        RoaringBitmap<T>.M118Format.s_typeToByte = new Dictionary<Type, byte>();
        RoaringBitmap<T>.M118Format.s_typeToByte[typeof (ArrayChunk)] = (byte) 3;
        RoaringBitmap<T>.M118Format.s_typeToByte[typeof (RunChunk)] = (byte) 2;
        RoaringBitmap<T>.M118Format.s_typeToByte[typeof (RawChunk)] = (byte) 1;
      }

      public M118Format(ITwoWayReadOnlyList<T> objectList) => this.ObjectList = objectList;

      public ITwoWayReadOnlyList<T> ObjectList { get; }

      public RoaringBitmap<T> Read(Stream stream, byte[] buffer)
      {
        int read = GitStreamUtil.ReadArray<int>(stream, 4, 1, buffer)[0];
        ushort[] numArray = GitStreamUtil.ReadArray<ushort>(stream, 2, read, buffer);
        GitStreamUtil.ReadArray<int>(stream, 4, read, buffer);
        Dictionary<ushort, IChunk> chunks = new Dictionary<ushort, IChunk>();
        for (int index = 0; index < read; ++index)
          chunks[numArray[index]] = this.ReadChunk(stream, buffer);
        return new RoaringBitmap<T>(this.ObjectList, chunks, true);
      }

      public int GetSizeToWrite(RoaringBitmap<T> bitmap)
      {
        int count = bitmap.ChunkIds.Count;
        int sizeToWrite = checked (4 + count * 7);
        for (int index = 0; index < count; ++index)
        {
          IChunk chunk = bitmap.GetChunk(bitmap.ChunkIds[index]);
          byte key;
          if (!RoaringBitmap<T>.M118Format.s_typeToByte.TryGetValue(chunk.GetType(), out key))
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Chunk type {0} not supported.", (object) chunk.GetType())));
          checked { sizeToWrite += RoaringBitmap<T>.M118Format.s_byteToFormat[key].GetBytesToWrite(chunk); }
        }
        return sizeToWrite;
      }

      public long Write(RoaringBitmap<T> bitmap, Stream stream, byte[] buffer)
      {
        int count = bitmap.ChunkIds.Count;
        ushort[] values1 = new ushort[count];
        int[] values2 = new int[count];
        byte[] numArray = new byte[count];
        int num1 = 0;
        for (int index = 0; index < count; ++index)
        {
          values1[index] = bitmap.ChunkIds[index];
          values2[index] = num1;
          IChunk chunk = bitmap.GetChunk(bitmap.ChunkIds[index]);
          if (!RoaringBitmap<T>.M118Format.s_typeToByte.TryGetValue(chunk.GetType(), out numArray[index]))
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Chunk type {0} not supported.", (object) chunk.GetType())));
          num1 += 1 + RoaringBitmap<T>.M118Format.s_byteToFormat[numArray[index]].GetBytesToWrite(chunk);
        }
        GitStreamUtil.WriteArray<int>(stream, new int[1]
        {
          count
        }, 4, 0, 1, buffer);
        GitStreamUtil.WriteArray<ushort>(stream, values1, 2, 0, count, buffer);
        GitStreamUtil.WriteArray<int>(stream, values2, 4, 0, count, buffer);
        long num2 = (long) (4 + count * 6);
        for (int index = 0; index < count; ++index)
        {
          IChunk chunk = bitmap.GetChunk(bitmap.ChunkIds[index]);
          RoaringBitmap<T>.IM118ChunkFormat m118ChunkFormat = RoaringBitmap<T>.M118Format.s_byteToFormat[numArray[index]];
          num2 = checked (num2 + 1L + (long) m118ChunkFormat.GetBytesToWrite(chunk));
          stream.WriteByte(numArray[index]);
          m118ChunkFormat.WriteChunk(chunk, stream, buffer);
        }
        return num2;
      }

      public IChunk ReadChunk(Stream stream, byte[] buffer)
      {
        byte key = stream.Read(buffer, 0, 1) != 0 ? buffer[0] : throw new EndOfStreamException();
        RoaringBitmap<T>.IM118ChunkFormat m118ChunkFormat;
        if (!RoaringBitmap<T>.M118Format.s_byteToFormat.TryGetValue(key, out m118ChunkFormat))
          throw new InvalidDataException(string.Format("Expected a valid chunk type. Current type {0} is invalid.", (object) buffer[0]));
        return m118ChunkFormat.ReadChunk(stream, buffer);
      }
    }

    internal interface IM118ChunkFormat
    {
      int GetBytesToWrite(IChunk chunk);

      void WriteChunk(IChunk chunk, Stream stream, byte[] buffer);

      IChunk ReadChunk(Stream stream, byte[] buffer);
    }

    internal class M118RawChunkFormat : RoaringBitmap<T>.IM118ChunkFormat
    {
      public int GetBytesToWrite(IChunk chunk) => checked (8 + ((RawChunk) chunk).Bitmap.Length * 8);

      public void WriteChunk(IChunk chunk, Stream stream, byte[] buffer)
      {
        RawChunk rawChunk = (RawChunk) chunk;
        GitStreamUtil.WriteArray<int>(stream, new int[2]
        {
          rawChunk.Offset,
          rawChunk.Bitmap.Length
        }, 4, 0, 2, buffer);
        GitStreamUtil.WriteArray<ulong>(stream, rawChunk.Bitmap, 8, 0, rawChunk.Bitmap.Length, buffer);
      }

      public IChunk ReadChunk(Stream stream, byte[] buffer)
      {
        int[] numArray = GitStreamUtil.ReadArray<int>(stream, 4, 2, buffer);
        return (IChunk) new RawChunk(GitStreamUtil.ReadArray<ulong>(stream, 8, numArray[1], buffer), numArray[0], true);
      }
    }

    internal class M118RunChunkFormat : RoaringBitmap<T>.IM118ChunkFormat
    {
      public int GetBytesToWrite(IChunk chunk) => checked (4 + ((AbstractChunk) chunk).CountRuns * 2 * 2);

      public void WriteChunk(IChunk chunk, Stream stream, byte[] buffer)
      {
        RunChunk runChunk = (RunChunk) chunk;
        GitStreamUtil.WriteArray<int>(stream, new int[1]
        {
          runChunk.CountRuns
        }, 4, 0, 1, buffer);
        GitStreamUtil.WriteArray<ushort>(stream, runChunk.Runs, 2, 0, 2 * chunk.CountRuns, buffer);
      }

      public IChunk ReadChunk(Stream stream, byte[] buffer)
      {
        int read = GitStreamUtil.ReadArray<int>(stream, 4, 1, buffer)[0];
        return (IChunk) new RunChunk(GitStreamUtil.ReadArray<ushort>(stream, 2, 2 * read, buffer), true);
      }
    }

    public sealed class M123RiffFormat
    {
      private readonly RoaringBitmap<T>.M118Format m_innerFormat;
      public const uint FormType = 829252210;

      public M123RiffFormat(ITwoWayReadOnlyList<T> objectList) => this.m_innerFormat = new RoaringBitmap<T>.M118Format(objectList);

      public RoaringBitmap<T> Read(Stream stream, byte[] buffer)
      {
        RiffFile riff;
        if (!RiffFile.TryLoad(stream, out riff, true))
          throw new InvalidDataException();
        using (riff)
        {
          RiffChunk chunk = RiffUtil.GetChunk(riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id)), 942748002U);
          chunk.Stream.Position = 0L;
          return this.m_innerFormat.Read(chunk.Stream, buffer);
        }
      }

      public void Write(RoaringBitmap<T> bitmap, Stream stream, byte[] buffer)
      {
        using (RiffWriter riffWriter = new RiffWriter(stream, true))
        {
          riffWriter.BeginRiffChunk(829252210U, 0U);
          uint sizeToWrite = checked ((uint) this.m_innerFormat.GetSizeToWrite(bitmap));
          riffWriter.BeginChunk(942748002U, sizeToWrite);
          this.m_innerFormat.Write(bitmap, stream, buffer);
          riffWriter.EndChunk();
          riffWriter.EndChunk();
        }
      }

      public static class ChunkIds
      {
        public const uint M118Bitmap = 942748002;
      }
    }
  }
}
