// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.M43GitPackIndex
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal sealed class M43GitPackIndex : IGitPackIndex, IDisposable
  {
    private readonly Stream m_stream;
    private readonly byte[] m_buf;
    private const byte c_largeFileFlag = 128;
    private static readonly byte[] s_indexHeader = new byte[3]
    {
      (byte) 84,
      (byte) 70,
      (byte) 83
    };

    public M43GitPackIndex(
      Stream stream,
      Sha1Id? id,
      Lazy<IReadOnlyDictionary<string, KnownFile>> knownFilesFromProvider)
    {
      this.m_stream = stream;
      this.m_buf = new byte[20];
      this.ReadHeader(stream);
      this.PackIds = (IReadOnlyList<Sha1Id>) this.ReadPackIds(stream);
      GitPackIndexVersion? version = this.Version;
      if (version.HasValue)
      {
        switch (version.GetValueOrDefault())
        {
          case GitPackIndexVersion.M43:
          case GitPackIndexVersion.M88:
            Dictionary<string, M43GitPackIndex.TypeAndExpiration> dictionary = new Dictionary<string, M43GitPackIndex.TypeAndExpiration>();
            foreach (Sha1Id packId in (IEnumerable<Sha1Id>) this.PackIds)
            {
              string key = packId.ToString() + ".pack";
              dictionary[key] = new M43GitPackIndex.TypeAndExpiration(KnownFileType.RawPackfile, DateTime.MaxValue);
            }
            this.KnownFiles = (IReadOnlyDictionary<string, M43GitPackIndex.TypeAndExpiration>) dictionary;
            this.PackStates = (IReadOnlyList<GitPackStates>) new M43GitPackIndex.PackStateList<KnownFile>(this.PackIds, knownFilesFromProvider, (Func<KnownFile, KnownFileType>) (x => x.Type));
            break;
          case GitPackIndexVersion.M44:
            this.KnownFiles = (IReadOnlyDictionary<string, M43GitPackIndex.TypeAndExpiration>) this.ReadKnownFiles(stream);
            this.PackStates = (IReadOnlyList<GitPackStates>) new M43GitPackIndex.PackStateList<M43GitPackIndex.TypeAndExpiration>(this.PackIds, new Lazy<IReadOnlyDictionary<string, M43GitPackIndex.TypeAndExpiration>>((Func<IReadOnlyDictionary<string, M43GitPackIndex.TypeAndExpiration>>) (() => this.KnownFiles), false), (Func<M43GitPackIndex.TypeAndExpiration, KnownFileType>) (x => x.Type));
            break;
          default:
            goto label_11;
        }
        FanoutTable fanout = FanoutTable.FromStream(stream);
        long position = stream.Position;
        long length1 = (long) (fanout.EntryCount * 20U);
        this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new Sha1IdSortedVirtualList((Stream) new GitRestrictedStream(stream, position, length1, true), fanout);
        if ((long) this.ObjectIds.Count != (long) fanout.EntryCount)
          throw new InvalidGitIndexException();
        long offset = position + length1;
        int num = this.HasLargePackfiles ? 19 : 11;
        long length2 = (long) fanout.EntryCount * (long) num;
        this.Entries = (IReadOnlyList<GitPackIndexEntry>) new M43GitPackIndex.EntryList((Stream) new GitRestrictedStream(stream, offset, length2, true), this.Version.Value, this.PackIds.Count, this.HasLargePackfiles);
        if ((long) this.Entries.Count != (long) fanout.EntryCount)
          throw new InvalidGitIndexException();
        this.Id = id;
        return;
      }
label_11:
      throw new ArgumentOutOfRangeException(nameof (Version), (object) this.Version, string.Format("{0} = {1}", (object) nameof (Version), (object) this.Version));
    }

    public void Dispose() => this.m_stream.Dispose();

    private void ReadHeader(Stream stream)
    {
      if (GitStreamUtil.TryReadGreedy(stream, this.m_buf, 0, 3) < 3 || !GitUtils.CompareByteArrays(this.m_buf, 0, M43GitPackIndex.s_indexHeader, 0, M43GitPackIndex.s_indexHeader.Length))
        throw new InvalidGitIndexException();
      int num1 = stream.ReadByte();
      if (num1 < 0)
        throw new InvalidGitIndexException();
      if ((num1 & 128) != 0)
        this.HasLargePackfiles = true;
      byte num2 = (byte) (num1 & -129);
      this.Version = Enum.IsDefined(typeof (GitPackIndexVersion), (object) num2) ? new GitPackIndexVersion?((GitPackIndexVersion) num2) : throw new InvalidGitIndexException();
    }

    private Sha1Id[] ReadPackIds(Stream stream)
    {
      ushort length = GitStreamUtil.TryReadGreedy(stream, this.m_buf, 0, 2) >= 2 ? BitConverter.ToUInt16(this.m_buf, 0) : throw new InvalidGitIndexException();
      Sha1Id[] sha1IdArray = new Sha1Id[(int) length];
      for (int index = 0; index < (int) length; ++index)
      {
        try
        {
          sha1IdArray[index] = Sha1Id.FromStream(stream);
        }
        catch (Sha1IdStreamReadException ex)
        {
          throw new InvalidGitIndexException((Exception) ex);
        }
      }
      GitPackIndexVersion? version = this.Version;
      GitPackIndexVersion packIndexVersion = GitPackIndexVersion.M44;
      if (version.GetValueOrDefault() < packIndexVersion & version.HasValue)
        Array.Reverse((Array) sha1IdArray);
      return sha1IdArray;
    }

    private Dictionary<string, M43GitPackIndex.TypeAndExpiration> ReadKnownFiles(Stream stream)
    {
      int capacity = GitStreamUtil.TryReadGreedy(stream, this.m_buf, 0, 4) >= 4 ? BitConverter.ToInt32(this.m_buf, 0) : throw new InvalidGitIndexException();
      if (capacity < 1)
        throw new InvalidGitIndexException();
      int length = GitStreamUtil.TryReadGreedy(stream, this.m_buf, 0, 4) >= 4 ? BitConverter.ToInt32(this.m_buf, 0) : throw new InvalidGitIndexException();
      if (length < 1)
        throw new InvalidGitIndexException();
      List<string> stringList = new List<string>(capacity);
      using (RestrictedStream restrictedStream = new RestrictedStream(stream, 0L, (long) length, true))
      {
        using (StreamReader streamReader = new StreamReader((Stream) restrictedStream, GitEncodingUtil.SafeUtf8NoBom))
        {
          for (int index = 0; index < capacity; ++index)
            stringList.Add(streamReader.ReadLine());
          if (restrictedStream.Position != restrictedStream.Length)
            throw new InvalidGitIndexException();
        }
      }
      Dictionary<string, M43GitPackIndex.TypeAndExpiration> dictionary = new Dictionary<string, M43GitPackIndex.TypeAndExpiration>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        DateTime expirationDate = GitStreamUtil.TryReadGreedy(stream, this.m_buf, 0, 9) >= 9 ? DateTime.FromBinary(BitConverter.ToInt64(this.m_buf, 0)) : throw new InvalidGitIndexException();
        KnownFileType type = (KnownFileType) this.m_buf[8];
        dictionary[stringList[index]] = new M43GitPackIndex.TypeAndExpiration(type, expirationDate);
      }
      return dictionary;
    }

    public Sha1Id? Id { get; }

    public GitPackIndexVersion? Version { get; private set; }

    public IReadOnlyList<Sha1Id> BaseIndexIds { get; } = (IReadOnlyList<Sha1Id>) Array.Empty<Sha1Id>();

    public IReadOnlyList<Sha1Id> PackIds { get; }

    public IReadOnlyList<GitPackStates> PackStates { get; }

    public IReadOnlyGitPackWatermarks PackWatermarks { get; } = GitPackWatermarks.Empty;

    public ISha1IdTwoWayReadOnlyList ObjectIds { get; }

    public IReadOnlyList<GitPackIndexEntry> Entries { get; }

    public Sha1Id? StableObjectOrderEpoch { get; }

    public IReadOnlyDictionary<string, M43GitPackIndex.TypeAndExpiration> KnownFiles { get; }

    internal static byte[] CurrentIndexHeader => M43GitPackIndex.s_indexHeader;

    internal static byte LargeFileFlag => 128;

    internal bool HasLargePackfiles { get; private set; }

    public struct TypeAndExpiration
    {
      public readonly KnownFileType Type;
      public readonly DateTime ExpirationDate;

      public TypeAndExpiration(KnownFileType type, DateTime expirationDate)
      {
        this.Type = type;
        this.ExpirationDate = expirationDate;
      }
    }

    private class PackStateList<TKnown> : VirtualReadOnlyListBase<GitPackStates>
    {
      private readonly IReadOnlyList<Sha1Id> m_packIds;
      private readonly Lazy<IReadOnlyDictionary<string, TKnown>> m_knownFiles;
      private readonly Func<TKnown, KnownFileType> m_toType;
      private readonly Lazy<int> m_lastRepackedPackIntId;

      public PackStateList(
        IReadOnlyList<Sha1Id> packIds,
        Lazy<IReadOnlyDictionary<string, TKnown>> knownFiles,
        Func<TKnown, KnownFileType> toType)
      {
        this.m_packIds = packIds;
        this.m_knownFiles = knownFiles;
        this.m_toType = toType;
        this.m_lastRepackedPackIntId = new Lazy<int>((Func<int>) (() =>
        {
          int index = this.m_packIds.Count - 1;
          TKnown known;
          while (index >= 0 && (!this.m_knownFiles.Value.TryGetValue(StorageUtils.GetPackFileName(this.m_packIds[index]), out known) || this.m_toType(known) == KnownFileType.RawPackfile))
            --index;
          return index;
        }), false);
        this.Count = this.m_packIds.Count;
      }

      public override int Count { get; }

      protected override GitPackStates DoGet(int index) => index > this.m_lastRepackedPackIntId.Value ? GitPackStates.None : GitPackStates.Derived;
    }

    private class EntryList : VirtualReadOnlyListBase<GitPackIndexEntry>
    {
      private readonly Stream m_stream;
      private readonly GitPackIndexVersion m_version;
      private readonly int m_packIdCount;
      private readonly bool m_large;
      private readonly byte[] m_buf;
      public const int ShortEntrySize = 11;
      public const int LongEntrySize = 19;

      public EntryList(Stream stream, GitPackIndexVersion version, int packIdCount, bool large)
      {
        this.m_stream = stream;
        this.m_version = version;
        this.m_packIdCount = packIdCount;
        this.m_large = large;
        this.Count = checked ((int) unchecked (this.m_stream.Length / large ? 19L : 11L));
        this.m_buf = large ? new byte[19] : new byte[11];
      }

      public override int Count { get; }

      protected override GitPackIndexEntry DoGet(int index)
      {
        int count = this.m_large ? 19 : 11;
        long offset1 = (long) (index * count);
        try
        {
          this.m_stream.Seek(offset1, SeekOrigin.Begin);
          GitStreamUtil.ReadGreedy(this.m_stream, this.m_buf, 0, count);
        }
        catch (Exception ex)
        {
          throw new InvalidGitIndexException(ex);
        }
        ushort packIntId = BitConverter.ToUInt16(this.m_buf, 0);
        int objectType = (int) this.m_buf[2];
        long offset2;
        long length;
        if (this.m_large)
        {
          offset2 = BitConverter.ToInt64(this.m_buf, 3);
          length = BitConverter.ToInt64(this.m_buf, 11);
        }
        else
        {
          offset2 = (long) BitConverter.ToUInt32(this.m_buf, 3);
          length = (long) BitConverter.ToUInt32(this.m_buf, 7);
        }
        if (this.m_version < GitPackIndexVersion.M44)
          packIntId = checked ((ushort) (this.m_packIdCount - (int) packIntId - 1));
        TfsGitObjectLocation location = new TfsGitObjectLocation(packIntId, offset2, length);
        return new GitPackIndexEntry((GitPackObjectType) objectType, location);
      }
    }
  }
}
