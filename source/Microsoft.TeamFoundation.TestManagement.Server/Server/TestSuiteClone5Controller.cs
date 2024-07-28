// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteClone5Controller
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
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestSuiteClone", ResourceVersion = 2)]
  public class TestSuiteClone5Controller : TestManagementController
  {
    [HttpPost]
    [ClientLocationId("181d4c97-0e98-4ee2-ad6a-4cada675e555")]
    public CloneTestSuiteOperationInformation CloneTestSuite(
      CloneTestSuiteParams cloneRequestBody,
      bool deepClone = false)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<CloneTestSuiteParams>(cloneRequestBody, nameof (cloneRequestBody), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<SourceTestSuiteInfo>(cloneRequestBody.sourceTestSuite, "cloneRequestBody.sourceTestSuite", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<DestinationTestSuiteInfo>(cloneRequestBody.destinationTestSuite, "cloneRequestBody.destinationTestSuite", this.TfsRequestContext.ServiceName);
      return this.RevisedTestSuitesHelper.BeginCloneOfNewTestSuite(this.ProjectInfo.Name, cloneRequestBody.sourceTestSuite.id, cloneRequestBody.destinationTestSuite.id, cloneRequestBody.destinationTestSuite.Project, cloneRequestBody.cloneOptions, deepClone);
    }

    [HttpGet]
    [ClientLocationId("181d4c97-0e98-4ee2-ad6a-4cada675e555")]
    public CloneTestSuiteOperationInformation GetSuiteCloneInformation(int cloneOperationId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.RevisedTestSuitesHelper.GetCloneInformation(cloneOperationId, this.ProjectInfo.Name);
    }
  }
}
