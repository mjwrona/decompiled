// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestReportsHelper : RestApiHelper, ITestReportsHelper
  {
    private TestMethodNameSanitizer m_testMethodNameSanitizer;

    public TestReportsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
      this.m_testMethodNameSanitizer = new TestMethodNameSanitizer();
    }

    public TestResultSummary QueryTestReportForBuild(
      GuidAndString projectId,
      BuildReference build,
      string sourceWorkflow,
      BuildReference buildToCompare,
      bool returnSummary,
      bool returnFailureDetails)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestReportForBuild", 50, true))
      {
        ArgumentUtility.CheckForNull<BuildReference>(build, nameof (build), "Test Results");
        if (build.Id == 0 && string.IsNullOrEmpty(build.Uri))
          throw new ArgumentNullException(nameof (build)).Expected("Test Results");
        if (string.IsNullOrEmpty(sourceWorkflow))
          sourceWorkflow = SourceWorkflow.ContinuousIntegration;
        IVssRequestContext requestContext = this.RequestContext;
        string str1 = projectId.ToString();
        string str2 = string.Join(",", (object) build.Id, !string.IsNullOrEmpty(build.Number) ? (object) build.Number : (object) string.Empty, !string.IsNullOrEmpty(build.Uri) ? (object) build.Uri : (object) string.Empty);
        string str3;
        if (buildToCompare != null)
          str3 = string.Join(",", (object) buildToCompare.Id, !string.IsNullOrEmpty(buildToCompare.Number) ? (object) buildToCompare.Number : (object) string.Empty, !string.IsNullOrEmpty(buildToCompare.Uri) ? (object) buildToCompare.Uri : (object) string.Empty);
        else
          str3 = string.Empty;
        requestContext.Trace(1015041, TraceLevel.Info, "TestManagement", "RestLayer", "TestReportsHelper.QueryTestReportForBuild projectId = {0}, build={1}, buildToCompare={2}", (object) str1, (object) str2, (object) str3);
        string str4 = string.IsNullOrEmpty(build.Uri) ? TestManagementServiceUtility.GetArtiFactUri("Build", "Build", build.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : build.Uri;
        BuildConfiguration currentBuildConfiguration = new BuildConfiguration()
        {
          BuildId = build.Id,
          BuildUri = str4
        };
        BuildConfiguration prevBuildConfiguration;
        return this.ExecuteAction<TestResultSummary>("TestReportsHelper.QueryTestReportForBuild", (Func<TestResultSummary>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(projectId.GuidId.ToString());
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          TestResultSummary summary;
          if (!flag1 && !flag2)
          {
            summary = this.QueryTestReportForBuildWithS2SMerge(projectId, build, sourceWorkflow, buildToCompare, returnSummary, returnFailureDetails, currentBuildConfiguration, prevBuildConfiguration);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            summary = this.RequestContext.GetService<ITeamFoundationTestReportService>().QueryTestSummaryAndInsightsForBuild(this.RequestContext, projectId, sourceWorkflow, currentBuildConfiguration, this.GetBuildConfigurationFromReference(this.RequestContext, projectId, buildToCompare) ?? throw new InvalidPropertyException("buildId", ServerResources.InvalidBuildIdSpecified), returnSummary, returnFailureDetails);
            if (summary != null)
              summary.TeamProject = projectReference;
          }
          this.SecureTestResultSummary(summary);
          return summary;
        }), 1015066, "TestManagement");
      }
    }

    public TestResultSummary QueryTestReportForPipeline(
      GuidAndString projectId,
      bool returnFailureDetails)
    {
      TestResultSummary summary = new TestResultSummary();
      summary.TeamProject = this.GetProjectReference(projectId.GuidId.ToString());
      this.SecureTestResultSummary(summary);
      return summary;
    }

    public TestResultSummary QueryTestReportForRelease(
      GuidAndString projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference release,
      string sourceWorkflow,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare,
      bool returnSummary,
      bool returnFailureDetails)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestReportForRelease", 50, true))
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference>(release, nameof (release), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) release.Id, "release.Id", "Test Results");
        if (string.IsNullOrEmpty(sourceWorkflow))
          sourceWorkflow = SourceWorkflow.ContinuousDelivery;
        IVssRequestContext requestContext = this.RequestContext;
        string str1 = projectId.ToString();
        string str2 = string.Join(",", (object) release.Id, (object) release.EnvironmentId);
        string str3;
        if (releaseToCompare != null)
          str3 = string.Join(",", (object) releaseToCompare.Id, (object) releaseToCompare.EnvironmentId);
        else
          str3 = string.Empty;
        requestContext.Trace(1015041, TraceLevel.Info, "TestManagement", "RestLayer", "TestReportsHelper.QueryTestReportForRelease projectId = {0}, release={1}, releaseToCompare={2}", (object) str1, (object) str2, (object) str3);
        ReleaseReference currentRelease = new ReleaseReference()
        {
          ReleaseId = release.Id,
          ReleaseEnvId = release.EnvironmentId
        };
        ReleaseReference releaseReference;
        if (releaseToCompare == null)
        {
          releaseReference = (ReleaseReference) null;
        }
        else
        {
          releaseReference = new ReleaseReference();
          releaseReference.ReleaseId = releaseToCompare.Id;
          releaseReference.ReleaseEnvId = releaseToCompare.EnvironmentId;
        }
        ReleaseReference prevRelease;
        return this.ExecuteAction<TestResultSummary>("TestReportsHelper.QueryTestReportForRelease", (Func<TestResultSummary>) (() =>
        {
          this.GetProjectReference(projectId.GuidId.ToString());
          ITeamFoundationTestReportService service = this.RequestContext.GetService<ITeamFoundationTestReportService>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          TestResultSummary summary;
          if (!flag1 && !flag2)
          {
            summary = this.QueryTestReportForReleaseWithS2SMerge(projectId, sourceWorkflow, releaseToCompare, returnSummary, returnFailureDetails, prevRelease, currentRelease);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            summary = service.QueryTestSummaryAndInsightsForRelease(this.RequestContext, projectId, sourceWorkflow, currentRelease, releaseReference ?? throw new InvalidPropertyException(nameof (release), ServerResources.InvalidReleaseSpecified), returnSummary, returnFailureDetails);
            if (summary != null)
              summary.TeamProject = this.GetProjectReference(projectId.GuidId.ToString());
          }
          this.SecureTestResultSummary(summary);
          return summary;
        }), 1015066, "TestManagement");
      }
    }

    public List<TestResultSummary> QueryTestSummaryForReleases(
      GuidAndString projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) releases, nameof (releases), "Test Results");
      return this.ExecuteAction<List<TestResultSummary>>("TestReportsHelper.QueryTestSummaryForReleases", (Func<List<TestResultSummary>>) (() =>
      {
        ITeamFoundationTestReportService reportService = this.RequestContext.GetService<ITeamFoundationTestReportService>();
        List<TestResultSummary> summaryByReleasesFromCurrentService = new List<TestResultSummary>();
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => summaryByReleasesFromCurrentService = reportService.QueryTestSummaryForReleases(this.RequestContext, projectId, releases)), this.RequestContext);
          List<TestResultSummary> testSummaryForReleases;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestResultsSummaryForReleases(this.RequestContext, projectId.GuidId, releases, out testSummaryForReleases))
            summaryByReleasesFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultSummaryLists(summaryByReleasesFromCurrentService, testSummaryForReleases);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          summaryByReleasesFromCurrentService = reportService.QueryTestSummaryForReleases(this.RequestContext, projectId, releases);
        }
        // ISSUE: explicit non-virtual call
        summaryByReleasesFromCurrentService?.ForEach((Action<TestResultSummary>) (summary => __nonvirtual (this.SecureTestResultSummary(summary))));
        return summaryByReleasesFromCurrentService;
      }), 1015066, "TestManagement");
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
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestResultWorkItems", 50, true))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(workItemCategory, nameof (workItemCategory), "Test Results");
        if (!string.Equals(workItemCategory, WitCategoryRefName.Bug, StringComparison.OrdinalIgnoreCase) && !string.Equals(workItemCategory, WitCategoryRefName.AllWorkItemsCategory, StringComparison.OrdinalIgnoreCase))
          throw new InvalidPropertyException(nameof (workItemCategory), ServerResources.InvalidPropertyMessage);
        if (string.IsNullOrEmpty(automatedTestName) && testCaseId == 0)
          throw new ArgumentNullException(nameof (automatedTestName)).Expected("Test Results");
        if (days > TestManagementServiceUtility.GetMaxDaysForTestResultsWorkItems(this.RequestContext) || days < 0)
          throw new InvalidPropertyException(nameof (days), ServerResources.QueryParameterOutOfRange);
        if (workItemCount < 0 || workItemCount > 100)
          throw new InvalidPropertyException(nameof (workItemCount), ServerResources.QueryParameterOutOfRange);
        if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodName"))
          automatedTestName = this.m_testMethodNameSanitizer.Sanitize(this.m_testManagementRequestContext, automatedTestName);
        this.TestManagementRequestContext.SecurityManager.CheckViewTestResultsPermission(this.TestManagementRequestContext, projectId.String);
        DateTime date = DateTime.MinValue;
        if (maxCompleteDate.HasValue)
        {
          date = DateTime.SpecifyKind(maxCompleteDate.Value, DateTimeKind.Utc);
          if (DateTime.Compare(date, this.MinSqlTime) < 0 || DateTime.Compare(date, this.MaxSqlTime) > 0)
            throw new InvalidPropertyException(nameof (maxCompleteDate), ServerResources.QueryParameterOutOfRange);
        }
        return this.ExecuteAction<List<WorkItemReference>>("TestReportsHelper.QueryTestResultWorkItems", (Func<List<WorkItemReference>>) (() =>
        {
          TeamProjectReference projectReference1 = this.GetProjectReference(projectId.GuidId.ToString());
          List<WorkItemReference> workItemReferenceList = new List<WorkItemReference>();
          List<WorkItemReference> workItems;
          if (this.RequestContext.IsFeatureEnabled("TestManagement.Server.WorkItemTestLinks"))
          {
            ITestManagementLinkedWorkItemService linkedWorkItemService = this.TestManagementLinkedWorkItemService;
            TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
            ProjectInfo projectReference2 = new ProjectInfo();
            projectReference2.Id = projectReference1.Id;
            projectReference2.Name = projectReference1.Name;
            LinkedWorkItemsQuery workItemQuery = new LinkedWorkItemsQuery();
            workItemQuery.WorkItemCategory = workItemCategory;
            List<int> intList;
            if (testCaseId <= 0)
            {
              intList = (List<int>) null;
            }
            else
            {
              intList = new List<int>();
              intList.Add(testCaseId);
            }
            workItemQuery.TestCaseIds = intList;
            List<string> stringList;
            if (string.IsNullOrEmpty(automatedTestName))
            {
              stringList = (List<string>) null;
            }
            else
            {
              stringList = new List<string>();
              stringList.Add(automatedTestName);
            }
            workItemQuery.AutomatedTestNames = stringList;
            List<LinkedWorkItemsQueryResult> workItemsByQuery = linkedWorkItemService.GetLinkedWorkItemsByQuery(managementRequestContext, projectReference2, workItemQuery);
            workItems = workItemsByQuery.Any<LinkedWorkItemsQueryResult>() ? workItemsByQuery.SelectMany<LinkedWorkItemsQueryResult, WorkItemReference>((Func<LinkedWorkItemsQueryResult, IEnumerable<WorkItemReference>>) (r => (IEnumerable<WorkItemReference>) r.WorkItems)).ToList<WorkItemReference>() : new List<WorkItemReference>();
          }
          else
          {
            List<TestCaseResult> results = this.TestManagementResultService.QueryTestResultHistory(this.TestManagementRequestContext, projectReference1, automatedTestName, testCaseId, date, days);
            workItems = this.TestManagementLinkedWorkItemService.BatchCreateWorkItemsRecordsForResults(this.TestManagementRequestContext, projectReference1.Id, workItemCategory, results, workItemCount);
          }
          this.SecurifyWorkItemsAssociatedWithTestResults(workItems, projectId.GuidId);
          return workItems;
        }), 1015066, "TestManagement");
      }
    }

    public TestResultHistory QueryTestCaseResultHistory(
      GuidAndString projectId,
      ResultsFilter filter)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestCaseResultHistory", 50, true))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(filter.AutomatedTestName, "AutomatedTestName", "Test Results");
        if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodName"))
          filter.AutomatedTestName = this.m_testMethodNameSanitizer.Sanitize(this.m_testManagementRequestContext, filter.AutomatedTestName);
        if (filter.TestResultsContext != null)
        {
          if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
            ArgumentUtility.CheckForNull<BuildReference>(filter.TestResultsContext.Build, "Build", "Test Results");
          if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
            ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference>(filter.TestResultsContext.Release, "Release", "Test Results");
        }
        if (filter.TrendDays > 7 || filter.TrendDays < 0)
          throw new InvalidPropertyException("trendDays", ServerResources.QueryParameterOutOfRange);
        DateTime maxCompleteDate = DateTime.MinValue;
        DateTime? maxCompleteDate1 = filter.MaxCompleteDate;
        if (maxCompleteDate1.HasValue)
        {
          maxCompleteDate1 = filter.MaxCompleteDate;
          maxCompleteDate = DateTime.SpecifyKind(maxCompleteDate1.Value, DateTimeKind.Utc);
          if (DateTime.Compare(maxCompleteDate, this.MinSqlTime) < 0 || DateTime.Compare(maxCompleteDate, this.MaxSqlTime) > 0)
            throw new InvalidPropertyException("maxCompleteDate", ServerResources.QueryParameterOutOfRange);
        }
        return this.ExecuteAction<TestResultHistory>("TestReportsHelper.QueryTestCaseResultHistory", (Func<TestResultHistory>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(projectId.GuidId.ToString());
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          TestResultHistory resultHistory;
          if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestCaseResultHistory(this.TestManagementRequestContext.RequestContext, projectReference.Id, filter, out resultHistory))
            return this.TestManagementRequestContext.MergeDataHelper.MergeTestResultHistory(resultHistory, this.QueryTestCaseResultHistory(projectReference, maxCompleteDate, filter));
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          return this.QueryTestCaseResultHistory(projectReference, maxCompleteDate, filter);
        }), 1015066, "TestManagement");
      }
    }

    public List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestResultTrendForBuild", 50, true))
        return this.ExecuteAction<List<AggregatedDataForResultTrend>>("TestReportsHelper.QueryTestResultTrendForBuild", (Func<List<AggregatedDataForResultTrend>>) (() =>
        {
          List<AggregatedDataForResultTrend> dataForResultTrendList = new List<AggregatedDataForResultTrend>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          List<AggregatedDataForResultTrend> aggrResults;
          if (!flag1 && !flag2)
          {
            aggrResults = this.QueryTestResultTrendForBuildWithS2SMerge(projectId, filter);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            aggrResults = this.GetQueryResultTrendForBuildLocal(projectId, filter);
          }
          this.SecureTestResultTrend(aggrResults, projectId.GuidId);
          return aggrResults;
        }), 1015066, "TestManagement");
    }

    public List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "TestReportsHelper.QueryTestResultTrendForRelease", 50, true))
        return this.ExecuteAction<List<AggregatedDataForResultTrend>>("TestReportsHelper.QueryTestResultTrendForRelease", (Func<List<AggregatedDataForResultTrend>>) (() =>
        {
          List<AggregatedDataForResultTrend> dataForResultTrendList = new List<AggregatedDataForResultTrend>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          List<AggregatedDataForResultTrend> aggrResults;
          if (!flag1 && !flag2)
          {
            aggrResults = this.QueryTestResultTrendForReleaseWithS2SMerge(projectId, filter);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            aggrResults = this.GetQueryResultTrendForReleaseLocal(projectId, filter);
          }
          this.SecureTestResultTrend(aggrResults, projectId.GuidId);
          return aggrResults;
        }), 1015066, "TestManagement");
    }

    public void SecureTestResultSummary(TestResultSummary summary)
    {
      summary?.AggregatedResultsAnalysis?.InitializeSecureObject((ISecuredObject) summary);
      summary?.TestFailures?.InitializeSecureObject((ISecuredObject) summary);
      summary?.TestResultsContext?.InitializeSecureObject((ISecuredObject) summary);
    }

    public void SecureTestResultTrend(
      List<AggregatedDataForResultTrend> aggrResults,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new TeamProjectReference()
      {
        Id = projectId
      };
      aggrResults?.ForEach((Action<AggregatedDataForResultTrend>) (aggregatedResult => aggregatedResult.InitializeSecureObject(securedObject)));
    }

    private TestResultSummary QueryTestReportForBuildWithS2SMerge(
      GuidAndString projectId,
      BuildReference build,
      string sourceWorkflow,
      BuildReference buildToCompare,
      bool returnSummary,
      bool returnFailureDetails,
      BuildConfiguration currentBuildConfiguration,
      BuildConfiguration prevBuildConfiguration)
    {
      TeamProjectReference projectReference = this.GetProjectReference(projectId.GuidId.ToString());
      TestResultSummary testReportFromCurrentService = (TestResultSummary) null;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        testReportFromCurrentService = this.RequestContext.GetService<ITeamFoundationTestReportService>().QueryTestSummaryAndInsightsForBuild(this.RequestContext, projectId, sourceWorkflow, currentBuildConfiguration, prevBuildConfiguration, returnSummary, returnFailureDetails);
        if (testReportFromCurrentService == null)
          return;
        testReportFromCurrentService.TeamProject = projectReference;
      }), this.RequestContext);
      TestResultSummary testReport;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestResultsReportForBuild(this.RequestContext, projectReference.Id, currentBuildConfiguration.BuildId, sourceWorkflow, returnFailureDetails, buildToCompare, out testReport))
        testReportFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultSummary(testReportFromCurrentService, testReport);
      return testReportFromCurrentService;
    }

    private TestResultSummary QueryTestReportForReleaseWithS2SMerge(
      GuidAndString projectId,
      string sourceWorkflow,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare,
      bool returnSummary,
      bool returnFailureDetails,
      ReleaseReference prevRelease,
      ReleaseReference currentRelease)
    {
      TeamProjectReference projectReference = this.GetProjectReference(projectId.GuidId.ToString());
      TestResultSummary testReportFromCurrentService = (TestResultSummary) null;
      ITeamFoundationTestReportService reportService = this.RequestContext.GetService<ITeamFoundationTestReportService>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        testReportFromCurrentService = reportService.QueryTestSummaryAndInsightsForRelease(this.RequestContext, projectId, sourceWorkflow, currentRelease, prevRelease, returnSummary, returnFailureDetails);
        if (testReportFromCurrentService == null)
          return;
        testReportFromCurrentService.TeamProject = this.GetProjectReference(projectId.GuidId.ToString());
      }), this.RequestContext);
      TestResultSummary testReport;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestResultsReportForRelease(this.RequestContext, projectReference.Id, currentRelease.ReleaseId, currentRelease.ReleaseEnvId, sourceWorkflow, returnFailureDetails, releaseToCompare, out testReport))
        testReportFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultSummary(testReportFromCurrentService, testReport);
      return testReportFromCurrentService;
    }

    private void SecurifyWorkItemsAssociatedWithTestResults(
      List<WorkItemReference> workItems,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      foreach (TestManagementBaseSecuredObject workItem in workItems)
        workItem.InitializeSecureObject(securedObject);
    }

    private List<AggregatedDataForResultTrend> QueryTestResultTrendForBuildWithS2SMerge(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> resultTrend;
      this.TestManagementRequestContext.TcmServiceHelper.TryQueryResultTrendForBuild(this.RequestContext, projectId.GuidId, filter, out resultTrend);
      List<AggregatedDataForResultTrend> resultTrendFromCurrentService = new List<AggregatedDataForResultTrend>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultTrendFromCurrentService = this.GetQueryResultTrendForBuildLocal(projectId, filter)), this.RequestContext);
      if (resultTrend != null)
        resultTrendFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultTrend(resultTrendFromCurrentService, resultTrend, TestResultsContextType.Build);
      return resultTrendFromCurrentService;
    }

    private List<AggregatedDataForResultTrend> GetQueryResultTrendForBuildLocal(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> dataForResultTrendList = new List<AggregatedDataForResultTrend>();
      this.ValidateTrendFilter(filter);
      if (string.IsNullOrEmpty(filter.PublishContext))
        filter.PublishContext = SourceWorkflow.ContinuousIntegration;
      return this.RequestContext.GetService<ITeamFoundationTestReportService>().QueryTestResultTrendForBuild(this.RequestContext, projectId, filter);
    }

    private List<AggregatedDataForResultTrend> QueryTestResultTrendForReleaseWithS2SMerge(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> resultTrend;
      this.TestManagementRequestContext.TcmServiceHelper.TryQueryResultTrendForRelease(this.RequestContext, projectId.GuidId, filter, out resultTrend);
      List<AggregatedDataForResultTrend> resultTrendFromCurrentService = new List<AggregatedDataForResultTrend>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultTrendFromCurrentService = this.GetQueryResultTrendForReleaseLocal(projectId, filter)), this.RequestContext);
      if (resultTrend != null)
        resultTrendFromCurrentService = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultTrend(resultTrendFromCurrentService, resultTrend, TestResultsContextType.Release);
      return resultTrendFromCurrentService;
    }

    private List<AggregatedDataForResultTrend> GetQueryResultTrendForReleaseLocal(
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> dataForResultTrendList = new List<AggregatedDataForResultTrend>();
      this.ValidateTrendFilter(filter);
      if (string.IsNullOrEmpty(filter.PublishContext))
        filter.PublishContext = SourceWorkflow.ContinuousDelivery;
      return this.RequestContext.GetService<ITeamFoundationTestReportService>().QueryTestResultTrendForRelease(this.RequestContext, projectId, filter);
    }

    private TestResultHistory QueryTestCaseResultHistory(
      TeamProjectReference projectReference,
      DateTime maxCompleteDate,
      ResultsFilter filter)
    {
      if (filter.TrendDays == 0)
        filter.TrendDays = 7;
      filter.MaxCompleteDate = new DateTime?(maxCompleteDate);
      return this.TestManagementResultService.QueryTestCaseResultHistory(this.TestManagementRequestContext, projectReference, filter);
    }

    private void ValidateTrendFilter(TestResultTrendFilter filter)
    {
      IVssRegistryService service = this.RequestContext.GetService<IVssRegistryService>();
      ArgumentUtility.CheckForNull<TestResultTrendFilter>(filter, nameof (filter), "Test Results");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) filter.DefinitionIds, "DefinitionIds", "Test Results");
      if (filter.BuildCount < 0)
        throw new InvalidPropertyException("buildCount", ServerResources.QueryParameterOutOfRange);
      if (filter.BuildCount == 0)
        filter.BuildCount = 10;
      else if (filter.BuildCount > 30)
      {
        int num = service.GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxBuildCountForResultTrend", 30);
        if (filter.BuildCount > num)
          throw new InvalidPropertyException("buildCount", ServerResources.QueryParameterOutOfRange);
      }
      if (filter.TrendDays < 0)
        throw new InvalidPropertyException("trendDays", ServerResources.QueryParameterOutOfRange);
      if (filter.TrendDays == 0)
        filter.TrendDays = 30;
      else if (filter.TrendDays > 30)
      {
        int num = service.GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxHistoryDaysForResultTrendByBuild", 30);
        if (filter.TrendDays > num)
          throw new InvalidPropertyException("trendDays", ServerResources.QueryParameterOutOfRange);
      }
      DateTime t1 = DateTime.MinValue;
      if (filter.MaxCompleteDate.HasValue)
      {
        t1 = DateTime.SpecifyKind(filter.MaxCompleteDate.Value, DateTimeKind.Utc);
        if (DateTime.Compare(t1, this.MinSqlTime) < 0 || DateTime.Compare(t1, this.MaxSqlTime) > 0)
          throw new InvalidPropertyException("maxCompleteDate", ServerResources.QueryParameterOutOfRange);
      }
      filter.MaxCompleteDate = new DateTime?(t1);
    }

    private BuildConfiguration GetBuildConfigurationFromReference(
      IVssRequestContext context,
      GuidAndString projectId,
      BuildReference build)
    {
      using (PerfManager.Measure(context, "RestLayer", "TestReportsHelper.GetBuildConfigurationFromReference"))
      {
        if (build != null)
        {
          if (build.Id > 0)
            return this.BuildServiceHelper.QueryBuildConfigurationById(context, projectId.GuidId, build.Id);
          if (!string.IsNullOrWhiteSpace(build.Number))
            return this.BuildServiceHelper.QueryBuildConfigurationByBuildNumber(context, projectId.GuidId, build.Number);
          if (!string.IsNullOrWhiteSpace(build.Uri))
            return this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(context, projectId.GuidId, build.Uri);
        }
        return (BuildConfiguration) null;
      }
    }
  }
}
