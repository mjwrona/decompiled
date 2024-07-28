// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.M91GitPackIndex
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Riff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal sealed class M91GitPackIndex : IGitPackIndex, IDisposable
  {
    private readonly RiffFile m_riff;
    public const uint M91FormType = 863528041;

    public M91GitPackIndex(RiffFile riff, Sha1Id? id)
    {
      this.m_riff = riff;
      this.Id = id;
      if (riff.ListType != 863528041U)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(string.Format("Expected RIFF form type 0x{0:x8}, actual was 0x{1:x8}", (object) 863528041U, (object) riff.ListType)));
      this.Version = new GitPackIndexVersion?(GitPackIndexVersion.M91);
      ILookup<uint, RiffChunk> lookup = riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id));
      this.BaseIndexIds = (IReadOnlyList<Sha1Id>) RiffUtil.ReadSha1Ids(RiffUtil.GetChunk(lookup, 538995042U).Stream);
      this.PackIds = (IReadOnlyList<Sha1Id>) RiffUtil.ReadSha1Ids(RiffUtil.GetChunk(lookup, 538995568U).Stream);
      this.PackStates = (IReadOnlyList<GitPackStates>) new M91GitPackIndex.PackStateList(RiffUtil.GetChunk(lookup, 538997616U).Stream);
      this.PackWatermarks = M91GitPackIndex.ReadPackWatermarks(lookup);
      Stream stream = RiffUtil.GetChunk(lookup, 538996326U).Stream;
      stream.Position = 0L;
      FanoutTable fanout = FanoutTable.FromStream(stream);
      if (fanout.EntryCount > (uint) int.MaxValue)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(string.Format("fanout.EntryCount: {0}", (object) fanout.EntryCount)));
      this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new Sha1IdSortedVirtualList(RiffUtil.GetChunk(lookup, 538976367U).Stream, fanout);
      if ((long) this.ObjectIds.Count != (long) fanout.EntryCount)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(string.Format("ObjectIds.Count ({0}) != fanout.EntryCount ({1})", (object) this.ObjectIds.Count, (object) fanout.EntryCount)));
      this.Entries = (IReadOnlyList<GitPackIndexEntry>) new M91GitPackIndex.EntryList(RiffUtil.GetChunk(lookup, 538976357U).Stream, lookup[544170597U].FirstOrDefault<RiffChunk>()?.Stream, lookup[543973989U].FirstOrDefault<RiffChunk>()?.Stream);
      if ((long) this.Entries.Count != (long) fanout.EntryCount)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(string.Format("Entries.Count ({0}) != fanout.EntryCount ({1})", (object) this.Entries.Count, (object) fanout.EntryCount)));
      M91GitPackIndex.StreamBackedList localToStable;
      M91GitPackIndex.StreamBackedList stableToLocal;
      this.StableObjectOrderEpoch = M91GitPackIndex.TryLoadStableObjectOrder(lookup, out localToStable, out stableToLocal);
      if (!this.StableObjectOrderEpoch.HasValue)
        return;
      this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new ReorderedObjectIdList(this.ObjectIds, (IReadOnlyList<int>) stableToLocal, (IReadOnlyList<int>) localToStable);
      this.Entries = (IReadOnlyList<GitPackIndexEntry>) new ReorderedEntryList(this.Entries, (IReadOnlyList<int>) stableToLocal);
    }

    private static IReadOnlyGitPackWatermarks ReadPackWatermarks(ILookup<uint, RiffChunk> chunks)
    {
      RiffChunk result;
      if (!chunks.TryGetChunk(538998640U, out result))
        return GitPackWatermarks.Empty;
      Stream stream = result.Stream;
      stream.Position = 0L;
      ushort[] numArray = RiffUtil.ReadArray<ushort>(stream, 2, new byte[stream.Length]);
      GitPackWatermarks gitPackWatermarks = new GitPackWatermarks();
      if (numArray.Length == 0)
        return (IReadOnlyGitPackWatermarks) gitPackWatermarks;
      gitPackWatermarks.Inner[GitPackWatermark.NumRepacked] = numArray[0];
      return (IReadOnlyGitPackWatermarks) gitPackWatermarks;
    }

    private static Sha1Id? TryLoadStableObjectOrder(
      ILookup<uint, RiffChunk> chunks,
      out M91GitPackIndex.StreamBackedList localToStable,
      out M91GitPackIndex.StreamBackedList stableToLocal)
    {
      RiffChunk result;
      if (!chunks.TryGetChunk(544173939U, out result))
      {
        localToStable = (M91GitPackIndex.StreamBackedList) null;
        stableToLocal = (M91GitPackIndex.StreamBackedList) null;
        return new Sha1Id?();
      }
      Stream stream = result.Stream;
      stream.Position = 0L;
      Sha1Id sha1Id = Sha1Id.FromStream(stream);
      long num = stream.Length - 20L;
      if (num % 2L != 0L)
        throw new InvalidDataException("remainLength % 2 != 0");
      int byteCount = checked ((int) unchecked (num / 2L));
      localToStable = new M91GitPackIndex.StreamBackedList(stream, 20, byteCount);
      stableToLocal = new M91GitPackIndex.StreamBackedList(stream, 20 + byteCount, byteCount);
      return new Sha1Id?(sha1Id);
    }

    public void Dispose() => this.m_riff.Dispose();

    public Sha1Id? Id { get; }

    public GitPackIndexVersion? Version { get; }

    public IReadOnlyList<Sha1Id> BaseIndexIds { get; }

    public IReadOnlyList<Sha1Id> PackIds { get; }

    public IReadOnlyList<GitPackStates> PackStates { get; }

    public IReadOnlyGitPackWatermarks PackWatermarks { get; }

    public ISha1IdTwoWayReadOnlyList ObjectIds { get; }

    public IReadOnlyList<GitPackIndexEntry> Entries { get; }

    public Sha1Id? StableObjectOrderEpoch { get; }

    public static class ChunkIds
    {
      public const uint BaseIndexIds = 538995042;
      public const uint PackIds = 538995568;
      public const uint PackStates = 538997616;
      public const uint Fanout = 538996326;
      public const uint ObjectIds = 538976367;
      public const uint Entries = 538976357;
      public const uint PackWatermarks = 538998640;
      public const uint EntryBigOffsets = 544170597;
      public const uint EntryBigLengths = 543973989;
      public const uint StableObjectOrder = 544173939;
      public const uint Id = 538993769;
    }

    private class PackStateList : VirtualReadOnlyListBase<GitPackStates>
    {
      private readonly Stream m_stream;
      private readonly Lazy<byte[]> m_packStates;

      public PackStateList(Stream stream)
      {
        this.m_stream = stream;
        this.m_packStates = new Lazy<byte[]>((Func<byte[]>) (() => RiffUtil.ReadBytes(stream, (int) ushort.MaxValue)), false);
        this.Count = checked ((int) this.m_stream.Length);
      }

      public override int Count { get; }

      protected override GitPackStates DoGet(int index) => (GitPackStates) this.m_packStates.Value[index];
    }

    private class EntryList : VirtualReadOnlyListBase<GitPackIndexEntry>
    {
      private readonly Stream m_entriesStream;
      private readonly Stream m_entryBigOffsetsStream;
      private readonly Stream m_entryBigLengthsStream;
      private readonly byte[] m_buf;
      private const int c_entryLength = 10;
      private const int c_entryBigOffsetLength = 8;
      private const int c_entryBigLengthLength = 8;

      public EntryList(
        Stream entriesStream,
        Stream entryBigOffsetsStream,
        Stream entryBigLengthsStream)
      {
        this.m_entriesStream = entriesStream;
        this.m_entryBigOffsetsStream = entryBigOffsetsStream;
        this.m_entryBigLengthsStream = entryBigLengthsStream;
        this.m_buf = new byte[10];
        this.Count = checked ((int) unchecked (this.m_entriesStream.Length / 10L));
      }

      public override int Count { get; }

      protected override GitPackIndexEntry DoGet(int index)
      {
        try
        {
          this.m_entriesStream.Seek((long) (index * 10), SeekOrigin.Begin);
          GitStreamUtil.ReadGreedy(this.m_entriesStream, this.m_buf, 0, 10);
        }
        catch (Exception ex)
        {
          throw new InvalidGitIndexException(ex);
        }
        ushort uint16 = BitConverter.ToUInt16(this.m_buf, 0);
        long uint32_1 = (long) BitConverter.ToUInt32(this.m_buf, 2);
        int num = ((ulong) uint32_1 & 2147483648UL) > 0UL ? 1 : 0;
        long offset = uint32_1 & (long) int.MaxValue;
        long uint32_2 = (long) BitConverter.ToUInt32(this.m_buf, 6);
        GitPackObjectType objectType = (GitPackObjectType) ((uint32_2 & 3758096384L) >> 29);
        bool flag = ((ulong) uint32_2 & 268435456UL) > 0UL;
        long length = uint32_2 & 268435455L;
        if (num != 0)
        {
          try
          {
            this.m_entryBigOffsetsStream.Seek(offset * 8L, SeekOrigin.Begin);
            GitStreamUtil.ReadGreedy(this.m_entryBigOffsetsStream, this.m_buf, 0, 8);
          }
          catch (Exception ex)
          {
            throw new InvalidGitIndexException(ex);
          }
          offset = BitConverter.ToInt64(this.m_buf, 0);
        }
        if (flag)
        {
          try
          {
            this.m_entryBigLengthsStream.Seek(length * 8L, SeekOrigin.Begin);
            GitStreamUtil.ReadGreedy(this.m_entryBigLengthsStream, this.m_buf, 0, 8);
          }
          catch (Exception ex)
          {
            throw new InvalidGitIndexException(ex);
          }
          length = BitConverter.ToInt64(this.m_buf, 0);
        }
        return new GitPackIndexEntry(objectType, new TfsGitObjectLocation(uint16, offset, length));
      }
    }

    private class StreamBackedList : VirtualReadOnlyListBase<int>
    {
      private readonly Stream m_stream;
      private readonly int m_byteOffset;
      private readonly byte[] m_buf;

      public StreamBackedList(Stream stream, int byteOffset, int byteCount)
      {
        this.m_stream = stream;
        this.m_byteOffset = byteOffset;
        this.m_buf = new byte[4];
        this.Count = byteCount / 4;
      }

      public override int Count { get; }

      protected override int DoGet(int index)
      {
        try
        {
          this.m_stream.Seek((long) (this.m_byteOffset + index * 4), SeekOrigin.Begin);
          GitStreamUtil.ReadGreedy(this.m_stream, this.m_buf, 0, 4);
        }
        catch (Exception ex)
        {
          throw new InvalidGitIndexException(ex);
        }
        return BitConverter.ToInt32(this.m_buf, 0);
      }
    }
  }
}
