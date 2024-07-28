// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.TestResult2Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "testresult2", ResourceVersion = 1)]
  public class TestResult2Controller : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("AEB42FC5-2E0B-4229-9BE9-C8F84894A0F1")]
    public void SyncTestResult(IEnumerable<TestResult2> testResults) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncTestResults(this.TestManagementRequestContext, testResults);
  }
}
