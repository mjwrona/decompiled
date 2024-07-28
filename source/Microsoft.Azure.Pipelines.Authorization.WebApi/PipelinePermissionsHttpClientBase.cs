// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.PipelinePermissionsHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [ResourceArea("a81a0441-de52-4000-aa15-ff0e07bfbbaa")]
  public abstract class PipelinePermissionsHttpClientBase : VssHttpClientBase
  {
    public PipelinePermissionsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PipelinePermissionsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PipelinePermissionsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PipelinePermissionsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PipelinePermissionsHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<ResourcePipelinePermissions> GetPipelinePermissionsForResourceAsync(
      string project,
      string resourceType,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ResourcePipelinePermissions>(new HttpMethod("GET"), new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2"), (object) new
      {
        project = project,
        resourceType = resourceType,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ResourcePipelinePermissions> GetPipelinePermissionsForResourceAsync(
      Guid project,
      string resourceType,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ResourcePipelinePermissions>(new HttpMethod("GET"), new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2"), (object) new
      {
        project = project,
        resourceType = resourceType,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResourceAsync(
      string project,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourceAuthorization,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2");
      object obj1 = (object) new
      {
        project = project,
        resourceType = resourceType,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourcePipelinePermissions>(resourceAuthorization, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResourcePipelinePermissions>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResourceAsync(
      Guid project,
      string resourceType,
      string resourceId,
      ResourcePipelinePermissions resourceAuthorization,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2");
      object obj1 = (object) new
      {
        project = project,
        resourceType = resourceType,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourcePipelinePermissions>(resourceAuthorization, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResourcePipelinePermissions>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResourcesAsync(
      string project,
      IEnumerable<ResourcePipelinePermissions> resourceAuthorizations,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ResourcePipelinePermissions>>(resourceAuthorizations, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourcePipelinePermissions>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResourcesAsync(
      Guid project,
      IEnumerable<ResourcePipelinePermissions> resourceAuthorizations,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5b9a4a4-e6cd-4096-853c-ab7d8b0c4eb2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ResourcePipelinePermissions>>(resourceAuthorizations, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourcePipelinePermissions>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
