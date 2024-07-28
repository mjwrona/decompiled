// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Client.Internal.InternalCargoHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cargo.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2A5EDF53-498F-4A63-B7BC-FF484C198E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Client.Internal.dll

using Microsoft.VisualStudio.Services.Cargo.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cargo.Client.Internal
{
  public class InternalCargoHttpClient : InternalCargoHttpClientBase
  {
    public InternalCargoHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalCargoHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalCargoHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalCargoHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public InternalCargoHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public override async Task<Stream> GetPackageContentStreamAsync(
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalCargoHttpClient internalCargoHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("92ba229f-c577-4a76-97a0-0205ce939b99");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalCargoHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      if (InternalCargoHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetPackageContentStreamAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalCargoHttpClient internalCargoHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("92ba229f-c577-4a76-97a0-0205ce939b99");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalCargoHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      if (InternalCargoHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetPackageContentStreamAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalCargoHttpClient internalCargoHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("92ba229f-c577-4a76-97a0-0205ce939b99");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalCargoHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      if (InternalCargoHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      return response.StatusCode != HttpStatusCode.SeeOther && !InternalCargoHttpClient.IsRedirect(response.StatusCode) ? base.HandleResponseAsync(response, cancellationToken) : (Task) Task.FromResult<bool>(true);
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther;
  }
}
