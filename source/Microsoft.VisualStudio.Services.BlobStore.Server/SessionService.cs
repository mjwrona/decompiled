// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.SessionService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class SessionService : BlobStoreServiceBase, ISessionService, IVssFrameworkService
  {
    protected ConcurrentDictionary<IDomainId, ISessionProvider> sessionProviders = new ConcurrentDictionary<IDomainId, ISessionProvider>();

    protected override string ProductTraceArea => "Session";

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.SetupAzureTableSessionProviders(systemRequestContext);
      foreach (KeyValuePair<IDomainId, ISessionProvider> sessionProvider in this.sessionProviders)
      {
        KeyValuePair<IDomainId, ISessionProvider> item = sessionProvider;
        systemRequestContext.PumpOrInline((Func<VssRequestPump.Processor, Task>) (processor => item.Value.InitializeAsync(processor)), item.Value.ProviderRequireVss);
      }
    }

    private void SetupAzureTableSessionProviders(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.AllowHostDomainAdminOperations())
      {
        foreach (PhysicalDomainInfo physicalDomainInfo in systemRequestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
          this.sessionProviders.TryAdd(physicalDomainInfo.DomainId, this.CreateSessionProvider(systemRequestContext, physicalDomainInfo));
      }
      else
        this.sessionProviders.TryAdd(WellKnownDomainIds.DefaultDomainId, this.CreateSessionProvider(systemRequestContext, (PhysicalDomainInfo) null));
    }

    private ISessionProvider CreateSessionProvider(
      IVssRequestContext systemRequestContext,
      PhysicalDomainInfo physicalDomainInfo)
    {
      IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(systemRequestContext, physicalDomainInfo);
      string defaultTableName1 = SessionService.GetDefaultTableName(systemRequestContext, true);
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(systemRequestContext.GetElevatedDeploymentRequestContext());
      Func<StrongBoxConnectionString, ITableClient> getTableClient = new Func<StrongBoxConnectionString, ITableClient>(systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudTableClientProvider>().GetTableClient);
      LocationMode? locationMode = tableLocationMode;
      string defaultTableName2 = defaultTableName1;
      int num = systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry") ? 1 : 0;
      return (ISessionProvider) new AzureTableSessionProvider((ITableClientFactory) new SessionService.SessionShardingAzureCloudTableClientFactory(connectionStrings, getTableClient, locationMode, defaultTableName2, "ConsistentHashing", num != 0), tableLocationMode);
    }

    public async Task<Guid> CreateSessionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      SessionService sessionService = this;
      Guid id;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704210, nameof (CreateSessionAsync)))
      {
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session = new Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session(requestContext.GetUserId());
        ISessionProvider sessionProvider = sessionService.GetSessionProvider(domainId);
        await requestContext.PumpOrInlineFromAsync((Func<VssRequestPump.Processor, Task>) (processor => sessionProvider.CreateSessionAsync(processor, session.Id, session.Owner, session.Status, session.Expiration, session.ContentId, session.Parts)), sessionProvider.ProviderRequireVss);
        id = session.Id;
      }
      return id;
    }

    public async Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> GetSessionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid sessionId)
    {
      SessionService sessionService = this;
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session sessionAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704220, nameof (GetSessionAsync)))
      {
        ISessionProvider sessionProvider = sessionService.GetSessionProvider(domainId);
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session = await requestContext.PumpOrInlineFromAsync<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session>((Func<VssRequestPump.Processor, Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session>>) (processor => sessionProvider.GetSessionAsync(processor, sessionId)), sessionProvider.ProviderRequireVss);
        if (!session.CanAccess(requestContext.GetUserId()))
          throw new UnauthorizedRequestException(Resources.SessionAccessForbidden(), HttpStatusCode.Unauthorized);
        if (!session.IsValid)
          await sessionService.InValidateSession(requestContext, domainId, session);
        sessionAsync = session;
      }
      return sessionAsync;
    }

    public async Task AbandonSessionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid sessionId)
    {
      SessionService sessionService = this;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704230, nameof (AbandonSessionAsync)))
      {
        // ISSUE: explicit non-virtual call
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session sessionAsync = await __nonvirtual (sessionService.GetSessionAsync(requestContext, domainId, sessionId));
        await sessionService.InValidateSession(requestContext, domainId, sessionAsync);
      }
    }

    public async Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> CompleteSessionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid sessionId)
    {
      SessionService sessionService = this;
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704240, nameof (CompleteSessionAsync)))
      {
        // ISSUE: explicit non-virtual call
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session1 = await __nonvirtual (sessionService.GetSessionAsync(requestContext, domainId, sessionId));
        if (session1.Status != SessionState.Active)
          throw new InvalidSessionOperationException(string.Format("Status: {0} is not Active.", (object) session1.Status));
        List<Part> list1 = session1.Parts.OrderBy<Part, long>((Func<Part, long>) (x => x.From)).ToList<Part>();
        for (int index = 1; index < list1.Count; ++index)
        {
          if (list1[index].From != list1[index - 1].To + 1L)
            throw new InvalidSessionOperationException(string.Format("Missing part {0}-{1}/{2}", (object) (list1[index - 1].To + 1L), (object) (list1[index].From - 1L), (object) list1[index].TotalSize));
          if (index == list1.Count - 1 && list1[index].To != list1[index].TotalSize - 1L)
            throw new InvalidSessionOperationException(string.Format("Missing part {0}-{1}/{2}", (object) (list1[index].To + 1L), (object) (list1[index].TotalSize - 1L), (object) list1[index].TotalSize));
        }
        session1.Status = SessionState.Completed;
        List<DedupIdentifier> list2 = session1.Parts.OrderBy<Part, long>((Func<Part, long>) (x => x.From)).Select<Part, DedupIdentifier>((Func<Part, DedupIdentifier>) (x => x.RootId)).ToList<DedupIdentifier>();
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session2 = session1;
        session2.ContentId = await sessionService.AssembleParts(requestContext, domainId, list2);
        session2 = (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session) null;
        ISessionProvider sessionProvider = sessionService.GetSessionProvider(domainId);
        await requestContext.PumpOrInlineFromAsync((Func<VssRequestPump.Processor, Task>) (processor => sessionProvider.UpdateSessionAsync(processor, session1.Id, session1.Owner, session1.Status, session1.Expiration, session1.ContentId, session1.Parts)), sessionProvider.ProviderRequireVss);
        session = session1;
      }
      return session;
    }

    public async Task AddPartAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid sessionId,
      Part part)
    {
      SessionService sessionService = this;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704250, nameof (AddPartAsync)))
      {
        // ISSUE: explicit non-virtual call
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session = await __nonvirtual (sessionService.GetSessionAsync(requestContext, domainId, sessionId));
        if (session.Status != SessionState.Active)
          throw new InvalidSessionOperationException(string.Format("Status: {0} is not Active.", (object) session.Status));
        List<Part> list1 = session.Parts.Select<Part, Part>((Func<Part, Part>) (s => s)).ToList<Part>();
        list1.Add(part);
        List<Part> list2 = list1.OrderBy<Part, long>((Func<Part, long>) (x => x.From)).ToList<Part>();
        for (int index = 1; index < list2.Count; ++index)
        {
          if (list2[index].From <= list2[index - 1].To)
            throw new InvalidSessionOperationException(string.Format("Part {0}-{1}/{2} already exists.", (object) part.From, (object) part.To, (object) part.TotalSize));
        }
        session.Parts.Add(part);
        ISessionProvider sessionProvider = sessionService.GetSessionProvider(domainId);
        await requestContext.PumpOrInlineFromAsync((Func<VssRequestPump.Processor, Task>) (processor => sessionProvider.UpdateSessionAsync(processor, session.Id, session.Owner, session.Status, session.Expiration, session.ContentId, session.Parts)), sessionProvider.ProviderRequireVss);
      }
    }

    public async Task<IEnumerable<Part>> GetPartsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid sessionId)
    {
      SessionService sessionService = this;
      IEnumerable<Part> parts;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, sessionService.traceData, 5704270, nameof (GetPartsAsync)))
      {
        // ISSUE: explicit non-virtual call
        parts = (IEnumerable<Part>) (await __nonvirtual (sessionService.GetSessionAsync(requestContext, domainId, sessionId))).Parts;
      }
      return parts;
    }

    private static string GetDefaultTableName(IVssRequestContext requestContext, bool withHostId) => BlobStoreProviderConstants.SessionMetadataPrefix + (withHostId ? requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString() : string.Empty);

    private ISessionProvider GetSessionProvider(IDomainId domainId)
    {
      ISessionProvider sessionProvider;
      this.sessionProviders.TryGetValue(domainId, out sessionProvider);
      return sessionProvider;
    }

    private async Task InValidateSession(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session)
    {
      if (session.Status != SessionState.Active)
        return;
      session.Status = SessionState.Expired;
      session.Parts.Clear();
      ISessionProvider sessionProvider = this.GetSessionProvider(domainId);
      await requestContext.PumpOrInlineFromAsync((Func<VssRequestPump.Processor, Task>) (processor => sessionProvider.UpdateSessionAsync(processor, session.Id, session.Owner, session.Status, session.Expiration, session.ContentId, session.Parts)), sessionProvider.ProviderRequireVss);
    }

    private async Task<DedupIdentifier> AssembleParts(
      IVssRequestContext requestContext,
      IDomainId domainId,
      List<DedupIdentifier> rootIds)
    {
      IDedupStore dedupService = requestContext.GetService<IDedupStore>();
      DedupUploadQueue uploadQueue = new DedupUploadQueue(requestContext, domainId, requestContext.CancellationToken);
      DedupTreeBuilder treeBuilder = new DedupTreeBuilder((IDedupProcessingQueue) uploadQueue);
      foreach (DedupIdentifier dedupId in rootIds)
      {
        DedupNode newNode;
        if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
          newNode = new DedupNode(new ChunkInfo(0UL, (uint) (await dedupService.GetChunkAsync(requestContext, domainId, (ChunkDedupIdentifier) dedupId)).Uncompressed.Count, dedupId.AlgorithmResult));
        else
          newNode = DedupNode.Deserialize((await dedupService.GetNodeAsync(requestContext, domainId, (NodeDedupIdentifier) dedupId)).Uncompressed.CreateCopy<byte>());
        treeBuilder.AddNode(newNode);
      }
      DedupIdentifier rootNodeId = treeBuilder.CreateRootNode();
      await uploadQueue.FlushAsync();
      DedupIdentifier dedupIdentifier = rootNodeId;
      dedupService = (IDedupStore) null;
      uploadQueue = (DedupUploadQueue) null;
      treeBuilder = (DedupTreeBuilder) null;
      rootNodeId = (DedupIdentifier) null;
      return dedupIdentifier;
    }

    public class SessionShardingAzureCloudTableClientFactory : ShardingAzureCloudTableClientFactory
    {
      public SessionShardingAzureCloudTableClientFactory(
        IEnumerable<StrongBoxConnectionString> accountConnectionStrings,
        Func<StrongBoxConnectionString, ITableClient> getTableClient,
        LocationMode? locationMode,
        string defaultTableName,
        string shardingStrategy,
        bool enableTracing)
        : base(accountConnectionStrings, getTableClient, locationMode, defaultTableName, shardingStrategy, enableTracing)
      {
      }

      protected override ulong GetKeyForShardHint(string shardHint) => BitConverter.ToUInt64(new Guid(shardHint).ToByteArray(), 0);
    }
  }
}
