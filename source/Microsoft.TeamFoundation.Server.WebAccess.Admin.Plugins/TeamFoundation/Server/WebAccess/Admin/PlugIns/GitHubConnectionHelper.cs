// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.GitHubConnectionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Users.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public static class GitHubConnectionHelper
  {
    private const string c_gitHubConnectCollectionPropertyName = "AllowGitHubInvitations";
    private const int expirationInMinutes = 10;
    private const string c_gitHubConnectUrlRegistryPath = "/Configuration/AdminEng/GitHubConnectAppUrl";
    private const string c_defaultGitHubConnectUrl = "https://github.com/apps/azure-devops-connector/installations/new";

    public static void SetUserContinuationToken(IVssRequestContext requestContext)
    {
      DateTime dateTime = DateTime.UtcNow.AddMinutes(10.0);
      string attributeValue = JsonConvert.SerializeObject((object) new
      {
        collectionId = requestContext.ServiceHost.InstanceId,
        expiration = dateTime.ToString("o")
      });
      Guid id = requestContext.GetUserIdentity().Id;
      requestContext.GetService<IUserService>().SetAttribute(requestContext, id, "githubconnect.wizard-continuation-token", attributeValue);
    }

    public static bool DetermineInviteSet(Collection collection)
    {
      bool result = false;
      string str;
      if (collection.Properties.TryGetValue<string>("AllowGitHubInvitations", out str) && !bool.TryParse(str, out result))
        result = false;
      return result;
    }

    public static bool DetermineCanLink(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Organization.Organization organization)
    {
      return OrganizationAdminDataProviderHelper.GetModifyPermissionBits(requestContext.To(TeamFoundationHostType.Application), organization.Id) == 16;
    }

    public static Microsoft.VisualStudio.Services.Organization.Organization GetOrganization(
      IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      return context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
    }

    public static Collection GetCollection(IVssRequestContext requestContext) => requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new List<string>()
    {
      "AllowGitHubInvitations"
    });

    public static string GetGitHubConnectUrl(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/AdminEng/GitHubConnectAppUrl", "https://github.com/apps/azure-devops-connector/installations/new");
    }
  }
}
