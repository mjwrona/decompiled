// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.KeyVaultProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Security;

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  public static class KeyVaultProvider
  {
    public static string GetCredentialsForHosted(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = KeyVaultProvider.GetDeploymentRequestContext(requestContext);
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext, "ConfigurationSecrets", false);
      if (drawerId.Equals(Guid.Empty))
        return (string) null;
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentRequestContext, drawerId, "AzureStorageTableConnectionString");
      SecureString secureString = service.GetSecureString(deploymentRequestContext, itemInfo);
      return new NetworkCredential(string.Empty, secureString).Password;
    }

    private static IVssRequestContext GetDeploymentRequestContext(IVssRequestContext requestContext) => (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext : requestContext.To(TeamFoundationHostType.Deployment)).Elevate();
  }
}
