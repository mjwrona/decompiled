// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.PipelinePermissionsHttpClient
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  public class PipelinePermissionsHttpClient : PipelinePermissionsHttpClientBase
  {
    public PipelinePermissionsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PipelinePermissionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PipelinePermissionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PipelinePermissionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PipelinePermissionsHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
