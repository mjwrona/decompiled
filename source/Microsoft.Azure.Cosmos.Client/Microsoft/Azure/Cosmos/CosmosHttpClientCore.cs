// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosHttpClientCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosHttpClientCore : CosmosHttpClient
  {
    private readonly HttpClient httpClient;
    private readonly ICommunicationEventSource eventSource;
    private bool disposedValue;

    private CosmosHttpClientCore(
      HttpClient httpClient,
      HttpMessageHandler httpMessageHandler,
      ICommunicationEventSource eventSource)
    {
      this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));
      this.eventSource = eventSource ?? throw new ArgumentNullException(nameof (eventSource));
      this.HttpMessageHandler = httpMessageHandler;
    }

    public override HttpMessageHandler HttpMessageHandler { get; }

    public static CosmosHttpClient CreateWithConnectionPolicy(
      ApiType apiType,
      ICommunicationEventSource eventSource,
      ConnectionPolicy connectionPolicy,
      HttpMessageHandler httpMessageHandler,
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs,
      EventHandler<ReceivedResponseEventArgs> receivedResponseEventArgs)
    {
      Func<HttpClient> func = connectionPolicy != null ? connectionPolicy.HttpClientFactory : throw new ArgumentNullException(nameof (connectionPolicy));
      if (func != null)
      {
        if (sendingRequestEventArgs != null && receivedResponseEventArgs != null)
          throw new InvalidOperationException("HttpClientFactory can not be set at the same time as sendingRequestEventArgs or ReceivedResponseEventArgs");
        return CosmosHttpClientCore.CreateHelper(func() ?? throw new ArgumentNullException("httpClientFactory returned null. httpClientFactory must return a HttpClient instance."), httpMessageHandler, connectionPolicy.RequestTimeout, connectionPolicy.UserAgentContainer, apiType, eventSource);
      }
      if (httpMessageHandler == null)
        httpMessageHandler = CosmosHttpClientCore.CreateHttpClientHandler(connectionPolicy.MaxConnectionLimit, (IWebProxy) null);
      if (sendingRequestEventArgs != null || receivedResponseEventArgs != null)
        httpMessageHandler = CosmosHttpClientCore.CreateHttpMessageHandler(httpMessageHandler, sendingRequestEventArgs, receivedResponseEventArgs);
      return CosmosHttpClientCore.CreateHelper(new HttpClient(httpMessageHandler), httpMessageHandler, connectionPolicy.RequestTimeout, connectionPolicy.UserAgentContainer, apiType, eventSource);
    }

    public static HttpMessageHandler CreateHttpClientHandler(
      int gatewayModeMaxConnectionLimit,
      IWebProxy webProxy)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler();
      if (webProxy != null)
        httpClientHandler.Proxy = webProxy;
      try
      {
        httpClientHandler.MaxConnectionsPerServer = gatewayModeMaxConnectionLimit;
      }
      catch (PlatformNotSupportedException ex)
      {
      }
      return (HttpMessageHandler) httpClientHandler;
    }

    private static HttpMessageHandler CreateHttpMessageHandler(
      HttpMessageHandler innerHandler,
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs,
      EventHandler<ReceivedResponseEventArgs> receivedResponseEventArgs)
    {
      return (HttpMessageHandler) new CosmosHttpClientCore.HttpRequestMessageHandler(sendingRequestEventArgs, receivedResponseEventArgs, innerHandler);
    }

    private static CosmosHttpClient CreateHelper(
      HttpClient httpClient,
      HttpMessageHandler httpMessageHandler,
      TimeSpan requestTimeout,
      UserAgentContainer userAgentContainer,
      ApiType apiType,
      ICommunicationEventSource eventSource)
    {
      if (httpClient == null)
        throw new ArgumentNullException(nameof (httpClient));
      httpClient.Timeout = requestTimeout > CosmosHttpClient.GatewayRequestTimeout ? requestTimeout : CosmosHttpClient.GatewayRequestTimeout;
      httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
      {
        NoCache = true
      };
      httpClient.AddUserAgentHeader((Microsoft.Azure.Documents.UserAgentContainer) userAgentContainer);
      httpClient.AddApiTypeHeader(apiType);
      httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      httpClient.DefaultRequestHeaders.Add("x-ms-cosmos-sdk-supportedcapabilities", Microsoft.Azure.Cosmos.Headers.SDKSUPPORTEDCAPABILITIES);
      httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
      return (CosmosHttpClient) new CosmosHttpClientCore(httpClient, httpMessageHandler, eventSource);
    }

    public override Task<HttpResponseMessage> GetAsync(
      Uri uri,
      INameValueCollection additionalHeaders,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      return this.SendHttpAsync(new Func<ValueTask<HttpRequestMessage>>(CreateRequestMessage), resourceType, timeoutPolicy, clientSideRequestStatistics, cancellationToken);

      ValueTask<HttpRequestMessage> CreateRequestMessage()
      {
        HttpRequestMessage result = new HttpRequestMessage(HttpMethod.Get, uri);
        if (additionalHeaders != null)
        {
          foreach (string additionalHeader in (IEnumerable) additionalHeaders)
          {
            if (GatewayStoreClient.IsAllowedRequestHeader(additionalHeader))
              result.Headers.TryAddWithoutValidation(additionalHeader, additionalHeaders[additionalHeader]);
          }
        }
        return new ValueTask<HttpRequestMessage>(result);
      }
    }

    public override Task<HttpResponseMessage> SendHttpAsync(
      Func<ValueTask<HttpRequestMessage>> createRequestMessageAsync,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      if (createRequestMessageAsync == null)
        throw new ArgumentNullException(nameof (createRequestMessageAsync));
      return this.SendHttpHelperAsync(createRequestMessageAsync, resourceType, timeoutPolicy, clientSideRequestStatistics, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendHttpHelperAsync(
      Func<ValueTask<HttpRequestMessage>> createRequestMessageAsync,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      DateTime startDateTimeUtc = DateTime.UtcNow;
      IEnumerator<(TimeSpan requestTimeout, TimeSpan delayForNextRequest)> timeoutEnumerator = timeoutPolicy.GetTimeoutEnumerator();
      timeoutEnumerator.MoveNext();
      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        (TimeSpan timeSpan1, TimeSpan timeSpan2) = timeoutEnumerator.Current;
        using (HttpRequestMessage requestMessage = await createRequestMessageAsync())
        {
          using (CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
          {
            cancellationTokenSource.CancelAfter(timeSpan1);
            DateTime requestStartTime = DateTime.UtcNow;
            try
            {
              HttpResponseMessage httpResponseMessage = await this.ExecuteHttpHelperAsync(requestMessage, resourceType, cancellationTokenSource.Token);
              if (clientSideRequestStatistics is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum)
                statisticsTraceDatum.RecordHttpResponse(requestMessage, httpResponseMessage, resourceType, requestStartTime);
              if (!timeoutPolicy.ShouldRetryBasedOnResponse(requestMessage.Method, httpResponseMessage))
                return httpResponseMessage;
              if (CosmosHttpClientCore.IsOutOfRetries(timeoutPolicy, startDateTimeUtc, timeoutEnumerator))
                return httpResponseMessage;
            }
            catch (Exception ex1)
            {
              if (clientSideRequestStatistics is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum)
                statisticsTraceDatum.RecordHttpException(requestMessage, ex1, resourceType, requestStartTime);
              bool flag = CosmosHttpClientCore.IsOutOfRetries(timeoutPolicy, startDateTimeUtc, timeoutEnumerator);
              switch (ex1)
              {
                case OperationCanceledException _:
                  if (cancellationToken.IsCancellationRequested)
                  {
                    throw;
                  }
                  else
                  {
                    if (!flag)
                    {
                      if (timeoutPolicy.IsSafeToRetry(requestMessage.Method))
                        break;
                    }
                    string str = string.Format("GatewayStoreClient Request Timeout. Start Time UTC:{0}; Total Duration:{1} Ms; Request Timeout {2} Ms; Http Client Timeout:{3} Ms; Activity id: {4};", (object) startDateTimeUtc, (object) (DateTime.UtcNow - startDateTimeUtc).TotalMilliseconds, (object) timeSpan1.TotalMilliseconds, (object) this.httpClient.Timeout.TotalMilliseconds, (object) Trace.CorrelationManager.ActivityId);
                    ex1.Data.Add((object) "Message", (object) str);
                    throw;
                  }
                case WebException ex2:
                  if (!flag)
                  {
                    if (!timeoutPolicy.IsSafeToRetry(requestMessage.Method))
                    {
                      if (WebExceptionUtility.IsWebExceptionRetriable((Exception) ex2))
                        break;
                    }
                    else
                      break;
                  }
                  throw;
                case HttpRequestException _:
                  if (!flag)
                  {
                    if (timeoutPolicy.IsSafeToRetry(requestMessage.Method))
                      break;
                  }
                  throw;
                default:
                  throw;
              }
            }
          }
        }
        if (timeSpan2 != TimeSpan.Zero)
          await Task.Delay(timeSpan2);
        timeSpan1 = new TimeSpan();
        timeSpan2 = new TimeSpan();
      }
    }

    private static bool IsOutOfRetries(
      HttpTimeoutPolicy timeoutPolicy,
      DateTime startDateTimeUtc,
      IEnumerator<(TimeSpan requestTimeout, TimeSpan delayForNextRequest)> timeoutEnumerator)
    {
      return DateTime.UtcNow - startDateTimeUtc > timeoutPolicy.MaximumRetryTimeLimit || !timeoutEnumerator.MoveNext();
    }

    private async Task<HttpResponseMessage> ExecuteHttpHelperAsync(
      HttpRequestMessage requestMessage,
      ResourceType resourceType,
      CancellationToken cancellationToken)
    {
      DateTime sendTimeUtc = DateTime.UtcNow;
      Guid localGuid = Guid.NewGuid();
      this.eventSource.Request(Trace.CorrelationManager.ActivityId, localGuid, requestMessage.RequestUri.ToString(), resourceType.ToResourceTypeString(), requestMessage.Headers);
      HttpResponseMessage httpResponseMessage = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      if (httpResponseMessage.RequestMessage == null)
        httpResponseMessage.RequestMessage = requestMessage;
      TimeSpan timeSpan = DateTime.UtcNow - sendTimeUtc;
      Guid activityId = Guid.Empty;
      IEnumerable<string> values;
      if (httpResponseMessage.Headers.TryGetValues("x-ms-activity-id", out values) && values.Any<string>())
        activityId = new Guid(values.First<string>());
      this.eventSource.Response(activityId, localGuid, (short) httpResponseMessage.StatusCode, timeSpan.TotalMilliseconds, httpResponseMessage.Headers);
      return httpResponseMessage;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
        this.httpClient.Dispose();
      this.disposedValue = true;
    }

    public override void Dispose() => this.Dispose(true);

    private class HttpRequestMessageHandler : DelegatingHandler
    {
      private readonly EventHandler<SendingRequestEventArgs> sendingRequest;
      private readonly EventHandler<ReceivedResponseEventArgs> receivedResponse;

      public HttpRequestMessageHandler(
        EventHandler<SendingRequestEventArgs> sendingRequest,
        EventHandler<ReceivedResponseEventArgs> receivedResponse,
        HttpMessageHandler innerHandler)
      {
        this.sendingRequest = sendingRequest;
        this.receivedResponse = receivedResponse;
        this.InnerHandler = innerHandler ?? throw new ArgumentNullException("innerHandler is null. This required for .NET core to limit the http connection. See CreateHttpClientHandler ");
      }

      protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
      {
        CosmosHttpClientCore.HttpRequestMessageHandler sender = this;
        EventHandler<SendingRequestEventArgs> sendingRequest = sender.sendingRequest;
        if (sendingRequest != null)
          sendingRequest((object) sender, new SendingRequestEventArgs(request));
        // ISSUE: reference to a compiler-generated method
        HttpResponseMessage response = await sender.\u003C\u003En__0(request, cancellationToken);
        EventHandler<ReceivedResponseEventArgs> receivedResponse = sender.receivedResponse;
        if (receivedResponse != null)
          receivedResponse((object) sender, new ReceivedResponseEventArgs(request, response));
        return response;
      }
    }
  }
}
