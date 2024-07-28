// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.Plans3Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  [ClientGroupByResource("Test Plans")]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "Plans", ResourceVersion = 1)]
  public class Plans3Controller : TestManagementController
  {
    [HttpPost]
    [ClientLocationId("0E292477-A0C2-47F3-A9B6-34F153D627F4")]
    [ClientExample("CreateTestPlanWithAreaPathAndIteration.json", "Create a test plan with name, area path and iteration.", null, null)]
    [ClientExample("CreateTestPlan.json", "Create a test plan with all details.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan CreateTestPlan(
      TestPlanCreateParams testPlanCreateParams)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestPlansHelper.CreateTestPlan(this.ProjectId.ToString(), testPlanCreateParams);
    }

    [HttpPatch]
    [ClientLocationId("0E292477-A0C2-47F3-A9B6-34F153D627F4")]
    [ClientExample("UpdateTestPlanWithAreaPathAndIteration.json", "Update test plan name, area and iteration.", null, null)]
    [ClientExample("UpdateTestPlan.json", "Update test plan details.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan UpdateTestPlan(
      int planId,
      TestPlanUpdateParams testPlanUpdateParams)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestPlansHelper.PatchTestPlan(this.ProjectId.ToString(), planId, testPlanUpdateParams);
    }

    [HttpGet]
    [ClientLocationId("0E292477-A0C2-47F3-A9B6-34F153D627F4")]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestPlans.json", "Get test plans.", null, null)]
    [ClientExample("GetActiveTestPlansFilteredByOwner.json", "Get active test plans filtered by owner.", null, null)]
    public HttpResponseMessage GetTestPlans(
      string owner = "",
      string continuationToken = null,
      bool includePlanDetails = false,
      bool filterActivePlans = false)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      bool generateContinuationToken;
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan> plans = this.RevisedTestPlansHelper.GetPlans(this.ProjectId.ToString(), out generateContinuationToken, owner, skipRows, topToFetch, watermark, includePlanDetails, filterActivePlans);
      continuationToken = (string) null;
      if (generateContinuationToken && plans != null && plans.Count > 0 && plans.Last<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>() != null)
      {
        continuationToken = Utils.GenerateContinuationToken(plans.Last<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>().Id, topRemaining);
        plans.RemoveAt(plans.Count - 1);
      }
      HttpResponseMessage response = this.GenerateResponse<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>((IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>) plans);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpGet]
    [ClientLocationId("0E292477-A0C2-47F3-A9B6-34F153D627F4")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestPlanById.json", "Get a test plan by id.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan GetTestPlanById(
      int planId)
    {
      return this.RevisedTestPlansHelper.GetTestPlanById(this.ProjectId.ToString(), planId);
    }

    [HttpDelete]
    [ClientLocationId("0E292477-A0C2-47F3-A9B6-34F153D627F4")]
    [ClientExample("DeletePlanById.json", "Delete a test plan.", null, null)]
    public void DeleteTestPlan(int planId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.RevisedTestPlansHelper.DeleteTestPlanV2(this.ProjectId.ToString(), planId);
    }
  }
}
