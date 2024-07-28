// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TcmServiceHelper : ITcmServiceHelper
  {
    private IVssRequestContext m_requestContext;
    private Guid m_exemptedS2SPrincipal;

    public TcmServiceHelper(IVssRequestContext context) => this.m_requestContext = context;

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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testToWorkItemLinks = this.TestResultsHttpClient.QueryTestMethodLinkedWorkItemsAsync(projectId.GuidId, testName)?.Result;
          return true;
        })) ? 1 : 0;
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
        return this.InvokeAction((Func<bool>) (() => !this.IsImpersonating() && this.TestResultsHttpClient != null && this.TestResultsHttpClient.DeleteTestMethodToWorkItemLinkAsync(projectId.GuidId, testName, workItemId).Result));
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
          if ((workItemToTestLinks1 != null ? (workItemToTestLinks1.ExecutedIn == Service.Tcm ? 1 : 0) : 0) == 0 || this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          witToTestLinks = this.TestResultsHttpClient.AddWorkItemToTestLinksAsync(workItemToTestLinks, projectId.GuidId)?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultHistory = this.TestResultsHttpClient.QueryTestResultHistoryAsync(filter, projectId)?.Result;
          return true;
        })) ? 1 : 0;
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
          TestResultsHttpClient resultsHttpClient = this.TestResultsHttpClientWithReadOnly("WebAccess.TestManagement.QueryTestHistory.EnableSqlReadReplica");
          if (this.IsImpersonating() || resultsHttpClient == null)
            return false;
          testHistoryQuery = resultsHttpClient.QueryTestHistoryAsync(filter, projectId)?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRuns), "Tcm")))
      {
        IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns = (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testRuns = this.TestResultsHttpClient.QueryTestRunsAsync2(projectId, minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        runs = testRuns;
        return num != 0;
      }
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          shallowTestCaseResults = (IList<ShallowTestCaseResult>) this.TestResultsHttpClient.GetTestResultsByReleaseAsync(projectId, releaseId, new int?(releaseEnvId), publishContext, (IEnumerable<TestOutcome>) outcomes, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          shallowTestCaseResults = (IList<ShallowTestCaseResult>) this.TestResultsHttpClient.GetTestResultsByBuildAsync(projectId, buildId, publishContext, (IEnumerable<TestOutcome>) outcomes, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          workItemReferences = this.TestResultsHttpClient.GetBugsLinkedToTestResultAsync(projectId, runId, testCaseResultId)?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          workItemReferences = this.TestResultsHttpClient.QueryTestResultWorkItemsAsync(projectId.GuidId, workItemCategory, automatedTestName, new int?(testCaseId), maxCompleteDate, new int?(days), new int?(workItemCount))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultsGroupsForRelease = this.TestResultsHttpClient.GetResultGroupsByReleaseV1Async(projectInfo.Id, releaseId, publishContext, new int?(releaseEnvironmentId), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultsGroupsForBuild = this.TestResultsHttpClient.GetResultGroupsByBuildV1Async(projectInfo.Id, buildId, publishContext, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          settings = this.TestResultsHttpClient.GetTestSettingsByIdAsync(projectId, testSettingsId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        testSettings = settings;
        return num != 0;
      }
    }

    public bool TryCreateTestSettings(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings,
      out int? testSettingsId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestSettings), "Tcm")))
      {
        int? settingsId = new int?();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          settingsId = this.TestResultsHttpClient.CreateTestSettingsAsync(testSettings, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        testSettingsId = settingsId;
        return num != 0;
      }
    }

    public bool TryDeleteTestSettings(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteTestSettings), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          this.TestResultsHttpClient.DeleteTestSettingsAsync(projectId, testSettingsId, (object) null, new CancellationToken()).Wait();
          return true;
        }));
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
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.ShouldCreateTestRunInTcm(projectId, testRunCreateModel) || this.TestResultsHttpClient == null)
            return false;
          testRun = this.TestResultsHttpClient.CreateTestRunAsync(testRunCreateModel, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        newTestRun = testRun;
        return num != 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          run = this.TestResultsHttpClient.GetTestRunByIdAsync(projectId, runId, new bool?(includeDetails), new bool?(), (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRuns), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testRuns = this.TestResultsHttpClient.GetTestRunsAsync(projectId, buildUri, owner, tmiRunId, new int?(planId), new bool?(includeRunDetails), automated, new int?(skip), new int?(top))?.Result;
          return true;
        })) ? 1 : 0;
        runs = testRuns;
        return num != 0;
      }
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunsByQuery), "Tcm")))
      {
        IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          runs = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.TcmHttpClient.GetTestRunsByQueryAsync(query, projectId, includeIdsOnly, new bool?(includeRunDetails), new int?(skip), new int?(top))?.Result;
          return true;
        })) ? 1 : 0;
        testRuns = runs;
        return num != 0;
      }
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.TestResultsHttpClient.GetTestResultsByQueryWiqlAsync(query, projectId.ToString(), new bool?(includeResultDetails), new bool?(includeIterationDetails), new int?(skip), new int?(top))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, false) || this.TestResultsHttpClient == null)
            return false;
          testRun = this.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        updatedTestRun = testRun;
        return num != 0;
      }
    }

    public bool TryDeleteTestRun(IVssRequestContext context, Guid projectId, int runId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteTestRun), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, false) || this.TestResultsHttpClient == null)
            return false;
          this.TestResultsHttpClient.DeleteTestRunAsync(projectId, runId)?.Wait();
          return true;
        }));
    }

    public bool TryGetTestRunStatistics(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic runStatistic)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunStatistics), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic testRunStatistic = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, false) || this.TestResultsHttpClient == null)
            return false;
          testRunStatistic = this.TestResultsHttpClient.GetTestRunStatisticsAsync(projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        runStatistic = testRunStatistic;
        return num != 0;
      }
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          newResults = this.TestResultsHttpClient.AddTestResultsToTestRunAsync(results, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          if (top.HasValue && top.Value == int.MaxValue)
            top = new int?();
          testResults = this.TestResultsHttpClient.GetTestResultsAsync(projectId, runId, detailsToInclude, skip, top, outcomes, new bool?(), (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          result = this.TestResultsHttpClient.GetTestResultByIdAsync(projectId, runId, resultId, new ResultDetails?(RestApiHelper.IncludeVariableToResultDetails(includeIterationDetails, includeAssociatedBugs, includeSubResultDetails)))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, false) || this.TestResultsHttpClient == null)
            return false;
          testCaseResults = this.TestResultsHttpClient.UpdateTestResultsAsync(results, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        updatedResults = testCaseResults;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultSummary = this.TestResultsHttpClient.QueryTestResultsReportForBuildAsync(projectId, buildId, publishContext, new bool?(includeFailureDetails), buildToCompare)?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultSummary = this.TestResultsHttpClient.QueryTestResultsReportForReleaseAsync(projectId, releaseId, releaseEnvId, publishContext, new bool?(includeFailureDetails), releaseToCompare)?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestResultsSummaryForReleases), "Tcm")))
      {
        List<TestResultSummary> testResultSummaries = (List<TestResultSummary>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultSummaries = this.TestResultsHttpClient.QueryTestResultsSummaryForReleasesAsync((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference>) releases, projectId)?.Result;
          return true;
        })) ? 1 : 0;
        testSummaryForReleases = testResultSummaries;
        return num != 0;
      }
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
          if ((resultsFilter != null ? (resultsFilter.ExecutedIn != Service.Tfs ? 1 : 0) : 1) == 0 || this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultsQuery = this.TestResultsHttpClientWithReadOnly("TestManagement.Server.QueryResults.EnableSqlReadReplica").GetTestResultsByQueryAsync(query, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        results = testResultsQuery;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          exMetaDataList = this.TestResultsHttpClient.QueryTestResultsMetaDataAsync((IEnumerable<string>) testReferenceIds.Select<int, string>((Func<int, string>) (x => x.ToString())).ToList<string>(), projectId, new ResultMetaDataDetails?(detailsToInclude), (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
        metaDataList = (IList<TestResultMetaData>) exMetaDataList;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultsDetails = this.TestResultsHttpClient.GetTestResultDetailsForBuildAsync(projectId, buildId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testResultsDetails = this.TestResultsHttpClient.GetTestResultDetailsForReleaseAsync(projectId, releaseId, releaseEnvId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          aggregatedDataForResultTrends = this.TestResultsHttpClient.QueryResultTrendForBuildAsync(filter, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          aggregatedDataForResultTrends = this.TestResultsHttpClient.QueryResultTrendForReleaseAsync(filter, projectId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
      return RestApiHelper.QueuePublishTestResultJob(context, projectInfo, runId, attachmentId, document, false, out testResultDocument);
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachmentReference = this.TestResultsHttpClient.CreateTestRunAttachmentAsync(attachmentRequestModel, projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachmentReference = this.TestResultsHttpClient.CreateTestResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachmentReference = this.TestResultsHttpClient.CreateTestSubResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, subResultId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachmentReference = this.TestResultsHttpClient.CreateTestIterationResultAttachmentAsync(attachmentRequestModel, projectId, runId, testResultId, iterationId, actionPath)?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunAttachment), "Tcm")))
      {
        Attachment testAttachment = (Attachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachment = this.TestResultsHttpClient.GetTcmRunAttachmentContentAsync(projectId, runId, attachmentId)?.Result;
          return true;
        })) ? 1 : 0;
        attachment = testAttachment;
        return num != 0;
      }
    }

    public bool TryGetTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      out Attachment attachment)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultAttachment), "Tcm")))
      {
        Attachment testAttachment = (Attachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachment = this.TestResultsHttpClient.GetTcmResultAttachmentContentAsync(projectId, runId, resultId, attachmentId)?.Result;
          return true;
        })) ? 1 : 0;
        attachment = testAttachment;
        return num != 0;
      }
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestSubResultAttachment), "Tcm")))
      {
        Attachment testAttachment = (Attachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachment = this.TestResultsHttpClient.GetTcmSubResultAttachmentContentAsync(projectId, runId, resultId, subResultId, attachmentId)?.Result;
          return true;
        })) ? 1 : 0;
        attachment = testAttachment;
        return num != 0;
      }
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestIterationAttachment), "Tcm")))
      {
        Attachment testAttachment = (Attachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachment = this.TestResultsHttpClient.GetTcmIterationAttachmentContentAsync(projectId, runId, resultId, attachmentId, iterationId)?.Result;
          return true;
        })) ? 1 : 0;
        attachment = testAttachment;
        return num != 0;
      }
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
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachments = this.TestResultsHttpClient.GetTestRunAttachmentsAsync(projectId, runId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachments = this.TestResultsHttpClient.GetTestResultAttachmentsAsync(projectId, runId, resultId, (object) null, new CancellationToken())?.Result;
          return true;
        })) ? 1 : 0;
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
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          testAttachments = this.TestResultsHttpClient.GetTestSubResultAttachmentsAsync(projectId, runId, resultId, subResultId)?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteTestRunAttachment), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          this.TestResultsHttpClient.DeleteTestRunAttachmentAsync(projectId, runId, attachmentId)?.Wait();
          return true;
        }));
    }

    public bool TryDeleteTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteTestResultAttachment), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsTestRunInTfs(runId, true) || this.TestResultsHttpClient == null)
            return false;
          this.TestResultsHttpClient.DeleteTestResultAttachmentAsync(projectId, runId, resultId, attachmentId)?.Wait();
          return true;
        }));
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
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          testSummaryForWorkItems = this.TestResultsHttpClient.QueryTestSummaryByRequirementAsync(resultsContext, projectId, (IEnumerable<int>) workItemIds)?.Result;
          return true;
        })) ? 1 : 0;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetCodeCoverageSummary), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary coverageSummary = (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary) null;
        int num = this.InvokeAction((Func<bool>) (() => !this.IsImpersonating() && this.TestResultsHttpClient != null && TcmServiceHelper.TryCompatClientRequest<Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary>(context, closure_0 ?? (closure_0 = (Func<Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary>) (() => this.TestResultsHttpClient.GetCodeCoverageSummaryAsync(projectId, buildId, new int?(deltaBuildId))?.Result)), out coverageSummary))) ? 1 : 0;
        codeCoverageSummary = coverageSummary;
        return num != 0;
      }
    }

    public bool TryGetBuildCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      int flag,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetBuildCodeCoverage), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverages = (List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) null;
        int num = this.InvokeAction((Func<bool>) (() => !this.IsImpersonating() && this.TestResultsHttpClient != null && TcmServiceHelper.TryCompatClientRequest<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>(context, closure_0 ?? (closure_0 = (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>) (() => this.TestResultsHttpClient.GetBuildCodeCoverageAsync(projectId, buildId, flag)?.Result)), out buildCoverages))) ? 1 : 0;
        buildCoverage = buildCoverages;
        return num != 0;
      }
    }

    public bool TryGetTestRunCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int flags,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunCodeCoverage), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverages = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsTestRunInTCM(context, runId, true) || this.TestResultsHttpClient == null)
            return false;
          runCoverages = this.TestResultsHttpClient.GetTestRunCodeCoverageAsync(projectId, runId, flags)?.Result;
          return true;
        })) ? 1 : 0;
        runCoverage = runCoverages;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryGetTestResultGroupsByRelease", "Tcm")))
      {
        IPagedList<FieldDetailsForTestResults> fieldDetailsForTestResults = (IPagedList<FieldDetailsForTestResults>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          fieldDetailsForTestResults = this.TestResultsHttpClient.GetResultGroupsByReleaseWithContinuationTokenAsync(projectInfo.Id, releaseId, publishContext, new int?(releaseEnvironmentId), (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), continuationToken)?.Result;
          return true;
        })) ? 1 : 0;
        resultsGroupsForRelease = fieldDetailsForTestResults;
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
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryGetTestResultGroupsByBuild", "Tcm")))
      {
        IPagedList<FieldDetailsForTestResults> fieldDetailsForTestResults = (IPagedList<FieldDetailsForTestResults>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TestResultsHttpClient == null)
            return false;
          fieldDetailsForTestResults = this.TestResultsHttpClient.GetResultGroupsByBuildWithContinuationTokenAsync(projectInfo.Id, buildId, publishContext, (IEnumerable<string>) ParsingHelper.ParseCommaSeparatedString(fields), continuationToken)?.Result;
          return true;
        })) ? 1 : 0;
        resultsGroupsForBuild = fieldDetailsForTestResults;
        return num != 0;
      }
    }

    public bool TryGetAfnStripsExistenceMapping(
      IVssRequestContext context,
      string projectName,
      IList<int> testCaseIds,
      out Dictionary<int, bool> existenceMapping)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryGetExistenceMapping", "Tcm")))
      {
        Dictionary<int, bool> response = new Dictionary<int, bool>();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          Task<List<AfnStrip>> afnStripsAsync = this.TcmHttpClient.GetAfnStripsAsync(projectName, (IEnumerable<int>) testCaseIds);
          Dictionary<int, bool> dictionary;
          if (afnStripsAsync == null)
          {
            dictionary = (Dictionary<int, bool>) null;
          }
          else
          {
            List<AfnStrip> result = afnStripsAsync.Result;
            dictionary = result != null ? result.ToDictionary<AfnStrip, int, bool>((Func<AfnStrip, int>) (afnStrip => afnStrip.TestCaseId), (Func<AfnStrip, bool>) (afnStrip => true)) : (Dictionary<int, bool>) null;
          }
          response = dictionary;
          return true;
        })) ? 1 : 0;
        existenceMapping = response;
        return num != 0;
      }
    }

    public bool TryGetAttachmentsByQuery(
      IVssRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetAttachmentsByQuery), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> response = new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.GetAttachmentsByQueryAsync(query, projectName)?.Result;
          return true;
        })) ? 1 : 0;
        attachments = response;
        attachments?.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) (attachment => attachment.PopulateUrlField(query.TeamProjectName, "tcm")));
        return num != 0;
      }
    }

    public bool TryUpdateDefaultStrips(
      IVssRequestContext context,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateDefaultStrips), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          this.TcmHttpClient.UpdateDefaultAfnStripsAsync((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) bindings, projectId)?.Wait();
          return true;
        }));
    }

    public bool TryGetTestAttachments(
      IVssRequestContext context,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestAttachments), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> response = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || sessionId > 0 || runId > 0 && !this.IsTestRunInTCM(context, runId, true) || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.GetAttachmentsAsync(projectName, attachmentId, runId, resultId, subResultId, sessionId)?.Result;
          return true;
        })) ? 1 : 0;
        response?.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) (attachment => attachment.PopulateUrlField(projectName, "tcm")));
        attachments = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) response;
        return num != 0;
      }
    }

    public bool TryGetTestAttachments(
      IVssRequestContext context,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestAttachments), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> response = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.GetAttachments2Async(projectName, attachmentId, getSiblingAttachments)?.Result;
          return true;
        })) ? 1 : 0;
        response?.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) (attachment => attachment.PopulateUrlField(projectName, "tcm")));
        attachments = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) response;
        return num != 0;
      }
    }

    public bool TryGetTestRunSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      List<string> dimensions,
      out TestExecutionReportData runSummaryReport)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestRunSummaryReport), "Tcm")))
      {
        TestExecutionReportData response = new TestExecutionReportData();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (runId > 0)
          {
            if (!this.IsImpersonating() && this.IsTestRunInTCM(context, runId, true) && this.TcmHttpClient != null)
            {
              response = this.TcmHttpClient.GetTestRunSummaryReportAsync(projectId, runId, (IEnumerable<string>) dimensions)?.Result;
              return true;
            }
          }
          else if (!this.IsImpersonating() && this.TcmHttpClient != null)
          {
            response = this.TcmHttpClient.GetTestRunSummaryReportAsync(projectId, runId, (IEnumerable<string>) dimensions)?.Result;
            return true;
          }
          return false;
        })) ? 1 : 0;
        runSummaryReport = response;
        return num != 0;
      }
    }

    public bool TryGetTestExecutionSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int planId,
      List<TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensions,
      out TestExecutionReportData executionSummaryReport)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestExecutionSummaryReport), "Tcm")))
      {
        TestExecutionReportData response = new TestExecutionReportData();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClientWithReadOnly("TestManagement.Server.GetTestExecutionSummaryReport.EnableSqlReadReplica");
          if (this.IsImpersonating() || tcmHttpClient == null)
            return false;
          response = tcmHttpClient.GetTestExecutionSummaryReportAsync((IEnumerable<TestAuthoringDetails>) testAuthoringDetails, projectId, planId, (IEnumerable<string>) dimensions)?.Result;
          return true;
        })) ? 1 : 0;
        executionSummaryReport = response;
        return num != 0;
      }
    }

    public bool TryCreateTestResultsLegacy(
      IVssRequestContext context,
      string projectName,
      CreateTestResultsRequest request)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestResultsLegacy), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsImpersonating())
          {
            LegacyTestCaseResult[] results = request.Results;
            if ((results != null ? (((IEnumerable<LegacyTestCaseResult>) results).Any<LegacyTestCaseResult>() ? 1 : 0) : 0) != 0 && this.TcmHttpClient != null)
            {
              this.TcmHttpClient.CreateTestResultsLegacyAsync(request, (object) projectName)?.Wait();
              return true;
            }
          }
          return false;
        }));
    }

    public bool TryCreateTestRunLegacy(
      IVssRequestContext context,
      CreateTestRunRequest createTestRunRequest,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun response)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestRunLegacy), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.ShouldCreateTestRunInTcm(createTestRunRequest.ProjectName, createTestRunRequest) || this.TcmHttpClient == null)
            return false;
          testRun = this.TcmHttpClient.CreateTestRunLegacyAsync(createTestRunRequest)?.Result;
          return true;
        })) ? 1 : 0;
        response = testRun;
        return num != 0;
      }
    }

    public bool TryUpdateTestResultsLegacy(
      IVssRequestContext requestContext,
      BulkResultUpdateRequest request,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateTestResultsLegacy), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromRemote = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsImpersonating())
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests = request.Requests;
            if ((requests != null ? (((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) requests).Any<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() ? 1 : 0) : 0) != 0 && this.TcmHttpClient != null)
            {
              responseFromRemote = this.TcmHttpClient.UpdateTestResultsLegacyAsync(request)?.Result;
              return true;
            }
          }
          return false;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestActionResults(
      IVssRequestContext requestContext,
      string projectName,
      TestCaseResultIdentifier identifier,
      out QueryTestActionResultResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestActionResults), "Tcm")))
      {
        QueryTestActionResultResponse responseFromRemote = (QueryTestActionResultResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, identifier.TestRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestActionResultRequest request = new QueryTestActionResultRequest();
          request.ProjectName = projectName;
          request.Identifier = TestCaseResultIdentifierConverter.Convert(identifier);
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryTestActionResultsAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryGetTestResultInMultipleProjects(
      IVssRequestContext requestContext,
      int testRunId,
      int testResultId,
      out TestResultAcrossProjectResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultInMultipleProjects), "Tcm")))
      {
        TestResultAcrossProjectResponse responseFromRemote = (TestResultAcrossProjectResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          LegacyTestCaseResultIdentifier identifier = new LegacyTestCaseResultIdentifier();
          identifier.TestRunId = testRunId;
          identifier.TestResultId = testResultId;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.GetTestResultsAcrossProjectsAsync(identifier, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryGetTestResultsByQuery(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out ResultsByQueryResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultsByQuery), "Tcm")))
      {
        ResultsByQueryResponse responseFromRemote = (ResultsByQueryResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          ResultsByQueryRequest request = new ResultsByQueryRequest();
          request.PageSize = pageSize;
          request.Query = query;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.GetTestResultsByQueryLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestRunByTmiRunId(
      IVssRequestContext requestContext,
      Guid tmiRunId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRunByTmiRunId), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun responseFromRemote = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.GetTestRunByTmiRunIdAsync(tmiRunId)?.Result;
          return true;
        })) ? 1 : 0;
        testRun = responseFromRemote;
        return num != 0;
      }
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
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRunsLegacy), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> responseFromRemote = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestRunsRequest request = new QueryTestRunsRequest();
          request.TestRunId = testRunId;
          request.Owner = owner;
          request.BuildUri = buildUri;
          request.TeamProjectName = teamProjectName;
          request.PlanId = planId;
          request.Skip = skip;
          request.Top = top;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryTestRunsLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        runsFromRemote = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestRunsInMultipleProjects(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRunsInMultipleProjects), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> responseFromRemote = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.QueryTestRunsAcrossMultipleProjectsAsync(query)?.Result;
          return true;
        })) ? 1 : 0;
        runsFromRemote = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestRuns2Legacy(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRuns2Legacy), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> responseFromRemote = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestRuns2Request request = new QueryTestRuns2Request();
          request.Query = resultsStoreQuery;
          request.IncludeStatistics = includeStatistics;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryTestRuns2Async(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        runsFromRemote = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryByRunAndOutcome(
      IVssRequestContext requestContext,
      int testRunId,
      byte outcome,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryByRunAndOutcome), "Tcm")))
      {
        FetchTestResultsResponse responseFromRemote = (FetchTestResultsResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryByRunRequest request = new QueryByRunRequest();
          request.TestRunId = testRunId;
          request.Outcome = outcome;
          request.PageSize = pageSize;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryByRunAndOutcomeAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryByRunAndState(
      IVssRequestContext requestContext,
      int testRunId,
      byte state,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryByRunAndState), "Tcm")))
      {
        FetchTestResultsResponse responseFromRemote = (FetchTestResultsResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryByRunRequest request = new QueryByRunRequest();
          request.TestRunId = testRunId;
          request.State = state;
          request.PageSize = pageSize;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryByRunAndStateAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryByRunAndOwner(
      IVssRequestContext requestContext,
      int testRunId,
      Guid owner,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryByRunAndOwner), "Tcm")))
      {
        FetchTestResultsResponse responseFromRemote = (FetchTestResultsResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryByRunRequest request = new QueryByRunRequest();
          request.TestRunId = testRunId;
          request.Owner = owner;
          request.PageSize = pageSize;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryByRunAndOwnerAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryByRun(
      IVssRequestContext requestContext,
      QueryByRunRequest queryByRunRequest,
      out FetchTestResultsResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryByRun), "Tcm")))
      {
        FetchTestResultsResponse responseFromRemote = (FetchTestResultsResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, queryByRunRequest.TestRunId, true) || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.QueryByRunAsync(queryByRunRequest)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryFetchTestResults(
      IVssRequestContext requestContext,
      FetchTestResultsRequest fetchTestResultsRequest,
      out FetchTestResultsResponse response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryFetchTestResults), "Tcm")))
      {
        FetchTestResultsResponse responseFromRemote = (FetchTestResultsResponse) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.FetchTestResultsAsync(fetchTestResultsRequest)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryCount(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out int? countFromRemote)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryCount), "Tcm")))
      {
        int? responseFromRemote = new int?();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.QueryTestRunsCountAsync(query)?.Result;
          return true;
        })) ? 1 : 0;
        countFromRemote = responseFromRemote;
        return num != 0;
      }
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
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateTestRunLegacy), "Tcm")))
      {
        UpdateTestRunResponse responseFromRemote = (UpdateTestRunResponse) null;
        int num1 = this.InvokeAction((Func<bool>) (() =>
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun legacyTestRun = testRun;
          int num2;
          if (legacyTestRun == null)
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] source1 = attachmentsToAdd;
            int? nullable = source1 != null ? ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) source1).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>()?.TestRunId : new int?();
            if (!nullable.HasValue)
            {
              Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] source2 = attachmentsToDelete;
              num2 = (source2 != null ? ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) source2).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>()?.TestRunId : new int?()).GetValueOrDefault();
            }
            else
              num2 = nullable.GetValueOrDefault();
          }
          else
            num2 = legacyTestRun.TestRunId;
          int runId = num2;
          if (this.IsImpersonating() || runId == 0 || runId > 0 && !this.IsTestRunInTCM(requestContext, runId, false) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          UpdateTestRunRequest request = new UpdateTestRunRequest();
          request.ProjectName = projectName;
          request.TestRun = testRun;
          request.AttachmentsToAdd = attachmentsToAdd;
          request.AttachmentsToDelete = attachmentsToDelete;
          request.ShouldHyderate = shouldHyderate;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.UpdateTestRunLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num1 != 0;
      }
    }

    public bool TryGetTestResultAttachment(
      TestManagementRequestContext tmRequestContext,
      string projectName,
      int attachmentId,
      out TcmAttachment tcmAttachment)
    {
      using (PerfManager.Measure(tmRequestContext.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultAttachment), "Tcm")))
      {
        TcmAttachment testAttachment = (TcmAttachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.IsAttachmentInTfs(tmRequestContext, attachmentId, projectName) || this.TcmHttpClient == null)
            return false;
          testAttachment = this.TcmHttpClient.GetTestResultAttachmentContentAsync(projectName, attachmentId)?.Result;
          return true;
        })) ? 1 : 0;
        tcmAttachment = testAttachment;
        return num != 0;
      }
    }

    public bool TryResetTestResults(
      IVssRequestContext requestContext,
      string projectName,
      LegacyTestCaseResultIdentifier[] ids,
      out LegacyTestCaseResult[] response)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName("TryUpdateTestRunLegacy", "Tcm")))
      {
        LegacyTestCaseResult[] responseFromRemote = (LegacyTestCaseResult[]) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsImpersonating())
          {
            LegacyTestCaseResultIdentifier[] source = ids;
            if ((source != null ? (((IEnumerable<LegacyTestCaseResultIdentifier>) source).Any<LegacyTestCaseResultIdentifier>() ? 1 : 0) : 0) != 0 && this.TcmHttpClient != null)
            {
              TcmHttpClient tcmHttpClient = this.TcmHttpClient;
              ResetTestResultsRequest request = new ResetTestResultsRequest();
              request.Ids = ids;
              request.ProjectName = projectName;
              CancellationToken cancellationToken = new CancellationToken();
              responseFromRemote = tcmHttpClient.ResetTestResultsAsync(request, cancellationToken: cancellationToken)?.Result;
              return true;
            }
          }
          return false;
        })) ? 1 : 0;
        response = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryAbortTestRun(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      int revision,
      int options,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryAbortTestRun), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties responseFromRemote = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, false) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          AbortTestRunRequest request = new AbortTestRunRequest();
          request.ProjectName = projectName;
          request.TestRunId = testRunId;
          request.Revision = revision;
          request.Options = options;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.AbortTestRunAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        updatedProperties = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestRunStats(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      out List<LegacyTestRunStatistic> testRunStats)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestRunStats), "Tcm")))
      {
        List<LegacyTestRunStatistic> responseFromRemote = (List<LegacyTestRunStatistic>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, testRunId, false) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestRunStatsRequest request = new QueryTestRunStatsRequest();
          request.TeamProjectName = projectName;
          request.TestRunId = testRunId;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryTestRunStatsAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        testRunStats = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryByPoint(
      IVssRequestContext requestContext,
      string projectName,
      int planId,
      int pointId,
      out List<LegacyTestCaseResult> resultsFromRemote)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName("TryQueryTestRunStats", "Tcm")))
      {
        List<LegacyTestCaseResult> responseFromRemote = (List<LegacyTestCaseResult>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryByPointRequest request = new QueryByPointRequest();
          request.ProjectName = projectName;
          request.TestPlanId = planId;
          request.TestPointId = pointId;
          CancellationToken cancellationToken = new CancellationToken();
          responseFromRemote = tcmHttpClient.QueryByPointAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        resultsFromRemote = responseFromRemote;
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
          if (this.IsImpersonating() || !this.IsTestRunInTCM(requestContext, runId, true) || this.TestResultsHttpClient == null)
            return false;
          responseFromRemote = this.TestResultsHttpClient.GetTestRunMessageLogsAsync(projectId, runId)?.Result;
          return true;
        })) ? 1 : 0;
        testMessageLogs = responseFromRemote;
        return num != 0;
      }
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.m_requestContext.GetClient<TestResultsHttpClient>();

    internal TestResultsHttpClient TestResultsHttpClientWithReadOnly(string featureFlag) => this.m_requestContext.GetClient<TestResultsHttpClient>(this.m_requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel(featureFlag));

    internal TcmHttpClient TcmHttpClient => this.m_requestContext.GetClient<TcmHttpClient>();

    public TcmHttpClient TcmHttpClientWithReadOnly(string featureFlag) => this.m_requestContext.GetClient<TcmHttpClient>(this.m_requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel(featureFlag));

    internal Guid ExemptedS2SPrincipal
    {
      get => !(this.m_exemptedS2SPrincipal == Guid.Empty) ? this.m_exemptedS2SPrincipal : TestManagementServerConstants.TFSPrincipal;
      set => this.m_exemptedS2SPrincipal = value;
    }

    private bool ShouldCreateTestRunInTcm(Guid projectId, RunCreateModel runCreateModel) => true;

    private bool ShouldCreateTestRunInTcm(
      string projectName,
      CreateTestRunRequest createTestRunRequest)
    {
      return true;
    }

    public bool IsTestRunInTCM(IVssRequestContext requestContext, int runId, bool queryFlow = true) => TCMServiceDataMigrationRestHelper.IsMigrationCompleted(requestContext) || !this.IsTestRunInTfs(runId, queryFlow);

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

    private bool IsAttachmentInTfs(
      TestManagementRequestContext context,
      int attachmentId,
      string projectName,
      bool queryFlow = true)
    {
      if (context.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun"))
        return false;
      IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = this.m_requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestAttachmentIdThreshold";
      ref RegistryQuery local1 = ref registryQuery;
      int num = service.GetValue<int>(requestContext1, in local1, int.MaxValue);
      IVssRequestContext requestContext2 = this.m_requestContext;
      registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceDataImportAccount";
      ref RegistryQuery local2 = ref registryQuery;
      if (service.GetValue<bool>(requestContext2, in local2, false) && this.m_requestContext.ExecutionEnvironment.IsHostedDeployment && attachmentId < num)
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        return TestManagementServiceUtility.getSessionIdByAttachmentId(context, attachmentId, projectFromName.GuidId) > 0;
      }
      return num != int.MaxValue && attachmentId < num || attachmentId % 2 != 0;
    }

    private static bool TryCompatClientRequest<T>(
      IVssRequestContext requestContext,
      Func<T> action,
      out T result)
    {
      try
      {
        result = action();
        return true;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException is VssResourceNotFoundException)
        {
          requestContext.TraceException(1015661, "TestManagement", "BusinessLayer", (Exception) ex);
          result = default (T);
        }
        else
          throw;
      }
      return false;
    }

    private bool IsImpersonating() => this.m_requestContext.IsImpersonating && this.m_requestContext.GetAuthenticatedId() != this.ExemptedS2SPrincipal;

    private bool InvokeAction(Func<bool> func)
    {
      try
      {
        return func();
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
