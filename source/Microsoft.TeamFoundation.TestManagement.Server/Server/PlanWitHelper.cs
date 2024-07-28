// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PlanWitHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class PlanWitHelper
  {
    internal static IEnumerable<IdToAreaIteration> QueryAreaIterationForPlanIds(
      TestManagementRequestContext context,
      List<int> planIds)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "PlanWitHelper.QueryAreaIterationForPlanIds");
        List<IdToAreaIteration> idToAreaIterationList = new List<IdToAreaIteration>();
        IWitHelper service = context.RequestContext.GetService<IWitHelper>();
        IEnumerable<WorkItem> workItems = service.GetWorkItems(context.RequestContext, planIds, new List<string>()
        {
          "System.Id",
          "System.AreaPath",
          "System.IterationPath"
        });
        if (workItems != null)
        {
          foreach (WorkItem workItem in workItems)
          {
            IdToAreaIteration idToAreaIteration1 = new IdToAreaIteration();
            idToAreaIteration1.Id = workItem.Id.Value;
            string path1;
            IdAndString idAndThrow;
            if (workItem.Fields.TryGetValue<string, string>("System.AreaPath", out path1) && !string.IsNullOrEmpty(path1))
            {
              IdToAreaIteration idToAreaIteration2 = idToAreaIteration1;
              idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, service.TranslateCSSPath(path1));
              int id = idAndThrow.Id;
              idToAreaIteration2.AreaId = id;
            }
            string path2;
            if (workItem.Fields.TryGetValue<string, string>("System.IterationPath", out path2) && !string.IsNullOrEmpty(path2))
            {
              IdToAreaIteration idToAreaIteration3 = idToAreaIteration1;
              idAndThrow = context.IterationsCache.GetIdAndThrow(context, service.TranslateCSSPath(path2));
              int id = idAndThrow.Id;
              idToAreaIteration3.IterationId = id;
            }
            idToAreaIterationList.Add(idToAreaIteration1);
            context.TraceVerbose("BusinessLayer", "PlanWitHelper.QueryAreaIterationForPlanIds added plan entry Id:{0} Area:{1} Iteration:{2}", (object) idToAreaIteration1.Id, (object) idToAreaIteration1.AreaId, (object) idToAreaIteration1.IterationId);
          }
        }
        return (IEnumerable<IdToAreaIteration>) idToAreaIterationList;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "PlanWitHelper.QueryAreaIterationForPlanIds");
      }
    }

    internal static IEnumerable<KeyValuePair<int, string>> QueryTitleForPlanIds(
      TestManagementRequestContext context,
      List<int> planIds)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "PlanWitHelper.QueryTitleForPlanIds");
        List<KeyValuePair<int, string>> keyValuePairList = new List<KeyValuePair<int, string>>();
        IEnumerable<WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, planIds, new List<string>()
        {
          "System.Id",
          "System.Title"
        });
        if (workItems != null)
        {
          foreach (WorkItem workItem in workItems)
          {
            int key = workItem.Id.Value;
            string str;
            if (workItem.Fields.TryGetValue<string, string>("System.Title", out str) && !string.IsNullOrEmpty(str))
              keyValuePairList.Add(new KeyValuePair<int, string>(key, str));
            context.TraceVerbose("BusinessLayer", "PlanWitHelper.QueryTitleForPlanIds added plan entry Id:{0} Title:{1}.", (object) key, (object) str);
          }
        }
        return (IEnumerable<KeyValuePair<int, string>>) keyValuePairList;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "PlanWitHelper.QueryTitleForPlanIds");
      }
    }

    internal static void UpdateTestPlanFromWorkItemAndValidate(
      TestManagementRequestContext context,
      GuidAndString projectGuidAndString,
      string projectName,
      TestPlan plan,
      int workItemId)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "PlanWitHelper.UpdateTestPlanFromWorkItem");
        TestPlan testPlan = PlanWitHelper.FetchPlan(context, projectName, workItemId);
        PlanWitHelper.ValidatePlanWorkItem(context, projectName, testPlan);
        PlanWitHelper.ValidateAreaAndIterationPath(context, projectGuidAndString, testPlan);
        PlanWitHelper.ValidatePermission(context, testPlan, projectGuidAndString);
        PlanWitHelper.CopyDataFromPlan(context, testPlan, plan);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "PlanWitHelper.UpdateTestPlanFromWorkItem");
      }
    }

    internal static TestPlan FetchPlan(
      TestManagementRequestContext context,
      string projectName,
      int workItemId)
    {
      List<TestPlan> testPlanList = TestPlanWorkItem.FetchPlans(context, projectName, new List<int>()
      {
        workItemId
      });
      if (testPlanList != null && testPlanList.Count == 1)
        return testPlanList[0];
      context.TraceError("BusinessLayer", "PlanWitHelper.UpdateTestPlanFromWorkItem: Unable to fetch work item with id {0}", (object) workItemId);
      bool projectOrCategoryMismatch = false;
      if (TCMWorkItemBase.WorkItemExists(context, projectName, workItemId, WitCategoryRefName.TestPlan, out projectOrCategoryMismatch) && !projectOrCategoryMismatch)
        throw new AccessDeniedException(ServerResources.CannotViewWorkItems);
      throw new TestObjectNotFoundException(context.RequestContext, workItemId, ObjectTypes.TestPlan);
    }

    private static void ValidateAreaAndIterationPath(
      TestManagementRequestContext context,
      GuidAndString projectGuidAndString,
      TestPlan fetchedPlan)
    {
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      service.ValidateTreePath(context as TfsTestManagementRequestContext, TreeStructureType.Area, fetchedPlan.AreaPath, projectGuidAndString);
      service.ValidateTreePath(context as TfsTestManagementRequestContext, TreeStructureType.Iteration, fetchedPlan.Iteration, projectGuidAndString);
    }

    private static void ValidatePermission(
      TestManagementRequestContext context,
      TestPlan fetchedPlan,
      GuidAndString projectId)
    {
      IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, fetchedPlan.AreaPath);
      context.SecurityManager.CheckManageTestPlansPermission(context, idAndThrow.String);
      context.SecurityManager.CheckManageTestSuitesPermission(context, idAndThrow.String);
    }

    private static void CopyDataFromPlan(
      TestManagementRequestContext context,
      TestPlan fromPlan,
      TestPlan toPlan)
    {
      toPlan.PlanId = fromPlan.PlanId;
      toPlan.Revision = fromPlan.Revision;
      toPlan.Name = fromPlan.Name;
      toPlan.PlanWorkItem.Id = fromPlan.PlanWorkItem.Id;
      toPlan.PlanWorkItem.WitTypeName = fromPlan.PlanWorkItem.WitTypeName;
      toPlan.CreatedByName = fromPlan.CreatedByName;
      toPlan.CreatedByDistinctName = fromPlan.CreatedByDistinctName;
      toPlan.LastUpdatedBy = fromPlan.LastUpdatedBy;
      toPlan.LastUpdatedByName = fromPlan.LastUpdatedByName;
      toPlan.LastUpdated = fromPlan.LastUpdated;
      toPlan.State = fromPlan.State;
      toPlan.AreaPath = fromPlan.AreaPath;
      toPlan.Iteration = fromPlan.Iteration;
      toPlan.IterationId = fromPlan.IterationId;
      toPlan.StartDate = fromPlan.StartDate;
      toPlan.EndDate = fromPlan.EndDate;
      toPlan.Description = fromPlan.Description;
      toPlan.Owner = fromPlan.Owner;
      toPlan.OwnerName = fromPlan.OwnerName;
      toPlan.Status = fromPlan.Status;
      toPlan.MigrationState = fromPlan.MigrationState;
      toPlan.TeamProjectUri = fromPlan.TeamProjectUri;
      toPlan.AreaId = fromPlan.AreaId;
      toPlan.AreaUri = fromPlan.AreaUri;
    }

    private static void ValidatePlanWorkItem(
      TestManagementRequestContext context,
      string projectUri,
      TestPlan plan)
    {
      bool flag = false;
      List<WorkItemTypeCategory> itemTypeCategories = context.RequestContext.GetService<IWitHelper>().GetWorkItemTypeCategories(context.RequestContext, projectUri, (IEnumerable<string>) new List<string>()
      {
        WitCategoryRefName.TestPlan
      });
      if (itemTypeCategories != null && itemTypeCategories.Count == 1)
      {
        foreach (WorkItemTypeReference workItemType in itemTypeCategories[0].WorkItemTypes)
        {
          if (TFStringComparer.WorkItemTypeName.Equals(workItemType.Name, plan.PlanWorkItem.WitTypeName))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        context.TraceError("BusinessLayer", "Plan category is not found or work item {0} does not belong to Microsoft.TestPlanCategory", (object) plan.PlanId);
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PlanCategoryValidationError, (object) plan.PlanId));
      }
    }

    internal static Dictionary<int, string> FindPlansAreaUri(
      TestManagementRequestContext context,
      List<int> planIds,
      IEnumerable<WorkItem> result,
      bool throwIfPlanDeleted = false)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "PlanWitHelper.FindPlansAreaUri"))
        {
          context.TraceEnter("BusinessLayer", "PlanWitHelper.FindPlansAreaUri");
          Dictionary<int, string> planAreaMap = new Dictionary<int, string>();
          IWitHelper service = context.RequestContext.GetService<IWitHelper>();
          if (planIds.Count == 0)
            return planAreaMap;
          if (result == null)
            result = service.GetWorkItems(context.RequestContext, planIds, new List<string>()
            {
              "System.Id",
              "System.AreaPath"
            });
          string empty = string.Empty;
          string str = string.Empty;
          int key = 0;
          if (result != null)
          {
            foreach (WorkItem workItem in result)
            {
              try
              {
                key = workItem.Id.Value;
                if (workItem.Fields.TryGetValue<string, string>("System.AreaPath", out empty))
                {
                  if (!string.IsNullOrEmpty(empty))
                    str = service.AreaPathToUri(context, empty);
                  planAreaMap.Add(key, str);
                }
              }
              catch (Exception ex)
              {
                context.RequestContext.TraceError("BusinessLayer", "PlanWitHelper.FindPlansAreaUri: An error occured for" + string.Format("PlanId:{0}, AreaPath:{1}, AreaPathUri:{2}, e.Message:{3}, e.Stack:{4}", (object) key, (object) empty, (object) str, (object) ex.Message, (object) ex.StackTrace));
                throw;
              }
            }
          }
          if (throwIfPlanDeleted && planIds.Count != planAreaMap.Count)
          {
            int id = planIds.Find((Predicate<int>) (p => !planAreaMap.ContainsKey(p)));
            throw new TestObjectNotFoundException(context.RequestContext, id, ObjectTypes.TestPlan);
          }
          return planAreaMap;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "PlanWitHelper.FindPlansAreaUri");
      }
    }

    internal static Dictionary<int, string> GetPlanProjectMap(
      TestManagementRequestContext context,
      List<int> planIds,
      IEnumerable<WorkItem> planWorkItems)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "PlanWitHelper.GetPlanProjectMap"))
        {
          context.TraceEnter("BusinessLayer", "PlanWitHelper.GetPlanProjectMap");
          Dictionary<int, string> planProjectMap = new Dictionary<int, string>();
          if (planIds.Count == 0 || planWorkItems == null)
            return planProjectMap;
          foreach (WorkItem planWorkItem in planWorkItems)
          {
            int key = planWorkItem.Id.Value;
            string str;
            if (planWorkItem.Fields.TryGetValue<string, string>("System.TeamProject", out str))
              planProjectMap.Add(key, str);
          }
          return planProjectMap;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "PlanWitHelper.GetPlanProjectMap");
      }
    }
  }
}
