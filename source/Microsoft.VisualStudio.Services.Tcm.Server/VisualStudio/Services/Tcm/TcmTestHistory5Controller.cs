// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestHistory5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "TestHistory", ResourceVersion = 1)]
  [AccessReadConsistencyFilter("TestManagement.Server.EnableSqlReadReplicaUsageInTcm")]
  public class TcmTestHistory5Controller : TcmControllerBase
  {
    private ITeamFoundationTestManagementResultService testManagementResultService;

    [HttpPost]
    [ClientLocationId("2A41BD6A-8118-4403-B74E-5BA7492AED9D")]
    [PublicProjectRequestRestrictions]
    public TestHistoryQuery QueryTestHistory(TestHistoryQuery filter) => this.TestManagementResultService.QueryTestHistory(this.TestManagementRequestContext, this.ProjectInfo, filter);

    internal ITeamFoundationTestManagementResultService TestManagementResultService
    {
      get
      {
        if (this.testManagementResultService == null)
          this.testManagementResultService = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
        return this.testManagementResultService;
      }
    }
  }
}
