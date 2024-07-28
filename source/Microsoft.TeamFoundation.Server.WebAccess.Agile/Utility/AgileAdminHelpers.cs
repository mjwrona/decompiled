// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.AgileAdminHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Admin;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class AgileAdminHelpers
  {
    public static ProjectAdminWorkModel CreateProjectModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo)
    {
      int count;
      return new ProjectAdminWorkModel()
      {
        processName = AgileAdminHelpers.GetProcessName(requestContext, projectInfo.Id),
        areas = (object) new
        {
          mode = AreaIterationsMode.Areas,
          treeValues = AgileAdminHelpers.GetAreas(requestContext, projectInfo, out count),
          areaOwners = AgileAdminHelpers.GetAreaOwners(requestContext, projectInfo)
        },
        iterations = (object) new
        {
          mode = AreaIterationsMode.Iterations,
          treeValues = AgileAdminHelpers.GetIterations(requestContext, projectInfo, out count)
        }
      };
    }

    public static object GetAreas(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo,
      out int count)
    {
      return requestContext.GetService<ILegacyCssUtilsService>().GetTreeValues(requestContext, projectInfo.Name, TreeStructureType.Area, out count);
    }

    public static object GetIterations(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo,
      out int count)
    {
      return requestContext.GetService<ILegacyCssUtilsService>().GetTreeValues(requestContext, projectInfo.Name, TreeStructureType.Iteration, out count);
    }

    public static TeamViewModel CreateTeamViewModel(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam requestTeam)
    {
      ArgumentUtility.CheckForNull<WebApiTeam>(requestTeam, nameof (requestTeam));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(requestTeam.Identity, "Identity");
      TeamFoundationIdentity readIdentity = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        requestTeam.Identity.Descriptor
      }, MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local)[0];
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, requestTeam, false, true);
      ProjectProcessConfiguration processSettings1 = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, false);
      ITeamSettings settings = teamSettings;
      ProjectProcessConfiguration processSettings2 = processSettings1;
      return new TeamViewModel(readIdentity, settings, processSettings2);
    }

    public static object AddTeamFieldViewData(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration processSettings,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo,
      WebApiTeam team,
      ITeamSettings teamSettings)
    {
      object obj = (object) null;
      ITeamFieldSettings teamFieldSettings = teamSettings.TeamFieldConfig ?? (ITeamFieldSettings) new TeamFieldSettings();
      WebAccessWorkItemService service1 = requestContext.GetService<WebAccessWorkItemService>();
      ITeamService service2 = requestContext.GetService<ITeamService>();
      FieldDefinition field = (FieldDefinition) null;
      if (service1.TryGetFieldDefinitionByName(requestContext, processSettings.TeamField.Name, out field))
      {
        string str = WebContextFactory.GetCurrentRequestWebContext<TfsWebContext>().Url.Action("TeamField", "Work", (object) new
        {
          routeArea = "Admin",
          serviceHost = requestContext.ServiceHost,
          project = projectInfo?.Name,
          team = team?.Name
        });
        obj = (object) new Result()
        {
          listValues = service1.GetAllowedValues(requestContext, field.Id),
          teamField = new TeamField()
          {
            DefaultValueIndex = teamFieldSettings.DefaultValueIndex,
            TeamFieldValues = ((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).Select<ITeamFieldValue, TeamFieldValue>((Func<ITeamFieldValue, TeamFieldValue>) (tfv => new TeamFieldValue()
            {
              Value = tfv.Value,
              IncludeChildren = false
            }))
          },
          fieldName = field.Name,
          url = str,
          userHasTeamWritePermission = service2.UserIsTeamAdmin(requestContext, team.Identity)
        };
      }
      return obj;
    }

    public static Microsoft.TeamFoundation.Core.WebApi.ProjectInfo GetProjectInfo(
      IVssRequestContext requestContext)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
      if (pageSource?.Project != null)
        return requestContext.GetService<IProjectService>().GetProject(requestContext, pageSource.Project.Id);
      throw new ProjectNotFoundException();
    }

    public static string GetProcessName(IVssRequestContext requestContext, Guid projectId)
    {
      if (!requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") && !requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload"))
        return (string) null;
      ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
      requestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, projectId, out processDescriptor);
      return processDescriptor?.Name;
    }

    public static IDictionary<string, IDictionary<string, TeamFieldProperty>> GetAreaOwners(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo)
    {
      Dictionary<string, IDictionary<string, TeamFieldProperty>> areaOwners = new Dictionary<string, IDictionary<string, TeamFieldProperty>>();
      if (requestContext.IsFeatureEnabled("Admin.WorkHub.DisplayAreaOwners"))
      {
        if (AgileAdminHelpers.IsTeamFieldAreaPath(requestContext, projectInfo))
        {
          try
          {
            IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> idToTeamMappings = requestContext.GetService<TeamConfigurationService>().GetAreaIdToTeamMappings(requestContext, projectInfo.Id);
            foreach (Guid key1 in (IEnumerable<Guid>) idToTeamMappings.Keys)
            {
              IDictionary<Guid, TeamFieldProperty> source = idToTeamMappings[key1];
              string key2 = Convert.ToString((object) key1);
              Dictionary<string, TeamFieldProperty> dictionary = source.ToDictionary<KeyValuePair<Guid, TeamFieldProperty>, string, TeamFieldProperty>((Func<KeyValuePair<Guid, TeamFieldProperty>, string>) (item => Convert.ToString((object) item.Key)), (Func<KeyValuePair<Guid, TeamFieldProperty>, TeamFieldProperty>) (item => item.Value));
              areaOwners.Add(key2, (IDictionary<string, TeamFieldProperty>) dictionary);
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(290611, "", "", ex);
          }
        }
      }
      return (IDictionary<string, IDictionary<string, TeamFieldProperty>>) areaOwners;
    }

    public static bool IsTeamFieldAreaPath(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo)
    {
      bool flag = false;
      try
      {
        if (requestContext.IsTeamFieldAreaPath(projectInfo))
          flag = true;
      }
      catch (ProjectSettingsException ex)
      {
        flag = true;
      }
      return flag;
    }
  }
}
