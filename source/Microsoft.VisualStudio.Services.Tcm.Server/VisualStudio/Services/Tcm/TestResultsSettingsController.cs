// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestResultsSettingsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "settings", ResourceVersion = 3)]
  public class TestResultsSettingsController : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("7319952E-E5A9-4E19-A006-84F3BE8B7C68")]
    [FeatureEnabled("TestManagement.Server.EnableTestProjectSettings")]
    public TestResultsSettings GetTestResultsSettings(TestResultsSettingsType settingsType = TestResultsSettingsType.All) => this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementTestResultsSettingsService>().GetTestResultsSettings(this.TestManagementRequestContext, this.ProjectInfo, settingsType);

    [HttpPatch]
    [ClientLocationId("7319952E-E5A9-4E19-A006-84F3BE8B7C68")]
    [FeatureEnabled("TestManagement.Server.EnableTestProjectSettings")]
    public TestResultsSettings UpdatePipelinesTestSettings(
      TestResultsUpdateSettings testResultsUpdateSettings)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementTestResultsSettingsService>().UpdateTestResultsSettings(this.TestManagementRequestContext, this.ProjectInfo, testResultsUpdateSettings);
    }
  }
}
