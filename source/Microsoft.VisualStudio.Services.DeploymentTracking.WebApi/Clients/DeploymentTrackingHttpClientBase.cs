// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.Clients.DeploymentTrackingHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F672626D-7DDA-4A84-9A4F-2205F04CA597
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.Clients
{
  [ResourceArea("b4bcf7e2-8869-45ce-9348-a087cba9d144")]
  public abstract class DeploymentTrackingHttpClientBase : VssHttpClientBase
  {
    public DeploymentTrackingHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DeploymentTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DeploymentTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DeploymentTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DeploymentTrackingHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> AddDeploymentResourceAsync(
      PostDeploymentResource deploymentResource,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PostDeploymentResource>(deploymentResource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> AddDeploymentResourceAsync(
      PostDeploymentResource deploymentResource,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PostDeploymentResource>(deploymentResource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> GetDeploymentResourceAsync(
      string project,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DeploymentResource>(new HttpMethod("GET"), new Guid("e3c30451-f373-45bf-9cf5-afe941256aad"), (object) new
      {
        project = project,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> GetDeploymentResourceAsync(
      Guid project,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DeploymentResource>(new HttpMethod("GET"), new Guid("e3c30451-f373-45bf-9cf5-afe941256aad"), (object) new
      {
        project = project,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentResource>> GetDeploymentResourcesAsync(
      string project,
      int? releaseDefinitionId = null,
      string resourceIdentifier = null,
      int? top = null,
      int? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (releaseDefinitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseDefinitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseDefinitionId), str);
      }
      if (resourceIdentifier != null)
        keyValuePairList.Add(nameof (resourceIdentifier), resourceIdentifier);
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$continuationToken", str);
      }
      return this.SendAsync<List<DeploymentResource>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentResource>> GetDeploymentResourcesAsync(
      Guid project,
      int? releaseDefinitionId = null,
      string resourceIdentifier = null,
      int? top = null,
      int? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (releaseDefinitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseDefinitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseDefinitionId), str);
      }
      if (resourceIdentifier != null)
        keyValuePairList.Add(nameof (resourceIdentifier), resourceIdentifier);
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$continuationToken", str);
      }
      return this.SendAsync<List<DeploymentResource>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> UpdateDeploymentResourceAsync(
      PatchDeploymentResource deploymentResource,
      string project,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object obj1 = (object) new
      {
        project = project,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PatchDeploymentResource>(deploymentResource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentResource> UpdateDeploymentResourceAsync(
      PatchDeploymentResource deploymentResource,
      Guid project,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e3c30451-f373-45bf-9cf5-afe941256aad");
      object obj1 = (object) new
      {
        project = project,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PatchDeploymentResource>(deploymentResource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
