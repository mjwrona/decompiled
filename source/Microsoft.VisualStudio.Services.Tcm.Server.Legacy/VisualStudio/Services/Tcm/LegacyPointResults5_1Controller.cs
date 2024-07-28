// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyPointResults5_1Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "pointresults", ResourceVersion = 1)]
  [AccessReadConsistencyFilter("TestManagement.Server.EnableSqlReadReplicaUsageInTcm")]
  public class LegacyPointResults5_1Controller : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("C1DA87E8-68E7-46C4-9C64-BC449AB358DD")]
    public TestResultsWithWatermark GetManualTestResultsByUpdatedDate(
      DateTime updateDate,
      int fromRunId,
      int fromResultId)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITestManagementLegacyResultService>().GetManualTestResultsByUpdatedDate(this.TestManagementRequestContext, this.ProjectInfo.Id, new TestResultWatermark()
      {
        ChangedDate = updateDate,
        TestResultId = fromResultId,
        TestRunId = fromRunId
      });
    }

    [HttpPost]
    [ClientLocationId("905D7331-24BE-4CBF-AC54-70C2C838AB7A")]
    public List<PointLastResult> FilterPoints(FilterPointQuery request) => this.TestManagementRequestContext.RequestContext.GetService<ITestManagementLegacyResultService>().FilterPoints(this.TestManagementRequestContext, this.ProjectInfo.Id, request);
  }
}
