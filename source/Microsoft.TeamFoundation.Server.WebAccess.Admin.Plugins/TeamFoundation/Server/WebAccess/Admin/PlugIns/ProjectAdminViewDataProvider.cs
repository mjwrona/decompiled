// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectAdminViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProjectAdminViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProjectAdminView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectAdminViewData data = new ProjectAdminViewData();
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      project.PopulateProcessTemplateProperties(requestContext);
      int maxTeams = 100;
      TeamProjectModel teamProjectModel = new TeamProjectModel(requestContext, project, true, requestContext.ServiceHost.Name, maxTeams: maxTeams);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = teamProjectModel.Teams.Count >= maxTeams;
      data.ProjectVisibility = "TeamMembers";
      data.OrganizationName = "";
      if (requestContext.IsHosted())
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
        if (organization != null && organization.IsActivated)
        {
          data.ShowOrgVisibilityOption = true;
          data.OrganizationName = organization.Name;
        }
        data.ShowPublicVisibilityOption = this.ShowPublicVisibilityOption(requestContext);
        flag1 = ProjectAdminViewDataProvider.IsPublicVisibilityOptionEnabled(requestContext);
        flag2 = ProjectsUtility.AllowOrganizationProjects(requestContext);
      }
      if (data.ShowOrgVisibilityOption || data.ShowPublicVisibilityOption)
      {
        if (teamProjectModel.Visibility == ProjectVisibility.Organization)
        {
          data.ProjectVisibility = "EveryoneInTenant";
          data.ShowOrgVisibilityOption = true;
        }
        else if (teamProjectModel.Visibility == ProjectVisibility.Public)
        {
          data.ProjectVisibility = "Everyone";
          data.ShowPublicVisibilityOption = true;
        }
      }
      data.DisplayName = teamProjectModel.DisplayName;
      data.HasRenamePermission = teamProjectModel.HasRenamePermission;
      data.ProcessTemplateName = teamProjectModel.ProcessTemplateName;
      IProjectService service1 = requestContext.GetService<IProjectService>();
      ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid id = project.Id;
      string[] strArray = new string[1]
      {
        ProcessTemplateIdPropertyNames.ProcessTemplateType
      };
      IEnumerable<ProjectProperty> projectProperties = service1.GetProjectProperties(requestContext1, id, strArray);
      ProcessDescriptor descriptor = (ProcessDescriptor) null;
      data.isHostedXmlTemplate = projectProperties != null && projectProperties.Count<ProjectProperty>() > 0 && service2.TryGetProcessDescriptor(requestContext, new Guid((string) projectProperties.First<ProjectProperty>().Value), out descriptor) && descriptor.IsCustom;
      if (!object.Equals((object) Guid.Empty, (object) teamProjectModel.DefaultTeamId))
        data.IdentityImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, teamProjectModel.DefaultTeamId);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, project.Uri);
      string str = (string) null;
      try
      {
        str = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, project.Uri, true).TeamField.Name;
      }
      catch (InvalidProjectSettingsException ex)
      {
        requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", nameof (ProjectAdminViewDataProvider), (Exception) ex);
      }
      catch (MissingProjectSettingsException ex)
      {
        requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", nameof (ProjectAdminViewDataProvider), (Exception) ex);
      }
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        data.EditProjectOptionsJson = JsonConvert.SerializeObject((object) new
        {
          description = teamProjectModel.Description,
          name = teamProjectModel.DisplayName,
          projectId = teamProjectModel.ProjectId,
          projectVisibility = data.ProjectVisibility,
          hasRenamePermission = teamProjectModel.HasRenamePermission,
          hasGenericWritePermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.GenericWrite),
          hasUpdateVisibilityPermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.UpdateVisibility),
          showOrgVisibilityOption = data.ShowOrgVisibilityOption,
          showPublicVisibilityOption = data.ShowPublicVisibilityOption,
          isPublicVisibilityOptionEnabled = flag1,
          isOrgVisibilityOptionEnabled = flag2
        });
        data.ProjectOverviewOptionsJson = JsonConvert.SerializeObject((object) new
        {
          teams = teamProjectModel.Teams,
          defaultTeamId = teamProjectModel.DefaultTeamId,
          launchNewTeamDialog = false,
          hasMoreTeams = flag3,
          teamFieldName = str
        });
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        data.EditProjectOptionsJson = scriptSerializer.Serialize((object) new
        {
          description = teamProjectModel.Description,
          name = teamProjectModel.DisplayName,
          projectId = teamProjectModel.ProjectId,
          projectVisibility = data.ProjectVisibility,
          hasRenamePermission = teamProjectModel.HasRenamePermission,
          hasGenericWritePermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.GenericWrite),
          hasUpdateVisibilityPermission = securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.UpdateVisibility),
          showOrgVisibilityOption = data.ShowOrgVisibilityOption,
          showPublicVisibilityOption = data.ShowPublicVisibilityOption,
          isPublicVisibilityOptionEnabled = flag1,
          isOrgVisibilityOptionEnabled = flag2
        });
        data.ProjectOverviewOptionsJson = scriptSerializer.Serialize((object) new
        {
          teams = teamProjectModel.Teams,
          defaultTeamId = teamProjectModel.DefaultTeamId,
          launchNewTeamDialog = false,
          hasMoreTeams = flag3,
          teamFieldName = str
        });
      }
      return (object) data;
    }

    internal static bool IsPublicVisibilityOptionEnabled(IVssRequestContext requestContext) => requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowAnonymousAccess", false).EffectiveValue;

    private bool ShowPublicVisibilityOption(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>();
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess");
    }
  }
}
