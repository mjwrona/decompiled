// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DefaultGitDependencyRoot
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.Git.Server.Settings;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class DefaultGitDependencyRoot : IGitDependencyRoot
  {
    public static readonly IGitDependencyRoot Instance = (IGitDependencyRoot) new DefaultGitDependencyRoot();

    private DefaultGitDependencyRoot()
    {
    }

    public IGitKnownFilesProvider CreateKnownFilesProvider(IVssRequestContext rc, OdbId odbId) => (IGitKnownFilesProvider) new SqlGitKnownFilesProvider(rc, odbId);

    public TfsGitRepository CreateRepo(
      IVssRequestContext rc,
      string name,
      RepoKey repoKey,
      bool isFork,
      long size,
      bool isDisabled = false,
      bool isInMaintenance = false)
    {
      ITfsGitBlobProvider blobProvider = this.GetBlobProvider(rc);
      IVssRegistryService regSvc = rc.GetService<IVssRegistryService>();
      Lazy<GitOdbSettings> odbSettings = new Lazy<GitOdbSettings>((Func<GitOdbSettings>) (() => new GitOdbSettingsProvider(rc, regSvc, repoKey.OdbId).GetSettings()));
      IGitDataFileProvider dataFilePrv = rc.GetService<GitDataFileProviderService>().Create(rc, repoKey.OdbId, blobProvider, odbSettings);
      IGitKnownFilesProvider knownFilesProvider = this.CreateKnownFilesProvider(rc, repoKey.OdbId);
      Odb odb = this.CreateOdb(rc, repoKey.OdbId, blobProvider, dataFilePrv, knownFilesProvider, odbSettings, regSvc);
      ITeamFoundationPolicyService policySvc = rc.GetService<ITeamFoundationPolicyService>();
      IContributedFeatureService featSvc = rc.GetService<IContributedFeatureService>();
      ILeaseService service = rc.GetService<ILeaseService>();
      Func<GitRepoSettings> repoSettingsFactory = (Func<GitRepoSettings>) (() => new GitRepoSettingsProvider(rc, regSvc, policySvc, featSvc, repoKey).GetFullSettings());
      IsolationBitmapProvider isolationPrv = new IsolationBitmapProvider((Func<ITwoWayReadOnlyList<Sha1Id>>) (() =>
      {
        ConcatGitPackIndex index = odb.ObjectSet.ContentDB.Index;
        return index == null ? (ITwoWayReadOnlyList<Sha1Id>) null : (ITwoWayReadOnlyList<Sha1Id>) index.ObjectIds;
      }), (IRepoBitmapFileProvider) new RepoBitmapFileProvider(rc, repoKey, blobProvider, dataFilePrv, knownFilesProvider, service));
      CherryPickRelationships cherryPickRelationships = new CherryPickRelationships(rc, repoKey, (IGitObjectSet) odb.ObjectSet);
      return new TfsGitRepository(name, repoKey, isFork, size, isDisabled, isInMaintenance, (ICherryPickRelationships) cherryPickRelationships, (IIsolationBitmapProvider) isolationPrv, odb.ObjectSet, odb, rc, repoSettingsFactory);
    }

    public Odb CreateOdb(IVssRequestContext rc, OdbId odbId)
    {
      odbId.CheckValid();
      ITfsGitBlobProvider blobProvider = this.GetBlobProvider(rc);
      IVssRegistryService regSvc = rc.GetService<IVssRegistryService>();
      Lazy<GitOdbSettings> odbSettings = new Lazy<GitOdbSettings>((Func<GitOdbSettings>) (() => new GitOdbSettingsProvider(rc, regSvc, odbId).GetSettings()));
      IGitDataFileProvider dataFilePrv = rc.GetService<GitDataFileProviderService>().Create(rc, odbId, blobProvider, odbSettings);
      IGitKnownFilesProvider knownFilesProvider = this.CreateKnownFilesProvider(rc, odbId);
      return this.CreateOdb(rc, odbId, blobProvider, dataFilePrv, knownFilesProvider, odbSettings, regSvc);
    }

    public Odb CreateOdb(
      IVssRequestContext rc,
      OdbId odbId,
      ITfsGitBlobProvider blobPrv,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilesPrv,
      Lazy<GitOdbSettings> odbSettings,
      IVssRegistryService regSvc)
    {
      GitPackIndexLoader packIndexLoader = new GitPackIndexLoader(rc.RequestTracer, dataFilePrv, knownFilesPrv);
      IGitPackIndexPointerProvider packIndexPtrPrv = this.CreatePackIdxPointerProvider(rc, odbId);
      ILeaseService leaseSvc = rc.GetService<ILeaseService>();
      Func<GitPackIndexTransaction> packIndexTranFactory = (Func<GitPackIndexTransaction>) (() => new GitPackIndexTransaction(rc, odbId, knownFilesPrv, leaseSvc, packIndexPtrPrv, odbSettings));
      ITeamFoundationHostManagementService service1 = rc.GetService<ITeamFoundationHostManagementService>();
      Lazy<ConcatGitPackIndex> index = new Lazy<ConcatGitPackIndex>((Func<ConcatGitPackIndex>) (() => packIndexLoader.LoadIndex(packIndexPtrPrv.GetIndex())), LazyThreadSafetyMode.None);
      IVssRequestContext context = rc.To(TeamFoundationHostType.Deployment);
      CacheKeys.CrossHostOdbId crossHostOdbId = new CacheKeys.CrossHostOdbId(rc.ServiceHost.InstanceId, odbId);
      ContentDB contentDB = new ContentDB(crossHostOdbId, index, dataFilePrv, context.GetService<TfsGitContentCacheService>());
      GitObjectSet objectSet = new GitObjectSet(crossHostOdbId, contentDB, context.GetService<GitObjectCoreCacheService>());
      GitGraphProvider graphPrv = new GitGraphProvider(rc, (IGitObjectSet) objectSet, context.GetService<GitCommitGraphCacheService>(), knownFilesPrv, blobPrv, dataFilePrv, crossHostOdbId);
      Func<PersistentBitmapProvider> bitmapPrvFactory = (Func<PersistentBitmapProvider>) (() => contentDB.Index.StableObjectOrderEpoch.HasValue ? new PersistentBitmapProvider(rc, (IGitObjectSet) objectSet, (IGitGraphProvider) graphPrv, contentDB.Index.StableObjectOrderEpoch.Value, (ITwoWayReadOnlyList<Sha1Id>) contentDB.Index.ObjectIds, (IOdbBitmapFileProvider) new OdbBitmapFileProvider(rc, odbId, blobPrv, dataFilePrv, knownFilesPrv)) : (PersistentBitmapProvider) null);
      ITeamFoundationEventService service2 = rc.GetService<ITeamFoundationEventService>();
      ObjectMetadata objectMetadata = new ObjectMetadata(rc, odbId, (IGitObjectSet) objectSet, service2);
      return new Odb(odbId, bitmapPrvFactory, blobPrv, contentDB, graphPrv, service1, (IObjectMetadata) objectMetadata, objectSet, packIndexLoader, packIndexPtrPrv, packIndexTranFactory, regSvc, rc, odbSettings, knownFilesPrv);
    }

    public IGitPackIndexPointerProvider CreatePackIdxPointerProvider(
      IVssRequestContext rc,
      OdbId odbId)
    {
      return (IGitPackIndexPointerProvider) new SqlGitPackIndexPointerProvider(rc, odbId);
    }

    public ITfsGitBlobProvider GetBlobProvider(IVssRequestContext rc) => rc.GetService<GitBlobProviderService>().BlobProvider;

    public ReceivePackTempRepo CreateReceivePackTempRepo(
      IVssRequestContext rc,
      ITfsGitRepository baseRepo,
      GitReceivePackDeserializer packDeserializer,
      FileBufferedStreamBase packStream,
      IBufferStreamFactory bufferStreamFactory,
      IReadOnlyList<TfsGitRefUpdateRequest> refUpdateRequests)
    {
      IVssRequestContext context = rc.To(TeamFoundationHostType.Deployment);
      return new ReceivePackTempRepo(rc, baseRepo, packDeserializer, packStream, bufferStreamFactory, context.GetService<GitObjectCoreCacheService>(), context.GetService<TfsGitContentCacheService>(), refUpdateRequests);
    }

    public PushPolicyManager CreatePushPolicyManager(
      IVssRequestContext requestContext,
      IReadOnlyList<ITeamFoundationGitPushPolicy> pushPolicies)
    {
      return new PushPolicyManager(requestContext, pushPolicies);
    }

    public GitRepoSizeCalculator CreateSizeCalculator(IVssRequestContext rc) => new GitRepoSizeCalculator(rc, (CreateRor) ((rcx, reach) => (IReachableObjectResolver) new BitmapReachableObjectResolver(rcx, reach)));

    public CodeMigrator CreateRepoMigrator(IVssRequestContext rc)
    {
      return new CodeMigrator(rc.RequestTracer, (IReachableObjectResolver) new ObjectDBReachableObjectResolver2(rc), new Func<ITfsGitRepository, IGitPackWriter>(getPackWriter), new Func<RepoKey, RepoKey, IProgress<ReceivePackStep>>(getProgress), new Func<RepoKey, List<TfsGitRefUpdateRequest>, TfsGitRefUpdateResultSet>(updateRefs));

      static IGitPackWriter getPackWriter(ITfsGitRepository repo) => (IGitPackWriter) new GitPackWriter((ITfsGitContentDB<TfsGitObjectLocation>) GitServerUtils.GetOdb(repo).ContentDB);

      IProgress<ReceivePackStep> getProgress(RepoKey sourceKey, RepoKey targetKey) => (IProgress<ReceivePackStep>) new DefaultGitDependencyRoot.RepoMigratorProgressReporter(rc.RequestTracer, sourceKey, targetKey);

      TfsGitRefUpdateResultSet updateRefs(RepoKey repoKey, List<TfsGitRefUpdateRequest> toUpdate) => rc.GetService<ITeamFoundationGitRefService>().UpdateRefs(rc, repoKey.RepoId, toUpdate, GitRefUpdateMode.AllOrNone);
    }

    private class RepoMigratorProgressReporter : IProgress<ReceivePackStep>
    {
      private readonly RepoKey m_source;
      private readonly RepoKey m_target;
      private readonly ITraceRequest m_tracer;

      public RepoMigratorProgressReporter(ITraceRequest tracer, RepoKey source, RepoKey target)
      {
        this.m_tracer = tracer;
        this.m_source = source;
        this.m_target = target;
      }

      public void Report(ReceivePackStep value) => this.m_tracer.TraceAlways(1013852, TraceLevel.Info, GitServerUtils.TraceArea, "CodeMigrator", string.Format("Repo {0} to {1}: {2} Step {3}", (object) this.m_source, (object) this.m_target, (object) "ReceivePackStep", (object) value));
    }
  }
}
