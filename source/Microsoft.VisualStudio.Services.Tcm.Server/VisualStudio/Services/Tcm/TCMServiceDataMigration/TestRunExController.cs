// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.TestRunExController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "testrunex2", ResourceVersion = 1)]
  public class TestRunExController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("EED0B1CB-1274-453D-AC25-14D47DFC90EE")]
    public void SyncTestRunEx(IEnumerable<TestRunEx2> testRunEx) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncTestRunEx(this.TestManagementRequestContext, testRunEx);
  }
}
