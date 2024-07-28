// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestImpact.WebApi.TestImpactHttpClient
// Assembly: Microsoft.Azure.Pipelines.TestImpact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BB589E1E-9019-4050-9752-2657E26FDC18
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.TestImpact.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Pipelines.TestImpact.WebApi
{
  [ResourceArea("2E9F9F41-088B-4B4E-8438-CB3FAA3BF7E4")]
  public class TestImpactHttpClient : TestImpactHttpClientBase
  {
    public TestImpactHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestImpactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestImpactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestImpactHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestImpactHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
