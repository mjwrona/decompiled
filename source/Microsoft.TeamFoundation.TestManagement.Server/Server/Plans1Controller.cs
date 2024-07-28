// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Plans1Controller
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
  [ClientGroupByResource("Test Plans")]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Plans", ResourceVersion = 1)]
  public class Plans1Controller : TestManagementController
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>), null, null)]
    [ClientLocationId("72493ce2-021d-42c4-a9c9-e60d3335d27f")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetPlans(
      string projectId,
      string owner = "",
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      bool includePlanDetails = false,
      bool filterActivePlans = false)
    {
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>) this.PlansHelper.GetPlans(projectId, owner, skip, top, includePlanDetails, filterActivePlans));
    }

    [HttpGet]
    [ClientLocationId("72493ce2-021d-42c4-a9c9-e60d3335d27f")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan GetPlanById(
      string projectId,
      int planId)
    {
      return this.PlansHelper.GetTestPlanById(projectId, planId);
    }

    [HttpPost]
    [ClientLocationId("72493ce2-021d-42c4-a9c9-e60d3335d27f")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan CreateTestPlan(
      string projectId,
      PlanUpdateModel testPlan)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.PlansHelper.CreateTestPlan(projectId, testPlan);
    }

    [HttpPatch]
    [ClientLocationId("72493ce2-021d-42c4-a9c9-e60d3335d27f")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan PatchTestPlan(
      string projectId,
      int planId,
      PlanUpdateModel planUpdateModel)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.PlansHelper.PatchTestPlan(projectId, planId, planUpdateModel);
    }
  }
}
