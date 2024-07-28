// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class ResultsHelper : RestApiHelper
  {
    private IMergeDataHelper m_mergeDataHelper;

    public ResultsHelper(TestManagementRequestContext context)
      : base(context)
    {
    }

    public void CreateTestResults(
      TestManagementRequestContext context,
      string projectName,
      LegacyTestCaseResult[] results)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        LegacyTestCaseResult[] legacyTestCaseResultArray1;
        if (results == null)
        {
          legacyTestCaseResultArray1 = (LegacyTestCaseResult[]) null;
        }
        else
        {
          IEnumerable<LegacyTestCaseResult> source = ((IEnumerable<LegacyTestCaseResult>) results).Where<LegacyTestCaseResult>((Func<LegacyTestCaseResult, bool>) (result => context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, result.TestRunId, false)));
          legacyTestCaseResultArray1 = source != null ? source.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        }
        LegacyTestCaseResult[] source1 = legacyTestCaseResultArray1;
        if (source1 != null && ((IEnumerable<LegacyTestCaseResult>) source1).Any<LegacyTestCaseResult>())
          context.TcmServiceHelper.TryCreateTestResultsLegacy(context.RequestContext, projectName, new CreateTestResultsRequest()
          {
            Results = source1,
            ProjectName = projectName
          });
        LegacyTestCaseResult[] legacyTestCaseResultArray2;
        if (results == null)
        {
          legacyTestCaseResultArray2 = (LegacyTestCaseResult[]) null;
        }
        else
        {
          IEnumerable<LegacyTestCaseResult> source2 = ((IEnumerable<LegacyTestCaseResult>) results).Where<LegacyTestCaseResult>((Func<LegacyTestCaseResult, bool>) (result => !context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, result.TestRunId, false)));
          legacyTestCaseResultArray2 = source2 != null ? source2.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        }
        LegacyTestCaseResult[] legacyTestCaseResultArray3 = legacyTestCaseResultArray2;
        if (legacyTestCaseResultArray3 == null || !((IEnumerable<LegacyTestCaseResult>) legacyTestCaseResultArray3).Any<LegacyTestCaseResult>())
          return;
        context.RequestContext.GetService<ITestResultsService>().CreateTestResults(context, projectName, legacyTestCaseResultArray3);
      }
      else
      {
        if (results == null)
          return;
        if (!((IEnumerable<LegacyTestCaseResult>) results).Any<LegacyTestCaseResult>())
          return;
        try
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          CreateTestResultsRequest request = new CreateTestResultsRequest();
          request.Results = results;
          request.ProjectName = projectName;
          string userState = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          tcmHttpClient.CreateTestResultsLegacyAsync(request, (object) userState, cancellationToken)?.Wait();
        }
        catch (AggregateException ex)
        {
          if (ex.InnerException != null)
            throw ex.InnerException;
          throw;
        }
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun CreateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun,
      LegacyTestCaseResult[] results,
      LegacyTestSettings testSettings)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          CreateTestRunRequest request = new CreateTestRunRequest();
          request.TestRun = testRun;
          request.TestSettings = testSettings;
          request.Results = results;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.CreateTestRunLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      ITcmServiceHelper tcmServiceHelper = context.TcmServiceHelper;
      IVssRequestContext requestContext = context.RequestContext;
      CreateTestRunRequest createTestRunRequest = new CreateTestRunRequest();
      createTestRunRequest.TestRun = testRun;
      createTestRunRequest.TestSettings = testSettings;
      createTestRunRequest.Results = results;
      createTestRunRequest.ProjectName = projectName;
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun legacyTestRun;
      ref Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun local = ref legacyTestRun;
      return tcmServiceHelper.TryCreateTestRunLegacy(requestContext, createTestRunRequest, out local) ? legacyTestRun : context.RequestContext.GetService<ITestResultsService>().CreateTestRun(context, projectName, testRun, results, testSettings);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties AbortTestRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      int revision,
      int options)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          AbortTestRunRequest request = new AbortTestRunRequest();
          request.ProjectName = projectName;
          request.TestRunId = testRunId;
          request.Revision = revision;
          request.Options = options;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.AbortTestRunAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      if (!context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, testRunId, false))
        return context.RequestContext.GetService<ITestResultsService>().AbortTestRun(context, projectName, testRunId, revision, options);
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties;
      context.TcmServiceHelper.TryAbortTestRun(context.RequestContext, projectName, testRunId, revision, options, out updatedProperties);
      return updatedProperties;
    }

    public QueryTestActionResultResponse QueryTestActionResults(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier identifier)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<QueryTestActionResultResponse>((Func<QueryTestActionResultResponse>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestActionResultRequest request = new QueryTestActionResultRequest();
          request.ProjectName = projectName;
          request.Identifier = TestCaseResultIdentifierConverter.Convert(identifier);
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.QueryTestActionResultsAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      QueryTestActionResultResponse response;
      return context.TcmServiceHelper.TryQueryTestActionResults(context.RequestContext, projectName, identifier, out response) ? response : context.RequestContext.GetService<ITestActionResultService>().QueryTestActionResults(context, projectName, TestCaseResultIdentifierConverter.Convert(identifier));
    }

    public LegacyTestCaseResult GetTestResultInMultipleProjects(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      out string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        TestResultAcrossProjectResponse response;
        if (!context.TcmServiceHelper.TryGetTestResultInMultipleProjects(context.RequestContext, testRunId, testResultId, out response))
          return context.RequestContext.GetService<ITestResultsService>().GetTestResultInMultipleProjects(context, testRunId, testResultId, out projectName);
        projectName = response.ProjectName;
        return response.TestResult;
      }
      TestResultAcrossProjectResponse acrossProjectResponse = this.InvokeAction<TestResultAcrossProjectResponse>((Func<TestResultAcrossProjectResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        LegacyTestCaseResultIdentifier identifier = new LegacyTestCaseResultIdentifier();
        identifier.TestRunId = testRunId;
        identifier.TestResultId = testResultId;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.GetTestResultsAcrossProjectsAsync(identifier, cancellationToken: cancellationToken)?.Result;
      }));
      projectName = acrossProjectResponse.ProjectName;
      return acrossProjectResponse.TestResult;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByRunId(
      TestManagementRequestContext context,
      Guid projectId,
      int runId)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(projectId, runId, new ResultDetails?(), new int?(), new int?(), (IEnumerable<TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results;
      context.TcmServiceHelper.TryGetTestResults(context.RequestContext, projectId, runId, out results);
      return results;
    }

    public List<LegacyTestCaseResult> GetTestResultsByQuery(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        ResultsByQueryResponse reponse;
        context.TcmServiceHelper.TryGetTestResultsByQuery(context.RequestContext, query, pageSize, out reponse);
        List<LegacyTestCaseResult> responseFromLocal = (List<LegacyTestCaseResult>) null;
        List<LegacyTestCaseResultIdentifier> localExcessIds = (List<LegacyTestCaseResultIdentifier>) null;
        TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => responseFromLocal = context.RequestContext.GetService<ITestResultsService>().GetTestResultsByQuery(context, query, pageSize, out localExcessIds)), context.RequestContext);
        IMergeDataHelper mergeDataHelper = this.MergeDataHelper;
        ResultsByQueryResponse response = reponse;
        ResultsByQueryResponse resultsByQueryResponse1 = new ResultsByQueryResponse();
        resultsByQueryResponse1.TestResults = responseFromLocal;
        resultsByQueryResponse1.ExcessIds = localExcessIds;
        int pageSize1 = pageSize;
        ResultsByQueryResponse resultsByQueryResponse2 = mergeDataHelper.MergeResultsByQueryResponse(response, resultsByQueryResponse1, pageSize1);
        excessIds = resultsByQueryResponse2.ExcessIds;
        return resultsByQueryResponse2.TestResults;
      }
      ResultsByQueryResponse resultsByQueryResponse = this.InvokeAction<ResultsByQueryResponse>((Func<ResultsByQueryResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        ResultsByQueryRequest request = new ResultsByQueryRequest();
        request.PageSize = pageSize;
        request.Query = query;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.GetTestResultsByQueryLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
      excessIds = resultsByQueryResponse.ExcessIds;
      return resultsByQueryResponse.TestResults;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun QueryTestRunByTmiRunId(
      TestManagementRequestContext context,
      Guid tmiRunId)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) (() => this.TcmHttpClient.GetTestRunByTmiRunIdAsync(tmiRunId)?.Result));
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun;
      return context.TcmServiceHelper.TryQueryTestRunByTmiRunId(context.RequestContext, tmiRunId, out testRun) && testRun != null ? testRun : context.RequestContext.GetService<ITestResultsService>().QueryTestRunByTmiRunId(context, tmiRunId);
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>) (() =>
        {
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
          return tcmHttpClient.QueryTestRunsLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote;
      context.TcmServiceHelper.TryQueryTestRunsLegacy(context.RequestContext, testRunId, owner, buildUri, teamProjectName, planId, skip, top, out runsFromRemote);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> localRuns = new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => localRuns = context.RequestContext.GetService<ITestResultsService>().Query(context, testRunId, owner, buildUri, teamProjectName, planId, skip, top)), context.RequestContext);
      return this.MergeDataHelper.MergeTestRunsLegacy(runsFromRemote, localRuns);
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRunsInMultipleProjects(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>) (() => this.TcmHttpClient.QueryTestRunsAcrossMultipleProjectsAsync(query)?.Result));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote;
      context.TcmServiceHelper.TryQueryTestRunsInMultipleProjects(context.RequestContext, query, out runsFromRemote);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> localRuns = new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => localRuns = context.RequestContext.GetService<ITestResultsService>().QueryTestRunsInMultipleProjects(context, query)), context.RequestContext);
      return this.MergeDataHelper.MergeTestRunsLegacy(runsFromRemote, localRuns);
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics = false)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestRuns2Request request = new QueryTestRuns2Request();
          request.Query = resultsStoreQuery;
          request.IncludeStatistics = includeStatistics;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.QueryTestRuns2Async(request, cancellationToken: cancellationToken)?.Result;
        }));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote;
      context.TcmServiceHelper.TryQueryTestRuns2Legacy(context.RequestContext, resultsStoreQuery, includeStatistics, out runsFromRemote);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> localRuns = new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => localRuns = context.RequestContext.GetService<ITestResultsService>().Query(context, resultsStoreQuery, includeStatistics)), context.RequestContext);
      return this.MergeDataHelper.MergeTestRunsLegacy(runsFromRemote, localRuns);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] Update(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] source1 = requests;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] resultUpdateRequestArray1;
        if (source1 == null)
        {
          resultUpdateRequestArray1 = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null;
        }
        else
        {
          IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> source2 = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) source1).Where<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest, bool>) (req => context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, req.TestRunId, false)));
          resultUpdateRequestArray1 = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null;
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] resultUpdateRequestArray2 = resultUpdateRequestArray1;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] resultUpdateResponseArray = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null;
        if (resultUpdateRequestArray2 != null && ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) resultUpdateRequestArray2).Any<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>())
        {
          ITcmServiceHelper tcmServiceHelper = context.TcmServiceHelper;
          IVssRequestContext requestContext = context.RequestContext;
          BulkResultUpdateRequest request = new BulkResultUpdateRequest();
          request.Requests = requests;
          request.ProjectName = projectName;
          ref Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] local = ref resultUpdateResponseArray;
          tcmServiceHelper.TryUpdateTestResultsLegacy(requestContext, request, out local);
          context.TestPointOutcomeHelper.UpdateTestPointOutcomeFromSOAPRequest(context.RequestContext, projectName, requests, resultUpdateResponseArray);
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] source3 = requests;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] resultUpdateRequestArray3;
        if (source3 == null)
        {
          resultUpdateRequestArray3 = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null;
        }
        else
        {
          IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> source4 = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) source3).Where<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest, bool>) (req => !context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, req.TestRunId, false)));
          resultUpdateRequestArray3 = source4 != null ? source4.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null;
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] resultUpdateRequestArray4 = resultUpdateRequestArray3;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromLocal = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null;
        if (resultUpdateRequestArray4 != null && ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) resultUpdateRequestArray4).Any<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>())
          responseFromLocal = context.RequestContext.GetService<ITestResultsService>().Update(context, resultUpdateRequestArray4, projectName);
        return this.MergeDataHelper.MergeUpdateResponseLegacy(requests, resultUpdateRequestArray2, resultUpdateResponseArray, resultUpdateRequestArray4, responseFromLocal);
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responses = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] source = requests;
      if ((source != null ? (((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) source).Any<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() ? 1 : 0) : 0) != 0)
      {
        responses = this.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          BulkResultUpdateRequest request = new BulkResultUpdateRequest();
          request.Requests = requests;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.UpdateTestResultsLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
        context.TestPointOutcomeHelper.UpdateTestPointOutcomeFromSOAPRequest(context.RequestContext, projectName, requests, responses);
      }
      return responses;
    }

    public List<LegacyTestRunStatistic> QueryTestRunStats(
      TestManagementRequestContext context,
      string projectName,
      int testRunId)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<LegacyTestRunStatistic>>((Func<List<LegacyTestRunStatistic>>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryTestRunStatsRequest request = new QueryTestRunStatsRequest();
          request.TeamProjectName = projectName;
          request.TestRunId = testRunId;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.QueryTestRunStatsAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      if (!context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, testRunId))
        return context.RequestContext.GetService<ITestResultsService>().QueryTestRunStats(context, projectName, testRunId);
      List<LegacyTestRunStatistic> testRunStats;
      context.TcmServiceHelper.TryQueryTestRunStats(context.RequestContext, projectName, testRunId, out testRunStats);
      return testRunStats;
    }

    public LegacyTestCaseResult[] ResetTestResults(
      TestManagementRequestContext context,
      LegacyTestCaseResultIdentifier[] identifiers,
      string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        LegacyTestCaseResultIdentifier[] source1 = identifiers;
        LegacyTestCaseResultIdentifier[] resultIdentifierArray1;
        if (source1 == null)
        {
          resultIdentifierArray1 = (LegacyTestCaseResultIdentifier[]) null;
        }
        else
        {
          IEnumerable<LegacyTestCaseResultIdentifier> source2 = ((IEnumerable<LegacyTestCaseResultIdentifier>) source1).Where<LegacyTestCaseResultIdentifier>((Func<LegacyTestCaseResultIdentifier, bool>) (id => context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, id.TestRunId, false)));
          resultIdentifierArray1 = source2 != null ? source2.ToArray<LegacyTestCaseResultIdentifier>() : (LegacyTestCaseResultIdentifier[]) null;
        }
        LegacyTestCaseResultIdentifier[] resultIdentifierArray2 = resultIdentifierArray1;
        LegacyTestCaseResult[] responseFromRemote = (LegacyTestCaseResult[]) null;
        if (resultIdentifierArray2 != null && ((IEnumerable<LegacyTestCaseResultIdentifier>) resultIdentifierArray2).Any<LegacyTestCaseResultIdentifier>())
          context.TcmServiceHelper.TryResetTestResults(context.RequestContext, projectName, resultIdentifierArray2, out responseFromRemote);
        LegacyTestCaseResultIdentifier[] source3 = identifiers;
        LegacyTestCaseResultIdentifier[] resultIdentifierArray3;
        if (source3 == null)
        {
          resultIdentifierArray3 = (LegacyTestCaseResultIdentifier[]) null;
        }
        else
        {
          IEnumerable<LegacyTestCaseResultIdentifier> source4 = ((IEnumerable<LegacyTestCaseResultIdentifier>) source3).Where<LegacyTestCaseResultIdentifier>((Func<LegacyTestCaseResultIdentifier, bool>) (id => !context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, id.TestRunId, false)));
          resultIdentifierArray3 = source4 != null ? source4.ToArray<LegacyTestCaseResultIdentifier>() : (LegacyTestCaseResultIdentifier[]) null;
        }
        LegacyTestCaseResultIdentifier[] resultIdentifierArray4 = resultIdentifierArray3;
        LegacyTestCaseResult[] responseFromLocal = (LegacyTestCaseResult[]) null;
        if (resultIdentifierArray4 != null && ((IEnumerable<LegacyTestCaseResultIdentifier>) resultIdentifierArray4).Any<LegacyTestCaseResultIdentifier>())
          responseFromLocal = context.RequestContext.GetService<ITestResultsService>().ResetTestResults(context, resultIdentifierArray4, projectName);
        return this.MergeDataHelper.MergeLegacyTestResults(identifiers, resultIdentifierArray2, responseFromRemote, resultIdentifierArray4, responseFromLocal);
      }
      LegacyTestCaseResult[] legacyTestCaseResultArray = (LegacyTestCaseResult[]) null;
      LegacyTestCaseResultIdentifier[] source = identifiers;
      if ((source != null ? (((IEnumerable<LegacyTestCaseResultIdentifier>) source).Any<LegacyTestCaseResultIdentifier>() ? 1 : 0) : 0) != 0)
        legacyTestCaseResultArray = this.InvokeAction<LegacyTestCaseResult[]>((Func<LegacyTestCaseResult[]>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          ResetTestResultsRequest request = new ResetTestResultsRequest();
          request.Ids = identifiers;
          request.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.ResetTestResultsAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      return legacyTestCaseResultArray;
    }

    public List<LegacyTestCaseResult> QueryByRunAndOutcome(
      TestManagementRequestContext context,
      int testRunId,
      byte outcome,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        FetchTestResultsResponse responseFromRemote;
        if (!context.TcmServiceHelper.TryQueryByRunAndOutcome(context.RequestContext, testRunId, outcome, pageSize, projectName, out responseFromRemote))
          return context.RequestContext.GetService<ITestResultsService>().QueryByRunAndOutcome(context, testRunId, outcome, pageSize, out excessIds, projectName);
        excessIds = responseFromRemote.DeletedIds;
        return responseFromRemote.Results;
      }
      FetchTestResultsResponse testResultsResponse = this.InvokeAction<FetchTestResultsResponse>((Func<FetchTestResultsResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        QueryByRunRequest request = new QueryByRunRequest();
        request.TestRunId = testRunId;
        request.Outcome = outcome;
        request.PageSize = pageSize;
        request.ProjectName = projectName;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.QueryByRunAndOutcomeAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
      excessIds = testResultsResponse.DeletedIds;
      return testResultsResponse.Results;
    }

    public List<LegacyTestCaseResult> QueryByRunAndState(
      TestManagementRequestContext context,
      int testRunId,
      byte state,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        FetchTestResultsResponse responseFromRemote;
        if (!context.TcmServiceHelper.TryQueryByRunAndState(context.RequestContext, testRunId, state, pageSize, projectName, out responseFromRemote))
          return context.RequestContext.GetService<ITestResultsService>().QueryByRunAndState(context, testRunId, state, pageSize, out excessIds, projectName);
        excessIds = responseFromRemote.DeletedIds;
        return responseFromRemote.Results;
      }
      FetchTestResultsResponse testResultsResponse = this.InvokeAction<FetchTestResultsResponse>((Func<FetchTestResultsResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        QueryByRunRequest request = new QueryByRunRequest();
        request.TestRunId = testRunId;
        request.State = state;
        request.PageSize = pageSize;
        request.ProjectName = projectName;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.QueryByRunAndStateAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
      excessIds = testResultsResponse.DeletedIds;
      return testResultsResponse.Results;
    }

    public List<LegacyTestCaseResult> QueryByRunAndOwner(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        FetchTestResultsResponse responseFromRemote;
        if (!context.TcmServiceHelper.TryQueryByRunAndOwner(context.RequestContext, testRunId, owner, pageSize, projectName, out responseFromRemote))
          return context.RequestContext.GetService<ITestResultsService>().QueryByRunAndOwner(context, testRunId, owner, pageSize, out excessIds, projectName);
        excessIds = responseFromRemote.DeletedIds;
        return responseFromRemote.Results;
      }
      FetchTestResultsResponse testResultsResponse = this.InvokeAction<FetchTestResultsResponse>((Func<FetchTestResultsResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        QueryByRunRequest request = new QueryByRunRequest();
        request.TestRunId = testRunId;
        request.Owner = owner;
        request.PageSize = pageSize;
        request.ProjectName = projectName;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.QueryByRunAndOwnerAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
      excessIds = testResultsResponse.DeletedIds;
      return testResultsResponse.Results;
    }

    public List<LegacyTestCaseResult> QueryByPoint(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      int pointId)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<List<LegacyTestCaseResult>>((Func<List<LegacyTestCaseResult>>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          QueryByPointRequest request = new QueryByPointRequest();
          request.ProjectName = projectName;
          request.TestPlanId = planId;
          request.TestPointId = pointId;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.QueryByPointAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      List<LegacyTestCaseResult> resultsFromRemote;
      context.TcmServiceHelper.TryQueryByPoint(context.RequestContext, projectName, planId, pointId, out resultsFromRemote);
      List<LegacyTestCaseResult> resultsFromLocal = (List<LegacyTestCaseResult>) null;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultsFromLocal = context.RequestContext.GetService<ITestResultsService>().QueryByPoint(context, projectName, planId, pointId)), context.RequestContext);
      return this.MergeDataHelper.MergeLegacyTestResults(resultsFromRemote, resultsFromLocal);
    }

    public List<LegacyTestCaseResult> QueryByRun(
      TestManagementRequestContext context,
      int testRunId,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> webApiExcessIds,
      string projectName,
      bool includeActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        ITcmServiceHelper tcmServiceHelper = context.TcmServiceHelper;
        IVssRequestContext requestContext = context.RequestContext;
        QueryByRunRequest queryByRunRequest = new QueryByRunRequest();
        queryByRunRequest.TestRunId = testRunId;
        queryByRunRequest.PageSize = pageSize;
        queryByRunRequest.ProjectName = projectName;
        queryByRunRequest.IncludeActionResults = includeActionResults;
        FetchTestResultsResponse testResultsResponse;
        ref FetchTestResultsResponse local = ref testResultsResponse;
        if (!tcmServiceHelper.TryQueryByRun(requestContext, queryByRunRequest, out local))
          return context.RequestContext.GetService<ITestResultsService>().QueryByRun(context, testRunId, pageSize, out webApiExcessIds, projectName, includeActionResults, out webApiActionResults, out webApiParams, out webApiAttachments);
        webApiExcessIds = testResultsResponse.DeletedIds;
        webApiActionResults = testResultsResponse.ActionResults;
        webApiParams = testResultsResponse.TestParameters;
        webApiAttachments = testResultsResponse.Attachments;
        return testResultsResponse.Results;
      }
      FetchTestResultsResponse testResultsResponse1 = this.InvokeAction<FetchTestResultsResponse>((Func<FetchTestResultsResponse>) (() =>
      {
        TcmHttpClient tcmHttpClient = this.TcmHttpClient;
        QueryByRunRequest request = new QueryByRunRequest();
        request.TestRunId = testRunId;
        request.PageSize = pageSize;
        request.ProjectName = projectName;
        request.IncludeActionResults = includeActionResults;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient.QueryByRunAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
      webApiExcessIds = testResultsResponse1.DeletedIds;
      webApiActionResults = testResultsResponse1.ActionResults;
      webApiParams = testResultsResponse1.TestParameters;
      webApiAttachments = testResultsResponse1.Attachments;
      return testResultsResponse1.Results;
    }

    public List<LegacyTestCaseResult> Fetch(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> webApiIdAndRevs,
      string projectName,
      bool includeActionResults,
      out List<LegacyTestCaseResultIdentifier> webApiDeletedIds,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments)
    {
      FetchTestResultsResponse testResultsResponse;
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> list1 = webApiIdAndRevs.Where<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev, bool>) (idAndRev => context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, idAndRev.Id.TestRunId))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>();
        ITcmServiceHelper tcmServiceHelper = context.TcmServiceHelper;
        IVssRequestContext requestContext = context.RequestContext;
        FetchTestResultsRequest fetchTestResultsRequest = new FetchTestResultsRequest();
        fetchTestResultsRequest.IdAndRevs = list1;
        fetchTestResultsRequest.ProjectName = projectName;
        fetchTestResultsRequest.IncludeActionResults = includeActionResults;
        FetchTestResultsResponse responseFromRemote;
        ref FetchTestResultsResponse local = ref responseFromRemote;
        tcmServiceHelper.TryFetchTestResults(requestContext, fetchTestResultsRequest, out local);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> list2 = webApiIdAndRevs.Where<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev, bool>) (idAndRev => !context.TcmServiceHelper.IsTestRunInTCM(context.RequestContext, idAndRev.Id.TestRunId))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>();
        List<LegacyTestCaseResultIdentifier> webApiDeletedIds1;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults1;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams1;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments1;
        List<LegacyTestCaseResult> legacyTestCaseResultList = context.RequestContext.GetService<ITestResultsService>().Fetch(context, list2, projectName, includeActionResults, out webApiDeletedIds1, out webApiActionResults1, out webApiParams1, out webApiAttachments1);
        testResultsResponse = this.MergeDataHelper.FetchTestResultsResponse(responseFromRemote, new FetchTestResultsResponse()
        {
          Results = legacyTestCaseResultList,
          ActionResults = webApiActionResults1,
          DeletedIds = webApiDeletedIds1,
          TestParameters = webApiParams1,
          Attachments = webApiAttachments1
        });
      }
      else
        testResultsResponse = this.InvokeAction<FetchTestResultsResponse>((Func<FetchTestResultsResponse>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          FetchTestResultsRequest request = new FetchTestResultsRequest();
          request.IdAndRevs = webApiIdAndRevs;
          request.ProjectName = projectName;
          request.IncludeActionResults = includeActionResults;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.FetchTestResultsAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
      webApiActionResults = testResultsResponse.ActionResults;
      webApiDeletedIds = testResultsResponse.DeletedIds;
      webApiParams = testResultsResponse.TestParameters;
      webApiAttachments = testResultsResponse.Attachments;
      return testResultsResponse.Results;
    }

    public void DeleteAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris)
    {
      context.RequestContext.GetService<ITestResultsService>().DeleteAssociatedWorkItems(context, identifiers, workItemUris);
    }

    public void CreateAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris)
    {
      context.RequestContext.GetService<ITestResultsService>().CreateAssociatedWorkItems(context, identifiers, workItemUris);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      out int[] attachmentIds,
      bool shouldHyderate)
    {
      UpdateTestRunResponse response = (UpdateTestRunResponse) null;
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        if (!context.TcmServiceHelper.TryUpdateTestRunLegacy(context.RequestContext, projectName, webApiTestRun, attachmentsToAdd, attachmentsToDelete, shouldHyderate, out response))
          return context.RequestContext.GetService<ITestResultsService>().UpdateTestRun(context, projectName, webApiTestRun, attachmentsToAdd, attachmentsToDelete, out attachmentIds, shouldHyderate);
        attachmentIds = response?.AttachmentIds;
        return response?.UpdatedProperties;
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun legacyTestRun = webApiTestRun;
      int num;
      if (legacyTestRun == null)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] source1 = attachmentsToAdd;
        int? nullable = source1 != null ? ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) source1).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>()?.TestRunId : new int?();
        if (!nullable.HasValue)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] source2 = attachmentsToDelete;
          num = (source2 != null ? ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) source2).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>()?.TestRunId : new int?()).GetValueOrDefault();
        }
        else
          num = nullable.GetValueOrDefault();
      }
      else
        num = legacyTestRun.TestRunId;
      if (num != 0)
      {
        UpdateTestRunResponse updateTestRunResponse = this.InvokeAction<UpdateTestRunResponse>((Func<UpdateTestRunResponse>) (() =>
        {
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          UpdateTestRunRequest request = new UpdateTestRunRequest();
          request.ProjectName = projectName;
          request.TestRun = webApiTestRun;
          request.AttachmentsToAdd = attachmentsToAdd;
          request.AttachmentsToDelete = attachmentsToDelete;
          request.ShouldHyderate = shouldHyderate;
          CancellationToken cancellationToken = new CancellationToken();
          return tcmHttpClient.UpdateTestRunLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
        }));
        attachmentIds = updateTestRunResponse?.AttachmentIds;
        return updateTestRunResponse.UpdatedProperties;
      }
      attachmentIds = (int[]) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties() { Revision = -2 };
    }

    public int QueryCount(TestManagementRequestContext context, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.InvokeAction<int>((Func<int>) (() =>
        {
          Task<int> task = this.TcmHttpClient.QueryTestRunsCountAsync(query);
          return task == null ? 0 : task.Result;
        }));
      int? countFromRemote;
      context.TcmServiceHelper.TryQueryCount(context.RequestContext, query, out countFromRemote);
      int localCount = 0;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => localCount = context.RequestContext.GetService<ITestResultsService>().QueryTestRunsCount(context, query)), context.RequestContext);
      return localCount + countFromRemote.GetValueOrDefault();
    }

    internal IMergeDataHelper MergeDataHelper
    {
      get
      {
        if (this.m_mergeDataHelper == null)
          this.m_mergeDataHelper = (IMergeDataHelper) new MergeTcmDataHelper();
        return this.m_mergeDataHelper;
      }
    }

    internal TcmHttpClient TcmHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TcmHttpClient>();

    internal TestResultsHttpClient TestResultsHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TestResultsHttpClient>();

    private T InvokeAction<T>(Func<T> func)
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
