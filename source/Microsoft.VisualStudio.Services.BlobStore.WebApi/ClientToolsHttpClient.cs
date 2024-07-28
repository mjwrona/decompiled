// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ClientToolsHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class ClientToolsHttpClient : 
    ClientToolsHttpClientBase,
    IArtifactHttpClient,
    IArtifactVersionHttpClient
  {
    private readonly string osName;
    private readonly string architecture;

    public IAppTraceSource Tracer { get; private set; }

    public ClientToolsHttpClient(Uri baseUrl, VssCredentials credentials)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null, (DelegatingHandler[]) null)
    {
    }

    public ClientToolsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : this(baseUrl, credentials, settings, (DelegatingHandler[]) null)
    {
    }

    public ClientToolsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null, handlers)
    {
    }

    public ClientToolsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.osName = OSUtilities.GetRuntimeOS();
      this.architecture = "amd64";
    }

    public ClientToolsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.osName = OSUtilities.GetRuntimeOS();
      this.architecture = "amd64";
    }

    public Task GetOptionsAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    public void SetTracer(IAppTraceSource tracer) => this.Tracer = tracer;

    public async Task<string> GetVersionAsync(string toolName, CancellationToken cancellationToken)
    {
      ClientToolsHttpClient clientToolsHttpClient1 = this;
      ClientToolsHttpClient clientToolsHttpClient2 = clientToolsHttpClient1;
      string toolName1 = toolName;
      string osName = clientToolsHttpClient1.osName;
      string architecture = clientToolsHttpClient1.architecture;
      CancellationToken cancellationToken1 = cancellationToken;
      bool? netfx = new bool?();
      CancellationToken cancellationToken2 = cancellationToken1;
      return (await clientToolsHttpClient2.GetReleaseAsync(toolName1, osName: osName, arch: architecture, netfx: netfx, cancellationToken: cancellationToken2)).Version;
    }

    public async Task<Stream> GetClientBinariesAsync(
      string toolName,
      CancellationToken cancellationToken)
    {
      ClientToolsHttpClient clientToolsHttpClient1 = this;
      ClientToolsHttpClient clientToolsHttpClient2 = clientToolsHttpClient1;
      string toolName1 = toolName;
      string osName = clientToolsHttpClient1.osName;
      string architecture = clientToolsHttpClient1.architecture;
      CancellationToken cancellationToken1 = cancellationToken;
      bool? netfx = new bool?();
      CancellationToken cancellationToken2 = cancellationToken1;
      ClientToolReleaseInfo releaseAsync = await clientToolsHttpClient2.GetReleaseAsync(toolName1, osName: osName, arch: architecture, netfx: netfx, cancellationToken: cancellationToken2);
      return await (await clientToolsHttpClient1.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, releaseAsync.Uri), cancellationToken).ConfigureAwait(false)).Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
