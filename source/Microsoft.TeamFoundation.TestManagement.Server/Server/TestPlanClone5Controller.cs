// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanClone5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestPlanClone", ResourceVersion = 2)]
  public class TestPlanClone5Controller : TestManagementController
  {
    [HttpPost]
    [ClientLocationId("e65df662-d8a3-46c7-ae1c-14e2d4df57e1")]
    public CloneTestPlanOperationInformation CloneTestPlan(
      CloneTestPlanParams cloneRequestBody,
      bool deepClone = false)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<CloneTestPlanParams>(cloneRequestBody, nameof (cloneRequestBody), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<SourceTestPlanInfo>(cloneRequestBody.sourceTestPlan, "cloneRequestBody.sourceTestPlan", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<DestinationTestPlanCloneParams>(cloneRequestBody.destinationTestPlan, "cloneRequestBody.destinationTestPlan", this.TfsRequestContext.ServiceName);
      return this.RevisedTestPlansHelper.CreateAndBeginNewCloneOfTestPlan(this.ProjectInfo.Name, cloneRequestBody.sourceTestPlan.id, cloneRequestBody.destinationTestPlan, cloneRequestBody.sourceTestPlan.suiteIds, cloneRequestBody.cloneOptions, deepClone);
    }

    [HttpGet]
    [ClientLocationId("e65df662-d8a3-46c7-ae1c-14e2d4df57e1")]
    public CloneTestPlanOperationInformation GetCloneInformation(int cloneOperationId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestPlansHelper.GetCloneInformation(cloneOperationId, this.ProjectInfo.Name);
    }
  }
}
