// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectAdminOverviewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProjectAdminOverviewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProjectAdminOverview";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      project.PopulateProcessTemplateProperties(requestContext);
      TeamProjectModel teamProjectModel = new TeamProjectModel(requestContext, project, false, requestContext.ServiceHost.Name, false);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, project.Uri);
      IClientLocationProviderService service = requestContext.GetService<IClientLocationProviderService>();
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.Application));
      ProjectAdminOverviewData data;
      try
      {
        data = new ProjectAdminOverviewData()
        {
          DisplayName = teamProjectModel.DisplayName,
          ProcessTemplateName = teamProjectModel.ProcessTemplateName,
          ProjectVisibility = "TeamMembers",
          EditProjectOptions = new ProjectEditOptionsData()
          {
            Description = teamProjectModel.Description,
            Name = teamProjectModel.DisplayName,
            ProjectId = teamProjectModel.ProjectId,
            HasRenamePermission = teamProjectModel.HasRenamePermission,
            HasGenericWritePermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.GenericWrite),
            HasUpdateVisibilityPermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.UpdateVisibility)
          },
          DeleteProjectOptions = new ProjectDeleteOptionsData()
          {
            ProjectId = teamProjectModel.ProjectId,
            HasDeletePermission = teamProjectModel.HasDeletePermission
          }
        };
        if (requestContext.IsHosted())
        {
          data.EditProjectOptions.IsPublicVisibilityOptionEnabled = ProjectAdminViewDataProvider.IsPublicVisibilityOptionEnabled(requestContext) && ProjectsUtility.AllowPublicProjects(requestContext);
          data.EditProjectOptions.IsOrgVisibilityOptionEnabled = ProjectsUtility.AllowOrganizationProjects(requestContext);
          requestContext.GetService<IVssRegistryService>();
          data.EditProjectOptions.ShowPublicVisibilityOption = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess");
          if (ProjectsUtility.AllowProjectVisibilityForMicrosoftTenant(requestContext))
            data.EditProjectOptions.ShowOrgVisibilityOption = true;
          if (!data.EditProjectOptions.ShowOrgVisibilityOption)
          {
            if (!data.EditProjectOptions.ShowPublicVisibilityOption)
              goto label_11;
          }
          if (teamProjectModel.Visibility == ProjectVisibility.Organization)
          {
            data.ProjectVisibility = "EveryoneInTenant";
            data.EditProjectOptions.ShowOrgVisibilityOption = true;
          }
          else if (teamProjectModel.Visibility == ProjectVisibility.Public)
          {
            data.ProjectVisibility = "Everyone";
            data.EditProjectOptions.ShowPublicVisibilityOption = true;
          }
        }
      }
      catch (Exception ex)
      {
        data = (ProjectAdminOverviewData) null;
        requestContext.TraceException(10050070, "ProjectOverview", "DataProvider", ex);
      }
label_11:
      return (object) data;
    }
  }
}
