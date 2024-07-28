// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.OAuthEndpointHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.ConnectedService.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class OAuthEndpointHelper
  {
    public static bool IsWellKnownStrongBoxLocation(string lookupKey) => string.Equals(lookupKey, "useWellKnownStrongBoxLocation", StringComparison.OrdinalIgnoreCase);

    public static OAuthEndpointHelper.OAuthAccessToken GetWellKnownOAuthTokenFromStrongBox(
      IVssRequestContext requestContext,
      string drawerName)
    {
      return JsonUtilities.Deserialize<OAuthEndpointHelper.OAuthAccessToken>(new UserCredentialsManager(drawerName).GetStoredCredentials(requestContext, false));
    }

    public static OAuthEndpointHelper.OAuthAccessToken GetOAuthTokenFromStrongBox(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey)
    {
      return new OAuthEndpointHelper.OAuthAccessToken()
      {
        Type = "Token",
        Token = OAuthEndpointHelper.GetStrongBoxContent(requestContext, drawerName, lookupKey)
      };
    }

    public static bool TryGetWellKnownOAuthTokenFromStrongBox(
      IVssRequestContext requestContext,
      string drawerName,
      out OAuthEndpointHelper.OAuthAccessToken oAuthAccessToken)
    {
      if (JsonUtilities.TryDeserialize<OAuthEndpointHelper.OAuthAccessToken>(new UserCredentialsManager(drawerName).GetStoredCredentials(requestContext, false), out oAuthAccessToken))
        return true;
      oAuthAccessToken = (OAuthEndpointHelper.OAuthAccessToken) null;
      return false;
    }

    public static ServiceEndpoint CreateInMemoryServiceEndpoint(
      IVssRequestContext requestContext,
      string type,
      OAuthEndpointHelper.OAuthAccessToken credentials)
    {
      bool flag = string.Equals(type, "Bitbucket", StringComparison.OrdinalIgnoreCase) && requestContext.IsFeatureEnabled(BitbucketFeatureFlags.BitbucketAzurePipelinesOAuthClient);
      ServiceEndpoint memoryServiceEndpoint = new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = type,
        Authorization = new EndpointAuthorization()
        {
          Scheme = flag ? "OAuth2" : "OAuth"
        }
      };
      memoryServiceEndpoint.Authorization.Parameters["AccessToken"] = credentials.Token;
      memoryServiceEndpoint.Authorization.Parameters["AccessTokenType"] = credentials.Type;
      memoryServiceEndpoint.Authorization.Parameters["RefreshToken"] = credentials.RefreshToken;
      memoryServiceEndpoint.Authorization.Parameters["IdToken"] = credentials.InstallationId;
      if (flag)
        memoryServiceEndpoint.Authorization.Parameters["ConfigurationId"] = requestContext.IsFeatureEnabled(BitbucketFeatureFlags.BitbucketAzurePipelinesBackupOAuthClient) ? InternalAuthConfigurationConstants.BitbucketAzurePipelinesBackupOAuthAppId.ToString() : InternalAuthConfigurationConstants.BitbucketAzurePipelinesOAuthAppId.ToString();
      return memoryServiceEndpoint;
    }

    private static string GetStrongBoxContent(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, drawerName, lookupKey, false);
      if (itemInfo != null)
        return service.GetString(vssRequestContext, itemInfo);
      requestContext.TraceError(0, nameof (OAuthEndpointHelper), "When trying to read, failed to retrieve Repository List strong box item info for key '{0}', box '{1}'", (object) lookupKey, (object) drawerName);
      return (string) null;
    }

    public class OAuthAccessToken
    {
      public string Type { get; set; }

      public string Token { get; set; }

      public string RefreshToken { get; set; }

      public string Nonce { get; set; }

      public string InstallationId { get; set; }
    }
  }
}
