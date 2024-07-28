// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementRunService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementRunService))]
  public interface ITeamFoundationTestManagementRunService : IVssFrameworkService
  {
    TestRun CreateTestRun(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      TeamProjectReference teamProject,
      IList<TestCaseResult> testCaseResults,
      bool populateRowDataCount,
      bool invokvedViaRestApi = false);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      TestManagementRequestContext requestContext,
      string projectId,
      RunCreateModel testRun);

    TestRun GetTestRunById(
      TestManagementRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject,
      bool includeTags = false);

    TestRun FetchTestRun(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunSummaryById(
      TestManagementRequestContext requestContext,
      int testRunId,
      ProjectInfo projectInfo);

    TestRun UpdateTestRun(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      TeamProjectReference teamProject,
      bool deleteInProgressTestResults = false,
      bool skipRunStateTransitionCheck = false);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      TestManagementRequestContext requestContext,
      TeamProjectReference teamProject,
      int testRunId,
      RunUpdateModel runUpdateModel);

    List<TestRun> QueryInMultipleProjects(
      TestManagementRequestContext context,
      ResultsStoreQuery query);

    List<TestMessageLogEntry> QueryTestRunLogs(
      TestManagementRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject);

    List<TestRun> QueryTestRuns(TestManagementRequestContext context, ResultsStoreQuery query);

    List<int> QueryTestRunIds(TestManagementRequestContext context, ResultsStoreQuery query);

    void AbortRunsAssociatedWithRelease(
      TestManagementRequestContext context,
      Guid teamProject,
      TestRunState state,
      int releaseId,
      int releaseEnvId);

    string GetTestRunWebAccessUrl(
      TestManagementRequestContext requestContext,
      int runId,
      string projectName);

    TestRunsByFilter GetTestRunsByFilter(
      TestManagementRequestContext requestContext,
      string projectId,
      UrlHelper Url,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state,
      IList<int> planIds,
      bool? isAutomated,
      TestRunPublishContext? publishContext,
      IList<int> buildIds,
      IList<int> buildDefIds,
      string branchName,
      IList<int> releaseIds,
      IList<int> releaseDefIds,
      IList<int> releaseEnvIds,
      IList<int> releaseEnvDefIds,
      string runTitle,
      int top,
      string continuationToken);

    List<TestRunRecord> QueryTestRunsByChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource);

    List<int> QueryTestRunIdsByChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource);

    void UpdateTestRunSummaryForNonConfigRuns(
      TestManagementRequestContext context,
      TeamProjectReference teamProject,
      int testRunId,
      IList<RunSummaryModel> runSummaryModel,
      byte runType);

    Task<List<TestTag>> UpdateTestRunTags(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      int testRunId,
      TestTagsUpdateModel testTagsUpdateModel);
  }
}
