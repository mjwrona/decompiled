// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultTrendForReleaseController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultTrendByRelease", ResourceVersion = 1)]
  public class TestResultTrendForReleaseController : TestResultsControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpPost]
    [ClientLocationId("DD178E93-D8DD-4887-9635-D6B9560B7B6E")]
    [PublicProjectRequestRestrictions]
    public List<AggregatedDataForResultTrend> QueryResultTrendForRelease(
      TestResultTrendFilter filter)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.TestReportsHelper.QueryTestResultTrendForRelease(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);
      List<AggregatedDataForResultTrend> result = TestManagementController.InvokeAction<List<AggregatedDataForResultTrend>>((Func<List<AggregatedDataForResultTrend>>) (() => this.TestResultsHttpClient.QueryResultTrendForReleaseAsync(filter, this.ProjectId, (object) null, new CancellationToken())?.Result));
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
