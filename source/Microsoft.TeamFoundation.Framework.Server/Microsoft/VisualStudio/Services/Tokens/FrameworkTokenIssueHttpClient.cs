// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.FrameworkTokenIssueHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.WebApis.CustomHandlers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Tokens.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [ResourceArea("6b10046c-829d-44d2-8a1d-02f88f4ff032")]
  [ClientCancellationTimeout(60)]
  [ClientCircuitBreakerSettings(2, 50, MaxConcurrentRequests = 40)]
  [AddOriginUserAgentHandler]
  internal class FrameworkTokenIssueHttpClient : TokenIssueHttpClient
  {
    public FrameworkTokenIssueHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FrameworkTokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FrameworkTokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FrameworkTokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FrameworkTokenIssueHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
