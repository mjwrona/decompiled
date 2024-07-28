// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestResultTrendForBuildController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "ResultTrendByBuild", ResourceVersion = 1)]
  public class TcmTestResultTrendForBuildController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpPost]
    [ClientLocationId("0886A7AE-315A-4DBA-9122-BCCE93301F3A")]
    [PublicProjectRequestRestrictions]
    public List<AggregatedDataForResultTrend> QueryResultTrendForBuild(TestResultTrendFilter filter) => this.TestReportsHelper.QueryTestResultTrendForBuild(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
