// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStoreService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DedupStoreService : 
    BlobStoreServiceBase,
    IDedupStore,
    IVssFrameworkService,
    IDedupInfoRetriever,
    IDedupMetadataModifier,
    IDedupRestorer
  {
    private const ulong DefaultBloomFilterSize = 1000000;
    private static readonly TimeSpan ClockSkewBonus = TimeSpan.FromHours(1.0);
    internal static readonly TimeSpan NodeSkewBonus = TimeSpan.FromDays(1.0);
    protected ITableClientFactory disposableTableFactory;
    protected IReceiptSecretProvider receiptSecretProvider;
    public DomainDedupStoreProvider DomainDedupStoreProvider = new DomainDedupStoreProvider(WellKnownDomainIds.DefaultDomainId);
    protected long keepUntilCacheHits;
    protected long keepUntilCacheMisses;

    protected virtual IClock Clock => UtcClock.Instance;

    protected override string ProductTraceArea => "DedupStore";

    private void ConfigureService(IVssRequestContext systemRequestContext)
    {
      Guid instanceId = systemRequestContext.ServiceHost.InstanceId;
      if (this.ServiceStartHostId != instanceId)
        throw new ArgumentException(string.Format("ServiceStart was called with HostId {0} but ConfigureService was called with {1}.", (object) this.ServiceStartHostId, (object) instanceId));
      ElevatedDomainSecurityValidator elevatedValidator = new ElevatedDomainSecurityValidator(systemRequestContext);
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      this.CreateProviders(systemRequestContext);
      List<IDedupProvider> dedupProviderList = new List<IDedupProvider>();
      string str = service.GetValue(systemRequestContext, (RegistryQuery) ServiceRegistryConstants.DedupProviderInitStatus, (string) null);
      List<string> first = new List<string>();
      if (!str.IsNullOrEmpty<char>())
        first = this.DecodeContainerUri(JsonUtilities.Deserialize<List<string>>(str));
      foreach (IDedupProvider dedupProvider in this.DomainDedupStoreProvider.GetProviders().Select(domainDedupProvider => new
      {
        domainDedupProvider = domainDedupProvider,
        dedupProvider = domainDedupProvider.Value
      }).Select(_param1 => _param1.dedupProvider))
      {
        foreach (Uri containerUrl in dedupProvider.GetContainerUrls((IDomainSecurityValidator) elevatedValidator))
        {
          if (!first.Contains(containerUrl.OriginalString))
          {
            dedupProviderList.Add(dedupProvider);
            first.AddRange(dedupProvider.GetContainerUrls((IDomainSecurityValidator) elevatedValidator).Select<Uri, string>((Func<Uri, string>) (uris => uris.OriginalString)));
            break;
          }
        }
      }
      foreach (IDedupProvider dedupProvider1 in dedupProviderList)
      {
        IDedupProvider dedupProvider = dedupProvider1;
        systemRequestContext.PumpOrInline((Func<VssRequestPump.Processor, Task>) (processor => dedupProvider.InitializeAsync(processor, (IDomainSecurityValidator) elevatedValidator)), dedupProvider.ProviderRequireVss);
        List<string> list = first.Union<string>(dedupProvider.GetContainerUrls((IDomainSecurityValidator) elevatedValidator).Select<Uri, string>((Func<Uri, string>) (uris => uris.OriginalString))).ToList<string>();
        service.SetValue<string>(systemRequestContext, ServiceRegistryConstants.DedupProviderInitStatus, this.EncodeContainerUri(list).Serialize<List<string>>());
      }
    }

    private List<string> EncodeContainerUri(List<string> containersList)
    {
      List<string> stringList = new List<string>();
      foreach (string containers in containersList)
      {
        byte[] bytes = Encoding.ASCII.GetBytes(containers);
        stringList.Add(Convert.ToBase64String(bytes));
      }
      return stringList;
    }

    private List<string> DecodeContainerUri(List<string> containersList)
    {
      List<string> stringList = new List<string>();
      foreach (string containers in containersList)
      {
        byte[] bytes = Convert.FromBase64String(containers);
        stringList.Add(Encoding.ASCII.GetString(bytes));
      }
      return stringList;
    }

    protected virtual IDictionary<IDomainId, IDedupProvider> CreateProviders(
      IVssRequestContext systemRequestContext)
    {
      string str = systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/DedupProviderImplementation", true, "AZUREBLOBTABLEDEDUPPROVIDER");
      string blobContainerName = this.GetBlobContainerName(systemRequestContext);
      switch (str)
      {
        case "MEMORYBLOBTABLEDEDUPPROVIDER":
          this.DomainDedupStoreProvider.AddDedupProvider(WellKnownDomainIds.DefaultDomainId, (IDedupProvider) new AzureBlobTableDedupProvider((IEnumerable<ICloudBlobContainer>) new MemoryBlobContainerReference[1]
          {
            new MemoryBlobContainerReference(MemoryBlobAccount.Global, blobContainerName)
          }, (ITableClientFactory) new MemoryTableClientFactory(MemoryTableStorage.Global, DedupStoreService.GetDefaultTableName(systemRequestContext)), new LocationMode?(), this.Clock, WellKnownDomainIds.DefaultDomainId));
          break;
        case "AZUREBLOBTABLEDEDUPPROVIDER":
          this.SetupAzureBlobTableProviders(systemRequestContext, blobContainerName);
          break;
        default:
          throw new NotImplementedException("DedupStoreService at CreateProviders unknown provider implementation: " + str + ".");
      }
      this.receiptSecretProvider = this.CreateReceiptSecretProvider(systemRequestContext);
      this.RegisterStrongBoxChangedEvents(systemRequestContext, new StrongBoxChangeHandlerMapping("ConfigurationSecrets", "PrimaryChunkDedupReceiptKey"), (ISecretItemChangeListener) this.receiptSecretProvider);
      this.RegisterStrongBoxChangedEvents(systemRequestContext, new StrongBoxChangeHandlerMapping("ConfigurationSecrets", "SecondaryChunkDedupReceiptKey"), (ISecretItemChangeListener) this.receiptSecretProvider);
      return (IDictionary<IDomainId, IDedupProvider>) this.DomainDedupStoreProvider.GetProviders();
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      this.disposableTableFactory?.Dispose();
    }

    protected virtual string GetBlobContainerName(IVssRequestContext systemRequestContext) => BlobStoreProviderConstants.DedupContainerPrefix + systemRequestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString();

    protected virtual ITableClientFactory GetTableFactory(
      IVssRequestContext systemRequestContext,
      PhysicalDomainInfo physicalDomainInfo = null)
    {
      IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(systemRequestContext, physicalDomainInfo);
      string defaultTableName1 = DedupStoreService.GetDefaultTableName(systemRequestContext, true);
      IAzureCloudTableClientProvider service = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudTableClientProvider>();
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(systemRequestContext.GetElevatedDeploymentRequestContext());
      Func<StrongBoxConnectionString, ITableClient> getTableClient = new Func<StrongBoxConnectionString, ITableClient>(service.GetTableClient);
      LocationMode? locationMode = tableLocationMode;
      string defaultTableName2 = defaultTableName1;
      int num = systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry") ? 1 : 0;
      return (ITableClientFactory) new DedupStoreService.DedupShardingAzureCloudTableClientFactory(connectionStrings, getTableClient, locationMode, defaultTableName2, "ConsistentHashing", num != 0);
    }

    protected virtual IReceiptSecretProvider CreateReceiptSecretProvider(
      IVssRequestContext systemRequestContext)
    {
      return (IReceiptSecretProvider) new ReceiptSecretProvider(systemRequestContext);
    }

    private static string GetDefaultTableName(IVssRequestContext requestContext, bool withHostId) => "dedup" + (withHostId ? requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString() : string.Empty);

    private static string GetDefaultTableName(IVssRequestContext requestContext) => "dedup" + requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString();

    protected override void ServiceStart(IVssRequestContext systemRequestContext) => this.ConfigureService(systemRequestContext);

    public async Task<DedupCompressedBuffer> GetChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId)
    {
      DedupCompressedBuffer chunkAsync;
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.GetChunksAsyncCall, nameof (GetChunkAsync)))
      {
        try
        {
          chunkAsync = await this.GetDedupAsyncInternal(requestContext, domainId, (DedupIdentifier) chunkId).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.GetChunksAsyncException, ex);
          throw;
        }
      }
      return chunkAsync;
    }

    public async Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil)
    {
      KeepUntilReceipt keepUntilReceipt;
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.PutChunkAndKeepUntilReferenceAsyncCall, nameof (PutChunkAndKeepUntilReferenceAsync)))
      {
        try
        {
          this.CheckKeepUntilReference(keepUntil);
          ArraySegment<byte> uncompressed = chunk.Uncompressed;
          if (uncompressed.Count > ChunkerHelper.GetMaxChunkContentSize())
          {
            uncompressed = chunk.Uncompressed;
            throw new ArgumentException(string.Format("Chunk is too large. (SizeInBytes:{0}, MaxSizeInBytes:{1})", (object) uncompressed.Count, (object) ChunkerHelper.GetMaxChunkContentSize()));
          }
          if ((DedupIdentifier) chunkId != (DedupIdentifier) chunk.ChunkIdentifier)
            throw new ArgumentException(string.Format("{0} does not match {1}. (Expected: {2}, Actual: {3})", (object) nameof (chunkId), (object) nameof (chunk), (object) chunkId.ValueString, (object) chunk.ChunkIdentifier));
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          DateTime dateTime = await this.PumpOrInlineFromAsync<DateTime>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DateTime>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).PutDedupAndKeepUntilReferenceAsync(processor, (DedupIdentifier) chunkId, chunk, keepUntil.KeepUntil, useHttpClient))).ConfigureAwait(true);
          this.GetKeepUntilCache(requestContext).SetKeepUntil(requestContext, domainId, (DedupIdentifier) chunkId, dateTime);
          keepUntilReceipt = KeepUntilReceipt.Create(this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, requestContext.ServiceHost.InstanceId, (DedupIdentifier) chunkId, new KeepUntilBlobReference(dateTime));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.PutChunkAndKeepUntilReferenceAsyncException, ex);
          throw;
        }
      }
      return keepUntilReceipt;
    }

    protected IDedupKeepUntilCacheService GetKeepUntilCache(IVssRequestContext requestContext) => requestContext.GetService<IDedupKeepUntilCacheService>();

    public async Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.TryKeepUntilReferenceChunkAsyncCall, nameof (TryKeepUntilReferenceChunkAsync)))
      {
        try
        {
          this.CheckKeepUntilReference(keepUntil);
          IDedupKeepUntilCacheService keepUntilCache = this.GetKeepUntilCache(requestContext);
          DateTime keepUntil1;
          if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) chunkId, out keepUntil1) && keepUntil1 >= keepUntil.KeepUntil)
          {
            Interlocked.Increment(ref this.keepUntilCacheHits);
            return KeepUntilReceipt.Create(this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, requestContext.ServiceHost.InstanceId, (DedupIdentifier) chunkId, new KeepUntilBlobReference(keepUntil1));
          }
          Interlocked.Increment(ref this.keepUntilCacheMisses);
          DateTime? nullable = await this.PumpOrInlineFromAsync<DateTime?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DateTime?>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).TryAddKeepUntilReferenceAsync(processor, (DedupIdentifier) chunkId, keepUntil.KeepUntil))).ConfigureAwait(true);
          if (!nullable.HasValue)
            return (KeepUntilReceipt) null;
          keepUntilCache.SetKeepUntil(requestContext, domainId, (DedupIdentifier) chunkId, nullable.Value);
          return KeepUntilReceipt.Create(this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, requestContext.ServiceHost.InstanceId, (DedupIdentifier) chunkId, new KeepUntilBlobReference(nullable.Value));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.TryKeepUntilReferenceChunkAsyncException, ex);
          throw;
        }
      }
    }

    public async Task<DedupCompressedBuffer> GetNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId)
    {
      DedupCompressedBuffer nodeAsync;
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.GetNodeAsyncCall, nameof (GetNodeAsync)))
      {
        try
        {
          nodeAsync = await this.GetDedupAsyncInternal(requestContext, domainId, (DedupIdentifier) nodeId).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.GetNodeAsyncException, ex);
          throw;
        }
      }
      return nodeAsync;
    }

    public async Task<DateTime> GetKeepUntilAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      return await this.PumpOrInlineFromAsync<DateTime>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DateTime>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetKeepUntilAsync(processor, dedupId))).ConfigureAwait(true);
    }

    public async Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer nodeBytes,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.PutNodeAndKeepUntilReferenceAsyncCall, nameof (PutNodeAndKeepUntilReferenceAsync)))
      {
        try
        {
          this.CheckKeepUntilReference(keepUntil);
          DedupNode node = DedupNode.Deserialize(nodeBytes.Uncompressed.CreateCopy<byte>());
          if (node.Type != DedupNode.NodeType.InnerNode)
            throw new ArgumentException("The value does not represent an inner node.");
          if (!((IEnumerable<byte>) node.Serialize()).SequenceEqual<byte>((IEnumerable<byte>) nodeBytes.Uncompressed.CreateCopy<byte>()))
            throw new ArgumentException("Node does not round-trip.");
          DedupIdentifier dedupIdentifier = DedupIdentifier.Create(node.Hash, nodeId.AlgorithmId);
          if ((DedupIdentifier) nodeId != dedupIdentifier)
            throw new ArgumentException("nodeId does not match node. (Actual: " + nodeId.ValueString + ", Expected: " + dedupIdentifier.ValueString + ")");
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          DedupIdentifier[] array = node.ChildNodes.Select<DedupNode, DedupIdentifier>(DedupStoreService.\u003C\u003EO.\u003C0\u003E__Create ?? (DedupStoreService.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))).ToArray<DedupIdentifier>();
          Dictionary<DedupIdentifier, DateTime> validatedReceipts = this.ValidateReceipts(requestContext, array, keepUntil, receipt);
          IDedupKeepUntilCacheService keepUntilCache = this.GetKeepUntilCache(requestContext);
          DateTime keepUntil1;
          if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) nodeId, out keepUntil1) && keepUntil1 >= keepUntil.KeepUntil)
          {
            Interlocked.Increment(ref this.keepUntilCacheHits);
            return new PutNodeResponse(new DedupNodeUpdated(this.CreateReceiptDictionary(requestContext, (DedupIdentifier) nodeId, keepUntil1)));
          }
          Interlocked.Increment(ref this.keepUntilCacheMisses);
          KeepUntilResult? nullable1 = await this.PumpOrInlineFromAsync<KeepUntilResult?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<KeepUntilResult?>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).ValidateKeepUntilReferenceAsync(processor, (DedupIdentifier) nodeId, keepUntil.KeepUntil))).ConfigureAwait(true);
          if (nullable1.HasValue && nullable1.Value.IsSatisfied)
          {
            keepUntilCache.SetKeepUntil(requestContext, domainId, (DedupIdentifier) nodeId, nullable1.Value.BestAvailableKeepUntil);
            return new PutNodeResponse(new DedupNodeUpdated(this.CreateReceiptDictionary(requestContext, (DedupIdentifier) nodeId, nullable1.Value.BestAvailableKeepUntil)));
          }
          List<DedupIdentifier> missingChildren = new List<DedupIdentifier>();
          List<DedupIdentifier> insufficientKeepUntilChildren = new List<DedupIdentifier>();
          Dictionary<DedupIdentifier, DateTime> keepUntilsToCache = new Dictionary<DedupIdentifier, DateTime>();
          Dictionary<DedupIdentifier, DateTime> keepUntilsToReturn = new Dictionary<DedupIdentifier, DateTime>();
          HashSet<DedupIdentifier> chunkDedupIdsToQuery = new HashSet<DedupIdentifier>();
          foreach (ChunkDedupIdentifier chunkDedupIdentifier in node.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (n => n.Type == DedupNode.NodeType.ChunkLeaf)).Select<DedupNode, ChunkDedupIdentifier>((Func<DedupNode, ChunkDedupIdentifier>) (c => new ChunkDedupIdentifier(c.Hash))).Distinct<ChunkDedupIdentifier>())
          {
            if (validatedReceipts.ContainsKey((DedupIdentifier) chunkDedupIdentifier))
              keepUntilsToCache.Add((DedupIdentifier) chunkDedupIdentifier, validatedReceipts[(DedupIdentifier) chunkDedupIdentifier]);
            else if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) chunkDedupIdentifier, out keepUntil1) && keepUntil1 >= keepUntil.KeepUntil)
            {
              Interlocked.Increment(ref this.keepUntilCacheHits);
              keepUntilsToReturn.Add((DedupIdentifier) chunkDedupIdentifier, keepUntil1);
            }
            else
            {
              Interlocked.Increment(ref this.keepUntilCacheMisses);
              chunkDedupIdsToQuery.Add((DedupIdentifier) chunkDedupIdentifier);
            }
          }
          IDictionary<DedupIdentifier, DateTime?> dictionary1 = await this.PumpOrInlineFromAsync<IDictionary<DedupIdentifier, DateTime?>>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<IDictionary<DedupIdentifier, DateTime?>>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).TryAddKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) chunkDedupIdsToQuery, keepUntil.KeepUntil))).ConfigureAwait(true);
          foreach (DedupIdentifier key in chunkDedupIdsToQuery)
          {
            DateTime? nullable2 = dictionary1[key];
            if (nullable2.HasValue)
            {
              keepUntilsToCache.Add(key, nullable2.Value);
              keepUntilsToReturn.Add(key, nullable2.Value);
            }
            else
              missingChildren.Add(key);
          }
          HashSet<DedupIdentifier> nodeDedupIdsToQuery = new HashSet<DedupIdentifier>();
          foreach (NodeDedupIdentifier nodeDedupIdentifier in node.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (n => n.Type == DedupNode.NodeType.InnerNode)).Select<DedupNode, NodeDedupIdentifier>((Func<DedupNode, NodeDedupIdentifier>) (c => new NodeDedupIdentifier(c.Hash))).Distinct<NodeDedupIdentifier>())
          {
            if (validatedReceipts.ContainsKey((DedupIdentifier) nodeDedupIdentifier))
              keepUntilsToCache.Add((DedupIdentifier) nodeDedupIdentifier, validatedReceipts[(DedupIdentifier) nodeDedupIdentifier]);
            else if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) nodeDedupIdentifier, out keepUntil1) && keepUntil1 >= keepUntil.KeepUntil)
            {
              Interlocked.Increment(ref this.keepUntilCacheHits);
              keepUntilsToReturn.Add((DedupIdentifier) nodeDedupIdentifier, keepUntil1);
            }
            else
            {
              Interlocked.Increment(ref this.keepUntilCacheMisses);
              nodeDedupIdsToQuery.Add((DedupIdentifier) nodeDedupIdentifier);
            }
          }
          IDictionary<DedupIdentifier, KeepUntilResult?> dictionary2 = await this.PumpOrInlineFromAsync<IDictionary<DedupIdentifier, KeepUntilResult?>>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<IDictionary<DedupIdentifier, KeepUntilResult?>>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).ValidateKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) nodeDedupIdsToQuery, keepUntil.KeepUntil))).ConfigureAwait(true);
          foreach (DedupIdentifier key in nodeDedupIdsToQuery)
          {
            KeepUntilResult? nullable3 = dictionary2[key];
            if (nullable3.HasValue && nullable3.Value.IsSatisfied)
            {
              keepUntilsToCache.Add(key, nullable3.Value.BestAvailableKeepUntil);
              keepUntilsToReturn.Add(key, nullable3.Value.BestAvailableKeepUntil);
            }
            else if (nullable3.HasValue)
              insufficientKeepUntilChildren.Add(key);
            else
              missingChildren.Add(key);
          }
          if (missingChildren.Any<DedupIdentifier>() || insufficientKeepUntilChildren.Any<DedupIdentifier>())
          {
            if (receipt != null && ((IEnumerable<KeepUntilBlobReference?>) receipt.KeepUntils).All<KeepUntilBlobReference?>((Func<KeepUntilBlobReference?, bool>) (ku => ku.HasValue)))
              throw new Exception("PutNode should not require action if all receipts are provided! (Missing: " + string.Join<DedupIdentifier>(",", (IEnumerable<DedupIdentifier>) missingChildren) + ", InsufficientKeepUntil: " + string.Join<DedupIdentifier>(",", (IEnumerable<DedupIdentifier>) insufficientKeepUntilChildren) + ")");
            return new PutNodeResponse(new DedupNodeChildrenNeedAction(missingChildren.ToArray(), insufficientKeepUntilChildren.ToArray(), this.ToReceipts(requestContext, keepUntilsToReturn)));
          }
          DateTime keepUntilToSet = keepUntilsToCache.Values.Concat<DateTime>((IEnumerable<DateTime>) keepUntilsToReturn.Values).Concat<DateTime>((IEnumerable<DateTime>) new DateTime[1]
          {
            keepUntil.KeepUntil + DedupStoreService.NodeSkewBonus
          }).Min<DateTime>();
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          DateTime dateTime = await this.PumpOrInlineFromAsync<DateTime>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DateTime>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).PutDedupAndKeepUntilReferenceAsync(processor, (DedupIdentifier) nodeId, nodeBytes, keepUntilToSet, useHttpClient))).ConfigureAwait(true);
          keepUntilsToCache.Add((DedupIdentifier) nodeId, dateTime);
          keepUntilsToReturn.Add((DedupIdentifier) nodeId, dateTime);
          foreach (KeyValuePair<DedupIdentifier, DateTime> keyValuePair in keepUntilsToCache)
            keepUntilCache.SetKeepUntil(requestContext, domainId, keyValuePair.Key, keyValuePair.Value);
          return new PutNodeResponse(new DedupNodeUpdated(this.ToReceipts(requestContext, keepUntilsToReturn), true));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.PutNodeAndKeepUntilReferenceAsyncException, ex);
          throw;
        }
      }
    }

    public async Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.TryKeepUntilReferenceChunkAsyncCall, nameof (TryKeepUntilReferenceNodeAsync)))
      {
        try
        {
          this.CheckKeepUntilReference(keepUntil);
          IDedupKeepUntilCacheService keepUntilCache = this.GetKeepUntilCache(requestContext);
          KeepUntilResult? nullable1 = await this.ValidateKeepUntilAndUpdateCacheAsync(requestContext, domainId, (DedupIdentifier) nodeId, keepUntil.KeepUntil).ConfigureAwait(true);
          if (!nullable1.HasValue)
            return new TryReferenceNodeResponse(new DedupNodeNotFound());
          if (nullable1.Value.IsSatisfied)
            return new TryReferenceNodeResponse(new DedupNodeUpdated(this.CreateReceiptDictionary(requestContext, (DedupIdentifier) nodeId, nullable1.Value.BestAvailableKeepUntil)));
          DedupNode? node = await this.PumpOrInlineFromAsync<DedupNode?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DedupNode?>>) (processor => this.GetDedupNodeAsync(processor, nodeId))).ConfigureAwait(true);
          if (!node.HasValue)
            return new TryReferenceNodeResponse(new DedupNodeNotFound());
          DedupIdentifier[] array = node.Value.ChildNodes.Select<DedupNode, DedupIdentifier>((Func<DedupNode, DedupIdentifier>) (n => DedupIdentifier.Create(n))).ToArray<DedupIdentifier>();
          Dictionary<DedupIdentifier, DateTime> validatedReceipts = this.ValidateReceipts(requestContext, array, keepUntil, receipt);
          HashSet<DedupIdentifier> childrenNeedAction = new HashSet<DedupIdentifier>();
          Dictionary<DedupIdentifier, DateTime> keepUntilsToCache = new Dictionary<DedupIdentifier, DateTime>();
          Dictionary<DedupIdentifier, DateTime> keepUntilsToReturn = new Dictionary<DedupIdentifier, DateTime>();
          HashSet<DedupIdentifier> chunkDedupIdsToQuery = new HashSet<DedupIdentifier>();
          foreach (ChunkDedupIdentifier chunkDedupIdentifier in node.Value.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (n => n.Type == DedupNode.NodeType.ChunkLeaf)).Select<DedupNode, ChunkDedupIdentifier>((Func<DedupNode, ChunkDedupIdentifier>) (c => new ChunkDedupIdentifier(c.Hash))).Distinct<ChunkDedupIdentifier>())
          {
            if (validatedReceipts.ContainsKey((DedupIdentifier) chunkDedupIdentifier))
            {
              keepUntilsToCache.Add((DedupIdentifier) chunkDedupIdentifier, validatedReceipts[(DedupIdentifier) chunkDedupIdentifier]);
            }
            else
            {
              DateTime keepUntil1;
              if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) chunkDedupIdentifier, out keepUntil1) && keepUntil1 >= keepUntil.KeepUntil)
              {
                Interlocked.Increment(ref this.keepUntilCacheHits);
                keepUntilsToReturn.Add((DedupIdentifier) chunkDedupIdentifier, keepUntil1);
              }
              else
              {
                Interlocked.Increment(ref this.keepUntilCacheMisses);
                chunkDedupIdsToQuery.Add((DedupIdentifier) chunkDedupIdentifier);
              }
            }
          }
          IDictionary<DedupIdentifier, DateTime?> dictionary1 = await this.PumpOrInlineFromAsync<IDictionary<DedupIdentifier, DateTime?>>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<IDictionary<DedupIdentifier, DateTime?>>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).TryAddKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) chunkDedupIdsToQuery, keepUntil.KeepUntil))).ConfigureAwait(true);
          foreach (DedupIdentifier key in chunkDedupIdsToQuery)
          {
            DateTime? nullable2 = dictionary1[key];
            if (nullable2.HasValue)
            {
              keepUntilsToCache.Add(key, nullable2.Value);
              keepUntilsToReturn.Add(key, nullable2.Value);
            }
            else
              childrenNeedAction.Add(key);
          }
          HashSet<DedupIdentifier> nodeDedupIdsToQuery = new HashSet<DedupIdentifier>();
          foreach (NodeDedupIdentifier nodeDedupIdentifier in node.Value.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (n => n.Type == DedupNode.NodeType.InnerNode)).Select<DedupNode, NodeDedupIdentifier>((Func<DedupNode, NodeDedupIdentifier>) (c => new NodeDedupIdentifier(c.Hash))).Distinct<NodeDedupIdentifier>())
          {
            if (validatedReceipts.ContainsKey((DedupIdentifier) nodeDedupIdentifier))
            {
              keepUntilsToCache.Add((DedupIdentifier) nodeDedupIdentifier, validatedReceipts[(DedupIdentifier) nodeDedupIdentifier]);
            }
            else
            {
              DateTime keepUntil2;
              if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, (DedupIdentifier) nodeDedupIdentifier, out keepUntil2) && keepUntil2 >= keepUntil.KeepUntil)
              {
                Interlocked.Increment(ref this.keepUntilCacheHits);
                keepUntilsToReturn.Add((DedupIdentifier) nodeDedupIdentifier, keepUntil2);
              }
              else
              {
                Interlocked.Increment(ref this.keepUntilCacheMisses);
                nodeDedupIdsToQuery.Add((DedupIdentifier) nodeDedupIdentifier);
              }
            }
          }
          IDictionary<DedupIdentifier, KeepUntilResult?> dictionary2 = await this.PumpOrInlineFromAsync<IDictionary<DedupIdentifier, KeepUntilResult?>>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<IDictionary<DedupIdentifier, KeepUntilResult?>>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).ValidateKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) nodeDedupIdsToQuery, keepUntil.KeepUntil))).ConfigureAwait(true);
          foreach (DedupIdentifier key in nodeDedupIdsToQuery)
          {
            KeepUntilResult? nullable3 = dictionary2[key];
            if (nullable3.HasValue && nullable3.Value.IsSatisfied)
            {
              keepUntilsToCache.Add(key, nullable3.Value.BestAvailableKeepUntil);
              keepUntilsToReturn.Add(key, nullable3.Value.BestAvailableKeepUntil);
            }
            else
              childrenNeedAction.Add(key);
          }
          if (childrenNeedAction.Any<DedupIdentifier>())
          {
            if (receipt != null && ((IEnumerable<KeepUntilBlobReference?>) receipt.KeepUntils).All<KeepUntilBlobReference?>((Func<KeepUntilBlobReference?, bool>) (ku => ku.HasValue)))
              throw new Exception("TryReferenceNode should not require action if all receipts are provided! (InsufficientKeepUntil: " + string.Join<DedupIdentifier>(",", (IEnumerable<DedupIdentifier>) childrenNeedAction) + ")");
            return new TryReferenceNodeResponse(new DedupNodeChildrenNotEnoughKeepUntil(childrenNeedAction.ToArray<DedupIdentifier>(), this.ToReceipts(requestContext, keepUntilsToReturn)));
          }
          DateTime? nullable4 = await this.PumpOrInlineFromAsync<DateTime?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DateTime?>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).TryAddKeepUntilReferenceAsync(processor, (DedupIdentifier) nodeId, keepUntil.KeepUntil))).ConfigureAwait(true);
          keepUntilsToCache.Add((DedupIdentifier) nodeId, nullable4.Value);
          keepUntilsToReturn.Add((DedupIdentifier) nodeId, nullable4.Value);
          foreach (KeyValuePair<DedupIdentifier, DateTime> keyValuePair in keepUntilsToCache)
            keepUntilCache.SetKeepUntil(requestContext, domainId, keyValuePair.Key, keyValuePair.Value);
          return new TryReferenceNodeResponse(new DedupNodeUpdated(this.ToReceipts(requestContext, keepUntilsToReturn)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.TryKeepUntilReferenceNodeAsyncException, ex);
          throw;
        }
      }
    }

    public async Task<IDictionary<DedupIdentifier, PreauthenticatedUri>> GetUris(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache)
    {
      IDictionary<DedupIdentifier, PreauthenticatedUri> uris;
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.GetUrisCall, nameof (GetUris)))
      {
        try
        {
          SASUriExpiry expiry = SASUriExpiry.CreateExpiry(new SASUriExpiryPolicy(this.Clock), requestContext);
          (string, Guid)[] sasTracing = ((string, Guid)[]) null;
          sasTracing = new (string, Guid)[2]
          {
            ("e2eid", requestContext.E2EId),
            ("session", requestContext.UniqueIdentifier)
          };
          IDictionary<DedupIdentifier, PreauthenticatedUri> dictionary = await this.PumpOrInlineFromAsync<IDictionary<DedupIdentifier, PreauthenticatedUri>>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<IDictionary<DedupIdentifier, PreauthenticatedUri>>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetDownloadUrlsAsync(processor, dedupIds, expiry, sasTracing))).ConfigureAwait(true);
          if (edgeCache == EdgeCache.Allowed && requestContext.IsFeatureEnabled("BlobStore.Features.AzureBlobDedupProviderAzureFrontDoor"))
          {
            DedupStoreService.IEdgeCachingService service = requestContext.GetService<DedupStoreService.IEdgeCachingService>();
            foreach (DedupIdentifier dedupId in (IEnumerable<DedupIdentifier>) dedupIds)
              dictionary[dedupId] = new PreauthenticatedUri(service.GetEdgeUri(dictionary[dedupId].NotNullUri, dictionary[dedupId].ExpiryTime.UtcDateTime - BlobEdgeCachingService.AzureFrontDoorSasUriExpiryBuffer), EdgeType.AzureFrontDoor);
          }
          uris = dictionary;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.GetUrisException, ex);
          throw;
        }
      }
      return uris;
    }

    public async Task<IList<DedupDownloadInfo>> GetDownloadInfoBatchAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks)
    {
      ConcurrentBag<DedupDownloadInfo> dedupDownloadInfos = new ConcurrentBag<DedupDownloadInfo>();
      if (dedupIds != null)
        await requestContext.ForkChildrenAsync<DedupIdentifier, DedupStoreService.IDownloadInfoTaskService>(Environment.ProcessorCount, (IEnumerable<DedupIdentifier>) dedupIds, (Func<IVssRequestContext, DedupIdentifier, Task>) (async (rc, dedupId) =>
        {
          DedupDownloadInfo dedupDownloadInfo = await this.GetDownloadInfoAsync(rc, domainId, dedupId, includeChunks).ConfigureAwait(true);
          dedupDownloadInfos.Add(dedupDownloadInfo);
        })).ConfigureAwait(true);
      return (IList<DedupDownloadInfo>) dedupDownloadInfos.ToList<DedupDownloadInfo>();
    }

    protected async Task<long> GetDedupSizeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      DedupCompressedBuffer compressedBuffer = await this.GetDedupAsyncInternal(processor, dedupId).ConfigureAwait(true);
      if (compressedBuffer == null)
        throw new DedupNotFoundException(dedupId.ValueString);
      long dedupSizeAsync;
      if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
      {
        dedupSizeAsync = (long) compressedBuffer.Uncompressed.Count;
      }
      else
      {
        if (!ChunkerHelper.IsNode(dedupId.AlgorithmId))
          throw new InvalidOperationException("DedupId " + dedupId.ValueString + " is an unknown dedup type. Unable to determine size.");
        dedupSizeAsync = (long) DedupNode.Deserialize(compressedBuffer.Uncompressed.CreateCopy<byte>()).TransitiveContentBytes;
      }
      return dedupSizeAsync;
    }

    public async Task<DedupDownloadInfo> GetDownloadInfoAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      bool includeChunks)
    {
      DedupCompressedBuffer buffer;
      PreauthenticatedUri url;
      ChunkDedupDownloadInfo[] chunks;
      DedupDownloadInfo downloadInfoAsync;
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.GetDownloadInfoAsyncCall, nameof (GetDownloadInfoAsync)))
      {
        buffer = await this.GetDedupAsyncInternal(requestContext, domainId, dedupId).ConfigureAwait(true);
        if (buffer == null)
          throw new DedupNotFoundException(dedupId.ValueString);
        SASUriExpiry expiry = SASUriExpiry.CreateExpiry(new SASUriExpiryPolicy(this.Clock), requestContext);
        (string, Guid)[] sasTracing = ((string, Guid)[]) null;
        sasTracing = new (string, Guid)[2]
        {
          ("e2eid", requestContext.E2EId),
          ("session", requestContext.UniqueIdentifier)
        };
        url = await this.PumpOrInlineFromAsync<PreauthenticatedUri>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<PreauthenticatedUri>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetDownloadUrlAsync(processor, dedupId, expiry, sasTracing))).ConfigureAwait(true);
        long transitiveSize;
        if (includeChunks)
        {
          if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
          {
            chunks = new ChunkDedupDownloadInfo[1]
            {
              new ChunkDedupDownloadInfo(dedupId, url.NotNullUri, (long) buffer.Uncompressed.Count)
            };
            transitiveSize = (long) buffer.Uncompressed.Count;
          }
          else
          {
            if (!ChunkerHelper.IsNode(dedupId.AlgorithmId))
              throw new InvalidOperationException("Something went wrong here.");
            List<ChunkDedupDownloadInfo> chunksDownloadInfo = new List<ChunkDedupDownloadInfo>();
            await this.PumpOrInlineFromAsync(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (processor => this.GetAllChunksDownloadInfoAsync(processor, chunksDownloadInfo, dedupId.CastToNodeDedupIdentifier(), expiry, sasTracing))).ConfigureAwait(true);
            chunks = chunksDownloadInfo.ToArray();
            transitiveSize = ((IEnumerable<ChunkDedupDownloadInfo>) chunks).Sum<ChunkDedupDownloadInfo>((Func<ChunkDedupDownloadInfo, long>) (c => c.Size));
          }
        }
        else
        {
          chunks = Array.Empty<ChunkDedupDownloadInfo>();
          if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
          {
            transitiveSize = (long) buffer.Uncompressed.Count;
          }
          else
          {
            if (!ChunkerHelper.IsNode(dedupId.AlgorithmId))
              throw new InvalidOperationException("Something went wrong here.");
            transitiveSize = await this.GetTransitiveSizeOfDedupNodeAsync(requestContext, domainId, (NodeDedupIdentifier) dedupId).ConfigureAwait(true);
          }
        }
        downloadInfoAsync = new DedupDownloadInfo(dedupId, url.NotNullUri, chunks, transitiveSize);
      }
      buffer = (DedupCompressedBuffer) null;
      url = new PreauthenticatedUri();
      chunks = (ChunkDedupDownloadInfo[]) null;
      return downloadInfoAsync;
    }

    public async Task DeleteRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.DeleteRootAsyncCall, nameof (DeleteRootAsync)))
      {
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Delete);
          rootRef.CheckIdReferenceDeletePermission(requestContext);
          await this.PumpOrInlineFromAsync(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).RemoveRootAsync(processor, dedupId, rootRef))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.DeleteRootAsyncException, ex);
          throw;
        }
      }
    }

    public async Task RestoreRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.RestoreRootAsyncCall, nameof (RestoreRootAsync)))
      {
        requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Delete);
        rootRef.CheckIdReferenceDeletePermission(requestContext);
        await this.PumpOrInlineFromAsync(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).RestoreRootAsync(processor, dedupId, rootRef))).ConfigureAwait(true);
      }
    }

    public async Task PutRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.PutRootAsyncCall, nameof (PutRootAsync)))
      {
        try
        {
          int num = await BlobStoreUtils.EvaluateQuotaCapAsync(requestContext, ResourceName.Artifacts, rootRef.Scope).ConfigureAwait(true) ? 1 : 0;
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Create);
          rootRef.CheckIdReferenceCreatePermission(requestContext);
          DateTime keepUntil = this.Clock.Now.UtcDateTime.AddHours(1.0);
          KeepUntilResult? nullable = await this.ValidateKeepUntilAndUpdateCacheAsync(requestContext, domainId, dedupId, keepUntil).ConfigureAwait(true);
          if (!nullable.HasValue)
            throw new DedupNotFoundException("Adding of root failed because dedup does not exist. Dedup ID: " + dedupId.AlgorithmResultString);
          if (!nullable.Value.IsSatisfied)
            throw new InvalidOperationException("Adding of root failed because dedup has expired. Dedup ID: " + dedupId.AlgorithmResultString);
          await this.PumpOrInlineFromAsync(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
          {
            long dedupSizeAsync = await this.GetDedupSizeAsync(processor, dedupId);
            await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).AddRootAsync(processor, dedupId, rootRef, dedupSizeAsync);
          })).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.DedupStoreService.PutRootAsyncException, ex);
          throw;
        }
      }
    }

    public async Task<MetadataOperationResult> TryModifyKeepUntilAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IKeepUntil data,
      bool throwError)
    {
      try
      {
        IDedupProvider provider = this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest);
        return await AsyncHttpRetryHelper<MetadataOperationResult>.InvokeAsync((Func<Task<MetadataOperationResult>>) (() => provider.TryExtendKeepUntilReferenceAsync(processor, dedupId, data)), 5, (IAppTraceSource) NoopAppTraceSource.Instance, (Func<Exception, bool>) null, processor.CancellationToken, true, nameof (TryModifyKeepUntilAsync));
      }
      catch (Exception ex) when (!throwError)
      {
        return MetadataOperationResult.Failed;
      }
    }

    private Action<TraceLevel, string> GetInContextTracer(
      VssRequestPump.Processor processor,
      int traceId,
      string header)
    {
      string logHeader = "<" + (string.IsNullOrEmpty(header) ? DateTime.Now.ToString("MMdd-HHmm") : header) + "> ";
      return (Action<TraceLevel, string>) (async (level, message) => await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (rc => rc.TraceAlways(traceId, level, this.traceData.Area, this.traceData.Layer, logHeader + message))).ConfigureAwait(true));
    }

    public async Task<RootsValidationResult> VerifyRootsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      RootsValidationConfig config)
    {
      IDedupProcessingCache cache = config.NoCache ? (IDedupProcessingCache) NullDedupProcessingCache.Instance : (IDedupProcessingCache) requestContext.GetService<DedupProcessingCacheService>();
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/BlobStore/ConcurrentIteratorCapacity", true, 10);
      DedupMetadataPageRetrievalOption option = new DedupMetadataPageRetrievalOption(string.Empty, config.Start, config.End, ResultArrangement.AllUnordered, 1000, StateFilter.Active);
      option.BoundedCapacity = num1;
      string scope = config.Scope?.Trim().ToLower();
      DateTimeOffset dateTimeOffset;
      string str1;
      if (!option.Start.HasValue)
      {
        str1 = "unspecified";
      }
      else
      {
        dateTimeOffset = option.Start.Value;
        str1 = dateTimeOffset.ToString();
      }
      string str2 = str1;
      string str3;
      if (!option.End.HasValue)
      {
        str3 = "unspecified";
      }
      else
      {
        dateTimeOffset = option.End.Value;
        str3 = dateTimeOffset.ToString();
      }
      string str4 = str3;
      string range = str2 + " - " + str4;
      RootsValidationResult result = new RootsValidationResult("Range: " + range);
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
      {
        int matched = 0;
        Action<TraceLevel, string> inContextTrace = this.GetInContextTracer((VssRequestPump.Processor) processor, 5702099, (string) null);
        ConfiguredTaskAwaitable configuredTaskAwaitable;
        Task task;
        if (config.ScanOnly)
        {
          configuredTaskAwaitable = this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option).ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Action<IEnumerable<DedupMetadataEntry>>) (page =>
          {
            foreach (DedupMetadataEntry dedupMetadataEntry in page)
            {
              if (scope == null || scope == dedupMetadataEntry.Scope)
                ++matched;
            }
          })).ConfigureAwait(false);
          await configuredTaskAwaitable;
          task = (Task) Task.FromResult<int>(0);
        }
        else
        {
          int total = 0;
          RootsValidator validator = (RootsValidator) null;
          inContextTrace(TraceLevel.Info, "[VALIDATION] Started scanning for roots within the time range: " + range);
          if (config.AdaptiveThreading)
          {
            inContextTrace(TraceLevel.Info, "[VALIDATION] Using adaptive threading. Node-level parallelism is determined based on the count of matching roots.");
            List<DedupMetadataEntry> entries = new List<DedupMetadataEntry>();
            await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option).ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Action<IEnumerable<DedupMetadataEntry>>) (page =>
            {
              foreach (DedupMetadataEntry dedupMetadataEntry in page)
              {
                ++total;
                if (scope == null || scope == dedupMetadataEntry.Scope)
                {
                  ++matched;
                  entries.Add(dedupMetadataEntry);
                }
              }
            })).ConfigureAwait(false);
            inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Completed scanning for roots within the time range: {0}. Scanned {1} roots.", (object) range, (object) total));
            int levelParallelism = config.RootLevelParallelism;
            int num2 = Math.Min(4, levelParallelism / Math.Max((int) Math.Min((double) matched / (double) levelParallelism, (double) levelParallelism), 1));
            config.DispatchingParallelism = num2;
            config.ProcessingParallelism = num2;
            validator = this.CreateValidator(processor, cache, config, result, inContextTrace);
            foreach (DedupMetadataEntry entry in entries)
              await validator.PostAsync(entry);
            inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Sent {0} roots for validation.", (object) matched));
          }
          else
          {
            validator = this.CreateValidator(processor, cache, config, result, inContextTrace);
            configuredTaskAwaitable = this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option).ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
            {
              foreach (DedupMetadataEntry entry in page)
              {
                total++;
                if (validator.IsAborting)
                {
                  await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (rc => rc.Cancel("Validation is aborted.")));
                  break;
                }
                if (scope == null || scope == entry.Scope)
                {
                  matched++;
                  await validator.PostAsync(entry);
                }
              }
            })).ConfigureAwait(false);
            await configuredTaskAwaitable;
            inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Completed scanning/dispatching for roots within the time range: {0}. Scanned {1} roots, sent {2} for validation.", (object) range, (object) total, (object) matched));
          }
          task = validator.CompleteAsync();
        }
        if (task != null)
        {
          if (config.ScanOnly)
          {
            result.SetFailure(new Exception(string.Format("No validation performed due to settings. Found {0} roots.", (object) matched)));
            inContextTrace = (Action<TraceLevel, string>) null;
          }
          else
          {
            try
            {
              configuredTaskAwaitable = task.ConfigureAwait(true);
              await configuredTaskAwaitable;
              inContextTrace = (Action<TraceLevel, string>) null;
            }
            catch (Exception ex)
            {
              result.SetFailure(ex);
              inContextTrace = (Action<TraceLevel, string>) null;
            }
          }
        }
        else
        {
          result.SetFailure(new Exception("Roots scanning failed due to unknown reason."));
          inContextTrace = (Action<TraceLevel, string>) null;
        }
      })).ConfigureAwait(true);
      return result;
    }

    private RootsValidator CreateValidator(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessingCache cache,
      RootsValidationConfig config,
      RootsValidationResult result,
      Action<TraceLevel, string> inContextTrace)
    {
      RootsValidator validator = new RootsValidator(processor, config, (IDedupInfoRetriever) this, cache, result, inContextTrace);
      inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Root dispatching parallelism  = {0}", (object) config.RootLevelParallelism));
      inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Dedup dispatching parallelism = {0}", (object) config.DispatchingParallelism));
      inContextTrace(TraceLevel.Info, string.Format("[VALIDATION] Dedup validating parellelism  = {0}", (object) config.ProcessingParallelism));
      return validator;
    }

    public async Task<DedupValidationResult> VerifyDedupAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier root,
      DedupTraversalConfig config)
    {
      DedupStoreService dedupStoreService = this;
      DedupValidationResult result = new DedupValidationResult(config, string.Format("Root: {0}", (object) root));
      IDedupProcessingCache cache = config.NoCache ? (IDedupProcessingCache) NullDedupProcessingCache.Instance : (IDedupProcessingCache) requestContext.GetService<DedupProcessingCacheService>();
      try
      {
        await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
        {
          Action<TraceLevel, string> log = (Action<TraceLevel, string>) ((level, message) => processor.ExecuteWorkAsync((Action<IVssRequestContext>) (rc => rc.Trace(5702099, level, this.traceData.Area, this.traceData.Layer, message))));
          long num = await new DedupValidator(processor, (IDedupInfoRetriever) this, cache, result, config).ValidateAsync(root, log).ConfigureAwait(false);
          result.VisitCount = num;
        })).ConfigureAwait(true);
        return result;
      }
      catch (Exception ex)
      {
        result.SetFailure(ex);
        requestContext.Trace(5702099, TraceLevel.Error, dedupStoreService.traceData.Area, dedupStoreService.traceData.Layer, "Dedup consistency validation failed with exception. Error Result: " + result.StatusMessage);
      }
      return result;
    }

    public Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      return this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetDedupInfoAsync(processor, dedupId);
    }

    public async Task<DedupNode?> GetDedupNodeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      NodeDedupIdentifier nodeId)
    {
      DedupNode? dedupNodeAsync;
      using (DedupCompressedBuffer compressedBuffer = await this.GetDedupAsyncInternal(processor, (DedupIdentifier) nodeId).ConfigureAwait(false))
        dedupNodeAsync = compressedBuffer != null ? new DedupNode?(DedupNode.Deserialize(compressedBuffer.Uncompressed.CreateCopy<byte>())) : new DedupNode?();
      return dedupNodeAsync;
    }

    public async Task<bool> RestoreIfNotExists(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      return await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).RestoreIfNotExists(processor, dedupId);
    }

    public async Task<bool> VerifyRootExists(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference blobReference)
    {
      DedupMetadataPageRetrievalOption option = new DedupMetadataPageRetrievalOption(dedupId.ValueString, new DateTimeOffset?(), new DateTimeOffset?(), ResultArrangement.AllOrdered, 50, StateFilter.Active, (IDomainId) null);
      bool matchesFound = false;
      Action<IEnumerable<DedupMetadataEntry>> action;
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option).ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, action ?? (action = (Action<IEnumerable<DedupMetadataEntry>>) (page =>
      {
        foreach (DedupMetadataEntry dedupMetadataEntry in page)
        {
          if (dedupMetadataEntry.DedupId == dedupId && dedupMetadataEntry.Scope == blobReference.Scope && dedupMetadataEntry.ReferenceId == blobReference.Name)
          {
            matchesFound = true;
            break;
          }
        }
      }))).ConfigureAwait(false))).ConfigureAwait(true);
      return matchesFound;
    }

    internal async Task<DedupCompressedBuffer> GetDedupAsyncInternal(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      return await this.PumpOrInlineFromAsync<DedupCompressedBuffer>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DedupCompressedBuffer>>) (processor => this.GetDedupAsyncInternal(processor, dedupId))).ConfigureAwait(true);
    }

    internal Task<DedupCompressedBuffer> GetDedupAsyncInternal(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      return this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetDedupAsync(processor, dedupId);
    }

    private async Task<bool> DedupExists(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      bool flag;
      using (DedupCompressedBuffer compressedBuffer = await this.GetDedupAsyncInternal(requestContext, domainId, dedupId).ConfigureAwait(true))
        flag = compressedBuffer != null;
      return flag;
    }

    private async Task<KeepUntilResult?> ValidateKeepUntilAndUpdateCacheAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      DateTime keepUntil)
    {
      IDedupKeepUntilCacheService keepUntilCache = this.GetKeepUntilCache(requestContext);
      DateTime keepUntil1;
      if (keepUntilCache.TryGetKeepUntil(requestContext, domainId, dedupId, out keepUntil1) && keepUntil1 >= keepUntil)
      {
        Interlocked.Increment(ref this.keepUntilCacheHits);
        return new KeepUntilResult?(new KeepUntilResult(true, keepUntil1));
      }
      Interlocked.Increment(ref this.keepUntilCacheMisses);
      KeepUntilResult? nullable = await this.PumpOrInlineFromAsync<KeepUntilResult?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<KeepUntilResult?>>) (processor => this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).ValidateKeepUntilReferenceAsync(processor, dedupId, keepUntil))).ConfigureAwait(true);
      if (nullable.HasValue && nullable.Value.IsSatisfied)
        keepUntilCache.SetKeepUntil(requestContext, domainId, dedupId, nullable.Value.BestAvailableKeepUntil);
      return nullable;
    }

    private Dictionary<DedupIdentifier, KeepUntilReceipt> CreateReceiptDictionary(
      IVssRequestContext requestContext,
      DedupIdentifier dedupId,
      DateTime keepUntil)
    {
      return new Dictionary<DedupIdentifier, KeepUntilReceipt>()
      {
        {
          dedupId,
          KeepUntilReceipt.Create(this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, requestContext.ServiceHost.InstanceId, dedupId, new KeepUntilBlobReference(keepUntil))
        }
      };
    }

    private async Task GetAllChunksDownloadInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      List<ChunkDedupDownloadInfo> chunksDownloadInfo,
      NodeDedupIdentifier id,
      SASUriExpiry expiry,
      (string, Guid)[] sasTracing)
    {
      int hashType = (int) ChunkerHelper.GetHashType(id.AlgorithmId);
      foreach (DedupNode childNode in (IEnumerable<DedupNode>) (await this.GetDedupNodeAsync(processor, id).ConfigureAwait(false)).Value.ChildNodes)
      {
        DedupNode child = childNode;
        if (child.Type == DedupNode.NodeType.ChunkLeaf)
        {
          PreauthenticatedUri preauthenticatedUri = await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetDownloadUrlAsync(processor, (DedupIdentifier) child.GetChunkIdentifier(), expiry, sasTracing).ConfigureAwait(false);
          chunksDownloadInfo.Add(new ChunkDedupDownloadInfo((DedupIdentifier) child.GetChunkIdentifier(), preauthenticatedUri.NotNullUri, (long) child.TransitiveContentBytes));
        }
        else
          await this.GetAllChunksDownloadInfoAsync(processor, chunksDownloadInfo, child.GetNodeIdentifier(), expiry, sasTracing).ConfigureAwait(false);
        child = new DedupNode();
      }
    }

    [DebuggerHidden]
    protected Task PumpOrInlineFromAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Func<VssRequestPump.SecuredDomainProcessor, Task> callback)
    {
      SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
      return requestContext.PumpOrInlineFromAsync((ISecuredDomainRequest) domainRequest, callback, this.DomainDedupStoreProvider.GetProvider((ISecuredDomainRequest) domainRequest).ProviderRequireVss);
    }

    [DebuggerHidden]
    protected Task<T> PumpOrInlineFromAsync<T>(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> callback)
    {
      SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
      return requestContext.PumpOrInlineFromAsync<T>((ISecuredDomainRequest) domainRequest, callback, this.DomainDedupStoreProvider.GetProvider((ISecuredDomainRequest) domainRequest).ProviderRequireVss);
    }

    protected async Task<long> VisitPreorderAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      Func<DedupIdentifier, Task<bool>> onDedup)
    {
      processor.CancellationToken.ThrowIfCancellationRequested();
      long count = 1;
      if (await onDedup(dedupId).ConfigureAwait(false) || !ChunkerHelper.IsNode(dedupId.AlgorithmId))
        return count;
      foreach (DedupIdentifier dedupId1 in (await this.GetDedupNodeAsync(processor, dedupId.CastToNodeDedupIdentifier()).ConfigureAwait(false)).Value.ChildNodes.Select<DedupNode, DedupIdentifier>((Func<DedupNode, DedupIdentifier>) (n => DedupIdentifier.Create(n))))
      {
        long num = count;
        count = num + await this.VisitPreorderAsync(processor, dedupId1, onDedup).ConfigureAwait(false);
      }
      return count;
    }

    protected virtual ICloudBlobContainer GetCloudBlobContainer(
      IVssRequestContext systemRequestContext,
      StrongBoxConnectionString connectionString,
      string containerName)
    {
      return systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudBlobClientProvider>().GetBlobClient(connectionString.StrongBoxItemKey).CreateContainerReference(containerName, systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry"));
    }

    private void SetupAzureBlobTableProviders(
      IVssRequestContext systemRequestContext,
      string containerName)
    {
      if (systemRequestContext.AllowHostDomainAdminOperations())
      {
        AdminHostDomainStoreService service = systemRequestContext.GetService<AdminHostDomainStoreService>();
        foreach (PhysicalDomainInfo physicalDomainInfo in service.GetPhysicalDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
        {
          IDomainId domainId = physicalDomainInfo.DomainId;
          this.DomainDedupStoreProvider.AddDedupProvider(domainId, (IDedupProvider) this.GetAzureBlobTableProvider(systemRequestContext, containerName, physicalDomainInfo, domainId));
        }
        if (!systemRequestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains"))
          return;
        foreach (ProjectDomainInfo projectDomainInfo in service.GetProjectDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
        {
          IDomainId domainId = projectDomainInfo.DomainId;
          string partitionName = projectDomainInfo.PartitionName;
          this.DomainDedupStoreProvider.AddDedupProvider(domainId, (IDedupProvider) this.GetAzureBlobTableProviderProjectDomain(systemRequestContext, partitionName, projectDomainInfo, domainId));
        }
      }
      else
        this.DomainDedupStoreProvider.AddDedupProvider(WellKnownDomainIds.DefaultDomainId, (IDedupProvider) this.GetAzureBlobTableProvider(systemRequestContext, containerName, (PhysicalDomainInfo) null, WellKnownDomainIds.DefaultDomainId));
    }

    private AzureBlobTableDedupProvider GetAzureBlobTableProvider(
      IVssRequestContext systemRequestContext,
      string containerName,
      PhysicalDomainInfo physicalDomainInfo,
      IDomainId domainId)
    {
      this.disposableTableFactory = this.GetTableFactory(systemRequestContext, physicalDomainInfo);
      IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(systemRequestContext, physicalDomainInfo);
      List<ICloudBlobContainer> containers = new List<ICloudBlobContainer>();
      foreach (StrongBoxConnectionString connectionString in connectionStrings)
        containers.Add(this.GetCloudBlobContainer(systemRequestContext, connectionString, containerName));
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(systemRequestContext.GetElevatedDeploymentRequestContext());
      return new AzureBlobTableDedupProvider((IEnumerable<ICloudBlobContainer>) containers, tableLocationMode, this.disposableTableFactory, this.Clock, domainId);
    }

    private AzureBlobTableDedupProvider GetAzureBlobTableProviderProjectDomain(
      IVssRequestContext systemRequestContext,
      string containerName,
      ProjectDomainInfo projectDomainInfo,
      IDomainId domainId)
    {
      IDomainId physicalDomainId = projectDomainInfo.PhysicalDomainId;
      PhysicalDomainInfo result = (PhysicalDomainInfo) systemRequestContext.GetService<HostDomainStoreService>().GetDomainForOrganizationAsync(systemRequestContext, physicalDomainId).Result;
      this.disposableTableFactory = this.GetTableFactory(systemRequestContext, result);
      IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(systemRequestContext, result);
      List<ICloudBlobContainer> containers = new List<ICloudBlobContainer>();
      foreach (StrongBoxConnectionString connectionString in connectionStrings)
        containers.Add(this.GetCloudBlobContainer(systemRequestContext, connectionString, containerName));
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(systemRequestContext.GetElevatedDeploymentRequestContext());
      return new AzureBlobTableDedupProvider((IEnumerable<ICloudBlobContainer>) containers, tableLocationMode, this.disposableTableFactory, this.Clock, domainId);
    }

    private Task<long> AddExpiredToBloomFilterRecursivelyAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      DateTime expiryCutoff,
      BloomFilter<DedupIdentifier> bloomFilter)
    {
      Func<DedupIdentifier, Task<bool>> onDedup = (Func<DedupIdentifier, Task<bool>>) (async id =>
      {
        bloomFilter.Insert(id);
        KeepUntilResult? nullable = await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).ValidateKeepUntilReferenceAsync(processor, dedupId, expiryCutoff).ConfigureAwait(false);
        return nullable.HasValue && nullable.Value.IsSatisfied;
      });
      return this.VisitPreorderAsync(processor, dedupId, onDedup);
    }

    private Dictionary<DedupIdentifier, DateTime> ValidateReceipts(
      IVssRequestContext requestContext,
      DedupIdentifier[] childIds,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt summaryReceipt)
    {
      Dictionary<DedupIdentifier, DateTime> validatedReceipts;
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/BlobStore/DedupReceiptsEnabled", true, true))
      {
        if (!this.TryValidateReceiptsAfterRegistryCheck(requestContext, childIds, keepUntil, summaryReceipt, this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, out validatedReceipts))
          this.TryValidateReceiptsAfterRegistryCheck(requestContext, childIds, keepUntil, summaryReceipt, this.receiptSecretProvider.ReceiptSecrets.SecondarySecret, out validatedReceipts);
      }
      else
        validatedReceipts = new Dictionary<DedupIdentifier, DateTime>();
      return validatedReceipts;
    }

    private bool TryValidateReceiptsAfterRegistryCheck(
      IVssRequestContext requestContext,
      DedupIdentifier[] childIds,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt summaryReceipt,
      string receiptSecret,
      out Dictionary<DedupIdentifier, DateTime> validatedReceipts)
    {
      validatedReceipts = new Dictionary<DedupIdentifier, DateTime>();
      List<KeepUntilReceipt> keepUntilReceiptList = new List<KeepUntilReceipt>();
      if (summaryReceipt != null)
      {
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        if (childIds.Length != summaryReceipt.KeepUntils.Length)
          throw new ArgumentException("Bad receipt");
        for (int index = 0; index < childIds.Length; ++index)
        {
          if (!summaryReceipt.KeepUntils[index].HasValue)
          {
            keepUntilReceiptList.Add((KeepUntilReceipt) null);
          }
          else
          {
            KeepUntilBlobReference untilBlobReference = summaryReceipt.KeepUntils[index].Value;
            if (untilBlobReference.KeepUntil < keepUntil.KeepUntil)
              throw new ArgumentException(string.Format("Not sufficient keepuntil for {0}", (object) childIds[index]));
            KeepUntilReceipt keepUntilReceipt = KeepUntilReceipt.Create(receiptSecret, instanceId, childIds[index], summaryReceipt.KeepUntils[index].Value);
            keepUntilReceiptList.Add(keepUntilReceipt);
            Dictionary<DedupIdentifier, DateTime> dictionary = validatedReceipts;
            DedupIdentifier childId = childIds[index];
            untilBlobReference = summaryReceipt.KeepUntils[index].Value;
            DateTime keepUntil1 = untilBlobReference.KeepUntil;
            dictionary[childId] = keepUntil1;
          }
        }
        if (!((IEnumerable<byte>) new SummaryKeepUntilReceipt(keepUntilReceiptList.ToArray()).Signature).SequenceEqual<byte>((IEnumerable<byte>) summaryReceipt.Signature))
        {
          validatedReceipts.Clear();
          return false;
        }
      }
      return true;
    }

    private void CheckKeepUntilReference(KeepUntilBlobReference keepuntil)
    {
      DateTime keepUntil = keepuntil.KeepUntil;
      DateTime dateTime1 = this.Clock.Now.UtcDateTime;
      dateTime1 = dateTime1.Add(DedupConstants.MaximumKeepuntil);
      DateTime dateTime2 = dateTime1.Add(DedupStoreService.ClockSkewBonus);
      if (keepUntil > dateTime2)
        throw new ArgumentException("KeepUntil value is too far in the future.");
    }

    private Dictionary<DedupIdentifier, KeepUntilReceipt> ToReceipts(
      IVssRequestContext requestContext,
      Dictionary<DedupIdentifier, DateTime> keepuntilsToReturn)
    {
      return keepuntilsToReturn.ToDictionary<KeyValuePair<DedupIdentifier, DateTime>, DedupIdentifier, KeepUntilReceipt>((Func<KeyValuePair<DedupIdentifier, DateTime>, DedupIdentifier>) (n => n.Key), (Func<KeyValuePair<DedupIdentifier, DateTime>, KeepUntilReceipt>) (m => KeepUntilReceipt.Create(this.receiptSecretProvider.ReceiptSecrets.PrimarySecret, requestContext.ServiceHost.InstanceId, m.Key, new KeepUntilBlobReference(m.Value))));
    }

    private async Task<long> GetTransitiveSizeOfDedupNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeDedupId)
    {
      return (long) (await this.PumpOrInlineFromAsync<DedupNode?>(requestContext, domainId, (Func<VssRequestPump.SecuredDomainProcessor, Task<DedupNode?>>) (processor => this.GetDedupNodeAsync(processor, nodeDedupId))).ConfigureAwait(true) ?? throw new DedupNotFoundException("Dedup not found.")).Value.TransitiveContentBytes;
    }

    public class DedupShardingAzureCloudTableClientFactory : ShardingAzureCloudTableClientFactory
    {
      public DedupShardingAzureCloudTableClientFactory(
        IEnumerable<StrongBoxConnectionString> accountConnectionStrings,
        Func<StrongBoxConnectionString, ITableClient> getTableClient,
        LocationMode? locationMode,
        string defaultTableName,
        string shardingStrategy,
        bool enableTracing)
        : base(accountConnectionStrings, getTableClient, locationMode, defaultTableName, shardingStrategy, enableTracing)
      {
      }

      protected override ulong GetKeyForShardHint(string shardHint) => DedupIdentifier.Create(shardHint).GetKey();
    }

    [DefaultServiceImplementation(typeof (DedupStoreService.DownloadInfoTaskService))]
    public interface IDownloadInfoTaskService : IVssTaskService, IVssFrameworkService
    {
    }

    private class DownloadInfoTaskService : 
      VssTaskService,
      DedupStoreService.IDownloadInfoTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }

    public class EdgeCachingService : 
      BlobEdgeCachingService,
      DedupStoreService.IEdgeCachingService,
      IBlobEdgeCachingService,
      IVssFrameworkService
    {
      public override string RootRegistryPath => "/Configuration/BlobStore/AzureFrontDoor";

      public override string UrlSigningKeySettingName => "PrimaryEdgeCacheUrlSigningKey";
    }

    [DefaultServiceImplementation(typeof (DedupStoreService.EdgeCachingService))]
    public interface IEdgeCachingService : IBlobEdgeCachingService, IVssFrameworkService
    {
    }
  }
}
