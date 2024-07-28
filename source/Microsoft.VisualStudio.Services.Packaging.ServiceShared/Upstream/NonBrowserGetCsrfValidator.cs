// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.NonBrowserGetCsrfValidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class NonBrowserGetCsrfValidator : IFactory<bool>
  {
    private readonly HttpRequestMessage requestMessage;
    private readonly IVssRequestContext requestContext;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IFeatureFlagService featureFlagService;
    private readonly string[] whitelistedAuthorizationSchemes = new string[2]
    {
      "Basic",
      "Bearer"
    };
    private readonly string[] onPremWhitelistedAuthorizationSchemes = new string[3]
    {
      "Negotiate",
      "NTLM",
      "Kerberos"
    };

    public NonBrowserGetCsrfValidator(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IExecutionEnvironment executionEnvironment,
      IFeatureFlagService featureFlagService)
    {
      this.requestContext = requestContext;
      this.requestMessage = requestMessage;
      this.executionEnvironment = executionEnvironment;
      this.featureFlagService = featureFlagService;
    }

    public bool Get()
    {
      if (!this.IsGetRequest() || this.SkipValidation())
        return true;
      return this.IsAuthenticationTypeWhitelisted() && !this.IsUserAgentBlacklisted();
    }

    private bool IsGetRequest() => this.requestMessage.Method == HttpMethod.Get;

    private bool IsUserAgentBlacklisted() => new UserAgentDetails(this.requestContext.UserAgent).IsBrowser;

    private bool IsAuthenticationTypeWhitelisted()
    {
      string a = this.requestMessage.Headers.Authorization?.Scheme;
      foreach (string authorizationScheme in this.whitelistedAuthorizationSchemes)
      {
        if (string.Equals(a, authorizationScheme, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      if (!this.executionEnvironment.IsHosted())
      {
        if (string.IsNullOrWhiteSpace(a))
          a = this.requestContext.AuthenticationType();
        foreach (string authorizationScheme in this.onPremWhitelistedAuthorizationSchemes)
        {
          if (string.Equals(a, authorizationScheme, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private bool SkipValidation() => this.executionEnvironment.IsDevFabric() && this.featureFlagService.IsEnabled("Packaging.SkipCSRFValidation");
  }
}
