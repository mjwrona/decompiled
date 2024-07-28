// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.RequirementsToTestsMappingController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "requirementstotestsmapping2", ResourceVersion = 1)]
  public class RequirementsToTestsMappingController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("E048EA20-C08E-48CF-82E7-3B930F4CF2A0")]
    public void SyncRequirementsToTestsMapping(
      IEnumerable<RequirementsToTestsMapping2> requirementsMapping)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncRequirementsToTestsMapping(this.TestManagementRequestContext, requirementsMapping);
    }
  }
}
