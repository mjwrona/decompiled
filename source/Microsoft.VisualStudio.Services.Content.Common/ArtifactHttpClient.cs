// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactHttpClient
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public abstract class ArtifactHttpClient : VssHttpClientBase, IArtifactHttpClient
  {
    protected IAppTraceSource maybeTracer;

    public ArtifactHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.UpdateServicePointSettings();
    }

    public ArtifactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.UpdateServicePointSettings();
    }

    public ArtifactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.UpdateServicePointSettings();
    }

    public ArtifactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.UpdateServicePointSettings();
    }

    public ArtifactHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.UpdateServicePointSettings();
    }

    public abstract Guid ResourceId { get; }

    public virtual Task GetOptionsAsync(CancellationToken cancellationToken) => (Task) this.SendAsync(HttpMethod.Options, this.ResourceId, cancellationToken: cancellationToken);

    protected IAppTraceSource tracer => this.maybeTracer ?? (IAppTraceSource) NoopAppTraceSource.Instance;

    public virtual void SetTracer(IAppTraceSource tracer) => this.maybeTracer = this.maybeTracer == null ? tracer : throw new InvalidOperationException("SetTracer was already called earlier. In order to preserve thread safety it cannot be changed.");

    protected override bool ShouldThrowError(HttpResponseMessage response)
    {
      HttpStatusCode[] source;
      if (response.RequestMessage.Properties.TryGetValue<HttpStatusCode[]>("ExpectedStatus", out source) && ((IEnumerable<HttpStatusCode>) source).Contains<HttpStatusCode>(response.StatusCode))
        return false;
      switch (response.StatusCode)
      {
        case HttpStatusCode.Found:
        case HttpStatusCode.SeeOther:
          return false;
        default:
          return base.ShouldThrowError(response);
      }
    }

    protected virtual ServicePointExtensions.ServicePointConfig GetServicePointSettings() => new ServicePointExtensions.ServicePointConfig()
    {
      MaxConnectionsPerProcessor = new int?(32),
      ConnectionLeaseTimeout = new TimeSpan?(),
      Expect100Continue = new bool?(false),
      UseNagleAlgorithm = new bool?(false),
      TcpKeepAlive = new ServicePointExtensions.ServicePointConfigKeepAlive?(new ServicePointExtensions.ServicePointConfigKeepAlive()
      {
        KeepAliveTime = TimeSpan.FromSeconds(30.0),
        KeepAliveInterval = TimeSpan.FromSeconds(5.0)
      })
    };

    private void UpdateServicePointSettings()
    {
      if (!(this.BaseAddress != (Uri) null))
        return;
      ServicePointManager.FindServicePoint(this.BaseAddress).UpdateServicePointSettings(this.GetServicePointSettings());
    }
  }
}
