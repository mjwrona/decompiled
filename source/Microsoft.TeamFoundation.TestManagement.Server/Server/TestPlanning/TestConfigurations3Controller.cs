// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestConfigurations3Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "Configurations", ResourceVersion = 1)]
  public class TestConfigurations3Controller : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("8369318E-38FA-4E84-9043-4B2A75D2C256")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestConfigurationById.json", "Get a test configuration by Id.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration GetTestConfigurationById(
      int testConfigurationId)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration) new TestConfigurationAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>().GetTestConfigurationById(this.TestManagementRequestContext, testConfigurationId, this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpGet]
    [ClientLocationId("8369318E-38FA-4E84-9043-4B2A75D2C256")]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestConfigurations.json", "Get a list of test configurations.", null, null)]
    public HttpResponseMessage GetTestConfigurations(string continuationToken = null)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration> configurationsWithPaging = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>().GetTestConfigurationsWithPaging(this.TestManagementRequestContext, this.ProjectInfo.Name, skipRows, topToFetch, watermark);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration> list = configurationsWithPaging != null ? configurationsWithPaging.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration>) null;
      continuationToken = (string) null;
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration> webApiTestConfigurations = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration>();
      if (!list.IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration>())
      {
        list.ForEach((Action<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration>) (configuration => webApiTestConfigurations.Add((Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration) new TestConfigurationAdapter(this.TestManagementRequestContext, configuration, (ProjectInfo) null))));
        if (webApiTestConfigurations != null && webApiTestConfigurations.Count >= topToFetch && webApiTestConfigurations[topToFetch - 1] != null)
        {
          continuationToken = Utils.GenerateContinuationToken(webApiTestConfigurations[topToFetch - 1].Id, topRemaining);
          webApiTestConfigurations.RemoveAt(topToFetch - 1);
        }
      }
      HttpResponseMessage response = this.GenerateResponse<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration>((IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration>) webApiTestConfigurations);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpPost]
    [ClientLocationId("8369318E-38FA-4E84-9043-4B2A75D2C256")]
    [ClientExample("CreateTestConfiguration.json", "Create a new test configuration.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration CreateTestConfiguration(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration) new TestConfigurationAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>().CreateTestConfiguration(this.TestManagementRequestContext, new TestConfigurationAdapter(testConfigurationCreateUpdateParameters).ToServerTestConfiguration(this.TestManagementRequestContext, this.ProjectInfo), this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpPatch]
    [ClientLocationId("8369318E-38FA-4E84-9043-4B2A75D2C256")]
    [ClientExample("UpdateTestConfiguration.json", "Update a test configuration.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration UpdateTestConfiguration(
      int testConfiguartionId,
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration) new TestConfigurationAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>().UpdateTestConfiguration(this.TestManagementRequestContext, testConfiguartionId, new TestConfigurationAdapter(testConfigurationCreateUpdateParameters).ToServerTestConfiguration(this.TestManagementRequestContext, this.ProjectInfo), this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpDelete]
    [ClientLocationId("8369318E-38FA-4E84-9043-4B2A75D2C256")]
    [ClientExample("DeleteTestConfiguration.json", "Delete a test configuration.", null, null)]
    public void DeleteTestConfguration(int testConfiguartionId) => this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementConfigurationService>().DeleteTestConfiguration(this.TestManagementRequestContext, testConfiguartionId, this.ProjectInfo.Name);
  }
}
