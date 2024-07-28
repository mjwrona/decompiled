// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthenticationSessionTokenHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class UserAuthenticationSessionTokenHandler
  {
    private static UserAuthenticationSessionTokenProvider userAuthTokenProvider = new UserAuthenticationSessionTokenProvider();
    internal const string RequestContextKeyName = "UserAuthenticationToken";
    public const string FedAuthShimIdentifier = "UserAuthenticationToken";

    public static UserAuthenticationSessionToken ReadSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      return UserAuthenticationSessionTokenHandler.userAuthTokenProvider.ReadSessionToken(requestContext, context);
    }

    public static UserAuthenticationSessionToken IssueSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.UserAuthToken,
      IEnumerable<Claim> additionalClaims = null,
      UserAuthenticationSessionToken currentToken = null)
    {
      return UserAuthenticationSessionTokenHandler.userAuthTokenProvider.IssueSessionToken(requestContext, httpContext, claimsPrincipal, authenticationMechanism, additionalClaims, currentToken);
    }

    public static UserAuthenticationSessionToken UpgradeFromFedAuthToUserAuthToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      SecurityToken fedAuthToken,
      AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.UserAuthToken,
      IEnumerable<Claim> additionalClaims = null)
    {
      return UserAuthenticationSessionTokenHandler.userAuthTokenProvider.UpgradeFromFedAuthToUserAuthToken(requestContext, httpContext, claimsPrincipal, fedAuthToken, authenticationMechanism, additionalClaims);
    }

    public static UserAuthenticationSessionToken CreateSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      AuthenticationMechanism authenticationMechanism,
      IEnumerable<Claim> additionalClaims = null,
      DateTime? tokenValidFrom = null)
    {
      return UserAuthenticationSessionTokenHandler.userAuthTokenProvider.CreateSessionToken(requestContext, httpContext, claimsPrincipal, authenticationMechanism, additionalClaims, tokenValidFrom);
    }

    public static void WriteTokenToCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token)
    {
      UserAuthenticationSessionTokenHandler.userAuthTokenProvider.WriteTokenToCookie(requestContext, httpContext, token);
    }

    public static void DeleteSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      UserAuthenticationSessionTokenHandler.userAuthTokenProvider.DeleteSessionToken(requestContext, httpContext);
    }

    internal static bool ShouldRefreshToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token)
    {
      return UserAuthenticationSessionTokenHandler.userAuthTokenProvider.ShouldRefreshToken(requestContext, httpContext, token);
    }

    internal static class ClaimTypeMapping
    {
      public static string GetValidJwtClaimType(string claimType) => UserAuthenticationSessionTokenProvider.ClaimTypeMapping.GetValidJwtClaimType(claimType);

      public static string GetValidLongClaimType(string jwtClaimType) => UserAuthenticationSessionTokenProvider.ClaimTypeMapping.GetValidLongClaimType(jwtClaimType);
    }
  }
}
