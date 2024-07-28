// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CloneController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "CloneOperation", ResourceVersion = 2)]
  public class CloneController : TestManagementController
  {
    [HttpPost]
    [ClientLocationId("edc3ef4b-8460-4e86-86fa-8e4f5e9be831")]
    [ClientExample("POST__test__projectName__plans__clone.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation CloneTestPlan(
      int planId,
      TestPlanCloneRequest cloneRequestBody)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<TestPlanCloneRequest>(cloneRequestBody, nameof (cloneRequestBody), this.TfsRequestContext.ServiceName);
      return this.PlansHelper.CreateAndBeginCloneOfTestPlan(this.ProjectInfo.Name, planId, cloneRequestBody.DestinationTestPlan, cloneRequestBody.SuiteIds, cloneRequestBody.Options);
    }

    [HttpPost]
    [ClientLocationId("751e4ab5-5bf6-4fb5-9d5d-19ef347662dd")]
    [ClientExample("POST__test__projectName__suites__clone.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation CloneTestSuite(
      int planId,
      int sourceSuiteId,
      TestSuiteCloneRequest cloneRequestBody)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<TestSuiteCloneRequest>(cloneRequestBody, nameof (cloneRequestBody), this.TfsRequestContext.ServiceName);
      return this.SuitesHelper.BeginCloneOfTestSuite(this.ProjectInfo.Name, sourceSuiteId, planId, cloneRequestBody.DestinationSuiteId, cloneRequestBody.DestinationSuiteProjectName, cloneRequestBody.CloneOptions);
    }

    [HttpGet]
    [ClientLocationId("5b9d6320-abed-47a5-a151-cd6dc3798be6")]
    [ClientExample("GET__test__projectName__cloneOperation.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation GetCloneInformation(
      int cloneOperationId,
      [FromUri(Name = "$includeDetails")] bool includeDetails = false)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.SuitesHelper.GetCloneInformation(cloneOperationId, this.ProjectInfo.Name, includeDetails);
    }
  }
}
