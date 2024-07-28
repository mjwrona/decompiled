// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementRunService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementRunService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementRunService,
    IVssFrameworkService
  {
    private const string c_testRunUriFormat = "{0}{1}/_TestManagement/Runs?runId={2}&_a=runCharts";
    private const int c_TestRunsQueryBatchSize = 100000;
    private const int c_MaxTestRunsQueryIterationCount = 10;

    public TeamFoundationTestManagementRunService()
    {
    }

    public TeamFoundationTestManagementRunService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public virtual TestRun GetTestRunById(
      TestManagementRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject,
      bool includeTags = false)
    {
      TestRun testRun = (TestRun) null;
      using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TeamFoundationTestManagementRunService.GetTestRunById"))
        testRun = this.ExecuteAction<TestRun>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestRunById", (Func<TestRun>) (() =>
        {
          ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject), "Test Results");
          List<TestRun> source = TestRun.Query(requestContext, testRunId, Guid.Empty, string.Empty, teamProject.Name);
          if (source.Count == 1)
          {
            requestContext.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementRunService.GetTestRunById :: Got test run details. TestRunId = {0}, State = {1}", (object) source[0].TestRunId, (object) source[0].State);
            return source.First<TestRun>();
          }
          Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.TestRunNotFound, (object) testRunId));
          requestContext.TraceError("BusinessLayer", "Error occurred in TeamFoundationTestManagementRunService.GetTestRunById. Error = {0}.", (object) notFoundException.Message);
          throw notFoundException;
        }), 1015090, "TestResultsInsights");
      if (testRun != null & includeTags)
      {
        ProjectInfo projectFromGuid = requestContext.ProjectServiceHelper.GetProjectFromGuid(teamProject.Id);
        TeamFoundationTestManagementRunService.AddTestTagToRun(requestContext, projectFromGuid, testRun);
      }
      return testRun;
    }

    public TestRun FetchTestRun(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.FetchTestRun(WithRunIdAndProjctName)"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        context.TraceVerbose("BusinessLayer", "TestRun.Query:: Querying test run. testRunId = {0}. ProjectName = {1}", (object) testRunId, (object) teamProjectName);
        context.SecurityManager.CheckViewTestResultsPermission(context, projectFromName.String);
        Dictionary<int, string> iterationMap = new Dictionary<int, string>();
        Dictionary<Guid, List<TestRun>> projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        List<TestRun> testRunList;
        using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
          testRunList = replicaAwareComponent.QueryTestRuns(testRunId, owner, buildUri, projectFromName.GuidId, out iterationMap, out projectsRunsMap, planId, skip, top);
        TestRun.UpdateProjectDataForRuns(context, projectsRunsMap);
        TestRun.UpdateIterationPathsForRuns(context, iterationMap, testRunList);
        TestRunBase.ResolveUserNames<TestRun>(context, testRunList);
        if (testRunList != null && testRunList.Count > 0 && testRunList[0] != null)
          testRunList[0].ServiceVersion = TestManagementHost.ServerVersion.ToString();
        testRunList.ForEach((Action<TestRun>) (r => r.Comment = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, r, "Comment", r.Comment)));
        testRunList.ForEach((Action<TestRun>) (r => r.ErrorMessage = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, r, "ErrorMessage", r.ErrorMessage)));
        context.TraceVerbose("BusinessLayer", "TestRun.Query:: Queried test runs successfully. TestRunsCount = {0}. TestRunId = {1} ProjectName = {2}", (object) testRunList.Count, (object) testRunId, (object) teamProjectName);
        if (testRunList.Count == 1)
        {
          context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementRunService.GetTestRunById :: Got test run details. TestRunId = {0}, State = {1}", (object) testRunList[0].TestRunId, (object) testRunList[0].State);
          return testRunList.First<TestRun>();
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.TestRunNotFound, (object) testRunId));
        context.TraceError("BusinessLayer", "Error occurred in TeamFoundationTestManagementRunService.GetTestRunById. Error = {0}.", (object) notFoundException.Message);
        throw notFoundException;
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      TestManagementRequestContext requestContext,
      string projectId,
      RunCreateModel testRun)
    {
      DataContractConverter contractConverter = new DataContractConverter(requestContext);
      TestRun run;
      List<TestCaseResult> testCaseResults;
      TeamProjectReference projectReference;
      contractConverter.Populate(projectId, testRun, out run, out testCaseResults, out projectReference);
      return contractConverter.ConvertTestRunToDataContract(this.CreateTestRun(requestContext, run, projectReference, (IList<TestCaseResult>) testCaseResults, false, true), projectReference, true, false, false);
    }

    public TestRun CreateTestRun(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      TeamProjectReference teamProject,
      IList<TestCaseResult> testCaseResults,
      bool populateRowDataCount,
      bool invokvedViaRestApi = false)
    {
      return this.ExecuteAction<TestRun>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.CreateTestRun", (Func<TestRun>) (() =>
      {
        ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
        ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject), "Test Results");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(requestContext, teamProject.Name);
        ProjectInfo projectFromGuid = requestContext.ProjectServiceHelper.GetProjectFromGuid(teamProject.Id);
        TeamFoundationTestManagementRunService._validateTestTagsForRun(requestContext, projectFromGuid, testRun);
        bool flag = requestContext.IsFeatureEnabled("TestManagement.Server.EnableUserCustomFieldsInTestRun");
        if (testRun.CustomFields != null && testRun.CustomFields.Any<TestExtensionField>())
        {
          IList<TestExtensionFieldDetails> extensionFieldDetailsList = requestContext.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(requestContext, projectFromName.GuidId, scopeFilter: CustomTestFieldScope.TestRun | CustomTestFieldScope.System);
          List<string> stringList = new List<string>();
          foreach (TestExtensionField customField in testRun.CustomFields)
          {
            TestExtensionFieldDetails extensionFieldDetails1 = (TestExtensionFieldDetails) null;
            foreach (TestExtensionFieldDetails extensionFieldDetails2 in (IEnumerable<TestExtensionFieldDetails>) extensionFieldDetailsList)
            {
              if (string.Equals(extensionFieldDetails2.Name, customField.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                extensionFieldDetails1 = extensionFieldDetails2;
                break;
              }
            }
            if (extensionFieldDetails1 != null)
            {
              if (flag)
                this.ValidateSizeOfCustomFieldValue(requestContext, projectFromName.ToString(), extensionFieldDetails1.Type, extensionFieldDetails1.Name, customField.Value);
              customField.Field = new TestExtensionFieldDetails()
              {
                Id = extensionFieldDetails1.Id,
                Name = extensionFieldDetails1.Name,
                Type = extensionFieldDetails1.Type
              };
            }
            else
              stringList.Add(customField.Field.Name);
          }
          if (stringList.Any<string>())
            throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.ExtensionFieldsNotFoundError, (object) string.Join(",", (IEnumerable<string>) stringList)));
        }
        TestRun testRun1 = testRun.Create(requestContext, (TestSettings) null, testCaseResults.ToArray<TestCaseResult>(), populateRowDataCount, teamProject.Name, invokvedViaRestApi);
        if (testRun.Tags != null && projectFromGuid != null)
        {
          using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TestRun.CreateTag"))
            testRun1.Tags = TeamFoundationTestManagementRunService._createTestTagForRun(requestContext, projectFromGuid, testRun1, (IList<TestTag>) testRun.Tags);
          this.AddTelemetryForTag(requestContext, projectFromGuid.Id, testRun1, (IList<TestTag>) testRun.Tags);
        }
        if (((testRun.CustomFields == null ? 0 : (testRun.CustomFields.Any<TestExtensionField>() ? 1 : 0)) & (flag ? 1 : 0)) != 0)
          this.AddTelemetryForCustomTestFields(requestContext, teamProject.Id, testRun);
        return testRun1;
      }), 1015090, "TestResultsInsights");
    }

    private int GetSqlSizeOfCustomFieldValueInBytes(object value, SqlDbType definedDataType)
    {
      switch (definedDataType)
      {
        case SqlDbType.Bit:
          return 1;
        case SqlDbType.DateTime:
          return 4;
        case SqlDbType.Float:
          return 4;
        case SqlDbType.Int:
          return 4;
        case SqlDbType.UniqueIdentifier:
          return 16;
        default:
          return Convert.ToString(value).Length * 2;
      }
    }

    public TestRun UpdateTestRun(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      TeamProjectReference teamProject,
      bool deleteInProgressTestResults = false,
      bool skipRunStateTransitionCheck = false)
    {
      return this.ExecuteAction<TestRun>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.UpdateTestRun", (Func<TestRun>) (() => this.UpdateTestRunInternal(requestContext, testRun, teamProject, deleteInProgressTestResults, skipRunStateTransitionCheck)), 1015090, "TestResultsInsights");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      TestManagementRequestContext requestContext,
      TeamProjectReference teamProject,
      int testRunId,
      RunUpdateModel runUpdateModel)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.UpdateTestRunWithModel", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() =>
      {
        DataContractConverter contractConverter = new DataContractConverter(requestContext);
        TestRun testRunById = this.GetTestRunById(requestContext, testRunId, teamProject, false);
        if (testRunById == null)
          throw new TestObjectNotFoundException(requestContext.RequestContext, testRunId, ObjectTypes.TestRun);
        if (runUpdateModel == null)
          return contractConverter.ConvertTestRunToDataContract(testRunById, teamProject, true, false, false);
        contractConverter.ValidateTestSettingsFromUpdateModel(runUpdateModel);
        TestRun testRun1 = contractConverter.UpdateRunDetailsFromModel(testRunById, runUpdateModel);
        bool enableCustomFields = requestContext.IsFeatureEnabled("TestManagement.Server.EnableUserCustomFieldsInTestRun");
        if (enableCustomFields)
        {
          testRun1 = this.UpdateCustomTestFields(requestContext, testRun1, runUpdateModel, teamProject);
          this.AddTelemetryForCustomTestFields(requestContext, teamProject.Id, testRun1);
        }
        ReleaseReference releaseRef = (ReleaseReference) null;
        if (!string.IsNullOrEmpty(runUpdateModel.ReleaseEnvironmentUri) && !string.IsNullOrEmpty(runUpdateModel.ReleaseUri))
          releaseRef = new ReleaseServiceHelper().QueryReleaseReferenceByUri(requestContext.RequestContext, new GuidAndString(teamProject.Url, teamProject.Id), runUpdateModel.ReleaseUri, runUpdateModel.ReleaseEnvironmentUri);
        BuildConfiguration buildRef = (BuildConfiguration) null;
        if (runUpdateModel.Build != null)
        {
          int result1 = 0;
          if (int.TryParse(runUpdateModel.Build.Id, out result1))
          {
            Guid result2;
            if (releaseRef != null && result1 == releaseRef.PrimaryArtifactBuildId && releaseRef.PrimaryArtifactProjectId != null && Guid.TryParse(releaseRef.PrimaryArtifactProjectId, out result2) && result2 != Guid.Empty)
            {
              buildRef = this.BuildServiceHelper.QueryBuildConfigurationById(requestContext.RequestContext, result2, result1);
            }
            else
            {
              buildRef = this.BuildServiceHelper.QueryBuildConfigurationById(requestContext.RequestContext, teamProject.Id, result1);
              if (buildRef == null)
                throw new ArgumentException(string.Format(ServerResources.BuildNotFound, (object) result1));
            }
            if (buildRef != null)
            {
              buildRef.BuildPlatform = runUpdateModel.BuildPlatform != null ? runUpdateModel.BuildPlatform : string.Empty;
              buildRef.BuildFlavor = runUpdateModel.BuildFlavor != null ? runUpdateModel.BuildFlavor : string.Empty;
            }
          }
        }
        bool? inProgressResults = runUpdateModel.DeleteInProgressResults;
        int num;
        if (!inProgressResults.HasValue)
        {
          num = 0;
        }
        else
        {
          inProgressResults = runUpdateModel.DeleteInProgressResults;
          num = inProgressResults.Value ? 1 : 0;
        }
        bool deleteInProgressTestResults = num != 0;
        TestRun testRun2 = this.UpdateTestRunInternal(requestContext, testRun1, teamProject, deleteInProgressTestResults, true, releaseRef: releaseRef, buildRef: buildRef, invokvedViaRestApi: true, enableCustomFields: enableCustomFields);
        return contractConverter.ConvertTestRunToDataContract(testRun2, teamProject, true, false, false);
      }), 1015090, "TestResultsInsights");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunSummaryById(
      TestManagementRequestContext requestContext,
      int testRunId,
      ProjectInfo projectInfo)
    {
      requestContext.TraceInfo("RestLayer", "TeamFoundationTestManagementRunService.GetTestRunSummaryById projectId = {0}, runId = {1}", (object) projectInfo.Id, (object) testRunId);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestRunSummaryById", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() =>
      {
        this.CheckForViewTestResultPermission(requestContext, projectInfo.Name);
        TestRun testRunById = this.GetTestRunById(requestContext, testRunId, new TeamProjectReference()
        {
          Name = projectInfo.Name
        }, false);
        if (testRunById != null & testRunById.State != (byte) 3)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.RunSummaryWhenTestRunIsNotCompletedException));
        return this.GetTestSummaryForRun(requestContext, projectInfo, testRunById);
      }), 1015090, "TestResultsInsights");
    }

    public List<TestRun> QueryInMultipleProjects(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      return this.ExecuteAction<List<TestRun>>(context.RequestContext, "TeamFoundationTestManagementRunService.QueryInMultipleProjects", (Func<List<TestRun>>) (() =>
      {
        context.RequestContext.Trace(1015090, TraceLevel.Verbose, "TestResultsInsights", "RestLayer", "QueryText: " + query.QueryText);
        return TestRun.QueryInMultipleProjects(context, query);
      }), 1015090, "TestResultsInsights");
    }

    public List<TestRun> QueryTestRuns(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      return this.ExecuteAction<List<TestRun>>(context.RequestContext, "TeamFoundationTestManagementRunService.QueryTestRuns", (Func<List<TestRun>>) (() =>
      {
        context.RequestContext.Trace(1015090, TraceLevel.Verbose, "TestResultsInsights", "RestLayer", "QueryText: " + query.QueryText);
        return TestRun.Query(context, query);
      }), 1015090, "TestResultsInsights");
    }

    public List<int> QueryTestRunIds(TestManagementRequestContext context, ResultsStoreQuery query) => this.ExecuteAction<List<int>>(context.RequestContext, "TeamFoundationTestManagementRunService.QueryTestRunIds", (Func<List<int>>) (() =>
    {
      context.RequestContext.Trace(1015090, TraceLevel.Verbose, "TestResultsInsights", "RestLayer", "QueryText: " + query.QueryText);
      return TestRun.QueryTestRunIds(context, query);
    }), 1015090, "TestResultsInsights");

    public List<TestMessageLogEntry> QueryTestRunLogs(
      TestManagementRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject)
    {
      return this.ExecuteAction<List<TestMessageLogEntry>>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.QueryTestRunLogs", (Func<List<TestMessageLogEntry>>) (() =>
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject), "Test Results");
        return TestRun.QueryLogEntriesForRun(requestContext, testRunId, 0, teamProject.Name);
      }), 1015090, "TestResultsInsights");
    }

    public void AbortRunsAssociatedWithRelease(
      TestManagementRequestContext requestContext,
      Guid teamProject,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState state,
      int releaseId,
      int releaseEnvId)
    {
      this.ExecuteAction(requestContext.RequestContext, "TeamFoundationTestManagementRunService.AbortRunsAssociatedWithRelease", (Action) (() =>
      {
        List<IdAndRev> idAndRevList = new List<IdAndRev>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          idAndRevList = managementDatabase.GetRunsAssociatedWithRelease(teamProject, (byte) state, releaseId, releaseEnvId);
        if (idAndRevList.Count <= 0)
          return;
        requestContext.SecurityManager.CheckPublishTestResultsPermission(requestContext, requestContext.ProjectServiceHelper.GetProjectFromGuid(teamProject).Uri);
        Guid teamFoundationId = requestContext.UserTeamFoundationId;
        foreach (IdAndRev idAndRev in idAndRevList)
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
            managementDatabase.AbortTestRun(teamProject, idAndRev.Id, idAndRev.Revision, TestRunAbortOptions.None, (byte) 0, teamFoundationId, out string _, out Guid _, out TestRun _);
        }
      }), 1015090, "TestResultsInsights");
    }

    public string GetTestRunWebAccessUrl(
      TestManagementRequestContext requestContext,
      int runId,
      string projectName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/_TestManagement/Runs?runId={2}&_a=runCharts", (object) new TestManagementServiceUtility(requestContext).GetTeamProjectCollectionUrl(), (object) Uri.EscapeDataString(projectName), (object) runId);
    }

    public TestRunsByFilter GetTestRunsByFilter(
      TestManagementRequestContext requestContext,
      string projectId,
      UrlHelper Url,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState? state,
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
      string continuationToken)
    {
      requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementRunService.GetTestRunsByFilter projectId = {0}, top = {1}", (object) projectId, (object) top);
      using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TeamFoundationTestManagementRunService.GetTestRunsByFilter", isTopLevelScenario: true))
      {
        DateTime continuationTokenLastUpdated = DateTime.MaxValue;
        IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunList = this.ExecuteAction<IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestRunsByFilter", (Func<IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(requestContext.RequestContext, projectId);
          requestContext.SecurityManager.CheckViewTestResultsPermission(requestContext, requestContext.ProjectServiceHelper.GetProjectFromGuid(projectReference.Id).Uri);
          if (!string.IsNullOrEmpty(continuationToken))
            QueryTestRunsFilter.ParseContinuationToken(continuationToken, out minLastUpdatedDate);
          this.ValidateInputParamForQueryTestRun(requestContext, minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle, top);
          QueryTestRunsFilter testRunFilter = this.PrepareQueryFilterForQueryTestRun(minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle);
          int batchSize = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestRunsQueryBatchSize", 100000);
          List<TestRun> testRuns = TestRun.QueryTestRuns(requestContext, projectReference.Id, testRunFilter, top, batchSize, out continuationTokenLastUpdated);
          return this.GetTestRunResponse(requestContext, projectReference, testRuns);
        }), 1015090, "TestResultsInsights", "BusinessLayer");
        TestRunsByFilter testRunsByFilter = new TestRunsByFilter()
        {
          TestRuns = testRunList,
          ContinuationToken = string.Empty
        };
        if (continuationTokenLastUpdated != DateTime.MaxValue)
          testRunsByFilter.ContinuationToken = QueryTestRunsFilter.GetContinuationToken(continuationTokenLastUpdated);
        else if (testRunsByFilter.TestRuns != null && testRunsByFilter.TestRuns.Count >= top)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = testRunsByFilter.TestRuns[testRunsByFilter.TestRuns.Count - 1];
          testRunsByFilter.ContinuationToken = QueryTestRunsFilter.GetContinuationToken(testRun.LastUpdatedDate);
        }
        return testRunsByFilter;
      }
    }

    public List<TestRunRecord> QueryTestRunsByChangedDate(
      TestManagementRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource source = TestArtifactSource.Tfs)
    {
      string str = "TeamFoundationTestManagementRunService.QueryTestRunsByChangedDate";
      try
      {
        requestContext.RequestContext.TraceEnter(1015090, "TestResultsInsights", "BusinessLayer", str);
        string prBranchName = (string) null;
        if (this.IsPRTestDataPublishDisabledToAXService(requestContext.RequestContext))
          prBranchName = "refs/pull/*";
        using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(requestContext.RequestContext))
            return replicaAwareComponent.QueryTestRunsByChangedDate(projectId, batchSize, prBranchName, fromDate, out toDate, source);
        }
      }
      finally
      {
        requestContext.RequestContext.TraceLeave(1015090, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public List<int> QueryTestRunIdsByChangedDate(
      TestManagementRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementRunService.QueryTestRunIdsByChangedDate";
      try
      {
        requestContext.RequestContext.TraceEnter(1015090, "TestResultsInsights", "BusinessLayer", str);
        string prBranchName = (string) null;
        if (this.IsPRTestDataPublishDisabledToAXService(requestContext.RequestContext))
          prBranchName = "refs/pull/*";
        using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(requestContext.RequestContext))
            return replicaAwareComponent.QueryTestRunIdsByChangedDate(projectId, batchSize, prBranchName, fromDate, out toDate, dataSource);
        }
      }
      finally
      {
        requestContext.RequestContext.TraceLeave(1015090, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public void UpdateTestRunSummaryForNonConfigRuns(
      TestManagementRequestContext context,
      TeamProjectReference teamProject,
      int testRunId,
      IList<RunSummaryModel> runSummaryModel,
      byte runType)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementRunService.UpdateTestRunSummaryForNonConfigRuns"))
      {
        if (runSummaryModel == null || !runSummaryModel.Any<RunSummaryModel>() || runType != (byte) 32 || !context.IsFeatureEnabled("TestManagement.Server.NonConfigRun"))
          return;
        List<RunSummaryByOutcome> runSummaryByOutcomes;
        new DataContractConverter(context).PopulateRunSummaryByRunSummaryByOutcomeModel((IList<RunSummaryModel>) runSummaryModel.ToList<RunSummaryModel>(), out runSummaryByOutcomes);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
          managementDatabase.UpdateTestRunSummaryForNonConfigRuns(teamProject.Id, testRunId, (IEnumerable<RunSummaryByOutcome>) runSummaryByOutcomes);
      }
    }

    public async Task<List<TestTag>> UpdateTestRunTags(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      int testRunId,
      TestTagsUpdateModel testTagsUpdateModel)
    {
      TeamFoundationTestManagementRunService managementRunService = this;
      List<TestTag> testTagList = new List<TestTag>();
      ArgumentUtility.CheckForNull<TestTagsUpdateModel>(testTagsUpdateModel, nameof (testTagsUpdateModel), "Test Results");
      ArgumentUtility.CheckForNull<IList<KeyValuePair<OperationType, IList<TestTag>>>>(testTagsUpdateModel.Tags, "Tags", "Test Results");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) testTagsUpdateModel.Tags, "Tags", "Test Results");
      ArgumentUtility.CheckCollectionForMaxLength<KeyValuePair<OperationType, IList<TestTag>>>((ICollection<KeyValuePair<OperationType, IList<TestTag>>>) testTagsUpdateModel.Tags, "Tags", 2);
      IList<TestTag> tagsToBeAdded = (IList<TestTag>) null;
      IList<TestTag> tagsToBeDeleted = (IList<TestTag>) null;
      foreach (KeyValuePair<OperationType, IList<TestTag>> tag in (IEnumerable<KeyValuePair<OperationType, IList<TestTag>>>) testTagsUpdateModel.Tags)
      {
        if (tag.Key == OperationType.Add)
          tagsToBeAdded = tag.Value;
        else
          tagsToBeDeleted = tag.Key == OperationType.Delete ? tag.Value : throw new InvalidPropertyException(nameof (testTagsUpdateModel), ServerResources.InvalidPropertyMessage);
      }
      bool flag = TeamFoundationTestManagementRunService._isTestTagSupported(requestContext);
      if (((tagsToBeDeleted != null ? 1 : (tagsToBeAdded != null ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        TeamProjectReference projectReference = managementRunService.ToTeamProjectReference(requestContext.RequestContext, projectInfo);
        return TeamFoundationTestManagementRunService._secureTestTagObject(await TeamFoundationTestManagementRunService._updateTestTag(requestContext, projectInfo, managementRunService.GetTestRunById(requestContext, testRunId, projectReference, false), tagsToBeAdded, tagsToBeDeleted).ConfigureAwait(false), projectInfo.Id);
      }
      if (!flag)
        throw new ArgumentException(ServerResources.TestTagForRunNotSupported);
      throw new InvalidPropertyException(nameof (testTagsUpdateModel), ServerResources.InvalidPropertyMessage);
    }

    private void ValidateSizeOfCustomFieldValue(
      TestManagementRequestContext requestContext,
      string projectId,
      SqlDbType definedDataType,
      string fieldName,
      object value)
    {
      int fieldValueInBytes = this.GetSqlSizeOfCustomFieldValueInBytes(value, definedDataType);
      string cacheContainerName = projectId;
      string propertyName = "TestRunCustomFieldValueMaxSizeInBytes";
      string propertyFromCache = CommonHelper.GetPropertyFromCache(requestContext.RequestContext, projectId, propertyName, "40b9310a-8b38-4129-9067-357cf33e1fd2", TimeSpan.FromHours(12.0));
      int num;
      if (propertyFromCache != null && propertyFromCache != string.Empty)
      {
        num = int.Parse(propertyFromCache);
      }
      else
      {
        num = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestRunCustomFieldValueMaxSizeInBytes", 2000);
        CommonHelper.SetPropertyInCache(requestContext.RequestContext, cacheContainerName, propertyName, num.ToString(), "40b9310a-8b38-4129-9067-357cf33e1fd2", TimeSpan.FromHours(12.0));
      }
      if (fieldValueInBytes > num)
      {
        requestContext.RequestContext.Trace(1015971, TraceLevel.Error, "TestManagement", "RestLayer", "Custom Field size excceded for the field :{0} with size {1} in bytes", (object) fieldName, (object) fieldValueInBytes);
        throw new TestObjectSizeExceededException("Custom Field:" + fieldName + " exceeded its size which has " + fieldValueInBytes.ToString() + "bytes");
      }
    }

    private TestRun UpdateCustomTestFields(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      RunUpdateModel runUpdateModel,
      TeamProjectReference teamProject)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(requestContext, teamProject.Name);
      if (runUpdateModel.CustomTestFields != null && runUpdateModel.CustomTestFields.Any<CustomTestField>())
      {
        IList<TestExtensionFieldDetails> source1 = requestContext.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(requestContext, projectFromName.GuidId, scopeFilter: CustomTestFieldScope.TestRun | CustomTestFieldScope.System);
        List<string> stringList = new List<string>();
        List<TestExtensionField> source2 = new List<TestExtensionField>();
        foreach (CustomTestField customTestField1 in runUpdateModel.CustomTestFields)
        {
          CustomTestField customTestField = customTestField1;
          TestExtensionFieldDetails extensionFieldDetails = source1.FirstOrDefault<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, customTestField.FieldName, StringComparison.OrdinalIgnoreCase)));
          if (extensionFieldDetails != null)
          {
            this.ValidateSizeOfCustomFieldValue(requestContext, teamProject.Id.ToString(), extensionFieldDetails.Type, extensionFieldDetails.Name, customTestField.Value);
            source2.Add(new TestExtensionField()
            {
              Field = new TestExtensionFieldDetails()
              {
                Id = extensionFieldDetails.Id,
                Name = extensionFieldDetails.Name,
                Type = extensionFieldDetails.Type
              },
              Value = customTestField.Value
            });
          }
          else
            stringList.Add(customTestField.FieldName);
        }
        if (stringList.Any<string>())
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.ExtensionFieldsNotFoundError, (object) string.Join(",", (IEnumerable<string>) stringList)));
        if (testRun.CustomFields != null && testRun.CustomFields.Any<TestExtensionField>())
        {
          foreach (TestExtensionField testExtensionField1 in source2)
          {
            TestExtensionField testExtensionField2 = (TestExtensionField) null;
            foreach (TestExtensionField customField in testRun.CustomFields)
            {
              if (string.Equals(customField.Field.Name, testExtensionField1.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                testExtensionField2 = customField;
                break;
              }
            }
            if (testExtensionField2 != null)
              testExtensionField2.Value = testExtensionField1.Value;
          }
          testRun.CustomFields.Add<List<TestExtensionField>, TestExtensionField>((IEnumerable<TestExtensionField>) source2);
        }
        else
        {
          TestRun testRun1 = testRun;
          List<TestExtensionField> destination = new List<TestExtensionField>();
          destination.Add<List<TestExtensionField>, TestExtensionField>((IEnumerable<TestExtensionField>) source2);
          testRun1.CustomFields = destination;
        }
      }
      return testRun;
    }

    private TestExtensionField CheckIfProvidedCustomFieldExists(
      List<TestExtensionField> testExtensionFields,
      string customFieldName)
    {
      foreach (TestExtensionField testExtensionField in testExtensionFields)
      {
        if (string.Equals(testExtensionField.Field.Name, customFieldName, StringComparison.OrdinalIgnoreCase))
          return testExtensionField;
      }
      return (TestExtensionField) null;
    }

    private TestRun UpdateTestRunInternal(
      TestManagementRequestContext requestContext,
      TestRun testRun,
      TeamProjectReference teamProject,
      bool deleteInProgressTestResults = false,
      bool skipRunStateTransitionCheck = false,
      TestSettings settings = null,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool invokvedViaRestApi = false,
      bool enableCustomFields = false)
    {
      ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
      ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject), "Test Results");
      requestContext.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementRunService.UpdateTestRun invoked with run details: {0}", (object) testRun.ToString());
      ProjectInfo projectFromGuid = requestContext.ProjectServiceHelper.GetProjectFromGuid(teamProject.Id);
      TeamFoundationTestManagementRunService._validateTestTagsForRun(requestContext, projectFromGuid, testRun, true);
      List<TestTag> tags = testRun.Tags;
      try
      {
        if (testRun.TestMessageLogEntries != null)
        {
          this.CreateLogEntriesForRun(requestContext, testRun.TestRunId, testRun.TestMessageLogEntries, teamProject.Name);
          if (testRun.TestMessageLogId == 0)
          {
            TestRun testRunById = this.GetTestRunById(requestContext, testRun.TestRunId, teamProject, false);
            testRun.TestMessageLogId = testRunById.TestMessageLogId;
            requestContext.TraceVerbose("BusinessLayer", "TestMessageLogId is {0} for test run id {1}", (object) testRun.TestMessageLogId, (object) testRun.TestRunId);
          }
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState testRunState = Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState.Unspecified;
        if (this.IsRunCompletionRequested(testRun))
        {
          testRunState = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) testRun.State;
          testRun.State = (byte) 0;
        }
        new UpdatedProperties().Revision = -2;
        if (testRun.Update(requestContext, teamProject.Name, skipRunStateTransitionCheck, settings, releaseRef, buildRef, invokvedViaRestApi, enableCustomFields).Revision == -1)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException(string.Format(ServerResources.TestRunAlreadyUpdated, (object) testRun.TestRunId));
        if (testRunState != Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState.Unspecified)
        {
          testRun = this.GetTestRunById(requestContext, testRun.TestRunId, teamProject, false);
          byte state = testRun.State;
          testRun.State = (byte) testRunState;
          this.HandleTestRunStateUpdate(requestContext, teamProject, testRun, state, deleteInProgressTestResults, skipRunStateTransitionCheck);
        }
      }
      catch (TestObjectNotFoundException ex)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.TestRunNotFound, (object) testRun.TestRunId));
        requestContext.TraceError("BusinessLayer", "Error occurred in TeamFoundationTestManagementRunService.UpdateTestRun. Error = {0}.", (object) notFoundException.Message);
        throw notFoundException;
      }
      TestRun testRunById1 = this.GetTestRunById(requestContext, testRun.TestRunId, teamProject, false);
      if (tags != null && projectFromGuid != null)
      {
        using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", "TestRun.CreateTag"))
          testRunById1.Tags = TeamFoundationTestManagementRunService._createTestTagForRun(requestContext, projectFromGuid, testRun, (IList<TestTag>) tags);
        this.AddTelemetryForTag(requestContext, projectFromGuid.Id, testRun, (IList<TestTag>) tags, true);
      }
      return testRunById1;
    }

    private bool IsRunCompletionRequested(TestRun updateRequestModel) => updateRequestModel.State == (byte) 3 || updateRequestModel.State == (byte) 4;

    private void HandleTestRunStateUpdate(
      TestManagementRequestContext context,
      TeamProjectReference teamProject,
      TestRun testRun,
      byte lastRunState,
      bool deleteInProgressTestResults,
      bool skipRunStateTransitionCheck)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementRunService.HandleTestRunStateUpdate", (Action) (() =>
      {
        context.RequestContext.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementRunService.HandleTestRunStateUpdateAndContinue :: Test run details {0}", (object) testRun.ToString());
        bool flag = false;
        context.TraceInfo("BusinessLayer", "entered method TeamFoundationTestManagementRunService.HandleTestRunStateUpdateAndContinue :: TestRunId = {0}, State = {1}, SubState = {2}, Type = {3}", (object) testRun.TestRunId, (object) testRun.State, (object) testRun.Substate, (object) testRun.Type);
        UpdatedProperties updatedProperties = new UpdatedProperties()
        {
          Revision = -2
        };
        if (testRun.Type == (byte) 16)
        {
          if (testRun.State == (byte) 4)
          {
            if (testRun.Substate == (byte) 3 || testRun.Substate == (byte) 4 || testRun.Substate == (byte) 5 || lastRunState == (byte) 1)
            {
              updatedProperties = this.AbortRun(context, testRun, new bool?(deleteInProgressTestResults), (TestRunSubstate) testRun.Substate);
              flag = true;
            }
            else
              updatedProperties = TestRun.Cancel(context, testRun.TestRunId, testRun.TeamProject);
          }
          else if (testRun.State == (byte) 3)
            updatedProperties = testRun.Update(context, teamProject.Name);
        }
        else if (testRun.State == (byte) 4)
        {
          if (testRun.IsAutomated)
          {
            updatedProperties = this.AbortRun(context, testRun, new bool?(false), TestRunSubstate.None);
            flag = true;
          }
        }
        else if (testRun.State == (byte) 3)
        {
          updatedProperties = !testRun.IsAutomated ? this.CompleteRun(context, testRun, testRun.State, new bool?(deleteInProgressTestResults), teamProject, skipRunStateTransitionCheck) : testRun.Update(context, teamProject.Name);
          flag = true;
        }
        if (updatedProperties != null && updatedProperties.Revision == -1)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException(string.Format(ServerResources.TestRunAlreadyUpdated, (object) testRun.TestRunId));
        if (!flag & deleteInProgressTestResults)
        {
          ArgumentException ex = new ArgumentException(ServerResources.DeleteInProgressResultSetForNotCompleted, "DeleteInProgressResults");
          context.TraceError("BusinessLayer", "error occurred in TeamFoundationTestManagementRunService.HandleTestRunStateUpdateAndContinue. Error = {0}, TestRunId = {1}.", (object) ex.Message, (object) testRun.TestRunId);
          throw ex.Expected("Test Results");
        }
      }), 1015090, "TestResultsInsights");
    }

    private void CreateLogEntriesForRun(
      TestManagementRequestContext context,
      int testRunId,
      List<TestMessageLogDetails> logDetails,
      string projectName)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementRunService.CreateLogEntriesForRun", (Action) (() =>
      {
        TestMessageLogEntry[] entries = new TestMessageLogEntry[logDetails.Count<TestMessageLogDetails>()];
        for (int index = 0; index < logDetails.Count<TestMessageLogDetails>(); ++index)
        {
          entries[index] = new TestMessageLogEntry();
          entries[index].DateCreated = logDetails[index].DateCreated;
          entries[index].EntryId = logDetails[index].EntryId;
          entries[index].Message = logDetails[index].Message;
        }
        TestRun.CreateLogEntriesForRun(context, testRunId, entries, projectName);
      }), 1015090, "TestResultsInsights");
    }

    private UpdatedProperties AbortRun(
      TestManagementRequestContext context,
      TestRun testRun,
      bool? DeleteInProgressResults,
      TestRunSubstate substate)
    {
      return this.ExecuteAction<UpdatedProperties>(context.RequestContext, "TeamFoundationTestManagementRunService.AbortRun", (Func<UpdatedProperties>) (() =>
      {
        if (DeleteInProgressResults.HasValue)
        {
          bool? nullable = DeleteInProgressResults;
          bool flag = true;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
            return TestRun.Abort(context, testRun.TestRunId, testRun.Revision, testRun.TeamProject, TestRunAbortOptions.DeleteUnexecutedResults, substate);
        }
        return TestRun.Abort(context, testRun.TestRunId, testRun.Revision, testRun.TeamProject, TestRunAbortOptions.None, substate);
      }), 1015090, "TestResultsInsights");
    }

    private UpdatedProperties CompleteRun(
      TestManagementRequestContext context,
      TestRun testRun,
      byte newTestRunState,
      bool? DeleteInProgressResults,
      TeamProjectReference teamProject,
      bool skipRunStateTransitionCheck = false)
    {
      return this.ExecuteAction<UpdatedProperties>(context.RequestContext, "TeamFoundationTestManagementRunService.CompleteRun", (Func<UpdatedProperties>) (() =>
      {
        List<TestCaseResult> testCaseResults = this.EndTestCaseResults(context, testRun.TeamProject, testRun.TestRunId);
        testRun = this.GetTestRunById(context, testRun.TestRunId, teamProject, false);
        testRun.State = newTestRunState;
        return this.IsAbortRequired(testCaseResults) ? this.AbortRun(context, testRun, DeleteInProgressResults, TestRunSubstate.None) : testRun.Update(context, teamProject.Name, skipRunStateTransitionCheck, (TestSettings) null, (ReleaseReference) null, (BuildConfiguration) null, false, false);
      }), 1015090, "TestResultsInsights");
    }

    private bool IsAbortRequired(List<TestCaseResult> testCaseResults)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (TestCaseResult testCaseResult in testCaseResults)
      {
        if (testCaseResult.State == (byte) 4)
          flag1 = true;
        else if (testCaseResult.State != (byte) 5)
          flag2 = true;
      }
      return !flag1 && flag2;
    }

    private List<TestCaseResult> EndTestCaseResults(
      TestManagementRequestContext context,
      string projectName,
      int testRunId)
    {
      return this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementRunService.EndTestCaseResults", (Func<List<TestCaseResult>>) (() =>
      {
        List<TestCaseResultIdentifier> excessIds = new List<TestCaseResultIdentifier>();
        List<TestCaseResult> testCaseResultList1 = TestCaseResult.QueryByRun(context, testRunId, int.MaxValue, out excessIds, projectName, false, out List<TestActionResult> _, out List<TestResultParameter> _, out List<TestResultAttachment> _);
        List<TestCaseResult> testCaseResultList2 = new List<TestCaseResult>();
        foreach (TestCaseResult testCaseResult in testCaseResultList1)
        {
          if (this.ShouldEndTestCaseResult((Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) testCaseResult.Outcome, (TestResultState) testCaseResult.State))
          {
            testCaseResult.State = (byte) 5;
            testCaseResultList2.Add(testCaseResult);
          }
        }
        if (testCaseResultList2.Count > 0)
          this.SaveTestResults(context, projectName, testCaseResultList2.ToArray());
        return testCaseResultList1;
      }), 1015090, "TestResultsInsights");
    }

    private bool ShouldEndTestCaseResult(Microsoft.TeamFoundation.TestManagement.Client.TestOutcome testOutcome, TestResultState state) => testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified && testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None && testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Paused && state != TestResultState.Completed;

    internal void SaveTestResults(
      TestManagementRequestContext context,
      string projectName,
      TestCaseResult[] testResults)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementRunService.SaveTestResults", (Action) (() => TestCaseResult.Update(context, ((IEnumerable<TestCaseResult>) testResults).Select<TestCaseResult, ResultUpdateRequest>((System.Func<TestCaseResult, ResultUpdateRequest>) (result => new ResultUpdateRequest()
      {
        TestCaseResult = result,
        TestResultId = result.TestResultId,
        TestRunId = result.TestRunId
      })).ToArray<ResultUpdateRequest>(), projectName, false)), 1015090, "TestResultsInsights");
    }

    private void ValidateInputParamForQueryTestRun(
      TestManagementRequestContext requestContext,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState? state,
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
      int top)
    {
      minLastUpdatedDate = TestManagementServiceUtility.CheckAndGetDateForSQL(requestContext.RequestContext, minLastUpdatedDate, nameof (minLastUpdatedDate));
      maxLastUpdatedDate = TestManagementServiceUtility.CheckAndGetDateForSQL(requestContext.RequestContext, maxLastUpdatedDate, nameof (maxLastUpdatedDate));
      this.ValidateDateRange(minLastUpdatedDate, maxLastUpdatedDate, TestManagementServiceUtility.GetMaxDaysForQueryTestRuns(requestContext.RequestContext));
      this.ValidateInputRange(planIds, "PlanIds");
      this.ValidateInputRange(buildIds, "BuildIds");
      this.ValidateInputRange(buildDefIds, "BuildDefIds");
      this.ValidateInputRange(releaseIds, "ReleaseIds");
      this.ValidateInputRange(releaseDefIds, "ReleaseDefIds");
      this.ValidateInputRange(releaseEnvIds, "ReleaseEnvIds");
      this.ValidateInputRange(releaseEnvDefIds, "ReleaseEnvDefIds");
      if (buildIds.Any<int>() && buildDefIds.Any<int>())
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildIdDefinitionAmbigious));
      if (releaseIds.Any<int>() && releaseDefIds.Any<int>())
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ReleaseIdDefinitionAmbigious));
      if (releaseEnvIds.Any<int>() && releaseEnvDefIds.Any<int>())
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ReleaseEnvironmentIdDefinitionAmbigious));
      ArgumentUtility.CheckForOutOfRange(top, "Top", 0, 100, "Test Results");
    }

    private QueryTestRunsFilter PrepareQueryFilterForQueryTestRun(
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState? state,
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
      string runTitle)
    {
      QueryTestRunsFilter queryTestRunsFilter = new QueryTestRunsFilter();
      queryTestRunsFilter.MinLastUpdatedDate = new DateTime?(minLastUpdatedDate);
      queryTestRunsFilter.MaxLastUpdatedDate = new DateTime?(maxLastUpdatedDate);
      queryTestRunsFilter.PlanIds = planIds.ToList<int>();
      queryTestRunsFilter.IsAutomated = isAutomated;
      queryTestRunsFilter.BuildIds = buildIds.ToList<int>();
      queryTestRunsFilter.BuildDefIds = buildDefIds.ToList<int>();
      queryTestRunsFilter.ReleaseDefIds = releaseDefIds.ToList<int>();
      queryTestRunsFilter.ReleaseIds = releaseIds.ToList<int>();
      queryTestRunsFilter.ReleaseEnvDefIds = releaseEnvDefIds.ToList<int>();
      queryTestRunsFilter.ReleaseEnvIds = releaseEnvIds.ToList<int>();
      queryTestRunsFilter.State = state.HasValue ? (int) state.Value : -1;
      if (publishContext.HasValue)
      {
        switch (publishContext.GetValueOrDefault())
        {
          case TestRunPublishContext.Build:
            queryTestRunsFilter.SourceWorkflow = SourceWorkflow.ContinuousIntegration;
            goto label_5;
          case TestRunPublishContext.Release:
            queryTestRunsFilter.SourceWorkflow = SourceWorkflow.ContinuousDelivery;
            goto label_5;
        }
      }
      queryTestRunsFilter.SourceWorkflow = (string) null;
label_5:
      queryTestRunsFilter.RunTitle = runTitle;
      queryTestRunsFilter.BranchName = branchName;
      return queryTestRunsFilter;
    }

    private void ValidateDateRange(
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      int maxDateRange)
    {
      if (DateTime.Compare(minLastUpdatedDate, maxLastUpdatedDate) > 0)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AmbiguousDate));
      if ((maxLastUpdatedDate - minLastUpdatedDate).Ticks > (long) maxDateRange * 864000000000L)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DateRangeInValid));
    }

    private void ValidateInputRange(IList<int> field, string fieldName)
    {
      if (field.Any<int>() && field.Count > 10)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.MaxIdsError, (object) 10, (object) fieldName)));
    }

    private IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunResponse(
      TestManagementRequestContext tcmRequestContext,
      TeamProjectReference projectReference,
      List<TestRun> testRuns)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunResponse = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      if (testRuns != null)
      {
        DataContractConverter contractConverter = new DataContractConverter(tcmRequestContext);
        foreach (TestRun testRun in testRuns)
          testRunResponse.Add(contractConverter.ConvertTestRunToDataContractForBulkApi(testRun, projectReference));
      }
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) testRunResponse;
    }

    private void AddTelemetryForCustomTestFields(
      TestManagementRequestContext context,
      Guid projectId,
      TestRun testRun)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["ProjectId"] = (object) projectId,
        ["TestRunId"] = (object) testRun.TestRunId.ToString(),
        ["NumberOfCustomTestFields"] = (object) (testRun.CustomFields != null ? testRun.CustomFields.Count : 0)
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, "TestRunCustomFields", cid);
    }

    private void AddTelemetryForTag(
      TestManagementRequestContext context,
      Guid projectId,
      TestRun testRun,
      IList<TestTag> testTag,
      bool isUpdateFlow = false)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ProjectId", (object) projectId);
      intelligenceData.Add("TestRunId", testRun.TestRunId.ToString());
      intelligenceData.Add("Workflow", testRun.SourceWorkflow);
      if (testRun.BuildReference != null && !string.IsNullOrEmpty(testRun.BuildReference.BuildUri))
        intelligenceData.Add("BuildUri", testRun.BuildReference.BuildUri);
      if (testRun.ReleaseReference != null && !string.IsNullOrEmpty(testRun.ReleaseReference.ReleaseUri) && !string.IsNullOrEmpty(testRun.ReleaseReference.ReleaseEnvUri))
      {
        intelligenceData.Add("ReleaseUri", testRun.ReleaseReference.ReleaseUri);
        intelligenceData.Add("ReleaseEnvUri", testRun.ReleaseReference.ReleaseEnvUri);
        intelligenceData.Add("Attempt", (double) testRun.ReleaseReference.Attempt);
      }
      ProjectInfo project = context.RequestContext.GetService<IProjectService>().GetProject(context.RequestContext, projectId);
      intelligenceData.AddDataspaceInformation(CustomerIntelligenceDataspaceType.Project, projectId.ToString(), ((int) project.Visibility).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (isUpdateFlow)
        intelligenceData.Add("UpdateTestRunTag", true);
      else
        intelligenceData.Add("CreateTestRunTag", true);
      intelligenceData.Add("TestRunTagCount", (double) testTag.Count);
      new TelemetryLogger().PublishData(context.RequestContext, "TestRunTag", intelligenceData);
    }

    private static void _validateAndRemoveExistingTestTagsBeforeUpdate(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestRun testRun,
      int testRunNoOfTagLimit)
    {
      if (testRun.Tags == null)
        return;
      ITestLogStoreService service = tcmRequestContext.RequestContext.GetService<ITestLogStoreService>();
      TestLogReference testLogReference = TeamFoundationTestManagementRunService._getTestLogReference(tcmRequestContext, projectInfo, testRun, true);
      IList<TestTag> existingTags = service.GetTestTagsForRun(tcmRequestContext, projectInfo, testLogReference).Result;
      testRun.Tags = testRun.Tags.Where<TestTag>((System.Func<TestTag, bool>) (t => !existingTags.Any<TestTag>((System.Func<TestTag, bool>) (et => et.Name == t.Name)))).ToList<TestTag>();
      if (existingTags.Count + testRun.Tags.Count > testRunNoOfTagLimit)
        throw new ArgumentException(string.Format(ServerResources.TestTagLimitReached, (object) (existingTags.Count + testRun.Tags.Count), (object) testRunNoOfTagLimit));
    }

    private static List<TestTag> _createTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestRun testRun,
      IList<TestTag> testTags)
    {
      List<TestTag> testTagForRun = new List<TestTag>();
      ITestLogStoreService service = tcmRequestContext.RequestContext.GetService<ITestLogStoreService>();
      TestLogReference testLogReference = TeamFoundationTestManagementRunService._getTestLogReference(tcmRequestContext, projectInfo, testRun, true);
      foreach (TestTag testTag in testTags.Distinct<TestTag>().ToList<TestTag>())
      {
        if (service.CreateTestTagForRun(tcmRequestContext, projectInfo, testLogReference, testTag?.Name).Result != TestLogStatusCode.Success)
          tcmRequestContext.RequestContext.Trace(0, TraceLevel.Error, "TestResultsInsights", "RestLayer", "test tag creation for runid {0} failed {1}", (object) testRun.TestRunId, (object) testTag.Name);
        else
          testTagForRun.Add(testTag);
      }
      return testTagForRun;
    }

    private static async Task<List<TestTag>> _createTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      ITestLogStoreService testLogStoreService,
      TestLogReference testLogReference,
      TestRun testRun,
      IList<TestTag> testTags)
    {
      List<TestTag> testTagsUpdated = new List<TestTag>();
      foreach (TestTag tag in testTags.Distinct<TestTag>().ToList<TestTag>())
      {
        if (await testLogStoreService.CreateTestTagForRun(tcmRequestContext, projectInfo, testLogReference, tag?.Name) != TestLogStatusCode.Success)
          tcmRequestContext.RequestContext.Trace(0, TraceLevel.Error, "TestResultsInsights", "RestLayer", "test tag creation for runid {0} failed {1}", (object) testRun.TestRunId, (object) tag.Name);
        else
          testTagsUpdated.Add(tag);
      }
      List<TestTag> testTagForRun = testTagsUpdated;
      testTagsUpdated = (List<TestTag>) null;
      return testTagForRun;
    }

    private static async Task<List<TestTag>> _deleteTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      ITestLogStoreService testLogStoreService,
      TestLogReference testLogReference,
      TestRun testRun,
      IList<TestTag> testTags)
    {
      List<TestTag> testTagsDeleted = new List<TestTag>();
      foreach (TestTag tag in testTags.Distinct<TestTag>().ToList<TestTag>())
      {
        if (await testLogStoreService.DeleteTestTagForRun(tcmRequestContext, projectInfo, testLogReference, tag?.Name) != TestLogStatusCode.Success)
          tcmRequestContext.RequestContext.Trace(0, TraceLevel.Error, "TestResultsInsights", "RestLayer", "test tag deletion for runid {0} failed {1}", (object) testRun.TestRunId, (object) tag.Name);
        else
          testTagsDeleted.Add(tag);
      }
      List<TestTag> testTagList = testTagsDeleted;
      testTagsDeleted = (List<TestTag>) null;
      return testTagList;
    }

    private static async Task<List<TestTag>> _updateTestTag(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestRun testRun,
      IList<TestTag> tagsToBeAdded,
      IList<TestTag> tagsToBeDeleted)
    {
      List<TestTag> testTag = new List<TestTag>();
      IVssRegistryService service = tcmRequestContext.RequestContext.GetService<IVssRegistryService>();
      int testRunNoOfTagLimit = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestRunNoOfTagLimit", 5);
      int testRunTagSizeLimit = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestRunTagSizeLimit", 50);
      string testRunTagAllowedSpecialChars = TeamFoundationTestManagementRunService.GetRunTagAllowedSpecialChars(tcmRequestContext);
      ITestLogStoreService testLogStoreService = tcmRequestContext.RequestContext.GetService<ITestLogStoreService>();
      TestLogReference testLogReference = TeamFoundationTestManagementRunService._getTestLogReference(tcmRequestContext, projectInfo, testRun, true);
      if (tagsToBeDeleted != null && tagsToBeDeleted.Count > 0)
      {
        List<TestTag> testTagList1 = await TeamFoundationTestManagementRunService._deleteTestTagForRun(tcmRequestContext, projectInfo, testLogStoreService, testLogReference, testRun, tagsToBeDeleted);
      }
      if (tagsToBeAdded != null && tagsToBeAdded.Count > 0)
      {
        foreach (TestTag tag in (IEnumerable<TestTag>) tagsToBeAdded)
          TeamFoundationTestManagementRunService._validateTestTagForRun(tag, testRunTagSizeLimit, testRunTagAllowedSpecialChars);
        IList<TestTag> existingTags = await testLogStoreService.GetTestTagsForRun(tcmRequestContext, projectInfo, testLogReference);
        tagsToBeAdded = (IList<TestTag>) tagsToBeAdded.Where<TestTag>((System.Func<TestTag, bool>) (t => !existingTags.Any<TestTag>((System.Func<TestTag, bool>) (et => et.Name == t.Name)))).ToList<TestTag>();
        if (existingTags.Count + tagsToBeAdded.Count > testRunNoOfTagLimit)
          throw new ArgumentException(string.Format(ServerResources.TestTagLimitReached, (object) (existingTags.Count + tagsToBeAdded.Count), (object) testRunNoOfTagLimit));
        if (tagsToBeAdded != null && tagsToBeAdded.Count > 0)
        {
          List<TestTag> testTagForRun = await TeamFoundationTestManagementRunService._createTestTagForRun(tcmRequestContext, projectInfo, testLogStoreService, testLogReference, testRun, tagsToBeAdded);
        }
      }
      IList<TestTag> testTagsForRun = await testLogStoreService.GetTestTagsForRun(tcmRequestContext, projectInfo, testLogReference);
      if (testTagsForRun != null && testTagsForRun.Count > 0)
        testTag.AddRange((IEnumerable<TestTag>) testTagsForRun);
      List<TestTag> testTagList2 = testTag;
      testTag = (List<TestTag>) null;
      testRunTagAllowedSpecialChars = (string) null;
      testLogStoreService = (ITestLogStoreService) null;
      testLogReference = (TestLogReference) null;
      return testTagList2;
    }

    private static TestLogReference _getTestLogReference(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestRun testRun,
      bool isThrowException)
    {
      TestLogReference testLogReference = new TestLogReference();
      if (testRun.ReleaseReference != null && testRun.ReleaseReference.ReleaseId > 0 && testRun.ReleaseReference.ReleaseEnvId > 0)
      {
        testLogReference.Scope = TestLogScope.Release;
        testLogReference.ReleaseId = testRun.ReleaseReference.ReleaseId;
        testLogReference.ReleaseEnvId = testRun.ReleaseReference.ReleaseEnvId;
      }
      else if (testRun.BuildReference != null && testRun.BuildReference.BuildId > 0)
      {
        testLogReference.Scope = TestLogScope.Build;
        testLogReference.BuildId = testRun.BuildReference.BuildId;
      }
      else if (isThrowException)
        throw new ArgumentException(ServerResources.TestTagNotSupportedForManual);
      testLogReference.RunId = testRun.TestRunId;
      return testLogReference;
    }

    private static void _validateTestTagsForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestRun testRun,
      bool isUpdateFlow = false)
    {
      if (testRun.Tags == null)
        return;
      IVssRegistryService registryService = TeamFoundationTestManagementRunService._isTestTagSupported(tcmRequestContext) ? tcmRequestContext.RequestContext.GetService<IVssRegistryService>() : throw new ArgumentException(ServerResources.TestTagForRunNotSupported);
      IVssRequestContext requestContext1 = tcmRequestContext.RequestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestRunNoOfTagLimit";
      ref RegistryQuery local1 = ref registryQuery;
      int testRunNoOfTagLimit = registryService.GetValue<int>(requestContext1, in local1, 5);
      IVssRequestContext requestContext2 = tcmRequestContext.RequestContext;
      registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestRunTagSizeLimit";
      ref RegistryQuery local2 = ref registryQuery;
      int testRunTagSizeLimit = registryService.GetValue<int>(requestContext2, in local2, 50);
      string allowedSpecialChars = TeamFoundationTestManagementRunService.GetRunTagAllowedSpecialChars(tcmRequestContext);
      if (testRun.Tags.Count > testRunNoOfTagLimit)
        throw new ArgumentException(string.Format(ServerResources.TestTagLimitReached, (object) testRun.Tags.Count, (object) testRunNoOfTagLimit));
      foreach (TestTag tag in testRun.Tags)
        TeamFoundationTestManagementRunService._validateTestTagForRun(tag, testRunTagSizeLimit, allowedSpecialChars);
      if (!isUpdateFlow)
        return;
      TeamFoundationTestManagementRunService._validateAndRemoveExistingTestTagsBeforeUpdate(tcmRequestContext, projectInfo, testRun, testRunNoOfTagLimit);
    }

    private static string GetRunTagAllowedSpecialChars(
      TestManagementRequestContext tcmRequestContext)
    {
      return "=-_" + tcmRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<string>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestRunAllowedSpecialCharsInTagName", "");
    }

    private static void _validateTestTagForRun(
      TestTag tag,
      int testRunTagSizeLimit,
      string testRunTagAllowedSpecialChars)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(tag.Name, "testTag", "Test Results");
      ArgumentUtility.CheckStringLength(tag.Name, "testTag", testRunTagSizeLimit, 1, "Test Results");
      if (!new LogStorePathFormatter().ValidateRunTagName(tag.Name, testRunTagAllowedSpecialChars))
        throw new ArgumentException(ServerResources.TestTagFormatError);
    }

    private static bool _isTestTagSupported(TestManagementRequestContext tcmRequestContext) => tcmRequestContext.IsTcmService && tcmRequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService") && tcmRequestContext.IsFeatureEnabled("TestManagement.Server.TagsInTestRun");

    private static void AddTestTagToRun(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      TestRun testRun)
    {
      ITestLogStoreService testLogStoreService = TeamFoundationTestManagementRunService._isTestTagSupported(requestContext) ? requestContext.RequestContext.GetService<ITestLogStoreService>() : throw new ArgumentException(ServerResources.TestTagForRunNotSupported);
      TestLogReference testLogReference = TeamFoundationTestManagementRunService._getTestLogReference(requestContext, projectInfo, testRun, false);
      if (testLogReference.Scope != TestLogScope.Build && testLogReference.Scope != TestLogScope.Release)
        return;
      IList<TestTag> result = testLogStoreService.GetTestTagsForRun(requestContext, projectInfo, testLogReference).Result;
      testRun.Tags = result != null ? result.ToList<TestTag>() : (List<TestTag>) null;
    }

    private static List<TestTag> _secureTestTagObject(List<TestTag> testTags, Guid projectId)
    {
      foreach (TestTag testTag in testTags)
      {
        ISecuredObject securedObject = (ISecuredObject) new TeamProjectReference()
        {
          Id = projectId
        };
        testTag.InitializeSecureObject(securedObject);
      }
      return testTags;
    }

    private bool IsPRTestDataPublishDisabledToAXService(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/DisablePublishPRTestDataToAXService", false);

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestSummaryForRun(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      TestRun run)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestSummaryForRun", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() =>
      {
        try
        {
          DataContractConverter dataContractConverter = new DataContractConverter(requestContext);
          Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic testSummaryForRun = new Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic();
          testSummaryForRun.Run = dataContractConverter.GetRunRepresentation(projectInfo.Name, run);
          testSummaryForRun.RunStatistics = new List<RunStatistic>();
          List<TestRunStatistic> source = this.QueryTestRunSummary(requestContext, projectInfo, run);
          if (source != null && source.Any<TestRunStatistic>())
            testSummaryForRun.RunStatistics.AddRange(source.Select<TestRunStatistic, RunStatistic>((System.Func<TestRunStatistic, RunStatistic>) (testRunStatistic => dataContractConverter.ConvertTestStatisticsToDataContract(testRunStatistic))));
          return testSummaryForRun;
        }
        catch (TestObjectNotFoundException ex)
        {
          requestContext.TraceInfo("RestLayer", "Error occured in TeamFoundationTestManagementRunService.GetTestSummaryForRun error = {0}", (object) ex);
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ex.Message));
        }
      }), 1015090, "TestResultsInsights");
    }

    private List<TestRunStatistic> QueryTestRunSummary(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      TestRun run)
    {
      Dictionary<int, List<TestRunStatistic>> dictionary;
      using (TestManagementDatabase managementDatabase1 = TestManagementDatabase.Create(requestContext))
      {
        TestManagementDatabase managementDatabase2 = managementDatabase1;
        List<int> testRunIds = new List<int>(1);
        testRunIds.Add(run.TestRunId);
        Guid id = projectInfo.Id;
        int num = requestContext.IsTcmService ? 1 : 0;
        dictionary = managementDatabase2.QueryTestRunStatistics(testRunIds, id, num != 0, false);
      }
      return dictionary != null && dictionary.ContainsKey(run.TestRunId) ? dictionary[run.TestRunId] : new List<TestRunStatistic>();
    }
  }
}
