// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TraceabilityHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TraceabilityHelper : RestApiHelper, ITraceabilityHelper
  {
    private const int c_defaultTestsCount = 50;
    private const int c_defaultWorkItemsCount = 50;

    public TraceabilityHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestToWorkItemLinks QueryLinkedWorkItems(GuidAndString projectId, string testName)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TraceabilityHelper.QueryLinkedWorkItems", 50, true))
      {
        if (string.IsNullOrEmpty(testName))
          throw new InvalidPropertyException(nameof (testName));
        this.TestManagementRequestContext.SecurityManager.CheckViewTestResultsPermission(this.TestManagementRequestContext, projectId.String);
        return this.ExecuteAction<TestToWorkItemLinks>("TraceabilityHelper.QueryLinkedWorkItems", (Func<TestToWorkItemLinks>) (() =>
        {
          TestToWorkItemLinks linkedRequirementsForTest = this.RequestContext.GetService<ITeamFoundationTestManagementTraceabilityService>().QueryLinkedRequirementsForTest(this.TestManagementRequestContext, this.GetProjectReference(projectId.GuidId.ToString()), new TestMethod()
          {
            Name = testName
          });
          this.SecurifyLinkedRequirementsForTest(linkedRequirementsForTest, projectId.GuidId);
          return linkedRequirementsForTest;
        }), 1015070, "TestResultsInsights");
      }
    }

    private void SecurifyLinkedRequirementsForTest(
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

    public WorkItemToTestLinks QueryLinkedTests(
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks)
    {
      return new WorkItemToTestLinks();
    }

    public List<TestSummaryForWorkItem> QueryAggregatedDataByRequirement(
      GuidAndString projectId,
      List<int> workItemIds,
      TestResultsContext resultsContext)
    {
      if (!this.CheckWorkItemsMaxLimit(workItemIds))
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxLimitForWorkItemsExeceeded, (object) 50));
      if (!this.IsValidResultsContext(resultsContext))
        throw new InvalidPropertyException(nameof (resultsContext));
      return this.ExecuteAction<List<TestSummaryForWorkItem>>("TraceabilityHelper.QueryAggregatedDataByRequirementForBuild", (Func<List<TestSummaryForWorkItem>>) (() =>
      {
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
          return this.QueryAggregatedDataByRequirementWithS2SMerge(projectId, workItemIds, resultsContext);
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.GetSummaryListLocal(projectId, workItemIds, resultsContext);
      }), 1015070, "TestResultsInsights");
    }

    private List<TestSummaryForWorkItem> QueryAggregatedDataByRequirementWithS2SMerge(
      GuidAndString projectId,
      List<int> workItemIds,
      TestResultsContext resultsContext)
    {
      List<TestSummaryForWorkItem> summaryListFromCurrentService = new List<TestSummaryForWorkItem>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => summaryListFromCurrentService = this.GetSummaryListLocal(projectId, workItemIds, resultsContext)), this.RequestContext);
      List<TestSummaryForWorkItem> summaryForWorkItemList;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestSummaryByRequirement(this.RequestContext, projectId.GuidId, resultsContext, workItemIds, out summaryForWorkItemList))
      {
        TestResultsContext testResultsContext1 = summaryListFromCurrentService.Any<TestSummaryForWorkItem>() ? summaryListFromCurrentService.First<TestSummaryForWorkItem>().Summary.TestResultsContext : (TestResultsContext) null;
        TestResultsContext testResultsContext2 = summaryForWorkItemList.Any<TestSummaryForWorkItem>() ? summaryForWorkItemList.First<TestSummaryForWorkItem>().Summary.TestResultsContext : (TestResultsContext) null;
        if (this.BelongsToSameContext(testResultsContext1, testResultsContext2, resultsContext.ContextType))
          summaryListFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestSummaryForWorkItemLists(summaryListFromCurrentService, summaryForWorkItemList);
        else if (this.IsExternalServiceContextLatest(testResultsContext1, testResultsContext2, resultsContext.ContextType))
          summaryListFromCurrentService = summaryForWorkItemList;
      }
      return summaryListFromCurrentService;
    }

    private List<TestSummaryForWorkItem> GetSummaryListLocal(
      GuidAndString projectId,
      List<int> workItemIds,
      TestResultsContext resultsContext)
    {
      ITeamFoundationTestManagementTraceabilityService service = this.RequestContext.GetService<ITeamFoundationTestManagementTraceabilityService>();
      List<TestSummaryForWorkItem> summaryListLocal = new List<TestSummaryForWorkItem>();
      if (resultsContext.ContextType == TestResultsContextType.Build)
        summaryListLocal = service.QueryAggregatedDataByRequirementForBuild(this.TestManagementRequestContext, projectId, workItemIds, new BuildConfiguration()
        {
          BuildDefinitionId = resultsContext.Build.DefinitionId,
          BranchName = resultsContext.Build.BranchName,
          RepositoryId = resultsContext.Build.RepositoryId
        });
      else if (resultsContext.ContextType == TestResultsContextType.Release)
        summaryListLocal = service.QueryAggregatedDataByRequirementForRelease(this.TestManagementRequestContext, projectId, workItemIds, new ReleaseReference()
        {
          ReleaseDefId = resultsContext.Release.DefinitionId,
          ReleaseEnvDefId = resultsContext.Release.EnvironmentDefinitionId
        });
      return summaryListLocal;
    }

    private bool BelongsToSameContext(
      TestResultsContext contextFromCurrentService,
      TestResultsContext contextFromExternalService,
      TestResultsContextType contextType)
    {
      if (contextFromCurrentService == null || contextFromExternalService == null || contextFromCurrentService.ContextType == (TestResultsContextType) 0 || contextFromExternalService.ContextType == (TestResultsContextType) 0)
        return false;
      switch (contextType)
      {
        case TestResultsContextType.Build:
          return contextFromCurrentService.Build.Id == contextFromExternalService.Build.Id;
        case TestResultsContextType.Release:
          return contextFromCurrentService.Release.Id == contextFromExternalService.Release.Id && contextFromCurrentService.Release.EnvironmentId == contextFromExternalService.Release.EnvironmentId;
        default:
          return false;
      }
    }

    private bool IsExternalServiceContextLatest(
      TestResultsContext contextFromCurrentService,
      TestResultsContext contextFromExternalService,
      TestResultsContextType contextType)
    {
      if (contextFromExternalService == null || contextFromExternalService.ContextType == (TestResultsContextType) 0)
        return false;
      if (contextFromCurrentService == null || contextFromCurrentService.ContextType == (TestResultsContextType) 0)
        return true;
      if (contextType == TestResultsContextType.Build)
        return contextFromCurrentService.Build.Id < contextFromExternalService.Build.Id;
      return contextType == TestResultsContextType.Release && contextFromCurrentService.Release.Id < contextFromExternalService.Release.Id;
    }

    public WorkItemToTestLinks AddWorkItemLink(
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks)
    {
      int workItemId = 0;
      if (workItemToTestLinks == null)
        throw new InvalidPropertyException(nameof (workItemToTestLinks));
      if (!int.TryParse(workItemToTestLinks.WorkItem.Id, out workItemId) || workItemId <= 0)
        throw new InvalidPropertyException("workItemToTestLinks.WorkItem");
      if (workItemToTestLinks.Tests == null || !workItemToTestLinks.Tests.Any<TestMethod>())
        throw new InvalidPropertyException("workItemToTestLinks.Tests");
      if (workItemToTestLinks.Tests.Count<TestMethod>() > 50)
        throw new InvalidPropertyException("workItemToTestLinks.Tests", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxLimitForLinkedTestsExeceeded, (object) 50));
      return this.ExecuteAction<WorkItemToTestLinks>("TraceabilityHelper.AddWorkItemLink", (Func<WorkItemToTestLinks>) (() => this.RequestContext.GetService<ITeamFoundationTestManagementTraceabilityService>().AddRequirementToTestLinks(this.TestManagementRequestContext, projectId, workItemId, workItemToTestLinks.Tests)), 1015070, "TestResultsInsights");
    }

    public bool DeleteWorkItemLink(GuidAndString projectId, string testName, int workItemId)
    {
      if (string.IsNullOrEmpty(testName))
        throw new InvalidPropertyException(nameof (testName));
      if (workItemId <= 0)
        throw new InvalidPropertyException(nameof (workItemId));
      return this.ExecuteAction<bool>("TraceabilityHelper.DeleteWorkItemLink", (Func<bool>) (() =>
      {
        this.RequestContext.GetService<ITeamFoundationTestManagementTraceabilityService>().DeleteRequirementToTestLink(this.TestManagementRequestContext, projectId, workItemId, new TestMethod()
        {
          Name = testName
        });
        return true;
      }), 1015070, "TestResultsInsights");
    }

    private bool CheckWorkItemsMaxLimit(List<int> workItemIds) => workItemIds == null || !workItemIds.Any<int>() || workItemIds.Count<int>() <= 50;

    private bool IsValidResultsContext(TestResultsContext resultsContext)
    {
      if (resultsContext != null)
      {
        switch (resultsContext.ContextType)
        {
          case TestResultsContextType.Build:
            return this.IsValidBuildContext(resultsContext.Build);
          case TestResultsContextType.Release:
            return this.IsValidReleaseContext(resultsContext.Release);
        }
      }
      return false;
    }

    private bool IsValidBuildContext(BuildReference buildRef) => buildRef != null && buildRef.DefinitionId > 0 && !string.IsNullOrEmpty(buildRef.BranchName);

    private bool IsValidReleaseContext(Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseRef) => releaseRef != null && releaseRef.DefinitionId > 0;
  }
}
