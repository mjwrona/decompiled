// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Suites4Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [ClientGroupByResource("Test Suites")]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Suites", ResourceVersion = 3)]
  public class Suites4Controller : TestManagementController
  {
    [HttpGet]
    [ActionName("GetTestSuitesForTestCase")]
    [ClientLocationId("09a6167b-e969-4775-9247-b94cf3819caf")]
    [ClientResponseType(typeof (List<TestSuite>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test_suites_testCaseId-_testcaseId_.json", null, null, null)]
    public HttpResponseMessage GetSuitesByTestCaseId(int testCaseId) => this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.GetSuitesByTestCaseId(testCaseId));

    [HttpGet]
    [ActionName("Suites")]
    [ClientResponseType(typeof (IList<TestSuite>), null, null)]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [ClientExample("GET__test__projectName__plans__planId__suites.json", null, null, null)]
    public HttpResponseMessage GetTestSuitesForPlan(
      int planId,
      [FromUri(Name = "$expand")] int expand = 0,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      [FromUri(Name = "$asTreeView")] bool asTreeView = false)
    {
      return this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.GetTestSuitesForPlan(this.ProjectId.ToString(), planId, expand, skip, top, asTreeView));
    }

    [HttpGet]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId_.json", null, null, null)]
    public TestSuite GetTestSuiteById(int planId, int suiteId, [FromUri(Name = "$expand")] int expand = 0) => this.SuitesHelper.GetTestSuiteById(this.ProjectId.ToString(), planId, suiteId, expand);

    [HttpPatch]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [ClientExample("PATCH_testmanagement_testsuite_rename_id.json", null, null, null)]
    public TestSuite UpdateTestSuite(int planId, int suiteId, SuiteUpdateModel suiteUpdateModel)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.SuitesHelper.PatchTestSuite(this.ProjectId.ToString(), planId, suiteId, suiteUpdateModel, true);
    }

    [HttpDelete]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [ClientExample("DELETE_testmanagement_testsuite_id.json", null, null, null)]
    public void DeleteTestSuite(int planId, int suiteId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.SuitesHelper.DeleteTestSuite(this.ProjectId.ToString(), planId, suiteId);
    }

    [HttpPost]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [ClientResponseType(typeof (List<TestSuite>), null, null)]
    [ClientExample("POST_testmanagement_static_testsuite.json", "Static suite", null, null)]
    [ClientExample("POST_testmanagement_query_testsuite.json", "Based on a query", null, null)]
    [ClientExample("POST_testmanagement_requirement_testsuite.json", "Based on requirements", null, null)]
    public HttpResponseMessage CreateTestSuite(int planId, int suiteId, SuiteCreateModel testSuite)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.CreateTestSuite(this.ProjectId.ToString(), planId, suiteId, testSuite));
    }

    [HttpGet]
    [ActionName("TestCases")]
    [ClientResponseType(typeof (IList<SuiteTestCase>), null, null)]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientExample("testcases.json", null, null, null)]
    public HttpResponseMessage GetTestCases(int planId, int suiteId) => this.GenerateResponse<SuiteTestCase>((IEnumerable<SuiteTestCase>) this.SuiteTestCaseHelper.GetTestCases(this.ProjectId.ToString(), planId, suiteId));

    [HttpGet]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientExample("GET_testmanagement_testsuite_testcase_id.json", null, null, null)]
    public SuiteTestCase GetTestCaseById(int planId, int suiteId, int testCaseIds) => this.SuiteTestCaseHelper.GetTestCaseById(this.ProjectId.ToString(), planId, suiteId, testCaseIds);

    [HttpDelete]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientExample("DELETE_testmanagement_testsuite_testcase_ids.json", null, null, null)]
    public void RemoveTestCasesFromSuiteUrl(int planId, int suiteId, string testCaseIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.SuiteTestCaseHelper.RemoveTestCasesFromSuite(this.ProjectId.ToString(), planId, suiteId, testCaseIds);
    }

    [HttpPost]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientResponseType(typeof (List<SuiteTestCase>), null, null)]
    [ClientExample("POST_testmanagement_testsuite_testcase_ids.json", null, null, null)]
    public HttpResponseMessage AddTestCasesToSuite(int planId, int suiteId, string testCaseIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.GenerateResponse<SuiteTestCase>((IEnumerable<SuiteTestCase>) this.SuiteTestCaseHelper.AddTestCasesToSuite(this.ProjectId.ToString(), planId, suiteId, testCaseIds));
    }

    [HttpPatch]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientExample("PATCH_testmanagement_testsuite_testcase_ids.json", null, null, null)]
    public List<SuiteTestCase> UpdateSuiteTestCases(
      int planId,
      int suiteId,
      string testCaseIds,
      SuiteTestCaseUpdateModel suiteTestCaseUpdateModel)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.SuiteTestCaseHelper.UpdateSuiteTestCases(this.ProjectId.ToString(), planId, suiteId, testCaseIds, suiteTestCaseUpdateModel);
    }
  }
}
