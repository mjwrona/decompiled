// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestResultHistoryController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "History", ResourceVersion = 1)]
  public class TcmTestResultHistoryController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;
    private const int c_defaultDays = 7;

    [HttpPost]
    [ClientLocationId("BDF7A97B-0395-4DA8-9D5D-F957619327D1")]
    public TestResultHistory QueryTestResultHistory(ResultsFilter filter) => this.TestReportsHelper.QueryTestCaseResultHistory(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
