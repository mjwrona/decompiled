// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsProjectLevelDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsProjectLevelDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProjectLevelPermissions";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      PermissionsProjectLevelData data = (PermissionsProjectLevelData) null;
      try
      {
        ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        TeamFoundationSecurityService service = requestContext.GetService<TeamFoundationSecurityService>();
        IVssSecurityNamespace securityNamespace1 = service.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        string token1 = securityNamespace1.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace1, project.Uri);
        IVssSecurityNamespace securityNamespace2 = service.GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId);
        string token2 = securityNamespace2.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace2, project.Uri);
        data = new PermissionsProjectLevelData()
        {
          CanEditIdentityInformation = securityNamespace2.HasPermission(requestContext, token2, TeamProjectPermissions.GenericWrite),
          CanEditProjectLevelInformation = securityNamespace1.HasPermission(requestContext, token1, TeamProjectPermissions.GenericWrite)
        };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050087, "ProjectPermissions", "DataProvider", ex);
      }
      return (object) data;
    }
  }
}
