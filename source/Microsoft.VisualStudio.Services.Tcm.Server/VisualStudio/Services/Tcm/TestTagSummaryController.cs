// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestTagSummaryController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "tagsummary", ResourceVersion = 1)]
  [FeatureEnabled("TestManagement.Server.TagsInTestRun")]
  public class TestTagSummaryController : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("655a8f6b-fec7-4b46-b672-68b44141b498")]
    [ClientResponseType(typeof (TestTagSummary), null, null)]
    [PublicProjectRequestRestrictions]
    public async Task<TestTagSummary> GetTestTagSummaryForBuild(int buildId)
    {
      TestTagSummaryController summaryController = this;
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build
      };
      // ISSUE: explicit non-virtual call
      return (await summaryController.TfsRequestContext.GetService<ITestLogStoreService>().GetTestTagDetailForBuildOrRelease(summaryController.TestManagementRequestContext, __nonvirtual (summaryController.ProjectInfo), logReference).ConfigureAwait(false)).TestTagSummary;
    }

    [HttpGet]
    [ClientLocationId("655a8f6b-fec7-4b46-b672-68b44141b498")]
    [ClientResponseType(typeof (TestTagSummary), null, null)]
    [PublicProjectRequestRestrictions]
    public async Task<TestTagSummary> GetTestTagSummaryForRelease(int releaseId, int releaseEnvId)
    {
      TestTagSummaryController summaryController = this;
      TestLogReference logReference = new TestLogReference()
      {
        ReleaseId = releaseId,
        ReleaseEnvId = releaseEnvId,
        Scope = TestLogScope.Release
      };
      // ISSUE: explicit non-virtual call
      return (await summaryController.TfsRequestContext.GetService<ITestLogStoreService>().GetTestTagDetailForBuildOrRelease(summaryController.TestManagementRequestContext, __nonvirtual (summaryController.ProjectInfo), logReference).ConfigureAwait(false)).TestTagSummary;
    }
  }
}
