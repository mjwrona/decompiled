// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.CloudQueueClient
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Queue.Protocol;
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

namespace Microsoft.Azure.Storage.Queue
{
  public class CloudQueueClient
  {
    private AuthenticationScheme authenticationScheme;
    internal HttpClient HttpClient;

    public AuthenticationScheme AuthenticationScheme
    {
      get => this.authenticationScheme;
      set => this.authenticationScheme = value;
    }

    public virtual IEnumerable<CloudQueue> ListQueues(
      string prefix = null,
      QueueListingDetails queueListingDetails = QueueListingDetails.None,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions modifiedOptions = QueueRequestOptions.ApplyDefaults(options, this);
      operationContext = operationContext ?? new OperationContext();
      return CommonUtility.LazyEnumerable<CloudQueue>((Func<IContinuationToken, ResultSegment<CloudQueue>>) (token => this.ListQueuesSegmentedCore(prefix, queueListingDetails, new int?(), token as QueueContinuationToken, modifiedOptions, operationContext)), long.MaxValue);
    }

    public virtual QueueResultSegment ListQueuesSegmented(QueueContinuationToken currentToken) => this.ListQueuesSegmented((string) null, QueueListingDetails.None, new int?(), currentToken);

    public virtual QueueResultSegment ListQueuesSegmented(
      string prefix,
      QueueContinuationToken currentToken)
    {
      return this.ListQueuesSegmented(prefix, QueueListingDetails.None, new int?(), currentToken);
    }

    public virtual QueueResultSegment ListQueuesSegmented(
      string prefix,
      QueueListingDetails queueListingDetails,
      int? maxResults,
      QueueContinuationToken currentToken,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this);
      operationContext = operationContext ?? new OperationContext();
      ResultSegment<CloudQueue> resultSegment = this.ListQueuesSegmentedCore(prefix, queueListingDetails, maxResults, currentToken, options1, operationContext);
      return new QueueResultSegment((IEnumerable<CloudQueue>) resultSegment.Results, (QueueContinuationToken) resultSegment.ContinuationToken);
    }

    private ResultSegment<CloudQueue> ListQueuesSegmentedCore(
      string prefix,
      QueueListingDetails queueListingDetails,
      int? maxResults,
      QueueContinuationToken currentToken,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ResultSegment<CloudQueue>>(this.ListQueuesImpl(prefix, maxResults, queueListingDetails, options, currentToken), options.RetryPolicy, operationContext);
    }

    public virtual ICancellableAsyncResult BeginListQueuesSegmented(
      QueueContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListQueuesSegmented((string) null, QueueListingDetails.None, new int?(), currentToken, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    public virtual ICancellableAsyncResult BeginListQueuesSegmented(
      string prefix,
      QueueContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListQueuesSegmented(prefix, QueueListingDetails.None, new int?(), currentToken, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    public virtual ICancellableAsyncResult BeginListQueuesSegmented(
      string prefix,
      QueueListingDetails queueListingDetails,
      int? maxResults,
      QueueContinuationToken currentToken,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<QueueResultSegment>((Func<CancellationToken, Task<QueueResultSegment>>) (token => this.ListQueuesSegmentedAsync(prefix, queueListingDetails, maxResults, currentToken, options, operationContext, token)), callback, state);
    }

    public virtual QueueResultSegment EndListQueuesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<QueueResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<QueueResultSegment> ListQueuesSegmentedAsync(
      QueueContinuationToken currentToken)
    {
      return this.ListQueuesSegmentedAsync(currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<QueueResultSegment> ListQueuesSegmentedAsync(
      QueueContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListQueuesSegmentedAsync((string) null, QueueListingDetails.None, new int?(), currentToken, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<QueueResultSegment> ListQueuesSegmentedAsync(
      string prefix,
      QueueContinuationToken currentToken)
    {
      return this.ListQueuesSegmentedAsync(prefix, currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<QueueResultSegment> ListQueuesSegmentedAsync(
      string prefix,
      QueueContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListQueuesSegmentedAsync(prefix, QueueListingDetails.None, new int?(), currentToken, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<QueueResultSegment> ListQueuesSegmentedAsync(
      string prefix,
      QueueListingDetails queueListingDetails,
      int? maxResults,
      QueueContinuationToken currentToken,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListQueuesSegmentedAsync(prefix, queueListingDetails, maxResults, currentToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<QueueResultSegment> ListQueuesSegmentedAsync(
      string prefix,
      QueueListingDetails queueListingDetails,
      int? maxResults,
      QueueContinuationToken currentToken,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudQueueClient serviceClient = this;
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, serviceClient);
      operationContext = operationContext ?? new OperationContext();
      ResultSegment<CloudQueue> resultSegment = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ResultSegment<CloudQueue>>(serviceClient.ListQueuesImpl(prefix, maxResults, queueListingDetails, options1, currentToken), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      return new QueueResultSegment((IEnumerable<CloudQueue>) resultSegment.Results, (QueueContinuationToken) resultSegment.ContinuationToken);
    }

    private RESTCommand<ResultSegment<CloudQueue>> ListQueuesImpl(
      string prefix,
      int? maxResults,
      QueueListingDetails queueListingDetails,
      QueueRequestOptions options,
      QueueContinuationToken currentToken)
    {
      ListingContext listingContext = new ListingContext(prefix, maxResults)
      {
        Marker = currentToken?.NextMarker
      };
      RESTCommand<ResultSegment<CloudQueue>> cmd1 = new RESTCommand<ResultSegment<CloudQueue>>(this.Credentials, this.StorageUri, this.HttpClient);
      options.ApplyToStorageCommand<ResultSegment<CloudQueue>>(cmd1);
      cmd1.CommandLocationMode = CommonUtility.GetListingLocationMode((IContinuationToken) currentToken);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<ResultSegment<CloudQueue>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.List(uri, serverTimeout, listingContext, queueListingDetails, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<ResultSegment<CloudQueue>>, HttpResponseMessage, Exception, OperationContext, ResultSegment<CloudQueue>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ResultSegment<CloudQueue>>(HttpStatusCode.OK, resp, (ResultSegment<CloudQueue>) null, (StorageCommandBase<ResultSegment<CloudQueue>>) cmd, ex));
      Func<QueueEntry, CloudQueue> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ResultSegment<CloudQueue>>, HttpResponseMessage, OperationContext, CancellationToken, Task<ResultSegment<CloudQueue>>>) (async (cmd, resp, ctx, ct) =>
      {
        ListQueuesResponse listQueuesResponse = await ListQueuesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        List<CloudQueue> list = listQueuesResponse.Queues.Select<QueueEntry, CloudQueue>(func ?? (func = (Func<QueueEntry, CloudQueue>) (item => new CloudQueue(item.Metadata, item.Name, this)))).ToList<CloudQueue>();
        QueueContinuationToken continuationToken = (QueueContinuationToken) null;
        if (listQueuesResponse.NextMarker != null)
          continuationToken = new QueueContinuationToken()
          {
            NextMarker = listQueuesResponse.NextMarker,
            TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation)
          };
        return new ResultSegment<CloudQueue>(list)
        {
          ContinuationToken = (IContinuationToken) continuationToken
        };
      });
      return cmd1;
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetServiceProperties((QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceProperties(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ServiceProperties>((Func<CancellationToken, Task<ServiceProperties>>) (token => this.GetServicePropertiesAsync(options, operationContext, token)), callback, state);
    }

    public virtual ServiceProperties EndGetServiceProperties(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<ServiceProperties>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync() => this.GetServicePropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetServicePropertiesAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      QueueRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServicePropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions requestOptions = QueueRequestOptions.ApplyDefaults(options, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ServiceProperties GetServiceProperties(
      QueueRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = QueueRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ServiceProperties>(this.GetServicePropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      ServiceProperties properties,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetServiceProperties(properties, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetServiceProperties(
      ServiceProperties properties,
      QueueRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = QueueRequestOptions.ApplyDefaults(requestOptions, this);
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
      return this.SetServicePropertiesAsync(properties, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetServicePropertiesAsync(properties, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      options = QueueRequestOptions.ApplyDefaults(options, this);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetServicePropertiesImpl(properties, options), options.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetServiceProperties(
      ServiceProperties properties,
      QueueRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = QueueRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetServicePropertiesImpl(properties, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceStats(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetServiceStats((QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetServiceStats(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ServiceStats>((Func<CancellationToken, Task<ServiceStats>>) (token => this.GetServiceStatsAsync(options, operationContext, token)), callback, state);
    }

    public virtual ServiceStats EndGetServiceStats(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<ServiceStats>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync() => this.GetServiceStatsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(CancellationToken cancellationToken) => this.GetServiceStatsAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(
      QueueRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServiceStatsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ServiceStats> GetServiceStatsAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions requestOptions = QueueRequestOptions.ApplyDefaults(options, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ServiceStats>(this.GetServiceStatsImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ServiceStats GetServiceStats(
      QueueRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = QueueRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ServiceStats>(this.GetServiceStatsImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    private RESTCommand<ServiceProperties> GetServicePropertiesImpl(
      QueueRequestOptions requestOptions)
    {
      RESTCommand<ServiceProperties> cmd1 = new RESTCommand<ServiceProperties>(this.Credentials, this.StorageUri, this.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetServiceProperties(uri, serverTimeout, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, Exception, OperationContext, ServiceProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceProperties>(HttpStatusCode.OK, resp, (ServiceProperties) null, (StorageCommandBase<ServiceProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceProperties>>) ((cmd, resp, ctx, ct) => QueueHttpResponseParsers.ReadServicePropertiesAsync(cmd.ResponseStream, ct));
      requestOptions.ApplyToStorageCommand<ServiceProperties>(cmd1);
      return cmd1;
    }

    private RESTCommand<NullType> SetServicePropertiesImpl(
      ServiceProperties properties,
      QueueRequestOptions requestOptions)
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
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.SetServiceProperties(uri, serverTimeout, cnt, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, (NullType) null, (StorageCommandBase<NullType>) cmd, ex));
      requestOptions.ApplyToStorageCommand<NullType>(cmd1);
      return cmd1;
    }

    private RESTCommand<ServiceStats> GetServiceStatsImpl(QueueRequestOptions requestOptions)
    {
      LocationMode? locationMode = requestOptions.LocationMode;
      if (0 == (int) locationMode.GetValueOrDefault() & locationMode.HasValue)
        throw new InvalidOperationException("GetServiceStats cannot be run with a 'PrimaryOnly' location mode.");
      RESTCommand<ServiceStats> cmd1 = new RESTCommand<ServiceStats>(this.Credentials, this.StorageUri, this.HttpClient);
      requestOptions.ApplyToStorageCommand<ServiceStats>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceStats>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetServiceStats(uri, serverTimeout, ctx, this.GetCanonicalizer(), this.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, Exception, OperationContext, ServiceStats>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceStats>(HttpStatusCode.OK, resp, (ServiceStats) null, (StorageCommandBase<ServiceStats>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceStats>>) ((cmd, resp, ctx, ct) => QueueHttpResponseParsers.ReadServiceStatsAsync(cmd.ResponseStream, ct));
      return cmd1;
    }

    public CloudQueueClient(
      Uri baseUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
      : this(new StorageUri(baseUri), credentials, delegatingHandler)
    {
    }

    public CloudQueueClient(
      StorageUri storageUri,
      StorageCredentials credentials,
      DelegatingHandler delegatingHandler = null)
    {
      this.StorageUri = storageUri;
      this.Credentials = credentials ?? new StorageCredentials();
      this.DefaultRequestOptions = new QueueRequestOptions(QueueRequestOptions.BaseDefaultRequestOptions)
      {
        RetryPolicy = (IRetryPolicy) new ExponentialRetry()
      };
      this.AuthenticationScheme = this.Credentials.IsToken ? AuthenticationScheme.Token : AuthenticationScheme.SharedKey;
      this.UsePathStyleUris = CommonUtility.UsePathStyleAddressing(this.BaseUri);
      this.HttpClient = HttpClientFactory.HttpClientFromDelegatingHandler(delegatingHandler);
    }

    public IBufferManager BufferManager { get; set; }

    public StorageCredentials Credentials { get; private set; }

    public Uri BaseUri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public QueueRequestOptions DefaultRequestOptions { get; set; }

    internal bool UsePathStyleUris { get; private set; }

    public virtual CloudQueue GetQueueReference(string queueName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (queueName), queueName);
      return new CloudQueue(queueName, this);
    }

    internal ICanonicalizer GetCanonicalizer() => this.AuthenticationScheme == AuthenticationScheme.SharedKeyLite ? (ICanonicalizer) SharedKeyLiteCanonicalizer.Instance : (ICanonicalizer) SharedKeyCanonicalizer.Instance;
  }
}
