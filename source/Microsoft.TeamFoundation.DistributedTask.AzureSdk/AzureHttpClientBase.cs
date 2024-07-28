// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.AzureHttpClientBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk
{
  public abstract class AzureHttpClientBase : HttpClient
  {
    private JsonSerializerSettings m_deserializationSettings;
    private readonly MediaTypeFormatter m_formatter;
    private readonly DateTime m_createdTime;
    private bool m_expired;
    private TimeSpan m_timeToLiveInCache;
    private bool m_ValidateHttpClientQueryParams;
    private const string c_subscriptionPathString = "subscriptions";
    private const string c_resourceGroupPathString = "resourceGroups";
    private const string c_apiVersion = "api-version";
    private static readonly TimeSpan s_defaultTimeToLiveInCache = TimeSpan.FromMinutes(30.0);
    private static readonly Lazy<HttpMethod> s_patchMethod = new Lazy<HttpMethod>((Func<HttpMethod>) (() => new HttpMethod("PATCH")));

    protected AzureHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler,
      TimeSpan timeToLiveInCache,
      bool validateHttpClientQueryParams)
      : base(pipeline, disposeHandler)
    {
      this.m_formatter = (MediaTypeFormatter) new VssJsonMediaTypeFormatter();
      this.m_createdTime = DateTime.UtcNow;
      this.m_timeToLiveInCache = timeToLiveInCache > TimeSpan.Zero ? timeToLiveInCache : AzureHttpClientBase.s_defaultTimeToLiveInCache;
      this.m_ValidateHttpClientQueryParams = validateHttpClientQueryParams;
      this.Timeout = TimeSpan.FromMilliseconds(-1.0);
      this.BaseAddress = baseUrl;
    }

    public static Uri DefaultManagementUrl { get; } = new Uri("https://management.azure.com/");

    public static Uri DefaultAuthenticationUrl { get; } = new Uri("https://management.azure.com/");

    public bool IsExpired
    {
      get
      {
        if (!this.m_expired)
          this.CheckExpiration();
        return this.m_expired;
      }
    }

    public int? LongRunningOperationRetryTimeout { get; set; }

    protected abstract string ApiVersion { get; set; }

    protected static IDictionary<string, object> BuildRequestParameters(
      params (string key, object value)[] parameters)
    {
      return (IDictionary<string, object>) ((IEnumerable<(string, object)>) parameters).ToDictionary<(string, object), string, object>((Func<(string, object), string>) (t => t.key), (Func<(string, object), object>) (t => t.value));
    }

    protected async Task<Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse> DeleteAsync(
      string url,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ConfiguredTaskAwaitable<Microsoft.Rest.Azure.AzureOperationResponse> configuredTaskAwaitable = this.BeginDeleteAsync(url, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse from;
      using (Microsoft.Rest.Azure.AzureOperationResponse azureOperationResponse1 = await configuredTaskAwaitable)
      {
        configuredTaskAwaitable = this.GetPostOrDeleteOperationResultAsync(activityId, invocationId, azureOperationResponse1, cancellationToken).ConfigureAwait(false);
        using (Microsoft.Rest.Azure.AzureOperationResponse azureOperationResponse2 = await configuredTaskAwaitable)
          from = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse.CreateFrom(azureOperationResponse2);
      }
      return from;
    }

    protected async Task<Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse> DeleteWithTracingAsync(
      string url,
      Guid activityId,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken = default (CancellationToken),
      [CallerMemberName] string method = null)
    {
      return await this.TraceScope<Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse>(activityId, method, parameters, (Func<string, Task<Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse>>) (invocationId => this.DeleteAsync(url, activityId, invocationId, cancellationToken))).ConfigureAwait(false);
    }

    protected async Task<TResult> GetAsync<TResult>(
      string url,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      TResult async;
      using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        async = await azureHttpClientBase.ReadContentAsAsync<TResult>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
      return async;
    }

    protected async Task<TResult> GetWithTracingAsync<TResult>(
      string url,
      Guid activityId,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken = default (CancellationToken),
      [CallerMemberName] string method = null)
    {
      return await this.TraceScope<TResult>(activityId, method, parameters, (Func<string, Task<TResult>>) (invocationId => this.GetAsync<TResult>(url, activityId, invocationId, cancellationToken))).ConfigureAwait(false);
    }

    protected async Task<IEnumerable<TResult>> GetPaginatedAsync<TResult>(
      string url,
      string expandParameters,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      if (url == null)
        throw new ArgumentNullException(nameof (url));
      if (!string.IsNullOrEmpty(expandParameters))
        url += expandParameters;
      List<TResult> results = new List<TResult>();
      do
      {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
        using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
        {
          AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
          (IEnumerable<TResult> collection, string str) = await azureHttpClientBase.ReadPossiblyPaginatedContentAsAsync<IEnumerable<TResult>>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
          results.AddRange(collection);
          url = str;
        }
        requestMessage = (HttpRequestMessage) null;
      }
      while (url != null);
      IEnumerable<TResult> paginatedAsync = (IEnumerable<TResult>) results;
      results = (List<TResult>) null;
      return paginatedAsync;
    }

    protected async Task<IEnumerable<TResult>> GetPaginatedWithTracingAsync<TResult>(
      string url,
      string expandParameters,
      Guid activityId,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken = default (CancellationToken),
      [CallerMemberName] string method = null)
    {
      return await this.TraceScope<IEnumerable<TResult>>(activityId, method, parameters, (Func<string, Task<IEnumerable<TResult>>>) (invocationId => this.GetPaginatedAsync<TResult>(url, expandParameters, activityId, invocationId, cancellationToken))).ConfigureAwait(false);
    }

    protected async Task<TResult> PatchAsync<T, TResult>(
      string url,
      T value,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      ObjectContent<T> objectContent = new ObjectContent<T>(value, azureHttpClientBase.m_formatter);
      HttpRequestMessage requestMessage = new HttpRequestMessage(AzureHttpClientBase.s_patchMethod.Value, url)
      {
        Content = (HttpContent) objectContent
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      TResult result;
      using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        result = await azureHttpClientBase.ReadContentAsAsync<TResult>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
      return result;
    }

    protected async Task<TResult> PatchWithTracingAsync<T, TResult>(
      string url,
      T value,
      Guid activityId,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken = default (CancellationToken),
      [CallerMemberName] string method = null)
    {
      return await this.TraceScope<TResult>(activityId, method, parameters, (Func<string, Task<TResult>>) (invocationId => this.PatchAsync<T, TResult>(url, value, activityId, invocationId, cancellationToken))).ConfigureAwait(false);
    }

    protected async Task<TResult> PostAsync<T, TResult>(
      string url,
      T value,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase client = this;
      url = client.AddApiVersion(url);
      ObjectContent<T> objectContent = new ObjectContent<T>(value, client.m_formatter);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = (HttpContent) objectContent
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      TResult result;
      using (HttpResponseMessage response = await client.PostAsync<T>(url, value, client.m_formatter, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        result = await client.ReadContentAsAsync<TResult>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
      return result;
    }

    protected async Task PostAsync<T>(
      string url,
      T value,
      Guid activityId,
      string invocationId = null,
      string queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase client = this;
      url = queryParameters != null ? client.AddApiVersionWithQueryParameters(url, queryParameters) : client.AddApiVersion(url);
      ObjectContent<T> objectContent = new ObjectContent<T>(value, client.m_formatter);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = (HttpContent) objectContent
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      using (HttpResponseMessage response = await client.PostAsync<T>(url, value, client.m_formatter, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        await client.CheckResponseForError(requestMessage, response, activityId, invocationId).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
    }

    protected async Task<TResult> PostAsync<TResult>(
      string url,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = (HttpContent) new StringContent("")
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      TResult result;
      using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        result = await azureHttpClientBase.ReadContentAsAsync<TResult>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
      return result;
    }

    protected async Task PostAsync(
      string url,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = (HttpContent) new StringContent("")
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        await azureHttpClientBase.CheckResponseForError(requestMessage, response, activityId, invocationId).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
    }

    protected async Task<TResult> PutAsync<T, TResult>(
      string url,
      T value,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      ObjectContent<T> objectContent = new ObjectContent<T>(value, azureHttpClientBase.m_formatter);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
      {
        Content = (HttpContent) objectContent
      };
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      TResult result;
      using (HttpResponseMessage response = await azureHttpClientBase.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
      {
        AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
        result = await azureHttpClientBase.ReadContentAsAsync<TResult>(requestMessage, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      }
      requestMessage = (HttpRequestMessage) null;
      return result;
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(
      string url,
      T value,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureHttpClientBase azureHttpClientBase = this;
      url = azureHttpClientBase.AddApiVersion(url);
      ObjectContent<T> objectContent = new ObjectContent<T>(value, azureHttpClientBase.m_formatter);
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
      {
        Content = (HttpContent) objectContent
      };
      AzureClientTracing.SendRequest(activityId, invocationId, request);
      HttpResponseMessage response = await azureHttpClientBase.SendAsync(request, cancellationToken).ConfigureAwait(false);
      AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
      return response;
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse> BeginDeleteAsync(
      string url,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      url = this.AddApiVersion(url);
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
      AzureClientTracing.SendRequest(activityId, invocationId, requestMessage);
      // ISSUE: reference to a compiler-generated method
      HttpResponseMessage response = await this.\u003C\u003En__0(requestMessage, cancellationToken).ConfigureAwait(false);
      AzureClientTracing.ReceiveResponse(activityId, invocationId, response);
      cancellationToken.ThrowIfCancellationRequested();
      HttpStatusCode[] successCodes = new HttpStatusCode[3]
      {
        HttpStatusCode.Accepted,
        HttpStatusCode.NoContent,
        HttpStatusCode.OK
      };
      await this.CheckResponseForError(requestMessage, response, (IEnumerable<HttpStatusCode>) successCodes, activityId, invocationId).ConfigureAwait(false);
      Microsoft.Rest.Azure.AzureOperationResponse operationResponse1 = new Microsoft.Rest.Azure.AzureOperationResponse();
      operationResponse1.Request = requestMessage;
      operationResponse1.Response = response;
      Microsoft.Rest.Azure.AzureOperationResponse operationResponse2 = operationResponse1;
      if (response.Headers.Contains("x-ms-request-id"))
        operationResponse2.RequestId = response.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      Microsoft.Rest.Azure.AzureOperationResponse operationResponse3 = operationResponse2;
      requestMessage = (HttpRequestMessage) null;
      response = (HttpResponseMessage) null;
      return operationResponse3;
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse> GetPostOrDeleteOperationResultAsync(
      Guid activityId,
      string invocationId,
      Microsoft.Rest.Azure.AzureOperationResponse response,
      CancellationToken cancellationToken)
    {
      return await this.GetLongRunningOperationResultAsync(activityId, invocationId, response, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse<TBody>> GetPutOrPatchOperationResultAsync<TBody>(
      Guid activityId,
      string invocationId,
      Microsoft.Rest.Azure.AzureOperationResponse<TBody> response,
      CancellationToken cancellationToken)
      where TBody : class
    {
      return await this.GetLongRunningOperationResultAsync<TBody>(activityId, invocationId, response, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse> GetLongRunningOperationResultAsync(
      Guid activityId,
      string invocationId,
      Microsoft.Rest.Azure.AzureOperationResponse response,
      CancellationToken cancellationToken)
    {
      Microsoft.Rest.Azure.AzureOperationResponse<object> operationResponse1 = new Microsoft.Rest.Azure.AzureOperationResponse<object>();
      operationResponse1.Request = response.Request;
      operationResponse1.Response = response.Response;
      operationResponse1.RequestId = response.RequestId;
      Microsoft.Rest.Azure.AzureOperationResponse<object> response1 = operationResponse1;
      Microsoft.Rest.Azure.AzureOperationResponse<object> operationResponse2 = await this.GetLongRunningOperationResultAsync<object>(activityId, invocationId, response1, cancellationToken).ConfigureAwait(false);
      Microsoft.Rest.Azure.AzureOperationResponse operationResultAsync = new Microsoft.Rest.Azure.AzureOperationResponse();
      operationResultAsync.Request = operationResponse2.Request;
      operationResultAsync.Response = operationResponse2.Response;
      operationResultAsync.RequestId = operationResponse2.RequestId;
      return operationResultAsync;
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse<TBody>> GetLongRunningOperationResultAsync<TBody>(
      Guid activityId,
      string invocationId,
      Microsoft.Rest.Azure.AzureOperationResponse<TBody> response,
      CancellationToken cancellationToken)
      where TBody : class
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, object> operationResponse1 = new Microsoft.Rest.Azure.AzureOperationResponse<TBody, object>();
      operationResponse1.Body = ((HttpOperationResponse<TBody>) response).Body;
      operationResponse1.Request = response.Request;
      operationResponse1.RequestId = response.RequestId;
      operationResponse1.Response = response.Response;
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, object> response1 = operationResponse1;
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, object> operationResponse2 = await this.GetLongRunningOperationResultAsync<TBody, object>(activityId, invocationId, response1, cancellationToken).ConfigureAwait(false);
      Microsoft.Rest.Azure.AzureOperationResponse<TBody> operationResultAsync = new Microsoft.Rest.Azure.AzureOperationResponse<TBody>();
      operationResultAsync.Body = operationResponse2.Body;
      operationResultAsync.Request = operationResponse2.Request;
      operationResultAsync.RequestId = operationResponse2.RequestId;
      operationResultAsync.Response = operationResponse2.Response;
      return operationResultAsync;
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader>> GetLongRunningOperationResultAsync<TBody, THeader>(
      Guid activityId,
      string invocationId,
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> response,
      CancellationToken cancellationToken)
      where TBody : class
      where THeader : class
    {
      HttpMethod initialRequestMethod = response.Request.Method;
      if (this.CheckResponseStatusCodeFailed<TBody, THeader>(response))
      {
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException(string.Format("Unexpected Operation Polling State: {0} ({1})", (object) response.Response.StatusCode, (object) initialRequestMethod));
        AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
        throw ex;
      }
      Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader> pollingState = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader>((HttpOperationResponse<TBody, THeader>) response, this.LongRunningOperationRetryTimeout);
      Uri requestUri = response.Request.RequestUri;
      while (!AzureAsyncOperation.TerminalStatuses.Any<string>((Func<string, bool>) (x => x.Equals(pollingState.Status, StringComparison.OrdinalIgnoreCase))))
      {
        AzureClientTracing.Information(activityId, invocationId, "Long Running Operation still running with poll status {0}, will sleep for {1}ms", (object) pollingState.Status, (object) pollingState.DelayInMilliseconds);
        await Task.Delay(pollingState.DelayInMilliseconds, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(pollingState.AzureAsyncOperationHeaderLink))
        {
          AzureClientTracing.Information(activityId, invocationId, "Updating State from AzureAsyncOperation Header (" + pollingState.AzureAsyncOperationHeaderLink + ")");
          await this.UpdateStateFromAzureAsyncOperationHeader<TBody, THeader>(activityId, invocationId, pollingState, cancellationToken).ConfigureAwait(false);
        }
        else if (!string.IsNullOrEmpty(pollingState.LocationHeaderLink))
        {
          AzureClientTracing.Information(activityId, invocationId, "Updating State from LocationHeaderLink (" + pollingState.LocationHeaderLink + ")");
          await this.UpdateStateFromLocationHeader<TBody, THeader>(activityId, invocationId, pollingState, initialRequestMethod, cancellationToken).ConfigureAwait(false);
        }
        else if (initialRequestMethod == HttpMethod.Put)
        {
          AzureClientTracing.Information(activityId, invocationId, string.Format("Updating State from Get Resource Operation ({0})", (object) requestUri));
          await this.UpdateStateFromGetResourceOperation<TBody, THeader>(activityId, invocationId, pollingState, requestUri, cancellationToken).ConfigureAwait(false);
        }
        else
        {
          Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("Location header is missing from long running operation.");
          AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
          throw ex;
        }
      }
      if ("Succeeded".Equals(pollingState.Status, StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(pollingState.AzureAsyncOperationHeaderLink) || (object) pollingState.Resource == null) && (initialRequestMethod == HttpMethod.Put || initialRequestMethod == new HttpMethod("PATCH")))
      {
        try
        {
          await this.UpdateStateFromGetResourceOperation<TBody, THeader>(activityId, invocationId, pollingState, requestUri, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          AzureClientTracing.Error(activityId, invocationId, ex);
          await Task.Delay(TimeSpan.FromSeconds(5.0), cancellationToken).ConfigureAwait(false);
          await this.UpdateStateFromGetResourceOperation<TBody, THeader>(activityId, invocationId, pollingState, requestUri, cancellationToken).ConfigureAwait(false);
        }
      }
      if (AzureAsyncOperation.FailedStatuses.Any<string>((Func<string, bool>) (x => x.Equals(pollingState.Status, StringComparison.OrdinalIgnoreCase))))
      {
        AzureClientTracing.Error(activityId, invocationId, (Exception) pollingState.CloudException);
        throw pollingState.CloudException;
      }
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> operationResponse = pollingState.AzureOperationResponse;
      initialRequestMethod = (HttpMethod) null;
      requestUri = (Uri) null;
      return operationResponse;
    }

    protected async Task UpdateStateFromAzureAsyncOperationHeader<TBody, THeader>(
      Guid activityId,
      string invocationId,
      Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader> pollingState,
      CancellationToken cancellationToken)
      where TBody : class
      where THeader : class
    {
      Microsoft.Rest.Azure.AzureOperationResponse<AzureAsyncOperation, object> operationResponse = await this.GetAsync<AzureAsyncOperation, object>(activityId, invocationId, pollingState.AzureAsyncOperationHeaderLink, cancellationToken).ConfigureAwait(false);
      AzureClientTracing.SendRequest(activityId, invocationId, operationResponse.Request);
      AzureClientTracing.ReceiveResponse(activityId, invocationId, operationResponse.Response);
      if (operationResponse.Body?.Status == null)
      {
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("AzureOperationResponse did not have a Body");
        AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
        throw ex;
      }
      pollingState.Status = operationResponse.Body.Status;
      AzureClientTracing.Information(activityId, invocationId, "UpdateStateFromAzureAsyncOperationHeader: AzureAsyncOperation.Status = " + pollingState.Status);
      pollingState.Error = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError.CreateFrom(operationResponse.Body.Error);
      pollingState.Response = operationResponse.Response;
      pollingState.Request = operationResponse.Request;
      pollingState.Resource = default (TBody);
      string json1 = await pollingState.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
      JObject json2 = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.HttpExtensions.ToJson(pollingState.Response.Headers);
      try
      {
        pollingState.Resource = JObject.Parse(json1).ToObject<TBody>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
        pollingState.ResourceHeaders = json2.ToObject<THeader>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
      }
      catch
      {
      }
    }

    protected async Task<PollingStateWrapper> UpdateStateFromAzureAsyncOperationHeader(
      Guid activityId,
      string invocationId,
      string azureAsyncOperationHeaderLink,
      CancellationToken cancellationToken)
    {
      Microsoft.Rest.Azure.AzureOperationResponse<AzureAsyncOperation, object> operation = await this.GetAsync<AzureAsyncOperation, object>(activityId, invocationId, azureAsyncOperationHeaderLink, cancellationToken).ConfigureAwait(false);
      AzureClientTracing.SendRequest(activityId, invocationId, operation.Request);
      AzureClientTracing.ReceiveResponse(activityId, invocationId, operation.Response);
      if (operation.Body?.Status == null)
      {
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("AzureOperationResponse did not have a Body");
        AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
        throw ex;
      }
      PollingStateWrapper pollingStateWrapper = new PollingStateWrapper();
      pollingStateWrapper.Error = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError.CreateFrom(operation.Body.Error);
      pollingStateWrapper.UpdateStateFrom(operation);
      return pollingStateWrapper;
    }

    protected async Task<Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader>> GetAsync<TBody, THeader>(
      Guid activityId,
      string invocationId,
      string operationUrl,
      CancellationToken cancellationToken)
      where TBody : class
      where THeader : class
    {
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = await this.GetRawAsync(activityId, invocationId, operationUrl, cancellationToken).ConfigureAwait(false);
      TBody body = default (TBody);
      if (operationResponse.Body != null)
        body = operationResponse.Body.ToObject<TBody>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> async = new Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader>();
      async.Request = operationResponse.Request;
      async.Response = operationResponse.Response;
      async.Body = body;
      async.Headers = operationResponse.Headers.ToObject<THeader>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
      return async;
    }

    private async Task<Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject>> GetRawAsync(
      Guid activityId,
      string invocationId,
      string operationUrl,
      CancellationToken cancellationToken)
    {
      AzureHttpClientBase azureHttpClientBase = this;
      string uriString = operationUrl.Replace(" ", "%20");
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri(uriString)
      };
      cancellationToken.ThrowIfCancellationRequested();
      AzureClientTracing.SendRequest(activityId, invocationId, httpRequestMessage);
      HttpResponseMessage httpResponseMessage = await azureHttpClientBase.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
      AzureClientTracing.ReceiveResponse(activityId, invocationId, httpResponseMessage);
      cancellationToken.ThrowIfCancellationRequested();
      string responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
      HttpStatusCode[] successCodes = new HttpStatusCode[4]
      {
        HttpStatusCode.OK,
        HttpStatusCode.Accepted,
        HttpStatusCode.Created,
        HttpStatusCode.NoContent
      };
      await azureHttpClientBase.CheckResponseForError(httpRequestMessage, httpResponseMessage, (IEnumerable<HttpStatusCode>) successCodes, activityId).ConfigureAwait(false);
      JObject jobject;
      if (!string.IsNullOrWhiteSpace(responseContent))
      {
        try
        {
          jobject = JObject.Parse(responseContent);
        }
        catch
        {
        }
      }
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = new Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject>();
      operationResponse.Request = httpRequestMessage;
      operationResponse.Response = httpResponseMessage;
      operationResponse.Body = jobject;
      operationResponse.Headers = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.HttpExtensions.ToJson(httpResponseMessage.Headers);
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> rawAsync = operationResponse;
      httpRequestMessage = (HttpRequestMessage) null;
      httpResponseMessage = (HttpResponseMessage) null;
      responseContent = (string) null;
      return rawAsync;
    }

    protected async Task UpdateStateFromLocationHeader<TBody, THeader>(
      Guid activityId,
      string invocationId,
      Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader> pollingState,
      HttpMethod method,
      CancellationToken cancellationToken)
      where TBody : class
      where THeader : class
    {
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = await this.GetRawAsync(activityId, invocationId, pollingState.LocationHeaderLink, cancellationToken).ConfigureAwait(false);
      pollingState.Response = operationResponse.Response;
      pollingState.Request = operationResponse.Request;
      HttpStatusCode statusCode = operationResponse.Response.StatusCode;
      JObject body1 = operationResponse.Body;
      if (statusCode == HttpStatusCode.Accepted)
      {
        pollingState.Status = "InProgress";
      }
      else
      {
        if (statusCode != HttpStatusCode.OK && (statusCode != HttpStatusCode.Created || !(method == HttpMethod.Put)) && (statusCode != HttpStatusCode.NoContent || !(method == HttpMethod.Delete) && !(method == HttpMethod.Post)))
          throw new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("The response from long running operation does not have a valid status code.");
        pollingState.Status = body1?["properties"]?[(object) "provisioningState"] != null ? (string) body1["properties"][(object) "provisioningState"] : "Succeeded";
        ((Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader>) pollingState).Error = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError()
        {
          Code = pollingState.Status,
          Message = "Long Running operation failed with " + pollingState.Status + ")"
        };
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader> pollingState1 = pollingState;
        JObject body2 = operationResponse.Body;
        TBody body3 = body2 != null ? body2.ToObject<TBody>(JsonSerializer.Create(this.CreateJsonDeserializationSettings())) : default (TBody);
        pollingState1.Resource = body3;
        pollingState.ResourceHeaders = operationResponse.Headers.ToObject<THeader>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
      }
    }

    protected async Task<PollingStateWrapper> UpdateStateFromLocationHeader(
      Guid activityId,
      string invocationId,
      string locationHeaderLink,
      HttpMethod method,
      CancellationToken cancellationToken)
    {
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = await this.GetRawAsync(activityId, invocationId, locationHeaderLink, cancellationToken).ConfigureAwait(false);
      PollingStateWrapper pollingStateWrapper = new PollingStateWrapper();
      pollingStateWrapper.UpdateFromResponseHeaders(operationResponse.Response);
      HttpStatusCode statusCode = operationResponse.Response.StatusCode;
      JObject body = operationResponse.Body;
      if (statusCode == HttpStatusCode.Accepted)
      {
        pollingStateWrapper.Status = "InProgress";
      }
      else
      {
        if (statusCode != HttpStatusCode.OK && (statusCode != HttpStatusCode.Created || !(method == HttpMethod.Put)) && (statusCode != HttpStatusCode.NoContent || !(method == HttpMethod.Delete) && !(method == HttpMethod.Post)))
          throw new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("The response from long running operation does not have a valid status code.");
        pollingStateWrapper.Status = body?["properties"]?[(object) "provisioningState"] != null ? (string) body["properties"][(object) "provisioningState"] : "Succeeded";
        pollingStateWrapper.Error = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError()
        {
          Code = pollingStateWrapper.Status,
          Message = "Long Running operation failed with " + pollingStateWrapper.Status + ")"
        };
        pollingStateWrapper.Resource = operationResponse.Body;
      }
      return pollingStateWrapper;
    }

    protected async Task UpdateStateFromGetResourceOperation<TBody, THeader>(
      Guid activityId,
      string invocationId,
      Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader> pollingState,
      Uri getOperationUri,
      CancellationToken cancellationToken)
      where TBody : class
      where THeader : class
    {
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = await this.GetRawAsync(activityId, invocationId, getOperationUri.AbsoluteUri, cancellationToken).ConfigureAwait(false);
      if (operationResponse.Body == null)
      {
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("AzureOperationResponse did not have a Body");
        AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
        throw ex;
      }
      JObject body = operationResponse.Body;
      pollingState.Status = body["properties"]?[(object) "provisioningState"] != null ? (string) body["properties"][(object) "provisioningState"] : "Succeeded";
      ((Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState<TBody, THeader>) pollingState).Error = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError()
      {
        Code = pollingState.Status,
        Message = "Long running operation failed with " + pollingState.Status
      };
      pollingState.Response = operationResponse.Response;
      pollingState.Request = operationResponse.Request;
      pollingState.Resource = operationResponse.Body.ToObject<TBody>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
      pollingState.ResourceHeaders = operationResponse.Headers.ToObject<THeader>(JsonSerializer.Create(this.CreateJsonDeserializationSettings()));
    }

    protected async Task UpdateStateFromGetResourceOperation(
      Guid activityId,
      string invocationId,
      PollingStateWrapper pollingState,
      Uri getOperationUri,
      CancellationToken cancellationToken)
    {
      Microsoft.Rest.Azure.AzureOperationResponse<JObject, JObject> operationResponse = await this.GetRawAsync(activityId, invocationId, getOperationUri.AbsoluteUri, cancellationToken).ConfigureAwait(false);
      if (operationResponse.Body == null)
      {
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException ex = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("AzureOperationResponse did not have a Body");
        AzureClientTracing.Error(activityId, invocationId, (Exception) ex);
        throw ex;
      }
      JObject body = operationResponse.Body;
      pollingState.Status = body["properties"]?[(object) "provisioningState"] != null ? (string) body["properties"][(object) "provisioningState"] : "Succeeded";
      pollingState.Error = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError()
      {
        Code = pollingState.Status,
        Message = "Long running operation failed with " + pollingState.Status
      };
      pollingState.UpdateFromResponseHeaders(operationResponse.Response);
      pollingState.Resource = operationResponse.Body;
    }

    private bool CheckResponseStatusCodeFailed<TBody, THeader>(
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> initialResponse)
    {
      HttpStatusCode statusCode = initialResponse.Response.StatusCode;
      HttpMethod method = initialResponse.Request.Method;
      if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Accepted || statusCode == HttpStatusCode.Created && method == HttpMethod.Put)
        return false;
      if (statusCode != HttpStatusCode.NoContent)
        return true;
      return method != HttpMethod.Delete && method != HttpMethod.Post;
    }

    protected async Task<(T result, string nextLink)> ReadPossiblyPaginatedContentAsAsync<T>(
      HttpRequestMessage request,
      HttpResponseMessage response,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.CheckResponseForError(request, response, activityId, invocationId).ConfigureAwait(false);
      if (!AzureHttpClientBase.IsJsonResponse(response))
        throw new UnsupportedMediaTypeException(string.Format("Unexpected response content type from Azure. Request Url: {0} {1} ", (object) request.Method.Method, (object) request.RequestUri) + string.Format("Response code: {0} {1} ", (object) response.StatusCode, (object) response.ReasonPhrase) + string.Format("Response content type: {0}", (object) response.Content.Headers.ContentType), response.Content.Headers.ContentType);
      if (typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()) && !typeof (byte[]).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()) && !typeof (JObject).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()))
      {
        AzureHttpClientBase.AzureJsonCollectionWrapper<T> collectionWrapper = await response.Content.ReadAsAsync<AzureHttpClientBase.AzureJsonCollectionWrapper<T>>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
        {
          this.m_formatter
        }, cancellationToken).ConfigureAwait(false);
        return (collectionWrapper.Value, collectionWrapper.NextLink);
      }
      return (await response.Content.ReadAsAsync<T>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        this.m_formatter
      }, cancellationToken).ConfigureAwait(false), (string) null);
    }

    protected async Task<T> ReadContentAsAsync<T>(
      HttpRequestMessage request,
      HttpResponseMessage response,
      Guid activityId,
      string invocationId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (T, string) valueTuple = await this.ReadPossiblyPaginatedContentAsAsync<T>(request, response, activityId, invocationId, cancellationToken).ConfigureAwait(false);
      T obj = valueTuple.Item1;
      if (valueTuple.Item2 != null)
        AzureClientTracing.Information(activityId, invocationId, "Request: {0} was paginated, but only the first page was read.", (object) request.RequestUri);
      return obj;
    }

    protected static AzureHttpClientBase.UrlPathBuilder GetSubscriptionUrlBuilder(
      Guid subscriptionId)
    {
      return new AzureHttpClientBase.UrlPathBuilder().Add("subscriptions").Add(subscriptionId.ToString());
    }

    protected static AzureHttpClientBase.UrlPathBuilder GetResourceGroupUrlBuilder(
      Guid subscriptionId,
      string resourceGroupName)
    {
      return AzureHttpClientBase.GetSubscriptionUrlBuilder(subscriptionId).Add("resourceGroups").Add(resourceGroupName);
    }

    private void CheckExpiration()
    {
      if (!(DateTime.UtcNow.Subtract(this.m_createdTime) > this.m_timeToLiveInCache))
        return;
      this.m_expired = true;
    }

    private static bool IsJsonResponse(HttpResponseMessage response) => !string.IsNullOrEmpty(response?.Content?.Headers?.ContentType?.MediaType) && string.Compare("application/json", response.Content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0;

    private async Task CheckResponseForError(
      HttpRequestMessage request,
      HttpResponseMessage response,
      Guid activityId,
      string invocationId = null)
    {
      if (!AzureHttpClientBase.IsJsonResponse(response) || response.IsSuccessStatusCode)
        return;
      await this.HandleError(request, response, activityId, invocationId).ConfigureAwait(false);
    }

    private async Task CheckResponseForError(
      HttpRequestMessage request,
      HttpResponseMessage response,
      IEnumerable<HttpStatusCode> successCodes,
      Guid activityId,
      string invocationId = null)
    {
      if (!AzureHttpClientBase.IsJsonResponse(response) || successCodes.Contains<HttpStatusCode>(response.StatusCode))
        return;
      await this.HandleError(request, response, activityId, invocationId).ConfigureAwait(false);
    }

    private async Task HandleError(
      HttpRequestMessage request,
      HttpResponseMessage response,
      Guid activityId,
      string invocationId = null)
    {
      using (request)
      {
        using (response)
        {
          Microsoft.Rest.Azure.CloudException cloudException = new Microsoft.Rest.Azure.CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) response.StatusCode));
          string responseString = string.Empty;
          if (response.Content != null)
            responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
          try
          {
            Microsoft.Rest.Azure.CloudError cloudError = SafeJsonConvert.DeserializeObject<Microsoft.Rest.Azure.CloudError>(responseString, this.CreateJsonDeserializationSettings());
            if (cloudError != null)
              cloudException = new Microsoft.Rest.Azure.CloudException(cloudError.Message)
              {
                Body = cloudError
              };
          }
          catch
          {
          }
          try
          {
            if (request.Content != null)
              cloudException.Request = new Microsoft.Rest.HttpRequestMessageWrapper(request, await request.Content.ReadAsStringAsync().ConfigureAwait(false));
          }
          catch
          {
          }
          cloudException.Response = new Microsoft.Rest.HttpResponseMessageWrapper(response, responseString);
          if (response.Headers.Contains("x-ms-request-id"))
            cloudException.RequestId = response.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
          int statusCode = (int) response.StatusCode;
          if (statusCode == 429 || statusCode >= 500)
          {
            this.m_expired = true;
            AzureClientTracing.Information(activityId, invocationId, "Setting Client instance to Expired after receiving a {0} HttpStatusCode response.", (object) statusCode);
          }
          Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException from = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException.CreateFrom(cloudException);
          AzureClientTracing.Error(activityId, invocationId, (Exception) from);
          throw from;
        }
      }
    }

    protected async Task ValidateResponseAsync(
      HttpRequestMessage request,
      HttpResponseMessage response,
      HttpStatusCode[] successCodes,
      Guid activityId,
      string invocationId)
    {
      if (!((IEnumerable<HttpStatusCode>) successCodes).Contains<HttpStatusCode>(response.StatusCode))
      {
        Microsoft.Rest.Azure.CloudException cloudException = new Microsoft.Rest.Azure.CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) response.StatusCode));
        string content = string.Empty;
        if (response.Content != null)
          content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        cloudException.Request = new Microsoft.Rest.HttpRequestMessageWrapper(request, (string) null);
        cloudException.Response = new Microsoft.Rest.HttpResponseMessageWrapper(response, content);
        if (response.Headers.Contains("x-ms-request-id"))
          cloudException.RequestId = response.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        int statusCode = (int) response.StatusCode;
        if (statusCode == 429 || statusCode >= 500)
        {
          this.m_expired = true;
          AzureClientTracing.Information(activityId, invocationId, "Setting Client instance to Expired after receiving a {0} HttpStatusCode response.", (object) statusCode);
        }
        Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException from = Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException.CreateFrom(cloudException);
        request.Dispose();
        response?.Dispose();
        throw from;
      }
    }

    protected internal string AddApiVersion(string relativeUri) => !this.UrlContainsApiVersionQueryParameter(relativeUri) ? relativeUri + "?api-version=" + Uri.EscapeDataString(this.ApiVersion) : relativeUri;

    protected internal string AddApiVersionWithQueryParameters(
      string relativeUri,
      string queryParameters)
    {
      if (!this.UrlContainsApiVersionQueryParameter(relativeUri) && !queryParameters.Contains("api-version"))
        return relativeUri + "?api-version=" + Uri.EscapeDataString(this.ApiVersion) + "&" + queryParameters;
      NameValueCollection queryParameters1 = this.GetQueryParameters(relativeUri);
      return queryParameters1 == null || queryParameters1.Count == 0 ? relativeUri + "?" + queryParameters : relativeUri + "&" + queryParameters;
    }

    protected JsonSerializerSettings CreateJsonDeserializationSettings()
    {
      if (this.m_deserializationSettings == null)
        this.m_deserializationSettings = new JsonSerializerSettings()
        {
          DateFormatHandling = DateFormatHandling.IsoDateFormat,
          DateTimeZoneHandling = DateTimeZoneHandling.Utc,
          NullValueHandling = NullValueHandling.Ignore,
          ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
          ContractResolver = (IContractResolver) new ReadOnlyJsonContractResolver(),
          Converters = (IList<JsonConverter>) new List<JsonConverter>()
          {
            (JsonConverter) new Iso8601TimeSpanConverter(),
            (JsonConverter) new TransformationJsonConverter(),
            (JsonConverter) new CloudErrorJsonConverter()
          }
        };
      return this.m_deserializationSettings;
    }

    private async Task<TResult> TraceScope<TResult>(
      Guid activityId,
      string method,
      IDictionary<string, object> parameters,
      Func<string, Task<TResult>> asyncCall)
    {
      AzureHttpClientBase instance = this;
      string invocationId = AzureClientTracing.NextInvocationId;
      AzureClientTracing.Enter(activityId, invocationId, (object) instance, method, parameters);
      TResult result1 = await asyncCall(invocationId).ConfigureAwait(false);
      AzureClientTracing.Exit(activityId, invocationId, (object) instance, method, (object) result1);
      TResult result2 = result1;
      invocationId = (string) null;
      return result2;
    }

    private bool UrlContainsApiVersionQueryParameter(string relativeUri)
    {
      if (!this.m_ValidateHttpClientQueryParams)
        return false;
      NameValueCollection queryParameters = this.GetQueryParameters(relativeUri);
      return queryParameters != null && queryParameters.Count != 0 && ((IEnumerable<string>) queryParameters.AllKeys).Contains<string>("api-version");
    }

    private NameValueCollection GetQueryParameters(string relativeUri) => HttpUtility.ParseQueryString(new Uri(this.BaseAddress, relativeUri).Query);

    protected sealed class UrlPathBuilder
    {
      private readonly IList<string> m_components = (IList<string>) new List<string>();

      public AzureHttpClientBase.UrlPathBuilder Add(string pathComponent)
      {
        this.m_components.Add(pathComponent);
        return this;
      }

      public string ToUrl() => string.Join("/", (IEnumerable<string>) this.m_components);
    }

    [DataContract]
    private sealed class AzureJsonCollectionWrapper<T> : VssJsonCollectionWrapperBase
    {
      [DataMember]
      public string NextLink { get; set; }

      [DataMember]
      public T Value { get; set; }
    }
  }
}
