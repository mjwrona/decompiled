// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Suites5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientGroupByResource("Test Suites")]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "Suites", ResourceVersion = 1)]
  public class Suites5Controller : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("A4080E84-F17B-4FAD-84F1-7960B6525BF2")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestSuitesByCase.json", "Get a test suites which contains testcase across projects.", null, null)]
    public IList<TestSuite> GetSuitesByTestCaseId(int testCaseId) => (IList<TestSuite>) this.RevisedTestSuitesHelper.GetSuitesByTestCaseId(testCaseId);

    [HttpGet]
    [ClientLocationId("1046D5D3-AB61-4CA7-A65A-36118A978256")]
    [ClientResponseType(typeof (IPagedList<TestSuite>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestSuitesForPlan.json", "Get a test suites for plan.", null, null)]
    [ClientExample("GetTestSuitesAsTreeView.json", "Get a test suites for plan as tree view.", null, null)]
    public HttpResponseMessage GetTestSuitesForPlan(
      int planId,
      SuiteExpand expand = SuiteExpand.None,
      string continuationToken = null,
      bool asTreeView = false)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      List<TestSuite> testSuitesForPlan = this.RevisedTestSuitesHelper.GetTestSuitesForPlan(this.ProjectInfo, planId, expand, skipRows, topToFetch, watermark, asTreeView);
      continuationToken = (string) null;
      if (!asTreeView && testSuitesForPlan != null && testSuitesForPlan.Count >= topToFetch && testSuitesForPlan[topToFetch - 1] != null)
      {
        continuationToken = Utils.GenerateContinuationToken(testSuitesForPlan[topToFetch - 1].Id, topRemaining);
        testSuitesForPlan.RemoveAt(topToFetch - 1);
      }
      HttpResponseMessage response = this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) testSuitesForPlan);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpGet]
    [ClientLocationId("1046D5D3-AB61-4CA7-A65A-36118A978256")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestSuiteById.json", "Get a test suite by Id.", null, null)]
    public TestSuite GetTestSuiteById(int planId, int suiteId, SuiteExpand expand = SuiteExpand.None) => this.RevisedTestSuitesHelper.GetTestSuiteById(this.ProjectInfo, planId, suiteId, expand);

    [HttpPost]
    [ClientLocationId("1046D5D3-AB61-4CA7-A65A-36118A978256")]
    [ClientExample("CreateTestSuite.json", "Create a test suite.", null, null)]
    public TestSuite CreateTestSuite(int planId, TestSuiteCreateParams testSuiteCreateParams)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.CreateTestSuite(this.ProjectInfo, planId, testSuiteCreateParams);
    }

    [HttpPost]
    [ClientLocationId("1E58FBE6-1761-43CE-97F6-5492EC9D438E")]
    [ClientInternalUseOnly(false)]
    public List<TestSuite> CreateBulkTestSuites(
      int planId,
      int parentSuiteId,
      TestSuiteCreateParams[] testSuiteCreateParams)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.CreateBulkTestSuites(this.ProjectInfo, planId, parentSuiteId, testSuiteCreateParams);
    }

    [HttpPatch]
    [ClientLocationId("1046D5D3-AB61-4CA7-A65A-36118A978256")]
    [ClientExample("UpdateTestSuiteParent.json", "Update a test suites parent.", null, null)]
    [ClientExample("UpdateTestSuiteProperties.json", "Update a test suites tester.", null, null)]
    public TestSuite UpdateTestSuite(
      int planId,
      int suiteId,
      TestSuiteUpdateParams testSuiteUpdateParams)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.UpdateTestSuite(this.ProjectInfo, planId, suiteId, testSuiteUpdateParams);
    }

    [HttpDelete]
    [ClientLocationId("1046D5D3-AB61-4CA7-A65A-36118A978256")]
    [ClientExample("DeleteTestSuite.json", "Delete a test suite.", null, null)]
    public void DeleteTestSuite(int planId, int suiteId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.RevisedTestSuitesHelper.DeleteTestSuite(this.ProjectId.ToString(), planId, suiteId);
    }
  }
}
