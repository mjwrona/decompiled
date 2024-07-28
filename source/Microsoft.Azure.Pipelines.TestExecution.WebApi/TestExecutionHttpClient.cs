// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.WebApi.TestExecutionHttpClient
// Assembly: Microsoft.Azure.Pipelines.TestExecution.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D8DB80F5-61CE-408E-B0A6-21067A361E64
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.TestExecution.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.WebApi
{
  [ResourceArea("75CAD6D7-EE47-4E86-9A06-DB41AE372B00")]
  public class TestExecutionHttpClient : TestExecutionHttpClientBase
  {
    public TestExecutionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestExecutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestExecutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestExecutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestExecutionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
