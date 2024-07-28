// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.ThrottledGitHttpClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ClientCircuitBreakerSettings(22, 80)]
  [ReactiveClientToThrottling(true)]
  public class ThrottledGitHttpClient : GitHttpClient
  {
    public ThrottledGitHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials, new VssHttpRequestSettings()
      {
        SendTimeout = GitHttpClient.s_sendTimeout
      })
    {
    }

    public ThrottledGitHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
    {
      Uri baseUrl1 = baseUrl;
      VssCredentials credentials1 = credentials;
      VssHttpRequestSettings settings = new VssHttpRequestSettings();
      settings.SendTimeout = GitHttpClient.s_sendTimeout;
      DelegatingHandler[] delegatingHandlerArray = handlers;
      // ISSUE: explicit constructor call
      base.\u002Ector(baseUrl1, credentials1, settings, delegatingHandlerArray);
    }

    public ThrottledGitHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ThrottledGitHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ThrottledGitHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }
  }
}
