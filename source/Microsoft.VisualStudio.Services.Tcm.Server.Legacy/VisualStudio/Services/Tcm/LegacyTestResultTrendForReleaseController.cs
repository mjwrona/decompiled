// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyTestResultTrendForReleaseController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "ResultTrendByRelease", ResourceVersion = 1)]
  public class LegacyTestResultTrendForReleaseController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpPost]
    [ClientLocationId("79376647-DD74-4B15-B0D4-243E332E169B")]
    [PublicProjectRequestRestrictions]
    public List<AggregatedDataForResultTrend> QueryResultTrendForRelease(
      TestResultTrendFilter filter)
    {
      return this.TestReportsHelper.QueryTestResultTrendForRelease(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
