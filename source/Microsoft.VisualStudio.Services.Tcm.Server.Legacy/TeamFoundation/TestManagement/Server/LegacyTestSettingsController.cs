// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyTestSettingsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Tcm;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "testsettings", ResourceVersion = 1)]
  public class LegacyTestSettingsController : TcmControllerBase
  {
    private TestSettingsHelper m_testSettingsHelper;

    [HttpGet]
    [ActionName("testsettings")]
    [ClientLocationId("5F0315CC-4D71-4ED8-B445-F7EED13A8399")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettingsById(
      int testSettingsId)
    {
      try
      {
        return this.TestSettingsHelper.GetTestSetting(this.ProjectId.ToString(), testSettingsId);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in TestSettingsController::GetTestSettingsById. Error = {0}, testSetingsId = {1} .", (object) ex.ToString(), (object) testSettingsId);
        throw;
      }
    }

    [HttpPost]
    [ActionName("testsettings")]
    [ClientLocationId("5F0315CC-4D71-4ED8-B445-F7EED13A8399")]
    public int CreateTestSettings(Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings)
    {
      try
      {
        return this.TestSettingsHelper.CreateTestSetting(this.ProjectId.ToString(), testSettings);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in TestSettingsController::CreateTestSettings. Error = {0}.", (object) ex.ToString());
        throw;
      }
    }

    [HttpDelete]
    [ActionName("testsettings")]
    [ClientLocationId("5F0315CC-4D71-4ED8-B445-F7EED13A8399")]
    public void DeleteTestSettings(int testSettingsId)
    {
      try
      {
        this.TestSettingsHelper.DeleteTestSetting(this.ProjectId.ToString(), testSettingsId);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in TestSettingsController::DeleteTestSettings. Error = {0}, testSetingsId = {1} .", (object) ex.ToString(), (object) testSettingsId);
        throw;
      }
    }

    internal TestSettingsHelper TestSettingsHelper
    {
      get
      {
        if (this.m_testSettingsHelper == null)
          this.m_testSettingsHelper = new TestSettingsHelper(this.TestManagementRequestContext);
        return this.m_testSettingsHelper;
      }
    }
  }
}
