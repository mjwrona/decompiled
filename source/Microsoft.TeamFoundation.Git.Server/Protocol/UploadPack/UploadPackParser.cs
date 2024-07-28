// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.UploadPackParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack
{
  public abstract class UploadPackParser
  {
    protected readonly HashSet<Sha1Id> m_wants;
    private readonly ClientTraceData m_ctData;
    private readonly bool m_isOptimized;
    private readonly bool m_isStateless;
    private readonly Odb m_odb;
    private Stream m_output;
    private GitHeartbeatStream m_gitHeartbeatStream;
    private readonly IVssRequestContext m_rc;
    private readonly ITfsGitRepository m_repo;
    private static readonly VssPerformanceCounter s_currentClones = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentClones");
    private static readonly VssPerformanceCounter s_currentFetches = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentFetches");
    private const string c_layer = "UploadPackParser";

    private protected UploadPackParser(
      IVssRequestContext rc,
      Odb odb,
      ITfsGitRepository repo,
      Stream output,
      ClientTraceData ctData,
      bool isOptimized,
      bool isStateless,
      HashSet<Sha1Id> wants)
    {
      this.m_rc = rc;
      this.m_odb = odb;
      this.m_repo = repo;
      this.m_output = output;
      this.m_ctData = ctData;
      this.m_isOptimized = isOptimized;
      this.m_isStateless = isStateless;
      this.m_wants = wants;
    }

    public abstract bool BodyStarted { get; }

    public abstract void UploadPack();

    public abstract void WriteError(string errorMessage);

    private bool IsBitmapSuitable(int numHaves, bool enforceMaxDepth) => numHaves == 0 && !enforceMaxDepth ? this.m_rc.IsFeatureEnabled("Git.Bitmap.UseOnClone") : this.m_rc.IsFeatureEnabled("Git.Bitmap.UseOnFetch");

    internal object TEST_AllWantsConnectedToHaves(
      IReadOnlyCollection<Sha1Id> haves,
      List<Sha1Id> wants)
    {
      return this.RemoveWantsConnectedToHaves(haves, wants, true);
    }

    protected bool AllWantsConnectedToHaves(IReadOnlyCollection<Sha1Id> haves, List<Sha1Id> wants)
    {
      object haves1 = this.RemoveWantsConnectedToHaves(haves, wants, this.m_rc.IsTracing(1013694, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (UploadPackParser)));
      if (haves1 != null)
        this.m_rc.TraceAlways(1013694, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (UploadPackParser), JsonConvert.SerializeObject(haves1));
      return wants.Count == 0;
    }

    private protected void ProcessHave(CommitIdSet haves, TfsGitObject haveObject)
    {
      TfsGitCommit commit = haveObject.TryResolveToCommit();
      DateTime commitTimeIfCommit = commit != null ? commit.GetCommitter().Time : DateTime.MaxValue;
      for (; GitObjectType.Tag == haveObject.ObjectType; haveObject = ((TfsGitTag) haveObject).GetReferencedObject())
        haves.Add(haveObject.ObjectId, commitTimeIfCommit);
      haves.Add(haveObject.ObjectId, commitTimeIfCommit);
    }

    private protected virtual void TransmitPackfile(
      CommitIdSet haves,
      HashSet<Sha1Id> wants,
      Shallows shallows,
      GitObjectFilter filter,
      GitPackFeature clientOptions)
    {
      if (wants.Count == 0)
        return;
      if (this.m_repo.Settings.IsGvfsOnly(this.m_rc, filter))
        throw new GitUploadPackDisabledException();
      this.InitializeHeartbeatStream(clientOptions);
      try
      {
        UploadPackParser.ResolverObserver statusObserver = (UploadPackParser.ResolverObserver) null;
        if ((clientOptions & GitPackFeature.SideBand) != GitPackFeature.None && (clientOptions & GitPackFeature.NoProgress) == GitPackFeature.None)
        {
          this.m_rc.Trace(1013070, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (UploadPackParser), "Logo Written");
          statusObserver = new UploadPackParser.ResolverObserver(this.m_output);
          ProtocolHelper.WriteSidebandLine(this.m_output, SidebandChannel.Output, "Azure Repos");
          if (haves.Count == 0 && this.m_repo.IsFork)
          {
            GitRepositoryRef parent = this.m_rc.GetService<IGitForkService>().GetParent(this.m_rc, this.m_repo.Key);
            if (parent != null)
              ProtocolHelper.WriteSidebandLine(this.m_output, SidebandChannel.Output, Resources.Get("GitCloneForkUpstreamRemoteLine1") + "\n" + Resources.Get("GitCloneForkUpstreamRemoteLine2") + "\n" + "git remote add upstream " + (this.m_isStateless ? parent.RemoteUrl : parent.SshUrl) + "\n");
          }
          if (GitServerUtils.GitClientNeedsUpdate(this.m_rc.UserAgent, this.m_repo.Settings.MinimumRecommendedGitVersion, this.m_repo.Settings.UserAgentExemptions))
            ProtocolHelper.WriteSidebandLine(this.m_output, SidebandChannel.Output, this.m_repo.Settings.UpdateGitMessage);
        }
        IReachableObjectResolver reachableObjectResolver = this.ChooseReachableObjectResolver(haves.Count, shallows.EnforceMaxDepth);
        this.m_rc.Trace(1013071, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (UploadPackParser), "Client has {0} known objects, reachableResolver.GetType() = {1}", (object) haves.Count, (object) reachableObjectResolver?.GetType());
        this.m_ctData?.Add("Action", haves.Count > 0 ? (object) "Fetch" : (object) "Clone");
        this.m_ctData?.Add("ReachableObjectResolverType", (object) reachableObjectResolver?.GetType().ToString());
        this.m_ctData?.Add("EnforceMaxDepth", (object) shallows.EnforceMaxDepth);
        this.m_ctData?.Add("MaxCommitDepth", (object) shallows.Depth);
        this.m_ctData?.Add("NumberOfWants", (object) wants.Count);
        this.m_ctData?.Add("NumberOfHaves", (object) haves.Count);
        this.m_ctData?.Add("NumberOfOldShallowCommits", (object) shallows.ClientShallows.Count);
        if (this.m_rc.IsFeatureEnabled("Git.EnablePartialClone"))
          filter.ReportClientTraceData(this.m_ctData);
        int streamBufferSize = this.m_rc.IsFeatureEnabled("Git.Use32KbStreamBufferSize") ? 32768 : 4096;
        if (reachableObjectResolver != null)
        {
          VssPerformanceCounter performanceCounter = haves.Count > 0 ? UploadPackParser.s_currentFetches : UploadPackParser.s_currentClones;
          performanceCounter.Increment();
          try
          {
            ISet<Sha1Id> foundHaves;
            ISet<Sha1Id> objectsToSend;
            using (this.m_rc.TraceBlock(1013543, 1013544, GitServerUtils.TraceArea, nameof (UploadPackParser), "ReachableResolver"))
              objectsToSend = reachableObjectResolver.Resolve(this.m_repo, (ISet<Sha1Id>) haves.InternalSet, (ISet<Sha1Id>) wants, shallows.ClientShallows, filter, shallows.EnforceMaxDepth, out foundHaves, (IObserver<int>) statusObserver);
            if ((clientOptions & GitPackFeature.IncludeTag) != GitPackFeature.None)
            {
              HashSet<Sha1Id> other = new HashSet<Sha1Id>();
              using (IEnumerator<TfsGitRef> enumerator = this.m_repo.Refs.AllRefTags().GetEnumerator())
              {
label_43:
                while (enumerator.MoveNext())
                {
                  if (this.m_repo.LookupObject(enumerator.Current.ObjectId) is TfsGitTag tfsGitTag)
                  {
                    for (TfsGitObject referencedObject = tfsGitTag.GetReferencedObject(); !objectsToSend.Contains(referencedObject.ObjectId); referencedObject = (referencedObject as TfsGitTag).GetReferencedObject())
                    {
                      if (referencedObject.ObjectType != GitObjectType.Tag)
                        goto label_43;
                    }
                    other.Add(tfsGitTag.ObjectId);
                  }
                }
              }
              objectsToSend.UnionWith((IEnumerable<Sha1Id>) other);
            }
            using (this.m_rc.TraceBlock(1013545, 1013546, GitServerUtils.TraceArea, nameof (UploadPackParser), "WritePack"))
            {
              using (SidebandStream sidebandStream = new SidebandStream(this.m_output, (clientOptions & GitPackFeature.SideBand) == GitPackFeature.None ? SidebandChannel.None : SidebandChannel.Pack, true))
              {
                using (WriteBufferStream output = new WriteBufferStream((Stream) sidebandStream, sidebandStream.MaxLineSize, true))
                {
                  if ((clientOptions & GitPackFeature.ThinPack) == GitPackFeature.None)
                    foundHaves = (ISet<Sha1Id>) null;
                  bool isTarpit;
                  using (GitPackWriterThrottler throttler = new GitPackWriterThrottler(this.m_rc, objectsToSend.Count, out isTarpit))
                  {
                    if (isTarpit)
                      ProtocolHelper.WriteSidebandLine(this.m_output, SidebandChannel.Output, Resources.Get("GitConcurrentCloneTarpitMessage"));
                    new GitPackWriter((ITfsGitContentDB<TfsGitObjectLocation>) this.m_odb.ContentDB, throttler, this.m_ctData, streamBufferSize).Write(objectsToSend, (Stream) output, foundHaves, (Action) (() => this.m_rc.UpdateTimeToFirstPage()), (Predicate<ObjectIdAndGitPackIndexEntry>) null);
                  }
                }
              }
            }
            this.m_ctData?.Add("NumberOfObjects", (object) objectsToSend.Count);
          }
          finally
          {
            performanceCounter.Decrement();
          }
        }
        else
        {
          using (this.m_rc.TraceBlock(1013547, 1013548, GitServerUtils.TraceArea, nameof (UploadPackParser), "WriteClonePack"))
          {
            UploadPackParser.s_currentClones.Increment();
            try
            {
              List<TfsGitObjectLocation> gitObjectLocationList = new List<TfsGitObjectLocation>(this.m_odb.ContentDB.ObjectCount);
              gitObjectLocationList.AddRange(this.m_odb.ContentDB.GetAllObjectLocations());
              this.m_ctData?.Add("NumberOfObjects", (object) gitObjectLocationList.Count);
              gitObjectLocationList.Sort();
              this.m_rc.UpdateTimeToFirstPage();
              using (SidebandStream sidebandStream = new SidebandStream(this.m_output, (clientOptions & GitPackFeature.SideBand) == GitPackFeature.None ? SidebandChannel.None : SidebandChannel.Pack, true))
              {
                using (WriteBufferStream writeBufferStream = new WriteBufferStream((Stream) sidebandStream, sidebandStream.MaxLineSize, true))
                {
                  using (GitPackSerializer gitPackSerializer = new GitPackSerializer((Stream) writeBufferStream, this.m_odb.ContentDB.ObjectCount))
                  {
                    long offset = -1;
                    long length = -1;
                    ushort packIntId = ushort.MaxValue;
                    int objectCount = 0;
                    long num = 0;
                    foreach (TfsGitObjectLocation gitObjectLocation in gitObjectLocationList)
                    {
                      if (offset != -1L && (int) packIntId == (int) gitObjectLocation.PackIntId && offset + length == gitObjectLocation.Offset)
                      {
                        ++objectCount;
                        length += gitObjectLocation.Length;
                      }
                      else
                      {
                        if (offset != -1L)
                        {
                          using (Stream rawContent = this.m_odb.ContentDB.GetRawContent(new TfsGitObjectLocation(packIntId, offset, length)))
                            gitPackSerializer.AddMultipleRaw(rawContent, 0L, length, objectCount);
                        }
                        objectCount = 1;
                        packIntId = gitObjectLocation.PackIntId;
                        offset = gitObjectLocation.Offset;
                        length = gitObjectLocation.Length;
                      }
                      num += gitObjectLocation.Length;
                    }
                    if (offset != -1L)
                    {
                      using (Stream rawContent = this.m_odb.ContentDB.GetRawContent(new TfsGitObjectLocation(packIntId, offset, length)))
                        gitPackSerializer.AddMultipleRaw(rawContent, 0L, length, objectCount);
                    }
                    this.m_ctData?.Add("NumberOfBytes", (object) num);
                    gitPackSerializer.Complete();
                  }
                }
              }
            }
            finally
            {
              UploadPackParser.s_currentClones.Decrement();
            }
          }
        }
        if ((GitPackFeature.SideBand & clientOptions) == GitPackFeature.None)
          return;
        ProtocolHelper.WriteLine(this.m_output, (string) null);
      }
      finally
      {
        this.m_gitHeartbeatStream?.Dispose();
      }
    }

    internal IReachableObjectResolver ChooseReachableObjectResolver(
      int numHaves,
      bool enforceMaxDepth)
    {
      if (numHaves == 0 && this.m_repo.Settings.ForceCloneHack)
        return (IReachableObjectResolver) null;
      if (this.IsBitmapSuitable(numHaves, enforceMaxDepth))
      {
        ICommitReachabilityProvider reachabilityProvider = GitServerUtils.GetOdb(this.m_repo).ReachabilityProvider;
        if (reachabilityProvider != null)
          return (IReachableObjectResolver) new BitmapReachableObjectResolver(this.m_rc, reachabilityProvider);
        using (this.m_rc.AllowAnonymousOrPublicUserWrites(GitSecuredObjectFactory.CreateRepositoryReadOnly(this.m_repo.Key)))
          KeyScopedJobUtil.QueueFor(this.m_rc, this.m_repo.Key.OdbId, "GitBitmapComputationJob", "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitBitmapComputationJob", JobPriorityLevel.Normal, JobPriorityClass.Normal);
      }
      return (IReachableObjectResolver) new ObjectDBReachableObjectResolver2(this.m_rc);
    }

    private object RemoveWantsConnectedToHaves(
      IReadOnlyCollection<Sha1Id> haves,
      List<Sha1Id> wants,
      bool forceStats)
    {
      if (wants.Count == 0 || haves.Count == 0)
        return !forceStats ? (object) null : (object) new
        {
          numWants = wants.Count,
          numHaves = haves.Count
        };
      Stopwatch stopwatch = Stopwatch.StartNew();
      IDirectedGraph<int, Sha1Id> graph = (IDirectedGraph<int, Sha1Id>) this.m_repo.GetCommitGraph((IEnumerable<Sha1Id>) wants);
      List<int> list = haves.Where<Sha1Id>((Func<Sha1Id, bool>) (id => graph.HasVertex(id))).Select<Sha1Id, int>((Func<Sha1Id, int>) (id => graph.GetLabel(id))).ToList<int>();
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>()
      {
        UseGetLabelsThatCanReachNew = this.m_rc.IsFeatureEnabled("Git.GetLabelsThatCanReachNew")
      };
      int count = wants.Count;
      HashSet<int> disconnectedWants = new HashSet<int>(ancestralGraphAlgorithm.GetLabelsThatCanReach(graph, graph.GetLabels((IEnumerable<Sha1Id>) wants), (IEnumerable<int>) list));
      wants.RemoveAll((Predicate<Sha1Id>) (x => disconnectedWants.Contains(graph.GetLabel(x))));
      return forceStats || stopwatch.ElapsedMilliseconds > 1000L ? (object) new
      {
        UseGetLabelsThatCanReachNew = ancestralGraphAlgorithm.UseGetLabelsThatCanReachNew,
        numWantsBefore = count,
        numWantsAfter = wants.Count,
        numHaves = haves.Count,
        numImportantAncestors = list.Count,
        millis = stopwatch.ElapsedMilliseconds
      } : (object) null;
    }

    private void InitializeHeartbeatStream(GitPackFeature clientOptions)
    {
      if (this.m_output == null || (clientOptions & GitPackFeature.SideBand) == GitPackFeature.None)
        return;
      IVssRegistryService service1 = this.m_rc.GetService<IVssRegistryService>();
      IVssRequestContext rc1 = this.m_rc;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/Git/Settings/HeartbeatIntervalMillis";
      ref RegistryQuery local1 = ref registryQuery;
      int heartbeatIntervalMillis = service1.GetValue<int>(rc1, in local1, true, 5000);
      IVssRegistryService service2 = this.m_rc.GetService<IVssRegistryService>();
      IVssRequestContext rc2 = this.m_rc;
      registryQuery = (RegistryQuery) "/Service/Git/Settings/HeartbeatMinimumWriteThresholdMillis";
      ref RegistryQuery local2 = ref registryQuery;
      int minimumWriteThresholdMillis = service2.GetValue<int>(rc2, in local2, true, 100);
      this.m_gitHeartbeatStream = new GitHeartbeatStream(this.m_output, heartbeatIntervalMillis, minimumWriteThresholdMillis);
      this.m_output = (Stream) this.m_gitHeartbeatStream;
    }

    private class ResolverObserver : IObserver<int>
    {
      private readonly Stream m_outputStream;
      private readonly Throttler m_throttler;
      private readonly int m_throttleMilliseconds = 3000;
      private readonly Timer m_heartbeatTimer;
      private bool m_heartbeatTimerEnabled;
      private readonly object m_lockObject = new object();
      private readonly object m_lock = new object();
      private int m_lastValue;

      public ResolverObserver(Stream outputStream)
      {
        this.m_outputStream = outputStream;
        this.m_throttler = new Throttler(this.m_throttleMilliseconds);
        this.m_heartbeatTimer = new Timer(new TimerCallback(this.HeartbeatTimerCallback));
        this.m_heartbeatTimerEnabled = this.m_heartbeatTimer.Change(this.m_throttleMilliseconds, this.m_throttleMilliseconds);
      }

      public void OnCompleted()
      {
        this.DisposeHeartbeatTimer();
        ProtocolHelper.WriteSidebandLine(this.m_outputStream, SidebandChannel.Output, string.Format("\rFound {0} objects to send. ({1} ms)", (object) this.m_lastValue, (object) this.m_throttler.TotalElapsedMilliseconds));
      }

      public void OnError(Exception error) => throw new NotImplementedException();

      public void OnNext(int value)
      {
        lock (this.m_lock)
        {
          this.m_lastValue = value;
          if (!this.m_throttler.IsAfterThreshold())
            return;
          ProtocolHelper.WriteSideband(this.m_outputStream, SidebandChannel.Output, string.Format("\rFound {0} objects to send.", (object) this.m_lastValue));
        }
      }

      private void HeartbeatTimerCallback(object state)
      {
        lock (this.m_lockObject)
        {
          if (!this.m_outputStream.CanWrite)
            return;
          try
          {
            this.OnNext(this.m_lastValue);
          }
          catch
          {
            this.DisposeHeartbeatTimer();
          }
        }
      }

      private void DisposeHeartbeatTimer()
      {
        if (this.m_heartbeatTimer == null)
          return;
        this.m_heartbeatTimer.Dispose();
        this.m_heartbeatTimerEnabled = false;
      }

      ~ResolverObserver() => this.DisposeHeartbeatTimer();
    }
  }
}
