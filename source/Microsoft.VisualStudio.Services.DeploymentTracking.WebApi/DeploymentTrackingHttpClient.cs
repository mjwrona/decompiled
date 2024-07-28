// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentTrackingHttpClient
// Assembly: Microsoft.VisualStudio.Services.DeploymentTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F672626D-7DDA-4A84-9A4F-2205F04CA597
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.Clients;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.DeploymentTracking.WebApi
{
  public class DeploymentTrackingHttpClient : DeploymentTrackingHttpClientBase
  {
    public DeploymentTrackingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DeploymentTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DeploymentTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DeploymentTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DeploymentTrackingHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
