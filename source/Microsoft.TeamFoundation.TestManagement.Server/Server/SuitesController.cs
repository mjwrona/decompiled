// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuitesController
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Suites", ResourceVersion = 2)]
  public class SuitesController : Suites1Controller
  {
    [HttpGet]
    [ActionName("Suites")]
    [ClientResponseType(typeof (IList<TestSuite>), null, null)]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    public virtual HttpResponseMessage GetTestSuitesForPlan(
      int planId,
      bool includeSuites = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      [FromUri(Name = "$asTreeView")] bool asTreeView = false)
    {
      return this.GenerateResponse<TestSuite>((IEnumerable<TestSuite>) this.SuitesHelper.GetTestSuitesForPlan(this.ProjectId.ToString(), planId, includeSuites, skip, top, false, asTreeView));
    }

    [HttpGet]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public virtual TestSuite GetTestSuiteById(int planId, int suiteId, bool includeChildSuites = false) => this.GetTestSuiteById(this.ProjectId.ToString(), planId, suiteId, includeChildSuites);

    [HttpDelete]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    public void DeleteTestSuite(int planId, int suiteId) => this.DeleteTestSuite(this.ProjectId.ToString(), planId, suiteId);

    [HttpPost]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    [ClientResponseType(typeof (List<TestSuite>), null, null)]
    public HttpResponseMessage CreateTestSuite(int planId, int suiteId, SuiteCreateModel testSuite) => this.CreateTestSuite(this.ProjectId.ToString(), planId, suiteId, testSuite);

    [HttpPatch]
    [ActionName("Suites")]
    [ClientLocationId("7B7619A0-CB54-4AB3-BF22-194056F45DD1")]
    public virtual TestSuite UpdateTestSuite(
      int planId,
      int suiteId,
      SuiteUpdateModel suiteUpdateModel)
    {
      return this.PatchTestSuite(this.ProjectId.ToString(), planId, suiteId, suiteUpdateModel);
    }

    [HttpGet]
    [ActionName("TestCases")]
    [ClientResponseType(typeof (IList<SuiteTestCase>), null, null)]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    public HttpResponseMessage GetTestCases(int planId, int suiteId) => this.GetTestCases(this.ProjectId.ToString(), planId, suiteId);

    [HttpGet]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    public SuiteTestCase GetTestCaseById(int planId, int suiteId, int testCaseIds) => this.GetTestCaseById(this.ProjectId.ToString(), planId, suiteId, testCaseIds);

    [HttpDelete]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    public void RemoveTestCasesFromSuiteUrl(int planId, int suiteId, string testCaseIds) => this.RemoveTestCasesFromSuiteUrl(this.ProjectId.ToString(), planId, suiteId, testCaseIds);

    [HttpPost]
    [ActionName("TestCases")]
    [ClientLocationId("A4A1EC1C-B03F-41CA-8857-704594ECF58E")]
    [ClientResponseType(typeof (List<SuiteTestCase>), null, null)]
    public HttpResponseMessage AddTestCasesToSuite(int planId, int suiteId, string testCaseIds) => this.AddTestCasesToSuite(this.ProjectId.ToString(), planId, suiteId, testCaseIds);
  }
}
