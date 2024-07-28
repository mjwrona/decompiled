// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestConfigurationsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Configurations", ResourceVersion = 1)]
  public class TestConfigurationsController : TestManagementController
  {
    private TestConfigurationsHelper m_testConfigurationsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>), null, null)]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("GET__test_configurations.json", "Get a list of test configurations", null, null)]
    [ClientExample("GET__test_configurations__top-2.json", "A page at a time", null, null)]
    [ClientExample("GET__test_configurations__continuationToken.json", "With continuation token", null, null)]
    [ClientExample("GET__test_configurations__includeAllProperties.json", "Include all properties of the test configurations", null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public virtual HttpResponseMessage GetTestConfigurations(
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      string continuationToken = null,
      bool includeAllProperties = false)
    {
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>) this.TestConfigurationsHelper.GetTestConfigurations(this.ProjectInfo.Name, skip, top, includeAllProperties));
    }

    [HttpGet]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("GET__test_configurations__configurationId_.json", "Get a test configuration", null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration GetTestConfigurationById(
      int testConfigurationId)
    {
      return this.TestConfigurationsHelper.GetTestConfigurationById(this.ProjectInfo.Name, testConfigurationId);
    }

    [HttpPost]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("POST__test_configurations.json", "Create a test configuration", null, null)]
    [ClientExample("POST__test_configurations2.json", "With Area", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration CreateTestConfiguration(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration testConfiguration)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(testConfiguration, "TestConfiguration", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testConfiguration.Name, "name", this.TfsRequestContext.ServiceName);
      return this.TestConfigurationsHelper.CreateTestConfiguration(this.ProjectInfo.Name, testConfiguration);
    }

    [HttpPatch]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("PATCH__test_configurations__configurationId_.json", "Update a test configuration", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration UpdateTestConfiguration(
      int testConfigurationId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration testConfiguration)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(testConfiguration, "TestConfiguration", this.TfsRequestContext.ServiceName);
      return this.TestConfigurationsHelper.UpdateTestConfiguration(this.ProjectInfo.Name, testConfigurationId, testConfiguration);
    }

    [HttpDelete]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("DELETE__test_configurations__configurationId_.json", "Delete a test configuration", null, null)]
    public void DeleteTestConfiguration(int testConfigurationId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.TestConfigurationsHelper.DeleteTestConfiguration(this.ProjectInfo.Name, testConfigurationId);
    }

    internal TestConfigurationsHelper TestConfigurationsHelper
    {
      get
      {
        if (this.m_testConfigurationsHelper == null)
          this.m_testConfigurationsHelper = new TestConfigurationsHelper(this.TestManagementRequestContext);
        return this.m_testConfigurationsHelper;
      }
      set => this.m_testConfigurationsHelper = value;
    }
  }
}
