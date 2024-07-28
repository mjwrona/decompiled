// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Hub;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class RunsHelper : RestApiHelper
  {
    private StatisticsHelper m_statisticsHelper;
    protected DataContractConverter m_dataContractConverter;

    public RunsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> QueryTestRuns(
      Guid projectId,
      UrlHelper url,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state,
      string planIds,
      bool? isAutomated,
      TestRunPublishContext? publishContext,
      string buildIds,
      string buildDefIds,
      string branchName,
      string releaseIds,
      string releaseDefIds,
      string releaseEnvIds,
      string releaseEnvDefIds,
      string runTitle,
      int top,
      string continuationToken,
      out string mergedContinuationToken)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (!string.IsNullOrEmpty(continuationToken))
      {
        List<string> commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(continuationToken);
        empty1 = commaSeparatedString.Count > 0 ? commaSeparatedString[0] : (string) null;
        empty2 = commaSeparatedString.Count > 1 ? commaSeparatedString[1] : (string) null;
      }
      List<int> listOfIds1 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, planIds, nameof (planIds));
      List<int> listOfIds2 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildIds, nameof (buildIds));
      List<int> listOfIds3 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildDefIds, nameof (buildDefIds));
      List<int> listOfIds4 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseIds, nameof (releaseIds));
      List<int> listOfIds5 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseDefIds, nameof (releaseDefIds));
      List<int> listOfIds6 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvIds, nameof (releaseEnvIds));
      List<int> listOfIds7 = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvDefIds, nameof (releaseEnvDefIds));
      bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
      bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
      if (!flag1 && !flag2)
        return this.QueryTestRunsAndMerge(projectId, url, minLastUpdatedDate, maxLastUpdatedDate, state, (IList<int>) listOfIds1, isAutomated, publishContext, (IList<int>) listOfIds2, (IList<int>) listOfIds3, branchName, (IList<int>) listOfIds4, (IList<int>) listOfIds5, (IList<int>) listOfIds6, (IList<int>) listOfIds7, runTitle, top, empty1, empty2, out mergedContinuationToken);
      if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
      TestRunsByFilter testRunsByFilter = this.TestManagementRunService.GetTestRunsByFilter(this.TestManagementRequestContext, projectId.ToString(), url, minLastUpdatedDate, maxLastUpdatedDate, state, (IList<int>) listOfIds1, isAutomated, publishContext, (IList<int>) listOfIds2, (IList<int>) listOfIds3, branchName, (IList<int>) listOfIds4, (IList<int>) listOfIds5, (IList<int>) listOfIds6, (IList<int>) listOfIds7, runTitle, top, empty1);
      mergedContinuationToken = testRunsByFilter.ContinuationToken;
      return testRunsByFilter.TestRuns;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRuns(
      string projectId,
      string buildUri,
      string ownerId,
      string tmiRunId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top)
    {
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) skip, nameof (skip), this.RequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) top, nameof (top), this.RequestContext.ServiceName);
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.GetTestRuns projectId = {0}", (object) projectId);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>("RunsHelper.GetTestRuns", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        this.CheckForViewTestResultPermission(projectReference.Name);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
          return this.GetTestRunsInternal(projectReference, buildUri, ownerId, tmiRunId, planId, includeRunDetails, automated, skip, top);
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return !string.IsNullOrEmpty(tmiRunId) ? this.QueryTestRunByTmiRunId(tmiRunId, projectReference) : this.GetTestRunsInBatches(projectReference, buildUri, ownerId, planId, includeRunDetails, automated, skip, top);
      }), 1015051, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(
      string projectId,
      int runId,
      bool includeDetails,
      bool includeTags = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.GetTestRunById projectId = {0}, runId = {1}", (object) projectId, (object) runId);
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>("RunsHelper.GetTestRunById", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        this.CheckForViewTestResultPermission(projectReference.Name);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun;
        if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunById(this.RequestContext, projectReference.Id, runId, includeDetails, out testRun))
        {
          int result;
          if (testRun.Plan != null && int.TryParse(testRun.Plan.Id, out result) && result > 0)
          {
            testRun.Iteration = this.TestManagementRequestContext.PlannedTestResultsHelper.GetTestPlanIteration(this.TestManagementRequestContext, projectReference.Name, result);
            if (string.IsNullOrEmpty(testRun.Plan.Name) & includeDetails)
              testRun.Plan = this.TestManagementRequestContext.PlannedTestResultsHelper.GetShallowTestPlan(this.TestManagementRequestContext, projectReference.Name, result);
          }
          this.DataContractConverter.SecureTestRunWebApiObject(testRun, projectReference.Id);
          return testRun;
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.DataContractConverter.ConvertTestRunToDataContract(this.TestManagementRunService.GetTestRunById(this.TestManagementRequestContext, runId, projectReference, includeTags), projectReference, true, includeDetails, true);
      }), 1015051, "TestManagement");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsByQuery(
      string projectId,
      QueryModel queryString,
      bool includeIdsOnly,
      bool includeRunDetails,
      int skip,
      int top)
    {
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.GetTestRuns projectId = {0}", (object) projectId);
      ArgumentUtility.CheckForNull<QueryModel>(queryString, "query", "Test Results");
      ArgumentUtility.CheckStringForNullOrEmpty(queryString.Query, "query", "Test Results");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>("RunsHelper.GetTestRunsByQuery", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TimeZoneInfo utc = TimeZoneInfo.Utc;
        ResultsStoreQuery query = new ResultsStoreQuery();
        query.QueryText = queryString.Query;
        query.TeamProjectName = projectReference.Name;
        query.TimeZone = utc.ToSerializedString();
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
          return this.GetTestRunsByQueryAndMerge(projectReference, query, queryString, includeIdsOnly, includeRunDetails, skip, top);
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.GetTestRunsByQueryInternal(projectReference, query, includeRunDetails, skip, top);
      }), 1015051, "TestManagement");
    }

    public void DeleteTestRun(string projectId, int runId)
    {
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.DeleteTestRun projectId = {0}, runId = {1}", (object) projectId, (object) runId);
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      this.ExecuteAction<object>("RunsHelper.DeleteTestRun", (Func<object>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        this.CheckForViewTestResultPermission(name);
        bool flag = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM") && !flag && this.TestManagementRequestContext.TcmServiceHelper.TryDeleteTestRun(this.RequestContext, projectReference.Id, runId))
          return new object();
        if (flag && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        TestRun.Delete(this.TestManagementRequestContext, new int[1]
        {
          runId
        }, name);
        return new object();
      }), 1015051, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun PatchTestRun(
      string projectId,
      int runId,
      RunUpdateModel runUpdateModel)
    {
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.PatchTestRun projectId = {0}, runId = {1}", (object) projectId, (object) runId);
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>("RunsHelper.PatchTestRun", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun updatedTestRun = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRun) null;
        bool flag1 = false;
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag3 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag2 && !flag3)
          flag1 = this.TestManagementRequestContext.TcmServiceHelper.TryUpdateTestRun(this.RequestContext, projectReference.Id, runId, runUpdateModel, out updatedTestRun);
        else if (flag2 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        if (!flag1)
        {
          byte runType = 0;
          if (runUpdateModel != null)
            runType = this.ValidateArgumentInUpdateForNonConfigRun(runUpdateModel.RunSummary, runId, projectReference, "RunUpdateModel");
          updatedTestRun = this.TestManagementRunService.UpdateTestRun(this.TestManagementRequestContext, projectReference, runId, runUpdateModel);
          if (runUpdateModel != null)
            this.TestManagementRunService.UpdateTestRunSummaryForNonConfigRuns(this.TestManagementRequestContext, projectReference, runId, runUpdateModel.RunSummary, runType);
        }
        this.NotifyClientsAboutTestRunChange(updatedTestRun, projectReference.Id);
        return updatedTestRun;
      }), 1015051, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      string projectId,
      RunCreateModel testRun)
    {
      ArgumentUtility.CheckForNull<RunCreateModel>(testRun, nameof (testRun), "Test Results");
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.CreateTestRun projectId = {0}", (object) projectId);
      RunType result1;
      Enum.TryParse<RunType>(testRun.Type, out result1);
      if (result1 == RunType.NoConfigRun && !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.NonConfigRun"))
        throw new NotSupportedException(ServerResources.NonConfigNotSupported);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>("RunsHelper.CreateTestRun", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        this.UpdateTestRunContext(projectReference.Id, testRun);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun;
        if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestRun(this.RequestContext, projectReference.Id, testRun, out newTestRun))
        {
          if (!this.TestManagementRequestContext.IsTcmService && testRun.PointIds != null && ((IEnumerable<int>) testRun.PointIds).Any<int>())
          {
            int result2;
            if (string.IsNullOrEmpty(testRun.Plan.Id) || !int.TryParse(testRun.Plan.Id, out result2) || result2 <= 0)
              throw new InvalidPropertyException("plan.Id", ServerResources.InvalidPropertyMessage);
            List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults;
            this.DataContractConverter.PopulatePlannedResultDetailsFromCreateModel(name, testRun, result2, out testCaseResults);
            this.TestManagementRequestContext.TcmServiceHelper.TryAddTestResultsToTestRun(this.RequestContext, projectReference.Id, newTestRun.Id, testCaseResults.ToArray(), out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> _);
          }
          return newTestRun;
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        TestRun testRun1;
        List<TestCaseResult> testCaseResults1;
        this.DataContractConverter.PopulateRunAndResultFromCreateModel(name, testRun, out testRun1, out testCaseResults1);
        this.ValidateArgumentForNonConfigRun(testRun.RunSummary, testRun1.Type, "RunCreateModel");
        TestRun testRun2 = this.TestManagementRunService.CreateTestRun(this.TestManagementRequestContext, testRun1, projectReference, (IList<TestCaseResult>) testCaseResults1, false, true);
        this.TestManagementRunService.UpdateTestRunSummaryForNonConfigRuns(this.TestManagementRequestContext, projectReference, testRun2.TestRunId, testRun.RunSummary, testRun2.Type);
        return this.DataContractConverter.ConvertTestRunToDataContract(testRun2, projectReference, true, false, false);
      }), 1015051, "TestManagement");
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetShallowTestRunsByQueryInternal(
      TeamProjectReference projectReference,
      ResultsStoreQuery query,
      int skip,
      int top)
    {
      return this.TestManagementRunService.QueryTestRunIds(this.TestManagementRequestContext, query).Skip<int>(skip).Take<int>(top).Select<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (runId => new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun()
      {
        Id = runId,
        Name = string.Empty,
        Url = string.Empty
      })).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsByQueryInternal(
      TeamProjectReference projectReference,
      ResultsStoreQuery query,
      bool includeRunDetails,
      int skip,
      int top)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> source = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      foreach (TestRun queryTestRun in this.TestManagementRunService.QueryTestRuns(this.TestManagementRequestContext, query))
        source.Add(this.DataContractConverter.ConvertTestRunToDataContract(queryTestRun, projectReference, includeRunDetails, false, true));
      return source.Skip<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>(skip).Take<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>(top).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
    }

    private byte ValidateArgumentInUpdateForNonConfigRun(
      IList<RunSummaryModel> runSummaryModels,
      int runId,
      TeamProjectReference projectReference,
      string modelName)
    {
      TestRun testRunById = this.TestManagementRunService.GetTestRunById(this.TestManagementRequestContext, runId, projectReference);
      this.ValidateArgumentForNonConfigRun(runSummaryModels, testRunById.Type, modelName);
      return testRunById.Type;
    }

    private void ValidateArgumentForNonConfigRun(
      IList<RunSummaryModel> runSummaryModels,
      byte typeOfRun,
      string modelName)
    {
      if (runSummaryModels == null || !runSummaryModels.Any<RunSummaryModel>())
        return;
      if (typeOfRun != (byte) 32)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RunSummaryWithRunTypeErrorMsg, (object) modelName));
      if (runSummaryModels.GroupBy<RunSummaryModel, TestOutcome>((Func<RunSummaryModel, TestOutcome>) (runSummary => runSummary.TestOutcome)).Any<IGrouping<TestOutcome, RunSummaryModel>>((Func<IGrouping<TestOutcome, RunSummaryModel>, bool>) (group => group.Count<RunSummaryModel>() > 1)))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RunSummaryDuplicateEntry, (object) modelName));
      foreach (RunSummaryModel runSummaryModel in (IEnumerable<RunSummaryModel>) runSummaryModels)
      {
        if (runSummaryModel.ResultCount < 0)
          throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RunSummaryNegativeValueError, (object) "ResultCount", (object) modelName, (object) runSummaryModel.TestOutcome.ToString()));
        if (runSummaryModel.Duration < 0L)
          throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RunSummaryNegativeValueError, (object) "Duration", (object) modelName, (object) runSummaryModel.TestOutcome.ToString()));
      }
    }

    private void UpdateTestRunContext(Guid projectId, RunCreateModel runCreateModel)
    {
      int artifactBuildId;
      string artifactProjectId;
      string artifactType;
      this.UpdateReleaseReference(projectId, runCreateModel, out artifactBuildId, out artifactProjectId, out artifactType);
      if (runCreateModel.ReleaseReference != null)
        runCreateModel.PipelineReference = (PipelineReference) null;
      this.UpdateBuildAndPipelineReference(projectId, runCreateModel, artifactBuildId, artifactProjectId, artifactType);
    }

    private void UpdateReleaseReference(
      Guid projectId,
      RunCreateModel runCreateModel,
      out int artifactBuildId,
      out string artifactProjectId,
      out string artifactType)
    {
      artifactProjectId = (string) null;
      artifactBuildId = 0;
      artifactType = (string) null;
      if (runCreateModel.ReleaseReference != null && runCreateModel.ReleaseReference.Id > 0 && runCreateModel.ReleaseReference.EnvironmentId > 0 || string.IsNullOrEmpty(runCreateModel.ReleaseUri) || string.IsNullOrEmpty(runCreateModel.ReleaseEnvironmentUri))
        return;
      ReleaseReference releaseReference = this.ReleaseServiceHelper.QueryReleaseReferenceByUri(this.m_requestContext, new GuidAndString(string.Empty, projectId), runCreateModel.ReleaseUri, runCreateModel.ReleaseEnvironmentUri);
      if (releaseReference == null)
        throw new InvalidPropertyException(string.Format(ServerResources.ReleaseEnvironmentNotFound, (object) runCreateModel.ReleaseUri, (object) runCreateModel.ReleaseEnvironmentUri));
      runCreateModel.ReleaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
      {
        Id = releaseReference.ReleaseId,
        EnvironmentId = releaseReference.ReleaseEnvId,
        DefinitionId = releaseReference.ReleaseDefId,
        EnvironmentDefinitionId = releaseReference.ReleaseEnvDefId,
        Name = releaseReference.ReleaseName,
        EnvironmentName = releaseReference.ReleaseEnvName,
        CreationDate = releaseReference.ReleaseCreationDate,
        EnvironmentCreationDate = releaseReference.EnvironmentCreationDate,
        Attempt = releaseReference.Attempt
      };
      artifactBuildId = releaseReference.PrimaryArtifactBuildId;
      artifactProjectId = releaseReference.PrimaryArtifactProjectId;
      artifactType = releaseReference.PrimaryArtifactType;
    }

    private void UpdateBuildAndPipelineReference(
      Guid projectId,
      RunCreateModel runCreateModel,
      int artifactBuildId,
      string artifactProjectId,
      string artifactType)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.PipelineAwareTestRuns"))
        runCreateModel.PipelineReference = (PipelineReference) null;
      else
        PipelineReferenceHelper.ValidateAndUpdatePipelineReferenceWhileCreatingRun(runCreateModel.PipelineReference);
      if (runCreateModel.BuildReference != null && runCreateModel.BuildReference.Id > 0 || artifactType != null && !artifactType.Equals("Build", StringComparison.OrdinalIgnoreCase))
        return;
      int result1 = 0;
      if (runCreateModel.Build != null && int.TryParse(runCreateModel.Build.Id, out result1) && runCreateModel.PipelineReference != null && runCreateModel.PipelineReference.PipelineId != result1)
        throw new InvalidPropertyException(string.Format(ServerResources.BuildIdAndPipelineIdAreNotEqual, (object) result1, (object) runCreateModel.PipelineReference.PipelineId));
      if (result1 == 0 && runCreateModel.PipelineReference != null)
        result1 = runCreateModel.PipelineReference.PipelineId;
      if (result1 <= 0)
        return;
      Guid result2;
      BuildConfiguration buildConfiguration = result1 != artifactBuildId || artifactProjectId == null || !Guid.TryParse(artifactProjectId, out result2) || !(result2 != Guid.Empty) ? this.BuildServiceHelper.QueryBuildConfigurationById(this.m_requestContext, projectId, result1) : this.BuildServiceHelper.QueryBuildConfigurationById(this.m_requestContext, result2, result1);
      if (buildConfiguration == null)
        throw new InvalidPropertyException(string.Format(ServerResources.BuildNotFound, (object) result1));
      runCreateModel.BuildReference = new Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration()
      {
        Id = buildConfiguration.BuildId,
        Number = buildConfiguration.BuildNumber,
        BranchName = buildConfiguration.BranchName,
        BuildDefinitionId = buildConfiguration.BuildDefinitionId,
        SourceVersion = buildConfiguration.SourceVersion,
        Flavor = runCreateModel.BuildFlavor != null ? runCreateModel.BuildFlavor : string.Empty,
        Platform = runCreateModel.BuildPlatform != null ? runCreateModel.BuildPlatform : string.Empty,
        CreationDate = buildConfiguration.CreatedDate,
        BuildSystem = buildConfiguration.BuildSystem,
        RepositoryType = buildConfiguration.RepositoryType,
        Uri = buildConfiguration.BuildUri,
        RepositoryGuid = buildConfiguration.RepositoryId,
        TargetBranchName = runCreateModel.BuildReference?.TargetBranchName
      };
    }

    private void NotifyClientsAboutTestRunChange(Microsoft.TeamFoundation.TestManagement.WebApi.TestRun updatedTestRun, Guid projectId)
    {
      try
      {
        if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.SignalRReportingRuns") || !(updatedTestRun.State == "Completed"))
          return;
        if (updatedTestRun.IsAutomated && updatedTestRun.Release == null && updatedTestRun.Build != null)
        {
          this.RequestContext.GetService<IBuildTestHubDispatcher>().HandleTestRunStatsChange(this.TestManagementRequestContext, projectId, int.Parse(updatedTestRun.Build.Id));
        }
        else
        {
          if (!updatedTestRun.IsAutomated || updatedTestRun.Release == null)
            return;
          this.RequestContext.GetService<IReleaseTestHubDispatcher>().HandleTestRunStatsChange(this.TestManagementRequestContext, projectId, updatedTestRun.Release.Id, updatedTestRun.Release.EnvironmentId);
        }
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.TraceException("RestLayer", ex);
      }
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> QueryTestRunByTmiRunId(
      string tmiRunIdParam,
      TeamProjectReference projectReference)
    {
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>("RunsHelper.QueryTestRunByTmiRunId", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() =>
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
        Guid result;
        if (!Guid.TryParse(tmiRunIdParam, out result))
          throw new InvalidPropertyException("TmiRunId", ServerResources.InvalidPropertyMessage);
        testRunList.Add(this.DataContractConverter.ConvertTestRunToDataContract(TestRun.QueryTestRunByTmiRunId(this.TestManagementRequestContext, result), projectReference, true, false, false));
        return testRunList;
      }), 1015051, "TestManagement");
    }

    private List<TestRun> QueryTestRuns(
      GuidAndString projectId,
      string buildUriParam,
      string ownerIdParam,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      return this.ExecuteAction<List<TestRun>>("RunsHelper.QueryTestRuns", (Func<List<TestRun>>) (() =>
      {
        string buildUri = string.Empty;
        Guid owner = Guid.Empty;
        if (!string.IsNullOrEmpty(buildUriParam))
          buildUri = buildUriParam;
        Guid result;
        if (!string.IsNullOrEmpty(ownerIdParam) && Guid.TryParse(ownerIdParam, out result))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityByAccountId(result);
          if (identity != null)
            owner = identity.Id;
        }
        return TestRun.Query2(this.TestManagementRequestContext, 0, owner, buildUri, projectId, planId, skip, top);
      }), 1015051, "TestManagement");
    }

    private bool IsDateTimeDefault(DateTime date) => DateTime.Compare(date.Kind != DateTimeKind.Utc ? date.ToUniversalTime() : date, new DateTime().ToUniversalTime()) == 0;

    private int DateTimeCompare(DateTime date1, DateTime date2) => DateTime.Compare(date1.Kind != DateTimeKind.Utc ? date1.ToUniversalTime() : date1, date2.Kind != DateTimeKind.Utc ? date2.ToUniversalTime() : date2);

    private void AdjustStartAndCompleteDateForTestRunIfRequired(TestRun testRun)
    {
      if (this.IsDateTimeDefault(testRun.CompleteDate))
        return;
      if (this.IsDateTimeDefault(testRun.StartDate))
      {
        testRun.StartDate = testRun.CompleteDate;
      }
      else
      {
        if (this.DateTimeCompare(testRun.CompleteDate, testRun.StartDate) >= 0)
          return;
        testRun.CompleteDate = testRun.StartDate;
      }
    }

    private IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> QueryTestRunsAndMerge(
      Guid projectId,
      UrlHelper url,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state,
      IList<int> planIdList,
      bool? isAutomated,
      TestRunPublishContext? publishContext,
      IList<int> buildIdList,
      IList<int> buildDefIdList,
      string branchName,
      IList<int> releaseIdList,
      IList<int> releaseDefIdList,
      IList<int> releaseEnvIdList,
      IList<int> releaseEnvDefIdList,
      string runTitle,
      int top,
      string localSkip,
      string externalSkip,
      out string mergedContinuationToken)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        if (localSkip == null)
          return;
        TestRunsByFilter testRunsByFilter = this.TestManagementRunService.GetTestRunsByFilter(this.TestManagementRequestContext, projectId.ToString(), url, minLastUpdatedDate, maxLastUpdatedDate, state, planIdList, isAutomated, publishContext, buildIdList, buildDefIdList, branchName, releaseIdList, releaseDefIdList, releaseEnvIdList, releaseEnvDefIdList, runTitle, top, localSkip);
        if (testRunsByFilter.TestRuns != null && testRunsByFilter.TestRuns.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>())
          runs.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) testRunsByFilter.TestRuns);
        localSkip = testRunsByFilter.ContinuationToken;
      }), this.RequestContext);
      int top1 = top - runs.Count;
      runs = TCMServiceDataMigrationRestHelper.FilterTfsRunsBelowThresholdFromTCM(this.TestManagementRequestContext, runs.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>());
      IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs1;
      if (top1 > 0 && externalSkip != null && this.TestManagementRequestContext.TcmServiceHelper.TryQueryTestRuns(this.RequestContext, projectId, minLastUpdatedDate, maxLastUpdatedDate, state, (IEnumerable<int>) planIdList, isAutomated, publishContext, (IEnumerable<int>) buildIdList, (IEnumerable<int>) buildDefIdList, branchName, (IEnumerable<int>) releaseIdList, (IEnumerable<int>) releaseDefIdList, (IEnumerable<int>) releaseEnvIdList, (IEnumerable<int>) releaseEnvDefIdList, runTitle, top1, externalSkip, out runs1))
      {
        runs = this.TestManagementRequestContext.MergeDataHelper.MergeTestRuns(runs.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>(), runs1 != null ? runs1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null);
        if (runs1 != null)
        {
          int count = runs1.Count;
        }
        List<string> commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(runs1?.ContinuationToken);
        externalSkip = commaSeparatedString.Count > 0 ? commaSeparatedString[0] : string.Empty;
      }
      mergedContinuationToken = !string.IsNullOrEmpty(localSkip) || !string.IsNullOrEmpty(externalSkip) ? string.Format("{0},{1}", (object) localSkip, (object) externalSkip) : (string) null;
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) runs;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsByQueryAndMerge(
      TeamProjectReference projectReference,
      ResultsStoreQuery query,
      QueryModel queryString,
      bool includeIdsOnly,
      bool includeRunDetails,
      int skip,
      int top)
    {
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns;
      this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunsByQuery(this.RequestContext, projectReference.Id, queryString, includeIdsOnly, includeRunDetails, skip, top, out testRuns);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> localRuns = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        if (includeIdsOnly)
          localRuns = this.GetShallowTestRunsByQueryInternal(projectReference, query, skip, top);
        else
          localRuns = this.GetTestRunsByQueryInternal(projectReference, query, includeRunDetails, skip, top);
      }), this.RequestContext);
      return this.TestManagementRequestContext.MergeDataHelper.MergeTestRuns(localRuns, testRuns != null ? testRuns.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null);
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsInBatches(
      TeamProjectReference projectReference,
      string buildUri,
      string ownerId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunsInBatches = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      GuidAndString projectId = new GuidAndString(projectReference.Name, projectReference.Id);
      int num1 = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/BatchSizeQueryTestRuns", 1000);
      List<TestRun> testRunList1 = new List<TestRun>();
      int skip1 = skip;
      int num2 = top;
      while (num2 > 0)
      {
        int top1 = num2 > num1 ? num1 : num2;
        List<TestRun> testRunList2 = this.QueryTestRuns(projectId, buildUri, ownerId, planId, skip1, top1);
        if (testRunList2 != null && testRunList2.Any<TestRun>())
        {
          testRunList1.AddRange((IEnumerable<TestRun>) testRunList2);
          if (testRunList2.Count<TestRun>() >= top1)
          {
            num2 -= top1;
            skip1 += top1;
          }
          else
            break;
        }
        else
          break;
      }
      if (automated.HasValue)
        testRunList1 = testRunList1.FindAll((Predicate<TestRun>) (testRun =>
        {
          int num3 = testRun.IsAutomated ? 1 : 0;
          bool? nullable = automated;
          int num4 = nullable.GetValueOrDefault() ? 1 : 0;
          return num3 == num4 & nullable.HasValue;
        }));
      if (testRunList1 != null)
      {
        foreach (TestRun testRun in testRunList1)
          testRunsInBatches.Add(this.DataContractConverter.ConvertTestRunToDataContract(testRun, projectReference, includeRunDetails, false, false));
      }
      return testRunsInBatches;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsInternal(
      TeamProjectReference projectReference,
      string buildUri,
      string ownerId,
      string tmiRunId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) null;
      if (!this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRuns(this.RequestContext, projectReference.Id, buildUri, ownerId, tmiRunId, planId, includeRunDetails, automated, skip, top, out runs))
        this.RequestContext.Trace(1015051, TraceLevel.Info, "TestManagement", "RestLayer", "No test runs found in Tcm service");
      runs?.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (r => this.DataContractConverter.SecureTestRunWebApiObject(r, projectReference.Id)));
      if (!string.IsNullOrEmpty(tmiRunId))
        return this.QueryTestRunByTmiRunId(tmiRunId, projectReference);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunDataContracts = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        testRunDataContracts = this.GetTestRunsInBatches(projectReference, buildUri, ownerId, planId, includeRunDetails, automated, skip, top);
        testRunDataContracts = TCMServiceDataMigrationRestHelper.FilterTfsRunsBelowThresholdFromTCM(this.TestManagementRequestContext, testRunDataContracts);
      }), this.RequestContext);
      return this.TestManagementRequestContext.MergeDataHelper.MergeTestRuns(testRunDataContracts, runs);
    }

    internal StatisticsHelper StatisticsHelper
    {
      get
      {
        if (this.m_statisticsHelper == null)
          this.m_statisticsHelper = new StatisticsHelper(this.TestManagementRequestContext);
        return this.m_statisticsHelper;
      }
      set => this.m_statisticsHelper = value;
    }

    protected internal virtual DataContractConverter DataContractConverter
    {
      get
      {
        if (this.m_dataContractConverter == null)
          this.m_dataContractConverter = new DataContractConverter(this.TestManagementRequestContext);
        return this.m_dataContractConverter;
      }
      set => this.m_dataContractConverter = value;
    }
  }
}
