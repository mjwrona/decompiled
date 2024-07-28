// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmTestSettingsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Tcm;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testsettings", ResourceVersion = 1)]
  public class TcmTestSettingsController : TcmControllerBase
  {
    private TestSettingsHelper m_testSettingsHelper;

    [HttpGet]
    [ActionName("testsettings")]
    [ClientLocationId("930BAD47-F826-4099-9597-F44D0A9C735C")]
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
    [ClientLocationId("930BAD47-F826-4099-9597-F44D0A9C735C")]
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
    [ClientLocationId("930BAD47-F826-4099-9597-F44D0A9C735C")]
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
