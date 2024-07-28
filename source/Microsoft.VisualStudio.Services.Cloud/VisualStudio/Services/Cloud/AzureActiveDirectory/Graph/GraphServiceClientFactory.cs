// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.GraphServiceClientFactory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal class GraphServiceClientFactory : IGraphServiceClientFactory
  {
    private readonly IConfigQueryable<string> MicrosoftGraphApiDomainNameConfig;
    private readonly IConfigQueryable<string> MicrosoftGraphApiVersionConfig;
    private readonly IConfigQueryable<byte> MicrosoftGraphApiLoggingPercentage;
    private readonly IConfigQueryable<int> MicrosoftGraphApiSlowThresholdInSeconds;
    private readonly IConfigQueryable<int> MicrosoftGraphApiMaxRetries;
    private const string HeaderClientRequestId = "client-request-id";

    private WinHttpHandler WinHttpHandler { get; set; }

    public GraphServiceClientFactory()
      : this(ConfigProxy.Create<string>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiDomainName), ConfigProxy.Create<string>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiVersion), ConfigProxy.Create<byte>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiLoggingPercentage), ConfigProxy.Create<int>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiSlowThresholdInSeconds), ConfigProxy.Create<int>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiMaxRetries), (WinHttpHandler) null)
    {
    }

    public GraphServiceClientFactory(
      IConfigQueryable<string> domainNameConfig,
      IConfigQueryable<string> versionConfig,
      IConfigQueryable<byte> loggingPercentage,
      IConfigQueryable<int> slowThresholdInSeconds,
      IConfigQueryable<int> maxRetries,
      WinHttpHandler winHttpHandler)
    {
      this.MicrosoftGraphApiDomainNameConfig = domainNameConfig;
      this.MicrosoftGraphApiVersionConfig = versionConfig;
      this.MicrosoftGraphApiLoggingPercentage = loggingPercentage;
      this.MicrosoftGraphApiSlowThresholdInSeconds = slowThresholdInSeconds;
      this.MicrosoftGraphApiMaxRetries = maxRetries;
      this.WinHttpHandler = winHttpHandler ?? new WinHttpHandler();
    }

    public string BaseUrlOverride { get; set; }

    public string VersionOverride { get; set; }

    public GraphServiceClient CreateGraphServiceClient(
      IVssRequestContext context,
      string accessToken)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      GraphServiceClientFactory.\u003C\u003Ec__DisplayClass20_0 cDisplayClass200 = new GraphServiceClientFactory.\u003C\u003Ec__DisplayClass20_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass200.accessToken = accessToken;
      string baseUrl = this.GetBaseUrl(context);
      // ISSUE: reference to a compiler-generated field
      cDisplayClass200.activityId = context.ActivityId.ToString();
      // ISSUE: method pointer
      DelegateAuthenticationProvider authenticationProvider = new DelegateAuthenticationProvider(new AuthenticateRequestAsyncDelegate((object) cDisplayClass200, __methodptr(\u003CCreateGraphServiceClient\u003Eb__0)));
      return new GraphServiceClient(baseUrl, (IAuthenticationProvider) authenticationProvider, (IHttpProvider) this.GetHttpProvider(context));
    }

    public virtual string GetBaseUrl(IVssRequestContext context)
    {
      if (!string.IsNullOrEmpty(this.BaseUrlOverride) && !string.IsNullOrEmpty(this.VersionOverride))
        throw new InvalidOperationException("Both BaseUrlOverride and VersionOverride cannot be set at the same time because BaseUrlOverride contains the Version already.");
      if (!string.IsNullOrEmpty(this.BaseUrlOverride))
        return this.BaseUrlOverride;
      string str = this.VersionOverride ?? this.MicrosoftGraphApiVersionConfig.QueryByCtx<string>(context);
      return this.MicrosoftGraphApiDomainNameConfig.QueryByCtx<string>(context) + "/" + str;
    }

    protected virtual List<DelegatingHandler> GetDelegatingHandlers(
      IVssRequestContext requestContext)
    {
      return ClientProviderHelper.GetMinimalDelegatingHandlers(requestContext, typeof (GraphServiceClient), new ClientProviderHelper.Options(this.MicrosoftGraphApiMaxRetries.QueryByCtx<int>(requestContext), TimeSpan.FromSeconds((double) this.MicrosoftGraphApiSlowThresholdInSeconds.QueryByCtx<int>(requestContext)), this.MicrosoftGraphApiLoggingPercentage.QueryByCtx<byte>(requestContext)), "MicrosoftGraph");
    }

    private HttpProvider GetHttpProvider(IVssRequestContext requestContext) => new HttpProvider(GraphClientFactory.CreatePipeline((IEnumerable<DelegatingHandler>) this.GetDelegatingHandlers(requestContext), (HttpMessageHandler) this.WinHttpHandler), false, (ISerializer) null);
  }
}
