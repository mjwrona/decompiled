// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteAuditHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuiteAuditHelper
  {
    internal static string ConsturctSuiteAuditForDeletedTestCases(List<int> deletedTestCases) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoDeletedTestCases, (object) string.Join<int>(",", (IEnumerable<int>) deletedTestCases));

    internal static string ConsturctSuiteAuditForDeletedSuites(List<int> deletedSuiteIds) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoDeletedSuites, (object) string.Join<int>(",", (IEnumerable<int>) deletedSuiteIds));

    internal static string ConsturctSuiteAuditForChildSuiteCreation(int parentSuiteId) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAddedChildSuite, (object) parentSuiteId);

    internal static string ConstructSuiteAuditForPlanNameChanged(string newPlanName) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoPlanNameChanged, (object) newPlanName);

    internal static string ConstructSuiteAuditForTestCaseOrdering(List<int> testCaseIds) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoTestCasesOrdering, (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds));

    internal static string ConstructSuiteAuditForSetSuiteEntryConfigurations(
      List<int> testCaseIds,
      List<int> configIds)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoSetEntryConfigurations, (object) string.Join<int>(",", (IEnumerable<int>) configIds), (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds));
    }

    internal static string ConstructSuiteAuditForClone(int opId) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoClone, (object) opId);

    internal static string ConstructSuiteConfigAudit(
      bool inheritDefaultConfigurations,
      ServerTestSuite suite)
    {
      string str = (string) null;
      if (!inheritDefaultConfigurations && suite.InheritDefaultConfigurations)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoInhertiConfig);
      else if (suite.DefaultConfigurations != null && suite.DefaultConfigurations.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAssignedConfig, (object) string.Join<int>(",", (IEnumerable<int>) suite.DefaultConfigurations));
      return str;
    }

    internal static string ConstructSuiteAuditForLinksAddition(
      List<int> testCaseIds,
      List<int> suiteIds)
    {
      string str = (string) null;
      if (testCaseIds.Count > 0 && suiteIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAddedTestCasesAndSuites, (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds), (object) string.Join<int>(",", (IEnumerable<int>) suiteIds));
      else if (testCaseIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAddedTestCases, (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds));
      else if (suiteIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAddedSuites, (object) string.Join<int>(",", (IEnumerable<int>) suiteIds));
      return str;
    }

    internal static string ConstructSuiteAuditForAssignedTesters(List<string> testers)
    {
      string str = (string) null;
      if (testers.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoAssignedTesters, (object) string.Join(",", (IEnumerable<string>) testers));
      return str;
    }

    internal static string ConstructSuiteAuditForLinksRemoval(
      List<int> testCaseIds,
      List<int> suiteIds)
    {
      string str = (string) null;
      if (testCaseIds.Count > 0 && suiteIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoRemovedTestCasesAndSuites, (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds), (object) string.Join<int>(",", (IEnumerable<int>) suiteIds));
      else if (testCaseIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoRemovedTestCases, (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds));
      else if (suiteIds.Count > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SuiteAuditInfoRemovedTestSuites, (object) string.Join<int>(",", (IEnumerable<int>) suiteIds));
      return str;
    }

    internal static string ConstructSuiteAuditForPlanDeleted() => ServerResources.SuiteAuditInfoTestPlanDeleted;

    internal static Tuple<TCMWorkItemBase, WorkItemUpdateData> GetUpdateDataTupleForAuditUpdate(
      TestManagementRequestContext context,
      string teamProjectName,
      UpdatedProperties props,
      string audit)
    {
      context.TraceEnter("BusinessLayer", "SuiteAuditHelper.GetUpdateDataTupleForAuditUpdate");
      TestSuiteWorkItem testSuiteWorkItem = new TestSuiteWorkItem();
      WorkItemUpdateData dataForAuditUpdate = testSuiteWorkItem.GetUpdateDataForAuditUpdate(context, teamProjectName, new IdAndRev(props.Id, props.Revision), audit);
      context.TraceLeave("BusinessLayer", "SuiteAuditHelper.GetUpdateDataTupleForAuditUpdate");
      return new Tuple<TCMWorkItemBase, WorkItemUpdateData>((TCMWorkItemBase) testSuiteWorkItem, dataForAuditUpdate);
    }

    internal static UpdatedProperties UpdateSuiteAudit(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IdAndRev parent,
      string audit,
      bool byPass = false,
      bool suppressNotifications = false)
    {
      context.TraceEnter("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAudit");
      UpdatedProperties props = new UpdatedProperties();
      props.Id = parent.Id;
      props.Revision = parent.Revision;
      SuiteAuditHelper.UpdateSuiteAudit(context, teamProjectName, projectId, ref props, audit, byPass, suppressNotifications);
      context.TraceLeave("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAudit");
      return props;
    }

    internal static void UpdateSuiteAudit(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      ref UpdatedProperties props,
      string audit,
      bool byPass = false,
      bool suppressNotifications = false)
    {
      context.TraceEnter("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAudit");
      TestSuiteWorkItem testSuiteWorkItem = new TestSuiteWorkItem();
      testSuiteWorkItem.UpdateAudit(context, teamProjectName, projectId, new IdAndRev(props.Id, props.Revision), audit, suppressNotifications, byPass);
      props = testSuiteWorkItem.GetUpdatedProperties(context as TfsTestManagementRequestContext);
      context.TraceLeave("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAudit");
    }

    internal static List<UpdatedProperties> UpdateSuiteAuditInBulk(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      List<UpdatedProperties> suites,
      string audit,
      bool isEnableBulkUpdateUsingServerOM)
    {
      context.TraceEnter("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAuditInBulk");
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tupleList = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
      List<UpdatedProperties> updatedPropertiesList = new List<UpdatedProperties>();
      foreach (UpdatedProperties suite in suites)
      {
        Tuple<TCMWorkItemBase, WorkItemUpdateData> tupleForAuditUpdate = SuiteAuditHelper.GetUpdateDataTupleForAuditUpdate(context, teamProjectName, suite, audit);
        tupleList.Add(tupleForAuditUpdate);
      }
      WorkItemUpdateContext itemUpdateContext = WorkItemUpdateContext.CreateWorkItemUpdateContext(context, teamProjectName, projectId, true);
      if (isEnableBulkUpdateUsingServerOM)
        TCMWorkItemBase.BulkUpdateWithServerOM(itemUpdateContext, tupleList, (MigrationLogger) null);
      else
        TCMWorkItemBase.BulkUpdate(itemUpdateContext, tupleList, (MigrationLogger) null);
      context.TraceLeave("BusinessLayer", "SuiteAuditHelper.UpdateSuiteAuditInBulk");
      return tupleList.Select<Tuple<TCMWorkItemBase, WorkItemUpdateData>, UpdatedProperties>((Func<Tuple<TCMWorkItemBase, WorkItemUpdateData>, UpdatedProperties>) (t => t.Item1.GetUpdatedProperties(context as TfsTestManagementRequestContext))).ToList<UpdatedProperties>();
    }
  }
}
