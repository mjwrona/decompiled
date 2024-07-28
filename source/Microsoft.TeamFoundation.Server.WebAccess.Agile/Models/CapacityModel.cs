// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.CapacityModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal class CapacityModel : BaseModel
  {
    private TeamCapacity m_teamCapacity;
    private IReadOnlyCollection<string> m_allowedActivityValues;
    private JsObject m_teamCapacityData;
    private TaskBoardData m_taskboardData;
    private IDictionary<string, string> m_allowedActivityValuesDictionary;
    private ILookup<string, TeamMemberCapacity> m_teamMemberLookup;
    private AgileTracingUtils m_tracingUtils = new AgileTracingUtils();
    private const string c_parentIdFieldName = "PARENT-ID";
    private const string c_actualActivityIdFieldName = "ACTUAL-ACTIVITY-NAME";
    private const string c_emptyValue = "__empty";
    private const int c_maxInitCapacityTeamMemberCount = 100;

    public CapacityModel(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo,
      WebApiTeam team,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode)
      : base(requestContext, agileSettings, projectInfo, team, requestIterationNode)
    {
    }

    public CapacityModel(TfsWebContext tfsWebContext, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode)
      : base(tfsWebContext, requestIterationNode)
    {
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.AddObject(this.GetCapacityControlData(true));
      JsObject data1 = new JsObject();
      data1.Add("aggregatedCapacity", (object) this.GetAggregatedCapacityViewDataJSObject());
      json.AddObject(data1);
      JsObject data2 = new JsObject();
      data2.Add("capacityOptions", (object) this.GetCapacityOptionsViewDataJSObject());
      json.AddObject(data2);
      json.AddObject(this.GetSprintProgressData());
      return json;
    }

    public JsObject GetCapacityControlData(bool preFill = false) => this.GetCapacityControlData(this.RequestIterationNode.CssNodeId, this.GetOrFillIterationCapacity(preFill), this.Team, this.RequestIterationNode.StartDate, this.RequestIterationNode.FinishDate, (IEnumerable<string>) this.AllowedActivityValues);

    public TeamCapacity GetOrFillIterationCapacity(bool preFill = false)
    {
      if (this.m_teamCapacity != null && (!preFill || this.m_teamCapacity.TeamMemberCapacityCollection.Any<TeamMemberCapacity>()))
        return this.m_teamCapacity;
      this.m_teamCapacity = CapacityModel.GetOrFillIterationCapacity(this.TfsRequestContext, this.Team, this.RequestIterationNode, this.AgileSettings.TeamSettings, preFill);
      return this.m_teamCapacity;
    }

    public static TeamCapacity GetOrFillIterationCapacity(
      IVssRequestContext requestContext,
      WebApiTeam team,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode,
      ITeamSettings teamSettings,
      bool preFill,
      SortedIterationSubscriptions sortedIteration = null)
    {
      AgileTracingUtils agileTracingUtils = new AgileTracingUtils();
      using (agileTracingUtils.TraceBlock(requestContext, 290777, 290778, memberName: nameof (GetOrFillIterationCapacity)))
      {
        Guid cssNodeId = requestIterationNode.CssNodeId;
        ITeamConfigurationService service = requestContext.GetService<ITeamConfigurationService>();
        TeamCapacity iterationCapacity1 = service.GetTeamIterationCapacity(requestContext, team, teamSettings, requestIterationNode.CssNodeId);
        if (preFill && !iterationCapacity1.TeamMemberCapacityCollection.Any<TeamMemberCapacity>())
        {
          SortedIterationSubscriptions iterationSubscriptions = sortedIteration ?? teamSettings.GetIterationTimeline(requestContext, team.ProjectId);
          Guid guid1 = Guid.Empty;
          Guid guid2 = Guid.Empty;
          Guid guid3 = Guid.Empty;
          if (iterationSubscriptions.IsValidSubscription)
          {
            guid1 = iterationSubscriptions.CurrentIteration.CssNodeId;
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode1 = iterationSubscriptions.FutureIterations.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
            guid3 = treeNode1 != null ? treeNode1.CssNodeId : Guid.Empty;
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode2 = iterationSubscriptions.PreviousIterations.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
            guid2 = treeNode2 != null ? treeNode2.CssNodeId : Guid.Empty;
          }
          if (cssNodeId == guid1 || cssNodeId == guid3)
          {
            Guid iterationId = cssNodeId == guid1 ? guid2 : guid1;
            bool flag = false;
            if (iterationId != Guid.Empty)
            {
              TeamCapacity iterationCapacity2 = service.GetTeamIterationCapacity(requestContext, team, teamSettings, iterationId);
              if (iterationCapacity2.TeamMemberCapacityCollection.Any<TeamMemberCapacity>())
              {
                foreach (TeamMemberCapacity teamMemberCapacity in (IEnumerable<TeamMemberCapacity>) iterationCapacity2.TeamMemberCapacityCollection)
                {
                  teamMemberCapacity.DaysOffDates = (IList<DateRange>) new List<DateRange>();
                  if (teamMemberCapacity.Activities != null)
                  {
                    foreach (Activity activity in (IEnumerable<Activity>) teamMemberCapacity.Activities)
                      activity.CapacityPerDay = 0.0f;
                  }
                }
                iterationCapacity2.TeamDaysOffDates = new List<DateRange>();
                flag = true;
                service.SaveTeamIterationCapacity(requestContext.Elevate(), team, teamSettings, cssNodeId, iterationCapacity2);
              }
            }
            if (!flag)
            {
              try
              {
                Microsoft.VisualStudio.Services.Identity.Identity[] array1 = new TeamMembershipExpander().GetMaterializedTeamMembers(requestContext, team.Id).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
                if (((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) array1).Any<Microsoft.VisualStudio.Services.Identity.Identity>())
                {
                  Array.Sort<Microsoft.VisualStudio.Services.Identity.Identity>(array1, (Comparison<Microsoft.VisualStudio.Services.Identity.Identity>) ((user1, user2) => VssStringComparer.IdentityName.Compare(user1.DisplayName, user2.DisplayName)));
                  Microsoft.VisualStudio.Services.Identity.Identity[] array2 = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) array1).Take<Microsoft.VisualStudio.Services.Identity.Identity>(100).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
                  service.SaveTeamIterationCapacity(requestContext.Elevate(), team, teamSettings, cssNodeId, new TeamCapacity()
                  {
                    TeamMemberCapacityCollection = (IReadOnlyCollection<TeamMemberCapacity>) ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) array2).Select<Microsoft.VisualStudio.Services.Identity.Identity, TeamMemberCapacity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, TeamMemberCapacity>) (tm =>
                    {
                      return new TeamMemberCapacity()
                      {
                        Activities = (IList<Activity>) new Activity[1]
                        {
                          new Activity()
                          {
                            Name = string.Empty,
                            CapacityPerDay = 0.0f
                          }
                        },
                        DaysOffDates = (IList<DateRange>) new List<DateRange>(),
                        TeamMemberId = tm.Id
                      };
                    })).ToList<TeamMemberCapacity>()
                  });
                }
              }
              catch (Exception ex)
              {
                agileTracingUtils.TraceException(requestContext, 599999, ex, TfsTraceLayers.Controller);
              }
            }
          }
        }
        return service.GetTeamIterationCapacity(requestContext, team, teamSettings, cssNodeId);
      }
    }

    private JsObject GetCapacityControlData(
      Guid iterationId,
      TeamCapacity teamCapacity,
      WebApiTeam team,
      DateTime? startDate,
      DateTime? endDate,
      IEnumerable<string> activityValues)
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290012, 290013, memberName: nameof (GetCapacityControlData)))
      {
        if (this.m_teamCapacityData == null)
        {
          DateTime dateTime1 = TimeZoneInfo.ConvertTime(DateTime.Now, this.TfsRequestContext.GetCollectionTimeZone());
          DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, 0, 0, 0, DateTimeKind.Utc);
          List<int> list = ((IEnumerable<DayOfWeek>) this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TfsRequestContext, team, false, false).Weekends.Days).Select<DayOfWeek, int>((Func<DayOfWeek, int>) (d => (int) d)).ToList<int>();
          JsObject jsObject = new JsObject();
          jsObject.Add("TeamCapacity", (object) teamCapacity.ToJson(this.TfsRequestContext, iterationId));
          jsObject.Add("ActivityValues", (object) activityValues);
          jsObject.Add("IterationId", (object) iterationId);
          jsObject.Add("IterationStartDate", (object) startDate);
          jsObject.Add("IterationEndDate", (object) endDate);
          jsObject.Add("Weekends", (object) list);
          jsObject.Add("CurrentDate", (object) dateTime2);
          this.m_teamCapacityData = jsObject;
        }
        return this.m_teamCapacityData;
      }
    }

    public JsObject GetAggregatedCapacityViewDataJSObject()
    {
      AggregatedCapacityDataViewModel capacityViewData = this.GetAggregatedCapacityViewData();
      JsObject viewDataJsObject = new JsObject();
      viewDataJsObject.Add("remainingWorkField", (object) capacityViewData.RemainingWorkField);
      viewDataJsObject.Add("aggregatedCapacity", (object) capacityViewData.AggregatedCapacity);
      viewDataJsObject.Add("previousValueData", (object) capacityViewData.PreviousValueData);
      viewDataJsObject.Add("aggregatedCapacityLimitExceeded", (object) capacityViewData.AggregatedCapacityLimitExceeded);
      return viewDataJsObject;
    }

    public AggregatedCapacityDataViewModel GetAggregatedCapacityViewData()
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290018, 290019, memberName: nameof (GetAggregatedCapacityViewData)))
      {
        List<string> stringList = new List<string>()
        {
          "PARENT-ID",
          "System.AssignedTo"
        };
        TypeField activityField = this.AgileSettings.Process.ActivityField;
        if (activityField != null)
        {
          stringList.Add(activityField.Name);
          stringList.Add("ACTUAL-ACTIVITY-NAME");
        }
        this.InitializeTaskBoardData(stringList.Where<string>((Func<string, bool>) (f => f != "PARENT-ID" && f != "ACTUAL-ACTIVITY-NAME")));
        AggregateFieldValues aggregatedCapacity = (AggregateFieldValues) null;
        PreviousFieldValues previousValueData = (PreviousFieldValues) null;
        bool flag = false;
        try
        {
          this.BuildAggregatedCapacity((IEnumerable<string>) stringList, new CapacityModel.TryGetFieldValueDelegate(this.CustomLookupHandler), out aggregatedCapacity, out previousValueData);
        }
        catch (AggregatedCapacityLimitExceededException ex)
        {
          this.m_tracingUtils.TraceWarning(this.TfsRequestContext, 290531, "Aggregated capacity data limit reached, limit is {0}, retrieved work item count is {1}", (object) ex.WorkItemLimit, (object) ex.WorkItemCount);
          flag = true;
        }
        return new AggregatedCapacityDataViewModel()
        {
          RemainingWorkField = this.AgileSettings.Process.RemainingWorkField.Name,
          AggregatedCapacity = aggregatedCapacity,
          PreviousValueData = previousValueData,
          AggregatedCapacityLimitExceeded = flag
        };
      }
    }

    public JsObject GetCapacityOptionsViewDataJSObject(bool isFirstIteration = false)
    {
      CapacityOptionsViewModel capacityOptionsViewData = this.GetCapacityOptionsViewData(isFirstIteration);
      JsObject viewDataJsObject = new JsObject();
      viewDataJsObject.Add("accountCurrentDate", (object) capacityOptionsViewData.AccountCurrentDate);
      viewDataJsObject.Add("activityFieldName", (object) capacityOptionsViewData.ActivityFieldName);
      viewDataJsObject.Add("allowedActivities", (object) capacityOptionsViewData.AllowedActivityValues);
      viewDataJsObject.Add("assignedToFieldDisplayName", (object) capacityOptionsViewData.AssignedToDisplayName);
      viewDataJsObject.Add("activityFieldDisplayName", (object) capacityOptionsViewData.ActivityFieldDisplayName);
      viewDataJsObject.Add("suffixFormat", (object) capacityOptionsViewData.SuffixFormat);
      viewDataJsObject.Add("childWorkItemTypes", (object) capacityOptionsViewData.ChildWorkItemTypes);
      viewDataJsObject.Add("inline", (object) capacityOptionsViewData.Inline);
      viewDataJsObject.Add("iterationId", (object) capacityOptionsViewData.IterationId);
      viewDataJsObject.Add("isEmpty", (object) capacityOptionsViewData.IsEmpty);
      viewDataJsObject.Add(nameof (isFirstIteration), (object) capacityOptionsViewData.IsFirstIteration);
      return viewDataJsObject;
    }

    public CapacityOptionsViewModel GetCapacityOptionsViewData(bool isFirstIteration = false)
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290020, 290021, memberName: nameof (GetCapacityOptionsViewData)))
      {
        FieldDefinition field1;
        this.WitService.TryGetFieldDefinitionByName(this.TfsRequestContext, "System.AssignedTo", out field1);
        string name = field1.Name;
        string str = (string) null;
        if (this.AgileSettings.Process.ActivityField != null)
        {
          FieldDefinition field2;
          this.WitService.TryGetFieldDefinitionByName(this.TfsRequestContext, this.AgileSettings.Process.ActivityField.Name, out field2);
          str = field2?.Name;
        }
        IReadOnlyCollection<string> workItemTypes = this.AgileSettings.BacklogConfiguration.TaskBacklog.WorkItemTypes;
        return new CapacityOptionsViewModel()
        {
          AccountCurrentDate = AgileUtils.GetTeamCapacityCollectionCurrentDate(this.TfsRequestContext),
          ActivityFieldName = str != null ? this.AgileSettings.Process.ActivityField.Name : (string) null,
          AllowedActivityValues = (IEnumerable<string>) this.AllowedActivityValues,
          AssignedToDisplayName = name,
          ActivityFieldDisplayName = str,
          SuffixFormat = this.AgileSettings.Process.RemainingWorkField.Format,
          ChildWorkItemTypes = (IEnumerable<string>) workItemTypes,
          Inline = true,
          IterationId = this.RequestIterationNode.CssNodeId,
          IsEmpty = this.GetOrFillIterationCapacity().IsEmpty,
          IsFirstIteration = isFirstIteration
        };
      }
    }

    public JsObject GetSprintProgressData()
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290022, 290023, memberName: nameof (GetSprintProgressData)))
      {
        BacklogLevelConfiguration requirementBacklog = this.AgileSettings.BacklogConfiguration.RequirementBacklog;
        HashSet<string> notStartedStates = new HashSet<string>((IEnumerable<string>) this.AgileSettings.BacklogConfiguration.GetWorkItemStates(requirementBacklog, new WorkItemStateCategory[1]
        {
          WorkItemStateCategory.Proposed
        }));
        HashSet<string> inProgressStates = new HashSet<string>((IEnumerable<string>) this.AgileSettings.BacklogConfiguration.GetWorkItemStates(requirementBacklog, new WorkItemStateCategory[2]
        {
          WorkItemStateCategory.InProgress,
          WorkItemStateCategory.Resolved
        }));
        int num1 = this.TaskboardData.WorkItemData.Count<WorkItemData>((Func<WorkItemData, bool>) (wid => wid.IsParentType && notStartedStates.Contains((string) wid.GetFieldValue("System.State"))));
        int num2 = this.TaskboardData.WorkItemData.Count<WorkItemData>((Func<WorkItemData, bool>) (wid => wid.IsParentType && inProgressStates.Contains((string) wid.GetFieldValue("System.State"))));
        JsObject sprintProgressData = new JsObject();
        sprintProgressData.Add("storiesPlural", (object) this.AgileSettings.Process.RequirementBacklog.PluralName);
        sprintProgressData.Add("storiesNotStarted", (object) num1);
        sprintProgressData.Add("storiesInProgress", (object) num2);
        return sprintProgressData;
      }
    }

    public TaskBoardData TaskboardData
    {
      get => this.m_taskboardData;
      internal set => this.m_taskboardData = value;
    }

    public bool UserHasWriteAccess => this.TfsRequestContext.GetService<ITeamService>().UserHasPermission(this.TfsRequestContext, this.Team.Identity, 2);

    private IReadOnlyCollection<string> AllowedActivityValues
    {
      get
      {
        if (this.m_allowedActivityValues == null)
        {
          if (this.AgileSettings.Process.ActivityField != null)
          {
            FieldDefinition field;
            if (this.WitService.TryGetFieldDefinitionByName(this.TfsRequestContext, this.AgileSettings.Process.ActivityField.Name, out field))
            {
              this.m_allowedActivityValues = (IReadOnlyCollection<string>) this.WitService.GetAllowedValues(this.TfsRequestContext, field.Id, this.ProjectInfo.Name).ToList<string>();
              if (this.m_allowedActivityValues == null || this.m_allowedActivityValues.Count == 0)
                this.TfsRequestContext.Trace(290026, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, string.Format("No allowed values for {0} teamId:{1} project:{2}", (object) field.ReferenceName, (object) this.AgileSettings.Team?.Id, (object) this.AgileSettings.ProjectName));
            }
          }
          else
            this.m_allowedActivityValues = (IReadOnlyCollection<string>) new List<string>();
        }
        return this.m_allowedActivityValues;
      }
    }

    private IDictionary<string, string> AllowedActivityValuesDictionary
    {
      get
      {
        if (this.m_allowedActivityValuesDictionary == null)
          this.m_allowedActivityValuesDictionary = (IDictionary<string, string>) this.AllowedActivityValues.ToDictionary<string, string>((Func<string, string>) (activity => activity));
        return this.m_allowedActivityValuesDictionary;
      }
    }

    private ILookup<string, TeamMemberCapacity> TeamMemberLookup
    {
      get
      {
        if (this.m_teamMemberLookup == null)
          this.m_teamMemberLookup = this.GetOrFillIterationCapacity().TeamMemberCapacityCollection.ToLookup<TeamMemberCapacity, string>((Func<TeamMemberCapacity, string>) (teamMember => teamMember.GetTeamMemberDisplayName(this.TfsRequestContext)));
        return this.m_teamMemberLookup;
      }
    }

    private void BuildAggregatedCapacity(
      IEnumerable<string> aggregateFieldIds,
      CapacityModel.TryGetFieldValueDelegate fieldLookupHandler,
      out AggregateFieldValues aggregatedCapacity,
      out PreviousFieldValues previousValueData)
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290024, 290025, memberName: nameof (BuildAggregatedCapacity)))
      {
        aggregatedCapacity = new AggregateFieldValues();
        previousValueData = new PreviousFieldValues();
        aggregatedCapacity.SetFields(aggregateFieldIds);
        IEnumerable<WorkItemData> source = this.TaskboardData.WorkItemData.Where<WorkItemData>((Func<WorkItemData, bool>) (wi => !wi.IsParentType));
        int workItemCountLimit = this.AgileSettings.Process.TaskBacklog.WorkItemCountLimit;
        if (source.Count<WorkItemData>() > workItemCountLimit)
          throw new AggregatedCapacityLimitExceededException(workItemCountLimit, source.Count<WorkItemData>());
        foreach (WorkItemData workItemData in source)
        {
          object obj1 = (object) null;
          if (this.TryLookupValue(fieldLookupHandler, workItemData, this.AgileSettings.Process.RemainingWorkField.Name, out obj1))
          {
            double capacity = Convert.ToDouble(obj1, (IFormatProvider) CultureInfo.InvariantCulture);
            this.StorePreviousValue(workItemData, obj1, this.AgileSettings.Process.RemainingWorkField.Name, previousValueData);
            foreach (string aggregateFieldId in aggregateFieldIds)
            {
              object obj2 = (object) null;
              if (this.TryLookupValue(fieldLookupHandler, workItemData, aggregateFieldId, out obj2))
              {
                object obj3 = obj2;
                if (obj3 == null || obj3 is string && string.IsNullOrEmpty((string) obj3))
                  obj3 = (object) "__empty";
                aggregatedCapacity.AddCapacity(aggregateFieldId, obj3.ToString(), capacity);
                this.StorePreviousValue(workItemData, obj2, aggregateFieldId, previousValueData);
              }
            }
          }
        }
      }
    }

    private bool TryLookupValue(
      CapacityModel.TryGetFieldValueDelegate fieldLookupHandler,
      WorkItemData workItemData,
      string fieldName,
      out object value)
    {
      value = (object) null;
      bool flag = fieldLookupHandler(workItemData, fieldName, out value);
      if (!flag)
        flag = workItemData.TryGetFieldValue(fieldName, out value);
      return flag;
    }

    private void StorePreviousValue(
      WorkItemData workItem,
      object value,
      string fieldName,
      PreviousFieldValues previousValueData)
    {
      if (value == null)
        value = (object) string.Empty;
      string workItemId = workItem.ID.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      previousValueData.SetData(fieldName, workItemId, value);
    }

    private bool CustomLookupHandler(WorkItemData workItemData, string fieldName, out object value)
    {
      bool flag = false;
      value = (object) null;
      TypeField activityField = this.AgileSettings.Process.ActivityField;
      if (VssStringComparer.FieldName.Compare(fieldName, "PARENT-ID") == 0)
      {
        value = (object) workItemData.ParentID;
        flag = true;
      }
      else if (activityField != null && (VssStringComparer.FieldName.Compare(fieldName, activityField.Name) == 0 || VssStringComparer.FieldName.Compare(fieldName, "ACTUAL-ACTIVITY-NAME") == 0))
      {
        if (workItemData.TryGetFieldValue(activityField.Name, out value))
        {
          string key = (string) value;
          flag = true;
          if (!string.IsNullOrEmpty(key) && !this.AllowedActivityValuesDictionary.ContainsKey(key))
          {
            value = (object) string.Empty;
            return flag;
          }
        }
        if (string.IsNullOrEmpty((string) value))
        {
          if (fieldName == "ACTUAL-ACTIVITY-NAME")
          {
            value = (object) string.Empty;
            return true;
          }
          object obj;
          if (workItemData.TryGetFieldValue("System.AssignedTo", out obj))
          {
            string key = (string) obj;
            if (!string.IsNullOrEmpty(key) && this.TeamMemberLookup.Contains(key))
            {
              TeamMemberCapacity teamMemberCapacity = this.TeamMemberLookup[key].First<TeamMemberCapacity>();
              if (teamMemberCapacity.Activities != null && teamMemberCapacity.Activities.Count == 1)
              {
                value = (object) teamMemberCapacity.Activities.First<Activity>().Name;
                flag = true;
              }
            }
          }
        }
      }
      return flag;
    }

    internal void InitializeTaskBoardData(IEnumerable<string> fields)
    {
      if (this.m_taskboardData != null)
        return;
      this.m_taskboardData = TaskBoardData.GetTaskBoardData(this.TfsRequestContext, this.AgileSettings, fields, this.RequestIterationNode.GetPath(this.TfsRequestContext), false);
    }

    private delegate bool TryGetFieldValueDelegate(
      WorkItemData workItemData,
      string fieldName,
      out object value);
  }
}
