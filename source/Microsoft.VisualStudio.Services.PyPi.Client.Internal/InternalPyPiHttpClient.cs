// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Client.Internal.InternalPyPiHttpClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2072801D-0EB4-49B3-8929-AFF365507D86
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PyPi.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Client.Internal
{
  public class InternalPyPiHttpClient : InternalPyPiHttpClientBase
  {
    public InternalPyPiHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalPyPiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalPyPiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalPyPiHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public InternalPyPiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    [Obsolete("Clients must now send aadTenantId.")]
    public override Task<Stream> GetFileInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotSupportedException("This internal http client method is no longer supported.");
    }

    [Obsolete("Clients must now send aadTenantId.")]
    public override Task<Stream> GetFileInternalAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotSupportedException("This internal http client method is no longer supported.");
    }

    [Obsolete("Clients must now send aadTenantId.")]
    public override Task<Stream> GetFileInternalAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotSupportedException("This internal http client method is no longer supported.");
    }

    public override async Task<Stream> GetFileInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPyPiHttpClient internalPyPiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbb01711-83d6-4794-ba4c-bf5b11cd13c6");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalPyPiHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(5.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      if (InternalPyPiHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetFileInternalAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPyPiHttpClient internalPyPiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbb01711-83d6-4794-ba4c-bf5b11cd13c6");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalPyPiHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(5.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      if (InternalPyPiHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetFileInternalAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPyPiHttpClient internalPyPiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbb01711-83d6-4794-ba4c-bf5b11cd13c6");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage = await internalPyPiHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(5.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      if (InternalPyPiHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      return response.StatusCode != HttpStatusCode.SeeOther && !InternalPyPiHttpClient.IsRedirect(response.StatusCode) ? base.HandleResponseAsync(response, cancellationToken) : (Task) Task.FromResult<bool>(true);
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther;
  }
}
