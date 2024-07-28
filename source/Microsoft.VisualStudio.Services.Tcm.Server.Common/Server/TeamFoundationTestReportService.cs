// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestReportService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.Interface;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Models;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestReportService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestReportService,
    IVssFrameworkService
  {
    private IBuildConfiguration _buildConfigurationHelper;
    private IReleaseServiceHelper _releaseServiceHelper;
    private const int c_secondsToSubtractFromBuildFinishTime = 1;

    public void UpdateTestRunSummaryAndInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildRef,
      ReleaseReference releaseRef)
    {
      try
      {
        int releaseRefId = releaseRef != null ? releaseRef.ReleaseRefId : 0;
        int buildConfigurationId = buildRef != null ? buildRef.BuildConfigurationId : 0;
        BuildConfiguration lastSuccessfulBuild = (BuildConfiguration) null;
        ReleaseReference lastSuccessfulRelease = (ReleaseReference) null;
        TestResultsContextType contextType = TestResultsContextType.Build;
        if (!context.IsFeatureEnabled("TestManagement.Server.DisableFailureInsightsCalculation"))
        {
          if (releaseRefId > 0)
          {
            lastSuccessfulRelease = this.ReleaseServiceHelper.QueryLastCompleteSuccessfulRelease(context, projectId, releaseRef, releaseRef.ReleaseCreationDate, buildRef?.BranchName ?? string.Empty);
            contextType = TestResultsContextType.Release;
          }
          else if (buildConfigurationId > 0)
          {
            buildRef = this.BuildConfigurationHelper.Query(context, projectId.GuidId, buildConfigurationId);
            lastSuccessfulBuild = this.BuildServiceHelper.QueryLastSuccessfulBuild(context, projectId.GuidId, buildRef, buildRef.CreatedDate);
          }
        }
        this.UpdateInsights(context, projectId, testRunIds, buildRef, releaseRef, contextType, lastSuccessfulBuild, lastSuccessfulRelease);
      }
      catch (Exception ex) when (!(ex is TestObjectNotFoundException))
      {
        TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
    }

    private static int GetBatchSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/SummaryAndInsightsPublishBatchSize", 10);

    internal void FetchFailureDetailsAndUpdateInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration currentBuild,
      ReleaseReference currentRelease,
      BuildConfiguration lastSuccessfulBuild,
      ReleaseReference lastSuccessfulRelease,
      Dictionary<int, TestCaseResult> previousFailedResultsMap,
      TestResultsContextType contextType)
    {
      bool shouldPublishOnlyFailedResults = context.IsFeatureEnabled("TestManagement.Server.PublishOnlyFailedResults") && GitHelper.IsPullRequest(currentBuild?.BranchName ?? string.Empty);
      Dictionary<int, ResultInsights> runToResultInsights = new Dictionary<int, ResultInsights>();
      Dictionary<int, Dictionary<int, string>> runToFailingSince = new Dictionary<int, Dictionary<int, string>>();
      bool shouldByPassFlaky = ReportingOptionsHelper.ShouldPublishFlakiness(this.GetTestManagementRequestContext(context), projectId);
      if (context.IsFeatureEnabled("TestManagement.Server.DisableFailureInsightsCalculation"))
      {
        foreach (int testRunId in testRunIds)
          runToResultInsights[testRunId] = new ResultInsights();
        this.publishTestSummaryAndFailureDetails(context, projectId, testRunIds, runToResultInsights, runToFailingSince, false, shouldPublishOnlyFailedResults);
      }
      else
      {
        string sourceWorkflow = contextType.Equals((object) TestResultsContextType.Build) ? SourceWorkflow.ContinuousIntegration : SourceWorkflow.ContinuousDelivery;
        Dictionary<int, List<TestCaseResult>> currentFailedResults;
        int prevTestRunContextId;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          bool fetchPreviousFailedResults = previousFailedResultsMap == null;
          Dictionary<int, TestCaseResult> previousFailedResultsMap1;
          managementDatabase.FetchTestFailureDetails(projectId, testRunIds, lastSuccessfulBuild, lastSuccessfulRelease, sourceWorkflow, fetchPreviousFailedResults, shouldByPassFlaky, out currentFailedResults, out previousFailedResultsMap1, out prevTestRunContextId);
          if (fetchPreviousFailedResults)
            previousFailedResultsMap = previousFailedResultsMap1;
        }
        foreach (int testRunId in testRunIds)
        {
          Dictionary<int, string> failingSinceMap;
          runToResultInsights[testRunId] = this.CalculateResultInsights(currentFailedResults[testRunId], previousFailedResultsMap, currentBuild, currentRelease, lastSuccessfulBuild, lastSuccessfulRelease, contextType, out failingSinceMap);
          runToResultInsights[testRunId].PrevRunContextId = prevTestRunContextId;
          runToFailingSince[testRunId] = failingSinceMap;
        }
        this.publishTestSummaryAndFailureDetails(context, projectId, testRunIds, runToResultInsights, runToFailingSince, true, shouldPublishOnlyFailedResults);
      }
    }

    internal void publishTestSummaryAndFailureDetails(
      IVssRequestContext context,
      GuidAndString projectId,
      List<int> testRunIds,
      Dictionary<int, ResultInsights> runToResultInsights,
      Dictionary<int, Dictionary<int, string>> runToFailingSince,
      bool includeFailureDetails,
      bool shouldPublishOnlyFailedResults = false)
    {
      List<TestResultIdentifierRecord> flakyResults = new List<TestResultIdentifierRecord>();
      bool publishPassCountOnly = context.IsFeatureEnabled("TestManagement.Server.SkipPassedTestsDetails") | shouldPublishOnlyFailedResults;
      bool shouldPublishFlakiness = !context.IsFeatureEnabled("TestManagement.Server.EnablePassedFlakyPropagation");
      bool shouldByPassFlaky = ReportingOptionsHelper.ShouldPublishFlakiness(this.GetTestManagementRequestContext(context), projectId);
      if (shouldByPassFlaky)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          flakyResults = managementDatabase.QueryFlakyTestResults(projectId.GuidId, testRunIds);
      }
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.PublishTestSummaryAndFailureDetails(projectId, testRunIds, runToResultInsights, runToFailingSince, flakyResults, includeFailureDetails, publishPassCountOnly, shouldPublishFlakiness, shouldByPassFlaky);
    }

    internal ResultInsights CalculateResultInsights(
      List<TestCaseResult> currentFailedResults,
      Dictionary<int, TestCaseResult> previousFailedResults,
      BuildConfiguration currentBuild,
      ReleaseReference currentRelease,
      BuildConfiguration lastSuccessfulBuild,
      ReleaseReference lastSuccessfulRelease,
      TestResultsContextType contextType,
      out Dictionary<int, string> failingSinceMap)
    {
      failingSinceMap = new Dictionary<int, string>();
      ResultInsights resultInsights = new ResultInsights();
      List<int> values1 = new List<int>();
      List<int> values2 = new List<int>();
      DateTime dateCompleted;
      foreach (TestCaseResult currentFailedResult in currentFailedResults)
      {
        if (!TeamFoundationTestReportService.IsNewFailure(previousFailedResults, lastSuccessfulBuild, lastSuccessfulRelease, contextType, currentFailedResult))
        {
          ++resultInsights.ExistingFailures;
          values1.Add(currentFailedResult.TestResultId);
          if (previousFailedResults[currentFailedResult.TestCaseReferenceId].FailingSince != null)
          {
            failingSinceMap[currentFailedResult.TestResultId] = previousFailedResults[currentFailedResult.TestCaseReferenceId].FailingSince.ToString();
          }
          else
          {
            Dictionary<int, string> dictionary = failingSinceMap;
            int testResultId = currentFailedResult.TestResultId;
            FailingSince failingSince = new FailingSince();
            BuildReference buildReference;
            if (contextType != TestResultsContextType.Build)
            {
              buildReference = (BuildReference) null;
            }
            else
            {
              buildReference = new BuildReference();
              buildReference.Id = lastSuccessfulBuild.BuildId;
              buildReference.Number = lastSuccessfulBuild.BuildNumber;
              buildReference.BuildSystem = lastSuccessfulBuild.BuildSystem;
            }
            failingSince.Build = buildReference;
            Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseReference;
            if (contextType != TestResultsContextType.Release)
            {
              releaseReference = (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null;
            }
            else
            {
              releaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
              releaseReference.Id = lastSuccessfulRelease.ReleaseId;
              releaseReference.Name = lastSuccessfulRelease.ReleaseName;
              releaseReference.EnvironmentId = lastSuccessfulRelease.ReleaseEnvId;
            }
            failingSince.Release = releaseReference;
            dateCompleted = previousFailedResults[currentFailedResult.TestCaseReferenceId].DateCompleted;
            failingSince.Date = dateCompleted.ToUniversalTime();
            string str = failingSince.ToString();
            dictionary[testResultId] = str;
          }
        }
        else
        {
          ++resultInsights.NewFailures;
          values2.Add(currentFailedResult.TestResultId);
          Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseReference1;
          if (contextType != TestResultsContextType.Release)
          {
            releaseReference1 = (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null;
          }
          else
          {
            releaseReference1 = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
            releaseReference1.Id = currentRelease.ReleaseId;
            releaseReference1.Name = currentRelease.ReleaseName;
            releaseReference1.EnvironmentId = currentRelease.ReleaseEnvId;
          }
          Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseReference2 = releaseReference1;
          BuildReference buildReference1;
          if (contextType != TestResultsContextType.Build)
          {
            buildReference1 = new BuildReference()
            {
              BranchName = currentBuild == null || !string.IsNullOrEmpty(currentBuild.BranchName) ? string.Empty : currentBuild.BranchName
            };
          }
          else
          {
            buildReference1 = new BuildReference();
            buildReference1.Id = currentBuild.BuildId;
            buildReference1.Number = currentBuild.BuildNumber;
            buildReference1.BuildSystem = currentBuild.BuildSystem;
          }
          BuildReference buildReference2 = buildReference1;
          FailingSince failingSince1 = new FailingSince();
          failingSince1.Build = buildReference2;
          failingSince1.Release = releaseReference2;
          dateCompleted = currentFailedResult.DateCompleted;
          failingSince1.Date = dateCompleted.ToUniversalTime();
          FailingSince failingSince2 = failingSince1;
          failingSinceMap[currentFailedResult.TestResultId] = failingSince2.ToString();
        }
      }
      resultInsights.NewFailedResults = string.Join<int>(",", (IEnumerable<int>) values2);
      resultInsights.ExistingFailedResults = string.Join<int>(",", (IEnumerable<int>) values1);
      resultInsights.FixedTestResults = string.Empty;
      return resultInsights;
    }

    internal static bool IsNewFailure(
      Dictionary<int, TestCaseResult> previousFailedResults,
      BuildConfiguration lastSuccessfulBuild,
      ReleaseReference lastSuccessfulRelease,
      TestResultsContextType contextType,
      TestCaseResult result)
    {
      int num = previousFailedResults.ContainsKey(result.TestCaseReferenceId) ? 1 : 0;
      bool flag = num != 0 && previousFailedResults[result.TestCaseReferenceId].FailingSince == null;
      if (num == 0)
        return true;
      if (!flag)
        return false;
      return contextType != TestResultsContextType.Build ? lastSuccessfulRelease == null : lastSuccessfulBuild == null;
    }

    public PipelineTestMetrics GetPipelineTestMetrics(
      IVssRequestContext context,
      GuidAndString projectId,
      PipelineReference pipelineReference,
      string metricNames,
      bool groupByNode = false)
    {
      PipelineTestMetrics pipelineTestMetrics = new PipelineTestMetrics()
      {
        CurrentContext = pipelineReference
      };
      string str = typeof (TeamFoundationTestReportService)?.ToString() + ".GetPipelineTestMetrics";
      context.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
      try
      {
        using (PerfManager.Measure(context, "BusinessLayer", str))
        {
          PipelineReferenceHelper.ValidateAndHandleDefaultValuesForPipelineRefInQuery(pipelineReference);
          List<Metrics> metrics1 = this.ValidateAndGetMetrics(metricNames);
          bool flag = false;
          bool resultsAnalysisFlag = false;
          bool runSummaryFlag = false;
          foreach (Metrics metrics2 in metrics1)
          {
            switch (metrics2)
            {
              case Metrics.All:
                flag = true;
                resultsAnalysisFlag = true;
                runSummaryFlag = true;
                continue;
              case Metrics.ResultSummary:
                flag = true;
                continue;
              case Metrics.ResultsAnalysis:
                resultsAnalysisFlag = true;
                continue;
              case Metrics.RunSummary:
                runSummaryFlag = true;
                continue;
              default:
                continue;
            }
          }
          this.BatchedUpdateRunSummaryAndInsightsForPipeline(context, projectId, pipelineReference);
          RunSummaryAndResultInsightsInPipeline runSummaryAndResultInsightsInPipeline = (RunSummaryAndResultInsightsInPipeline) null;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            runSummaryAndResultInsightsInPipeline = managementDatabase.GetPipelineTestMetrics(projectId, pipelineReference, flag, resultsAnalysisFlag, runSummaryFlag, groupByNode);
          if (runSummaryAndResultInsightsInPipeline != null)
          {
            BuildConfiguration buildConfiguration = (BuildConfiguration) null;
            if (runSummaryAndResultInsightsInPipeline.ResultInsights != null && runSummaryAndResultInsightsInPipeline.ResultInsights.Any<ResultInsightsInPipeline>() && runSummaryAndResultInsightsInPipeline.ResultInsights[0].PrevPipelineRefId > 0)
              buildConfiguration = this.BuildConfigurationHelper.Query(context, projectId.GuidId, runSummaryAndResultInsightsInPipeline.ResultInsights[0].PrevPipelineRefId);
            if (!groupByNode)
            {
              pipelineTestMetrics = MetricsCalculatorHelper.CalculatePipelineTestMetricsForAParticularNodeFromServerOM(context, pipelineReference, runSummaryAndResultInsightsInPipeline, buildConfiguration != null ? buildConfiguration.BuildId : 0, flag, resultsAnalysisFlag, runSummaryFlag);
            }
            else
            {
              PipelineNodeHierarchyLevel requiredResultLevel = PipelineNodeHierarchyLevel.Job;
              if (string.IsNullOrEmpty(pipelineReference.JobReference?.JobName))
                requiredResultLevel = PipelineNodeHierarchyLevel.Phase;
              if (string.IsNullOrEmpty(pipelineReference.PhaseReference?.PhaseName))
                requiredResultLevel = PipelineNodeHierarchyLevel.Stage;
              if (string.IsNullOrEmpty(pipelineReference.StageReference?.StageName))
                requiredResultLevel = PipelineNodeHierarchyLevel.PipelineInstance;
              pipelineTestMetrics = MetricsCalculatorHelper.CalculatePipelineTestMatricsForEachNodeFromServerOM(context, pipelineReference, runSummaryAndResultInsightsInPipeline, buildConfiguration != null ? buildConfiguration.BuildId : 0, flag, resultsAnalysisFlag, runSummaryFlag, requiredResultLevel);
            }
          }
        }
        TeamProjectReference projectReference = this.GetProjectReference(context, projectId.GuidId.ToString());
        pipelineTestMetrics?.InitializeSecureObject((ISecuredObject) projectReference);
        return pipelineTestMetrics;
      }
      catch (Exception ex)
      {
        if (!(ex is TestObjectNotFoundException))
          TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
      finally
      {
        context.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    private List<Metrics> ValidateAndGetMetrics(string metricNames)
    {
      List<Metrics> metrics = new List<Metrics>();
      if (string.IsNullOrWhiteSpace(metricNames))
      {
        metrics.Add(Metrics.ResultSummary);
      }
      else
      {
        foreach (string property in ParsingHelper.ParseCommaSeparatedString(metricNames))
        {
          Metrics result;
          if (!Enum.TryParse<Metrics>(property, out result))
            throw new InvalidPropertyException(property, "Not a valid Metrics Name");
          metrics.Add(result);
        }
      }
      return metrics;
    }

    public TestResultSummary QueryTestSummaryAndInsightsForPipeline(
      IVssRequestContext context,
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool includeFailureDetails)
    {
      string str = typeof (TeamFoundationTestReportService)?.ToString() + ".QueryTestSummaryAndInsightsForPipeline";
      try
      {
        context.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context, "BusinessLayer", str))
        {
          PipelineReferenceHelper.ValidateAndHandleDefaultValuesForPipelineRefInQuery(pipelineReference);
          ReportingOptions reportingOptions = ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId);
          BuildConfiguration prevBuild = (BuildConfiguration) null;
          BuildConfiguration buildConfiguration = new BuildConfiguration()
          {
            BuildId = pipelineReference.PipelineId
          };
          RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights();
          int runsCount = 0;
          this.BatchedUpdateRunSummaryAndInsightsForPipeline(context, projectId, pipelineReference);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            runSummaryAndInsights = managementDatabase.QueryTestRunSummaryAndInsightsForPipeline(projectId, pipelineReference, includeFailureDetails, reportingOptions.GroupByCategory, out runsCount);
          if (runSummaryAndInsights.TestResultInsights.Any<ResultInsights>() && runSummaryAndInsights.TestResultInsights[0].PrevBuildRefId > 0)
            prevBuild = this.BuildConfigurationHelper.Query(context, projectId.GuidId, runSummaryAndInsights.TestResultInsights[0].PrevBuildRefId);
          List<TestCaseResult> resultsByCategory = TeamFoundationTestReportService.GetResultsByCategory(context, projectId, buildConfiguration, SourceWorkflow.ContinuousIntegration, reportingOptions.GroupByCategory);
          TestResultSummary webApiDataContract = this.ConvertServerOMToWebApiDataContract(context, runSummaryAndInsights, buildConfiguration, prevBuild, (ReleaseReference) null, (ReleaseReference) null, true, reportingOptions, resultsByCategory);
          if (webApiDataContract != null)
          {
            TeamProjectReference projectReference = this.GetProjectReference(context, projectId.GuidId.ToString());
            webApiDataContract.TeamProject = projectReference;
            this.SecureTestResultSummary(webApiDataContract);
          }
          return webApiDataContract;
        }
      }
      catch (Exception ex)
      {
        if (!(ex is TestObjectNotFoundException))
          TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
      finally
      {
        context.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public TestResultSummary QueryTestSummaryAndInsightsForBuild(
      IVssRequestContext context,
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration currentBuild,
      BuildConfiguration previousBuild,
      bool returnSummary,
      bool returnFailureDetails)
    {
      try
      {
        using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.QueryTestSummaryAndInsightsForBuild"))
        {
          RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights();
          int runsCount = 0;
          bool isBuildOld = false;
          ReportingOptions reportingOptions = ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId);
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context);
          if (currentBuild != null)
            this.BatchedUpdateRunSummaryAndInsights(context, projectId, TestRunPublishContext.Build, currentBuild.BuildId, (Action<List<TestRun>>) (testRuns =>
            {
              this.UpdateTestRunSummaryAndInsights(context, projectId, testRuns.Select<TestRun, int>((Func<TestRun, int>) (testRun => testRun.TestRunId)).ToList<int>(), testRuns.First<TestRun>().BuildReference, (ReleaseReference) null);
              IVssRequestContext context1 = context;
              Dictionary<string, object> keyValues = new Dictionary<string, object>();
              keyValues.Add("TotalNumberOfRunsWithoutSummary", (object) testRuns.Count);
              keyValues.Add("SummaryAndInsightsCalculationFlow", (object) TelemetryParameterNames.SummaryAndInsightsCalculationFlowType.QueryFlow);
              BuildConfiguration build = currentBuild;
              TestManagementServiceUtility.PublishTelemetry(context1, "CalculateSummaryAndInsights", keyValues, build: build);
            }));
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          {
            if (previousBuild == null)
              runSummaryAndInsights = managementDatabase.QueryTestRunSummaryAndInsightsForBuild(projectId, sourceWorkflow, currentBuild, returnSummary, returnFailureDetails, reportingOptions.GroupByCategory, out runsCount, out isBuildOld, progressOrFailed);
          }
          if (runSummaryAndInsights.TestResultInsights.Any<ResultInsights>() && runSummaryAndInsights.TestResultInsights[0].PrevBuildRefId > 0)
            previousBuild = this.BuildConfigurationHelper.Query(context, projectId.GuidId, runSummaryAndInsights.TestResultInsights[0].PrevBuildRefId);
          List<TestCaseResult> resultsByCategory = TeamFoundationTestReportService.GetResultsByCategory(context, projectId, currentBuild, sourceWorkflow, reportingOptions.GroupByCategory);
          return this.ConvertServerOMToWebApiDataContract(context, runSummaryAndInsights, currentBuild, previousBuild, (ReleaseReference) null, (ReleaseReference) null, !isBuildOld, ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId), resultsByCategory);
        }
      }
      catch (Exception ex)
      {
        if (!(ex is TestObjectNotFoundException))
          TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
    }

    public TestResultSummary QueryTestSummaryAndInsightsForRelease(
      IVssRequestContext context,
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference currentRelease,
      ReleaseReference previousRelease,
      bool returnSummary,
      bool returnFailureDetails)
    {
      try
      {
        using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.QueryTestSummaryAndInsightsForRelease"))
        {
          RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights();
          int runsCount = 0;
          ReportingOptions reportingOptions = ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId);
          bool enableLastCompletedReleaseSearchInAT = context.IsFeatureEnabled("TestManagement.Server.EnableLastCompletedReleaseSearchInAT");
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context);
          if (currentRelease != null)
            this.BatchedUpdateRunSummaryAndInsights(context, projectId, TestRunPublishContext.Release, currentRelease.ReleaseId, (Action<List<TestRun>>) (testRuns =>
            {
              IEnumerable<IGrouping<int, TestRun>> groupRunByEnvironmentId = testRuns.GroupBy<TestRun, int>((Func<TestRun, int>) (testRun => testRun.ReleaseReference.ReleaseEnvId));
              if (enableLastCompletedReleaseSearchInAT)
              {
                this.UpdateTestRunSummaryAndInsights2(context, projectId, currentRelease, groupRunByEnvironmentId);
              }
              else
              {
                foreach (IGrouping<int, TestRun> source in groupRunByEnvironmentId)
                {
                  this.UpdateTestRunSummaryAndInsights(context, projectId, source.Select<TestRun, int>((Func<TestRun, int>) (testRun => testRun.TestRunId)).ToList<int>(), (BuildConfiguration) null, source.First<TestRun>().ReleaseReference);
                  IVssRequestContext context1 = context;
                  Dictionary<string, object> keyValues = new Dictionary<string, object>();
                  keyValues.Add("TotalNumberOfRunsWithoutSummary", (object) source.Count<TestRun>());
                  keyValues.Add("SummaryAndInsightsCalculationFlow", (object) TelemetryParameterNames.SummaryAndInsightsCalculationFlowType.QueryFlow);
                  ReleaseReference release = currentRelease;
                  TestManagementServiceUtility.PublishTelemetry(context1, "CalculateSummaryAndInsights", keyValues, release);
                }
              }
            }));
          if (previousRelease == null)
          {
            using (TestManagementDatabase managementDatabase1 = TestManagementDatabase.Create(context))
            {
              if (enableLastCompletedReleaseSearchInAT)
              {
                TestManagementDatabase managementDatabase2 = managementDatabase1;
                GuidAndString projectId1 = projectId;
                string sourceWorkflow1 = sourceWorkflow;
                ReleaseReference releaseRef = new ReleaseReference();
                releaseRef.ReleaseId = currentRelease.ReleaseId;
                releaseRef.ReleaseEnvId = 0;
                releaseRef.Attempt = currentRelease.Attempt;
                int num1 = returnSummary ? 1 : 0;
                int num2 = returnFailureDetails ? 1 : 0;
                string groupByCategory = reportingOptions.GroupByCategory;
                ref int local = ref runsCount;
                int runIdThreshold = progressOrFailed;
                runSummaryAndInsights = managementDatabase2.QueryTestRunSummaryAndInsightsForRelease(projectId1, sourceWorkflow1, releaseRef, num1 != 0, num2 != 0, groupByCategory, out local, runIdThreshold);
              }
              else
                runSummaryAndInsights = managementDatabase1.QueryTestRunSummaryAndInsightsForRelease(projectId, sourceWorkflow, currentRelease, returnSummary, returnFailureDetails, reportingOptions.GroupByCategory, out runsCount, progressOrFailed);
            }
          }
          if (runSummaryAndInsights.TestResultInsights.Any<ResultInsights>() && runSummaryAndInsights.TestResultInsights[0].PrevReleaseRefId > 0)
            previousRelease = (ReleaseReference) null;
          return this.ConvertServerOMToWebApiDataContract(context, runSummaryAndInsights, (BuildConfiguration) null, (BuildConfiguration) null, currentRelease, previousRelease, false, ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId), (List<TestCaseResult>) null);
        }
      }
      catch (Exception ex)
      {
        if (!(ex is TestObjectNotFoundException))
          TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
    }

    private void UpdateTestRunSummaryAndInsights2(
      IVssRequestContext context,
      GuidAndString projectId,
      ReleaseReference currentRelease,
      IEnumerable<IGrouping<int, TestRun>> groupRunByEnvironmentId)
    {
      try
      {
        ReleaseReference releaseReference1;
        if (groupRunByEnvironmentId == null)
        {
          releaseReference1 = (ReleaseReference) null;
        }
        else
        {
          IGrouping<int, TestRun> source = groupRunByEnvironmentId.FirstOrDefault<IGrouping<int, TestRun>>();
          releaseReference1 = source != null ? source.FirstOrDefault<TestRun>()?.ReleaseReference : (ReleaseReference) null;
        }
        ReleaseReference releaseReference2 = releaseReference1;
        if (releaseReference2 == null)
          return;
        DateTime releaseCreationDate = releaseReference2.ReleaseCreationDate;
        Dictionary<int, ReleaseReference> releasesForEnvDefIds = this.ReleaseServiceHelper.GetLastCompletedReleasesForEnvDefIds(context, projectId, currentRelease, releaseCreationDate);
        foreach (IGrouping<int, TestRun> source in groupRunByEnvironmentId)
        {
          ReleaseReference releaseReference3 = source.First<TestRun>().ReleaseReference;
          if (releasesForEnvDefIds != null && releasesForEnvDefIds.ContainsKey(releaseReference3.ReleaseEnvDefId))
            this.UpdateInsights(context, projectId, source.Select<TestRun, int>((Func<TestRun, int>) (testRun => testRun.TestRunId)).ToList<int>(), (BuildConfiguration) null, releaseReference3, TestResultsContextType.Release, (BuildConfiguration) null, releasesForEnvDefIds[releaseReference3.ReleaseEnvDefId]);
          else
            this.UpdateTestRunSummaryAndInsights(context, projectId, source.Select<TestRun, int>((Func<TestRun, int>) (testRun => testRun.TestRunId)).ToList<int>(), (BuildConfiguration) null, releaseReference3);
          IVssRequestContext context1 = context;
          Dictionary<string, object> keyValues = new Dictionary<string, object>();
          keyValues.Add("TotalNumberOfRunsWithoutSummary", (object) source.Count<TestRun>());
          keyValues.Add("SummaryAndInsightsCalculationFlow", (object) TelemetryParameterNames.SummaryAndInsightsCalculationFlowType.QueryFlow);
          ReleaseReference release = currentRelease;
          TestManagementServiceUtility.PublishTelemetry(context1, "CalculateSummaryAndInsights", keyValues, release);
        }
      }
      catch (Exception ex) when (!(ex is TestObjectNotFoundException))
      {
        TeamFoundationEventLog.Default.Log(context, ex.ToString(), TeamFoundationEventId.TestResultsReportsOperationFailedId, EventLogEntryType.Error);
        throw;
      }
    }

    private void BatchedUpdateRunSummaryAndInsightsForPipeline(
      IVssRequestContext context,
      GuidAndString projectId,
      PipelineReference pipelineReference)
    {
      this.BatchedUpdateRunSummaryAndInsights(context, projectId, TestRunPublishContext.Build, pipelineReference.PipelineId, (Action<List<TestRun>>) (testRuns =>
      {
        this.UpdateTestRunSummaryAndInsights(context, projectId, testRuns.Select<TestRun, int>((Func<TestRun, int>) (testRun => testRun.TestRunId)).ToList<int>(), testRuns.First<TestRun>().BuildReference, (ReleaseReference) null);
        IVssRequestContext context1 = context;
        Dictionary<string, object> keyValues = new Dictionary<string, object>();
        keyValues.Add("TotalNumberOfRunsWithoutSummary", (object) testRuns.Count);
        keyValues.Add("SummaryAndInsightsCalculationFlow", (object) TelemetryParameterNames.SummaryAndInsightsCalculationFlowType.QueryFlow);
        int pipelineId = pipelineReference.PipelineId;
        TestManagementServiceUtility.PublishTelemetry(context1, "CalculateSummaryAndInsights", keyValues, pipelineId: pipelineId);
      }));
    }

    private void BatchedUpdateRunSummaryAndInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      TestRunPublishContext publishContext,
      int id,
      Action<List<TestRun>> update)
    {
      if (context.IsFeatureEnabled("TestManagement.Server.AnonymousPublicUserFeature"))
      {
        TestManagementRequestContext managementRequestContext = this.GetTestManagementRequestContext(context);
        if (!managementRequestContext.SecurityManager.HasTestManagementPermission(managementRequestContext))
          return;
      }
      List<TestRun> runsWithoutInsights = this.GetTestRunsWithoutInsights(context, projectId, publishContext, id);
      List<TestRun> testRunList = TCMServiceDataMigrationRestHelper.FilterTfsRunsBelowThresholdFromTCM(new TestManagementRequestContext(context), runsWithoutInsights);
      testRunList?.RemoveAll((Predicate<TestRun>) (tr => tr.Type == (byte) 32));
      if (testRunList == null || testRunList.Count == 0)
        return;
      update(testRunList);
    }

    public List<TestResultSummary> QueryTestSummaryForReleases(
      IVssRequestContext context,
      GuidAndString projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases)
    {
      List<TestResultSummary> testSummaryList = new List<TestResultSummary>();
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap = new Dictionary<ReleaseReference, RunSummaryAndInsights>();
      TeamProjectReference projectReference = this.GetProjectReference(context, projectId.GuidId.ToString());
      List<ReleaseReference> list = releases.Select<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference, ReleaseReference>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference, ReleaseReference>) (r => this.GetServerOMReleaseRefFromReleaseRefContract(r))).ToList<ReleaseReference>();
      ReportingOptions reportingOptions = ReportingOptionsHelper.GetReportingOptions(this.GetTestManagementRequestContext(context), projectId);
      TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        runSummaryAndInsightsMap = managementDatabase.QueryTestRunSummaryForReleases(projectId, list, reportingOptions.GroupByCategory, 0);
      this.ConvertToWebAPIDataContract(context, releases, runSummaryAndInsightsMap, reportingOptions, projectReference, ref testSummaryList);
      TestManagementServiceUtility.PublishTelemetry(context, nameof (QueryTestSummaryForReleases), new Dictionary<string, object>()
      {
        {
          "TotalReleasesCount",
          (object) releases.Count
        }
      });
      return testSummaryList;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      IVssRequestContext context,
      GuidAndString projectId,
      ResultsFilter filter)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.QueryTestResultTrendReport"))
      {
        if (filter.TestCaseReferenceIds == null || !filter.TestCaseReferenceIds.Any<int>())
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context))
            return replicaAwareComponent.QueryTestResultTrendReport(projectId.GuidId, filter);
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            return managementDatabase.QueryTestResultTrendReport(projectId.GuidId, filter);
        }
      }
    }

    public List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      IVssRequestContext context,
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> aggrResults = new List<AggregatedDataForResultTrend>();
      using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.QueryTestResultTrendForBuild"))
      {
        if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamicForNonWIQLTypeQueries"))
        {
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            aggrResults = managementDatabase.QueryTestResultTrendForBuild3(projectId.GuidId, filter, true, progressOrFailed);
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            aggrResults = managementDatabase.QueryTestResultTrendForBuild(projectId.GuidId, filter);
        }
        this.UpdateTotalTestsInResultTrendData(context, aggrResults);
        aggrResults.Sort((Comparison<AggregatedDataForResultTrend>) ((x, y) => y.TestResultsContext.Build.Id.CompareTo(x.TestResultsContext.Build.Id)));
      }
      return aggrResults;
    }

    public List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      IVssRequestContext context,
      GuidAndString projectId,
      TestResultTrendFilter filter)
    {
      List<AggregatedDataForResultTrend> aggrResults = new List<AggregatedDataForResultTrend>();
      using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.QueryTestResultTrendForBuild"))
      {
        if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamicForNonWIQLTypeQueries"))
        {
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            aggrResults = managementDatabase.QueryTestResultTrendForRelease3(projectId.GuidId, filter, true, progressOrFailed);
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            aggrResults = managementDatabase.QueryTestResultTrendForRelease(projectId.GuidId, filter);
        }
        this.UpdateTotalTestsInResultTrendData(context, aggrResults);
        aggrResults.Sort((Comparison<AggregatedDataForResultTrend>) ((x, y) => y.TestResultsContext.Release.Id.CompareTo(x.TestResultsContext.Release.Id)));
      }
      return aggrResults;
    }

    private void SecureTestResultSummary(TestResultSummary summary)
    {
      summary?.AggregatedResultsAnalysis?.InitializeSecureObject((ISecuredObject) summary);
      summary?.TestFailures?.InitializeSecureObject((ISecuredObject) summary);
      summary?.TestResultsContext?.InitializeSecureObject((ISecuredObject) summary);
    }

    private void UpdateInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildRef,
      ReleaseReference releaseRef,
      TestResultsContextType contextType,
      BuildConfiguration lastSuccessfulBuild,
      ReleaseReference lastSuccessfulRelease)
    {
      int batchSize = TeamFoundationTestReportService.GetBatchSize(context);
      int count1 = 0;
      Dictionary<int, TestCaseResult> previousFailedResultsMap = (Dictionary<int, TestCaseResult>) null;
      int count2;
      for (; count1 < testRunIds.Count; count1 += count2)
      {
        count2 = Math.Min(testRunIds.Count - count1, batchSize);
        this.FetchFailureDetailsAndUpdateInsights(context, projectId, testRunIds.Skip<int>(count1).Take<int>(count2).ToList<int>(), buildRef, releaseRef, lastSuccessfulBuild, lastSuccessfulRelease, previousFailedResultsMap, contextType);
      }
    }

    private List<TestRun> GetTestRunsWithoutInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      TestRunPublishContext runContext,
      int id)
    {
      if (runContext.Equals((object) TestRunPublishContext.Build))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.GetTestRunIdsWithoutInsightsForBuild(projectId.GuidId, id);
      }
      else
      {
        if (!runContext.Equals((object) TestRunPublishContext.Release))
          return (List<TestRun>) null;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.GetTestRunIdsWithoutInsightsForRelease(projectId.GuidId, id);
      }
    }

    private void UpdateTotalTestsInResultTrendData(
      IVssRequestContext context,
      List<AggregatedDataForResultTrend> aggrResults)
    {
      foreach (AggregatedDataForResultTrend aggrResult in aggrResults)
      {
        aggrResult.TotalTests = 0;
        if (context.IsFeatureEnabled("TestManagement.Server.TriPassPercentageCalculation"))
        {
          int count1 = aggrResult.ResultsByOutcome.ContainsKey(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Passed) ? aggrResult.ResultsByOutcome[Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Passed].Count : 0;
          int count2 = aggrResult.ResultsByOutcome.ContainsKey(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed) ? aggrResult.ResultsByOutcome[Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed].Count : 0;
          aggrResult.TotalTests = count1 + count2;
        }
        else
        {
          foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome key in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) aggrResult.ResultsByOutcome.Keys)
          {
            if (key != Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.NotImpacted)
              aggrResult.TotalTests += aggrResult.ResultsByOutcome[key].Count;
          }
        }
      }
    }

    private void ConvertToWebAPIDataContract(
      IVssRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases,
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap,
      ReportingOptions reportingOptions,
      TeamProjectReference projectReference,
      ref List<TestResultSummary> testSummaryList)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "TeamFoundationTestReportService.ConvertToWebAPIDataContract"))
      {
        foreach (ReleaseReference key in runSummaryAndInsightsMap.Keys)
        {
          TestResultSummary webApiDataContract = this.ConvertServerOMToWebApiDataContract(context, runSummaryAndInsightsMap[key], (BuildConfiguration) null, (BuildConfiguration) null, key, (ReleaseReference) null, false, reportingOptions, (List<TestCaseResult>) null);
          webApiDataContract.TeamProject = projectReference;
          testSummaryList.Add(webApiDataContract);
        }
      }
    }

    private TestResultSummary ConvertServerOMToWebApiDataContract(
      IVssRequestContext context,
      RunSummaryAndInsights runSummaryAndInsights,
      BuildConfiguration currentBuild,
      BuildConfiguration prevBuild,
      ReleaseReference currentRelease,
      ReleaseReference prevRelease,
      bool populateInsights,
      ReportingOptions reportingOptions,
      List<TestCaseResult> allResultsByCategory)
    {
      TestResultSummary webApiDataContract = new TestResultSummary();
      webApiDataContract.TotalRunsCount = runSummaryAndInsights.TestRunSummary.TotalRunsCount;
      webApiDataContract.NoConfigRunsCount = runSummaryAndInsights.TestRunSummary.NoConfigRunsCount;
      List<RunSummaryByOutcome> list1 = runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Where<RunSummaryByOutcome>((Func<RunSummaryByOutcome, bool>) (aggr => aggr.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || aggr.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation || aggr.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Aborted)).ToList<RunSummaryByOutcome>();
      long aggregateDuration = this.CalculateAggregateDuration(context, (ISecuredObject) webApiDataContract, (IList<RunSummaryByOutcome>) list1);
      List<RunSummaryByOutcome> list2 = runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Where<RunSummaryByOutcome>((Func<RunSummaryByOutcome, bool>) (aggr => aggr.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed || aggr.TestRunState == Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation)).ToList<RunSummaryByOutcome>();
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> aggregateDataByOutCome1 = this.CalculateAggregateDataByOutCome(context, (ISecuredObject) webApiDataContract, (IList<RunSummaryByOutcome>) list2, out long _);
      int totalTests1;
      int passedCount1;
      int failedCount1;
      int othersCount1;
      this.CalculateResultCountByOutcomeForResultSummary(aggregateDataByOutCome1, out totalTests1, out passedCount1, out failedCount1, out othersCount1);
      webApiDataContract.AggregatedResultsAnalysis = new AggregatedResultsAnalysis((ISecuredObject) webApiDataContract);
      webApiDataContract.AggregatedResultsAnalysis.TotalTests = totalTests1;
      webApiDataContract.AggregatedResultsAnalysis.Duration = TimeSpan.FromMilliseconds((double) aggregateDuration);
      webApiDataContract.AggregatedResultsAnalysis.ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) aggregateDataByOutCome1;
      webApiDataContract.AggregatedResultsAnalysis.RunSummaryByState = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>) this.CalculateAggregateRunSummaryByState(context, (ISecuredObject) webApiDataContract, runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome, runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState);
      webApiDataContract.AggregatedResultsAnalysis.RunSummaryByOutcome = this.CalculateAggregateRunSummaryByOutcome((ISecuredObject) webApiDataContract, (IList<RunSummaryByOutcome>) list2);
      webApiDataContract.AggregatedResultsAnalysis.NotReportedResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) this.CalculateFlakyAggregateDataByOutCome((ISecuredObject) webApiDataContract, (IList<RunSummaryByOutcome>) list2);
      long aggregatedRunDurationInMs;
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> aggregateDataByOutCome2 = this.CalculateAggregateDataByOutCome(context, (ISecuredObject) webApiDataContract, runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome, out aggregatedRunDurationInMs);
      int totalTests2;
      int passedCount2;
      int failedCount2;
      int othersCount2;
      this.CalculateResultCountByOutcomeForResultSummary(aggregateDataByOutCome2, out totalTests2, out passedCount2, out failedCount2, out othersCount2);
      if (populateInsights)
      {
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference = new AggregatedResultsDifference((ISecuredObject) webApiDataContract);
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference.IncreaseInPassedTests = passedCount1 - passedCount2;
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference.IncreaseInFailures = failedCount1 - failedCount2;
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference.IncreaseInOtherTests = othersCount1 - othersCount2;
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference.IncreaseInTotalTests = webApiDataContract.AggregatedResultsAnalysis.TotalTests - totalTests2;
        webApiDataContract.AggregatedResultsAnalysis.ResultsDifference.IncreaseInDuration = webApiDataContract.AggregatedResultsAnalysis.Duration - TimeSpan.FromMilliseconds((double) aggregatedRunDurationInMs);
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList3 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
        foreach (ResultInsights testResultInsight in runSummaryAndInsights.TestResultInsights)
        {
          num1 += testResultInsight.NewFailures;
          num2 += testResultInsight.ExistingFailures;
          num3 += testResultInsight.FixedTests;
          int testRunId = testResultInsight.TestRunId;
          if (!string.IsNullOrEmpty(testResultInsight.NewFailedResults))
            resultIdentifierList1.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) this.GetResultIds(testResultInsight.NewFailedResults, testRunId, (ISecuredObject) webApiDataContract));
          if (!string.IsNullOrEmpty(testResultInsight.ExistingFailedResults))
            resultIdentifierList2.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) this.GetResultIds(testResultInsight.ExistingFailedResults, testRunId, (ISecuredObject) webApiDataContract));
          if (!string.IsNullOrEmpty(testResultInsight.FixedTestResults))
            resultIdentifierList3.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) this.GetResultIds(testResultInsight.FixedTestResults, testRunId, (ISecuredObject) webApiDataContract));
        }
        if (runSummaryAndInsights.TestResultInsights.Count<ResultInsights>() > 0)
        {
          webApiDataContract.TestFailures = new TestFailuresAnalysis((ISecuredObject) webApiDataContract);
          webApiDataContract.TestFailures.NewFailures = new TestFailureDetails((ISecuredObject) webApiDataContract.TestFailures)
          {
            Count = num1,
            TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList1
          };
          webApiDataContract.TestFailures.ExistingFailures = new TestFailureDetails((ISecuredObject) webApiDataContract.TestFailures)
          {
            Count = num2,
            TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList2
          };
          webApiDataContract.TestFailures.FixedTests = new TestFailureDetails((ISecuredObject) webApiDataContract.TestFailures)
          {
            Count = num3,
            TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList3
          };
        }
      }
      webApiDataContract.TestResultsContext = new TestResultsContext((ISecuredObject) webApiDataContract);
      if (currentRelease != null)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference serverOmReleaseRef = this.GetReleaseRefContractFromServerOMReleaseRef(currentRelease, (ISecuredObject) webApiDataContract);
        webApiDataContract.TestResultsContext.Release = serverOmReleaseRef;
        webApiDataContract.TestResultsContext.ContextType = TestResultsContextType.Release;
      }
      else if (currentBuild != null)
      {
        BuildReference buildConfiguration = this.GetBuildReferenceFromBuildConfiguration(currentBuild, (ISecuredObject) webApiDataContract);
        webApiDataContract.TestResultsContext.Build = buildConfiguration;
        webApiDataContract.TestResultsContext.ContextType = TestResultsContextType.Build;
      }
      webApiDataContract.AggregatedResultsAnalysis.PreviousContext = new TestResultsContext((ISecuredObject) webApiDataContract);
      if (prevRelease != null)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference serverOmReleaseRef = this.GetReleaseRefContractFromServerOMReleaseRef(prevRelease, (ISecuredObject) webApiDataContract);
        webApiDataContract.AggregatedResultsAnalysis.PreviousContext.Release = serverOmReleaseRef;
        webApiDataContract.AggregatedResultsAnalysis.PreviousContext.ContextType = TestResultsContextType.Release;
        if (webApiDataContract.TestFailures != null)
        {
          webApiDataContract.TestFailures.PreviousContext = new TestResultsContext((ISecuredObject) webApiDataContract);
          webApiDataContract.TestFailures.PreviousContext.Release = serverOmReleaseRef;
          webApiDataContract.TestFailures.PreviousContext.ContextType = TestResultsContextType.Release;
        }
      }
      else if (prevBuild != null)
      {
        BuildReference buildConfiguration = this.GetBuildReferenceFromBuildConfiguration(prevBuild, (ISecuredObject) webApiDataContract);
        webApiDataContract.AggregatedResultsAnalysis.PreviousContext.Build = buildConfiguration;
        webApiDataContract.AggregatedResultsAnalysis.PreviousContext.ContextType = TestResultsContextType.Build;
        if (webApiDataContract.TestFailures != null)
        {
          webApiDataContract.TestFailures.PreviousContext = new TestResultsContext((ISecuredObject) webApiDataContract);
          webApiDataContract.TestFailures.PreviousContext.Build = buildConfiguration;
          webApiDataContract.TestFailures.PreviousContext.ContextType = TestResultsContextType.Build;
        }
      }
      if (context.IsFeatureEnabled("TestManagement.Server.TRIReportCustomization") && ReportingOptionsHelper.GetRegistrySettingsEnabled(context, "/Service/TestManagement/Settings/CustomReportingOptions"))
        this.ExcludeNotReportedResultsFromReport(runSummaryAndInsights, webApiDataContract, aggregateDataByOutCome2, reportingOptions, allResultsByCategory, populateInsights);
      return webApiDataContract;
    }

    private IDictionary<TestRunOutcome, AggregatedRunsByOutcome> CalculateAggregateRunSummaryByOutcome(
      ISecuredObject testReportSecuredObject,
      IList<RunSummaryByOutcome> aggregateDataByOutcome)
    {
      Dictionary<TestRunOutcome, AggregatedRunsByOutcome> runsByOutcomeMap = new Dictionary<TestRunOutcome, AggregatedRunsByOutcome>();
      foreach (IEnumerable<RunSummaryByOutcome> source in aggregateDataByOutcome.GroupBy<RunSummaryByOutcome, int>((Func<RunSummaryByOutcome, int>) (rs => rs.TestRunId)))
      {
        int passedCount;
        int failedCount;
        int nonImpactedCount;
        this.CalculateResultCountByOutcomeForRunSummary(source.ToList<RunSummaryByOutcome>(), out int _, out passedCount, out failedCount, out nonImpactedCount, out int _);
        if (failedCount == 0 && passedCount > 0)
          TeamFoundationTestReportService.SafeUpdateRunOutcomeMap(runsByOutcomeMap, TestRunOutcome.Passed, testReportSecuredObject);
        else if (failedCount > 0)
          TeamFoundationTestReportService.SafeUpdateRunOutcomeMap(runsByOutcomeMap, TestRunOutcome.Failed, testReportSecuredObject);
        else if (nonImpactedCount > 0)
          TeamFoundationTestReportService.SafeUpdateRunOutcomeMap(runsByOutcomeMap, TestRunOutcome.NotImpacted, testReportSecuredObject);
        else
          TeamFoundationTestReportService.SafeUpdateRunOutcomeMap(runsByOutcomeMap, TestRunOutcome.Others, testReportSecuredObject);
      }
      return (IDictionary<TestRunOutcome, AggregatedRunsByOutcome>) runsByOutcomeMap;
    }

    private Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> CalculateAggregateDataByOutCome(
      IVssRequestContext context,
      ISecuredObject securedObject,
      IList<RunSummaryByOutcome> aggregateDataByOutcome,
      out long aggregatedRunDurationInMs)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> resultsByOutcomeMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
      SortedSet<RunSummaryByOutcome> runSummary = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
      aggregatedRunDurationInMs = 0L;
      foreach (RunSummaryByOutcome aggr in (IEnumerable<RunSummaryByOutcome>) aggregateDataByOutcome ?? Enumerable.Empty<RunSummaryByOutcome>())
      {
        if ((int) aggr.ResultMetadata != (int) Convert.ToByte((object) ResultMetadata.Flaky))
        {
          runSummary.Add(aggr);
          TeamFoundationTestReportService.SafeUpdateOutcomeMap(resultsByOutcomeMap, aggr, securedObject);
        }
      }
      aggregatedRunDurationInMs = TestManagementServiceUtility.CalculateEffectiveTestRunDuration(runSummary);
      return resultsByOutcomeMap;
    }

    private long CalculateAggregateDuration(
      IVssRequestContext context,
      ISecuredObject securedObject,
      IList<RunSummaryByOutcome> aggregateDataByOutcome)
    {
      SortedSet<RunSummaryByOutcome> runSummary = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
      foreach (RunSummaryByOutcome summaryByOutcome in (IEnumerable<RunSummaryByOutcome>) aggregateDataByOutcome ?? Enumerable.Empty<RunSummaryByOutcome>())
      {
        if ((int) summaryByOutcome.ResultMetadata != (int) Convert.ToByte((object) ResultMetadata.Flaky))
          runSummary.Add(summaryByOutcome);
      }
      return TestManagementServiceUtility.CalculateEffectiveTestRunDuration(runSummary);
    }

    private Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> CalculateFlakyAggregateDataByOutCome(
      ISecuredObject securedObject,
      IList<RunSummaryByOutcome> aggregateDataByOutcome)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> resultsByOutcomeMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
      SortedSet<RunSummaryByOutcome> sortedSet = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
      foreach (RunSummaryByOutcome aggr in (IEnumerable<RunSummaryByOutcome>) aggregateDataByOutcome ?? Enumerable.Empty<RunSummaryByOutcome>())
      {
        if ((int) aggr.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Flaky))
        {
          sortedSet.Add(aggr);
          TeamFoundationTestReportService.SafeUpdateOutcomeMap(resultsByOutcomeMap, aggr, securedObject);
        }
      }
      return resultsByOutcomeMap;
    }

    private Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState> CalculateAggregateRunSummaryByState(
      IVssRequestContext context,
      ISecuredObject testReportSecuredObject,
      IList<RunSummaryByOutcome> aggregateRunSummaryByOutcome,
      IList<RunSummaryByState> runsCountByState)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState> runSummaryByState1 = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>();
      foreach (RunSummaryByState runSummaryByState2 in (IEnumerable<RunSummaryByState>) runsCountByState ?? Enumerable.Empty<RunSummaryByState>())
        runSummaryByState1[(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) runSummaryByState2.RunState] = new AggregatedRunsByState(testReportSecuredObject)
        {
          State = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) runSummaryByState2.RunState,
          RunsCount = runSummaryByState2.RunsCount
        };
      foreach (IGrouping<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, RunSummaryByOutcome> source in aggregateRunSummaryByOutcome.GroupBy<RunSummaryByOutcome, Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState>((Func<RunSummaryByOutcome, Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState>) (rs => (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) rs.TestRunState)))
      {
        if (runSummaryByState1.ContainsKey(source.Key))
          runSummaryByState1[source.Key].ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) this.CalculateAggregateDataByOutCome(context, testReportSecuredObject, (IList<RunSummaryByOutcome>) source.ToList<RunSummaryByOutcome>(), out long _);
      }
      return runSummaryByState1;
    }

    private void CalculateResultCountByOutcomeForResultSummary(
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> aggregatedResultsByOutcomeMap,
      out int totalTests,
      out int passedCount,
      out int failedCount,
      out int othersCount)
    {
      othersCount = 0;
      passedCount = 0;
      failedCount = 0;
      totalTests = 0;
      if (aggregatedResultsByOutcomeMap.Values == null || !aggregatedResultsByOutcomeMap.Values.Any<AggregatedResultsByOutcome>())
        return;
      int nonImpactedCount;
      MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcomeInternal(aggregatedResultsByOutcomeMap.Values.Select<AggregatedResultsByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>((Func<AggregatedResultsByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>) (x => new KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>(x.Outcome, x.Count))).ToList<KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>(), out totalTests, out passedCount, out failedCount, out nonImpactedCount, out othersCount);
      totalTests -= nonImpactedCount;
      othersCount += nonImpactedCount;
    }

    private void CalculateResultCountByOutcomeForRunSummary(
      List<RunSummaryByOutcome> runsSummaryByOutcome,
      out int totalTests,
      out int passedCount,
      out int failedCount,
      out int nonImpactedCount,
      out int othersCount)
    {
      othersCount = 0;
      passedCount = 0;
      failedCount = 0;
      nonImpactedCount = 0;
      totalTests = 0;
      if (runsSummaryByOutcome == null || !runsSummaryByOutcome.Any<RunSummaryByOutcome>())
        return;
      MetricsCalculatorHelper.CalculateCountOfResultsForDifferentOutcomeInternal(runsSummaryByOutcome.Select<RunSummaryByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>((Func<RunSummaryByOutcome, KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>) (x => new KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) x.TestOutcome, x.ResultCount))).ToList<KeyValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, int>>(), out totalTests, out passedCount, out failedCount, out nonImpactedCount, out othersCount);
    }

    private void ExcludeNotReportedResultsFromReport(
      RunSummaryAndInsights runSummaryAndInsights,
      TestResultSummary testReport,
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> previousAggregatedResultsByOutcomeMap,
      ReportingOptions reportingOptions,
      List<TestCaseResult> allResultsByCategory,
      bool calculateAggregateDelta)
    {
      IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> dictionary1 = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
      IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> dictionary2 = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
      if (!string.IsNullOrEmpty(reportingOptions.GroupByCategory))
      {
        IResultsByCategoryBuilder byCategoryBuilder = ResultsByCategoryBuilderFactory.Create(reportingOptions);
        dictionary1 = byCategoryBuilder.GetAggregatesByCategory(runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByReportingCategory);
        if (runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByReportingCategory != null)
          dictionary2 = byCategoryBuilder.GetAggregatesByCategory(runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByReportingCategory);
        if (allResultsByCategory != null && allResultsByCategory.Any<TestCaseResult>())
        {
          HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultsByCategoryMap = this.GetFailedResultsByCategoryMap(byCategoryBuilder.FilterResultsByCategory(allResultsByCategory));
          if (testReport != null && testReport.TestFailures != null && resultsByCategoryMap != null)
          {
            this.UpdateFailureDetailsFilteredByCategory(testReport.TestFailures.NewFailures, resultsByCategoryMap);
            this.UpdateFailureDetailsFilteredByCategory(testReport.TestFailures.ExistingFailures, resultsByCategoryMap);
          }
        }
      }
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> dictionary3 = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
      foreach (AggregatedResultsByOutcome resultsByOutcome in (IEnumerable<AggregatedResultsByOutcome>) testReport.AggregatedResultsAnalysis.ResultsByOutcome.Values)
      {
        int num1 = 0;
        TimeSpan t2 = TimeSpan.FromTicks(0L);
        if (dictionary1.ContainsKey(resultsByOutcome.Outcome))
        {
          num1 = dictionary1[resultsByOutcome.Outcome].Count;
          t2 = dictionary1[resultsByOutcome.Outcome].Duration;
        }
        int num2 = 0;
        TimeSpan.FromTicks(0L);
        if (dictionary2.ContainsKey(resultsByOutcome.Outcome))
        {
          num2 = dictionary2[resultsByOutcome.Outcome].Count;
          TimeSpan duration = dictionary2[resultsByOutcome.Outcome].Duration;
        }
        if (previousAggregatedResultsByOutcomeMap.ContainsKey(resultsByOutcome.Outcome))
        {
          int count = previousAggregatedResultsByOutcomeMap[resultsByOutcome.Outcome].Count;
        }
        if (reportingOptions.NotReportedOutcomes.Contains((Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) resultsByOutcome.Outcome))
        {
          num1 = resultsByOutcome.Count;
          t2 = resultsByOutcome.Duration;
          if (previousAggregatedResultsByOutcomeMap.ContainsKey(resultsByOutcome.Outcome))
          {
            num2 = previousAggregatedResultsByOutcomeMap[resultsByOutcome.Outcome].Count;
            TimeSpan duration = previousAggregatedResultsByOutcomeMap[resultsByOutcome.Outcome].Duration;
          }
          dictionary1[resultsByOutcome.Outcome] = new AggregatedResultsByOutcome()
          {
            Count = resultsByOutcome.Count,
            Duration = resultsByOutcome.Duration,
            Outcome = resultsByOutcome.Outcome
          };
        }
        resultsByOutcome.Count -= num1;
        resultsByOutcome.Duration = TeamFoundationTestReportService.SafeSubtractTimeSpan(resultsByOutcome.Duration, t2);
        if (resultsByOutcome.Outcome != Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.NotImpacted)
          testReport.AggregatedResultsAnalysis.TotalTests -= num1;
        if (calculateAggregateDelta)
        {
          int num3 = num1 - num2;
          testReport.AggregatedResultsAnalysis.ResultsDifference.IncreaseInTotalTests -= num3;
          if (resultsByOutcome.Outcome == Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Passed)
            testReport.AggregatedResultsAnalysis.ResultsDifference.IncreaseInPassedTests -= num3;
          else if (resultsByOutcome.Outcome == Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed)
            testReport.AggregatedResultsAnalysis.ResultsDifference.IncreaseInFailures -= num3;
          else
            testReport.AggregatedResultsAnalysis.ResultsDifference.IncreaseInOtherTests -= num3;
        }
        if (resultsByOutcome.Count > 0)
          dictionary3.Add(resultsByOutcome.Outcome, resultsByOutcome);
      }
      testReport.AggregatedResultsAnalysis.ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) dictionary3;
      if (testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome.Count == 0)
      {
        testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome = dictionary1;
      }
      else
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> keyValuePair in (IEnumerable<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>>) dictionary1)
        {
          if (testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome.ContainsKey(keyValuePair.Key))
          {
            testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome[keyValuePair.Key].Count += keyValuePair.Value.Count;
            testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome[keyValuePair.Key].Duration += keyValuePair.Value.Duration;
          }
          else
            testReport.AggregatedResultsAnalysis.NotReportedResultsByOutcome[keyValuePair.Key] = new AggregatedResultsByOutcome()
            {
              Count = keyValuePair.Value.Count,
              Duration = keyValuePair.Value.Duration,
              Outcome = keyValuePair.Value.Outcome
            };
        }
      }
    }

    private void UpdateFailureDetailsFilteredByCategory(
      TestFailureDetails failureDetails,
      HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> failedResultsByCategoryMap)
    {
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> testResults = failureDetails.TestResults;
      if (testResults == null || !testResults.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>())
        return;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIdentifierList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier resultIdentifier in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) testResults)
      {
        if (!failedResultsByCategoryMap.Contains(resultIdentifier))
          resultIdentifierList.Add(resultIdentifier);
      }
      failureDetails.TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIdentifierList;
      failureDetails.Count = failureDetails.TestResults.Count;
    }

    private HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> GetFailedResultsByCategoryMap(
      List<TestCaseResult> filteredResults)
    {
      HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultsByCategoryMap = new HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      if (filteredResults != null)
      {
        foreach (TestCaseResult filteredResult in filteredResults)
        {
          if (filteredResult.Outcome == (byte) 3)
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier resultIdentifier = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier()
            {
              TestRunId = filteredResult.TestRunId,
              TestResultId = filteredResult.TestResultId
            };
            resultsByCategoryMap.Add(resultIdentifier);
          }
        }
      }
      return resultsByCategoryMap;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> GetResultIds(
      string failureDetails,
      int runId,
      ISecuredObject securedObject)
    {
      string[] strArray = failureDetails.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIds = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      foreach (string str in strArray)
        resultIds.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier(securedObject)
        {
          TestRunId = runId,
          TestResultId = Convert.ToInt32(str)
        });
      return resultIds;
    }

    private BuildReference GetBuildReferenceFromBuildConfiguration(
      BuildConfiguration buildConfig,
      ISecuredObject securedObject)
    {
      return new BuildReference(securedObject)
      {
        Id = buildConfig.BuildId,
        Number = buildConfig.BuildNumber,
        Uri = buildConfig.BuildUri,
        BuildSystem = buildConfig.BuildSystem
      };
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference GetReleaseRefContractFromServerOMReleaseRef(
      ReleaseReference serverReleaseRef,
      ISecuredObject securedObject)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference(securedObject)
      {
        Id = serverReleaseRef.ReleaseId,
        EnvironmentId = serverReleaseRef.ReleaseEnvId,
        DefinitionId = serverReleaseRef.ReleaseDefId,
        EnvironmentDefinitionId = serverReleaseRef.ReleaseEnvDefId
      };
    }

    private ReleaseReference GetServerOMReleaseRefFromReleaseRefContract(Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseRef) => new ReleaseReference()
    {
      ReleaseId = releaseRef.Id,
      ReleaseEnvId = releaseRef.EnvironmentId,
      ReleaseDefId = releaseRef.DefinitionId,
      ReleaseEnvDefId = releaseRef.EnvironmentDefinitionId
    };

    private static List<TestCaseResult> GetResultsByCategory(
      IVssRequestContext context,
      GuidAndString projectId,
      BuildConfiguration build,
      string sourceWorkflow,
      string category)
    {
      List<TestCaseResult> testCaseResults = new List<TestCaseResult>();
      if (build != null && !string.IsNullOrEmpty(sourceWorkflow) && !string.IsNullOrEmpty(category))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testCaseResults = managementDatabase.QueryTestResultsByCategory(projectId, sourceWorkflow, build, category);
      }
      return TCMServiceDataMigrationRestHelper.FilterTfsResultsBelowThresholdFromTCM(new TestManagementRequestContext(context), testCaseResults);
    }

    private static void SafeUpdateOutcomeMap(
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> resultsByOutcomeMap,
      RunSummaryByOutcome aggr,
      ISecuredObject securedObject)
    {
      if (!resultsByOutcomeMap.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome))
      {
        resultsByOutcomeMap[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome] = new AggregatedResultsByOutcome(securedObject)
        {
          Count = aggr.ResultCount,
          Outcome = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome,
          Duration = TimeSpan.FromMilliseconds((double) Validator.CheckOverflowAndGetSafeValue(aggr.ResultDuration, 0L)),
          RerunResultCount = (int) aggr.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Rerun) ? aggr.ResultCount : 0
        };
      }
      else
      {
        resultsByOutcomeMap[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome].Count += aggr.ResultCount;
        resultsByOutcomeMap[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome].RerunResultCount += (int) aggr.ResultMetadata == (int) Convert.ToByte((object) ResultMetadata.Rerun) ? aggr.ResultCount : 0;
        long safeValue = Validator.CheckOverflowAndGetSafeValue((long) resultsByOutcomeMap[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome].Duration.TotalMilliseconds, aggr.ResultDuration);
        resultsByOutcomeMap[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) aggr.TestOutcome].Duration = TimeSpan.FromMilliseconds((double) safeValue);
      }
    }

    private static void SafeUpdateRunOutcomeMap(
      Dictionary<TestRunOutcome, AggregatedRunsByOutcome> runsByOutcomeMap,
      TestRunOutcome testRunOutcome,
      ISecuredObject securedObject)
    {
      if (!runsByOutcomeMap.ContainsKey(testRunOutcome))
        runsByOutcomeMap[testRunOutcome] = new AggregatedRunsByOutcome(securedObject)
        {
          RunsCount = 1,
          Outcome = testRunOutcome
        };
      else
        ++runsByOutcomeMap[testRunOutcome].RunsCount;
    }

    private static TimeSpan SafeSubtractTimeSpan(TimeSpan t1, TimeSpan t2)
    {
      TimeSpan timeSpan = t1 - t2;
      if (timeSpan.TotalMilliseconds < 0.0)
        timeSpan = TimeSpan.FromTicks(0L);
      return timeSpan;
    }

    internal IBuildConfiguration BuildConfigurationHelper
    {
      get => this._buildConfigurationHelper ?? (IBuildConfiguration) new BuildConfiguration();
      set => this._buildConfigurationHelper = value;
    }

    internal IReleaseServiceHelper ReleaseServiceHelper
    {
      get => this._releaseServiceHelper ?? (IReleaseServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper();
      set => this._releaseServiceHelper = value;
    }
  }
}
