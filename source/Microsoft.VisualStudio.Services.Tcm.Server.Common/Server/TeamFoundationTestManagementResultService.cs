// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementResultService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.TeamFoundation.TestManagement.Server.Results;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementResultService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementResultService,
    IVssFrameworkService
  {
    private TestMethodNameSanitizer m_testMethodNameSanitizer = new TestMethodNameSanitizer();
    private IReleaseServiceHelper m_releaseServiceHelper;
    private TestConfigurationHelper m_testConfigurationHelper;
    private IResultsSortingHelper m_resutsSortingHelper;
    private ITelemetryLogger m_telemetryLogger;
    private string m_tcmServiceName = "TCM";
    private string m_tfsServiceName = "TFS";
    private char m_tokenSeparator = '_';

    public TeamFoundationTestManagementResultService()
    {
    }

    public TeamFoundationTestManagementResultService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] AddTestResultsToTestRun(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool testSessionProperties = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.AddTestResultsToTestRun"))
        return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(context.RequestContext, "TeamFoundationTestManagementResultService.AddTestResultsToTestRun", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>) (() =>
        {
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) results, nameof (results), "Test Results");
          if ((byte) 4 == testRun.State || (byte) 3 == testRun.State)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultsCreateErrorDueToRunState, (object) ((Microsoft.TeamFoundation.TestManagement.Client.TestRunState) testRun.State).ToString()));
          bool isPlannedRun = testRun.TestPlanId > 0;
          this.ValidatePublishResultsPageSizeLimit(results.Length, isPlannedRun, context.RequestContext.IsImpersonating);
          List<Tuple<TestCaseResult, List<WorkItem>>> resultCreateModel = this.GetTestCaseResultsFromResultCreateModel(context, projectReference, testRun, results, this.GetTfsIdentityCache(context, results), testSessionProperties);
          if (resultCreateModel == null || !resultCreateModel.Any<Tuple<TestCaseResult, List<WorkItem>>>())
            return Array.Empty<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          TestCaseResult[] results1 = resultCreateModel.Select<Tuple<TestCaseResult, List<WorkItem>>, TestCaseResult>((System.Func<Tuple<TestCaseResult, List<WorkItem>>, TestCaseResult>) (it => it.Item1)).ToArray<TestCaseResult>();
          if (isPlannedRun && context.IsTcmService && context.RequestContext.IsFeatureEnabled("TestManagement.Server.MakeDistinctResultsForCreateTestResultExtension"))
            results1 = this.RemoveResultsWithDuplicatePointId(context, results1, testRun.TestRunId, testRun.TestPlanId);
          Dictionary<int, int[]> dictionary1 = new Dictionary<int, int[]>();
          bool shouldPublishOnlyFailedResults = context.IsFeatureEnabled("TestManagement.Server.PublishOnlyFailedResults") && GitHelper.IsPullRequest(testRun.BuildReference?.BranchName ?? string.Empty);
          Dictionary<int, int[]> dictionary2 = TestCaseResult.Create2(context, projectReference.Name, testRun.TestRunId, results1, false, !context.IsTcmService, shouldPublishOnlyFailedResults, testSessionProperties);
          bool hierarchicalResultFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResult");
          bool flag1 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResultBatchUploadAttachment");
          bool flag2 = false;
          for (int index = 0; index < resultCreateModel.Count<Tuple<TestCaseResult, List<WorkItem>>>(); ++index)
          {
            TestCaseResult testResult = resultCreateModel[index].Item1;
            List<WorkItem> workItemList = resultCreateModel[index].Item2;
            testResult.TestResultId = dictionary2[testRun.TestRunId][index];
            results[index].Id = testResult.TestResultId;
            results[index].TestRun = new ShallowReference()
            {
              Id = testRun.TestRunId.ToString()
            };
            if (testResult != null && workItemList != null && workItemList.Any<WorkItem>())
              this.CreateAssociatedWorkItemsForTestResult(context, projectReference.Name, testResult, workItemList);
            if (results[index].SubResults != null && results[index].SubResults.Any<TestSubResult>())
            {
              flag2 = true;
              if (!flag1)
                this.PublishSubResults(context, projectReference, testRun.TestRunId, testResult.TestResultId, results[index].SubResults, hierarchicalResultFFEnabled);
            }
          }
          if (flag2 & flag1)
          {
            try
            {
              context.RequestContext.RequestTimer.PauseTimeToFirstPageTimer();
              this.PublishSubResultsV2(context, projectReference, testRun.TestRunId, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), hierarchicalResultFFEnabled);
            }
            finally
            {
              context.RequestContext.RequestTimer.ResumeTimeToFirstPageTimer();
            }
          }
          return results;
        }), 1015095, "TestResultsInsights");
    }

    public void UpdateTestCaseReferences(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      IList<TestCaseResult> results)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateTestCaseReferences", (Action) (() =>
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, nameof (projectReference), "Test Results");
        ArgumentUtility.CheckForNull<IList<TestCaseResult>>(results, nameof (results), "Test Results");
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
        {
          if (!results.Any<TestCaseResult>((System.Func<TestCaseResult, bool>) (result => result.TestCaseReferenceId == 0)))
            managementDatabase.UpdateTestCaseReference(projectReference.Id, (IEnumerable<TestCaseResult>) results);
          else
            managementDatabase.UpdateTestCaseReference2(projectReference.Id, (IEnumerable<TestCaseResult>) results);
        }
      }), 1015095, "TestResultsInsights");
    }

    public void UpdateTestRunSummaryForResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      IList<TestCaseResult> results)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateTestRunSummaryForResults", (Action) (() =>
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, nameof (projectReference), "Test Results");
        ArgumentUtility.CheckForNull<IList<TestCaseResult>>(results, nameof (results), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) testRunId, nameof (testRunId), "Test Results");
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
          managementDatabase.UpdateTestRunSummaryForResults(projectReference.Id, testRunId, (IEnumerable<TestCaseResult>) results);
      }), 1015095, "TestResultsInsights");
    }

    public void UpdateFlakinessFieldForResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      IList<TestCaseResult> results)
    {
      this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateFlakinessFieldForResults", (Action) (() =>
      {
        ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, nameof (projectReference), "Test Results");
        ArgumentUtility.CheckForNull<IList<TestCaseResult>>(results, nameof (results), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) testRunId, nameof (testRunId), "Test Results");
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
          managementDatabase.UpdateFlakinessFieldForResults(projectReference.Id, testRunId, (IEnumerable<TestCaseResult>) results);
      }), 1015095, "TestResultsInsights");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] UpdateTestResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out TeamProjectTestArtifacts teamProjectTestArtifacts,
      bool autoComputeTestRunState = true)
    {
      TeamProjectTestArtifacts testArtifacts = (TeamProjectTestArtifacts) null;
      this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateTestResults", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>) (() =>
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(results, nameof (results), "Test Results");
        this.ValidatePublishResultsPageSizeLimit(results.Length, testRun.TestPlanId > 0, context.RequestContext.IsImpersonating);
        return this.UpdateTestResultsInternal(context, projectReference, testRun, results, out testArtifacts, autoComputeTestRunState);
      }), 1015095, "TestResultsInsights");
      teamProjectTestArtifacts = testArtifacts;
      return results;
    }

    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetSimilarTestCaseResults(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int runId,
      int resultId,
      int subResultId,
      int top,
      string continuationToken)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetSimilarTestCaseResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) resultId, nameof (resultId), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) top, "$top", "Test Results");
        if (top > context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/SimilarTestResultsPageSize", 10000))
          throw new InvalidPropertyException("$top", ServerResources.QueryParameterOutOfRange);
        int continuationTokenRunId;
        int continuationTokenResultId;
        ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
        context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
          return managementDatabase.QuerySimilarTestResults(projectInfo.Id, runId, resultId, subResultId, continuationTokenRunId, continuationTokenResultId, top);
      }), 1015095, "TestResultsInsights");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] UpdateTestResultsWithIterationDetails(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool autoComputeTestRunState = true)
    {
      TeamProjectTestArtifacts testArtifacts = (TeamProjectTestArtifacts) null;
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateTestResultsWithIterationDetails", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>) (() =>
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(results, nameof (results), "Test Results");
        return this.UpdateTestResultsInternal(context, projectReference, testRun, results, out testArtifacts, autoComputeTestRunState, true);
      }), 1015095, "TestResultsInsights");
    }

    public TestCaseResult FetchTestCaseResult(
      TestManagementRequestContext context,
      int runId,
      int resultId,
      string projectName,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> testResultParmeters,
      out List<TestResultAttachment> testResultAttachments)
    {
      List<TestActionResult> actionResultList = new List<TestActionResult>();
      List<TestResultParameter> testParams = new List<TestResultParameter>();
      List<TestResultAttachment> testAttachments = new List<TestResultAttachment>();
      TestCaseResult testCaseResult1 = this.ExecuteAction<TestCaseResult>(context.RequestContext, "TeamFoundationTestManagementResultService.FetchTestCaseResult", (Func<TestCaseResult>) (() =>
      {
        TestManagementRequestContext context1 = context;
        TestCaseResultIdentifier id = new TestCaseResultIdentifier();
        id.TestResultId = resultId;
        id.TestRunId = runId;
        string projectName1 = projectName;
        ref List<TestActionResult> local1 = ref actionResultList;
        ref List<TestResultParameter> local2 = ref testParams;
        ref List<TestResultAttachment> local3 = ref testAttachments;
        TestCaseResult testCaseResult2 = TestCaseResult.FetchSingularly(context1, id, projectName1, true, out local1, out local2, out local3);
        return testCaseResult2 != null && testCaseResult2.TestRunId > 0 ? testCaseResult2 : throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) runId, (object) resultId), ObjectTypes.TestResult);
      }), 1015095, "TestResultsInsights");
      actionResults = actionResultList;
      testResultParmeters = testParams;
      testResultAttachments = testAttachments;
      return testCaseResult1;
    }

    public IList<TestCaseResult> FetchTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> results,
      string projectName,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> testResultParmeters,
      out List<TestResultAttachment> testResultAttachments)
    {
      List<TestActionResult> actionResultList = new List<TestActionResult>();
      List<TestResultParameter> testParams = new List<TestResultParameter>();
      List<TestResultAttachment> testAttachments = new List<TestResultAttachment>();
      List<TestCaseResultIdentifier> deletedIds = new List<TestCaseResultIdentifier>();
      List<TestCaseResult> testCaseResultList1 = this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.FetchTestCaseResults", (Func<List<TestCaseResult>>) (() =>
      {
        List<TestCaseResult> testCaseResultList2 = TestCaseResult.Fetch(context, results.Select<TestCaseResult, TestCaseResultIdAndRev>((System.Func<TestCaseResult, TestCaseResultIdAndRev>) (r => new TestCaseResultIdAndRev()
        {
          Id = new TestCaseResultIdentifier()
          {
            TestResultId = r.TestResultId,
            TestRunId = r.TestRunId
          },
          Revision = 0
        })).ToArray<TestCaseResultIdAndRev>(), projectName, true, deletedIds, out actionResultList, out testParams, out testAttachments);
        if (!deletedIds.Any<TestCaseResultIdentifier>())
          return testCaseResultList2;
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) deletedIds[0].TestRunId, (object) deletedIds[0].TestResultId), ObjectTypes.TestResult);
      }), 1015095, "TestResultsInsights");
      actionResults = actionResultList;
      testResultParmeters = testParams;
      testResultAttachments = testAttachments;
      return (IList<TestCaseResult>) testCaseResultList1;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] BulkUpdateTestResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      int[] resultIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result,
      out TeamProjectTestArtifacts teamProjectTestArtifacts,
      bool autoComputeTestRunState = true)
    {
      TeamProjectTestArtifacts testArtifacts = (TeamProjectTestArtifacts) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResultArray = this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(context.RequestContext, "TeamFoundationTestManagementResultService.BulkUpdateTestResults", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>) (() =>
      {
        if (resultIds == null || resultIds.Length == 0)
          return Array.Empty<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        if (resultIds.Length > 200)
          throw new InvalidPropertyException(nameof (resultIds), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BulkUpdateResultApiMaxLimitError, (object) 200));
        if (result.Id > 0)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultIdSpecifiedInBulkUpdateTestResults));
        resultIds = ((IEnumerable<int>) resultIds).Distinct<int>().ToArray<int>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(resultIds.Length);
        foreach (int resultId in resultIds)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult copyOf = this.CreateCopyOf(result);
          copyOf.Id = resultId;
          testCaseResultList.Add(copyOf);
        }
        return this.UpdateTestResults(context, projectReference, testRun, testCaseResultList.ToArray(), out testArtifacts, autoComputeTestRunState);
      }), 1015095, "TestResultsInsights");
      teamProjectTestArtifacts = testArtifacts;
      return testCaseResultArray;
    }

    public List<TestCaseResult> QueryTestResults(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      out List<TestCaseResultIdentifier> excessIds)
    {
      List<TestCaseResultIdentifier> extraIds = (List<TestCaseResultIdentifier>) null;
      List<TestCaseResult> testCaseResultList = this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.QueryTestResults", (Func<List<TestCaseResult>>) (() => TestCaseResult.Query(context, query, int.MaxValue, out extraIds)), 1015095, "TestResultsInsights");
      excessIds = extraIds;
      return testCaseResultList;
    }

    public List<TestCaseResult> QueryTestResultsByRun(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int runId)
    {
      List<TestCaseResultIdentifier> extraIds = (List<TestCaseResultIdentifier>) null;
      string projectName = projectReference.Name;
      List<TestActionResult> actionResults = new List<TestActionResult>();
      List<TestResultParameter> resultParameters = new List<TestResultParameter>();
      List<TestResultAttachment> resultAttachments = new List<TestResultAttachment>();
      return this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.QueryTestResults", (Func<List<TestCaseResult>>) (() => TestCaseResult.QueryByRun(context, runId, int.MaxValue, out extraIds, projectName, false, out actionResults, out resultParameters, out resultAttachments)), 1015095, "TestResultsInsights");
    }

    public TestResultArtifacts FetchTestResultArtifacts(
      TestManagementRequestContext context,
      int runId,
      int testCaseResultId,
      string projectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.FetchTestResultArtifacts"))
        return this.ExecuteAction<TestResultArtifacts>(context.RequestContext, "TeamFoundationTestManagementResultService.FetchTestResultArtifacts", (Func<TestResultArtifacts>) (() =>
        {
          List<TestActionResult> actionResults = new List<TestActionResult>();
          List<TestResultParameter> testResultParmeters = new List<TestResultParameter>();
          List<TestResultAttachment> testResultAttachments = new List<TestResultAttachment>();
          return new TestResultArtifacts(this.FetchTestCaseResult(context, runId, testCaseResultId, projectName, out actionResults, out testResultParmeters, out testResultAttachments), actionResults, testResultParmeters, testResultAttachments);
        }), 1015095, "TestResultsInsights");
    }

    public IList<TestResultArtifacts> FetchTestResultsArtifacts(
      TestManagementRequestContext context,
      List<TestCaseResult> results,
      string projectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.FetchTestResultArtifacts"))
        return this.ExecuteAction<IList<TestResultArtifacts>>(context.RequestContext, "TeamFoundationTestManagementResultService.FetchTestResultArtifacts", (Func<IList<TestResultArtifacts>>) (() =>
        {
          List<TestActionResult> actionResults = new List<TestActionResult>();
          List<TestResultParameter> testResultParmeters = new List<TestResultParameter>();
          List<TestResultAttachment> testResultAttachments = new List<TestResultAttachment>();
          return this.MapResultsToIterationDetails(this.FetchTestCaseResults(context, results, projectName, out actionResults, out testResultParmeters, out testResultAttachments).ToList<TestCaseResult>(), actionResults, testResultParmeters, testResultAttachments);
        }), 1015095, "TestResultsInsights");
    }

    public int GetIterationCount(List<TestActionResult> testActionResults)
    {
      int iterationCount = 0;
      if (testActionResults != null && testActionResults.Any<TestActionResult>())
        iterationCount = testActionResults[testActionResults.Count - 1].IterationId;
      return iterationCount;
    }

    public Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> GetTfsIdentityGuidToIdentityMappingForTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> testResults)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults"))
        return this.ExecuteAction<Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults", (Func<Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>>>) (() =>
        {
          Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults = new Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>>();
          if (testResults == null || !testResults.Any<TestCaseResult>())
            return new Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>>();
          foreach (KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identitesByAccountId in new TestManagementServiceUtility(context).ReadIdentitesByAccountIds(this.GetUniqueIdentitiyGuidsFromTestCaseResults(testResults)))
            forTestCaseResults.Add(identitesByAccountId.Key, new Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>(identitesByAccountId.Value, identitesByAccountId.Value.ToIdentityRef(context.RequestContext)));
          return forTestCaseResults;
        }), 1015095, "TestResultsInsights");
    }

    public Dictionary<string, ShallowReference> GetAreaPathUriMappingForTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> testResults)
    {
      return this.ExecuteAction<Dictionary<string, ShallowReference>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetAreaPathUriMappingForTestCaseResults", (Func<Dictionary<string, ShallowReference>>) (() =>
      {
        if (testResults == null || !testResults.Any<TestCaseResult>())
          return new Dictionary<string, ShallowReference>();
        Dictionary<string, ShallowReference> forTestCaseResults = new Dictionary<string, ShallowReference>();
        foreach (TestCaseResult testResult in testResults)
        {
          if (!string.IsNullOrEmpty(testResult.AreaUri) && !forTestCaseResults.ContainsKey(testResult.AreaUri))
            forTestCaseResults.Add(testResult.AreaUri, this.GetAreaPathShallowReferenceFromUri(context, testResult.AreaUri));
        }
        return forTestCaseResults;
      }), 1015095, "TestResultsInsights");
    }

    public TeamProjectTestArtifacts GetTeamProjectTestArtifacts(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      bool includeConfigurations = false,
      bool includeFailureTypes = true)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.GetTeamProjectTestArtifacts"))
        return this.ExecuteAction<TeamProjectTestArtifacts>(context.RequestContext, "TeamFoundationTestManagementResultService.GetTeamProjectTestArtifacts", (Func<TeamProjectTestArtifacts>) (() =>
        {
          TeamProjectTestArtifacts projectTestArtifacts = new TeamProjectTestArtifacts();
          if (includeConfigurations)
            projectTestArtifacts.TestConfigurations = this.TestConfigurationHelper.FetchConfigurations(context, projectReference.Name);
          if (includeFailureTypes)
          {
            projectTestArtifacts.FailureTypes = TestFailureType.Query(context, -1, projectReference.Name);
            projectTestArtifacts.ResolutionStates = TestResolutionState.Query(context, 0, projectReference.Name);
          }
          return projectTestArtifacts;
        }), 1015095, "TestResultsInsights");
    }

    public List<TestCaseResult> GetTestResultsByFQDN(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int buildId,
      int releaseId,
      int releaseEnvironmentId,
      string sourceWorkflow,
      List<TestCaseReference> testIdentities)
    {
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
        return managementDatabase.GetTestResultsByFQDN(projectReference.Id, releaseId, releaseEnvironmentId, buildId, sourceWorkflow, testIdentities);
    }

    public TestResultsDetails GetTestResultsGroupDetails(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      PipelineReference pipelineReference,
      bool shouldIncludeFailedAndAbortedResults = false,
      bool queryGroupSummaryForInProgress = false)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultGroupDetailsForPipeline";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          PipelineReferenceHelper.ValidateAndHandleDefaultValuesForPipelineRefInQuery(pipelineReference);
          IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState> source = (IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) new List<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>()
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Aborted,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.InProgress
          };
          TestResultsDetails groupedResults = new TestResultsDetails();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            groupedResults = managementDatabase.GetTestResultsGroupDetails(projectInfo.Id, pipelineReference, source != null ? (IList<byte>) source.Select<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>((System.Func<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>) (t => (byte) t)).ToList<byte>() : (IList<byte>) null, shouldIncludeFailedAndAbortedResults, queryGroupSummaryForInProgress);
          TestResultsDetails resultsGroupDetails = this.ResultsSortingHelper.OrderGroupsForGroupedResults(groupedResults, (Dictionary<string, string>) null, new List<string>()
          {
            ValidTestResultGroupByFields.TestRun
          });
          if (resultsGroupDetails != null)
            resultsGroupDetails.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
            {
              Project = new ShallowReference()
              {
                Id = projectInfo.Id.ToString()
              }
            });
          return resultsGroupDetails;
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public TestResultsDetails GetAggregatedTestResultDetailsForBuild(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int buildId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter = false,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectReference.Name);
      context.SecurityManager.CheckViewTestResultsPermission(context, projectFromName.String);
      this.ValidatePermissionCheckForPublicUsersForExpensiveCalls(context, shouldIncludeResults, groupByFields, filterValues);
      string buildUri = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", buildId.ToString()));
      Dictionary<int, int> oldTestCaseRefMap = new Dictionary<int, int>();
      orderBy = this.ValidateAndSetOrderBy(context, orderBy);
      bool shouldFetchOldTestCaseRefId = false;
      List<string> groupByFields1 = new List<string>((IEnumerable<string>) groupByFields);
      bool isRefLinkToRequirementFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement2");
      if (isRefLinkToRequirementFFEnabled && this.IsGroupByRequirement(groupByFields))
      {
        groupByFields1 = new List<string>() { string.Empty };
        shouldFetchOldTestCaseRefId = context.IsFeatureEnabled("TestManagement.Server.ShouldFetchOldTestCaseRefId");
      }
      bool isAbortedRunEnabled = context.IsFeatureEnabled("TestManagement.Server.ReportingAbortedRuns");
      bool isInProgressRunsEnabled = context.IsFeatureEnabled("TestManagement.Server.ReportingInProgressRuns");
      bool isDefaultFilterWithFilteredIndex = this.IsDefaultFilterWithFilteredIndex(context, filterValues);
      int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
      TestResultsDetails testResultsDetails;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
      {
        TestResultsDetailsGroupData resultsForBuild4 = managementDatabase.GetAggregatedTestResultsForBuild4(projectReference.Id, buildId, buildUri, sourceWorkflow, groupByFields1, filterValues, orderBy, isRerunOnPassedFilter, isAbortedRunEnabled, isInProgressRunsEnabled, shouldIncludeResults, queryRunSummaryForInProgress, isDefaultFilterWithFilteredIndex, shouldFetchOldTestCaseRefId, progressOrFailed);
        testResultsDetails = resultsForBuild4.TestResultsDetails;
        oldTestCaseRefMap = resultsForBuild4.OldTestCaseRefIdMap;
      }
      this.UpdateAggregatedResultsWithPlanName(context, projectReference.Id, testResultsDetails, groupByFields);
      this.UpdateAggregatedResultsWithWorkItemName(context, projectReference.Id, testResultsDetails, groupByFields, isRefLinkToRequirementFFEnabled, oldTestCaseRefMap);
      return this.ResultsSortingHelper.OrderGroupsForGroupedResults(testResultsDetails, orderBy, groupByFields);
    }

    public TestResultsDetails GetAggregatedTestResultDetailsForRelease(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter = false,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectReference.Name);
      context.SecurityManager.CheckViewTestResultsPermission(context, projectFromName.String);
      this.ValidatePermissionCheckForPublicUsersForExpensiveCalls(context, shouldIncludeResults, groupByFields, filterValues);
      Dictionary<int, int> oldTestCaseRefMap = new Dictionary<int, int>();
      orderBy = this.ValidateAndSetOrderBy(context, orderBy);
      bool shouldFetchOldTestCaseRefId = false;
      List<string> groupByFields1 = new List<string>((IEnumerable<string>) groupByFields);
      bool isRefLinkToRequirementFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement2");
      if (isRefLinkToRequirementFFEnabled && this.IsGroupByRequirement(groupByFields))
      {
        groupByFields1 = new List<string>() { string.Empty };
        shouldFetchOldTestCaseRefId = context.IsFeatureEnabled("TestManagement.Server.ShouldFetchOldTestCaseRefId");
      }
      bool isAbortedRunEnabled = context.IsFeatureEnabled("TestManagement.Server.ReportingAbortedRuns");
      bool isInProgressRunsEnabled = context.IsFeatureEnabled("TestManagement.Server.ReportingInProgressRuns");
      bool isDefaultFilterWithFilteredIndex = this.IsDefaultFilterWithFilteredIndex(context, filterValues);
      int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
      TestResultsDetails testResultsDetails;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
      {
        TestResultsDetailsGroupData resultsForRelease4 = managementDatabase.GetAggregatedTestResultsForRelease4(projectReference.Id, releaseId, releaseEnvId, sourceWorkflow, groupByFields1, filterValues, orderBy, isRerunOnPassedFilter, isAbortedRunEnabled, isInProgressRunsEnabled, shouldIncludeResults, queryRunSummaryForInProgress, isDefaultFilterWithFilteredIndex, shouldFetchOldTestCaseRefId, progressOrFailed);
        testResultsDetails = resultsForRelease4.TestResultsDetails;
        oldTestCaseRefMap = resultsForRelease4.OldTestCaseRefIdMap;
      }
      this.UpdateAggregatedResultsWithPlanName(context, projectReference.Id, testResultsDetails, groupByFields);
      this.UpdateAggregatedResultsWithWorkItemName(context, projectReference.Id, testResultsDetails, groupByFields, isRefLinkToRequirementFFEnabled, oldTestCaseRefMap);
      return this.ResultsSortingHelper.OrderGroupsForGroupedResults(testResultsDetails, orderBy, groupByFields);
    }

    private TestCaseResult[] RemoveResultsWithDuplicatePointId(
      TestManagementRequestContext context,
      TestCaseResult[] results,
      int testRunId,
      int testPlanId)
    {
      int length = results.Length;
      Dictionary<int, TestCaseResult> dictionary = new Dictionary<int, TestCaseResult>();
      for (int index = 0; index < length; ++index)
        dictionary[((IEnumerable<TestCaseResult>) results).ElementAt<TestCaseResult>(index).TestPointId] = ((IEnumerable<TestCaseResult>) results).ElementAt<TestCaseResult>(index);
      results = dictionary.Values.ToArray<TestCaseResult>();
      int num = length - results.Length;
      TestManagementServiceUtility.PublishTelemetry(context.RequestContext, "DuplicateResultsWithSamePointId", new Dictionary<string, object>()
      {
        {
          "TestRunId",
          (object) testRunId
        },
        {
          "PlanId",
          (object) testPlanId
        },
        {
          "CountOfDuplicates",
          (object) num
        }
      });
      return results;
    }

    private void ValidatePermissionCheckForPublicUsersForExpensiveCalls(
      TestManagementRequestContext context,
      bool shouldIncludeResults,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues)
    {
      if (!shouldIncludeResults || this.IsDefaultFlow(context, groupByFields, filterValues))
        return;
      context.SecurityManager.CheckTestManagementPermission(context);
    }

    private bool IsDefaultFlow(
      TestManagementRequestContext context,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues)
    {
      return this.IsGroupByTestRun(groupByFields) && this.IsDefaultFilterWithFilteredIndex(context, filterValues);
    }

    private bool IsDefaultFilterWithFilteredIndex(
      TestManagementRequestContext context,
      Dictionary<string, Tuple<string, List<string>>> filterValues)
    {
      bool flag = false;
      if (context.IsFeatureEnabled("TestManagement.Server.TestOutcomeIndex") && filterValues != null)
      {
        foreach (KeyValuePair<string, Tuple<string, List<string>>> filterValue in filterValues)
        {
          if (string.Equals(filterValue.Key, TestResultsConstants.OutcomeColumnName, StringComparison.InvariantCultureIgnoreCase))
          {
            foreach (string str in filterValue.Value.Item2)
            {
              if (3 == Convert.ToInt32(str) || 6 == Convert.ToInt32(str))
              {
                flag = true;
              }
              else
              {
                flag = false;
                break;
              }
            }
          }
        }
      }
      return flag;
    }

    private int GetMaxOrderByCount(TestManagementRequestContext context) => context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxOrderByCount", 5000);

    private int GetMaxBranchLimitForTestFlakiness(TestManagementRequestContext context) => context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxBranchLimitForTestFlakiness", 5);

    private void UpdateAggregatedResultsWithWorkItemName(
      TestManagementRequestContext context,
      Guid projectId,
      TestResultsDetails groupedResults,
      List<string> groupByFields,
      bool isRefLinkToRequirementFFEnabled,
      Dictionary<int, int> oldTestCaseRefMap)
    {
      if (!this.IsGroupByRequirement(groupByFields) || groupedResults == null)
        return;
      if (isRefLinkToRequirementFFEnabled)
      {
        Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> testCaseRefIdToResultMap = new Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>();
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in groupedResults.ResultsForGroup.Select<TestResultsDetailsForGroup, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((System.Func<TestResultsDetailsForGroup, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (grres => grres.Results)).SelectMany<IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((System.Func<IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>, IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (r => (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) r)))
        {
          testCaseRefIdToResultMap[testCaseResult.TestCaseReferenceId] = !testCaseRefIdToResultMap.ContainsKey(testCaseResult.TestCaseReferenceId) ? new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : testCaseRefIdToResultMap[testCaseResult.TestCaseReferenceId];
          testCaseRefIdToResultMap[testCaseResult.TestCaseReferenceId].Add(testCaseResult);
        }
        groupedResults.GroupByField = ValidTestResultGroupByFields.Requirement;
        groupedResults.ResultsForGroup = this.GetGroupDetails(context, projectId, testCaseRefIdToResultMap, oldTestCaseRefMap);
      }
      else
      {
        IList<TestResultsDetailsForGroup> resultsForGroup = groupedResults.ResultsForGroup;
        List<int> workItemIds = new List<int>();
        foreach (TestResultsDetailsForGroup resultsDetailsForGroup in (IEnumerable<TestResultsDetailsForGroup>) resultsForGroup)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference groupByValue = resultsDetailsForGroup.GroupByValue as Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference;
          int result = 0;
          if (groupByValue != null && int.TryParse(groupByValue.Id, out result))
            workItemIds.Add(result);
        }
        List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> workItemReference1 = new TestResultsWorkItemHelper().GetWorkItemReference(context.RequestContext, workItemIds, true);
        foreach (TestResultsDetailsForGroup resultsDetailsForGroup in (IEnumerable<TestResultsDetailsForGroup>) resultsForGroup)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference witRef = resultsDetailsForGroup.GroupByValue as Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference;
          if (witRef != null)
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference workItemReference2 = workItemReference1.Where<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, bool>) (wi => string.Equals(wi.Id, witRef.Id, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
            if (workItemReference2 != null)
              resultsDetailsForGroup.GroupByValue = (object) workItemReference2;
          }
        }
      }
    }

    private IList<TestResultsDetailsForGroup> GetGroupDetails(
      TestManagementRequestContext context,
      Guid projectId,
      Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> testCaseRefIdToResultMap,
      Dictionary<int, int> oldTestCaseRefMap)
    {
      ITestManagementLinkedWorkItemService service = context.RequestContext.GetService<ITestManagementLinkedWorkItemService>();
      Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> testCaseReference = service.BatchCreateWorkItemsRecordsForTestCaseReference(context, projectId, "Microsoft.RequirementCategory", testCaseRefIdToResultMap.Keys.ToList<int>(), 0);
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultMap = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<int, TestResultsDetailsForGroup> workItemToGroupMap = new Dictionary<int, TestResultsDetailsForGroup>();
      if (testCaseReference != null && testCaseReference.Any<KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>>())
      {
        foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> uriToWitPair in testCaseReference)
        {
          int artifactId = TestManagementServiceUtility.GetArtifactId(uriToWitPair.Key);
          this.GroupByResultsForWorkItems(testCaseRefIdToResultMap, resultMap, workItemToGroupMap, uriToWitPair, artifactId);
        }
      }
      if (oldTestCaseRefMap != null && oldTestCaseRefMap.Any<KeyValuePair<int, int>>())
      {
        List<string> list = oldTestCaseRefMap.Keys.Select<int, string>((System.Func<int, string>) (t => TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(t, false))).ToList<string>();
        Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> recordsForArtifactUris = service.BatchCreateWorkItemsRecordsForArtifactUris(context, projectId, "Microsoft.RequirementCategory", list, 0);
        if (recordsForArtifactUris != null && recordsForArtifactUris.Any<KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>>())
        {
          foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> uriToWitPair in recordsForArtifactUris)
          {
            int oldTestCaseRef = oldTestCaseRefMap[TestManagementServiceUtility.GetArtifactId(uriToWitPair.Key)];
            this.GroupByResultsForWorkItems(testCaseRefIdToResultMap, resultMap, workItemToGroupMap, uriToWitPair, oldTestCaseRef);
          }
        }
      }
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> source = testCaseRefIdToResultMap.Values.SelectMany<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((System.Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>, IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (r => (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) r)).Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (r => !resultMap.ContainsKey(new TestCaseResultIdentifier(Convert.ToInt32(r.TestRun.Id), r.Id))));
      if (source.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
      {
        Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> resByOutcome = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
        source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>().ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r =>
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome enumValue = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>(r.Outcome, Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.None);
          Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> dictionary = resByOutcome;
          int key = (int) enumValue;
          AggregatedResultsByOutcome resultsByOutcome;
          if (resByOutcome.ContainsKey(enumValue))
          {
            resultsByOutcome = resByOutcome[enumValue];
          }
          else
          {
            resultsByOutcome = new AggregatedResultsByOutcome();
            resultsByOutcome.Outcome = enumValue;
            resultsByOutcome.Count = 0;
          }
          dictionary[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key] = resultsByOutcome;
          ++resByOutcome[enumValue].Count;
          resByOutcome[enumValue].Duration = Validator.CheckOverflowAndGetSafeValue(resByOutcome[enumValue].Duration, TimeSpan.FromMilliseconds(r.DurationInMs));
        }));
        workItemToGroupMap[-1] = new TestResultsDetailsForGroup()
        {
          GroupByValue = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference(),
          Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(),
          ResultsCountByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) resByOutcome
        };
      }
      return (IList<TestResultsDetailsForGroup>) workItemToGroupMap.Values.ToList<TestResultsDetailsForGroup>();
    }

    internal void UpdateAggregatedResultsWithPlanName(
      TestManagementRequestContext context,
      Guid projectId,
      TestResultsDetails groupedResults,
      List<string> groupByFields)
    {
      if (!this.IsGroupBySuite(groupByFields) || groupedResults == null)
        return;
      List<TestSuite> aggregatedResults = this.GetRootSuitesFromAggregatedResults(groupedResults.ResultsForGroup);
      List<int> idsForRootSuites = this.GetTestPlanIdsForRootSuites(aggregatedResults);
      Dictionary<int, string> testPlanTitles = context.PlannedTestResultsHelper.GetTestPlanTitles(context, projectId, idsForRootSuites);
      if (testPlanTitles == null)
        return;
      this.UpdateAggregatedResultsWithSuiteName(aggregatedResults, testPlanTitles);
    }

    private void GroupByResultsForWorkItems(
      Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> testCaseRefIdToResultMap,
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultMap,
      Dictionary<int, TestResultsDetailsForGroup> workItemToGroupMap,
      KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> uriToWitPair,
      int tcRefId)
    {
      if (!testCaseRefIdToResultMap.ContainsKey(tcRefId))
        return;
      uriToWitPair.Value.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (wit =>
      {
        int witId = Convert.ToInt32(wit.Id);
        Dictionary<int, TestResultsDetailsForGroup> dictionary = workItemToGroupMap;
        int key1 = witId;
        TestResultsDetailsForGroup resultsDetailsForGroup;
        if (workItemToGroupMap.ContainsKey(witId))
        {
          resultsDetailsForGroup = workItemToGroupMap[witId];
        }
        else
        {
          resultsDetailsForGroup = new TestResultsDetailsForGroup();
          resultsDetailsForGroup.GroupByValue = (object) wit;
          resultsDetailsForGroup.Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          resultsDetailsForGroup.ResultsCountByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>();
        }
        dictionary[key1] = resultsDetailsForGroup;
        workItemToGroupMap[witId].Results.AddRange<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseRefIdToResultMap[tcRefId]);
        testCaseRefIdToResultMap[tcRefId].ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r =>
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome enumValue = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>(r.Outcome, Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.None);
          IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome> resultsCountByOutcome = workItemToGroupMap[witId].ResultsCountByOutcome;
          int key2 = (int) enumValue;
          AggregatedResultsByOutcome resultsByOutcome;
          if (workItemToGroupMap[witId].ResultsCountByOutcome.ContainsKey(enumValue))
          {
            resultsByOutcome = workItemToGroupMap[witId].ResultsCountByOutcome[enumValue];
          }
          else
          {
            resultsByOutcome = new AggregatedResultsByOutcome();
            resultsByOutcome.Outcome = enumValue;
            resultsByOutcome.Count = 0;
            resultsByOutcome.Duration = new TimeSpan();
          }
          resultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key2] = resultsByOutcome;
          ++workItemToGroupMap[witId].ResultsCountByOutcome[enumValue].Count;
          workItemToGroupMap[witId].ResultsCountByOutcome[enumValue].Duration = Validator.CheckOverflowAndGetSafeValue(workItemToGroupMap[witId].ResultsCountByOutcome[enumValue].Duration, TimeSpan.FromMilliseconds(r.DurationInMs));
          resultMap[new TestCaseResultIdentifier(Convert.ToInt32(r.TestRun.Id), r.Id)] = r;
        }));
      }));
    }

    private Dictionary<string, string> ValidateAndSetOrderBy(
      TestManagementRequestContext context,
      Dictionary<string, string> orderBy)
    {
      if (orderBy == null)
      {
        orderBy = new Dictionary<string, string>();
        orderBy.Add(TestResultsConstants.TestCaseTitleColumnName, ODataQueryConstants.OrderByAsc);
      }
      return orderBy;
    }

    public List<TestCaseResult> GetTestCaseResultsByIds(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      List<TestCaseResultIdentifier> resultIds,
      List<string> fields)
    {
      return this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetTestCaseResultsByIds", (Func<List<TestCaseResult>>) (() =>
      {
        context.SecurityManager.CheckViewTestResultsPermission(context, context.ProjectServiceHelper.GetProjectFromGuid(projectReference.Id).Uri);
        List<TestCaseResult> results = new List<TestCaseResult>();
        if (resultIds != null)
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            results = managementDatabase.GetTestCaseResultsByIds(projectReference.Id, resultIds, fields);
          if (resultIds.Any<TestCaseResultIdentifier>() && TestResultExtensionLogstore.ShouldTestExtensionBeStoredInLogstore(context, fields, resultIds[0].TestRunId, projectReference.Id))
            TestResultExtensionLogstore.FetchMutlipleResultExFromLogstore(context, ref results, projectReference.Id, fields);
        }
        return results;
      }), 1015095, "TestResultsInsights");
    }

    public List<TestCaseResult> GetTestCaseResultsByPointIds(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int planId,
      List<int> pointIds)
    {
      return this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetTestCaseResultsByPointIds", (Func<List<TestCaseResult>>) (() =>
      {
        context.SecurityManager.CheckViewTestResultsPermission(context, context.ProjectServiceHelper.GetProjectFromGuid(projectReference.Id).Uri);
        if (context.IsFeatureEnabled("TestManagement.Server.BypassPointResultDetails"))
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            return managementDatabase.GetTestCaseResultsByPointIds2(projectReference.Id, planId, pointIds);
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            return managementDatabase.GetTestCaseResultsByPointIds(projectReference.Id, planId, pointIds);
        }
      }), 1015095, "TestResultsInsights");
    }

    public List<TestCaseResult> QueryTestResultHistory(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.QueryTestResultHistory"))
        return this.ExecuteAction<List<TestCaseResult>>(context.RequestContext, "TeamFoundationTestManagementResultService.QueryTestResultHistory", (Func<List<TestCaseResult>>) (() =>
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            return managementDatabase.QueryTestResultHistory(projectReference.Id, automatedTestName, testCaseId, maxCompleteDate, historyDays);
        }), 1015095, "TestResultsInsights");
    }

    public TestResultHistory QueryTestCaseResultHistory(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      ResultsFilter filter)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultService.QueryTestCaseResultHistory"))
      {
        int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
        string groupBy = filter.GroupBy;
        if (string.IsNullOrEmpty(groupBy) || !groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase) && !groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
          throw new InvalidPropertyException("GroupBy", ServerResources.InvalidValueSpecified);
        TestResultHistory groupedResults;
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamicForNonWIQLTypeQueries"))
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            groupedResults = managementDatabase.QueryTestCaseResultHistory2(projectReference.Id, filter, progressOrFailed);
        }
        else
        {
          bool isTfvcBranchFilteringEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.FilteringTfvcGatedCheckInBranches");
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            groupedResults = managementDatabase.QueryTestCaseResultHistory(projectReference.Id, filter, isTfvcBranchFilteringEnabled);
        }
        this.ResolveResultHistoryGroupNames(context, projectReference, filter, groupedResults);
        return groupedResults;
      }
    }

    public TestHistoryQuery QueryTestHistory(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestHistoryQuery filter)
    {
      using (PerfManager.Measure(context.RequestContext, "RestLayer", "TeamFoundationTestManagementResultService.QueryTestHistory"))
      {
        TestHistoryQuery testHistoryQuery = this._QueryTestHistory(context, projectInfo, filter);
        this.SecurifyTestResultHistory(testHistoryQuery, projectInfo.Id);
        return testHistoryQuery;
      }
    }

    public List<TestCaseReferenceRecord> QueryTestCaseReferencesByChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int testCaseRefBatchSize,
      TestCaseReferenceWatermark fromWatermark,
      out TestCaseReferenceWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementResultService.QueryTestCaseReferencesByChangedDate";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
            return replicaAwareComponent.QueryTestCaseReferenceByChangedDate(projectId, testCaseRefBatchSize, fromWatermark, out toWatermark, dataSource);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public List<TestResultRecord> QueryTestResultsByTestRunChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int runBatchSize,
      int resultBatchSize,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource source = TestArtifactSource.Tfs)
    {
      string str = "TeamFoundationTestManagementResultService.QueryTestResultsByTestRunChangedDate";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        string prBranchName = (string) null;
        if (context.RequestContext.GetService<IVssRegistryService>().GetValue<bool>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/DisablePublishPRTestDataToAXService", false))
          prBranchName = "refs/pull/*";
        bool includeFlakyData = context.RequestContext.IsFeatureEnabled("TestManagement.Server.StageFlakyDataWithTestResults");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
            return replicaAwareComponent.QueryTestResultsByTestRunChangedDate(projectId, runBatchSize, resultBatchSize, prBranchName, fromWatermark, out toWatermark, source, includeFlakyData);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public List<TestResultExArchivalRecord> QueryTestResultExtensionsByTestRunChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int runBatchSize,
      int resultExBatchSize,
      TestResultExArchivalWatermark fromWatermark,
      DateTime maxTestRunUpdatedDate,
      out TestResultExArchivalWatermark toWatermark,
      TestArtifactSource source = TestArtifactSource.Tcm)
    {
      string str = "TeamFoundationTestManagementResultService.QueryTestResultExtensionsByTestRunChangedDate";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckForNull<TestResultExArchivalWatermark>(fromWatermark, nameof (fromWatermark));
          ArgumentUtility.CheckForDateTimeRange(fromWatermark.TestRunUpdatedDate, nameof (fromWatermark), new DateTime(), maxTestRunUpdatedDate);
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
            return replicaAwareComponent.QueryTestResultExtensionsByTestRunChangedDate(projectId, runBatchSize, resultExBatchSize, fromWatermark, maxTestRunUpdatedDate, out toWatermark, source);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public TestResultsGroupsForBuild GetTestResultGroupsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultGroupsByBuild";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), "Test Results");
          this.ValidateInputFieldsAndPublishContext(fields, publishContext);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
          TestResultsGroupsData andOwnersByBuild;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            andOwnersByBuild = managementDatabase.GetTestResultAutomatedTestStorageAndOwnersByBuild(projectInfo.Id, buildId, publishContext, progressOrFailed);
          TestResultsGroupsForBuild testResultsGroupsForBuild = this.PopulateTestResultsGroupsForBuild(buildId, andOwnersByBuild, fields);
          this.SecureTestResultsGroupsForBuild(testResultsGroupsForBuild, projectInfo.Id);
          return testResultsGroupsForBuild;
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public TestResultsGroupsForRelease GetTestResultGroupsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string fields,
      string publishContext)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultGroupsByRelease";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) releaseId, nameof (releaseId), "Test Results");
          ArgumentUtility.CheckGreaterThanOrEqualToZero((float) releaseEnvId, nameof (releaseEnvId), "Test Results");
          this.ValidateInputFieldsAndPublishContext(fields, publishContext);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
          TestResultsGroupsData andOwnersByRelease;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            andOwnersByRelease = managementDatabase.GetTestResultAutomatedTestStorageAndOwnersByRelease(projectInfo.Id, releaseId, releaseEnvId, publishContext, progressOrFailed);
          TestResultsGroupsForRelease testResultsGroupsForRelease = this.PopulateTestResultsGroupsForRelease(releaseId, releaseEnvId, andOwnersByRelease, fields);
          this.SecureTestResultsGroupsForRelease(testResultsGroupsForRelease, projectInfo.Id);
          return testResultsGroupsForRelease;
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public IPagedList<FieldDetailsForTestResults> GetTestResultGroupsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultGroupsByBuild";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), "Test Results");
          this.ValidateInputFieldsAndPublishContext(fields, publishContext);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          int top = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/BatchSizeForResultsFieldValues", 50000);
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
          TestResultsGroupsDataWithWaterMark andOwnersByBuild;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            andOwnersByBuild = managementDatabase.GetTestResultAutomatedTestStorageAndOwnersByBuild(projectInfo.Id, buildId, publishContext, continuationTokenRunId, continuationTokenResultId, top, progressOrFailed);
          return this.PopulateTestResultsGroups(andOwnersByBuild, fields);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public IPagedList<FieldDetailsForTestResults> GetTestResultGroupsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string fields,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultGroupsByRelease";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) releaseId, nameof (releaseId), "Test Results");
          ArgumentUtility.CheckGreaterThanOrEqualToZero((float) releaseEnvId, nameof (releaseEnvId), "Test Results");
          this.ValidateInputFieldsAndPublishContext(fields, publishContext);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          int top = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/BatchSizeForResultsFieldValues", 50000);
          int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
          TestResultsGroupsDataWithWaterMark andOwnersByRelease;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            andOwnersByRelease = managementDatabase.GetTestResultAutomatedTestStorageAndOwnersByRelease(projectInfo.Id, releaseId, releaseEnvId, publishContext, continuationTokenRunId, continuationTokenResultId, top, progressOrFailed);
          return this.PopulateTestResultsGroups(andOwnersByRelease, fields);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public IList<ShallowTestCaseResult> GetTestResultsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultsByBuild";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), "Test Results");
          this.ValidatePageSize(context.RequestContext, top);
          this.ValidatePublishContext(publishContext);
          this.ValidateOutcomeFilters(outcomes);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState> source = (IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) new List<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>()
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation
          };
          if (context.IsFeatureEnabled("TestManagement.Server.ReportingAbortedRuns"))
            source.Add(Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Aborted);
          if (context.IsFeatureEnabled("TestManagement.Server.ReportingInProgressRuns"))
            source.Add(Microsoft.TeamFoundation.TestManagement.Client.TestRunState.InProgress);
          bool fetchFailedTestsOnly = outcomes != null && (outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed) || outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Aborted));
          List<TestCaseResult> serverTestResults;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            serverTestResults = managementDatabase.QueryTestResultsByBuildOrRelease(projectInfo.Id, buildId, 0, 0, publishContext, source != null ? (IList<byte>) source.Select<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>((System.Func<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>) (t => (byte) t)).ToList<byte>() : (IList<byte>) null, fetchFailedTestsOnly, continuationTokenRunId, continuationTokenResultId, top);
          return this.PopulateShallowTestResults(context, (IList<TestCaseResult>) serverTestResults, projectInfo.Id);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public IList<TestCaseReferenceRecord> GetTestResultsMetaData(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      IList<int> testReferenceIds,
      bool shouldFetchFlakyDetails = false)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultsMetaData";
      try
      {
        List<TestCaseReferenceRecord> testResultsMetaData = new List<TestCaseReferenceRecord>();
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            testResultsMetaData = managementDatabase.GetTestResultsMetaData(projectInfo.Id, testReferenceIds, shouldFetchFlakyDetails);
          return (IList<TestCaseReferenceRecord>) testResultsMetaData;
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public TestResultMetaData UpdateTestResultsMetaData(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int testCaseReferenceId,
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput)
    {
      string str = "TeamFoundationTestManagementResultService.UpdateTestResultsMetaData";
      this.ValidateUpdateTestResultsMetaDatParameters(projectInfo, testCaseReferenceId, testResultMetaDataUpdateInput);
      context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "RestLayer", str);
      IList<TestCaseReferenceRecord> testResultsMetaData = this.GetTestResultsMetaData(context, projectInfo, (IList<int>) new List<int>()
      {
        testCaseReferenceId
      }, false);
      if (testResultsMetaData == null || testResultsMetaData.Count == 0)
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseReferenceNotFoundError, (object) testCaseReferenceId));
      this.UpdateTestResultsMetaDataUtil(context, projectInfo, testCaseReferenceId, testResultMetaDataUpdateInput, str);
      return new TestResultMetaData()
      {
        TestCaseReferenceId = testCaseReferenceId
      };
    }

    public IList<ShallowTestCaseResult> GetTestResultsByPipeline(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      PipelineReference pipelineReference,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultsByPipeline";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          PipelineReferenceHelper.ValidateAndHandleDefaultValuesForPipelineRefInQuery(pipelineReference);
          this.ValidatePageSize(context.RequestContext, top);
          this.ValidateOutcomeFilters(outcomes);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState> source = (IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) new List<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>()
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Aborted,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.InProgress
          };
          bool fetchFailedTestsOnly = outcomes != null && (outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed) || outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Aborted));
          List<TestCaseResult> serverTestResults;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            serverTestResults = managementDatabase.QueryTestResultsByPipeline(projectInfo.Id, pipelineReference, source != null ? (IList<byte>) source.Select<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>((System.Func<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>) (t => (byte) t)).ToList<byte>() : (IList<byte>) null, fetchFailedTestsOnly, continuationTokenRunId, continuationTokenResultId, top);
          return this.PopulateShallowTestResults(context, (IList<TestCaseResult>) serverTestResults, projectInfo.Id);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    public IList<ShallowTestCaseResult> GetTestResultsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      string str = "TeamFoundationTestManagementResultService.GetTestResultsByRelease";
      try
      {
        context.RequestContext.TraceEnter(1015095, "TestResultsInsights", "BusinessLayer", str);
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          ArgumentUtility.CheckGreaterThanZero((float) releaseId, nameof (releaseId), "Test Results");
          this.ValidatePageSize(context.RequestContext, top);
          this.ValidatePublishContext(publishContext);
          this.ValidateOutcomeFilters(outcomes);
          context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
          IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState> source = (IList<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>) new List<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>()
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed,
            Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NeedsInvestigation
          };
          if (context.IsFeatureEnabled("TestManagement.Server.ReportingAbortedRuns"))
            source.Add(Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Aborted);
          if (context.IsFeatureEnabled("TestManagement.Server.ReportingInProgressRuns"))
            source.Add(Microsoft.TeamFoundation.TestManagement.Client.TestRunState.InProgress);
          bool fetchFailedTestsOnly = outcomes != null && (outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed) || outcomes.Contains(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Aborted));
          List<TestCaseResult> serverTestResults;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
            serverTestResults = managementDatabase.QueryTestResultsByBuildOrRelease(projectInfo.Id, 0, releaseId, releaseEnvId, publishContext, source != null ? (IList<byte>) source.Select<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>((System.Func<Microsoft.TeamFoundation.TestManagement.Client.TestRunState, byte>) (t => (byte) t)).ToList<byte>() : (IList<byte>) null, fetchFailedTestsOnly, continuationTokenRunId, continuationTokenResultId, top);
          return this.PopulateShallowTestResults(context, (IList<TestCaseResult>) serverTestResults, projectInfo.Id);
        }
      }
      finally
      {
        context.RequestContext.TraceLeave(1015095, "TestResultsInsights", "BusinessLayer", str);
      }
    }

    internal List<Tuple<TestCaseResult, List<WorkItem>>> GetTestCaseResultsFromResultCreateModel(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestRun run,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache,
      bool testSessionProperties = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementResultsService.GetTestCaseResultsFromResultCreateModel"))
      {
        List<Tuple<TestCaseResult, List<WorkItem>>> resultCreateModel = new List<Tuple<TestCaseResult, List<WorkItem>>>();
        List<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>> resultsMap = new List<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>>();
        Dictionary<int, TestPoint> dictionary = new Dictionary<int, TestPoint>();
        TeamProjectTestArtifacts projectTestArtifacts = (TeamProjectTestArtifacts) null;
        bool hierarchicalResultFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResult");
        IList<TestExtensionFieldDetails> fieldDefinitions = this.GetResultExtensionFieldDefinitions(context, projectRef);
        int totalResults = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results).Count<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        int resultsWithOwner = 0;
        int resultsWithValidOwnerDisplayName = 0;
        int resultsWithValidOwnerDirectoryAlias = 0;
        int totalSubResults = 0;
        int hierarchicalResults = 0;
        int rerunCount = 0;
        int ddCount = 0;
        int otCount = 0;
        int genericCount = 0;
        int num = 0;
        int passedOnRerunCount = 0;
        int rerunDD = 0;
        int rerunOT = 0;
        int rerunGeneric = 0;
        int maxAttemptId = 0;
        int failedResults = 0;
        double sizeOfComments = 0.0;
        double sizeOfErrorMessage = 0.0;
        double sizeOfStackTrace = 0.0;
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result1 in results)
        {
          TestCaseResult result2 = new TestCaseResult();
          result2.TestRunId = run.TestRunId;
          if (context.IsTcmService || run.TestPlanId <= 0)
          {
            byte result3;
            if (!byte.TryParse(result1.Priority.ToString(), out result3))
              throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "Priority"));
            result2.Priority = result3;
          }
          result2.Duration = 0L;
          TimeSpan timeSpan;
          if (result1.DurationInMs >= 0.0)
          {
            TestCaseResult testCaseResult = result2;
            timeSpan = TimeSpan.FromMilliseconds(result1.DurationInMs);
            long ticks = timeSpan.Ticks;
            testCaseResult.Duration = ticks;
          }
          bool flag1 = TestManagementServiceUtility.CheckIfDateIsDefaultOrOutsideAllowedRange(context.RequestContext, result1.StartedDate);
          bool flag2 = TestManagementServiceUtility.CheckIfDateIsDefaultOrOutsideAllowedRange(context.RequestContext, result1.CompletedDate);
          if (!flag1 && !flag2)
          {
            result2.DateStarted = result1.StartedDate.ToUniversalTime();
            result2.DateCompleted = result1.CompletedDate.ToUniversalTime();
          }
          else if (!flag1)
          {
            result2.DateStarted = result1.StartedDate.ToUniversalTime();
            result2.DateCompleted = result2.DateStarted.AddTicks(result2.Duration);
          }
          else if (!flag2)
          {
            result2.DateCompleted = result1.CompletedDate.ToUniversalTime();
            result2.DateStarted = result2.DateCompleted.AddTicks(-result2.Duration);
          }
          else
          {
            result2.DateStarted = DateTime.UtcNow;
            result2.DateCompleted = result2.DateStarted.AddTicks(result2.Duration);
          }
          if (result2.DateCompleted < result2.DateStarted)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultCompletedDateGreaterThanStartDate));
          if (result2.Duration == 0L)
          {
            TestCaseResult testCaseResult = result2;
            timeSpan = result2.DateCompleted - result2.DateStarted;
            long ticks = timeSpan.Ticks;
            testCaseResult.Duration = ticks;
          }
          if (projectTestArtifacts == null && (result1.Configuration != null || !string.IsNullOrEmpty(result1.ResolutionState) || !string.IsNullOrEmpty(result1.FailureType)))
            projectTestArtifacts = this.GetTeamProjectTestArtifacts(context, projectRef, true, true);
          int result4;
          if (result1.Configuration != null && int.TryParse(result1.Configuration.Id, out result4) && result4 > 0)
          {
            result2.ConfigurationId = result4;
            result2.ConfigurationName = result1.Configuration.Name;
          }
          if (!string.IsNullOrEmpty(result1.ResolutionState))
            result2.ResolutionStateId = (int) Convert.ToByte(this.GetResolutionStateIdFromResolutionStateName(result1.ResolutionState, projectTestArtifacts.ResolutionStates));
          if (!string.IsNullOrEmpty(result1.FailureType))
            result2.FailureType = Convert.ToByte(TestManagementServiceUtility.GetFailureTypeIdFromFailureTypeName(context, result1.FailureType, projectTestArtifacts.FailureTypes));
          result2.ErrorMessage = result1.ErrorMessage;
          result2.Comment = result1.Comment;
          result2.ComputerName = result1.ComputerName;
          if (!string.IsNullOrEmpty(result1.Outcome))
            result2.Outcome = this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result1.Outcome, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None);
          if (result2.Outcome == (byte) 3)
            ++failedResults;
          if (!string.IsNullOrEmpty(result1.Comment))
            sizeOfComments += (double) result1.Comment.Length;
          if (!string.IsNullOrEmpty(result1.ErrorMessage))
            sizeOfErrorMessage += (double) result1.ErrorMessage.Length;
          if (!string.IsNullOrEmpty(result1.StackTrace))
            sizeOfStackTrace += (double) result1.StackTrace.Length;
          if (!string.IsNullOrEmpty(result1.State))
            result2.State = this.ValidateAndGetEnumValue<TestResultState>(result1.State, TestResultState.Pending);
          Guid ownerId;
          string ownerName;
          this.PopulateResultOwner(result1.Owner, tfsIdentityCache, ref resultsWithOwner, ref resultsWithValidOwnerDirectoryAlias, ref resultsWithValidOwnerDisplayName, out ownerId, out ownerName);
          result2.Owner = ownerId;
          result2.OwnerName = ownerName;
          if (testSessionProperties)
          {
            result2.TopologyId = result1.TopologyId;
            result2.IsSystemIssue = result1.IsSystemIssue;
            result2.BucketUid = result1.BucketUid;
            result2.ExceptionType = result1.ExceptionType;
            result2.BucketingSystem = result1.BucketingSystem;
            result2.ExecutionNumber = result1.ExecutionNumber;
            result2.LayoutUid = result1.LayoutUid;
            result2.Locale = result1.Locale;
            result2.Attempt = result1.Attempt;
            result2.IsSystemIssue = result1.IsSystemIssue;
            result2.BuildType = result1.BuildType;
            if (!string.IsNullOrEmpty(result1.TestPhase))
              result2.TestPhase = this.ValidateAndGetEnumValue<TestPhase>(result1.TestPhase, TestPhase.Unspecified);
            result2.ExceptionType = result1.ExceptionType;
            result2.Dimensions = new List<TestResultDimension>();
            if (result1.Dimensions != null)
              result2.Dimensions.AddRange((IEnumerable<TestResultDimension>) result1.Dimensions);
            result2.Links = new List<Link<ResultLinkType>>();
            if (result1.Links != null)
              result2.Links.AddRange((IEnumerable<Link<ResultLinkType>>) result1.Links);
          }
          Guid runById;
          string runByName;
          this.PopulateResultRunBy(result1.RunBy, tfsIdentityCache, out runById, out runByName);
          result2.RunBy = runById;
          result2.RunByName = runByName;
          if (result1.Area != null && !string.IsNullOrEmpty(result1.Area.Name))
            result2.AreaUri = context.AreaPathsCache.GetCssNodeAndThrow(context, result1.Area.Name).Uri;
          if (!string.IsNullOrEmpty(result1.AutomatedTestTypeId))
          {
            Guid result5 = Guid.Empty;
            if (!Guid.TryParse(result1.AutomatedTestTypeId, out result5))
              throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "automatedTestTypeId"));
            if (result5 != Guid.Empty)
              result2.AutomatedTestTypeId = result1.AutomatedTestTypeId;
          }
          if (result1.SubResults != null && result1.SubResults.Any<TestSubResult>())
          {
            int maxIterations;
            result2.SubResultCount = this.CalculateSubResults(context, result1.SubResults, hierarchicalResultFFEnabled, out maxIterations);
            totalSubResults += result2.SubResultCount;
            ++hierarchicalResults;
            num = Math.Max(num, maxIterations);
            TestManagementRequestContext context1 = context;
            Guid id = projectRef.Id;
            TestCaseResult result6 = result2;
            Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webResult = result1;
            BuildConfiguration buildReference = run.BuildReference;
            int buildDefinitionId = buildReference != null ? buildReference.BuildDefinitionId : 0;
            IList<TestExtensionFieldDetails> fieldDetails = fieldDefinitions;
            this.PopulateFlakyCustomFieldIfRequired(context1, id, result6, webResult, buildDefinitionId, fieldDetails);
            this.CalculateTelemetryInfo(result1, ref rerunCount, ref ddCount, ref otCount, ref genericCount, ref passedOnRerunCount, ref rerunDD, ref rerunOT, ref rerunGeneric, ref maxAttemptId);
          }
          result2.ResultGroupType = result1.ResultGroupType;
          this.PopulateAdditionalTestFieldDetails(context, result2, result1.StackTrace, (IList<CustomTestField>) result1.CustomFields, fieldDefinitions);
          this.PopulateTestCasePropertiesIfRequired(context, run.TestPlanId, run, result1, result2, result1.AutomatedTestName, result1.AutomatedTestStorage, result1.AutomatedTestTypeId, result1.AutomatedTestType, result1.AutomatedTestId, result1.TestCaseTitle, fieldDefinitions);
          List<WorkItem> workItemList = (List<WorkItem>) null;
          if (result1.AssociatedBugs != null && result1.AssociatedBugs.Any<ShallowReference>())
          {
            int[] array = result1.AssociatedBugs.Select<ShallowReference, int>((System.Func<ShallowReference, int>) (w =>
            {
              int result7 = 0;
              if (string.IsNullOrEmpty(w.Id))
                return result7;
              int.TryParse(w.Id, out result7);
              return result7;
            })).ToArray<int>();
            workItemList = this.ValidateAndGetAssociatedWorkItems(context, projectRef.Id, array);
          }
          resultCreateModel.Add(new Tuple<TestCaseResult, List<WorkItem>>(result2, workItemList));
          resultsMap.Add(new Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>(result1, result2));
        }
        if (run.TestPlanId > 0)
          context.PlannedTestResultsHelper.UpdatePlannedResultDetails(context.RequestContext, projectRef.Name, run.TestPlanId, run.IsAutomated, resultsMap);
        this.PublishTestResultTelemetry(context, run.TestRunId, totalResults, resultsWithOwner, resultsWithValidOwnerDisplayName, resultsWithValidOwnerDirectoryAlias, totalSubResults, hierarchicalResults, rerunCount, ddCount, otCount, genericCount, num, passedOnRerunCount, rerunDD, rerunOT, rerunGeneric, maxAttemptId, ((TestRunType) run.Type).ToString(), failedResults, sizeOfComments, sizeOfErrorMessage, sizeOfStackTrace, "Insert");
        return resultCreateModel;
      }
    }

    private DateTime CheckAndRoundOffDateIfRequired(
      IVssRequestContext requestContext,
      DateTime date,
      string propertyName)
    {
      try
      {
        return TestManagementServiceUtility.CheckAndGetDate(requestContext, date, propertyName);
      }
      catch (InvalidPropertyException ex)
      {
        return DateTime.MinValue;
      }
    }

    private void ValidateInputFieldsAndPublishContext(string fields, string publishContext)
    {
      IList<string> source = (IList<string>) new List<string>()
      {
        SourceWorkflow.ContinuousIntegration,
        SourceWorkflow.ContinuousDelivery,
        SourceWorkflow.Manual
      };
      ArgumentUtility.CheckForNull<string>(fields, nameof (fields), "Test Results");
      ArgumentUtility.CheckForNull<string>(publishContext, nameof (publishContext), "Test Results");
      List<string> commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(fields);
      if (commaSeparatedString.Count > 2 || !commaSeparatedString.Any<string>((System.Func<string, bool>) (o => string.Equals(ValidTestResultGroupByFields.Container, o, StringComparison.OrdinalIgnoreCase))) && !commaSeparatedString.Any<string>((System.Func<string, bool>) (o => string.Equals(ValidTestResultGroupByFields.Owner, o, StringComparison.OrdinalIgnoreCase))))
        throw new InvalidPropertyException(nameof (fields), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.UnsupportedGroupByFieldsError, (object) ValidTestResultGroupByFields.Container, (object) ValidTestResultGroupByFields.Owner));
      if (!string.IsNullOrEmpty(publishContext) && !source.Any<string>((System.Func<string, bool>) (o => string.Equals(publishContext, o, StringComparison.OrdinalIgnoreCase))))
        throw new ArgumentException(nameof (publishContext));
    }

    private void ValidatePublishContext(string publishContext)
    {
      ArgumentUtility.CheckForNull<string>(publishContext, nameof (publishContext), "Test Results");
      IList<string> source = (IList<string>) new List<string>()
      {
        SourceWorkflow.ContinuousIntegration,
        SourceWorkflow.ContinuousDelivery,
        SourceWorkflow.Manual
      };
      if (!string.IsNullOrEmpty(publishContext) && !source.Any<string>((System.Func<string, bool>) (o => string.Equals(publishContext, o, StringComparison.OrdinalIgnoreCase))))
        throw new ArgumentException(nameof (publishContext));
    }

    private void ValidatePageSize(IVssRequestContext requestContext, int top)
    {
      ArgumentUtility.CheckGreaterThanZero((float) top, "$top", "Test Results");
      int defaultValue = requestContext.IsFeatureEnabled("TestManagement.Server.FetchAllResultsUntilMegaBatchSize") ? 20000 : 10000;
      if (top <= defaultValue)
        return;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestResultsForBuildOrReleasePageSize", defaultValue);
      if (top > num)
        throw new InvalidPropertyException("$top", ServerResources.QueryParameterOutOfRange);
    }

    private void ValidateOutcomeFilters(List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomeList)
    {
      if (outcomeList != null && (outcomeList.Count > 2 || outcomeList.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, bool>) (o => o != Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Failed && o != Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Aborted))))
        throw new InvalidPropertyException("outcomes", ServerResources.OtherOutcomesNotSupportedError);
    }

    private TestResultsGroupsForBuild PopulateTestResultsGroupsForBuild(
      int buildId,
      TestResultsGroupsData testResultsGroupsData,
      string fields)
    {
      IList<FieldDetailsForTestResults> fieldDetailsForTestResults = (IList<FieldDetailsForTestResults>) new List<FieldDetailsForTestResults>();
      TeamFoundationTestManagementResultService.GetFieldDetailsForTestResults(testResultsGroupsData, fieldDetailsForTestResults, fields);
      return new TestResultsGroupsForBuild()
      {
        BuildId = buildId,
        Fields = fieldDetailsForTestResults
      };
    }

    private TestResultsGroupsForRelease PopulateTestResultsGroupsForRelease(
      int releaseId,
      int releaseEnvId,
      TestResultsGroupsData testResultsGroupsData,
      string fields)
    {
      IList<FieldDetailsForTestResults> fieldDetailsForTestResults = (IList<FieldDetailsForTestResults>) new List<FieldDetailsForTestResults>();
      TeamFoundationTestManagementResultService.GetFieldDetailsForTestResults(testResultsGroupsData, fieldDetailsForTestResults, fields);
      return new TestResultsGroupsForRelease()
      {
        ReleaseId = releaseId,
        ReleaseEnvId = releaseEnvId,
        Fields = fieldDetailsForTestResults
      };
    }

    private IPagedList<FieldDetailsForTestResults> PopulateTestResultsGroups(
      TestResultsGroupsDataWithWaterMark testResultsGroupsData,
      string fields)
    {
      IList<FieldDetailsForTestResults> detailsForTestResultsList = (IList<FieldDetailsForTestResults>) new List<FieldDetailsForTestResults>();
      TeamFoundationTestManagementResultService.GetFieldDetailsForTestResults(testResultsGroupsData.TestResultGroups, detailsForTestResultsList, fields);
      string continuationToken = testResultsGroupsData.LastResultWaterMark == null ? (string) null : Utils.GenerateTestResultsContinuationToken(testResultsGroupsData.LastResultWaterMark.TestRunId, testResultsGroupsData.LastResultWaterMark.TestResultId);
      return (IPagedList<FieldDetailsForTestResults>) new PagedList<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) detailsForTestResultsList, continuationToken);
    }

    private IList<ShallowTestCaseResult> PopulateShallowTestResults(
      TestManagementRequestContext context,
      IList<TestCaseResult> serverTestResults,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      List<ShallowTestCaseResult> shallowTestCaseResultList = new List<ShallowTestCaseResult>();
      foreach (TestCaseResult serverTestResult in (IEnumerable<TestCaseResult>) serverTestResults)
      {
        double num = 0.0;
        if (!serverTestResult.DateStarted.Equals(new DateTime()) && !serverTestResult.DateCompleted.Equals(new DateTime()))
          num = (serverTestResult.DateCompleted - serverTestResult.DateStarted).TotalMilliseconds;
        ShallowTestCaseResult shallowTestCaseResult = new ShallowTestCaseResult()
        {
          Id = serverTestResult.TestResultId,
          RunId = serverTestResult.TestRunId,
          RefId = serverTestResult.TestCaseReferenceId,
          AutomatedTestName = serverTestResult.AutomatedTestName,
          AutomatedTestStorage = serverTestResult.AutomatedTestStorage,
          TestCaseTitle = serverTestResult.TestCaseTitle,
          Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) serverTestResult.Outcome),
          Owner = serverTestResult.OwnerName,
          Priority = (int) serverTestResult.Priority,
          IsReRun = serverTestResult.IsRerun,
          DurationInMs = num
        };
        shallowTestCaseResult.InitializeSecureObject(securedObject);
        shallowTestCaseResultList.Add(shallowTestCaseResult);
      }
      return (IList<ShallowTestCaseResult>) shallowTestCaseResultList;
    }

    private static void GetFieldDetailsForTestResults(
      TestResultsGroupsData testResultsGroupsData,
      IList<FieldDetailsForTestResults> fieldDetailsForTestResults,
      string fields)
    {
      List<string> commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(fields);
      if (testResultsGroupsData == null)
        return;
      if (commaSeparatedString.Any<string>((System.Func<string, bool>) (o => string.Equals(ValidTestResultGroupByFields.Container, o, StringComparison.OrdinalIgnoreCase))))
      {
        FieldDetailsForTestResults detailsForTestResults1 = new FieldDetailsForTestResults();
        detailsForTestResults1.FieldName = ValidTestResultGroupByFields.Container;
        List<string> automatedTestStorage = testResultsGroupsData.AutomatedTestStorage;
        detailsForTestResults1.GroupsForField = automatedTestStorage != null ? (IList<object>) automatedTestStorage.Distinct<string>().Cast<object>().ToList<object>() : (IList<object>) null;
        FieldDetailsForTestResults detailsForTestResults2 = detailsForTestResults1;
        fieldDetailsForTestResults.Add(detailsForTestResults2);
      }
      if (!commaSeparatedString.Any<string>((System.Func<string, bool>) (o => string.Equals(ValidTestResultGroupByFields.Owner, o, StringComparison.OrdinalIgnoreCase))))
        return;
      FieldDetailsForTestResults detailsForTestResults3 = new FieldDetailsForTestResults();
      detailsForTestResults3.FieldName = ValidTestResultGroupByFields.Owner;
      List<string> owner = testResultsGroupsData.Owner;
      detailsForTestResults3.GroupsForField = owner != null ? (IList<object>) owner.Distinct<string>().Cast<object>().ToList<object>() : (IList<object>) null;
      FieldDetailsForTestResults detailsForTestResults4 = detailsForTestResults3;
      fieldDetailsForTestResults.Add(detailsForTestResults4);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetTfsIdentity(
      string name,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache)
    {
      if (tfsIdentityCache.ContainsKey(name))
        return tfsIdentityCache[name];
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in tfsIdentityCache.Values)
      {
        object obj1;
        object obj2;
        if (string.Equals(identity.DisplayName, name, StringComparison.OrdinalIgnoreCase) || string.Equals(Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetUniqueName(identity), name, StringComparison.OrdinalIgnoreCase) || identity.TryGetProperty("Account", out obj1) && string.Equals(obj1.ToString(), name, StringComparison.OrdinalIgnoreCase) || identity.TryGetProperty("DirectoryAlias", out obj2) && string.Equals(obj2.ToString(), name, StringComparison.OrdinalIgnoreCase))
          return identity;
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private IList<TestResultArtifacts> MapResultsToIterationDetails(
      List<TestCaseResult> testCaseResults,
      List<TestActionResult> actionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments)
    {
      Dictionary<TestCaseResultIdentifier, TestResultArtifacts> dictionary = new Dictionary<TestCaseResultIdentifier, TestResultArtifacts>();
      foreach (TestCaseResult testCaseResult in testCaseResults)
        dictionary[testCaseResult.Id] = new TestResultArtifacts(testCaseResult, new List<TestActionResult>(), new List<TestResultParameter>(), new List<TestResultAttachment>());
      foreach (TestActionResult actionResult in actionResults)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier()
        {
          TestResultId = actionResult.TestResultId,
          TestRunId = actionResult.TestRunId
        };
        if (dictionary.ContainsKey(key))
          dictionary[key].ActionResults.Add(actionResult);
      }
      foreach (TestResultParameter resultParameter in resultParameters)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier()
        {
          TestResultId = resultParameter.TestResultId,
          TestRunId = resultParameter.TestRunId
        };
        if (dictionary.ContainsKey(key))
          dictionary[key].ParameterResults.Add(resultParameter);
      }
      foreach (TestResultAttachment resultAttachment in resultAttachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier()
        {
          TestResultId = resultAttachment.TestResultId,
          TestRunId = resultAttachment.TestRunId
        };
        if (dictionary.ContainsKey(key))
          dictionary[key].AttachmentResults.Add(resultAttachment);
      }
      return (IList<TestResultArtifacts>) dictionary.Values.ToList<TestResultArtifacts>();
    }

    private List<TestSuite> GetRootSuitesFromAggregatedResults(
      IList<TestResultsDetailsForGroup> aggregatedResultDetails)
    {
      List<TestSuite> aggregatedResults = new List<TestSuite>();
      foreach (TestResultsDetailsForGroup aggregatedResultDetail in (IEnumerable<TestResultsDetailsForGroup>) aggregatedResultDetails)
      {
        if (aggregatedResultDetail.GroupByValue is TestSuite groupByValue && groupByValue.Id > 0 && groupByValue.Parent != null && int.Parse(groupByValue.Parent.Id) == 0)
          aggregatedResults.Add(groupByValue);
      }
      return aggregatedResults;
    }

    private void UpdateAggregatedResultsWithSuiteName(
      List<TestSuite> rootSuites,
      Dictionary<int, string> testPlanTitles)
    {
      foreach (TestSuite rootSuite in rootSuites)
      {
        if (rootSuite != null && rootSuite.Id > 0 && rootSuite.Parent != null && int.Parse(rootSuite.Parent.Id) == 0)
          rootSuite.Name = testPlanTitles[int.Parse(rootSuite.Plan.Id)];
      }
    }

    private List<int> GetTestPlanIdsForRootSuites(List<TestSuite> rootSuites)
    {
      HashSet<int> source = new HashSet<int>();
      foreach (TestSuite rootSuite in rootSuites)
      {
        if (rootSuite != null && rootSuite.Id > 0 && rootSuite.Parent != null && int.Parse(rootSuite.Parent.Id) == 0)
          source.Add(int.Parse(rootSuite.Plan.Id));
      }
      return source.ToList<int>();
    }

    private bool IsGroupBySuite(List<string> groupByFields)
    {
      if (groupByFields != null && groupByFields.Any<string>())
      {
        foreach (string groupByField in groupByFields)
        {
          if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private bool IsGroupByRequirement(List<string> groupByFields)
    {
      if (groupByFields != null && groupByFields.Any<string>())
      {
        foreach (string groupByField in groupByFields)
        {
          if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private bool IsGroupByTestRun(List<string> groupByFields)
    {
      if (groupByFields != null && groupByFields.Any<string>())
      {
        foreach (string groupByField in groupByFields)
        {
          if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private ShallowReference GetAreaPathShallowReferenceFromUri(
      TestManagementRequestContext context,
      string areaUri)
    {
      ShallowReference referenceFromUri = (ShallowReference) null;
      if (!string.IsNullOrEmpty(areaUri))
      {
        IdAndString idAndPath = context.AreaPathsCache.GetIdAndPath(context, areaUri);
        string workItemPathFromUri = context.CSSHelper.GetWorkItemPathFromUri(areaUri);
        referenceFromUri = new ShallowReference(idAndPath.Id.ToString(), !string.IsNullOrEmpty(workItemPathFromUri) ? workItemPathFromUri : (string) null, areaUri);
      }
      return referenceFromUri;
    }

    private void ValidatePublishResultsPageSizeLimit(
      int publishResultsCount,
      bool isPlannedRun = false,
      bool isImpersonating = false)
    {
      if (!(isPlannedRun & isImpersonating) && publishResultsCount > 1000)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxTestResultsLimitCrossedError, (object) 1000));
    }

    private void ValidatePublishHierarchicalResultsPageSizeLimit(int hierarchicalResultCount)
    {
      if (hierarchicalResultCount > 100)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxHierarchicalResultsLimitCrossedError, (object) 100));
    }

    private Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetTfsIdentityCache(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      if (results == null || !((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results).Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
        return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      HashSet<string> source1 = new HashSet<string>();
      HashSet<string> source2 = new HashSet<string>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        if (result.RunBy != null && !string.IsNullOrEmpty(result.RunBy.DisplayName))
          source1.Add(result.RunBy.DisplayName);
        if (result.Owner != null && !string.IsNullOrEmpty(result.Owner.DirectoryAlias))
          source2.Add(result.Owner.DirectoryAlias);
        else if (result.Owner != null && !string.IsNullOrEmpty(result.Owner.DisplayName))
          source1.Add(result.Owner.DisplayName);
      }
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache = this.GetTfsIdentitiesByNames(context, source1.ToList<string>());
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = tfsIdentityCache.Select<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (elem => elem.Value));
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> aliasLookUp = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        string property = identity.GetProperty<string>("DirectoryAlias", string.Empty);
        if (!string.Equals(property, string.Empty))
          aliasLookUp.TryAdd<string, Microsoft.VisualStudio.Services.Identity.Identity>(property, identity);
      }
      List<string> list = source2.Where<string>((System.Func<string, bool>) (alias =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity valueOrDefault = aliasLookUp.GetValueOrDefault<string, Microsoft.VisualStudio.Services.Identity.Identity>(alias, (Microsoft.VisualStudio.Services.Identity.Identity) null);
        if (valueOrDefault == null)
          return true;
        tfsIdentityCache.TryAdd<string, Microsoft.VisualStudio.Services.Identity.Identity>(alias, valueOrDefault);
        return false;
      })).ToList<string>();
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCacheByAlias = this.GetTfsIdentitiesByDirectoryAlias(context, list);
      if (!context.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted && list.Count != tfsIdentityCacheByAlias.Count)
      {
        IEnumerable<string> source3 = list.Where<string>((System.Func<string, bool>) (t => !tfsIdentityCacheByAlias.ContainsKey(t)));
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identitiesByName in this.GetTfsIdentitiesByNames(context, source3.ToList<string>()))
          tfsIdentityCache.TryAdd<string, Microsoft.VisualStudio.Services.Identity.Identity>(identitiesByName.Key, identitiesByName.Value);
      }
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> keyValuePair in tfsIdentityCacheByAlias)
        tfsIdentityCache.TryAdd<string, Microsoft.VisualStudio.Services.Identity.Identity>(keyValuePair.Key, keyValuePair.Value);
      return tfsIdentityCache;
    }

    private List<Guid> GetUniqueIdentitiyGuidsFromTestCaseResults(
      List<TestCaseResult> testCaseResults)
    {
      if (testCaseResults == null || !testCaseResults.Any<TestCaseResult>())
        return new List<Guid>();
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TestCaseResult testCaseResult in testCaseResults)
      {
        if (testCaseResult.RunBy != Guid.Empty)
          source.Add(testCaseResult.RunBy);
        if (testCaseResult.Owner != Guid.Empty)
          source.Add(testCaseResult.Owner);
        if (testCaseResult.LastUpdatedBy != Guid.Empty)
          source.Add(testCaseResult.LastUpdatedBy);
      }
      return source.ToList<Guid>();
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] UpdateTestResultsInternal(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] resultModels,
      out TeamProjectTestArtifacts testArtifacts,
      bool autoComputeTestRunState = true,
      bool updateIterationDetails = false)
    {
      TeamProjectTestArtifacts teamProjectTestArtifacts = (TeamProjectTestArtifacts) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResultArray = this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>(context.RequestContext, "TeamFoundationTestManagementResultService.UpdateTestResults", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[]>) (() =>
      {
        teamProjectTestArtifacts = this.GetTeamProjectTestArtifacts(context, projectReference, false, true);
        List<Tuple<ResultUpdateRequest, List<WorkItem>>> resultUpdateModel = this.GetResultUpdateRequestFromResultUpdateModel(context, projectReference, testRun, resultModels, teamProjectTestArtifacts, this.GetTfsIdentityCache(context, resultModels), updateIterationDetails);
        if (resultUpdateModel == null || !resultUpdateModel.Any<Tuple<ResultUpdateRequest, List<WorkItem>>>())
          return Array.Empty<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        Dictionary<int, ResultUpdateResponse> dictionary = ((IEnumerable<ResultUpdateResponse>) TestCaseResult.Update(context, resultUpdateModel.Select<Tuple<ResultUpdateRequest, List<WorkItem>>, ResultUpdateRequest>((System.Func<Tuple<ResultUpdateRequest, List<WorkItem>>, ResultUpdateRequest>) (t => t.Item1)).ToArray<ResultUpdateRequest>(), projectReference.Name, autoComputeTestRunState)).ToDictionary<ResultUpdateResponse, int, ResultUpdateResponse>((System.Func<ResultUpdateResponse, int>) (x => x.TestResultId), (System.Func<ResultUpdateResponse, ResultUpdateResponse>) (x => x));
        bool hierarchicalResultFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResult");
        bool flag1 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResultBatchUploadAttachment");
        IVssRegistryService service = context.RequestContext.GetService<IVssRegistryService>();
        IVssRequestContext requestContext1 = context.RequestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestResultMaxSubResultIterationCount";
        ref RegistryQuery local1 = ref registryQuery;
        int maxSubResultIterationCount = service.GetValue<int>(requestContext1, in local1, 1000);
        IVssRequestContext requestContext2 = context.RequestContext;
        registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestResultMaxSubResultHierarchyLevel";
        ref RegistryQuery local2 = ref registryQuery;
        int maxSubResultHierarchyLevel = service.GetValue<int>(requestContext2, in local2, 3);
        bool flag2 = false;
        for (int index = 0; index < resultUpdateModel.Count<Tuple<ResultUpdateRequest, List<WorkItem>>>(); ++index)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult resultModel = resultModels[index];
          resultModels[index].Revision = dictionary[resultModel.Id].Revision;
          resultModels[index].LastUpdatedDate = dictionary[resultModel.Id].LastUpdated;
          TestCaseResult testCaseResult = resultUpdateModel[index].Item1.TestCaseResult;
          List<WorkItem> workItemList = resultUpdateModel[index].Item2;
          if (workItemList != null && workItemList.Any<WorkItem>())
            this.CreateAssociatedWorkItemsForTestResult(context, projectReference.Name, testCaseResult, workItemList);
          int reservedSubResultId = dictionary[resultModel.Id].MaxReservedSubResultId;
          if (resultModel.SubResults != null && resultModel.SubResults.Any<TestSubResult>())
          {
            flag2 = true;
            if (reservedSubResultId - testCaseResult.SubResultCount > 0)
              this.PopulateSubResultsProperties(resultModel.SubResults, maxSubResultIterationCount, maxSubResultHierarchyLevel, out int _, reservedSubResultId - resultModel.SubResults.Count + 1);
            if (!flag1)
              this.PublishSubResults(context, projectReference, testRun.TestRunId, resultModel.Id, resultModel.SubResults, hierarchicalResultFFEnabled);
          }
        }
        if (flag2 & flag1)
        {
          try
          {
            context.RequestContext.RequestTimer.PauseTimeToFirstPageTimer();
            this.PublishSubResultsV2(context, projectReference, testRun.TestRunId, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) resultModels).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), hierarchicalResultFFEnabled);
          }
          finally
          {
            context.RequestContext.RequestTimer.ResumeTimeToFirstPageTimer();
          }
        }
        return resultModels;
      }), 1015095, "TestResultsInsights");
      testArtifacts = teamProjectTestArtifacts;
      return testCaseResultArray;
    }

    private List<int> GetWorkItemIdsFromArtiFacts(Artifact[] artifacts)
    {
      List<int> idsFromArtiFacts = new List<int>();
      foreach (Artifact artifact in artifacts)
      {
        int result;
        if (int.TryParse(LinkingUtilities.DecodeUri(artifact.Uri).ToolSpecificId, out result))
          idsFromArtiFacts.Add(result);
      }
      return idsFromArtiFacts;
    }

    private LinkFilter GetLinkFilterValue() => new LinkFilter()
    {
      FilterType = FilterType.ToolType,
      FilterValues = new string[1]{ "WorkItemTracking" }
    };

    private string[] GetWorkItemUris(
      TestManagementRequestContext context,
      string teamProjectName,
      List<WorkItem> workItems)
    {
      if (workItems.Count<WorkItem>() <= 0)
        return Array.Empty<string>();
      List<string> stringList1 = new List<string>(workItems.Count<WorkItem>());
      foreach (WorkItem workItem in workItems)
      {
        if (workItem != null)
        {
          int? id = workItem.Id;
          if (id.HasValue)
          {
            List<string> stringList2 = stringList1;
            id = workItem.Id;
            string artiFactUri = TestManagementServiceUtility.GetArtiFactUri("WorkItem", "WorkItemTracking", id.ToString());
            stringList2.Add(artiFactUri);
          }
        }
      }
      return stringList1.ToArray();
    }

    private void CreateAssociatedWorkItemsForTestResult(
      TestManagementRequestContext context,
      string projectName,
      TestCaseResult testResult,
      List<WorkItem> workItems)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CreateAssociatedWorkItemsForTestResult), "WorkItem")))
        this.ExecuteAction(context.RequestContext, "TeamFoundationTestManagementResultService.CreateAssociatedWorkItemsForTestResult", (Action) (() =>
        {
          List<WorkItem> associatedWithTestResult = this.GetFilteredListOfWorkItemsToBeAssociatedWithTestResult(context, workItems, projectName, testResult);
          if (associatedWithTestResult == null || !associatedWithTestResult.Any<WorkItem>())
            return;
          IWorkItemServiceHelper itemServiceHelper = context.WorkItemServiceHelper;
          string uriForTestResult = TestManagementServiceUtility.GetArtiFactUriForTestResult(testResult);
          Dictionary<string, object> attributes = new Dictionary<string, object>();
          attributes.Add(WorkItemLinkAttributes.Name, (object) WorkItemLinkedArtifactName.TestResult);
          List<WorkItem> workItems1 = associatedWithTestResult;
          itemServiceHelper.LinkArtifactToWorkItems(uriForTestResult, attributes, (IList<WorkItem>) workItems1);
          TestCaseResultIdentifier[] identifiers = new TestCaseResultIdentifier[associatedWithTestResult.Count];
          TestCaseResultIdentifier resultIdentifier = new TestCaseResultIdentifier(testResult.TestRunId, testResult.TestResultId);
          for (int index = 0; index < associatedWithTestResult.Count; ++index)
            identifiers[index] = resultIdentifier;
          string[] workItemUris = this.GetWorkItemUris(context, projectName, associatedWithTestResult);
          TestCaseResult.CreateAssociatedWorkItems(context, identifiers, workItemUris, projectName);
        }), 1015095, "TestResultsInsights");
    }

    private List<TestCaseResultIdentifier> GetTestCaseResultIdentifiersInTestRun(
      TestManagementRequestContext tcmRequestContext,
      TeamProjectReference teamProjectRef,
      int runId)
    {
      return this.ExecuteAction<List<TestCaseResultIdentifier>>(tcmRequestContext.RequestContext, "TeamFoundationTestManagementResultService.GetTestCaseResultIdentifiersInTestRun", (Func<List<TestCaseResultIdentifier>>) (() =>
      {
        tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TeamFoundationTestManagementResultService.GetTestResults projectId = {0} runId = {1}", (object) teamProjectRef.Id, (object) runId);
        return TestCaseResult.GetTestCaseResultIdsInTestRun(tcmRequestContext, runId, teamProjectRef.Name);
      }), 1015095, "TestResultsInsights");
    }

    private TestConfiguration ValidateAndGetTestConfiguration(
      ShallowReference testConfigurationRef,
      int testPointId,
      List<TestConfiguration> projectTestConfigs)
    {
      TestConfiguration testConfiguration = (TestConfiguration) null;
      if (testConfigurationRef != null && (!string.IsNullOrEmpty(testConfigurationRef.Name) || !string.IsNullOrEmpty(testConfigurationRef.Id)))
      {
        if (testPointId > int.MinValue)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationSpecifiedError));
        if (projectTestConfigs == null)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "configuration"));
        if (!string.IsNullOrEmpty(testConfigurationRef.Name))
        {
          testConfiguration = projectTestConfigs.Find((Predicate<TestConfiguration>) (x => string.Compare(x.Name, testConfigurationRef.Name, true) == 0));
        }
        else
        {
          int configId = 0;
          if (int.TryParse(testConfigurationRef.Id, out configId) && configId > 0)
            testConfiguration = projectTestConfigs.Find((Predicate<TestConfiguration>) (x => object.Equals((object) x.Id, (object) configId)));
        }
        if (testConfiguration == null || testConfiguration.Id < 1)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationCannotBeFoundError, string.IsNullOrEmpty(testConfigurationRef.Name) ? (object) testConfigurationRef.Id : (object) testConfigurationRef.Name), ObjectTypes.TestConfiguration);
      }
      return testConfiguration;
    }

    private void PopulateTestCasePropertiesIfRequired(
      TestManagementRequestContext context,
      int testPlanId,
      TestRun run,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webResult,
      TestCaseResult result,
      string automatedTestName,
      string automatedTestStorage,
      string automatedTestTypeId,
      string automatedTestType,
      string automatedTestId,
      string testCaseTitle,
      IList<TestExtensionFieldDetails> testExtensionFieldDetails)
    {
      int result1 = int.MinValue;
      int result2 = 0;
      if (testPlanId > 0)
      {
        if (webResult.TestPoint != null)
          int.TryParse(webResult.TestPoint.Id, out result1);
        if (webResult.TestCase != null)
          int.TryParse(webResult.TestCase.Id, out result2);
        if (result1 <= int.MinValue && result2 <= 0)
          return;
        if (!context.IsTcmService)
        {
          if (!string.IsNullOrEmpty(automatedTestName))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "AutomatedTestName"));
          if (!string.IsNullOrEmpty(automatedTestStorage))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "AutomatedTestStorage"));
          if (!string.IsNullOrEmpty(automatedTestTypeId))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "AutomatedTestTypeId"));
          if (!string.IsNullOrEmpty(automatedTestType))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "AutomatedTestType"));
          if (!string.IsNullOrEmpty(automatedTestId))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "AutomatedTestId"));
          if (!string.IsNullOrEmpty(testCaseTitle))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCasePropertiesSpecifiedError, (object) "TestCaseTitle"));
        }
        else
        {
          if (result1 == int.MinValue || result2 <= 0 || string.IsNullOrEmpty(testCaseTitle) || webResult.TestCaseRevision <= 0)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PlannedTestResultPropertiesNotSpecifiedError));
          result.TestPointId = result1;
          result.TestCaseId = result2;
          result.AutomatedTestName = automatedTestName;
          result.AutomatedTestStorage = automatedTestStorage;
          result.AutomatedTestType = automatedTestType;
          result.AutomatedTestId = automatedTestId;
          result.AutomatedTestTypeId = automatedTestTypeId;
          result.TestCaseTitle = testCaseTitle;
          result.TestCaseRevision = webResult.TestCaseRevision;
        }
      }
      else
      {
        if (string.IsNullOrEmpty(automatedTestName))
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomatedTestNameNotSpecifiedError));
        if (!run.IsAutomated)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomatedTestParameterSpecifiedForManualRunError, (object) nameof (automatedTestName)));
        result.AutomatedTestName = automatedTestName;
        if (!string.IsNullOrEmpty(automatedTestStorage))
        {
          if (!run.IsAutomated)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomatedTestParameterSpecifiedForManualRunError, (object) nameof (automatedTestStorage)));
          result.AutomatedTestStorage = automatedTestStorage;
        }
        if (!string.IsNullOrEmpty(automatedTestType))
        {
          if (!run.IsAutomated)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomatedTestParameterSpecifiedForManualRunError, (object) nameof (automatedTestType)));
          result.AutomatedTestType = automatedTestType;
        }
        if (!string.IsNullOrEmpty(testCaseTitle))
          result.TestCaseTitle = testCaseTitle;
        if (string.IsNullOrEmpty(result.TestCaseTitle))
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseTitleNotSpecifiedError));
        if (webResult.TestCaseRevision > 0)
          result.TestCaseRevision = webResult.TestCaseRevision;
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestCaseResultProps"))
          TestCasePropertiesSanitizerFactory.Create(context.RequestContext).Sanitize(context, result);
        if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodName"))
          return;
        string automatedTestName1 = result.AutomatedTestName;
        string testCaseTitle1 = result.TestCaseTitle;
        this.m_testMethodNameSanitizer.Sanitize(context, result);
        if (context.IsFeatureEnabled("TestManagement.Server.DisableSanitizationCustomFields"))
          return;
        if (result.CustomFields == null)
          result.CustomFields = new List<TestExtensionField>();
        int num = (object) result.AutomatedTestName != (object) automatedTestName1 ? 1 : 0;
        bool flag = (object) result.TestCaseTitle != (object) testCaseTitle1;
        if (num != 0 && !context.IsFeatureEnabled("TestManagement.Server.DisableAutomatedTestNameSanitizationCustomField"))
        {
          TestExtensionFieldDetails extensionFieldDetails = testExtensionFieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "UnsanitizedAutomatedTestName", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
          TestExtensionField extensionField = this.GetExtensionField("UnsanitizedAutomatedTestName", extensionFieldDetails != null ? extensionFieldDetails.Id : 0);
          extensionField.Value = (object) automatedTestName1;
          result.CustomFields.Add(extensionField);
        }
        if (num == 0 && !(context.IsFeatureEnabled("TestManagement.Server.HonourTestCaseTitleSanitization") & flag))
          return;
        TestExtensionFieldDetails extensionFieldDetails1 = testExtensionFieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "UnsanitizedTestCaseTitle", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        TestExtensionField extensionField1 = this.GetExtensionField("UnsanitizedTestCaseTitle", extensionFieldDetails1 != null ? extensionFieldDetails1.Id : 0);
        extensionField1.Value = (object) testCaseTitle1;
        result.CustomFields.Add(extensionField1);
      }
    }

    private TestExtensionField GetExtensionField(string fieldName, int fieldId)
    {
      TestExtensionField extensionField = new TestExtensionField()
      {
        Field = new TestExtensionFieldDetails()
      };
      extensionField.Field.Type = SqlDbType.NVarChar;
      extensionField.Field.IsResultScoped = true;
      extensionField.Field.IsSystemField = true;
      extensionField.Field.Id = fieldId;
      extensionField.Field.Name = fieldName;
      return extensionField;
    }

    private int GetResolutionStateIdFromResolutionStateName(
      string strResolutionState,
      List<TestResolutionState> resolutionStates)
    {
      if (string.IsNullOrEmpty(strResolutionState))
        return resolutionStates.Min<TestResolutionState>((System.Func<TestResolutionState, int>) (x => x.Id));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resolutionStates, nameof (resolutionStates), "Test Results");
      TestResolutionState testResolutionState = resolutionStates.Find((Predicate<TestResolutionState>) (x => string.Compare(x.Name, strResolutionState, true) == 0));
      return testResolutionState != null && testResolutionState.Id > 0 ? testResolutionState.Id : throw new InvalidPropertyException("resolutionState", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidValueSpecified));
    }

    private IList<TestExtensionFieldDetails> GetResultExtensionFieldDefinitions(
      TestManagementRequestContext context,
      TeamProjectReference projectRef)
    {
      return this.ExecuteAction<IList<TestExtensionFieldDetails>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetResultExtensionFieldDefinitions", (Func<IList<TestExtensionFieldDetails>>) (() =>
      {
        ITeamFoundationTestExtensionFieldsService service = context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>();
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
        TestManagementRequestContext context1 = context;
        Guid guidId = projectFromName.GuidId;
        return service.QueryFields(context1, guidId, scopeFilter: CustomTestFieldScope.TestResult | CustomTestFieldScope.System);
      }), 1015095, "TestResultsInsights");
    }

    private void PopulateAdditionalTestFieldDetails(
      TestManagementRequestContext context,
      TestCaseResult result,
      string stackTrace,
      IList<CustomTestField> additionalFields,
      IList<TestExtensionFieldDetails> fieldDetails)
    {
      List<TestExtensionField> matchingExtensionField = TeamFoundationTestManagementResultService.ConvertCustomTestFieldToMatchingExtensionField(context.RequestContext, additionalFields, fieldDetails, true);
      bool flag = context.IsFeatureEnabled("TestManagement.Server.SkipCommentForNonFailedResults") && result.Outcome != (byte) 3 && result.Outcome != (byte) 6;
      if (!string.IsNullOrWhiteSpace(stackTrace))
      {
        TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "StackTrace", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
        {
          TestExtensionField testExtensionField = matchingExtensionField.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "StackTrace", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>();
          if (testExtensionField == null)
            matchingExtensionField.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) stackTrace
            });
          else
            testExtensionField.Value = (object) stackTrace;
          result.StackTrace = (TestExtensionField) null;
        }
      }
      if (!string.IsNullOrWhiteSpace(result.Comment) && !flag)
      {
        TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "Comment", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
        {
          int forResultComment = TestManagementServiceUtility.GetMaxLengthForResultComment(context.RequestContext);
          string str = result.Comment.Substring(0, result.Comment.Length > forResultComment ? forResultComment : result.Comment.Length);
          TestExtensionField testExtensionField = matchingExtensionField.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "Comment", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>();
          if (testExtensionField == null)
            matchingExtensionField.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) str
            });
          else
            testExtensionField.Value = (object) str;
        }
      }
      if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
      {
        TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
        {
          TestExtensionField testExtensionField = matchingExtensionField.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>();
          if (testExtensionField == null)
            matchingExtensionField.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) result.ErrorMessage
            });
          else
            testExtensionField.Value = (object) result.ErrorMessage;
        }
      }
      if (result.SubResultCount != 0)
      {
        TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "MaxReservedSubResultId", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
          matchingExtensionField.Add(new TestExtensionField()
          {
            Field = extensionFieldDetails,
            Value = (object) result.SubResultCount
          });
      }
      if (result.ResultGroupType != ResultGroupType.None)
      {
        TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "TestResultGroupType", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
        {
          TestExtensionField testExtensionField = matchingExtensionField.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "TestResultGroupType", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>();
          if (testExtensionField == null)
            matchingExtensionField.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) result.ResultGroupType
            });
          else
            testExtensionField.Value = (object) result.ResultGroupType;
        }
      }
      if (!matchingExtensionField.Any<TestExtensionField>())
        return;
      if (result.CustomFields != null)
        result.CustomFields.AddRange((IEnumerable<TestExtensionField>) matchingExtensionField);
      else
        result.CustomFields = matchingExtensionField;
    }

    private static List<TestExtensionField> ConvertCustomTestFieldToMatchingExtensionField(
      IVssRequestContext tfsRequestContext,
      IList<CustomTestField> additionalFields,
      IList<TestExtensionFieldDetails> fieldDetails,
      bool throwOnMissingFields)
    {
      List<TestExtensionField> matchingExtensionField = new List<TestExtensionField>();
      if (additionalFields != null && additionalFields.Any<CustomTestField>())
      {
        List<string> stringList = new List<string>();
        foreach (CustomTestField additionalField in (IEnumerable<CustomTestField>) additionalFields)
        {
          CustomTestField field = additionalField;
          if (!TeamFoundationTestManagementResultService.ShouldSkipCustomField(tfsRequestContext, field.FieldName) && (!string.Equals(field.FieldName, "AttemptId", StringComparison.OrdinalIgnoreCase) || Convert.ToInt32(field.Value) != 0) && !string.Equals(field.FieldName, "MaxReservedSubResultId", StringComparison.OrdinalIgnoreCase))
          {
            bool result;
            if (!tfsRequestContext.IsFeatureEnabled("TestManagement.Server.EnablePassedFlakyPropagation") && string.Equals(field.FieldName, "IsTestResultFlaky", StringComparison.OrdinalIgnoreCase) && field.Value != null && bool.TryParse(field.Value.ToString(), out result))
            {
              field.FieldName = "OutcomeConfidence";
              field.Value = (object) (result ? 0.0 : 1.0);
            }
            TestExtensionFieldDetails extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, field.FieldName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
            if (extensionFieldDetails != null)
              matchingExtensionField.Add(new TestExtensionField()
              {
                Field = new TestExtensionFieldDetails()
                {
                  Id = extensionFieldDetails.Id,
                  Name = extensionFieldDetails.Name,
                  Type = extensionFieldDetails.Type
                },
                Value = field.Value
              });
            else
              stringList.Add(field.FieldName);
          }
        }
        if (throwOnMissingFields && stringList.Any<string>())
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.ExtensionFieldsNotFoundError, (object) string.Join(",", (IEnumerable<string>) stringList)));
      }
      return matchingExtensionField;
    }

    private static bool ShouldSkipCustomField(
      IVssRequestContext tfsRequestContext,
      string fieldName)
    {
      return string.Equals(fieldName, "OutcomeConfidence", StringComparison.OrdinalIgnoreCase) && !tfsRequestContext.IsFeatureEnabled("TestManagement.Server.TRIFlakiness") || string.Equals(fieldName, "UnsanitizedAutomatedTestName", StringComparison.OrdinalIgnoreCase) || string.Equals(fieldName, "UnsanitizedTestCaseTitle", StringComparison.OrdinalIgnoreCase);
    }

    private List<WorkItem> ValidateAndGetAssociatedWorkItems(
      TestManagementRequestContext context,
      Guid projectId,
      int[] associatedWorkItems)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (ValidateAndGetAssociatedWorkItems), "WorkItem")))
        return this.ExecuteAction<List<WorkItem>>(context.RequestContext, "TeamFoundationTestManagementResultService.ValidateAndGetAssociatedWorkItems", (Func<List<WorkItem>>) (() =>
        {
          IList<WorkItem> workItems = context.WorkItemServiceHelper.GetWorkItems(projectId, (IList<int>) associatedWorkItems, (IList<string>) null, WorkItemExpand.None, WorkItemErrorPolicy.Omit);
          if (workItems.Count<WorkItem>() != ((IEnumerable<int>) associatedWorkItems).Count<int>())
            context.RequestContext.Trace(1015095, TraceLevel.Warning, "TestResultsInsights", "RestLayer", ServerResources.SomeTestResultLinkedWorkItemsCannotBeObtained);
          return workItems.ToList<WorkItem>();
        }), 1015095, "TestResultsInsights");
    }

    private List<WorkItem> GetFilteredListOfWorkItemsToBeAssociatedWithTestResult(
      TestManagementRequestContext context,
      List<WorkItem> wiToBeAssociated,
      string projectName,
      TestCaseResult result)
    {
      return this.ExecuteAction<List<WorkItem>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetFilteredListOfWorkItemsToBeAssociatedWithTestResult", (Func<List<WorkItem>>) (() =>
      {
        IEnumerable<WorkItem> source = (IEnumerable<WorkItem>) wiToBeAssociated;
        if (wiToBeAssociated != null && wiToBeAssociated.Any<WorkItem>() && result != null)
        {
          List<int> existingWiIds = context.RequestContext.GetService<ITestManagementLinkedWorkItemService>().GetWorkItemIdsAssociatedToTestResults(new TestCaseResult[1]
          {
            result
          }, context, projectName).Values.SelectMany<List<int>, int>((System.Func<List<int>, IEnumerable<int>>) (wiIds => (IEnumerable<int>) wiIds ?? (IEnumerable<int>) new List<int>())).ToList<int>();
          if (existingWiIds != null && existingWiIds.Any<int>())
            source = wiToBeAssociated.Where<WorkItem>((System.Func<WorkItem, bool>) (x => x.Id.HasValue && !existingWiIds.Contains(x.Id.Value)));
        }
        return source == null || !source.Any<WorkItem>() ? (List<WorkItem>) null : source.ToList<WorkItem>();
      }), 1015095, "TestResultsInsights");
    }

    private int GetTestResultIdFromShallowReference(ShallowReference testResultRef)
    {
      if (testResultRef == null || string.IsNullOrEmpty(testResultRef.Id))
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.IdMustBeSpecifiedError, (object) "TestResult Id"));
      int result = 0;
      if (!int.TryParse(testResultRef.Id, out result) || result < 1)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultIdCannotBeObtainedError));
      return result;
    }

    private List<Tuple<ResultUpdateRequest, List<WorkItem>>> GetResultUpdateRequestFromResultUpdateModel(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestRun run,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] resultModels,
      TeamProjectTestArtifacts teamProjectTestArtifacts,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache,
      bool updateIterationDetails)
    {
      return this.ExecuteAction<List<Tuple<ResultUpdateRequest, List<WorkItem>>>>(context.RequestContext, "TeamFoundationTestManagementResultService.GetResultUpdateRequestFromResultUpdateModel", (Func<List<Tuple<ResultUpdateRequest, List<WorkItem>>>>) (() =>
      {
        if (resultModels == null || resultModels.Length == 0)
          return new List<Tuple<ResultUpdateRequest, List<WorkItem>>>();
        IList<TestExtensionFieldDetails> fieldDefinitions = this.GetResultExtensionFieldDefinitions(context, projectRef);
        List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
        List<TestActionResult> testActionResultList = new List<TestActionResult>();
        List<TestResultParameter> testResultParameterList = new List<TestResultParameter>();
        List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
        List<TestCaseResultIdentifier> identifiersInTestRun = this.GetTestCaseResultIdentifiersInTestRun(context, projectRef, run.TestRunId);
        HashSet<TestCaseResultIdentifier> existingResults = identifiersInTestRun != null && identifiersInTestRun.Count >= 1 ? new HashSet<TestCaseResultIdentifier>((IEnumerable<TestCaseResultIdentifier>) identifiersInTestRun) : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestRunNoResultsError));
        return this.BuildResultUpdateRequests(context, projectRef.Id, resultModels, run, teamProjectTestArtifacts, tfsIdentityCache, fieldDefinitions, existingResults, updateIterationDetails);
      }), 1015095, "TestResultsInsights");
    }

    private List<Tuple<ResultUpdateRequest, List<WorkItem>>> BuildResultUpdateRequests(
      TestManagementRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] resultModels,
      TestRun run,
      TeamProjectTestArtifacts teamProjectTestArtifacts,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache,
      IList<TestExtensionFieldDetails> resultExFieldDetails,
      HashSet<TestCaseResultIdentifier> existingResults,
      bool updateIterationDetails)
    {
      bool hierarchicalResultFFEnabled = context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResult");
      List<Tuple<ResultUpdateRequest, List<WorkItem>>> tupleList = new List<Tuple<ResultUpdateRequest, List<WorkItem>>>();
      int totalResults = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) resultModels).Count<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      int resultsWithOwner = 0;
      int resultsWithValidOwnerDisplayName = 0;
      int resultsWithValidOwnerDirectoryAlias = 0;
      int totalSubResults = 0;
      int hierarchicalResults = 0;
      int rerunCount = 0;
      int ddCount = 0;
      int otCount = 0;
      int genericCount = 0;
      int num = 0;
      int passedOnRerunCount = 0;
      int rerunDD = 0;
      int rerunOT = 0;
      int rerunGeneric = 0;
      int maxAttemptId = 0;
      int failedResults = 0;
      double sizeOfComments = 0.0;
      double sizeOfErrorMessage = 0.0;
      double sizeOfStackTrace = 0.0;
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult resultModel in resultModels)
      {
        int id = resultModel.Id;
        ArgumentUtility.CheckGreaterThanZero((float) id, "testResultId", "Test Results");
        if (!existingResults.Contains(new TestCaseResultIdentifier(run.TestRunId, id)))
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) run.TestRunId, (object) id), ObjectTypes.TestResult);
        ResultUpdateRequest resultUpdateRequest = new ResultUpdateRequest();
        resultUpdateRequest.TestRunId = run.TestRunId;
        resultUpdateRequest.TestResultId = id;
        resultUpdateRequest.TestCaseResult = new TestCaseResult();
        resultUpdateRequest.TestCaseResult.TestRunId = run.TestRunId;
        resultUpdateRequest.TestCaseResult.TestResultId = id;
        if (!string.IsNullOrEmpty(resultModel.AutomatedTestTypeId))
          resultUpdateRequest.TestCaseResult.AutomatedTestTypeId = resultModel.AutomatedTestTypeId;
        byte result1;
        if (!byte.TryParse(resultModel.Priority.ToString(), out result1))
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "Priority"));
        resultUpdateRequest.TestCaseResult.Priority = result1;
        if (!string.IsNullOrEmpty(resultModel.Comment))
          resultUpdateRequest.TestCaseResult.Comment = resultModel.Comment;
        if (resultModel.DurationInMs > 0.0)
          resultUpdateRequest.TestCaseResult.Duration = TimeSpan.FromMilliseconds(resultModel.DurationInMs).Ticks;
        bool flag1 = TestManagementServiceUtility.CheckIfDateIsDefaultOrOutsideAllowedRange(context.RequestContext, resultModel.StartedDate);
        bool flag2 = TestManagementServiceUtility.CheckIfDateIsDefaultOrOutsideAllowedRange(context.RequestContext, resultModel.CompletedDate);
        if (!flag1 && !flag2)
        {
          resultUpdateRequest.TestCaseResult.DateStarted = resultModel.StartedDate.ToUniversalTime();
          resultUpdateRequest.TestCaseResult.DateCompleted = resultModel.CompletedDate.ToUniversalTime();
          if (context.IsFeatureEnabled("TestManagement.Server.ResultDurationPrioritization") && resultModel.DurationInMs > 0.0 && run.IsAutomated)
            resultUpdateRequest.TestCaseResult.DateCompleted = resultUpdateRequest.TestCaseResult.DateStarted.AddTimeSpan(TimeSpan.FromMilliseconds(resultModel.DurationInMs));
        }
        else if (!flag1 && run.IsAutomated && resultModel.DurationInMs > 0.0)
        {
          resultUpdateRequest.TestCaseResult.DateStarted = resultModel.StartedDate.ToUniversalTime();
          resultUpdateRequest.TestCaseResult.DateCompleted = resultUpdateRequest.TestCaseResult.DateStarted.AddTicks(resultUpdateRequest.TestCaseResult.Duration);
        }
        else if (!flag2 && run.IsAutomated && resultModel.DurationInMs > 0.0)
        {
          resultUpdateRequest.TestCaseResult.DateCompleted = resultModel.CompletedDate.ToUniversalTime();
          resultUpdateRequest.TestCaseResult.DateStarted = resultUpdateRequest.TestCaseResult.DateCompleted.AddTicks(-resultUpdateRequest.TestCaseResult.Duration);
        }
        else if (run.IsAutomated && resultModel.DurationInMs > 0.0)
        {
          resultUpdateRequest.TestCaseResult.DateStarted = DateTime.UtcNow;
          resultUpdateRequest.TestCaseResult.DateCompleted = resultUpdateRequest.TestCaseResult.DateStarted.AddTicks(resultUpdateRequest.TestCaseResult.Duration);
        }
        if (!string.IsNullOrEmpty(resultModel.ComputerName))
          resultUpdateRequest.TestCaseResult.ComputerName = resultModel.ComputerName;
        if (!string.IsNullOrEmpty(resultModel.State))
          resultUpdateRequest.TestCaseResult.State = this.ValidateAndGetEnumValue<TestResultState>(resultModel.State, TestResultState.Pending);
        if (!string.IsNullOrEmpty(resultModel.ErrorMessage))
          resultUpdateRequest.TestCaseResult.ErrorMessage = resultModel.ErrorMessage;
        if (!string.IsNullOrEmpty(resultModel.FailureType))
          resultUpdateRequest.TestCaseResult.FailureType = Convert.ToByte(TestManagementServiceUtility.GetFailureTypeIdFromFailureTypeName(context, resultModel.FailureType, teamProjectTestArtifacts.FailureTypes));
        if (!string.IsNullOrEmpty(resultModel.ResolutionState))
          resultUpdateRequest.TestCaseResult.ResolutionStateId = (int) Convert.ToByte(this.GetResolutionStateIdFromResolutionStateName(resultModel.ResolutionState, teamProjectTestArtifacts.ResolutionStates));
        else if (resultModel.ResolutionStateId > 0)
          resultUpdateRequest.TestCaseResult.ResolutionStateId = resultModel.ResolutionStateId;
        if (!string.IsNullOrEmpty(resultModel.Outcome))
          resultUpdateRequest.TestCaseResult.Outcome = this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(resultModel.Outcome, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None);
        if (resultUpdateRequest.TestCaseResult.Outcome == (byte) 3)
          ++failedResults;
        if (!string.IsNullOrEmpty(resultModel.Comment))
          sizeOfComments += (double) resultModel.Comment.Length;
        if (!string.IsNullOrEmpty(resultModel.ErrorMessage))
          sizeOfErrorMessage += (double) resultModel.ErrorMessage.Length;
        if (!string.IsNullOrEmpty(resultModel.StackTrace))
          sizeOfStackTrace += (double) resultModel.StackTrace.Length;
        Guid ownerId;
        string ownerName;
        this.PopulateResultOwner(resultModel.Owner, tfsIdentityCache, ref resultsWithOwner, ref resultsWithValidOwnerDirectoryAlias, ref resultsWithValidOwnerDisplayName, out ownerId, out ownerName);
        resultUpdateRequest.TestCaseResult.Owner = ownerId;
        resultUpdateRequest.TestCaseResult.OwnerName = ownerName;
        Guid runById;
        string runByName;
        this.PopulateResultRunBy(resultModel.RunBy, tfsIdentityCache, out runById, out runByName);
        resultUpdateRequest.TestCaseResult.RunBy = runById;
        resultUpdateRequest.TestCaseResult.RunByName = runByName;
        if (resultModel.SubResults != null && resultModel.SubResults.Any<TestSubResult>())
        {
          int maxIterations;
          resultUpdateRequest.TestCaseResult.SubResultCount = this.CalculateSubResults(context, resultModel.SubResults, hierarchicalResultFFEnabled, out maxIterations);
          totalSubResults += resultUpdateRequest.TestCaseResult.SubResultCount;
          ++hierarchicalResults;
          num = Math.Max(num, maxIterations);
          TestManagementRequestContext context1 = context;
          Guid projectId1 = projectId;
          TestCaseResult testCaseResult = resultUpdateRequest.TestCaseResult;
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webResult = resultModel;
          BuildConfiguration buildReference = run.BuildReference;
          int buildDefinitionId = buildReference != null ? buildReference.BuildDefinitionId : 0;
          IList<TestExtensionFieldDetails> fieldDetails = resultExFieldDetails;
          this.PopulateFlakyCustomFieldIfRequired(context1, projectId1, testCaseResult, webResult, buildDefinitionId, fieldDetails);
          this.CalculateTelemetryInfo(resultModel, ref rerunCount, ref ddCount, ref otCount, ref genericCount, ref passedOnRerunCount, ref rerunDD, ref rerunOT, ref rerunGeneric, ref maxAttemptId);
        }
        resultUpdateRequest.TestCaseResult.ResultGroupType = resultModel.ResultGroupType;
        this.PopulateAdditionalTestFieldDetails(context, resultUpdateRequest.TestCaseResult, resultModel.StackTrace, (IList<CustomTestField>) resultModel.CustomFields, resultExFieldDetails);
        this.ResetErrorMessageStacktraceForRerunPassedAttempt(resultUpdateRequest.TestCaseResult, resultExFieldDetails);
        if (updateIterationDetails && resultModel.IterationDetails != null)
          this.PopulateIterationDetails(context, run.TestRunId, id, resultModel.IterationDetails, resultUpdateRequest);
        List<WorkItem> workItemList = (List<WorkItem>) null;
        if (resultModel.AssociatedBugs != null && resultModel.AssociatedBugs.Any<ShallowReference>())
        {
          int[] array = resultModel.AssociatedBugs.Select<ShallowReference, int>((System.Func<ShallowReference, int>) (w =>
          {
            int result2 = 0;
            if (string.IsNullOrEmpty(w.Id))
              return result2;
            int.TryParse(w.Id, out result2);
            return result2;
          })).ToArray<int>();
          workItemList = this.ValidateAndGetAssociatedWorkItems(context, projectId, array);
        }
        tupleList.Add(new Tuple<ResultUpdateRequest, List<WorkItem>>(resultUpdateRequest, workItemList));
      }
      this.PublishTestResultTelemetry(context, run.TestRunId, totalResults, resultsWithOwner, resultsWithValidOwnerDisplayName, resultsWithValidOwnerDirectoryAlias, totalSubResults, hierarchicalResults, rerunCount, ddCount, otCount, genericCount, num, passedOnRerunCount, rerunDD, rerunOT, rerunGeneric, maxAttemptId, ((TestRunType) run.Type).ToString(), failedResults, sizeOfComments, sizeOfErrorMessage, sizeOfStackTrace, "Update", (int) run.State);
      return tupleList;
    }

    private void PopulateResultOwner(
      IdentityRef owner,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache,
      ref int resultsWithOwner,
      ref int resultsWithValidOwnerDirectoryAlias,
      ref int resultsWithValidOwnerDisplayName,
      out Guid ownerId,
      out string ownerName)
    {
      ownerId = Guid.Empty;
      ownerName = (string) null;
      if (owner != null && !string.IsNullOrEmpty(owner.DirectoryAlias))
      {
        ++resultsWithOwner;
        Microsoft.VisualStudio.Services.Identity.Identity tfsIdentity = this.GetTfsIdentity(owner.DirectoryAlias, tfsIdentityCache);
        if (tfsIdentity != null)
        {
          ownerName = tfsIdentity.DisplayName;
          Guid result;
          ownerId = string.IsNullOrEmpty(owner.Id) || !Guid.TryParse(owner.Id, out result) || !(result != Guid.Empty) ? tfsIdentity.Id : result;
          ++resultsWithValidOwnerDirectoryAlias;
        }
        else
        {
          ownerName = owner.DirectoryAlias;
          Guid result;
          ownerId = string.IsNullOrEmpty(owner.Id) || !Guid.TryParse(owner.Id, out result) ? Guid.Empty : result;
        }
      }
      else if (owner != null && !string.IsNullOrEmpty(owner.DisplayName))
      {
        ++resultsWithOwner;
        Microsoft.VisualStudio.Services.Identity.Identity tfsIdentity = this.GetTfsIdentity(owner.DisplayName, tfsIdentityCache);
        if (tfsIdentity != null)
        {
          ownerName = tfsIdentity.DisplayName;
          Guid result;
          ownerId = string.IsNullOrEmpty(owner.Id) || !Guid.TryParse(owner.Id, out result) || !(result != Guid.Empty) ? tfsIdentity.Id : result;
          ++resultsWithValidOwnerDisplayName;
        }
        else
        {
          ownerName = owner.DisplayName;
          Guid result;
          ownerId = string.IsNullOrEmpty(owner.Id) || !Guid.TryParse(owner.Id, out result) ? Guid.Empty : result;
        }
      }
      else
      {
        Guid result;
        if (owner == null || string.IsNullOrEmpty(owner.Id) || !Guid.TryParse(owner.Id, out result) || !(result != Guid.Empty))
          return;
        ++resultsWithOwner;
        ownerId = result;
      }
    }

    private void PopulateResultRunBy(
      IdentityRef runBy,
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> tfsIdentityCache,
      out Guid runById,
      out string runByName)
    {
      runById = Guid.Empty;
      runByName = !string.IsNullOrEmpty(runBy?.DisplayName) ? runBy.DisplayName : (string) null;
      Guid result;
      if (runBy != null && !string.IsNullOrEmpty(runBy.Id) && Guid.TryParse(runBy.Id, out result) && result != Guid.Empty)
      {
        runById = result;
      }
      else
      {
        if (runBy == null || string.IsNullOrEmpty(runBy.DisplayName))
          return;
        runById = tfsIdentityCache.ContainsKey(runBy.DisplayName) ? tfsIdentityCache[runBy.DisplayName].Id : Guid.Empty;
      }
    }

    private void ResetErrorMessageStacktraceForRerunPassedAttempt(
      TestCaseResult result,
      IList<TestExtensionFieldDetails> resultExFieldDetails)
    {
      List<TestExtensionField> customFields = result.CustomFields;
      if ((customFields != null ? customFields.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "AttemptId", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>() : (TestExtensionField) null) == null || (int) result.Outcome != (int) Convert.ToByte((object) Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Passed))
        return;
      if (result.CustomFields.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "StackTrace", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>() == null)
      {
        TestExtensionFieldDetails extensionFieldDetails = resultExFieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "StackTrace", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (extensionFieldDetails != null)
          result.CustomFields.Add(new TestExtensionField()
          {
            Field = extensionFieldDetails,
            Value = (object) string.Empty
          });
      }
      if (result.CustomFields.Where<TestExtensionField>((System.Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionField>() != null)
        return;
      TestExtensionFieldDetails extensionFieldDetails1 = resultExFieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
      if (extensionFieldDetails1 == null)
        return;
      result.CustomFields.Add(new TestExtensionField()
      {
        Field = extensionFieldDetails1,
        Value = (object) string.Empty
      });
    }

    private void PopulateFlakyCustomFieldIfRequired(
      TestManagementRequestContext context,
      Guid projectId,
      TestCaseResult result,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webResult,
      int buildDefinitionId,
      IList<TestExtensionFieldDetails> fieldDetails)
    {
      List<int> allowedPipelines;
      if (!this.IsRerunDetectionEnabled(context, projectId, out allowedPipelines) || webResult.ResultGroupType != ResultGroupType.Rerun || buildDefinitionId == 0 || allowedPipelines.Count > 0 && !allowedPipelines.Contains(buildDefinitionId))
        return;
      List<CustomTestField> customFields = webResult.CustomFields;
      CustomTestField customTestField = customFields != null ? customFields.Where<CustomTestField>((System.Func<CustomTestField, bool>) (field => string.Equals(field?.FieldName, "AttemptId", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<CustomTestField>() : (CustomTestField) null;
      if (customTestField == null || Convert.ToInt32(customTestField.Value) <= 0 || !string.Equals(webResult.Outcome, "Passed", StringComparison.OrdinalIgnoreCase))
        return;
      TestExtensionFieldDetails extensionFieldDetails;
      object obj;
      if (context.IsFeatureEnabled("TestManagement.Server.EnablePassedFlakyPropagation"))
      {
        extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "IsTestResultFlaky", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        obj = (object) true;
      }
      else
      {
        extensionFieldDetails = fieldDetails.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "OutcomeConfidence", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        obj = (object) 0.0;
      }
      if (extensionFieldDetails == null)
        return;
      if (result.CustomFields != null && result.CustomFields.Any<TestExtensionField>())
      {
        result.CustomFields.Add(new TestExtensionField()
        {
          Field = extensionFieldDetails,
          Value = obj
        });
      }
      else
      {
        result.CustomFields = new List<TestExtensionField>();
        result.CustomFields.Add(new TestExtensionField()
        {
          Field = extensionFieldDetails,
          Value = obj
        });
      }
    }

    private bool IsRerunDetectionEnabled(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      out List<int> allowedPipelines)
    {
      allowedPipelines = new List<int>();
      if (!tcmRequestContext.IsFeatureEnabled("TestManagement.Server.EnableTestProjectSettings"))
        return tcmRequestContext.IsFeatureEnabled("TestManagement.Server.EnableTaskRerunDetection");
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      TestResultsSettings testResultsSettings = tcmRequestContext.RequestContext.GetService<ITeamFoundationTestManagementTestResultsSettingsService>().GetTestResultsSettings(tcmRequestContext, projectFromGuid, TestResultsSettingsType.Flaky);
      switch (testResultsSettings.FlakySettings?.FlakyDetection?.FlakyDetectionType.GetValueOrDefault())
      {
        case FlakyDetectionType.Custom:
          return false;
        case FlakyDetectionType.System:
          ref List<int> local = ref allowedPipelines;
          FlakySettings flakySettings = testResultsSettings.FlakySettings;
          List<int> intList;
          if (flakySettings == null)
          {
            intList = (List<int>) null;
          }
          else
          {
            FlakyDetection flakyDetection = flakySettings.FlakyDetection;
            if (flakyDetection == null)
            {
              intList = (List<int>) null;
            }
            else
            {
              FlakyDetectionPipelines detectionPipelines = flakyDetection.FlakyDetectionPipelines;
              if (detectionPipelines == null)
              {
                intList = (List<int>) null;
              }
              else
              {
                int[] allowedPipelines1 = detectionPipelines.AllowedPipelines;
                intList = allowedPipelines1 != null ? ((IEnumerable<int>) allowedPipelines1).ToList<int>() : (List<int>) null;
              }
            }
          }
          if (intList == null)
            intList = new List<int>();
          local = intList;
          return true;
        default:
          return false;
      }
    }

    private void PopulateIterationDetails(
      TestManagementRequestContext context,
      int runId,
      int testResultId,
      List<TestIterationDetailsModel> iterationDetails,
      ResultUpdateRequest resultUpdateRequest)
    {
      TestResultArtifacts iterationDetails1 = context.RequestContext.GetService<ITeamFoundationTestManagementIterationResultService>().GetIterationDetails(context.RequestContext, runId, testResultId, iterationDetails);
      resultUpdateRequest.ActionResults = iterationDetails1.ActionResults.ToArray();
      resultUpdateRequest.Parameters = iterationDetails1.ParameterResults.ToArray();
    }

    private void ResolveResultHistoryGroupNames(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      ResultsFilter filter,
      TestResultHistory groupedResults)
    {
      if (!string.Equals(filter.GroupBy, "Environment", StringComparison.OrdinalIgnoreCase))
        return;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectReference.Name);
      Dictionary<int, string> definitionIdToNameMap = this.ReleaseServiceHelper.GetEnvironmentDefinitionIdToNameMap(context.RequestContext, projectFromName);
      foreach (TestResultHistoryDetailsForGroup historyDetailsForGroup in (IEnumerable<TestResultHistoryDetailsForGroup>) groupedResults.ResultsForGroup)
      {
        string empty = string.Empty;
        Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference groupByValue = historyDetailsForGroup.GroupByValue as Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference;
        if (definitionIdToNameMap.TryGetValue(groupByValue.EnvironmentDefinitionId, out empty))
          groupByValue.EnvironmentDefinitionName = empty;
      }
    }

    private TestHistoryQuery _QueryTestHistory(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestHistoryQuery filter)
    {
      context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
      ArgumentUtility.CheckForNull<TestHistoryQuery>(filter, nameof (filter), "Test Results");
      if (filter.TestCaseId < 0)
        throw new InvalidPropertyException("TestCaseId", ServerResources.QueryParameterOutOfRange);
      if (filter.TestCaseId == 0)
        ArgumentUtility.CheckStringForNullOrEmpty(filter.AutomatedTestName, "AutomatedTestName", "Test Results");
      IVssRegistryService service = context.RequestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxTrendDaysForTestHistory", 7);
      if (filter.TrendDays > num1 || filter.TrendDays < 0)
        throw new InvalidPropertyException("TrendDays", ServerResources.QueryParameterOutOfRange);
      if (filter.TrendDays == 0)
        filter.TrendDays = num1;
      DateTime minValue = DateTime.MinValue;
      DateTime t1;
      if (filter.MaxCompleteDate.HasValue)
      {
        t1 = DateTime.SpecifyKind(filter.MaxCompleteDate.Value, DateTimeKind.Utc);
        if (DateTime.Compare(t1, SqlDateTime.MinValue.Value) < 0 || DateTime.Compare(t1, SqlDateTime.MaxValue.Value) > 0)
          throw new InvalidPropertyException("MaxCompleteDate", ServerResources.QueryParameterOutOfRange);
      }
      else
        t1 = DateTime.UtcNow;
      filter.MaxCompleteDate = new DateTime?(t1);
      if (filter.GroupBy != TestResultGroupBy.Branch && filter.GroupBy != TestResultGroupBy.Environment)
        throw new InvalidPropertyException(string.Format(ServerResources.NotSupportedArgumentValue, (object) filter.GroupBy, (object) "GroupBy"));
      if (filter.TestCaseId == 0 && context.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodName"))
        filter.AutomatedTestName = this.m_testMethodNameSanitizer.Sanitize(context, filter.AutomatedTestName);
      context.TraceVerbose("BusinessLayer", "Test history: Looking for override RegKey {0}", (object) "/Service/TestManagement/TestHistoryOperatingTestResultBatchLimit");
      int testHistoryOperatingTestResultBatchLimit = service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/TestHistoryOperatingTestResultBatchLimit", 4000);
      bool currentServiceSameAsContinuationToken;
      int continuationTokenMinRunId;
      int continuationTokenMaxRunId;
      this.ParseContinuationTokenResultId(context, filter.ContinuationToken, out continuationTokenMinRunId, out continuationTokenMaxRunId, out currentServiceSameAsContinuationToken);
      if (continuationTokenMinRunId <= 0 || continuationTokenMinRunId > continuationTokenMaxRunId)
      {
        continuationTokenMinRunId = 0;
        continuationTokenMaxRunId = 0;
      }
      TestHistoryContinuationTokenAndResults contTokenAndResults = new TestHistoryContinuationTokenAndResults()
      {
        ContinuationTokenMinRunId = 0,
        ContinuationTokenMaxRunId = 0,
        results = new Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>()
      };
      TestHistoryQuery currentResult = new TestHistoryQuery()
      {
        AutomatedTestName = filter.AutomatedTestName,
        MaxCompleteDate = filter.MaxCompleteDate,
        TrendDays = filter.TrendDays,
        GroupBy = filter.GroupBy,
        Branch = filter.Branch,
        BuildDefinitionId = filter.BuildDefinitionId,
        TestCaseId = filter.TestCaseId,
        ReleaseEnvDefinitionId = filter.ReleaseEnvDefinitionId,
        ContinuationToken = filter.ContinuationToken,
        ResultsForGroup = (IList<TestResultHistoryForGroup>) new List<TestResultHistoryForGroup>()
      };
      return this.ExecuteAction<TestHistoryQuery>(context.RequestContext, "TeamFoundationTestManagementResultService.QueryTestHistory", (Func<TestHistoryQuery>) (() =>
      {
        int num2 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM") ? 1 : 0;
        int testRunIdThreshold = 0;
        if (num2 == 0)
        {
          if (currentServiceSameAsContinuationToken)
          {
            TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => testRunIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext)), context.RequestContext);
            currentResult = this.QueryTestHistoryLocal(context, projectInfo, ref filter, contTokenAndResults, currentResult, continuationTokenMinRunId, continuationTokenMaxRunId, testHistoryOperatingTestResultBatchLimit, context.IsTcmService ? this.m_tcmServiceName : this.m_tfsServiceName, testRunIdThreshold);
            if (!string.IsNullOrEmpty(currentResult.ContinuationToken))
              return currentResult;
          }
          TestHistoryQuery result;
          if (context.TcmServiceHelper.TryQueryTestHistory(context.RequestContext, projectInfo.Id, filter, out result))
            currentResult = context.MergeDataHelper.MergeTestHistory(currentResult, result);
        }
        else
          currentResult = this.QueryTestHistoryLocal(context, projectInfo, ref filter, contTokenAndResults, currentResult, continuationTokenMinRunId, continuationTokenMaxRunId, testHistoryOperatingTestResultBatchLimit, this.m_tcmServiceName);
        return currentResult;
      }), 1015095, "TestResultsInsights");
    }

    private TestHistoryQuery QueryTestHistoryLocal(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      ref TestHistoryQuery filter,
      TestHistoryContinuationTokenAndResults contTokenAndResults,
      TestHistoryQuery currentResult,
      int continuationTokenMinRunId,
      int continuationTokenMaxRunId,
      int testHistoryOperatingTestResultBatchLimit,
      string svcName,
      int testRunIdThreshold = 0)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
        contTokenAndResults = managementDatabase.QueryTestHistory(projectInfo.Id, filter, continuationTokenMinRunId, continuationTokenMaxRunId, testHistoryOperatingTestResultBatchLimit, testRunIdThreshold);
      if (contTokenAndResults.ContinuationTokenMaxRunId > contTokenAndResults.ContinuationTokenMinRunId)
        currentResult.ContinuationToken = contTokenAndResults.ContinuationTokenMinRunId.ToString() + this.m_tokenSeparator.ToString() + contTokenAndResults.ContinuationTokenMaxRunId.ToString() + this.m_tokenSeparator.ToString() + svcName;
      else
        currentResult.ContinuationToken = string.Empty;
      Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> results = contTokenAndResults.results;
      if (results != null)
        results.ForEach<KeyValuePair<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>>((Action<KeyValuePair<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>>) (groupResult => currentResult.ResultsForGroup.Add(new TestResultHistoryForGroup()
        {
          GroupByValue = groupResult.Key,
          DisplayName = groupResult.Key.ToString(),
          Results = groupResult.Value
        })));
      this.ResolveDisplayNameForGroupIdentifier(context, new GuidAndString(projectInfo.Uri, projectInfo.Id), currentResult);
      if (string.IsNullOrEmpty(currentResult.ContinuationToken))
        filter.ContinuationToken = string.Empty;
      return currentResult;
    }

    private void ResolveDisplayNameForGroupIdentifier(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestHistoryQuery filter)
    {
      if (filter.GroupBy == TestResultGroupBy.Environment)
      {
        Dictionary<int, string> definitionIdToNameMap = this.ReleaseServiceHelper.GetEnvironmentDefinitionIdToNameMap(context.RequestContext, projectId);
        foreach (TestResultHistoryForGroup resultHistoryForGroup in (IEnumerable<TestResultHistoryForGroup>) filter.ResultsForGroup)
        {
          string str;
          if (definitionIdToNameMap.TryGetValue(Convert.ToInt32(resultHistoryForGroup.GroupByValue), out str))
            resultHistoryForGroup.DisplayName = str;
        }
      }
      else
      {
        if (filter.GroupBy != TestResultGroupBy.Branch || filter.BuildDefinitionId <= 0)
          return;
        string definitionNameFromId = this.BuildServiceHelper.GetBuildDefinitionNameFromId(context.RequestContext, projectId.GuidId, filter.BuildDefinitionId);
        if (filter.ResultsForGroup.Count <= 0)
          return;
        filter.ResultsForGroup[0].DisplayName = definitionNameFromId;
      }
    }

    private void ParseContinuationTokenResultId(
      TestManagementRequestContext context,
      string continuationToken,
      out int continuationTokenMinRunId,
      out int continuationTokenMaxRunId,
      out bool currentServiceSameAsContinuationToken)
    {
      continuationTokenMinRunId = 0;
      continuationTokenMaxRunId = 0;
      currentServiceSameAsContinuationToken = true;
      if (string.IsNullOrEmpty(continuationToken))
        return;
      List<string> list = ((IEnumerable<string>) continuationToken.Split(this.m_tokenSeparator)).ToList<string>();
      if (list.Count != 2 && list.Count != 3)
        return;
      int.TryParse(list[0], out continuationTokenMinRunId);
      int.TryParse(list[1], out continuationTokenMaxRunId);
      if (list.Count != 3)
        return;
      currentServiceSameAsContinuationToken = context.IsTcmService ? string.Equals(list[2], this.m_tcmServiceName, StringComparison.OrdinalIgnoreCase) : string.Equals(list[2], this.m_tfsServiceName, StringComparison.OrdinalIgnoreCase);
    }

    private void SecurifyTestResultHistory(TestHistoryQuery testHistoryQuery, Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      testHistoryQuery.InitializeSecureObject(securedObject);
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult CreateCopyOf(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Id = result.Id,
        AfnStripId = result.AfnStripId,
        Comment = result.Comment,
        Configuration = result.Configuration,
        Project = result.Project,
        StartedDate = result.StartedDate,
        CompletedDate = result.CompletedDate,
        DurationInMs = result.DurationInMs,
        Outcome = result.Outcome,
        Owner = result.Owner,
        Revision = result.Revision,
        RunBy = result.RunBy,
        State = result.State,
        TestCase = result.TestCase,
        TestPoint = result.TestPoint,
        TestRun = result.TestRun,
        ResolutionStateId = result.ResolutionStateId,
        ResolutionState = result.ResolutionState,
        LastUpdatedDate = result.LastUpdatedDate,
        LastUpdatedBy = result.LastUpdatedBy,
        Priority = result.Priority,
        ComputerName = result.ComputerName,
        ResetCount = result.ResetCount,
        Build = result.Build,
        Release = result.Release,
        ErrorMessage = result.ErrorMessage,
        CreatedDate = result.CreatedDate,
        IterationDetails = result.IterationDetails,
        AssociatedBugs = result.AssociatedBugs,
        Url = result.Url,
        FailureType = result.FailureType,
        AutomatedTestName = result.AutomatedTestName,
        AutomatedTestStorage = result.AutomatedTestStorage,
        AutomatedTestType = result.AutomatedTestType,
        AutomatedTestTypeId = result.AutomatedTestTypeId,
        AutomatedTestId = result.AutomatedTestId,
        Area = result.Area,
        TestCaseTitle = result.TestCaseTitle,
        StackTrace = result.StackTrace,
        CustomFields = result.CustomFields,
        FailingSince = result.FailingSince,
        BuildReference = result.BuildReference,
        ReleaseReference = result.ReleaseReference
      };
    }

    private void PublishTestResultTelemetry(
      TestManagementRequestContext context,
      int testRunId,
      int totalResults,
      int resultsWithOwnerInfo,
      int resultsWithValidOwnerDisplayName,
      int resultsWithValidOwnerDirectoryAlias,
      int totalSubResults,
      int hierarchicalResults,
      int rerunCount,
      int ddCount,
      int otCount,
      int genericCount,
      int maxIteration,
      int passedOnRerunCount,
      int rerunDD,
      int rerunOT,
      int rerunGeneric,
      int maxAttemptId,
      string testRunType = "",
      int failedResults = 0,
      double sizeOfComments = 0.0,
      double sizeOfErrorMessage = 0.0,
      double sizeOfStackTrace = 0.0,
      string typeOfOperation = "",
      int runState = -1)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestRunId", testRunId.ToString());
      cid.Add("TestRunType", testRunType);
      cid.Add("TotalResults", totalResults.ToString());
      cid.Add("ResultsWithOwnerInfo", resultsWithOwnerInfo.ToString());
      cid.Add("ResultsWithValidOwnerDisplayName", resultsWithValidOwnerDisplayName.ToString());
      cid.Add("ResultsWithValidOwnerDirectioryAlias", resultsWithValidOwnerDirectoryAlias.ToString());
      cid.Add("TotalSubResults", totalSubResults.ToString());
      cid.Add("TotalHierarchicalResults", hierarchicalResults.ToString());
      cid.Add("MaxResultIterations", maxIteration.ToString());
      cid.Add("TotalRerunResults", rerunCount.ToString());
      cid.Add("TotalDDResults", ddCount.ToString());
      cid.Add("TotalOTResults", otCount.ToString());
      cid.Add("TotalGenericResults", genericCount.ToString());
      cid.Add("MaxRerunAttempts", maxAttemptId.ToString());
      cid.Add("TotalPassedOnRerun", passedOnRerunCount.ToString());
      cid.Add("TotalDDRerun", rerunDD.ToString());
      cid.Add("TotalOTRerun", rerunOT.ToString());
      cid.Add("TotalPassedOnRerun", rerunGeneric.ToString());
      cid.Add("FailedTests", failedResults.ToString());
      cid.Add("ResultCommentsSize", sizeOfComments.ToString());
      cid.Add("ResultErrorMessageSize", sizeOfErrorMessage.ToString());
      cid.Add("ResultStackTraceSize", sizeOfStackTrace.ToString());
      cid.Add("ResultOperation", typeOfOperation);
      cid.Add("RunState", (double) runState);
      this.TelemetryLogger.PublishData(context.RequestContext, "PublishTestResults", cid);
    }

    private void PublishTestResultTelemetry(
      TestManagementRequestContext context,
      int testCaseRefId,
      int flakyCount,
      int unFlakyCount)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("FlakyTestCaseReferenceId", testCaseRefId.ToString());
      cid.Add("FlakyTestCaseReferenceCount", flakyCount.ToString());
      cid.Add("UnFlakyTestCaseReferenceCount", unFlakyCount.ToString());
      this.TelemetryLogger.PublishData(context.RequestContext, "FlakyTestCase", cid);
    }

    private void CalculateTelemetryInfo(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webResult,
      ref int rerunCount,
      ref int ddCount,
      ref int otCount,
      ref int genericCount,
      ref int passedOnRerunCount,
      ref int rerunDD,
      ref int rerunOT,
      ref int rerunGeneric,
      ref int maxAttemptId)
    {
      if (webResult.ResultGroupType == ResultGroupType.Rerun)
      {
        List<CustomTestField> customFields = webResult.CustomFields;
        CustomTestField customTestField = customFields != null ? customFields.Where<CustomTestField>((System.Func<CustomTestField, bool>) (field => string.Equals(field?.FieldName, "AttemptId", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<CustomTestField>() : (CustomTestField) null;
        if (customTestField != null)
          maxAttemptId = Math.Max(maxAttemptId, Convert.ToInt32(customTestField.Value));
        ++rerunCount;
        if (string.Equals(webResult.Outcome, "Passed", StringComparison.OrdinalIgnoreCase))
          ++passedOnRerunCount;
        if (webResult.SubResults == null || !webResult.SubResults.Any<TestSubResult>())
          return;
        if (webResult.SubResults.FirstOrDefault<TestSubResult>().ResultGroupType == ResultGroupType.DataDriven)
        {
          ++ddCount;
          ++rerunDD;
        }
        else if (webResult.SubResults.FirstOrDefault<TestSubResult>().ResultGroupType == ResultGroupType.OrderedTest)
        {
          ++otCount;
          ++rerunOT;
        }
        else
        {
          if (webResult.SubResults.FirstOrDefault<TestSubResult>().ResultGroupType != ResultGroupType.Generic)
            return;
          ++genericCount;
          ++rerunGeneric;
        }
      }
      else if (webResult.ResultGroupType == ResultGroupType.DataDriven)
        ++ddCount;
      else if (webResult.ResultGroupType == ResultGroupType.OrderedTest)
      {
        ++otCount;
      }
      else
      {
        if (webResult.ResultGroupType != ResultGroupType.Generic)
          return;
        ++genericCount;
      }
    }

    private int CalculateSubResults(
      TestManagementRequestContext context,
      List<TestSubResult> subResults,
      bool hierarchicalResultFFEnabled,
      out int maxIterations)
    {
      if (!hierarchicalResultFFEnabled)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.HierarchicalResultNotSupported));
      IVssRegistryService service = context.RequestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = context.RequestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestResultMaxSubResultIterationCount";
      ref RegistryQuery local1 = ref registryQuery;
      int maxSubResultIterationCount = service.GetValue<int>(requestContext1, in local1, 1000);
      IVssRequestContext requestContext2 = context.RequestContext;
      registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TestResultMaxSubResultHierarchyLevel";
      ref RegistryQuery local2 = ref registryQuery;
      int maxSubResultHierarchyLevel = service.GetValue<int>(requestContext2, in local2, 3);
      return this.PopulateSubResultsProperties(subResults, maxSubResultIterationCount, maxSubResultHierarchyLevel, out maxIterations);
    }

    private int PopulateSubResultsProperties(
      List<TestSubResult> subResults,
      int maxSubResultIterationCount,
      int maxSubResultHierarchyLevel,
      out int maxIterations,
      int startId = 1,
      int parentId = 0,
      int level = 1)
    {
      int maxIterations1 = 0;
      if (subResults == null || !subResults.Any<TestSubResult>())
      {
        maxIterations = 0;
        return 0;
      }
      if (level > maxSubResultHierarchyLevel)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSubResultInvalidMaxHierarchy, (object) maxSubResultHierarchyLevel));
      if (subResults.Count > maxSubResultIterationCount)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSubResultCountMaxedOut, (object) maxSubResultIterationCount));
      int num1 = 0;
      maxIterations = subResults.Count;
      foreach (TestSubResult subResult in subResults)
      {
        subResult.Id = startId++;
        subResult.ParentId = parentId;
        int num2 = this.PopulateSubResultsProperties(subResult.SubResults, maxSubResultIterationCount, maxSubResultHierarchyLevel, out maxIterations1, startId, subResult.Id, level + 1);
        maxIterations = Math.Max(maxIterations, maxIterations1);
        startId += num2;
        num1 += num2;
      }
      return num1 + subResults.Count;
    }

    private void PublishSubResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      int testResultId,
      List<TestSubResult> subResults,
      bool hierarchicalResultFFEnabled)
    {
      if (!hierarchicalResultFFEnabled)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.HierarchicalResultNotSupported));
      string plainText = subResults.Serialize<List<TestSubResult>>();
      TestAttachmentRequestModel attachmentRequestModel = new TestAttachmentRequestModel()
      {
        FileName = TestResultsConstants.TestSubResultFileName,
        Stream = this.Base64Encode(plainText),
        AttachmentType = TestResultsConstants.TestSubResultJsonAttachmentType
      };
      context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().CreateTestAttachment(context, attachmentRequestModel, projectReference, testRunId, testResultId);
      context.RequestContext.Trace(1015098, TraceLevel.Info, "TestManagement", "RestLayer", "PublishSubResults used for runId = {0)", (object) testRunId);
    }

    private void PublishSubResultsV2(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      bool hierarchicalResultFFEnabled)
    {
      if (!hierarchicalResultFFEnabled)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.HierarchicalResultNotSupported));
      Dictionary<int, List<TestSubResult>> dictionary1 = results.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (webResult => webResult.Id > 0 && webResult.SubResults != null && webResult.SubResults.Any<TestSubResult>())).Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, (int, List<TestSubResult>)>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, (int, List<TestSubResult>)>) (r => (r.Id, r.SubResults))).ToDictionary<(int, List<TestSubResult>), int, List<TestSubResult>>((System.Func<(int, List<TestSubResult>), int>) (t => t.Id), (System.Func<(int, List<TestSubResult>), List<TestSubResult>>) (t => t.SubResults));
      if (dictionary1 == null || !dictionary1.Any<KeyValuePair<int, List<TestSubResult>>>())
        return;
      string subResultJsonArray = dictionary1.Serialize<Dictionary<int, List<TestSubResult>>>();
      int totalSubResultCount = 0;
      Dictionary<string, object> data = new Dictionary<string, object>();
      List<Dictionary<string, object>> dictionaryList1 = new List<Dictionary<string, object>>();
      foreach (KeyValuePair<int, List<TestSubResult>> keyValuePair in dictionary1)
      {
        Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
        dictionary2["ResultId"] = (object) keyValuePair.Key;
        List<Dictionary<string, object>> dictionaryList2 = new List<Dictionary<string, object>>();
        foreach (TestSubResult testSubResult in keyValuePair.Value)
          dictionaryList2.Add(new Dictionary<string, object>()
          {
            ["SubResultId"] = (object) testSubResult.Id,
            ["OutCome"] = (object) testSubResult.Outcome
          });
        dictionary2["subResultDetails"] = (object) dictionaryList2;
        dictionaryList1.Add(dictionary2);
        totalSubResultCount += keyValuePair.Value.Count;
      }
      data["testResultWithSubResultDetails"] = (object) dictionaryList1;
      data["TestRunId"] = (object) testRunId;
      this.TelemetryLogger.PublishData(context.RequestContext, "TestSubResultDetails", new CustomerIntelligenceData((IDictionary<string, object>) data));
      this.StoreSubResultAsLogFile(context, projectReference, testRunId, subResultJsonArray, dictionary1, this.ShouldUploadFileToLogStore(context, totalSubResultCount));
    }

    private void StoreSubResultAsLogFile(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      string subResultJsonArray,
      Dictionary<int, List<TestSubResult>> subResultMap,
      bool shouldUploadToLogStore)
    {
      TestAttachmentRequestModel attachmentRequestModel = new TestAttachmentRequestModel()
      {
        FileName = TestResultsConstants.TestSubResultV2FileName,
        Stream = subResultJsonArray,
        AttachmentType = TestResultsConstants.TestSubResultJsonAttachmentType
      };
      context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().CreateTestSubResultJsonAttachment(context, attachmentRequestModel, projectReference, testRunId, subResultMap.Select<KeyValuePair<int, List<TestSubResult>>, int>((System.Func<KeyValuePair<int, List<TestSubResult>>, int>) (t => t.Key)).ToArray<int>(), shouldUploadToLogStore);
    }

    private string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

    private bool ShouldUploadFileToLogStore(
      TestManagementRequestContext context,
      int totalSubResultCount)
    {
      int num1 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.StoreAllSubResultToLogStore") ? 1 : 0;
      bool flag1 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.StoreSubResultJSONToLogStore");
      bool flag2 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService");
      int num2 = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxSubResultCountForAttachmentInLogStore", 10000);
      int num3 = flag2 ? 1 : 0;
      return (num1 & num3 & (flag1 ? 1 : 0) | (!(flag2 & flag1) ? (false ? 1 : 0) : (totalSubResultCount <= num2 ? 1 : 0))) != 0;
    }

    private void SecureTestResultsGroupsForBuild(
      TestResultsGroupsForBuild testResultsGroupsForBuild,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      testResultsGroupsForBuild.InitializeSecureObject(securedObject);
    }

    private void SecureTestResultsGroupsForRelease(
      TestResultsGroupsForRelease testResultsGroupsForRelease,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      testResultsGroupsForRelease.InitializeSecureObject(securedObject);
    }

    private void ValidateUpdateTestResultsMetaDatParameters(
      ProjectInfo projectInfo,
      int testCaseReferenceId,
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput)
    {
      this.ValidateProjectInfo(projectInfo);
      ArgumentUtility.CheckForNull<TestResultMetaDataUpdateInput>(testResultMetaDataUpdateInput, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "testResultMetaDataUpdateModel"));
      ArgumentUtility.CheckForNull<List<TestFlakyIdentifier>>(testResultMetaDataUpdateInput.FlakyIdentifiers, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FlakyIdentifiers"));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) testResultMetaDataUpdateInput.FlakyIdentifiers, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FlakyIdentifiers"));
      foreach (TestFlakyIdentifier flakyIdentifier in testResultMetaDataUpdateInput.FlakyIdentifiers)
      {
        ArgumentUtility.CheckForNull<TestFlakyIdentifier>(flakyIdentifier, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FlakyIdentifiers"));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(flakyIdentifier.BranchName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "BranchName"));
        ArgumentUtility.CheckForNonPositiveInt(testCaseReferenceId, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "TestCaseReferenceId"));
      }
      this.ValidateInputDuplication(testResultMetaDataUpdateInput);
    }

    private void ValidateProjectInfo(ProjectInfo projectInfo)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo), "BusinessLayer");
      ArgumentUtility.CheckForEmptyGuid(projectInfo.Id, "teamProjectId", "BusinessLayer");
      ArgumentUtility.CheckStringForNullOrEmpty(projectInfo.Name, "teamProjectName", "BusinessLayer");
    }

    private void ValidateInputDuplication(
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput)
    {
      HashSet<TestBranchFlakinesStateMap> flakinesStateMapSet = new HashSet<TestBranchFlakinesStateMap>();
      foreach (TestFlakyIdentifier flakyIdentifier in testResultMetaDataUpdateInput.FlakyIdentifiers)
      {
        TestBranchFlakinesStateMap flakinesStateMap = !flakyIdentifier.IsFlaky ? new TestBranchFlakinesStateMap(flakyIdentifier.BranchName, false) : new TestBranchFlakinesStateMap(flakyIdentifier.BranchName, true);
        if (flakinesStateMapSet.Contains(flakinesStateMap))
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateBranchFlakyStateUpdate, (object) flakinesStateMap.BranchName, (object) flakinesStateMap.IsFlaky));
        flakinesStateMapSet.Add(flakinesStateMap);
      }
    }

    private void UpdateTestResultsMetaDataUtil(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int testCaseReferenceId,
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput,
      string actionName)
    {
      List<TestBranchFlakinesStateMap> markFlakyMap = new List<TestBranchFlakinesStateMap>();
      List<TestBranchFlakinesStateMap> unMarkFlakyMap = new List<TestBranchFlakinesStateMap>();
      foreach (TestFlakyIdentifier flakyIdentifier in testResultMetaDataUpdateInput.FlakyIdentifiers)
      {
        if (flakyIdentifier.IsFlaky)
          markFlakyMap.Add(new TestBranchFlakinesStateMap(flakyIdentifier.BranchName, true));
        else
          unMarkFlakyMap.Add(new TestBranchFlakinesStateMap(flakyIdentifier.BranchName, false));
      }
      context.SecurityManager.CheckViewTestResultsPermission(context, projectInfo.Uri);
      int count1 = markFlakyMap.Count;
      int count2 = unMarkFlakyMap.Count;
      this.PublishTestResultTelemetry(context, testCaseReferenceId, count1, count2);
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", actionName, isTopLevelScenario: true))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context.RequestContext))
          managementDatabase.UpdateTestResultsMetaData(projectInfo.Id, testCaseReferenceId, this.GetMaxBranchLimitForTestFlakiness(context), markFlakyMap, unMarkFlakyMap);
      }
    }

    private TestConfigurationHelper TestConfigurationHelper
    {
      get => this.m_testConfigurationHelper ?? (this.m_testConfigurationHelper = new TestConfigurationHelper());
      set => this.m_testConfigurationHelper = value;
    }

    private IReleaseServiceHelper ReleaseServiceHelper
    {
      get => this.m_releaseServiceHelper ?? (IReleaseServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper();
      set => this.m_releaseServiceHelper = value;
    }

    private IResultsSortingHelper ResultsSortingHelper
    {
      get => this.m_resutsSortingHelper ?? (IResultsSortingHelper) new Microsoft.TeamFoundation.TestManagement.Server.ResultsSortingHelper();
      set => this.m_resutsSortingHelper = value;
    }

    internal ITelemetryLogger TelemetryLogger
    {
      get
      {
        if (this.m_telemetryLogger == null)
          this.m_telemetryLogger = (ITelemetryLogger) new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger();
        return this.m_telemetryLogger;
      }
      set => this.m_telemetryLogger = value;
    }
  }
}
