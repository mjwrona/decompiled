// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TestSettings2Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "TCMServiceMigration", ResourceName = "testsettings2", ResourceVersion = 1)]
  public class TestSettings2Controller : TestManagementController
  {
    private ITeamFoundationTestManagementSettingsService m_testManagementSettingsService;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TestSettings2>), null, null)]
    [ClientLocationId("F64D9B94-AAD3-4460-89A6-0258726C2B46")]
    public HttpResponseMessage GetTestSettings([FromUri(Name = "$top")] int top = 1000, int continuationToken = 0)
    {
      try
      {
        IList<TestSettings2> testSettings = this.TestManagementSettingsService.GetTestSettings(this.TestManagementRequestContext, this.ProjectInfo.Name, ref top, continuationToken);
        HttpResponseMessage response = this.GenerateResponse<TestSettings2>((IEnumerable<TestSettings2>) testSettings);
        if (testSettings != null && testSettings.Count == top)
          Utils.SetContinuationToken(response, testSettings[testSettings.Count - 1].TestSettingsId.ToString());
        return response;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in TestSettingsController::GetTestSettings. Error = {0}.", (object) ex.ToString());
        throw;
      }
    }

    internal ITeamFoundationTestManagementSettingsService TestManagementSettingsService
    {
      get
      {
        if (this.m_testManagementSettingsService == null)
          this.m_testManagementSettingsService = this.TfsRequestContext.GetService<ITeamFoundationTestManagementSettingsService>();
        return this.m_testManagementSettingsService;
      }
    }
  }
}
