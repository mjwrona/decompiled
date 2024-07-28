// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Utility.ChartUtils
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Boards.Charts.Utility
{
  public static class ChartUtils
  {
    public static BurndownChartInputs GetBurnDownInputs(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      string iterationPath,
      TimeZoneInfo timeZone,
      IdentityChartCache identityChartCache,
      TeamCapacity teamCapacity,
      bool enforceLimit,
      int workItemCountLimit)
    {
      using (requestContext.TraceBlock(290052, 290053, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetBurnDownInputs)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
        ArgumentUtility.CheckStringForNullOrEmpty(iterationPath, nameof (iterationPath));
        BacklogLevelConfiguration taskBacklog = settings.BacklogConfiguration.TaskBacklog;
        List<string> list1 = taskBacklog.WorkItemTypes.OrderBy<string, string>((Func<string, string>) (wit => wit), (IComparer<string>) TFStringComparer.WorkItemTypeName).ToList<string>();
        List<string> list2 = settings.BacklogConfiguration.GetWorkItemStates(taskBacklog, new WorkItemStateCategory[3]
        {
          WorkItemStateCategory.Proposed,
          WorkItemStateCategory.InProgress,
          WorkItemStateCategory.Resolved
        }).OrderBy<string, string>((Func<string, string>) (state => state), (IComparer<string>) TFStringComparer.WorkItemStateName).ToList<string>();
        BurndownChartInputs burnDownInputs = new BurndownChartInputs();
        burnDownInputs.FilterFieldName = settings.Process.TeamField.Name;
        burnDownInputs.FilterFieldValues = new List<Microsoft.Azure.Boards.Charts.FieldValue>(((IEnumerable<ITeamFieldValue>) settings.TeamSettings.TeamFieldConfig.TeamFieldValues).Select<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>((Func<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>) (c => new Microsoft.Azure.Boards.Charts.FieldValue()
        {
          Value = c.Value,
          IncludeHierarchy = c.IncludeChildren && settings.Process.IsTeamFieldAreaPath()
        })));
        burnDownInputs.Iteration = new IterationProperties()
        {
          IterationPath = iterationPath
        };
        burnDownInputs.RemainingWorkField = settings.Process.RemainingWorkField.Name;
        burnDownInputs.WeekendDays = ((IEnumerable<DayOfWeek>) settings.TeamSettings.Weekends.Days).ToList<DayOfWeek>();
        burnDownInputs.TeamCapacityOffDays = teamCapacity.TeamDaysOffDates;
        burnDownInputs.WorkItemTypes = list1;
        burnDownInputs.InProgressStates = list2;
        burnDownInputs.TimeZone = timeZone;
        burnDownInputs.IdentityChartCache = identityChartCache;
        burnDownInputs.EnforceLimit = enforceLimit;
        burnDownInputs.WorkItemCountLimit = workItemCountLimit;
        burnDownInputs.TeamMemberCapacityCollection = (IEnumerable<TeamMemberCapacity>) teamCapacity.TeamMemberCapacityCollection;
        return burnDownInputs;
      }
    }

    public static string[] ValidateBurnDownIterationDates(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(iterationNode, nameof (iterationNode));
      List<string> stringList = new List<string>();
      if (!iterationNode.StartDate.HasValue || !iterationNode.FinishDate.HasValue)
        stringList.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ChartsResources.BurndownValidation_Dates, (object) iterationNode.GetPath(requestContext)));
      return stringList.ToArray();
    }

    public static VelocityChartInputs GetVelocityInputs(
      IVssRequestContext requestContext,
      Guid projectId,
      IAgileSettings settings,
      int iterationsNumber,
      SortedIterationSubscriptions sortedIteration = null)
    {
      using (requestContext.TraceBlock(290050, 290051, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetVelocityInputs)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
        SortedIterationSubscriptions iterationSubscriptions = sortedIteration ?? settings.TeamSettings.GetIterationTimeline(requestContext, projectId);
        List<string> stringList = iterationsNumber > 1 ? new List<string>(iterationSubscriptions.PreviousIterations.Skip<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(iterationSubscriptions.PreviousIterations.Count - iterationsNumber - 1).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>) (c => c.GetPath(requestContext)))) : new List<string>();
        if (iterationSubscriptions.CurrentIteration != null)
          stringList.Add(iterationSubscriptions.CurrentIteration.GetPath(requestContext));
        BacklogLevelConfiguration requirementBacklog = settings.BacklogConfiguration.RequirementBacklog;
        List<string> list1 = requirementBacklog.WorkItemTypes.ToList<string>();
        List<string> list2 = settings.BacklogConfiguration.GetWorkItemStates(requirementBacklog, new WorkItemStateCategory[2]
        {
          WorkItemStateCategory.InProgress,
          WorkItemStateCategory.Resolved
        }).ToList<string>();
        List<string> list3 = settings.BacklogConfiguration.GetWorkItemStates(requirementBacklog, new WorkItemStateCategory[1]
        {
          WorkItemStateCategory.Completed
        }).ToList<string>();
        VelocityChartInputs velocityInputs = new VelocityChartInputs();
        velocityInputs.CompletedStates = list3;
        velocityInputs.FilterFieldName = settings.Process.TeamField.Name;
        velocityInputs.FilterFieldValues = new List<Microsoft.Azure.Boards.Charts.FieldValue>(((IEnumerable<ITeamFieldValue>) settings.TeamSettings.TeamFieldConfig.TeamFieldValues).Select<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>((Func<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>) (c => new Microsoft.Azure.Boards.Charts.FieldValue()
        {
          Value = c.Value,
          IncludeHierarchy = c.IncludeChildren && settings.Process.IsTeamFieldAreaPath()
        })));
        velocityInputs.EffortField = settings.Process.EffortField.Name;
        velocityInputs.InProgressStates = list2;
        velocityInputs.Iterations = stringList;
        velocityInputs.WorkItemTypes = list1;
        return velocityInputs;
      }
    }

    public static string[] ValidateVelocityInputs(
      IVssRequestContext requestContext,
      Guid projectId,
      IAgileSettings settings,
      int iterationsNumber,
      SortedIterationSubscriptions sortedIteration = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      List<string> stringList = new List<string>();
      if (ChartUtils.GetVelocityInputs(requestContext, projectId, settings, iterationsNumber, sortedIteration).Iterations.Count == 0)
        stringList.Add(ChartsResources.VelocityValidation_IterationsMissing);
      return stringList.ToArray();
    }

    private static DayOfWeek GetFirstWorkDay(DayOfWeek[] weekends)
    {
      DayOfWeek firstWorkDay = DayOfWeek.Sunday;
      if (weekends != null && weekends.Length != 0)
      {
        for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; ++dayOfWeek)
        {
          if (!((IEnumerable<DayOfWeek>) weekends).Contains<DayOfWeek>(dayOfWeek))
          {
            firstWorkDay = dayOfWeek;
            break;
          }
        }
      }
      return firstWorkDay;
    }

    public static CumulativeFlowDiagramInputs GetCumulativeFlowInputs(
      IVssRequestContext requestContext,
      Guid projectId,
      IAgileSettings settings,
      BacklogLevelConfiguration backlogLevel,
      TimeZoneInfo timeZone,
      WebApiTeam team,
      DateTime? startDate = null,
      bool hideIncoming = false,
      bool hideOutgoing = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      using (requestContext.TraceBlock(290054, 290055, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetCumulativeFlowInputs)))
      {
        IEnumerable<BoardColumnRevision> boardColumnRevisions;
        string boardColumnExtensionFieldName;
        ChartUtils.Implementation.Instance.GetBoardSettings(projectId, team, requestContext, backlogLevel, out boardColumnRevisions, out boardColumnExtensionFieldName);
        IDictionary<Guid, Tuple<string, BoardColumnType, bool>> dictionary1 = (IDictionary<Guid, Tuple<string, BoardColumnType, bool>>) null;
        IReadOnlyCollection<string> workItemTypes = settings.BacklogConfiguration.GetBacklogByName(backlogLevel.Name).WorkItemTypes;
        IEnumerable<Tuple<string, WorkItemStateCategory>> statesWithCategory = settings.BacklogConfiguration.GetWorkItemStatesWithCategory(backlogLevel);
        List<string> stringList;
        IDictionary<string, BoardColumnType> dictionary2;
        if (boardColumnRevisions == null)
        {
          stringList = new List<string>((IEnumerable<string>) settings.BacklogConfiguration.GetWorkItemStates(backlogLevel));
          dictionary2 = ChartUtils.CreateStateMap(statesWithCategory);
        }
        else
        {
          dictionary1 = ChartUtils.GenerateColumnMappings(boardColumnRevisions, timeZone);
          stringList = dictionary1.Values.Select<Tuple<string, BoardColumnType, bool>, string>((Func<Tuple<string, BoardColumnType, bool>, string>) (v => v.Item1)).ToList<string>();
          dictionary2 = (IDictionary<string, BoardColumnType>) dictionary1.Values.ToDictionary<Tuple<string, BoardColumnType, bool>, string, BoardColumnType>((Func<Tuple<string, BoardColumnType, bool>, string>) (k => k.Item1), (Func<Tuple<string, BoardColumnType, bool>, BoardColumnType>) (v => v.Item2));
        }
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan timeSpan = TimeSpan.FromDays(217.0);
        bool flag = false;
        if (!startDate.HasValue)
          startDate = new DateTime?(utcNow - timeSpan);
        else if (utcNow - startDate.Value > timeSpan)
          startDate = new DateTime?(utcNow - timeSpan);
        else
          flag = true;
        CumulativeFlowDiagramInputs flowDiagramInputs = new CumulativeFlowDiagramInputs();
        flowDiagramInputs.FilterFieldName = settings.Process.TeamField.Name;
        flowDiagramInputs.FilterFieldValues = new List<Microsoft.Azure.Boards.Charts.FieldValue>(((IEnumerable<ITeamFieldValue>) settings.TeamSettings.TeamFieldConfig.TeamFieldValues).Select<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>((Func<ITeamFieldValue, Microsoft.Azure.Boards.Charts.FieldValue>) (c => new Microsoft.Azure.Boards.Charts.FieldValue()
        {
          Value = c.Value,
          IncludeHierarchy = c.IncludeChildren && settings.Process.IsTeamFieldAreaPath()
        })));
        flowDiagramInputs.WorkItemStates = stringList;
        flowDiagramInputs.WorkItemTypes = new List<string>((IEnumerable<string>) workItemTypes);
        flowDiagramInputs.StateColumnTypeMap = dictionary2;
        flowDiagramInputs.EndDate = utcNow;
        flowDiagramInputs.UseActualEndDateTime = false;
        flowDiagramInputs.StartDate = startDate.Value;
        flowDiagramInputs.IsCustomStartDate = flag;
        flowDiagramInputs.TimeZone = timeZone;
        flowDiagramInputs.FirstWorkDay = ChartUtils.GetFirstWorkDay(settings.Process.Weekends);
        flowDiagramInputs.BoardColumnRevisions = boardColumnRevisions;
        flowDiagramInputs.BoardColumnExtensionFieldName = boardColumnExtensionFieldName;
        flowDiagramInputs.BoardColumnMappings = dictionary1;
        CumulativeFlowDiagramInputs cumulativeFlowInputs = flowDiagramInputs;
        if (hideIncoming)
          cumulativeFlowInputs.RemoveState(cumulativeFlowInputs.NewStateName);
        if (hideOutgoing)
          cumulativeFlowInputs.RemoveState(cumulativeFlowInputs.DoneStateName);
        return cumulativeFlowInputs;
      }
    }

    public static IDictionary<Guid, Tuple<string, BoardColumnType, bool>> GenerateColumnMappings(
      IEnumerable<BoardColumnRevision> revisions,
      TimeZoneInfo timeZone)
    {
      return (IDictionary<Guid, Tuple<string, BoardColumnType, bool>>) revisions.Where<BoardColumnRevision>((Func<BoardColumnRevision, bool>) (c =>
      {
        if (c.Deleted)
          return true;
        DateTime? revisedDate = c.RevisedDate;
        DateTime futureDateTimeValue = SharedVariables.FutureDateTimeValue;
        if (!revisedDate.HasValue)
          return false;
        return !revisedDate.HasValue || revisedDate.GetValueOrDefault() == futureDateTimeValue;
      })).OrderBy<BoardColumnRevision, BoardColumnRevision>((Func<BoardColumnRevision, BoardColumnRevision>) (c => c), (IComparer<BoardColumnRevision>) new BoardColumnRevisionComparer()).ToDictionary<BoardColumnRevision, Guid, Tuple<string, BoardColumnType, bool>>((Func<BoardColumnRevision, Guid>) (k => k.Id.Value), (Func<BoardColumnRevision, Tuple<string, BoardColumnType, bool>>) (v =>
      {
        string str1 = v.Name;
        DateTime? revisedDate;
        DateTime dateTime;
        if (v.Deleted)
        {
          CultureInfo currentUiCulture = CultureInfo.CurrentUICulture;
          string flowDeletedColumn = ChartsResources.CumulativeFlowDeletedColumn;
          string str2 = str1;
          revisedDate = v.RevisedDate;
          dateTime = TimeZoneInfo.ConvertTime(revisedDate.Value, TimeZoneInfo.Utc, timeZone);
          string str3 = dateTime.ToString();
          str1 = string.Format((IFormatProvider) currentUiCulture, flowDeletedColumn, (object) str2, (object) str3);
        }
        string str4 = str1;
        int columnType = (int) v.ColumnType;
        revisedDate = v.RevisedDate;
        dateTime = SharedVariables.FutureDateTimeValue;
        int num = revisedDate.HasValue ? (revisedDate.HasValue ? (revisedDate.GetValueOrDefault() == dateTime ? 1 : 0) : 1) : 0;
        return Tuple.Create<string, BoardColumnType, bool>(str4, (BoardColumnType) columnType, num != 0);
      }));
    }

    private static IDictionary<string, BoardColumnType> CreateStateMap(
      IEnumerable<Tuple<string, WorkItemStateCategory>> states)
    {
      IDictionary<string, BoardColumnType> stateMap = (IDictionary<string, BoardColumnType>) new Dictionary<string, BoardColumnType>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (Tuple<string, WorkItemStateCategory> state in states)
      {
        BoardColumnType boardColumnType = BoardColumnType.InProgress;
        if (state.Item2 == WorkItemStateCategory.Proposed)
          boardColumnType = BoardColumnType.Incoming;
        else if (state.Item2 == WorkItemStateCategory.Completed)
          boardColumnType = BoardColumnType.Outgoing;
        stateMap[state.Item1] = boardColumnType;
      }
      return stateMap;
    }

    public class Implementation
    {
      private static ChartUtils.Implementation s_instance;

      protected Implementation()
      {
      }

      public static ChartUtils.Implementation Instance
      {
        get
        {
          if (ChartUtils.Implementation.s_instance == null)
            ChartUtils.Implementation.s_instance = new ChartUtils.Implementation();
          return ChartUtils.Implementation.s_instance;
        }
        internal set => ChartUtils.Implementation.s_instance = value;
      }

      public virtual void GetBoardSettings(
        Guid projectId,
        WebApiTeam team,
        IVssRequestContext tfsRequestContext,
        BacklogLevelConfiguration backlogLevel,
        out IEnumerable<BoardColumnRevision> boardColumnRevisions,
        out string boardColumnExtensionFieldName)
      {
        boardColumnRevisions = (IEnumerable<BoardColumnRevision>) null;
        boardColumnExtensionFieldName = string.Empty;
        if (team == null)
          return;
        BoardService service = tfsRequestContext.GetService<BoardService>();
        IEnumerable<BoardColumnRevision> boardColumnRevisions1 = service.GetBoardColumnRevisions(tfsRequestContext, projectId, team.Id, backlogLevel.Id);
        if (boardColumnRevisions1 != null && boardColumnRevisions1.Any<BoardColumnRevision>())
          boardColumnRevisions = boardColumnRevisions1;
        BoardSettings board = service.GetBoard(tfsRequestContext, projectId, team.Id, backlogLevel.Id);
        if (board == null)
          return;
        WorkItemTypeExtension extension = tfsRequestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(tfsRequestContext, board.ExtensionId.Value);
        boardColumnExtensionFieldName = KanbanUtils.Instance.GetFieldReferenceName(tfsRequestContext, extension, "Kanban.Column");
      }
    }
  }
}
