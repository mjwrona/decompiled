// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class TeamConfigurationService : ITeamConfigurationService, IVssFrameworkService
  {
    private const string s_area = "Agile";
    private const string s_layer = "TeamConfigurationService";
    public const int MaxAllowedTeamIterationPairs = 200;
    public const int DefaultMaxAllowedTeamAreaPaths = 300;
    private const string MaxAllowedTeamAreaPathsKey = "/Service/Agile/Settings/MaxAllowedTeamAreaPaths";
    public const int DefaultMaxAllowedTeamIterations = 300;
    private const string MaxAllowedTeamIterationsKey = "/Service/Agile/Settings/MaxAllowedTeamIterations";
    private ConcurrentDictionary<Guid, Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel>> m_cache = new ConcurrentDictionary<Guid, Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel>>();
    private IDictionary<Guid, Action<Guid, string>> m_notifications;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      Dictionary<Guid, Action<Guid, string>> notifications = new Dictionary<Guid, Action<Guid, string>>()
      {
        {
          SqlNotificationEventClasses.TeamSettingsChanged,
          new Action<Guid, string>(this.ClearTeamsFromCache)
        },
        {
          SqlNotificationEventClasses.ProjectSettingsChanged,
          new Action<Guid, string>(this.ClearAllTeams)
        },
        {
          DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged,
          new Action<Guid, string>(this.ClearAllTeams)
        }
      };
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        notifications[SpecialGuids.WorkItemTypeletChanged] = new Action<Guid, string>(this.ClearAllTeams);
      this.RegisterNotifications(requestContext, (IDictionary<Guid, Action<Guid, string>>) notifications);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => this.UnregisterNotifications(requestContext);

    private void RegisterNotifications(
      IVssRequestContext requestContext,
      IDictionary<Guid, Action<Guid, string>> notifications)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (Guid key in (IEnumerable<Guid>) notifications.Keys)
        service.RegisterNotification(requestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
      this.m_notifications = notifications;
    }

    private void UnregisterNotifications(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      if (this.m_notifications == null)
        return;
      foreach (Guid key in (IEnumerable<Guid>) this.m_notifications.Keys)
        service.UnregisterNotification(requestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
      this.m_notifications = (IDictionary<Guid, Action<Guid, string>>) null;
    }

    public int GetComponentVersion(IVssRequestContext requestContext)
    {
      using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
        return component.Version;
    }

    public ITeamSettings GetTeamSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      bool validate,
      bool bypassCache,
      bool skipOptionalProperties = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      using (requestContext.TraceBlock(240201, 240209, "Agile", nameof (TeamConfigurationService), nameof (GetTeamSettings)))
      {
        bool teamFieldIsAreaPath = this.IsTeamFieldAreaPath(requestContext, team.ProjectId);
        TeamConfigurationService.ValidationLevel validationLevel = validate ? TeamConfigurationService.ValidationLevel.Full : (skipOptionalProperties ? TeamConfigurationService.ValidationLevel.None : TeamConfigurationService.ValidationLevel.Partial);
        ITeamSettings teamSettings;
        if (!bypassCache && this.TryGetSettingsFromCache(team.Id, validationLevel, out teamSettings) && (teamFieldIsAreaPath && this.TeamFieldValuesAreValidAreaPaths(requestContext, team.Id, teamSettings) || !teamFieldIsAreaPath))
          return teamSettings;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
        {
          requestContext.Trace(290407, TraceLevel.Verbose, "Agile", nameof (TeamConfigurationService), "Loading team configuration: {0}", (object) team.Id);
          teamSettings = (ITeamSettings) component.GetTeamConfiguration(team.ProjectId, team.Id);
        }
        if (!skipOptionalProperties)
          this.SetExtendedProperties(requestContext, team, teamSettings);
        this.SanitizeTeamIterations(requestContext, team.ProjectId, teamSettings);
        this.PreCacheValidation(requestContext, team, teamSettings, validate, teamFieldIsAreaPath);
        this.AddUpdateCacheItem(requestContext, team, teamSettings, validationLevel, teamFieldIsAreaPath);
        return teamSettings;
      }
    }

    private void SanitizeTeamIterations(
      IVssRequestContext context,
      Guid projectId,
      ITeamSettings teamSettings)
    {
      HashSet<Guid> validNodes = teamSettings.GetIterationTimeline(context, projectId).Iterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, Guid>) (i => i.CssNodeId)).ToHashSet<Guid>();
      TeamIterationsCollection iterationsCollection = new TeamIterationsCollection();
      foreach (ITeamIteration iteration in teamSettings.Iterations.Values.Where<ITeamIteration>((Func<ITeamIteration, bool>) (i => validNodes.Contains(i.IterationId))))
        iterationsCollection.AddIteration(iteration);
      teamSettings.Iterations = (ITeamIterationsCollection) iterationsCollection;
    }

    protected virtual void PreCacheValidation(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings teamSettings,
      bool validate,
      bool teamFieldIsAreaPath)
    {
      if (validate)
      {
        requestContext.Trace(240202, TraceLevel.Verbose, "Agile", nameof (TeamConfigurationService), nameof (PreCacheValidation));
        TeamConfigurationHelper.ValidateBacklogIteration(teamSettings.BacklogIterationId, requestContext);
        bool corrected;
        ITeamFieldValue[] rawFieldValues;
        TeamConfigurationService.Validate(requestContext, team, teamSettings.TeamFieldConfig, out corrected, out rawFieldValues);
        if (!corrected)
          return;
        try
        {
          this.SaveTeamFields(requestContext.Elevate(), team, rawFieldValues, teamSettings.TeamFieldConfig.DefaultValueIndex, true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(290097, TraceLevel.Error, "Agile", nameof (TeamConfigurationService), ex);
        }
      }
      else
      {
        if (!teamFieldIsAreaPath)
          return;
        this.ResolveAreaPathTeamFieldValue(requestContext, team, teamSettings);
      }
    }

    public IEnumerable<ITeamSettings> GetAllTeamSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      bool validate,
      bool bypassCache)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240256, 240257, "Agile", nameof (TeamConfigurationService), nameof (GetAllTeamSettings)))
      {
        List<ITeamSettings> allTeamSettings = new List<ITeamSettings>();
        foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, projectId))
          allTeamSettings.Add(this.GetTeamSettings(requestContext, team, validate, bypassCache, false));
        return (IEnumerable<ITeamSettings>) allTeamSettings;
      }
    }

    public IDictionary<Guid, ITeamSettings> GetAllTeamSettingsByTeam(
      IVssRequestContext requestContext,
      Guid projectId,
      bool validate,
      bool bypassCache)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240256, 240257, "Agile", nameof (TeamConfigurationService), nameof (GetAllTeamSettingsByTeam)))
      {
        Dictionary<Guid, ITeamSettings> teamSettingsByTeam = new Dictionary<Guid, ITeamSettings>();
        foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, projectId))
          teamSettingsByTeam.Add(team.Id, this.GetTeamSettings(requestContext, team, validate, bypassCache, false));
        return (IDictionary<Guid, ITeamSettings>) teamSettingsByTeam;
      }
    }

    public IDictionary<string, IDictionary<Guid, bool>> GetAllTeamFieldsForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240290, 240291, "Agile", nameof (TeamConfigurationService), nameof (GetAllTeamFieldsForProject)))
      {
        this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
        IDictionary<string, IDictionary<Guid, bool>> fieldsForProject;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          fieldsForProject = component.GetAllTeamFieldsForProject(projectId);
        return fieldsForProject;
      }
    }

    public IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> GetAreaIdToTeamMappings(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240292, 240293, "Agile", nameof (TeamConfigurationService), nameof (GetAreaIdToTeamMappings)))
      {
        if (!this.IsTeamFieldAreaPath(requestContext, projectId))
          throw new InvalidOperationException(Resources.AreaIdToTeamMappings_UseAreaPathAsTeamFieldReference);
        this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
        IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> result = (IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>>) new Dictionary<Guid, IDictionary<Guid, TeamFieldProperty>>();
        PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", nameof (GetAreaIdToTeamMappings));
        IDictionary<string, IDictionary<Guid, bool>> fieldsForProject;
        using (performanceScenarioHelper.Measure("GetAllTeamFieldsForProject"))
          fieldsForProject = this.GetAllTeamFieldsForProject(requestContext, projectId);
        IDictionary<Guid, string> dictionary;
        using (performanceScenarioHelper.Measure("GetAllTeamsForProject"))
        {
          Guid[] array = fieldsForProject.Values.SelectMany<IDictionary<Guid, bool>, Guid>((Func<IDictionary<Guid, bool>, IEnumerable<Guid>>) (t => (IEnumerable<Guid>) t.Keys)).Distinct<Guid>().ToArray<Guid>();
          performanceScenarioHelper.Add("TeamIdsCount", (object) array.Length);
          try
          {
            dictionary = (IDictionary<Guid, string>) requestContext.GetService<ITeamService>().GetTeamsByGuid(requestContext, (IEnumerable<Guid>) array).ToDictionary<WebApiTeam, Guid, string>((Func<WebApiTeam, Guid>) (t => t.Id), (Func<WebApiTeam, string>) (t => t.Name));
          }
          catch (Exception ex)
          {
            performanceScenarioHelper.Add("Exception", (object) ex);
            requestContext.TraceException(15101003, TraceLevel.Error, "Agile", nameof (TeamConfigurationService), ex);
            throw;
          }
        }
        using (performanceScenarioHelper.Measure("IdentifyAreaIdToTeamMappings"))
        {
          IDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> children = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, projectId, false).Children;
          if (children != null)
          {
            if (children.Count > 0)
            {
              Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node;
              children.TryGetValue("Area", out node);
              if (node != null)
                AreaIdToTeamMappingsHelper.UpdateAreaIdToTeamMappingsResult(requestContext, projectId, node, fieldsForProject, dictionary, result);
            }
          }
        }
        bool flag = result.Values.Any<IDictionary<Guid, TeamFieldProperty>>((Func<IDictionary<Guid, TeamFieldProperty>, bool>) (x => x.Keys.Count > 1));
        performanceScenarioHelper.Add("HasOverlapOwners", (object) flag);
        performanceScenarioHelper.EndScenario();
        return result;
      }
    }

    public TeamCapacity GetTeamIterationCapacity(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings settings,
      Guid iterationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      using (requestContext.TraceBlock(240210, 240219, "Agile", nameof (TeamConfigurationService), nameof (GetTeamIterationCapacity)))
      {
        TeamCapacity iterationCapacity;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          iterationCapacity = component.GetTeamIterationCapacity(team.ProjectId, team.Id, iterationId);
        return iterationCapacity;
      }
    }

    public IEnumerable<Tuple<Guid, Guid>> GetTeamAndIterationPairsWithCapacityForCollection(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(240285, 240286, "Agile", nameof (TeamConfigurationService), nameof (GetTeamAndIterationPairsWithCapacityForCollection)))
      {
        if (!requestContext.IsSystemContext)
          throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateNoReadPermissionException();
        IEnumerable<Tuple<Guid, Guid>> capacitiesForCollection;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          capacitiesForCollection = component.GetTeamIterationsWithCapacitiesForCollection();
        return capacitiesForCollection;
      }
    }

    public IDictionary<WebApiTeam, TeamCapacity> GetTeamCapacityForIteration(
      IVssRequestContext requestContext,
      Guid iterationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(240285, 240286, "Agile", nameof (TeamConfigurationService), nameof (GetTeamCapacityForIteration)))
      {
        IReadOnlyCollection<WebApiTeam> source = requestContext.GetService<ITeamService>().QueryAllTeamsInCollection(requestContext);
        List<Tuple<Guid, Guid>> teamIdAndIterationIdPairs = new List<Tuple<Guid, Guid>>();
        foreach (WebApiTeam webApiTeam in (IEnumerable<WebApiTeam>) source)
          teamIdAndIterationIdPairs.Add(new Tuple<Guid, Guid>(webApiTeam.Id, iterationId));
        IEnumerable<TeamCapacity> bulkCapacityData;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          bulkCapacityData = component.GetBulkCapacityData((IEnumerable<Tuple<Guid, Guid>>) teamIdAndIterationIdPairs);
        Dictionary<WebApiTeam, TeamCapacity> capacityForIteration = new Dictionary<WebApiTeam, TeamCapacity>();
        int index = 0;
        foreach (TeamCapacity teamCapacity in bulkCapacityData)
        {
          if (!teamCapacity.IsEmpty)
            capacityForIteration.Add(source.ElementAt<WebApiTeam>(index), teamCapacity);
          ++index;
        }
        return (IDictionary<WebApiTeam, TeamCapacity>) capacityForIteration;
      }
    }

    public IEnumerable<TeamCapacity> GetBulkCapacityData(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<Guid, Guid>> teamAndIterationPairs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamAndIterationPairs, nameof (teamAndIterationPairs));
      using (requestContext.TraceBlock(240287, 240288, "Agile", nameof (TeamConfigurationService), nameof (GetBulkCapacityData)))
      {
        if (!requestContext.IsSystemContext)
          throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateNoReadPermissionException();
        if (teamAndIterationPairs.Count<Tuple<Guid, Guid>>() > 200)
          throw new NotSupportedException(string.Format(Resources.Validation_BulkTeamCapacity_MaximumPairs, (object) 200));
        IEnumerable<TeamCapacity> bulkCapacityData;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          bulkCapacityData = component.GetBulkCapacityData(teamAndIterationPairs);
        return bulkCapacityData;
      }
    }

    public IEnumerable<Tuple<Guid, Guid, int>> GetChangedTeamConfigurationCapacity(
      IVssRequestContext requestContext,
      int watermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(240294, 240295, "Agile", nameof (TeamConfigurationService), nameof (GetChangedTeamConfigurationCapacity)))
      {
        IEnumerable<Tuple<Guid, Guid, int>> configurationCapacity;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          configurationCapacity = component.GetChangedTeamConfigurationCapacity(watermark);
        return configurationCapacity;
      }
    }

    public IEnumerable<Tuple<Guid, int>> GetChangedTeamSettings(
      IVssRequestContext requestContext,
      int watermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(240296, 240297, "Agile", nameof (TeamConfigurationService), nameof (GetChangedTeamSettings)))
      {
        IEnumerable<Tuple<Guid, int>> changedTeamSettings;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          changedTeamSettings = component.GetChangedTeamSettings(watermark);
        return changedTeamSettings;
      }
    }

    protected virtual void CheckWritePermissions(IVssRequestContext requestContext, WebApiTeam team)
    {
      if (!requestContext.GetService<ITeamService>().UserHasPermission(requestContext, team.Identity, 2))
        throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateGenericWritePermissionException();
    }

    private void CheckAdminPermissions(IVssRequestContext requestContext, WebApiTeam team)
    {
      if (!requestContext.GetService<ITeamService>().UserIsTeamAdmin(requestContext, team.Identity))
        throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateGenericWritePermissionException();
      if (requestContext.IsFeatureEnabled("TeamService.Safeguard.BlockCreateTeamAndAdminOperations"))
        throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateAdminOperationsBlockedException();
    }

    protected virtual void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, ProjectInfo.GetProjectUri(projectId));
      securityNamespace.CheckPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators);
    }

    public IEnumerable<DateRange> GetTeamIterationDaysOff(
      IVssRequestContext requestContext,
      WebApiTeam team,
      Guid iterationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      using (requestContext.TraceBlock(240262, 240265, "Agile", nameof (TeamConfigurationService), nameof (GetTeamIterationDaysOff)))
      {
        if (!this.GetTeamSettings(requestContext, team, false, false, true).Iterations.Any<ITeamIteration>((Func<ITeamIteration, bool>) (iteration => iteration.IterationId == iterationId)))
          throw new IterationNotFoundException(iterationId.ToString());
        TeamCapacity iterationCapacity;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          iterationCapacity = component.GetTeamIterationCapacity(team.ProjectId, team.Id, iterationId);
        return (IEnumerable<DateRange>) iterationCapacity.TeamDaysOffDates;
      }
    }

    public virtual IDictionary<WebApiTeam, ITeamSettings> GetTeamSettingsInBulkWithoutProperties(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WebApiTeam>>(teams, nameof (teams));
      if (teams.Any<WebApiTeam>((Func<WebApiTeam, bool>) (team => team == null)))
        throw new ArgumentException(Resources.GetTeamSettings_TeamIsNull, nameof (teams));
      return (IDictionary<WebApiTeam, ITeamSettings>) requestContext.TraceBlock<Dictionary<WebApiTeam, ITeamSettings>>(290256, 290257, 290258, "Agile", nameof (TeamConfigurationService), (Func<Dictionary<WebApiTeam, ITeamSettings>>) (() =>
      {
        Dictionary<WebApiTeam, ITeamSettings> withoutProperties = new Dictionary<WebApiTeam, ITeamSettings>();
        List<WebApiTeam> source = new List<WebApiTeam>();
        foreach (WebApiTeam team in teams)
        {
          bool flag = this.IsTeamFieldAreaPath(requestContext, team.ProjectId);
          ITeamSettings teamSettings;
          if (this.TryGetSettingsFromCache(team.Id, TeamConfigurationService.ValidationLevel.None, out teamSettings) & flag && this.TeamFieldValuesAreValidAreaPaths(requestContext, team.Id, teamSettings))
            withoutProperties[team] = teamSettings;
          else
            source.Add(team);
        }
        if (source.Any<WebApiTeam>())
        {
          List<WebApiTeam> list1 = source.ToList<WebApiTeam>();
          IList<Tuple<Guid, Guid>> list2 = (IList<Tuple<Guid, Guid>>) list1.Select<WebApiTeam, Tuple<Guid, Guid>>((Func<WebApiTeam, Tuple<Guid, Guid>>) (team => Tuple.Create<Guid, Guid>(team.Id, team.ProjectId))).ToList<Tuple<Guid, Guid>>();
          IList<ITeamSettings> teamSettingsList;
          using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          {
            try
            {
              requestContext.Trace(290408, TraceLevel.Verbose, "Agile", nameof (TeamConfigurationService), "Loading team configurations in bulk for {0} team(s)", (object) list2.Count);
              teamSettingsList = component.GetTeamConfigurations(list2);
            }
            catch (NotSupportedException ex)
            {
              teamSettingsList = (IList<ITeamSettings>) ((IEnumerable<ITeamSettings>) list2.Select<Tuple<Guid, Guid>, TeamConfiguration>((Func<Tuple<Guid, Guid>, TeamConfiguration>) (tuple => component.GetTeamConfiguration(tuple.Item2, tuple.Item1)))).AsEnumerable<ITeamSettings>().ToList<ITeamSettings>();
            }
          }
          for (int index = 0; index < list1.Count; ++index)
          {
            WebApiTeam webApiTeam = list1[index];
            ITeamSettings teamSettings = teamSettingsList[index];
            bool teamFieldIsAreaPath = this.IsTeamFieldAreaPath(requestContext, webApiTeam.ProjectId);
            if (teamFieldIsAreaPath)
              this.ResolveAreaPathTeamFieldValue(requestContext, webApiTeam, teamSettings);
            if (!teamSettings.TeamFieldConfig.IsDefaultIndexValid())
            {
              requestContext.Trace(290424, TraceLevel.Warning, "Agile", nameof (TeamConfigurationService), "DefaultValueIndex was not in the correct range: {0}", (object) teamSettings.TeamFieldConfig.DefaultValueIndex);
              teamSettings.TeamFieldConfig.DefaultValueIndex = 0;
            }
            this.AddUpdateCacheItem(requestContext, webApiTeam, teamSettings, TeamConfigurationService.ValidationLevel.None, teamFieldIsAreaPath);
            withoutProperties[webApiTeam] = teamSettings;
          }
        }
        return withoutProperties;
      }), nameof (GetTeamSettingsInBulkWithoutProperties));
    }

    public void SaveTeamFields(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamFieldValue[] fieldValues,
      int defaultValueIndex)
    {
      this.SaveTeamFields(requestContext, team, fieldValues, defaultValueIndex, false);
    }

    public void SaveTeamFields(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamFieldValue[] fieldValues,
      int defaultValueIndex,
      bool skipValidation,
      bool skipKanbanBoardProvisioning = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamFieldValue[]>(fieldValues, nameof (fieldValues));
      using (requestContext.TraceBlock(240220, 240229, "Agile", nameof (TeamConfigurationService), nameof (SaveTeamFields)))
      {
        this.CheckAdminPermissions(requestContext, team);
        requestContext.Trace(240222, TraceLevel.Verbose, "Agile", nameof (TeamConfigurationService), nameof (SaveTeamFields));
        TeamFieldSettings settings = new TeamFieldSettings();
        settings.TeamFieldValues = fieldValues;
        settings.DefaultValueIndex = defaultValueIndex;
        if (!settings.IsDefaultIndexValid())
        {
          requestContext.Trace(290423, TraceLevel.Warning, "Agile", nameof (TeamConfigurationService), "DefaultValueIndex was not in the correct range: {0}. Setting to 0.", (object) defaultValueIndex);
          defaultValueIndex = 0;
        }
        ITeamFieldValue[] rawFieldValues = fieldValues;
        if (!skipValidation)
        {
          TeamConfigurationService.Validate(requestContext, team, (ITeamFieldSettings) settings, out bool _, out rawFieldValues);
          ProjectProcessConfiguration processSettings = requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(team.ProjectId), false);
          ITeamSettings teamSettings = this.GetTeamSettings(requestContext, team, false, false, true);
          if (processSettings.IsTeamFieldAreaPath())
            this.ValidateMaximumAllowedTeamAreaPaths(requestContext, fieldValues.Length, teamSettings.TeamFieldConfig.TeamFieldValues.Length);
        }
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SaveTeamFields(team.ProjectId, team.Id, rawFieldValues, defaultValueIndex);
        this.RemoveSettingsFromCache(team.Id);
        if (!skipKanbanBoardProvisioning)
          this.EnsureKanbanBoardIsProvisionAndUpToDateAsync(requestContext, team, nameof (SaveTeamFields));
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateTeamFields);
      }
    }

    public void SetBugsBehavior(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BugsBehavior behavior,
      bool skipKanbanBoardProvisioning = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      using (requestContext.TraceBlock(240262, 240263, "Agile", nameof (TeamConfigurationService), nameof (SetBugsBehavior)))
      {
        this.CheckAdminPermissions(requestContext, team);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SetTeamConfigurationProperties(team.ProjectId, team.Id, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
          {
            new KeyValuePair<string, string>("BugsBehavior", behavior.ToString())
          });
        this.RecordBugBehaviorTelemetry(requestContext, team, behavior);
        this.RemoveSettingsFromCache(team.Id);
        if (!skipKanbanBoardProvisioning)
          this.EnsureKanbanBoardIsProvisionAndUpToDate(requestContext, team, nameof (SetBugsBehavior));
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateBugsBehavior);
      }
    }

    public void SaveDefaultIteration(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string defaultIteration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<string>(defaultIteration, nameof (defaultIteration));
      using (requestContext.TraceBlock(240282, 240283, "Agile", nameof (TeamConfigurationService), nameof (SaveDefaultIteration)))
      {
        this.CheckAdminPermissions(requestContext, team);
        TeamConfigurationHelper.ValidateDefaultIteration(requestContext, defaultIteration);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SetTeamConfigurationProperties(team.ProjectId, team.Id, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
          {
            new KeyValuePair<string, string>("DefaultIteration", defaultIteration)
          });
        this.RemoveSettingsFromCache(team.Id);
        this.EnsureKanbanBoardIsProvisionAndUpToDate(requestContext, team, nameof (SaveDefaultIteration));
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateDefaultIteration);
      }
    }

    public void SetCumulativeFlowDiagramSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IDictionary<string, CumulativeFlowDiagramSettings> cfdSettings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IDictionary<string, CumulativeFlowDiagramSettings>>(cfdSettings, nameof (cfdSettings));
      using (requestContext.TraceBlock(240252, 240253, "Agile", nameof (TeamConfigurationService), nameof (SetCumulativeFlowDiagramSettings)))
      {
        this.CheckAdminPermissions(requestContext, team);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SetCFDProperties(team.ProjectId, team.Id, cfdSettings);
        this.RemoveSettingsFromCache(team.Id);
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateCumulativeFlowDiagram);
      }
    }

    public void SetBacklogVisibilities(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IDictionary<string, bool> visibilities)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IDictionary<string, bool>>(visibilities, nameof (visibilities));
      using (requestContext.TraceBlock(290231, 290232, "Agile", nameof (TeamConfigurationService), nameof (SetBacklogVisibilities)))
      {
        this.CheckAdminPermissions(requestContext, team);
        requestContext.GetService<IProjectService>();
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, team.ProjectId, false);
        TeamConfigurationHelper.EnsureValidBacklogVisibilities(requestContext, visibilities, backlogConfiguration, false, true, out IList<string> _);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SetBacklogVisibilitiesProperty(team.ProjectId, team.Id, visibilities);
        this.RemoveSettingsFromCache(team.Id);
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateBacklogVisibilities);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        foreach (KeyValuePair<string, bool> visibility in (IEnumerable<KeyValuePair<string, bool>>) visibilities)
          properties.Add(visibility.Key, visibility.Value);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Agile, "AgileChooseBacklogLevels", properties);
      }
    }

    public IDictionary<Guid, IDictionary<string, bool>> GetTeamBacklogVisibilitiesForProject(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      IEnumerable<Guid> teams)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(15101005, 15101006, "Agile", nameof (TeamConfigurationService), nameof (GetTeamBacklogVisibilitiesForProject)))
      {
        IDictionary<Guid, IDictionary<string, bool>> visibilitiesForProject;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          visibilitiesForProject = component.GetTeamBacklogVisibilitiesForProject(projectId);
        foreach (Guid team in teams)
        {
          if (!visibilitiesForProject.ContainsKey(team))
            visibilitiesForProject[team] = (IDictionary<string, bool>) new Dictionary<string, bool>();
        }
        foreach (Guid key in (IEnumerable<Guid>) visibilitiesForProject.Keys)
          TeamConfigurationHelper.EnsureValidBacklogVisibilities(requestContext, visibilitiesForProject[key], backlogConfiguration, true, true, out IList<string> _);
        return visibilitiesForProject;
      }
    }

    public IEnumerable<Guid> GetTeamsWithSubscribedIterations(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(15101007, 15101008, "Agile", nameof (TeamConfigurationService), nameof (GetTeamsWithSubscribedIterations)))
      {
        IEnumerable<Guid> subscribedIterations;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          subscribedIterations = component.GetTeamsWithSubscribedIterations(projectId);
        return subscribedIterations;
      }
    }

    public IDictionary<Guid, IEnumerable<Guid>> GetIterationSubscriptionsForTeams(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> teamIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(15101009, 15101010, "Agile", nameof (TeamConfigurationService), nameof (GetIterationSubscriptionsForTeams)))
      {
        IDictionary<Guid, IEnumerable<Guid>> subscriptionsForTeams;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          subscriptionsForTeams = component.GetIterationSubscriptionsForTeams(projectId, teamIds);
        return subscriptionsForTeams;
      }
    }

    public void SaveBacklogIterations(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IEnumerable<Guid> iterationIds,
      Guid rootIterationId,
      bool skipKanbanBoardProvisioning = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(iterationIds, nameof (iterationIds));
      using (requestContext.TraceBlock(240230, 240239, "Agile", nameof (TeamConfigurationService), nameof (SaveBacklogIterations)))
      {
        this.CheckAdminPermissions(requestContext, team);
        WorkItemTrackingTreeService treeService = requestContext.GetService<WorkItemTrackingTreeService>();
        List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> list = iterationIds.Select<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (nodeId => treeService.GetTreeNode(requestContext, team.ProjectId, nodeId, false))).Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n != null)).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
        ITeamIterationsCollection source = (ITeamIterationsCollection) new TeamIterationsCollection();
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode in list)
          source.CreateIteration(iterationNode);
        TeamConfigurationHelper.ValidateBacklogIteration(rootIterationId, requestContext);
        ITeamSettings teamSettings1 = this.GetTeamSettings(requestContext, team, false, false, true);
        requestContext.TraceAlways(1530023, TraceLevel.Info, "Agile", nameof (TeamConfigurationService), string.Format("previous team setting iteration count: {0}, current team setting iteration count: {1}", (object) teamSettings1.Iterations.Count<ITeamIteration>(), (object) source.Count<ITeamIteration>()));
        this.ValidateMaximumAllowedTeamIterations(requestContext, source.Count<ITeamIteration>(), teamSettings1.Iterations.Count<ITeamIteration>());
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SaveBacklogIterations(team.ProjectId, team.Id, source.Values.Select<ITeamIteration, Guid>((Func<ITeamIteration, Guid>) (iteration => iteration.IterationId)), rootIterationId);
        this.RemoveSettingsFromCache(team.Id);
        ITeamSettings teamSettings2 = (ITeamSettings) null;
        if (this.TryGetSettingsFromCache(team.Id, TeamConfigurationService.ValidationLevel.None, out teamSettings2))
          requestContext.TraceAlways(1530024, TraceLevel.Error, "Agile", nameof (TeamConfigurationService), string.Format("team  {0} should be removed from cache", (object) team.Id));
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateBacklogIterations);
        if (skipKanbanBoardProvisioning)
          return;
        this.EnsureKanbanBoardIsProvisionAndUpToDateAsync(requestContext, team, nameof (SaveBacklogIterations));
      }
    }

    internal virtual void ResolveAreaPathTeamFieldValue(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings settings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamSettings>(settings, nameof (settings));
      using (requestContext.TraceBlock(290081, 290082, "Agile", nameof (TeamConfigurationService), nameof (ResolveAreaPathTeamFieldValue)))
      {
        ITeamFieldSettings teamFieldConfig = settings.TeamFieldConfig;
        ITeamFieldValue[] teamFieldValues = teamFieldConfig.TeamFieldValues;
        if (teamFieldValues == null || teamFieldValues.Length == 0)
          return;
        WorkItemTrackingTreeService treeDictionary = requestContext.GetService<WebAccessWorkItemService>().GetTreeDictionary(requestContext);
        List<ITeamFieldValue> teamFieldValueList = new List<ITeamFieldValue>(teamFieldValues.Length);
        int num = 0;
        for (int index = 0; index < teamFieldValues.Length; ++index)
        {
          bool flag = false;
          ITeamFieldValue teamFieldValue = teamFieldValues[index];
          Guid result = Guid.Empty;
          if (Guid.TryParse(teamFieldValue.Value, out result))
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = treeDictionary.LegacyGetTreeNode(requestContext, result, false);
            if (treeNode != null)
            {
              teamFieldValue.Value = treeNode.GetPath(requestContext);
              teamFieldValueList.Add(teamFieldValue);
              flag = true;
            }
            else
              requestContext.Trace(290083, TraceLevel.Warning, "Agile", nameof (TeamConfigurationService), "Failed to resolve area id {0} to an area path for team {1}:{2} in project {3}", (object) teamFieldValue.Value, (object) team.Name, (object) team.Id, (object) team.ProjectId);
          }
          else if (this.IsValidAreaPath(requestContext, teamFieldValue.Value))
          {
            teamFieldValueList.Add(teamFieldValue);
            flag = true;
          }
          if (!flag && index <= teamFieldConfig.DefaultValueIndex)
            ++num;
        }
        teamFieldConfig.TeamFieldValues = teamFieldValueList.ToArray();
        teamFieldConfig.DefaultValueIndex = Math.Max(0, teamFieldConfig.DefaultValueIndex - num);
      }
    }

    internal void RecordBugBehaviorTelemetry(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BugsBehavior newBehavior)
    {
      using (requestContext.TraceBlock(15101003, 15101004, "Agile", nameof (TeamConfigurationService), nameof (RecordBugBehaviorTelemetry)))
      {
        try
        {
          BugsBehavior bugsBehavior = this.GetTeamSettings(requestContext, team, false, false, false).BugsBehavior;
          string str = string.Empty;
          ProcessDescriptor processDescriptor;
          requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, team.ProjectId, out processDescriptor);
          if (processDescriptor != null)
            str = processDescriptor.Name;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("TeamId", team.Id.ToString());
          properties.Add("PreviousBugBehavior", bugsBehavior.ToString());
          properties.Add("BugBehavior", newBehavior.ToString());
          properties.Add("ProcessTemplateIndicator", str);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Agile, "AgileTeamBugHandling", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(599999, "Agile", nameof (TeamConfigurationService), ex);
        }
      }
    }

    internal bool TeamFieldValuesAreValidAreaPaths(
      IVssRequestContext requestContext,
      Guid teamId,
      ITeamSettings settings)
    {
      foreach (ITeamFieldValue teamFieldValue in settings.TeamFieldConfig.TeamFieldValues)
      {
        if (!this.IsValidAreaPath(requestContext, teamFieldValue.Value))
          return false;
      }
      return true;
    }

    internal virtual bool IsValidAreaPath(IVssRequestContext requestContext, string path) => requestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(requestContext).TryGetNodeFromPath(requestContext, path, TreeStructureType.Area, out Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode _);

    internal static void Validate(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamFieldSettings teamFieldSettings,
      out bool corrected,
      out ITeamFieldValue[] rawFieldValues)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamFieldSettings>(teamFieldSettings, nameof (teamFieldSettings));
      using (requestContext.TraceBlock(290084, 290085, "Agile", nameof (TeamConfigurationService), nameof (Validate)))
      {
        corrected = false;
        string projectUri = ProjectInfo.GetProjectUri(team.ProjectId);
        if (teamFieldSettings.TeamFieldValues == null || teamFieldSettings.TeamFieldValues.Length == 0)
          throw new InvalidTeamSettingsException(Resources.Settings_MissingTeamFieldSettings, TeamSettingsFields.TeamField);
        ProjectProcessConfiguration processSettings = requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, false);
        if (processSettings.IsTeamFieldAreaPath())
        {
          IList<ITeamFieldValue> teamFieldValueList1 = (IList<ITeamFieldValue>) new List<ITeamFieldValue>();
          ITeamFieldValue[] teamFieldValues = teamFieldSettings.TeamFieldValues;
          WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
          int num = 0;
          List<ITeamFieldValue> teamFieldValueList2 = new List<ITeamFieldValue>();
          for (int index = 0; index < teamFieldValues.Length; ++index)
          {
            ITeamFieldValue teamFieldValue = teamFieldValues[index];
            Guid result;
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode;
            if (Guid.TryParse(teamFieldValue.Value, out result))
              treeNode = service.GetTreeNode(requestContext, team.ProjectId, result, false);
            else
              service.GetSnapshot(requestContext).TryGetNodeFromPath(requestContext, teamFieldValue.Value, TreeStructureType.Area, out treeNode);
            if (treeNode != null)
            {
              if (result != Guid.Empty)
                teamFieldValues[index].Value = treeNode.GetPath(requestContext);
              teamFieldValueList2.Add((ITeamFieldValue) new TeamFieldValue()
              {
                IncludeChildren = teamFieldValue.IncludeChildren,
                Value = treeNode.CssNodeId.ToString()
              });
            }
            else
            {
              requestContext.Trace(240202, TraceLevel.Info, "Agile", nameof (TeamConfigurationService), "Removing invalid team field value since the corresponding Area Path does not exist. Project {0}; Team {1}; Path {2}", (object) projectUri, (object) team.Id, (object) teamFieldValue.Value);
              teamFieldValueList1.Add(teamFieldValue);
              if (index <= teamFieldSettings.DefaultValueIndex)
                ++num;
            }
          }
          if (teamFieldValueList1.Any<ITeamFieldValue>())
          {
            teamFieldSettings.DefaultValueIndex = Math.Max(0, teamFieldSettings.DefaultValueIndex - num);
            teamFieldSettings.TeamFieldValues = ((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).Except<ITeamFieldValue>((IEnumerable<ITeamFieldValue>) teamFieldValueList1).ToArray<ITeamFieldValue>();
            if (teamFieldSettings.TeamFieldValues.Length == 0)
              throw new InvalidTeamSettingsException(Resources.Settings_MissingTeamFieldSettings, TeamSettingsFields.TeamField);
            corrected = true;
          }
          rawFieldValues = teamFieldValueList2.ToArray();
        }
        else
        {
          WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
          FieldEntry field;
          if (requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(requestContext, processSettings.TeamField.Name, out field))
          {
            IEnumerable<string> allowedValues = service.GetAllowedValues(requestContext, field.FieldId, team.ProjectName);
            if (allowedValues.Any<string>())
            {
              if (((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).Where<ITeamFieldValue>((Func<ITeamFieldValue, bool>) (v => v.IncludeChildren)).Any<ITeamFieldValue>())
                requestContext.Trace(290010, TraceLevel.Error, "Agile", nameof (TeamConfigurationService), "Shouldn't have IncludeChildren on any values with a non-tree path team field");
              if (!((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).Select<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (v => v.Value)).Intersect<string>(allowedValues).Any<string>())
                throw new InvalidTeamSettingsException(Resources.Settings_MissingTeamFieldSettings, TeamSettingsFields.TeamField);
            }
          }
          else
          {
            string message = "Couldn't locate field '{0}' when attempting to validate the team work item values. This should be validated when the project-level settings are retrieved.";
            requestContext.Trace(290011, TraceLevel.Error, "Agile", nameof (TeamConfigurationService), message, (object) processSettings.TeamField.Name);
          }
          rawFieldValues = teamFieldSettings.TeamFieldValues;
        }
        if (teamFieldSettings.IsDefaultIndexValid())
          return;
        requestContext.Trace(290425, TraceLevel.Info, "Agile", nameof (TeamConfigurationService), "Adjusting the default value index to 0. Team: {0}, Previous Value: {1}, #TeamFieldValues: {2}", (object) team.Id, (object) teamFieldSettings.DefaultValueIndex, (object) teamFieldSettings.TeamFieldValues.Length);
        teamFieldSettings.DefaultValueIndex = 0;
        corrected = true;
      }
    }

    internal void ValidateMaximumAllowedTeamAreaPaths(
      IVssRequestContext requestContext,
      int newAreaPathCount,
      int existingAreaPathCount)
    {
      if (newAreaPathCount <= existingAreaPathCount || !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("TeamConfigurationService.SaveTeamFields.LimitAllowedTeamAreaPathsAndIterations"))
        return;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Agile/Settings/MaxAllowedTeamAreaPaths", true, 300);
      if (newAreaPathCount > num)
        throw new InvalidTeamSettingsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Settings_NumberOfAreaPathsExceedsLimit, (object) newAreaPathCount, (object) num), TeamSettingsFields.TeamField);
    }

    internal void ValidateMaximumAllowedTeamIterations(
      IVssRequestContext requestContext,
      int newIterationPathCount,
      int existingIterationPathCount)
    {
      if (newIterationPathCount <= existingIterationPathCount || !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("TeamConfigurationService.SaveTeamFields.LimitAllowedTeamAreaPathsAndIterations"))
        return;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Agile/Settings/MaxAllowedTeamIterations", true, 300);
      if (newIterationPathCount > num)
        throw new InvalidTeamSettingsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Settings_NumberOfIterationsExceedsLimit, (object) newIterationPathCount, (object) num), TeamSettingsFields.TeamField);
    }

    private void EnsureKanbanBoardIsProvisionAndUpToDateAsync(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string source)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((request, param) =>
      {
        IdentityDescriptor user = (IdentityDescriptor) param;
        if (user != (IdentityDescriptor) null)
        {
          using (IVssRequestContext userContext = request.CreateUserContext(user))
            this.EnsureKanbanBoardIsProvisionAndUpToDate(userContext, team, source);
        }
        else
          this.EnsureKanbanBoardIsProvisionAndUpToDate(request, team, source);
      }), (object) requestContext.UserContext, 0));
    }

    private void EnsureKanbanBoardIsProvisionAndUpToDate(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string source)
    {
      try
      {
        if (requestContext.GetProjectProcessConfiguration(ProjectInfo.GetProjectUri(team.ProjectId), false).IsDefault)
          return;
        KanbanUtils.Instance.EnsureKanbanForTeam(requestContext, team);
      }
      catch (Exception ex)
      {
        requestContext.Trace(240421, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban sync failed for team {0}. Exception detail: {1}. Source: {2}", (object) (team.Name ?? "<null team name>"), (object) ex.ToString(), (object) source);
      }
    }

    public void SaveTeamIterationCapacity(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings settings,
      Guid iterationId,
      TeamCapacity capacity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      ArgumentUtility.CheckForNull<TeamCapacity>(capacity, nameof (capacity));
      using (requestContext.TraceBlock(240240, 240249, "Agile", nameof (TeamConfigurationService), nameof (SaveTeamIterationCapacity)))
      {
        this.CheckWritePermissions(requestContext, team);
        this.ValidateCapacityData(requestContext, settings, iterationId, capacity);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SaveIterationCapacity(team.ProjectId, team.Id, iterationId, capacity);
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateTeamIterationCapacity);
        try
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("CapacityHasMultipleActivity", capacity.TeamMemberCapacityCollection.Any<TeamMemberCapacity>((Func<TeamMemberCapacity, bool>) (c => c.Activities != null && c.Activities.Count > 1)));
          properties.Add("TeamId", team.Id.ToString());
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Agile", "Capacity_MultipleActivities", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(290247, "Agile", nameof (TeamConfigurationService), ex);
        }
      }
    }

    private void ValidateCapacityData(
      IVssRequestContext requestContext,
      ITeamSettings settings,
      Guid iterationId,
      TeamCapacity capacity)
    {
      ArgumentUtility.CheckForNull<ITeamIteration>(settings.Iterations.GetIteration(iterationId), "iteration");
      IdentityService service = requestContext.GetService<IdentityService>();
      List<Guid> list = capacity.TeamMemberCapacityCollection.Select<TeamMemberCapacity, Guid>((Func<TeamMemberCapacity, Guid>) (x => x.TeamMemberId)).ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = service.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      IEnumerable<Guid> guids = list.Where<Guid>((Func<Guid, bool>) (x => !identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => id != null && id.Id == x))));
      if (guids.Any<Guid>())
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Validation_InvalidUserId, (object) string.Join<Guid>(",", guids)));
      foreach (TeamMemberCapacity teamMemberCapacity in (IEnumerable<TeamMemberCapacity>) capacity.TeamMemberCapacityCollection)
        teamMemberCapacity.Validate();
    }

    private void PublishTeamSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      TeamSettingsChangeType changeType)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      List<Guid> teamIds = new List<Guid>();
      teamIds.Add(teamId);
      int changeType1 = (int) changeType;
      this.PublishTeamSettingsChangedEvent(requestContext1, projectId1, (IReadOnlyCollection<Guid>) teamIds, (TeamSettingsChangeType) changeType1);
    }

    private void PublishTeamSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyCollection<Guid> teamIds,
      TeamSettingsChangeType changeType)
    {
      try
      {
        TeamSettingsChangedEvent notificationEvent = new TeamSettingsChangedEvent()
        {
          ProjectId = projectId,
          TeamIds = teamIds,
          ChangeType = changeType
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        requestContext.Trace(290543, TraceLevel.Info, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "TeamSettingsChangedEvent fired for ProjectId : {0} and TeamIds : {1}", (object) projectId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) teamIds));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290546, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    public void SetTeamIterationDaysOff(
      IVssRequestContext requestContext,
      Guid projectId,
      WebApiTeam team,
      Guid iterationId,
      IEnumerable<DateRange> teamDaysOff)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      ArgumentUtility.CheckForNull<IEnumerable<DateRange>>(teamDaysOff, nameof (teamDaysOff));
      using (requestContext.TraceBlock(240266, 240270, "Agile", nameof (TeamConfigurationService), nameof (SetTeamIterationDaysOff)))
      {
        this.CheckWritePermissions(requestContext, team);
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, iterationId, false);
        if (treeNode == null)
          throw new IterationNotFoundException(iterationId.ToString());
        if (treeNode.StartDate.HasValue && treeNode.FinishDate.HasValue)
        {
          foreach (DateRange dateRange in teamDaysOff)
          {
            DateTime? startDate = treeNode.StartDate;
            DateTime start = dateRange.Start;
            if ((startDate.HasValue ? (startDate.GetValueOrDefault() <= start ? 1 : 0) : 0) != 0)
            {
              DateTime? finishDate = treeNode.FinishDate;
              DateTime end = dateRange.End;
              if ((finishDate.HasValue ? (finishDate.GetValueOrDefault() >= end ? 1 : 0) : 0) != 0)
                continue;
            }
            throw new TeamDaysOutOfRangeException(treeNode.StartDate.Value, treeNode.FinishDate.Value);
          }
        }
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.SaveIterationTeamDaysOff(team.ProjectId, team.Id, iterationId, teamDaysOff);
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateTeamIterationDaysOff);
      }
    }

    public void DeleteProjectTeamSettings(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240181, 240189, "Agile", nameof (TeamConfigurationService), nameof (DeleteProjectTeamSettings)))
      {
        this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.Delete);
        IEnumerable<Guid> teamIds = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, projectId).Select<WebApiTeam, Guid>((Func<WebApiTeam, Guid>) (team => team.Id));
        this.DeleteTeamSettings(requestContext.Elevate(), teamIds);
      }
    }

    public void DeleteTeamSettings(IVssRequestContext requestContext, IEnumerable<Guid> teamIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(teamIds, nameof (teamIds));
      using (requestContext.TraceBlock(240191, 240199, "Agile", nameof (TeamConfigurationService), nameof (DeleteTeamSettings)))
      {
        if (!requestContext.IsSystemContext)
          throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateGenericWritePermissionException();
        if (!teamIds.Any<Guid>())
          return;
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.DeleteTeamConfiguration(teamIds);
        foreach (Guid teamId in teamIds)
          this.RemoveSettingsFromCache(teamId);
        this.PublishTeamSettingsChangedEvent(requestContext, Guid.Empty, (IReadOnlyCollection<Guid>) teamIds.ToList<Guid>(), TeamSettingsChangeType.Delete);
      }
    }

    public void CleanupDeletedTeamSettings(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290944, 290945, "Agile", nameof (TeamConfigurationService), nameof (CleanupDeletedTeamSettings)))
      {
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.CleanupDeletedTeamConfiguration();
      }
    }

    public void SaveTeamWeekends(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamWeekends weekends)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamWeekends>(weekends, nameof (weekends));
      using (requestContext.TraceBlock(240250, 240251, "Agile", nameof (TeamConfigurationService), nameof (SaveTeamWeekends)))
      {
        this.CheckAdminPermissions(requestContext, team);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
        {
          component.SaveTeamWeekends(team.ProjectId, team.Id, weekends);
          this.RemoveSettingsFromCache(team.Id);
        }
        this.PublishTeamSettingsChangedEvent(requestContext, team.ProjectId, team.Id, TeamSettingsChangeType.UpdateTeamWeekends);
      }
    }

    public void PopulateTeamWeekends(
      IVssRequestContext requestContext,
      Guid projectId,
      out TeamRetrievalError[] teamErrors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceBlock(240254, 240255, "Agile", nameof (TeamConfigurationService), nameof (PopulateTeamWeekends)))
      {
        IReadOnlyCollection<WebApiTeam> teams = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext.Elevate(), projectId);
        teamErrors = new TeamRetrievalError[0];
        if (this.IsDefaultProjectSettings(requestContext, projectId, out ProjectProcessConfiguration _))
          return;
        this.SetWeekendsForGivenTeams(requestContext, (IEnumerable<WebApiTeam>) teams, out teamErrors);
      }
    }

    public void PopulateTeamWeekends(
      IVssRequestContext requestContext,
      out TeamRetrievalError[] teamErrors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(240254, 240255, "Agile", nameof (TeamConfigurationService), nameof (PopulateTeamWeekends)))
      {
        IReadOnlyCollection<WebApiTeam> teams = requestContext.GetService<ITeamService>().QueryAllTeamsInCollection(requestContext.Elevate());
        this.SetWeekendsForGivenTeams(requestContext, (IEnumerable<WebApiTeam>) teams, out teamErrors);
      }
    }

    public void PopulateTeamWeekends(IVssRequestContext requestContext, WebApiTeam team)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      using (requestContext.TraceBlock(240254, 240255, "Agile", nameof (TeamConfigurationService), nameof (PopulateTeamWeekends)))
      {
        ProjectProcessConfiguration projectSettings;
        if (this.IsDefaultProjectSettings(requestContext, team.ProjectId, out projectSettings))
          return;
        this.SaveTeamWeekends(requestContext, team, (ITeamWeekends) new TeamWeekends()
        {
          Days = projectSettings.Weekends
        });
      }
    }

    public void DeleteBacklogIterations(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IEnumerable<Guid> iterationIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(iterationIds, nameof (iterationIds));
      using (requestContext.TraceBlock(15101000, 15101001, "Agile", nameof (TeamConfigurationService), nameof (DeleteBacklogIterations)))
      {
        this.CheckAdminPermissions(requestContext, team);
        using (TeamConfigurationComponent component = TeamConfigurationComponent.CreateComponent(requestContext))
          component.DeleteTeamIterations(team.ProjectId, team.Id, iterationIds);
        this.RemoveSettingsFromCache(team.Id);
        this.EnsureKanbanBoardIsProvisionAndUpToDateAsync(requestContext, team, nameof (DeleteBacklogIterations));
      }
    }

    private void SetWeekendsForGivenTeams(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams,
      out TeamRetrievalError[] teamErrors)
    {
      List<TeamRetrievalError> teamRetrievalErrorList = new List<TeamRetrievalError>();
      foreach (WebApiTeam team in teams)
      {
        try
        {
          this.PopulateTeamWeekends(requestContext, team);
        }
        catch (Exception ex)
        {
          teamRetrievalErrorList.Add(new TeamRetrievalError()
          {
            Exception = ex,
            Project = ProjectInfo.GetProjectUri(team.ProjectId),
            Team = team.Name
          });
        }
      }
      teamErrors = teamRetrievalErrorList.Count > 0 ? teamRetrievalErrorList.ToArray() : new TeamRetrievalError[0];
    }

    private bool IsDefaultProjectSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProjectProcessConfiguration projectSettings)
    {
      ProjectConfigurationService service = requestContext.GetService<ProjectConfigurationService>();
      projectSettings = service.GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
      return projectSettings.IsDefault;
    }

    protected virtual void SetExtendedProperties(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings teamSettings)
    {
      string projectUri = ProjectInfo.GetProjectUri(team.ProjectId);
      if (!requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, false).IsConfigValidForBugsBehavior(requestContext, projectUri))
        teamSettings.BugsBehavior = BugsBehavior.Off;
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, team.ProjectId, false);
      IList<string> defaultedVisibilities;
      TeamConfigurationHelper.EnsureValidBacklogVisibilities(requestContext, teamSettings.BacklogVisibilities, backlogConfiguration, true, true, out defaultedVisibilities);
      teamSettings.DefaultedBacklogVisibilities = defaultedVisibilities;
      ITeamService service = requestContext.GetService<ITeamService>();
      teamSettings.Weekends.CanEditWeekends = service.UserHasPermission(requestContext, team.Identity, 2);
    }

    private bool IsTeamFieldAreaPath(IVssRequestContext requestContext, Guid projectId)
    {
      string projectUri = ProjectInfo.GetProjectUri(projectId);
      return TFStringComparer.WorkItemFieldReferenceName.Equals("System.AreaPath", requestContext.GetService<ProjectConfigurationService>().GetTeamFieldsForProjects(requestContext, (IEnumerable<string>) new string[1]
      {
        projectUri
      })[projectUri]);
    }

    protected void OnNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.Trace(290406, TraceLevel.Info, "Agile", nameof (TeamConfigurationService), "Received change notification. Clearing TeamConfigurationService cache. Event Class: {0}; Event Data: {1}", (object) eventClass, (object) (eventData ?? ""));
      Action<Guid, string> action;
      if (this.m_notifications == null || !this.m_notifications.TryGetValue(eventClass, out action))
        return;
      action(eventClass, eventData);
    }

    private void AddUpdateCacheItem(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings teamSettings,
      TeamConfigurationService.ValidationLevel level,
      bool teamFieldIsAreaPath)
    {
      Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel> newValue = new Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel>(teamSettings, level);
      this.m_cache.AddOrUpdate(team.Id, newValue, (Func<Guid, Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel>, Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel>>) ((k, v) => newValue));
      this.PublishTeamAreaAndIterationPathCountTelemetry(requestContext, team, teamSettings, teamFieldIsAreaPath);
    }

    private void RemoveSettingsFromCache(Guid key) => this.m_cache.TryRemove(key, out Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel> _);

    private bool TryGetSettingsFromCache(
      Guid key,
      TeamConfigurationService.ValidationLevel requiredValidationLevel,
      out ITeamSettings teamSettings)
    {
      Tuple<ITeamSettings, TeamConfigurationService.ValidationLevel> tuple;
      if (this.m_cache.TryGetValue(key, out tuple) && tuple.Item2 >= requiredValidationLevel)
      {
        teamSettings = tuple.Item1;
        return true;
      }
      teamSettings = (ITeamSettings) null;
      return false;
    }

    private void ClearTeamsFromCache(Guid eventClass, string eventData)
    {
      foreach (Guid guid in TeamConfigurationService.ParseGuids(eventData))
        this.RemoveSettingsFromCache(guid);
    }

    internal static IEnumerable<Guid> ParseGuids(string value)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] strArray = value.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
        {
          Guid result;
          if (Guid.TryParse(strArray[index], out result))
            yield return result;
        }
        strArray = (string[]) null;
      }
    }

    private void ClearAllTeams(Guid eventClass, string eventData) => this.m_cache.Clear();

    private void PublishTeamAreaAndIterationPathCountTelemetry(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings teamSettings,
      bool teamFieldIsAreaPath)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      int num1 = 0;
      if (teamFieldIsAreaPath && teamSettings.TeamFieldConfig != null && teamSettings.TeamFieldConfig.TeamFieldValues != null)
        num1 = teamSettings.TeamFieldConfig.TeamFieldValues.Length;
      int num2 = 0;
      if (teamSettings.Iterations != null)
        num2 = teamSettings.Iterations.Count<ITeamIteration>();
      properties.Add("AreaPathCount", (double) num1);
      properties.Add("IterationPathCount", (double) num2);
      properties.Add("TeamId", team.Id.ToString());
      properties.Add("TeamName", team.Name);
      properties.Add("ProjectUri", ProjectInfo.GetProjectUri(team.ProjectId));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Agile", nameof (TeamConfigurationService), properties);
    }

    private enum ValidationLevel
    {
      None,
      Partial,
      Full,
    }
  }
}
