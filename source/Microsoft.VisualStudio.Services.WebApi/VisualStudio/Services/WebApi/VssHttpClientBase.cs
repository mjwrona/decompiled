// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi.Utilities.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public abstract class VssHttpClientBase : IVssHttpClient, IDisposable
  {
    private const int c_keepAliveTime = 30000;
    private const int c_keepAliveInterval = 5000;
    private bool m_isDisposed;
    private object m_disposeLock = new object();
    private readonly HttpClient m_client;
    private MediaTypeFormatter m_formatter;
    private VssResponseContext m_LastResponseContext;
    private ApiResourceLocationCollection m_resourceLocations;
    private ApiResourceVersion m_defaultApiVersion = new ApiResourceVersion(1.0);
    private bool m_excludeUrlsHeader;
    private bool m_lightweightHeader;
    private Lazy<HttpMethod> s_patchMethod = new Lazy<HttpMethod>((Func<HttpMethod>) (() => new HttpMethod("PATCH")));
    private const string c_optionsRelativePath = "_apis/";
    private const string c_optionsRelativePathWithAllHostTypes = "_apis/?allHostTypes=true";
    private const string c_jsonMediaType = "application/json";
    public static readonly string UserStatePropertyName = "VssClientBaseUserState";

    protected VssHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null)
    {
    }

    protected VssHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : this(baseUrl, credentials, settings, (DelegatingHandler[]) null)
    {
    }

    protected VssHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null, handlers)
    {
    }

    protected VssHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : this(baseUrl, VssHttpClientBase.BuildHandler(credentials, settings, handlers), true)
    {
    }

    protected VssHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
    {
      this.m_client = new HttpClient(pipeline, disposeHandler);
      this.m_client.Timeout = TimeSpan.FromMilliseconds(-1.0);
      this.m_client.BaseAddress = baseUrl;
      this.m_formatter = (MediaTypeFormatter) new VssJsonMediaTypeFormatter();
      this.SetServicePointOptions();
      this.SetTokenStorageUrlIfNeeded(pipeline);
    }

    private void SetTokenStorageUrlIfNeeded(HttpMessageHandler handler)
    {
      switch (handler)
      {
        case VssHttpMessageHandler httpMessageHandler:
          if (httpMessageHandler.Credentials == null)
            break;
          if (httpMessageHandler.Credentials.Federated != null && httpMessageHandler.Credentials.Federated.TokenStorageUrl == (Uri) null)
            httpMessageHandler.Credentials.Federated.TokenStorageUrl = this.m_client.BaseAddress;
          if (httpMessageHandler.Credentials.Windows == null || !(httpMessageHandler.Credentials.Windows.TokenStorageUrl == (Uri) null))
            break;
          httpMessageHandler.Credentials.Windows.TokenStorageUrl = this.m_client.BaseAddress;
          break;
        case DelegatingHandler delegatingHandler:
          this.SetTokenStorageUrlIfNeeded(delegatingHandler.InnerHandler);
          break;
      }
    }

    private static HttpMessageHandler BuildHandler(
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      DelegatingHandler[] handlers)
    {
      VssHttpMessageHandler innerHandler = new VssHttpMessageHandler(credentials, settings ?? new VssHttpRequestSettings());
      return handlers == null || handlers.Length == 0 ? (HttpMessageHandler) innerHandler : HttpClientFactory.CreatePipeline((HttpMessageHandler) innerHandler, (IEnumerable<DelegatingHandler>) handlers);
    }

    public Uri BaseAddress => this.m_client.BaseAddress;

    public VssResponseContext LastResponseContext => this.m_LastResponseContext;

    protected HttpClient Client => this.m_client;

    protected MediaTypeFormatter Formatter => this.m_formatter;

    protected virtual IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) null;

    protected HttpResponseMessage Send(HttpRequestMessage message, object userState = null)
    {
      try
      {
        return this.SendAsync(message, userState).Result;
      }
      catch (AggregateException ex)
      {
        AggregateException aggregateException = ex.Flatten();
        if (aggregateException.InnerExceptions.Count == 1)
          throw aggregateException.InnerExceptions[0];
        throw;
      }
    }

    protected Task<HttpResponseMessage> DeleteAsync(
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Delete, locationId, routeValues, version, queryParameters: queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    protected Task<HttpResponseMessage> GetAsync(
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Get, locationId, routeValues, version, queryParameters: queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    protected Task<TResult> GetAsync<TResult>(
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TResult>(HttpMethod.Get, locationId, routeValues, version, queryParameters: queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    protected Task<HttpResponseMessage> PatchAsync<T>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(this.s_patchMethod.Value, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<TResult> PatchAsync<T, TResult>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TResult>(this.s_patchMethod.Value, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<HttpResponseMessage> PostAsync<T>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Post, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<TResult> PostAsync<T, TResult>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TResult>(HttpMethod.Post, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<HttpResponseMessage> PutAsync<T>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Put, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<TResult> PutAsync<T, TResult>(
      T value,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TResult>(HttpMethod.Put, locationId, routeValues, version, (HttpContent) new ObjectContent<T>(value, this.m_formatter), queryParameters, userState, cancellationToken);
    }

    protected Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken);
    }

    protected async Task<T> SendAsync<T>(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await this.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await this.SendAsync<T>(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<HttpResponseMessage> SendAsync(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpResponseMessage httpResponseMessage;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await this.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          httpResponseMessage = await this.SendAsync(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    protected HttpResponseMessage Send(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null)
    {
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage message = this.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, userState, CancellationToken.None).SyncResult<HttpRequestMessage>())
          return this.Send(message, userState);
      }
    }

    protected async Task<HttpResponseMessage> SendAsync(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpResponseMessage httpResponseMessage;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await this.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          httpResponseMessage = await this.SendAsync(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    protected async Task<HttpResponseMessage> SendAsync(
      HttpMethod method,
      Guid locationId,
      HttpCompletionOption completionOption,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpResponseMessage httpResponseMessage;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await this.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          httpResponseMessage = await this.SendAsync(requestMessage, completionOption, userState, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    protected Task<HttpRequestMessage> CreateRequestMessageAsync(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      string mediaType = "application/json")
    {
      return this.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, mediaType);
    }

    protected virtual async Task<HttpRequestMessage> CreateRequestMessageAsync(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      string mediaType = "application/json")
    {
      return this.CreateRequestMessage(method, additionalHeaders, await this.GetResourceLocationAsync(locationId, userState, cancellationToken).ConfigureAwait(false) ?? throw new VssResourceNotFoundException(locationId, this.BaseAddress), routeValues, version, content, queryParameters, mediaType);
    }

    protected HttpRequestMessage CreateRequestMessage(
      HttpMethod method,
      ApiResourceLocation location,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      string mediaType = "application/json")
    {
      return this.CreateRequestMessage(method, (IEnumerable<KeyValuePair<string, string>>) null, location, routeValues, version, content, queryParameters, mediaType);
    }

    protected HttpRequestMessage CreateRequestMessage(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      ApiResourceLocation location,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      string mediaType = "application/json")
    {
      this.CheckForDisposed();
      ApiResourceVersion requestVersion = this.NegotiateRequestVersion(location, version);
      if (requestVersion == null)
        throw new VssVersionNotSupportedException(location, version.ApiVersion, location.MinVersion, this.BaseAddress);
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues, location.Area, location.ResourceName);
      Uri uri = VssHttpUriUtility.ConcatUri(this.BaseAddress, VssHttpUriUtility.ReplaceRouteValues(location.RouteTemplate, routeDictionary));
      if (queryParameters != null && queryParameters.Any<KeyValuePair<string, string>>())
        uri = uri.AppendQuery(queryParameters);
      HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri.AbsoluteUri);
      MediaTypeWithQualityHeaderValue acceptHeader = this.CreateAcceptHeader(requestVersion, mediaType);
      if (this.m_excludeUrlsHeader)
        acceptHeader.Parameters.Add(new NameValueHeaderValue("excludeUrls", "true"));
      if (this.m_lightweightHeader)
        acceptHeader.Parameters.Add(new NameValueHeaderValue("lightweight", "true"));
      requestMessage.Headers.Accept.Add(acceptHeader);
      if (additionalHeaders != null)
      {
        foreach (KeyValuePair<string, string> additionalHeader in additionalHeaders)
          requestMessage.Headers.Add(additionalHeader.Key, additionalHeader.Value);
      }
      if (content != null)
      {
        requestMessage.Content = content;
        if (requestMessage.Content.Headers.ContentType != null && !requestMessage.Content.Headers.ContentType.Parameters.Any<NameValueHeaderValue>((Func<NameValueHeaderValue, bool>) (p => p.Name.Equals("api-version"))))
          requestMessage.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("api-version", requestVersion.ToString()));
      }
      return requestMessage;
    }

    protected virtual MediaTypeWithQualityHeaderValue CreateAcceptHeader(
      ApiResourceVersion requestVersion,
      string mediaType)
    {
      MediaTypeWithQualityHeaderValue acceptHeader = new MediaTypeWithQualityHeaderValue(mediaType);
      acceptHeader.Parameters.AddApiResourceVersionValues(requestVersion, true, requestVersion.ApiVersion.Major <= 1);
      return acceptHeader;
    }

    protected virtual void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string parameterName,
      object model)
    {
      JObject jObject = JObject.FromObject(model, new VssJsonMediaTypeFormatter().CreateJsonSerializer());
      this.AddModelAsQueryParams(queryParams, parameterName, jObject);
    }

    protected virtual void AddIEnumerableAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string parameterName,
      object model)
    {
      JArray array = JArray.FromObject(model, new VssJsonMediaTypeFormatter().CreateJsonSerializer());
      this.AddModelAsQueryParams(queryParams, parameterName, array);
    }

    private void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string parameterName,
      JObject jObject)
    {
      foreach (JProperty property in jObject.Properties())
        this.AddModelAsQueryParams(queryParams, parameterName, property);
    }

    private void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string key,
      JProperty property)
    {
      if (property.Value == null)
        return;
      string key1 = string.Format("{0}[{1}]", (object) key, (object) property.Name);
      this.AddModelAsQueryParams(queryParams, key1, property.Value);
    }

    private void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string key,
      JArray array)
    {
      int num = 0;
      foreach (JToken child in array.Children())
      {
        string key1 = string.Format("{0}[{1}]", (object) key, (object) num);
        this.AddModelAsQueryParams(queryParams, key1, child);
        ++num;
      }
    }

    private void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string key,
      JToken token)
    {
      if (token.Type == JTokenType.Array)
        this.AddModelAsQueryParams(queryParams, key, (JArray) token);
      else if (token.Type == JTokenType.Object)
        this.AddModelAsQueryParams(queryParams, key, (JObject) token);
      else if (token.Type == JTokenType.Property)
        this.AddModelAsQueryParams(queryParams, key, (JProperty) token);
      else if (token.Type == JTokenType.Date)
        this.AddDateTimeToQueryParams(queryParams, key, (DateTime) token);
      else
        queryParams.Add(key, token.ToString());
    }

    protected void AddDateTimeToQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string name,
      DateTime localDateTime)
    {
      queryParams.Add(name, localDateTime.ToUniversalTime().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    protected void AddDateTimeToQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string name,
      DateTimeOffset dateTimeOffset)
    {
      queryParams.Add(name, dateTimeOffset.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    protected void AddDateTimeToHeaders(
      IList<KeyValuePair<string, string>> queryParams,
      string name,
      DateTimeOffset dateTimeOffset)
    {
      queryParams.Add(name, dateTimeOffset.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      T obj1;
      using (HttpResponseMessage response = await this.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
      {
        T obj2 = await this.ReadContentAsAsync<T>(response, cancellationToken).ConfigureAwait(false);
        if (obj2 is IPagedList pagedList)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> header in (System.Net.Http.Headers.HttpHeaders) response.Headers)
          {
            if (header.Key.Equals("X-MS-ContinuationToken", StringComparison.OrdinalIgnoreCase))
            {
              pagedList.ContinuationToken = header.Value.FirstOrDefault<string>();
              break;
            }
          }
        }
        obj1 = obj2;
      }
      return obj1;
    }

    protected async Task<T> ReadContentAsAsync<T>(
      HttpResponseMessage response,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      bool flag = this.IsJsonResponse(response);
      bool isMismatchedContentType = false;
      try
      {
        if (flag && typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()) && !typeof (byte[]).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()) && !typeof (JObject).GetTypeInfo().IsAssignableFrom(typeof (T).GetTypeInfo()))
          return (await this.ReadJsonContentAsync<VssJsonCollectionWrapper<T>>(response, cancellationToken).ConfigureAwait(false)).Value;
        if (flag)
          return await this.ReadJsonContentAsync<T>(response, cancellationToken).ConfigureAwait(false);
      }
      catch (JsonReaderException ex)
      {
        isMismatchedContentType = true;
      }
      return this.HasContent(response) ? await this.HandleInvalidContentType<T>(response, isMismatchedContentType).ConfigureAwait(false) : default (T);
    }

    protected virtual async Task<T> ReadJsonContentAsync<T>(
      HttpResponseMessage response,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await response.Content.ReadAsAsync<T>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        this.m_formatter
      }, cancellationToken).ConfigureAwait(false);
    }

    protected Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(message, HttpCompletionOption.ResponseContentRead, userState, cancellationToken);
    }

    protected async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage message,
      HttpCompletionOption completionOption,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      if (message.Headers.UserAgent != null)
      {
        foreach (ProductInfoHeaderValue productInfoHeaderValue in UserAgentUtility.GetDefaultRestUserAgent())
        {
          if (!message.Headers.UserAgent.Contains(productInfoHeaderValue))
            message.Headers.UserAgent.Add(productInfoHeaderValue);
        }
      }
      VssTraceActivity activity = VssTraceActivity.GetOrCreate();
      HttpResponseMessage httpResponseMessage;
      using (activity.EnterCorrelationScope())
      {
        if (userState != null)
          message.Properties[VssHttpClientBase.UserStatePropertyName] = userState;
        if (!message.Headers.Contains("X-VSS-E2EID"))
          message.Headers.Add("X-VSS-E2EID", Guid.NewGuid().ToString("D"));
        VssHttpEventSource.Log.HttpRequestStart(activity, message);
        message.Trace();
        message.Properties["MS.VSS.Diagnostics.TraceActivity"] = (object) activity;
        message.Properties["MS.VS.HttpCompletionOption"] = (object) completionOption;
        HttpResponseMessage response = await this.Client.SendAsync(message, completionOption, cancellationToken).ConfigureAwait(false);
        ConfiguredTaskAwaitable configuredTaskAwaitable;
        if (VssHttpClientBase.TestDelay != TimeSpan.Zero)
        {
          configuredTaskAwaitable = this.ProcessDelayAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
        configuredTaskAwaitable = this.HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
        httpResponseMessage = response;
      }
      return httpResponseMessage;
    }

    [Obsolete("Use VssHttpClientBase.HandleResponseAsync instead")]
    protected virtual void HandleResponse(HttpResponseMessage response)
    {
    }

    protected virtual async Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      response.Trace();
      VssHttpEventSource.Log.HttpRequestStop(VssTraceActivity.Current, response);
      this.m_LastResponseContext = new VssResponseContext(response.StatusCode, response.Headers);
      if (response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
        throw this.m_LastResponseContext.Exception = (Exception) new ProxyAuthenticationRequiredException();
      if (this.ShouldThrowError(response))
      {
        Exception innerException = (Exception) null;
        if (this.IsJsonResponse(response))
          innerException = await this.UnwrapExceptionAsync(response.Content, cancellationToken).ConfigureAwait(false);
        if (innerException == null || !(innerException is VssException))
        {
          string message = (string) null;
          if (innerException != null)
            message = innerException.Message;
          IEnumerable<string> values;
          if (response.Headers.TryGetValues("X-TFS-ServiceError", out values))
            message = UriUtility.UrlDecode(values.FirstOrDefault<string>());
          else if (string.IsNullOrEmpty(message))
            message = string.IsNullOrWhiteSpace(response.ReasonPhrase) ? response.StatusCode.ToString() : response.ReasonPhrase;
          innerException = (Exception) new VssServiceResponseException(response.StatusCode, message, innerException);
        }
        this.m_LastResponseContext.Exception = innerException;
        throw innerException;
      }
    }

    protected async Task<Exception> UnwrapExceptionAsync(
      HttpContent content,
      CancellationToken cancellationToken)
    {
      return (await content.ReadAsAsync<WrappedException>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        this.m_formatter
      }, cancellationToken).ConfigureAwait(false)).Unwrap(this.TranslatedExceptions);
    }

    protected virtual bool ShouldThrowError(HttpResponseMessage response) => !response.IsSuccessStatusCode;

    protected async Task<ApiResourceVersion> NegotiateRequestVersionAsync(
      Guid locationId,
      ApiResourceVersion version = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ApiResourceLocation location = await this.GetResourceLocationAsync(locationId, userState, cancellationToken).ConfigureAwait(false);
      return location != null ? this.NegotiateRequestVersion(location, version) : (ApiResourceVersion) null;
    }

    protected ApiResourceVersion NegotiateRequestVersion(
      ApiResourceLocation location,
      ApiResourceVersion version = null)
    {
      if (version == null)
        version = this.m_defaultApiVersion;
      if (location.MinVersion > version.ApiVersion)
        return (ApiResourceVersion) null;
      if (location.MaxVersion < version.ApiVersion)
        return new ApiResourceVersion(location.MaxVersion)
        {
          IsPreview = location.ReleasedVersion < location.MaxVersion
        };
      int resourceVersion = Math.Min(version.ResourceVersion, location.ResourceVersion);
      return new ApiResourceVersion(version.ApiVersion, resourceVersion)
      {
        IsPreview = location.ReleasedVersion < version.ApiVersion || version.IsPreview
      };
    }

    public void SetResourceLocations(ApiResourceLocationCollection resourceLocations)
    {
      if (this.m_resourceLocations != null)
        return;
      this.m_resourceLocations = resourceLocations;
    }

    public bool ExcludeUrlsHeader
    {
      get => this.m_excludeUrlsHeader;
      set => this.m_excludeUrlsHeader = value;
    }

    public bool LightweightHeader
    {
      get => this.m_lightweightHeader;
      set => this.m_lightweightHeader = value;
    }

    protected async Task<ApiResourceLocation> GetResourceLocationAsync(
      Guid locationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      await this.EnsureResourceLocationsPopulated(userState, cancellationToken).ConfigureAwait(false);
      return this.m_resourceLocations.TryGetLocationById(locationId);
    }

    internal virtual async Task<IEnumerable<ApiResourceLocation>> GetResourceLocationsAsync(
      bool allHostTypes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      IEnumerable<ApiResourceLocation> resourceLocationsAsync;
      using (HttpRequestMessage optionsRequest = new HttpRequestMessage(HttpMethod.Options, VssHttpUriUtility.ConcatUri(this.BaseAddress, allHostTypes ? "_apis/?allHostTypes=true" : "_apis/")))
        resourceLocationsAsync = await this.SendAsync<IEnumerable<ApiResourceLocation>>(optionsRequest, userState, cancellationToken).ConfigureAwait(false);
      return resourceLocationsAsync;
    }

    internal async Task EnsureResourceLocationsPopulated(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.m_resourceLocations != null)
        return;
      VssHttpUriUtility.ConcatUri(this.BaseAddress, "_apis/");
      IEnumerable<ApiResourceLocation> locations = await this.GetResourceLocationsAsync(false, userState, cancellationToken).ConfigureAwait(false);
      ApiResourceLocationCollection locationCollection = new ApiResourceLocationCollection();
      locationCollection.AddResourceLocations(locations);
      this.m_resourceLocations = locationCollection;
    }

    private bool HasContent(HttpResponseMessage response)
    {
      if (response == null || response.StatusCode == HttpStatusCode.NoContent || !(response.RequestMessage?.Method != HttpMethod.Head) || response.Content?.Headers == null)
        return false;
      long? contentLength = response.Content.Headers.ContentLength;
      if (!contentLength.HasValue)
        return true;
      contentLength = response.Content.Headers.ContentLength;
      if (!contentLength.HasValue)
        return false;
      contentLength = response.Content.Headers.ContentLength;
      long num = 0;
      return !(contentLength.GetValueOrDefault() == num & contentLength.HasValue);
    }

    private bool IsJsonResponse(HttpResponseMessage response) => this.HasContent(response) && string.Equals(response.Content.Headers.ContentType?.MediaType, "application/json", StringComparison.OrdinalIgnoreCase);

    private async Task<T> HandleInvalidContentType<T>(
      HttpResponseMessage response,
      bool isMismatchedContentType)
    {
      string responseType = response.Content?.Headers?.ContentType?.MediaType ?? "Unknown";
      using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
      {
        using (StreamReader streamReader = new StreamReader(responseStream))
        {
          char[] contentBuffer = new char[4096];
          int contentLength = 0;
          for (int i = 0; i < 4; ++i)
          {
            int num = await streamReader.ReadAsync(contentBuffer, i * 1024, 1024).ConfigureAwait(false);
            contentLength += num;
            if (num < 1024)
              break;
          }
          throw new VssServiceResponseException(response.StatusCode, !isMismatchedContentType ? "Invalid response content type: " + responseType + " Response Content: " + new string(contentBuffer, 0, contentLength) : "Mismatched response content type. " + responseType + " Response Content: " + new string(contentBuffer, 0, contentLength), (Exception) null);
        }
      }
    }

    private void SetServicePointOptions()
    {
      if (!(this.BaseAddress != (Uri) null))
        return;
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(this.BaseAddress);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.SetTcpKeepAlive(true, 30000, 5000);
    }

    public bool IsDisposed() => this.m_isDisposed;

    [Obsolete("This overload of Dispose has been deprecated.  Use the Dispose() method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.m_isDisposed)
        return;
      lock (this.m_disposeLock)
      {
        if (this.m_isDisposed)
          return;
        this.m_isDisposed = true;
        this.m_client.Dispose();
      }
    }

    private void CheckForDisposed()
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected IEnumerable<string> GetHeaderValue(HttpResponseMessage response, string headerName)
    {
      IEnumerable<string> values;
      if (!response.Headers.TryGetValues(headerName, out values) && response.Content != null)
        response.Content.Headers.TryGetValues(headerName, out values);
      return values;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TimeSpan TestDelay { get; set; }

    private async Task ProcessDelayAsync()
    {
      await Task.Delay(Math.Abs((int) VssHttpClientBase.TestDelay.TotalMilliseconds)).ConfigureAwait(false);
      if (VssHttpClientBase.TestDelay < TimeSpan.Zero)
        throw new Exception("User injected failure.");
    }

    internal bool HasResourceLocations => this.m_resourceLocations != null;

    protected sealed class OperationScope : IDisposable
    {
      private string m_area;
      private string m_operation;
      private bool m_disposed;
      private VssTraceActivity m_activity;
      private IDisposable m_correlationScope;

      public OperationScope(string area, string operation)
      {
        this.m_area = area;
        this.m_operation = operation;
        this.m_activity = VssTraceActivity.GetOrCreate();
        this.m_correlationScope = this.m_activity.EnterCorrelationScope();
        VssHttpEventSource.Log.HttpOperationStart(this.m_activity, this.m_area, operation);
      }

      public void Dispose()
      {
        if (this.m_disposed)
          return;
        this.m_disposed = true;
        VssHttpEventSource.Log.HttpOperationStop(this.m_activity, this.m_area, this.m_operation);
        if (this.m_correlationScope == null)
          return;
        this.m_correlationScope.Dispose();
        this.m_correlationScope = (IDisposable) null;
      }
    }
  }
}
