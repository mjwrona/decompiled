// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server.Hub;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class ResultsHelper : RestApiHelper
  {
    private TestConfigurationHelper m_testConfigurationHelper;
    private RunsHelper m_runsHelper;
    private TestMethodNameSanitizer m_testMethodNameSanitizer;
    private const int c_ResultsBatchSizeToFetchAssociatedWorkItems = 100;
    private const int c_TestResultsByTestCaseIdPageSize = 200;
    private const char c_stepIdentifierDelimiter = ';';

    public ResultsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
      this.m_testMethodNameSanitizer = new TestMethodNameSanitizer();
    }

    public IList<ShallowTestCaseResult> GetTestResultsByPipeline(
      ProjectInfo projectInfo,
      PipelineReference pipelineReference,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int top,
      string continuationToken)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResultsByPipeline", 50, true))
        return this.ExecuteAction<IList<ShallowTestCaseResult>>("ResultsHelper.GetTestResultsByBuild", (Func<IList<ShallowTestCaseResult>>) (() =>
        {
          int continuationTokenRunId;
          int continuationTokenResultId;
          ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
          IList<ShallowTestCaseResult> resultsByPipeline = this.TestManagementResultService.GetTestResultsByPipeline(this.TestManagementRequestContext, projectInfo, pipelineReference, outcomes, continuationTokenRunId, continuationTokenResultId, top);
          this.SecureTestCaseShallowReferences(projectInfo, resultsByPipeline);
          return resultsByPipeline;
        }), 1015052, "TestResultsInsights");
    }

    public IList<ShallowTestCaseResult> GetTestResultsByBuild(
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int top,
      string continuationToken)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResultsByBuild", 50, true))
        return this.ExecuteAction<IList<ShallowTestCaseResult>>("ResultsHelper.GetTestResultsByBuild", (Func<IList<ShallowTestCaseResult>>) (() =>
        {
          IList<ShallowTestCaseResult> shallowTestCaseResultList = (IList<ShallowTestCaseResult>) new List<ShallowTestCaseResult>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          IList<ShallowTestCaseResult> testResults;
          if (!flag1 && !flag2)
          {
            testResults = this.GetTestResultsByBuildWithS2SMerge(projectInfo, buildId, publishContext, outcomes, top, continuationToken);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            int continuationTokenRunId;
            int continuationTokenResultId;
            ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
            testResults = this.TestManagementResultService.GetTestResultsByBuild(this.TestManagementRequestContext, projectInfo, buildId, publishContext, outcomes, continuationTokenRunId, continuationTokenResultId, top);
          }
          this.SecureTestCaseShallowReferences(projectInfo, testResults);
          return testResults;
        }), 1015052, "TestResultsInsights");
    }

    public IList<ShallowTestCaseResult> GetTestResultsByRelease(
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int top,
      string continuationToken)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResultsByRelease", 50, true))
        return this.ExecuteAction<IList<ShallowTestCaseResult>>("ResultsHelper.GetTestResultsByRelease", (Func<IList<ShallowTestCaseResult>>) (() =>
        {
          IList<ShallowTestCaseResult> shallowTestCaseResultList = (IList<ShallowTestCaseResult>) new List<ShallowTestCaseResult>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          IList<ShallowTestCaseResult> testResults;
          if (!flag1 && !flag2)
          {
            testResults = this.GetTestResultsByReleaseWithS2SMerge(projectInfo, releaseId, releaseEnvId, publishContext, outcomes, top, continuationToken);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            int continuationTokenRunId;
            int continuationTokenResultId;
            ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
            testResults = this.TestManagementResultService.GetTestResultsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvId, publishContext, outcomes, continuationTokenRunId, continuationTokenResultId, top);
          }
          this.SecureTestCaseShallowReferences(projectInfo, testResults);
          return testResults;
        }), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResults(
      string projectId,
      int runId,
      bool includeIterationDetails,
      bool includeAssociatedWorkItems,
      bool includePoint,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomeList,
      int skip,
      int top,
      ResultDetails detailsToInclude = ResultDetails.None,
      bool populateConfigNames = false,
      bool newTestsOnly = false,
      bool testSessionProperties = false)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResults", 50, true))
      {
        ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) skip, nameof (skip), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) top, nameof (top), "Test Results");
        this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.GetTestResults projectId = {0} runId = {1}", (object) projectId, (object) runId);
        return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.GetTestResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(projectId);
          string name = projectReference.Name;
          this.CheckForViewTestResultPermission(projectReference.Name);
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResults(this.RequestContext, projectReference.Id, runId, out results, new ResultDetails?(detailsToInclude), new int?(skip), new int?(top), (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) outcomeList))
            {
              if (includePoint)
                this.TestManagementRequestContext.PlannedTestResultsHelper.PopulateTestSuiteDetails(results, name);
              if (populateConfigNames)
                TestCaseResult.UpdateConfigurgationNameForResults(this.TestManagementRequestContext, results, name);
              results.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.SecureTestResultWebApiObject(r)));
              return results;
            }
          }
          else if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          List<TestActionResult> testActionResults = new List<TestActionResult>();
          List<TestResultParameter> resultParameters = new List<TestResultParameter>();
          List<TestResultAttachment> resultAttachments = new List<TestResultAttachment>();
          List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
          Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          Dictionary<TestCaseResultIdentifier, TestResultArtifacts> dictionary = new Dictionary<TestCaseResultIdentifier, TestResultArtifacts>();
          TestManagementRequestContext context = !newTestsOnly || this.TestManagementRequestContext.IsFeatureEnabled("TestManagement.Server.EnableNewTestResultLogging") ? this.TestManagementRequestContext : throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.NewTestOnlySetToTrueWithoutFF));
          int testRunId = runId;
          int pageSize = top;
          ref List<TestCaseResultIdentifier> local1 = ref resultIdentifierList;
          string projectName = name;
          ref List<TestActionResult> local2 = ref testActionResults;
          ref List<TestResultParameter> local3 = ref resultParameters;
          ref List<TestResultAttachment> local4 = ref resultAttachments;
          int offset = skip;
          IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> source = outcomeList;
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> list = source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) null;
          int num1 = newTestsOnly ? 1 : 0;
          int num2 = testSessionProperties ? 1 : 0;
          List<TestCaseResult> testCaseResultList = TestCaseResult.QueryByRun(context, testRunId, pageSize, out local1, projectName, false, out local2, out local3, out local4, offset, list, num1 != 0, num2 != 0);
          if (testCaseResultList != null && testCaseResultList.Any<TestCaseResult>())
          {
            TestRun run = this.FetchTestRun(runId, name);
            Dictionary<int, ResultsHelper.TestRunDetails> testRunDetails = new Dictionary<int, ResultsHelper.TestRunDetails>();
            testRunDetails.Add(run.TestRunId, new ResultsHelper.TestRunDetails()
            {
              TestRun = this.GetRunRepresentation(projectReference.Name, run),
              Build = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, run.BuildReference),
              Project = this.ProjectServiceHelper.GetProjectRepresentation(projectReference),
              ReleaseReference = this.GetReleaseReference(run.ReleaseReference)
            });
            TeamProjectTestArtifacts projectTestArtifacts = this.TestManagementResultService.GetTeamProjectTestArtifacts(this.TestManagementRequestContext, projectReference, false);
            Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults1 = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, testCaseResultList);
            Dictionary<string, ShallowReference> forTestCaseResults2 = this.TestManagementResultService.GetAreaPathUriMappingForTestCaseResults(this.TestManagementRequestContext, testCaseResultList);
            if (includeIterationDetails)
            {
              foreach (TestResultArtifacts testResultsArtifact in (IEnumerable<TestResultArtifacts>) this.TestManagementResultService.FetchTestResultsArtifacts(this.TestManagementRequestContext, testCaseResultList, name))
                dictionary[testResultsArtifact.TestCaseResult.Id] = testResultsArtifact;
            }
            List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList = new List<Tuple<TestCaseResultIdentifier, int, IdAndRev>>();
            foreach (TestCaseResult testCaseResult1 in testCaseResultList)
            {
              TestCaseResult testCaseResult2;
              if (includeIterationDetails && dictionary.ContainsKey(testCaseResult1.Id))
              {
                testCaseResultDataContracts.Add(testCaseResult1.Id, this.ConvertTestCaseResultToDataContract(testRunDetails, projectTestArtifacts, dictionary[testCaseResult1.Id].TestCaseResult, includeIterationDetails, false, dictionary[testCaseResult1.Id].ActionResults, dictionary[testCaseResult1.Id].ParameterResults, dictionary[testCaseResult1.Id].AttachmentResults, true, forTestCaseResults1, forTestCaseResults2, projectReference, testSessionProperties));
                testCaseResult2 = dictionary[testCaseResult1.Id].TestCaseResult;
              }
              else
              {
                testCaseResultDataContracts.Add(testCaseResult1.Id, this.ConvertTestCaseResultToDataContract(testRunDetails, projectTestArtifacts, testCaseResult1, false, false, testActionResults, resultParameters, resultAttachments, true, forTestCaseResults1, forTestCaseResults2, projectReference, testSessionProperties));
                testCaseResult2 = testCaseResult1;
              }
              if (includePoint)
                this.PrepareListOfPointsForEachPlan(testResultTupleList, testCaseResult2);
            }
            if (includePoint)
              this.AddPointDetailsToDataContract(testCaseResultDataContracts, testResultTupleList, projectReference.Name);
            if (includeAssociatedWorkItems)
            {
              Dictionary<TestCaseResultIdentifier, List<ShallowReference>> associatedWorkItems = this.GetAssociatedWorkItems(testCaseResultList, this.TestManagementRequestContext, projectReference.Name);
              if (associatedWorkItems.Any<KeyValuePair<TestCaseResultIdentifier, List<ShallowReference>>>())
              {
                foreach (TestCaseResultIdentifier key in associatedWorkItems.Keys)
                  testCaseResultDataContracts[key].AssociatedBugs = associatedWorkItems[key];
              }
            }
          }
          return testCaseResultDataContracts.Values.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        }), 1015052, "TestResultsInsights");
      }
    }

    private void PrepareListOfPointsForEachPlan(
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList,
      TestCaseResult testCaseResult)
    {
      if (testCaseResult.TestPlanId <= 0)
        return;
      testResultTupleList.Add(new Tuple<TestCaseResultIdentifier, int, IdAndRev>(testCaseResult.Id, testCaseResult.TestPlanId, new IdAndRev()
      {
        Id = testCaseResult.TestPointId
      }));
    }

    private void AddPointDetailsToDataContract(
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts,
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList,
      string projectName)
    {
      if (testResultTupleList == null || testResultTupleList.Count <= 0)
        return;
      this.TestManagementRequestContext.PlannedTestResultsHelper.PopulateTestPointDetails(testCaseResultDataContracts, testResultTupleList, projectName);
    }

    public virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetAssociatedWorkItemsForResult(
      string projectId,
      int runId,
      int testCaseResultId,
      bool getAllWorkitems = false)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetAssociatedWorkItemsForResult", 50, true))
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        List<TestCaseResult> results = new List<TestCaseResult>();
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestRunId = runId;
        testCaseResult.TestResultId = testCaseResultId;
        results.Add(testCaseResult);
        Dictionary<TestCaseResultIdentifier, List<ShallowReference>> associatedWorkItems = this.GetAssociatedWorkItems(results, this.TestManagementRequestContext, projectReference.Name, getAllWorkitems);
        if (associatedWorkItems.Count == 1)
        {
          TestCaseResultIdentifier key = associatedWorkItems.Keys.First<TestCaseResultIdentifier>();
          if (associatedWorkItems[key].Any<ShallowReference>())
            return associatedWorkItems[key].Select<ShallowReference, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>((Func<ShallowReference, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (wi =>
            {
              Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference workItemsForResult = new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference();
              workItemsForResult.Id = wi.Id;
              workItemsForResult.Name = wi.Name;
              workItemsForResult.Url = string.Empty;
              workItemsForResult.WebUrl = string.Empty;
              workItemsForResult.InitializeSecureObject((ISecuredObject) wi);
              return workItemsForResult;
            })).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
        }
        return new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
      }
    }

    public void PopulateSuiteAndConfigurationDetails(List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results, Guid projectId)
    {
      List<TestCaseResult> source1 = new List<TestCaseResult>();
      List<Tuple<int, int, int>> source2 = new List<Tuple<int, int, int>>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        if (result.TestPlan != null && result.TestPoint != null && result.Configuration != null)
          source2.Add(Tuple.Create<int, int, int>(Convert.ToInt32(result.TestPlan.Id), Convert.ToInt32(result.TestPoint.Id), Convert.ToInt32(result.Configuration.Id)));
      }
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(this.TestManagementRequestContext.RequestContext))
      {
        IEnumerable<TestCaseResult> source3 = source2.Distinct<Tuple<int, int, int>>().Select<Tuple<int, int, int>, TestCaseResult>((Func<Tuple<int, int, int>, TestCaseResult>) (tuple => new TestCaseResult()
        {
          TestPlanId = tuple.Item1,
          TestPointId = tuple.Item2,
          ConfigurationId = tuple.Item3
        }));
        source1 = managementDatabase.FetchSuiteAndConfigurationDetails(projectId, source3.ToList<TestCaseResult>());
      }
      Dictionary<Tuple<int, int, int>, TestCaseResult> dictionary = source1.ToDictionary<TestCaseResult, Tuple<int, int, int>, TestCaseResult>((Func<TestCaseResult, Tuple<int, int, int>>) (details => Tuple.Create<int, int, int>(details.TestPlanId, details.TestPointId, details.ConfigurationId)), (Func<TestCaseResult, TestCaseResult>) (details => details));
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in results.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
      {
        if (testCaseResult.TestPlan != null && testCaseResult.TestPoint != null && testCaseResult.Configuration != null)
        {
          Tuple<int, int, int> key = Tuple.Create<int, int, int>(Convert.ToInt32(testCaseResult.TestPlan.Id), Convert.ToInt32(testCaseResult.TestPoint.Id), Convert.ToInt32(testCaseResult.Configuration.Id));
          if (!dictionary.ContainsKey(key))
            results.Remove(testCaseResult);
        }
      }
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        if (result.TestPlan != null && result.TestPoint != null && result.Configuration != null)
        {
          Tuple<int, int, int> key = Tuple.Create<int, int, int>(Convert.ToInt32(result.TestPlan.Id), Convert.ToInt32(result.TestPoint.Id), Convert.ToInt32(result.Configuration.Id));
          TestCaseResult testCaseResult = dictionary[key];
          result.Configuration.Name = testCaseResult.ConfigurationName;
          result.TestSuite = new ShallowReference();
          result.TestSuite.Id = testCaseResult.TestSuiteId.ToString();
          result.TestSuite.Name = testCaseResult.SuiteName;
        }
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult GetTestResultById(
      string projectId,
      int runId,
      int testCaseResultId,
      bool includeIterationDetails,
      bool includeAssociatedBugs,
      bool includeSubResultDetails = false)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResultById", 50, true))
      {
        ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) testCaseResultId, nameof (testCaseResultId), "Test Results");
        this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.GetTestResultById projectId = {0} runId = {1} resultId = {2}", (object) projectId, (object) runId, (object) testCaseResultId);
        return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>("ResultsHelper.GetTestResultById", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(projectId);
          string name1 = projectReference.Name;
          this.CheckForViewTestResultPermission(projectReference.Name);
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultById(this.RequestContext, projectReference.Id, runId, testCaseResultId, includeIterationDetails, includeAssociatedBugs, includeSubResultDetails, out result))
            {
              IPlannedTestResultsHelper testResultsHelper = this.TestManagementRequestContext.PlannedTestResultsHelper;
              List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
              testCaseResults.Add(result);
              string name2 = projectReference.Name;
              testResultsHelper.PopulateTestSuiteDetails(testCaseResults, name2);
              this.SecureTestResultWebApiObject(result);
              return result;
            }
          }
          else if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          TestRun run = this.FetchTestRun(runId, name1);
          Dictionary<int, ResultsHelper.TestRunDetails> testRunDetails = new Dictionary<int, ResultsHelper.TestRunDetails>();
          testRunDetails.Add(run.TestRunId, new ResultsHelper.TestRunDetails()
          {
            TestRun = this.GetRunRepresentation(projectReference.Name, run),
            Build = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, run.BuildReference),
            Project = this.ProjectServiceHelper.GetProjectRepresentation(projectReference),
            ReleaseReference = this.GetReleaseReference(run.ReleaseReference)
          });
          TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name1);
          TeamProjectTestArtifacts projectTestArtifacts = this.TestManagementResultService.GetTeamProjectTestArtifacts(this.TestManagementRequestContext, projectReference, false);
          List<TestCaseResult> testCaseResultList = new List<TestCaseResult>(1);
          testCaseResultList.Add(testResultArtifacts.TestCaseResult);
          Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults1 = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, testCaseResultList);
          Dictionary<string, ShallowReference> forTestCaseResults2 = this.TestManagementResultService.GetAreaPathUriMappingForTestCaseResults(this.TestManagementRequestContext, testCaseResultList);
          Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList = new List<Tuple<TestCaseResultIdentifier, int, IdAndRev>>();
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult dataContract = this.ConvertTestCaseResultToDataContract(testRunDetails, projectTestArtifacts, testResultArtifacts.TestCaseResult, includeIterationDetails, includeSubResultDetails, testResultArtifacts.ActionResults, testResultArtifacts.ParameterResults, testResultArtifacts.AttachmentResults, true, forTestCaseResults1, forTestCaseResults2, projectReference);
          testCaseResultDataContracts.Add(testResultArtifacts.TestCaseResult.Id, dataContract);
          this.PrepareListOfPointsForEachPlan(testResultTupleList, testResultArtifacts.TestCaseResult);
          this.AddPointDetailsToDataContract(testCaseResultDataContracts, testResultTupleList, projectReference.Name);
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResultById = testCaseResultDataContracts.Values.First<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          if (includeAssociatedBugs)
          {
            Dictionary<TestCaseResultIdentifier, List<ShallowReference>> associatedWorkItems = this.GetAssociatedWorkItems(testCaseResultList, this.TestManagementRequestContext, projectReference.Name);
            if (associatedWorkItems.Count == 1)
              testResultById.AssociatedBugs = associatedWorkItems[testCaseResultList[0].Id];
          }
          return testResultById;
        }), 1015052, "TestResultsInsights");
      }
    }

    public TestResultsQuery GetTestResults(GuidAndString projectId, TestResultsQuery query)
    {
      ArgumentUtility.CheckForNull<TestResultsQuery>(query, nameof (query), "Test Results");
      if ((query.Results == null || !query.Results.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>()) && query.ResultsFilter == null)
        throw new ArgumentNullException("Results").Expected("Test Results");
      this.CheckForViewTestResultPermission(projectId.GuidId);
      string actualAutomatedTestCaseName = (string) null;
      if (query.ResultsFilter != null)
      {
        actualAutomatedTestCaseName = query.ResultsFilter.AutomatedTestName;
        if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodName"))
          query.ResultsFilter.AutomatedTestName = this.m_testMethodNameSanitizer.Sanitize(this.m_testManagementRequestContext, query.ResultsFilter.AutomatedTestName);
      }
      bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
      bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
      if (!flag1 && !flag2)
      {
        query.Results = this.GetTestResultsByQueryWithS2SMerge(projectId, query, actualAutomatedTestCaseName);
      }
      else
      {
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        query.Results = this.GetLocalTestResults(projectId, query);
      }
      this.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      query.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.GuidId.ToString()
        }
      });
      if (query.ResultsFilter != null)
        query.ResultsFilter.AutomatedTestName = actualAutomatedTestCaseName;
      return query;
    }

    public List<TestResultMetaData> GetTestResultsMetaData(
      ProjectInfo projectInfo,
      List<string> testReferenceIds,
      ResultMetaDataDetails detailsToInclude = ResultMetaDataDetails.None)
    {
      if (testReferenceIds == null)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) nameof (testReferenceIds))));
      if (testReferenceIds.Count<string>() > 200)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.GetTestResultsMetaDataApiMaxLimitError, (object) 200));
      bool shouldIncludeFlakyDetails = detailsToInclude == ResultMetaDataDetails.FlakyIdentifiers;
      List<int> testRefIdList = this.GetListOfRefIds(this.TestManagementRequestContext.RequestContext, testReferenceIds, nameof (testReferenceIds));
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.GetTestResultsMetaData", 50, true))
        return this.ExecuteAction<List<TestResultMetaData>>("ResultsHelper.GetTestResultsMetaData", (Func<List<TestResultMetaData>>) (() =>
        {
          IList<TestResultMetaData> testResultMetaDataList1 = (IList<TestResultMetaData>) new List<TestResultMetaData>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          IList<TestResultMetaData> testResultMetaDataList2;
          if (!flag1 && !flag2)
          {
            testResultMetaDataList2 = this.GetTestResultsMetaDataWithS2SMerge(projectInfo, testRefIdList, detailsToInclude);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            testResultMetaDataList2 = this.ConvertTestReferenceToMetaData(this.TestManagementResultService.GetTestResultsMetaData(this.TestManagementRequestContext, projectInfo, (IList<int>) testRefIdList, shouldIncludeFlakyDetails));
          }
          this.SecureTestResultsMetaData(projectInfo, testResultMetaDataList2);
          return testResultMetaDataList2.ToList<TestResultMetaData>();
        }), 1015052, "TestResultsInsights");
    }

    private IList<TestResultMetaData> ConvertTestReferenceToMetaData(
      IList<TestCaseReferenceRecord> testCaseReferenceRecords)
    {
      IList<TestResultMetaData> metaData1 = (IList<TestResultMetaData>) new List<TestResultMetaData>();
      foreach (TestCaseReferenceRecord caseReferenceRecord in (IEnumerable<TestCaseReferenceRecord>) testCaseReferenceRecords)
      {
        TestResultMetaData metaData2 = new TestResultMetaData()
        {
          TestCaseReferenceId = caseReferenceRecord.TestCaseReferenceId,
          AutomatedTestName = caseReferenceRecord.AutomatedTestName,
          AutomatedTestStorage = caseReferenceRecord.AutomatedTestStorage,
          Priority = caseReferenceRecord.Priority,
          Owner = caseReferenceRecord.Owner,
          TestCaseTitle = caseReferenceRecord.TestCaseTitle
        };
        this.UpdateFlakyIdentifiers(caseReferenceRecord, metaData2);
        metaData1.Add(metaData2);
      }
      return metaData1;
    }

    private int GetPageSizeForQuery() => this.TestManagementRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TestManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TestResultyByTestCaseIdBatchSize", 200);

    public List<TestIterationDetailsModel> GetTestIterations(
      string projectId,
      int runId,
      int testCaseResultId,
      bool includeActionResults)
    {
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) testCaseResultId, nameof (testCaseResultId), "Test Results");
      return this.ExecuteAction<List<TestIterationDetailsModel>>("ResultsHelper.GetTestIterations", (Func<List<TestIterationDetailsModel>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result;
        if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultById(this.RequestContext, projectReference.Id, runId, testCaseResultId, true, false, false, out result))
        {
          this.SecureTestResultWebApiObject(result);
          return result.IterationDetails;
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name);
        return this.CreateIterationDetails(runId, testCaseResultId, name, testResultArtifacts.ActionResults, testResultArtifacts.ParameterResults, testResultArtifacts.AttachmentResults, includeActionResults);
      }), 1015052, "TestResultsInsights");
    }

    public TestIterationDetailsModel GetTestIteration(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId,
      bool includeActionResults)
    {
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) testCaseResultId, nameof (testCaseResultId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) iterationId, nameof (iterationId), "Test Results");
      return this.ExecuteAction<TestIterationDetailsModel>("ResultsHelper.GetTestIteration", (Func<TestIterationDetailsModel>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result;
        if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultById(this.RequestContext, projectReference.Id, runId, testCaseResultId, true, false, false, out result))
        {
          this.SecureTestResultWebApiObject(result);
          foreach (TestIterationDetailsModel iterationDetail in result.IterationDetails)
          {
            if (iterationDetail.Id == iterationId)
              return iterationDetail;
          }
          return new TestIterationDetailsModel();
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name);
        return this.CreateIterationDetailsModel(runId, testCaseResultId, name, testResultArtifacts.ActionResults, testResultArtifacts.ParameterResults, testResultArtifacts.AttachmentResults, iterationId, includeActionResults) ?? throw new TestObjectNotFoundException(string.Format(ServerResources.IterationResultNotFound, (object) iterationId), ObjectTypes.TestResult);
      }), 1015052, "TestResultsInsights");
    }

    public List<TestActionResultModel> GetTestActionResults(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath)
    {
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) testCaseResultId, nameof (testCaseResultId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) iterationId, nameof (iterationId), "Test Results");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(actionPath, nameof (actionPath), "Test Results");
      return this.ExecuteAction<List<TestActionResultModel>>("ResultsHelper.GetTestActionResults", (Func<List<TestActionResultModel>>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name);
        return this.CreateTestActionResultModels(runId, testCaseResultId, name, testResultArtifacts.ActionResults, new int?(iterationId), actionPath);
      }), 1015052, "TestResultsInsights");
    }

    public List<TestResultParameterModel> GetTestResultParameters(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId,
      string parameterName)
    {
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) testCaseResultId, nameof (testCaseResultId), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) iterationId, nameof (iterationId), "Test Results");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(parameterName, nameof (parameterName), "Test Results");
      return this.ExecuteAction<List<TestResultParameterModel>>("ResultsHelper.GetTestResultParameters", (Func<List<TestResultParameterModel>>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name);
        return this.CreateTestResultParameterModels(runId, testCaseResultId, name, testResultArtifacts.ParameterResults, new int?(iterationId), parameterName);
      }), 1015052, "TestResultsInsights");
    }

    public List<TestCaseResultAttachmentModel> GetTestResultAttachments(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId)
    {
      return this.ExecuteAction<List<TestCaseResultAttachmentModel>>("ResultsHelper.GetTestResultAttachments", (Func<List<TestCaseResultAttachmentModel>>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        TestResultArtifacts testResultArtifacts = this.TestManagementResultService.FetchTestResultArtifacts(this.TestManagementRequestContext, runId, testCaseResultId, name);
        return this.CreateTestResultAttachmentModels(runId, testCaseResultId, name, testResultArtifacts.AttachmentResults, new int?(iterationId));
      }), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      string teamProjectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool updateTestCaseResults = false,
      bool testSessionProperties = false)
    {
      this.ValidateAddTestResultToRun<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(teamProjectId, runId, results);
      this.RequestContext.RequestTimer.SetTimeToFirstPageBegin();
      using (PerfManager.Measure(this.RequestContext, "RestLayer", nameof (AddTestResultsToTestRun), 50, true))
      {
        this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.AddTestResultsToTestRun projectId = {0}", (object) teamProjectId);
        TeamProjectReference projectReference = this.GetProjectReference(teamProjectId);
        if (updateTestCaseResults && this.IsPlannedTestRun(results) && this.TestManagementRequestContext.TcmServiceHelper.IsTestRunInTCM(this.RequestContext, runId) && projectReference != null)
        {
          Guid id = projectReference.Id;
          Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRunById = this.RunsHelper.GetTestRunById(projectReference.Id.ToString(), runId, true);
          if (testRunById != null && testRunById.Plan != null && testRunById.Plan.Id != null && int.Parse(testRunById.Plan.Id) > 0)
            results = this.TestManagementRequestContext.WorkItemFieldDataHelper.PopulateTestResultFromWorkItem(this.TestManagementRequestContext, projectReference.Name, results, int.Parse(testRunById.Plan.Id));
        }
        return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.AddTestResultsToTestRun", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> newTestResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
          bool flag1 = false;
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag3 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag2 && !flag3)
            flag1 = this.TestManagementRequestContext.TcmServiceHelper.TryAddTestResultsToTestRun(this.RequestContext, projectReference.Id, runId, results, out newTestResults);
          else if (flag2 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          if (!flag1)
            newTestResults = this.GetShallowTestResultList(this.TestManagementResultService.AddTestResultsToTestRun(this.TestManagementRequestContext, projectReference, this.TestManagementRunService.FetchTestRun(this.TestManagementRequestContext, runId, Guid.Empty, string.Empty, projectReference.Name), results, testSessionProperties), true);
          return newTestResults;
        }), 1015052, "TestResultsInsights");
      }
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      string teamProjectId,
      int runId,
      TestResultCreateModel[] resultCreateModels)
    {
      this.ValidateAddTestResultToRun<TestResultCreateModel>(teamProjectId, runId, resultCreateModels);
      this.RequestContext.RequestTimer.SetTimeToFirstPageBegin();
      using (PerfManager.Measure(this.RequestContext, "RestLayer", nameof (AddTestResultsToTestRun), 50, true))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] modelToTestResults = this.ConvertResultCreateModelToTestResults(resultCreateModels);
        return this.AddTestResultsToTestRun(teamProjectId, runId, modelToTestResults, true);
      }
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
      string projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) results, nameof (results), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      HashSet<int> intSet = new HashSet<int>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(result, "result", "Test Results");
        if (intSet.Contains(result.Id))
          throw new InvalidPropertyException(nameof (results), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateIdsExceptionMessage, (object) result.Id));
        intSet.Add(result.Id);
      }
      this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.UpdateTestResults projectId = {0}", (object) projectId);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.UpdateTestResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> updatedResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
        bool flag1 = false;
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag3 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag2 && !flag3)
          flag1 = this.TestManagementRequestContext.TcmServiceHelper.TryUpdateTestResults(this.RequestContext, projectReference.Id, runId, results, out updatedResults);
        else if (flag2 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        if (!flag1)
        {
          TestRun testRun = this.TestManagementRunService.FetchTestRun(this.TestManagementRequestContext, runId, Guid.Empty, string.Empty, projectReference.Name);
          updatedResults = this.GetShallowTestResultList(this.TestManagementResultService.UpdateTestResultsWithIterationDetails(this.TestManagementRequestContext, projectReference, testRun, results, !testRun.IsAutomated), true);
          this.NotifyClientsAboutTestRunChange(projectReference.Id, testRun);
        }
        return updatedResults;
      }), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
      string projectId,
      int runId,
      TestCaseResultUpdateModel[] resultUpdateModels)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resultUpdateModels, nameof (resultUpdateModels), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.UpdateTestResults projectId = {0}", (object) projectId);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.UpdateTestResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.UpdateTestResults(projectId, runId, this.ConvertResultUpdateModelToTestResults(resultUpdateModels))), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> BulkUpdateTestResults(
      string projectId,
      int runId,
      string resultIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(result, nameof (result), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckStringForNullOrEmpty(resultIds, nameof (resultIds), "Test Results");
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      int[] ids = TestManagementServiceUtility.GetDistinctTestResultIdsFromString(resultIds);
      this.RequestContext.TraceInfo("RestLayer", "ResultsHelper.BulkUpdateTestResults projectId = {0}", (object) projectId);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.BulkUpdateTestResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TestRun testRunById = this.TestManagementRunService.GetTestRunById(this.TestManagementRequestContext, runId, projectReference);
        TeamProjectTestArtifacts teamProjectTestArtifacts = (TeamProjectTestArtifacts) null;
        return this.GetShallowTestResultList(this.TestManagementResultService.BulkUpdateTestResults(this.TestManagementRequestContext, projectReference, testRunById, ids, result, out teamProjectTestArtifacts, false));
      }), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> BulkUpdateTestResults(
      string projectId,
      int runId,
      string resultIds,
      TestCaseResultUpdateModel resultUpdateModel)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      ArgumentUtility.CheckForNull<TestCaseResultUpdateModel>(resultUpdateModel, nameof (resultUpdateModel), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckStringForNullOrEmpty(resultIds, nameof (resultIds), "Test Results");
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.BulkUpdateTestResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        TestCaseResultUpdateModel[] resultUpdateModel1 = new TestCaseResultUpdateModel[1]
        {
          resultUpdateModel
        };
        return this.BulkUpdateTestResults(projectId, runId, resultIds, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ConvertResultUpdateModelToTestResults(resultUpdateModel1)).First<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>());
      }), 1015052, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByQuery(
      string projectId,
      QueryModel queryModel,
      bool includeResultDetails,
      bool includeIterationDetails,
      int skip,
      int top)
    {
      this.RequestContext.TraceInfo("RestLayer", "RunsHelper.GetTestResultsByQuery projectId = {0}", (object) projectId);
      ArgumentUtility.CheckForNull<QueryModel>(queryModel, "query", "Test Results");
      ArgumentUtility.CheckStringForNullOrEmpty(queryModel.Query, "query", "Test Results");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.GetTestResultsByQuery", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResults;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.RequestContext, projectReference.Id, queryModel, includeResultDetails, includeIterationDetails, skip, top, out testResults))
            testResults.ForEach<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.SecureTestResultWebApiObject(r)));
          Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => testCaseResultDataContracts = this.GetTestResultsByQueryLocal(projectReference, queryModel, includeResultDetails, includeIterationDetails, skip, top)), this.RequestContext);
          IMergeDataHelper mergeDataHelper = this.TestManagementRequestContext.MergeDataHelper;
          Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>.ValueCollection values = testCaseResultDataContracts.Values;
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> list1 = values != null ? values.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> list2 = testResults != null ? testResults.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
          return mergeDataHelper.MergeTestResults(list1, list2);
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>.ValueCollection values1 = this.GetTestResultsByQueryLocal(projectReference, queryModel, includeResultDetails, includeIterationDetails, skip, top).Values;
        return values1 == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null : values1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      }), 1015052, "TestResultsInsights");
    }

    public TestResultsDetails GetTestResultDetailsForBuild(
      string projectId,
      int buildId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), "Test Results");
      if (!string.IsNullOrEmpty(groupBy) && !this.IsGroupByFieldValid(groupBy))
        throw new InvalidPropertyException(nameof (groupBy), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) nameof (groupBy), (object) groupBy));
      if (string.IsNullOrEmpty(sourceWorkflow))
        sourceWorkflow = SourceWorkflow.ContinuousIntegration;
      return this.ExecuteAction<TestResultsDetails>("ResultsHelper.GetTestResultDetailsForBuild", (Func<TestResultsDetails>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        TestResultsDetails testResultsDetails;
        if (!flag1 && !flag2)
        {
          testResultsDetails = this.GetTestResultDetailsForBuildWithS2SMerge(projectReference, buildId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          testResultsDetails = this.GetResultDetailsFromCurrentServiceForBuild(projectReference, buildId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
        }
        this.SecureTestResultDetailsWebApiObject(testResultsDetails, projectReference.Id);
        return testResultsDetails;
      }), 1015052, "TestResultsInsights");
    }

    public TestResultsDetails GetTestResultDetailsForRelease(
      string projectId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      ArgumentUtility.CheckGreaterThanZero((float) releaseId, nameof (releaseId), "Test Results");
      if (!string.IsNullOrEmpty(groupBy) && !this.IsGroupByFieldValid(groupBy))
        throw new InvalidPropertyException(nameof (groupBy), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) nameof (groupBy), (object) groupBy));
      if (string.IsNullOrEmpty(sourceWorkflow))
        sourceWorkflow = SourceWorkflow.ContinuousDelivery;
      return this.ExecuteAction<TestResultsDetails>("ResultsHelper.GetTestResultDetailsForRelease", (Func<TestResultsDetails>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        TestResultsDetails testResultsDetails;
        if (!flag1 && !flag2)
        {
          testResultsDetails = this.GetTestResultDetailsForReleaseWithS2SMerge(projectReference, releaseId, releaseEnvId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
        }
        else
        {
          if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
          testResultsDetails = this.GetResultDetailsFromCurrentServiceForRelease(projectReference, releaseId, releaseEnvId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
        }
        this.SecureTestResultDetailsWebApiObject(testResultsDetails, projectReference.Id);
        return testResultsDetails;
      }), 1015052, "TestResultsInsights");
    }

    public TestResultDocument PublishTestResultDocument(
      ProjectInfo projectInfo,
      int runId,
      TestResultDocument document)
    {
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckForNull<TestResultDocument>(document, nameof (document), "Test Results");
      ArgumentUtility.CheckForNull<TestResultPayload>(document.Payload, "Payload", "Test Results");
      ArgumentUtility.CheckForNull<string>(document.Payload.Name, "Name", "Test Results");
      ArgumentUtility.CheckForNull<string>(document.Payload.Stream, "Stream", "Test Results");
      return this.ExecuteAction<TestResultDocument>("ResultsHelper.PublishTestResultDocument", (Func<TestResultDocument>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectInfo.Name);
        TestAttachmentRequestModel attachmentRequestModel = new TestAttachmentRequestModel()
        {
          FileName = document.Payload.Name,
          Stream = document.Payload.Stream,
          Comment = document.Payload.Comment
        };
        TestAttachmentReference attachment;
        int attachmentId = !this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestRunAttachment(this.RequestContext, projectInfo.Id, attachmentRequestModel, runId, out attachment) ? this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().CreateTestAttachment(this.TestManagementRequestContext, attachmentRequestModel, projectReference, runId) : attachment.Id;
        TestResultDocument testResultDocument = new TestResultDocument();
        this.TestManagementRequestContext.TcmServiceHelper.TryQueuePublishTestResultJob(this.TestManagementRequestContext.RequestContext, projectInfo, runId, attachmentId, document, out testResultDocument);
        return testResultDocument;
      }), 1015052, "TestResultsInsights");
    }

    public bool IsPlannedTestRun(Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResults)
    {
      bool isPlannedTestRun = false;
      if (testCaseResults != null)
        ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResults).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>().ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result =>
        {
          if (result == null || result.TestPoint == null || string.IsNullOrEmpty(result.TestPoint.Id) || int.Parse(result.TestPoint.Id) <= 0)
            return;
          isPlannedTestRun = true;
        }));
      return isPlannedTestRun;
    }

    public void SecureTestResultWebApiObject(Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult)
    {
      this.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      testResult.EnsureSecureObject();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] ConvertResultCreateModelToTestResults(
      TestResultCreateModel[] resultCreateModel)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resultCreateModel, nameof (resultCreateModel), "Test Results");
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      foreach (TestResultCreateModel var in resultCreateModel)
      {
        ArgumentUtility.CheckForNull<TestResultCreateModel>(var, "resultModel", "Test Results");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testCaseResult.TestCase = var.TestCase;
        testCaseResult.Configuration = var.Configuration;
        testCaseResult.TestPoint = var.TestPoint;
        testCaseResult.State = var.State;
        testCaseResult.ComputerName = var.ComputerName;
        testCaseResult.ResolutionState = var.ResolutionState;
        testCaseResult.FailureType = var.FailureType;
        testCaseResult.AutomatedTestName = var.AutomatedTestName;
        testCaseResult.AutomatedTestStorage = var.AutomatedTestStorage;
        testCaseResult.AutomatedTestType = var.AutomatedTestType;
        testCaseResult.AutomatedTestTypeId = var.AutomatedTestTypeId;
        testCaseResult.AutomatedTestId = var.AutomatedTestId;
        testCaseResult.TestCaseTitle = var.TestCaseTitle;
        testCaseResult.Area = var.Area;
        testCaseResult.Owner = var.Owner;
        testCaseResult.RunBy = var.RunBy;
        testCaseResult.Outcome = var.Outcome;
        testCaseResult.ErrorMessage = var.ErrorMessage;
        testCaseResult.Comment = var.Comment;
        testCaseResult.StackTrace = var.StackTrace;
        testCaseResult.CustomFields = var.CustomFields;
        if (!string.IsNullOrEmpty(var.DurationInMs))
        {
          double result = 0.0;
          if (!double.TryParse(var.DurationInMs, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result < 0.0)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DurationInMsError));
          testCaseResult.DurationInMs = result;
        }
        else
          testCaseResult.DurationInMs = 0.0;
        if (!string.IsNullOrEmpty(var.StartedDate))
        {
          DateTime result;
          if (!DateTime.TryParse(var.StartedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
            throw new InvalidPropertyException("StartedDate", ServerResources.InvalidPropertyMessage);
          testCaseResult.StartedDate = result;
        }
        if (!string.IsNullOrEmpty(var.CompletedDate))
        {
          DateTime result;
          if (!DateTime.TryParse(var.CompletedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
            throw new InvalidPropertyException("CompletedDate", ServerResources.InvalidPropertyMessage);
          testCaseResult.CompletedDate = result;
        }
        if (var.AssociatedWorkItems != null)
          testCaseResult.AssociatedBugs = ((IEnumerable<int>) var.AssociatedWorkItems).Select<int, ShallowReference>((Func<int, ShallowReference>) (w => new ShallowReference(Convert.ToString(w)))).ToList<ShallowReference>();
        if (!string.IsNullOrEmpty(var.TestCasePriority))
        {
          byte result;
          if (!byte.TryParse(var.TestCasePriority, out result))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "TestCasePriority"));
          testCaseResult.Priority = (int) result;
        }
        else
          testCaseResult.Priority = (int) byte.MaxValue;
        testCaseResultList.Add(testCaseResult);
      }
      return testCaseResultList.ToArray();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] ConvertResultUpdateModelToTestResults(
      TestCaseResultUpdateModel[] resultUpdateModel)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      HashSet<int> intSet = new HashSet<int>();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resultUpdateModel, nameof (resultUpdateModel), "Test Results");
      foreach (TestCaseResultUpdateModel var in resultUpdateModel)
      {
        ArgumentUtility.CheckForNull<TestCaseResultUpdateModel>(var, "resultModel", "Test Results");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testCaseResult.State = var.State;
        testCaseResult.ComputerName = var.ComputerName;
        testCaseResult.ResolutionState = var.ResolutionState;
        testCaseResult.FailureType = var.FailureType;
        testCaseResult.AutomatedTestTypeId = var.AutomatedTestTypeId;
        testCaseResult.Owner = var.Owner;
        testCaseResult.RunBy = var.RunBy;
        testCaseResult.Outcome = var.Outcome;
        testCaseResult.ErrorMessage = var.ErrorMessage;
        testCaseResult.Comment = var.Comment;
        testCaseResult.StackTrace = var.StackTrace;
        testCaseResult.CustomFields = var.CustomFields;
        int result1;
        if (var.TestResult != null && !string.IsNullOrEmpty(var.TestResult.Id) && int.TryParse(var.TestResult.Id, out result1))
        {
          testCaseResult.Id = result1;
          if (intSet.Contains(result1))
            throw new InvalidPropertyException("resultModel", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateIdsExceptionMessage, (object) result1));
          intSet.Add(testCaseResult.Id);
        }
        if (!string.IsNullOrEmpty(var.StartedDate))
        {
          DateTime result2;
          if (!DateTime.TryParse(var.StartedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result2))
            throw new InvalidPropertyException("StartedDate", ServerResources.InvalidPropertyMessage);
          testCaseResult.StartedDate = result2;
        }
        if (!string.IsNullOrEmpty(var.CompletedDate))
        {
          DateTime result3;
          if (!DateTime.TryParse(var.CompletedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result3))
            throw new InvalidPropertyException("CompletedDate", ServerResources.InvalidPropertyMessage);
          testCaseResult.CompletedDate = result3;
        }
        if (!string.IsNullOrEmpty(var.DurationInMs))
        {
          double result4 = 0.0;
          if (!double.TryParse(var.DurationInMs, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result4) || result4 < 0.0)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DurationInMsError));
          testCaseResult.DurationInMs = result4;
        }
        else if (testCaseResult.CompletedDate != new DateTime() && testCaseResult.StartedDate != new DateTime())
          testCaseResult.DurationInMs = (testCaseResult.CompletedDate - testCaseResult.StartedDate).TotalMilliseconds;
        if (var.AssociatedWorkItems != null)
          testCaseResult.AssociatedBugs = ((IEnumerable<int>) var.AssociatedWorkItems).Select<int, ShallowReference>((Func<int, ShallowReference>) (w => new ShallowReference(Convert.ToString(w)))).ToList<ShallowReference>();
        if (!string.IsNullOrEmpty(var.TestCasePriority))
        {
          byte result5;
          if (!byte.TryParse(var.TestCasePriority, out result5))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "TestCasePriority"));
          testCaseResult.Priority = (int) result5;
        }
        else
          testCaseResult.Priority = (int) byte.MaxValue;
        testCaseResultList.Add(testCaseResult);
      }
      return testCaseResultList.ToArray();
    }

    public static string GetFailureTypeName(TestFailureType failureType)
    {
      if (failureType == null)
        return (string) null;
      string failureTypeName;
      switch (failureType.Id)
      {
        case 0:
          failureTypeName = ServerResources.FailureTypeNone;
          break;
        case 1:
          failureTypeName = ServerResources.FailureTypeRegression;
          break;
        case 2:
          failureTypeName = ServerResources.FailureTypeNewIssue;
          break;
        case 3:
          failureTypeName = ServerResources.FailureTypeKnownIssue;
          break;
        case 4:
          failureTypeName = ServerResources.FailureTypeUnknown;
          break;
        default:
          failureTypeName = failureType.Name;
          break;
      }
      return failureTypeName;
    }

    protected string GetTestResultUrl(string projectName, int runId, int resultId)
    {
      if (!this.TestManagementRequestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestResult))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResult];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        action = "Results"
      });
    }

    protected string GetTestResultParameterUrl(
      string projectName,
      int runId,
      int resultId,
      int? iterationId,
      string parameterName)
    {
      if (!this.TestManagementRequestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestResultParameters))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultParameters];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        iterationId = iterationId,
        action = "ParameterResults",
        paramName = parameterName
      });
    }

    protected string GetTestResultAttachmentUrl(
      string projectName,
      int runId,
      int resultId,
      int? iterationId)
    {
      if (!this.TestManagementRequestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestResultAttachments))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultAttachments];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        iterationId = iterationId,
        action = "TestResultAttachments"
      });
    }

    protected string GetTestIterationDetailsUrl(
      string projectName,
      int runId,
      int resultId,
      int? iterationId)
    {
      if (!this.TestManagementRequestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestIterationDetails))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestIterationDetails];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        iterationId = iterationId
      });
    }

    protected string GetTestActionResultsUrl(
      string projectName,
      int runId,
      int resultId,
      int? iterationId,
      string actionPath)
    {
      if (!this.TestManagementRequestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestActionResult))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestActionResult];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        iterationId = iterationId,
        action = "ActionResults",
        actionPath = actionPath
      });
    }

    private IList<TestResultMetaData> GetTestResultsMetaDataWithS2SMerge(
      ProjectInfo projectInfo,
      List<int> testRefIdList,
      ResultMetaDataDetails detailsToInclude = ResultMetaDataDetails.None)
    {
      IList<TestResultMetaData> testResultsMetaData = (IList<TestResultMetaData>) new List<TestResultMetaData>();
      bool shouldIncludeFlakyDetails = detailsToInclude == ResultMetaDataDetails.FlakyIdentifiers;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        if (TCMServiceDataMigrationRestHelper.IsRunningInTfsAndMigrationInProgressOrFailed(this.RequestContext))
          return;
        testResultsMetaData = this.ConvertTestReferenceToMetaData(this.TestManagementResultService.GetTestResultsMetaData(this.TestManagementRequestContext, projectInfo, (IList<int>) testRefIdList, shouldIncludeFlakyDetails));
      }), this.RequestContext);
      IList<TestResultMetaData> metaDataList;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultsMetaData(this.RequestContext, projectInfo.Id, (IList<int>) testRefIdList, detailsToInclude, out metaDataList))
      {
        IMergeDataHelper mergeDataHelper = this.TestManagementRequestContext.MergeDataHelper;
        IList<TestResultMetaData> source = testResultsMetaData;
        List<TestResultMetaData> list1 = source != null ? source.ToList<TestResultMetaData>() : (List<TestResultMetaData>) null;
        List<TestResultMetaData> list2 = metaDataList != null ? metaDataList.ToList<TestResultMetaData>() : (List<TestResultMetaData>) null;
        testResultsMetaData = (IList<TestResultMetaData>) mergeDataHelper.MergeTestResultsMetaData(list1, list2);
      }
      return testResultsMetaData;
    }

    private IList<ShallowTestCaseResult> GetTestResultsByReleaseWithS2SMerge(
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int top,
      string continuationToken)
    {
      IList<ShallowTestCaseResult> testResults = (IList<ShallowTestCaseResult>) new List<ShallowTestCaseResult>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        int continuationTokenRunId;
        int continuationTokenResultId;
        ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
        testResults = this.TestManagementResultService.GetTestResultsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvId, publishContext, outcomes, continuationTokenRunId, continuationTokenResultId, top);
        testResults = TCMServiceDataMigrationRestHelper.FilterTfsShallowResultsBelowThresholdFromTCM(this.TestManagementRequestContext, testResults);
      }), this.RequestContext);
      IList<ShallowTestCaseResult> results;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultsByRelease(this.RequestContext, projectInfo.Id, releaseId, releaseEnvId, publishContext, outcomes, top, continuationToken, out results))
      {
        IMergeDataHelper mergeDataHelper = this.TestManagementRequestContext.MergeDataHelper;
        IList<ShallowTestCaseResult> source = testResults;
        List<ShallowTestCaseResult> list1 = source != null ? source.ToList<ShallowTestCaseResult>() : (List<ShallowTestCaseResult>) null;
        List<ShallowTestCaseResult> list2 = results != null ? results.ToList<ShallowTestCaseResult>() : (List<ShallowTestCaseResult>) null;
        int top1 = top;
        testResults = (IList<ShallowTestCaseResult>) mergeDataHelper.MergeTestResultReferences(list1, list2, top1);
      }
      return testResults;
    }

    private IList<ShallowTestCaseResult> GetTestResultsByBuildWithS2SMerge(
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes,
      int top,
      string continuationToken)
    {
      IList<ShallowTestCaseResult> testResults = (IList<ShallowTestCaseResult>) new List<ShallowTestCaseResult>();
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        int continuationTokenRunId;
        int continuationTokenResultId;
        ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
        testResults = this.TestManagementResultService.GetTestResultsByBuild(this.TestManagementRequestContext, projectInfo, buildId, publishContext, outcomes, continuationTokenRunId, continuationTokenResultId, top);
        testResults = TCMServiceDataMigrationRestHelper.FilterTfsShallowResultsBelowThresholdFromTCM(this.TestManagementRequestContext, testResults);
      }), this.RequestContext);
      IList<ShallowTestCaseResult> results;
      if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultsByBuild(this.RequestContext, projectInfo.Id, buildId, publishContext, outcomes, top, continuationToken, out results))
      {
        IMergeDataHelper mergeDataHelper = this.TestManagementRequestContext.MergeDataHelper;
        IList<ShallowTestCaseResult> source = testResults;
        List<ShallowTestCaseResult> list1 = source != null ? source.ToList<ShallowTestCaseResult>() : (List<ShallowTestCaseResult>) null;
        List<ShallowTestCaseResult> list2 = results != null ? results.ToList<ShallowTestCaseResult>() : (List<ShallowTestCaseResult>) null;
        int top1 = top;
        testResults = (IList<ShallowTestCaseResult>) mergeDataHelper.MergeTestResultReferences(list1, list2, top1);
      }
      return testResults;
    }

    private TestResultsDetails GetTestResultDetailsForReleaseWithS2SMerge(
      TeamProjectReference projectReference,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      TestResultsDetails resultDetails;
      this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultDetailsForRelease(this.RequestContext, projectReference.Id, releaseId, releaseEnvId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress, out resultDetails);
      TestResultsDetails resultDetailsFromCurrentService = (TestResultsDetails) null;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultDetailsFromCurrentService = this.GetResultDetailsFromCurrentServiceForRelease(projectReference, releaseId, releaseEnvId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress)), this.RequestContext);
      return this.TestManagementRequestContext.MergeDataHelper.MergeTestResultDetails(resultDetailsFromCurrentService, resultDetails);
    }

    private TestResultsDetails GetResultDetailsFromCurrentServiceForRelease(
      TeamProjectReference projectReference,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      bool isRerunOnPassedFilter;
      Dictionary<string, Tuple<string, List<string>>> odataQueryFilter = ParsingHelper.ParseODataQueryFilter(filter, out isRerunOnPassedFilter);
      Dictionary<string, string> odataOrderBy = ParsingHelper.ParseODataOrderBy(orderBy);
      ITeamFoundationTestManagementResultService managementResultService = this.TestManagementResultService;
      TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
      TeamProjectReference projectReference1 = projectReference;
      int releaseId1 = releaseId;
      int releaseEnvId1 = releaseEnvId;
      string sourceWorkflow1 = sourceWorkflow;
      List<string> groupByFields = new List<string>();
      groupByFields.Add(groupBy);
      Dictionary<string, Tuple<string, List<string>>> filterValues = odataQueryFilter;
      Dictionary<string, string> orderBy1 = odataOrderBy;
      int num1 = isRerunOnPassedFilter ? 1 : 0;
      int num2 = shouldIncludeResults ? 1 : 0;
      int num3 = queryRunSummaryForInProgress ? 1 : 0;
      return managementResultService.GetAggregatedTestResultDetailsForRelease(managementRequestContext, projectReference1, releaseId1, releaseEnvId1, sourceWorkflow1, groupByFields, filterValues, orderBy1, num1 != 0, num2 != 0, num3 != 0);
    }

    private TestResultsDetails GetTestResultDetailsForBuildWithS2SMerge(
      TeamProjectReference projectReference,
      int buildId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      TestResultsDetails resultDetails;
      this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultDetailsForBuild(this.RequestContext, projectReference.Id, buildId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress, out resultDetails);
      TestResultsDetails resultDetailsFromCurrentService = (TestResultsDetails) null;
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultDetailsFromCurrentService = this.GetResultDetailsFromCurrentServiceForBuild(projectReference, buildId, sourceWorkflow, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress)), this.RequestContext);
      return this.TestManagementRequestContext.MergeDataHelper.MergeTestResultDetails(resultDetailsFromCurrentService, resultDetails);
    }

    private TestResultsDetails GetResultDetailsFromCurrentServiceForBuild(
      TeamProjectReference projectReference,
      int buildId,
      string sourceWorkflow,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress)
    {
      bool isRerunOnPassedFilter;
      Dictionary<string, Tuple<string, List<string>>> odataQueryFilter = ParsingHelper.ParseODataQueryFilter(filter, out isRerunOnPassedFilter);
      Dictionary<string, string> odataOrderBy = ParsingHelper.ParseODataOrderBy(orderBy);
      ITeamFoundationTestManagementResultService managementResultService = this.TestManagementResultService;
      TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
      TeamProjectReference projectReference1 = projectReference;
      int buildId1 = buildId;
      string sourceWorkflow1 = sourceWorkflow;
      List<string> groupByFields = new List<string>();
      groupByFields.Add(groupBy);
      Dictionary<string, Tuple<string, List<string>>> filterValues = odataQueryFilter;
      Dictionary<string, string> orderBy1 = odataOrderBy;
      int num1 = isRerunOnPassedFilter ? 1 : 0;
      int num2 = shouldIncludeResults ? 1 : 0;
      int num3 = queryRunSummaryForInProgress ? 1 : 0;
      return managementResultService.GetAggregatedTestResultDetailsForBuild(managementRequestContext, projectReference1, buildId1, sourceWorkflow1, groupByFields, filterValues, orderBy1, num1 != 0, num2 != 0, num3 != 0);
    }

    private Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByQueryLocal(
      TeamProjectReference projectReference,
      QueryModel queryModel,
      bool includeResultDetails,
      bool includeIterationDetails,
      int skip,
      int top)
    {
      List<TestActionResult> testActionResults = new List<TestActionResult>();
      List<TestResultParameter> resultParameters = new List<TestResultParameter>();
      List<TestResultAttachment> resultAttachments = new List<TestResultAttachment>();
      List<TestCaseResultIdentifier> excessIds = new List<TestCaseResultIdentifier>();
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<TestCaseResultIdentifier, TestResultArtifacts> dictionary = new Dictionary<TestCaseResultIdentifier, TestResultArtifacts>();
      TimeZoneInfo utc = TimeZoneInfo.Utc;
      string name = projectReference.Name;
      List<TestCaseResult> source = this.TestManagementResultService.QueryTestResults(this.TestManagementRequestContext, new ResultsStoreQuery()
      {
        QueryText = queryModel.Query,
        TeamProjectName = name,
        TimeZone = utc.ToSerializedString()
      }, out excessIds);
      if (source != null && source.Any<TestCaseResult>())
      {
        List<TestCaseResult> list = source.Skip<TestCaseResult>(skip).Take<TestCaseResult>(top).ToList<TestCaseResult>();
        TeamProjectTestArtifacts projectTestArtifacts = this.TestManagementResultService.GetTeamProjectTestArtifacts(this.TestManagementRequestContext, projectReference, false);
        Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults1 = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, list);
        Dictionary<string, ShallowReference> forTestCaseResults2 = this.TestManagementResultService.GetAreaPathUriMappingForTestCaseResults(this.TestManagementRequestContext, list);
        Dictionary<int, ResultsHelper.TestRunDetails> mappingFromTestResults = this.GetTestRunDetailsMappingFromTestResults(projectReference, list);
        if (includeIterationDetails)
        {
          foreach (TestResultArtifacts testResultsArtifact in (IEnumerable<TestResultArtifacts>) this.TestManagementResultService.FetchTestResultsArtifacts(this.TestManagementRequestContext, list, name))
            dictionary[testResultsArtifact.TestCaseResult.Id] = testResultsArtifact;
        }
        List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList = new List<Tuple<TestCaseResultIdentifier, int, IdAndRev>>();
        foreach (TestCaseResult testCaseResult1 in list)
        {
          TestCaseResult testCaseResult2;
          if (includeIterationDetails && dictionary.ContainsKey(testCaseResult1.Id))
          {
            testCaseResultDataContracts.Add(testCaseResult1.Id, this.ConvertTestCaseResultToDataContract(mappingFromTestResults, projectTestArtifacts, dictionary[testCaseResult1.Id].TestCaseResult, includeIterationDetails, false, dictionary[testCaseResult1.Id].ActionResults, dictionary[testCaseResult1.Id].ParameterResults, dictionary[testCaseResult1.Id].AttachmentResults, includeResultDetails, forTestCaseResults1, forTestCaseResults2, projectReference));
            testCaseResult2 = dictionary[testCaseResult1.Id].TestCaseResult;
          }
          else
          {
            testCaseResultDataContracts.Add(testCaseResult1.Id, this.ConvertTestCaseResultToDataContract(mappingFromTestResults, projectTestArtifacts, testCaseResult1, false, false, testActionResults, resultParameters, resultAttachments, includeResultDetails, forTestCaseResults1, forTestCaseResults2, projectReference));
            testCaseResult2 = testCaseResult1;
          }
          this.PrepareListOfPointsForEachPlan(testResultTupleList, testCaseResult2);
        }
        this.AddPointDetailsToDataContract(testCaseResultDataContracts, testResultTupleList, projectReference.Name);
        if (includeResultDetails)
        {
          Dictionary<TestCaseResultIdentifier, List<ShallowReference>> associatedWorkItems = this.GetAssociatedWorkItems(list, this.TestManagementRequestContext, projectReference.Name);
          if (associatedWorkItems.Any<KeyValuePair<TestCaseResultIdentifier, List<ShallowReference>>>())
          {
            foreach (TestCaseResultIdentifier key in associatedWorkItems.Keys)
              testCaseResultDataContracts[key].AssociatedBugs = associatedWorkItems[key];
          }
        }
      }
      return testCaseResultDataContracts;
    }

    private IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetLocalTestResults(
      GuidAndString projectId,
      TestResultsQuery query)
    {
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList1 = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> localTestResults;
      if (query.Results != null && query.Results.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() && query.ResultsFilter != null)
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> source = query.Results.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (r => !string.IsNullOrEmpty(r.AutomatedTestName)));
        if (source == null || !source.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
          throw new InvalidPropertyException("TestResultsQuery", ServerResources.ResultQueryInvalid);
        localTestResults = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.TestCaseIdentityBasedQuery") ? (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.GetTestResultsByAutomatedTestName(projectId.GuidId.ToString(), query.ResultsFilter.TestResultsContext, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>()) : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      }
      else if (query.Results != null && query.Results.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.Fields, "Fields", "Test Results");
        List<TestCaseResultIdentifier> resultIds = new List<TestCaseResultIdentifier>();
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) query.Results)
        {
          ArgumentUtility.CheckForNull<ShallowReference>(result.TestRun, "TestRun");
          resultIds.Add(new TestCaseResultIdentifier(Convert.ToInt32(result.TestRun.Id), result.Id));
        }
        localTestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.GetTestResultsByIds(projectId.GuidId.ToString(), resultIds, query.Fields.ToList<string>());
      }
      else if (query.ResultsFilter != null && query.ResultsFilter.TestPointIds != null && query.ResultsFilter.TestPointIds.Any<int>())
      {
        ArgumentUtility.CheckForNonPositiveInt(query.ResultsFilter.TestPlanId, "ResultsFilter.TestPlanId");
        localTestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.GetTestResultsByPointIds(projectId.GuidId.ToString(), query.ResultsFilter.TestPlanId, query.ResultsFilter.TestPointIds.ToList<int>());
      }
      else if (query.ResultsFilter != null && query.ResultsFilter.TestCaseId != 0)
      {
        ArgumentUtility.CheckForNonPositiveInt(query.ResultsFilter.TestCaseId, "query.Resultsfilter.TestCaseId");
        TeamProjectReference projectReferece = this.GetProjectReference(projectId.GuidId.ToString());
        List<TestCaseResult> testCaseResultList2 = TestCaseResult.QueryTestResultsForTestCaseId(this.TestManagementRequestContext, projectReferece.Name, query.ResultsFilter.TestCaseId, this.GetPageSizeForQuery());
        Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> tfsIdentityCache = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, testCaseResultList2);
        localTestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultList2.Select<TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result => this.ConvertTestCaseResultToDataContract(new Dictionary<int, ResultsHelper.TestRunDetails>(), new TeamProjectTestArtifacts(), result, false, false, new List<TestActionResult>(), new List<TestResultParameter>(), new List<TestResultAttachment>(), true, tfsIdentityCache, new Dictionary<string, ShallowReference>(), projectReferece))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      }
      else
        localTestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.QueryTestResultTrend(projectId, query.ResultsFilter);
      return localTestResults;
    }

    private IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByQueryWithS2SMerge(
      GuidAndString projectId,
      TestResultsQuery query,
      string actualAutomatedTestCaseName)
    {
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> localResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      TestResultsQuery results;
      this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.RequestContext, projectId.GuidId, query, out results);
      TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
      {
        localResults = this.GetLocalTestResults(projectId, query);
        localResults = TCMServiceDataMigrationRestHelper.FilterTfsResultsBelowThresholdFromTCM(this.TestManagementRequestContext, localResults);
      }), this.RequestContext);
      TestResultsQuery testResultsQuery = query;
      IMergeDataHelper mergeDataHelper = this.TestManagementRequestContext.MergeDataHelper;
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> source = localResults;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> list1 = source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> list2 = results != null ? results.Results.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = mergeDataHelper.MergeTestResults(list1, list2);
      testResultsQuery.Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultList;
      return query.Results;
    }

    private void UpdateFlakyIdentifiers(
      TestCaseReferenceRecord testCaseReferenceRecord,
      TestResultMetaData metaData)
    {
      if (testCaseReferenceRecord.TestFlakyBranchName == null || testCaseReferenceRecord.TestFlakyBranchName.Count == 0)
        return;
      metaData.FlakyIdentifiers = (IList<TestFlakyIdentifier>) new List<TestFlakyIdentifier>();
      foreach (string str in testCaseReferenceRecord.TestFlakyBranchName)
        metaData.FlakyIdentifiers.Add(new TestFlakyIdentifier()
        {
          BranchName = str,
          IsFlaky = true
        });
    }

    private void ValidateAddTestResultToRun<T>(string teamProjectId, int runId, T[] results)
    {
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(this.RequestContext, runId);
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "AddTestResultsToTestRun", 50, true))
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(teamProjectId, nameof (teamProjectId), "Test Results");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) results, nameof (results), "Test Results");
        if (runId <= 0)
          throw new InvalidPropertyException(nameof (runId), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) nameof (runId), (object) runId));
        foreach (T result in results)
          ArgumentUtility.CheckGenericForNull((object) result, "result", "Test Results");
      }
    }

    private void NotifyClientsAboutTestRunChange(Guid projectId, TestRun testRun)
    {
      try
      {
        if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.SignalRReportingRuns"))
          return;
        if (ResultsHelper.IsBuildContext(testRun))
        {
          this.RequestContext.GetService<IBuildTestHubDispatcher>().HandleTestRunStatsChange(this.TestManagementRequestContext, projectId, testRun.BuildReference.BuildId);
        }
        else
        {
          if (!ResultsHelper.IsReleaseContext(testRun))
            return;
          this.RequestContext.GetService<IReleaseTestHubDispatcher>().HandleTestRunStatsChange(this.TestManagementRequestContext, projectId, testRun.ReleaseReference.ReleaseId, testRun.ReleaseReference.ReleaseEnvId);
        }
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.TraceException("RestLayer", ex);
      }
    }

    private static bool IsReleaseContext(TestRun testRun)
    {
      if (!testRun.IsAutomated || testRun.State != (byte) 2)
        return false;
      ReleaseReference releaseReference = testRun.ReleaseReference;
      return releaseReference != null && releaseReference.ReleaseId > 0;
    }

    private static bool IsBuildContext(TestRun testRun)
    {
      if (testRun.IsAutomated && testRun.State == (byte) 2)
      {
        ReleaseReference releaseReference = testRun.ReleaseReference;
        if ((releaseReference != null ? (releaseReference.ReleaseId == 0 ? 1 : 0) : 0) != 0)
        {
          BuildConfiguration buildReference = testRun.BuildReference;
          return buildReference != null && buildReference.BuildId > 0;
        }
      }
      return false;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> ConvertTestCaseResults(
      List<TestCaseResult> testResults,
      TeamProjectReference project)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      foreach (TestCaseResult testResult1 in testResults)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult2 = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testResult2.StartedDate = testResult1.DateStarted;
        testResult2.CompletedDate = testResult1.DateCompleted;
        testResult2.DurationInMs = (double) testResult1.Duration;
        testResult2.AutomatedTestName = testResult1.AutomatedTestName;
        testResult2.TestRun = new ShallowReference()
        {
          Id = testResult1.TestRunId.ToString()
        };
        if (testResult1.Outcome != (byte) 1)
          testResult2.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) testResult1.Outcome);
        testResult2.Project = new ShallowReference()
        {
          Id = project.Id.ToString()
        };
        this.SecureTestResultWebApiObject(testResult2);
        testCaseResultList.Add(testResult2);
      }
      return testCaseResultList;
    }

    private void SecureTestResultDetailsWebApiObject(
      TestResultsDetails testResultsDetails,
      Guid projectId)
    {
      this.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      if (testResultsDetails == null)
        return;
      testResultsDetails.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      });
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByAutomatedTestName(
      string projectId,
      TestResultsContext testResultContext,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testcaseResults)
    {
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.GetTestResultsByAutomatedTestName", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        string sourceWorkflow = SourceWorkflow.ContinuousIntegration;
        int releaseId = 0;
        int releaseEnvironmentId = 0;
        int buildId = 0;
        if (testResultContext.ContextType == TestResultsContextType.Release)
        {
          if (testResultContext.Release == null)
            throw new InvalidPropertyException("Results.GetTestResultsByQuery", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) "testResultContext.Release", (object) null));
          sourceWorkflow = SourceWorkflow.ContinuousDelivery;
          releaseId = testResultContext.Release.Id;
          ArgumentUtility.CheckGreaterThanZero((float) releaseId, "releaseId", "Test Results");
          releaseEnvironmentId = testResultContext.Release.EnvironmentId;
        }
        else if (testResultContext.ContextType == TestResultsContextType.Build)
        {
          if (testResultContext.Build == null)
            throw new InvalidPropertyException("Results.GetTestResultsByQuery", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) "testResultContext.Build", (object) null));
          buildId = testResultContext.Build.Id;
          ArgumentUtility.CheckGreaterThanZero((float) buildId, "buildId", "Test Results");
        }
        if (testcaseResults.Count<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() > 1000)
          throw new InvalidPropertyException("query.Results", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidArgument, (object) "query.Results.Count", (object) testcaseResults.Count<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>()));
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        List<TestCaseReference> list = testcaseResults.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseReference>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseReference>) (tc => new TestCaseReference()
        {
          AutomatedTestName = tc.AutomatedTestName
        })).ToList<TestCaseReference>();
        return this.ConvertTestCaseResults(this.TestManagementResultService.GetTestResultsByFQDN(this.TestManagementRequestContext, projectReference, buildId, releaseId, releaseEnvironmentId, sourceWorkflow, list), projectReference);
      }), 1015052, "TestResultsInsights");
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByIds(
      string projectId,
      List<TestCaseResultIdentifier> resultIds,
      List<string> fields)
    {
      this.ValidateAndSetMaxPageSizeForRunArtifacts(new int?(resultIds.Count), true, "resultsCount");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.GetTestResultByIds", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        List<TestCaseResult> caseResultsByIds = this.TestManagementResultService.GetTestCaseResultsByIds(this.TestManagementRequestContext, projectReference, resultIds, fields);
        TeamProjectTestArtifacts projectTestArtifacts = this.TestManagementResultService.GetTeamProjectTestArtifacts(this.TestManagementRequestContext, projectReference, false);
        Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, caseResultsByIds);
        Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList = new List<Tuple<TestCaseResultIdentifier, int, IdAndRev>>();
        foreach (TestCaseResult testCaseResult in caseResultsByIds)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult dataContract = this.ConvertTestCaseResultToDataContract(new Dictionary<int, ResultsHelper.TestRunDetails>(), projectTestArtifacts, testCaseResult, false, false, (List<TestActionResult>) null, (List<TestResultParameter>) null, (List<TestResultAttachment>) null, true, forTestCaseResults, new Dictionary<string, ShallowReference>(), projectReference);
          testCaseResultDataContracts.Add(testCaseResult.Id, dataContract);
          this.PrepareListOfPointsForEachPlan(testResultTupleList, testCaseResult);
        }
        this.AddPointDetailsToDataContract(testCaseResultDataContracts, testResultTupleList, projectReference.Name);
        return testCaseResultDataContracts.Values.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      }), 1015052, "TestResultsInsights");
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByPointIds(
      string projectId,
      int planId,
      List<int> pointIds)
    {
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts = new Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      List<TestCaseResult> resultsByPointIds = this.TestManagementResultService.GetTestCaseResultsByPointIds(this.TestManagementRequestContext, projectReference, planId, pointIds);
      TeamProjectTestArtifacts projectTestArtifacts = this.TestManagementResultService.GetTeamProjectTestArtifacts(this.TestManagementRequestContext, projectReference, false);
      Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> forTestCaseResults = this.TestManagementResultService.GetTfsIdentityGuidToIdentityMappingForTestCaseResults(this.TestManagementRequestContext, resultsByPointIds);
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList = new List<Tuple<TestCaseResultIdentifier, int, IdAndRev>>();
      foreach (TestCaseResult testCaseResult in resultsByPointIds)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult dataContract = this.ConvertTestCaseResultToDataContract(new Dictionary<int, ResultsHelper.TestRunDetails>(), projectTestArtifacts, testCaseResult, false, false, (List<TestActionResult>) null, (List<TestResultParameter>) null, (List<TestResultAttachment>) null, true, forTestCaseResults, new Dictionary<string, ShallowReference>(), projectReference);
        if (dataContract.BuildReference == null && !string.IsNullOrEmpty(testCaseResult.BuildNumber))
          dataContract.BuildReference = new BuildReference()
          {
            Number = testCaseResult.BuildNumber
          };
        testCaseResultDataContracts.Add(testCaseResult.Id, dataContract);
        this.PrepareListOfPointsForEachPlan(testResultTupleList, testCaseResult);
      }
      this.AddPointDetailsToDataContract(testCaseResultDataContracts, testResultTupleList, projectReference.Name);
      return testCaseResultDataContracts.Values.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrend(
      GuidAndString projectId,
      ResultsFilter filter)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.QueryTestResultTrend", 50, true))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(filter.AutomatedTestName, "AutomatedTestName", "Test Results");
        ArgumentUtility.CheckForNull<TestResultsContext>(filter.TestResultsContext, "TestResultsContext", "Test Results");
        if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
          ArgumentUtility.CheckForNull<BuildReference>(filter.TestResultsContext.Build, "Build", "Test Results");
        if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
          ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference>(filter.TestResultsContext.Release, "Release", "Test Results");
        if (filter.ResultsCount < 0)
          throw new InvalidPropertyException("resultsCount", ServerResources.QueryParameterOutOfRange);
        if (filter.TrendDays > 15 || filter.TrendDays < 0)
          throw new InvalidPropertyException("trendDays", ServerResources.QueryParameterOutOfRange);
        if (filter.TrendDays == 0)
          filter.TrendDays = 15;
        if (filter.ResultsCount == 0)
          filter.ResultsCount = 10;
        DateTime t1 = DateTime.MinValue;
        if (filter.MaxCompleteDate.HasValue)
        {
          t1 = DateTime.SpecifyKind(filter.MaxCompleteDate.Value, DateTimeKind.Utc);
          if (DateTime.Compare(t1, this.MinSqlTime) < 0 || DateTime.Compare(t1, this.MaxSqlTime) > 0)
            throw new InvalidPropertyException("maxCompleteDate", ServerResources.QueryParameterOutOfRange);
        }
        filter.MaxCompleteDate = new DateTime?(t1);
        return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>("ResultsHelper.QueryTestResultTrend", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.RequestContext.GetService<ITeamFoundationTestReportService>().QueryTestResultTrendReport(this.RequestContext, projectId, filter)), 1015052, "TestManagement");
      }
    }

    private bool IsGroupByFieldValid(string groupBy) => string.Equals(ValidTestResultGroupByFields.Container, groupBy, StringComparison.OrdinalIgnoreCase) || string.Equals(ValidTestResultGroupByFields.TestRun, groupBy, StringComparison.OrdinalIgnoreCase) || string.Equals(ValidTestResultGroupByFields.Priority, groupBy, StringComparison.OrdinalIgnoreCase) || string.Equals(ValidTestResultGroupByFields.TestSuite, groupBy, StringComparison.OrdinalIgnoreCase) || string.Equals(ValidTestResultGroupByFields.Requirement, groupBy, StringComparison.OrdinalIgnoreCase) || string.Equals(ValidTestResultGroupByFields.Owner, groupBy, StringComparison.OrdinalIgnoreCase);

    private Dictionary<int, ResultsHelper.TestRunDetails> GetTestRunDetailsMappingFromTestResults(
      TeamProjectReference projectReference,
      List<TestCaseResult> results)
    {
      Dictionary<int, ResultsHelper.TestRunDetails> mappingFromTestResults = new Dictionary<int, ResultsHelper.TestRunDetails>();
      IEnumerable<int> ints = results.Select<TestCaseResult, int>((Func<TestCaseResult, int>) (r => r.TestRunId)).Distinct<int>();
      ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectReference);
      foreach (int num in ints)
      {
        if (!mappingFromTestResults.ContainsKey(num))
        {
          TestRun testRunById = this.TestManagementRunService.GetTestRunById(this.TestManagementRequestContext, num, projectReference);
          ShallowReference runRepresentation = this.GetRunRepresentation(projectReference.Name, testRunById);
          ShallowReference buildRepresentation = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, testRunById.BuildReference);
          Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseReference = this.GetReleaseReference(testRunById.ReleaseReference);
          mappingFromTestResults.Add(num, new ResultsHelper.TestRunDetails()
          {
            TestRun = runRepresentation,
            Build = buildRepresentation,
            Project = projectRepresentation,
            ReleaseReference = releaseReference
          });
        }
      }
      return mappingFromTestResults;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetShallowTestResultList(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool includeDetail = false)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> shallowTestResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      for (int index = 0; index < ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results).Count<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(); ++index)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testCaseResult.Id = results[index].Id;
        testCaseResult.Project = new ShallowReference();
        testCaseResult.TestRun = results[index].TestRun;
        testCaseResult.LastUpdatedBy = new IdentityRef();
        testCaseResult.LastUpdatedDate = results[index].LastUpdatedDate;
        testCaseResult.Url = string.Empty;
        if (includeDetail)
        {
          testCaseResult.SubResults = this.GetShallowSubResults(results[index].SubResults);
          testCaseResult.AutomatedTestStorage = results[index].AutomatedTestStorage;
          testCaseResult.AutomatedTestName = results[index].AutomatedTestName;
          testCaseResult.Outcome = results[index].Outcome;
        }
        shallowTestResultList.Add(testCaseResult);
      }
      return shallowTestResultList;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult ConvertTestCaseResultToDataContractTestAssistant(
      TeamProjectTestArtifacts teamProjectTestArtifacts,
      TestCaseResult testCaseResult,
      bool includeIterationDetails,
      bool includeSubResultDetails,
      List<TestActionResult> testActionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments,
      bool includeResultDetails,
      TeamProjectReference projectReference)
    {
      return this.ConvertTestCaseResultToDataContract(new Dictionary<int, ResultsHelper.TestRunDetails>(), teamProjectTestArtifacts, testCaseResult, includeIterationDetails, includeSubResultDetails, testActionResults, resultParameters, resultAttachments, includeResultDetails, new Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>>(), new Dictionary<string, ShallowReference>(), projectReference);
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult ConvertTestCaseResultToDataContract(
      Dictionary<int, ResultsHelper.TestRunDetails> testRunDetails,
      TeamProjectTestArtifacts teamProjectTestArtifacts,
      TestCaseResult testCaseResult,
      bool includeIterationDetails,
      bool includeSubResultDetails,
      List<TestActionResult> testActionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments,
      bool includeResultDetails,
      Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> tfsIdentityGuidToIdentityMapping,
      Dictionary<string, ShallowReference> areaMapping,
      TeamProjectReference projectReference,
      bool testSessionProperties = false)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultsHelper.ConvertTestCaseResultToDataContract2"))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        dataContract.Id = testCaseResult.Id.TestResultId;
        dataContract.TestCaseReferenceId = testCaseResult.TestCaseReferenceId;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult1 = dataContract;
        int num;
        ShallowReference shallowReference1;
        if (!testRunDetails.ContainsKey(testCaseResult.TestRunId))
        {
          ShallowReference shallowReference2 = new ShallowReference();
          num = testCaseResult.TestRunId;
          shallowReference2.Id = num.ToString();
          shallowReference2.Name = testCaseResult.TestRunTitle;
          shallowReference1 = shallowReference2;
        }
        else
          shallowReference1 = testRunDetails[testCaseResult.TestRunId].TestRun;
        testCaseResult1.TestRun = shallowReference1;
        if (testCaseResult.State != (byte) 0)
          dataContract.State = Enum.GetName(typeof (TestResultState), (object) testCaseResult.State);
        if (testCaseResult.Outcome != (byte) 1)
          dataContract.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) testCaseResult.Outcome);
        dataContract.DurationInMs = new TimeSpan(testCaseResult.Duration).TotalMilliseconds;
        if (testCaseResult.TestCaseId > 0 || !string.IsNullOrEmpty(testCaseResult.TestCaseTitle))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult2 = dataContract;
          ShallowReference shallowReference3 = new ShallowReference();
          string str;
          if (testCaseResult.TestCaseId <= 0)
          {
            str = (string) null;
          }
          else
          {
            num = testCaseResult.TestCaseId;
            str = num.ToString();
          }
          shallowReference3.Id = str;
          shallowReference3.Name = testCaseResult.TestCaseTitle;
          testCaseResult2.TestCase = shallowReference3;
        }
        dataContract.TestCaseRevision = testCaseResult.TestCaseRevision;
        if (!string.IsNullOrEmpty(testCaseResult.AutomatedTestName))
          dataContract.AutomatedTestName = testCaseResult.AutomatedTestName;
        Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> tuple1 = !(testCaseResult.Owner != Guid.Empty) || !tfsIdentityGuidToIdentityMapping.Keys.Contains<Guid>(testCaseResult.Owner) ? (Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) null : tfsIdentityGuidToIdentityMapping[testCaseResult.Owner];
        if (tuple1 != null)
          dataContract.Owner = tuple1.Item2;
        else if (!string.IsNullOrEmpty(testCaseResult.OwnerName))
          dataContract.Owner = IdentityHelper.ToIdentityRef(this.RequestContext, string.Empty, testCaseResult.OwnerName);
        Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> tuple2 = !(testCaseResult.RunBy != Guid.Empty) || !tfsIdentityGuidToIdentityMapping.Keys.Contains<Guid>(testCaseResult.RunBy) ? (Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) null : tfsIdentityGuidToIdentityMapping[testCaseResult.RunBy];
        if (tuple2 != null)
          dataContract.RunBy = tuple2.Item2;
        else if (!string.IsNullOrEmpty(testCaseResult.RunByName))
          dataContract.RunBy = IdentityHelper.ToIdentityRef(this.RequestContext, string.Empty, testCaseResult.RunByName);
        if (testCaseResult.TestPointId > int.MinValue)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult3 = dataContract;
          ShallowReference shallowReference4 = new ShallowReference();
          num = testCaseResult.TestPointId;
          shallowReference4.Id = num.ToString();
          testCaseResult3.TestPoint = shallowReference4;
        }
        if (testCaseResult.TestPlanId > 0)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult4 = dataContract;
          ShallowReference shallowReference5 = new ShallowReference();
          num = testCaseResult.TestPlanId;
          shallowReference5.Id = num.ToString();
          testCaseResult4.TestPlan = shallowReference5;
        }
        if (testCaseResult.ConfigurationId > 0 || !string.IsNullOrEmpty(testCaseResult.ConfigurationName))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult5 = dataContract;
          ShallowReference shallowReference6 = new ShallowReference();
          num = testCaseResult.ConfigurationId;
          shallowReference6.Id = num.ToString();
          shallowReference6.Name = testCaseResult.ConfigurationName;
          testCaseResult5.Configuration = shallowReference6;
        }
        if (includeResultDetails)
        {
          dataContract.TestCaseTitle = testCaseResult.TestCaseTitle;
          dataContract.Revision = testCaseResult.Revision;
          dataContract.ResetCount = testCaseResult.ResetCount;
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult6 = dataContract;
          ShallowReference shallowReference7;
          if (!testRunDetails.ContainsKey(testCaseResult.TestRunId))
            shallowReference7 = new ShallowReference()
            {
              Id = projectReference.Id.ToString()
            };
          else
            shallowReference7 = testRunDetails[testCaseResult.TestRunId].Project;
          testCaseResult6.Project = shallowReference7;
          dataContract.Build = testRunDetails.ContainsKey(testCaseResult.TestRunId) ? testRunDetails[testCaseResult.TestRunId].Build : (ShallowReference) null;
          dataContract.ReleaseReference = testRunDetails.ContainsKey(testCaseResult.TestRunId) ? testRunDetails[testCaseResult.TestRunId].ReleaseReference : (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null;
          if (testCaseResult.BuildReference != null)
            dataContract.BuildReference = new BuildReference()
            {
              Id = testCaseResult.BuildReference.BuildId,
              DefinitionId = testCaseResult.BuildReference.BuildDefinitionId,
              Number = string.IsNullOrEmpty(testCaseResult.BuildReference.BuildNumber) ? testCaseResult.BuildNumber : testCaseResult.BuildReference.BuildNumber
            };
          if (testCaseResult.ReleaseReference != null)
            dataContract.ReleaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
            {
              Id = testCaseResult.ReleaseReference.ReleaseId,
              EnvironmentId = testCaseResult.ReleaseReference.ReleaseEnvId,
              DefinitionId = testCaseResult.ReleaseReference.ReleaseDefId,
              EnvironmentDefinitionId = testCaseResult.ReleaseReference.ReleaseEnvDefId
            };
          dataContract.ResolutionStateId = testCaseResult.ResolutionStateId;
          TestResolutionState testResolutionState = teamProjectTestArtifacts.ResolutionStates?.Find((Predicate<TestResolutionState>) (x => x.Id == testCaseResult.ResolutionStateId));
          dataContract.ResolutionState = testResolutionState?.Name;
          TestFailureType failureType = teamProjectTestArtifacts.FailureTypes?.Find((Predicate<TestFailureType>) (x => x.Id == (int) testCaseResult.FailureType));
          dataContract.FailureType = !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.UseLocalizedValueForFailureTypeName") ? failureType?.Name : ResultsHelper.GetFailureTypeName(failureType);
          dataContract.Priority = (int) testCaseResult.Priority;
          dataContract.CreatedDate = testCaseResult.CreationDate;
          dataContract.StartedDate = testCaseResult.DateStarted;
          dataContract.CompletedDate = testCaseResult.DateCompleted;
          dataContract.LastUpdatedDate = testCaseResult.LastUpdated;
          if (!string.IsNullOrEmpty(testCaseResult.ComputerName))
            dataContract.ComputerName = testCaseResult.ComputerName;
          if (!string.IsNullOrEmpty(testCaseResult.Comment))
            dataContract.Comment = testCaseResult.Comment;
          if (!string.IsNullOrEmpty(testCaseResult.ErrorMessage))
            dataContract.ErrorMessage = testCaseResult.ErrorMessage;
          if (testCaseResult.StackTrace != null)
            dataContract.StackTrace = testCaseResult.StackTrace.Value as string;
          if (testCaseResult.FailingSince != null)
            dataContract.FailingSince = new FailingSince()
            {
              Build = testCaseResult.FailingSince.Build,
              Release = testCaseResult.FailingSince.Release,
              Date = testCaseResult.FailingSince.Date
            };
          if (testCaseResult.CustomFields != null)
          {
            dataContract.CustomFields = new List<CustomTestField>();
            foreach (TestExtensionField customField in testCaseResult.CustomFields)
              dataContract.CustomFields.Add(new CustomTestField()
              {
                FieldName = customField.Field.Name,
                Value = customField.Value
              });
          }
          dataContract.AfnStripId = testCaseResult.AfnStripId;
          Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> tuple3 = !(testCaseResult.LastUpdatedBy != Guid.Empty) || !tfsIdentityGuidToIdentityMapping.Keys.Contains<Guid>(testCaseResult.LastUpdatedBy) ? (Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) null : tfsIdentityGuidToIdentityMapping[testCaseResult.LastUpdatedBy];
          if (tuple3 != null)
            dataContract.LastUpdatedBy = tuple3.Item2;
          else if (!string.IsNullOrEmpty(testCaseResult.LastUpdatedByName))
            dataContract.LastUpdatedBy = IdentityHelper.ToIdentityRef(this.RequestContext, string.Empty, testCaseResult.LastUpdatedByName);
          if (!string.IsNullOrEmpty(testCaseResult.AutomatedTestStorage))
            dataContract.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
          if (!string.IsNullOrEmpty(testCaseResult.AutomatedTestType))
            dataContract.AutomatedTestType = testCaseResult.AutomatedTestType;
          if (!string.IsNullOrEmpty(testCaseResult.AutomatedTestTypeId))
            dataContract.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
          if (!string.IsNullOrEmpty(testCaseResult.AutomatedTestId))
            dataContract.AutomatedTestId = testCaseResult.AutomatedTestId;
          if (!string.IsNullOrEmpty(testCaseResult.AreaUri) && areaMapping.ContainsKey(testCaseResult.AreaUri))
            dataContract.Area = areaMapping[testCaseResult.AreaUri];
          if (testCaseResult.ResultGroupType != ResultGroupType.None)
            dataContract.ResultGroupType = testCaseResult.ResultGroupType;
        }
        if (includeIterationDetails)
          dataContract.IterationDetails = this.CreateIterationDetails(testCaseResult.Id.TestRunId, testCaseResult.Id.TestResultId, testRunDetails[testCaseResult.TestRunId].Project.Name, testActionResults, resultParameters, resultAttachments);
        if (testSessionProperties)
        {
          if (testCaseResult.ExecutionNumber > 0)
            dataContract.ExecutionNumber = testCaseResult.ExecutionNumber;
          if (testCaseResult.Attempt > 0)
            dataContract.Attempt = testCaseResult.Attempt;
          if (!string.IsNullOrEmpty(testCaseResult.Locale))
            dataContract.Locale = testCaseResult.Locale;
          if (!string.IsNullOrEmpty(testCaseResult.BuildType))
            dataContract.BuildType = testCaseResult.BuildType;
          if (testCaseResult.TestPhase != (byte) 0)
            dataContract.TestPhase = Enum.GetName(typeof (TestPhase), (object) testCaseResult.TestPhase);
          if (testCaseResult.TopologyId > 0)
            dataContract.TopologyId = testCaseResult.TopologyId;
          if (!string.IsNullOrEmpty(testCaseResult.ExceptionType))
            dataContract.ExceptionType = testCaseResult.ExceptionType;
          if (!string.IsNullOrEmpty(testCaseResult.BucketUid))
            dataContract.BucketUid = testCaseResult.BucketUid;
          if (!string.IsNullOrEmpty(testCaseResult.BucketingSystem))
            dataContract.BucketingSystem = testCaseResult.BucketingSystem;
          dataContract.IsSystemIssue = testCaseResult.IsSystemIssue;
          if (testCaseResult.Links != null)
          {
            dataContract.Links = new List<Link<ResultLinkType>>();
            foreach (Link<ResultLinkType> link in testCaseResult.Links)
              dataContract.Links.Add(new Link<ResultLinkType>()
              {
                Url = link.Url,
                DisplayName = link.DisplayName,
                OperationType = link.OperationType,
                Type = link.Type
              });
          }
          if (testCaseResult.Dimensions != null)
          {
            dataContract.Dimensions = new List<TestResultDimension>();
            foreach (TestResultDimension dimension in testCaseResult.Dimensions)
              dataContract.Dimensions.Add(new TestResultDimension()
              {
                Name = dimension.Name,
                Value = dimension.Value
              });
          }
        }
        int result;
        if (testRunDetails.ContainsKey(testCaseResult.TestRunId) && int.TryParse(testRunDetails[testCaseResult.TestRunId].TestRun.Id, out result))
          dataContract.Url = this.GetTestResultUrl(testRunDetails[testCaseResult.TestRunId].Project.Name, result, testCaseResult.Id.TestResultId);
        if (includeSubResultDetails)
          this.PopulateSubResults(this.TestManagementRequestContext, projectReference, dataContract, testCaseResult.Id.TestRunId, testCaseResult.Id.TestResultId);
        this.SecureTestResultWebApiObject(dataContract);
        return dataContract;
      }
    }

    private TestIterationDetailsModel CreateIterationDetailsModel(
      int runId,
      int resultId,
      string projectName,
      List<TestActionResult> testActionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments,
      int iterationId,
      bool includeActionResults)
    {
      TestIterationDetailsModel iterationDetails = new TestIterationDetailsModel();
      this.PopulateIterationDetails(iterationDetails, this.GetDefaultActionResult(testActionResults, iterationId));
      if (includeActionResults)
      {
        iterationDetails.ActionResults = this.CreateTestActionResultModels(runId, resultId, projectName, testActionResults, new int?(iterationId));
        iterationDetails.Parameters = this.CreateTestResultParameterModels(runId, resultId, projectName, resultParameters, new int?(iterationId));
        iterationDetails.Attachments = this.CreateTestResultAttachmentModels(runId, resultId, projectName, resultAttachments, new int?(iterationId));
      }
      iterationDetails.Url = this.GetTestIterationDetailsUrl(projectName, runId, resultId, new int?(iterationId));
      return iterationDetails;
    }

    private List<TestIterationDetailsModel> CreateIterationDetails(
      int runId,
      int resultId,
      string projectName,
      List<TestActionResult> testActionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments,
      bool includeActionResults = true)
    {
      List<TestIterationDetailsModel> iterationDetails = new List<TestIterationDetailsModel>();
      int iterationCount = this.TestManagementResultService.GetIterationCount(testActionResults);
      for (int iterationId = 1; iterationId <= iterationCount; ++iterationId)
        iterationDetails.Add(this.CreateIterationDetailsModel(runId, resultId, projectName, testActionResults, resultParameters, resultAttachments, iterationId, includeActionResults));
      return iterationDetails;
    }

    private void PopulateIterationDetails(
      TestIterationDetailsModel iterationDetails,
      TestActionResult actionResult)
    {
      iterationDetails.Id = actionResult.IterationId;
      iterationDetails.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) actionResult.Outcome);
      if (!string.IsNullOrEmpty(actionResult.Comment))
        iterationDetails.Comment = actionResult.Comment;
      iterationDetails.ErrorMessage = actionResult.ErrorMessage;
      iterationDetails.StartedDate = actionResult.DateStarted;
      iterationDetails.CompletedDate = actionResult.DateCompleted;
      iterationDetails.DurationInMs = new TimeSpan(actionResult.Duration).TotalMilliseconds;
    }

    private TestActionResult GetDefaultActionResult(
      List<TestActionResult> actionResults,
      int iterationId)
    {
      IEnumerable<TestActionResult> source = actionResults.Where<TestActionResult>((Func<TestActionResult, bool>) (actionResult => string.IsNullOrEmpty(actionResult.ActionPath) && actionResult.IterationId == iterationId));
      return source.Count<TestActionResult>() != 0 ? source.First<TestActionResult>() : throw new TestObjectNotFoundException(string.Format(ServerResources.IterationResultNotFound, (object) iterationId), ObjectTypes.TestResult);
    }

    private List<TestResultParameterModel> CreateTestResultParameterModels(
      int runId,
      int resultId,
      string projectName,
      List<TestResultParameter> resultParameters,
      int? iterationId = null,
      string parameterName = "")
    {
      List<TestResultParameterModel> resultParameterModels = new List<TestResultParameterModel>();
      foreach (TestResultParameter resultParameter in resultParameters)
      {
        if (iterationId.HasValue)
        {
          int iterationId1 = resultParameter.IterationId;
          int? nullable = iterationId;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (!(iterationId1 == valueOrDefault & nullable.HasValue))
            continue;
        }
        if ((string.IsNullOrEmpty(parameterName) || string.Equals(parameterName, resultParameter.ParameterName, StringComparison.OrdinalIgnoreCase)) && !string.IsNullOrEmpty(resultParameter.ActionPath))
        {
          TestResultParameterModel resultParameterModel = new TestResultParameterModel(resultParameter.IterationId, resultParameter.ActionPath, resultParameter.ParameterName, resultParameter.DataType, resultParameter.Expected);
          if (!this.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
            resultParameterModel.Url = this.GetTestResultParameterUrl(projectName, runId, resultId, iterationId, resultParameterModel.ParameterName);
          resultParameterModel.StepIdentifier = this.GetStepIdentifierFromActionPath(resultParameter.ActionPath);
          resultParameterModels.Add(resultParameterModel);
        }
      }
      return resultParameterModels;
    }

    private List<TestCaseResultAttachmentModel> CreateTestResultAttachmentModels(
      int runId,
      int resultId,
      string projectName,
      List<TestResultAttachment> resultAttachments,
      int? iterationId = null)
    {
      List<TestCaseResultAttachmentModel> attachmentModels = new List<TestCaseResultAttachmentModel>();
      foreach (TestResultAttachment resultAttachment in resultAttachments)
      {
        if (iterationId.HasValue)
        {
          int iterationId1 = resultAttachment.IterationId;
          int? nullable = iterationId;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (!(iterationId1 == valueOrDefault & nullable.HasValue))
            continue;
        }
        attachmentModels.Add(new TestCaseResultAttachmentModel()
        {
          Name = resultAttachment.FileName,
          Id = resultAttachment.Id,
          Size = resultAttachment.Length,
          IterationId = resultAttachment.IterationId,
          ActionPath = resultAttachment.ActionPath,
          Url = this.GetTestResultAttachmentUrl(projectName, runId, resultId, iterationId)
        });
      }
      return attachmentModels;
    }

    private List<TestActionResultModel> CreateTestActionResultModels(
      int runId,
      int resultId,
      string projectName,
      List<TestActionResult> testActionResults,
      int? iterationId = null,
      string actionPath = "")
    {
      List<TestActionResultModel> actionResultModels = new List<TestActionResultModel>();
      foreach (TestActionResult testActionResult in testActionResults)
      {
        if (iterationId.HasValue)
        {
          int iterationId1 = testActionResult.IterationId;
          int? nullable = iterationId;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (!(iterationId1 == valueOrDefault & nullable.HasValue))
            continue;
        }
        if ((string.IsNullOrEmpty(actionPath) || string.Equals(actionPath, testActionResult.ActionPath, StringComparison.Ordinal)) && !string.IsNullOrEmpty(testActionResult.ActionPath))
        {
          string actionPath1 = testActionResult.ActionPath;
          string iterationId2 = testActionResult.IterationId.ToString();
          SharedStepModel sharedStepModel = new SharedStepModel();
          sharedStepModel.Id = testActionResult.SetId;
          sharedStepModel.Revision = testActionResult.SetRevision;
          string name = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) testActionResult.Outcome);
          DateTime dateTime = testActionResult.DateStarted;
          string startedDate = dateTime.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo);
          dateTime = testActionResult.DateCompleted;
          string completedDate = dateTime.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo);
          string duration = new TimeSpan(testActionResult.Duration).TotalMilliseconds.ToString();
          string errorMessage = testActionResult.ErrorMessage;
          string comment = testActionResult.Comment;
          TestActionResultModel actionResultModel = new TestActionResultModel(actionPath1, iterationId2, sharedStepModel, name, startedDate, completedDate, duration, errorMessage, comment);
          if (!this.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
            actionResultModel.Url = this.GetTestActionResultsUrl(projectName, runId, resultId, iterationId, testActionResult.ActionPath);
          actionResultModel.StepIdentifier = this.GetStepIdentifierFromActionPath(actionResultModel.ActionPath);
          actionResultModels.Add(actionResultModel);
        }
      }
      return actionResultModels;
    }

    private string GetStepIdentifierFromActionPath(string actionPath)
    {
      StringBuilder stringBuilder = new StringBuilder();
      IEnumerable<string> strings = Enumerable.Empty<string>();
      if (!string.IsNullOrEmpty(actionPath))
        strings = ResultsHelper.SplitActionPathIntoChunks(actionPath, 8);
      try
      {
        foreach (string s in strings)
        {
          stringBuilder.Append(int.Parse(s, NumberStyles.HexNumber));
          stringBuilder.Append(';');
        }
        if (stringBuilder.Length > 0)
          stringBuilder.Remove(stringBuilder.Length - 1, 1);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case FormatException _:
          case OverflowException _:
            stringBuilder.Clear();
            break;
          default:
            throw;
        }
      }
      return stringBuilder.ToString();
    }

    private static IEnumerable<string> SplitActionPathIntoChunks(string str, int maxChunkSize)
    {
      for (int i = 0; i < str.Length; i += maxChunkSize)
        yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
    }

    private TestRun FetchTestRun(int runId, string projectName)
    {
      TestRun[] array = TestRun.Query(this.TestManagementRequestContext, runId, Guid.Empty, string.Empty, projectName).ToArray();
      return array.Length != 0 ? ((IEnumerable<TestRun>) array).First<TestRun>() : throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, runId, ObjectTypes.TestRun);
    }

    private Dictionary<TestCaseResultIdentifier, List<ShallowReference>> GetAssociatedWorkItems(
      List<TestCaseResult> results,
      TestManagementRequestContext tcmRequestContext,
      string projectName,
      bool getAllWorkitems = false)
    {
      using (PerfManager.Measure(tcmRequestContext.RequestContext, "RestLayer", "ResultsHelper.GetAssociatedWorkItems"))
        return this.ExecuteAction<Dictionary<TestCaseResultIdentifier, List<ShallowReference>>>("ResultsHelper.GetAssociatedWorkItems", (Func<Dictionary<TestCaseResultIdentifier, List<ShallowReference>>>) (() =>
        {
          TeamProjectReference projectReference = this.GetProjectReference(projectName);
          WorkItemTypeCategory itemTypeCategory = tcmRequestContext.WorkItemServiceHelper.GetWorkItemTypeCategory(projectReference.Id, WitCategoryRefName.Bug);
          IEnumerable<string> workItemTypeNamesForBugs = itemTypeCategory != null ? itemTypeCategory.WorkItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (wt => wt.Name)) : (IEnumerable<string>) null;
          Dictionary<TestCaseResultIdentifier, List<ShallowReference>> associatedWorkItems = new Dictionary<TestCaseResultIdentifier, List<ShallowReference>>();
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
          {
            Project = new ShallowReference()
            {
              Id = projectReference.Id.ToString()
            }
          };
          int count1 = 0;
          Dictionary<TestCaseResultIdentifier, List<int>> dictionary = new Dictionary<TestCaseResultIdentifier, List<int>>();
          List<int> workItemIds = new List<int>();
          List<ShallowReference> shallowReferenceList = new List<ShallowReference>();
          int count2;
          for (; results.Count > count1; count1 += count2)
          {
            dictionary.Clear();
            workItemIds.Clear();
            count2 = Math.Min(results.Count - count1, 100);
            dictionary = this.TestManagementLinkedWorkItemService.GetWorkItemIdsAssociatedToTestResults(results.Skip<TestCaseResult>(count1).Take<TestCaseResult>(count2).ToArray<TestCaseResult>(), tcmRequestContext, projectName);
            workItemIds = dictionary.Values.SelectMany<List<int>, int>((Func<List<int>, IEnumerable<int>>) (id => (IEnumerable<int>) id)).ToList<int>();
            Dictionary<int, ShallowReference> itemsInfoFromIds = this.GetLinkedWorkItemsInfoFromIds(tcmRequestContext, projectReference.Id, workItemIds, workItemTypeNamesForBugs, getAllWorkitems);
            if (itemsInfoFromIds != null && itemsInfoFromIds.Any<KeyValuePair<int, ShallowReference>>())
            {
              foreach (TestCaseResultIdentifier key1 in dictionary.Keys)
              {
                associatedWorkItems.Add(key1, new List<ShallowReference>());
                foreach (int key2 in dictionary[key1])
                {
                  if (itemsInfoFromIds.ContainsKey(key2))
                  {
                    itemsInfoFromIds[key2].InitializeSecureObject((ISecuredObject) testCaseResult);
                    associatedWorkItems[key1].Add(itemsInfoFromIds[key2]);
                  }
                }
              }
            }
          }
          return associatedWorkItems;
        }), 1015052, "TestResultsInsights");
    }

    private void SecureTestCaseShallowReferences(
      ProjectInfo projectInfo,
      IList<ShallowTestCaseResult> testResults)
    {
      if (testResults == null)
        return;
      testResults.ForEach<ShallowTestCaseResult>((Action<ShallowTestCaseResult>) (r => r.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectInfo.Id.ToString()
        }
      })));
    }

    private void SecureTestResultsMetaData(
      ProjectInfo projectInfo,
      IList<TestResultMetaData> testResults)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectInfo.Id.ToString()
        }
      };
      if (testResults == null)
        return;
      testResults.ForEach<TestResultMetaData>((Action<TestResultMetaData>) (r => r.InitializeSecureObject((ISecuredObject) securedObject)));
    }

    public List<int> GetListOfRefIds(
      IVssRequestContext requestContext,
      List<string> ids,
      string fieldName)
    {
      List<int> intList = new List<int>();
      ids.RemoveAll((Predicate<string>) (item => item == null));
      List<string> source = ids;
      try
      {
        return source != null && source.Count<string>() > 0 ? source.ToList<string>().ConvertAll<int>((Converter<string, int>) (pointId => int.Parse(pointId))) : throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) fieldName)));
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) fieldName)));
          default:
            throw;
        }
      }
    }

    private List<TestSubResult> GetShallowSubResults(List<TestSubResult> subResults)
    {
      if (subResults == null || !subResults.Any<TestSubResult>())
        return (List<TestSubResult>) null;
      List<TestSubResult> shallowSubResults = new List<TestSubResult>();
      foreach (TestSubResult subResult in subResults)
        shallowSubResults.Add(new TestSubResult()
        {
          Id = subResult.Id,
          ParentId = subResult.ParentId,
          SubResults = this.GetShallowSubResults(subResult.SubResults)
        });
      return shallowSubResults;
    }

    private void PopulateSubResults(
      TestManagementRequestContext tcmRequestContext,
      TeamProjectReference projectReference,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult,
      int runId,
      int testCaseResultId)
    {
      if (!tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHierarchicalResult"))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.HierarchicalResultNotSupported));
      testCaseResult.SubResults = new List<TestSubResult>();
      this.FetchJsonAndParse(tcmRequestContext, projectReference, testCaseResult, runId, testCaseResultId);
    }

    private TestLogReference GetTestLogReferenceToDownloadAttachment(int runId, string fileName = "") => new TestLogReference()
    {
      RunId = runId,
      Scope = TestLogScope.Run,
      FilePath = TestResultsConstants.TestSubResultV2LogStoreFilePath + fileName,
      Type = TestLogType.System
    };

    private void FetchJsonAndParse(
      TestManagementRequestContext tcmRequestContext,
      TeamProjectReference projectReference,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult,
      int runId,
      int testCaseResultId)
    {
      List<TestResultAttachment> list = this.GetSubResultsAttachmentsList(tcmRequestContext, projectReference, runId, testCaseResultId).Where<TestResultAttachment>((Func<TestResultAttachment, bool>) (t => string.Equals(t.AttachmentType, TestResultsConstants.TestSubResultJsonAttachmentType, StringComparison.OrdinalIgnoreCase))).ToList<TestResultAttachment>();
      if (list == null || !list.Any<TestResultAttachment>())
        return;
      foreach (TestResultAttachment resultAttachment in list)
      {
        string attachmentFileName = resultAttachment.FileName;
        CompressionType attachmentCompressionType = CompressionType.None;
        Stream input = (Stream) new MemoryStream();
        try
        {
          if (resultAttachment.FileId != 0)
          {
            input = this.TestManagementAttachmentsService.GetTestAttachment(tcmRequestContext, projectReference, runId, testCaseResultId, resultAttachment.Id, out attachmentFileName, out attachmentCompressionType);
          }
          else
          {
            ITestLogStoreService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
            ProjectInfo projectFromGuid = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectReference.Id);
            TestManagementRequestContext tcmRequestContext1 = tcmRequestContext;
            ProjectInfo projectInfo = projectFromGuid;
            TestLogReference downloadAttachment = this.GetTestLogReferenceToDownloadAttachment(runId, resultAttachment.FileName);
            Stream targetStream = input;
            service.DownloadToStream(tcmRequestContext1, projectInfo, downloadAttachment, targetStream);
          }
          if (input != null)
          {
            if (string.Equals(Path.GetExtension(attachmentFileName), ".json", StringComparison.OrdinalIgnoreCase))
            {
              if (attachmentCompressionType == CompressionType.None)
              {
                input.Seek(0L, SeekOrigin.Begin);
                string str = Encoding.UTF8.GetString(ResultsHelper.ReadStream(input));
                if (!string.IsNullOrWhiteSpace(str))
                {
                  List<TestSubResult> collection = (List<TestSubResult>) null;
                  try
                  {
                    if (str.Count<char>() < 104857600)
                    {
                      if (string.Equals(attachmentFileName, TestResultsConstants.TestSubResultFileName, StringComparison.OrdinalIgnoreCase))
                      {
                        collection = TestJsonUtilities.Deserialize<List<TestSubResult>>(str);
                        CustomerIntelligenceData cid = new CustomerIntelligenceData();
                        cid.Add("SubResultOldJson", 1.0);
                        this.TelemetryLogger.PublishData(tcmRequestContext.RequestContext, "GetResultByIdOldJson", cid);
                      }
                      else
                      {
                        if (!string.Equals(attachmentFileName, TestResultsConstants.TestSubResultV2FileName, StringComparison.OrdinalIgnoreCase))
                        {
                          if (resultAttachment.FileId != 0)
                            goto label_23;
                        }
                        Dictionary<int, List<TestSubResult>> dictionary = TestJsonUtilities.Deserialize<Dictionary<int, List<TestSubResult>>>(str);
                        if (dictionary != null)
                        {
                          if (dictionary.ContainsKey(testCaseResultId))
                            dictionary.TryGetValue(testCaseResultId, out collection);
                        }
                      }
                    }
                    else
                      this.RequestContext.Trace(1015078, TraceLevel.Error, "TestManagement", "RestLayer", "SubResult parsing file too big for runid = {0} , Resultid = {1}", (object) runId, (object) testCaseResultId);
                  }
                  catch (Exception ex)
                  {
                    this.RequestContext.Trace(1015078, TraceLevel.Error, "TestManagement", "RestLayer", "SubResult parsing Failed for runid = {0} , Resultid = {1}, ex = {2}", (object) runId, (object) testCaseResultId, (object) ex.ToString());
                  }
label_23:
                  if (collection != null)
                    testCaseResult.SubResults.AddRange((IEnumerable<TestSubResult>) collection);
                }
              }
            }
          }
        }
        finally
        {
          input.Dispose();
        }
      }
    }

    private List<TestResultAttachment> GetSubResultsAttachmentsList(
      TestManagementRequestContext tcmRequestContext,
      TeamProjectReference projectReference,
      int runId,
      int testCaseResultId)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      List<TestResultAttachment> resultsAttachmentsList;
      if (this.ShouldFetchFromLogStoreAttachmentTable(tcmRequestContext, runId))
      {
        tcmRequestContext.RequestContext.Trace(1015052, TraceLevel.Info, "TestManagement", "RestLayer", "ResultsHelper.GetSubResultsAttachmentsList querying from logstore table with runId = {0}", (object) runId);
        resultsAttachmentsList = tcmRequestContext.RequestContext.GetService<ITeamFoundationLogStoreAttachmentService>().GetLogStoreAttachments(tcmRequestContext, projectReference, runId, testCaseResultId, 0, true);
      }
      else
      {
        tcmRequestContext.RequestContext.Trace(1015052, TraceLevel.Info, "TestManagement", "RestLayer", "ResultsHelper.GetSubResultsAttachmentsList querying from attachment table with runId = {0}", (object) runId);
        resultsAttachmentsList = this.TestManagementAttachmentsService.GetTestAttachments(tcmRequestContext, projectReference, runId, testCaseResultId);
      }
      return resultsAttachmentsList;
    }

    private bool ShouldFetchFromLogStoreAttachmentTable(
      TestManagementRequestContext tcmRequestContext,
      int runId)
    {
      if (!tcmRequestContext.IsFeatureEnabled("TestManagement.Server.StoreSubResultJSONToLogStore") || !tcmRequestContext.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForSubResult"))
        return false;
      return tcmRequestContext.IsFeatureEnabled("TestManagement.Server.DTUpdatedWithNewQueryLogStoreAttachmentProc") || ResultsHelper.GetFromRegistryLogic(tcmRequestContext, runId);
    }

    private static bool GetFromRegistryLogic(
      TestManagementRequestContext tcmRequestContext,
      int runId)
    {
      int num = tcmRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreSubResultsAttachmentsRunIdWatermark", 0);
      tcmRequestContext.RequestContext.Trace(1015052, TraceLevel.Info, "TestManagement", "RestLayer", "ResultsHelper.ShouldFetchFromLogStoreAttachmentTable runId = {0} and runIdThreshold = {1}", (object) runId, (object) num);
      return num != 0 && runId >= num;
    }

    private static byte[] ReadStream(Stream input)
    {
      using (MemoryStream destination = new MemoryStream())
      {
        input.CopyTo((Stream) destination);
        return destination.ToArray();
      }
    }

    private TestConfigurationHelper TestConfigurationHelper
    {
      get => this.m_testConfigurationHelper ?? (this.m_testConfigurationHelper = new TestConfigurationHelper());
      set => this.m_testConfigurationHelper = value;
    }

    internal RunsHelper RunsHelper
    {
      get
      {
        if (this.m_runsHelper == null)
          this.m_runsHelper = new RunsHelper(this.TestManagementRequestContext);
        return this.m_runsHelper;
      }
    }

    private class TestRunDetails
    {
      internal ShallowReference TestRun { get; set; }

      internal ShallowReference Build { get; set; }

      internal ShallowReference Project { get; set; }

      internal Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference ReleaseReference { get; set; }
    }
  }
}
