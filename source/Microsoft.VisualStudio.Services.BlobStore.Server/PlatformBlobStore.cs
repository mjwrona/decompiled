// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.PlatformBlobStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.OnPrem;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.ShardManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class PlatformBlobStore : BlobStoreServiceBase, IBlobStore, IVssFrameworkService
  {
    protected const int MaxMetadataCompareSwapRetries = 20;
    private const int MaxBlobUploadRetries = 5;
    protected internal readonly TimeSpan MinimumKeepUntil = TimeSpan.FromDays(1.0);
    private const int PhysicalNodeCount = 24;
    private static readonly SemaphoreSlim ConcurrentReferenceOperations = new SemaphoreSlim(Environment.ProcessorCount * 512, Environment.ProcessorCount * 512);
    private static readonly HttpClient httpClient = new HttpClient();
    private bool providerRequiresVss;
    protected internal IDomainBlobMetadataProvider<IAdminBlobMetadataProviderWithTestHooks> BlobMetadataDomainProvider;

    protected override string ProductTraceArea => "BlobStore";

    protected IDomainProvider<IBlobProviderDomain> DomainProvider { get; private set; }

    protected internal IAdminBlobMetadataProviderWithTestHooks MetadataProvider { get; set; }

    protected IHostDomainProvider HostDomainMetadataProvider { get; set; }

    protected virtual IClock Clock => UtcClock.Instance;

    protected string TestNamespacePrefix { get; set; }

    private string TestBlobNamespace => this.TestNamespacePrefix + BlobStoreProviderConstants.BlobStoreSuffix;

    public virtual void ConfigureService(
      IVssRequestContext systemRequestContext,
      string blobProviderType = null,
      string metadataProviderType = null,
      string shardingStrategy = null)
    {
      blobProviderType = blobProviderType ?? "FILE";
      metadataProviderType = metadataProviderType ?? "AZURETABLEBLOBMETADATAPROVIDER";
      shardingStrategy = shardingStrategy ?? "ConsistentHashing";
      blobProviderType = blobProviderType.ToUpperInvariant();
      metadataProviderType = metadataProviderType.ToUpperInvariant();
      shardingStrategy = shardingStrategy.ToUpperInvariant();
      this.providerRequiresVss = blobProviderType == "SQL" || systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry");
      this.ConfigureBlobStorage(systemRequestContext, blobProviderType, shardingStrategy);
      this.ConfigureMetadataStorage(systemRequestContext, metadataProviderType, blobProviderType, shardingStrategy);
    }

    public async Task<Stream> GetBlobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId)
    {
      Stream blobAsync;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.GetBlobAsyncCall, nameof (GetBlobAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Read);
          Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).FindProvider(blobId);
          DisposableEtagValue<Stream> result = new DisposableEtagValue<Stream>();
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => result = await provider.GetBlobAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false))).ConfigureAwait(true);
          blobAsync = result.Value;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.GetBlobAsyncException, ex);
          throw;
        }
      }
      return blobAsync;
    }

    public async Task PutBlobBlockAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash)
    {
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.PutBlobAsyncCall, nameof (PutBlobBlockAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        bool bypassExceptionOnRetryHashMatch = requestContext.IsFeatureEnabled("Blobstore.Features.BypassExceptionIfHashRetryMatches");
        try
        {
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).FindProvider(blobId);
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.PutBlobBlockInternalAsync((VssRequestPump.Processor) vssProcessor, provider, blobId, blobBuffer, blockLength, blockHash, useHttpClient, bypassExceptionOnRetryHashMatch))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.PutBlobAsyncException, ex);
          throw;
        }
      }
    }

    public Task RemoveReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      return (Task) this.RemoveReferencesWithResultsAsync(requestContext, domainId, referencesGroupedByBlobIds);
    }

    public async Task<IDictionary<BlobIdentifier, RemoveReferencesResult>> RemoveReferencesWithResultsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      IDictionary<BlobIdentifier, RemoveReferencesResult> dictionary;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.RemoveReferencesWithResultsAsyncCall, nameof (RemoveReferencesWithResultsAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          ConcurrentDictionary<BlobIdentifier, RemoveReferencesResult> results = new ConcurrentDictionary<BlobIdentifier, RemoveReferencesResult>();
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Delete);
          List<KeyValuePair<BlobIdentifier, IdBlobReference>> keyValuePairList = new List<KeyValuePair<BlobIdentifier, IdBlobReference>>();
          foreach (KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobId in (IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>>) referencesGroupedByBlobIds)
          {
            foreach (IdBlobReference reference in referencesGroupedByBlobId.Value)
            {
              if (reference.IdReferenceScopedToFileList())
                keyValuePairList.Add(new KeyValuePair<BlobIdentifier, IdBlobReference>(referencesGroupedByBlobId.Key, reference));
            }
          }
          foreach (KeyValuePair<BlobIdentifier, IdBlobReference> keyValuePair in keyValuePairList)
          {
            IVssRequestContext context = requestContext;
            SingleLocationTracePoint resultsAsyncBlobInfo = ContentTracePoints.PlatformBlobStore.RemoveReferencesWithResultsAsyncBlobInfo;
            string[] strArray = new string[6]
            {
              "Blob ID=",
              keyValuePair.Key.ValueString,
              "Reference name=",
              null,
              null,
              null
            };
            IdBlobReference idBlobReference = keyValuePair.Value;
            strArray[3] = idBlobReference.Name;
            strArray[4] = ", scope=";
            idBlobReference = keyValuePair.Value;
            strArray[5] = idBlobReference.Scope;
            string messageFormat = string.Concat(strArray);
            object[] objArray = Array.Empty<object>();
            context.TraceAlways(resultsAsyncBlobInfo, messageFormat, objArray);
          }
          referencesGroupedByBlobIds.SelectMany<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, IdBlobReference>((Func<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, IEnumerable<IdBlobReference>>) (x => x.Value)).CheckReferencesBatchDeletePermission(requestContext);
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async vssProcessor =>
          {
            Func<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, Task> action = (Func<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, Task>) (grouping => Task.Run((Func<Task>) (async () =>
            {
              await PlatformBlobStore.ConcurrentReferenceOperations.WaitAsync().ConfigureAwait(false);
              try
              {
                IEnumerable<IdBlobReference> references = grouping.Value.SelectMany<IdBlobReference, IdBlobReference>((Func<IdBlobReference, IEnumerable<IdBlobReference>>) (refid =>
                {
                  return refid.Scope != null ? (IEnumerable<IdBlobReference>) new IdBlobReference[2]
                  {
                    refid,
                    new IdBlobReference(refid.Name, (string) null)
                  } : throw new ArgumentException("Id reference without scope. Reference: " + refid.ToString());
                }));
                BlobIdentifier key2 = grouping.Key;
                Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain(vssProcessor.SecuredDomainRequest).FindProvider(key2);
                ConcurrentDictionary<BlobIdentifier, RemoveReferencesResult> concurrentDictionary = results;
                BlobIdentifier key = key2;
                concurrentDictionary[key] = await this.RemoveReferencesAsync(vssProcessor, provider, key2, references).ConfigureAwait(false);
                concurrentDictionary = (ConcurrentDictionary<BlobIdentifier, RemoveReferencesResult>) null;
                key = (BlobIdentifier) null;
              }
              finally
              {
                PlatformBlobStore.ConcurrentReferenceOperations.Release();
              }
            })));
            await NonSwallowingActionBlock.Create<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>>(action, new ExecutionDataflowBlockOptions()
            {
              BoundedCapacity = 2 * Environment.ProcessorCount,
              MaxDegreeOfParallelism = Environment.ProcessorCount,
              CancellationToken = vssProcessor.CancellationToken,
              EnsureOrdered = false
            }).SendAllAndCompleteSingleBlockNetworkAsync<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>>((IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>>) referencesGroupedByBlobIds, vssProcessor.CancellationToken).ConfigureAwait(true);
          }));
          dictionary = (IDictionary<BlobIdentifier, RemoveReferencesResult>) results;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.RemoveReferencesWithResultsAsyncException, ex);
          throw;
        }
      }
      return dictionary;
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> failed;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.TryReferenceAsyncCall, nameof (TryReferenceAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          string scope;
          if (this.GetScopeFromReferencesGroupedByBlobIds(referencesGroupedByBlobIds, out scope))
          {
            int num = await BlobStoreUtils.EvaluateQuotaCapAsync(requestContext, ResourceName.Artifacts, scope).ConfigureAwait(true) ? 1 : 0;
          }
          referencesGroupedByBlobIds.SelectMany<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, BlobReference>((Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, IEnumerable<BlobReference>>) (x => x.Value)).CheckReferencesBatchCreatePermission(requestContext, this.Clock);
          IDictionary<BlobIdentifier, IEnumerable<BlobReference>> filteredReferencesGroupedByBlobIds = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) this.GetKeepUntilCacheMisses<BlobIdentifier>(requestContext, domainId, (Func<BlobIdentifier, BlobIdentifier>) (blobId => blobId), this.BlobMetadataDomainProvider.GetMetadataProvider((ISecuredDomainRequest) domainRequest).MaxExpectedClockSkew, referencesGroupedByBlobIds);
          ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> results = new ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>();
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.TryReferenceInternalAsync(vssProcessor, filteredReferencesGroupedByBlobIds, results))).ConfigureAwait(true);
          PlatformBlobStore.UpdateKeepUntilCache(requestContext, domainId, (IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>) results);
          failed = PlatformBlobStore.GetFailed((IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>) results);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.TryReferenceAsyncException, ex);
          throw;
        }
      }
      return failed;
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> failed;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.TryReferenceWithBlocksAsyncCall, nameof (TryReferenceWithBlocksAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          string scope;
          if (this.GetScopeFromReferencesGroupedByBlobIdsWithBlocks(referencesGroupedByBlobIds, out scope))
          {
            int num = await BlobStoreUtils.EvaluateQuotaCapAsync(requestContext, ResourceName.Artifacts, scope).ConfigureAwait(true) ? 1 : 0;
          }
          referencesGroupedByBlobIds.SelectMany<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, BlobReference>((Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, IEnumerable<BlobReference>>) (x => x.Value)).CheckReferencesBatchCreatePermission(requestContext, this.Clock);
          IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> filteredReferencesGroupedByBlobIds = (IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) this.GetKeepUntilCacheMisses<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>(requestContext, domainId, (Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, BlobIdentifier>) (blobIdWithBlocks => blobIdWithBlocks.BlobId), this.BlobMetadataDomainProvider.GetMetadataProvider((ISecuredDomainRequest) domainRequest).MaxExpectedClockSkew, referencesGroupedByBlobIds);
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> results = new ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>();
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.TryReferenceWithBlocksAsyncInternal(vssProcessor, filteredReferencesGroupedByBlobIds, results, useHttpClient))).ConfigureAwait(true);
          PlatformBlobStore.UpdateKeepUntilCache(requestContext, domainId, (IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>) results);
          failed = PlatformBlobStore.GetFailed((IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>) results);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.TryReferenceWithBlocksAsyncException, ex);
          throw;
        }
      }
      return failed;
    }

    private string GetScopeFromBlobReferences(IEnumerable<BlobReference> blobReferences) => blobReferences.FirstOrDefault<BlobReference>((Func<BlobReference, bool>) (blobRef => !string.IsNullOrWhiteSpace(blobRef.Scope)))?.Scope;

    private bool GetScopeFromReferencesGroupedByBlobIdsWithBlocks(
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIdsWithBlocks,
      out string scope)
    {
      scope = (string) null;
      if (referencesGroupedByBlobIdsWithBlocks != null && referencesGroupedByBlobIdsWithBlocks.Any<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>())
      {
        foreach (KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> blobIdsWithBlock in (IEnumerable<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>) referencesGroupedByBlobIdsWithBlocks)
        {
          scope = this.GetScopeFromBlobReferences(blobIdsWithBlock.Value);
          if (!string.IsNullOrWhiteSpace(scope))
            break;
        }
      }
      return !string.IsNullOrWhiteSpace(scope);
    }

    private bool GetScopeFromReferencesGroupedByBlobIds(
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      out string scope)
    {
      scope = (string) null;
      if (referencesGroupedByBlobIds != null && referencesGroupedByBlobIds.Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>())
      {
        foreach (KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobId in (IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>) referencesGroupedByBlobIds)
        {
          scope = this.GetScopeFromBlobReferences(referencesGroupedByBlobId.Value);
          if (!string.IsNullOrWhiteSpace(scope))
            break;
        }
      }
      return !string.IsNullOrWhiteSpace(scope);
    }

    public async Task PutBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference reference)
    {
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.PutBlobAndReferenceAsyncCall, nameof (PutBlobAndReferenceAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        bool bypassExceptionOnRetryHashMatch = requestContext.IsFeatureEnabled("Blobstore.Features.BypassExceptionIfHashRetryMatches");
        try
        {
          if (reference != (BlobReference) null)
            reference.CheckReferenceCreatePermission(requestContext, this.Clock);
          int num = await BlobStoreUtils.EvaluateQuotaCapAsync(requestContext, ResourceName.Artifacts, reference.Scope).ConfigureAwait(true) ? 1 : 0;
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          using (Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).FindProvider(blobId))
            await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.PutBlobAndReferenceInternalAsync(provider, vssProcessor, blobId, blob, reference, useHttpClient, bypassExceptionOnRetryHashMatch))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.PutBlobAndReferenceAsyncException, ex);
          throw;
        }
        domainRequest = (SecuredDomainRequest) null;
      }
    }

    public async Task<IDictionary<ulong, PreauthenticatedUri>> GetSasUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      IDictionary<ulong, PreauthenticatedUri> sasUrisAsync;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.GetSasUrisAsyncCall, nameof (GetSasUrisAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Read);
          string policyId = this.GetAzureSharedAccessPolicyId(requestContext);
          IDictionary<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider> physicalMappedProviders = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).MapShardToProvider();
          ConcurrentDictionary<string, PreauthenticatedUri> uris = new ConcurrentDictionary<string, PreauthenticatedUri>();
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
          {
            Func<KeyValuePair<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>, Task> action = (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>, Task>) (async pair =>
            {
              ConcurrentDictionary<string, PreauthenticatedUri> concurrentDictionary = uris;
              string key = pair.Key;
              concurrentDictionary[key] = await pair.Value.GetContainerUri((VssRequestPump.Processor) processor, policyId).ConfigureAwait(false);
              concurrentDictionary = (ConcurrentDictionary<string, PreauthenticatedUri>) null;
              key = (string) null;
            });
            await NonSwallowingActionBlock.Create<KeyValuePair<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>>(action, new ExecutionDataflowBlockOptions()
            {
              BoundedCapacity = 2 * Environment.ProcessorCount,
              MaxDegreeOfParallelism = Environment.ProcessorCount,
              CancellationToken = requestContext.CancellationToken,
              EnsureOrdered = false
            }).SendAllAndCompleteSingleBlockNetworkAsync<KeyValuePair<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>>((IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>>) physicalMappedProviders, requestContext.CancellationToken).ConfigureAwait(false);
          })).ConfigureAwait(true);
          Dictionary<ulong, PreauthenticatedUri> dictionary = new Dictionary<ulong, PreauthenticatedUri>();
          foreach (KeyValuePair<string, IEnumerable<ulong>> keyValuePair in (IEnumerable<KeyValuePair<string, IEnumerable<ulong>>>) this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).MapIdToShard())
          {
            foreach (ulong key in keyValuePair.Value)
              dictionary[key] = uris[keyValuePair.Key];
          }
          sasUrisAsync = (IDictionary<ulong, PreauthenticatedUri>) dictionary;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.GetSasUrisAsyncException, ex);
          throw;
        }
      }
      return sasUrisAsync;
    }

    public async Task PutSingleBlockBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      BlobReference reference)
    {
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.PutSingleBlockBlobAndReferenceAsyncCall, nameof (PutSingleBlockBlobAndReferenceAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          reference.CheckReferenceCreatePermission(requestContext, this.Clock);
          int num = await BlobStoreUtils.EvaluateQuotaCapAsync(requestContext, ResourceName.Artifacts, reference.Scope).ConfigureAwait(true) ? 1 : 0;
          bool useHttpClient = BlobStoreUtils.UseHttpClientForStorageOperations(requestContext);
          Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).FindProvider(blobId);
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.PutSingleBlockBlobAndReferenceInternalAsync(vssProcessor, provider, blobId, blobBuffer, blockLength, reference, useHttpClient))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.PutSingleBlockBlobAndReferenceAsyncException, ex);
          throw;
        }
        domainRequest = (SecuredDomainRequest) null;
      }
    }

    public virtual async Task<HttpResponseMessage> CheckBlobExistsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri blobUri)
    {
      return await PlatformBlobStore.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, blobUri), HttpCompletionOption.ResponseHeadersRead, requestContext.CancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<PreauthenticatedUri> GetDownloadUriAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdWithHeaders blobId)
    {
      PreauthenticatedUri downloadUriAsync;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.GetDownloadUriAsyncCall, nameof (GetDownloadUriAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Read);
          SASUriExpiry expiry = SASUriExpiry.CreateExpiry(new SASUriExpiryPolicy(this.Clock), requestContext, blobId);
          string policyId = this.GetAzureSharedAccessPolicyId(requestContext);
          (string, Guid)[] sasTracing = ((string, Guid)[]) null;
          sasTracing = new (string, Guid)[2]
          {
            ("e2eid", requestContext.E2EId),
            ("session", requestContext.UniqueIdentifier)
          };
          Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).FindProvider(blobId.BlobId);
          PreauthenticatedUri preauthenticatedUri = await this.PumpOrInlineFromAsync<PreauthenticatedUri>(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<PreauthenticatedUri>>) (processor => provider.GetDownloadUriAsync((VssRequestPump.Processor) processor, blobId, expiry, policyId, sasTracing)));
          if (blobId.EdgeCache == EdgeCache.Allowed && requestContext.IsFeatureEnabled("BlobStore.Features.BlobStoreAzureFrontDoor"))
            preauthenticatedUri = new PreauthenticatedUri(requestContext.GetService<IBlobStoreBlobEdgeCachingService>().GetEdgeUri(preauthenticatedUri.NotNullUri, preauthenticatedUri.ExpiryTime.UtcDateTime - BlobEdgeCachingService.AzureFrontDoorSasUriExpiryBuffer), EdgeType.AzureFrontDoor);
          downloadUriAsync = preauthenticatedUri;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.GetDownloadUriAsyncException, ex);
          throw;
        }
      }
      return downloadUriAsync;
    }

    public virtual async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime = null)
    {
      IDictionary<BlobIdentifier, PreauthenticatedUri> downloadUrisAsync;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.GetDownloadUrisAsyncCall, nameof (GetDownloadUrisAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Read);
          SASUriExpiry expiry = SASUriExpiry.CreateExpiry(new SASUriExpiryPolicy(this.Clock), requestContext, expiryTime);
          ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri> results = new ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri>();
          string policyId = this.GetAzureSharedAccessPolicyId(requestContext);
          (string, Guid)[] sasTracing = ((string, Guid)[]) null;
          sasTracing = new (string, Guid)[2]
          {
            ("e2eid", requestContext.E2EId),
            ("session", requestContext.UniqueIdentifier)
          };
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (vssProcessor => this.GetDownloadUrisInternalAsync(vssProcessor, blobIds, expiry, sasTracing, results, policyId))).ConfigureAwait(true);
          if (edgeCache == EdgeCache.Allowed && requestContext.IsFeatureEnabled("BlobStore.Features.BlobStoreAzureFrontDoor"))
          {
            IBlobStoreBlobEdgeCachingService service = requestContext.GetService<IBlobStoreBlobEdgeCachingService>();
            foreach (BlobIdentifier blobId in blobIds)
              results[blobId] = new PreauthenticatedUri(service.GetEdgeUri(results[blobId].NotNullUri, results[blobId].ExpiryTime.UtcDateTime), EdgeType.AzureFrontDoor);
          }
          downloadUrisAsync = (IDictionary<BlobIdentifier, PreauthenticatedUri>) results;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.GetDownloadUrisAsyncException, ex);
          throw;
        }
      }
      return downloadUrisAsync;
    }

    public async Task<IDictionary<BlobIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil)
    {
      IDictionary<BlobIdentifier, KeepUntilResult?> dictionary;
      using (requestContext.Enter(ContentTracePoints.PlatformBlobStore.ValidateKeepUntilReferencesAsyncCall, nameof (ValidateKeepUntilReferencesAsync)))
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
        try
        {
          requestContext.CheckBlobRootPermission(BlobNamespace.Permissions.Read);
          IDictionary<BlobIdentifier, DateTime> hits = this.GetKeepUntilCacheHits(requestContext, (ISecuredDomainRequest) domainRequest, blobIds, keepUntil);
          ConcurrentDictionary<BlobIdentifier, KeepUntilResult?> results = new ConcurrentDictionary<BlobIdentifier, KeepUntilResult?>();
          foreach (KeyValuePair<BlobIdentifier, DateTime> keyValuePair in (IEnumerable<KeyValuePair<BlobIdentifier, DateTime>>) hits)
            results.TryAdd(keyValuePair.Key, new KeepUntilResult?(new KeepUntilResult(true, keyValuePair.Value)));
          Func<BlobIdentifier, bool> func;
          await this.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async vssProcessor =>
          {
            Func<BlobIdentifier, Task> action = (Func<BlobIdentifier, Task>) (async blobId =>
            {
              KeepUntilBlobReference? nullable = await this.BlobMetadataDomainProvider.GetMetadataProvider((ISecuredDomainRequest) domainRequest).GetKeepUntilReferenceAsync((VssRequestPump.Processor) vssProcessor, blobId).ConfigureAwait(false);
              if (!nullable.HasValue)
              {
                results.TryAdd(blobId, new KeepUntilResult?());
              }
              else
              {
                bool isSatisfied = nullable.Value.KeepUntil - this.BlobMetadataDomainProvider.GetMetadataProvider((ISecuredDomainRequest) domainRequest).MaxExpectedClockSkew > keepUntil;
                results.TryAdd(blobId, new KeepUntilResult?(new KeepUntilResult(isSatisfied, nullable.Value.KeepUntil - this.BlobMetadataDomainProvider.GetMetadataProvider((ISecuredDomainRequest) domainRequest).MaxExpectedClockSkew)));
              }
            });
            await NonSwallowingActionBlock.Create<BlobIdentifier>(action, new ExecutionDataflowBlockOptions()
            {
              BoundedCapacity = 2 * Environment.ProcessorCount,
              MaxDegreeOfParallelism = Environment.ProcessorCount,
              CancellationToken = vssProcessor.CancellationToken,
              EnsureOrdered = false
            }).SendAllAndCompleteSingleBlockNetworkAsync<BlobIdentifier>(blobIds.Where<BlobIdentifier>(func ?? (func = (Func<BlobIdentifier, bool>) (blobId => !hits.ContainsKey(blobId)))), vssProcessor.CancellationToken).ConfigureAwait(true);
          }));
          dictionary = (IDictionary<BlobIdentifier, KeepUntilResult?>) results;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.PlatformBlobStore.ValidateKeepUntilReferencesAsyncException, ex);
          throw;
        }
      }
      return dictionary;
    }

    private async Task GetDownloadUrisInternalAsync(
      VssRequestPump.SecuredDomainProcessor vssProcessor,
      IEnumerable<BlobIdentifier> blobIds,
      SASUriExpiry expiry,
      (string, Guid)[] sasTracing,
      ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri> results,
      string policyId)
    {
      Func<(IEnumerable<BlobIdentifier>, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider), Task> action = (Func<(IEnumerable<BlobIdentifier>, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider), Task>) (async pair =>
      {
        foreach (KeyValuePair<BlobIdentifier, PreauthenticatedUri> keyValuePair in (IEnumerable<KeyValuePair<BlobIdentifier, PreauthenticatedUri>>) await pair.BlobProvider.GetDownloadUrisAsync((VssRequestPump.Processor) vssProcessor, pair.BlobIds, expiry, policyId, sasTracing).ConfigureAwait(false))
          results.TryAdd(keyValuePair.Key, keyValuePair.Value);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = 2 * Environment.ProcessorCount;
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = vssProcessor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<(IEnumerable<BlobIdentifier>, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider)>(action, dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<(IEnumerable<BlobIdentifier>, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider)>(this.DomainProvider.GetDomain(vssProcessor.SecuredDomainRequest).FindProviders(blobIds), vssProcessor.CancellationToken).ConfigureAwait(true);
    }

    internal static IEnumerable<BlobStoragePhysicalNode> CreatePhysicalNodes(
      Func<int, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider> providerConstructor,
      int physicalNodeCount)
    {
      return Enumerable.Range(0, physicalNodeCount).Select<int, BlobStoragePhysicalNode>((Func<int, BlobStoragePhysicalNode>) (i => new BlobStoragePhysicalNode(providerConstructor(i), i.ToString((IFormatProvider) CultureInfo.InvariantCulture))));
    }

    internal static IShardManager<BlobStoragePhysicalNode> CreateBlobStorageShardManager(
      IEnumerable<BlobStoragePhysicalNode> physicalNodes,
      string shardingStrategy)
    {
      if (ShardingConstants.UseLinearSharding(shardingStrategy))
        return (IShardManager<BlobStoragePhysicalNode>) new LinearShardManager<BlobStoragePhysicalNode>(physicalNodes);
      if (ShardingConstants.UseConsistentHashingSharding(shardingStrategy))
        return (IShardManager<BlobStoragePhysicalNode>) new ConsistentHashShardManager<BlobStoragePhysicalNode>(physicalNodes, 128);
      throw new ArgumentException("No appropriate sharding manager is available for manager type: " + shardingStrategy);
    }

    protected virtual ITableClientFactory GetAzureTableClientFactory(
      IVssRequestContext systemRequestContext,
      string shardingStrategy,
      PhysicalDomainInfo physicalDomainInfo,
      string defaultTableName)
    {
      IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(systemRequestContext, physicalDomainInfo);
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(systemRequestContext.GetElevatedDeploymentRequestContext());
      Func<StrongBoxConnectionString, ITableClient> getTableClient = new Func<StrongBoxConnectionString, ITableClient>(systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudTableClientProvider>().GetTableClient);
      LocationMode? locationMode = tableLocationMode;
      string defaultTableName1 = defaultTableName;
      string shardingStrategy1 = shardingStrategy;
      int num = systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry") ? 1 : 0;
      return (ITableClientFactory) new PlatformBlobStore.BlobShardingAzureCloudTableClientFactory(connectionStrings, getTableClient, locationMode, defaultTableName1, shardingStrategy1, num != 0);
    }

    private ITableClient CreateTableClientFromConnectionString(
      StrongBoxConnectionString connectionString,
      LocationMode? locationMode)
    {
      return (ITableClient) new AzureCloudTableClientAdapter(connectionString.ConnectionString, locationMode, (IRetryPolicy) null);
    }

    protected virtual IAzureBlobContainerFactory CreateAzureContainerFactory(
      IVssRequestContext requestContext,
      StrongBoxConnectionString connectionString)
    {
      return (IAzureBlobContainerFactory) requestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudBlobClientProvider>().GetBlobClient(connectionString.StrongBoxItemKey, BlobStoreProviderConstants.BlobContainerPrefix);
    }

    protected virtual ITableClientFactory GetSQLTableClientFactory(IVssRequestContext requestContext) => (ITableClientFactory) new SQLTableClientFactory(this.GetDefaultTableName(requestContext, false));

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      ThreadPoolHelper.IncreaseThreadCounts(service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/WorkerThreadsPerCore", true, 180), service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/CompletionThreadsPerCore", true, 180));
      string blobProviderType = service.GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/BlobProviderImplementation", true, (string) null);
      string metadataProviderType = service.GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/BlobMetadataProviderImplementation", true, (string) null);
      string shardingStrategy = service.GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/ShardingStrategy", true, (string) null);
      this.ConfigureService(systemRequestContext, blobProviderType, metadataProviderType, shardingStrategy);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.MetadataProvider?.Dispose();
      this.BlobMetadataDomainProvider?.Dispose();
      base.ServiceEnd(systemRequestContext);
    }

    protected virtual AdminAzureTableBlobMetadataProviderWithTestHooks ConstructTableMetadataProvider(
      ITableClientFactory tableClientFactory,
      AzureTableBlobMetadataProviderOptions options)
    {
      this.MetadataProvider = (IAdminBlobMetadataProviderWithTestHooks) new AdminAzureTableBlobMetadataProviderWithTestHooks(tableClientFactory, options);
      return new AdminAzureTableBlobMetadataProviderWithTestHooks(tableClientFactory, options);
    }

    protected IEnumerable<BlobStoragePhysicalNode> CreatePhysicalNodes(
      IVssRequestContext systemRequestContext,
      string blobNameSuffix,
      PhysicalDomainInfo physicalDomainInfo)
    {
      string partitionKey = systemRequestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString();
      return this.GetAzureConnectionStrings(systemRequestContext, physicalDomainInfo).Select<StrongBoxConnectionString, BlobStoragePhysicalNode>((Func<StrongBoxConnectionString, BlobStoragePhysicalNode>) (connectionString =>
      {
        IAzureBlobContainerFactory containerFactory = this.CreateAzureContainerFactory(systemRequestContext, connectionString);
        AzureStorageAccountInfo accountInfo = StorageAccountUtilities.GetAccountInfo(connectionString.ConnectionString);
        return new BlobStoragePhysicalNode((Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider) new AzureBlobBlobProvider(partitionKey, containerFactory, blobNameSuffix, systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry")), accountInfo.Name);
      }));
    }

    protected IEnumerable<BlobStoragePhysicalNode> CreatePhysicalNodes(
      IVssRequestContext systemRequestContext,
      string blobNameSuffix,
      ProjectDomainInfo projectDomainInfo)
    {
      IDomainId physicalDomainId = projectDomainInfo.PhysicalDomainId;
      PhysicalDomainInfo result = (PhysicalDomainInfo) systemRequestContext.GetService<HostDomainStoreService>().GetDomainForOrganizationAsync(systemRequestContext, physicalDomainId).Result;
      string partitionKey = projectDomainInfo.PartitionName;
      return this.GetAzureConnectionStrings(systemRequestContext, result).Select<StrongBoxConnectionString, BlobStoragePhysicalNode>((Func<StrongBoxConnectionString, BlobStoragePhysicalNode>) (connectionString =>
      {
        IAzureBlobContainerFactory containerFactory = this.CreateAzureContainerFactory(systemRequestContext, connectionString);
        AzureStorageAccountInfo accountInfo = StorageAccountUtilities.GetAccountInfo(connectionString.ConnectionString);
        return new BlobStoragePhysicalNode((Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider) new AzureBlobBlobProvider(partitionKey, containerFactory, blobNameSuffix, systemRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry")), accountInfo.Name);
      }));
    }

    private static void UpdateKeepUntilCache(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> results)
    {
      IBlobKeepUntilCacheService service = requestContext.GetService<IBlobKeepUntilCacheService>();
      foreach (KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>> result in (IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>>) results)
      {
        DateTime? nullable = result.Value.Where<ReferenceResult>((Func<ReferenceResult, bool>) (r => r.Success)).Select<ReferenceResult, DateTime?>((Func<ReferenceResult, DateTime?>) (r => r.KeepUntilToCache)).Max<DateTime?>();
        if (nullable.HasValue)
          service.SetKeepUntil(requestContext, domainId, result.Key, nullable.Value);
      }
    }

    private static IDictionary<BlobIdentifier, IEnumerable<BlobReference>> GetFailed(
      IDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> results)
    {
      return (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) results.Where<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>, bool>) (kvp => kvp.Value.Any<ReferenceResult>((Func<ReferenceResult, bool>) (v => !v.Success)))).ToDictionary<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>, BlobIdentifier, IEnumerable<BlobReference>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>, BlobIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<BlobIdentifier, IEnumerable<ReferenceResult>>, IEnumerable<BlobReference>>) (kvp => kvp.Value.FailedRefs()));
    }

    private string GetDefaultTableName(IVssRequestContext requestContext, bool withHostId) => BlobStoreProviderConstants.MetadataPrefix + (withHostId ? requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString() : string.Empty);

    private IDictionary<BlobIdentifier, DateTime> GetKeepUntilCacheHits(
      IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil)
    {
      IBlobKeepUntilCacheService service = requestContext.GetService<IBlobKeepUntilCacheService>();
      Dictionary<BlobIdentifier, DateTime> keepUntilCacheHits = new Dictionary<BlobIdentifier, DateTime>();
      foreach (BlobIdentifier blobId in (IEnumerable<BlobIdentifier>) blobIds)
      {
        DateTime keepUntil1;
        if (service.TryGetKeepUntil(requestContext, domainRequest.DomainId, blobId, out keepUntil1) && keepUntil1 - this.BlobMetadataDomainProvider.GetMetadataProvider(domainRequest).MaxExpectedClockSkew > keepUntil)
          keepUntilCacheHits.Add(blobId, keepUntil1 - this.BlobMetadataDomainProvider.GetMetadataProvider(domainRequest).MaxExpectedClockSkew);
      }
      return (IDictionary<BlobIdentifier, DateTime>) keepUntilCacheHits;
    }

    private Dictionary<T, IEnumerable<BlobReference>> GetKeepUntilCacheMisses<T>(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Func<T, BlobIdentifier> getBlobId,
      TimeSpan maxClockSkew,
      IDictionary<T, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      IBlobKeepUntilCacheService service = requestContext.GetService<IBlobKeepUntilCacheService>();
      Dictionary<T, IEnumerable<BlobReference>> untilCacheMisses1 = new Dictionary<T, IEnumerable<BlobReference>>();
      foreach (KeyValuePair<T, IEnumerable<BlobReference>> referencesGroupedByBlobId in (IEnumerable<KeyValuePair<T, IEnumerable<BlobReference>>>) referencesGroupedByBlobIds)
      {
        BlobIdentifier blobId = getBlobId(referencesGroupedByBlobId.Key);
        IEnumerable<BlobReference> references = referencesGroupedByBlobId.Value;
        List<BlobReference> untilCacheMisses2 = service.GetKeepUntilCacheMisses(requestContext, domainId, blobId, references, maxClockSkew);
        if (untilCacheMisses2.Any<BlobReference>())
          untilCacheMisses1.Add(referencesGroupedByBlobId.Key, (IEnumerable<BlobReference>) untilCacheMisses2);
      }
      return untilCacheMisses1;
    }

    private void ConfigureBlobStorage(
      IVssRequestContext systemRequestContext,
      string blobProviderType,
      string shardingStrategy)
    {
      using (systemRequestContext.Enter(ContentTracePoints.PlatformBlobStore.ConfigureBlobStorageCall, nameof (ConfigureBlobStorage)))
      {
        IEnumerable<BlobStoragePhysicalNode> physicalNodes = (IEnumerable<BlobStoragePhysicalNode>) null;
        switch (blobProviderType)
        {
          case "MEMORY":
            physicalNodes = PlatformBlobStore.CreatePhysicalNodes((Func<int, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>) (shardIndex => (Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider) new InMemoryBlobProvider(string.Format("{0}_shard{1}", (object) this.TestBlobNamespace, (object) shardIndex))), 24);
            break;
          case "FILE":
            string fileRoot = Environment.GetEnvironmentVariable("%CR_FILE_PROVIDER_ROOT%") ?? Environment.ExpandEnvironmentVariables("%TEMP%\\cr_root");
            physicalNodes = PlatformBlobStore.CreatePhysicalNodes((Func<int, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>) (shardIndex => (Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider) new FileBlobProvider(Path.Combine(fileRoot, "content_" + shardIndex.ToString()))), 24);
            break;
          case "AZURE":
            if (systemRequestContext.AllowHostDomainAdminOperations())
            {
              AdminHostDomainStoreService service = systemRequestContext.GetService<AdminHostDomainStoreService>();
              List<PhysicalDomainInfo> list = service.GetPhysicalDomainsForOrganizationForAdminAsync(systemRequestContext).Result.ToList<PhysicalDomainInfo>();
              PhysicalDomainInfo physicalDomainInfo1 = list.SingleOrDefault<PhysicalDomainInfo>((Func<PhysicalDomainInfo, bool>) (dom => dom.IsDefault));
              if (physicalDomainInfo1 == null)
                throw new InvalidOperationException("ConfigureBlobStorage: Cannot configure the back-end, defaultDomain - is not configured - blobProviderType: " + blobProviderType + ".");
              HashSet<ShardedBlobProviderDomain> physicalDomains = new HashSet<ShardedBlobProviderDomain>();
              foreach (PhysicalDomainInfo physicalDomainInfo2 in list)
                physicalDomains.Add(new ShardedBlobProviderDomain(physicalDomainInfo2.DomainId, PlatformBlobStore.CreateBlobStorageShardManager(this.CreatePhysicalNodes(systemRequestContext, BlobStoreProviderConstants.BlobStoreSuffix, physicalDomainInfo2) ?? throw new ArgumentException(string.Format("{0}: cannot create physical nodes for domain: {1} ", (object) nameof (ConfigureBlobStorage), (object) physicalDomainInfo2.DomainId) + blobProviderType), shardingStrategy)));
              if (systemRequestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains"))
              {
                foreach (ProjectDomainInfo projectDomainInfo in service.GetProjectDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
                  physicalDomains.Add(new ShardedBlobProviderDomain(projectDomainInfo.DomainId, PlatformBlobStore.CreateBlobStorageShardManager(this.CreatePhysicalNodes(systemRequestContext, BlobStoreProviderConstants.BlobStoreSuffix, projectDomainInfo) ?? throw new ArgumentException(string.Format("{0}: cannot create physical nodes for domain: {1} ", (object) nameof (ConfigureBlobStorage), (object) projectDomainInfo.DomainId) + blobProviderType), shardingStrategy)));
              }
              this.DomainProvider = (IDomainProvider<IBlobProviderDomain>) new Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainProvider<IBlobProviderDomain>(physicalDomainInfo1.DomainId, (IEnumerable<IBlobProviderDomain>) physicalDomains);
              return;
            }
            physicalNodes = this.CreatePhysicalNodes(systemRequestContext, BlobStoreProviderConstants.BlobStoreSuffix, (PhysicalDomainInfo) null);
            break;
          case "SQL":
            physicalNodes = PlatformBlobStore.CreatePhysicalNodes((Func<int, Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>) (shardIndex => (Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider) new SqlBlobProvider(BlobStoreProviderConstants.BlobStoreSuffix)), 1);
            break;
        }
        if (physicalNodes == null)
          throw new ArgumentException("Unknown blob provider type: " + blobProviderType);
        this.DomainProvider = (IDomainProvider<IBlobProviderDomain>) new Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainProvider<IBlobProviderDomain>(WellKnownDomainIds.DefaultDomainId, (IEnumerable<IBlobProviderDomain>) new ShardedBlobProviderDomain[1]
        {
          new ShardedBlobProviderDomain(WellKnownDomainIds.DefaultDomainId, PlatformBlobStore.CreateBlobStorageShardManager(physicalNodes, shardingStrategy))
        });
      }
    }

    private async void ConfigureMetadataStorage(
      IVssRequestContext systemRequestContext,
      string metadataProviderType,
      string blobProviderType,
      string shardingStrategy)
    {
      PlatformBlobStore platformBlobStore = this;
      using (systemRequestContext.Enter(ContentTracePoints.PlatformBlobStore.ConfigureMetadataStorageCall, nameof (ConfigureMetadataStorage)))
      {
        List<(IDomainId, IAdminBlobMetadataProviderWithTestHooks)> providers = new List<(IDomainId, IAdminBlobMetadataProviderWithTestHooks)>();
        switch (metadataProviderType)
        {
          case "MEMORYTABLEBLOBMETADATAPROVIDER":
            ITableClientFactory tableClientFactory1 = (ITableClientFactory) new MemoryTableClientFactory(MemoryTableStorage.Global, platformBlobStore.TestBlobNamespace + "_" + platformBlobStore.GetDefaultTableName(systemRequestContext, true));
            providers.Add((WellKnownDomainIds.DefaultDomainId, (IAdminBlobMetadataProviderWithTestHooks) platformBlobStore.ConstructTableMetadataProvider(tableClientFactory1, new AzureTableBlobMetadataProviderOptions()
            {
              TotalExistsRows = 2
            })));
            break;
          case "AZURETABLEBLOBMETADATAPROVIDER":
            if (systemRequestContext.AllowHostDomainAdminOperations())
            {
              AdminHostDomainStoreService domainStoreService = systemRequestContext.GetService<AdminHostDomainStoreService>();
              foreach (PhysicalDomainInfo physicalDomainInfo in domainStoreService.GetPhysicalDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
              {
                ITableClientFactory tableClientFactory2 = platformBlobStore.GetAzureTableClientFactory(systemRequestContext, shardingStrategy, physicalDomainInfo, platformBlobStore.GetDefaultTableName(systemRequestContext, true));
                providers.Add((physicalDomainInfo.DomainId, (IAdminBlobMetadataProviderWithTestHooks) platformBlobStore.ConstructTableMetadataProvider(tableClientFactory2, new AzureTableBlobMetadataProviderOptions())));
              }
              if (systemRequestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains"))
              {
                foreach (ProjectDomainInfo domInfo in domainStoreService.GetProjectDomainsForOrganizationForAdminAsync(systemRequestContext).Result)
                {
                  PhysicalDomainInfo organizationAsync = (PhysicalDomainInfo) await domainStoreService.GetDomainForOrganizationAsync(systemRequestContext, domInfo.PhysicalDomainId);
                  ITableClientFactory tableClientFactory3 = platformBlobStore.GetAzureTableClientFactory(systemRequestContext, shardingStrategy, organizationAsync, BlobStoreProviderConstants.MetadataPrefix + domInfo.PartitionName);
                  providers.Add((domInfo.DomainId, (IAdminBlobMetadataProviderWithTestHooks) platformBlobStore.ConstructTableMetadataProvider(tableClientFactory3, new AzureTableBlobMetadataProviderOptions())));
                }
              }
              domainStoreService = (AdminHostDomainStoreService) null;
              break;
            }
            ITableClientFactory tableClientFactory4 = platformBlobStore.GetAzureTableClientFactory(systemRequestContext, shardingStrategy, (PhysicalDomainInfo) null, platformBlobStore.GetDefaultTableName(systemRequestContext, true));
            providers.Add((WellKnownDomainIds.DefaultDomainId, (IAdminBlobMetadataProviderWithTestHooks) platformBlobStore.ConstructTableMetadataProvider(tableClientFactory4, new AzureTableBlobMetadataProviderOptions())));
            break;
          case "SQLTABLEBLOBMETADATAPROVIDER":
            ITableClientFactory tableClientFactory5 = platformBlobStore.GetSQLTableClientFactory(systemRequestContext);
            providers.Add((WellKnownDomainIds.DefaultDomainId, (IAdminBlobMetadataProviderWithTestHooks) platformBlobStore.ConstructTableMetadataProvider(tableClientFactory5, new AzureTableBlobMetadataProviderOptions()
            {
              TotalExistsRows = 1
            })));
            break;
          default:
            throw new ArgumentException("Unknown metadata provider type: " + blobProviderType);
        }
        platformBlobStore.BlobMetadataDomainProvider = (IDomainBlobMetadataProvider<IAdminBlobMetadataProviderWithTestHooks>) new DomainBlobMetadataProvider<IAdminBlobMetadataProviderWithTestHooks>((IEnumerable<(IDomainId, IAdminBlobMetadataProviderWithTestHooks)>) providers);
        providers = (List<(IDomainId, IAdminBlobMetadataProviderWithTestHooks)>) null;
      }
    }

    private string GetAzureSharedAccessPolicyId(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/BlobStore/SharedAccessPolicy/Id", 1).ToString();

    private async Task TryReferenceInternalAsync(
      VssRequestPump.SecuredDomainProcessor vssProcessor,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> filteredReferencesGroupedByBlobIds,
      ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> results)
    {
      Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, Task> action = (Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, Task>) (async grouping =>
      {
        await PlatformBlobStore.ConcurrentReferenceOperations.WaitAsync().ConfigureAwait(false);
        try
        {
          ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> concurrentDictionary = results;
          BlobIdentifier key = grouping.Key;
          concurrentDictionary[key] = await this.BlobMetadataDomainProvider.GetMetadataProvider(vssProcessor.SecuredDomainRequest).TryReferenceAsync((VssRequestPump.Processor) vssProcessor, grouping.Key, grouping.Value).ConfigureAwait(false);
          concurrentDictionary = (ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>>) null;
          key = (BlobIdentifier) null;
        }
        finally
        {
          PlatformBlobStore.ConcurrentReferenceOperations.Release();
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = 2 * Environment.ProcessorCount;
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = vssProcessor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>(action, dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>((IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>) filteredReferencesGroupedByBlobIds, vssProcessor.CancellationToken).ConfigureAwait(true);
    }

    private async Task TryReferenceWithBlocksAsyncInternal(
      VssRequestPump.SecuredDomainProcessor vssProcessor,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> filteredReferencesGroupedByBlobIds,
      ConcurrentDictionary<BlobIdentifier, IEnumerable<ReferenceResult>> result,
      bool useHttpClient)
    {
      Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, Task> action = (Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, Task>) (async grouping =>
      {
        using (Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = this.DomainProvider.GetDomain(vssProcessor.SecuredDomainRequest).FindProvider(grouping.Key.BlobId))
        {
          PlatformBlobStore.SealBlobResult sealBlobResult = await this.TrySealBlobAsync(vssProcessor, provider, grouping.Key, grouping.Value, useHttpClient).ConfigureAwait(false);
          result[grouping.Key.BlobId] = sealBlobResult.ReferenceResults;
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = 2 * Environment.ProcessorCount;
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = vssProcessor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>(action, dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>((IEnumerable<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>) filteredReferencesGroupedByBlobIds, vssProcessor.CancellationToken).ConfigureAwait(true);
    }

    protected async Task PutBlobBlockInternalAsync(
      VssRequestPump.Processor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      bool useHttpClient,
      bool bypassExceptionOnRetryHashMatch)
    {
      if (blobId.IsOfNothing())
      {
        if (blockLength != 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Provided blobBuffer should be empty for blobId {0}", (object) blobId));
      }
      else
      {
        if (!await this.ValidateBlockHashAsync(processor, blobId, blobBuffer, blockLength, blockHash, bypassExceptionOnRetryHashMatch))
          throw new ArgumentException(string.Format("Provided block hash '{0}' does not match the hash of the content received for blob '{1}'.", (object) blockHash, (object) blobId.ValueString));
        await blobProvider.PutBlobBlockByteArrayAsync(processor, blobId, new ArraySegment<byte>(blobBuffer, 0, blockLength), BlockInfo.ConstructBlockName(blockHash), useHttpClient).ConfigureAwait(false);
      }
    }

    private async Task<bool> ValidateBlockHashAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      bool bypassExceptionOnRetryHashMatch)
    {
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash1 = blobBuffer.CalculateBlockHash(blockLength, blobId.GetBlobHasher());
      if (!(blockHash1 != blockHash))
        return true;
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash retriedBlockHash = blobBuffer.CalculateBlockHash(blockLength, blobId.GetBlobHasher());
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash retriedSha256Hash = Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.HashBlockCng(blobBuffer, blockLength);
      byte[] numArray1 = blockLength > 100 ? ((IEnumerable<byte>) blobBuffer).Take<byte>(5).ToArray<byte>() : Array.Empty<byte>();
      byte[] numArray2 = blockLength > 100 ? ((IEnumerable<byte>) blobBuffer).Skip<byte>(blockLength - 5).Take<byte>(5).ToArray<byte>() : Array.Empty<byte>();
      await processor.TraceErrorAsync(ContentTracePoints.PlatformBlobStore.PutBlobBlockHashMismatch, string.Format("Provided block hash '{0}' does not match the hash of the content received for blob '{1}'.", (object) blockHash, (object) blobId.ValueString) + string.Format("Server used {0} to calculate the hash '{1}' with a sent length of {2}.  ", Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.BCryptAvailable ? (object) "BCrypt" : (object) "SHA256", (object) blockHash1, (object) blockLength) + string.Format("After retry, computed hash was '{0}'.  ", (object) retriedBlockHash) + string.Format("Third attempt with SHA256 algorithm was '{0}'.  ", (object) retriedSha256Hash) + "Data: " + BitConverter.ToString(numArray1) + "..." + BitConverter.ToString(numArray2));
      return (retriedBlockHash == blockHash || retriedSha256Hash == blockHash) && bypassExceptionOnRetryHashMatch;
    }

    protected async Task<PlatformBlobStore.WriteBlockListResult> TryWriteBlockListAsync(
      VssRequestPump.Processor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdWithBlocks,
      bool useHttpClient)
    {
      int retries = 100;
      if (blobIdWithBlocks.BlobId.IsOfNothing())
        return new PlatformBlobStore.WriteBlockListResult((await provider.PutBlobByteArrayAsync(processor, blobIdWithBlocks.BlobId, (string) null, Array.Empty<byte>(), useHttpClient).ConfigureAwait(false)).Etag, (string) null);
      while (retries-- > 0)
      {
        EtagValue<IList<BlockInfo>> etagValue1 = await provider.GetBlockListAsync(processor, blobIdWithBlocks.BlobId).ConfigureAwait(false);
        if (etagValue1.Value == null)
          return new PlatformBlobStore.WriteBlockListResult((string) null, string.Format("No blocks have been uploaded, yet attempted to seal {0}.", (object) blobIdWithBlocks));
        if (blobIdWithBlocks.BlockHashes.SequenceEqual<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>(etagValue1.Value.Where<BlockInfo>((Func<BlockInfo, bool>) (block => block.Committed)).Select<BlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>((Func<BlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (block => block.GetBlockHash()))))
          return new PlatformBlobStore.WriteBlockListResult(etagValue1.Etag, (string) null);
        Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, BlockInfo> dictionary = etagValue1.Value.ToDictionary<BlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, BlockInfo>((Func<BlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (b => b.GetBlockHash()), (Func<BlockInfo, BlockInfo>) (b => b));
        for (int index = 0; index < blobIdWithBlocks.BlockHashes.Count; ++index)
        {
          Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash = blobIdWithBlocks.BlockHashes[index];
          BlockInfo blockInfo;
          if (!dictionary.TryGetValue(blockHash, out blockInfo))
            return new PlatformBlobStore.WriteBlockListResult((string) null, "One or more blocks are not present.");
          if (blockInfo.Length > 2097152L)
            return new PlatformBlobStore.WriteBlockListResult((string) null, "One or more of the blocks is too large.");
          if (index + 1 != blobIdWithBlocks.BlockHashes.Count && blockInfo.Length != 2097152L)
            return new PlatformBlobStore.WriteBlockListResult((string) null, "One or more of the non-terminal blocks is not block-sized.");
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<string> blockIds = blobIdWithBlocks.BlockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>(PlatformBlobStore.\u003C\u003EO.\u003C0\u003E__ConstructBlockName ?? (PlatformBlobStore.\u003C\u003EO.\u003C0\u003E__ConstructBlockName = new Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>(BlockInfo.ConstructBlockName)));
        EtagValue<bool> etagValue2 = await provider.PutBlockListAsync(processor, blobIdWithBlocks.BlobId, etagValue1.Etag, blockIds).ConfigureAwait(false);
        if (etagValue2.Value)
          return new PlatformBlobStore.WriteBlockListResult(etagValue2.Etag, (string) null);
      }
      return new PlatformBlobStore.WriteBlockListResult((string) null, "Failed to seal blob due to contention.");
    }

    private async Task<RemoveReferencesResult> RemoveReferencesAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      BlobIdentifier blobId,
      IEnumerable<IdBlobReference> references)
    {
      IEnumerable<RemoveReferenceResult> results = await this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).RemoveReferencesAsync((VssRequestPump.Processor) processor, blobId, references).ConfigureAwait(false);
      RemoveReferencesResult referencesResult = new RemoveReferencesResult(results, (await this.CheckForDeleteAsync(processor, blobProvider, blobId).ConfigureAwait(false)).HasValue);
      results = (IEnumerable<RemoveReferenceResult>) null;
      return referencesResult;
    }

    protected async Task<long?> CheckForDeleteAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      BlobIdentifier blobId)
    {
      for (int attempt = 0; attempt < 20; ++attempt)
      {
        IBlobMetadata metadata = await this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false);
        switch (metadata.StoredReferenceState)
        {
          case BlobReferenceState.AddedBlob:
            if (this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).HasReferences(metadata))
              return new long?();
            metadata.DesiredReferenceState = BlobReferenceState.DeletingBlob;
            if (await this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryUpdateAsync((VssRequestPump.Processor) processor, metadata).ConfigureAwait(false))
            {
              IBlobMetadata blobMetadata = await this.CleanupMetadataAsync(processor, blobProvider, blobId).ConfigureAwait(false);
              return new long?(metadata.BlobLength.GetValueOrDefault());
            }
            metadata = (IBlobMetadata) null;
            continue;
          case BlobReferenceState.DeletingBlob:
            IBlobMetadata blobMetadata1 = await this.CleanupMetadataAsync(processor, blobProvider, blobId).ConfigureAwait(false);
            return new long?(metadata.BlobLength.GetValueOrDefault());
          default:
            return new long?();
        }
      }
      throw new PlatformBlobStore.BlobMetadataContentionException("Unable to successfully set metadata to 'DeletingBlob' state.");
    }

    private async Task<IBlobMetadata> CleanupMetadataAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      BlobIdentifier blobId)
    {
      int attempt = 0;
      for (attempt = 0; attempt < 20; ++attempt)
      {
        IBlobMetadata metadata = await this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false);
        switch (metadata.StoredReferenceState)
        {
          case BlobReferenceState.Empty:
          case BlobReferenceState.AddingBlob:
          case BlobReferenceState.AddedBlob:
            return metadata;
          case BlobReferenceState.DeletingBlob:
            ConfiguredTaskAwaitable<EtagValue<bool>> configuredTaskAwaitable1 = blobProvider.DeleteBlobAsync((VssRequestPump.Processor) processor, metadata.BlobId, metadata.BlobEtag).ConfigureAwait(false);
            EtagValue<bool> etagValue = await configuredTaskAwaitable1;
            bool blobIsDeleted = etagValue.Value;
            ConfiguredTaskAwaitable<bool> configuredTaskAwaitable2;
            if (!blobIsDeleted)
            {
              metadata.BlobEtag = etagValue.Etag;
              configuredTaskAwaitable2 = this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryUpdateAsync((VssRequestPump.Processor) processor, metadata).ConfigureAwait(false);
              if (await configuredTaskAwaitable2)
              {
                configuredTaskAwaitable1 = blobProvider.DeleteBlobAsync((VssRequestPump.Processor) processor, metadata.BlobId, metadata.BlobEtag).ConfigureAwait(false);
                blobIsDeleted = (await configuredTaskAwaitable1).Value;
              }
            }
            if (blobIsDeleted)
            {
              configuredTaskAwaitable2 = this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryDeleteAsync((VssRequestPump.Processor) processor, metadata).ConfigureAwait(false);
              int num = await configuredTaskAwaitable2 ? 1 : 0;
            }
            metadata = (IBlobMetadata) null;
            continue;
          default:
            throw new InvalidOperationException();
        }
      }
      throw new PlatformBlobStore.BlobMetadataContentionException(string.Format("Could not cleanup metadata after {0} attempts.", (object) 20));
    }

    protected internal virtual IEnumerable<BlobReference> AddMinimumKeepUntil(
      IEnumerable<BlobReference> references,
      out IEnumerable<KeepUntilBlobReference> givenKeepUntilReferences)
    {
      DateTime date = this.Clock.Now.UtcDateTime.Add(this.MinimumKeepUntil);
      givenKeepUntilReferences = ((IEnumerable<ITaggedUnion<IdBlobReference, KeepUntilBlobReference>>) references).SelectTwos<IdBlobReference, KeepUntilBlobReference>();
      if (givenKeepUntilReferences.Any<KeepUntilBlobReference>() && !(givenKeepUntilReferences.Max<KeepUntilBlobReference>().KeepUntil < date))
        return references;
      return references.Concat<BlobReference>((IEnumerable<BlobReference>) new BlobReference[1]
      {
        new BlobReference(date)
      });
    }

    internal static IEnumerable<ReferenceResult> FixKeepUntilReferencesResult(
      IEnumerable<ReferenceResult> results,
      IEnumerable<KeepUntilBlobReference> givenReferences)
    {
      DateTime? keepUntilToCache = results.Where<ReferenceResult>((Func<ReferenceResult, bool>) (x => x.Success)).MaxOrDefault<ReferenceResult, DateTime?>((Func<ReferenceResult, DateTime?>) (x => x.KeepUntilToCache));
      IEnumerable<ReferenceResult> second = givenReferences.Select<KeepUntilBlobReference, ReferenceResult>((Func<KeepUntilBlobReference, ReferenceResult>) (x =>
      {
        BlobReference reference = new BlobReference(x);
        DateTime keepUntil = x.KeepUntil;
        DateTime? nullable = keepUntilToCache;
        int num = nullable.HasValue ? (keepUntil <= nullable.GetValueOrDefault() ? 1 : 0) : 0;
        DateTime? keepUntilToCache1 = keepUntilToCache;
        return new ReferenceResult(reference, num != 0, keepUntilToCache1);
      }));
      return results.Where<ReferenceResult>((Func<ReferenceResult, bool>) (x => !x.Reference.IsKeepUntil)).Concat<ReferenceResult>(second);
    }

    private async Task<PlatformBlobStore.SealBlobResult> TrySealBlobAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdWithBlocks,
      IEnumerable<BlobReference> originalReferences,
      bool useHttpClient)
    {
      PlatformBlobStore platformBlobStore = this;
      for (int attempt = 0; attempt < 20; ++attempt)
      {
        // ISSUE: reference to a compiler-generated method
        IEnumerable<BlobReference> references = originalReferences.Where<BlobReference>(new Func<BlobReference, bool>(platformBlobStore.\u003CTrySealBlobAsync\u003Eb__77_0));
        ConfiguredTaskAwaitable<IBlobMetadata> configuredTaskAwaitable = platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAsync((VssRequestPump.Processor) processor, blobIdWithBlocks.BlobId).ConfigureAwait(false);
        IBlobMetadata metadata = await configuredTaskAwaitable;
        switch (metadata.StoredReferenceState)
        {
          case BlobReferenceState.Empty:
            metadata.DesiredReferenceState = BlobReferenceState.AddingBlob;
            int num = await platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryUpdateAsync((VssRequestPump.Processor) processor, metadata).ConfigureAwait(false) ? 1 : 0;
            break;
          case BlobReferenceState.AddingBlob:
            PlatformBlobStore.WriteBlockListResult writeBlockListResult = await platformBlobStore.TryWriteBlockListAsync((VssRequestPump.Processor) processor, blobProvider, blobIdWithBlocks, useHttpClient).ConfigureAwait(false);
            if (!writeBlockListResult.Success)
              return new PlatformBlobStore.SealBlobResult(new PlatformBlobStore.WriteBlockListResult?(writeBlockListResult), references.Select<BlobReference, ReferenceResult>((Func<BlobReference, ReferenceResult>) (r => new ReferenceResult(r, false, new DateTime?()))));
            metadata.DesiredReferenceState = BlobReferenceState.AddedBlob;
            metadata.BlobEtag = writeBlockListResult.FinalEtag;
            IBlobMetadata blobMetadata = metadata;
            blobMetadata.BlobLength = await blobProvider.GetBlobLengthAsync((VssRequestPump.Processor) processor, blobIdWithBlocks.BlobId).ConfigureAwait(false);
            blobMetadata = (IBlobMetadata) null;
            metadata.BlobAddedTime = new DateTimeOffset?(DateTimeOffset.UtcNow);
            IEnumerable<KeepUntilBlobReference> givenKeepUntilReferences;
            references = platformBlobStore.AddMinimumKeepUntil(references, out givenKeepUntilReferences);
            List<ReferenceResult> list1 = (await platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryReferenceWithBlobAsync((VssRequestPump.Processor) processor, metadata, references).ConfigureAwait(false)).ToList<ReferenceResult>();
            if (!list1.Any<ReferenceResult>((Func<ReferenceResult, bool>) (r => !r.Success)))
              return new PlatformBlobStore.SealBlobResult(new PlatformBlobStore.WriteBlockListResult?(), PlatformBlobStore.FixKeepUntilReferencesResult((IEnumerable<ReferenceResult>) list1, givenKeepUntilReferences));
            break;
          case BlobReferenceState.AddedBlob:
            List<ReferenceResult> list2 = (await platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryReferenceAsync((VssRequestPump.Processor) processor, blobIdWithBlocks.BlobId, references).ConfigureAwait(false)).ToList<ReferenceResult>();
            if (!list2.Any<ReferenceResult>((Func<ReferenceResult, bool>) (r => !r.Success)))
              return new PlatformBlobStore.SealBlobResult(new PlatformBlobStore.WriteBlockListResult?(), (IEnumerable<ReferenceResult>) list2);
            break;
          default:
            configuredTaskAwaitable = platformBlobStore.CleanupMetadataAsync(processor, blobProvider, metadata.BlobId).ConfigureAwait(false);
            IBlobMetadata blobMetadata1 = await configuredTaskAwaitable;
            break;
        }
      }
      throw new TimeoutException(string.Format("Could not seal blob after {0} attempts.", (object) 20));
    }

    private async Task PutSingleBlockBlobAndReferenceInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      BlobReference reference,
      bool useHttpClient)
    {
      IEnumerable<BlobReference> references;
      ArraySegment<byte> content;
      if (reference.IsInThePast(this.Clock))
      {
        references = (IEnumerable<BlobReference>) null;
        content = new ArraySegment<byte>();
      }
      else
      {
        references = (IEnumerable<BlobReference>) new BlobReference[1]
        {
          reference
        };
        content = new ArraySegment<byte>(blobBuffer, 0, blockLength);
        BlobIdentifier blobIdentifier = content.CalculateBlobIdentifier(blobId.GetBlobHasher());
        if (blobIdentifier != blobId)
          throw new ContentDoesNotMatchException(blobId, blobIdentifier);
        int attempt = 0;
        for (attempt = 0; attempt < 20; ++attempt)
        {
          ConfiguredTaskAwaitable<IBlobMetadata> configuredTaskAwaitable1 = this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false);
          IBlobMetadata metadata = await configuredTaskAwaitable1;
          ConfiguredTaskAwaitable<IEnumerable<BlobReference>> configuredTaskAwaitable2;
          switch (metadata.StoredReferenceState)
          {
            case BlobReferenceState.Empty:
              metadata.DesiredReferenceState = BlobReferenceState.AddingBlob;
              int num = await this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryUpdateAsync((VssRequestPump.Processor) processor, metadata).ConfigureAwait(false) ? 1 : 0;
              break;
            case BlobReferenceState.AddingBlob:
              EtagValue<bool> etagValue = await blobProvider.PutBlobByteArrayAsync((VssRequestPump.Processor) processor, blobId, (string) null, content, useHttpClient).ConfigureAwait(false);
              metadata.DesiredReferenceState = BlobReferenceState.AddedBlob;
              metadata.BlobEtag = etagValue.Etag;
              metadata.BlobLength = new long?((long) content.Count);
              metadata.BlobAddedTime = new DateTimeOffset?(DateTimeOffset.UtcNow);
              references = this.AddMinimumKeepUntil(references, out IEnumerable<KeepUntilBlobReference> _);
              configuredTaskAwaitable2 = this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryReferenceWithBlobAsync((VssRequestPump.Processor) processor, metadata, references).FailedRefsAsync().ConfigureAwait(false);
              if (!(await configuredTaskAwaitable2).ToList<BlobReference>().Any<BlobReference>())
              {
                references = (IEnumerable<BlobReference>) null;
                content = new ArraySegment<byte>();
                return;
              }
              break;
            case BlobReferenceState.AddedBlob:
              configuredTaskAwaitable2 = this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryReferenceAsync((VssRequestPump.Processor) processor, blobId, references).FailedRefsAsync().ConfigureAwait(false);
              if (!(await configuredTaskAwaitable2).ToList<BlobReference>().Any<BlobReference>())
              {
                references = (IEnumerable<BlobReference>) null;
                content = new ArraySegment<byte>();
                return;
              }
              break;
            case BlobReferenceState.DeletingBlob:
              configuredTaskAwaitable1 = this.CleanupMetadataAsync(processor, blobProvider, metadata.BlobId).ConfigureAwait(false);
              IBlobMetadata blobMetadata = await configuredTaskAwaitable1;
              break;
            default:
              metadata = (IBlobMetadata) null;
              break;
          }
        }
        throw new TimeoutException(string.Format("Could not add and reference single block blob after {0} attempts.", (object) 20));
      }
    }

    private async Task PutBlobAndReferenceInternalAsync(
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider blobProvider,
      VssRequestPump.SecuredDomainProcessor processor,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference reference,
      bool useHttpClient,
      bool bypassExceptionOnRetryHashMatch)
    {
      if (reference == (BlobReference) null)
        throw new ArgumentNullException(nameof (reference));
      if (reference.IsInThePast(this.Clock))
        ;
      else
      {
        bool blobUploaded = false;
        int attempt = 5;
        do
        {
          --attempt;
          if (attempt < 0)
            throw new TimeoutException(string.Format("Could not upload blob after {0} attempts.", (object) 5));
          blob.Position = 0L;
          await blobId.GetBlobHasher().WalkBlocksAsync(blob, true, (Microsoft.VisualStudio.Services.BlobStore.Common.SingleBlockBlobCallbackAsync) (async (block, blockLength, blockHash) =>
          {
            await this.PutSingleBlockBlobAndReferenceInternalAsync(processor, blobProvider, blobId, block, blockLength, reference, useHttpClient).ConfigureAwait(false);
            blobUploaded = true;
          }), (MultiBlockBlobCallbackAsync) ((block, blockLength, blockHash, isFinalBlock) => this.PutBlobBlockInternalAsync((VssRequestPump.Processor) processor, blobProvider, blobId, block, blockLength, blockHash, useHttpClient, bypassExceptionOnRetryHashMatch)), (MultiBlockBlobSealCallbackAsync) (async blobIdWithBlocks =>
          {
            if (blobId != blobIdWithBlocks.BlobId)
              throw new ArgumentException(string.Format("Given hash ({0}) does not match computed hash ({1}).", (object) blobId, (object) blobIdWithBlocks), nameof (blobId));
            blobUploaded = (await this.TrySealBlobAsync(processor, blobProvider, blobIdWithBlocks, (IEnumerable<BlobReference>) new BlobReference[1]
            {
              reference
            }, (useHttpClient ? 1 : 0) != 0).ConfigureAwait(false)).ReferenceResults.Single<ReferenceResult>().Success;
          })).ConfigureAwait(false);
        }
        while (!blobUploaded);
      }
    }

    [DebuggerStepThrough]
    protected Task PumpOrInlineFromAsync(
      IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task> callback)
    {
      return requestContext.PumpOrInlineFromAsync(domainRequest, callback, this.providerRequiresVss);
    }

    [DebuggerStepThrough]
    protected Task<T> PumpOrInlineFromAsync<T>(
      IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> callback)
    {
      return requestContext.PumpOrInlineFromAsync<T>(domainRequest, callback, this.providerRequiresVss);
    }

    protected struct WriteBlockListResult
    {
      public readonly string FinalEtag;
      public readonly string FailureReason;

      public WriteBlockListResult(string finalEtag, string failureReason)
      {
        this.FinalEtag = finalEtag;
        this.FailureReason = failureReason;
      }

      public bool Success => this.FailureReason == null;
    }

    private struct SealBlobResult
    {
      public readonly PlatformBlobStore.WriteBlockListResult? BlockListResult;
      public readonly IEnumerable<ReferenceResult> ReferenceResults;

      public SealBlobResult(
        PlatformBlobStore.WriteBlockListResult? blockListResult,
        IEnumerable<ReferenceResult> referenceResults)
      {
        this.BlockListResult = blockListResult;
        this.ReferenceResults = referenceResults;
      }
    }

    [Serializable]
    public class BlobMetadataContentionException : Exception
    {
      public BlobMetadataContentionException(string message)
        : base(message)
      {
      }

      public BlobMetadataContentionException(string message, Exception ex)
        : base(message, ex)
      {
      }
    }

    public class BlobShardingAzureCloudTableClientFactory : ShardingAzureCloudTableClientFactory
    {
      public BlobShardingAzureCloudTableClientFactory(
        IEnumerable<StrongBoxConnectionString> accountConnectionStrings,
        Func<StrongBoxConnectionString, ITableClient> getTableClient,
        LocationMode? locationMode,
        string defaultTableName,
        string shardingStrategy,
        bool enableTracing)
        : base(accountConnectionStrings, getTableClient, locationMode, defaultTableName, shardingStrategy, enableTracing)
      {
      }

      protected override ulong GetKeyForShardHint(string shardHint) => BlobIdentifier.Deserialize(shardHint).GetKey();
    }
  }
}
