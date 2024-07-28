// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitGraphProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Riff;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class GitGraphProvider : IGitGraphProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly IGitObjectSet m_odb;
    private readonly GitCommitGraphCacheService m_graphCache;
    private readonly IGitKnownFilesProvider m_knownFilesPrv;
    private readonly ITfsGitBlobProvider m_blobPrv;
    private readonly IGitDataFileProvider m_filePrv;
    private readonly CacheKeys.CrossHostOdbId m_odbId;
    private static readonly ConcurrentDictionary<CacheKeys.CrossHostOdbId, object> s_graphBuildLocks = new ConcurrentDictionary<CacheKeys.CrossHostOdbId, object>();
    private const string c_layer = "GitGraphProvider";
    private const long c_newObjectsUntilReserialize = 5120;
    private const int c_maxChangesInBloomFilter = 512;
    private const int c_numMillisToWriteStats = 1000;
    private const byte c_maxGenerations = 10;

    public GitGraphProvider(
      IVssRequestContext rc,
      IGitObjectSet odb,
      GitCommitGraphCacheService graphCache,
      IGitKnownFilesProvider knownFilesPrv,
      ITfsGitBlobProvider blobPrv,
      IGitDataFileProvider filePrv,
      CacheKeys.CrossHostOdbId odbId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<IGitObjectSet>(odb, nameof (odb));
      ArgumentUtility.CheckForNull<GitCommitGraphCacheService>(graphCache, nameof (graphCache));
      ArgumentUtility.CheckForNull<IGitKnownFilesProvider>(knownFilesPrv, nameof (knownFilesPrv));
      ArgumentUtility.CheckForNull<ITfsGitBlobProvider>(blobPrv, nameof (blobPrv));
      ArgumentUtility.CheckForNull<IGitDataFileProvider>(filePrv, nameof (filePrv));
      this.m_rc = rc;
      this.m_odb = odb;
      this.m_graphCache = graphCache;
      this.m_knownFilesPrv = knownFilesPrv;
      this.m_blobPrv = blobPrv;
      this.m_filePrv = filePrv;
      this.m_odbId = odbId;
    }

    public IGitCommitGraph Get(IEnumerable<Sha1Id> requiredCommits)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(requiredCommits, nameof (requiredCommits));
      using (this.m_rc.TraceBlock(1013493, 1013494, GitServerUtils.TraceArea, nameof (GitGraphProvider), nameof (Get)))
      {
        foreach (Sha1Id requiredCommit in requiredCommits)
          this.m_odb.LookupObject<TfsGitCommit>(requiredCommit);
        GitCommitGraph graph = (GitCommitGraph) null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (this.m_graphCache.TryGetValue(this.m_rc, this.m_odbId, out graph) && requiredCommits.All<Sha1Id>((Func<Sha1Id, bool>) (id => graph.HasVertex(id) && graph.GetAncestryDepth(graph.GetLabel(id)) >= 0)))
          return (IGitCommitGraph) graph;
        GitGraphProvider.Statistics statistics = new GitGraphProvider.Statistics();
        statistics.CacheMillis = stopwatch.ElapsedMilliseconds;
        stopwatch.Restart();
        lock (GitGraphProvider.s_graphBuildLocks.GetOrAdd(this.m_odbId, new object()))
        {
          if (!this.m_graphCache.TryGetValue(this.m_rc, this.m_odbId, out graph) && !this.TryLoadFromStorage(out graph, out Sha1Id? _))
          {
            using (this.m_rc.AllowAnonymousOrPublicUserWrites())
              KeyScopedJobUtil.QueueFor(this.m_rc, this.m_odbId.OdbId, "GitGraphSerializationJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitGraphSerializationJob", JobPriorityLevel.Normal, JobPriorityClass.Normal);
            graph = new GitCommitGraph();
          }
          statistics.GetLockMillis = stopwatch.ElapsedMilliseconds;
          stopwatch.Restart();
          int numVertices = graph.NumVertices;
          this.ExtendGraph(graph, requiredCommits);
          statistics.ExtendGraphMillis = stopwatch.ElapsedMilliseconds;
          statistics.NumVerticesAdded = graph.NumVertices - numVertices;
          this.m_graphCache.Set(this.m_rc, this.m_odbId, graph);
        }
        if (statistics.TotalMillis >= 1000L || this.m_rc.IsTracing(1013684, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitGraphProvider)))
          this.m_rc.TraceAlways(1013684, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitGraphProvider), JsonConvert.SerializeObject((object) statistics));
        return (IGitCommitGraph) graph;
      }
    }

    internal void ExtendGraph(GitCommitGraph graph, IEnumerable<Sha1Id> requiredCommits)
    {
      CacheAdjacenciesGraphWrapper<Sha1Id, TfsGitCommit> graph1 = new CacheAdjacenciesGraphWrapper<Sha1Id, TfsGitCommit>((IDirectedGraph<Sha1Id, TfsGitCommit>) new SubgraphWrapper<Sha1Id, TfsGitCommit>((IDirectedGraph<Sha1Id, TfsGitCommit>) new GitRepoGraph(this.m_odb), (Predicate<Sha1Id>) (id => !graph.HasVertex(id) || graph.GetAncestryDepth(graph.GetLabel(id)) < 0), true));
      IEnumerable<Sha1Id> reachableLabels = new AncestralGraphAlgorithm<Sha1Id, TfsGitCommit>().GetReachableLabels((IDirectedGraph<Sha1Id, TfsGitCommit>) graph1, requiredCommits);
      Sha1Id? nullable = new Sha1Id?();
      foreach (Sha1Id sha1Id in reachableLabels)
      {
        if (graph1.HasLabel(sha1Id))
        {
          TfsGitCommit vertex = graph1.GetVertex(sha1Id);
          graph.AddVertex(sha1Id, graph1.OutNeighborsOfLabel(sha1Id));
          int label = graph.GetLabel(sha1Id);
          graph.SetCommitTime(label, vertex.GetCommitter().Time);
          graph.SetRootTreeId(label, vertex.GetTree().ObjectId);
        }
        if (nullable.HasValue)
          graph1.ClearLabel(nullable.Value);
        nullable = new Sha1Id?(sha1Id);
      }
      List<int> list = new AncestralGraphAlgorithm<int, Sha1Id>().Order<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) new SubgraphWrapper<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) graph, (Predicate<int>) (i => graph.GetAncestryDepth(i) < 0)), requiredCommits).Select<Sha1Id, int>((Func<Sha1Id, int>) (id => graph.GetLabel(id))).ToList<int>();
      for (int index = list.Count - 1; index >= 0; --index)
      {
        int label = list[index];
        int depth = graph.OutNeighborsOfLabel(label).Select<int, int>((Func<int, int>) (j => graph.GetAncestryDepth(j))).DefaultIfEmpty<int>(-1).Max() + 1;
        graph.SetAncestryDepth(label, depth);
      }
    }

    internal void ExtendChangedPathFilters(
      GitCommitGraph graph,
      IEnumerable<Sha1Id> requiredCommits,
      int maxChangesPerCommit = 512)
    {
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      IEnumerable<int> labels = graph.GetLabels(requiredCommits);
      GitCommitGraph graph1 = graph;
      IEnumerable<int> reachableFrom = labels;
      foreach (int reachableLabel in ancestralGraphAlgorithm.GetReachableLabels((IDirectedGraph<int, Sha1Id>) graph1, reachableFrom))
      {
        if (graph.GetFilterStatus(reachableLabel) == BloomFilterStatus.NotComputed)
        {
          TfsGitCommit tfsGitCommit = this.m_odb.LookupObject<TfsGitCommit>(graph.GetVertex(reachableLabel));
          TfsGitTree tree = tfsGitCommit.GetTree();
          IList<string> list = (IList<string>) TfsGitDiffHelper.DiffTreesLazy(tfsGitCommit.GetParents().FirstOrDefault<TfsGitCommit>()?.GetTree(), tree).Select<TfsGitDiffEntry, string>((Func<TfsGitDiffEntry, string>) (entry => entry.RelativePath)).Take<string>(maxChangesPerCommit + 1).ToList<string>();
          if (list.Count > maxChangesPerCommit)
            graph.ChangedPathFilters.SetFilterStatus(reachableLabel, BloomFilterStatus.TooLarge);
          else
            graph.ChangedPathFilters.ComputeFilter(reachableLabel, list);
        }
      }
    }

    public bool TryLoadFromStorage(out GitCommitGraph graph, out Sha1Id? graphId) => this.TryLoad(out graph, out graphId, false);

    private bool TryLoad(out GitCommitGraph graph, out Sha1Id? graphId, bool loadGenerations)
    {
      using (this.m_rc.TraceBlock(1013561, 1013562, GitServerUtils.TraceArea, nameof (GitGraphProvider), nameof (TryLoad)))
      {
        Stream stream = (Stream) null;
        RiffFile riff = (RiffFile) null;
        try
        {
          graphId = this.GetGraphId();
          if (!graphId.HasValue)
          {
            graph = (GitCommitGraph) null;
            return false;
          }
          stream = this.m_filePrv.GetStream(StorageUtils.GetOdbFileName(graphId.Value, KnownFileType.Graph));
          if (RiffFile.TryLoad(stream, out riff, true))
          {
            IGitCommitGraphReader commitGraphReader = (IGitCommitGraphReader) new GitCommitGraph.M101Format();
            graph = commitGraphReader.Read(riff, loadGenerations);
            return true;
          }
          graph = (GitCommitGraph) null;
          return false;
        }
        finally
        {
          riff?.Dispose();
          stream?.Dispose();
        }
      }
    }

    private Sha1Id? GetGraphId()
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId.OdbId))
        return gitOdbComponent.ReadPointer(OdbPointerType.Graph);
    }

    internal int GetNumObjectsFromStorage()
    {
      Stream stream = (Stream) null;
      RiffFile riff = (RiffFile) null;
      try
      {
        Sha1Id? graphId = this.GetGraphId();
        if (!graphId.HasValue)
          return 0;
        stream = this.m_filePrv.GetStream(StorageUtils.GetOdbFileName(graphId.Value, KnownFileType.Graph));
        RiffChunk result;
        if (!RiffFile.TryLoad(stream, out riff, true) || !riff.ToLookup<RiffChunk, uint>((Func<RiffChunk, uint>) (x => x.Id)).TryGetChunk(1869444462U, out result))
          return 0;
        byte[] buf = new byte[4];
        result.Stream.Position = 0L;
        GitStreamUtil.ReadGreedy(result.Stream, buf, 0, 4);
        return BitConverter.ToInt32(buf, 0);
      }
      finally
      {
        riff?.Dispose();
        stream?.Dispose();
      }
    }

    public bool EnsureUpdated(bool forceRecomputeGraph = false)
    {
      int objectsFromStorage = this.GetNumObjectsFromStorage();
      int objectCount = GitServerUtils.GetContentDB(this.m_odb).ObjectCount;
      if (!forceRecomputeGraph && objectsFromStorage != 0 && (long) objectCount <= (long) objectsFromStorage + 5120L)
        return false;
      GitCommitGraph graph;
      Sha1Id? graphId;
      if (forceRecomputeGraph)
      {
        this.m_rc.TraceAlways(1013887, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitGraphProvider), "Forcing full recomputation of commit graph for ODB {0}", (object) this.m_odbId.OdbId);
        graph = new GitCommitGraph();
        graphId = this.GetGraphId();
      }
      else if (!this.TryLoad(out graph, out graphId, true))
        graph = new GitCommitGraph();
      this.WriteToStorage(graph, graphId);
      if (!this.m_rc.IsServicingContext)
        KeyScopedJobUtil.QueueFor(this.m_rc, this.m_odbId.OdbId, "GitBitmapComputationJob", "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitBitmapComputationJob", JobPriorityLevel.Normal, JobPriorityClass.Normal);
      return true;
    }

    internal void WriteToStorage(GitCommitGraph graph, Sha1Id? existingGraphId)
    {
      using (this.m_rc.TraceBlock(1013563, 1013564, GitServerUtils.TraceArea, nameof (GitGraphProvider), "EnsureUpdated"))
      {
        IGitPackIndex index = (IGitPackIndex) GitServerUtils.GetContentDB(this.m_odb).Index;
        List<Sha1Id> sha1IdList = new List<Sha1Id>();
        int count = index.ObjectIds.Count;
        if (!index.StableObjectOrderEpoch.HasValue)
          return;
        graph.IncrementGenerations();
        for (int objectsAtSerialization = graph.NumObjectsAtSerialization; objectsAtSerialization < count; ++objectsAtSerialization)
        {
          if (index.Entries[objectsAtSerialization].ObjectType == GitPackObjectType.Commit)
            sha1IdList.Add(index.ObjectIds[objectsAtSerialization]);
        }
        this.ExtendGraph(graph, (IEnumerable<Sha1Id>) sha1IdList);
        AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
        List<Sha1Id> requiredCommits = new List<Sha1Id>();
        GitCommitGraph graph1 = graph;
        IEnumerable<int> labels = graph.GetLabels((IEnumerable<Sha1Id>) sha1IdList);
        foreach (int reachableLabel in ancestralGraphAlgorithm.GetReachableLabels((IDirectedGraph<int, Sha1Id>) graph1, labels))
        {
          graph.SetGeneration(reachableLabel, (byte) 0);
          if (index.ObjectIds.GetIndex<Sha1Id>(graph.GetVertex(reachableLabel)) < graph.NumObjectsAtSerialization && graph.GetFilterStatus(reachableLabel) == BloomFilterStatus.NotComputed)
            requiredCommits.Add(graph.GetVertex(reachableLabel));
        }
        PackedBloomFilters changedPathFilter = new PackedBloomFilters(graph.ChangedPathFilters.CurNumLabels);
        GitCommitGraph graph2 = new GitCommitGraph(graph.NumVertices, count, changedPathFilter);
        for (int label1 = 0; label1 < graph.NumVertices; ++label1)
        {
          if (graph.GetGeneration(label1) < (byte) 10)
          {
            Sha1Id vertex = graph.GetVertex(label1);
            graph2.AddVertex(vertex, graph.OutNeighbors(vertex));
            int label2 = graph2.GetLabel(vertex);
            graph2.SetAncestryDepth(label2, graph.GetAncestryDepth(label1));
            graph2.SetRootTreeId(label2, graph.GetRootTreeId(label1));
            graph2.SetCommitTime(label2, graph.GetCommitTime(label1));
            graph2.SetGeneration(label2, graph.GetGeneration(label1));
            BloomFilterStatus filterStatus = graph.GetFilterStatus(label1);
            switch (filterStatus)
            {
              case BloomFilterStatus.NotComputed:
                continue;
              case BloomFilterStatus.Computed:
                if (graph.GetReadOnlyFilter(label1) is BloomFilter readOnlyFilter)
                {
                  changedPathFilter.SetFilter(label2, readOnlyFilter);
                  continue;
                }
                continue;
              default:
                changedPathFilter.SetFilterStatus(label2, filterStatus);
                continue;
            }
          }
        }
        this.ExtendChangedPathFilters(graph2, (IEnumerable<Sha1Id>) requiredCommits);
        GitKnownFilesBuilder knownFiles = new GitKnownFilesBuilder();
        Sha1Id sha1Id1 = GitDataFileUtil.WriteGraph(this.m_rc, this.m_blobPrv, this.m_odbId.OdbId, knownFiles, graph2);
        this.m_knownFilesPrv.Update(knownFiles.GetCreates());
        using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId.OdbId))
        {
          Sha1Id? nullable = gitOdbComponent.UpdatePointer(OdbPointerType.Graph, existingGraphId, new Sha1Id?(sha1Id1));
          Sha1Id sha1Id2 = sha1Id1;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != sha1Id2 ? 1 : 0) : 0) : 1) == 0)
            return;
          this.m_rc.Trace(1013661, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitGraphProvider), "Compare and swap on graph pointer failed. {0}: {1}.", (object) nameof (existingGraphId), (object) existingGraphId);
        }
      }
    }

    private struct Statistics
    {
      public int NumVerticesAdded { get; set; }

      public long CacheMillis { get; set; }

      public long GetLockMillis { get; set; }

      public long ExtendGraphMillis { get; set; }

      public long TotalMillis => this.CacheMillis + this.GetLockMillis + this.ExtendGraphMillis;
    }
  }
}
