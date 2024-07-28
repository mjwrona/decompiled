// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.RestApiResourceProvider
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Tokens;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class RestApiResourceProvider : IVssApiResourceProvider
  {
    private static readonly Version DefaultVersion = new Version(0, 0, 0, 0);

    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("Token", "0AD75E84-88AE-4325-84B5-EBB30910283C");
      areas.RegisterArea("oauth2", "585028FE-17D8-49E2-9A1B-EFB4D8502156");
      areas.RegisterArea("DelegatedAuth", "A0848FA1-3593-4AEC-949C-694C73F4C4CE");
      areas.RegisterArea("Tokens", "55967393-20EF-45C6-A96C-B5D5D5986A9A");
      HttpRouteCollection routes1 = routes;
      Guid aadUserToken = TokenResourceIds.AadUserToken;
      Version defaultVersion1 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion2 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion3 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion1 = defaultVersion2;
      routes1.MapLegacyResourceRoute(TeamFoundationHostType.All, aadUserToken, "Token", "AadUserTokens", "{area}/{resource}", defaultVersion1, releasedApiVersion: defaultVersion3, defaultApiVersion: defaultApiVersion1, routeName: "AadTokens-UserAccessTokens");
      HttpRouteCollection routes2 = routes;
      Guid aadAppToken = TokenResourceIds.AadAppToken;
      Version defaultVersion4 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion5 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion6 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion2 = defaultVersion5;
      routes2.MapLegacyResourceRoute(TeamFoundationHostType.All, aadAppToken, "Token", "AadAppTokens", "{area}/{resource}", defaultVersion4, releasedApiVersion: defaultVersion6, defaultApiVersion: defaultApiVersion2, routeName: "AadTokens-AppAccessTokens");
      HttpRouteCollection routes3 = routes;
      Guid accessToken = TokenResourceIds.AccessToken;
      Version defaultVersion7 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion8 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion9 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion3 = defaultVersion8;
      var defaults1 = new{ key = RouteParameter.Optional };
      routes3.MapLegacyResourceRoute(TeamFoundationHostType.All, accessToken, "Token", "AccessTokens", "{area}/{resource}/{key}", defaultVersion7, releasedApiVersion: defaultVersion9, defaultApiVersion: defaultApiVersion3, defaults: (object) defaults1, routeName: "DelegatedAuthorization-AccessToken");
      HttpRouteCollection routes4 = routes;
      Guid sessionToken = TokenResourceIds.SessionToken;
      Version defaultVersion10 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion11 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion12 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion4 = defaultVersion11;
      var defaults2 = new
      {
        authorizationId = RouteParameter.Optional
      };
      routes4.MapLegacyResourceRoute(TeamFoundationHostType.All, sessionToken, "Token", "SessionTokens", "{area}/{resource}/{authorizationId}", defaultVersion10, releasedApiVersion: defaultVersion12, defaultApiVersion: defaultApiVersion4, defaults: (object) defaults2, routeName: "DelegatedAuthorization-AccessTokenKey");
      HttpRouteCollection routes5 = routes;
      Guid appSessionToken = TokenResourceIds.AppSessionToken;
      Version defaultVersion13 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion14 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion15 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion5 = defaultVersion14;
      routes5.MapLegacyResourceRoute(TeamFoundationHostType.All, appSessionToken, "Token", "AppSessionTokens", "{area}/{resource}", defaultVersion13, releasedApiVersion: defaultVersion15, defaultApiVersion: defaultApiVersion5, routeName: "DelegatedAuthorization-IssueAppSessionToken");
      routes.MapResourceRoute(TeamFoundationHostType.All, DelegatedAuthResourceIds.Authorization, "DelegatedAuth", "Authorizations", "{area}/{resource}/{userId}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        userId = RouteParameter.Optional
      }, routeName: "DelegatedAuthorization-Authorizations");
      HttpRouteCollection routes6 = routes;
      Guid hostAuthorizeId = DelegatedAuthResourceIds.HostAuthorizeId;
      Version defaultVersion16 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion17 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion18 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion6 = defaultVersion17;
      routes6.MapLegacyResourceRoute(TeamFoundationHostType.All, hostAuthorizeId, "DelegatedAuth", "HostAuthorization", "{area}/{resource}", defaultVersion16, releasedApiVersion: defaultVersion18, defaultApiVersion: defaultApiVersion6, routeName: "DelegatedAuthorization-HostAuthorization");
      routes.MapResourceRoute(TeamFoundationHostType.All, DelegatedAuthResourceIds.Registration, "DelegatedAuth", "Registration", "{area}/{resource}/{registrationId}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 2, (object) new
      {
        registrationId = RouteParameter.Optional
      }, routeName: "DelegatedAuthorization-ClientRegistration");
      routes.MapResourceRoute(TeamFoundationHostType.All, DelegatedAuthResourceIds.RegistrationSecret, "DelegatedAuth", "RegistrationSecret", "{area}/{resource}/{registrationId}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        registrationId = RouteParameter.Optional
      }, routeName: "DelegatedAuthorization-RegistrationSecret");
      routes.MapLegacyResourceRoute(TeamFoundationHostType.All, OAuth2ResourceIds.Token, "oauth2", "token", "{area}/{resource}", RestApiResourceProvider.DefaultVersion, routeName: "OAuth2-Tokens");
      HttpRouteCollection routes7 = routes;
      Guid appTokenPair = TokenResourceIds.AppTokenPair;
      Version defaultVersion19 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion20 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion21 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion7 = defaultVersion20;
      routes7.MapLegacyResourceRoute(TeamFoundationHostType.Deployment, appTokenPair, "Token", "AppTokenPairs", "{area}/{resource}", defaultVersion19, releasedApiVersion: defaultVersion21, defaultApiVersion: defaultApiVersion7, routeName: "DelegatedAuthorization-AppTokenPair");
      HttpRouteCollection routes8 = routes;
      Guid pat = PublicPatTokenResourceIds.Pat;
      Version defaultVersion22 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion23 = RestApiResourceProvider.DefaultVersion;
      Version defaultVersion24 = RestApiResourceProvider.DefaultVersion;
      Version defaultApiVersion8 = defaultVersion23;
      var defaults3 = new{ id = RouteParameter.Optional };
      routes8.MapLegacyResourceRoute(TeamFoundationHostType.ProjectCollection, pat, "Tokens", "Pats", "{area}/{resource}/{id}", defaultVersion22, releasedApiVersion: defaultVersion24, defaultApiVersion: defaultApiVersion8, defaults: (object) defaults3);
    }
  }
}
