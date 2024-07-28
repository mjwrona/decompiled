// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointHttpClient2
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public class ServiceEndpointHttpClient2 : ServiceEndpointHttpClient
  {
    public ServiceEndpointHttpClient2(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ServiceEndpointHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ServiceEndpointHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ServiceEndpointHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ServiceEndpointHttpClient2(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<IPagedCollection<ServiceEndpointExecutionRecord>> GetServiceEndpointExecutionRecordsAsync2(
      string project,
      Guid endpointId,
      int? top = null,
      long? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10a16738-9299-4cd1-9a81-fd23ad6200d0");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<IPagedCollection<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ServiceEndpointExecutionRecord>>>(this.GetPagedList<ServiceEndpointExecutionRecord>));
    }

    public virtual Task<IPagedCollection<ServiceEndpointExecutionRecord>> GetServiceEndpointExecutionRecordsAsync2(
      Guid project,
      Guid endpointId,
      int? top = null,
      long? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10a16738-9299-4cd1-9a81-fd23ad6200d0");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<IPagedCollection<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ServiceEndpointExecutionRecord>>>(this.GetPagedList<ServiceEndpointExecutionRecord>));
    }

    protected async Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      return await this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse).ConfigureAwait(false);
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
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      ServiceEndpointHttpClient2 endpointHttpClient2 = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await endpointHttpClient2.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await endpointHttpClient2.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      ServiceEndpointHttpClient2 endpointHttpClient2 = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) endpointHttpClient2).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await endpointHttpClient2.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    protected async Task<IPagedCollection<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      ServiceEndpointHttpClient2 endpointHttpClient2 = this;
      string continuationToken = endpointHttpClient2.GetContinuationToken(responseMessage);
      IPagedCollection<T> pagedList = (IPagedCollection<T>) new PagedCollection<T>(await endpointHttpClient2.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      if (responseMessage == null || responseMessage.Headers == null)
        return (string) null;
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }
  }
}
