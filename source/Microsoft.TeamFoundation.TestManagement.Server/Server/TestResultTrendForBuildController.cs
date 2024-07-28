// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultTrendForBuildController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultTrendByBuild", ResourceVersion = 1)]
  [DemandFeature("D104EA57-16EA-4191-9B60-160D664EE9A8", true)]
  public class TestResultTrendForBuildController : TestResultsControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpPost]
    [ClientLocationId("FBC82A85-0786-4442-88BB-EB0FDA6B01B0")]
    [PublicProjectRequestRestrictions]
    public List<AggregatedDataForResultTrend> QueryResultTrendForBuild(TestResultTrendFilter filter)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.TestReportsHelper.QueryTestResultTrendForBuild(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);
      List<AggregatedDataForResultTrend> result = TestManagementController.InvokeAction<List<AggregatedDataForResultTrend>>((Func<List<AggregatedDataForResultTrend>>) (() => this.TestResultsHttpClient.QueryResultTrendForBuildAsync(filter, this.ProjectId, (object) null, new CancellationToken())?.Result));
      this.TestReportsHelper.SecureTestResultTrend(result, this.ProjectId);
      return result;
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
