// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.ReceivePackParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  public class ReceivePackParser : IGitPushReporter
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly ITfsGitRepository m_repository;
    private readonly Odb m_odb;
    private readonly Stream m_inputStream;
    private Stream m_outputStream;
    private GitHeartbeatStream m_gitHeartbeatStream;
    private readonly ClientTraceData m_ctData;
    private readonly List<TfsGitRefUpdateRequest> m_refUpdateRequests;
    private readonly IGitDependencyRoot m_dependencyRoot;
    private readonly bool m_isStateless;
    private GitPackFeature m_clientFeatures;
    private bool m_attemptedShallow;
    private const string c_layer = "ReceivePackParser";
    private static VssPerformanceCounter s_currentPushes = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentPushes");

    internal ReceivePackParser(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      Stream inputStream,
      Stream outputStream,
      ClientTraceData ctData,
      IGitDependencyRoot dependencyRoot,
      bool isStateless)
    {
      this.m_requestContext = rc;
      this.m_repository = repo;
      this.m_odb = GitServerUtils.GetOdb(repo);
      this.m_inputStream = inputStream;
      this.m_outputStream = outputStream;
      this.m_ctData = ctData;
      this.m_refUpdateRequests = new List<TfsGitRefUpdateRequest>();
      this.m_dependencyRoot = dependencyRoot;
      this.m_isStateless = isStateless;
    }

    public void ReceivePack()
    {
      this.m_requestContext.TraceEnter(1013096, GitServerUtils.TraceArea, nameof (ReceivePackParser), nameof (ReceivePack));
      try
      {
        ReceivePackParser.s_currentPushes.Increment();
        bool expectPackFile = false;
        bool flag = true;
        while (true)
        {
          Sha1Id oldObjectId;
          Sha1Id newObjectId;
          string branchName;
          do
          {
            string str1;
            try
            {
              if ((str1 = ProtocolHelper.ReadLine(this.m_inputStream)) == null)
                goto label_16;
            }
            catch (GitProtocolException ex) when (flag && !this.m_isStateless)
            {
              this.m_requestContext.Trace(1013869, TraceLevel.Info, GitServerUtils.TraceArea, nameof (ReceivePackParser), "Couldn't read first pkt-line: {0}", (object) ex);
              return;
            }
            flag = false;
            if (str1.Substring(0, "shallow".Length) == "shallow")
            {
              this.m_attemptedShallow = true;
            }
            else
            {
              oldObjectId = new Sha1Id(str1.Substring(0, 40));
              newObjectId = new Sha1Id(str1.Substring(41, 40));
              string str2 = str1.Substring(82);
              int length = str2.IndexOf(char.MinValue);
              if (length < 0)
              {
                branchName = str2.Trim();
              }
              else
              {
                string str3 = str2.Substring(length + 1).Trim();
                char[] separator = new char[1]{ ' ' };
                foreach (string featureName in str3.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                  this.m_clientFeatures |= GitPackFeatureHelpers.FeatureFromString(featureName);
                branchName = str2.Substring(0, length).Trim();
              }
              this.m_requestContext.Trace(1013038, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (ReceivePackParser), "RefUpdateRequest {0} {1} {2}", (object) str1.Substring(0, 40), (object) str1.Substring(41, 40), (object) branchName);
            }
          }
          while (string.IsNullOrEmpty(branchName));
          expectPackFile |= this.AddRefUpdateRequest(branchName, oldObjectId, newObjectId);
        }
label_16:
        this.BodyStarted = true;
        this.m_requestContext.UpdateTimeToFirstPage();
        TfsGitRefUpdateResultSet refUpdateResultSet = this.ProcessPackAndRefUpdates(expectPackFile);
        if (GitServerUtils.GitClientNeedsUpdate(this.m_requestContext.UserAgent, this.m_repository.Settings.MinimumRecommendedGitVersion, this.m_repository.Settings.UserAgentExemptions))
          this.WriteOutputLine(this.m_repository.Settings.UpdateGitMessage);
        if (this.m_ctData != null && refUpdateResultSet.PushId.HasValue)
          this.m_ctData.Add("PushId", (object) refUpdateResultSet.PushId);
        this.m_requestContext.Trace(1013111, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (ReceivePackParser), "Push Complete.");
      }
      finally
      {
        this.m_gitHeartbeatStream?.Dispose();
        ReceivePackParser.s_currentPushes.Decrement();
      }
    }

    internal bool AddRefUpdateRequest(string branchName, Sha1Id oldObjectId, Sha1Id newObjectId)
    {
      this.m_refUpdateRequests.Add(new TfsGitRefUpdateRequest(branchName, oldObjectId, newObjectId));
      return !newObjectId.IsEmpty;
    }

    internal TfsGitRefUpdateResultSet ProcessPackAndRefUpdates(
      bool expectPackFile,
      IProgress<ReceivePackStep> stepObserver = null,
      bool applyContentPolicies = true)
    {
      List<TfsIncludedGitCommit> includedGitCommitList = (List<TfsIncludedGitCommit>) null;
      QueuedGitPushJobsContext queuedGitPushJobsContext = (QueuedGitPushJobsContext) null;
      if (expectPackFile)
        (includedGitCommitList, queuedGitPushJobsContext) = this.ProcessPackfile(stepObserver, applyContentPolicies);
      else
        this.InitializeHeartbeatStream();
      this.WritePushReportLine("unpack ok");
      TfsGitRefUpdateResultSet refUpdateResultSet;
      if (this.m_refUpdateRequests.Count > 0)
      {
        stepObserver?.Report(ReceivePackStep.UpdateRefs);
        refUpdateResultSet = this.UpdateRefs(includedGitCommitList, queuedGitPushJobsContext);
      }
      else
        refUpdateResultSet = new TfsGitRefUpdateResultSet();
      stepObserver?.Report(ReceivePackStep.Done);
      return refUpdateResultSet;
    }

    private (List<TfsIncludedGitCommit> includedGitCommits, QueuedGitPushJobsContext queuedGitPushJobsContext) ProcessPackfile(
      IProgress<ReceivePackStep> stepObserver,
      bool applyContentPolicies)
    {
      Lazy<GitPackIndexTransaction> lazy = new Lazy<GitPackIndexTransaction>((Func<GitPackIndexTransaction>) (() => this.m_odb.PackIndexTranFactory()), false);
      bool flag = false;
      FileBufferedStreamBase fileBuffered = this.m_inputStream as FileBufferedStreamBase;
      try
      {
        if (fileBuffered == null)
        {
          flag = true;
          fileBuffered = (FileBufferedStreamBase) new FileBufferedStream(this.m_requestContext, this.m_repository.Key.RepoId, this.m_inputStream, true, this.m_repository.Settings.MaxPushSize);
        }
        this.InitializeHeartbeatStream((Func<bool>) (() => fileBuffered.BufferingComplete));
        ReceivePackParser.DeserializeObserver progressObserver = new ReceivePackParser.DeserializeObserver(this, (Func<bool>) (() => fileBuffered.BufferingComplete), this.m_ctData);
        IObjectOrderer objectOrderer = this.m_odb.ObjectOrdererFactory(false);
        IBufferStreamFactory dataFileProvider = (IBufferStreamFactory) GitServerUtils.GetContentDB(this.m_repository).DataFileProvider;
        PushPolicyManager pushPolicyManager = this.m_dependencyRoot.CreatePushPolicyManager(this.m_requestContext, this.GetApplicablePushPolicies());
        CommitParserOptions commitParserOptions = CommitParserOptions.RejectInvalidComments;
        GitReceivePackDeserializer deserializer = new GitReceivePackDeserializer(this.m_requestContext, (Stream) fileBuffered, dataFileProvider, this.m_repository.Objects, this.m_repository.Settings.ToTreeFsckOptions(), commitParserOptions, this.m_attemptedShallow, this.m_repository.OdbSettings.UnstablePackfileCapSize, objectOrderer, this.m_ctData, pushPolicyManager, (IObserver<GitPackDeserializerProgress>) progressObserver);
        stepObserver?.Report(ReceivePackStep.AnalyzePackFile);
        deserializer.Deserializer.Deserialize();
        this.AddPushCtData(deserializer);
        QueuedGitPushJobsContext gitPushJobsContext = (QueuedGitPushJobsContext) null;
        if (deserializer.ObjectParser.TotalObjectCount > 0)
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          SplitPackResult result = deserializer.Splitter.Result;
          stopwatch.Stop();
          if (applyContentPolicies)
            this.ApplyContentPolicies(this.m_repository, fileBuffered, deserializer, result, dataFileProvider, pushPolicyManager);
          stopwatch.Start();
          stepObserver?.Report(ReceivePackStep.StorePackFile);
          this.WriteOutput("Storing packfile... ");
          this.m_odb.StorePack(lazy.Value.KnownFilesBuilder, result.SplitPacks, (Stream) fileBuffered, fileBuffered.Name);
          this.WriteOutputLine(string.Format("done ({0} ms)", (object) stopwatch.ElapsedMilliseconds));
          this.m_ctData?.Add("StorePackMs", (object) stopwatch.ElapsedMilliseconds);
          stopwatch.Restart();
          stepObserver?.Report(ReceivePackStep.StoreIndexFile);
          this.WriteOutput("Storing index... ");
          GitPackIndexer indexer = result.Indexer;
          List<Sha1Id> sha1IdList = objectOrderer.DequeueAll();
          List<Sha1Id> list1 = sha1IdList.ToList<Sha1Id>();
          this.m_odb.StoreIndex(lazy.Value, indexer, sha1IdList);
          this.WriteOutputLine(string.Format("done ({0} ms)", (object) stopwatch.ElapsedMilliseconds));
          this.m_ctData?.Add("StoreIndexMs", (object) stopwatch.ElapsedMilliseconds);
          if (this.m_requestContext.IsFeatureEnabled("Git.NewOnIndexUpdateAndOnRefsUpdateJobs"))
          {
            string projectUri = this.m_repository.Key.GetProjectUri();
            Guid repoId = this.m_repository.Key.RepoId;
            string name = this.m_repository.Name;
            IdentityDescriptor userContext = this.m_requestContext.UserContext;
            string authenticatedUserName = this.m_requestContext.AuthenticatedUserName;
            string str = this.m_requestContext.RemoteIPAddress();
            IEnumerable<Sha1Id> sha1Ids = deserializer.IncludedCommits.Select<TfsIncludedGitCommit, Sha1Id>((Func<TfsIncludedGitCommit, Sha1Id>) (x => x.CommitId));
            DateTime utcNow = DateTime.UtcNow;
            List<TfsGitRefUpdateResult> refUpdateResults = new List<TfsGitRefUpdateResult>();
            IEnumerable<Sha1Id> includedCommits = sha1Ids;
            DateTime pushTime = utcNow;
            string pusherIpAddress = str;
            PushNotification pushNotification = new PushNotification(projectUri, repoId, name, userContext, authenticatedUserName, (IEnumerable<TfsGitRefUpdateResult>) refUpdateResults, includedCommits, pushTime, 0, pusherIpAddress);
            gitPushJobsContext = this.m_requestContext.GetService<ITeamFoundationGitPushJobService>().QueueOnIndexUpdateJobs(this.m_requestContext, this.m_repository, pushNotification);
          }
          if (this.m_requestContext.IsFeatureEnabled("Git.IsolationBitmap.Write"))
          {
            stopwatch.Restart();
            this.WriteOutput("Updating isolation bitmap... ");
            IIsolationBitmapProvider isolationBitmapProvider = GitServerUtils.GetIsolationBitmapProvider(this.m_repository);
            if (deserializer.ObjectParser.RequiredObjects.Count > 0)
            {
              BitmapReachableObjectResolver reachableObjectResolver = new BitmapReachableObjectResolver(this.m_requestContext, this.m_odb.ReachabilityProvider);
              IReadOnlyBitmap<Sha1Id> iso = isolationBitmapProvider.GetOdb();
              List<Sha1Id> list2 = deserializer.ObjectParser.RequiredObjects.Where<ObjectIdAndType>((Func<ObjectIdAndType, bool>) (x => !iso.Contains(x.ObjectId))).Select<ObjectIdAndType, Sha1Id>((Func<ObjectIdAndType, Sha1Id>) (x => x.ObjectId)).ToList<Sha1Id>();
              ISet<Sha1Id> first = (ISet<Sha1Id>) null;
              if (list2.Count > 0)
                first = reachableObjectResolver.Resolve(this.m_repository, (ISet<Sha1Id>) iso, (ISet<Sha1Id>) list2.ToHashSet<Sha1Id>(), (ICollection<Sha1Id>) Array.Empty<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null);
              isolationBitmapProvider.AddOdbObjectsAndSerialize(first == null ? (IEnumerable<Sha1Id>) list1 : first.Concat<Sha1Id>((IEnumerable<Sha1Id>) list1));
            }
            else
              isolationBitmapProvider.AddOdbObjectsAndSerialize((IEnumerable<Sha1Id>) list1);
            this.WriteOutputLine(string.Format("done ({0} ms)", (object) stopwatch.ElapsedMilliseconds));
            this.m_ctData?.Add("UpdateIsolationBitmapMs", (object) stopwatch.ElapsedMilliseconds);
          }
          ITeamFoundationEventService service = this.m_requestContext.GetService<ITeamFoundationEventService>();
          KnownObjectLengthsNotification lengthsNotification = new KnownObjectLengthsNotification(this.m_repository.Key.OdbId, deserializer.ObjectSizeMap.Take<ObjectIdAndSize>(100000));
          IVssRequestContext requestContext = this.m_requestContext;
          KnownObjectLengthsNotification notificationEvent = lengthsNotification;
          service.PublishNotification(requestContext, (object) notificationEvent);
        }
        return (deserializer.IncludedCommits, gitPushJobsContext);
      }
      catch (Exception ex)
      {
        if (flag)
          fileBuffered?.Dispose();
        if (fileBuffered?.Exception is FileSizeLimitReachedException)
        {
          GitPushRejectedException rejectedException = new GitPushRejectedException(fileBuffered.SizeLimit / 1024L / 1024L);
          this.WritePushReportLine(string.Format("unpack error {0}", (object) rejectedException.Message));
          throw rejectedException;
        }
        if (ex is GitObjectRejectedException)
        {
          using (StringReader stringReader = new StringReader(ex.Message))
            this.WritePushReportLine(string.Format("unpack error {0}", (object) stringReader.ReadLine()));
          throw;
        }
        else
        {
          this.WritePushReportLine(string.Format("unpack error {0}", (object) ex.Message));
          throw;
        }
      }
      finally
      {
        if (flag)
          fileBuffered?.Dispose();
        if (lazy.IsValueCreated)
          lazy.Value.TryExpirePendingExtantAndDispose();
        this.m_requestContext.Trace(1013106, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (ReceivePackParser), "Unpack finished.");
      }
    }

    private void ApplyContentPolicies(
      ITfsGitRepository baseRepo,
      FileBufferedStreamBase packStream,
      GitReceivePackDeserializer deserializer,
      SplitPackResult splitResult,
      IBufferStreamFactory bufferStreamFactory,
      PushPolicyManager pushPolicyManager)
    {
      if (!pushPolicyManager.CanApplyContentPolicies)
        return;
      PolicyEvaluationObserver progressObserver = new PolicyEvaluationObserver(new Action<string>(this.WriteOutput));
      using (ReceivePackTempRepo receivePackTempRepo = this.m_dependencyRoot.CreateReceivePackTempRepo(this.m_requestContext, baseRepo, deserializer, packStream, bufferStreamFactory, (IReadOnlyList<TfsGitRefUpdateRequest>) this.m_refUpdateRequests))
        pushPolicyManager.VerifyGitPackObjectContent(receivePackTempRepo, (IObserver<GitPushPolicyEvaluationProgress>) progressObserver, deserializer.ObjectParser.BlobCount, (Action) (() => this.m_requestContext.RequestContextInternal().CheckCanceled()));
    }

    private IReadOnlyList<ITeamFoundationGitPushPolicy> GetApplicablePushPolicies()
    {
      List<PolicyFailures> failedToInitialize;
      IReadOnlyList<ITeamFoundationGitPushPolicy> applicablePushPolicies = this.m_requestContext.GetService<ITeamFoundationPolicyService>().GetApplicablePushPolicies(this.m_requestContext, this.m_repository.Key.GetProjectUri(), this.m_repository.Key.RepoId, out failedToInitialize, true);
      if (failedToInitialize == null || failedToInitialize.Count <= 0)
        return applicablePushPolicies;
      throw new PolicyImplementationException(failedToInitialize[0].Configuration);
    }

    private void AddPushCtData(GitReceivePackDeserializer deserializer)
    {
      if (this.m_ctData == null)
        return;
      this.m_ctData.Add("NumberOfCommits", (object) deserializer.ObjectParser.CommitCount);
      this.m_ctData.Add("NumberOfTrees", (object) deserializer.ObjectParser.TreeCount);
      this.m_ctData.Add("NumberOfTags", (object) deserializer.ObjectParser.TagCount);
      this.m_ctData.Add("NumberOfBlobs", (object) deserializer.ObjectParser.BlobCount);
      this.m_ctData.Add("NumberOfCommitBytes", (object) deserializer.ObjectParser.TotalCommitBytes);
      this.m_ctData.Add("NumberOfTreeBytes", (object) deserializer.ObjectParser.TotalTreeBytes);
      this.m_ctData.Add("NumberOfTagBytes", (object) deserializer.ObjectParser.TotalTagBytes);
      this.m_ctData.Add("NumberOfObjectsThatAlreadyExisted", (object) deserializer.ObjectParser.ObjectsThatAlreadyExistedCount);
      if (!this.m_requestContext.IsFeatureEnabled("Git.Telemetry.NumberOfExistedBytes"))
        return;
      this.m_ctData.Add("NumberOfExistedBytes", (object) deserializer.ObjectParser.TotalExistedBytes);
    }

    private TfsGitRefUpdateResultSet UpdateRefs(
      List<TfsIncludedGitCommit> includedCommits,
      QueuedGitPushJobsContext queuedGitPushJobsContext)
    {
      TfsGitRefUpdateResultSet refUpdateResultSet = this.m_requestContext.GetService<IInternalGitRefService>().UpdateRefs(this.m_requestContext, this.m_repository.Key.RepoId, this.m_refUpdateRequests, includedCommits, pushReporter: (IGitPushReporter) this, ctData: this.m_ctData, queuedGitPushJobsContext: queuedGitPushJobsContext);
      foreach (TfsGitRefUpdateResult result in refUpdateResultSet.Results)
      {
        if (result.Status == GitRefUpdateStatus.Succeeded)
          this.m_requestContext.Trace(1013042, TraceLevel.Info, GitServerUtils.TraceArea, nameof (ReceivePackParser), "Ref Updated: {0}", (object) result.Name);
        else if (result.Status == GitRefUpdateStatus.SucceededNonExistentRef)
        {
          this.m_requestContext.Trace(1013352, TraceLevel.Info, GitServerUtils.TraceArea, nameof (ReceivePackParser), Resources.Format("WarningNonExistentRefDelete", (object) result.Name));
          this.WriteOutputLine(Resources.Format("WarningNonExistentRefDelete", (object) result.Name));
        }
        else if (result.Status == GitRefUpdateStatus.SucceededCorruptRef)
        {
          this.m_requestContext.Trace(1013353, TraceLevel.Info, GitServerUtils.TraceArea, nameof (ReceivePackParser), Resources.Format("WarningCorruptRefDelete", (object) result.Name));
          this.WriteOutputLine(Resources.Format("WarningCorruptRefDelete", (object) result.Name));
        }
      }
      foreach (TfsGitRefUpdateResult result in refUpdateResultSet.Results)
      {
        if (result.Succeeded)
          this.WritePushReportLine(string.Format("ok {0}", (object) result.Name));
      }
      if (this.m_repository.Refs.GetDefault() == null)
      {
        IEnumerable<TfsGitRefUpdateResult> source = refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (s => s.Succeeded && s.Name.StartsWith("refs/heads/", StringComparison.Ordinal) && !s.NewObjectId.IsEmpty));
        TfsGitRefUpdateResult gitRefUpdateResult = source.FirstOrDefault<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (s => s.Name.Equals("refs/heads/master") && !s.NewObjectId.IsEmpty)) ?? source.FirstOrDefault<TfsGitRefUpdateResult>();
        if (gitRefUpdateResult != null)
        {
          this.m_requestContext.Trace(1013043, TraceLevel.Info, GitServerUtils.TraceArea, nameof (ReceivePackParser), "Setting HEAD to {0}", (object) gitRefUpdateResult.Name);
          this.m_repository.Refs.SetDefault(gitRefUpdateResult.Name);
        }
      }
      return refUpdateResultSet;
    }

    private void InitializeHeartbeatStream(Func<bool> shouldSendHeartbeats = null)
    {
      if (this.m_outputStream == null || (this.m_clientFeatures & GitPackFeature.SideBand) == GitPackFeature.None)
        return;
      IVssRegistryService service1 = this.m_requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = this.m_requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/Git/Settings/HeartbeatIntervalMillis";
      ref RegistryQuery local1 = ref registryQuery;
      int heartbeatIntervalMillis = service1.GetValue<int>(requestContext1, in local1, true, 5000);
      IVssRegistryService service2 = this.m_requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext2 = this.m_requestContext;
      registryQuery = (RegistryQuery) "/Service/Git/Settings/HeartbeatMinimumWriteThresholdMillis";
      ref RegistryQuery local2 = ref registryQuery;
      int minimumWriteThresholdMillis = service2.GetValue<int>(requestContext2, in local2, true, 100);
      this.m_gitHeartbeatStream = new GitHeartbeatStream(this.m_outputStream, heartbeatIntervalMillis, minimumWriteThresholdMillis, shouldSendHeartbeats);
      this.m_outputStream = (Stream) this.m_gitHeartbeatStream;
    }

    public void CompletePushReport()
    {
      if (this.m_outputStream == null)
        return;
      this.WritePushReportLine((string) null);
      ProtocolHelper.WriteLine(this.m_outputStream, (string) null);
    }

    internal void WriteError(string errorMessage)
    {
      if (this.m_outputStream == null)
        return;
      errorMessage = SecretUtility.ScrubSecrets(errorMessage);
      if (this.m_refUpdateRequests == null || this.m_refUpdateRequests.Count == 0)
      {
        if ((GitPackFeature.SideBand & this.m_clientFeatures) != GitPackFeature.None)
          ProtocolHelper.WriteSideband(this.m_outputStream, SidebandChannel.Error, errorMessage);
        else
          ProtocolHelper.WriteLine(this.m_outputStream, errorMessage);
      }
      else
      {
        foreach (TfsGitRefUpdateRequest refUpdateRequest in this.m_refUpdateRequests)
          this.WritePushReportLine(string.Format("ng {0} {1}", (object) refUpdateRequest.Name, (object) errorMessage));
      }
      this.CompletePushReport();
    }

    public void WritePushReportLine(string message)
    {
      if (this.m_outputStream == null || (GitPackFeature.ReportStatus & this.m_clientFeatures) == GitPackFeature.None)
        return;
      if ((GitPackFeature.SideBand & this.m_clientFeatures) != GitPackFeature.None)
        ProtocolHelper.WriteNestedSidebandLine(this.m_outputStream, SidebandChannel.Pack, message);
      else
        ProtocolHelper.WriteLine(this.m_outputStream, message);
    }

    internal void WriteOutput(string message)
    {
      if (this.m_outputStream == null || (GitPackFeature.SideBand & this.m_clientFeatures) == GitPackFeature.None || (GitPackFeature.Quiet & this.m_clientFeatures) != GitPackFeature.None)
        return;
      ProtocolHelper.WriteSideband(this.m_outputStream, SidebandChannel.Output, message);
    }

    public void WriteOutputLine(string line)
    {
      if (this.m_outputStream == null || (GitPackFeature.SideBand & this.m_clientFeatures) == GitPackFeature.None || (GitPackFeature.Quiet & this.m_clientFeatures) != GitPackFeature.None)
        return;
      ProtocolHelper.WriteSidebandLine(this.m_outputStream, SidebandChannel.Output, line);
    }

    public bool BodyStarted { get; private set; }

    private class DeserializeObserver : IObserver<GitPackDeserializerProgress>
    {
      private readonly ReceivePackParser m_parser;
      private readonly Func<bool> m_canReportProgress;
      private readonly ClientTraceData m_ctData;
      private readonly Throttler m_objectReportThrottler;

      public DeserializeObserver(
        ReceivePackParser parser,
        Func<bool> canReportProgress,
        ClientTraceData ctData)
      {
        this.m_parser = parser;
        this.m_canReportProgress = canReportProgress;
        this.m_ctData = ctData;
        this.m_objectReportThrottler = new Throttler(3000);
      }

      public void OnNext(GitPackDeserializerProgress progress)
      {
        if (!this.m_canReportProgress() || !this.m_objectReportThrottler.IsAfterThreshold() && !progress.EnumerationComplete)
          return;
        this.m_parser.WriteOutput(string.Format("\rAnalyzing objects... ({0}/{1})", (object) progress.ObjectsEnumerated, (object) progress.TotalObjects));
        if (!progress.EnumerationComplete)
          return;
        this.m_parser.WriteOutputLine(string.Format(" ({0} ms)", (object) this.m_objectReportThrottler.TotalElapsedMilliseconds));
        this.m_ctData?.Add("AnalyzeObjectsMs", (object) this.m_objectReportThrottler.TotalElapsedMilliseconds);
      }

      public void OnError(Exception error) => throw new NotImplementedException();

      public void OnCompleted() => throw new NotImplementedException();
    }
  }
}
