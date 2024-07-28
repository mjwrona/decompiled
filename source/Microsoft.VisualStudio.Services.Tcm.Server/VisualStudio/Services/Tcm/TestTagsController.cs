// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestTagsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "tags", ResourceVersion = 1)]
  [FeatureEnabled("TestManagement.Server.TagsInTestRun")]
  public class TestTagsController : TcmControllerBase
  {
    private ITeamFoundationTestManagementRunService testManagementRunService;

    [HttpGet]
    [ClientLocationId("52EE2057-4B54-41A6-A18C-ED4375A00F8D")]
    [ClientResponseType(typeof (IList<TestTag>), null, null)]
    [PublicProjectRequestRestrictions]
    public async Task<IList<TestTag>> GetTestTagsForBuild(int buildId)
    {
      TestTagsController testTagsController = this;
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build
      };
      // ISSUE: explicit non-virtual call
      IList<TestTagSecureObject> testTagSecureObjectList = await testTagsController.TfsRequestContext.GetService<ITestLogStoreService>().GetUniqueTestTagsForBuildOrRelease(testTagsController.TestManagementRequestContext, __nonvirtual (testTagsController.ProjectInfo), logReference).ConfigureAwait(false);
      List<TestTag> testTagsForBuild = new List<TestTag>();
      foreach (TestTagSecureObject testTagSecureObject in (IEnumerable<TestTagSecureObject>) testTagSecureObjectList)
        testTagsForBuild.Add(testTagSecureObject.TestTag);
      return (IList<TestTag>) testTagsForBuild;
    }

    [HttpGet]
    [ClientLocationId("52EE2057-4B54-41A6-A18C-ED4375A00F8D")]
    [ClientResponseType(typeof (IList<TestTag>), null, null)]
    [PublicProjectRequestRestrictions]
    public async Task<IList<TestTag>> GetTestTagsForRelease(int releaseId, int releaseEnvId)
    {
      TestTagsController testTagsController = this;
      TestLogReference logReference = new TestLogReference()
      {
        ReleaseId = releaseId,
        ReleaseEnvId = releaseEnvId,
        Scope = TestLogScope.Release
      };
      // ISSUE: explicit non-virtual call
      IList<TestTagSecureObject> testTagSecureObjectList = await testTagsController.TfsRequestContext.GetService<ITestLogStoreService>().GetUniqueTestTagsForBuildOrRelease(testTagsController.TestManagementRequestContext, __nonvirtual (testTagsController.ProjectInfo), logReference).ConfigureAwait(false);
      List<TestTag> testTagsForRelease = new List<TestTag>();
      foreach (TestTagSecureObject testTagSecureObject in (IEnumerable<TestTagSecureObject>) testTagSecureObjectList)
        testTagsForRelease.Add(testTagSecureObject.TestTag);
      return (IList<TestTag>) testTagsForRelease;
    }

    [HttpPatch]
    [ClientLocationId("A5E2F411-2B43-45F3-989C-05B71339F5B8")]
    [ClientResponseType(typeof (IList<TestTag>), null, null)]
    public async Task<IList<TestTag>> UpdateTestRunTags(
      int runId,
      TestTagsUpdateModel testTagsUpdateModel)
    {
      TestTagsController testTagsController = this;
      // ISSUE: explicit non-virtual call
      return (IList<TestTag>) await testTagsController.TestManagementRunService.UpdateTestRunTags(testTagsController.TestManagementRequestContext, __nonvirtual (testTagsController.ProjectInfo), runId, testTagsUpdateModel);
    }

    internal ITeamFoundationTestManagementRunService TestManagementRunService
    {
      get
      {
        if (this.testManagementRunService == null)
          this.testManagementRunService = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
        return this.testManagementRunService;
      }
    }
  }
}
