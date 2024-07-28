// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.BitbucketHttpRequesterFactory
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  internal sealed class BitbucketHttpRequesterFactory : IExternalProviderHttpRequesterFactory
  {
    private const string c_area = "ExternalProviders";
    private const string c_layer = "BitbucketHttpRequesterFactory";
    private IVssRequestContext m_requestContext;

    public string ProviderType => "bitbucket";

    public void Initialize(object requestContext) => this.m_requestContext = requestContext as IVssRequestContext;

    public IExternalProviderHttpRequester GetRequester(HttpMessageHandler httpMessageHandler)
    {
      if (this.m_requestContext != null)
      {
        List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetMinimalDelegatingHandlers(this.m_requestContext, typeof (BitbucketVssHttpClientRequester), new ClientProviderHelper.Options(3, TimeSpan.FromSeconds(5.0), (byte) 100), "Bitbucket");
        return (IExternalProviderHttpRequester) new BitbucketVssHttpClientRequester(HttpClientFactory.CreatePipeline(httpMessageHandler, (IEnumerable<DelegatingHandler>) delegatingHandlers));
      }
      TeamFoundationTracingService.TraceRaw(ExternalProvidersTracePoints.HttpRequesterFactoryUninitialized, TraceLevel.Error, "ExternalProviders", nameof (BitbucketHttpRequesterFactory), "BitbucketVssHttpClientRequester created without a request context!");
      return (IExternalProviderHttpRequester) new BitbucketVssHttpClientRequester(httpMessageHandler);
    }
  }
}
