// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TCMServiceMigrationController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "TCMServiceMigration", ResourceName = "tcmservicemigration", ResourceVersion = 1)]
  public class TCMServiceMigrationController : TestManagementController
  {
    private TCMServiceMigrationHelper m_migrationHelper;

    [HttpGet]
    [ClientLocationId("F79DAAD9-7A92-4FB0-A1BD-DB8EC573E013")]
    public IEnumerable<KeyValuePair<string, int>> GetMigrationThreshold() => this.MigrationHelper.GetMigrationThreshold();

    internal TCMServiceMigrationHelper MigrationHelper
    {
      get
      {
        if (this.m_migrationHelper == null)
          this.m_migrationHelper = new TCMServiceMigrationHelper(this.TestManagementRequestContext);
        return this.m_migrationHelper;
      }
    }
  }
}
