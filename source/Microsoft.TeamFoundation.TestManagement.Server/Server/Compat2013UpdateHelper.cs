// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Compat2013UpdateHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class Compat2013UpdateHelper
  {
    internal static string ConvertTcmStateToWorkItemState(
      TestManagementRequestContext context,
      string teamProjectName,
      byte tcmState,
      string categoryRefName)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      return TCMWorkItemBase.CreateTCMWorkItem(context, categoryRefName).ToWorkItemState(context, projectUriFromName, tcmState, false);
    }

    internal static byte ConvertWorkItemStateToTcmState(
      TestManagementRequestContext context,
      string teamProjectName,
      string witState,
      string categoryRefName)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      return TCMWorkItemBase.CreateTCMWorkItem(context, categoryRefName).ToTcmState(context, projectUriFromName, witState);
    }

    internal static List<TestPlan> ConvertFromWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      List<TestPlan> plans)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "Compat2013UpdateHelper.ConvertFromWorkItem");
        if (plans != null)
        {
          foreach (TestPlan plan in plans)
          {
            plan.State = Compat2013UpdateHelper.WorkItemStateToTcmState(context, teamProjectName, plan.Status, WitCategoryRefName.TestPlan);
            plan.SuitesMetaData = Compat2013UpdateHelper.ConvertFromWorkItem(context, teamProjectName, plan.SuitesMetaData);
          }
        }
        return plans;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "Compat2013UpdateHelper.ConvertFromWorkItem");
      }
    }

    internal static string ConvertTestPlanDescriptionToHtml(string description) => !string.IsNullOrEmpty(description) ? HtmlConverter.ConvertToHtml((object) description, true) : description;

    internal static List<ServerTestSuite> ConvertFromWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      List<ServerTestSuite> suites)
    {
      if (suites != null)
      {
        foreach (ServerTestSuite suite in suites)
          suite.State = Compat2013UpdateHelper.WorkItemStateToTcmState(context, teamProjectName, suite.Status, WitCategoryRefName.TestSuite);
      }
      return suites;
    }

    internal static byte WorkItemStateToTcmState(
      TestManagementRequestContext context,
      string teamProjectName,
      string workItemState,
      string categoryRefName)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "Compat2013UpdateHelper.WorkItemStateToTcmState");
        return Compat2013UpdateHelper.ConvertWorkItemStateToTcmState(context, teamProjectName, workItemState, categoryRefName);
      }
      catch (TestManagementValidationException ex)
      {
        context.TraceError("BusinessLayer", ex.Message);
        TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, categoryRefName);
        List<StateTypeEnumAndStateString> list = tcmWorkItem.GetDefaultStatesMap().Where<StateTypeEnumAndStateString>((Func<StateTypeEnumAndStateString, bool>) (m => string.Equals(m.State, workItemState, StringComparison.OrdinalIgnoreCase))).ToList<StateTypeEnumAndStateString>();
        return list != null && list.Count != 0 ? tcmWorkItem.FromMetaState((StateTypeEnum) list[0].StateType) : tcmWorkItem.GetDefaultTcmState();
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "Compat2013UpdateHelper.WorkItemStateToTcmState");
      }
    }

    internal static string ConvertTcmStateToWorkItemState(
      TestManagementRequestContext context,
      string teamProjectName,
      int workItemId,
      byte tcmState,
      string categoryRefName)
    {
      string stateFromWorkItem = Compat2013UpdateHelper.GetStateFromWorkItem(context, teamProjectName, workItemId, categoryRefName);
      if (!string.IsNullOrEmpty(stateFromWorkItem))
      {
        if ((int) Compat2013UpdateHelper.ConvertWorkItemStateToTcmState(context, teamProjectName, stateFromWorkItem, categoryRefName) == (int) tcmState)
          return stateFromWorkItem;
      }
      else
        context.TraceWarning("BusinessLayer", "No valid state of workitem object could be fetched for category:'{0}' id:'{1}'", (object) categoryRefName, (object) workItemId);
      return Compat2013UpdateHelper.ConvertTcmStateToWorkItemState(context, teamProjectName, tcmState, categoryRefName);
    }

    private static string GetStateFromWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      int workItemId,
      string categoryRefName)
    {
      TestManagementRequestContext context1 = context;
      string projectName = teamProjectName;
      List<int> workItemIds = new List<int>();
      workItemIds.Add(workItemId);
      string[] workItemFields = new string[1]
      {
        "System.State"
      };
      string workItemCategoryRefName = categoryRefName;
      List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems(context1, projectName, workItemIds, workItemFields, workItemCategoryRefName, false);
      if (workItems != null && workItems.Count == 1)
        return workItems[0].State;
      context.TraceError("BusinessLayer", "No valid workitem object could be fetched for category:'{0}' id:'{1}'", (object) categoryRefName, (object) workItemId);
      if (string.Equals(WitCategoryRefName.TestPlan, categoryRefName, StringComparison.OrdinalIgnoreCase))
        throw new TestObjectNotFoundException(context.RequestContext, workItemId, ObjectTypes.TestPlan);
      throw new TestObjectNotFoundException(context.RequestContext, workItemId, ObjectTypes.TestSuite);
    }
  }
}
