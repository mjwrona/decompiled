// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSettingsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestSettingsHelper : RestApiHelper
  {
    internal TestSettingsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSetting(
      string projectId,
      int settingsId)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestSettingsHelper.GetTestSetting");
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings>("TestSettingsHelper.GetTestSetting", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        try
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestSettingById(this.RequestContext, projectReference.Id, settingsId, out testSettings))
            return testSettings;
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          return this.GetTestSettingById(settingsId, projectReference);
        }
        return this.GetTestSettingById(settingsId, projectReference);
      }), 1015057, "TestManagement");
    }

    public int CreateTestSetting(string projectId, Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettingsDetails)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestSettingsHelper.CreateTestSetting");
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings>(testSettingsDetails, "testSettings", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testSettingsDetails.TestSettingsContent, "TestSettingsContent", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testSettingsDetails.AreaPath, "areaPath", this.RequestContext.ServiceName);
      return this.ExecuteAction<int>("TestSettingsHelper.CreateTestSetting", (Func<int>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        int? testSettingsId;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestSettings(this.RequestContext, projectReference.Id, testSettingsDetails, out testSettingsId))
          return testSettingsId.GetValueOrDefault();
        ITeamFoundationTestManagementSettingsService service = this.RequestContext.GetService<ITeamFoundationTestManagementSettingsService>();
        TestSettings testSettings = this.GetTestSettings(testSettingsDetails);
        this.RequestContext.TraceInfo("RestLayer", "TestSettingsHelper.CreateTestSetting:: Creating test settings. projectName = {0}, testSettings name = {1}", (object) projectReference.Name, (object) testSettingsDetails.TestSettingsName);
        return service.CreateTestSettings(this.TestManagementRequestContext, testSettings, projectReference);
      }), 1015057, "TestManagement");
    }

    public bool DeleteTestSetting(string projectId, int settingsId)
    {
      this.RequestContext.TraceEnter("RestLayer", "TestSettingsHelper.DeleteTestSetting");
      if (settingsId <= 0)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "TestSettingsId"));
      this.ExecuteAction<bool>("TestSettingsHelper.DeleteTestSetting", (Func<bool>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        try
        {
          if (this.TestManagementRequestContext.TcmServiceHelper.TryDeleteTestSettings(this.RequestContext, projectReference.Id, settingsId))
            return true;
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          return this.DeleteTestSettingsById(settingsId, projectReference);
        }
        return this.DeleteTestSettingsById(settingsId, projectReference);
      }), 1015057, "TestManagement");
      return false;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings ConvertTestSettingToDataContract(
      TestSettings testSettings)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings()
      {
        TestSettingsId = testSettings.Id,
        TestSettingsName = testSettings.Name,
        TestSettingsContent = testSettings.Settings,
        AreaPath = testSettings.AreaPath,
        Description = testSettings.Description,
        IsPublic = testSettings.IsPublic,
        MachineRoles = TestSettingsMachineRole.ToXml(testSettings.MachineRoles)
      };
    }

    private TestSettings GetTestSettings(Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettingsDetails) => new TestSettings()
    {
      Name = testSettingsDetails.TestSettingsName,
      Description = testSettingsDetails.Description,
      AreaPath = testSettingsDetails.AreaPath,
      Settings = testSettingsDetails.TestSettingsContent,
      MachineRoles = TestSettingsMachineRole.FromXml(this.RequestContext, testSettingsDetails.MachineRoles),
      IsPublic = testSettingsDetails.IsPublic
    };

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettingById(
      int settingsId,
      TeamProjectReference projectReference)
    {
      return this.ConvertTestSettingToDataContract(this.RequestContext.GetService<ITeamFoundationTestManagementSettingsService>().GetTestSettingsById(this.TestManagementRequestContext, settingsId, projectReference));
    }

    private bool DeleteTestSettingsById(int settingsId, TeamProjectReference projectReference)
    {
      ITeamFoundationTestManagementSettingsService service = this.RequestContext.GetService<ITeamFoundationTestManagementSettingsService>();
      this.RequestContext.TraceInfo("RestLayer", "TestSettingsHelper.DeleteTestSetting:: Deleting TestSettings. projectName = {0}, settingsId = {1}", (object) projectReference.Name, (object) settingsId);
      service.DeleteTestSettings(this.TestManagementRequestContext, settingsId, projectReference);
      return true;
    }
  }
}
