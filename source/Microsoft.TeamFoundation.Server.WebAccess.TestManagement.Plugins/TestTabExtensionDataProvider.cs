// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestTabExtensionDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestTabExtensionDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.TestTabExtensionDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project != null)
      {
        Guid id = project.Id;
        string securityToken1 = string.Format("/{0}/{1}/{2}", (object) "WorkItemsHub", (object) id, (object) "NewWorkItem");
        string securityToken2 = string.Format("/{0}/{1}/{2}", (object) "TestManagement", (object) id, (object) "TestManagementUserSettings");
        IClientSecurityProviderService service = requestContext.GetService<IClientSecurityProviderService>();
        service.AddPermissions(requestContext, providerContext.SharedData, WorkItemsHubSecurityConstants.NamespaceId, securityToken1);
        service.AddPermissions(requestContext, providerContext.SharedData, TestManagementSecurityNamespaceConstants.NamespaceId, securityToken2);
        string token = TeamProjectSecurityConstants.GetToken(CommonStructureUtils.GetProjectUri(id));
        service.AddPermissions(requestContext, providerContext.SharedData, FrameworkSecurity.TeamProjectNamespaceId, token);
      }
      requestContext.GetService<IClientFeatureProviderService>().AddFeatureState(requestContext, providerContext.SharedData, "ms.vss-work.agile");
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        requestContext.GetService<IClientLocationProviderService>().AddLocation(requestContext, providerContext.SharedData, TestManagementServerConstants.TCMServiceInstanceType, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      requestContext.GetService<IClientRouteProviderService>().AddRoute(requestContext, providerContext.SharedData, "ms.vss-releaseManagement-web.cd-release-progress-default-route");
      return (object) null;
    }
  }
}
