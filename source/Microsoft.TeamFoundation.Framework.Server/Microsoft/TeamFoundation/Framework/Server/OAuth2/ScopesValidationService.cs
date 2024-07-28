// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ScopesValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class ScopesValidationService : IScopesValidationService, IVssFrameworkService
  {
    private const string TraceArea = "Authorization";
    private const string TraceLayer = "ScopesValidationService";

    public bool ValidateScopes(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      string scopesClaim = this.GetScopesClaim(token.Claims);
      if (string.IsNullOrEmpty(scopesClaim))
      {
        requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ScopesValidationService), "Token validation failed for issuer {0}. Expected claim {1} was not found", (object) token.Issuer, (object) "scp");
        return false;
      }
      if (scopesClaim.Contains("vso.authorization_grant"))
      {
        requestContext.Trace(5510322, TraceLevel.Error, "Authorization", nameof (ScopesValidationService), "Token validation failed for issuer {0}. Token scopes contains {1} scope, which is not valid for use as an access token", (object) token.Issuer, (object) "vso.authorization_grant");
        return false;
      }
      try
      {
        if (!this.IsRequestMatchingScopes(requestContext, scopesClaim, token))
          return false;
        requestContext.Items.Add(RequestContextItemsKeys.Scope, (object) scopesClaim);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5510300, TraceLevel.Error, "Authorization", nameof (ScopesValidationService), ex);
        return false;
      }
      return true;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private string GetScopesClaim(IEnumerable<Claim> claims) => claims.FirstOrDefault<Claim>((Func<Claim, bool>) (f => f.Type.Equals("scp")))?.Value;

    private bool IsRequestMatchingScopes(
      IVssRequestContext requestContext,
      string claimValue,
      JwtSecurityToken token)
    {
      AuthorizationScopeConfiguration configuration = this.GetConfiguration(requestContext);
      bool usingUriScopes;
      string normalizedScopes = configuration.NormalizeScopes(claimValue, out usingUriScopes, true);
      if (usingUriScopes)
      {
        string requestUri = this.GetRequestUri(requestContext);
        HttpMethod requestHttpMethod = this.GetRequestHttpMethod(requestContext);
        if (!configuration.HasScopePatternMatch(normalizedScopes, requestUri, requestHttpMethod))
        {
          requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ScopesValidationService), "Token validation failed for issuer {0}. Token scope claim '{1}' not valid for request", (object) token.Issuer, (object) normalizedScopes);
          return false;
        }
      }
      return true;
    }

    private AuthorizationScopeConfiguration GetConfiguration(IVssRequestContext requestContext) => requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);

    private string GetRequestUri(IVssRequestContext requestContext)
    {
      string uri = (string) null;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.RootContext is IVssWebRequestContext rootContext)
        uri = rootContext.RelativePath;
      if (string.IsNullOrEmpty(uri))
        uri = requestContext.RequestUri().OriginalString;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        uri = new UriBuilder(uri)
        {
          Path = requestContext.RelativePath()
        }.Uri.ToString();
      return uri;
    }

    private HttpMethod GetRequestHttpMethod(IVssRequestContext requestContext)
    {
      string requestHttpMethod = requestContext.HttpMethod();
      return new HttpMethod(HttpContextFactory.Current.Request.GetHttpMethodFromMethodOverrideHeader(requestHttpMethod) ?? requestHttpMethod);
    }
  }
}
