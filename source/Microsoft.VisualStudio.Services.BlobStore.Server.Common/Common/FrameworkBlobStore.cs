// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.FrameworkBlobStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class FrameworkBlobStore : ArtifactsServiceBase, IBlobStore, IVssFrameworkService
  {
    internal const ushort MaxAttempts = 3;
    private const FrameworkBlobStoreVersion DefaultVersion = FrameworkBlobStoreVersion.V2;
    private FrameworkBlobStoreVersion clientVersion;
    private static readonly HttpClient httpClient = new HttpClient();

    protected override string ProductTraceArea => "BlobStore";

    public static Uri GetBlobStoreServiceUri(IVssRequestContext requestContext)
    {
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.BlobStore, AccessMappingConstants.ClientAccessMappingMoniker);
      return locationServiceUrl != null ? new Uri(locationServiceUrl) : (Uri) null;
    }

    public virtual IBlobStoreHttpClient GetHttpClient(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      if (domainId == (IDomainId) null)
        throw new ArgumentNullException("DomainId cannot be null");
      IBlobStoreHttpClient httpClient;
      if (!domainId.Equals(WellKnownDomainIds.DefaultDomainId))
      {
        DomainBlobstoreHttpClient client = requestContext.Elevate().GetClient<DomainBlobstoreHttpClient>();
        client.ParallelHttpCallsSupported = false;
        httpClient = (IBlobStoreHttpClient) new DomainBlobHttpClientWrapper(domainId, (IDomainBlobStoreHttpClient) client);
      }
      else
      {
        BlobStore2HttpClient client = requestContext.Elevate().GetClient<BlobStore2HttpClient>();
        client.ParallelHttpCallsSupported = false;
        httpClient = (IBlobStoreHttpClient) client;
      }
      return httpClient;
    }

    public Task<Stream> GetBlobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId)
    {
      return this.GetHttpClient(requestContext, domainId).GetBlobAsync(blobId, requestContext.CancellationToken);
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      Stopwatch.StartNew();
      IBlobStoreHttpClient blobClient = this.GetHttpClient(requestContext, domainId);
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> result = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) null;
      TimeSpan requestTimeout = requestContext.RequestTimeout;
      requestContext = (IVssRequestContext) null;
      ushort attempt = 0;
      do
      {
        try
        {
          result = await blobClient.TryReferenceAsync(referencesGroupedByBlobIds, new CancellationToken()).ConfigureAwait(false);
        }
        catch (TimeoutException ex)
        {
          if (++attempt == (ushort) 3)
            throw;
        }
      }
      while (result == null && attempt <= (ushort) 3);
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionary = result;
      blobClient = (IBlobStoreHttpClient) null;
      result = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) null;
      return dictionary;
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      Stopwatch.StartNew();
      IBlobStoreHttpClient blobClient = this.GetHttpClient(requestContext, domainId);
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> result = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) null;
      requestContext = (IVssRequestContext) null;
      ushort attempt = 0;
      do
      {
        try
        {
          result = await blobClient.TryReferenceWithBlocksAsync(referencesGroupedByBlobIds, new CancellationToken()).ConfigureAwait(false);
        }
        catch (TimeoutException ex)
        {
          if (++attempt == (ushort) 3)
            throw;
        }
      }
      while (result == null && attempt <= (ushort) 3);
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionary = result;
      blobClient = (IBlobStoreHttpClient) null;
      result = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) null;
      return dictionary;
    }

    public Task RemoveReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      return this.GetHttpClient(requestContext, domainId).RemoveReferencesAsync(referencesGroupedByBlobIds);
    }

    public Task<PreauthenticatedUri> GetDownloadUriAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdWithHeaders blobId)
    {
      return this.GetHttpClient(requestContext, domainId).GetDownloadUriAsync(blobId, requestContext.CancellationToken);
    }

    public Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime = null)
    {
      return this.GetHttpClient(requestContext, domainId).GetDownloadUrisAsync(blobIds, edgeCache, requestContext.CancellationToken, expiryTime);
    }

    public Task PutBlobBlockAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blobBlockHash)
    {
      return this.GetHttpClient(requestContext, domainId).PutBlobBlockAsync(blobId, blockBuffer, blockLength, requestContext.CancellationToken);
    }

    public Task PutSingleBlockBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      BlobReference reference)
    {
      return this.GetHttpClient(requestContext, domainId).PutSingleBlockBlobAndReferenceAsync(blobId, blockBuffer, blockLength, reference, requestContext.CancellationToken);
    }

    public async Task PutBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference reference)
    {
      using (SemaphoreSlim semaphore = new SemaphoreSlim(4, 4))
        await requestContext.PumpFromAsync((Func<VssRequestPump.Processor, Task>) (processor => Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.WalkBlocksAsync(blob, semaphore, true, (Microsoft.VisualStudio.Services.BlobStore.Common.SingleBlockBlobCallbackAsync) ((blockBuffer, blockLength, blobIdWithBlocks) => processor.ExecuteAsyncWorkAsync((Func<IVssRequestContext, Task>) (rq =>
        {
          if (blobId != blobIdWithBlocks.BlobId)
            throw new ArgumentException(string.Format("Given hash ({0}) does not match computed hash ({1}).", (object) blobId, (object) blobIdWithBlocks), nameof (blobId));
          return this.GetHttpClient(rq, domainId).PutSingleBlockBlobAndReferenceAsync(blobId, blockBuffer, blockLength, reference, rq.CancellationToken);
        }))), (MultiBlockBlobCallbackAsync) ((blockBuffer, blockLength, blockHash, isFinalBlock) => processor.ExecuteWorkAsync<Task>((Func<IVssRequestContext, Task>) (rq => rq.Fork((Func<IVssRequestContext, Task>) (forkedContext => this.GetHttpClient(forkedContext, domainId).PutBlobBlockAsync(blobId, blockBuffer, blockLength, forkedContext.CancellationToken)), nameof (PutBlobAndReferenceAsync)))).Unwrap()), (MultiBlockBlobSealCallbackAsync) (blobIdWithBlocks => processor.ExecuteAsyncWorkAsync((Func<IVssRequestContext, Task>) (async rq =>
        {
          if (blobId != blobIdWithBlocks.BlobId)
            throw new ArgumentException(string.Format("Given hash ({0}) does not match computed hash ({1}).", (object) blobId, (object) blobIdWithBlocks), nameof (blobId));
          if (!await this.GetHttpClient(rq, domainId).TryReferenceWithBlocksAsync(blobIdWithBlocks, reference, rq.CancellationToken))
            throw new InvalidOperationException(string.Format("TryReferenceWithBlocksAsync failed for ({0})", (object) blobId));
        }))))));
    }

    public Task<IDictionary<BlobIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil)
    {
      throw new NotImplementedException();
    }

    public Task<IDictionary<ulong, PreauthenticatedUri>> GetSasUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      throw new NotImplementedException();
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      string str = systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStoreShared/FrameworkBlobStoreVersion", true, string.Empty);
      if (!Enum.IsDefined(typeof (FrameworkBlobStoreVersion), (object) str))
        return;
      Enum.TryParse<FrameworkBlobStoreVersion>(str, out this.clientVersion);
    }

    public virtual async Task<HttpResponseMessage> CheckBlobExistsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri blobUri)
    {
      return await FrameworkBlobStore.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, blobUri), HttpCompletionOption.ResponseHeadersRead, requestContext.CancellationToken).ConfigureAwait(false);
    }

    [DefaultServiceImplementation(typeof (FrameworkBlobStore.BlockUploadTaskService))]
    public interface IBlockUploadTaskService : IVssTaskService, IVssFrameworkService
    {
    }

    private class BlockUploadTaskService : 
      VssTaskService,
      FrameworkBlobStore.IBlockUploadTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }
  }
}
