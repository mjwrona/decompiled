// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Client.Internal.InternalMavenHttpClient
// Assembly: Microsoft.VisualStudio.Services.Maven.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17C8743C-D7E4-4D17-A72C-2BE62109EBD0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Client.Internal
{
  public class InternalMavenHttpClient : InternalMavenHttpClientBase
  {
    public InternalMavenHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalMavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalMavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalMavenHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public InternalMavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public async Task<Stream> GetPackageFileAsync(
      string project,
      string feed,
      string path,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalMavenHttpClient internalMavenHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f285a171-0df5-4c49-aaf2-17d0d37d9f0e");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalMavenHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      if (InternalMavenHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<Package> GetPackageVersionAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      Guid aadTenantId,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther;
  }
}
