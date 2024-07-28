// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.BuildRefController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "builderef2", ResourceVersion = 1)]
  public class BuildRefController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("A8710559-B314-4C4A-AD18-94A0011E2CA2")]
    public void SyncBuildRef(IEnumerable<BuildReference2> references) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncBuildRefs(this.TestManagementRequestContext, references);
  }
}
