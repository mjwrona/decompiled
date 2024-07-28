// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ITeamConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DefaultServiceImplementation(typeof (TeamConfigurationService))]
  public interface ITeamConfigurationService : IVssFrameworkService
  {
    ITeamSettings GetTeamSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      bool validate,
      bool bypassCache,
      bool skipOptionalProperties = false);

    IEnumerable<ITeamSettings> GetAllTeamSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      bool validate,
      bool bypassCache);

    IDictionary<Guid, ITeamSettings> GetAllTeamSettingsByTeam(
      IVssRequestContext requestContext,
      Guid projectId,
      bool validate,
      bool bypassCache);

    IDictionary<string, IDictionary<Guid, bool>> GetAllTeamFieldsForProject(
      IVssRequestContext requestContext,
      Guid projectId);

    IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> GetAreaIdToTeamMappings(
      IVssRequestContext requestContext,
      Guid projectId);

    TeamCapacity GetTeamIterationCapacity(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings settings,
      Guid iterationId);

    IEnumerable<DateRange> GetTeamIterationDaysOff(
      IVssRequestContext requestContext,
      WebApiTeam team,
      Guid iterationId);

    void SetTeamIterationDaysOff(
      IVssRequestContext requestContext,
      Guid projectId,
      WebApiTeam team,
      Guid iterationId,
      IEnumerable<DateRange> teamDaysOff);

    void SaveTeamFields(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamFieldValue[] fieldValues,
      int defaultValueIndex);

    void SaveTeamFields(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamFieldValue[] fieldValues,
      int defaultValueIndex,
      bool skipValidation,
      bool skipKanbanBoardProvisioning);

    void SaveBacklogIterations(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IEnumerable<Guid> iterationIds,
      Guid rootIterationId,
      bool skipKanbanBoardProvisioning = false);

    void SaveTeamIterationCapacity(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings settings,
      Guid iterationId,
      TeamCapacity capacity);

    void DeleteProjectTeamSettings(IVssRequestContext requestContext, Guid projectId);

    void SaveTeamWeekends(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamWeekends weekends);

    void PopulateTeamWeekends(IVssRequestContext requestContext, WebApiTeam team);

    void PopulateTeamWeekends(
      IVssRequestContext requestContext,
      Guid projectId,
      out TeamRetrievalError[] teamErrors);

    void PopulateTeamWeekends(
      IVssRequestContext requestContext,
      out TeamRetrievalError[] teamErrors);

    void SetBugsBehavior(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BugsBehavior behavior,
      bool skipKanbanBoardProvisioning = false);

    void SaveDefaultIteration(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string defaultIteration);

    void SetCumulativeFlowDiagramSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IDictionary<string, CumulativeFlowDiagramSettings> cfdSettings);

    void SetBacklogVisibilities(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IDictionary<string, bool> visibilities);

    IEnumerable<Guid> GetTeamsWithSubscribedIterations(
      IVssRequestContext requestContext,
      Guid projectId);

    IDictionary<Guid, IEnumerable<Guid>> GetIterationSubscriptionsForTeams(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> teamIds);

    IDictionary<Guid, IDictionary<string, bool>> GetTeamBacklogVisibilitiesForProject(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      IEnumerable<Guid> teams);

    void DeleteBacklogIterations(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IEnumerable<Guid> iterationIds);

    IEnumerable<Tuple<Guid, Guid>> GetTeamAndIterationPairsWithCapacityForCollection(
      IVssRequestContext requestContext);

    IDictionary<WebApiTeam, TeamCapacity> GetTeamCapacityForIteration(
      IVssRequestContext requestContext,
      Guid iterationId);

    IEnumerable<TeamCapacity> GetBulkCapacityData(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<Guid, Guid>> teamAndIterationPairs);

    IEnumerable<Tuple<Guid, Guid, int>> GetChangedTeamConfigurationCapacity(
      IVssRequestContext requestContext,
      int watermark);

    IEnumerable<Tuple<Guid, int>> GetChangedTeamSettings(
      IVssRequestContext requestContext,
      int watermark);

    int GetComponentVersion(IVssRequestContext requestContext);

    IDictionary<WebApiTeam, ITeamSettings> GetTeamSettingsInBulkWithoutProperties(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams);
  }
}
