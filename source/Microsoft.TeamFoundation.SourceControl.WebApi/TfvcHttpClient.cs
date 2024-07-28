// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcHttpClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ClientCircuitBreakerSettings(22, 80)]
  [ResourceArea("8AA40520-446D-40E6-89F6-9C9F9CE44C48")]
  public class TfvcHttpClient : TfvcHttpClientBase
  {
    public TfvcHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TfvcHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TfvcHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TfvcHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TfvcHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected override void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string parameterName,
      object model)
    {
      if (model == null)
        return;
      if (model is TfvcVersionDescriptor)
      {
        foreach (JProperty property in JObject.FromObject(model, new VssJsonMediaTypeFormatter().CreateJsonSerializer()).Properties())
        {
          if (property.Value != null)
            queryParams.Add(property.Name, property.Value.ToString());
        }
      }
      else
        base.AddModelAsQueryParams(queryParams, parameterName, model);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItem>> GetItemsPagedAsync(
      string project,
      int top,
      string scopePath = null,
      int? changeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (changeset.HasValue)
        keyValuePairList.Add(nameof (changeset), changeset.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TfvcItem>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TfvcItem>>>(this.GetPagedList<TfvcItem>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItem>> GetItemsPagedAsync(
      Guid project,
      int top,
      string scopePath = null,
      int? changeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsPagedAsync(project.ToString(), top, scopePath, changeset, continuationToken, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItem>> GetItemsPagedAsync(
      int top,
      string scopePath = null,
      int? changeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsPagedAsync((string) null, top, scopePath, changeset, continuationToken, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItemPreviousHash>> GetItemsByChangesetPagedAsync(
      string project,
      int top,
      int baseChangeset,
      string scopePath = null,
      int? targetChangeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      keyValuePairList.Add(nameof (baseChangeset), baseChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (targetChangeset.HasValue)
        keyValuePairList.Add(nameof (targetChangeset), targetChangeset.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TfvcItemPreviousHash>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TfvcItemPreviousHash>>>(this.GetPagedList<TfvcItemPreviousHash>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItemPreviousHash>> GetItemsByChangesetPagedAsync(
      Guid project,
      int top,
      int baseChangeset,
      string scopePath = null,
      int? targetChangeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsByChangesetPagedAsync(project.ToString(), top, baseChangeset, scopePath, targetChangeset, continuationToken, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcItemPreviousHash>> GetItemsByChangesetPagedAsync(
      int top,
      int baseChangeset,
      string scopePath = null,
      int? targetChangeset = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsByChangesetPagedAsync((string) null, top, baseChangeset, scopePath, targetChangeset, continuationToken, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<TfvcChange>> GetChangesetChangesPagedAsync(
      int id,
      int top,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f32b86f2-15b9-4fe6-81b1-6f8938617ee5");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TfvcChange>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TfvcChange>>>(this.GetPagedList<TfvcChange>));
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      TfvcHttpClient tfvcHttpClient = this;
      string continuationToken = tfvcHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await tfvcHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
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
      TfvcHttpClient tfvcHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await tfvcHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await tfvcHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TfvcHttpClient tfvcHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) tfvcHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await tfvcHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }
  }
}
