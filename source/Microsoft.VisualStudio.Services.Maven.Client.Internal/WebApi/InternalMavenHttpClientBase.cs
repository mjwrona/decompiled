// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.InternalMavenHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Maven.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17C8743C-D7E4-4D17-A72C-2BE62109EBD0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.WebApi.Generated;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.WebApi
{
  [ResourceArea("6F7F8C07-FF36-473C-BCF3-BD6CC9B6C066")]
  public abstract class InternalMavenHttpClientBase : MavenHttpClient
  {
    public InternalMavenHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalMavenHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalMavenHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalMavenHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public InternalMavenHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7806a5ec-ab38-4ec2-bed2-4cb2989c1f3c");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<VersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7806a5ec-ab38-4ec2-bed2-4cb2989c1f3c");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<VersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string feed,
      string groupId,
      string artifactId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7806a5ec-ab38-4ec2-bed2-4cb2989c1f3c");
      object routeValues = (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<VersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
