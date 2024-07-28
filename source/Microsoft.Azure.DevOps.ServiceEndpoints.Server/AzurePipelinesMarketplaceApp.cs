// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.AzurePipelinesMarketplaceApp
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal static class AzurePipelinesMarketplaceApp
  {
    private const string PrimaryClientSecretKey = "GitHubAzurePipelinesAppClientSecretPrimary";
    private const string SecondaryClientSecretKey = "GitHubAzurePipelinesAppClientSecretSecondary";
    private const string DrawerName = "ConfigurationSecrets";
    private const string UsePrimarySecretKey = "/Configuration/GitHubAzurePipelinesApp/UsePrimaryGitHubCredentials";

    public static AuthConfiguration Initialize(
      IVssRequestContext requestContext,
      bool secretsRequired = false)
    {
      if (!requestContext.IsFeatureEnabled("ServiceEndpoints.EnableAzurePipelinesMarketplaceAppId"))
        return (AuthConfiguration) null;
      AuthConfiguration authConfiguration1 = new AuthConfiguration();
      authConfiguration1.Name = InternalAuthConfigurationConstants.AzurePipelinesMarketplaceAppName;
      authConfiguration1.Id = InternalAuthConfigurationConstants.AzurePipelinesMarketplaceAppId;
      authConfiguration1.Url = InternalAuthConfigurationConstants.GitHubUri;
      authConfiguration1.EndpointType = "GitHub";
      AuthConfiguration authConfiguration2 = authConfiguration1;
      try
      {
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        int num = vssRequestContext1.GetService<CachedRegistryService>().GetValue<bool>(vssRequestContext1, (RegistryQuery) "/Configuration/GitHubAzurePipelinesApp/UsePrimaryGitHubCredentials", true) ? 1 : 0;
        IVssRequestContext vssRequestContext2 = vssRequestContext1.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext2.GetService<ITeamFoundationStrongBoxService>();
        string lookupKey = num != 0 ? "GitHubAzurePipelinesAppClientSecretPrimary" : "GitHubAzurePipelinesAppClientSecretSecondary";
        if (secretsRequired)
        {
          StrongBoxItemInfo itemInfo1 = service.GetItemInfo(vssRequestContext2, "ConfigurationSecrets", lookupKey, true);
          StrongBoxItemInfo itemInfo2 = service.GetItemInfo(vssRequestContext2, "ConfigurationSecrets", GitHubConstants.StrongBoxKey.GitHubLaunchPrimary, true);
          authConfiguration2.ClientId = itemInfo1.CredentialName;
          authConfiguration2.ClientSecret = service.GetString(vssRequestContext2, itemInfo1);
          authConfiguration2.Parameters.Add("GitHubLaunchPrimaryAppId", (Parameter) itemInfo2.CredentialName);
          authConfiguration2.Parameters.Add("GitHubLaunchPrimaryPrivateKey", (Parameter) service.GetString(vssRequestContext2, itemInfo2));
        }
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
