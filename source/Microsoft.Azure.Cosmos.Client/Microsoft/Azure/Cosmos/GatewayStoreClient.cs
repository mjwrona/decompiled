// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.GatewayStoreClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class GatewayStoreClient : TransportClient
  {
    private readonly ICommunicationEventSource eventSource;
    private readonly CosmosHttpClient httpClient;
    private readonly JsonSerializerSettings SerializerSettings;
    private static readonly HttpMethod httpPatchMethod = new HttpMethod("PATCH");

    public GatewayStoreClient(
      CosmosHttpClient httpClient,
      ICommunicationEventSource eventSource,
      JsonSerializerSettings serializerSettings = null)
    {
      this.httpClient = httpClient;
      this.SerializerSettings = serializerSettings;
      this.eventSource = eventSource;
    }

    public async Task<DocumentServiceResponse> InvokeAsync(
      DocumentServiceRequest request,
      ResourceType resourceType,
      Uri physicalAddress,
      CancellationToken cancellationToken)
    {
      DocumentServiceResponse responseAsync;
      using (HttpResponseMessage responseMessage = await this.InvokeClientAsync(request, resourceType, physicalAddress, cancellationToken))
        responseAsync = await GatewayStoreClient.ParseResponseAsync(responseMessage, request.SerializerSettings ?? this.SerializerSettings, request);
      return responseAsync;
    }

    public static bool IsFeedRequest(OperationType requestOperationType) => requestOperationType == OperationType.Create || requestOperationType == OperationType.Upsert || requestOperationType == OperationType.ReadFeed || requestOperationType == OperationType.Query || requestOperationType == OperationType.SqlQuery || requestOperationType == OperationType.QueryPlan || requestOperationType == OperationType.Batch;

    internal override async Task<StoreResponse> InvokeStoreAsync(
      Uri baseAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      Uri physicalAddress = GatewayStoreClient.IsFeedRequest(request.OperationType) ? HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, baseAddress, request) : HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, baseAddress, request);
      StoreResponse storeResponse;
      using (HttpResponseMessage responseMessage = await this.InvokeClientAsync(request, resourceOperation.resourceType, physicalAddress, new CancellationToken()))
        storeResponse = await HttpTransportClient.ProcessHttpResponse(request.ResourceAddress, string.Empty, responseMessage, physicalAddress, request);
      physicalAddress = (Uri) null;
      return storeResponse;
    }

    internal Task<HttpResponseMessage> SendHttpAsync(
      Func<ValueTask<HttpRequestMessage>> requestMessage,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.httpClient.SendHttpAsync(requestMessage, resourceType, timeoutPolicy, clientSideRequestStatistics, cancellationToken);
    }

    internal static async Task<DocumentServiceResponse> ParseResponseAsync(
      HttpResponseMessage responseMessage,
      JsonSerializerSettings serializerSettings = null,
      DocumentServiceRequest request = null)
    {
      using (responseMessage)
      {
        IClientSideRequestStatistics requestStatistics = request?.RequestContext?.ClientRequestStatistics;
        INameValueCollection headers;
        if (responseMessage.StatusCode < HttpStatusCode.BadRequest)
        {
          headers = GatewayStoreClient.ExtractResponseHeaders(responseMessage);
          return new DocumentServiceResponse(await GatewayStoreClient.BufferContentIfAvailableAsync(responseMessage), headers, responseMessage.StatusCode, requestStatistics, serializerSettings);
        }
        if (request == null || !request.IsValidStatusCodeForExceptionlessRetry((int) responseMessage.StatusCode))
          throw await GatewayStoreClient.CreateDocumentClientExceptionAsync(responseMessage, requestStatistics);
        headers = GatewayStoreClient.ExtractResponseHeaders(responseMessage);
        return new DocumentServiceResponse(await GatewayStoreClient.BufferContentIfAvailableAsync(responseMessage), headers, responseMessage.StatusCode, requestStatistics, serializerSettings);
      }
    }

    internal static INameValueCollection ExtractResponseHeaders(HttpResponseMessage responseMessage) => (INameValueCollection) new HttpResponseHeadersWrapper(responseMessage.Headers, responseMessage.Content?.Headers);

    internal static async Task<DocumentClientException> CreateDocumentClientExceptionAsync(
      HttpResponseMessage responseMessage,
      IClientSideRequestStatistics requestStatistics)
    {
      bool isNameBased = false;
      bool isFeed = false;
      string resourceIdOrFullName;
      PathsHelper.TryParsePathSegments(responseMessage.RequestMessage.RequestUri.LocalPath, out isFeed, out string _, out resourceIdOrFullName, out isNameBased);
      if (string.Equals(responseMessage.Content?.Headers?.ContentType?.MediaType, "application/json", StringComparison.OrdinalIgnoreCase))
        return new DocumentClientException(JsonSerializable.LoadFrom<Error>(await responseMessage.Content.ReadAsStreamAsync()), responseMessage.Headers, new HttpStatusCode?(responseMessage.StatusCode))
        {
          StatusDescription = responseMessage.ReasonPhrase,
          ResourceAddress = resourceIdOrFullName,
          RequestStatistics = requestStatistics
        };
      StringBuilder context = new StringBuilder();
      StringBuilder stringBuilder = context;
      stringBuilder.AppendLine(await responseMessage.Content.ReadAsStringAsync());
      stringBuilder = (StringBuilder) null;
      HttpRequestMessage requestMessage = responseMessage.RequestMessage;
      if (requestMessage != null)
      {
        context.AppendLine("RequestUri: " + requestMessage.RequestUri.ToString() + ";");
        context.AppendLine("RequestMethod: " + requestMessage.Method.Method + ";");
        if (requestMessage.Headers != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> header in (System.Net.Http.Headers.HttpHeaders) requestMessage.Headers)
            context.AppendLine(string.Format("Header: {0} Length: {1};", (object) header.Key, (object) string.Join(",", header.Value).Length));
        }
      }
      string str = await responseMessage.Content.ReadAsStringAsync();
      return new DocumentClientException(context.ToString(), (Exception) null, responseMessage.Headers, new HttpStatusCode?(responseMessage.StatusCode), responseMessage.RequestMessage.RequestUri)
      {
        StatusDescription = responseMessage.ReasonPhrase,
        ResourceAddress = resourceIdOrFullName,
        RequestStatistics = requestStatistics
      };
    }

    internal static bool IsAllowedRequestHeader(string headerName)
    {
      if (headerName.StartsWith("x-ms", StringComparison.OrdinalIgnoreCase))
        return true;
      switch (headerName)
      {
        case "A-IM":
        case "Accept":
        case "Content-Type":
        case "Host":
        case "If-Match":
        case "If-Modified-Since":
        case "If-None-Match":
        case "If-Range":
        case "If-Unmodified-Since":
        case "Prefer":
        case "User-Agent":
        case "authorization":
        case "x-ms-documentdb-query":
          return true;
        default:
          return false;
      }
    }

    private static async Task<Stream> BufferContentIfAvailableAsync(
      HttpResponseMessage responseMessage)
    {
      if (responseMessage.Content == null)
        return (Stream) null;
      MemoryStream bufferedStream = new MemoryStream();
      await responseMessage.Content.CopyToAsync((Stream) bufferedStream);
      bufferedStream.Position = 0L;
      return (Stream) bufferedStream;
    }

    private async ValueTask<HttpRequestMessage> PrepareRequestMessageAsync(
      DocumentServiceRequest request,
      Uri physicalAddress)
    {
      HttpMethod head = HttpMethod.Head;
      HttpMethod method;
      if (request.OperationType == OperationType.Create || request.OperationType == OperationType.Upsert || request.OperationType == OperationType.Query || request.OperationType == OperationType.SqlQuery || request.OperationType == OperationType.Batch || request.OperationType == OperationType.ExecuteJavaScript || request.OperationType == OperationType.QueryPlan || request.ResourceType == ResourceType.PartitionKey && request.OperationType == OperationType.Delete)
        method = HttpMethod.Post;
      else if (ChangeFeedHelper.IsChangeFeedWithQueryRequest(request.OperationType, request.Body != null))
        method = HttpMethod.Post;
      else if (request.OperationType == OperationType.Read || request.OperationType == OperationType.ReadFeed)
        method = HttpMethod.Get;
      else if (request.OperationType == OperationType.Replace || request.OperationType == OperationType.CollectionTruncate)
        method = HttpMethod.Put;
      else if (request.OperationType == OperationType.Delete)
      {
        method = HttpMethod.Delete;
      }
      else
      {
        if (request.OperationType != OperationType.Patch)
          throw new NotImplementedException();
        method = GatewayStoreClient.httpPatchMethod;
      }
      HttpRequestMessage requestMessage = new HttpRequestMessage(method, physicalAddress);
      if (request.Body != null)
      {
        await request.EnsureBufferedBodyAsync();
        MemoryStream memoryStream = new MemoryStream();
        request.CloneableBody.WriteTo((Stream) memoryStream);
        memoryStream.Position = 0L;
        requestMessage.Content = (HttpContent) new StreamContent((Stream) memoryStream);
      }
      if (request.Headers != null)
      {
        foreach (string header in (IEnumerable) request.Headers)
        {
          if (GatewayStoreClient.IsAllowedRequestHeader(header))
          {
            if (header.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
              requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(request.Headers[header]);
            else
              requestMessage.Headers.TryAddWithoutValidation(header, request.Headers[header]);
          }
        }
      }
      requestMessage.Headers.Add("x-ms-activity-id", Trace.CorrelationManager.ActivityId.ToString());
      string regionName = request?.RequestContext?.RegionName;
      if (regionName != null)
        requestMessage.Properties.Add(ClientSideRequestStatisticsTraceDatum.HttpRequestRegionNameProperty, (object) regionName);
      HttpRequestMessage httpRequestMessage = requestMessage;
      requestMessage = (HttpRequestMessage) null;
      return httpRequestMessage;
    }

    private Task<HttpResponseMessage> InvokeClientAsync(
      DocumentServiceRequest request,
      ResourceType resourceType,
      Uri physicalAddress,
      CancellationToken cancellationToken)
    {
      return this.httpClient.SendHttpAsync((Func<ValueTask<HttpRequestMessage>>) (() => this.PrepareRequestMessageAsync(request, physicalAddress)), resourceType, HttpTimeoutPolicy.GetTimeoutPolicy(request), request.RequestContext.ClientRequestStatistics, cancellationToken);
    }
  }
}
