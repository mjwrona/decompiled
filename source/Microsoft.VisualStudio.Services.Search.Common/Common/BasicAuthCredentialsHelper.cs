// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.BasicAuthCredentialsHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  internal class BasicAuthCredentialsHelper
  {
    internal virtual KeyValuePair<string, string> GetCredentials(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return this.GetCredentialsForOnPrem(requestContext);
      return requestContext.ExecutionEnvironment.IsHostedDeployment ? this.GetCredentialsForHosted(requestContext) : new KeyValuePair<string, string>();
    }

    private KeyValuePair<string, string> GetCredentialsForOnPrem(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = BasicAuthCredentialsHelper.GetDeploymentRequestContext(requestContext);
      string key = deploymentRequestContext.GetService<IVssRegistryService>().GetValue<string>(deploymentRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/ElasticsearchUser", string.Empty);
      string empty = string.Empty;
      if (!string.IsNullOrEmpty(key))
      {
        TeamFoundationStrongBoxService service = deploymentRequestContext.GetService<TeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(deploymentRequestContext, "ElasticsearchPasswordDrawer", false);
        if (!drawerId.Equals(Guid.Empty))
          empty = service.GetString(deploymentRequestContext, drawerId, "ElasticsearchPassword");
        if (!string.IsNullOrEmpty(empty))
        {
          string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(empty));
          return new KeyValuePair<string, string>(key, base64String);
        }
      }
      return new KeyValuePair<string, string>();
    }

    private KeyValuePair<string, string> GetCredentialsForHosted(IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = BasicAuthCredentialsHelper.GetDeploymentRequestContext(requestContext);
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext, "ConfigurationSecrets", false);
      if (!drawerId.Equals(Guid.Empty))
      {
        StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentRequestContext, drawerId, "ElasticSearchHostedAuthPassword");
        string credentialName = itemInfo.CredentialName;
        if (!string.IsNullOrEmpty(credentialName))
        {
          SecureString secureString = service.GetSecureString(deploymentRequestContext, itemInfo);
          string password = new NetworkCredential(string.Empty, secureString).Password;
          if (!string.IsNullOrEmpty(password))
          {
            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            return new KeyValuePair<string, string>(credentialName, base64String);
          }
        }
      }
      return new KeyValuePair<string, string>();
    }

    private static IVssRequestContext GetDeploymentRequestContext(IVssRequestContext requestContext) => (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext : requestContext.To(TeamFoundationHostType.Deployment)).Elevate();
  }
}
