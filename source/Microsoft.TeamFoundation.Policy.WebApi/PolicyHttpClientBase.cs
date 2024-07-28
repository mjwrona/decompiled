// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyHttpClientBase
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PolicyHttpClientBase : PolicyCompatHttpClientBase
  {
    public PolicyHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PolicyHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PolicyHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PolicyHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PolicyHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<IPagedList<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      string project,
      string scope = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(scope))
        keyValuePairList.Add(nameof (scope), scope);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<PolicyConfiguration>>>(this.GetPagedList<PolicyConfiguration>));
    }

    public Task<IPagedList<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      Guid project,
      string scope = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPolicyConfigurationsAsync(project.ToString(), scope, top, continuationToken, userState, cancellationToken);
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      PolicyHttpClientBase policyHttpClientBase = this;
      string continuationToken = policyHttpClientBase.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await policyHttpClientBase.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }

    protected Task<T> SendAsync<T>(
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
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse);
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
      PolicyHttpClientBase policyHttpClientBase = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await policyHttpClientBase.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await policyHttpClientBase.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      PolicyHttpClientBase policyHttpClientBase = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) policyHttpClientBase).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await policyHttpClientBase.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }
  }
}
