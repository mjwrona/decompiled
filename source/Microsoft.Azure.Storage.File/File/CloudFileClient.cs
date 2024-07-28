// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileClient
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.File.Protocol;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  public class CloudFileClient
  {
    private AuthenticationScheme authenticationScheme;
    internal HttpClient HttpClient;

    public AuthenticationScheme AuthenticationScheme
    {
      get => this.authenticationScheme;
      set => this.authenticationScheme = value;
    }

    [DoesServiceRequest]
    public virtual IEnumerable<CloudFileShare> ListShares(
      string prefix = null,
      ShareListingDetails detailsIncluded = ShareListingDetails.None,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this);
      return CommonUtility.LazyEnumerable<CloudFileShare>((Func<IContinuationToken, ResultSegment<CloudFileShare>>) (token => this.ListSharesSegmentedCore(prefix, detailsIncluded, new int?(), (FileContinuationToken) token, modifiedOptions, operationContext)), long.MaxValue);
    }

    [DoesServiceRequest]
    public virtual ShareResultSegment ListSharesSegmented(FileContinuationToken currentToken) => this.ListSharesSegmented((string) null, ShareListingDetails.None, new int?(), currentToken);

    [DoesServiceRequest]
    public virtual ShareResultSegment ListSharesSegmented(
      string prefix,
      FileContinuationToken currentToken)
    {
      return this.ListSharesSegmented(prefix, ShareListingDetails.None, new int?(), currentToken);
    }

    [DoesServiceRequest]
    public virtual ShareResultSegment ListSharesSegmented(
      string prefix,
      ShareListingDetails detailsIncluded,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this);
      ResultSegment<CloudFileShare> resultSegment = this.ListSharesSegmentedCore(prefix, detailsIncluded, maxResults, currentToken, options1, operationContext);
      return new ShareResultSegment((IEnumerable<CloudFileShare>) resultSegment.Results, (FileContinuationToken) resultSegment.ContinuationToken);
    }

    private ResultSegment<CloudFileShare> ListSharesSegmentedCore(
      string prefix,
      ShareListingDetails detailsIncluded,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ResultSegment<CloudFileShare>>(this.ListSharesImpl(prefix, detailsIncluded, currentToken, maxResults, options), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListSharesSegmented(
      FileContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListSharesSegmented((string) null, ShareListingDetails.None, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListSharesSegmented(
      string prefix,
      FileContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListSharesSegmented(prefix, ShareListingDetails.None, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListSharesSegmented(
      string prefix,
      ShareListingDetails detailsIncluded,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this);
      return CancellableAsyncResultTaskWrapper.Create<ShareResultSegment>((Func<CancellationToken, Task<ShareResultSegment>>) (token => this.ListSharesSegmentedAsync(prefix, detailsIncluded, maxResults, currentToken, modifiedOptions, operationContext)), callback, state);
    }

    public virtual ShareResultSegment EndListSharesSegmented(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      ShareResultSegment result = ((CancellableAsyncResultTaskWrapper<ShareResultSegment>) asyncResult).GetAwaiter().GetResult();
      return new ShareResultSegment(result.Results, result.ContinuationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ShareResultSegment> ListSharesSegmentedAsync(
      FileContinuationToken currentToken)
    {
      return this.ListSharesSegmentedAsync(currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ShareResultSegment> ListSharesSegmentedAsync(
      FileContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListSharesSegmentedAsync((string) null, currentToken, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ShareResultSegment> ListSharesSegmentedAsync(
      string prefix,
      FileContinuationToken currentToken)
    {
      return this.ListSharesSegmentedAsync(prefix, currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ShareResultSegment> ListSharesSegmentedAsync(
      string prefix,
      FileContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListSharesSegmentedAsync(prefix, ShareListingDetails.None, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ShareResultSegment> ListSharesSegmentedAsync(
      string prefix,
      ShareListingDetails detailsIncluded,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListSharesSegmentedAsync(prefix, detailsIncluded, maxResults, currentToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<ShareResultSegment> ListSharesSegmentedAsync(
      string prefix,
      ShareListingDetails detailsIncluded,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudFileClient serviceClient = this;
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, serviceClient);
      ResultSegment<CloudFileShare> resultSegment = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ResultSegment<CloudFileShare>>(serviceClient.ListSharesImpl(prefix, detailsIncluded, currentToken, maxResults, options1), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      return new ShareResultSegment((IEnumerable<CloudFileShare>) resultSegment.Results, (FileContinuationToken) resultSegment.ContinuationToken);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetServiceProperties((FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      FileRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<FileServiceProperties>((Func<CancellationToken, Task<FileServiceProperties>>) (token => this.GetServicePropertiesAsync(requestOptions, operationContext)), callback, state);
    }

    public virtual FileServiceProperties EndGetServiceProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<FileServiceProperties>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<FileServiceProperties> GetServicePropertiesAsync() => this.GetServicePropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<FileServiceProperties> GetServicePropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetServicePropertiesAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<FileServiceProperties> GetServicePropertiesAsync(
      FileRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServicePropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<FileServiceProperties> GetServicePropertiesAsync(
      FileRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<FileServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual FileServiceProperties GetServiceProperties(
      FileRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<FileServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      FileServiceProperties properties,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetServiceProperties(properties, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      FileServiceProperties properties,
      FileRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetServicePropertiesAsync(properties, requestOptions, operationContext)), callback, state);
    }

    public virtual void EndSetServiceProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(FileServiceProperties properties) => this.SetServicePropertiesAsync(properties, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      FileServiceProperties properties,
      CancellationToken cancellationToken)
    {
      return this.SetServicePropertiesAsync(properties, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      FileServiceProperties properties,
      FileRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.SetServicePropertiesAsync(properties, requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      FileServiceProperties properties,
      FileRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetServicePropertiesImpl(properties, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetServiceProperties(
      FileServiceProperties properties,
      FileRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = FileRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetServicePropertiesImpl(properties, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    private RESTCommand<ResultSegment<CloudFileShare>> ListSharesImpl(
      string prefix,
      ShareListingDetails detailsIncluded,
      FileContinuationToken currentToken,
      int? maxResults,
      FileRequestOptions options)
    {
      ListingContext listingContext = new ListingContext(prefix, maxResults)
      {
        Marker = currentToken?.NextMarker
      };
      RESTCommand<ResultSegment<CloudFileShare>> cmd1 = new RESTCommand<ResultSegment<CloudFileShare>>(this.Credentials, this.StorageUri, this.HttpClient);
      options.ApplyToStorageCommand<ResultSegment<CloudFileShare>>(cmd1);
      cmd1.CommandLocationMode = CommonUtility.GetListingLocationMode((IContinuationToken) currentToken);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<ResultSegment<CloudFileShare>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.List(uri, serverTimeout, listingContext, detailsIncluded, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<ResultSegment<CloudFileShare>>, HttpResponseMessage, Exception, OperationContext, ResultSegment<CloudFileShare>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ResultSegment<CloudFileShare>>(HttpStatusCode.OK, resp, (ResultSegment<CloudFileShare>) null, (StorageCommandBase<ResultSegment<CloudFileShare>>) cmd, ex));
      Func<FileShareEntry, CloudFileShare> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ResultSegment<CloudFileShare>>, HttpResponseMessage, OperationContext, CancellationToken, Task<ResultSegment<CloudFileShare>>>) (async (cmd, resp, ctx, ct) =>
      {
        ListSharesResponse listSharesResponse = await ListSharesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        List<CloudFileShare> list = listSharesResponse.Shares.Select<FileShareEntry, CloudFileShare>(func ?? (func = (Func<FileShareEntry, CloudFileShare>) (item => new CloudFileShare(item.Properties, item.Metadata, item.Name, item.SnapshotTime, this)))).ToList<CloudFileShare>();
        FileContinuationToken continuationToken = (FileContinuationToken) null;
        if (listSharesResponse.NextMarker != null)
          continuationToken = new FileContinuationToken()
          {
            NextMarker = listSharesResponse.NextMarker,
            TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation)
          };
        return new ResultSegment<CloudFileShare>(list)
        {
          ContinuationToken = (IContinuationToken) continuationToken
        };
      });
      return cmd1;
    }

    private RESTCommand<FileServiceProperties> GetServicePropertiesImpl(
      FileRequestOptions requestOptions)
    {
      RESTCommand<FileServiceProperties> cmd1 = new RESTCommand<FileServiceProperties>(this.Credentials, this.StorageUri, this.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<FileServiceProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.GetServiceProperties(uri, serverTimeout, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<FileServiceProperties>, HttpResponseMessage, Exception, OperationContext, FileServiceProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<FileServiceProperties>(HttpStatusCode.OK, resp, (FileServiceProperties) null, (StorageCommandBase<FileServiceProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<FileServiceProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<FileServiceProperties>>) ((cmd, resp, ctx, ct) => FileHttpResponseParsers.ReadServicePropertiesAsync(cmd.ResponseStream, ct));
      requestOptions.ApplyToStorageCommand<FileServiceProperties>(cmd1);
      return cmd1;
    }

    private RESTCommand<NullType> SetServicePropertiesImpl(
      FileServiceProperties properties,
      FileRequestOptions requestOptions)
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
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.SetServiceProperties(uri, serverTimeout, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.RecoveryAction = new Action<StorageCommandBase<NullType>, Exception, OperationContext>(RecoveryActions.RewindStream<NullType>);
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, (NullType) null, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    public CloudFileClient(
      Uri baseUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
      : this(new StorageUri(baseUri), credentials, delegatingHandler)
    {
    }

    public CloudFileClient(
      StorageUri storageUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
    {
      this.StorageUri = storageUri;
      this.Credentials = credentials ?? new StorageCredentials();
      this.DefaultRequestOptions = new FileRequestOptions()
      {
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(),
        LocationMode = FileRequestOptions.BaseDefaultRequestOptions.LocationMode,
        ParallelOperationThreadCount = FileRequestOptions.BaseDefaultRequestOptions.ParallelOperationThreadCount
      };
      this.AuthenticationScheme = AuthenticationScheme.SharedKey;
      this.UsePathStyleUris = CommonUtility.UsePathStyleAddressing(this.BaseUri);
      this.HttpClient = HttpClientFactory.HttpClientFromDelegatingHandler(delegatingHandler);
    }

    public IBufferManager BufferManager { get; set; }

    public StorageCredentials Credentials { get; private set; }

    public Uri BaseUri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public FileRequestOptions DefaultRequestOptions { get; set; }

    internal bool UsePathStyleUris { get; private set; }

    public virtual CloudFileShare GetShareReference(string shareName) => this.GetShareReference(shareName, new DateTimeOffset?());

    public CloudFileShare GetShareReference(string shareName, DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (shareName), shareName);
      return new CloudFileShare(shareName, snapshotTime, this);
    }

    internal ICanonicalizer GetCanonicalizer() => this.AuthenticationScheme == AuthenticationScheme.SharedKeyLite ? (ICanonicalizer) SharedKeyLiteCanonicalizer.Instance : (ICanonicalizer) SharedKeyCanonicalizer.Instance;
  }
}
