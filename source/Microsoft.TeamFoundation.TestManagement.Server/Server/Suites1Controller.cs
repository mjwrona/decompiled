// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Suites1Controller
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
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Suites", ResourceVersion = 1)]
  public class Suites1Controller : TestManagementController
  {
    [HttpGet]
    [ActionName("GetTestSuitesForTestCase")]
    [ClientLocationId("09a6167b-e969-4775-9247-b94cf3819caf")]
    [ClientResponseType(typeof (List<TestSuite>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetSuitesByTestCaseId(int testCaseId) => this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.GetSuitesByTestCaseId(testCaseId));

    [HttpGet]
    [ActionName("Suites")]
    [ClientResponseType(typeof (IList<TestSuite>), null, null)]
    [ClientLocationId("82243633-baf3-423d-8cbd-b272a469febe")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestSuitesForPlan(
      string projectId,
      int planId,
      bool includeSuites = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.GetTestSuitesForPlan(projectId, planId, includeSuites, skip, top, false));
    }

    [HttpGet]
    [ActionName("Suites")]
    [ClientLocationId("82243633-baf3-423d-8cbd-b272a469febe")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public TestSuite GetTestSuiteById(
      string projectId,
      int planId,
      int suiteId,
      bool includeChildSuites = false)
    {
      return this.SuitesHelper.GetTestSuiteById(projectId, planId, suiteId, includeChildSuites, false);
    }

    [HttpDelete]
    [ActionName("Suites")]
    [ClientLocationId("82243633-baf3-423d-8cbd-b272a469febe")]
    public void DeleteTestSuite(string projectId, int planId, int suiteId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.SuitesHelper.DeleteTestSuite(projectId, planId, suiteId);
    }

    [HttpPost]
    [ActionName("Suites")]
    [ClientLocationId("82243633-baf3-423d-8cbd-b272a469febe")]
    [ClientResponseType(typeof (List<TestSuite>), null, null)]
    public HttpResponseMessage CreateTestSuite(
      string projectId,
      int planId,
      int suiteId,
      SuiteCreateModel testSuite)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.CreateTestSuite(projectId, planId, suiteId, testSuite));
    }

    [HttpPatch]
    [ActionName("Suites")]
    [ClientLocationId("82243633-baf3-423d-8cbd-b272a469febe")]
    public TestSuite PatchTestSuite(
      string projectId,
      int planId,
      int suiteId,
      SuiteUpdateModel suiteUpdateModel)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.SuitesHelper.PatchTestSuite(projectId, planId, suiteId, suiteUpdateModel, false);
    }

    [HttpGet]
    [ActionName("TestCases")]
    [ClientResponseType(typeof (IList<SuiteTestCase>), null, null)]
    [ClientLocationId("F91D0D0B-E292-4132-B818-2503BB2847C2")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestCases(string projectId, int planId, int suiteId) => this.GenerateResponse<SuiteTestCase>((IEnumerable<SuiteTestCase>) this.SuiteTestCaseHelper.GetTestCases(projectId, planId, suiteId));

    [HttpGet]
    [ActionName("TestCases")]
    [ClientLocationId("F91D0D0B-E292-4132-B818-2503BB2847C2")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public SuiteTestCase GetTestCaseById(
      string projectId,
      int planId,
      int suiteId,
      int testCaseIds)
    {
      return this.SuiteTestCaseHelper.GetTestCaseById(projectId, planId, suiteId, testCaseIds);
    }

    [HttpDelete]
    [ActionName("TestCases")]
    [ClientLocationId("F91D0D0B-E292-4132-B818-2503BB2847C2")]
    public void RemoveTestCasesFromSuiteUrl(
      string projectId,
      int planId,
      int suiteId,
      string testCaseIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.SuiteTestCaseHelper.RemoveTestCasesFromSuite(projectId, planId, suiteId, testCaseIds);
    }

    [HttpPost]
    [ActionName("TestCases")]
    [ClientLocationId("F91D0D0B-E292-4132-B818-2503BB2847C2")]
    [ClientResponseType(typeof (List<SuiteTestCase>), null, null)]
    public HttpResponseMessage AddTestCasesToSuite(
      string projectId,
      int planId,
      int suiteId,
      string testCaseIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.GenerateResponse<SuiteTestCase>((IEnumerable<SuiteTestCase>) this.SuiteTestCaseHelper.AddTestCasesToSuite(projectId, planId, suiteId, testCaseIds));
    }
  }
}
