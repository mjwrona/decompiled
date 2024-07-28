// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyTestHistory5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "TestHistory", ResourceVersion = 1)]
  public class LegacyTestHistory5Controller : TcmControllerBase
  {
    private ITeamFoundationTestManagementResultService testManagementResultService;

    [HttpPost]
    [ClientLocationId("98A1887B-E629-4744-A62E-99E8F0E17704")]
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
