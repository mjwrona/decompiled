// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSettingsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "TestSettings", ResourceVersion = 1)]
  public class TestSettingsController : TestManagementController
  {
    private TestSettingsHelper m_testSettingsHelper;

    [HttpGet]
    [ActionName("TestSettings")]
    [ClientLocationId("8133CE14-962F-42AF-A5F9-6AA9DEFCB9C8")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
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
    [ActionName("TestSettings")]
    [ClientLocationId("8133CE14-962F-42AF-A5F9-6AA9DEFCB9C8")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
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
    [ActionName("TestSettings")]
    [ClientLocationId("8133CE14-962F-42AF-A5F9-6AA9DEFCB9C8")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
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
