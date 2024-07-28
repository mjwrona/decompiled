// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultRestentionSettingsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultRetentionSettings", ResourceVersion = 1)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  public class ResultRestentionSettingsController : TestResultsControllerBase
  {
    private ResultRetentionSettingsHelper m_retentionSettingsHelper;

    [HttpGet]
    [ClientLocationId("a3206d9e-fa8d-42d3-88cb-f75c51e69cde")]
    [ClientExample("GET__test_resultretentionsettings.json", null, null, null)]
    public ResultRetentionSettings GetResultRetentionSettings() => this.RetentionSettingsHelper.Get();

    [HttpPatch]
    [ClientLocationId("a3206d9e-fa8d-42d3-88cb-f75c51e69cde")]
    [ClientExample("PATCH__test_resultretentionsettings.json", null, null, null)]
    public ResultRetentionSettings UpdateResultRetentionSettings(
      ResultRetentionSettings retentionSettings)
    {
      return this.RetentionSettingsHelper.Update(retentionSettings);
    }

    private ResultRetentionSettingsHelper RetentionSettingsHelper => this.m_retentionSettingsHelper ?? (this.m_retentionSettingsHelper = new ResultRetentionSettingsHelper(this.TestManagementRequestContext, new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id)));
  }
}
