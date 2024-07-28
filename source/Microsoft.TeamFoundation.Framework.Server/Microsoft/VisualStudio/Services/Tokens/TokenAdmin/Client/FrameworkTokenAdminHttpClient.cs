// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.TokenAdmin.Client.FrameworkTokenAdminHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.WebApis.CustomHandlers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Tokens.TokenAdmin.Client
{
  [ResourceArea("95935461-9E54-44BD-B9FB-04F4DD05D640")]
  [ClientCancellationTimeout(60)]
  [ClientCircuitBreakerSettings(10, 50)]
  [AddOriginUserAgentHandler]
  internal class FrameworkTokenAdminHttpClient : TokenAdministrationHttpClient
  {
    public FrameworkTokenAdminHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FrameworkTokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FrameworkTokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FrameworkTokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FrameworkTokenAdminHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
