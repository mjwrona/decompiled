// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TaggingHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("1F131D7F-CFBB-4EC9-B358-FB4E8341CE59")]
  public class TaggingHttpClient : VssHttpClientBase
  {
    public TaggingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaggingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaggingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaggingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaggingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<WebApiTagDefinition> GetTagAsync(
      Guid scopeId,
      Guid tagId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAsync<WebApiTagDefinition>(TaggingWebApiConstants.TagsLocationId, (object) new
      {
        scopeId = scopeId,
        tagId = tagId
      }, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetTagAsync(
      Guid scopeId,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.GetAsync<WebApiTagDefinition>(TaggingWebApiConstants.TagsLocationId, (object) new
      {
        scopeId = scopeId,
        tagId = name
      }, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<WebApiTagDefinitionList> GetTagsAsync(
      Guid scopeId,
      bool includeInactive = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaggingHttpClient taggingHttpClient1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (includeInactive), includeInactive.ToString());
      TaggingHttpClient taggingHttpClient2 = taggingHttpClient1;
      Guid tagsLocationId = TaggingWebApiConstants.TagsLocationId;
      var routeValues = new{ scopeId = scopeId };
      object obj = userState;
      List<KeyValuePair<string, string>> queryParameters = collection;
      object userState1 = obj;
      CancellationToken cancellationToken1 = cancellationToken;
      return new WebApiTagDefinitionList(await taggingHttpClient2.GetAsync<IEnumerable<WebApiTagDefinition>>(tagsLocationId, (object) routeValues, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false));
    }

    public Task<WebApiTagDefinition> CreateTagAsync(
      Guid scopeId,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.PostAsync<WebApiCreateTagRequestData, WebApiTagDefinition>(new WebApiCreateTagRequestData()
      {
        Name = name
      }, TaggingWebApiConstants.TagsLocationId, (object) new
      {
        scopeId = scopeId
      }, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WebApiTagDefinition> UpdateTagAsync(
      Guid scopeId,
      Guid tagId,
      string name,
      bool? active,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (name != null)
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      WebApiTagDefinition apiTagDefinition = new WebApiTagDefinition();
      if (name != null)
        apiTagDefinition.Name = name;
      if (active.HasValue)
        apiTagDefinition.Active = new bool?(active.Value);
      return this.PatchAsync<WebApiTagDefinition, WebApiTagDefinition>(apiTagDefinition, TaggingWebApiConstants.TagsLocationId, (object) new
      {
        scopeId = scopeId,
        tagId = tagId
      }, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteTagAsync(
      Guid scopeId,
      Guid tagId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.DeleteAsync(TaggingWebApiConstants.TagsLocationId, (object) new
      {
        scopeId = scopeId,
        tagId = tagId
      }, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
