// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultTcmServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DefaultTcmServiceHelper : ITcmServiceHelper
  {
    private IVssRequestContext m_requestContext;

    public DefaultTcmServiceHelper(IVssRequestContext context) => this.m_requestContext = context;

    public bool isOnPremiseDeployment() => this.m_requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    public bool TryQueryLinkedWorkItems(
      IVssRequestContext context,
      GuidAndString projectId,
      string testName,
      out TestToWorkItemLinks links)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryLinkedWorkItems), "Tcm")))
      {
        TestToWorkItemLinks testToWorkItemLinks = (TestToWorkItemLinks) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testToWorkItemLinks = this.TestManagementClient.QueryTestMethodLinkedWorkItemsAsync(projectId.GuidId, testName)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        links = testToWorkItemLinks;
        return num != 0;
      }
    }

    public bool TryDeleteWorkItemLink(
      IVssRequestContext context,
      GuidAndString projectId,
      string testName,
      int workItemId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteWorkItemLink), "Tcm")))
        return this.InvokeAction((Func<bool>) (() => !context.IsImpersonating && this.TestManagementClient != null && this.TestManagementClient.DeleteTestMethodToWorkItemLinkAsync(projectId.GuidId, testName, workItemId).Result), context, true);
    }

    public bool TryAddWorkItemLink(
      IVssRequestContext context,
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks,
      out WorkItemToTestLinks links)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryAddWorkItemLink), "Tcm")))
      {
        WorkItemToTestLinks witToTestLinks = (WorkItemToTestLinks) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          WorkItemToTestLinks workItemToTestLinks1 = workItemToTestLinks;
          if ((workItemToTestLinks1 != null ? (workItemToTestLinks1.ExecutedIn != Service.Tcm ? 1 : 0) : 1) == 0 || context.IsImpersonating || this.TestManagementClient == null)
            return false;
          witToTestLinks = this.TestManagementClient.AddWorkItemToTestLinksAsync(workItemToTestLinks, projectId.GuidId)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        links = witToTestLinks;
        return num != 0;
      }
    }

    public bool TryQueryTestCaseResultHistory(
      IVssRequestContext context,
      Guid projectId,
      ResultsFilter filter,
      out TestResultHistory resultHistory)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestCaseResultHistory), "Tcm")))
      {
        TestResultHistory testResultHistory = (TestResultHistory) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultHistory = this.TestManagementClient.QueryTestResultHistoryAsync(filter, projectId)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultHistory = testResultHistory;
        return num != 0;
      }
    }

    public bool TryQueryTestHistory(
      IVssRequestContext context,
      Guid projectId,
      TestHistoryQuery filter,
      out TestHistoryQuery result)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestHistory), "Tcm")))
      {
        TestHistoryQuery testHistoryQuery = (TestHistoryQuery) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testHistoryQuery = this.TestManagementClient.QueryTestHistoryAsync(filter, projectId)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        result = testHistoryQuery;
        return num != 0;
      }
    }

    public bool TryQueryTestRuns(
      IVssRequestContext context,
      Guid projectId,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state,
      IEnumerable<int> planIds,
      bool? isAutomated,
      TestRunPublishContext? publishContext,
      IEnumerable<int> buildIds,
      IEnumerable<int> buildDefIds,
      string branchName,
      IEnumerable<int> releaseIds,
      IEnumerable<int> releaseDefIds,
      IEnumerable<int> releaseEnvIds,
      IEnumerable<int> releaseEnvDefIds,
      string runTitle,
      int top,
      string continuationToken,
      out IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs)
    {
      runs = (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
      return true;
    }

    public bool TryGetTestResultsByRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<TestOutcome> outcomes,
      int top,
      string continuationToken,
      out IList<ShallowTestCaseResult> results)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsByRelease), "Tcm")))
      {
        IList<ShallowTestCaseResult> shallowTestCaseResults = (IList<ShallowTestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          shallowTestCaseResults = (IList<ShallowTestCaseResult>) this.TestManagementClient.GetTestResultsByReleaseAsync(projectId, releaseId, new int?(releaseEnvId), publishContext, (IEnumerable<TestOutcome>) outcomes, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        results = shallowTestCaseResults;
        return num != 0;
      }
    }

    public bool TryGetTestResultsByBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      List<TestOutcome> outcomes,
      int top,
      string continuationToken,
      out IList<ShallowTestCaseResult> results)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsByBuild), "Tcm")))
      {
        IList<ShallowTestCaseResult> shallowTestCaseResults = (IList<ShallowTestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          shallowTestCaseResults = (IList<ShallowTestCaseResult>) this.TestManagementClient.GetTestResultsByBuildAsync(projectId, buildId, publishContext, (IEnumerable<TestOutcome>) outcomes, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        results = shallowTestCaseResults;
        return num != 0;
      }
    }

    public bool TryGetAssociatedBugsForResult(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int testCaseResultId,
      out List<WorkItemReference> references)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetAssociatedBugsForResult), "Tcm")))
      {
        List<WorkItemReference> workItemReferences = (List<WorkItemReference>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          workItemReferences = this.TestManagementClient.GetBugsLinkedToTestResultAsync(projectId, runId, testCaseResultId)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        references = workItemReferences;
        return num != 0;
      }
    }

    public bool TryQueryTestResultWorkItems(
      IVssRequestContext context,
      GuidAndString projectId,
      string workItemCategory,
      string automatedTestName,
      int testCaseId,
      DateTime? maxCompleteDate,
      int days,
      int workItemCount,
      out List<WorkItemReference> references)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestResultWorkItems), "Tcm")))
      {
        List<WorkItemReference> workItemReferences = (List<WorkItemReference>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          workItemReferences = this.TestManagementClient.QueryTestResultWorkItemsAsync(projectId.GuidId, workItemCategory, automatedTestName, new int?(testCaseId), maxCompleteDate, new int?(days), new int?(workItemCount))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        references = workItemReferences;
        return num != 0;
      }
    }

    public bool TryGetTestResultGroupsByRelease(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string fields,
      string publishContext,
      out TestResultsGroupsForRelease resultsGroupsForRelease)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultGroupsByRelease), "Tcm")))
      {
        TestResultsGroupsForRelease testResultsGroupsForRelease = (TestResultsGroupsForRelease) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsGroupsForRelease = this.TestManagementClient.GetResultGroupsByReleaseV1Async(projectInfo.Id, releaseId, publishContext, new int?(releaseEnvironmentId), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultsGroupsForRelease = testResultsGroupsForRelease;
        return num != 0;
      }
    }

    public bool TryGetTestResultGroupsByBuild(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      out TestResultsGroupsForBuild resultsGroupsForBuild)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultGroupsByBuild), "Tcm")))
      {
        TestResultsGroupsForBuild testResultsGroupsForBuild = (TestResultsGroupsForBuild) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsGroupsForBuild = this.TestManagementClient.GetResultGroupsByBuildV1Async(projectInfo.Id, buildId, publishContext, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultsGroupsForBuild = testResultsGroupsForBuild;
        return num != 0;
      }
    }

    public bool TryGetTestResultGroupsByReleaseWithWatermark(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string fields,
      string publishContext,
      string continuationToken,
      out IPagedList<FieldDetailsForTestResults> resultsGroupsForRelease)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultGroupsByReleaseWithWatermark), "Tcm")))
      {
        IPagedList<FieldDetailsForTestResults> testResultsGroupsForRelease = (IPagedList<FieldDetailsForTestResults>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsGroupsForRelease = this.TestManagementClient.GetResultGroupsByReleaseWithContinuationTokenAsync(projectInfo.Id, releaseId, publishContext, new int?(releaseEnvironmentId), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), continuationToken)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultsGroupsForRelease = testResultsGroupsForRelease;
        return num != 0;
      }
    }

    public bool TryGetTestResultGroupsByBuildWithWatermark(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      string continuationToken,
      out IPagedList<FieldDetailsForTestResults> resultsGroupsForBuild)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultGroupsByBuildWithWatermark), "Tcm")))
      {
        IPagedList<FieldDetailsForTestResults> testResultsGroupsForBuild = (IPagedList<FieldDetailsForTestResults>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsGroupsForBuild = this.TestManagementClient.GetResultGroupsByBuildWithContinuationTokenAsync(projectInfo.Id, buildId, publishContext, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), continuationToken)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultsGroupsForBuild = testResultsGroupsForBuild;
        return num != 0;
      }
    }

    public bool TryGetTestSettingById(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestSettingById), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings settings = (Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          settings = this.TestManagementClient.GetTestSettingsByIdAsync(projectId, testSettingsId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        testSettings = settings;
        return num != 0;
      }
    }

    public bool TryGetTestRunLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      int runId,
      out List<TestMessageLogDetails> testMessageLogs)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunLogs), "Tcm")))
      {
        List<TestMessageLogDetails> responseFromRemote = (List<TestMessageLogDetails>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (requestContext.IsImpersonating || !this.IsTestRunInTfs(runId, true) || this.TestManagementClient == null)
            return false;
          responseFromRemote = this.TestManagementClient.GetTestRunLogsAsync(projectId, runId)?.Result;
          return true;
        }), requestContext, true) ? 1 : 0;
        testMessageLogs = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryCreateTestSettings(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings,
      out int? testSettingsId)
    {
      testSettingsId = new int?();
      return false;
    }

    public bool TryDeleteTestSettings(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId)
    {
      return false;
    }

    public bool TryCreateTestRun(
      IVssRequestContext context,
      Guid projectId,
      RunCreateModel testRunCreateModel,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestRun), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRun) null;
        newTestRun = testRun;
        return false;
      }
    }

    public bool TryGetTestRunById(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      bool includeDetails,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunById), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun run = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRun) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsTestRunInTfs(runId, true) || this.TestManagementClient == null)
            return false;
          run = this.TestManagementClient.GetTestRunByIdAsync(projectId, runId, new bool?(includeDetails), new bool?(), (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        testRun = run;
        return num != 0;
      }
    }

    public bool TryGetTestRuns(
      IVssRequestContext context,
      Guid projectId,
      string buildUri,
      string owner,
      string tmiRunId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs)
    {
      runs = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
      return false;
    }

    public bool TryGetTestRunsByQuery(
      IVssRequestContext context,
      Guid projectId,
      QueryModel query,
      bool includeIdsOnly,
      bool includeRunDetails,
      int skip,
      int top,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns)
    {
      testRuns = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
      return false;
    }

    public bool TryGetTestResultsByQuery(
      IVssRequestContext context,
      Guid projectId,
      QueryModel query,
      bool includeResultDetails,
      bool includeIterationDetails,
      int skip,
      int top,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsByQuery), "Tcm")))
      {
        IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.TestManagementClient.GetTestResultsByQueryAsync(query, projectId.ToString(), new bool?(includeResultDetails), new bool?(includeIterationDetails), new int?(skip), new int?(top))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        results = testResults;
        return num != 0;
      }
    }

    public bool TryUpdateTestRun(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      RunUpdateModel runUpdateModel,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun updatedTestRun)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateTestRun), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRun) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testRun = this.TestManagementClient.UpdateTestRunAsync(runUpdateModel, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        updatedTestRun = testRun;
        return num != 0;
      }
    }

    public bool TryDeleteTestRun(IVssRequestContext context, Guid projectId, int runId) => false;

    public bool TryGetTestRunStatistics(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic testRunStatistic)
    {
      testRunStatistic = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic) null;
      return false;
    }

    public bool TryAddTestResultsToTestRun(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> newTestResults)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryAddTestResultsToTestRun), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> newResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, true) || this.TestManagementClient == null)
            return false;
          newResults = this.TestManagementClient.AddTestResultsToTestRunAsync(results, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        newTestResults = newResults;
        return num != 0;
      }
    }

    public bool TryGetTestResults(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResults), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testResults = this.TestManagementClient.GetTestResultsAsync(projectId, runId, detailsToInclude, skip, top, outcomes, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        results = testResults;
        return num != 0;
      }
    }

    public bool TryGetTestResultById(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      bool includeIterationDetails,
      bool includeAssociatedBugs,
      bool includeSubResultDetails,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultById), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result = (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, true) || this.TestManagementClient == null)
            return false;
          result = this.TestManagementClient.GetTestResultByIdAsync(projectId, runId, resultId, new ResultDetails?(RestApiHelper.IncludeVariableToResultDetails(includeIterationDetails, includeAssociatedBugs, includeSubResultDetails)))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        testCaseResult = result;
        return num != 0;
      }
    }

    public bool TryUpdateTestResults(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> updatedResults)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateTestResults), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testCaseResults = this.TestManagementClient.UpdateTestResultsAsync(results, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        updatedResults = testCaseResults;
        return num != 0;
      }
    }

    public bool TryGetTestResultsMetaData(
      IVssRequestContext context,
      Guid projectId,
      IList<int> testReferenceIds,
      ResultMetaDataDetails detailsToInclude,
      out IList<TestResultMetaData> metaDataList)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsMetaData), "Tcm")))
      {
        List<TestResultMetaData> exMetaDataList = (List<TestResultMetaData>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          exMetaDataList = this.TestManagementClient.QueryTestResultsMetaDataAsync((IEnumerable<string>) testReferenceIds.Select<int, string>((Func<int, string>) (x => x.ToString())).ToList<string>(), projectId)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        metaDataList = (IList<TestResultMetaData>) exMetaDataList;
        return num != 0;
      }
    }

    public bool TryQueryTestResultsReportForBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      bool includeFailureDetails,
      BuildReference buildToCompare,
      out TestResultSummary testReport)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestResultsReportForBuild), "Tcm")))
      {
        TestResultSummary testResultSummary = (TestResultSummary) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultSummary = this.TestManagementClient.QueryTestResultsReportForBuildAsync(projectId, buildId, publishContext, new bool?(includeFailureDetails), buildToCompare)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        testReport = testResultSummary;
        return num != 0;
      }
    }

    public bool TryQueryTestResultsReportForRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      bool includeFailureDetails,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare,
      out TestResultSummary testReport)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestResultsReportForRelease), "Tcm")))
      {
        TestResultSummary testResultSummary = (TestResultSummary) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultSummary = this.TestManagementClient.QueryTestResultsReportForReleaseAsync(projectId, releaseId, releaseEnvId, publishContext, new bool?(includeFailureDetails), releaseToCompare)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        testReport = testResultSummary;
        return num != 0;
      }
    }

    public bool TryQueryTestResultsSummaryForReleases(
      IVssRequestContext context,
      Guid projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases,
      out List<TestResultSummary> testSummaryForReleases)
    {
      testSummaryForReleases = (List<TestResultSummary>) null;
      return false;
    }

    public bool TryGetTestResultsByQuery(
      IVssRequestContext context,
      Guid projectId,
      TestResultsQuery query,
      out TestResultsQuery results)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsByQuery), "Tcm")))
      {
        TestResultsQuery testResultsQuery = (TestResultsQuery) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          ResultsFilter resultsFilter = query.ResultsFilter;
          if ((resultsFilter != null ? (resultsFilter.ExecutedIn != Service.Tcm ? 1 : 0) : 1) == 0 || this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsQuery = this.TestManagementClient.GetTestResultsByQueryAsync(query, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        results = testResultsQuery;
        return num != 0;
      }
    }

    public bool TryGetTestResultDetailsForBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      out TestResultsDetails resultDetails)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultDetailsForBuild), "Tcm")))
      {
        TestResultsDetails testResultsDetails = (TestResultsDetails) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsDetails = this.TestManagementClient.GetTestResultDetailsForBuildAsync(projectId, buildId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultDetails = testResultsDetails;
        return num != 0;
      }
    }

    public bool TryGetTestResultDetailsForRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      out TestResultsDetails resultDetails)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultDetailsForRelease), "Tcm")))
      {
        TestResultsDetails testResultsDetails = (TestResultsDetails) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.m_requestContext.IsImpersonating || this.TestManagementClient == null)
            return false;
          testResultsDetails = this.TestManagementClient.GetTestResultDetailsForReleaseAsync(projectId, releaseId, releaseEnvId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultDetails = testResultsDetails;
        return num != 0;
      }
    }

    public bool TryQueryResultTrendForBuild(
      IVssRequestContext context,
      Guid projectId,
      TestResultTrendFilter filter,
      out List<AggregatedDataForResultTrend> resultTrend)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryResultTrendForBuild), "Tcm")))
      {
        List<AggregatedDataForResultTrend> aggregatedDataForResultTrends = (List<AggregatedDataForResultTrend>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          aggregatedDataForResultTrends = this.TestManagementClient.QueryResultTrendForBuildAsync(filter, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultTrend = aggregatedDataForResultTrends;
        return num != 0;
      }
    }

    public bool TryQueryResultTrendForRelease(
      IVssRequestContext context,
      Guid projectId,
      TestResultTrendFilter filter,
      out List<AggregatedDataForResultTrend> resultTrend)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryGetTestResultDetailsForRelease", "Tcm")))
      {
        List<AggregatedDataForResultTrend> aggregatedDataForResultTrends = (List<AggregatedDataForResultTrend>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          aggregatedDataForResultTrends = this.TestManagementClient.QueryResultTrendForReleaseAsync(filter, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        resultTrend = aggregatedDataForResultTrends;
        return num != 0;
      }
    }

    public bool TryQueuePublishTestResultJob(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int runId,
      int attachmentId,
      TestResultDocument document,
      out TestResultDocument testResultDocument)
    {
      return RestApiHelper.QueuePublishTestResultJob(context, projectInfo, runId, attachmentId, document, true, out testResultDocument);
    }

    public bool TryCreateTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      out TestAttachmentReference attachment)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestRunAttachment), "Tcm")))
      {
        TestAttachmentReference testAttachmentReference = (TestAttachmentReference) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachmentReference = this.TestManagementClient.CreateTestRunAttachmentAsync(attachmentRequestModel, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        attachment = testAttachmentReference;
        return num != 0;
      }
    }

    public bool TryCreateTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      out TestAttachmentReference attachment)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestResultAttachment), "Tcm")))
      {
        TestAttachmentReference testAttachmentReference = (TestAttachmentReference) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachmentReference = this.TestManagementClient.CreateTestResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        attachment = testAttachmentReference;
        return num != 0;
      }
    }

    public bool TryCreateTestIterationResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      int iterationId,
      string actionPath,
      out TestAttachmentReference attachment)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestIterationResultAttachment), "Tcm")))
      {
        TestAttachmentReference testAttachmentReference = (TestAttachmentReference) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachmentReference = this.TestManagementClient.CreateTestIterationResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, iterationId, actionPath)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        attachment = testAttachmentReference;
        return num != 0;
      }
    }

    public bool TryCreateTestSubResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      int subResultId,
      out TestAttachmentReference attachment)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestSubResultAttachment), "Tcm")))
      {
        TestAttachmentReference testAttachmentReference = (TestAttachmentReference) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachmentReference = this.TestManagementClient.CreateTestSubResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, subResultId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, true) ? 1 : 0;
        attachment = testAttachmentReference;
        return num != 0;
      }
    }

    public bool TryGetTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int attachmentId,
      out Attachment attachment)
    {
      attachment = (Attachment) null;
      return false;
    }

    public bool TryGetTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      out Attachment attachment)
    {
      attachment = (Attachment) null;
      return false;
    }

    public bool TryGetTestSubResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId,
      out Attachment attachment)
    {
      attachment = (Attachment) null;
      return false;
    }

    public bool TryGetTestIterationAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      int iterationId,
      out Attachment attachment)
    {
      attachment = (Attachment) null;
      return false;
    }

    public bool TryGetTestRunAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out List<TestAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunAttachments), "Tcm")))
      {
        List<TestAttachment> testAttachments = (List<TestAttachment>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachments = this.TestManagementClient.GetTestRunAttachmentsAsync(projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, false) ? 1 : 0;
        attachments = testAttachments;
        return num != 0;
      }
    }

    public bool TryGetTestResultAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      out List<TestAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultAttachments), "Tcm")))
      {
        List<TestAttachment> testAttachments = (List<TestAttachment>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachments = this.TestManagementClient.GetTestResultAttachmentsAsync(projectId, runId, resultId, (object) null, new CancellationToken())?.Result;
          return true;
        }), context, false) ? 1 : 0;
        attachments = testAttachments;
        return num != 0;
      }
    }

    public bool TryGetTestSubResultAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      out List<TestAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestSubResultAttachments), "Tcm")))
      {
        List<TestAttachment> testAttachments = (List<TestAttachment>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || !this.IsTestRunInTfs(runId, false) || this.TestManagementClient == null)
            return false;
          testAttachments = this.TestManagementClient.GetTestSubResultAttachmentsAsync(projectId, runId, resultId, subResultId)?.Result;
          return true;
        }), context, false) ? 1 : 0;
        attachments = testAttachments;
        return num != 0;
      }
    }

    public bool TryDeleteTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int attachmentId)
    {
      return false;
    }

    public bool TryDeleteTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId)
    {
      return false;
    }

    public bool TryQueryTestSummaryByRequirement(
      IVssRequestContext context,
      Guid projectId,
      TestResultsContext resultsContext,
      List<int> workItemIds,
      out List<TestSummaryForWorkItem> summaryForWorkItemList)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestSummaryByRequirement), "Tcm")))
      {
        List<TestSummaryForWorkItem> testSummaryForWorkItems = (List<TestSummaryForWorkItem>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (context.IsImpersonating || this.TestManagementClient == null)
            return false;
          testSummaryForWorkItems = this.TestManagementClient.QueryTestSummaryByRequirementAsync(resultsContext, projectId, (IEnumerable<int>) workItemIds)?.Result;
          return true;
        }), context, true) ? 1 : 0;
        summaryForWorkItemList = testSummaryForWorkItems;
        return num != 0;
      }
    }

    public bool TryGetCodeCoverageSummary(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      int deltaBuildId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary)
    {
      codeCoverageSummary = (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary) null;
      return false;
    }

    public bool TryGetBuildCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      int flag,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage)
    {
      buildCoverage = (List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) null;
      return false;
    }

    public bool TryGetTestRunCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int flags,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage)
    {
      runCoverage = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) null;
      return false;
    }

    public bool TryGetAfnStripsExistenceMapping(
      IVssRequestContext context,
      string projectName,
      IList<int> testCaseIds,
      out Dictionary<int, bool> existenceMapping)
    {
      existenceMapping = (Dictionary<int, bool>) null;
      return false;
    }

    public bool TryGetAttachmentsByQuery(
      IVssRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      throw new NotImplementedException();
    }

    public bool TryUpdateDefaultStrips(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> list)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestAttachments(
      IVssRequestContext requestContext,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestAttachments(
      IVssRequestContext requestContext,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestRunSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      List<string> dimensions,
      out TestExecutionReportData runSummaryReport)
    {
      runSummaryReport = (TestExecutionReportData) null;
      return false;
    }

    public bool TryGetTestExecutionSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int planId,
      List<TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensions,
      out TestExecutionReportData executionSummaryReport)
    {
      executionSummaryReport = (TestExecutionReportData) null;
      return false;
    }

    public bool IsTestRunInTCM(IVssRequestContext requestContext, int runId, bool queryFlow = true) => throw new NotImplementedException();

    public bool TryQueryTestActionResults(
      IVssRequestContext requestContext,
      string projectName,
      TestCaseResultIdentifier identifier,
      out QueryTestActionResultResponse response)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestResultInMultipleProjects(
      IVssRequestContext requestContext,
      int testRunId,
      int testResultId,
      out TestResultAcrossProjectResponse response)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestResultsByQuery(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out ResultsByQueryResponse reponse)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryTestRunByTmiRunId(
      IVssRequestContext requestContext,
      Guid tmiRunId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryTestRunsLegacy(
      IVssRequestContext requestContext,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId,
      int skip,
      int top,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryTestRunsInMultipleProjects(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryByRunAndOutcome(
      IVssRequestContext requestContext,
      int testRunId,
      byte outcome,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryByRunAndState(
      IVssRequestContext requestContext,
      int testRunId,
      byte state,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryByRunAndOwner(
      IVssRequestContext requestContext,
      int testRunId,
      Guid owner,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryByRun(
      IVssRequestContext requestContext,
      QueryByRunRequest queryByRunRequest,
      out FetchTestResultsResponse responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryFetchTestResults(
      IVssRequestContext requestContext,
      FetchTestResultsRequest fetchTestResultsRequest,
      out FetchTestResultsResponse responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryCreateTestRunLegacy(
      IVssRequestContext context,
      CreateTestRunRequest createTestRunRequest,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun response)
    {
      throw new NotImplementedException();
    }

    public bool TryUpdateTestResultsLegacy(
      IVssRequestContext requestContext,
      BulkResultUpdateRequest request,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] response)
    {
      throw new NotImplementedException();
    }

    bool ITcmServiceHelper.TryQueryTestRuns2Legacy(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryCreateTestResultsLegacy(
      IVssRequestContext context,
      string projectName,
      CreateTestResultsRequest request)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryCount(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out int? countFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryUpdateTestRunLegacy(
      IVssRequestContext requestContext,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      bool shouldHyderate,
      out UpdateTestRunResponse response)
    {
      throw new NotImplementedException();
    }

    public bool TryGetTestResultAttachment(
      TestManagementRequestContext requestContext,
      string projectName,
      int attachmentId,
      out TcmAttachment tcmAttachment)
    {
      throw new NotImplementedException();
    }

    public bool TryResetTestResults(
      IVssRequestContext requestContext,
      string projectName,
      LegacyTestCaseResultIdentifier[] requestsForRemote,
      out LegacyTestCaseResult[] responseFromRemote)
    {
      throw new NotImplementedException();
    }

    public bool TryAbortTestRun(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      int revision,
      int options,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryTestRunStats(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      out List<LegacyTestRunStatistic> testRunStats)
    {
      throw new NotImplementedException();
    }

    public bool TryQueryByPoint(
      IVssRequestContext requestContext,
      string projectName,
      int planId,
      int pointId,
      out List<LegacyTestCaseResult> resultsFromRemote)
    {
      throw new NotImplementedException();
    }

    internal TestManagementHttpClient TestManagementClient => this.m_requestContext.GetClient<TestManagementHttpClient>();

    private TestManagementHttpClient GetTcmHttpClient() => this.m_requestContext.GetClient<TestManagementHttpClient>();

    public bool IsTestRunInTfs(int runId, bool queryFlow = true)
    {
      if (this.m_requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun"))
        return false;
      IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = this.m_requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold";
      ref RegistryQuery local1 = ref registryQuery;
      int num = service.GetValue<int>(requestContext1, in local1, int.MaxValue);
      IVssRequestContext requestContext2 = this.m_requestContext;
      registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceDataImportAccount";
      ref RegistryQuery local2 = ref registryQuery;
      return !(this.m_requestContext.ExecutionEnvironment.IsHostedDeployment & service.GetValue<bool>(requestContext2, in local2, false)) && (num != int.MaxValue && runId < num || runId % 2 != 0);
    }

    private bool ShouldCreateTestRunInTfs(Guid projectId, RunCreateModel runCreateModel) => false;

    private bool InvokeAction(Func<bool> func, IVssRequestContext context, bool isMigrated)
    {
      try
      {
        return (!isMigrated || !TCMServiceDataMigrationRestHelper.IsMigrationCompleted(context)) && func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }
  }
}
