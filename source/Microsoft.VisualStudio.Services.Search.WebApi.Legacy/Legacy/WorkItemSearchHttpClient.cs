// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.WorkItemSearchHttpClient
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [ResourceArea("EA48A0A1-269C-42D8-B8AD-DDC8FCDCF578")]
  public class WorkItemSearchHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "SearchException",
        typeof (SearchException)
      },
      {
        "InvalidQueryException",
        typeof (InvalidQueryException)
      }
    };

    public WorkItemSearchHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemSearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemSearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemSearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemSearchHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<WorkItemSearchResponse> CreateWorkItemQueryResultsAsync(
      WorkItemSearchRequest query,
      object userState = null)
    {
      return this.SendAsync<WorkItemSearchResponse>(HttpMethod.Post, SearchConstants.WorkItemQueryResultsLocationId, content: (HttpContent) new ObjectContent<WorkItemSearchRequest>(query, this.Formatter), userState: userState);
    }

    public virtual Task<WorkItemSearchResponse> CreateWorkItemQueryResultsAsync(
      WorkItemSearchRequest query,
      Guid projectId,
      object userState = null)
    {
      object routeValues = (object) new
      {
        project = projectId
      };
      return this.SendAsync<WorkItemSearchResponse>(HttpMethod.Post, SearchConstants.WorkItemQueryResultsLocationId, routeValues, content: (HttpContent) new ObjectContent<WorkItemSearchRequest>(query, this.Formatter), userState: userState);
    }

    public virtual Task<IEnumerable<WorkItemFieldMetadata>> CreateWorkItemFieldsQueryResultsAsync(
      object userState = null)
    {
      return this.SendAsync<IEnumerable<WorkItemFieldMetadata>>(HttpMethod.Get, SearchConstants.WorkItemFieldsLocationId, userState: userState);
    }

    public virtual Task<CountResponse> ResultsCountAsync(
      CountRequest query,
      string entityType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(entityType))
        collection.Add(nameof (entityType), entityType);
      HttpMethod post = HttpMethod.Post;
      Guid entityCountLocationId = SearchConstants.EntityCountLocationId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      ObjectContent<CountRequest> content = new ObjectContent<CountRequest>(query, this.Formatter);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CountResponse>(post, entityCountLocationId, content: (HttpContent) content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CountResponse> ResultsCountAsync(
      CountRequest query,
      string entityType,
      Guid projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      object obj1 = (object) new{ project = projectId };
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(entityType))
        collection.Add(nameof (entityType), entityType);
      HttpMethod post = HttpMethod.Post;
      Guid entityCountLocationId = SearchConstants.EntityCountLocationId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      object routeValues = obj1;
      CancellationToken cancellationToken1 = cancellationToken;
      ObjectContent<CountRequest> content = new ObjectContent<CountRequest>(query, this.Formatter);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CountResponse>(post, entityCountLocationId, routeValues, content: (HttpContent) content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) WorkItemSearchHttpClient.s_translatedExceptions;
  }
}
