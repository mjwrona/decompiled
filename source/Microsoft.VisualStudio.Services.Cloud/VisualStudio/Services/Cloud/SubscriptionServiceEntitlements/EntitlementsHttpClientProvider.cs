// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.EntitlementsHttpClientProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  public class EntitlementsHttpClientProvider : IEntitlementsHttpClientProvider, IDisposable
  {
    private readonly HttpClientHandler entitlementsHttpClientHandler;

    public EntitlementsHttpClientProvider()
      : this(new HttpClientHandler())
    {
    }

    public EntitlementsHttpClientProvider(HttpClientHandler entitlementsHttpClientHandler) => this.entitlementsHttpClientHandler = entitlementsHttpClientHandler;

    public HttpClient GetEv4EntitlementsHttpClient(
      IVssRequestContext requestContext,
      string serviceName)
    {
      List<DelegatingHandler> delegatingHandlers = EntitlementsHttpClientProvider.CreateDelegatingHandlers(requestContext, serviceName);
      return this.GetEv4EntitlementsHttpClient(requestContext, serviceName, delegatingHandlers);
    }

    public HttpClient GetEv4EntitlementsHttpClient(
      IVssRequestContext requestContext,
      string serviceName,
      List<DelegatingHandler> delegatingHandlers)
    {
      return new HttpClient(HttpClientFactory.CreatePipeline((HttpMessageHandler) this.entitlementsHttpClientHandler, (IEnumerable<DelegatingHandler>) delegatingHandlers), false);
    }

    private static List<DelegatingHandler> CreateDelegatingHandlers(
      IVssRequestContext requestContext,
      string serviceName)
    {
      return ClientProviderHelper.GetMinimalDelegatingHandlers(requestContext, typeof (EntitlementService), new ClientProviderHelper.Options(3, TimeSpan.FromSeconds(20.0), (byte) 100), serviceName);
    }

    public void Dispose() => this.entitlementsHttpClientHandler.Dispose();
  }
}
