// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlobClient
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudBlobClient
  {
    internal HttpClient HttpClient;
    private string defaultDelimiter;
    private AuthenticationScheme authenticationScheme;

    public AuthenticationScheme AuthenticationScheme
    {
      get => this.authenticationScheme;
      set => this.authenticationScheme = value;
    }

    [DoesServiceRequest]
    public virtual IEnumerable<CloudBlobContainer> ListContainers(
      string prefix = null,
      ContainerListingDetails detailsIncluded = ContainerListingDetails.None,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return CommonUtility.LazyEnumerable<CloudBlobContainer>((Func<IContinuationToken, ResultSegment<CloudBlobContainer>>) (token => this.ListContainersSegmentedCore(prefix, detailsIncluded, new int?(), (BlobContinuationToken) token, modifiedOptions, operationContext)), long.MaxValue);
    }

    [DoesServiceRequest]
    public virtual ContainerResultSegment ListContainersSegmented(BlobContinuationToken currentToken) => this.ListContainersSegmented((string) null, ContainerListingDetails.None, new int?(), currentToken);

    [DoesServiceRequest]
    public virtual ContainerResultSegment ListContainersSegmented(
      string prefix,
      BlobContinuationToken currentToken)
    {
      return this.ListContainersSegmented(prefix, ContainerListingDetails.None, new int?(), currentToken);
    }

    [DoesServiceRequest]
    public virtual ContainerResultSegment ListContainersSegmented(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      ResultSegment<CloudBlobContainer> resultSegment = this.ListContainersSegmentedCore(prefix, detailsIncluded, maxResults, currentToken, options1, operationContext);
      return new ContainerResultSegment((IEnumerable<CloudBlobContainer>) resultSegment.Results, (BlobContinuationToken) resultSegment.ContinuationToken);
    }

    private ResultSegment<CloudBlobContainer> ListContainersSegmentedCore(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken continuationToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ResultSegment<CloudBlobContainer>>(this.ListContainersImpl(prefix, detailsIncluded, continuationToken, maxResults, options), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListContainersSegmented(
      BlobContinuationToken continuationToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListContainersSegmented((string) null, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListContainersSegmented(
      string prefix,
      BlobContinuationToken continuationToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListContainersSegmented(prefix, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListContainersSegmented(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken continuationToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ContainerResultSegment>((Func<CancellationToken, Task<ContainerResultSegment>>) (token => this.ListContainersSegmentedAsync(prefix, detailsIncluded, maxResults, continuationToken, options, operationContext, token)), callback, state);
    }

    public virtual ContainerResultSegment EndListContainersSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<ContainerResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<ContainerResultSegment> ListContainersSegmentedAsync(
      BlobContinuationToken continuationToken)
    {
      return this.ListContainersSegmentedAsync((string) null, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ContainerResultSegment> ListContainersSegmentedAsync(
      BlobContinuationToken continuationToken,
      CancellationToken cancellationToken)
    {
      return this.ListContainersSegmentedAsync((string) null, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ContainerResultSegment> ListContainersSegmentedAsync(
      string prefix,
      BlobContinuationToken continuationToken)
    {
      return this.ListContainersSegmentedAsync(prefix, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ContainerResultSegment> ListContainersSegmentedAsync(
      string prefix,
      BlobContinuationToken continuationToken,
      CancellationToken cancellationToken)
    {
      return this.ListContainersSegmentedAsync(prefix, ContainerListingDetails.None, new int?(), continuationToken, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ContainerResultSegment> ListContainersSegmentedAsync(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken continuationToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListContainersSegmentedAsync(prefix, detailsIncluded, maxResults, continuationToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<ContainerResultSegment> ListContainersSegmentedAsync(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken continuationToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudBlobClient serviceClient = this;
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, serviceClient);
      ResultSegment<CloudBlobContainer> resultSegment = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ResultSegment<CloudBlobContainer>>(serviceClient.ListContainersImpl(prefix, detailsIncluded, continuationToken, maxResults, options1), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      return new ContainerResultSegment((IEnumerable<CloudBlobContainer>) resultSegment.Results, (BlobContinuationToken) resultSegment.ContinuationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<IListBlobItem> ListBlobs(
      string prefix,
      bool useFlatBlobListing = false,
      BlobListingDetails blobListingDetails = BlobListingDetails.None,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      string containerName;
      string listingPrefix;
      CloudBlobClient.ParseUserPrefix(prefix, out containerName, out listingPrefix);
      return this.GetContainerReference(containerName).ListBlobs(listingPrefix, useFlatBlobListing, blobListingDetails, options, operationContext);
    }

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
      string containerName;
      string listingPrefix;
      CloudBlobClient.ParseUserPrefix(prefix, out containerName, out listingPrefix);
      return this.GetContainerReference(containerName).ListBlobsSegmented(listingPrefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext);
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
      string prefix,
      BlobContinuationToken currentToken)
    {
      return this.ListBlobsSegmentedAsync(prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
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
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      string containerName;
      string listingPrefix;
      CloudBlobClient.ParseUserPrefix(prefix, out containerName, out listingPrefix);
      return this.GetContainerReference(containerName).ListBlobsSegmentedAsync(listingPrefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ICloudBlob GetBlobReferenceFromServer(
      Uri blobUri,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (blobUri), (object) blobUri);
      return this.GetBlobReferenceFromServer(new StorageUri(blobUri), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICloudBlob GetBlobReferenceFromServer(
      StorageUri blobUri,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (blobUri), (object) blobUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ICloudBlob>(this.GetBlobReferenceImpl(blobUri, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetBlobReferenceFromServer(
      Uri blobUri,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetBlobReferenceFromServer(blobUri, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetBlobReferenceFromServer(
      Uri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (blobUri), (object) blobUri);
      return this.BeginGetBlobReferenceFromServer(new StorageUri(blobUri), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetBlobReferenceFromServer(
      StorageUri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ICloudBlob>((Func<CancellationToken, Task<ICloudBlob>>) (token => this.GetBlobReferenceFromServerAsync(blobUri, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual ICloudBlob EndGetBlobReferenceFromServer(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<ICloudBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(Uri blobUri) => this.GetBlobReferenceFromServerAsync(blobUri, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      Uri blobUri,
      CancellationToken cancellationToken)
    {
      return this.GetBlobReferenceFromServerAsync(new StorageUri(blobUri), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      Uri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetBlobReferenceFromServerAsync(new StorageUri(blobUri), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      Uri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.GetBlobReferenceFromServerAsync(new StorageUri(blobUri), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      StorageUri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetBlobReferenceFromServerAsync(blobUri, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ICloudBlob> GetBlobReferenceFromServerAsync(
      StorageUri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (blobUri), (object) blobUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ICloudBlob>(this.GetBlobReferenceImpl(blobUri, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual UserDelegationKey GetUserDelegationKey(
      DateTimeOffset keyStart,
      DateTimeOffset keyEnd,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<UserDelegationKey>(this.GetUserDelegationKeyImpl(keyStart, keyEnd, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetUserDelegationKey(
      DateTimeOffset keyStart,
      DateTimeOffset keyEnd,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetUserDelegationKey(keyStart, keyEnd, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetUserDelegationKey(
      DateTimeOffset keyStart,
      DateTimeOffset keyEnd,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return CancellableAsyncResultTaskWrapper.Create<UserDelegationKey>((Func<CancellationToken, Task<UserDelegationKey>>) (token => this.GetUserDelegationKeyAsync(keyStart, keyEnd, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual UserDelegationKey EndGetUserDelegationKey(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<UserDelegationKey>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<UserDelegationKey> GetUserDelegationKeyAsync(
      DateTimeOffset keyStart,
      DateTimeOffset keyEnd)
    {
      return this.GetUserDelegationKeyAsync(keyStart, keyEnd, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<UserDelegationKey> GetUserDelegationKeyAsync(
      DateTimeOffset keyStart,
      DateTimeOffset keyEnd,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<UserDelegationKey>(this.GetUserDelegationKeyImpl(keyStart, keyEnd, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<ResultSegment<CloudBlobContainer>> ListContainersImpl(
      string prefix,
      ContainerListingDetails detailsIncluded,
      BlobContinuationToken currentToken,
      int? maxResults,
      BlobRequestOptions options)
    {
      ListingContext listingContext = new ListingContext(prefix, maxResults)
      {
        Marker = currentToken?.NextMarker
      };
      RESTCommand<ResultSegment<CloudBlobContainer>> cmd1 = new RESTCommand<ResultSegment<CloudBlobContainer>>(this.Credentials, this.StorageUri, this.HttpClient);
      options.ApplyToStorageCommand<ResultSegment<CloudBlobContainer>>(cmd1);
      cmd1.CommandLocationMode = CommonUtility.GetListingLocationMode((IContinuationToken) currentToken);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<ResultSegment<CloudBlobContainer>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ContainerHttpRequestMessageFactory.List(uri, serverTimeout, listingContext, detailsIncluded, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<ResultSegment<CloudBlobContainer>>, HttpResponseMessage, Exception, OperationContext, ResultSegment<CloudBlobContainer>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ResultSegment<CloudBlobContainer>>(HttpStatusCode.OK, resp, (ResultSegment<CloudBlobContainer>) null, (StorageCommandBase<ResultSegment<CloudBlobContainer>>) cmd, ex));
      Func<BlobContainerEntry, CloudBlobContainer> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ResultSegment<CloudBlobContainer>>, HttpResponseMessage, OperationContext, CancellationToken, Task<ResultSegment<CloudBlobContainer>>>) (async (cmd, resp, ctx, ct) =>
      {
        ListContainersResponse containersResponse = await ListContainersResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        List<CloudBlobContainer> list = containersResponse.Containers.Select<BlobContainerEntry, CloudBlobContainer>(func ?? (func = (Func<BlobContainerEntry, CloudBlobContainer>) (item => new CloudBlobContainer(item.Properties, item.Metadata, item.Name, this)))).ToList<CloudBlobContainer>();
        BlobContinuationToken continuationToken = (BlobContinuationToken) null;
        if (containersResponse.NextMarker != null)
          continuationToken = new BlobContinuationToken()
          {
            NextMarker = containersResponse.NextMarker,
            TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation)
          };
        return new ResultSegment<CloudBlobContainer>(list)
        {
          ContinuationToken = (IContinuationToken) continuationToken
        };
      });
      return cmd1;
    }

    private RESTCommand<ICloudBlob> GetBlobReferenceImpl(
      StorageUri blobUri,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      StorageCredentials parsedCredentials;
      DateTimeOffset? parsedSnapshot;
      blobUri = NavigationHelper.ParseBlobQueryAndVerify(blobUri, out parsedCredentials, out parsedSnapshot);
      CloudBlobClient client = parsedCredentials != null ? new CloudBlobClient(this.StorageUri, parsedCredentials) : this;
      RESTCommand<ICloudBlob> cmd1 = new RESTCommand<ICloudBlob>(client.Credentials, blobUri, client.HttpClient);
      options.ApplyToStorageCommand<ICloudBlob>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ICloudBlob>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetProperties(uri, serverTimeout, parsedSnapshot, accessCondition, cnt, ctx, client.GetCanonicalizer(), client.Credentials, options));
      cmd1.PreProcessResponse = (Func<RESTCommand<ICloudBlob>, HttpResponseMessage, Exception, OperationContext, ICloudBlob>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<ICloudBlob>(HttpStatusCode.OK, resp, (ICloudBlob) null, (StorageCommandBase<ICloudBlob>) cmd, ex);
        BlobAttributes blobAttributes = new BlobAttributes()
        {
          StorageUri = blobUri,
          SnapshotTime = parsedSnapshot
        };
        CloudBlob.UpdateAfterFetchAttributes(blobAttributes, resp);
        switch (blobAttributes.Properties.BlobType)
        {
          case BlobType.PageBlob:
            return (ICloudBlob) new CloudPageBlob(blobAttributes, client);
          case BlobType.BlockBlob:
            return (ICloudBlob) new CloudBlockBlob(blobAttributes, client);
          case BlobType.AppendBlob:
            return (ICloudBlob) new CloudAppendBlob(blobAttributes, client);
          default:
            throw new InvalidOperationException();
        }
      });
      return cmd1;
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
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<AccountProperties>((Func<CancellationToken, Task<AccountProperties>>) (token => this.GetAccountPropertiesAsync(requestOptions, operationContext)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetServiceProperties((BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<ServiceProperties>((Func<CancellationToken, Task<ServiceProperties>>) (token => this.GetServicePropertiesAsync(requestOptions, operationContext)), callback, state);
    }

    public virtual AccountProperties EndGetAccountProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<AccountProperties>) asyncResult).GetAwaiter().GetResult();
    }

    public virtual ServiceProperties EndGetServiceProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<ServiceProperties>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync() => this.GetAccountPropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetAccountPropertiesAsync((BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
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
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual AccountProperties GetAccountProperties(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync() => this.GetServicePropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetServicePropertiesAsync((BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServicePropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ServiceProperties GetServiceProperties(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      ServiceProperties properties,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetServiceProperties(properties, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      ServiceProperties properties,
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetServicePropertiesAsync(properties, requestOptions, operationContext)), callback, state);
    }

    public virtual void EndSetServiceProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(ServiceProperties properties) => this.SetServicePropertiesAsync(properties, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      CancellationToken cancellationToken)
    {
      return this.SetServicePropertiesAsync(properties, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.SetServicePropertiesAsync(properties, requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetServicePropertiesImpl(properties, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetServiceProperties(
      ServiceProperties properties,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetServicePropertiesImpl(properties, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceStats(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetServiceStats((BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceStats(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<ServiceStats>((Func<CancellationToken, Task<ServiceStats>>) (token => this.GetServiceStatsAsync(requestOptions, operationContext)), callback, state);
    }

    public virtual ServiceStats EndGetServiceStats(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<ServiceStats>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync() => this.GetServiceStatsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(CancellationToken cancellationToken) => this.GetServiceStatsAsync((BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServiceStatsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ServiceStats>(this.GetServiceStatsImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ServiceStats GetServiceStats(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ServiceStats>(this.GetServiceStatsImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    private RESTCommand<AccountProperties> GetAccountPropertiesImpl(
      BlobRequestOptions requestOptions)
    {
      RESTCommand<AccountProperties> cmd1 = new RESTCommand<AccountProperties>(this.Credentials, this.StorageUri, this.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<AccountProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetAccountProperties(uri, builder, serverTimeout, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, Exception, OperationContext, AccountProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<AccountProperties>(HttpStatusCode.OK, resp, (AccountProperties) null, (StorageCommandBase<AccountProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<AccountProperties>>) ((cmd, resp, ctx, ct) => Task.FromResult<AccountProperties>(HttpResponseParsers.ReadAccountProperties(resp)));
      requestOptions.ApplyToStorageCommand<AccountProperties>(cmd1);
      return cmd1;
    }

    private RESTCommand<ServiceProperties> GetServicePropertiesImpl(
      BlobRequestOptions requestOptions)
    {
      RESTCommand<ServiceProperties> cmd1 = new RESTCommand<ServiceProperties>(this.Credentials, this.StorageUri, this.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetServiceProperties(uri, serverTimeout, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, Exception, OperationContext, ServiceProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceProperties>(HttpStatusCode.OK, resp, (ServiceProperties) null, (StorageCommandBase<ServiceProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceProperties>>) ((cmd, resp, ctx, ct) => BlobHttpResponseParsers.ReadServicePropertiesAsync(cmd.ResponseStream, ct));
      requestOptions.ApplyToStorageCommand<ServiceProperties>(cmd1);
      return cmd1;
    }

    private RESTCommand<NullType> SetServicePropertiesImpl(
      ServiceProperties properties,
      BlobRequestOptions requestOptions)
    {
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream(this.BufferManager, 1024);
      try
      {
        properties.WriteServiceProperties((Stream) memoryStream);
      }
      catch (InvalidOperationException ex)
      {
        memoryStream.Dispose();
        throw new ArgumentException(ex.Message, nameof (properties));
      }
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.Credentials, this.StorageUri, this.HttpClient);
      requestOptions.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.SetServiceProperties(uri, serverTimeout, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      requestOptions.ApplyToStorageCommand<NullType>(cmd1);
      return cmd1;
    }

    private RESTCommand<ServiceStats> GetServiceStatsImpl(BlobRequestOptions requestOptions)
    {
      LocationMode? locationMode = requestOptions.LocationMode;
      if (0 == (int) locationMode.GetValueOrDefault() & locationMode.HasValue)
        throw new InvalidOperationException("GetServiceStats cannot be run with a 'PrimaryOnly' location mode.");
      RESTCommand<ServiceStats> cmd1 = new RESTCommand<ServiceStats>(this.Credentials, this.StorageUri, this.HttpClient);
      requestOptions.ApplyToStorageCommand<ServiceStats>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceStats>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetServiceStats(uri, serverTimeout, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, Exception, OperationContext, ServiceStats>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceStats>(HttpStatusCode.OK, resp, (ServiceStats) null, (StorageCommandBase<ServiceStats>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceStats>>) ((cmd, resp, ctx, ct) => BlobHttpResponseParsers.ReadServiceStatsAsync(cmd.ResponseStream, ct));
      return cmd1;
    }

    private RESTCommand<UserDelegationKey> GetUserDelegationKeyImpl(
      DateTimeOffset start,
      DateTimeOffset expiry,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream((IBufferManager) null, 1024);
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8,
        NewLineHandling = NewLineHandling.Entitize
      };
      using (XmlWriter xmlWriter = XmlWriter.Create((Stream) memoryStream, settings))
      {
        xmlWriter.WriteStartElement("KeyInfo");
        xmlWriter.WriteElementString("Start", start.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
        xmlWriter.WriteElementString("Expiry", expiry.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
        xmlWriter.WriteEndDocument();
      }
      memoryStream.Seek(0L, SeekOrigin.Begin);
      bool? nullable = options.ChecksumOptions.UseTransactionalMD5;
      string md5;
      if (nullable.HasValue)
      {
        nullable = options.ChecksumOptions.UseTransactionalMD5;
        if (nullable.Value)
        {
          md5 = memoryStream.ComputeMD5Hash();
          goto label_9;
        }
      }
      md5 = (string) null;
label_9:
      nullable = options.ChecksumOptions.UseTransactionalCRC64;
      string crc64;
      if (nullable.HasValue)
      {
        nullable = options.ChecksumOptions.UseTransactionalCRC64;
        if (nullable.Value)
        {
          crc64 = memoryStream.ComputeCRC64Hash();
          goto label_13;
        }
      }
      crc64 = (string) null;
label_13:
      Checksum contentChecksum = new Checksum(md5, crc64);
      RESTCommand<UserDelegationKey> postCmd = new RESTCommand<UserDelegationKey>(this.Credentials, this.StorageUri, this.HttpClient);
      options.ApplyToStorageCommand<UserDelegationKey>(postCmd);
      postCmd.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      postCmd.BuildRequest = (Func<RESTCommand<UserDelegationKey>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetUserDelegationKey(uri, serverTimeout, accessCondition, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      postCmd.BuildContent = (Func<RESTCommand<UserDelegationKey>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<UserDelegationKey>((Stream) memoryStream, 0L, new long?(memoryStream.Length), contentChecksum, cmd, ctx));
      postCmd.StreamToDispose = (Stream) memoryStream;
      postCmd.RetrieveResponseStream = true;
      postCmd.PreProcessResponse = (Func<RESTCommand<UserDelegationKey>, HttpResponseMessage, Exception, OperationContext, UserDelegationKey>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<UserDelegationKey>(HttpStatusCode.OK, resp, (UserDelegationKey) null, (StorageCommandBase<UserDelegationKey>) cmd, ex));
      postCmd.PostProcessResponseAsync = (Func<RESTCommand<UserDelegationKey>, HttpResponseMessage, OperationContext, CancellationToken, Task<UserDelegationKey>>) ((cmd, resp, ctx, ct) => GetUserDelegationKeyResponse.ParseAsync(postCmd.ResponseStream, ct));
      return postCmd;
    }

    public CloudBlobClient(Uri baseUri, DelegatingHandler delegatingHandler = null)
      : this(baseUri, (StorageCredentials) null, delegatingHandler)
    {
    }

    public CloudBlobClient(
      Uri baseUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
      : this(new StorageUri(baseUri), credentials, delegatingHandler)
    {
    }

    public CloudBlobClient(
      StorageUri storageUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
    {
      this.StorageUri = storageUri;
      this.Credentials = credentials ?? new StorageCredentials();
      this.DefaultRequestOptions = new BlobRequestOptions()
      {
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(),
        LocationMode = BlobRequestOptions.BaseDefaultRequestOptions.LocationMode,
        SingleBlobUploadThresholdInBytes = BlobRequestOptions.BaseDefaultRequestOptions.SingleBlobUploadThresholdInBytes,
        ParallelOperationThreadCount = BlobRequestOptions.BaseDefaultRequestOptions.ParallelOperationThreadCount,
        ChecksumOptions = new ChecksumOptions()
        {
          StoreContentCRC64 = new bool?(false)
        }
      };
      this.DefaultDelimiter = "/";
      this.AuthenticationScheme = this.Credentials.IsToken ? AuthenticationScheme.Token : AuthenticationScheme.SharedKey;
      this.UsePathStyleUris = CommonUtility.UsePathStyleAddressing(this.BaseUri);
      this.HttpClient = HttpClientFactory.HttpClientFromDelegatingHandler(delegatingHandler);
    }

    public IBufferManager BufferManager { get; set; }

    public StorageCredentials Credentials { get; private set; }

    public Uri BaseUri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public BlobRequestOptions DefaultRequestOptions { get; set; }

    [Obsolete("Use DefaultRequestOptions.RetryPolicy.")]
    public IRetryPolicy RetryPolicy
    {
      get => this.DefaultRequestOptions.RetryPolicy;
      set => this.DefaultRequestOptions.RetryPolicy = value;
    }

    public string DefaultDelimiter
    {
      get => this.defaultDelimiter;
      set
      {
        CommonUtility.AssertNotNullOrEmpty(nameof (DefaultDelimiter), value);
        CommonUtility.AssertNotNullOrEmpty(nameof (DefaultDelimiter), value);
        this.defaultDelimiter = !(value == "\\") ? value : throw new ArgumentException("\\ is an invalid delimiter.");
      }
    }

    internal bool UsePathStyleUris { get; private set; }

    public virtual CloudBlobContainer GetRootContainerReference() => new CloudBlobContainer("$root", this);

    public virtual CloudBlobContainer GetContainerReference(string containerName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (containerName), containerName);
      return new CloudBlobContainer(containerName, this);
    }

    internal ICanonicalizer GetCanonicalizer() => this.AuthenticationScheme == AuthenticationScheme.SharedKeyLite ? (ICanonicalizer) SharedKeyLiteCanonicalizer.Instance : (ICanonicalizer) SharedKeyCanonicalizer.Instance;

    private static void ParseUserPrefix(
      string prefix,
      out string containerName,
      out string listingPrefix)
    {
      CommonUtility.AssertNotNull(nameof (prefix), (object) prefix);
      containerName = (string) null;
      listingPrefix = (string) null;
      string[] strArray = prefix.Split(NavigationHelper.SlashAsSplitOptions, 2, StringSplitOptions.None);
      if (strArray.Length == 1)
      {
        listingPrefix = strArray[0];
      }
      else
      {
        containerName = strArray[0];
        listingPrefix = strArray[1];
      }
      if (string.IsNullOrEmpty(containerName))
        containerName = "$root";
      if (!string.IsNullOrEmpty(listingPrefix))
        return;
      listingPrefix = (string) null;
    }

    public Task<IList<BlobBatchSubOperationResponse>> ExecuteBatchAsync(
      BatchOperation batchOperation,
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommonUtility.AssertNotNull(nameof (batchOperation), (object) batchOperation);
      return batchOperation.ExecuteAsync(this, requestOptions, operationContext, cancellationToken);
    }
  }
}
