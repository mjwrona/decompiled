// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmService.PlannedResultsTCMServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server.TcmService
{
  public class PlannedResultsTCMServiceHelper
  {
    private const int c_pointResultsBatchSize = 1000;
    private const int c_TestPointIdsBatchSizeForSuiteDeletion = 1000;
    private const int c_TestRunIdsBatchSizeForSuiteDeletion = 1000;

    public bool isOnPremiseDeployment(IVssRequestContext context) => context.ExecutionEnvironment.IsOnPremisesDeployment;

    public bool IsTCMEnabledForPlannedTestResults(
      TestManagementRequestContext tcmContext,
      int planId)
    {
      return !tcmContext.IsTcmService;
    }

    public List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> UpdatePointsWithLatestResults(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> points,
      bool excludeRunBy = false,
      string projectName = "")
    {
      List<int> values1 = new List<int>();
      List<int> values2 = new List<int>();
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPointList = new List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>();
      bool flag = this.ShouldUseOutcomeFromTfs(requestContext);
      if (flag & excludeRunBy)
        return points;
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> pointResultDetails = this.GetPointResultDetails(requestContext, projectId, planId, points.Select<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>) (p => p.PointId)).ToList<int>());
      List<TestPointOutcomeUpdateFromTestResultRequest> results = new List<TestPointOutcomeUpdateFromTestResultRequest>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint point in points)
      {
        if (pointResultDetails.ContainsKey(point.PointId))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result1 = pointResultDetails[point.PointId];
          if (!(point.LastUpdated <= result1.LastUpdatedDate))
          {
            DateTime lastResetToActive = point.LastResetToActive;
            if (!(point.LastUpdated > result1.LastUpdatedDate) || !(point.LastResetToActive < result1.LastUpdatedDate))
            {
              if (requestContext.TestPointOutcomeHelper.IsDualWriteEnabled(requestContext.RequestContext))
              {
                this.FillPointDataWithNullOutcome(point);
                goto label_25;
              }
              else
                goto label_25;
            }
          }
          point.LastResultDetails = new Microsoft.TeamFoundation.TestManagement.Server.LastResultDetails()
          {
            DateCompleted = result1.CompletedDate,
            Duration = (long) result1.DurationInMs,
            RunBy = result1.RunBy != null ? Guid.Parse(result1.RunBy.Id) : Guid.Empty,
            RunByName = result1.RunBy?.DisplayName
          };
          point.LastRunBuildNumber = result1.BuildReference?.Number;
          if (!flag)
          {
            int result2;
            if (int.TryParse(result1.TestRun.Id, out result2))
              point.LastTestRunId = result2;
            point.LastTestResultId = result1.Id;
            Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result3;
            if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result1.Outcome, out result3))
              point.LastResultOutcome = (byte) result3;
            TestResultState result4;
            if (Enum.TryParse<TestResultState>(result1.State, out result4))
              point.LastResultState = (byte) result4;
            point.State = (byte) this.ComputePointState(point, result1);
            point.LastResolutionStateId = result1.ResolutionStateId;
            FailureType result5;
            if (Enum.TryParse<FailureType>(result1.FailureType, out result5))
              point.FailureType = (byte) result5;
          }
          else
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result6;
            Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result7;
            if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result1.Outcome, out result6) && Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(point.LastResultOutcome.ToString(), out result7) && result6 != result7 && result1.CompletedDate < DateTime.UtcNow.AddHours(-1.0))
            {
              values2.Add(point.PointId);
              values1.Add(point.SuiteId);
            }
          }
        }
        else if (point.LastTestRunId != 0 || point.LastTestResultId != 0)
        {
          if (requestContext.RequestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            requestContext.TraceInfo("BusinessLayer", "Setting result of pointId: {0} to NULL since TCM did not return any results", (object) point.PointId);
            this.FillPointDataWithNullOutcome(point);
          }
          if (requestContext.RequestContext.ExecutionEnvironment.IsHostedDeployment && !string.IsNullOrEmpty(projectName))
            results.Add(new TestPointOutcomeUpdateFromTestResultRequest()
            {
              FailureType = new byte?((byte) 0),
              LastUpdated = DateTime.UtcNow,
              LastUpdatedBy = TestManagementServerConstants.TCMServiceInstanceType,
              Outcome = new byte?(),
              ResolutionStateId = 0,
              State = new byte?(),
              TestPlanId = planId,
              TestPointId = point.PointId,
              TestRunId = 0,
              TestResultId = 0
            });
        }
label_25:
        testPointList.Add(point);
      }
      if (results.Count > 0)
      {
        TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext.RequestContext);
        managementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementOutcomeService>().UpdateTestPointOutcome(managementRequestContext.RequestContext, projectName, (IList<TestPointOutcomeUpdateFromTestResultRequest>) results);
      }
      if (flag && values2.Count > 0)
      {
        ClientTraceData properties = new ClientTraceData();
        properties.Add("PlanId", (object) planId);
        properties.Add("ProjectId", (object) projectId);
        properties.Add("SuiteIds", (object) string.Join<int>(",", (IEnumerable<int>) values1));
        properties.Add("TestRunPoints", (object) string.Join<int>(",", (IEnumerable<int>) values2));
        requestContext.RequestContext.GetService<ClientTraceService>().Publish(requestContext.RequestContext, "TestManagement", "TestOutcomeOutOfSync", properties);
      }
      return testPointList;
    }

    public void UnblockTestPointResultsIfAny(
      TestManagementRequestContext requestContext,
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints,
      Guid projectId,
      int planId)
    {
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> pointResultDetails = this.GetPointResultDetails(requestContext, projectId, planId, testPoints.Select<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>) (p => p.PointId)).ToList<int>());
      Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> dictionary = new Dictionary<int, List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint in testPoints)
      {
        if (pointResultDetails.Keys.Contains<int>(testPoint.PointId))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = pointResultDetails[testPoint.PointId];
          TestResultState result1;
          Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome result2;
          if (Enum.TryParse<TestResultState>(testCaseResult.State, true, out result1) && result1 == TestResultState.Completed && Enum.TryParse<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>(testCaseResult.Outcome, true, out result2) && result2 == Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.Blocked)
          {
            testCaseResult.State = Enum.GetName(typeof (TestResultState), (object) TestResultState.Pending);
            testCaseResult.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome.None);
            testCaseResult.ResolutionStateId = 0;
            testCaseResult.ComputerName = string.Empty;
            testCaseResult.StartedDate = new DateTime();
            testCaseResult.CompletedDate = new DateTime();
            testCaseResult.LastUpdatedDate = DateTime.UtcNow;
            testCaseResult.AfnStripId = 0;
            testCaseResult.FailureType = (string) null;
            ++testCaseResult.Revision;
            testCaseResult.RunBy = new IdentityRef();
            int result3;
            if (int.TryParse(testCaseResult.TestRun.Id, out result3))
            {
              if (!dictionary.Keys.Contains<int>(result3))
                dictionary[result3] = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
              dictionary[result3].Add(testCaseResult);
            }
          }
        }
      }
      bool flag = requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
      int val1 = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TestPointResultBatchSize", 1000);
      TestResultsHttpClient testResultsHttpClient = requestContext.RequestContext.GetClient<TestResultsHttpClient>();
      foreach (int key in dictionary.Keys)
      {
        int runId = key;
        RunUpdateModel runUpdateModel = new RunUpdateModel(state: "InProgress");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun updatedTestRun = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRun) null;
        if (!flag)
          requestContext.TcmServiceHelper.TryUpdateTestRun(requestContext.RequestContext, projectId, runId, runUpdateModel, out updatedTestRun);
        else
          TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => testResultsHttpClient.UpdateTestRunAsync(runUpdateModel, projectId, runId, (object) null, new CancellationToken())?.Result));
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = dictionary[runId];
        for (int index = 0; index < testCaseResultList.Count; index += val1)
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> batch = testCaseResultList.GetRange(index, Math.Min(val1, testCaseResultList.Count - index));
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> updatedResults = (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) null;
          if (!flag)
            requestContext.TcmServiceHelper.TryUpdateTestResults(requestContext.RequestContext, projectId, runId, batch.ToArray(), out updatedResults);
          else
            updatedResults = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => testResultsHttpClient.UpdateTestResultsAsync(batch.ToArray(), projectId, runId, (object) null, new CancellationToken())?.Result));
        }
      }
    }

    public void FireSuiteDeletedNotificationForTCM(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      List<int> affectedPointIds)
    {
      if (context.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      if (planId <= 0)
      {
        context.TraceError("BusinessLayer", "Invalid plan id for deleted suites. Not firing the service bus notification.");
      }
      else
      {
        int num1 = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/TestPointIdsBatchSizeForSuiteDeletion", 1000);
        int index = 0;
        while (true)
        {
          int num2 = index;
          int? count1 = affectedPointIds?.Count;
          int valueOrDefault = count1.GetValueOrDefault();
          if (num2 < valueOrDefault & count1.HasValue)
          {
            int count2 = affectedPointIds.Count > index + num1 ? num1 : affectedPointIds.Count - index;
            TestSuiteOrTestCaseDeletedEvent payload = new TestSuiteOrTestCaseDeletedEvent(projectGuid, planId, affectedPointIds.GetRange(index, count2));
            context.TestManagementHost.PublishToServiceBus(context, "Microsoft.TestManagement.PlannedTestMetaData.Server", PlannedTestMetaDataEventType.TestSuiteDeleted, (object) payload);
            index += count2;
          }
          else
            break;
        }
      }
    }

    public void FirePointDeletedNotificationForTCM(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      List<int> affectedRunIds)
    {
      if (planId <= 0)
      {
        context.TraceError("BusinessLayer", "Invalid plan id for deleted points. Not firing the service bus notification.");
      }
      else
      {
        int num = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/TestRunIdsBatchSizeForSuiteDeletion", 1000);
        int count1 = affectedRunIds == null ? 0 : affectedRunIds.Count;
        int count2;
        for (int index = 0; index < count1; index += count2)
        {
          count2 = affectedRunIds.Count > index + num ? num : affectedRunIds.Count - index;
          TestPointDeletedEvent payload = new TestPointDeletedEvent(projectGuid, planId, affectedRunIds.GetRange(index, count2));
          context.TestManagementHost.PublishToServiceBus(context, "Microsoft.TestManagement.PlannedTestMetaData.Server", PlannedTestMetaDataEventType.TestPointDeleted, (object) payload);
        }
      }
    }

    public void FirePlanDeletedNotificationForTCM(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testPlanId)
    {
      if (context.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      TestPlanDeletedEvent payload = new TestPlanDeletedEvent(projectGuid, testPlanId);
      context.TestManagementHost.PublishToServiceBus(context, "Microsoft.TestManagement.PlannedTestMetaData.Server", PlannedTestMetaDataEventType.TestPlanDeleted, (object) payload);
    }

    public bool TryBulkMarkTestPoints(
      TestManagementRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      string runTitle,
      ITestOutcomeUpdateRequest updateRequest,
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdate = null,
      List<int> testPointIds = null)
    {
      if (!this.IsTCMEnabledForPlannedTestResults(requestContext, testPlan.PlanId))
        return false;
      DateTime utcNow = DateTime.UtcNow;
      string str1 = "Web";
      if (updateRequest.Outcome == Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Blocked)
        str1 = "Blocking";
      string name = runTitle;
      ShallowReference shallowReference = new ShallowReference()
      {
        Id = testPlan.PlanId.ToString()
      };
      int[] numArray = bulkUpdate == null ? updateRequest.TestPointIds : testPointIds.ToArray();
      string str2 = str1;
      IdentityRef identityRef = new IdentityRef()
      {
        Id = updateRequest.UserId.ToString()
      };
      string str3 = utcNow.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      string str4 = utcNow.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      int buildArtifactId = !string.IsNullOrEmpty(testPlan.BuildUri) ? new BuildServiceHelper().GetBuildArtifactId(testPlan.BuildUri) : 0;
      bool? nullable = new bool?(false);
      string iteration = testPlan.Iteration;
      int[] pointIds = numArray;
      ShallowReference plan = shallowReference;
      int buildId = buildArtifactId;
      bool? isAutomated = nullable;
      string type = str2;
      string startedDate = str3;
      string completedDate = str4;
      IdentityRef owner = identityRef;
      TimeSpan? runTimeout = new TimeSpan?();
      RunCreateModel testRun = new RunCreateModel(name, iteration, pointIds, plan, buildId: buildId, state: "InProgress", isAutomated: isAutomated, type: type, startedDate: startedDate, completedDate: completedDate, owner: owner, runTimeout: runTimeout);
      TestManagementHttpClient client = requestContext.RequestContext.GetClient<TestManagementHttpClient>();
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun result1 = client.CreateTestRunAsync(testRun, updateRequest.ProjectName, (object) null, new CancellationToken()).Result;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> result2 = client.GetTestResultsAsync(updateRequest.ProjectName, result1.Id, new ResultDetails?(), new int?(), new int?(), (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) null, (object) null, new CancellationToken()).Result;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext.RequestContext);
      managementRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(managementRequestContext.RequestContext, updateRequest.ProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) result2);
      if (bulkUpdate != null)
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in result2)
        {
          Microsoft.TeamFoundation.TestManagement.Client.TestOutcome testOutcome;
          bulkUpdate.TryGetValue(Convert.ToInt32(testCaseResult.TestPoint.Id), out testOutcome);
          testCaseResult.Outcome = testOutcome.ToString();
          testCaseResult.State = "Completed";
        }
      }
      else
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in result2)
        {
          testCaseResult.Outcome = updateRequest.Outcome.ToString();
          testCaseResult.State = "Completed";
        }
      }
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> shallowObject = client.UpdateTestResultsAsync(result2.ToArray(), updateRequest.ProjectName, result1.Id, (object) null, new CancellationToken()).SyncResult<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>();
      TestPointOutcomeUpdateRequestConverter.UpdateWebApiResult((IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) result2, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) shallowObject);
      managementRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(managementRequestContext.RequestContext, updateRequest.ProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) result2);
      return true;
    }

    public Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetPointResultDetails(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int planId,
      List<int> pointIds)
    {
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> pointResultDetails = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      int val1 = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TestPointResultBatchSize", 1000);
      TestResultsHttpClient testResultsHttpClient = requestContext.RequestContext.GetClient<TestResultsHttpClient>();
      pointIds = pointIds.Distinct<int>().ToList<int>();
      for (int index = 0; index < pointIds.Count; index += val1)
      {
        List<int> range = pointIds.GetRange(index, Math.Min(val1, pointIds.Count - index));
        TestResultsQuery testResultsQuery = new TestResultsQuery()
        {
          ResultsFilter = new ResultsFilter()
          {
            AutomatedTestName = string.Empty,
            TestPlanId = planId,
            TestPointIds = (IList<int>) range
          }
        };
        TestResultsQuery results = (TestResultsQuery) null;
        bool flag = true;
        if (!requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
          flag = requestContext.TcmServiceHelper.TryGetTestResultsByQuery(requestContext.RequestContext, projectId, testResultsQuery, out results);
        else
          results = TestManagementController.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => testResultsHttpClient.GetTestResultsByQueryAsync(testResultsQuery, projectId, (object) null, new CancellationToken())?.Result));
        if (flag && results != null)
        {
          foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result1 in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results.Results)
          {
            int result2;
            if (int.TryParse(result1.TestPoint.Id, out result2))
              pointResultDetails[result2] = result1;
          }
        }
      }
      return pointResultDetails;
    }

    private Microsoft.TeamFoundation.TestManagement.Client.TestPointState ComputePointState(
      Microsoft.TeamFoundation.TestManagement.Server.TestPoint point,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      if (point.LastResetToActive >= result.LastUpdatedDate)
        return (Microsoft.TeamFoundation.TestManagement.Client.TestPointState) point.State;
      if (string.Equals(result.State, Enum.GetName(typeof (TestResultState), (object) TestResultState.Unspecified), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.TestManagement.Client.TestPointState.Ready;
      if (!string.Equals(result.State, Enum.GetName(typeof (TestResultState), (object) TestResultState.Completed), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.TestManagement.Client.TestPointState.InProgress;
      if (string.Equals(result.Outcome, Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Passed), StringComparison.OrdinalIgnoreCase) || string.Equals(result.Outcome, Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotApplicable), StringComparison.OrdinalIgnoreCase) || string.Equals(result.Outcome, Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.TestManagement.Client.TestPointState.Completed;
      return string.Equals(result.Outcome, Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotExecuted), StringComparison.OrdinalIgnoreCase) ? Microsoft.TeamFoundation.TestManagement.Client.TestPointState.Ready : Microsoft.TeamFoundation.TestManagement.Client.TestPointState.NotReady;
    }

    private bool ShouldUseOutcomeFromTfs(TestManagementRequestContext requestContext) => requestContext.IsFeatureEnabled("TestManagement.Server.EnableDualWriteForTestPoint") && requestContext.IsFeatureEnabled("TestManagement.Server.EnablePointOutcomeFromTFS");

    private Microsoft.TeamFoundation.TestManagement.Server.TestPoint FillPointDataWithNullOutcome(
      Microsoft.TeamFoundation.TestManagement.Server.TestPoint point)
    {
      point.LastTestRunId = 0;
      point.LastTestResultId = 0;
      point.LastResultOutcome = (byte) 0;
      point.LastResultState = (byte) 0;
      point.State = (byte) 1;
      point.LastResolutionStateId = 0;
      point.FailureType = (byte) 0;
      return point;
    }
  }
}
