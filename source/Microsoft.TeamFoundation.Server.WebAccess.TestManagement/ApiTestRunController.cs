// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ApiTestRunController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [ValidateInput(false)]
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  public class ApiTestRunController : TestManagementAreaController
  {
    private TestRunHelper m_testRunHelper;

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Create(
      [ModelBinder(typeof (JsonModelBinder))] TestRunModel runModel,
      [ModelBinder(typeof (JsonModelBinder))] TestResultCreationRequestModel[] testResultCreationRequestModels)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      ArgumentUtility.CheckForNull<TestRunModel>(runModel, nameof (runModel));
      ArgumentUtility.CheckForNull<TestResultCreationRequestModel[]>(testResultCreationRequestModels, nameof (testResultCreationRequestModels));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestRunHelper.Create(runModel, testResultCreationRequestModels, TestManagementConstants.c_firstTestCaseResultId));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult CreateTestRunForTestPoints(int testPlanId, int[] testPointIds)
    {
      DataContractJsonResult runForTestPoints = new DataContractJsonResult((object) this.TestRunHelper.CreateTestRunForTestPoints(testPlanId, testPointIds));
      runForTestPoints.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) runForTestPoints;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestRunForTestPoint(int testPlanId, int testPointId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testRunForTestPoint = new DataContractJsonResult((object) this.TestRunHelper.GetTestRunAndResultsForTestPoint(testPlanId, testPointId));
      testRunForTestPoint.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testRunForTestPoint;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestRun(int testRunId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testRun = new DataContractJsonResult((object) this.TestRunHelper.GetTestRunAndResults(testRunId));
      testRun.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testRun;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestRunAttachments(int testRunId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testRunAttachments = new DataContractJsonResult((object) TestResultHelper.GetAttachments(this.TestContext, testRunId));
      testRunAttachments.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testRunAttachments;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Update([ModelBinder(typeof (JsonModelBinder))] TestRunModel runModel)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TestRunHelper.Update(runModel);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Abort([ModelBinder(typeof (JsonModelBinder))] TestRunModel runModel)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TestRunHelper.Abort(runModel);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult End([ModelBinder(typeof (JsonModelBinder))] TestRunModel runModel)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TestRunHelper.End(runModel);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    internal TestRunHelper TestRunHelper
    {
      get
      {
        if (this.m_testRunHelper == null)
          this.m_testRunHelper = new TestRunHelper(this.TestContext);
        return this.m_testRunHelper;
      }
    }
  }
}
