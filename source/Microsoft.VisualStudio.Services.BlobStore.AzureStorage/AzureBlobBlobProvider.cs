// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureBlobBlobProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureBlobBlobProvider : IBlobProvider, IDisposable
  {
    public const int MaxBlockSize = 4194304;
    public const int MaxOperationRetryAttempts = 5;
    public static readonly IRetryPolicy DefaultRetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromMilliseconds(10.0), 5);
    internal readonly IAzureBlobContainerFactory azureBlobContainerFactory;
    private readonly IRetryPolicy concurrentOperationFailureRetryPolicy;
    private readonly string blobNameSuffix;
    private readonly string partitionName;
    private readonly AsyncReaderWriterLock policyReaderWriterLock = new AsyncReaderWriterLock();
    private string currentExistingPolicyId;
    private readonly bool enableTracing;

    protected virtual IClock Clock => UtcClock.Instance;

    public AzureBlobBlobProvider(
      string partitionName,
      IAzureBlobContainerFactory blobContainerFactory,
      string blobNameSuffix,
      bool enableTracing)
      : this(partitionName, blobContainerFactory, AzureBlobBlobProvider.DefaultRetryPolicy, blobNameSuffix, enableTracing)
    {
    }

    public AzureBlobBlobProvider(
      string partitionName,
      IAzureBlobContainerFactory blobContainerFactory,
      IRetryPolicy retryPolicy,
      string blobNameSuffix,
      bool enableTracing)
    {
      this.enableTracing = enableTracing;
      this.azureBlobContainerFactory = blobContainerFactory != null ? blobContainerFactory : throw new ArgumentNullException(nameof (blobContainerFactory));
      this.concurrentOperationFailureRetryPolicy = retryPolicy;
      this.partitionName = partitionName;
      this.blobNameSuffix = blobNameSuffix;
    }

    public Task<string> GetBlobEtagAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier identifier)
    {
      ICloudBlockBlob blob = !(identifier == (BlobIdentifier) null) ? this.GetBlobReference(identifier) : throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      return this.FetchEtagAsync(processor, blob);
    }

    public Task<long?> GetBlobLengthAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier identifier)
    {
      ICloudBlockBlob blob = !(identifier == (BlobIdentifier) null) ? this.GetBlobReference(identifier) : throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      return this.FetchBlobSizeAsync(processor, blob);
    }

    public async Task<DisposableEtagValue<Stream>> GetBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      ICloudBlockBlob blob = !string.IsNullOrEmpty(blobId?.ValueString) ? this.GetBlobReference(blobId) : throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      try
      {
        return new DisposableEtagValue<Stream>(await blob.OpenReadNeedsRetryAsync(processor).ConfigureAwait(false), this.GetCachedEtag(blob));
      }
      catch (StorageException ex)
      {
        if (ex.HasHttpStatus(HttpStatusCode.NotFound))
          return new DisposableEtagValue<Stream>((Stream) null, (string) null);
        throw new ExpandedStorageException(ex, this.GetAccountName());
      }
    }

    public async Task PutBlobBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool useHttpClient)
    {
      if (string.IsNullOrEmpty(blobId?.ValueString))
        throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      DateTimeOffset operationStartTime = this.Clock.Now;
      ICloudBlockBlob blob = this.GetBlobReference(blobId);
      bool createContainerAndRetry = false;
      try
      {
        if (useHttpClient)
        {
          await blob.PutBlockByteArrayAsync(processor, blockName, blobBlock).ConfigureAwait(false);
        }
        else
        {
          using (MemoryStream ms = blobBlock.AsMemoryStream())
            await blob.PutBlockStreamAsync(processor, blockName, (Stream) ms).ConfigureAwait(false);
        }
      }
      catch (StorageException ex)
      {
        if (!ex.HasHttpStatus(HttpStatusCode.NotFound))
          throw new ExpandedStorageException(ex, this.GetAccountName());
        createContainerAndRetry = true;
      }
      if (!createContainerAndRetry)
      {
        blob = (ICloudBlockBlob) null;
      }
      else
      {
        await this.CreateContainerAsync(processor, operationStartTime);
        await blob.PutBlockByteArrayAsync(processor, blockName, blobBlock).ConfigureAwait(false);
        blob = (ICloudBlockBlob) null;
      }
    }

    public async Task<PreauthenticatedUri> GetDownloadUriAsync(
      VssRequestPump.Processor processor,
      BlobIdWithHeaders blobId,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      await this.EnsureContainerAndPolicyExistAsync(processor, policyId).ConfigureAwait(false);
      ICloudBlockBlob blobReference = this.GetBlobReference(blobId.BlobId);
      SharedAccessBlobHeaders blobHeaders = this.ToBlobHeaders(blobId);
      DateTimeOffset now = this.Clock.Now;
      DateTimeOffset blobExpiry = expiry.GetBlobExpiry(blobId.BlobId);
      string compatibleSasToken = AzureBlobBlobProvider.GetAzureFrontDoorCompatibleSASToken(blobReference, blobExpiry, expiry, policyId, blobHeaders, sasTracing);
      LocationMode? locationMode1 = this.azureBlobContainerFactory.LocationMode;
      LocationMode locationMode2 = LocationMode.SecondaryOnly;
      return new PreauthenticatedUri(new Uri((locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue ? (object) blobReference.StorageUri.SecondaryUri : (object) blobReference.StorageUri.PrimaryUri)?.ToString() + compatibleSasToken), EdgeType.NotEdge);
    }

    public async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      VssRequestPump.Processor processor,
      IEnumerable<BlobIdentifier> identifiers,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      await this.EnsureContainerAndPolicyExistAsync(processor, policyId).ConfigureAwait(false);
      ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri> downloadUrisAsync = new ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri>();
      foreach (BlobIdentifier identifier in identifiers)
      {
        ICloudBlockBlob blobReference = this.GetBlobReference(identifier);
        DateTimeOffset blobExpiry = expiry.GetBlobExpiry(identifier);
        string compatibleSasToken = AzureBlobBlobProvider.GetAzureFrontDoorCompatibleSASToken(blobReference, blobExpiry, expiry, policyId, (SharedAccessBlobHeaders) null, sasTracing);
        LocationMode? locationMode1 = this.azureBlobContainerFactory.LocationMode;
        LocationMode locationMode2 = LocationMode.SecondaryOnly;
        Uri uri = locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue ? blobReference.StorageUri.SecondaryUri : blobReference.StorageUri.PrimaryUri;
        downloadUrisAsync.TryAdd(identifier, new PreauthenticatedUri(new Uri(uri?.ToString() + compatibleSasToken), EdgeType.NotEdge));
      }
      return (IDictionary<BlobIdentifier, PreauthenticatedUri>) downloadUrisAsync;
    }

    internal static string GetAzureFrontDoorCompatibleSASToken(
      ICloudBlockBlob blob,
      DateTimeOffset blobExpiryTime,
      SASUriExpiry expiry,
      string policyId,
      SharedAccessBlobHeaders headers,
      params (string, Guid)[] tracingGuids)
    {
      if (headers != null)
      {
        string cacheControl = headers.CacheControl;
        if ((cacheControl != null ? (cacheControl.Contains("+") ? 1 : 0) : 0) == 0)
        {
          string contentDisposition = headers.ContentDisposition;
          if ((contentDisposition != null ? (contentDisposition.Contains("+") ? 1 : 0) : 0) == 0)
          {
            string contentEncoding = headers.ContentEncoding;
            if ((contentEncoding != null ? (contentEncoding.Contains("+") ? 1 : 0) : 0) == 0)
            {
              string contentLanguage = headers.ContentLanguage;
              if ((contentLanguage != null ? (contentLanguage.Contains("+") ? 1 : 0) : 0) == 0)
              {
                string contentType = headers.ContentType;
                if ((contentType != null ? (contentType.Contains("+") ? 1 : 0) : 0) == 0)
                  goto label_7;
              }
            }
          }
        }
        throw new ArgumentException(Resources.PlusInHeaders());
      }
label_7:
      Stopwatch stopwatch = Stopwatch.StartNew();
      TimeSpan zero = TimeSpan.Zero;
      while (!(stopwatch.Elapsed > TimeSpan.FromSeconds(5.0)))
      {
        SharedAccessBlobPolicy sasPolicy = expiry.GetSASPolicy(blobExpiryTime, zero);
        if (policyId == null)
          sasPolicy.Permissions = SharedAccessBlobPermissions.Read;
        if (tracingGuids != null && tracingGuids.Length != 0)
        {
          headers = headers ?? new SharedAccessBlobHeaders();
          headers.ApplyTracingGuids(tracingGuids);
        }
        string sharedAccessSignature = blob.GetSharedAccessSignature(sasPolicy, headers, policyId);
        zero += TimeSpan.FromSeconds(1.0);
        if (!sharedAccessSignature.Contains("%2B"))
          return sharedAccessSignature;
      }
      throw new TimeoutException(Resources.UriGenerationTimeout());
    }

    public async Task<EtagValue<IList<BlockInfo>>> GetBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      ICloudBlockBlob blob = !string.IsNullOrEmpty(blobId?.ValueString) ? this.GetBlobReference(blobId) : throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      try
      {
        return new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) (await blob.DownloadBlockListAsync(processor, BlockListingFilter.All).ConfigureAwait(false)).Select<ListBlockItem, BlockInfo>((System.Func<ListBlockItem, BlockInfo>) (blockList =>
        {
          BlockInfo blockListAsync = (BlockInfo) null;
          try
          {
            blockListAsync = new BlockInfo(blockList.Name)
            {
              Length = blockList.Length,
              Committed = blockList.Committed
            };
          }
          catch
          {
          }
          return blockListAsync;
        })).Where<BlockInfo>((System.Func<BlockInfo, bool>) (blockInfo => blockInfo != null)).ToList<BlockInfo>(), this.GetCachedEtag(blob));
      }
      catch (StorageException ex)
      {
        if (!ex.HasHttpStatus(HttpStatusCode.NotFound))
          throw new ExpandedStorageException(ex, this.GetAccountName());
        return new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) null, (string) null);
      }
      catch (AggregateException ex)
      {
        HttpStatusCode[] httpStatusCodeArray = new HttpStatusCode[1]
        {
          HttpStatusCode.NotFound
        };
        if (ExtensionMethods.ContainsStorageExceptionWithHttpStatus(ex, httpStatusCodeArray))
          return new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) null, (string) null);
        throw;
      }
    }

    public async Task<EtagValue<bool>> PutBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      IEnumerable<string> blockIds)
    {
      if (string.IsNullOrEmpty(blobId?.ValueString))
        throw new ArgumentException(Resources.BlobIdentifierMustBeDefined());
      DateTimeOffset operationStartTime = this.Clock.Now;
      ICloudBlockBlob blob = this.GetBlobReference(blobId);
      EtagValue<bool>? result = new EtagValue<bool>?();
      bool containerCreated = false;
      bool invalidBlockList = false;
      bool containerCreationNeeded = false;
      int attempt;
      for (attempt = 0; attempt < 100; ++attempt)
      {
        if (containerCreationNeeded)
        {
          await this.CreateContainerAsync(processor, operationStartTime).ConfigureAwait(false);
          containerCreated = true;
          containerCreationNeeded = false;
        }
        try
        {
          await blob.PutBlockListAsync(processor, blockIds, etagToMatch == null ? AccessCondition.GenerateIfNoneMatchCondition("*") : AccessCondition.GenerateIfMatchCondition(etagToMatch)).ConfigureAwait(false);
          result = new EtagValue<bool>?(new EtagValue<bool>(true, this.GetCachedEtag(blob)));
          break;
        }
        catch (Exception ex)
        {
          if (!containerCreated)
          {
            if (ex.ContainsStorageExceptionWithHttpStatus(HttpStatusCode.NotFound))
            {
              containerCreationNeeded = true;
              goto label_18;
            }
          }
          if (!ex.ContainsStorageExceptionWithHttpStatus(HttpStatusCode.PreconditionFailed))
          {
            if (!ex.ContainsStorageExceptionWithHttpStatus(HttpStatusCode.Conflict))
            {
              if (ex.ContainsStorageExceptionWithHttpStatus(HttpStatusCode.BadRequest))
              {
                invalidBlockList = true;
              }
              else
              {
                if (ex is StorageException e)
                  throw new ExpandedStorageException(e, this.GetAccountName());
                throw;
              }
            }
          }
        }
label_18:
        string etag = await this.FetchEtagAsync(processor, blob).ConfigureAwait(false);
        if (invalidBlockList || etag != etagToMatch)
        {
          result = new EtagValue<bool>?(new EtagValue<bool>(false, etag));
          break;
        }
      }
      if (attempt >= 100)
        throw new TimeoutException(string.Format("Could not put block list after {0} attempts.", (object) 100));
      if (!result.HasValue)
        throw new ConstraintException("Function must not exit without setting EtagValue<bool> result.");
      if (!invalidBlockList && etagToMatch == result.Value.Etag)
        throw new ConstraintException("Function must not exit with result.Etag == etagToMatch unless block list is invalid.");
      EtagValue<bool> etagValue = result.Value;
      blob = (ICloudBlockBlob) null;
      result = new EtagValue<bool>?();
      return etagValue;
    }

    public async Task<EtagValue<bool>> PutBlobByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      ArraySegment<byte> data,
      bool useHttpClient)
    {
      if (data.Array == null)
        throw new ArgumentNullException(nameof (data));
      if (blobId == (BlobIdentifier) null)
        throw new ArgumentNullException(nameof (blobId));
      DateTimeOffset operationStartTime = this.Clock.Now;
      ICloudBlockBlob blob = this.GetBlobReference(blobId);
      bool containerCreated = false;
      bool exceptionEncountered = false;
      bool containerCreationNeeded = false;
      string finalEtag = (string) null;
      try
      {
        for (int attempt = 0; attempt < 100; ++attempt)
        {
          if (containerCreationNeeded)
          {
            await this.CreateContainerAsync(processor, operationStartTime).ConfigureAwait(false);
            containerCreated = true;
            containerCreationNeeded = false;
          }
          try
          {
            await blob.UploadFromByteArrayAsync(processor, data, useHttpClient, etagToMatch == null ? AccessCondition.GenerateIfNoneMatchCondition("*") : AccessCondition.GenerateIfMatchCondition(etagToMatch)).ConfigureAwait(false);
            finalEtag = this.GetCachedEtag(blob);
            return new EtagValue<bool>(true, finalEtag);
          }
          catch (StorageException ex)
          {
            if (!containerCreated && ex.HasHttpStatus(HttpStatusCode.NotFound))
              containerCreationNeeded = true;
            else if (!ex.HasHttpStatus(HttpStatusCode.Conflict))
            {
              if (!ex.HasHttpStatus(HttpStatusCode.PreconditionFailed))
                throw new ExpandedStorageException(ex, this.GetAccountName());
            }
          }
          finalEtag = await this.FetchEtagAsync(processor, blob);
          if (etagToMatch != finalEtag)
            return new EtagValue<bool>(false, finalEtag);
        }
        throw new TimeoutException(string.Format("Could not put block list after {0} attempts.", (object) 100));
      }
      catch
      {
        exceptionEncountered = true;
        throw;
      }
      finally
      {
        int num;
        if (num < 0 && !exceptionEncountered && etagToMatch == finalEtag)
          throw new ConstraintException("Function must not exit with finalEtag == etagToMatch");
      }
    }

    public IConcurrentIterator<BlobIdentifier> GetBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      ICloudBlobContainer container = this.GetBlobContainerReference();
      return (IConcurrentIterator<BlobIdentifier>) new ConcurrentIterator<BlobIdentifier>(new int?(8), processor.CancellationToken, (Func<TryAddValueAsyncFunc<BlobIdentifier>, CancellationToken, Task>) (async (tryAddAsync, ct) =>
      {
        BlobContinuationToken continuation = (BlobContinuationToken) null;
        do
        {
          ICloudBlobContainer cloudBlobContainer = container;
          VssRequestPump.Processor processor1 = processor;
          string empty = string.Empty;
          BlobContinuationToken continuationToken = continuation;
          int? maxResults = new int?();
          BlobContinuationToken currentToken = continuationToken;
          IBlobResultSegment blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(processor1, empty, blobListingDetails: BlobListingDetails.Metadata, maxResults: maxResults, currentToken: currentToken).ConfigureAwait(false);
          continuation = blobResultSegment.ContinuationToken;
          foreach (IListBlobItem result in (IEnumerable<IListBlobItem>) blobResultSegment.Results)
          {
            if (result is ICloudBlockBlob cloudBlockBlob2 && cloudBlockBlob2.Name.EndsWith(this.blobNameSuffix, StringComparison.Ordinal))
            {
              if (!await tryAddAsync(BlobIdentifier.Deserialize(cloudBlockBlob2.Name.Substring(0, cloudBlockBlob2.Name.Length - this.blobNameSuffix.Length))))
              {
                continuation = (BlobContinuationToken) null;
                return;
              }
            }
          }
        }
        while (continuation != null);
        continuation = (BlobContinuationToken) null;
      }));
    }

    public IConcurrentIterator<IEnumerable<BasicBlobMetadata>> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.Processor processor,
      string prefix)
    {
      ICloudBlobContainer container = this.GetBlobContainerReference();
      return (IConcurrentIterator<IEnumerable<BasicBlobMetadata>>) new ConcurrentIterator<IEnumerable<BasicBlobMetadata>>(new int?(2), processor.CancellationToken, (Func<TryAddValueAsyncFunc<IEnumerable<BasicBlobMetadata>>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) => await this.AddBasicBlobMetadata(processor, container, valueAdderAsync, prefix).ConfigureAwait(false)));
    }

    public async Task<EtagValue<bool>> DeleteBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToDelete)
    {
      ICloudBlockBlob blob = this.GetBlobReference(blobId);
      try
      {
        if (await blob.DeleteIfExistsAsync(processor, accessCondition: AccessCondition.GenerateIfMatchCondition(etagToDelete)))
          return new EtagValue<bool>(true, (string) null);
      }
      catch (StorageException ex)
      {
        if (ex.HasHttpStatus(HttpStatusCode.NotFound))
          return new EtagValue<bool>(true, (string) null);
        if (!ex.HasHttpStatus(HttpStatusCode.PreconditionFailed))
          throw new ExpandedStorageException(ex, this.GetAccountName());
      }
      string etag = await this.FetchEtagAsync(processor, blob).ConfigureAwait(false);
      return new EtagValue<bool>(etag == null, etag);
    }

    public async Task<PreauthenticatedUri> GetContainerUri(
      VssRequestPump.Processor processor,
      string policyId)
    {
      await this.EnsureContainerAndPolicyExistAsync(processor, policyId).ConfigureAwait(false);
      SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
      {
        SharedAccessExpiryTime = new DateTimeOffset?(this.Clock.Now.Add(SASUriExpiryPolicy.DefaultBounds.MaxExpiry))
      };
      ICloudBlobContainer containerReference = this.GetBlobContainerReference();
      LocationMode? locationMode1 = this.azureBlobContainerFactory.LocationMode;
      LocationMode locationMode2 = LocationMode.SecondaryOnly;
      return new PreauthenticatedUri(new Uri((locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue ? (object) containerReference.StorageUri.SecondaryUri : (object) containerReference.StorageUri.PrimaryUri)?.ToString() + containerReference.GetSharedAccessSignature(processor, policy, policyId)), EdgeType.NotEdge);
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
    }

    protected virtual string GetCachedEtag(ICloudBlockBlob blob) => blob.Properties.ETag;

    private async Task EnsureContainerAndPolicyExistAsync(
      VssRequestPump.Processor processor,
      string policyId)
    {
      SharedAccessBlobPolicy policy;
      ICloudBlobContainer container;
      if (string.IsNullOrEmpty(policyId))
      {
        policy = (SharedAccessBlobPolicy) null;
        container = (ICloudBlobContainer) null;
      }
      else if (this.currentExistingPolicyId == policyId)
      {
        policy = (SharedAccessBlobPolicy) null;
        container = (ICloudBlobContainer) null;
      }
      else
      {
        policy = new SharedAccessBlobPolicy()
        {
          Permissions = SharedAccessBlobPermissions.Read
        };
        container = this.GetBlobContainerReference();
        using (await this.policyReaderWriterLock.WriterLockAsync())
        {
          if (this.currentExistingPolicyId == policyId)
          {
            policy = (SharedAccessBlobPolicy) null;
            container = (ICloudBlobContainer) null;
            return;
          }
          LocationMode? locationMode1 = this.azureBlobContainerFactory.LocationMode;
          LocationMode locationMode2 = LocationMode.SecondaryOnly;
          if (!(locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue))
          {
            int num = await container.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0;
          }
          try
          {
            BlobContainerPermissions permissions = await container.GetPermissionsAsync(processor).ConfigureAwait(false);
            if (!permissions.SharedAccessPolicies.ContainsKey(policyId))
            {
              permissions.SharedAccessPolicies[policyId] = policy;
              await container.SetPermissionsAsync(processor, permissions).ConfigureAwait(false);
            }
            this.currentExistingPolicyId = policyId;
          }
          catch (StorageException ex)
          {
            if (!(await container.GetPermissionsAsync(processor).ConfigureAwait(false)).SharedAccessPolicies.ContainsKey(policyId))
              throw new ExpandedStorageException(ex, container.AccountName);
            this.currentExistingPolicyId = policyId;
            policy = (SharedAccessBlobPolicy) null;
            container = (ICloudBlobContainer) null;
            return;
          }
        }
        policy = (SharedAccessBlobPolicy) null;
        container = (ICloudBlobContainer) null;
      }
    }

    private async Task<bool> FetchAttributesAsync(
      VssRequestPump.Processor processor,
      ICloudBlockBlob blob)
    {
      try
      {
        await blob.FetchAttributesAsync(processor).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        if (ex.ContainsStorageExceptionWithHttpStatus(HttpStatusCode.NotFound))
          return false;
        if (ex is StorageException e)
          throw new ExpandedStorageException(e);
        throw;
      }
      return true;
    }

    private async Task<string> FetchEtagAsync(
      VssRequestPump.Processor processor,
      ICloudBlockBlob blob)
    {
      return await this.FetchAttributesAsync(processor, blob).ConfigureAwait(false) ? this.GetCachedEtag(blob) : (string) null;
    }

    private async Task<long?> FetchBlobSizeAsync(
      VssRequestPump.Processor processor,
      ICloudBlockBlob blob)
    {
      return await this.FetchAttributesAsync(processor, blob).ConfigureAwait(false) ? new long?(blob.Properties.Length) : new long?();
    }

    private IRetryPolicy CreateRetryPolicy() => this.concurrentOperationFailureRetryPolicy.CreateInstance();

    private async Task CreateContainerAsync(
      VssRequestPump.Processor processor,
      DateTimeOffset operationStartTime)
    {
      bool operationComplete = false;
      IRetryPolicy retryPolicy = this.CreateRetryPolicy();
      ICloudBlobContainer blobContainer = this.GetBlobContainerReference();
      int retryCount = 0;
      while (!operationComplete)
      {
        try
        {
          int num = await blobContainer.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0;
          operationComplete = true;
        }
        catch (StorageException ex)
        {
          if (!ex.HasHttpStatus(HttpStatusCode.Conflict))
            throw new ExpandedStorageException(ex, blobContainer?.AccountName);
          this.ThrowIfRetryPolicyExhausted(retryPolicy, ++retryCount, (Exception) ex, operationStartTime);
        }
      }
      retryPolicy = (IRetryPolicy) null;
      blobContainer = (ICloudBlobContainer) null;
    }

    private void ThrowIfRetryPolicyExhausted(
      IRetryPolicy retryPolicy,
      int currentRetryCount,
      Exception lastException,
      DateTimeOffset operationStartTime)
    {
      TimeSpan retryInterval;
      if (!retryPolicy.ShouldRetry(currentRetryCount, 200, lastException, out retryInterval, (OperationContext) null))
        throw new ContentResourceContentionTimeoutException(Resources.ResourceContentionTimeout((object) this.Clock.Now.Subtract(operationStartTime)), lastException);
      Thread.Sleep(retryInterval);
    }

    private ICloudBlobContainer GetBlobContainerReference() => this.azureBlobContainerFactory.CreateContainerReference(this.partitionName, this.enableTracing);

    private string GetAccountName() => this.GetBlobContainerReference().AccountName;

    private ICloudBlockBlob GetBlobReference(BlobIdentifier identifier) => this.GetBlobContainerReference().GetBlockBlobReference(identifier.ValueString + this.blobNameSuffix);

    private SharedAccessBlobHeaders ToBlobHeaders(BlobIdWithHeaders blobId)
    {
      bool flag1 = !string.IsNullOrEmpty(blobId.FileName);
      bool flag2 = !string.IsNullOrEmpty(blobId.ContentType);
      if (flag1 && blobId.FileName.Contains<char>('"'))
        throw new ArgumentException("Double quotes aren't supported in the file name.");
      return new SharedAccessBlobHeaders()
      {
        ContentDisposition = flag1 ? "attachment; filename=\"" + blobId.FileName + "\"" : (string) null,
        ContentType = flag2 ? blobId.ContentType : (string) null
      };
    }

    private async Task AddBasicBlobMetadata(
      VssRequestPump.Processor processor,
      ICloudBlobContainer container,
      TryAddValueAsyncFunc<IEnumerable<BasicBlobMetadata>> valueAdderAsync,
      string prefixHex)
    {
      BlobContinuationToken continuationToken = (BlobContinuationToken) null;
      do
      {
        IBlobResultSegment blobResultSegment;
        try
        {
          blobResultSegment = await container.ListBlobsSegmentedAsync(processor, prefixHex, true, BlobListingDetails.Metadata, new int?(5000), continuationToken).ConfigureAwait(false);
        }
        catch (StorageException ex) when (ex.HasHttpStatus(HttpStatusCode.NotFound))
        {
          continuationToken = (BlobContinuationToken) null;
          return;
        }
        List<BasicBlobMetadata> valueToAdd = new List<BasicBlobMetadata>(blobResultSegment.Results.Count);
        foreach (IListBlobItem result in (IEnumerable<IListBlobItem>) blobResultSegment.Results)
        {
          if (result is CloudBlockBlobWrapper blockBlobWrapper)
            valueToAdd.Add(new BasicBlobMetadata(blockBlobWrapper.Name, blockBlobWrapper.Properties.Length, blockBlobWrapper.Properties.LastModified));
        }
        continuationToken = blobResultSegment.ContinuationToken;
        if (!await valueAdderAsync((IEnumerable<BasicBlobMetadata>) valueToAdd).ConfigureAwait(false))
          goto label_15;
      }
      while (continuationToken != null);
      goto label_18;
label_15:
      continuationToken = (BlobContinuationToken) null;
      return;
label_18:
      continuationToken = (BlobContinuationToken) null;
    }

    private int GetPrefixCountFromNumOfDigits(int numDigits) => (int) Math.Pow(16.0, (double) numDigits);

    private int GetPrefixNumOfDigitsFromMaxDegreeOfParallelism(int degreeOfParallelism) => (int) (Math.Ceiling(Math.Log((double) degreeOfParallelism, 16.0)) + 0.001);

    private string GetHexStringFromInteger(int num, int digits) => num.ToString("X" + digits.ToString());
  }
}
