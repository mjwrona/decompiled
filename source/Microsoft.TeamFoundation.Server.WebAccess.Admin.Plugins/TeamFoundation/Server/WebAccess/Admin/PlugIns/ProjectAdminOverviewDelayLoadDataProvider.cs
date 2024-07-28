// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectAdminOverviewDelayLoadDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProjectAdminOverviewDelayLoadDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProjectAdminOverviewDelayLoad";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectAdminOverviewDelayLoadData data = new ProjectAdminOverviewDelayLoadData();
      string empty = string.Empty;
      if (providerContext.Properties.ContainsKey("projectId") && providerContext.Properties["projectId"] != null)
        empty = providerContext.Properties["projectId"].ToString();
      if (string.IsNullOrEmpty(empty))
      {
        requestContext.Trace(10050062, TraceLevel.Error, "ProjectOverview", "DataProvider", "Project ID is null.");
        return (object) null;
      }
      try
      {
        Guid defaultTeamId = this.GetDefaultTeamId(requestContext, new Guid(empty));
        data.DefaultTeamId = defaultTeamId;
        data.IsProjectImageSet = this.IsProjectImageSet(requestContext, defaultTeamId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050071, "ProjectOverview", "DataProvider", ex);
      }
      if (requestContext.IsHosted())
      {
        try
        {
          TeamFoundationIdentity administratorsGroup = this.GetProjectAdministratorsGroup(requestContext);
          if (administratorsGroup != null)
          {
            if (administratorsGroup.IsContainer)
            {
              data.ProjectAdmins = new ProjectAdministratorsData();
              TeamFoundationFilteredIdentitiesList filteredIdentitiesList = new TeamFoundationFilteredIdentitiesList();
              TeamFoundationFilteredIdentitiesList filteredIdentities = this.ReadProjectAdminMembers(requestContext, administratorsGroup.TeamFoundationId);
              data.ProjectAdmins = ProjectAdminOverviewDelayLoadDataProvider.getFilteredIdentitiesList(requestContext, filteredIdentities);
              data.ProjectAdmins.GroupDescriptor = (string) administratorsGroup.Descriptor.ToSubjectDescriptor(requestContext);
              data.ProjectAdmins.CanAddMemberToAdminGroup = this.CanAddMemberToGroup(requestContext, administratorsGroup.TeamFoundationId);
              Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, Enumerable.Empty<string>());
              data.ProjectAdmins.CollectionName = collection.Name;
              IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
              string str = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, collection.Id, ServiceInstanceTypes.TFS)?.ToString();
              data.ProjectAdmins.OrgSettingsUrl = str + "_settings";
            }
          }
        }
        catch (Exception ex)
        {
          data.ProjectAdmins = (ProjectAdministratorsData) null;
          requestContext.TraceException(10050069, "ProjectOverview", "DataProvider", ex);
        }
      }
      return (object) data;
    }

    private Guid GetDefaultTeamId(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<ITeamService>().GetDefaultTeamId(requestContext, projectId);

    private bool IsProjectImageSet(IVssRequestContext requestContext, Guid teamId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      string name = "Microsoft.TeamFoundation.Identity.Image.Id";
      IVssRequestContext requestContext1 = requestContext;
      Guid[] identityIds = new Guid[1]{ teamId };
      string[] propertyNameFilters = new string[1]{ name };
      object obj;
      service.ReadIdentities(requestContext1, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) propertyNameFilters).Single<Microsoft.VisualStudio.Services.Identity.Identity>().TryGetProperty(name, out obj);
      return obj != null;
    }

    private TeamFoundationIdentity GetProjectAdministratorsGroup(IVssRequestContext requestContext)
    {
      TeamFoundationIdentity[] array = ((IEnumerable<TeamFoundationIdentity>) TfsAdminIdentityHelper.ListScopedApplicationGroupsForProject(requestContext)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null && IdentityHelper.IsWellKnownGroup(x.Descriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))).ToArray<TeamFoundationIdentity>();
      return !((IEnumerable<TeamFoundationIdentity>) array).Any<TeamFoundationIdentity>() ? (TeamFoundationIdentity) null : array[0];
    }

    private TeamFoundationFilteredIdentitiesList ReadProjectAdminMembers(
      IVssRequestContext requestContext,
      Guid scope)
    {
      return requestContext.GetService<TeamFoundationIdentityService>().ReadFilteredIdentitiesById(requestContext, new Guid[1]
      {
        scope
      }, IdentityManagementHelpers.GetPageSize(new int?()), (IEnumerable<IdentityFilter>) new List<IdentityFilter>(), (string) null, true, MembershipQuery.Direct, MembershipQuery.None, true, (IEnumerable<string>) null);
    }

    private static ProjectAdministratorsData getFilteredIdentitiesList(
      IVssRequestContext requestContext,
      TeamFoundationFilteredIdentitiesList filteredIdentities,
      bool filterServiceIdentities = false)
    {
      IdentityViewData identityViewData = TfsAdminIdentityHelper.JsonFromFilteredIdentitiesList(requestContext, filteredIdentities, filterServiceIdentities);
      return new ProjectAdministratorsData()
      {
        Identities = identityViewData.Identities,
        HasMore = identityViewData.HasMore,
        TotalIdentityCount = identityViewData.TotalIdentityCount
      };
    }

    private bool CanAddMemberToGroup(
      IVssRequestContext requestContext,
      Guid groupId,
      bool isOrganizationLevel = false)
    {
      if (ProjectAdminOverviewDelayLoadDataProvider.ShouldElevateToOrganization(isOrganizationLevel, requestContext))
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentity[] source = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        groupId
      });
      TeamFoundationIdentity groupIdentity = ((IEnumerable<TeamFoundationIdentity>) source).Any<TeamFoundationIdentity>() ? source[0] : (TeamFoundationIdentity) null;
      return ProjectAdminOverviewDelayLoadDataProvider.HasManageGroupMembershipPermission(requestContext, groupIdentity, true);
    }

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }

    private static bool HasManageGroupMembershipPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity,
      bool alwaysAllowAdministrators = false)
    {
      if (groupIdentity == null)
        return false;
      return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? requestContext.GetClient<SecurityHttpClient>(ServiceInstanceTypes.SPS).HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, IdentityUtil.CreateSecurityToken(groupIdentity), 8, alwaysAllowAdministrators).Result : GroupHelpers.HasManageGroupMembershipPermission(requestContext, groupIdentity, alwaysAllowAdministrators);
    }
  }
}
