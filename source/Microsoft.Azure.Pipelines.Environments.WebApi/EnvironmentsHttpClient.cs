// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.WebApi.EnvironmentsHttpClient
// Assembly: Microsoft.Azure.Pipelines.Environments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C70F6957-8B2D-4EE8-9E67-48CC1F1F106C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Pipelines.Environments.WebApi
{
  public class EnvironmentsHttpClient : EnvironmentsHttpClientBase
  {
    public EnvironmentsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public EnvironmentsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public EnvironmentsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public EnvironmentsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public EnvironmentsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
