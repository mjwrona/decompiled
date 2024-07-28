// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.TestRunController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "testrun2", ResourceVersion = 1)]
  public class TestRunController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("27DF61F5-6A07-4D72-B81C-8F16866D324F")]
    public void SyncTestRun(IEnumerable<TestRun2> testRuns) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncTestRuns(this.TestManagementRequestContext, testRuns);
  }
}
