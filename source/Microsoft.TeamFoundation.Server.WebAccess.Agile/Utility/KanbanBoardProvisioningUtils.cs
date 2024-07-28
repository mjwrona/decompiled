// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.KanbanBoardProvisioningUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Charts;
using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.Azure.Boards.Charts.Utility;
using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class KanbanBoardProvisioningUtils
  {
    internal static bool ProvisionKanbanBoardsForTeam(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      int reconcileTimeout,
      KanbanBoardProvisioningUtils.Logger logger)
    {
      using (requestContext.TraceBlock(290056, 290057, "Agile", TfsTraceLayers.BusinessLogic, nameof (ProvisionKanbanBoardsForTeam)))
      {
        try
        {
          requestContext.GetService<IBacklogConfigurationService>();
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = new AgileSettings(requestContext, project, team).BacklogConfiguration;
          BoardSettings boardSettings = KanbanUtils.Instance.GetOrCreateBoardSettings(requestContext, project, team, backlogConfiguration.RequirementBacklog, reconcileTimeout, true);
          if (boardSettings != null)
            KanbanBoardProvisioningUtils.BackFillCumulativeFlowDiagramForRequirementBacklog(requestContext, project, team, boardSettings.ExtensionId.Value);
          foreach (BacklogLevelConfiguration portfolioBacklog in (IEnumerable<BacklogLevelConfiguration>) backlogConfiguration.PortfolioBacklogs)
            KanbanUtils.Instance.GetOrCreateBoardSettings(requestContext, project, team, portfolioBacklog, reconcileTimeout, true);
          return true;
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case InvalidTeamSettingsException _:
            case InvalidProjectSettingsException _:
              logger?.Log(TraceLevel.Info, string.Format(AgileServerResources.ProvisionKanbanBoard_InvalidTeamSetting, (object) team.Name));
              return true;
            case MissingProjectSettingsException _:
              logger?.Log(TraceLevel.Info, string.Format(AgileServerResources.ProvisionKanbanBoard_MissingTeamSetting, (object) team.Name));
              return true;
            default:
              requestContext.TraceException(230303, "Agile", TfsTraceLayers.BusinessLogic, ex);
              logger?.Log(TraceLevel.Warning, string.Format(AgileServerResources.ProvisionKanbanBoard_Failed, (object) team.Name, (object) team.Id.ToString("D"), (object) ex.Message, (object) ex.StackTrace));
              return false;
          }
        }
      }
    }

    internal static bool ProvisionKanbanBoards(
      IVssRequestContext requestContext,
      int reconcileTimeout,
      KanbanBoardProvisioningUtils.Logger logger,
      int retries = 0)
    {
      if (retries > 0 && logger != null)
        logger.TraceLevel = TraceLevel.Info;
      List<Guid> failedTeamIds;
      bool flag;
      for (flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoard(requestContext, reconcileTimeout, logger, out failedTeamIds); retries-- > 0 && !flag; flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoard(requestContext, (IEnumerable<Guid>) failedTeamIds, reconcileTimeout, logger, out failedTeamIds))
      {
        if (logger != null)
        {
          logger.Log(TraceLevel.Info, AgileServerResources.ProvisionKanbanBoard_Retry);
          if (retries == 0)
            logger.TraceLevel = TraceLevel.Warning;
        }
      }
      return flag;
    }

    internal static bool ProvisionKanbanBoard(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamIds,
      int reconcileTimeout,
      KanbanBoardProvisioningUtils.Logger logger,
      int retries = 0)
    {
      if (retries > 0 && logger != null)
        logger.TraceLevel = TraceLevel.Info;
      List<Guid> failedTeamIds;
      bool flag;
      for (flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoard(requestContext, teamIds, reconcileTimeout, logger, out failedTeamIds); retries-- > 0 && !flag; flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoard(requestContext, (IEnumerable<Guid>) failedTeamIds, reconcileTimeout, logger, out failedTeamIds))
      {
        if (logger != null)
        {
          logger.Log(TraceLevel.Info, AgileServerResources.ProvisionKanbanBoard_Retry);
          if (retries == 0)
            logger.TraceLevel = TraceLevel.Warning;
        }
      }
      return flag;
    }

    private static bool ProvisionKanbanBoard(
      IVssRequestContext requestContext,
      int reconcileTimeout,
      KanbanBoardProvisioningUtils.Logger logger,
      out List<Guid> failedTeamIds)
    {
      CommonStructureService service1 = requestContext.GetService<CommonStructureService>();
      ITeamService service2 = requestContext.GetService<ITeamService>();
      bool flag1 = true;
      failedTeamIds = new List<Guid>();
      requestContext.Trace(230301, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Start Provisioning boards for all teams in all projects...");
      IVssRequestContext requestContext1 = requestContext;
      foreach (CommonStructureProjectInfo wellFormedProject in service1.GetWellFormedProjects(requestContext1))
      {
        bool flag2 = true;
        requestContext.Trace(230306, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, "Provisioning boards for all teams in project {0}", (object) wellFormedProject.Name);
        foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) service2.QueryTeamsInProject(requestContext, wellFormedProject.GetId()))
        {
          requestContext.Trace(230304, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Provisioning board for team {0}", (object) team.Name);
          bool flag3 = KanbanBoardProvisioningUtils.ProvisionKanbanBoardsForTeam(requestContext, wellFormedProject, team, reconcileTimeout, logger);
          if (!flag3)
          {
            flag1 = false;
            flag2 = false;
            failedTeamIds.Add(team.Id);
          }
          requestContext.Trace(230305, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, flag3 ? "Successfully provisioned board for team {0}" : "Provisioning board failed for team {0}", (object) team.Name);
        }
        requestContext.Trace(230307, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, flag2 ? "Successfully provisioned all boards for project {0}" : "Failed to provision all boards for project {0}", (object) wellFormedProject.Name);
      }
      requestContext.Trace(230302, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, flag1 ? "Successfully provisioned board for all teams" : "Failed to provision at least one board");
      return flag1;
    }

    private static bool ProvisionKanbanBoard(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamIds,
      int reconcileTimeout,
      KanbanBoardProvisioningUtils.Logger logger,
      out List<Guid> failedTeamIds)
    {
      CommonStructureService service1 = requestContext.GetService<CommonStructureService>();
      ITeamService service2 = requestContext.GetService<ITeamService>();
      requestContext.GetService<BoardService>();
      bool flag1 = true;
      failedTeamIds = new List<Guid>();
      requestContext.Trace(230301, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Start Provisioning boards for requested teams...");
      foreach (Guid teamId in teamIds)
      {
        WebApiTeam teamByGuid = service2.GetTeamByGuid(requestContext, teamId);
        CommonStructureProjectInfo project = service1.GetProject(requestContext, teamByGuid.ProjectId);
        requestContext.Trace(230304, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Provisioning board for team {0}", (object) teamByGuid.Name);
        bool flag2 = KanbanBoardProvisioningUtils.ProvisionKanbanBoardsForTeam(requestContext, project, teamByGuid, reconcileTimeout, logger);
        if (!flag2)
        {
          flag1 = false;
          failedTeamIds.Add(teamByGuid.Id);
        }
        requestContext.Trace(230305, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, flag2 ? "Successfully provisioned board for team {0}" : "Provisioning board failed for team {0}", (object) teamByGuid.Name);
      }
      requestContext.Trace(230302, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, flag1 ? "Successfully provisioned board for all teams" : "Failed to provision at least one board");
      return flag1;
    }

    private static void BackFillCumulativeFlowDiagramForRequirementBacklog(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      Guid extensionId)
    {
      requestContext.TraceEnter(230308, "Agile", TfsTraceLayers.BusinessLogic, "-->BackFillCumulativeFlowDiagram");
      IAgileSettings settings = (IAgileSettings) new AgileSettings(requestContext, project, team);
      TimeZoneInfo collectionTimeZone = requestContext.GetCollectionTimeZone();
      IdentityChartCache view = IdentityPropertiesView.CreateView<IdentityChartCache>(requestContext, team.Id, TeamConstants.TeamChartCachePropertyName);
      CumulativeFlowDiagramInputs cumulativeFlowInputs = ChartUtils.GetCumulativeFlowInputs(requestContext, project.GetId(), settings, settings.BacklogConfiguration.RequirementBacklog, collectionTimeZone, team);
      string fieldReferenceName = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(requestContext, extensionId);
      cumulativeFlowInputs.EndDate = DateTime.Now;
      cumulativeFlowInputs.UseActualEndDateTime = true;
      IList<TrendDataPoint<string>> list = (IList<TrendDataPoint<string>>) new ChartDataComponent().GetCumulativeFlowDiagramDataViaQuery(requestContext, cumulativeFlowInputs, view).Select<CumulativeFlowDiagramDataPoint, TrendDataPoint<string>>((Func<CumulativeFlowDiagramDataPoint, TrendDataPoint<string>>) (dataPoint => new TrendDataPoint<string>(dataPoint.Date, dataPoint.StateCounts))).ToList<TrendDataPoint<string>>();
      WorkItemTrendService service1 = requestContext.GetService<WorkItemTrendService>();
      service1.SetTrendDataBaseline(requestContext, fieldReferenceName, collectionTimeZone, list);
      IWorkItemTypeExtensionService service2 = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension = service2.GetExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        extensionId
      }).FirstOrDefault<WorkItemTypeExtension>();
      if (extension != null)
      {
        bool everReconciled = false;
        int reconciliationStatus = (int) service2.GetReconciliationStatus(requestContext, extension, out everReconciled);
        if (everReconciled)
          service1.StampTrendDataBaseline(requestContext, fieldReferenceName);
      }
      requestContext.TraceEnter(230309, "Agile", TfsTraceLayers.BusinessLogic, "<--BackFillCumulativeFlowDiagram");
    }

    internal abstract class Logger
    {
      public Logger() => this.TraceLevel = TraceLevel.Info;

      public void Log(TraceLevel traceLevel, string message)
      {
        if (traceLevel == TraceLevel.Off || this.TraceLevel == TraceLevel.Off)
          return;
        this.LogMessage(traceLevel < this.TraceLevel ? this.TraceLevel : traceLevel, message);
      }

      public abstract void LogMessage(TraceLevel traceLevel, string message);

      public TraceLevel TraceLevel { get; set; }
    }
  }
}
