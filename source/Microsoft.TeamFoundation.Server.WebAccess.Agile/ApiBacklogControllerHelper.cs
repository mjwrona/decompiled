// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ApiBacklogControllerHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class ApiBacklogControllerHelper : BacklogsControllerHelper
  {
    public ApiBacklogControllerHelper(BacklogsController controller)
      : base(controller)
    {
    }

    public virtual string _GetProductBacklogQueryWiql(
      BacklogContext backlogContext,
      IAgileSettings teamAgileSettings)
    {
      ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(this.TfsRequestContext, teamAgileSettings, backlogContext, teamAgileSettings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext));
      backlogQueryBuilder.Fields = (IEnumerable<string>) new string[1]
      {
        CoreFieldReferenceNames.Title
      };
      WorkItemStateCategory[] queryStates = QueryUtils.GetQueryStates(backlogContext.ShowInProgress);
      return backlogQueryBuilder.GetQuery(queryStates);
    }

    private BacklogLevelConfiguration GetParentBacklogLevel(string childBacklogLevelId)
    {
      this.Settings.BacklogConfiguration.GetAllBacklogLevels();
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      if (this.Settings.BacklogConfiguration.TryGetHighestPortfolioLevel(out backlogLevel) && TFStringComparer.BacklogLevelId.Equals(backlogLevel.Id, childBacklogLevelId))
        throw new InvalidOperationException(AgileServerResources.ProductBacklog_ParentLevel_ChildIsRoot);
      return this.Settings.BacklogConfiguration.GetBacklogParent(childBacklogLevelId);
    }

    public BacklogContext GetRequestBacklogContext(
      Team team,
      string backlogLevelName,
      bool showInProgress = true)
    {
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      if (!this.Settings.BacklogConfiguration.TryGetBacklogByName(backlogLevelName, out backlogLevel))
        backlogLevel = this.Settings.BacklogConfiguration.RequirementBacklog;
      return new BacklogContext(team, backlogLevel)
      {
        ShowInProgress = showInProgress
      };
    }

    private JsObject GetIterationQueryJsonPackage(
      CommonStructureNodeInfo iteration,
      string[] fields)
    {
      BacklogLevelConfiguration taskBacklog = this.Settings.BacklogConfiguration.TaskBacklog;
      return this.GetQueryJsonPackage(new TaskBoardQueryBuilder(this.TfsRequestContext, this.Settings, (IEnumerable<string>) fields, iteration.GetWitPath(), this.Settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext)).GetQuery());
    }

    private JsObject GetQueryJsonPackage(string wiql)
    {
      JsObject queryJsonPackage = new JsObject();
      queryJsonPackage[nameof (wiql)] = (object) wiql;
      return queryJsonPackage;
    }

    public JsObject GetIterationBacklogQuery(string[] fields, Guid iterationId)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      return this.GetIterationQueryJsonPackage(this.TfsRequestContext.GetExtension<ICommonStructureServiceProvider>().GetNode(this.TfsRequestContext, iterationId), fields);
    }

    public JsObject GetBoardModelJsObject(string backlogLevelId) => ApiBacklogControllerHelper.GetBoardModelJsObject(this.TfsRequestContext, this.TfsWebContext.Project, this.Controller.Team, this.Settings, backlogLevelId);

    public static JsObject GetBoardModelJsObject(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      IAgileSettings agileSettings,
      string backlogLevelId,
      bool validateBoardSettings = false,
      bool pageIdentityRefs = false)
    {
      bool isBoardSettingsValid;
      return ApiBacklogControllerHelper.GetBoardModel(requestContext, project, team, agileSettings, backlogLevelId, out isBoardSettingsValid, validateBoardSettings, pageIdentityRefs).ToJson(requestContext, isBoardSettingsValid);
    }

    public static BoardModel GetBoardModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      IAgileSettings agileSettings,
      string backlogLevelId,
      out bool isBoardSettingsValid,
      bool validateBoardSettings = false,
      bool pageIdentityRefs = false)
    {
      using (new PerformanceScenarioHelper(requestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.GetBoardModel))
      {
        BacklogContext backlogContext = ApiBacklogControllerHelper.GetBacklogContext(agileSettings, backlogLevelId);
        BoardSettings boardSettings = (BoardSettings) null;
        IBoard fromBoardSettings = BoardFactory.GetBoardFromBoardSettings(requestContext, project, team, backlogContext, agileSettings, true, out boardSettings, pageIdentityRefs);
        CardSettingsUtils.Instance.ConvertTagIdToTagName(requestContext, boardSettings.BoardCardSettings.Rules);
        IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
        WorkItemTypeExtension extension = service.GetExtension(requestContext, boardSettings.ExtensionId.Value);
        bool flag = true;
        switch (extension.ReconciliationStatus)
        {
          case WorkItemTypeExtensionReconciliationStatus.Error:
          case WorkItemTypeExtensionReconciliationStatus.NeverReconciled:
          case WorkItemTypeExtensionReconciliationStatus.Pending:
            if (service.ReconcileExtension(requestContext, extension, 60000, true) == ReconcileRequestResult.CompletedSynchronously)
            {
              flag = false;
              break;
            }
            break;
          case WorkItemTypeExtensionReconciliationStatus.Reconciled:
            flag = false;
            break;
        }
        BoardModel boardModel = new BoardModel()
        {
          Board = fromBoardSettings,
          BoardSettings = boardSettings,
          BoardCardSettings = boardSettings.BoardCardSettings,
          BoardFilterSettings = boardSettings.BoardFilterSettings,
          WorkItemTypes = (IEnumerable<string>) backlogContext.CurrentLevelConfiguration.WorkItemTypes,
          ItemSource = fromBoardSettings.ItemDataSource,
          ReconciliationComplete = !flag
        };
        isBoardSettingsValid = true;
        if (validateBoardSettings)
          isBoardSettingsValid = boardModel.BoardSettings.IsValid(requestContext, agileSettings.Process, agileSettings.ProjectName, team, agileSettings.TeamSettings);
        return boardModel;
      }
    }

    private BacklogLevelConfiguration GetBacklogLevelConfiguration(string backlogLevelId)
    {
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      if (!this.Settings.BacklogConfiguration.TryGetBacklogLevelConfiguration(backlogLevelId, out backlogLevel))
        backlogLevel = this.Settings.BacklogConfiguration.RequirementBacklog;
      return backlogLevel;
    }

    public static BacklogContext GetBacklogContext(
      IAgileSettings agileSettings,
      string backlogLevelId)
    {
      BacklogLevelConfiguration levelConfiguration = agileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(backlogLevelId);
      return new BacklogContext(agileSettings.Team, levelConfiguration);
    }
  }
}
