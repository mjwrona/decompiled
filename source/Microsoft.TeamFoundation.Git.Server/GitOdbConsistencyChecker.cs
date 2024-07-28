// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitOdbConsistencyChecker
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Settings;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitOdbConsistencyChecker
  {
    private readonly IVssRequestContext m_rc;
    private readonly Odb m_odb;
    private readonly ITfsGitContentDB<TfsGitObjectLocation> m_contentDb;
    private readonly ClientTraceService m_ctService;
    private readonly IGitCommitGraph m_graph;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly IGitKnownFilesProvider m_knownFilesProvider;

    public GitOdbConsistencyChecker(
      IVssRequestContext requestContext,
      Odb odb,
      ClientTraceService ctService,
      IGitCommitGraph graph,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilesProvider)
    {
      this.m_rc = requestContext;
      this.m_odb = odb;
      this.m_contentDb = (ITfsGitContentDB<TfsGitObjectLocation>) odb.ObjectSet.ContentDB;
      this.m_ctService = ctService;
      this.m_graph = graph;
      this.m_dataFilePrv = dataFilePrv;
      this.m_knownFilesProvider = knownFilesProvider;
    }

    public void CheckOdb()
    {
      try
      {
        this.EnsureODBConsistency();
        this.EnsureGraphConsistency();
        this.EnsureLfsConsistency();
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("Odb: {0} passed checkOdb.", (object) this.m_odb.Id)));
      }
      catch (Exception ex)
      {
        this.m_rc.TraceAlways(1013697, TraceLevel.Error, GitServerUtils.TraceArea, "GitDataFileProviderService", "Consistency check failed: {0}", (object) ex);
        if (ex is GitFileIsDeletableException)
          this.m_knownFilesProvider.ResetIntervals(this.m_odb.Id);
        throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, ex);
      }
    }

    private void EnsureODBConsistency()
    {
      ClientTraceData clientTraceData = this.InitializeCTData("CheckODBConsistency");
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.FsckOdbContent(clientTraceData);
      stopwatch.Stop();
      clientTraceData.Add("ElapsedTimeMs", (object) stopwatch.ElapsedMilliseconds);
      this.m_ctService.Publish(this.m_rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", clientTraceData);
      this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} passed ODB ", (object) nameof (EnsureODBConsistency), (object) this.m_odb.Id)) + FormattableString.Invariant(FormattableStringFactory.Create("consistency checker.  Time to complete: {0}", (object) stopwatch.Elapsed)));
    }

    private void EnsureLfsConsistency()
    {
      if (!this.m_rc.IsFeatureEnabled("Git.FsckLfs"))
      {
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} skipped by feature flag.", (object) nameof (EnsureLfsConsistency), (object) this.m_odb.Id)));
      }
      else
      {
        ClientTraceData properties = this.InitializeCTData("CheckLfsConsistency");
        Stopwatch stopwatch = Stopwatch.StartNew();
        GitContainerLfsProvider containerLfsProvider = new GitContainerLfsProvider((Func<IVssRequestContext, ITfsGitBlobProvider>) (_ => this.m_odb.BlobProvider));
        bool flag = false;
        long num = 0;
        using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
        {
          foreach (GitLfsObject readAllLfsObject in (IEnumerable<GitLfsObject>) containerLfsProvider.ReadAllLfsObjects(this.m_rc, this.m_odb.Id))
          {
            num += readAllLfsObject.Size;
            using (Stream lfsObject = containerLfsProvider.GetLfsObject(this.m_rc, this.m_odb.Id, readAllLfsObject.ObjectId))
            {
              using (HashingStream<SHA256Managed> hashingStream = new HashingStream<SHA256Managed>(lfsObject, FileAccess.Read))
              {
                GitStreamUtil.EnsureDrained((Stream) hashingStream, byteArray.Bytes);
                Sha256Id sha256Id = new Sha256Id(hashingStream.Hash);
                if (readAllLfsObject.ObjectId != sha256Id)
                {
                  flag = true;
                  this.m_rc.TraceAlways(1013697, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} has potentially corrupt LFS object.", (object) nameof (EnsureLfsConsistency), (object) this.m_odb.Id)) + FormattableString.Invariant(FormattableStringFactory.Create(" Computed: {0} In DB:{1}.", (object) sha256Id, (object) readAllLfsObject.ObjectId)));
                }
              }
            }
          }
        }
        stopwatch.Stop();
        if (flag)
          throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, "One or more LFS objects show corruption.");
        properties.Add("NumberOfBytes", (object) num);
        properties.Add("ElapsedTimeMs", (object) stopwatch.ElapsedMilliseconds);
        this.m_ctService.Publish(this.m_rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} passed LFS ", (object) nameof (EnsureLfsConsistency), (object) this.m_odb.Id)) + FormattableString.Invariant(FormattableStringFactory.Create("consistency checker.  Time to complete: {0}", (object) stopwatch.Elapsed)));
      }
    }

    private void EnsureGraphConsistency()
    {
      if (this.m_graph == null)
      {
        this.m_rc.Trace(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} ", (object) nameof (EnsureGraphConsistency), (object) this.m_odb.Id)) + " No graph was passed in to process.");
      }
      else
      {
        ClientTraceData properties = this.InitializeCTData("CheckGraphConsistency");
        Stopwatch stopwatch = Stopwatch.StartNew();
        string empty = string.Empty;
        foreach (Sha1Id vertex in this.m_graph.GetVertices())
        {
          GitPackObjectType packType;
          if (!this.m_contentDb.TryLookupObject(vertex, out packType, out TfsGitObjectLocation _) || packType != GitPackObjectType.Commit)
            throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, string.Format("Graph commit {0} not found.", (object) vertex));
          TfsGitCommit tfsGitCommit = new TfsGitCommit((ICachedGitObjectSet) this.m_odb.ObjectSet, vertex);
          List<Sha1Id> list1 = tfsGitCommit.GetParents().Select<TfsGitCommit, Sha1Id>((Func<TfsGitCommit, Sha1Id>) (c => c.ObjectId)).ToList<Sha1Id>();
          List<Sha1Id> list2 = this.m_graph.OutNeighbors(vertex).ToList<Sha1Id>();
          if (list1.Except<Sha1Id>((IEnumerable<Sha1Id>) list2).Count<Sha1Id>() > 0 || list2.Except<Sha1Id>((IEnumerable<Sha1Id>) list1).Count<Sha1Id>() > 0)
            throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, "ParentsNeighborsDoNotMatch");
          if (list1.Count > 0)
          {
            if (this.m_graph.GetAncestryDepth(this.m_graph.GetLabel(vertex)) != list1.Max<Sha1Id>((Func<Sha1Id, int>) (parent => this.m_graph.GetAncestryDepth(this.m_graph.GetLabel(parent)))) + 1)
              throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, "Ancestry depth mismatch: depth != (maxParentDepth + 1)");
          }
          if (GitServerConstants.UtcEpoch.Add(TimeSpan.FromSeconds((double) this.m_graph.GetCommitTime(this.m_graph.GetLabel(vertex)))) != tfsGitCommit.GetCommitter().Time)
            throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, "commitTimeGraph != commitTimeCommit");
          if (this.m_graph.GetRootTreeId(this.m_graph.GetLabel(vertex)) != tfsGitCommit.GetTree().ObjectId)
            throw new GitOdbConsistencyCheckerFailedException(this.m_odb.Id, "graphRootTreeId != commitTreeId.ObjectId");
        }
        stopwatch.Stop();
        properties.Add("ElapsedTimeMs", (object) stopwatch.ElapsedMilliseconds);
        this.m_ctService.Publish(this.m_rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
        this.m_rc.TraceAlways(1013697, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitOdbConsistencyChecker), FormattableString.Invariant(FormattableStringFactory.Create("{0} Odb: {1} passed graph", (object) nameof (EnsureGraphConsistency), (object) this.m_odb.Id)) + FormattableString.Invariant(FormattableStringFactory.Create(" consistency checker. Time to complete: {0}", (object) stopwatch.Elapsed)));
      }
    }

    private void FsckOdbContent(ClientTraceData ctData)
    {
      ctData.Add("NumberOfObjects", (object) this.m_contentDb.ObjectCount);
      Dictionary<ushort, GitRepacker.GitPack> knownEntries = new Dictionary<ushort, GitRepacker.GitPack>();
      int index = 0;
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) this.m_odb.ContentDB.Index.Entries)
      {
        GitRepacker.GitPack gitPack;
        if (!knownEntries.TryGetValue(entry.Location.PackIntId, out gitPack))
        {
          gitPack = new GitRepacker.GitPack();
          knownEntries[entry.Location.PackIntId] = gitPack;
        }
        gitPack.Add(new ObjectIdAndGitPackIndexEntry(this.m_odb.ContentDB.Index.ObjectIds[index], entry.ObjectType, entry.Location));
        ++index;
      }
      GitOdbConsistencyChecker.FsckOdbContentStats fsckOdbContentStats;
      if (this.m_rc.IsFeatureEnabled("Git.EnableParallelFsckOdbContent"))
      {
        fsckOdbContentStats = this.ParallelFsckOdbContentEntries(knownEntries);
      }
      else
      {
        fsckOdbContentStats = this.SerialFsckOdbContentEntries(knownEntries);
        ctData.Add("FsckOdbContentThreads", (object) 1);
      }
      fsckOdbContentStats.AddToClientTrace(this.m_rc, ctData);
    }

    private GitOdbConsistencyChecker.FsckOdbContentStats ParallelFsckOdbContentEntries(
      Dictionary<ushort, GitRepacker.GitPack> knownEntries)
    {
      GitOdbSettings odbSettings = new GitOdbSettingsProvider(this.m_rc, this.m_rc.GetService<IVssRegistryService>(), this.m_odb.Id).GetSettings();
      Lazy<GitOdbSettings> lazyOdbSettings = new Lazy<GitOdbSettings>((Func<GitOdbSettings>) (() => odbSettings));
      GitOdbConsistencyChecker.FsckOdbContentStats fsckOdbContentStats = new GitOdbConsistencyChecker.FsckOdbContentStats();
      new ConcurrentOperation<KeyValuePair<ushort, GitRepacker.GitPack>>(this.m_rc, (Action<IVssRequestContext, ConcurrentQueue<KeyValuePair<ushort, GitRepacker.GitPack>>>) ((requestContext, queue) => GitOdbConsistencyChecker.ParallelFsckOdbEachThread(requestContext, queue, this.m_odb.Id, lazyOdbSettings, fsckOdbContentStats)), (IReadOnlyCollection<KeyValuePair<ushort, GitRepacker.GitPack>>) knownEntries, nameof (ParallelFsckOdbContentEntries)).Execute();
      return fsckOdbContentStats;
    }

    private static void ParallelFsckOdbEachThread(
      IVssRequestContext thisThreadContext,
      ConcurrentQueue<KeyValuePair<ushort, GitRepacker.GitPack>> workToDo,
      OdbId odbId,
      Lazy<GitOdbSettings> lazyOdbSettings,
      GitOdbConsistencyChecker.FsckOdbContentStats fsckOdbContentStats)
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
          string packFileName = StorageUtils.GetPackFileName(odb.ContentDB.Index.PackIds[(int) result.Key]);
          using (Stream stream = dataFileProvider.GetStream(packFileName))
          {
            fsckOdbContentStats.AddRepoSizeAtomic(stream.Length);
            GitPackDeserializer forOdbFsck1 = GitPackDeserializer.CreateForOdbFsck(thisThreadContext, odb, stream, false);
            GitPackDeserializerObjectParserTrait forOdbFsck2 = GitPackDeserializerObjectParserTrait.CreateForOdbFsck();
            forOdbFsck1.AddTrait((IGitPackDeserializerTrait) forOdbFsck2);
            forOdbFsck1.AddTrait((IGitPackDeserializerTrait) new GitPackDeserializerAllObjectsExistTrait(thisThreadContext, result.Key, (IEnumerable<ObjectIdAndGitPackIndexEntry>) result.Value, odb.ContentDB.Index.PackStates[(int) result.Key] == GitPackStates.None));
            forOdbFsck1.Deserialize();
            fsckOdbContentStats.AddGitPackDeserializerStatsAtomic(forOdbFsck2);
          }
        }
      }
    }

    private GitOdbConsistencyChecker.FsckOdbContentStats SerialFsckOdbContentEntries(
      Dictionary<ushort, GitRepacker.GitPack> knownEntries)
    {
      GitOdbConsistencyChecker.FsckOdbContentStats fsckOdbContentStats = new GitOdbConsistencyChecker.FsckOdbContentStats();
      foreach (KeyValuePair<ushort, GitRepacker.GitPack> knownEntry in knownEntries)
      {
        using (Stream stream = this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(this.m_odb.ContentDB.Index.PackIds[(int) knownEntry.Key])))
        {
          fsckOdbContentStats.AddRepoSize(stream.Length);
          GitPackDeserializer forOdbFsck1 = GitPackDeserializer.CreateForOdbFsck(this.m_rc, this.m_odb, stream, false);
          GitPackDeserializerObjectParserTrait forOdbFsck2 = GitPackDeserializerObjectParserTrait.CreateForOdbFsck();
          forOdbFsck1.AddTrait((IGitPackDeserializerTrait) forOdbFsck2);
          forOdbFsck1.AddTrait((IGitPackDeserializerTrait) new GitPackDeserializerAllObjectsExistTrait(this.m_rc, knownEntry.Key, (IEnumerable<ObjectIdAndGitPackIndexEntry>) knownEntry.Value, this.m_odb.ContentDB.Index.PackStates[(int) knownEntry.Key] == GitPackStates.None));
          forOdbFsck1.Deserialize();
          fsckOdbContentStats.AddGitPackDeserializerStats(forOdbFsck2);
        }
      }
      return fsckOdbContentStats;
    }

    private ClientTraceData InitializeCTData(string action)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add("Action", (object) action);
      clientTraceData.Add("OdbId", (object) this.m_odb.Id);
      return clientTraceData;
    }

    private struct FsckOdbContentStats
    {
      private long m_repoSize;
      private long m_totalCommitBytes;
      private long m_totalExistedBytes;
      private long m_totalTagBytes;
      private long m_totalTreeBytes;
      private int m_blobCount;
      private int m_commitCount;
      private int m_tagCount;
      private int m_treeCount;

      public long RepoSize => this.m_repoSize;

      public long TotalCommitBytes => this.m_totalCommitBytes;

      public long TotalTagBytes => this.m_totalTagBytes;

      public long TotalTreeBytes => this.m_totalTreeBytes;

      public int BlobCount => this.m_blobCount;

      public int CommitCount => this.m_commitCount;

      public int TagCount => this.m_tagCount;

      public int TreeCount => this.m_treeCount;

      public long TotalExistedBytes => this.m_totalExistedBytes;

      public void AddRepoSize(long repoSize) => this.m_repoSize += repoSize;

      public void AddRepoSizeAtomic(long repoSize) => Interlocked.Add(ref this.m_repoSize, repoSize);

      public void AddGitPackDeserializerStats(GitPackDeserializerObjectParserTrait objectParser)
      {
        this.m_commitCount += objectParser.CommitCount;
        this.m_treeCount += objectParser.TreeCount;
        this.m_tagCount += objectParser.TagCount;
        this.m_blobCount += objectParser.BlobCount;
        this.m_totalCommitBytes += objectParser.TotalCommitBytes;
        this.m_totalTreeBytes += objectParser.TotalTreeBytes;
        this.m_totalTagBytes += objectParser.TotalTagBytes;
        this.m_totalExistedBytes += objectParser.TotalExistedBytes;
      }

      public void AddGitPackDeserializerStatsAtomic(
        GitPackDeserializerObjectParserTrait objectParser)
      {
        Interlocked.Add(ref this.m_commitCount, objectParser.CommitCount);
        Interlocked.Add(ref this.m_treeCount, objectParser.TreeCount);
        Interlocked.Add(ref this.m_tagCount, objectParser.TagCount);
        Interlocked.Add(ref this.m_blobCount, objectParser.BlobCount);
        Interlocked.Add(ref this.m_totalCommitBytes, objectParser.TotalCommitBytes);
        Interlocked.Add(ref this.m_totalTreeBytes, objectParser.TotalTreeBytes);
        Interlocked.Add(ref this.m_totalTagBytes, objectParser.TotalTagBytes);
        Interlocked.Add(ref this.m_totalExistedBytes, objectParser.TotalExistedBytes);
      }

      public void AddToClientTrace(IVssRequestContext requestContext, ClientTraceData ctData)
      {
        ctData.Add("NumberOfBytes", (object) this.RepoSize);
        ctData.Add("NumberOfCommits", (object) this.CommitCount);
        ctData.Add("NumberOfTrees", (object) this.TreeCount);
        ctData.Add("NumberOfTags", (object) this.TagCount);
        ctData.Add("NumberOfBlobs", (object) this.BlobCount);
        ctData.Add("NumberOfCommitBytes", (object) this.TotalCommitBytes);
        ctData.Add("NumberOfTreeBytes", (object) this.TotalTreeBytes);
        ctData.Add("NumberOfTagBytes", (object) this.TotalTagBytes);
        if (!requestContext.IsFeatureEnabled("Git.Telemetry.NumberOfExistedBytes"))
          return;
        ctData.Add("NumberOfExistedBytes", (object) this.TotalExistedBytes);
      }
    }
  }
}
