// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GatewayStoreClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal class GatewayStoreClient : TransportClient
  {
    private HttpClient httpClient;
    private readonly ICommunicationEventSource eventSource;
    private JsonSerializerSettings SerializerSettings;

    public GatewayStoreClient(
      HttpClient httpClient,
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

    public static bool IsFeedRequest(OperationType requestOperationType) => requestOperationType == OperationType.Create || requestOperationType == OperationType.Upsert || requestOperationType == OperationType.ReadFeed || requestOperationType == OperationType.Query || requestOperationType == OperationType.SqlQuery || requestOperationType == OperationType.QueryPlan || requestOperationType == OperationType.ServiceReservation;

    internal override async Task<StoreResponse> InvokeStoreAsync(
      Uri baseAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      Uri physicalAddress = GatewayStoreClient.IsFeedRequest(request.OperationType) ? HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, baseAddress, request) : HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, baseAddress, request);
      StoreResponse storeResponse;
      using (HttpResponseMessage responseMessage = await this.InvokeClientAsync(request, resourceOperation.resourceType, physicalAddress, new CancellationToken()))
        storeResponse = await HttpTransportClient.ProcessHttpResponse(request.ResourceAddress, string.Empty, responseMessage, physicalAddress, request);
      return storeResponse;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    internal Task<HttpResponseMessage> SendHttpAsync(
      HttpRequestMessage requestMessage,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.httpClient.SendHttpAsync(requestMessage, cancellationToken);
    }

    internal static async Task<DocumentServiceResponse> ParseResponseAsync(
      HttpResponseMessage responseMessage,
      JsonSerializerSettings serializerSettings = null,
      DocumentServiceRequest request = null)
    {
      using (responseMessage)
      {
        if (responseMessage.StatusCode < HttpStatusCode.BadRequest)
        {
          MemoryStream bufferedStream = new MemoryStream();
          await responseMessage.Content.CopyToAsync((Stream) bufferedStream);
          bufferedStream.Position = 0L;
          return new DocumentServiceResponse((Stream) bufferedStream, GatewayStoreClient.ExtractResponseHeaders(responseMessage), responseMessage.StatusCode, serializerSettings);
        }
        if (request != null && request.IsValidStatusCodeForExceptionlessRetry((int) responseMessage.StatusCode))
          return new DocumentServiceResponse((Stream) null, GatewayStoreClient.ExtractResponseHeaders(responseMessage), responseMessage.StatusCode, serializerSettings);
        throw await GatewayStoreClient.CreateDocumentClientException(responseMessage);
      }
    }

    internal static INameValueCollection ExtractResponseHeaders(HttpResponseMessage responseMessage)
    {
      INameValueCollection responseHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      foreach (KeyValuePair<string, IEnumerable<string>> header in (System.Net.Http.Headers.HttpHeaders) responseMessage.Headers)
      {
        if (string.Compare(header.Key, "x-ms-alt-content-path", StringComparison.Ordinal) == 0)
        {
          foreach (string stringToUnescape in header.Value)
            responseHeaders.Add(header.Key, Uri.UnescapeDataString(stringToUnescape));
        }
        else
        {
          foreach (string str in header.Value)
            responseHeaders.Add(header.Key, str);
        }
      }
      if (responseMessage.Content != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (System.Net.Http.Headers.HttpHeaders) responseMessage.Content.Headers)
        {
          if (string.Compare(header.Key, "x-ms-alt-content-path", StringComparison.Ordinal) == 0)
          {
            foreach (string stringToUnescape in header.Value)
              responseHeaders.Add(header.Key, Uri.UnescapeDataString(stringToUnescape));
          }
          else
          {
            foreach (string str in header.Value)
              responseHeaders.Add(header.Key, str);
          }
        }
      }
      return responseHeaders;
    }

    internal static async Task<DocumentClientException> CreateDocumentClientException(
      HttpResponseMessage responseMessage)
    {
      Trace.CorrelationManager.ActivityId = Guid.Empty;
      bool isNameBased = false;
      bool isFeed = false;
      string resourceIdOrFullName;
      PathsHelper.TryParsePathSegments(responseMessage.RequestMessage.RequestUri.LocalPath, out isFeed, out string _, out resourceIdOrFullName, out isNameBased);
      if (string.Equals(responseMessage.Content?.Headers?.ContentType?.MediaType, "application/json", StringComparison.OrdinalIgnoreCase))
        return new DocumentClientException(JsonSerializable.LoadFrom<Error>(await responseMessage.Content.ReadAsStreamAsync()), responseMessage.Headers, new HttpStatusCode?(responseMessage.StatusCode))
        {
          StatusDescription = responseMessage.ReasonPhrase,
          ResourceAddress = resourceIdOrFullName
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
      return new DocumentClientException(context.ToString(), (Exception) null, responseMessage.Headers, new HttpStatusCode?(responseMessage.StatusCode), responseMessage.RequestMessage.RequestUri)
      {
        StatusDescription = responseMessage.ReasonPhrase,
        ResourceAddress = resourceIdOrFullName
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

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    private async Task<HttpRequestMessage> PrepareRequestMessageAsync(
      DocumentServiceRequest request,
      Uri physicalAddress)
    {
      HttpMethod head = HttpMethod.Head;
      HttpMethod method;
      if (request.OperationType == OperationType.Create || request.OperationType == OperationType.Upsert || request.OperationType == OperationType.Query || request.OperationType == OperationType.SqlQuery || request.OperationType == OperationType.ExecuteJavaScript || request.OperationType == OperationType.QueryPlan || request.OperationType == OperationType.ServiceReservation)
        method = HttpMethod.Post;
      else if (request.OperationType == OperationType.Read || request.OperationType == OperationType.ReadFeed)
        method = HttpMethod.Get;
      else if (request.OperationType == OperationType.Replace)
      {
        method = HttpMethod.Put;
      }
      else
      {
        if (request.OperationType != OperationType.Delete)
          throw new NotImplementedException();
        method = HttpMethod.Delete;
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
      return requestMessage;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    private async Task<HttpResponseMessage> InvokeClientAsync(
      DocumentServiceRequest request,
      ResourceType resourceType,
      Uri physicalAddress,
      CancellationToken cancellationToken)
    {
      return await BackoffRetryUtility<HttpResponseMessage>.ExecuteAsync((Func<Task<HttpResponseMessage>>) (async () =>
      {
        HttpResponseMessage httpResponseMessage1;
        using (HttpRequestMessage requestMessage = await this.PrepareRequestMessageAsync(request, physicalAddress))
        {
          DateTime sendTimeUtc = DateTime.UtcNow;
          Guid localGuid = Guid.NewGuid();
          this.eventSource.Request(Guid.Empty, localGuid, requestMessage.RequestUri.ToString(), resourceType.ToResourceTypeString(), requestMessage.Headers);
          try
          {
            HttpResponseMessage httpResponseMessage2 = await this.httpClient.SendAsync(requestMessage, cancellationToken);
            double totalMilliseconds = (DateTime.UtcNow - sendTimeUtc).TotalMilliseconds;
            Guid activityId = Guid.Empty;
            IEnumerable<string> values;
            if (httpResponseMessage2.Headers.TryGetValues("x-ms-activity-id", out values) && values.Count<string>() != 0)
              activityId = new Guid(values.First<string>());
            this.eventSource.Response(activityId, localGuid, (short) httpResponseMessage2.StatusCode, totalMilliseconds, httpResponseMessage2.Headers);
            httpResponseMessage1 = httpResponseMessage2;
          }
          catch (TaskCanceledException ex)
          {
            if (!cancellationToken.IsCancellationRequested)
              throw new RequestTimeoutException((Exception) ex, requestMessage.RequestUri);
            throw;
          }
        }
        return httpResponseMessage1;
      }), (IRetryPolicy) new WebExceptionRetryPolicy(), cancellationToken);
    }
  }
}
