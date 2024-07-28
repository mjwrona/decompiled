// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultHistoryController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(3.0)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "History", ResourceVersion = 1)]
  public class TestResultHistoryController : TestResultsControllerBase
  {
    private ITestReportsHelper _testReportsHelper;
    private const int c_defaultDays = 7;

    [HttpPost]
    [ClientLocationId("234616F5-429C-4E7B-9192-AFFD76731DFD")]
    public TestResultHistory QueryTestResultHistory(ResultsFilter filter) => !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.TestReportsHelper.QueryTestCaseResultHistory(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter) : TestManagementController.InvokeAction<TestResultHistory>((Func<TestResultHistory>) (() => this.TestResultsHttpClient.QueryTestResultHistoryAsync(filter, this.ProjectId)?.Result));

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
