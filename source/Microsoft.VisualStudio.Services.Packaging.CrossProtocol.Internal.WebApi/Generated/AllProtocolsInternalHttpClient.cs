// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Generated.AllProtocolsInternalHttpClient
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Generated
{
  [ResourceArea("{9A7E7451-3763-432F-A213-2E40F4012709}")]
  public class AllProtocolsInternalHttpClient : VssHttpClientBase
  {
    public AllProtocolsInternalHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AllProtocolsInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AllProtocolsInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AllProtocolsInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AllProtocolsInternalHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<UpstreamHealthStatus>> GetUpstreamStatusAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<UpstreamHealthStatus>>(new HttpMethod("GET"), new Guid("7112c2dd-6ca1-421e-bf9b-40eda2cb6985"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<UpstreamHealthStatus>> GetUpstreamStatusAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<UpstreamHealthStatus>>(new HttpMethod("GET"), new Guid("7112c2dd-6ca1-421e-bf9b-40eda2cb6985"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<UpstreamHealthStatus>> GetUpstreamStatusAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<UpstreamHealthStatus>>(new HttpMethod("GET"), new Guid("7112c2dd-6ca1-421e-bf9b-40eda2cb6985"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
