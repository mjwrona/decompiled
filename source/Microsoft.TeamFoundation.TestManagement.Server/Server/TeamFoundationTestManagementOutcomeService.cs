// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementOutcomeService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TeamFoundationTestManagementOutcomeService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementOutcomeService,
    IVssFrameworkService
  {
    private const int c_firstTestCaseResultId = 100000;

    public TeamFoundationTestManagementOutcomeService()
    {
    }

    public TeamFoundationTestManagementOutcomeService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public void BulkMarkTestPoints(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest updateRequest)
    {
      try
      {
        if (!((IEnumerable<int>) updateRequest.TestPointIds).Any<int>())
          return;
        this.BulkMarkTestPointsInternal(requestContext, updateRequest);
      }
      catch (TestObjectNotFoundException ex)
      {
        string message = this.GetErrorMessage(ex);
        if (string.IsNullOrWhiteSpace(message))
        {
          if (ex.ObjectType != ObjectTypes.AssociatedBuild)
            throw;
          else
            message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) ex.Message, (object) ServerResources.UpdateTestPlanSettingsWithCorrectBuild);
        }
        throw new TestObjectNotFoundException(message, ex.ObjectType);
      }
    }

    private void BulkMarkTestPointsInternal(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest updateRequest)
    {
      TfsTestManagementRequestContext managementRequestContext1 = this.GetTfsTestManagementRequestContext(requestContext);
      TestPlan testPlan = this.FetchTestPlan(requestContext, updateRequest);
      if (testPlan == null)
        return;
      string runTitle = this.GetRunTitle(requestContext, updateRequest, testPlan);
      TfsTestManagementRequestContext managementRequestContext2 = this.GetTfsTestManagementRequestContext(requestContext);
      if (managementRequestContext2.PlannedTestingTCMServiceHelper.TryBulkMarkTestPoints((TestManagementRequestContext) managementRequestContext2, testPlan, runTitle, updateRequest))
        return;
      List<TestPoint> testPoints = this.FetchTestPointsFromIds(requestContext, updateRequest);
      DateTime now = DateTime.Now;
      DateTime dateTime = now;
      Guid userId = requestContext.GetUserId();
      TestCaseResult[] testCaseResults = PlannedTestResultsHelper.CreateTestCaseResults(testPoints, dateTime, userId, 100000);
      TestRunHelper.CreateTestRun(runTitle, testPlan, testCaseResults, updateRequest.Outcome, now, updateRequest.UserId, (TestManagementRequestContext) managementRequestContext1, updateRequest.ProjectName, TestRunType.Web);
      TestResultHelper.UpdateResultWithOutcome(testCaseResults, updateRequest.Outcome);
      this.SaveTestResults(requestContext, updateRequest.ProjectName, (IEnumerable<TestCaseResult>) testCaseResults);
    }

    public bool ShouldSyncOutcomeAcrossSuites(
      IVssRequestContext requestContext,
      int planId,
      Guid currentProjectGuid)
    {
      if (planId <= 0)
        return false;
      string path = string.Format("MS.VS.TestManagement/TestOutcomeSettings/TestPlan/{0}", (object) planId.ToString());
      using (ISettingsProvider webSettings = WebSettings.GetWebSettings(requestContext, currentProjectGuid, (WebApiTeam) null, WebSettingsScope.Project))
        return webSettings.GetSetting<bool>(path, false);
    }

    public void SyncTestOutcome(
      IVssRequestContext requestContext,
      TestOutcomeUpdateRequest updateRequest)
    {
      TeamFoundationTestManagementOutcomeService.InvokeSyncOutcomeJob((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), updateRequest);
    }

    public void UpdateTestPointOutcome(
      IVssRequestContext requestContext,
      string projectName,
      IList<TestPointOutcomeUpdateFromTestResultRequest> results)
    {
      string methodName = "TeamFoundationTestManagementOutcomeService.UpdateTestPoinstOutcome";
      try
      {
        using (PerfManager.Measure(requestContext, "BusinessLayer", TraceUtils.GetActionName(methodName, "TestManagement")))
        {
          requestContext.TraceEnter(1015082, "TestManagement", "BusinessLayer", methodName);
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), projectName);
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
            planningDatabase.UpdateTestPointOutcome(projectFromName.GuidId, results);
          this.FireNotification((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), 0, 0, projectName);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015082, "TestManagement", "BusinessLayer", ex);
      }
      finally
      {
        requestContext.TraceLeave("BusinessLayer", methodName);
      }
    }

    private static void InvokeSyncOutcomeJob(
      TestManagementRequestContext context,
      TestOutcomeUpdateRequest updateRequest)
    {
      TeamFoundationJobService service = context.RequestContext.GetService<TeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
      foundationJobDefinition.JobId = Guid.NewGuid();
      foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.TestOutcomeSynchronizationJob";
      foundationJobDefinition.Data = TeamFoundationSerializationUtility.SerializeToXml((object) updateRequest);
      foundationJobDefinition.Name = "SyncTestOutcome";
      foundationJobDefinition.PriorityClass = JobPriorityClass.High;
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      };
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        foundationJobDefinition.ToJobReference()
      };
      service.UpdateJobDefinitions(context.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      service.QueueJobsNow(context.RequestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
    }

    private static void FilterDeletedPoints(List<TestPoint> points, ICollection<int> deletedIds)
    {
      if (deletedIds == null || deletedIds.Count <= 0)
        return;
      points.RemoveAll((Predicate<TestPoint>) (tp => deletedIds.Contains(tp.PointId)));
    }

    private static IdAndRev[] ConvertToIdAndRevs(int[] testPointIds) => ((IEnumerable<int>) testPointIds).Select<int, IdAndRev>((Func<int, IdAndRev>) (id => new IdAndRev(id, 0))).ToArray<IdAndRev>();

    private string GetRunTitle(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest updateRequest,
      TestPlan testPlan)
    {
      ServerTestSuite serverTestSuite = this.FetchTestSuite(requestContext, updateRequest);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BulkMarkRunTitle, serverTestSuite == null || updateRequest.SuiteId == testPlan.RootSuiteId ? (object) testPlan.Name : (object) serverTestSuite.Title);
    }

    private ServerTestSuite FetchTestSuite(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest updateRequest)
    {
      if (updateRequest.SuiteId == 0)
        return (ServerTestSuite) null;
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), updateRequest.ProjectName, new List<IdAndRev>()
      {
        new IdAndRev() { Id = updateRequest.SuiteId }
      }.ToArray(), (List<int>) null);
      return serverTestSuiteList.Count != 1 ? (ServerTestSuite) null : serverTestSuiteList[0];
    }

    private TestPlan FetchTestPlan(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest testOutcomeUpdateRequest)
    {
      List<TestPlan> testPlanList = TestPlan.Fetch((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), new List<int>()
      {
        testOutcomeUpdateRequest.PlanId
      }.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), new List<int>(), testOutcomeUpdateRequest.ProjectName, false, false);
      return testPlanList.Count != 1 ? (TestPlan) null : testPlanList[0];
    }

    private List<TestPoint> FetchTestPointsFromIds(
      IVssRequestContext requestContext,
      ITestOutcomeUpdateRequest testOutcomeUpdateRequest)
    {
      TcmArgumentValidator.CheckNull((object) testOutcomeUpdateRequest.TestPointIds, "testPointIds");
      TfsTestManagementRequestContext managementRequestContext = this.GetTfsTestManagementRequestContext(requestContext);
      List<int> deletedIds1 = new List<int>();
      string projectName = testOutcomeUpdateRequest.ProjectName;
      int planId = testOutcomeUpdateRequest.PlanId;
      IdAndRev[] idAndRevs = TeamFoundationTestManagementOutcomeService.ConvertToIdAndRevs(testOutcomeUpdateRequest.TestPointIds);
      string[] testCaseProperties = Array.Empty<string>();
      List<int> deletedIds2 = deletedIds1;
      List<TestPoint> points = TestPoint.Fetch((TestManagementRequestContext) managementRequestContext, projectName, planId, idAndRevs, testCaseProperties, deletedIds2);
      if (points != null)
        TeamFoundationTestManagementOutcomeService.FilterDeletedPoints(points, (ICollection<int>) deletedIds1);
      else
        points = new List<TestPoint>();
      if (deletedIds1.Count > 0)
        throw new TestObjectNotFoundException(requestContext, deletedIds1[0], ObjectTypes.TestPoint);
      return points;
    }

    private void FireNotification(
      TestManagementRequestContext context,
      int pointId,
      int planId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestPointChangedNotification(pointId, planId, projectName));
    }

    protected virtual void SaveTestResults(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<TestCaseResult> testResults)
    {
      ResultUpdateRequest[] array = testResults.Select<TestCaseResult, ResultUpdateRequest>((Func<TestCaseResult, ResultUpdateRequest>) (result => new ResultUpdateRequest()
      {
        TestCaseResult = result,
        TestResultId = result.TestResultId,
        TestRunId = result.TestRunId
      })).ToArray<ResultUpdateRequest>();
      TestCaseResult.Update((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), array, projectName);
    }
  }
}
