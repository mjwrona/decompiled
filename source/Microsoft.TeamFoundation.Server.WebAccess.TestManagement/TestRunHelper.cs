// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.Settings;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestRunHelper : TestHelperBase
  {
    private TestPlansHelper testPlanHelper;
    private Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper buildServiceHelper;
    private TestRunHubUserSettings m_testRunUserSettings;

    internal TestRunHelper(
      TestManagerRequestContext testContext,
      IAccountUserSettingsHive userSettings)
      : base(testContext)
    {
      this.m_testRunUserSettings = new TestRunHubUserSettings(testContext, userSettings);
    }

    internal TestRunHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
    }

    public bool isOnPremiseDeployment() => this.TestContext.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    internal TestRunAndResultsModel Create(
      TestRunModel runModel,
      TestResultCreationRequestModel[] testResultCreationRequestModels,
      int firstResultId)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "CreateTestRun", 10, true))
      {
        Tuple<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[]> andTestCaseResults = this.CreateTestRunAndTestCaseResults(runModel, testResultCreationRequestModels, firstResultId);
        return this.CreateModelFromRunAndResults(andTestCaseResults.Item1, andTestCaseResults.Item2);
      }
    }

    internal TestRunResultsAndActionResults CreateTestRunForTestPoints(
      int testPlanId,
      int[] testPointIds)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (CreateTestRunForTestPoints), 10, true))
      {
        List<TestPointModel> testPointModelList = this.TestPlanHelper.FetchTestPointsForWitCardWebRunner(testPlanId, testPointIds);
        if (testPointModelList == null || testPointModelList.Count == 0)
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(TestManagementServerResources.TestPointNotFoundInKanbanBoardError, ObjectTypes.TestPoint);
        TestPlanModel testPlanModel = this.TestPlanHelper.GetTestPlansById((IEnumerable<int>) new int[1]
        {
          testPlanId
        }).FirstOrDefault<TestPlanModel>();
        string str1 = (string) null;
        string str2 = (string) null;
        string str3 = (string) null;
        int buildDefinitionId = 0;
        if (testPlanModel != null)
        {
          str1 = testPlanModel.Iteration;
          str2 = testPlanModel.BuildUri;
          buildDefinitionId = testPlanModel.BuildDefinitionId;
        }
        TestPointModel testPointModel = testPointModelList.FirstOrDefault<TestPointModel>();
        if (testPointModel != null)
          str3 = string.Format(TestManagementResources.BulkMarkRunTitle, (object) testPointModel.SuiteName);
        TestRunModel runModel = new TestRunModel();
        runModel.Owner = this.TestContext.TfsRequestContext.GetUserId();
        runModel.Title = str3 ?? string.Empty;
        runModel.Iteration = str1 ?? this.TestContext.ProjectName;
        runModel.BuildUri = str2 ?? string.Empty;
        runModel.State = (byte) 2;
        runModel.IsAutomated = false;
        runModel.TestPlanId = testPlanId;
        Tuple<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[]> andTestCaseResults = this.CreateTestRunAndTestCaseResults(runModel, this.CreateTestResultCreationRequestModelFromTestPoints(testPointModelList), TestManagementConstants.c_firstTestCaseResultId, this.TestContext.ProjectName, buildDefinitionId);
        string itemTypeInCategory = this.TestContext.TfsRequestContext.GetService<IWitHelper>().GetDefaultWorkItemTypeInCategory((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, "Microsoft.BugCategory");
        TestRunResultsAndActionResults runForTestPoints = new TestRunResultsAndActionResults()
        {
          TestRun = new TestRunModel(andTestCaseResults.Item1),
          TestCaseResults = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) andTestCaseResults.Item2).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResultModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResultModel>) (testCaseResult => new TestCaseResultModel(testCaseResult))).ToList<TestCaseResultModel>(),
          BugCategoryTypeName = itemTypeInCategory
        };
        foreach (TestCaseResultModel testCaseResult in runForTestPoints.TestCaseResults)
          testCaseResult.UseTeamSettings = true;
        return runForTestPoints;
      }
    }

    internal TestRunResultsAndActionResults GetTestRunAndResultsForTestPoint(
      int testPlanId,
      int testPointId)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetTestRunAndResultsForTestPoint), 10, true))
      {
        TestRunResultsAndActionResults testRunAndResults = this.GetTestRunAndResults((this.TestPlanHelper.FetchTestPointsForWitCardWebRunner(testPlanId, new int[1]
        {
          testPointId
        }).FirstOrDefault<TestPointModel>() ?? throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(TestManagementServerResources.TestPointNotFoundInKanbanBoardError, ObjectTypes.TestPoint)).MostRecentRunId);
        testRunAndResults.BugCategoryTypeName = this.TestContext.TfsRequestContext.GetService<IWitHelper>().GetDefaultWorkItemTypeInCategory((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, "Microsoft.BugCategory");
        return testRunAndResults;
      }
    }

    internal TestRunResultsAndActionResults GetTestRunAndResults(int testRunId)
    {
      if (this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        return this.GetTestRunAndResultsFromTCMAsync(testRunId);
      TestRunResultsAndActionResults runResults;
      if (!this.TryGetTestRunAndResultsFromTCM(testRunId, out runResults))
        runResults = this.GetTestRunAndResultsInternal(testRunId);
      return runResults;
    }

    internal List<TestSession> GetTestSessionList(
      WebApiTeam team,
      int period,
      bool allSessions,
      bool includeAllProperties,
      List<int> sources,
      List<int> states)
    {
      return new SessionHelper((TestManagementRequestContext) this.TestContext.TestRequestContext).GetTestSessions(Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName).ToString(), team, period, allSessions, includeAllProperties, sources, states).ToList<TestSession>();
    }

    internal List<WorkItemTypeCategoryModel> GetWorkItemTypeCategoryModelList()
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory> list = this.TestContext.TfsRequestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(this.TestContext.TfsRequestContext, this.TestContext.ProjectName).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory>();
      List<WorkItemTypeCategoryModel> categoryModelList = new List<WorkItemTypeCategoryModel>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory itemTypeCategory in list)
      {
        List<WorkItemTypeReference> itemTypeReferenceList = new List<WorkItemTypeReference>();
        foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
          itemTypeReferenceList.Add(new WorkItemTypeReference(workItemTypeName));
        WorkItemTypeCategoryModel typeCategoryModel = new WorkItemTypeCategoryModel()
        {
          DefaultWorkItemType = new WorkItemTypeReference(itemTypeCategory.DefaultWorkItemTypeName),
          Name = itemTypeCategory.Name,
          ReferenceName = itemTypeCategory.ReferenceName,
          WorkItemTypes = itemTypeReferenceList
        };
        categoryModelList.Add(typeCategoryModel);
      }
      return categoryModelList;
    }

    internal ExploratorySessionSettingModel GetExploratorySessionUserSettings() => this.m_testRunUserSettings != null ? this.m_testRunUserSettings.GetExploratorySessionSettings() : (ExploratorySessionSettingModel) null;

    private List<TestResultLinkedBugsModel> GetAssociatedWorkItems(
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> resultIds)
    {
      List<TestResultLinkedBugsModel> source = new List<TestResultLinkedBugsModel>();
      this.TestContext.TfsRequestContext.GetService<TeamFoundationLinkingService>();
      this.GetLinkFilterValue();
      string[] strArray = new string[3]
      {
        "System.Id",
        "System.Title",
        "System.WorkItemType"
      };
      IEnumerable<string> workItemTypeNames = this.TestContext.TfsRequestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(this.TestContext.TfsRequestContext, this.TestContext.ProjectName, WitCategoryRefName.Bug).WorkItemTypeNames;
      for (int index = 0; index < resultIds.Count; ++index)
      {
        string urisForTestResult = this.GetArtiFactUrisForTestResult(resultIds[index].TestRunId, resultIds[index].TestResultId);
        List<int> list = this.TestContext.TfsRequestContext.GetService<IWorkItemArtifactUriQueryRemotableService>().QueryWorkItemsForArtifactUris(this.TestContext.TfsRequestContext, new ArtifactUriQuery()
        {
          ArtifactUris = (IEnumerable<string>) new string[1]
          {
            urisForTestResult
          }
        }).ArtifactUrisQueryResult.Values.SelectMany<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>, int>((Func<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>, IEnumerable<int>>) (wie => wie.Select<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>) (wi => wi.Id)))).ToList<int>();
        List<TestResultLinkedBugsModel> fromIdsFoTestResult = this.GetLinkedBugsInfoFromIdsFoTestResult(resultIds[index].TestResultId, list, workItemTypeNames);
        source.AddRange((IEnumerable<TestResultLinkedBugsModel>) fromIdsFoTestResult);
      }
      return source.OrderBy<TestResultLinkedBugsModel, int>((Func<TestResultLinkedBugsModel, int>) (linkedBug => linkedBug.BugId)).ToList<TestResultLinkedBugsModel>();
    }

    internal void Update(TestRunModel runModel)
    {
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        if (this.TryUpdateRunInTCM(runModel))
          return;
        this.UpdateInternal(runModel);
      }
      else
        this.UpdateRunInTCMAsync(runModel);
    }

    internal void Abort(TestRunModel runModel, List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results = null)
    {
      Guid projectId = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      if (this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("WebAccess.TestManagement.EnableTfsResultsCleanupOnAbort") && results == null)
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(projectId, runModel.TestRunId, new ResultDetails?(), new int?(), new int?(), (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result));
        results = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in testCaseResultList)
        {
          results.Add(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult()
          {
            TestPlanId = testCaseResult.TestPlan != null ? int.Parse(testCaseResult.TestPlan.Id) : 0,
            TestPointId = testCaseResult.TestPoint != null ? int.Parse(testCaseResult.TestPoint.Id) : 0
          });
          this.TestContext.TestRequestContext.RequestContext.TraceAlways(1015009, TraceLevel.Info, "TestManagement", "WebService", string.Format("Project id: {0}, Test plan id: {1}, Test point id: {2}, Run id: {3}, Result id: {4}", (object) projectId, (object) testCaseResult.TestPlan?.Id, (object) testCaseResult.TestPoint?.Id, (object) runModel.TestRunId, (object) testCaseResult.Id));
        }
      }
      RunUpdateModel runUpdateModel = new RunUpdateModel(state: "Aborted", deleteUnexecutedResults: new bool?(true));
      Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper resultsHelper = new Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) resultsHelper.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, runModel.TestRunId, Guid.Empty, (string) null, this.TestContext.ProjectName));
      List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> testRunList = Microsoft.TeamFoundation.TestManagement.Server.TestRun.FilterNotOfType(source != null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
      if (testRunList.Count != 1)
        return;
      Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun = runModel.UpdateFromModel(testRunList[0]);
      resultsHelper.AbortTestRun((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, runModel.TestRunId, testRun.Revision, 1);
      // ISSUE: explicit non-virtual call
      if (!this.TestContext.TestRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.TestContext.TfsRequestContext) || results == null || __nonvirtual (results.Count) <= 0)
        return;
      int testPlanId = results[0].TestPlanId;
      List<int> list = results.Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, int>) (res => res.TestPointId)).ToList<int>();
      TestResultsQuery testResultsQuery = new TestResultsQuery()
      {
        ResultsFilter = new ResultsFilter()
        {
          AutomatedTestName = string.Empty,
          TestPlanId = testPlanId,
          TestPointIds = (IList<int>) list
        }
      };
      TestResultsQuery fetchedResults = (TestResultsQuery) null;
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.TestContext.TfsRequestContext, projectId, testResultsQuery, out fetchedResults);
      else
        fetchedResults = TestManagementController.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => this.TestResultsHttpClient.GetTestResultsByQueryAsync(testResultsQuery, projectId, (object) null, new CancellationToken())?.Result));
      TestResultsQuery testResultsQuery1 = fetchedResults;
      int num1;
      if (testResultsQuery1 == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = testResultsQuery1.Results?.Count;
        int num2 = 0;
        num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
      }
      if (num1 != 0)
      {
        this.TestContext.TestRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.TestContext.TfsRequestContext, this.TestContext.ProjectName, fetchedResults.Results);
        list = list.Where<int>((Func<int, bool>) (pointId => !fetchedResults.Results.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (result => result != null && result.TestPoint != null && !string.IsNullOrEmpty(result.TestPoint.Id))).Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>) (result => int.Parse(result.TestPoint.Id))).ToList<int>().Contains(pointId))).ToList<int>();
      }
      if (list.Count <= 0)
        return;
      this.TestContext.TestRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeWithoutResult(this.TestContext.TfsRequestContext, this.TestContext.ProjectName, testPlanId, (IList<int>) list);
    }

    internal void End(TestRunModel runModel)
    {
      if (runModel == null)
        return;
      TestResultHelper testResultHelper = new TestResultHelper(this.TestContext);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> serverTestResults = (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null;
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        if (!testResultHelper.TryEndTestCaseResultsInTCM(this.TestContext, runModel, out serverTestResults))
          serverTestResults = testResultHelper.EndTestCaseResults(runModel, this.TestContext);
      }
      else
        serverTestResults = testResultHelper.EndTestCaseResultsInTCMAsync(this.TestContext, runModel);
      if (!this.IsAbortRequired(serverTestResults))
        return;
      this.Abort(runModel, serverTestResults);
    }

    internal static string GetErrorMessage(Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException e)
    {
      if (e.ObjectType == ObjectTypes.TestPoint)
        return TestManagementServerResources.TestPointNotFoundError;
      if (e.ObjectType == ObjectTypes.TestRun)
        return TestManagementResources.TestRunNotFoundError;
      if (e.ObjectType == ObjectTypes.TestResult)
        return TestManagementServerResources.TestCaseResultNotFoundError;
      return e.ObjectType == ObjectTypes.TestCase ? TestManagementServerResources.TestCaseNotFoundError : string.Empty;
    }

    private TestRunResultsAndActionResults GetTestRunAndResultsInternal(int testRunId)
    {
      try
      {
        List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> testRunList = Microsoft.TeamFoundation.TestManagement.Server.TestRun.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, testRunId, Guid.Empty, string.Empty, this.TestContext.ProjectName);
        Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun = testRunList.Count != 0 ? testRunList[0] : throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.TestContext.TfsRequestContext, testRunId, ObjectTypes.TestRun);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> actionResults;
        List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parameters;
        List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> attachments;
        List<TestCaseResultModel> list1 = Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.QueryByRun((TestManagementRequestContext) this.TestContext.TestRequestContext, testRunId, int.MaxValue, out List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> _, this.TestContext.ProjectName, true, out actionResults, out parameters, out attachments).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResultModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResultModel>) (testCaseResult => new TestCaseResultModel(testCaseResult))).ToList<TestCaseResultModel>();
        TestResultHelper.PopulateDataRowCountsInTestResults((IEnumerable<TestCaseResultModel>) list1, this.TestContext);
        List<TestActionResultModel> list2 = actionResults.Select<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, TestActionResultModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, TestActionResultModel>) (actionResult => new TestActionResultModel(actionResult))).ToList<TestActionResultModel>();
        List<TestResultParameterModel> list3 = parameters.Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, TestResultParameterModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, TestResultParameterModel>) (parameter => new TestResultParameterModel(parameter))).ToList<TestResultParameterModel>();
        List<TestAttachmentModel> list4 = attachments.Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, TestAttachmentModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, TestAttachmentModel>) (attachment => new TestAttachmentModel(attachment))).ToList<TestAttachmentModel>();
        List<TestResultLinkedBugsModel> associatedWorkItems = this.GetAssociatedWorkItems(list1.Select<TestCaseResultModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>((Func<TestCaseResultModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) (r => new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(r.TestRunId, r.TestResultId))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>());
        Microsoft.TeamFoundation.TestManagement.Server.TestSettings testSettings = Microsoft.TeamFoundation.TestManagement.Server.TestSettings.QueryById((TestManagementRequestContext) this.TestContext.TestRequestContext, testRun.PublicTestSettingsId, this.TestContext.ProjectName);
        TestRunModel testRunModel = new TestRunModel(testRun)
        {
          TestSettingsName = testSettings != null ? testSettings.Name : string.Empty
        };
        if (testRun.RunHasDtlEnvironment)
        {
          string str = testRun.DtlTestEnvironment != null ? testRun.DtlTestEnvironment.Url : string.Empty;
          int num = string.IsNullOrEmpty(str) ? -1 : str.LastIndexOf('/');
          if (num != -1)
            testRunModel.TestEnvironment = str.Substring(num + 1);
        }
        return new TestRunResultsAndActionResults()
        {
          TestRun = testRunModel,
          TestCaseResults = list1,
          TestActionResults = new TestActionResultDetailsModel(list2, list3, list4, associatedWorkItems)
        };
      }
      catch (Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException ex)
      {
        string errorMessage = TestRunHelper.GetErrorMessage(ex);
        if (!string.IsNullOrWhiteSpace(errorMessage))
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(errorMessage, ex.ObjectType);
        throw;
      }
    }

    private bool TryGetTestRunAndResultsFromTCM(
      int testRunId,
      out TestRunResultsAndActionResults runResults)
    {
      try
      {
        runResults = (TestRunResultsAndActionResults) null;
        Guid projectGuidFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun;
        if (!this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestRunById(this.TestContext.TfsRequestContext, projectGuidFromName, testRunId, true, out testRun))
          return false;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results;
        this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestResults(this.TestContext.TfsRequestContext, projectGuidFromName, testRunId, out results, new ResultDetails?(ResultDetails.Iterations));
        runResults = this.PopulateActionResultDetails(testRun, results);
        return true;
      }
      catch (Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException ex)
      {
        string errorMessage = TestRunHelper.GetErrorMessage(ex);
        if (!string.IsNullOrWhiteSpace(errorMessage))
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(errorMessage, ex.ObjectType);
        throw;
      }
    }

    private TestRunResultsAndActionResults GetTestRunAndResultsFromTCMAsync(int testRunId)
    {
      try
      {
        Guid projectId = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
        return this.PopulateActionResultDetails(TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.GetTestRunByIdAsync(projectId, testRunId, new bool?(true), new bool?(), (object) null, new CancellationToken())?.Result)), TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(projectId, testRunId, new ResultDetails?(ResultDetails.Iterations), new int?(), new int?(), (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result)));
      }
      catch (Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException ex)
      {
        string errorMessage = TestRunHelper.GetErrorMessage(ex);
        if (!string.IsNullOrWhiteSpace(errorMessage))
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(errorMessage, ex.ObjectType);
        throw;
      }
    }

    private TestRunResultsAndActionResults PopulateActionResultDetails(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results)
    {
      List<TestCaseResultModel> list = results.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>) (testCaseResult => new TestCaseResultModel(testCaseResult))).ToList<TestCaseResultModel>();
      TestResultHelper.PopulateDataRowCountsInTestResults((IEnumerable<TestCaseResultModel>) list, this.TestContext);
      List<TestActionResultModel> actionResults = new List<TestActionResultModel>();
      List<TestResultParameterModel> parameters = new List<TestResultParameterModel>();
      List<TestAttachmentModel> attachments = new List<TestAttachmentModel>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result1 in results)
      {
        int result2;
        if (int.TryParse(result1.TestRun.Id, out result2))
        {
          (List<TestActionResultModel> ActionResults, List<TestResultParameterModel> ParameterResults, List<TestAttachmentModel> Attachments) mvcModel = TestResultHelper.ConvertWebApiIterationDetailsToMvcModel(result2, result1.Id, result1);
          actionResults.AddRange((IEnumerable<TestActionResultModel>) mvcModel.ActionResults);
          parameters.AddRange((IEnumerable<TestResultParameterModel>) mvcModel.ParameterResults);
          attachments.AddRange((IEnumerable<TestAttachmentModel>) mvcModel.Attachments);
        }
      }
      List<TestResultLinkedBugsModel> associatedWorkItems = this.GetAssociatedWorkItems(list.Select<TestCaseResultModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>((Func<TestCaseResultModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) (r => new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(r.TestRunId, r.TestResultId))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>());
      TestRunModel testRunModel = new TestRunModel(testRun);
      return new TestRunResultsAndActionResults()
      {
        TestRun = testRunModel,
        TestCaseResults = list,
        TestActionResults = new TestActionResultDetailsModel(actionResults, parameters, attachments, associatedWorkItems)
      };
    }

    private void UpdateInternal(TestRunModel runModel)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> testRunList = Microsoft.TeamFoundation.TestManagement.Server.TestRun.FilterNotOfType((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) Microsoft.TeamFoundation.TestManagement.Server.TestRun.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, runModel.TestRunId, runModel.Owner, string.Empty, this.TestContext.ProjectName), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
      if (testRunList.Count != 1)
        return;
      runModel.UpdateFromModel(testRunList[0]).Update((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
    }

    private bool TryUpdateRunInTCM(TestRunModel runModel)
    {
      Guid projectId;
      RunUpdateModel runUpdateModel = this.CreateRunUpdateModel(runModel, out projectId);
      return this.TestContext.TestRequestContext.TcmServiceHelper.TryUpdateTestRun(this.TestContext.TfsRequestContext, projectId, runModel.TestRunId, runUpdateModel, out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun _);
    }

    private void UpdateRunInTCMAsync(TestRunModel runModel)
    {
      Guid projectId;
      RunUpdateModel runUpdateModel = this.CreateRunUpdateModel(runModel, out projectId);
      TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, projectId, runModel.TestRunId, (object) null, new CancellationToken())?.Result));
    }

    private RunUpdateModel CreateRunUpdateModel(TestRunModel runModel, out Guid projectId)
    {
      projectId = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      int buildArtifactId = !string.IsNullOrEmpty(runModel.BuildUri) ? this.BuildServiceHelper.GetBuildArtifactId(runModel.BuildUri) : 0;
      ShallowReference shallowReference1;
      if (buildArtifactId <= 0)
      {
        shallowReference1 = (ShallowReference) null;
      }
      else
      {
        shallowReference1 = new ShallowReference();
        shallowReference1.Id = buildArtifactId.ToString();
        shallowReference1.Name = runModel.BuildNumber;
      }
      ShallowReference shallowReference2 = shallowReference1;
      string title = runModel.Title;
      string str1 = runModel.State.ToString();
      string iteration1 = runModel.Iteration;
      ShallowReference shallowReference3 = shallowReference2;
      DateTime dateTime = runModel.StartDate;
      dateTime = dateTime.ToUniversalTime();
      string str2 = dateTime.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      dateTime = runModel.CompleteDate;
      dateTime = dateTime.ToUniversalTime();
      string completedDate = dateTime.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      string iteration2 = iteration1;
      string state = str1;
      ShallowReference build = shallowReference3;
      string startedDate = str2;
      ShallowReference testSettings = new ShallowReference();
      testSettings.Id = runModel.TestSettingsId.ToString();
      string testEnvironmentId = runModel.TestEnvironmentId.ToString();
      bool? deleteUnexecutedResults = new bool?();
      return new RunUpdateModel(title, completedDate, iteration2, state, build: build, startedDate: startedDate, testSettings: testSettings, testEnvironmentId: testEnvironmentId, deleteUnexecutedResults: deleteUnexecutedResults);
    }

    private Tuple<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[]> CreateTestRunAndTestCaseResults(
      TestRunModel runModel,
      TestResultCreationRequestModel[] testResultCreationRequestModels,
      int firstResultId,
      string projectName = null,
      int buildDefinitionId = 0)
    {
      try
      {
        Microsoft.TeamFoundation.TestManagement.Server.TestRun run1 = new Microsoft.TeamFoundation.TestManagement.Server.TestRun();
        Microsoft.TeamFoundation.TestManagement.Server.TestRun run2 = runModel.UpdateFromModel(run1);
        Microsoft.TeamFoundation.TestManagement.Server.TestRunHelper.PopulateBuildDetails(new TestManagementRequestContext(this.TestContext.TfsRequestContext), run2, runModel.BuildUri, projectName, buildDefinitionId);
        run2.Type = (byte) 4;
        run2.StartDate = DateTime.Now;
        run2.CompleteDate = DateTime.Now;
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] testCaseResults = TestResultHelper.CreateTestCaseResults(testResultCreationRequestModels, runModel.Owner, firstResultId, runModel.TestPlanId);
        if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
        {
          if (!this.TryCreateTestRunAndResultsInTCM(this.TestContext.ProjectName, run2, testCaseResults))
            run2.Create((TestManagementRequestContext) this.TestContext.TestRequestContext, (Microsoft.TeamFoundation.TestManagement.Server.TestSettings) null, testCaseResults, true, this.TestContext.ProjectName);
        }
        else
          this.CreateTestRunAndResultsInTCMAsync(this.TestContext.ProjectName, run2, testCaseResults);
        return Tuple.Create<Microsoft.TeamFoundation.TestManagement.Server.TestRun, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[]>(run2, testCaseResults);
      }
      catch (Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException ex)
      {
        string errorMessage = TestRunHelper.GetErrorMessage(ex);
        if (!string.IsNullOrWhiteSpace(errorMessage))
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(errorMessage, ex.ObjectType);
        throw;
      }
    }

    private bool IsAbortRequired(List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResults)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult in testCaseResults)
      {
        if (testCaseResult.State == (byte) 4)
          flag1 = true;
        else if (testCaseResult.State != (byte) 5)
          flag2 = true;
      }
      return !flag1 && flag2;
    }

    private LinkFilter GetLinkFilterValue() => new LinkFilter()
    {
      FilterType = FilterType.ToolType,
      FilterValues = new string[1]{ "WorkItemTracking" }
    };

    private List<TestResultLinkedBugsModel> GetLinkedBugsInfoFromIdsFoTestResult(
      int resultId,
      List<int> workItemIds,
      IEnumerable<string> workItemTypeNamesForBugs)
    {
      List<TestResultLinkedBugsModel> fromIdsFoTestResult = new List<TestResultLinkedBugsModel>();
      if (workItemIds.Count > 0)
      {
        fromIdsFoTestResult = new List<TestResultLinkedBugsModel>();
        string[] fields = new string[3]
        {
          "System.Id",
          "System.Title",
          "System.WorkItemType"
        };
        foreach (WorkItemFieldData workItemFieldValue in this.TestContext.TfsRequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(this.TestContext.TfsRequestContext, (IEnumerable<int>) workItemIds, (IEnumerable<string>) fields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false))
        {
          string title = workItemFieldValue.Title;
          int id = workItemFieldValue.Id;
          string workItemType = workItemFieldValue.WorkItemType;
          if (workItemTypeNamesForBugs.Contains<string>(workItemType, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          {
            TestResultLinkedBugsModel resultLinkedBugsModel = new TestResultLinkedBugsModel(resultId, id, title);
            if (!fromIdsFoTestResult.Contains(resultLinkedBugsModel))
              fromIdsFoTestResult.Add(resultLinkedBugsModel);
          }
        }
      }
      return fromIdsFoTestResult;
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

    private string GetArtiFactUrisForTestResult(int runId, int resultId) => LinkingUtilities.EncodeUri(new ArtifactId()
    {
      ArtifactType = "TcmResult",
      Tool = "TestManagement",
      ToolSpecificId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) runId, (object) resultId)
    });

    private TestResultCreationRequestModel[] CreateTestResultCreationRequestModelFromTestPoints(
      List<TestPointModel> testPoints)
    {
      return testPoints.Select<TestPointModel, TestResultCreationRequestModel>((Func<TestPointModel, TestResultCreationRequestModel>) (testPoint => new TestResultCreationRequestModel()
      {
        TestCaseId = testPoint.TestCaseId,
        ConfigurationId = testPoint.ConfigurationId,
        ConfigurationName = testPoint.ConfigurationName,
        TestPointId = testPoint.TestPointId,
        Owner = testPoint.AssignedTo
      })).ToArray<TestResultCreationRequestModel>();
    }

    private TestRunAndResultsModel CreateModelFromRunAndResults(
      Microsoft.TeamFoundation.TestManagement.Server.TestRun run,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results)
    {
      return new TestRunAndResultsModel()
      {
        TestRun = new TestRunModel(run),
        TestResultCreationResponseModels = TestResultHelper.CreateTestResultCreationResponseModels(results)
      };
    }

    private bool TryCreateTestRunAndResultsInTCM(
      string projectName,
      Microsoft.TeamFoundation.TestManagement.Server.TestRun run,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results)
    {
      GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, projectName);
      RunCreateModel testRunCreateModel = this.GetTestRunCreateModel(run);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun;
      if (!this.TestContext.TestRequestContext.TcmServiceHelper.TryCreateTestRun(this.TestContext.TfsRequestContext, projectFromName.GuidId, testRunCreateModel, out newTestRun))
        return false;
      this.UpdatePointTableWithNewOutcome(projectFromName, run, results, newTestRun);
      return true;
    }

    private void CreateTestRunAndResultsInTCMAsync(
      string projectName,
      Microsoft.TeamFoundation.TestManagement.Server.TestRun run,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results)
    {
      GuidAndString project = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, projectName);
      RunCreateModel runCreateModel = this.GetTestRunCreateModel(run);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun webApiTestRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.CreateTestRunAsync(runCreateModel, project.GuidId, (object) null, new CancellationToken())?.Result));
      this.UpdatePointTableWithNewOutcome(project, run, results, webApiTestRun);
    }

    private void UpdatePointTableWithNewOutcome(
      GuidAndString project,
      Microsoft.TeamFoundation.TestManagement.Server.TestRun run,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun webApiTestRun)
    {
      run.TestRunId = webApiTestRun.Id;
      this.TestContext.TestRequestContext.WorkItemFieldDataHelper.PopulateResultsFromTestCases((TestManagementRequestContext) this.TestContext.TestRequestContext, project, results, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) results).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, int>) (r => r.TestCaseId)).ToArray<int>(), true);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] webApiTestResultCreateModels = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) results).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => TestResultHelper.ConvertTestResultToWebApiModel(this.TestContext, r))).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.AddTestResultsToTestRunAsync(webApiTestResultCreateModels, project.GuidId, run.TestRunId, (object) null, new CancellationToken())?.Result));
      webApiTestResultCreateModels = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) webApiTestResultCreateModels).Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result =>
      {
        result.TestRun = new ShallowReference()
        {
          Id = webApiTestRun.Id.ToString()
        };
        result.TestPlan = new ShallowReference()
        {
          Id = webApiTestRun.Plan?.Id.ToString()
        };
        result.LastUpdatedDate = DateTime.UtcNow;
        return result;
      })).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      this.TestContext.TestRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.TestContext.TfsRequestContext, this.TestContext.ProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) webApiTestResultCreateModels);
      for (int index = 0; index < results.Length; ++index)
      {
        if (results[index] != null && testCaseResultList != null && testCaseResultList[index] != null)
          results[index].Id = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier()
          {
            TestRunId = run.TestRunId,
            TestResultId = testCaseResultList[index].Id
          };
      }
    }

    private RunCreateModel GetTestRunCreateModel(Microsoft.TeamFoundation.TestManagement.Server.TestRun run)
    {
      string name = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestRunState), (object) run.State);
      string iteration1 = run.Iteration;
      bool? nullable = new bool?(run.IsAutomated);
      string title = run.Title;
      string iteration2 = iteration1;
      IdentityRef identityRef = new IdentityRef()
      {
        Id = run.Owner.ToString(),
        DisplayName = run.OwnerName
      };
      int buildArtifactId = !string.IsNullOrEmpty(run.BuildUri) ? this.BuildServiceHelper.GetBuildArtifactId(run.BuildUri) : 0;
      string str1 = run.StartDate.ToUniversalTime().ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      DateTime dateTime = run.CompleteDate;
      dateTime = dateTime.ToUniversalTime();
      string str2 = dateTime.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      ShallowReference plan = new ShallowReference();
      plan.Id = run.TestPlanId.ToString();
      ShallowReference testSettings;
      if (run.PublicTestSettingsId <= 0)
      {
        testSettings = (ShallowReference) null;
      }
      else
      {
        testSettings = new ShallowReference();
        testSettings.Id = run.PublicTestSettingsId.ToString();
      }
      int buildId = buildArtifactId;
      string state = name;
      bool? isAutomated = nullable;
      string testEnvironmentId = run.TestEnvironmentId.ToString();
      string startedDate = str1;
      string completedDate = str2;
      IdentityRef owner = identityRef;
      TimeSpan? runTimeout = new TimeSpan?();
      return new RunCreateModel(title, iteration2, plan: plan, testSettings: testSettings, buildId: buildId, state: state, isAutomated: isAutomated, testEnvironmentId: testEnvironmentId, startedDate: startedDate, completedDate: completedDate, owner: owner, runTimeout: runTimeout);
    }

    internal TestPlansHelper TestPlanHelper
    {
      get
      {
        if (this.testPlanHelper == null)
          this.testPlanHelper = new TestPlansHelper(this.TestContext);
        return this.testPlanHelper;
      }
    }

    internal IBuildServiceHelper BuildServiceHelper
    {
      get
      {
        if (this.buildServiceHelper == null)
          this.buildServiceHelper = new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
        return (IBuildServiceHelper) this.buildServiceHelper;
      }
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.TestContext.TestRequestContext.RequestContext.GetClient<TestResultsHttpClient>();
  }
}
