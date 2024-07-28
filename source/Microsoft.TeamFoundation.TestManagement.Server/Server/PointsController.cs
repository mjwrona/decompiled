// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PointsController
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Points", ResourceVersion = 2)]
  public class PointsController : Points1Controller
  {
    private PointsHelper m_pointsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>), null, null)]
    [ClientLocationId("3BCFD5C8-BE62-488E-B1DA-B8289CE9299C")]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points_witFields-_fields_.json", "With fields", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points_configuration-_configuration_.json", "By configuration", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points_testcaseid-_testcaseId_.json", "By test case", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points_testPointIds-_testpointIds_.json", "Specific points", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points_includePointDetails-true.json", "With details", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points__skip-_skip___top-_top_.json", "A page at a time", null, null)]
    public HttpResponseMessage GetPoints(
      int planId,
      int suiteId,
      string witFields = "",
      string configurationId = "",
      string testCaseId = "",
      string testPointIds = "",
      bool includePointDetails = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return this.GetPoints(this.ProjectId.ToString(), planId, suiteId, witFields, configurationId, testCaseId, testPointIds, includePointDetails, skip, top);
    }

    [HttpGet]
    [ClientLocationId("3BCFD5C8-BE62-488E-B1DA-B8289CE9299C")]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points__pointId_.json", "By id", null, null)]
    [ClientExample("GET__test__projectName__plans__planId__suites__suiteId__points__pointId__witFields-_fields_.json", "With fields", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint GetPoint(
      int planId,
      int suiteId,
      int pointIds,
      string witFields = "")
    {
      return this.GetPoint(this.ProjectId.ToString(), planId, suiteId, pointIds, witFields);
    }

    [HttpPost]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("B4264FD0-A5D1-43E2-82A5-B9C46B7DA9CE")]
    [ClientExample("POST__test__projectName__points_.json", "By test case id", null, null)]
    [ClientExample("POST__test__projectName__points__configuration_.json", "With Configuration filter", null, null)]
    [ClientExample("POST__test__projectName__points__tester_.json", "With tester filter", null, null)]
    [ClientExample("POST__test__projectName__points__page_.json", "fetch a page using skip , top", null, null)]
    public TestPointsQuery GetPointsByQuery(TestPointsQuery query, [FromUri(Name = "$skip")] int skip = 0, [FromUri(Name = "$top")] int top = 1000) => this.TestManagementPointService.GetPointsByQuery(this.TestManagementRequestContext.RequestContext, this.ProjectId.ToString(), query, skip, top);

    [HttpPatch]
    [ClientLocationId("3BCFD5C8-BE62-488E-B1DA-B8289CE9299C")]
    [ClientExample("PATCH__test__projectName__plans__planId__suites__suiteId__points__pointId_.json", "Re-activate", null, null)]
    [ClientExample("PATCH__test__projectName__plans__planId__suites__suiteId__points__pointId_2.json", "Set the outcome", null, null)]
    [ClientExample("PATCH__test__projectName__plans__planId__suites__suiteId__points__pointId_23.json", "Change the tester", null, null)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> UpdateTestPoints(
      int planId,
      int suiteId,
      string pointIds,
      PointUpdateModel pointUpdateModel)
    {
      return this.PatchTestPoints(this.ProjectId.ToString(), planId, suiteId, pointIds, pointUpdateModel);
    }

    private PointsHelper PointsHelper
    {
      get
      {
        if (this.m_pointsHelper == null)
          this.m_pointsHelper = new PointsHelper(this.TestManagementRequestContext);
        return this.m_pointsHelper;
      }
    }
  }
}
