// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackIndexer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPackIndexer
  {
    private readonly List<GitPackIndexer.Pack> m_packs;
    private readonly Dictionary<Sha1Id, int> m_packIdToIntIds;
    private GitPackWatermarks m_packWatermarks;
    private readonly List<GitPackIndexer.MutableIndexEntry> m_entries;
    private readonly IDictionary<Sha1Id, int> m_objectIdToIntIds;
    private Sha1Id? m_stableObjectOrderEpoch;
    private IReadOnlyList<Sha1Id> m_stableObjectOrder;
    private bool m_isStableObjectOrderExact;
    private ConcatGitPackIndex m_baseIndex;
    private bool m_hasPendingPack;
    private Sha1Id? m_pendingPackId;
    private GitPackStates m_pendingPackState;
    private ConcatGitPackIndex m_index;

    public GitPackIndexer()
    {
      this.m_packs = new List<GitPackIndexer.Pack>();
      this.m_packIdToIntIds = new Dictionary<Sha1Id, int>();
      this.m_packWatermarks = new GitPackWatermarks();
      this.m_entries = new List<GitPackIndexer.MutableIndexEntry>();
      this.m_objectIdToIntIds = (IDictionary<Sha1Id, int>) new ShardedDictionary<Sha1Id, int>();
    }

    public void BeginPackFile(Sha1Id? packId, GitPackStates packState)
    {
      this.CheckToIndexNotCalled();
      this.CheckNoPendingPackId();
      if (this.m_packs.Count >= (int) ushort.MaxValue)
        throw new GitPackIndexer.MaxPackFileCountException();
      if (packId.HasValue)
        this.CheckPackIdIsNew(packId.Value);
      this.m_pendingPackId = packId;
      this.m_pendingPackState = packState;
      this.m_hasPendingPack = true;
    }

    public void EndPackFile(Sha1Id packId)
    {
      this.CheckHasPendingPackId();
      if (this.m_pendingPackId.HasValue)
      {
        Sha1Id? pendingPackId = this.m_pendingPackId;
        Sha1Id sha1Id = packId;
        if ((pendingPackId.HasValue ? (pendingPackId.HasValue ? (pendingPackId.GetValueOrDefault() != sha1Id ? 1 : 0) : 0) : 1) != 0)
          throw new ArgumentException(string.Format("{0} was called with {1}, which doesn't match {2}.", (object) "BeginPackFile", (object) this.m_pendingPackId, (object) packId), nameof (packId));
      }
      else
        this.CheckPackIdIsNew(packId);
      this.DoAddPackFile(packId, this.m_pendingPackState);
      this.m_hasPendingPack = false;
    }

    private void DoAddPackFile(Sha1Id packId, GitPackStates packState)
    {
      this.m_packIdToIntIds.Add(packId, this.m_packs.Count);
      this.m_packs.Add(new GitPackIndexer.Pack(packId, packState));
    }

    public void AddObject(Sha1Id objectId, GitPackObjectType objectType, long offset, long length)
    {
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0L);
      ArgumentUtility.CheckForOutOfRange(length, nameof (length), 0L);
      this.CheckHasPendingPackId();
      this.DoAddObject(objectId, objectType, (ushort) this.m_packs.Count, offset, length);
    }

    public void AddFromIndex(ConcatGitPackIndex other, Predicate<Sha1Id> shouldAddObject = null)
    {
      ArgumentUtility.CheckForNull<ConcatGitPackIndex>(other, nameof (other));
      this.CheckNoPendingPackId();
      this.CheckBaseIndexIsSet();
      this.CheckToIndexNotCalled();
      this.DoAddFromSubindexRange((IGitPackIndex) other.GetRangeNotIn(this.m_baseIndex), shouldAddObject);
    }

    private void DoAddFromSubindexRange(IGitPackIndex otherSub, Predicate<Sha1Id> includeObject)
    {
      int count = this.m_packs.Count;
      int[] numArray = new int[otherSub.PackIds.Count];
      int index = 0;
      using (IEnumerator<Sha1Id> enumerator1 = otherSub.PackIds.GetEnumerator())
      {
        using (IEnumerator<GitPackStates> enumerator2 = otherSub.PackStates.GetEnumerator())
        {
          while (enumerator1.MoveNext())
          {
            if (enumerator2.MoveNext())
            {
              Sha1Id current1 = enumerator1.Current;
              GitPackStates current2 = enumerator2.Current;
              if (!this.m_packIdToIntIds.TryGetValue(current1, out numArray[index]))
              {
                numArray[index] = this.m_packs.Count != (int) ushort.MaxValue ? this.m_packs.Count : throw new GitPackIndexer.MaxPackFileCountException();
                this.DoAddPackFile(current1, current2);
              }
              ++index;
            }
            else
              break;
          }
        }
      }
      using (IEnumerator<Sha1Id> enumerator3 = otherSub.ObjectIds.GetEnumerator())
      {
        using (IEnumerator<GitPackIndexEntry> enumerator4 = otherSub.Entries.GetEnumerator())
        {
          while (enumerator3.MoveNext() && enumerator4.MoveNext())
          {
            Sha1Id current3 = enumerator3.Current;
            GitPackIndexEntry current4 = enumerator4.Current;
            ushort num = (ushort) numArray[(int) current4.Location.PackIntId];
            if ((int) num < count)
            {
              if (!this.m_objectIdToIntIds.ContainsKey(current3))
                throw new InvalidOperationException(string.Format("New object {0} in a pack file {1} previously added to this indexer.", (object) current3, (object) this.m_packs[(int) num].Id));
            }
            else if (includeObject == null || includeObject(current3))
              this.DoAddObject(current3, current4.ObjectType, num, current4.Location.Offset, current4.Location.Length);
          }
        }
      }
    }

    private void DoAddObject(
      Sha1Id objectId,
      GitPackObjectType objectType,
      ushort packIntId,
      long offset,
      long length)
    {
      int index;
      if (this.m_baseIndex != null && this.m_baseIndex.ObjectIds.TryGetIndex(objectId, out index))
        return;
      if (this.m_objectIdToIntIds.TryGetValue(objectId, out index))
      {
        GitPackIndexer.MutableIndexEntry entry = this.m_entries[index];
        if ((int) packIntId >= (int) entry.PackIntId && ((int) packIntId != (int) entry.PackIntId || offset >= entry.Offset))
          return;
        entry.PackIntId = packIntId;
        entry.ObjectType = objectType;
        entry.Offset = offset;
        entry.Length = length;
      }
      else
      {
        this.m_objectIdToIntIds.Add(objectId, this.m_entries.Count);
        this.m_entries.Add(new GitPackIndexer.MutableIndexEntry(objectId, objectType, packIntId, offset, length));
      }
    }

    public void SetWatermark(GitPackWatermark watermark)
    {
      this.CheckToIndexNotCalled();
      this.CheckNoPendingPackId();
      this.m_packWatermarks.Inner[watermark] = checked ((ushort) this.m_packs.Count);
    }

    public ConcatGitPackIndex ToIndex()
    {
      this.CheckBaseIndexIsSet();
      this.CheckNoPendingPackId();
      if (this.m_index == null)
      {
        bool[] flagArray = new bool[this.m_packs.Count];
        int index1 = -1;
        for (int index2 = 0; index2 < this.m_entries.Count; ++index2)
        {
          GitPackIndexer.MutableIndexEntry entry = this.m_entries[index2];
          if (this.m_baseIndex.ObjectIds.TryGetIndex(entry.ObjectId, out int _))
          {
            this.m_objectIdToIntIds.Remove(entry.ObjectId);
          }
          else
          {
            flagArray[(int) entry.PackIntId] = true;
            ++index1;
            if (index1 != index2)
            {
              this.m_entries[index1] = entry;
              this.m_objectIdToIntIds[entry.ObjectId] = index1;
            }
          }
        }
        this.m_entries.RemoveRange(index1 + 1, this.m_entries.Count - (index1 + 1));
        if (this.m_stableObjectOrderEpoch.HasValue && this.m_stableObjectOrder != null && this.m_stableObjectOrder.Count != 0)
        {
          if (!this.m_isStableObjectOrderExact)
            ((List<Sha1Id>) this.m_stableObjectOrder).RemoveAll((Predicate<Sha1Id>) (x => !this.m_objectIdToIntIds.ContainsKey(x)));
          this.m_objectIdToIntIds.Clear();
          int num = 0;
          foreach (Sha1Id key in (IEnumerable<Sha1Id>) this.m_stableObjectOrder)
            this.m_objectIdToIntIds.Add(key, num++);
          foreach (GitPackIndexer.MutableIndexEntry entry in this.m_entries)
          {
            if (!this.m_objectIdToIntIds.ContainsKey(entry.ObjectId))
            {
              if (this.m_isStableObjectOrderExact)
                throw new InvalidOperationException("There are missing objects in the stable order");
              this.m_objectIdToIntIds.Add(entry.ObjectId, num++);
            }
          }
          this.m_entries.Sort((Comparison<GitPackIndexer.MutableIndexEntry>) ((x, y) => this.m_objectIdToIntIds[x.ObjectId].CompareTo(this.m_objectIdToIntIds[y.ObjectId])));
        }
        ushort[] numArray = new ushort[this.m_packs.Count];
        ushort index3 = 0;
        for (int index4 = 0; index4 < numArray.Length; ++index4)
        {
          if (flagArray[index4])
          {
            if ((int) index3 != index4)
              this.m_packs[(int) index3] = this.m_packs[index4];
            numArray[index4] = index3++;
          }
          else
            numArray[index4] = (ushort) ((uint) index3 - 1U);
        }
        this.m_packs.RemoveRange((int) index3, this.m_packs.Count - (int) index3);
        if ((int) index3 != flagArray.Length)
        {
          GitPackWatermarks gitPackWatermarks = new GitPackWatermarks();
          foreach (KeyValuePair<GitPackWatermark, ushort> keyValuePair in this.m_packWatermarks.NonZero)
          {
            int index5 = (int) keyValuePair.Value - 1;
            int num = (int) numArray[index5];
            gitPackWatermarks.Inner[keyValuePair.Key] = (ushort) (num + 1);
          }
          this.m_packWatermarks = gitPackWatermarks;
          foreach (GitPackIndexer.MutableIndexEntry entry in this.m_entries)
            entry.PackIntId = numArray[(int) entry.PackIntId];
        }
        this.m_index = ConcatGitPackIndex.CreateFull(this.m_baseIndex.Subindexes.Concat<IGitPackIndex>((IEnumerable<IGitPackIndex>) new GitPackIndexer.NewIndex[1]
        {
          new GitPackIndexer.NewIndex((IReadOnlyList<Sha1Id>) this.m_baseIndex.Subindexes.Select<IGitPackIndex, Sha1Id>((Func<IGitPackIndex, Sha1Id>) (x => x.Id.Value)).ToArray<Sha1Id>(), (IReadOnlyList<GitPackIndexer.Pack>) this.m_packs, (IReadOnlyGitPackWatermarks) this.m_packWatermarks, (IReadOnlyList<GitPackIndexer.MutableIndexEntry>) this.m_entries, this.m_objectIdToIntIds, this.m_stableObjectOrderEpoch)
        }));
      }
      return this.m_index;
    }

    public void SetBaseIndex(ConcatGitPackIndex baseIndex)
    {
      ArgumentUtility.CheckForNull<ConcatGitPackIndex>(baseIndex, nameof (baseIndex));
      if (this.m_baseIndex != null)
        throw new InvalidOperationException("Cannot be set multiple times.");
      foreach (Sha1Id packId in (IEnumerable<Sha1Id>) baseIndex.PackIds)
        this.CheckPackIdIsNew(packId);
      this.m_baseIndex = baseIndex;
      this.m_stableObjectOrderEpoch = baseIndex.StableObjectOrderEpoch;
    }

    public void PreserveStableObjectOrderIfCompatible(IGitPackIndex otherFullIndex)
    {
      this.CheckBaseIndexIsSet();
      this.CheckToIndexNotCalled();
      ArgumentUtility.CheckForNull<IGitPackIndex>(otherFullIndex, nameof (otherFullIndex));
      if (!otherFullIndex.StableObjectOrderEpoch.HasValue)
        return;
      Sha1Id? objectOrderEpoch1 = this.m_stableObjectOrderEpoch;
      Sha1Id? objectOrderEpoch2 = otherFullIndex.StableObjectOrderEpoch;
      if ((objectOrderEpoch1.HasValue == objectOrderEpoch2.HasValue ? (objectOrderEpoch1.HasValue ? (objectOrderEpoch1.GetValueOrDefault() != objectOrderEpoch2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        return;
      if (this.m_baseIndex.ObjectIds.Count == 0)
      {
        this.DoSetStableObjectOrder((IReadOnlyList<Sha1Id>) otherFullIndex.ObjectIds, true);
      }
      else
      {
        List<Sha1Id> order = new List<Sha1Id>(otherFullIndex.ObjectIds.Count - this.m_baseIndex.ObjectIds.Count);
        for (int count = this.m_baseIndex.ObjectIds.Count; count < otherFullIndex.ObjectIds.Count; ++count)
          order.Add(otherFullIndex.ObjectIds[count]);
        this.DoSetStableObjectOrder((IReadOnlyList<Sha1Id>) order, true);
      }
    }

    public void StartStableObjectOrderEpoch(Sha1Id epoch)
    {
      this.CheckBaseIndexIsSet();
      this.CheckToIndexNotCalled();
      if (this.m_baseIndex.Subindexes.Count != 0)
        throw new InvalidOperationException("Base index not empty");
      this.m_stableObjectOrderEpoch = !this.m_stableObjectOrderEpoch.HasValue ? new Sha1Id?(epoch) : throw new InvalidOperationException("Epoch already set");
    }

    public void SetStableObjectOrder(List<Sha1Id> order)
    {
      this.CheckBaseIndexIsSet();
      this.CheckToIndexNotCalled();
      ArgumentUtility.CheckForNull<List<Sha1Id>>(order, nameof (order));
      if (!this.m_stableObjectOrderEpoch.HasValue)
        throw new InvalidOperationException("Epoch not set");
      this.DoSetStableObjectOrder((IReadOnlyList<Sha1Id>) order, false);
    }

    private void DoSetStableObjectOrder(IReadOnlyList<Sha1Id> order, bool isExact)
    {
      this.m_stableObjectOrder = this.m_stableObjectOrder == null ? order : throw new InvalidOperationException("Stable object order was already set");
      this.m_isStableObjectOrderExact = isExact;
    }

    private void CheckBaseIndexIsSet()
    {
      if (this.m_baseIndex == null)
        throw new InvalidOperationException("Not allowed before SetBaseIndex.");
    }

    private void CheckToIndexNotCalled()
    {
      if (this.m_index != null)
        throw new InvalidOperationException("Not allowed after ToIndex().");
    }

    private void CheckHasPendingPackId()
    {
      if (!this.m_hasPendingPack)
        throw new InvalidOperationException("Not allowed before BeginPackFile().");
    }

    private void CheckNoPendingPackId()
    {
      if (this.m_hasPendingPack)
        throw new InvalidOperationException("Not allowed after BeginPackFile until EndPackFile() is called.");
    }

    private void CheckPackIdIsNew(Sha1Id packId)
    {
      if (this.m_packIdToIntIds.ContainsKey(packId))
        throw new InvalidOperationException(string.Format("Pack file {0} was already added.", (object) packId));
    }

    private struct Pack
    {
      public readonly Sha1Id Id;
      public readonly GitPackStates State;

      public Pack(Sha1Id id, GitPackStates state)
      {
        this.Id = id;
        this.State = state;
      }
    }

    private class MutableIndexEntry
    {
      public readonly Sha1Id ObjectId;
      public GitPackObjectType ObjectType;
      public ushort PackIntId;
      public long Offset;
      public long Length;

      public MutableIndexEntry(
        Sha1Id objectId,
        GitPackObjectType objectType,
        ushort packIntId,
        long offset,
        long length)
      {
        this.ObjectId = objectId;
        this.ObjectType = objectType;
        this.PackIntId = packIntId;
        this.Offset = offset;
        this.Length = length;
      }

      public MutableIndexEntry(Sha1Id objectId, GitPackIndexEntry entry)
        : this(objectId, entry.ObjectType, entry.Location.PackIntId, entry.Location.Offset, entry.Location.Length)
      {
      }
    }

    private class NewIndex : IGitPackIndex, IDisposable
    {
      public NewIndex(
        IReadOnlyList<Sha1Id> baseIndexIds,
        IReadOnlyList<GitPackIndexer.Pack> packs,
        IReadOnlyGitPackWatermarks packWatermarks,
        IReadOnlyList<GitPackIndexer.MutableIndexEntry> entries,
        IDictionary<Sha1Id, int> objectIdToIntIds,
        Sha1Id? stableObjectOrderEpoch)
      {
        this.BaseIndexIds = baseIndexIds;
        this.PackIds = (IReadOnlyList<Sha1Id>) new GitPackIndexer.NewIndex.PackIdList(packs);
        this.PackStates = (IReadOnlyList<GitPackStates>) new GitPackIndexer.NewIndex.PackStateList(packs);
        this.PackWatermarks = packWatermarks;
        this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new GitPackIndexer.NewIndex.ObjectIdList(entries, objectIdToIntIds);
        this.Entries = (IReadOnlyList<GitPackIndexEntry>) new GitPackIndexer.NewIndex.EntryList(entries);
        this.StableObjectOrderEpoch = stableObjectOrderEpoch;
      }

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
      }

      private class PackIdList : VirtualReadOnlyListBase<Sha1Id>
      {
        private readonly IReadOnlyList<GitPackIndexer.Pack> m_packs;

        public PackIdList(IReadOnlyList<GitPackIndexer.Pack> packs)
        {
          this.m_packs = packs;
          this.Count = packs.Count;
        }

        public override int Count { get; }

        protected override Sha1Id DoGet(int index) => this.m_packs[index].Id;
      }

      private class PackStateList : VirtualReadOnlyListBase<GitPackStates>
      {
        private readonly IReadOnlyList<GitPackIndexer.Pack> m_packs;

        public PackStateList(IReadOnlyList<GitPackIndexer.Pack> packs)
        {
          this.m_packs = packs;
          this.Count = packs.Count;
        }

        public override int Count { get; }

        protected override GitPackStates DoGet(int index) => this.m_packs[index].State;
      }

      private class ObjectIdList : 
        VirtualReadOnlyListBase<Sha1Id>,
        ISha1IdTwoWayReadOnlyList,
        ITwoWayReadOnlyList<Sha1Id>,
        IReadOnlyList<Sha1Id>,
        IReadOnlyCollection<Sha1Id>,
        IEnumerable<Sha1Id>,
        IEnumerable
      {
        private readonly IReadOnlyList<GitPackIndexer.MutableIndexEntry> m_entries;
        private readonly IDictionary<Sha1Id, int> m_objectIdToIntIds;

        public ObjectIdList(
          IReadOnlyList<GitPackIndexer.MutableIndexEntry> entries,
          IDictionary<Sha1Id, int> objectIdToIntIds)
        {
          this.m_entries = entries;
          this.m_objectIdToIntIds = objectIdToIntIds;
          this.Count = entries.Count;
        }

        public override int Count { get; }

        protected override Sha1Id DoGet(int index) => this.m_entries[index].ObjectId;

        public bool TryGetIndex(Sha1Id value, out int index) => this.m_objectIdToIntIds.TryGetValue(value, out index);

        public IEnumerable<Sha1Id> FindBetween(Sha1Id min, Sha1Id max) => this.m_entries.Select<GitPackIndexer.MutableIndexEntry, Sha1Id>((Func<GitPackIndexer.MutableIndexEntry, Sha1Id>) (entry => entry.ObjectId)).Where<Sha1Id>((Func<Sha1Id, bool>) (id => id.CompareTo(min) >= 0 && id.CompareTo(max) <= 0));
      }

      private class EntryList : VirtualReadOnlyListBase<GitPackIndexEntry>
      {
        private readonly IReadOnlyList<GitPackIndexer.MutableIndexEntry> m_entries;

        public EntryList(
          IReadOnlyList<GitPackIndexer.MutableIndexEntry> entries)
        {
          this.m_entries = entries;
          this.Count = entries.Count;
        }

        public override int Count { get; }

        protected override GitPackIndexEntry DoGet(int index)
        {
          GitPackIndexer.MutableIndexEntry entry = this.m_entries[index];
          return new GitPackIndexEntry(entry.ObjectType, new TfsGitObjectLocation(entry.PackIntId, entry.Offset, entry.Length));
        }
      }
    }

    [Serializable]
    internal class MaxPackFileCountException : InvalidOperationException
    {
    }
  }
}
