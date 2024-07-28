// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ClientSettingsHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [ResourceArea("3FDA18BA-DFF2-42E6-8D10-C521B23B85FC")]
  public class ClientSettingsHttpClient : 
    ClientToolsHttpClientBase,
    IArtifactHttpClient,
    IClientSettingsHttpClient
  {
    public IAppTraceSource Tracer { get; private set; }

    public ClientSettingsHttpClient(Uri baseUrl, VssCredentials credentials)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null, (DelegatingHandler[]) null)
    {
    }

    public ClientSettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : this(baseUrl, credentials, settings, (DelegatingHandler[]) null)
    {
    }

    public ClientSettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(baseUrl, credentials, (VssHttpRequestSettings) null, handlers)
    {
    }

    public ClientSettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ClientSettingsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task GetOptionsAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    public void SetTracer(IAppTraceSource tracer) => this.Tracer = tracer;

    public async Task<ClientSettingsInfo> TryGetSettingsAsync(
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Client toolName,
      object userState = null,
      TraceLevel errorLevel = TraceLevel.Info,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        // ISSUE: reference to a compiler-generated method
        return await this.\u003C\u003En__0(toolName, userState, cancellationToken);
      }
      catch (Exception ex)
      {
        this.Trace(errorLevel, "Failed to retrieve Client Settings with error: " + ex.Message + ".");
      }
      return (ClientSettingsInfo) null;
    }

    private void Trace(TraceLevel traceLevel, string message)
    {
      if (this.Tracer == null)
        return;
      switch (traceLevel)
      {
        case TraceLevel.Error:
          this.Tracer.Error(message);
          break;
        case TraceLevel.Warning:
          this.Tracer.Warn(message);
          break;
        case TraceLevel.Verbose:
          this.Tracer.Verbose(message);
          break;
        default:
          this.Tracer.Info(message);
          break;
      }
    }
  }
}
