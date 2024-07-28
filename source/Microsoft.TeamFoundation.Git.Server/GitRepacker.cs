// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepacker
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.TeamFoundation.Git.Server.Settings;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Git.Server.Telemetry;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRepacker
  {
    private bool m_hasExecuted;
    private IVssRegistryService m_registryService;
    private readonly IVssRequestContext m_rc;
    private readonly Odb m_odb;
    private readonly ConcatGitPackIndex m_unpackedIndex;
    private readonly ITfsGitBlobProvider m_blob;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly GitPackIndexLoader m_packIndexLoader;
    private readonly IGitPackIndexPointerProvider m_packIndexPtrPrv;
    private readonly Func<GitPackIndexTransaction> m_packIndexTranFactory;
    private readonly bool m_noReuseIndexes;
    private readonly bool m_noReusePacks;
    private readonly IDictionary<Sha1Id, ushort> m_deltaChainLengthTable;
    private readonly int? m_batchSize;
    private readonly IObjectOrderer m_objectOrderer;
    private const string c_layer = "TfsGitRepacker";
    private const ushort c_maxDeltaChainLengthBeforeCut = 50;

    public GitRepacker(
      IVssRequestContext rc,
      Odb odb,
      ConcatGitPackIndex index,
      ITfsGitBlobProvider blobProvider,
      IGitDataFileProvider dataFilePrv,
      GitPackIndexLoader packIndexLoader,
      IGitPackIndexPointerProvider packIndexPointerProvider,
      Func<GitPackIndexTransaction> packIndexTranFactory,
      bool noReuseIndexes,
      bool noReusePacks,
      int? batchSize)
    {
      this.m_rc = rc;
      this.m_odb = odb;
      this.m_unpackedIndex = index;
      this.m_blob = blobProvider;
      this.m_dataFilePrv = dataFilePrv;
      this.m_packIndexLoader = packIndexLoader;
      this.m_packIndexPtrPrv = packIndexPointerProvider;
      this.m_packIndexTranFactory = packIndexTranFactory;
      this.m_noReuseIndexes = noReuseIndexes | noReusePacks;
      this.m_noReusePacks = noReusePacks;
      if (!this.m_unpackedIndex.StableObjectOrderEpoch.HasValue)
      {
        this.m_noReuseIndexes = true;
        this.m_objectOrderer = odb.ObjectOrdererFactory(true);
      }
      this.m_hasExecuted = false;
      this.m_deltaChainLengthTable = (IDictionary<Sha1Id, ushort>) new ShardedDictionary<Sha1Id, ushort>();
      this.m_batchSize = batchSize;
      this.m_registryService = this.m_rc.GetService<IVssRegistryService>();
    }

    public bool Execute()
    {
      this.m_hasExecuted = !this.m_hasExecuted ? true : throw new InvalidOperationException("Execute cannot be called twice on the same GitRepacker.");
      this.m_rc.Trace(1013073, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepacker", "Repacking: {0} , Don't Reuse Indexes: {1} Don't Reuse Packs: {2}", (object) this.m_odb, (object) this.m_noReuseIndexes, (object) this.m_noReusePacks);
      GitPackIndexTransaction tran = this.m_packIndexTranFactory();
      try
      {
        GitPackIndexer indexer = new GitPackIndexer();
        ConcatGitPackIndex subIdxRangeForBase;
        ConcatGitPackIndex subIdxRangeToRewrite;
        GitRepacker.GetSubIndexRangeToRewrite(this.m_unpackedIndex, GitDataFileUtil.GetPackIndexWriter(this.m_rc), this.m_noReuseIndexes, out subIdxRangeForBase, out subIdxRangeToRewrite, this.m_odb.GetAggressivePackIndexMergeStrategy());
        this.m_rc.ReportProgress(70, "GetSubIndexRangeToRewrite");
        this.m_rc.Trace(1013595, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepacker", "{0} objects in {1} sub-indexes will be considered for repacking\n{2} objects in {3} sub-indexes will not be considered for repacking", (object) subIdxRangeToRewrite.Entries.Count, (object) subIdxRangeToRewrite.Subindexes.Count, (object) subIdxRangeForBase.Entries.Count, (object) subIdxRangeForBase.Subindexes.Count);
        indexer.SetBaseIndex(subIdxRangeForBase);
        bool flag = true;
        if (subIdxRangeToRewrite.Subindexes.Count > 0)
        {
          flag = this.RepackSubIndexRange(tran.KnownFilesBuilder, indexer, (IGitPackIndex) subIdxRangeToRewrite);
          if (this.m_objectOrderer != null)
          {
            indexer.StartStableObjectOrderEpoch(StorageUtils.CreateUniqueId());
            indexer.SetStableObjectOrder(this.m_objectOrderer.DequeueAll());
          }
          else
            indexer.PreserveStableObjectOrderIfCompatible((IGitPackIndex) this.m_unpackedIndex);
        }
        this.m_rc.ReportProgress(75, "RepackSubIndexRange");
        Sha1Id sha1Id = GitDataFileUtil.WriteIndex(this.m_rc, this.m_blob, this.m_odb.Id, tran.KnownFilesBuilder, indexer, this.m_odb.GetAggressivePackIndexMergeStrategy());
        this.m_rc.ReportProgress(78, "WriteIndex");
        using (ConcatGitPackIndex optimisticIndex = this.m_packIndexLoader.LoadIndex(new Sha1Id?(sha1Id)))
        {
          this.m_rc.ReportProgress(80, "m_packIndexLoader.LoadIndex");
          this.FsckOptimisticIndex(optimisticIndex);
          this.m_rc.ReportProgress(90, "FsckOptimisticIndex");
          this.MergeConflictingIndexesAndUpdatePointer(tran, optimisticIndex);
          this.m_rc.ReportProgress(94, "MergeConflictingIndexesAndUpdatePointer");
        }
        if (flag && this.m_batchSize.HasValue)
          this.DeleteFullRepackLastKnownObjectId();
        return flag;
      }
      finally
      {
        tran.TryExpirePendingExtantAndDispose();
      }
    }

    private static void GetSubIndexRangeToRewrite(
      ConcatGitPackIndex unpackedIndex,
      IGitPackIndexWriter writer,
      bool noReuseIndexes,
      out ConcatGitPackIndex subIdxRangeForBase,
      out ConcatGitPackIndex subIdxRangeToRewrite,
      IPackIndexMergeStrategy packIndexMergeStrategy = null)
    {
      int num1;
      if (noReuseIndexes)
      {
        num1 = 0;
      }
      else
      {
        ConcatGitPackIndex concatGitPackIndex = writer.Merge(unpackedIndex, packIndexMergeStrategy ?? PackIndexMergeStrategy.Aggressive);
        int num2 = concatGitPackIndex.Subindexes.Count <= 1 ? unpackedIndex.Subindexes.Count : unpackedIndex.Subindexes.Count - concatGitPackIndex.Subindexes.Count;
        for (num1 = 0; num1 < num2; ++num1)
        {
          IGitPackIndex subindex = unpackedIndex.Subindexes[num1];
          if ((int) subindex.PackWatermarks[GitPackWatermark.NumRepacked] != subindex.PackIds.Count)
            break;
        }
      }
      subIdxRangeForBase = unpackedIndex.GetRange(0, num1);
      int count = unpackedIndex.Subindexes.Count - num1;
      subIdxRangeToRewrite = unpackedIndex.GetRange(num1, count);
    }

    internal static void TEST_GetSubIndexRangeToRewrite(
      ConcatGitPackIndex unpackedIndex,
      IGitPackIndexWriter writer,
      bool repackAll,
      out ConcatGitPackIndex subIdxRangeForBase,
      out ConcatGitPackIndex subIdxRangeToRewrite)
    {
      GitRepacker.GetSubIndexRangeToRewrite(unpackedIndex, writer, repackAll, out subIdxRangeForBase, out subIdxRangeToRewrite);
    }

    private bool RepackSubIndexRange(
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IGitPackIndex subidx)
    {
      List<ObjectIdAndGitPackIndexEntry> allEntries;
      List<GitRepacker.GitPack> gitPackList1 = GitRepacker.CollectCurrentPacksLayout(subidx, out allEntries);
      bool finished;
      List<GitRepacker.GitPack> gitPackList2;
      if (this.m_batchSize.HasValue)
      {
        gitPackList2 = this.ConstructProgressiveNewPacksLayout(allEntries, gitPackList1, out finished);
      }
      else
      {
        gitPackList2 = this.ConstructNewPacksLayout(allEntries, new Sha1Id?());
        finished = true;
      }
      object.Equals((object) subidx.Entries.Count, (object) gitPackList1.SelectMany<GitRepacker.GitPack, ObjectIdAndGitPackIndexEntry>((Func<GitRepacker.GitPack, IEnumerable<ObjectIdAndGitPackIndexEntry>>) (x => (IEnumerable<ObjectIdAndGitPackIndexEntry>) x)).Count<ObjectIdAndGitPackIndexEntry>());
      object.Equals((object) subidx.Entries.Count, (object) gitPackList2.SelectMany<GitRepacker.GitPack, ObjectIdAndGitPackIndexEntry>((Func<GitRepacker.GitPack, IEnumerable<ObjectIdAndGitPackIndexEntry>>) (x => (IEnumerable<ObjectIdAndGitPackIndexEntry>) x)).Count<ObjectIdAndGitPackIndexEntry>());
      this.m_rc.Trace(1013596, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepacker", "Considering {0} objects from {1} packfiles to be repacked into {2} packfiles", (object) subidx.Entries.Count, (object) gitPackList1.Count, (object) gitPackList2.Count);
      int createdPackfiles;
      int reusedPackfiles;
      List<GitRepacker.PackingResult> packingResults = GitRepacker.ComputePackingResults(gitPackList1, gitPackList2, this.m_noReusePacks, out List<ushort> _, out createdPackfiles, out reusedPackfiles);
      this.ApplyPackingResults(knownFiles, indexer, subidx, packingResults);
      object.Equals((object) gitPackList2.Count, (object) (createdPackfiles + reusedPackfiles));
      this.m_rc.Trace(1013597, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepacker", "{0} packfiles have been created, {1} packfiles have been reused.", (object) createdPackfiles, (object) reusedPackfiles);
      return finished;
    }

    internal static List<GitRepacker.GitPack> CollectCurrentPacksLayout(
      IGitPackIndex subidx,
      out List<ObjectIdAndGitPackIndexEntry> allEntries)
    {
      allEntries = new List<ObjectIdAndGitPackIndexEntry>(subidx.Entries.Count);
      List<GitRepacker.GitPack> gitPackList = new List<GitRepacker.GitPack>(Enumerable.Range(0, subidx.PackIds.Count).Select<int, GitRepacker.GitPack>((Func<int, GitRepacker.GitPack>) (x => new GitRepacker.GitPack())));
      int index = 0;
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) subidx.Entries)
      {
        Sha1Id objectId = subidx.ObjectIds[index];
        int packIntId = (int) entry.Location.PackIntId;
        int objectType = (int) entry.ObjectType;
        TfsGitObjectLocation location = entry.Location;
        ObjectIdAndGitPackIndexEntry objectEntry = new ObjectIdAndGitPackIndexEntry(objectId, (GitPackObjectType) objectType, location);
        allEntries.Add(objectEntry);
        gitPackList[packIntId].Add(objectEntry);
        ++index;
      }
      return gitPackList;
    }

    private List<GitRepacker.GitPack> ConstructProgressiveNewPacksLayout(
      List<ObjectIdAndGitPackIndexEntry> entries,
      List<GitRepacker.GitPack> curLayout,
      out bool finished)
    {
      Sha1Id hash = this.ReadFullRepackLastKnownObjectId();
      if (hash == Sha1Id.Empty)
      {
        hash = curLayout.Last<GitRepacker.GitPack>().Last<ObjectIdAndGitPackIndexEntry>().ObjectId;
        this.WriteFullRepackLastKnownObjectId(hash);
      }
      finished = true;
      List<GitRepacker.GitPack> gitPackList = this.ConstructNewPacksLayout(entries, new Sha1Id?(hash));
      if (!this.m_batchSize.HasValue || this.m_batchSize.Value < 1)
        return gitPackList;
      int num1 = this.m_batchSize.Value;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      List<GitRepacker.GitPack> source1 = new List<GitRepacker.GitPack>();
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>(entries.Count);
      HashSet<GitRepacker.GitPack> source2 = new HashSet<GitRepacker.GitPack>();
      HashSet<GitRepacker.GitPack> hashSet = curLayout.ToHashSet<GitRepacker.GitPack>();
      foreach (GitRepacker.GitPack source3 in gitPackList)
      {
        source1.Add(source3);
        ++num2;
        sha1IdSet.UnionWith(source3.Select<ObjectIdAndGitPackIndexEntry, Sha1Id>((Func<ObjectIdAndGitPackIndexEntry, Sha1Id>) (o => o.ObjectId)));
        if (!hashSet.Contains(source3))
        {
          --num1;
        }
        else
        {
          ++num4;
          source2.Add(source3);
        }
        if (num1 <= 0)
          break;
      }
      foreach (GitRepacker.GitPack source4 in curLayout)
      {
        if (!source2.Contains(source4))
        {
          bool flag = false;
          foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in source4)
          {
            if (!sha1IdSet.Contains(gitPackIndexEntry.ObjectId))
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            ++num3;
            finished = false;
            source1.Add(source4);
            sha1IdSet.UnionWith(source4.Select<ObjectIdAndGitPackIndexEntry, Sha1Id>((Func<ObjectIdAndGitPackIndexEntry, Sha1Id>) (o => o.ObjectId)));
          }
        }
      }
      int progressPc = (int) (100.0 * (double) num4 / (double) gitPackList.Count);
      this.m_rc.ReportProgress(progressPc, nameof (ConstructProgressiveNewPacksLayout), done: false, result: string.Format("newPacksUsed: {0}, oldPacksUsedDoNotNeedRepacked: {1}, intendedPackCount: {2}", (object) num2, (object) num4, (object) gitPackList.Count));
      ClientTraceData properties = new ClientTraceData();
      properties.Add("oldPacksPreserved", (object) source2.Count<GitRepacker.GitPack>());
      properties.Add("progressPercent", (object) progressPc);
      properties.Add("maxPacksToRepackRemaining", (object) num1);
      properties.Add("currentPackCount", (object) curLayout.Count);
      properties.Add("newPackCount", (object) gitPackList.Count);
      properties.Add("totalEntries", (object) entries.Count<ObjectIdAndGitPackIndexEntry>());
      properties.Add("finalEntries", (object) source1.SelectMany<GitRepacker.GitPack, ObjectIdAndGitPackIndexEntry>((Func<GitRepacker.GitPack, IEnumerable<ObjectIdAndGitPackIndexEntry>>) (x => (IEnumerable<ObjectIdAndGitPackIndexEntry>) x)).Count<ObjectIdAndGitPackIndexEntry>());
      properties.Add("lastKnownObjectId", (object) string.Format("{0}", (object) hash));
      properties.Add(nameof (finished), (object) finished);
      this.m_rc.GetService<ClientTraceService>().Publish(this.m_rc, "Microsoft.TeamFoundation.Git.Server", nameof (ConstructProgressiveNewPacksLayout), properties);
      return source1;
    }

    private Sha1Id ReadFullRepackLastKnownObjectId()
    {
      string sha1IdString = this.m_registryService.GetValue<string>(this.m_rc, (RegistryQuery) this.FullRepackLastKnownObjectIdRegistryPath(), true, (string) null);
      Sha1Id id;
      return !string.IsNullOrEmpty(sha1IdString) && Sha1Id.TryParse(sha1IdString, out id) ? id : Sha1Id.Empty;
    }

    private void WriteFullRepackLastKnownObjectId(Sha1Id hash) => this.m_registryService.Write(this.m_rc, (IEnumerable<RegistryItem>) new RegistryItem[1]
    {
      new RegistryItem(this.FullRepackLastKnownObjectIdRegistryPath(), hash.ToString())
    });

    private void DeleteFullRepackLastKnownObjectId() => this.m_registryService.DeleteEntries(this.m_rc, this.FullRepackLastKnownObjectIdRegistryPath());

    private string FullRepackLastKnownObjectIdRegistryPath() => "/Service/Git/FullRepackLastKnownObjectId/" + this.m_odb.Id.ToString();

    private List<GitRepacker.GitPack> ConstructNewPacksLayout(
      List<ObjectIdAndGitPackIndexEntry> entries,
      Sha1Id? preserveAfterObjectId)
    {
      List<GitRepacker.GitPack> gitPackList = new List<GitRepacker.GitPack>();
      IEnumerable<ObjectIdAndGitPackIndexEntry> collection = Enumerable.Empty<ObjectIdAndGitPackIndexEntry>();
      List<ObjectIdAndGitPackIndexEntry> remainingUnpackedEntries;
      if (!preserveAfterObjectId.HasValue)
      {
        remainingUnpackedEntries = entries;
      }
      else
      {
        entries.Sort();
        IEnumerable<ObjectIdAndGitPackIndexEntry> gitPackIndexEntries = entries.Where<ObjectIdAndGitPackIndexEntry>((Func<ObjectIdAndGitPackIndexEntry, bool>) (e => e.ObjectId == preserveAfterObjectId.Value)).Take<ObjectIdAndGitPackIndexEntry>(1);
        if (gitPackIndexEntries.Any<ObjectIdAndGitPackIndexEntry>())
        {
          remainingUnpackedEntries = entries.TakeWhile<ObjectIdAndGitPackIndexEntry>((Func<ObjectIdAndGitPackIndexEntry, bool>) (e => e.ObjectId != preserveAfterObjectId.Value)).Concat<ObjectIdAndGitPackIndexEntry>(gitPackIndexEntries).ToList<ObjectIdAndGitPackIndexEntry>();
          collection = entries.SkipWhile<ObjectIdAndGitPackIndexEntry>((Func<ObjectIdAndGitPackIndexEntry, bool>) (e => e.ObjectId != preserveAfterObjectId.Value)).Skip<ObjectIdAndGitPackIndexEntry>(1);
        }
        else
        {
          remainingUnpackedEntries = entries;
          this.m_rc.Trace(1013075, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitRepacker", string.Format("ConstructNewPacksLayout - Sha1Id {0} not found in entries list (which has {1} elements. OdbId: {2}", (object) preserveAfterObjectId, (object) entries.Count, (object) this.m_odb.Id));
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      remainingUnpackedEntries.Sort(GitRepacker.\u003C\u003EO.\u003C0\u003E__RepackerTypeComparison ?? (GitRepacker.\u003C\u003EO.\u003C0\u003E__RepackerTypeComparison = new Comparison<ObjectIdAndGitPackIndexEntry>(GitRepacker.RepackerTypeComparison)));
      gitPackList.AddRange((IEnumerable<GitRepacker.GitPack>) GitRepacker.SplitEntriesIntoPacks(remainingUnpackedEntries, this.m_odb.Settings.StablePackfileCapSize, true, out remainingUnpackedEntries));
      remainingUnpackedEntries.AddRange(collection);
      remainingUnpackedEntries.Sort();
      gitPackList.AddRange((IEnumerable<GitRepacker.GitPack>) GitRepacker.SplitEntriesIntoPacks(remainingUnpackedEntries, this.m_odb.Settings.UnstablePackfileCapSize, false, out remainingUnpackedEntries));
      return gitPackList;
    }

    private static List<GitRepacker.PackingResult> ComputePackingResults(
      List<GitRepacker.GitPack> curPacksLayout,
      List<GitRepacker.GitPack> newPacksLayout,
      bool noReusePacks,
      out List<ushort> noLongerReferencedPackIntIds,
      out int createdPackfiles,
      out int reusedPackfiles)
    {
      createdPackfiles = 0;
      reusedPackfiles = 0;
      noLongerReferencedPackIntIds = new List<ushort>();
      HashSet<GitRepacker.GitPack> gitPackSet = new HashSet<GitRepacker.GitPack>((IEnumerable<GitRepacker.GitPack>) curPacksLayout);
      List<GitRepacker.PackingResult> packingResults = new List<GitRepacker.PackingResult>(newPacksLayout.Count);
      foreach (GitRepacker.GitPack source in newPacksLayout)
      {
        if (!noReusePacks && gitPackSet.Contains(source))
        {
          packingResults.Add(new GitRepacker.PackingResult()
          {
            Pack = source,
            Action = GitRepacker.GitPackingAction.CopyEntriesOnly
          });
          ++reusedPackfiles;
        }
        else
        {
          packingResults.Add(new GitRepacker.PackingResult()
          {
            Pack = source,
            Action = GitRepacker.GitPackingAction.CopyEntriesAndContent
          });
          noLongerReferencedPackIntIds.AddRange(source.Select<ObjectIdAndGitPackIndexEntry, ushort>((Func<ObjectIdAndGitPackIndexEntry, ushort>) (x => x.Location.PackIntId)).Distinct<ushort>());
          ++createdPackfiles;
        }
      }
      noLongerReferencedPackIntIds = noLongerReferencedPackIntIds.Distinct<ushort>().ToList<ushort>();
      return packingResults;
    }

    internal static List<GitRepacker.PackingResult> TEST_ComputePackingResults(
      List<GitRepacker.GitPack> curPacksLayout,
      List<GitRepacker.GitPack> newPacksLayout,
      bool noReusePacks,
      out List<ushort> noLongerReferencedPackIntIds,
      out int createdPackfiles,
      out int reusedPackfiles)
    {
      return GitRepacker.ComputePackingResults(curPacksLayout, newPacksLayout, noReusePacks, out noLongerReferencedPackIntIds, out createdPackfiles, out reusedPackfiles);
    }

    private void ApplyPackingResults(
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IGitPackIndex subidx,
      List<GitRepacker.PackingResult> packingResults)
    {
      foreach (GitRepacker.PackingResult packingResult in packingResults)
      {
        if (packingResult.Action == GitRepacker.GitPackingAction.CopyEntriesAndContent)
          this.CopyIndexEntriesAndObjectsContentForRepackedPackFile(knownFiles, indexer, subidx, packingResult.Pack);
        else if (packingResult.Action == GitRepacker.GitPackingAction.CopyEntriesOnly)
          GitRepacker.CopyIndexEntriesOnlyForUntouchedPackFile(indexer, subidx, packingResult.Pack);
      }
      indexer.SetWatermark(GitPackWatermark.NumRepacked);
      if (this.m_objectOrderer == null)
        return;
      foreach (Sha1Id objectId in packingResults.SelectMany<GitRepacker.PackingResult, ObjectIdAndGitPackIndexEntry>((Func<GitRepacker.PackingResult, IEnumerable<ObjectIdAndGitPackIndexEntry>>) (x => (IEnumerable<ObjectIdAndGitPackIndexEntry>) x.Pack)).Select<ObjectIdAndGitPackIndexEntry, Sha1Id>((Func<ObjectIdAndGitPackIndexEntry, Sha1Id>) (x => x.ObjectId)))
      {
        IEnumerable<Sha1Id> list = (IEnumerable<Sha1Id>) this.m_odb.ObjectSet.LookupObject(objectId).ReferencedObjectIds.ToList<Sha1Id>();
        this.m_objectOrderer.EnqueueObject(objectId, list);
      }
    }

    private static List<GitRepacker.GitPack> SplitEntriesIntoPacks(
      List<ObjectIdAndGitPackIndexEntry> entries,
      int packfileSoftCapSize,
      bool splitByType,
      out List<ObjectIdAndGitPackIndexEntry> remainingUnpackedEntries)
    {
      List<GitRepacker.GitPack> gitPackList = new List<GitRepacker.GitPack>();
      GitRepacker.GitPack collection = new GitRepacker.GitPack();
      remainingUnpackedEntries = new List<ObjectIdAndGitPackIndexEntry>();
      long num = 0;
      GitPackObjectType objectType = GitPackObjectType.None;
      bool flag1 = false;
      bool flag2 = false;
      foreach (ObjectIdAndGitPackIndexEntry entry in entries)
      {
        if (num > 0L)
        {
          if (splitByType && GitRepacker.RepackerTypeOrder(objectType) != GitRepacker.RepackerTypeOrder(entry.ObjectType))
            flag1 = true;
          if (num > (long) packfileSoftCapSize || !flag1 && num + entry.Location.Length > (long) packfileSoftCapSize)
            flag2 = true;
        }
        if (flag2 | flag1)
        {
          if (flag2)
            gitPackList.Add(collection);
          else
            remainingUnpackedEntries.AddRange((IEnumerable<ObjectIdAndGitPackIndexEntry>) collection);
          num = 0L;
          collection = new GitRepacker.GitPack();
          flag2 = false;
          flag1 = false;
        }
        collection.Add(entry);
        num += entry.Location.Length;
        objectType = entry.ObjectType;
      }
      if (collection.Count > 0)
      {
        if (!splitByType || num > (long) packfileSoftCapSize)
          gitPackList.Add(collection);
        else
          remainingUnpackedEntries.AddRange((IEnumerable<ObjectIdAndGitPackIndexEntry>) collection);
      }
      return gitPackList;
    }

    internal static List<GitRepacker.GitPack> TEST_SplitEntriesIntoPacks(
      List<ObjectIdAndGitPackIndexEntry> entries,
      int packfileSoftCapSize,
      bool splitByType,
      out List<ObjectIdAndGitPackIndexEntry> remainingUnpackedEntries)
    {
      return GitRepacker.SplitEntriesIntoPacks(entries, packfileSoftCapSize, splitByType, out remainingUnpackedEntries);
    }

    private void CopyIndexEntriesAndObjectsContentForRepackedPackFile(
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IGitPackIndex subidx,
      GitRepacker.GitPack pack)
    {
      GitDataFileUtil.WritePackFile(this.m_rc, this.m_blob, this.m_odb.Id, knownFiles, true, (Action<Sha1Id, Stream>) ((packId, stream) =>
      {
        string packFileName = StorageUtils.GetPackFileName(packId);
        indexer.BeginPackFile(new Sha1Id?(packId), GitPackStates.Derived);
        using (GitPackSerializer serializer = new GitPackSerializer(stream, pack.Count, true))
        {
          long packHeaderLength = (long) GitPackSerializer.PackHeaderLength;
          long currentContiguousOffset = -1;
          long currentContiguousLength = 0;
          int objectCount = 0;
          ushort packIntId = ushort.MaxValue;
          foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in pack)
          {
            ushort num = this.GetDeltaChainLength(this.m_deltaChainLengthTable, gitPackIndexEntry.ObjectId);
            if (num <= (ushort) 50)
            {
              indexer.AddObject(gitPackIndexEntry.ObjectId, gitPackIndexEntry.ObjectType, packHeaderLength, gitPackIndexEntry.Location.Length);
              packHeaderLength += gitPackIndexEntry.Location.Length;
              if ((int) packIntId == (int) gitPackIndexEntry.Location.PackIntId && currentContiguousOffset + currentContiguousLength == gitPackIndexEntry.Location.Offset)
              {
                currentContiguousLength += gitPackIndexEntry.Location.Length;
                ++objectCount;
              }
              else
              {
                GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
                packIntId = gitPackIndexEntry.Location.PackIntId;
                currentContiguousOffset = gitPackIndexEntry.Location.Offset;
                currentContiguousLength = gitPackIndexEntry.Location.Length;
                objectCount = 1;
              }
            }
            else
            {
              num = (ushort) 0;
              GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
              currentContiguousOffset = -1L;
              currentContiguousLength = 0L;
              packIntId = ushort.MaxValue;
              objectCount = 0;
              TfsGitObject gitObj = this.m_odb.ObjectSet.LookupObject(gitPackIndexEntry.ObjectId);
              long deflatedSize;
              serializer.AddObject(gitObj, out deflatedSize);
              indexer.AddObject(gitPackIndexEntry.ObjectId, gitPackIndexEntry.ObjectType, packHeaderLength, deflatedSize);
              packHeaderLength += deflatedSize;
            }
            this.m_deltaChainLengthTable[gitPackIndexEntry.ObjectId] = num;
          }
          GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
          serializer.Complete();
        }
        indexer.EndPackFile(packId);
        knownFiles.QueueExtant(packFileName, KnownFileType.DerivedPackFile);
      }));
    }

    internal static void SerializeContiguousArea(
      IGitDataFileProvider dataFilePrv,
      GitPackSerializer serializer,
      long currentContiguousOffset,
      long currentContiguousLength,
      int objectCount,
      ushort packIntId,
      IGitPackIndex packIntIdSrc)
    {
      if (currentContiguousLength <= 0L)
        return;
      serializer.AddMultipleRaw(dataFilePrv.GetStream(StorageUtils.GetPackFileName(packIntIdSrc.PackIds[(int) packIntId]), currentContiguousOffset, currentContiguousLength), 0L, currentContiguousLength, objectCount);
    }

    internal static void CopyIndexEntriesOnlyForUntouchedPackFile(
      GitPackIndexer indexer,
      IGitPackIndex subidx,
      GitRepacker.GitPack pack)
    {
      int packIntId = (int) pack.First<ObjectIdAndGitPackIndexEntry>().Location.PackIntId;
      indexer.BeginPackFile(new Sha1Id?(subidx.PackIds[packIntId]), subidx.PackStates[packIntId]);
      foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in pack)
      {
        if ((int) gitPackIndexEntry.Location.PackIntId != packIntId)
          throw new InvalidOperationException("This method only supports one pack at a time");
        indexer.AddObject(gitPackIndexEntry.ObjectId, gitPackIndexEntry.ObjectType, gitPackIndexEntry.Location.Offset, gitPackIndexEntry.Location.Length);
      }
      indexer.EndPackFile(subidx.PackIds[packIntId]);
    }

    private ushort GetDeltaChainLength(
      IDictionary<Sha1Id, ushort> deltaChainLengthTable,
      Sha1Id objectId,
      bool memoize = false)
    {
      ushort deltaChainLength;
      if (!deltaChainLengthTable.TryGetValue(objectId, out deltaChainLength))
      {
        Sha1Id? baseIfRefDelta = this.GetBaseIfRefDelta(objectId);
        deltaChainLength = !baseIfRefDelta.HasValue ? (ushort) 0 : (ushort) ((uint) this.GetDeltaChainLength(deltaChainLengthTable, baseIfRefDelta.Value, true) + 1U);
        if (memoize)
          deltaChainLengthTable[objectId] = deltaChainLength;
      }
      return deltaChainLength;
    }

    private Sha1Id? GetBaseIfRefDelta(Sha1Id objectId)
    {
      TfsGitObjectLocation location = this.m_unpackedIndex.LookupObject(objectId).Location;
      using (Stream stream = this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(this.m_unpackedIndex.PackIds[(int) location.PackIntId]), location.Offset, location.Length))
      {
        GitPackObjectType type;
        GitServerUtils.ReadPackEntryHeader(stream, out type, out long _);
        if (type == GitPackObjectType.RefDelta)
        {
          try
          {
            return new Sha1Id?(Sha1Id.FromStream(stream));
          }
          catch (Sha1IdStreamReadException ex)
          {
            throw new InvalidGitPackEntryHeaderException((Exception) ex);
          }
        }
      }
      return new Sha1Id?();
    }

    private void FsckOptimisticIndex(ConcatGitPackIndex optimisticIndex)
    {
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>((IEnumerable<Sha1Id>) this.m_unpackedIndex.PackIds);
      this.m_rc.ReportProgress(80, nameof (FsckOptimisticIndex), done: false, result: string.Format("Preparing packs dictionary for {0} packIds", (object) optimisticIndex.PackIds.Count));
      Dictionary<ushort, GitRepacker.GitPack> repackedPacksByIntId = new Dictionary<ushort, GitRepacker.GitPack>();
      for (int index = 0; index < optimisticIndex.PackIds.Count; ++index)
      {
        if (!sha1IdSet.Contains(optimisticIndex.PackIds[index]))
          repackedPacksByIntId.Add(checked ((ushort) index), new GitRepacker.GitPack());
      }
      this.m_rc.ReportProgress(82, nameof (FsckOptimisticIndex), done: false, result: string.Format("Packing {0} objects", (object) optimisticIndex.Entries.Count));
      int index1 = 0;
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) optimisticIndex.Entries)
      {
        GitRepacker.GitPack gitPack;
        if (repackedPacksByIntId.TryGetValue(entry.Location.PackIntId, out gitPack))
          gitPack.Add(new ObjectIdAndGitPackIndexEntry(optimisticIndex.ObjectIds[index1], entry.ObjectType, entry.Location));
        ++index1;
      }
      this.m_rc.ReportProgress(85, nameof (FsckOptimisticIndex), done: false, result: string.Format("CreateRepackedPackFsck for {0} entries", (object) repackedPacksByIntId.Count));
      if (this.m_rc.IsFeatureEnabled("Git.EnableParallelFsckGitRepacker"))
        this.ParallelFsckOptimisticIndex(optimisticIndex, repackedPacksByIntId);
      else
        this.SerialFsckOptimisticIndex(optimisticIndex, repackedPacksByIntId);
    }

    private void ParallelFsckOptimisticIndex(
      ConcatGitPackIndex optimisticIndex,
      Dictionary<ushort, GitRepacker.GitPack> repackedPacksByIntId)
    {
      GitOdbSettings odbSettings = new GitOdbSettingsProvider(this.m_rc, this.m_rc.GetService<IVssRegistryService>(), this.m_odb.Id).GetSettings();
      Lazy<GitOdbSettings> lazyOdbSettings = new Lazy<GitOdbSettings>((Func<GitOdbSettings>) (() => odbSettings));
      new ConcurrentOperation<KeyValuePair<ushort, GitRepacker.GitPack>>(this.m_rc, (Action<IVssRequestContext, ConcurrentQueue<KeyValuePair<ushort, GitRepacker.GitPack>>>) ((requestContext, queue) => GitRepacker.ParallelFsckOptimisticIndexEachThread(requestContext, queue, this.m_odb.Id, optimisticIndex, lazyOdbSettings)), (IReadOnlyCollection<KeyValuePair<ushort, GitRepacker.GitPack>>) repackedPacksByIntId, nameof (ParallelFsckOptimisticIndex)).Execute();
    }

    private static void ParallelFsckOptimisticIndexEachThread(
      IVssRequestContext thisThreadContext,
      ConcurrentQueue<KeyValuePair<ushort, GitRepacker.GitPack>> workToDo,
      OdbId odbId,
      ConcatGitPackIndex optimisticIndex,
      Lazy<GitOdbSettings> lazyOdbSettings)
    {
      GitDataFileProviderService service1 = thisThreadContext.GetService<GitDataFileProviderService>();
      GitBlobProviderService service2 = thisThreadContext.GetService<GitBlobProviderService>();
      IVssRequestContext rc = thisThreadContext;
      OdbId odbId1 = odbId;
      ITfsGitBlobProvider blobProvider = service2.BlobProvider;
      Lazy<GitOdbSettings> odbSettings = lazyOdbSettings;
      IGitDataFileProvider dataFileProvider = service1.Create(rc, odbId1, blobProvider, odbSettings);
      using (Odb odb = DefaultGitDependencyRoot.Instance.CreateOdb(thisThreadContext, odbId))
      {
        KeyValuePair<ushort, GitRepacker.GitPack> result;
        while (workToDo.TryDequeue(out result) && !thisThreadContext.IsCanceled)
        {
          ushort key = result.Key;
          GitRepacker.GitPack expected = result.Value;
          string packFileName = StorageUtils.GetPackFileName(optimisticIndex.PackIds[(int) key]);
          using (Stream stream = dataFileProvider.GetStream(packFileName))
          {
            if (thisThreadContext.IsCanceled)
              break;
            GitRepacker.CreateRepackedPackFsck(thisThreadContext, odb, stream, key, expected).Deserialize();
          }
        }
      }
    }

    private void SerialFsckOptimisticIndex(
      ConcatGitPackIndex optimisticIndex,
      Dictionary<ushort, GitRepacker.GitPack> repackedPacksByIntId)
    {
      foreach (KeyValuePair<ushort, GitRepacker.GitPack> keyValuePair in repackedPacksByIntId)
      {
        ushort key = keyValuePair.Key;
        GitRepacker.GitPack expected = keyValuePair.Value;
        using (Stream stream = this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(optimisticIndex.PackIds[(int) key])))
          GitRepacker.CreateRepackedPackFsck(this.m_rc, this.m_odb, stream, key, expected).Deserialize();
      }
    }

    private void MergeConflictingIndexesAndUpdatePointer(
      GitPackIndexTransaction tran,
      ConcatGitPackIndex optimisticIndex)
    {
      GitRepacker.INTERNAL_MergeConflictingIndexesAndUpdatePointer(this.m_rc, this.m_blob, this.m_odb, this.m_packIndexLoader, this.m_packIndexPtrPrv, tran, this.m_unpackedIndex, optimisticIndex, true);
    }

    internal static void INTERNAL_MergeConflictingIndexesAndUpdatePointer(
      IVssRequestContext rc,
      ITfsGitBlobProvider blob,
      Odb odb,
      GitPackIndexLoader packIndexLoader,
      IGitPackIndexPointerProvider packIndexPtrPrv,
      GitPackIndexTransaction tran,
      ConcatGitPackIndex unpackedIndex,
      ConcatGitPackIndex optimisticIndex,
      bool setLastPackedIfPossible)
    {
      tran.EnsureIndexLease();
      Sha1Id? index = packIndexPtrPrv.GetIndex();
      using (ConcatGitPackIndex concatGitPackIndex = packIndexLoader.LoadIndex(index))
      {
        Sha1Id? nullable = index;
        Sha1Id? realTipSubindexId = unpackedIndex.GetRealTipSubindexId(true);
        if ((nullable.HasValue == realTipSubindexId.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != realTipSubindexId.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        {
          GitPackIndexer indexer = new GitPackIndexer();
          indexer.SetBaseIndex(optimisticIndex);
          indexer.AddFromIndex(concatGitPackIndex);
          indexer.PreserveStableObjectOrderIfCompatible((IGitPackIndex) concatGitPackIndex);
          Sha1Id sha1Id = GitDataFileUtil.WriteIndex(rc, blob, odb.Id, tran.KnownFilesBuilder, indexer, odb.GetFastPackIndexMergeStrategy());
          using (ConcatGitPackIndex newIndex = packIndexLoader.LoadIndex(new Sha1Id?(sha1Id)))
            tran.CommitAndDispose(concatGitPackIndex, newIndex);
        }
        else
          tran.CommitAndDispose(concatGitPackIndex, optimisticIndex, setLastPackedIfPossible);
      }
    }

    private static int RepackerTypeComparison(
      ObjectIdAndGitPackIndexEntry first,
      ObjectIdAndGitPackIndexEntry second)
    {
      int num = GitRepacker.RepackerTypeOrder(first.ObjectType).CompareTo(GitRepacker.RepackerTypeOrder(second.ObjectType));
      return num != 0 ? num : first.Location.CompareTo(second.Location);
    }

    internal static int TEST_comparer(
      ObjectIdAndGitPackIndexEntry first,
      ObjectIdAndGitPackIndexEntry second)
    {
      return GitRepacker.RepackerTypeComparison(first, second);
    }

    private static int RepackerTypeOrder(GitPackObjectType objectType)
    {
      switch (objectType)
      {
        case GitPackObjectType.Commit:
        case GitPackObjectType.Tag:
          return 0;
        case GitPackObjectType.Tree:
          return 1;
        case GitPackObjectType.Blob:
          return 2;
        default:
          throw new InvalidOperationException("Base type required");
      }
    }

    private static GitPackDeserializer CreateRepackedPackFsck(
      IVssRequestContext requestContext,
      Odb odb,
      Stream actual,
      ushort expectedPackIntId,
      GitRepacker.GitPack expected)
    {
      GitPackDeserializer forOdbFsck = GitPackDeserializer.CreateForOdbFsck(requestContext, odb, actual, false);
      forOdbFsck.AddTrait((IGitPackDeserializerTrait) new GitPackDeserializerAllObjectsExistTrait(requestContext, expectedPackIntId, (IEnumerable<ObjectIdAndGitPackIndexEntry>) expected, false));
      return forOdbFsck;
    }

    internal class GitPack : 
      IEquatable<GitRepacker.GitPack>,
      IEnumerable<ObjectIdAndGitPackIndexEntry>,
      IEnumerable
    {
      private List<ObjectIdAndGitPackIndexEntry> m_entries;
      private int m_incrementalHash;
      private bool m_isSorted;

      public GitPack()
      {
        this.m_incrementalHash = 0;
        this.m_isSorted = true;
        this.m_entries = new List<ObjectIdAndGitPackIndexEntry>();
      }

      public int Count => this.m_entries.Count;

      public void Add(ObjectIdAndGitPackIndexEntry objectEntry)
      {
        this.m_entries.Add(objectEntry);
        this.m_isSorted = false;
        this.m_incrementalHash += objectEntry.ObjectId.GetHashCode();
      }

      public override int GetHashCode() => this.m_incrementalHash * 486187739 + this.m_entries.Count.GetHashCode();

      public bool Equals(GitRepacker.GitPack other)
      {
        if (!this.m_isSorted)
        {
          this.m_entries.Sort();
          this.m_isSorted = true;
        }
        if (!other.m_isSorted)
        {
          other.m_entries.Sort();
          other.m_isSorted = true;
        }
        return this.m_entries.SequenceEqual<ObjectIdAndGitPackIndexEntry>((IEnumerable<ObjectIdAndGitPackIndexEntry>) other.m_entries);
      }

      public IEnumerator<ObjectIdAndGitPackIndexEntry> GetEnumerator()
      {
        if (!this.m_isSorted)
        {
          this.m_entries.Sort();
          this.m_isSorted = true;
        }
        return (IEnumerator<ObjectIdAndGitPackIndexEntry>) this.m_entries.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        if (!this.m_isSorted)
        {
          this.m_entries.Sort();
          this.m_isSorted = true;
        }
        return (IEnumerator) this.m_entries.GetEnumerator();
      }
    }

    internal enum GitPackingAction
    {
      CopyEntriesAndContent,
      CopyEntriesOnly,
    }

    internal struct PackingResult
    {
      public GitRepacker.GitPack Pack;
      public GitRepacker.GitPackingAction Action;
    }
  }
}
