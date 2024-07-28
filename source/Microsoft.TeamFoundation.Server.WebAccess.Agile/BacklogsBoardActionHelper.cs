// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsBoardActionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.TaskBoard;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class BacklogsBoardActionHelper : BacklogsControllerHelper
  {
    private static readonly string[] DefaultFilterFieldNames = new string[6]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.WorkItemType
    };

    public BacklogsBoardActionHelper(BacklogsController backlogsController)
      : base(backlogsController)
    {
    }

    public JsObject SetBoardCardSettings(BoardCardSettings data)
    {
      ArgumentUtility.CheckForNull<BoardCardSettings>(data, nameof (data));
      Stopwatch stopwatch = Stopwatch.StartNew();
      string str = this.Controller.Team.Id.ToString();
      this.TfsRequestContext.Trace(290098, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Updating card settings for board {0}, team {1} with settings : {2}", (object) data.ScopeId, (object) str, (object) data.ToJson().ToString());
      string message = CardSettingsUtils.Instance.ValidateAndReconcileBoardCardSettingsForSET(this.TfsRequestContext, (BacklogLevelConfiguration) null, this.Settings, data, this.Controller.Team.Id);
      if (!string.IsNullOrEmpty(message))
      {
        this.TfsRequestContext.Trace(290099, TraceLevel.Error, "Agile", TfsTraceLayers.Controller, "Invalid board card settings data received for board {0} for team {1}. Error message: {2}", (object) data.ScopeId, (object) str, (object) message);
        throw new ArgumentException(message);
      }
      this.TfsRequestContext.Trace(290100, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Updating card settings for board {0}, teamId {1}", (object) data.ScopeId, (object) str);
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      CommonStructureProjectInfo project = CommonStructureProjectInfo.ConvertProjectInfo(this.TfsWebContext.Project);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid id = project.GetId();
      BoardCardSettings boardCardSettings = data;
      service.UpdateBoardCardSettings(tfsRequestContext, id, boardCardSettings);
      this.TfsRequestContext.Trace(290101, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Completed updating card settings for board {0}, teamId {1}.", (object) data.ScopeId, (object) str);
      stopwatch.Stop();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(AgileCustomerIntelligencePropertyName.ElapsedTime, (double) stopwatch.ElapsedMilliseconds);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.SaveCardCustomizationSettings, properties);
      return this.GetDefaultSuccessJsObject();
    }

    public static BoardCardSettings GetBoardCardSettings(
      IVssRequestContext context,
      WebApiTeam team,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      IAgileSettings settings)
    {
      BoardCardSettings.ScopeType boardScope = BoardCardSettings.ScopeType.TASKBOARD;
      Guid id = team.Id;
      string str = id.ToString();
      BacklogLevelConfiguration taskBacklog = settings.BacklogConfiguration.TaskBacklog;
      context.Trace(290103, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Get card settings for board {0}: {1} for team {2} ", (object) boardScope, (object) id, (object) str);
      BoardCardSettings boardCardSettings = BoardFactory.GetBoardCardSettings(context, project, settings, taskBacklog, boardScope, id);
      List<CardRule> boardCardRules = BoardFactory.GetBoardCardRules(context, project, boardScope, id);
      CardSettingsUtils.Instance.ConvertTagIdToTagName(context, boardCardRules);
      boardCardSettings.Rules = CardSettingsUtils.Instance.TransformWiqls(context, boardCardRules);
      context.Trace(290104, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Completed get card settings for board {0}: {1} for team {2}.", (object) boardScope, (object) id, (object) str);
      return boardCardSettings;
    }

    public JsObject SetBoardCardRules(CardRules cardRules)
    {
      ArgumentUtility.CheckForNull<CardRules>(cardRules, nameof (cardRules));
      Guid id1 = this.Controller.Team.Id;
      this.TfsRequestContext.Trace(290249, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Updating card rules for taskboard, team {0} ", (object) id1);
      Dictionary<string, BoardCardRules> rowView = this.ConvertCardRulesToRowView(cardRules);
      BoardCardRules boardCardRules1 = new BoardCardRules();
      List<string> stringList = new List<string>();
      foreach (string key in rowView.Keys)
      {
        rowView[key].ScopeId = id1;
        rowView[key].Scope = "TASKBOARD";
        string message = CardSettingsUtils.Instance.ValidateCardRules(this.TfsRequestContext, rowView[key], key, (IDictionary<Guid, string>) null);
        if (!string.IsNullOrEmpty(message))
        {
          this.TfsRequestContext.Trace(290250, TraceLevel.Error, "Agile", TfsTraceLayers.Controller, "Invalid board card rules data received for taskboard for team {0}. Error message: {1}", (object) id1.ToString(), (object) message);
          throw new ArgumentException(message);
        }
        boardCardRules1.Attributes = boardCardRules1.Attributes.Concat<RuleAttributeRow>((IEnumerable<RuleAttributeRow>) rowView[key].Attributes).ToList<RuleAttributeRow>();
        boardCardRules1.Rules = boardCardRules1.Rules.Concat<BoardCardRuleRow>((IEnumerable<BoardCardRuleRow>) rowView[key].Rules).ToList<BoardCardRuleRow>();
        stringList.Add(key);
      }
      boardCardRules1.ScopeId = id1;
      boardCardRules1.Scope = "TASKBOARD";
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      CommonStructureProjectInfo project = CommonStructureProjectInfo.ConvertProjectInfo(this.TfsWebContext.Project);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid id2 = project.GetId();
      BoardCardRules boardCardRules2 = boardCardRules1;
      List<string> typesToBeDeleted = stringList;
      service.UpdateBoardCardRules(tfsRequestContext, id2, boardCardRules2, typesToBeDeleted);
      this.TfsRequestContext.Trace(290251, TraceLevel.Verbose, "Agile", TfsTraceLayers.Controller, "Completed updating card rules for taskboard teamId {0}.", (object) id1);
      return this.GetDefaultSuccessJsObject();
    }

    public ViewResult GetBoardView()
    {
      string methodName = this.IsEmbedded ? "GetBoardEmbeddedView" : nameof (GetBoardView);
      using (this.TfsRequestContext.TraceBlock(290038, 290039, "Agile", TfsTraceLayers.Controller, methodName))
      {
        BacklogContext requestBacklogContext = this.Controller.RequestBacklogContext;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "AddSprintViewControlData"))
          this.ControlDataHelper.AddSprintViewControlData(this.Settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext), "TaskBoard");
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "AddTeamSettingsData"))
          this.ControlDataHelper.AddTeamSettingsData();
        if (!this.IsEmbedded)
        {
          using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "AddCumulativeFlowChartData"))
            this.ChartHelper.AddCumulativeFlowChartData(requestBacklogContext.CurrentLevelConfiguration, (IDictionary<string, object>) this.Controller.ViewData);
          if (this.Controller.RequestBacklogContext.CurrentLevelConfiguration.IsRequirementsBacklog)
            this.ChartHelper.AddVelocityChartData(this.TfsWebContext.Project.Id, 5, (IDictionary<string, object>) this.Controller.ViewData);
        }
        IBoard board = (IBoard) null;
        BoardSettings boardSettings = (BoardSettings) null;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GetBoardFromBoardSettings"))
          board = BoardFactory.GetBoardFromBoardSettings(this.TfsRequestContext, this.TfsWebContext.Project, this.Controller.Team, requestBacklogContext, this.Settings, false, out boardSettings);
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, methodName + "." + AgileCustomerIntelligencePropertyName.ConvertTagIdToTagName))
          CardSettingsUtils.Instance.ConvertTagIdToTagName(this.TfsRequestContext, boardSettings.BoardCardSettings.Rules);
        IWorkItemTypeExtensionService service = (IWorkItemTypeExtensionService) null;
        WorkItemTypeExtension extension = (WorkItemTypeExtension) null;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, methodName + "." + AgileCustomerIntelligencePropertyName.GetExtension))
        {
          service = this.TfsRequestContext.GetService<IWorkItemTypeExtensionService>();
          extension = service.GetExtension(this.TfsRequestContext, boardSettings.ExtensionId.Value);
        }
        bool flag = true;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, methodName + "." + AgileCustomerIntelligencePropertyName.ReconcileExtension))
        {
          switch (extension.ReconciliationStatus)
          {
            case WorkItemTypeExtensionReconciliationStatus.Error:
            case WorkItemTypeExtensionReconciliationStatus.NeverReconciled:
            case WorkItemTypeExtensionReconciliationStatus.Pending:
              if (service.ReconcileExtension(this.TfsRequestContext, extension, 60000, false) == ReconcileRequestResult.CompletedSynchronously)
              {
                flag = false;
                break;
              }
              break;
            case WorkItemTypeExtensionReconciliationStatus.Reconciled:
              flag = false;
              break;
          }
        }
        WorkItemSource workItemSource = (WorkItemSource) null;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GetBacklogBoardItemSource"))
          workItemSource = BoardFactory.GetBacklogBoardItemSource(this.TfsRequestContext, this.TfsWebContext.Project, this.Controller.Team, requestBacklogContext, this.Settings, boardSettings);
        BoardModel boardModel = new BoardModel()
        {
          Board = board,
          BoardSettings = boardSettings,
          BoardCardSettings = boardSettings.BoardCardSettings,
          BoardFilterSettings = boardSettings.BoardFilterSettings,
          WorkItemTypes = (IEnumerable<string>) requestBacklogContext.CurrentLevelConfiguration.WorkItemTypes,
          ItemSource = (IItemSource) workItemSource,
          ReconciliationComplete = !flag
        };
        BacklogBoardViewModel backlogBoardViewModel = new BacklogBoardViewModel();
        backlogBoardViewModel.BacklogContext = requestBacklogContext;
        backlogBoardViewModel.BoardModel = boardModel;
        BacklogBoardViewModel model = backlogBoardViewModel;
        this.Controller.ViewData["ProductBacklogAnchoredLevel"] = (object) this.Controller.RequestBacklogContext.LevelName;
        this.Controller.ViewData["ProductBacklogShowParents"] = (object) this.Controller.RequestBacklogContext.IncludeParents;
        using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, methodName + "." + AgileCustomerIntelligencePropertyName.AddBacklogContributionModel))
          this.ControlDataHelper.AddBacklogContributionModel(requestBacklogContext.LevelName);
        return this.IsEmbedded ? this.View("BoardEmbedded", (object) model) : this.View("Board", (object) model);
      }
    }

    public ViewResult GetTaskboardExceededView()
    {
      this.PrepareTaskboardViewData();
      object obj1 = (object) new ExpandoObject();
      // ISSUE: reference to a compiler-generated field
      if (BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "WorkItemLimit", typeof (BacklogsBoardActionHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__0, obj1, 0);
      // ISSUE: reference to a compiler-generated field
      if (BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "WorkItemCount", typeof (BacklogsBoardActionHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__1.Target((CallSite) BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__1, obj1, 0);
      // ISSUE: reference to a compiler-generated field
      if (BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, ViewResult>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ViewResult), typeof (BacklogsBoardActionHelper)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ViewResult> target = BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ViewResult>> p3 = BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__2 = CallSite<Func<CallSite, BacklogsBoardActionHelper, string, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "View", (IEnumerable<Type>) null, typeof (BacklogsBoardActionHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__2.Target((CallSite) BacklogsBoardActionHelper.\u003C\u003Eo__6.\u003C\u003Ep__2, this, "IterationBoardWorkItemLimitExceeded", obj1);
      return target((CallSite) p3, obj4);
    }

    internal static JsObject GetTaskboardViewData(
      TfsWebContext context,
      WebAccessWorkItemService witService,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      IAgileSettings settings,
      TaskBoardData taskBoardData,
      string filterPerson,
      string filterGroup)
    {
      IVssRequestContext tfsRequestContext = context.TfsRequestContext;
      BacklogLevelConfiguration taskBacklog = settings.BacklogConfiguration.TaskBacklog;
      BacklogLevelConfiguration requirementBacklog = settings.BacklogConfiguration.RequirementBacklog;
      BacklogLevelConfiguration backlogParent = settings.BacklogConfiguration.GetBacklogParent(requirementBacklog.Id);
      IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = tfsRequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(tfsRequestContext, project.Id);
      IEnumerable<string> childWorkItems = (IEnumerable<string>) null;
      IDictionary<string, IDictionary<string, ISet<string>>> first = (IDictionary<string, IDictionary<string, ISet<string>>>) null;
      using (context.TfsRequestContext.TraceBlock(6100000, 6100001, "Agile", TfsTraceLayers.BusinessLogic, "PrepareTaskBoardData_GetStateTransitions"))
      {
        childWorkItems = (IEnumerable<string>) taskBacklog.WorkItemTypes;
        if (context.TfsRequestContext.IsFeatureEnabled("WebAccess.Agile.Taskboard.DelayloadStateTransitions"))
        {
          first = witService.GetStateTransitions(context.TfsRequestContext, settings.ProjectName, childWorkItems, (IEnumerable<string>) settings.BacklogConfiguration.GetWorkItemStates(taskBacklog));
          IDictionary<string, IDictionary<string, ISet<string>>> stateTransitions = BacklogsBoardActionHelper.GetValidWorkItemStateTransitions(context.TfsRequestContext, settings, witService, requirementBacklog);
          first = (IDictionary<string, IDictionary<string, ISet<string>>>) first.Concat<KeyValuePair<string, IDictionary<string, ISet<string>>>>((IEnumerable<KeyValuePair<string, IDictionary<string, ISet<string>>>>) stateTransitions).ToDictionary<KeyValuePair<string, IDictionary<string, ISet<string>>>, string, IDictionary<string, ISet<string>>>((Func<KeyValuePair<string, IDictionary<string, ISet<string>>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IDictionary<string, ISet<string>>>, IDictionary<string, ISet<string>>>) (kvp => kvp.Value));
        }
      }
      IEnumerable<ParentChildWIMap> parentChildWiMap = BacklogsBoardActionHelper.GetParentChildWIMap(tfsRequestContext, settings, backlogParent, (IEnumerable<int>) taskBoardData.ParentIDs);
      JsObject taskboardViewData = new JsObject();
      taskboardViewData["states"] = (object) taskBoardData.States;
      taskboardViewData[nameof (team)] = (object) settings.Team.ToJson();
      if (taskBoardData.IsEmpty)
      {
        taskboardViewData["isEmpty"] = (object) true;
      }
      else
      {
        IEnumerable<ICardFieldDefinition> fieldDefinitions = CardSettingsUtils.Instance.GetCardFieldDefinitions(context.TfsRequestContext, taskBoardData.Fields);
        taskboardViewData["remainingWorkFormat"] = (object) settings.Process.RemainingWorkField.Format;
        taskboardViewData["workRollupField"] = (object) settings.Process.RemainingWorkField.Name;
        taskboardViewData["parentIds"] = (object) taskBoardData.ParentIDs;
        taskboardViewData["orderByField"] = (object) settings.Process.OrderByField.Name;
        taskboardViewData["parentNamePlural"] = (object) settings.Process.RequirementBacklog.PluralName;
        taskboardViewData["transitions"] = (object) first;
        taskboardViewData["payload"] = taskBoardData.PayloadData;
        taskboardViewData["teamFieldRefName"] = (object) settings.Process.TeamField.Name;
        taskboardViewData["teamFieldDefaultValue"] = (object) settings.TeamSettings.TeamFieldConfig.GetDefaultValue();
        taskboardViewData["childWorkItemTypes"] = (object) childWorkItems;
        taskboardViewData["hasNestedTasks"] = (object) taskBoardData.HasNestedTasks;
        taskboardViewData["reorderBlockingWorkItemIds"] = (object) taskBoardData.ReorderBlockingWorkItemIds.ToList<int>();
        taskboardViewData["missingWorkItemIds"] = (object) taskBoardData.MissingWorkItemIds.ToList<int>();
        taskboardViewData["filters"] = (object) new object[2]
        {
          (object) new
          {
            fieldName = CoreFieldReferenceNames.AssignedTo,
            values = taskBoardData.GetPeopleFilterValues(),
            selectedValue = filterPerson
          },
          (object) new
          {
            fieldName = string.Empty,
            values = new object[2]
            {
              (object) AgileServerResources.TaskBoard_PeopleFilter,
              (object) settings.BacklogConfiguration.RequirementBacklog.Name
            },
            selectedValue = filterGroup
          }
        };
        taskboardViewData["fieldDefinitions"] = (object) fieldDefinitions.Select<ICardFieldDefinition, JsObject>((Func<ICardFieldDefinition, JsObject>) (m => m.ToJson()));
        BoardCardSettings boardCardSettings = BacklogsBoardActionHelper.GetBoardCardSettings(context.TfsRequestContext, team, project, settings);
        IEnumerable<string> source = CardSettingsUtils.Instance.GetBoardFields(context.TfsRequestContext, boardCardSettings).Union<string>((IEnumerable<string>) BacklogsBoardActionHelper.DefaultFilterFieldNames).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        taskboardViewData["filterableFieldNames"] = (object) source.ToList<string>();
        taskboardViewData["parentMappings"] = parentChildWiMap != null ? (object) parentChildWiMap.ToList<ParentChildWIMap>() : (object) (List<ParentChildWIMap>) null;
        taskboardViewData["enabledWorkItemTypes"] = (object) new HashSet<string>(workItemTypes.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, bool>) (t => !t.IsDisabled)).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, string>) (t => t.Name)), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      }
      if (!tfsRequestContext.IsFeatureEnabled("WebAccess.Agile.Taskboard.DisableCustomColumn"))
      {
        using (WebPerformanceTimer.StartMeasure(context.RequestContext, "Taskboard.CustomColumn"))
        {
          Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumns columns = tfsRequestContext.GetService<ITaskboardColumnService>().GetColumns(tfsRequestContext, project, team);
          taskboardViewData["columns"] = (object) columns.Columns.Select(c => new
          {
            id = c.Id,
            name = c.Name,
            order = c.Order
          });
          taskboardViewData["columnIdToWorkItemTypeStateMap"] = (object) columns.Columns.ToDedupedDictionary<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumn, Guid, Dictionary<string, string>>((Func<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumn, Guid>) (c => c.Id), (Func<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumn, Dictionary<string, string>>) (c => c.Mappings.ToDedupedDictionary<ITaskboardColumnMapping, string, string>((Func<ITaskboardColumnMapping, string>) (m => m.WorkItemType), (Func<ITaskboardColumnMapping, string>) (m => m.State))));
          taskboardViewData["areColumnsCustomized"] = (object) columns.IsCustomized;
          taskboardViewData["isColumnMappingValid"] = (object) columns.IsValidMapping;
          taskboardViewData["columnMappingsValidationMessage"] = (object) columns.ValidationException?.Message;
          taskboardViewData["taskWorkItemTypesToStateMap"] = (object) settings.BacklogConfiguration.GetTaskWorkItemTypeStateMap();
          if (columns.IsCustomized)
          {
            if (columns.IsValidMapping)
            {
              Dictionary<int, (string, string)> dedupedDictionary = taskBoardData.WorkItemData.Where<WorkItemData>((Func<WorkItemData, bool>) (w => !w.IsParentType)).ToDedupedDictionary<WorkItemData, int, (string, string)>((Func<WorkItemData, int>) (item => item.ID), (Func<WorkItemData, (string, string)>) (item => (item.GetFieldValue(CoreFieldReferenceNames.WorkItemType).ToString(), item.GetFieldValue(CoreFieldReferenceNames.State).ToString())));
              IReadOnlyCollection<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumn> workItemColumns = tfsRequestContext.GetService<ITaskboardWorkItemService>().GetWorkItemColumns(tfsRequestContext, project, team, dedupedDictionary, columns);
              taskboardViewData["workItemColumnIdMap"] = (object) workItemColumns.ToDedupedDictionary<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumn, int, Guid>((Func<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumn, int>) (w => w.WorkItemId), (Func<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumn, Guid>) (w => w.ColumnId));
            }
          }
        }
      }
      return taskboardViewData;
    }

    private void PrepareTaskboardViewData(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIteration = null)
    {
      using (this.TfsRequestContext.TraceBlock(290561, 290562, "Agile", TfsTraceLayers.Controller, nameof (PrepareTaskboardViewData)))
      {
        this.Controller.ViewData["iterationBacklogOptions"] = (object) BacklogsIterationActionHelper.CreateIterationBacklogOptions(this.Settings.BacklogConfiguration.TaskBacklog);
        if (requestIteration == null)
          requestIteration = this.Controller.GetRequestIterationNode();
        this.ControlDataHelper.AddSprintViewControlData(requestIteration.GetPath(this.TfsRequestContext), "TaskBoard");
        this.ControlDataHelper.AddSprintName(this.Controller.Team.Name);
        this.ControlDataHelper.AddSprintInformation(this.TfsRequestContext);
        this.ControlDataHelper.AddRequirementBacklogContextData();
        this.ChartHelper.AddBurnDownChartData((IDictionary<string, object>) this.Controller.ViewData, requestIteration);
        this.ControlDataHelper.AddTeamSettingsData();
        this.ControlDataHelper.AddIterationTabContributionModel();
      }
    }

    private static IDictionary<string, IDictionary<string, ISet<string>>> GetValidWorkItemStateTransitions(
      IVssRequestContext context,
      IAgileSettings settings,
      WebAccessWorkItemService witService,
      BacklogLevelConfiguration backlog)
    {
      IDictionary<string, IDictionary<string, ISet<string>>> stateTransitions1 = (IDictionary<string, IDictionary<string, ISet<string>>>) null;
      FieldDefinition field;
      if (witService.TryGetFieldDefinitionByName(context, CoreFieldReferenceNames.State, out field))
      {
        foreach (string workItemType in (IEnumerable<string>) backlog.WorkItemTypes)
        {
          IEnumerable<string> allowedValues = witService.GetAllowedValues(context, field.Id, settings.ProjectName, (IEnumerable<string>) new List<string>()
          {
            workItemType
          });
          WebAccessWorkItemService witService1 = witService;
          IVssRequestContext requestContext = context;
          string projectName = settings.ProjectName;
          List<string> childWorkItems = new List<string>();
          childWorkItems.Add(workItemType);
          IEnumerable<string> displayStates = allowedValues;
          IDictionary<string, IDictionary<string, ISet<string>>> stateTransitions2 = witService1.GetStateTransitions(requestContext, projectName, (IEnumerable<string>) childWorkItems, displayStates);
          if (stateTransitions1 == null)
            stateTransitions1 = stateTransitions2;
          else
            stateTransitions1.Add(stateTransitions2.First<KeyValuePair<string, IDictionary<string, ISet<string>>>>());
        }
      }
      return stateTransitions1;
    }

    public static void RecordTaskBoardViewModelTelemetry(
      TfsWebContext tfsWebContext,
      int workitemCounts,
      int teamFieldValuesCount,
      string agileBoardFilterGroup,
      string agileBoardFilterPerson,
      long elapsedTime,
      BoardCardSettings boardCardSettings,
      IAgileSettings agileSettings)
    {
      BacklogsBoardActionHelper.RecordBoardViewModelTelemetry(tfsWebContext, false, workitemCounts, teamFieldValuesCount, agileBoardFilterGroup: agileBoardFilterGroup, agileBoardFilterPerson: agileBoardFilterPerson, elapsedTime: elapsedTime, boardCardSettings: boardCardSettings, agileSettings: agileSettings);
    }

    private static void RecordBoardViewModelTelemetry(
      TfsWebContext tfsWebContext,
      bool isKanbanBoard,
      int workitemCounts,
      int teamFieldValuesCount,
      string backlogLevelId = null,
      string agileBoardFilterGroup = null,
      string agileBoardFilterPerson = null,
      long elapsedTime = 0,
      BoardCardSettings boardCardSettings = null,
      IAgileSettings agileSettings = null)
    {
      try
      {
        ILicenseType[] licensesForUser = tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().GetLicensesForUser(tfsWebContext.TfsRequestContext, tfsWebContext.TfsRequestContext.UserContext);
        ITeamSettings teamSettings = tfsWebContext.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(tfsWebContext.TfsRequestContext, tfsWebContext.TfsRequestContext.GetWebTeamContext().Team, false, false);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(AgileCustomerIntelligencePropertyName.Licenses, (object) ((IEnumerable<ILicenseType>) licensesForUser).Select<ILicenseType, string>((Func<ILicenseType, string>) (license => license.Name)));
        properties.Add(AgileCustomerIntelligencePropertyName.ServiceName, tfsWebContext.TfsRequestContext.ServiceName);
        properties.Add(AgileCustomerIntelligencePropertyName.RequestPath, tfsWebContext.TfsRequestContext is IVssWebRequestContext ? ((IVssWebRequestContext) tfsWebContext.TfsRequestContext).RequestPath : string.Empty);
        properties.Add(AgileCustomerIntelligencePropertyName.Title, tfsWebContext.TfsRequestContext.Title());
        properties.Add(AgileCustomerIntelligencePropertyName.NumberOfWorkItems, (double) workitemCounts);
        properties.Add(AgileCustomerIntelligencePropertyName.AgileBoardFilterGroup, agileBoardFilterGroup);
        properties.Add(AgileCustomerIntelligencePropertyName.AgileBoardFilterPerson, !string.IsNullOrWhiteSpace(agileBoardFilterPerson));
        properties.Add(AgileCustomerIntelligencePropertyName.BacklogLevelName, backlogLevelId);
        properties.Add(AgileCustomerIntelligencePropertyName.TeamId, tfsWebContext.TfsRequestContext.GetWebTeamContext().Team.Id.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.BugBehavior, teamSettings.BugsBehavior.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.ProcessTemplateIndicator, BacklogsControllerHelper.GetProcessTemplateName(tfsWebContext.TfsRequestContext, tfsWebContext.Project.Id));
        properties.Add(AgileCustomerIntelligencePropertyName.ViewType, isKanbanBoard ? AgileCustomerIntelligencePropertyValue.KanbanBoardFeature : AgileCustomerIntelligencePropertyValue.TaskBoardFeature);
        properties.Add(AgileCustomerIntelligencePropertyName.ElapsedTime, (double) elapsedTime);
        properties.Add(AgileCustomerIntelligencePropertyName.TeamFieldValuesCount, (double) teamFieldValuesCount);
        if (boardCardSettings != null)
        {
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) CardSettingsUtils.Instance.GetBoardCardSettingsCIData(boardCardSettings, agileSettings))
            properties.Add(keyValuePair.Key, keyValuePair.Value);
        }
        tfsWebContext.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(tfsWebContext.TfsRequestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.Pageload, properties);
      }
      catch (Exception ex)
      {
        tfsWebContext.TfsRequestContext.TraceException(599999, "Agile", TfsTraceLayers.Controller, ex);
      }
    }

    private Dictionary<string, BoardCardRules> ConvertCardRulesToRowView(
      CardRules boardCardRuleSettings)
    {
      Dictionary<string, BoardCardRules> rowView = new Dictionary<string, BoardCardRules>();
      if (boardCardRuleSettings != null && boardCardRuleSettings.rules != null)
      {
        foreach (string key in boardCardRuleSettings.rules.Keys)
        {
          BoardCardRules boardCardRules = new BoardCardRules();
          int order = 1;
          foreach (Rule rule in boardCardRuleSettings.rules[key])
          {
            ArgumentUtility.CheckForNull<Dictionary<string, string>>(rule.settings, "rule.settings");
            bool result;
            if (!bool.TryParse(rule.isEnabled, out result))
              throw new ArgumentException(AgileServerResources.CardRules_IsEnabled_InvalidValue);
            BoardCardRuleRow boardCardRuleRow;
            try
            {
              if (!string.IsNullOrEmpty(rule.filter))
              {
                rule.filter = CardSettingsUtils.Instance.TransformWiqlNamesToIds(this.TfsRequestContext, rule.filter);
                boardCardRuleRow = new BoardCardRuleRow(rule.name, key, result, order, DateTime.UtcNow, rule.filter, CardSettingsUtils.Instance.GetFilterModel(rule.filter, this.TfsRequestContext));
              }
              else
              {
                rule.filter = string.Empty;
                boardCardRuleRow = new BoardCardRuleRow(rule.name, key, result, order, DateTime.UtcNow, rule.filter, (FilterModel) null);
              }
            }
            catch (Exception ex)
            {
              throw new ArgumentException(ex.Message);
            }
            foreach (KeyValuePair<string, string> setting in rule.settings)
            {
              RuleAttributeRow ruleAttributeRow = new RuleAttributeRow(rule.name, setting.Key, setting.Value, key);
              boardCardRules.Attributes.Add(ruleAttributeRow);
            }
            boardCardRules.Rules.Add(boardCardRuleRow);
            ++order;
          }
          rowView[key] = boardCardRules;
        }
      }
      return rowView;
    }

    private static IEnumerable<ParentChildWIMap> GetParentChildWIMap(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogLevelConfiguration parentLevel,
      IEnumerable<int> parentIds)
    {
      return (IEnumerable<ParentChildWIMap>) new BoardParentWIFilterHelper().GetParentChildWIMap(requestContext, settings, parentLevel, parentIds.ToArray<int>());
    }
  }
}
