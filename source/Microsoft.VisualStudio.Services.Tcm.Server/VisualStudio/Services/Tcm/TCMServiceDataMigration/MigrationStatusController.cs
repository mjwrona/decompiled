// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.MigrationStatusController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "migrationstatus", ResourceVersion = 1)]
  public class MigrationStatusController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("58935008-F4A4-4F3B-B2B8-1122F20321E1")]
    public void SyncMigrationStatus(TCMServiceDataMigrationStatus migrationStatus) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncMigrationStatus(this.TestManagementRequestContext, migrationStatus);
  }
}
