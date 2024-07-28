// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.ReleaseRefController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "releaseref2", ResourceVersion = 1)]
  public class ReleaseRefController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("32B77C5B-D44D-4B2E-A30F-9F6414E71413")]
    public void SyncReleaseRef(IEnumerable<ReleaseReference2> references) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncReleaseRefs(this.TestManagementRequestContext, references);
  }
}
