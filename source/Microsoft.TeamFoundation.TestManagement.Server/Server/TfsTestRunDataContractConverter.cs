// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsTestRunDataContractConverter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TfsTestRunDataContractConverter : DataContractConverter
  {
    internal TfsTestRunDataContractConverter(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
      this.m_testEnvironmentHelper = (ITestEnvironmentsHelper) new TestEnvironmentsHelper((TestManagementRequestContext) requestContext);
    }

    internal TfsTestRunDataContractConverter(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
      this.m_testEnvironmentHelper = (ITestEnvironmentsHelper) new TestEnvironmentsHelper(requestContext);
    }

    internal TfsTestRunDataContractConverter(
      TfsTestManagementRequestContext requestContext,
      TestEnvironmentsHelper testEnvironmentsHelper,
      StatisticsHelper statisticsHelper,
      IReleaseServiceHelper releaseServiceHelper)
      : base((TestManagementRequestContext) requestContext, (ITestEnvironmentsHelper) testEnvironmentsHelper, statisticsHelper, releaseServiceHelper)
    {
    }

    protected override void PopulateTestResults(
      string projectName,
      RunCreateModel createModel,
      int planId,
      int firstTestCaseResultId,
      out List<TestCaseResult> testCaseResults)
    {
      testCaseResults = new List<TestCaseResult>();
      ArgumentUtility.CheckForNull<int[]>(createModel.PointIds, "PointIds", "Test Results");
      foreach (int pointId in createModel.PointIds)
      {
        TestPoint testPoint;
        if (!this.TryGetPointFromPointId(projectName, planId, pointId, out testPoint))
          throw new TestObjectNotFoundException(string.Format(ServerResources.TestPointNotFound, (object) pointId), ObjectTypes.TestPoint);
        testCaseResults.Add(this.PopulateTestCaseResult(testPoint, firstTestCaseResultId++));
      }
    }

    protected override void PopulateTestRun(
      RunCreateModel createModel,
      string projectName,
      out TestRun testRun)
    {
      base.PopulateTestRun(createModel, projectName, out testRun);
      if (!string.IsNullOrEmpty(createModel.Iteration) || testRun.TestPlanId == 0)
        return;
      IPlannedTestsObjectHelper testsObjectFactory = this.PlannedTestsObjectFactory;
      TfsTestManagementRequestContext requestContext = this.m_requestContext as TfsTestManagementRequestContext;
      List<int> testPlanIds = new List<int>();
      testPlanIds.Add(testRun.TestPlanId);
      string projectName1 = projectName;
      List<TestPlan> testPlanList = testsObjectFactory.FetchTestPlans(requestContext, (IEnumerable<int>) testPlanIds, projectName1, false);
      testRun.Iteration = testPlanList != null && testPlanList.Count > 0 ? testPlanList[0].Iteration : throw new TestObjectNotFoundException(ServerResources.TestPlanNotFound, ObjectTypes.TestPlan);
    }

    protected override ShallowReference GetPlanRepresentation(string projectName, int planId) => this.m_requestContext.PlannedTestResultsHelper.GetShallowTestPlan(this.m_requestContext, projectName, planId);

    protected override string GetTestRunUrl(int testRunId, string projectName) => UrlBuildHelper.GetResourceUrl(this.m_requestContext.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestRunProject, (object) new
    {
      runId = testRunId,
      project = projectName
    });

    protected virtual IPlannedTestsObjectHelper PlannedTestsObjectFactory => this.ObjectFactory as IPlannedTestsObjectHelper;

    protected internal override ITestManagementObjectHelper ObjectFactory
    {
      get
      {
        if (this.m_objectFactory == null)
          this.m_objectFactory = (ITestManagementObjectHelper) new PlannedTestsObjectHelper();
        return this.m_objectFactory;
      }
      set => this.m_objectFactory = value;
    }

    private TestCaseResult PopulateTestCaseResult(TestPoint testPoint, int testCaseResultId)
    {
      TestCaseResult testCaseResult = new TestCaseResult();
      testCaseResult.TestCaseId = testPoint.TestCaseId;
      testCaseResult.TestPointId = testPoint.PointId;
      testCaseResult.ConfigurationName = testPoint.ConfigurationName;
      testCaseResult.ConfigurationId = testPoint.ConfigurationId;
      Guid teamFoundationId;
      Guid guid = teamFoundationId = this.m_requestContext.UserTeamFoundationId;
      testCaseResult.RunBy = teamFoundationId;
      testCaseResult.Owner = guid;
      string teamFoundationName;
      string str = teamFoundationName = this.m_requestContext.UserTeamFoundationName;
      testCaseResult.RunByName = teamFoundationName;
      testCaseResult.OwnerName = str;
      testCaseResult.TestResultId = testCaseResultId;
      return testCaseResult;
    }

    private bool TryGetPointFromPointId(
      string projectName,
      int planId,
      int pointId,
      out TestPoint testPoint)
    {
      this.ObjectFactory.CheckForViewTestResultPermission(projectName, this.m_requestContext);
      List<TestPoint> testPointList = new List<TestPoint>();
      List<int> deletedIds = new List<int>();
      List<TestPoint> source = this.PlannedTestsObjectFactory.FetchTestPointsFromIds((TestManagementRequestContext) (this.m_requestContext as TfsTestManagementRequestContext), projectName, planId, new IdAndRev[1]
      {
        new IdAndRev() { Id = pointId }
      }, (string[]) null, deletedIds);
      testPoint = deletedIds.Count <= 0 ? source.First<TestPoint>() : (TestPoint) null;
      return testPoint != null;
    }
  }
}
