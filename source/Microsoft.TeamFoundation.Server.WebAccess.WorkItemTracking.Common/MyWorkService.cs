// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.MyWorkService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class MyWorkService : IVssFrameworkService
  {
    private const string c_stateSeparator = "','";
    private const int c_topCount = 50;
    internal const string c_accountWorkResultsCountPath = "/Service/MyWork/ResultsCount";
    private const int c_followedTopCount = 200;
    private const string c_accountFollowedWorkResultsCountPath = "/Service/MyWork/FollowedResultsCount";
    private const string c_assignedToMeQueryExcludeMultipleStates = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] NOT IN ('{0}')\r\n                            ORDER BY [System.ChangedDate] DESC";
    private const string c_assignedToMeQueryExcludeOneState = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND NOT [System.State] = '{0}'\r\n                            ORDER BY [System.ChangedDate] DESC";
    private const string c_assignedToMeQueryExcludeNoStates = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                            ORDER BY [System.ChangedDate] DESC";
    private const string c_assignedToMeQueryOneState = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] = '{0}'\r\n                            ORDER BY [System.ChangedDate] DESC";
    private const string c_assignedToMeQueryMultipleStates = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] IN ('{0}') \r\n                            ORDER BY [System.ChangedDate] DESC";
    private const string c_myFollowedWorkItemsQuery = "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems\r\n                            WHERE [System.Id] IN (@Follows)\r\n                            ORDER BY [System.ChangedDate] DESC";
    private static string[] s_pagedFields = new string[7]
    {
      "System.Id",
      "System.WorkItemType",
      "System.Title",
      "System.State",
      "System.AssignedTo",
      "System.TeamProject",
      "System.ChangedDate"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<AccountWorkWorkItemModel> GetDoingWorkItemsAssignedToMe(
      IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetDoingWorkItemsAssignedToMe)))
      {
        IReadOnlyCollection<string> states = WorkItemTrackingFeatureFlags.IsQueryMyWorkByExcludingDoneStatesEnabled(requestContext) ? this.GetDoneStates(requestContext) : this.GetDoingStates(requestContext);
        IEnumerable<int> meQuery = this.RunAssignedToMeQuery(requestContext, states);
        return this.PageWorkItems(requestContext, meQuery);
      }
    }

    public IList<AccountWorkWorkItemModel> GetDoneWorkItemsAssignedToMe(
      IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetDoneWorkItemsAssignedToMe)))
      {
        IReadOnlyCollection<string> states = WorkItemTrackingFeatureFlags.IsQueryMyWorkByExcludingDoneStatesEnabled(requestContext) ? this.GetDoingStates(requestContext) : this.GetDoneStates(requestContext);
        IEnumerable<int> meQuery = this.RunAssignedToMeQuery(requestContext, states);
        return this.PageWorkItems(requestContext, meQuery);
      }
    }

    public IList<AccountWorkWorkItemModel> GetWorkItemsFollowedByMe(
      IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetWorkItemsFollowedByMe)))
      {
        IEnumerable<int> workItemIds = (IEnumerable<int>) this.RunQuery(requestContext, "\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems\r\n                            WHERE [System.Id] IN (@Follows)\r\n                            ORDER BY [System.ChangedDate] DESC", true);
        return this.PageWorkItems(requestContext, workItemIds);
      }
    }

    private IList<AccountWorkWorkItemModel> PageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      List<AccountWorkWorkItemModel> workWorkItemModelList = new List<AccountWorkWorkItemModel>();
      using (PerformanceTimer.StartMeasure(requestContext, nameof (PageWorkItems)))
      {
        if (!workItemIds.Any<int>())
          return (IList<AccountWorkWorkItemModel>) new List<AccountWorkWorkItemModel>();
        ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
        IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
        IEnumerable<FieldEntry> fieldEntries = ((IEnumerable<string>) MyWorkService.s_pagedFields).Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<int> workItemIds1 = workItemIds;
        IEnumerable<FieldEntry> fields = fieldEntries;
        DateTime? asOf = new DateTime?();
        return (IList<AccountWorkWorkItemModel>) service.GetWorkItemFieldValues(requestContext1, workItemIds1, fields, asOf: asOf).Select<WorkItemFieldData, AccountWorkWorkItemModel>((Func<WorkItemFieldData, AccountWorkWorkItemModel>) (fieldData => new AccountWorkWorkItemModel()
        {
          AssignedTo = fieldData.AssignedTo,
          Id = fieldData.Id,
          ChangedDate = fieldData.ModifiedDate,
          State = fieldData.State,
          Title = fieldData.Title,
          WorkItemType = fieldData.WorkItemType,
          TeamProject = fieldData.GetProjectName(requestContext)
        })).OrderByDescending<AccountWorkWorkItemModel, DateTime>((Func<AccountWorkWorkItemModel, DateTime>) (item => item.ChangedDate)).ToList<AccountWorkWorkItemModel>();
      }
    }

    private IEnumerable<int> RunAssignedToMeQuery(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> states)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(states, nameof (states));
      using (PerformanceTimer.StartMeasure(requestContext, nameof (RunAssignedToMeQuery)))
      {
        bool useExclusionQueries = WorkItemTrackingFeatureFlags.IsQueryMyWorkByExcludingDoneStatesEnabled(requestContext);
        if (!useExclusionQueries && states.Count == 0)
          return Enumerable.Empty<int>();
        string wiqlQuery = MyWorkService.GetWIQLQuery(states, useExclusionQueries);
        return (IEnumerable<int>) this.RunQuery(requestContext, wiqlQuery);
      }
    }

    private IReadOnlyCollection<int> RunQuery(
      IVssRequestContext requestContext,
      string wiql,
      bool isFollowed = false)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (RunQuery)))
      {
        IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
        int maxCount = this.GetMaxCount(requestContext, isFollowed);
        IVssRequestContext requestContext1 = requestContext;
        string wiql1 = wiql;
        int num = maxCount;
        Guid? projectId = new Guid?();
        int topCount = num;
        return (IReadOnlyCollection<int>) service.ExecuteQuery(requestContext1, wiql1, projectId: projectId, topCount: topCount).WorkItemIds.ToList<int>();
      }
    }

    public int GetMaxCount(IVssRequestContext requestContext, bool isFollowed)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return isFollowed ? service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MyWork/FollowedResultsCount", 200) : service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MyWork/ResultsCount", 50);
    }

    private static string GetWIQLQuery(IReadOnlyCollection<string> states, bool useExclusionQueries)
    {
      string empty = string.Empty;
      return !useExclusionQueries ? (states.Count != 1 ? string.Format("\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] IN ('{0}') \r\n                            ORDER BY [System.ChangedDate] DESC", (object) string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues((IEnumerable<string>) states))) : string.Format("\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] = '{0}'\r\n                            ORDER BY [System.ChangedDate] DESC", (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(states.First<string>()))) : (states.Count != 0 ? (states.Count != 1 ? string.Format("\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND [System.State] NOT IN ('{0}')\r\n                            ORDER BY [System.ChangedDate] DESC", (object) string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues((IEnumerable<string>) states))) : string.Format("\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                                AND NOT [System.State] = '{0}'\r\n                            ORDER BY [System.ChangedDate] DESC", (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(states.First<string>()))) : string.Format("\r\n                            SELECT [System.Id]\r\n                            FROM WorkItems \r\n                            WHERE [System.AssignedTo] = @me \r\n                            ORDER BY [System.ChangedDate] DESC"));
    }

    private IReadOnlyCollection<string> GetDoingStates(IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetDoingStates)))
        return requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStates(requestContext, StateGroup.Doing);
    }

    private IReadOnlyCollection<string> GetDoneStates(IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetDoneStates)))
      {
        IWorkItemMetadataFacadeService service = requestContext.GetService<IWorkItemMetadataFacadeService>();
        IReadOnlyCollection<string> workItemStates = service.GetWorkItemStates(requestContext, StateGroup.Done, false);
        if (!WorkItemTrackingFeatureFlags.IsQueryMyWorkByExcludingDoneStatesEnabled(requestContext))
          return workItemStates;
        IReadOnlyCollection<string> doingStates = service.GetWorkItemStates(requestContext, StateGroup.Doing, false);
        return (IReadOnlyCollection<string>) workItemStates.Where<string>((Func<string, bool>) (state => !doingStates.Contains<string>(state))).ToHashSet<string>();
      }
    }
  }
}
