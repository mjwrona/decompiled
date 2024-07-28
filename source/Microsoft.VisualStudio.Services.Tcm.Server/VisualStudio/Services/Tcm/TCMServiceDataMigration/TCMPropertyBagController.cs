// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.TCMPropertyBagController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "tcmpropertybag2", ResourceVersion = 1)]
  public class TCMPropertyBagController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("5C7FEE2B-3D87-4D29-A0D3-63025E9E4ACA")]
    public void SyncTCMPropertyBag(IEnumerable<TCMPropertyBag2> tcmPropertyBag) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncTCMPropertyBag(this.TestManagementRequestContext, tcmPropertyBag);
  }
}
