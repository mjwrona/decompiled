// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.TeamConfigurationWebServiceUtil
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  internal class TeamConfigurationWebServiceUtil
  {
    private const string s_area = "Agile";
    private const string s_layer = "TeamConfigurationWebServiceUtil";

    public TeamConfiguration[] GetTeamConfigurations(
      IVssRequestContext requestContext,
      Guid[] teamIds)
    {
      using (requestContext.TraceBlock(290503, 290504, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetTeamConfigurations)))
      {
        ArgumentUtility.CheckForNull<Guid[]>(teamIds, nameof (teamIds));
        ITeamService service = requestContext.GetService<ITeamService>();
        IList<WebApiTeam> webApiTeamList = (IList<WebApiTeam>) new List<WebApiTeam>(teamIds.Length);
        using (requestContext.TraceBlock(290505, 290506, "Agile", nameof (TeamConfigurationWebServiceUtil), "GetTeamConfigurations.ReadingTeams"))
        {
          foreach (Guid teamGuid in ((IEnumerable<Guid>) teamIds).Distinct<Guid>())
          {
            WebApiTeam teamByGuid = service.GetTeamByGuid(requestContext, teamGuid);
            webApiTeamList.Add(teamByGuid);
          }
        }
        IDictionary<string, Guid> defaultTeamIds = TeamConfigurationWebServiceUtil.GetDefaultTeamIds(requestContext, webApiTeamList.Where<WebApiTeam>((Func<WebApiTeam, bool>) (team => team != null)).Select<WebApiTeam, string>((Func<WebApiTeam, string>) (team => Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(team.ProjectId))));
        return this.GetTeamConfigurationsInternal(requestContext, (IEnumerable<WebApiTeam>) webApiTeamList, defaultTeamIds);
      }
    }

    public TeamConfiguration[] GetTeamConfigurationsForUser(
      IVssRequestContext requestContext,
      string[] projectUris)
    {
      using (requestContext.TraceBlock(290507, 290508, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetTeamConfigurationsForUser)))
      {
        TeamConfiguration[] configurationsForUser = Array.Empty<TeamConfiguration>();
        if (((IEnumerable<string>) projectUris).Any<string>())
        {
          IList<WebApiTeam> teams = (IList<WebApiTeam>) requestContext.TraceBlock<List<WebApiTeam>>(290509, 290510, 290511, "Agile", nameof (TeamConfigurationWebServiceUtil), "GetTeamConfigurationsForUser.QueryTeams", (Func<List<WebApiTeam>>) (() => requestContext.GetService<ITeamService>().QueryMyTeamsInCollection(requestContext, requestContext.UserContext).Where<WebApiTeam>((Func<WebApiTeam, bool>) (team => ((IEnumerable<string>) projectUris).Contains<string>(Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(team.ProjectId), (IEqualityComparer<string>) TFStringComparer.ProjectString))).ToList<WebApiTeam>()));
          IDictionary<string, Guid> defaultTeamIds = TeamConfigurationWebServiceUtil.GetDefaultTeamIds(requestContext, (IEnumerable<string>) projectUris);
          if (!requestContext.IsFeatureEnabled("VisualStudio.TeamExplorer.NoImplicitDefaultTeamAccess"))
            teams = TeamConfigurationWebServiceUtil.AddDefaultTeams(requestContext, teams, defaultTeamIds);
          configurationsForUser = this.GetTeamConfigurationsInternal(requestContext, (IEnumerable<WebApiTeam>) teams, defaultTeamIds);
        }
        else
          requestContext.Trace(290402, TraceLevel.Info, "Agile", nameof (TeamConfigurationWebServiceUtil), "No projectUris");
        return configurationsForUser;
      }
    }

    private static IList<WebApiTeam> AddDefaultTeams(
      IVssRequestContext requestContext,
      IList<WebApiTeam> teams,
      IDictionary<string, Guid> defaultTeamIds)
    {
      return requestContext.TraceBlock<IList<WebApiTeam>>(290512, 290513, 290514, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (AddDefaultTeams), (Func<IList<WebApiTeam>>) (() =>
      {
        HashSet<Guid> teamIds = new HashSet<Guid>();
        foreach (Guid guid in (IEnumerable<Guid>) defaultTeamIds.Values)
        {
          if (guid != Guid.Empty)
            teamIds.Add(guid);
        }
        foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) teams)
          teamIds.Remove(team.Id);
        if (teamIds.Count > 0)
          teams = (IList<WebApiTeam>) teams.Union<WebApiTeam>((IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().GetTeamsByGuid(requestContext, (IEnumerable<Guid>) teamIds)).ToList<WebApiTeam>();
        return teams;
      }));
    }

    public void SetTeamSettings(
      IVssRequestContext requestContext,
      Guid teamId,
      TeamSettings teamSettings)
    {
      using (requestContext.TraceBlock(290515, 290516, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (SetTeamSettings)))
      {
        ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
        ArgumentUtility.CheckForNull<TeamSettings>(teamSettings, nameof (teamSettings));
        TeamConfigurationHelper.SetTeamConfiguration(requestContext, teamId, teamSettings.BacklogIterationPath, teamSettings.TeamFieldValues, teamSettings.IterationPaths);
      }
    }

    internal static IDictionary<string, Guid> GetDefaultTeamIds(
      IVssRequestContext requestContext,
      IEnumerable<string> projectUris)
    {
      using (requestContext.TraceBlock(290517, 290518, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetDefaultTeamIds)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "tfsRequestContext");
        ArgumentUtility.CheckForNull<IEnumerable<string>>(projectUris, nameof (projectUris));
        IProjectService service = requestContext.GetService<IProjectService>();
        IDictionary<string, Guid> dictionary = (IDictionary<string, Guid>) projectUris.Distinct<string>().ToDictionary<string, string, Guid>((Func<string, string>) (uri => uri), (Func<string, Guid>) (uri => Guid.Empty), (IEqualityComparer<string>) TFStringComparer.ProjectUri);
        IVssRequestContext requestContext1 = requestContext;
        foreach (Microsoft.TeamFoundation.Core.WebApi.ProjectInfo populateProperty in service.GetProjects(requestContext1, ProjectState.WellFormed).PopulateProperties(requestContext, TeamConstants.DefaultTeamPropertyName))
        {
          if (dictionary.ContainsKey(populateProperty.Uri))
          {
            Microsoft.TeamFoundation.Core.WebApi.ProjectProperty projectProperty = populateProperty.Properties.FirstOrDefault<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty>();
            Guid result;
            if (projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result))
              dictionary[populateProperty.Uri] = result;
          }
        }
        return dictionary;
      }
    }

    internal virtual void PopulateTeamConfigurationIterations(
      IVssRequestContext requestContext,
      ICollection<TeamConfiguration> teamConfigurations,
      ICollection<Guid> nodeIds)
    {
      using (requestContext.TraceBlock(290519, 290520, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (PopulateTeamConfigurationIterations)))
      {
        Dictionary<Guid, CommonStructureNodeInfo> dictionary = requestContext.GetService<ILegacyCssUtilsService>().GetNodes(requestContext, (IEnumerable<Guid>) nodeIds).ToDictionary<CommonStructureNodeInfo, Guid>((Func<CommonStructureNodeInfo, Guid>) (node => node.GetId()));
        foreach (TeamConfiguration teamConfiguration in (IEnumerable<TeamConfiguration>) teamConfigurations)
          teamConfiguration?.TeamSettings.PopulateIterations(requestContext, teamConfiguration.ProjectUri, (IDictionary<Guid, CommonStructureNodeInfo>) dictionary);
      }
    }

    private TeamConfiguration[] GetTeamConfigurationsInternal(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams,
      IDictionary<string, Guid> defaultTeamIds)
    {
      using (requestContext.TraceBlock(290521, 290522, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetTeamConfigurationsInternal)))
      {
        ArgumentUtility.CheckForNull<IEnumerable<WebApiTeam>>(teams, nameof (teams));
        ArgumentUtility.CheckForNull<IDictionary<string, Guid>>(defaultTeamIds, nameof (defaultTeamIds));
        ISet<Guid> nodeIds = (ISet<Guid>) new HashSet<Guid>();
        IList<TeamConfiguration> source = (IList<TeamConfiguration>) new List<TeamConfiguration>();
        ISet<string> projectUris = (ISet<string>) new HashSet<string>(teams.Where<WebApiTeam>((Func<WebApiTeam, bool>) (team => team != null)).Select<WebApiTeam, string>((Func<WebApiTeam, string>) (team => Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(team.ProjectId))), (IEqualityComparer<string>) TFStringComparer.ProjectUri);
        IDictionary<string, string> teamFields = this.GetTeamFields(requestContext, (IEnumerable<string>) projectUris);
        IDictionary<Guid, ITeamSettings> teamSettingsInBulk = this.GetTeamSettingsInBulk(requestContext, teams);
        foreach (WebApiTeam team in teams)
        {
          TeamConfiguration teamConfiguration = (TeamConfiguration) null;
          if (team != null)
          {
            try
            {
              string projectUri = Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(team.ProjectId);
              string teamField = teamFields[projectUri];
              bool isDefaultTeam = defaultTeamIds.ContainsKey(projectUri) && defaultTeamIds[projectUri] == team.Id;
              ITeamSettings teamSettings = teamSettingsInBulk[team.Id];
              teamConfiguration = new TeamConfiguration(requestContext, team, teamSettings, teamField, isDefaultTeam);
              nodeIds.Add(teamSettings.BacklogIterationId);
              foreach (ITeamIteration iteration in (IEnumerable<ITeamIteration>) teamSettings.Iterations)
                nodeIds.Add(iteration.IterationId);
            }
            catch (UnauthorizedAccessException ex)
            {
            }
          }
          source.Add(teamConfiguration);
        }
        this.PopulateTeamConfigurationIterations(requestContext, (ICollection<TeamConfiguration>) source, (ICollection<Guid>) nodeIds);
        return source.ToArray<TeamConfiguration>();
      }
    }

    private IDictionary<Guid, ITeamSettings> GetTeamSettingsInBulk(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams)
    {
      using (requestContext.TraceBlock(290523, 290524, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetTeamSettingsInBulk)))
        return (IDictionary<Guid, ITeamSettings>) requestContext.GetService<ITeamConfigurationService>().GetTeamSettingsInBulkWithoutProperties(requestContext, teams.Where<WebApiTeam>((Func<WebApiTeam, bool>) (team => team != null))).ToDictionary<KeyValuePair<WebApiTeam, ITeamSettings>, Guid, ITeamSettings>((Func<KeyValuePair<WebApiTeam, ITeamSettings>, Guid>) (setting => setting.Key.Id), (Func<KeyValuePair<WebApiTeam, ITeamSettings>, ITeamSettings>) (setting => setting.Value));
    }

    private IDictionary<string, string> GetTeamFields(
      IVssRequestContext requestContext,
      IEnumerable<string> projectUris)
    {
      using (requestContext.TraceBlock(290525, 290526, "Agile", nameof (TeamConfigurationWebServiceUtil), nameof (GetTeamFields)))
      {
        ArgumentUtility.CheckForNull<IEnumerable<string>>(projectUris, nameof (projectUris));
        return requestContext.GetService<ProjectConfigurationService>().GetTeamFieldsForProjects(requestContext, projectUris);
      }
    }
  }
}
