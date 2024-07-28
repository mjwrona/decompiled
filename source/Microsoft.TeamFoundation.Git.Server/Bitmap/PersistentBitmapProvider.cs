// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.PersistentBitmapProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Riff;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class PersistentBitmapProvider : 
    ICommitReachabilityProvider,
    IReachabilityBitmapProvider,
    IDisposable
  {
    private readonly IVssRequestContext m_rc;
    private readonly IGitObjectSet m_repo;
    private readonly IGitGraphProvider m_graphPrv;
    private readonly Sha1Id m_stableOrderEpoch;
    private readonly IOdbBitmapFileProvider m_filePrv;
    private Sha1Id? m_fileId;
    private ReachabilityBitmapCollection m_collection;
    private DeltaForestAlgorithm<int, Sha1Id> m_algorithm;
    private const string c_layer = "PersistentBitmapProvider";
    private const int c_numObjectsBeyondGraph = 10000;

    public PersistentBitmapProvider(
      IVssRequestContext rc,
      IGitObjectSet repo,
      IGitGraphProvider graphPrv,
      Sha1Id stableOrderEpoch,
      ITwoWayReadOnlyList<Sha1Id> objectList,
      IOdbBitmapFileProvider filePrv)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<IGitObjectSet>(repo, nameof (repo));
      ArgumentUtility.CheckForNull<IGitGraphProvider>(graphPrv, nameof (graphPrv));
      ArgumentUtility.CheckForNull<ITwoWayReadOnlyList<Sha1Id>>(objectList, nameof (objectList));
      ArgumentUtility.CheckForNull<IOdbBitmapFileProvider>(filePrv, nameof (filePrv));
      this.m_rc = rc;
      this.m_repo = repo;
      this.m_graphPrv = graphPrv;
      this.m_stableOrderEpoch = stableOrderEpoch;
      this.ObjectList = objectList;
      this.m_filePrv = filePrv;
      this.m_algorithm = new DeltaForestAlgorithm<int, Sha1Id>();
      this.Stats = new PersistentBitmapProvider.Statistics();
    }

    public void Dispose() => this.m_collection?.Dispose();

    public ITwoWayReadOnlyList<Sha1Id> ObjectList { get; }

    public bool HasReachabilityBitmap()
    {
      if (!this.m_fileId.HasValue)
        this.m_fileId = this.m_filePrv.GetReachabilityBitmapFileId();
      return this.m_fileId.HasValue;
    }

    public IBitmap<Sha1Id> GetReachableIndexSet(
      IEnumerable<Sha1Id> reachableFromCommits,
      IEnumerable<Sha1Id> notReachableFromCommits = null,
      IReadOnlyBitmap<Sha1Id> notInSet = null,
      IObserver<int> statusObserver = null)
    {
      RoaringBitmap<Sha1Id> notInBitmap = (RoaringBitmap<Sha1Id>) notInSet;
      if (this.GetReachabilityBitmaps() == null)
        this.m_collection = new ReachabilityBitmapCollection(this.m_stableOrderEpoch, this.ObjectList);
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      IGitCommitGraph graph = this.m_graphPrv.Get((notReachableFromCommits != null ? notReachableFromCommits.Concat<Sha1Id>(reachableFromCommits) : (IEnumerable<Sha1Id>) null) ?? reachableFromCommits);
      IReadOnlySet<int> labelsToWalk = (IReadOnlySet<int>) null;
      if (notReachableFromCommits != null)
      {
        List<int> list = graph.GetLabels(notReachableFromCommits).ToList<int>();
        if (list.Count > 0)
        {
          this.Stats.HasNotReachableLabels = true;
          labelsToWalk = (IReadOnlySet<int>) new NotReachableLabels<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) graph, (IEnumerable<int>) list);
        }
      }
      RoaringBitmap<Sha1Id> resultBitmap = new RoaringBitmap<Sha1Id>(this.ObjectList);
      SubgraphWrapper<int, Sha1Id> subgraphWrapper = new SubgraphWrapper<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) graph, (Predicate<int>) (label =>
      {
        IReadOnlySet<int> readOnlySet = labelsToWalk;
        if ((readOnlySet != null ? (readOnlySet.Contains(label) ? 1 : 0) : 1) == 0)
          return false;
        ++this.Stats.NumCommitGetIndex;
        return !resultBitmap.Contains<Sha1Id>(graph.GetVertex(label));
      }));
      Stopwatch stopwatch = Stopwatch.StartNew();
      HashSet<Sha1Id> source = new HashSet<Sha1Id>();
      HashSet<int> intSet = new HashSet<int>();
      SubgraphWrapper<int, Sha1Id> graph1 = subgraphWrapper;
      IEnumerable<Sha1Id> reachableFrom = reachableFromCommits;
      IReadOnlySet<int> restrictedLabels = labelsToWalk;
      foreach (Sha1Id vertex in ancestralGraphAlgorithm.GetReachable((IDirectedGraph<int, Sha1Id>) graph1, reachableFrom, restrictedLabels))
      {
        if (this.m_collection.DeltaForest.HasVertex(vertex))
        {
          IReadOnlyList<int> reachableLabels = this.m_algorithm.CompareDeltaChains(this.m_collection.DeltaForest, (IEnumerable<int>) new int[1]
          {
            this.m_collection.DeltaForest.GetLabel(vertex)
          }, (IEnumerable<int>) intSet).ReachableLabels;
          if (reachableLabels.Count > 0)
          {
            resultBitmap = RoaringBitmapCombiner<Sha1Id>.Union(resultBitmap, this.m_collection.Combine((IEnumerable<int>) reachableLabels, notInBitmap));
            ++this.Stats.NumCombineCalls;
            this.Stats.NumCombinedBitmaps += (long) reachableLabels.Count;
            intSet.AddRange<int, HashSet<int>>((IEnumerable<int>) reachableLabels);
          }
        }
        else
          source.Add(vertex);
      }
      this.Stats.NumReachableObjectsBeforeWalk = resultBitmap.Count;
      this.Stats.CombineBitmapsMillis = stopwatch.ElapsedMilliseconds;
      this.Stats.NumCommitsToWalk = source.Count;
      stopwatch.Restart();
      new ObjectWalker().Walk((IEnumerable<TfsGitObject>) source.Where<Sha1Id>((Func<Sha1Id, bool>) (commitId => resultBitmap.Add(commitId))).Select<Sha1Id, TfsGitTree>((Func<Sha1Id, TfsGitTree>) (commitId =>
      {
        ++this.Stats.NumRootTreesToWalk;
        return this.m_repo.LookupObject<TfsGitTree>(graph.GetRootTreeId(graph.GetLabel(commitId)));
      })), (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) =>
      {
        ++this.Stats.NumObjectGetIndex;
        int index = this.ObjectList.GetIndex<Sha1Id>(id);
        RoaringBitmap<Sha1Id> roaringBitmap = notInBitmap;
        if ((roaringBitmap != null ? (!roaringBitmap.ContainsIndex(index) ? 1 : 0) : 1) == 0 || !resultBitmap.AddIndex(index))
          return false;
        statusObserver?.OnNext(resultBitmap.Count);
        return true;
      }));
      this.Stats.NumObjectsAddedByWalk = resultBitmap.Count - this.Stats.NumReachableObjectsBeforeWalk;
      this.Stats.ExtendBitmapsMillis = stopwatch.ElapsedMilliseconds;
      this.m_rc.TraceConditionally(1013729, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (PersistentBitmapProvider), (Func<string>) (() => JsonConvert.SerializeObject((object) this.Stats)));
      return (IBitmap<Sha1Id>) resultBitmap;
    }

    public ReachabilityBitmapCollection GetReachabilityBitmaps() => this.m_collection == null && !this.TryLoadReachabilityBitmapCollectionFromStorage(out this.m_collection, out Sha1Id? _) ? (ReachabilityBitmapCollection) null : this.m_collection;

    internal bool TryLoadReachabilityBitmapCollectionFromStorage(
      out ReachabilityBitmapCollection collection,
      out Sha1Id? fileId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      fileId = this.m_filePrv.GetReachabilityBitmapFileId();
      if (!fileId.HasValue)
      {
        collection = (ReachabilityBitmapCollection) null;
        return false;
      }
      Stream stream = (Stream) null;
      RiffFile riffFile = (RiffFile) null;
      try
      {
        stream = this.m_filePrv.GetStream(fileId.Value);
        ReachabilityBitmapCollection.MetaFormat metaFormat = new ReachabilityBitmapCollection.MetaFormat();
        collection = metaFormat.Read(this.m_stableOrderEpoch, this.ObjectList, stream);
        stream = (Stream) null;
        this.Stats.LoadBitmapFromStorageMillis = stopwatch.ElapsedMilliseconds;
        return true;
      }
      finally
      {
        riffFile?.Dispose();
        stream?.Dispose();
      }
    }

    internal void UpdateReachabilityBitmapStorage(
      ReachabilityBitmapCollection collection,
      Sha1Id? existingFileId)
    {
      if (!this.m_stableOrderEpoch.Equals(collection.StableOrderEpoch))
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Stable order for {0} does not match!", (object) this.m_repo)));
      Sha1Id? nullable = existingFileId;
      Sha1Id? reachabilityBitmapFileId = this.m_filePrv.GetReachabilityBitmapFileId();
      if ((nullable.HasValue == reachabilityBitmapFileId.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != reachabilityBitmapFileId.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Cannot store bitmap collection. {0}: {1}.", (object) nameof (existingFileId), (object) existingFileId)));
      this.m_rc.TraceConditionally(1013724, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (PersistentBitmapProvider), (Func<string>) (() => JsonConvert.SerializeObject((object) new
      {
        odb = this.m_repo.ToString(),
        numObjects = collection.ObjectList.Count,
        numBitmaps = ((Sha1IdDeltaForest) collection.DeltaForest).NumVertices
      })));
      this.m_filePrv.SerializeUpdatedCollection(collection, existingFileId);
    }

    public bool TryComputeOrExtendReachabilityBitmaps()
    {
      if (this.m_collection != null)
      {
        this.m_collection.Dispose();
        this.m_collection = (ReachabilityBitmapCollection) null;
      }
      GitCommitGraph graph;
      if (!this.m_graphPrv.TryLoadFromStorage(out graph, out Sha1Id? _))
        return false;
      ReachabilityBitmapBuilder reachabilityBitmapBuilder = new ReachabilityBitmapBuilder(this.m_rc, this.m_repo, this.m_stableOrderEpoch, this.ObjectList, (IGitCommitGraph) graph);
      ReachabilityBitmapCollection collection = (ReachabilityBitmapCollection) null;
      try
      {
        Sha1Id? fileId;
        if (this.TryLoadReachabilityBitmapCollectionFromStorage(out collection, out fileId) && collection.NumObjectsAtSerialization + 10000 >= graph.NumObjectsAtSerialization)
        {
          this.m_collection = collection;
          collection = (ReachabilityBitmapCollection) null;
          return false;
        }
        this.m_collection = reachabilityBitmapBuilder.ComputeBitmaps(collection);
        this.UpdateReachabilityBitmapStorage(this.m_collection, fileId);
      }
      finally
      {
        collection?.Dispose();
      }
      return true;
    }

    internal HashSet<Sha1Id> GetFilteredRefs(IEnumerable<Sha1Id> currentTips)
    {
      HashSet<Sha1Id> filteredRefs = new HashSet<Sha1Id>();
      foreach (Sha1Id currentTip in currentTips)
      {
        TfsGitObject gitObject = this.m_repo.TryLookupObject(currentTip);
        TfsGitCommit commit = gitObject != null ? gitObject.TryResolveToCommit() : (TfsGitCommit) null;
        if (commit != null)
          filteredRefs.Add(commit.ObjectId);
      }
      return filteredRefs;
    }

    private PersistentBitmapProvider.Statistics Stats { get; }

    private class Statistics
    {
      public ReachabilityBitmapBuilder.Statistics BuilderStats { get; set; }

      public long LoadBitmapFromStorageMillis { get; set; }

      public long CombineBitmapsMillis { get; set; }

      public long ExtendBitmapsMillis { get; set; }

      public long NumCombinedBitmaps { get; set; }

      public long NumCombineCalls { get; set; }

      public bool HasNotReachableLabels { get; set; }

      public int NumCommitsToWalk { get; set; }

      public int NumReachableCommitsBeforeFilters { get; set; }

      public int NumReachableObjectsBeforeWalk { get; set; }

      public int NumObjectsAddedByWalk { get; set; }

      public int NumRootTreesToWalk { get; set; }

      public int NumCommitGetIndex { get; set; }

      public int NumObjectGetIndex { get; set; }
    }
  }
}
