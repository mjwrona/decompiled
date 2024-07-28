// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLinkedWorkItemsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestLinkedWorkItemsHelper : RestApiHelper
  {
    private ITraceabilityHelper _traceabilitysHelper;
    private ITestReportsHelper _testReportsHelper;
    protected ResultsHelper m_resultsHelper;

    internal TestLinkedWorkItemsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestToWorkItemLinks QueryLinkedWorkItems(GuidAndString projectId, string testName)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.QueryLinkedWorkItems");
      return this.ExecuteAction<TestToWorkItemLinks>("TestLinkedWorkItemsHelper.QueryLinkedWorkItems", (Func<TestToWorkItemLinks>) (() =>
      {
        TestToWorkItemLinks links = (TestToWorkItemLinks) null;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => links = this.TraceabilityHelper.QueryLinkedWorkItems(projectId, testName)), this.RequestContext);
          TestToWorkItemLinks links1;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryLinkedWorkItems(this.RequestContext, projectId, testName, out links1))
            links = this.TestManagementRequestContext.MergeDataHelper.MergeTestToWorkItemLinks(links, links1);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          links = this.TraceabilityHelper.QueryLinkedWorkItems(projectId, testName);
        }
        this.SecurifyLinkedRequirementsForTest(links, projectId.GuidId);
        return links;
      }), 1015076, "TestManagement");
    }

    public WorkItemToTestLinks AddWorkItemLink(
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.AddWorkItemLink");
      return this.ExecuteAction<WorkItemToTestLinks>("TestLinkedWorkItemsHelper.AddWorkItemLink", (Func<WorkItemToTestLinks>) (() =>
      {
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          WorkItemToTestLinks links;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryAddWorkItemLink(this.RequestContext, projectId, workItemToTestLinks, out links))
            return links;
        }
        else if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.TraceabilityHelper.AddWorkItemLink(projectId, workItemToTestLinks);
      }), 1015076, "TestManagement");
    }

    public bool DeleteWorkItemLink(GuidAndString projectId, string testName, int workItemId)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.DeleteWorkItemLink");
      return this.ExecuteAction<bool>("TestLinkedWorkItemsHelper.DeleteWorkItemLink", (Func<bool>) (() =>
      {
        bool deletedInCurrentService = false;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          bool flag3 = this.TestManagementRequestContext.TcmServiceHelper.TryDeleteWorkItemLink(this.RequestContext, projectId, testName, workItemId);
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => deletedInCurrentService = this.TraceabilityHelper.DeleteWorkItemLink(projectId, testName, workItemId)), this.RequestContext);
          return deletedInCurrentService | flag3;
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.TraceabilityHelper.DeleteWorkItemLink(projectId, testName, workItemId);
      }), 1015076, "TestManagement");
    }

    public List<WorkItemReference> GetAssociatedBugsForResult(
      Guid projectId,
      int runId,
      int testCaseResultId)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.GetAssociatedBugsForResult");
      return this.ExecuteAction<List<WorkItemReference>>("TestLinkedWorkItemsHelper.GetAssociatedBugsForResult", (Func<List<WorkItemReference>>) (() =>
      {
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        List<WorkItemReference> references;
        if (!flag1 && !flag2)
        {
          if (!this.TestManagementRequestContext.TcmServiceHelper.TryGetAssociatedBugsForResult(this.RequestContext, projectId, runId, testCaseResultId, out references))
            references = this.ResultsHelper.GetAssociatedWorkItemsForResult(projectId.ToString(), runId, testCaseResultId);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          references = this.ResultsHelper.GetAssociatedWorkItemsForResult(projectId.ToString(), runId, testCaseResultId);
        }
        this.SecureWorkItemReferences(projectId, references);
        return references;
      }), 1015077, "TestManagement");
    }

    public List<WorkItemReference> GetAssociatedWorkItemsForResult(
      GuidAndString projectId,
      int runId,
      int testCaseResultId)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.GetAssociatedWorkItemsForResult");
      return this.ExecuteAction<List<WorkItemReference>>("TestLinkedWorkItemsHelper.GetAssociatedWorkItemsForResult", (Func<List<WorkItemReference>>) (() =>
      {
        List<WorkItemReference> references = (List<WorkItemReference>) null;
        bool getAllWorkItemTypes = true;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => references = this.ResultsHelper.GetAssociatedWorkItemsForResult(projectId.GuidId.ToString(), runId, testCaseResultId, getAllWorkItemTypes)), this.RequestContext);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          references = this.ResultsHelper.GetAssociatedWorkItemsForResult(projectId.GuidId.ToString(), runId, testCaseResultId, getAllWorkItemTypes);
        }
        this.SecureWorkItemReferences(projectId.GuidId, references);
        return references;
      }), 1015076, "TestManagement");
    }

    public List<WorkItemReference> QueryTestResultWorkItems(
      GuidAndString projectId,
      string workItemCategory,
      string automatedTestName,
      int testCaseId,
      DateTime? maxCompleteDate,
      int days,
      int workItemCount)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestLinkedWorkItemsHelper.QueryTestResultWorkItems");
      return this.ExecuteAction<List<WorkItemReference>>("TestLinkedWorkItemsHelper.QueryTestResultWorkItems", (Func<List<WorkItemReference>>) (() =>
      {
        List<WorkItemReference> references = (List<WorkItemReference>) null;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => references = this.TestReportsHelper.QueryTestResultWorkItems(projectId, workItemCategory, automatedTestName, testCaseId, maxCompleteDate, days, workItemCount)), this.RequestContext);
          List<WorkItemReference> references1;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestResultWorkItems(this.RequestContext, projectId, workItemCategory, automatedTestName, testCaseId, maxCompleteDate, days, workItemCount, out references1))
            references = this.TestManagementRequestContext.MergeDataHelper.MergeWorkItemReferences(references, references1);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          references = this.TestReportsHelper.QueryTestResultWorkItems(projectId, workItemCategory, automatedTestName, testCaseId, maxCompleteDate, days, workItemCount);
        }
        this.SecureWorkItemReferences(projectId.GuidId, references);
        return references;
      }), 1015076, "TestManagement");
    }

    public void SecurifyLinkedRequirementsForTest(
      TestToWorkItemLinks linkedRequirementsForTest,
      Guid projectGuid)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectGuid.ToString()
        }
      };
      linkedRequirementsForTest.InitializeSecureObject(securedObject);
    }

    public void SecureWorkItemReferences(Guid projectId, List<WorkItemReference> references) => references?.ForEach((Action<WorkItemReference>) (r => r.InitializeSecureObject((ISecuredObject) new TeamProjectReference()
    {
      Id = projectId
    })));

    private ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }

    internal ResultsHelper ResultsHelper
    {
      get => this.m_resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this.m_resultsHelper = value;
    }

    internal ITraceabilityHelper TraceabilityHelper
    {
      get => this._traceabilitysHelper ?? (ITraceabilityHelper) new Microsoft.TeamFoundation.TestManagement.Server.TraceabilityHelper(this.TestManagementRequestContext);
      set => this._traceabilitysHelper = value;
    }
  }
}
