// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.PointResultsController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceDataMigration", ResourceName = "pointresults2", ResourceVersion = 1)]
  public class PointResultsController : TcmDataMigrationControllerBase
  {
    [HttpPost]
    [ClientLocationId("67DA20AB-6E4C-4F24-A9A1-1B2DEEE46215")]
    public void SyncPointResults(IEnumerable<PointsResults2> pointResults) => Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.SyncPointsResults(this.TestManagementRequestContext, pointResults);

    [HttpPost]
    [ClientLocationId("58AC0A52-FB0F-404F-926F-5ED1A72C4C22")]
    public IEnumerable<PointsResults2> FetchPointResults(
      IEnumerable<PointsReference2> pointReferences,
      int batchSize)
    {
      return Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration.FetchPointsResults(this.TestManagementRequestContext, pointReferences);
    }
  }
}
