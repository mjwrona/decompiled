// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.BitbucketAzurePipelinesOAuthApp
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal static class BitbucketAzurePipelinesOAuthApp
  {
    private static readonly string SecretLookupKey = BitbucketConstants.StrongBox.PrimaryAzurePipelinesRegistrationLookupKey;
    private const string DrawerName = "ConfigurationSecrets";

    public static AuthConfiguration Initialize(
      IVssRequestContext requestContext,
      bool secretsRequired = false)
    {
      AuthConfiguration authConfiguration1 = new AuthConfiguration();
      authConfiguration1.Id = InternalAuthConfigurationConstants.BitbucketAzurePipelinesOAuthAppId;
      authConfiguration1.Url = InternalAuthConfigurationConstants.BitbucketUri;
      authConfiguration1.Name = InternalAuthConfigurationConstants.BitbucketAzurePipelinesOAuthAppName;
      authConfiguration1.EndpointType = "Bitbucket";
      AuthConfiguration authConfiguration2 = authConfiguration1;
      try
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRequestContext vssRequestContext = requestContext1.Elevate();
        StrongBoxItemInfo itemInfo = vssRequestContext.GetService<ITeamFoundationStrongBoxService>().GetItemInfo(vssRequestContext, "ConfigurationSecrets", BitbucketAzurePipelinesOAuthApp.SecretLookupKey, true);
        authConfiguration2.ClientId = itemInfo != null ? itemInfo.CredentialName : throw new ApplicationException(ServiceEndpointResources.BitbucketNotRegisteredError());
        if (secretsRequired)
          authConfiguration2.ClientSecret = ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext1, itemInfo);
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        throw new ApplicationException(ServiceEndpointResources.DeploymentNotRegisteredWithBitbucket((object) authConfiguration2.Name, (object) ex.Message));
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw new ApplicationException(ServiceEndpointResources.DeploymentNotProperlyRegisteredWithBitbucket((object) authConfiguration2.Name, (object) ex.Message));
      }
      return authConfiguration2;
    }
  }
}
