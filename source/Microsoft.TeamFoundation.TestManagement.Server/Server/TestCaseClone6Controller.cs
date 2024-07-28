// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseClone6Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestCaseClone", ResourceVersion = 2)]
  public class TestCaseClone6Controller : TestManagementController
  {
    [HttpPost]
    [ClientLocationId("529b2b8d-82f4-4893-b1e4-1e74ea534673")]
    public CloneTestCaseOperationInformation CloneTestCase(CloneTestCaseParams cloneRequestBody)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<CloneTestCaseParams>(cloneRequestBody, nameof (cloneRequestBody), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<TestPlanReference>(cloneRequestBody.sourceTestPlan, "cloneRequestBody.sourceTestPlan", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<TestPlanReference>(cloneRequestBody.destinationTestPlan, "cloneRequestBody.destinationTestPlan", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<SourceTestSuiteInfo>(cloneRequestBody.sourceTestSuite, "cloneRequestBody.sourceTestSuite", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<List<int>>(cloneRequestBody.testCaseIds, "cloneRequestBody.testCaseIds", this.TfsRequestContext.ServiceName);
      return this.TestCaseHelper.CreateAndBeginNewCloneOfTestCases(this.ProjectInfo.Name, cloneRequestBody.sourceTestPlan.Id, cloneRequestBody.destinationTestPlan.Id, cloneRequestBody.sourceTestSuite.id, cloneRequestBody.destinationTestSuite, cloneRequestBody.testCaseIds, cloneRequestBody.cloneOptions);
    }

    [HttpGet]
    [ClientLocationId("529b2b8d-82f4-4893-b1e4-1e74ea534673")]
    public CloneTestCaseOperationInformation GetTestCaseCloneInformation(int cloneOperationId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.TestCaseHelper.GetCloneInformation(cloneOperationId, this.ProjectInfo.Name);
    }
  }
}
