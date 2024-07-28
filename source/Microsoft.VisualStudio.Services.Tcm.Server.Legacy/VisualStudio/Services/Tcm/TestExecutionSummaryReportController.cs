// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestExecutionSummaryReportController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "testexecutionsummaryreport", ResourceVersion = 1)]
  [AccessReadConsistencyFilter("TestManagement.Server.EnableSqlReadReplicaUsageInTcm")]
  public class TestExecutionSummaryReportController : TcmControllerBase
  {
    [HttpPost]
    [ClientLocationId("C012317D-845C-4E0E-B93E-56C3FCEDB95A")]
    public TestExecutionReportData GetTestExecutionSummaryReport(
      int planId,
      List<TestAuthoringDetails> testAuthoringDetails,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string dimensionList = "")
    {
      List<string> commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(dimensionList);
      return this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementChartingService>().GetTestExecutionSummaryReport(this.TestManagementRequestContext, this.ProjectId, planId, testAuthoringDetails, commaSeparatedString);
    }
  }
}
