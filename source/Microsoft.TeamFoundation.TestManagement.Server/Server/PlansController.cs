// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PlansController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Plans", ResourceVersion = 2)]
  public class PlansController : Plans1Controller
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>), null, null)]
    [ClientLocationId("51712106-7278-4208-8563-1C96F40CF5E4")]
    [ClientExample("GET__test__projectName__plans_includePlanDetails-true.json", "Get list of test plans with their details.", null, null)]
    [ClientExample("GET__test__projectName__plans_filterActivePlans-true.json", "Get a list of active test plans.", null, null)]
    [ClientExample("GET__test__projectName__plans__top-3.json", "Get top 3 plans.", null, null)]
    public HttpResponseMessage GetPlans(
      string owner = "",
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      bool includePlanDetails = false,
      bool filterActivePlans = false)
    {
      return this.GetPlans(this.ProjectId.ToString(), owner, skip, top, includePlanDetails, filterActivePlans);
    }

    [HttpGet]
    [ClientLocationId("51712106-7278-4208-8563-1C96F40CF5E4")]
    [ClientExample("GET_testmanagement_testplan_id.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan GetPlanById(int planId) => this.GetPlanById(this.ProjectId.ToString(), planId);

    [HttpPost]
    [ClientLocationId("51712106-7278-4208-8563-1C96F40CF5E4")]
    [ClientExample("POST__test__plans_description.json", "Create a test plan with a description.", null, null)]
    [ClientExample("POST__test__plans_areaiteration.json", "Create a test plan in an area and iteration.", null, null)]
    [ClientExample("POST__test__plans_startdateenddate.json", "Create a test plan with start date and end date.", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan CreateTestPlan(
      PlanUpdateModel testPlan)
    {
      return this.CreateTestPlan(this.ProjectId.ToString(), testPlan);
    }

    [HttpPatch]
    [ClientLocationId("51712106-7278-4208-8563-1C96F40CF5E4")]
    [ClientExample("PATCH__test__projectName__plans__planId_.json", "Update name of a test plan.", null, null)]
    [ClientExample("PATCH__test__projectName__plans__planId_namedescription.json", "Update name and description of a test plan.", null, null)]
    [ClientExample("PATCH__test__projectName__plans__planId_2areaiteration.json", "Update area and iteration of a test plan.", null, null)]
    [ClientExample("PATCH__test__projectName__plans__planId_23state.json", "Update state of a test plan.", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan UpdateTestPlan(
      int planId,
      PlanUpdateModel planUpdateModel)
    {
      return this.PatchTestPlan(this.ProjectId.ToString(), planId, planUpdateModel);
    }

    [HttpDelete]
    [ClientLocationId("51712106-7278-4208-8563-1C96F40CF5E4")]
    [ClientExample("DELETE__test_plans__planId_.json", null, null, null)]
    public void DeleteTestPlan(int planId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.PlansHelper.DeleteTestPlanV2(this.ProjectId.ToString(), planId);
    }
  }
}
