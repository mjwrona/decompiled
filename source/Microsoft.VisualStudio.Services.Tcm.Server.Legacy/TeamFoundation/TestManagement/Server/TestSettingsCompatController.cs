// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSettingsCompatController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "testsettingscompat", ResourceVersion = 1)]
  public class TestSettingsCompatController : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("06C36C34-DD95-4FFD-BE7F-70135FCDF4EC")]
    public LegacyTestSettings GetTestSettingsCompatById(int testSettingsId) => this.TestManagementRequestContext.RequestContext.GetService<ITestSettingsService>().GetTestSettingsById(this.TestManagementRequestContext, this.ProjectId, testSettingsId);

    [HttpPost]
    [ClientLocationId("898FBF7E-D847-4145-BCB8-F6E887A4EC67")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties CreateTestSettingsCompat(
      LegacyTestSettings legacyTestSettings)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITestSettingsService>().CreateTestSettings(this.TestManagementRequestContext, this.ProjectId, legacyTestSettings);
    }

    [HttpPost]
    [ClientLocationId("E87C5C1A-1109-4F47-AFC7-9205095F3B5A")]
    public List<LegacyTestSettings> QueryTestSettings(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query, bool omitSettings) => this.TestManagementRequestContext.RequestContext.GetService<ITestSettingsService>().QueryTestSettings(this.TestManagementRequestContext, query, omitSettings);

    [HttpPost]
    [ClientLocationId("6133DDC0-A54A-4ECA-B0D7-B86FA87D48BA")]
    public int QueryTestSettingsCount(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query) => this.TestManagementRequestContext.RequestContext.GetService<ITestSettingsService>().QueryTestSettingsCount(this.TestManagementRequestContext, query);

    [HttpPatch]
    [ClientLocationId("06C36C34-DD95-4FFD-BE7F-70135FCDF4EC")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestSettings(
      LegacyTestSettings legacyTestSettings)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITestSettingsService>().UpdateTestSettings(this.TestManagementRequestContext, this.ProjectId, legacyTestSettings);
    }
  }
}
