// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlobContainer
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudBlobContainer
  {
    [DoesServiceRequest]
    public virtual void Create(BlobRequestOptions requestOptions = null, OperationContext operationContext = null) => this.Create(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, requestOptions, operationContext);

    [DoesServiceRequest]
    public virtual void Create(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      this.Create(accessType, (BlobContainerEncryptionScopeOptions) null, requestOptions, operationContext);
    }

    [DoesServiceRequest]
    public virtual void Create(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      if (accessType == BlobContainerPublicAccessType.Unknown)
        throw new ArgumentException("The argument is out of range. Value passed: {0}", nameof (accessType));
      BlobRequestOptions options = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateContainerImpl(options, accessType, encryptionScopeOptions), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(AsyncCallback callback, object state) => this.BeginCreate((BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreate(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreate(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateAsync(accessType, encryptionScopeOptions, options, operationContext, token)), callback, state);
    }

    public virtual void EndCreate(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateAsync() => this.CreateAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(CancellationToken cancellationToken) => this.CreateAsync(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateAsync(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateAsync(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task CreateAsync(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      if (accessType == BlobContainerPublicAccessType.Unknown)
        throw new ArgumentException("The argument is out of range. Value passed: {0}", nameof (accessType));
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateContainerImpl(options1, accessType, encryptionScopeOptions), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      return this.CreateIfNotExists(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, requestOptions, operationContext);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      return this.CreateIfNotExists(accessType, (BlobContainerEncryptionScopeOptions) null, requestOptions, operationContext);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      if (accessType == BlobContainerPublicAccessType.Unknown)
        throw new ArgumentException("The argument is out of range. Value passed: {0}", nameof (accessType));
      BlobRequestOptions requestOptions1 = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        this.Create(accessType, encryptionScopeOptions, requestOptions1, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerAlreadyExists))
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateIfNotExists((BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateIfNotExists(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateIfNotExists(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.CreateIfNotExistsAsync(accessType, encryptionScopeOptions, options, operationContext, token)), callback, state);
    }

    public virtual bool EndCreateIfNotExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync() => this.CreateIfNotExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(CancellationToken cancellationToken) => this.CreateIfNotExistsAsync((BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateIfNotExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, (BlobContainerEncryptionScopeOptions) null, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateIfNotExistsAsync(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateIfNotExistsAsync(accessType, (BlobContainerEncryptionScopeOptions) null, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> CreateIfNotExistsAsync(
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      if (accessType == BlobContainerPublicAccessType.Unknown)
        throw new ArgumentException("The argument is out of range. Value passed: {0}", nameof (accessType));
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        await this.CreateAsync(accessType, encryptionScopeOptions, options1, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerAlreadyExists))
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual void Delete(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteContainerImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(accessCondition, modifiedOptions, operationContext)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task DeleteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteContainerImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions blobRequestOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!this.Exists(true, blobRequestOptions, operationContext))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        this.Delete(accessCondition, blobRequestOptions, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!await this.ExistsAsync(true, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        await this.DeleteAsync(accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICloudBlob GetBlobReferenceFromServer(
      string blobName,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return this.ServiceClient.GetBlobReferenceFromServer(NavigationHelper.AppendPathToUri(this.StorageUri, blobName), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetBlobReferenceFromServer(
      string blobName,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetBlobReferenceFromServer(blobName, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetBlobReferenceFromServer(
      string blobName,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ICloudBlob>((Func<CancellationToken, Task<ICloudBlob>>) (token => this.GetBlobReferenceFromServerAsync(blobName, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual ICloudBlob EndGetBlobReferenceFromServer(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<ICloudBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName) => this.GetBlobReferenceFromServerAsync(blobName, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      string blobName,
      CancellationToken cancellationToken)
    {
      return this.GetBlobReferenceFromServerAsync(blobName, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      string blobName,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetBlobReferenceFromServerAsync(blobName, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      string blobName,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return this.ServiceClient.GetBlobReferenceFromServerAsync(NavigationHelper.AppendPathToUri(this.StorageUri, blobName), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<IListBlobItem> ListBlobs(
      string prefix = null,
      bool useFlatBlobListing = false,
      BlobListingDetails blobListingDetails = BlobListingDetails.None,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return CommonUtility.LazyEnumerable<IListBlobItem>((Func<IContinuationToken, ResultSegment<IListBlobItem>>) (token => this.ListBlobsSegmentedCore(prefix, useFlatBlobListing, blobListingDetails, new int?(), (BlobContinuationToken) token, modifiedOptions, operationContext)), long.MaxValue);
    }

    [DoesServiceRequest]
    public virtual BlobResultSegment ListBlobsSegmented(BlobContinuationToken currentToken) => this.ListBlobsSegmented((string) null, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null);

    [DoesServiceRequest]
    public virtual BlobResultSegment ListBlobsSegmented(
      string prefix,
      BlobContinuationToken currentToken)
    {
      return this.ListBlobsSegmented(prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null);
    }

    [DoesServiceRequest]
    public virtual BlobResultSegment ListBlobsSegmented(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      ResultSegment<IListBlobItem> resultSegment = this.ListBlobsSegmentedCore(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options1, operationContext);
      return new BlobResultSegment((IEnumerable<IListBlobItem>) resultSegment.Results, (BlobContinuationToken) resultSegment.ContinuationToken);
    }

    private ResultSegment<IListBlobItem> ListBlobsSegmentedCore(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ResultSegment<IListBlobItem>>(this.ListBlobsImpl(prefix, maxResults, useFlatBlobListing, blobListingDetails, options, currentToken), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListBlobsSegmented(
      BlobContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListBlobsSegmented((string) null, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListBlobsSegmented(
      string prefix,
      BlobContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListBlobsSegmented(prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListBlobsSegmented(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<BlobResultSegment>((Func<CancellationToken, Task<BlobResultSegment>>) (token => this.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, token)), callback, state);
    }

    public virtual BlobResultSegment EndListBlobsSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<BlobResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      BlobContinuationToken currentToken)
    {
      return this.ListBlobsSegmentedAsync(currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      BlobContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListBlobsSegmentedAsync((string) null, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      string prefix,
      BlobContinuationToken currentToken)
    {
      return this.ListBlobsSegmentedAsync(prefix, currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      string prefix,
      BlobContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListBlobsSegmentedAsync(prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<BlobResultSegment> ListBlobsSegmentedAsync(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      ResultSegment<IListBlobItem> resultSegment = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ResultSegment<IListBlobItem>>(this.ListBlobsImpl(prefix, maxResults, useFlatBlobListing, blobListingDetails, options1, currentToken), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      return new BlobResultSegment((IEnumerable<IListBlobItem>) resultSegment.Results, (BlobContinuationToken) resultSegment.ContinuationToken);
    }

    [DoesServiceRequest]
    public virtual void SetPermissions(
      BlobContainerPermissions permissions,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPermissionsImpl(permissions, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      BlobContainerPermissions permissions,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetPermissions(permissions, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      BlobContainerPermissions permissions,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPermissionsAsync(permissions, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(BlobContainerPermissions permissions) => this.SetPermissionsAsync(permissions, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      BlobContainerPermissions permissions,
      CancellationToken cancellationToken)
    {
      return this.SetPermissionsAsync(permissions, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      BlobContainerPermissions permissions,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPermissionsAsync(permissions, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      BlobContainerPermissions permissions,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPermissionsImpl(permissions, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual BlobContainerPermissions GetPermissions(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<BlobContainerPermissions>(this.GetPermissionsImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(AsyncCallback callback, object state) => this.BeginGetPermissions((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<BlobContainerPermissions>((Func<CancellationToken, Task<BlobContainerPermissions>>) (token => this.GetPermissionsAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual BlobContainerPermissions EndGetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<BlobContainerPermissions>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<BlobContainerPermissions> GetPermissionsAsync() => this.GetPermissionsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<BlobContainerPermissions> GetPermissionsAsync(
      CancellationToken cancellationToken)
    {
      return this.GetPermissionsAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<BlobContainerPermissions> GetPermissionsAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetPermissionsAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<BlobContainerPermissions> GetPermissionsAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<BlobContainerPermissions>(this.GetPermissionsImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool Exists(BlobRequestOptions requestOptions = null, OperationContext operationContext = null) => this.Exists(false, requestOptions, operationContext);

    private bool Exists(
      bool primaryOnly,
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      BlobRequestOptions options = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<bool>(this.ExistsImpl(options, primaryOnly), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(AsyncCallback callback, object state) => this.BeginExists((BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExists(false, options, operationContext, callback, state);
    }

    private ICancellableAsyncResult BeginExists(
      bool primaryOnly,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.ExistsAsync(primaryOnly, options, operationContext, token)), callback, state);
    }

    public virtual bool EndExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync() => this.ExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync(false, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ExistsAsync(false, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ExistsAsync(false, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      bool primaryOnly,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<bool>(this.ExistsImpl(options1, primaryOnly), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void FetchAttributes(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AsyncCallback callback,
      object state)
    {
      return this.BeginFetchAttributes((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FetchAttributesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndFetchAttributes(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync() => this.FetchAttributesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(CancellationToken cancellationToken) => this.FetchAttributesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.FetchAttributesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetMetadata(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state) => this.BeginSetMetadata((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetMetadataAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetMetadata(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync() => this.SetMetadataAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(CancellationToken cancellationToken) => this.SetMetadataAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetMetadataAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string AcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.AcquireLeaseImpl(leaseTime, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAcquireLease(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.AcquireLeaseAsync(leaseTime, proposedLeaseId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndAcquireLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(TimeSpan? leaseTime, string proposedLeaseId = null) => this.AcquireLeaseAsync(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken)
    {
      return this.AcquireLeaseAsync(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AcquireLeaseAsync(leaseTime, proposedLeaseId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.AcquireLeaseImpl(leaseTime, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void RenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.RenewLeaseImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginRenewLease(accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.RenewLeaseAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndRenewLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(AccessCondition accessCondition) => this.RenewLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.ReleaseLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.RenewLeaseAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.RenewLeaseImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string ChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.ChangeLeaseImpl(proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginChangeLease(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    public virtual ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.ChangeLeaseAsync(proposedLeaseId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndChangeLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.ChangeLeaseImpl(proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void ReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ReleaseLeaseImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginReleaseLease(accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ReleaseLeaseAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndReleaseLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(AccessCondition accessCondition) => this.ReleaseLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.ReleaseLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ReleaseLeaseAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ReleaseLeaseImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual TimeSpan BreakLease(
      TimeSpan? breakPeriod = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<TimeSpan>(this.BreakLeaseImpl(breakPeriod, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AsyncCallback callback,
      object state)
    {
      return this.BeginBreakLease(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<TimeSpan>((Func<CancellationToken, Task<TimeSpan>>) (token => this.BreakLeaseAsync(breakPeriod, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual TimeSpan EndBreakLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<TimeSpan>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(TimeSpan? breakPeriod) => this.BreakLeaseAsync(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      CancellationToken cancellationToken)
    {
      return this.BreakLeaseAsync(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.BreakLeaseAsync(breakPeriod, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<TimeSpan>(this.BreakLeaseImpl(breakPeriod, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    internal RESTCommand<string> AcquireLeaseImpl(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      int leaseDuration = -1;
      if (leaseTime.HasValue)
      {
        CommonUtility.AssertInBounds<TimeSpan>(nameof (leaseTime), leaseTime.Value, TimeSpan.FromSeconds(15.0), TimeSpan.FromSeconds(60.0));
        leaseDuration = (int) leaseTime.Value.TotalSeconds;
      }
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Acquire, proposedLeaseId, new int?(leaseDuration), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.Created, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return BlobHttpResponseParsers.GetLeaseId(resp);
      });
      return cmd1;
    }

    internal RESTCommand<NullType> RenewLeaseImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when renewing a lease.", nameof (accessCondition));
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Renew, (string) null, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<string> ChangeLeaseImpl(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      CommonUtility.AssertNotNull(nameof (proposedLeaseId), (object) proposedLeaseId);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when changing a lease.", nameof (accessCondition));
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Change, proposedLeaseId, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.OK, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return BlobHttpResponseParsers.GetLeaseId(resp);
      });
      return cmd1;
    }

    internal RESTCommand<NullType> ReleaseLeaseImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when releasing a lease.", nameof (accessCondition));
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Release, (string) null, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<TimeSpan> BreakLeaseImpl(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      int? breakSeconds = new int?();
      if (breakPeriod.HasValue)
      {
        CommonUtility.AssertInBounds<TimeSpan>(nameof (breakPeriod), breakPeriod.Value, TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(60.0));
        breakSeconds = new int?((int) breakPeriod.Value.TotalSeconds);
      }
      RESTCommand<TimeSpan> cmd1 = new RESTCommand<TimeSpan>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<TimeSpan>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<TimeSpan>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Break, (string) null, new int?(), breakSeconds, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<TimeSpan>, HttpResponseMessage, Exception, OperationContext, TimeSpan>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<TimeSpan>(HttpStatusCode.Accepted, resp, TimeSpan.Zero, (StorageCommandBase<TimeSpan>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return TimeSpan.FromSeconds((double) (BlobHttpResponseParsers.GetRemainingLeaseTime(resp) ?? throw new StorageException(cmd.CurrentResult, "Valid lease time expected but not received from the service.", (Exception) null)).Value);
      });
      return cmd1;
    }

    private RESTCommand<NullType> CreateContainerImpl(
      BlobRequestOptions options,
      BlobContainerPublicAccessType accessType,
      BlobContainerEncryptionScopeOptions encryptionScopeOptions)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ContainerHttpRequestMessageFactory.Create(uri, serverTimeout, cnt, ctx, accessType, encryptionScopeOptions, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        ContainerHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.Properties = ContainerHttpResponseParsers.GetProperties(resp);
        this.Metadata = ContainerHttpResponseParsers.GetMetadata(resp);
        this.Properties.PublicAccess = new BlobContainerPublicAccessType?(accessType);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> DeleteContainerImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.Delete(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<NullType> FetchAttributesImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.GetProperties(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.Properties = ContainerHttpResponseParsers.GetProperties(resp);
        this.Metadata = ContainerHttpResponseParsers.GetMetadata(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(BlobRequestOptions options, bool primaryOnly)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = primaryOnly ? CommandLocationMode.PrimaryOnly : CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.GetProperties(uri, serverTimeout, (AccessCondition) null, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
        this.Properties = ContainerHttpResponseParsers.GetProperties(resp);
        this.Metadata = ContainerHttpResponseParsers.GetMetadata(resp);
        return true;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetMetadataImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ContainerHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        ContainerHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPermissionsImpl(
      BlobContainerPermissions acl,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      if (acl.PublicAccess == BlobContainerPublicAccessType.Unknown)
        throw new ArgumentException("The argument is out of range. Value passed: {0}", "accessType");
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream(this.ServiceClient.BufferManager, 1024);
      BlobRequest.WriteSharedAccessIdentifiers(acl.SharedAccessPolicies, (Stream) memoryStream);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.SetAcl(uri, serverTimeout, acl.PublicAccess, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        this.Properties.PublicAccess = new BlobContainerPublicAccessType?(acl.PublicAccess);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<BlobContainerPermissions> GetPermissionsImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      BlobContainerPermissions containerAcl = (BlobContainerPermissions) null;
      RESTCommand<BlobContainerPermissions> cmd1 = new RESTCommand<BlobContainerPermissions>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<BlobContainerPermissions>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<BlobContainerPermissions>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.GetAcl(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<BlobContainerPermissions>, HttpResponseMessage, Exception, OperationContext, BlobContainerPermissions>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<BlobContainerPermissions>(HttpStatusCode.OK, resp, (BlobContainerPermissions) null, (StorageCommandBase<BlobContainerPermissions>) cmd, ex);
        containerAcl = new BlobContainerPermissions()
        {
          PublicAccess = ContainerHttpResponseParsers.GetAcl(resp)
        };
        return containerAcl;
      });
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<BlobContainerPermissions>, HttpResponseMessage, OperationContext, CancellationToken, Task<BlobContainerPermissions>>) (async (cmd, resp, ctx, ct) =>
      {
        this.UpdateETagAndLastModified(resp);
        await ContainerHttpResponseParsers.ReadSharedAccessIdentifiersAsync(cmd.ResponseStream, containerAcl, ct).ConfigureAwait(false);
        this.Properties.PublicAccess = new BlobContainerPublicAccessType?(containerAcl.PublicAccess);
        return containerAcl;
      });
      return cmd1;
    }

    private IListBlobItem SelectListBlobItem(IListBlobEntry protocolItem)
    {
      switch (protocolItem)
      {
        case ListBlobEntry listBlobEntry:
          BlobAttributes attributes = listBlobEntry.Attributes;
          attributes.StorageUri = NavigationHelper.AppendPathToUri(this.StorageUri, listBlobEntry.Name);
          if (attributes.Properties.BlobType == BlobType.BlockBlob)
            return (IListBlobItem) new CloudBlockBlob(attributes, this.ServiceClient);
          if (attributes.Properties.BlobType == BlobType.PageBlob)
            return (IListBlobItem) new CloudPageBlob(attributes, this.ServiceClient);
          if (attributes.Properties.BlobType == BlobType.AppendBlob)
            return (IListBlobItem) new CloudAppendBlob(attributes, this.ServiceClient);
          throw new InvalidOperationException("Invalid blob list item returned");
        case ListBlobPrefixEntry listBlobPrefixEntry:
          return (IListBlobItem) this.GetDirectoryReference(listBlobPrefixEntry.Name);
        default:
          throw new InvalidOperationException("Invalid blob list item returned");
      }
    }

    private RESTCommand<ResultSegment<IListBlobItem>> ListBlobsImpl(
      string prefix,
      int? maxResults,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      BlobRequestOptions options,
      BlobContinuationToken currentToken)
    {
      if (!useFlatBlobListing && (blobListingDetails & BlobListingDetails.Snapshots) == BlobListingDetails.Snapshots)
        throw new ArgumentException("Listing snapshots is only supported in flat mode (no delimiter). Consider setting the useFlatBlobListing parameter to true.", nameof (blobListingDetails));
      string defaultDelimiter = useFlatBlobListing ? (string) null : this.ServiceClient.DefaultDelimiter;
      BlobListingContext blobListingContext = new BlobListingContext(prefix, maxResults, defaultDelimiter, blobListingDetails);
      blobListingContext.Marker = currentToken?.NextMarker;
      BlobListingContext listingContext = blobListingContext;
      RESTCommand<ResultSegment<IListBlobItem>> cmd1 = new RESTCommand<ResultSegment<IListBlobItem>>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<ResultSegment<IListBlobItem>>(cmd1);
      cmd1.CommandLocationMode = CommonUtility.GetListingLocationMode((IContinuationToken) currentToken);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<ResultSegment<IListBlobItem>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.ListBlobs(uri, serverTimeout, listingContext, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<ResultSegment<IListBlobItem>>, HttpResponseMessage, Exception, OperationContext, ResultSegment<IListBlobItem>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ResultSegment<IListBlobItem>>(HttpStatusCode.OK, resp, (ResultSegment<IListBlobItem>) null, (StorageCommandBase<ResultSegment<IListBlobItem>>) cmd, ex));
      Func<IListBlobEntry, IListBlobItem> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ResultSegment<IListBlobItem>>, HttpResponseMessage, OperationContext, CancellationToken, Task<ResultSegment<IListBlobItem>>>) (async (cmd, resp, ctx, ct) =>
      {
        ListBlobsResponse listBlobsResponse = await ListBlobsResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        List<IListBlobItem> list = listBlobsResponse.Blobs.Select<IListBlobEntry, IListBlobItem>(func ?? (func = (Func<IListBlobEntry, IListBlobItem>) (item => this.SelectListBlobItem(item)))).ToList<IListBlobItem>();
        BlobContinuationToken continuationToken = (BlobContinuationToken) null;
        if (listBlobsResponse.NextMarker != null)
          continuationToken = new BlobContinuationToken()
          {
            NextMarker = listBlobsResponse.NextMarker,
            TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation)
          };
        return new ResultSegment<IListBlobItem>(list)
        {
          ContinuationToken = (IContinuationToken) continuationToken
        };
      });
      return cmd1;
    }

    private void UpdateETagAndLastModified(HttpResponseMessage response)
    {
      BlobContainerProperties properties = ContainerHttpResponseParsers.GetProperties(response);
      this.Properties.ETag = properties.ETag;
      this.Properties.LastModified = properties.LastModified;
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetAccountProperties(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetAccountProperties((BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetAccountProperties(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<AccountProperties>((Func<CancellationToken, Task<AccountProperties>>) (token => this.GetAccountPropertiesAsync(requestOptions, operationContext)), callback, state);
    }

    public virtual AccountProperties EndGetAccountProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<AccountProperties>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync() => this.GetAccountPropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetAccountPropertiesAsync((BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetAccountPropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual AccountProperties GetAccountProperties(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    private RESTCommand<AccountProperties> GetAccountPropertiesImpl(
      BlobRequestOptions requestOptions)
    {
      RESTCommand<AccountProperties> cmd1 = new RESTCommand<AccountProperties>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<AccountProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.GetAccountProperties(uri, builder, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, Exception, OperationContext, AccountProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<AccountProperties>(HttpStatusCode.OK, resp, (AccountProperties) null, (StorageCommandBase<AccountProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<AccountProperties>>) ((cmd, resp, ctx, ct) => Task.FromResult<AccountProperties>(ContainerHttpResponseParsers.ReadAccountProperties(resp)));
      requestOptions.ApplyToStorageCommand<AccountProperties>(cmd1);
      return cmd1;
    }

    public CloudBlobContainer(Uri containerAddress)
      : this(containerAddress, (StorageCredentials) null)
    {
    }

    public CloudBlobContainer(Uri containerAddress, StorageCredentials credentials)
      : this(new StorageUri(containerAddress), credentials)
    {
    }

    public CloudBlobContainer(StorageUri containerAddress, StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (containerAddress), (object) containerAddress);
      CommonUtility.AssertNotNull(nameof (containerAddress), (object) containerAddress.PrimaryUri);
      this.ParseQueryAndVerify(containerAddress, credentials);
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Properties = new BlobContainerProperties();
    }

    internal CloudBlobContainer(string containerName, CloudBlobClient serviceClient)
      : this(new BlobContainerProperties(), (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), containerName, serviceClient)
    {
    }

    internal CloudBlobContainer(
      BlobContainerProperties properties,
      IDictionary<string, string> metadata,
      string containerName,
      CloudBlobClient serviceClient)
    {
      this.StorageUri = NavigationHelper.AppendPathToUri(serviceClient.StorageUri, containerName);
      this.ServiceClient = serviceClient;
      this.Name = containerName;
      this.Metadata = metadata;
      this.Properties = properties;
    }

    public CloudBlobClient ServiceClient { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public string Name { get; private set; }

    public IDictionary<string, string> Metadata { get; private set; }

    public BlobContainerProperties Properties { get; private set; }

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      this.StorageUri = NavigationHelper.ParseBlobQueryAndVerify(address, out parsedCredentials, out DateTimeOffset? _);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      this.ServiceClient = new CloudBlobClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      this.Name = NavigationHelper.GetContainerNameFromContainerAddress(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }

    private string GetSharedAccessCanonicalName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) "blob", (object) (this.ServiceClient.Credentials.AccountName ?? NavigationHelper.GetAccountNameFromUri(this.ServiceClient.BaseUri, new bool?())), (object) this.Name);

    public string GetSharedAccessSignature(SharedAccessBlobPolicy policy) => this.GetSharedAccessSignature(policy, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, groupPolicyIdentifier, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      string groupPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string accessCanonicalName = this.GetSharedAccessCanonicalName();
      StorageAccountKey key = this.ServiceClient.Credentials.Key;
      string hash = BlobSharedAccessSignatureHelper.GetHash(policy, (SharedAccessBlobHeaders) null, groupPolicyIdentifier, accessCanonicalName, "2019-07-07", protocols, ipAddressOrRange, key.KeyValue, "c");
      string keyName = key.KeyName;
      return BlobSharedAccessSignatureHelper.GetSignature(policy, (SharedAccessBlobHeaders) null, groupPolicyIdentifier, "c", hash, keyName, "2019-07-07", protocols, ipAddressOrRange).ToString();
    }

    public string GetUserDelegationSharedAccessSignature(
      UserDelegationKey delegationKey,
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers = null,
      SharedAccessProtocol? protocols = null,
      IPAddressOrRange ipAddressOrRange = null)
    {
      string accessCanonicalName = this.GetSharedAccessCanonicalName();
      string hash = BlobSharedAccessSignatureHelper.GetHash(policy, headers, accessCanonicalName, "2019-07-07", "c", new DateTimeOffset?(), protocols, ipAddressOrRange, delegationKey);
      return BlobSharedAccessSignatureHelper.GetSignature(policy, headers, (string) null, "c", hash, (string) null, "2019-07-07", protocols, ipAddressOrRange, delegationKey).ToString();
    }

    public virtual CloudPageBlob GetPageBlobReference(string blobName) => this.GetPageBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudPageBlob GetPageBlobReference(string blobName, DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudPageBlob(blobName, snapshotTime, this);
    }

    public virtual CloudBlockBlob GetBlockBlobReference(string blobName) => this.GetBlockBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudBlockBlob GetBlockBlobReference(
      string blobName,
      DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudBlockBlob(blobName, snapshotTime, this);
    }

    public virtual CloudAppendBlob GetAppendBlobReference(string blobName) => this.GetAppendBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudAppendBlob GetAppendBlobReference(
      string blobName,
      DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudAppendBlob(blobName, snapshotTime, this);
    }

    public virtual CloudBlob GetBlobReference(string blobName) => this.GetBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudBlob GetBlobReference(string blobName, DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudBlob(blobName, snapshotTime, this);
    }

    public virtual CloudBlobDirectory GetDirectoryReference(string relativeAddress)
    {
      CommonUtility.AssertNotNull(nameof (relativeAddress), (object) relativeAddress);
      if (!string.IsNullOrEmpty(relativeAddress) && !relativeAddress.EndsWith(this.ServiceClient.DefaultDelimiter, StringComparison.Ordinal))
        relativeAddress += this.ServiceClient.DefaultDelimiter;
      return new CloudBlobDirectory(NavigationHelper.AppendPathToUri(this.StorageUri, relativeAddress), relativeAddress, this);
    }
  }
}
