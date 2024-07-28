// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyTestResultHistoryController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "History", ResourceVersion = 1)]
  public class LegacyTestResultHistoryController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;
    private const int c_defaultDays = 7;

    [HttpPost]
    [ClientLocationId("EB9D9B32-4CCB-4C70-A72A-C7BA31D92FD2")]
    public TestResultHistory QueryTestResultHistory(ResultsFilter filter) => this.TestReportsHelper.QueryTestCaseResultHistory(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), filter);

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
