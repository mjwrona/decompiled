// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteTestCase6_1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "SuiteTestCase", ResourceVersion = 3)]
  public class SuiteTestCase6_1Controller : SuiteTestCase5Controller
  {
    [HttpDelete]
    [ClientLocationId("a9bd61ac-45cf-4d13-9441-43dcd01edf8d")]
    public void RemoveTestCasesListFromSuite(int planId, int suiteId, string testIds)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.RevisedTestSuitesHelper.RemoveTestCasesFromSuite(this.ProjectInfo, planId, suiteId, testIds);
    }
  }
}
