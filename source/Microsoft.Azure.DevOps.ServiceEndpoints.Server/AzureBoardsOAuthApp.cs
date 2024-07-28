// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.AzureBoardsOAuthApp
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal static class AzureBoardsOAuthApp
  {
    private const string DrawerName = "ConfigurationSecrets";
    private const string UsePrimarySecretKey = "/Configuration/GitHub/UsePrimaryGitHubBoardsCredentials";
    private const string PrimaryClientSecretKey = "GitHubBoardsClientSecretPrimary";
    private const string SecondaryClientSecretKey = "GitHubBoardsClientSecretSecondary";

    public static AuthConfiguration Initialize(
      IVssRequestContext requestContext,
      bool secretsRequired = false)
    {
      if (!requestContext.IsFeatureEnabled("ServiceEndpoints.EnableAzureBoardsOAuthAppId"))
        return (AuthConfiguration) null;
      AuthConfiguration authConfiguration1 = new AuthConfiguration();
      authConfiguration1.Id = InternalAuthConfigurationConstants.AzureBoardsOAuthAppId;
      authConfiguration1.Url = InternalAuthConfigurationConstants.GitHubUri;
      authConfiguration1.Name = InternalAuthConfigurationConstants.AzureBoardsOAuthAppName;
      authConfiguration1.EndpointType = "GitHub";
      AuthConfiguration authConfiguration2 = authConfiguration1;
      try
      {
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        int num = vssRequestContext1.GetService<CachedRegistryService>().GetValue<bool>(vssRequestContext1, (RegistryQuery) "/Configuration/GitHub/UsePrimaryGitHubBoardsCredentials", true) ? 1 : 0;
        IVssRequestContext vssRequestContext2 = vssRequestContext1.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext2.GetService<ITeamFoundationStrongBoxService>();
        string lookupKey = num != 0 ? "GitHubBoardsClientSecretPrimary" : "GitHubBoardsClientSecretSecondary";
        StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext2, "ConfigurationSecrets", lookupKey, true);
        authConfiguration2.ClientId = itemInfo.CredentialName;
        if (secretsRequired)
          authConfiguration2.ClientSecret = service.GetString(vssRequestContext2, itemInfo);
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        throw new ApplicationException(ServiceEndpointResources.DeploymentNotRegisteredWithGitHub((object) authConfiguration2.Name, (object) ex.Message));
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw new ApplicationException(ServiceEndpointResources.DeploymentNotProperlyRegisteredWithGitHub((object) authConfiguration2.Name, (object) ex.Message));
      }
      return authConfiguration2;
    }
  }
}
