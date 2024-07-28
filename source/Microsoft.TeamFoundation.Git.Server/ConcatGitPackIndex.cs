// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ConcatGitPackIndex
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Collections;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class ConcatGitPackIndex : IGitPackIndex, IDisposable
  {
    private readonly List<IGitPackIndex> m_subindexes;
    public static readonly ConcatGitPackIndex Empty = new ConcatGitPackIndex(new List<IGitPackIndex>(0), new List<Sha1Id>(0), new Sha1Id?());

    private ConcatGitPackIndex(
      List<IGitPackIndex> subindexes,
      List<Sha1Id> baseIndexIds,
      Sha1Id? stableObjectOrderEpoch)
    {
      this.m_subindexes = subindexes;
      this.BaseIndexIds = (IReadOnlyList<Sha1Id>) baseIndexIds;
      this.PackIds = (IReadOnlyList<Sha1Id>) new ConcatReadOnlyList<Sha1Id>(subindexes.Select<IGitPackIndex, IReadOnlyList<Sha1Id>>((Func<IGitPackIndex, IReadOnlyList<Sha1Id>>) (x => x.PackIds)));
      this.PackStates = (IReadOnlyList<GitPackStates>) new ConcatReadOnlyList<GitPackStates>(subindexes.Select<IGitPackIndex, IReadOnlyList<GitPackStates>>((Func<IGitPackIndex, IReadOnlyList<GitPackStates>>) (x => x.PackStates)));
      int[] packIntIdOffsets = new int[subindexes.Count];
      int num = 0;
      for (int index = 0; index < packIntIdOffsets.Length; ++index)
      {
        packIntIdOffsets[index] = num;
        num += subindexes[index].PackIds.Count;
      }
      this.PackWatermarks = (IReadOnlyGitPackWatermarks) ConcatGitPackIndex.ConcatWatermarks(subindexes.Select<IGitPackIndex, IReadOnlyGitPackWatermarks>((Func<IGitPackIndex, IReadOnlyGitPackWatermarks>) (x => x.PackWatermarks)), packIntIdOffsets);
      this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new ConcatSha1IdTwoWayReadOnlyList(subindexes.Select<IGitPackIndex, ISha1IdTwoWayReadOnlyList>((Func<IGitPackIndex, ISha1IdTwoWayReadOnlyList>) (x => x.ObjectIds)));
      this.Entries = (IReadOnlyList<GitPackIndexEntry>) new ConcatGitPackIndex.EntryList(subindexes.Select<IGitPackIndex, IReadOnlyList<GitPackIndexEntry>>((Func<IGitPackIndex, IReadOnlyList<GitPackIndexEntry>>) (x => x.Entries)), packIntIdOffsets);
      this.StableObjectOrderEpoch = stableObjectOrderEpoch;
      this.CheckCounts();
    }

    private static GitPackWatermarks ConcatWatermarks(
      IEnumerable<IReadOnlyGitPackWatermarks> subwatermarks,
      int[] packIntIdOffsets)
    {
      GitPackWatermarks gitPackWatermarks = new GitPackWatermarks();
      int index = 0;
      foreach (IReadOnlyGitPackWatermarks subwatermark in subwatermarks)
      {
        foreach (KeyValuePair<GitPackWatermark, ushort> keyValuePair in subwatermark.NonZero)
        {
          ushort num1 = gitPackWatermarks[keyValuePair.Key];
          int packIntIdOffset = packIntIdOffsets[index];
          if ((int) num1 != packIntIdOffset)
            throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Watermark gap before {0}[{1}][{2}], {3}: {4}, {5}: {6}", (object) nameof (subwatermarks), (object) index, (object) keyValuePair.Key, (object) "prevValue", (object) num1, (object) "numPacksInEarlierSubindexes", (object) packIntIdOffset))));
          ushort num2;
          try
          {
            num2 = checked ((ushort) ((int) num1 + (int) keyValuePair.Value));
          }
          catch (OverflowException ex)
          {
            throw new InvalidGitIndexException(FormattableString.Invariant(FormattableStringFactory.Create("Overflow for the index hardlimit watermark in {0}: {1}, {2}[{3}][{4}]: {5}", (object) ushort.MaxValue, (object) num1, (object) nameof (subwatermarks), (object) index, (object) keyValuePair.Key, (object) keyValuePair.Value)));
          }
          gitPackWatermarks.Inner[keyValuePair.Key] = num2;
        }
        ++index;
      }
      return gitPackWatermarks;
    }

    private void CheckCounts()
    {
      if (this.PackIds.Count > (int) ushort.MaxValue)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} > {2}. {3}.{4} = {5}", (object) "PackIds", (object) "Count", (object) ushort.MaxValue, (object) "PackIds", (object) "Count", (object) this.PackIds.Count))));
      foreach (KeyValuePair<GitPackWatermark, ushort> keyValuePair in this.PackWatermarks.NonZero)
      {
        if ((int) keyValuePair.Value > this.PackIds.Count)
          throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("{0}[{1}]: {2} > {3}.{4}: {5}", (object) "PackWatermarks", (object) keyValuePair.Key, (object) keyValuePair.Value, (object) "PackIds", (object) "Count", (object) this.PackIds.Count))));
      }
      if (this.ObjectIds.Count != this.Entries.Count)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} != {2}.{3}", (object) "ObjectIds", (object) "Count", (object) "Entries", (object) "Count"))));
    }

    public static ConcatGitPackIndex CreateFull(IEnumerable<IGitPackIndex> subindexes)
    {
      ArgumentUtility.CheckForNull<IEnumerable<IGitPackIndex>>(subindexes, nameof (subindexes));
      Sha1Id? stableObjectOrderEpoch = ConcatGitPackIndex.ConcatStableOrderEpochs(subindexes.Select<IGitPackIndex, Sha1Id?>((Func<IGitPackIndex, Sha1Id?>) (x => x.StableObjectOrderEpoch)));
      return new ConcatGitPackIndex(subindexes.ToList<IGitPackIndex>(), new List<Sha1Id>(0), stableObjectOrderEpoch);
    }

    private static Sha1Id? ConcatStableOrderEpochs(IEnumerable<Sha1Id?> epochs)
    {
      using (IEnumerator<Sha1Id?> enumerator = epochs.GetEnumerator())
      {
        Sha1Id? nullable1 = enumerator.MoveNext() ? enumerator.Current : new Sha1Id?();
        int num = 1;
        Sha1Id? nullable2;
        while (enumerator.MoveNext())
        {
          Sha1Id? nullable3 = nullable1;
          nullable2 = enumerator.Current;
          if ((nullable3.HasValue == nullable2.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("expected: {0} != {1}[{2}]: {3}", (object) nullable1, (object) nameof (epochs), (object) num, (object) enumerator.Current))));
          ++num;
        }
        nullable2 = nullable1;
        return nullable2;
      }
    }

    public ConcatGitPackIndex GetRange(int index, int count)
    {
      ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, this.m_subindexes.Count - index);
      List<Sha1Id> baseIndexIds = new List<Sha1Id>(index);
      for (int index1 = 0; index1 < index; ++index1)
      {
        Sha1Id? id = this.m_subindexes[index1].Id;
        if (!id.HasValue)
          ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0, index1 - 1);
        baseIndexIds.Add(id.Value);
      }
      return new ConcatGitPackIndex(this.m_subindexes.GetRange(index, count), baseIndexIds, this.StableObjectOrderEpoch);
    }

    public ConcatGitPackIndex GetRangeNotIn(ConcatGitPackIndex other)
    {
      ArgumentUtility.CheckForNull<ConcatGitPackIndex>(other, nameof (other));
      int index;
      for (index = 0; index < this.m_subindexes.Count; ++index)
      {
        Sha1Id? subId = this.m_subindexes[index].Id;
        if (!subId.HasValue || !other.Subindexes.Any<IGitPackIndex>((Func<IGitPackIndex, bool>) (o =>
        {
          Sha1Id? id = o.Id;
          Sha1Id? nullable = subId;
          if (id.HasValue != nullable.HasValue)
            return false;
          return !id.HasValue || id.GetValueOrDefault() == nullable.GetValueOrDefault();
        })))
          break;
      }
      return this.GetRange(index, this.m_subindexes.Count - index);
    }

    public Sha1Id? GetRealTipSubindexId(bool allowEmpty)
    {
      if (this.m_subindexes.Count == 0)
      {
        if (!allowEmpty)
          throw new InvalidOperationException("Unexpected empty index");
        return new Sha1Id?();
      }
      return this.m_subindexes.Last<IGitPackIndex>().Id ?? throw new InvalidOperationException("Unexpected virtual tip subindex");
    }

    public IReadOnlyList<IGitPackIndex> Subindexes => (IReadOnlyList<IGitPackIndex>) this.m_subindexes;

    public Sha1Id? Id { get; }

    public GitPackIndexVersion? Version { get; }

    public IReadOnlyList<Sha1Id> BaseIndexIds { get; }

    public IReadOnlyList<Sha1Id> PackIds { get; }

    public IReadOnlyList<GitPackStates> PackStates { get; }

    public IReadOnlyGitPackWatermarks PackWatermarks { get; }

    public ISha1IdTwoWayReadOnlyList ObjectIds { get; }

    public IReadOnlyList<GitPackIndexEntry> Entries { get; }

    public Sha1Id? StableObjectOrderEpoch { get; }

    public void Dispose()
    {
      foreach (IDisposable subindex in this.m_subindexes)
        subindex.Dispose();
    }

    private class EntryList : ConcatReadOnlyList<GitPackIndexEntry>
    {
      private readonly int[] m_packIntIdOffsets;

      public EntryList(
        IEnumerable<IReadOnlyList<GitPackIndexEntry>> sublists,
        int[] packIntIdOffsets)
        : base(sublists)
      {
        this.m_packIntIdOffsets = packIntIdOffsets;
      }

      protected override GitPackIndexEntry FromRawValue(GitPackIndexEntry rawValue, int iSublist) => new GitPackIndexEntry(rawValue.ObjectType, new TfsGitObjectLocation((ushort) ((uint) rawValue.Location.PackIntId + (uint) this.m_packIntIdOffsets[iSublist]), rawValue.Location.Offset, rawValue.Location.Length));
    }
  }
}
