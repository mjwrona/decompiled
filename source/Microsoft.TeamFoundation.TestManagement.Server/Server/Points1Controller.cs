// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Points1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Points", ResourceVersion = 1)]
  public class Points1Controller : TestManagementController
  {
    private PointsHelper m_pointsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>), null, null)]
    [ClientLocationId("3ecbe2f1-c419-4d6c-be9e-d2919bc7e581")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetPoints(
      string projectId,
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
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>) this.PointsHelper.GetPoints(projectId, planId, suiteId, witFields, configurationId, testCaseId, testPointIds, includePointDetails, skip, top));
    }

    [HttpGet]
    [ClientLocationId("3ecbe2f1-c419-4d6c-be9e-d2919bc7e581")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint GetPoint(
      string projectId,
      int planId,
      int suiteId,
      int pointIds,
      string witFields = "")
    {
      string pointId = pointIds.ToString();
      return this.PointsHelper.GetPointById(projectId, planId, suiteId, pointId, witFields).ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(0);
    }

    [HttpPatch]
    [ClientLocationId("3ecbe2f1-c419-4d6c-be9e-d2919bc7e581")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> PatchTestPoints(
      string projectId,
      int planId,
      int suiteId,
      string pointIds,
      PointUpdateModel pointUpdateModel)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.PointsHelper.PatchTestPoints(projectId, planId, suiteId, pointIds, pointUpdateModel);
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
