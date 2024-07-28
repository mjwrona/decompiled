// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GatewayStoreModel
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal class GatewayStoreModel : IStoreModel, IDisposable
  {
    private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(65.0);
    private readonly GlobalEndpointManager endpointManager;
    private readonly DocumentClientEventSource eventSource;
    private readonly ISessionContainer sessionContainer;
    private readonly ConsistencyLevel defaultConsistencyLevel;
    private GatewayStoreClient gatewayStoreClient;
    private CookieContainer cookieJar;

    public GatewayStoreModel(
      GlobalEndpointManager endpointManager,
      ISessionContainer sessionContainer,
      TimeSpan requestTimeout,
      ConsistencyLevel defaultConsistencyLevel,
      DocumentClientEventSource eventSource,
      JsonSerializerSettings SerializerSettings,
      UserAgentContainer userAgent,
      ApiType apiType = ApiType.None,
      HttpMessageHandler messageHandler = null)
    {
      this.cookieJar = new CookieContainer();
      this.endpointManager = endpointManager;
      HttpMessageHandler handler = messageHandler;
      if (handler == null)
        handler = (HttpMessageHandler) new HttpClientHandler()
        {
          CookieContainer = this.cookieJar
        };
      HttpClient httpClient = new HttpClient(handler);
      this.sessionContainer = sessionContainer;
      this.defaultConsistencyLevel = defaultConsistencyLevel;
      httpClient.Timeout = requestTimeout > this.requestTimeout ? requestTimeout : this.requestTimeout;
      httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
      {
        NoCache = true
      };
      httpClient.AddUserAgentHeader(userAgent);
      httpClient.AddApiTypeHeader(apiType);
      httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
      this.eventSource = eventSource;
      this.gatewayStoreClient = new GatewayStoreClient(httpClient, (ICommunicationEventSource) this.eventSource, SerializerSettings);
    }

    public virtual async Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.ApplySessionToken(request);
      DocumentServiceResponse documentServiceResponse;
      try
      {
        Uri physicalAddress = GatewayStoreClient.IsFeedRequest(request.OperationType) ? this.GetFeedUri(request) : this.GetEntityUri(request);
        documentServiceResponse = await this.gatewayStoreClient.InvokeAsync(request, request.ResourceType, physicalAddress, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        if (!ReplicatedResourceClient.IsMasterResource(request.ResourceType))
        {
          HttpStatusCode? statusCode = ex.StatusCode;
          HttpStatusCode httpStatusCode1 = HttpStatusCode.PreconditionFailed;
          if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
          {
            statusCode = ex.StatusCode;
            HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
            if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
            {
              statusCode = ex.StatusCode;
              HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
              if (!(statusCode.GetValueOrDefault() == httpStatusCode3 & statusCode.HasValue) || ex.GetSubStatus() == SubStatusCodes.PartitionKeyRangeGone)
                goto label_8;
            }
          }
          this.CaptureSessionToken(request, ex.Headers);
        }
label_8:
        throw;
      }
      this.CaptureSessionToken(request, documentServiceResponse.Headers);
      return documentServiceResponse;
    }

    public virtual async Task<DatabaseAccount> GetDatabaseAccountAsync(
      HttpRequestMessage requestMessage,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DatabaseAccount databaseAccountAsync = (DatabaseAccount) null;
      using (HttpResponseMessage responseMessage = await this.gatewayStoreClient.SendHttpAsync(requestMessage, cancellationToken))
      {
        using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(responseMessage))
          databaseAccountAsync = responseAsync.GetInternalResource<DatabaseAccount>(new Func<DatabaseAccount>(DatabaseAccount.CreateNewInstance));
        IEnumerable<string> values;
        long result;
        if (responseMessage.Headers.TryGetValues("x-ms-max-media-storage-usage-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.MaxMediaStorageUsageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-media-storage-usage-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.MediaStorageUsageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-consumed-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.ConsumedDocumentStorageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-provisioned-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.ProvisionedDocumentStorageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-reserved-mb", out values))
        {
          if (values.Count<string>() != 0)
          {
            if (long.TryParse(values.First<string>(), out result))
              databaseAccountAsync.ReservedDocumentStorageInMB = result;
          }
        }
      }
      return databaseAccountAsync;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void CaptureSessionToken(
      DocumentServiceRequest request,
      INameValueCollection responseHeaders)
    {
      if (request.ResourceType == ResourceType.Collection && request.OperationType == OperationType.Delete)
        this.sessionContainer.ClearTokenByResourceId(!request.IsNameBased ? request.ResourceId : responseHeaders["x-ms-content-path"]);
      else
        this.sessionContainer.SetSessionToken(request, responseHeaders);
    }

    private void ApplySessionToken(DocumentServiceRequest request)
    {
      if (request.Headers != null && !string.IsNullOrEmpty(request.Headers["x-ms-session-token"]))
      {
        if (!ReplicatedResourceClient.IsMasterResource(request.ResourceType))
          return;
        request.Headers.Remove("x-ms-session-token");
      }
      else
      {
        if (request.Headers != null && request.OperationType == OperationType.QueryPlan)
        {
          string header = request.Headers["x-ms-cosmos-is-query-plan-request"];
          bool flag = false;
          ref bool local = ref flag;
          if (bool.TryParse(header, out local) & flag)
            return;
        }
        string header1 = request.Headers["x-ms-consistency-level"];
        if ((this.defaultConsistencyLevel == ConsistencyLevel.Session ? 1 : (string.IsNullOrEmpty(header1) ? 0 : (string.Equals(header1, ConsistencyLevel.Session.ToString(), StringComparison.OrdinalIgnoreCase) ? 1 : 0))) == 0 || ReplicatedResourceClient.IsMasterResource(request.ResourceType))
          return;
        string str = this.sessionContainer.ResolveGlobalSessionToken(request);
        if (string.IsNullOrEmpty(str))
          return;
        request.Headers["x-ms-session-token"] = str;
      }
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.gatewayStoreClient == null)
        return;
      try
      {
        this.gatewayStoreClient.Dispose();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Exception {0} thrown during dispose of HttpClient, this could happen if there are inflight request during the dispose of client", (object) ex);
      }
      this.gatewayStoreClient = (GatewayStoreClient) null;
    }

    private Uri GetEntityUri(DocumentServiceRequest entity)
    {
      string header = entity.Headers["Content-Location"];
      return !string.IsNullOrEmpty(header) ? new Uri(this.endpointManager.ResolveServiceEndpoint(entity), new Uri(header).AbsolutePath) : new Uri(this.endpointManager.ResolveServiceEndpoint(entity), PathsHelper.GeneratePath(entity.ResourceType, entity, false));
    }

    private Uri GetFeedUri(DocumentServiceRequest request) => new Uri(this.endpointManager.ResolveServiceEndpoint(request), PathsHelper.GeneratePath(request.ResourceType, request, true));
  }
}
