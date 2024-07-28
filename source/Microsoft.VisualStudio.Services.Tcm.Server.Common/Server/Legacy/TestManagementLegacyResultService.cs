// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestManagementLegacyResultService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class TestManagementLegacyResultService : 
    TeamFoundationTestManagementService,
    ITestManagementLegacyResultService,
    IVssFrameworkService
  {
    private const string c_TestRunBatchSizeForPointOutcomeSync = "/Service/TestManagement/Settings/PointOutcomeSyncJob/TestRunBatchSize";
    private const string c_TestResultBatchSizeForPointOutcomeSync = "/Service/TestManagement/Settings/PointOutcomeSyncJob/TestResultBatchSize";
    private const int c_DefaultTestRunBatchSizeForPointOutcomeSync = 10;
    private const int c_DefaultTestResultBatchSizeForPointOutcomeSync = 1000;

    public TestManagementLegacyResultService()
    {
    }

    public TestManagementLegacyResultService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<LegacyTestCaseResult> CreateBlockedResults(
      TestManagementRequestContext requestContext,
      GuidAndString projectId,
      List<LegacyTestCaseResult> testCaseResults)
    {
      if (testCaseResults == null || !testCaseResults.Any<LegacyTestCaseResult>())
        return testCaseResults;
      Guid teamFoundationId = requestContext.UserTeamFoundationId;
      List<int> intList = new List<int>();
      bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(requestContext.RequestContext);
      foreach (IGrouping<int, LegacyTestCaseResult> source in testCaseResults.GroupBy<LegacyTestCaseResult, int>((Func<LegacyTestCaseResult, int>) (result => result.TestPlanId)))
      {
        Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun = new Microsoft.TeamFoundation.TestManagement.Server.TestRun();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        {
          testRun.Title = string.Empty;
          testRun.Owner = teamFoundationId;
          testRun.TestPlanId = source.Key;
          testRun.LegacySharePath = string.Empty;
          testRun.State = (byte) 3;
          testRun.Type = (byte) 2;
          testRun = managementDatabase.CreateTestRun(projectId.GuidId, testRun, teamFoundationId, changeCounterInterval, true);
          intList.Add(testRun.TestRunId);
        }
        int[] resultIds;
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.CreateResults2(requestContext, testRun.TestRunId, source.Select<LegacyTestCaseResult, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>((Func<LegacyTestCaseResult, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) (result => TestCaseResultContractConverter.Convert(result))).ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>(), projectId, false, false, out int[] _, out int _, out resultIds);
        int index = 0;
        foreach (LegacyTestCaseResult legacyTestCaseResult in (IEnumerable<LegacyTestCaseResult>) source)
        {
          legacyTestCaseResult.TestRunId = testRun.TestRunId;
          legacyTestCaseResult.TestResultId = resultIds[index];
          ++index;
        }
      }
      foreach (int testRunId in intList)
        Microsoft.TeamFoundation.TestManagement.Server.TestRun.FireNotification(requestContext, testRunId, projectId.String);
      return testCaseResults;
    }

    public List<PointLastResult> FilterPoints(
      TestManagementRequestContext testManagementRequestContext,
      Guid ProjectId,
      FilterPointQuery request)
    {
      if (testManagementRequestContext.IsFeatureEnabled("TestManagement.Server.BypassPointResultDetails"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          return managementDatabase.FilterPointsOnOutcome2(ProjectId, request.PlanId, request.PointIds, request.PointOutcome, request.ResultState).ToList<PointLastResult>();
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          return managementDatabase.FilterPointsOnOutcome(ProjectId, request.PlanId, request.PointIds, request.PointOutcome, request.ResultState).ToList<PointLastResult>();
      }
    }

    public TestResultsWithWatermark GetManualTestResultsByUpdatedDate(
      TestManagementRequestContext testManagementRequestContext,
      Guid projectId,
      TestResultWatermark fromWatermark)
    {
      using (PerfManager.Measure(testManagementRequestContext.RequestContext, "RestLayer", "TestManagementLegacyResultService.GetManualTestResultsByUpdatedDate", 50, true))
      {
        if (!ServicePrincipals.IsServicePrincipal(testManagementRequestContext.RequestContext, testManagementRequestContext.RequestContext.UserContext))
          throw new Microsoft.TeamFoundation.TestManagement.Server.AccessDeniedException(FrameworkResources.UnauthorizedUserError((object) testManagementRequestContext.RequestContext.GetUserId()));
        IVssRegistryService service = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>();
        int runBatchSize = service.GetValue<int>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/PointOutcomeSyncJob/TestRunBatchSize", 10);
        int resultBatchSize = service.GetValue<int>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/PointOutcomeSyncJob/TestResultBatchSize", 1000);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
        {
          TestResultWatermark toWatermark;
          List<PointsResults2> pointsResults2List = managementDatabase.QueryManualTestResultsByUpdateDate(projectId, runBatchSize, resultBatchSize, fromWatermark, out toWatermark);
          return new TestResultsWithWatermark()
          {
            PointsResults = pointsResults2List,
            ChangedDate = toWatermark.ChangedDate,
            RunId = toWatermark.TestRunId,
            ResultId = toWatermark.TestResultId
          };
        }
      }
    }
  }
}
