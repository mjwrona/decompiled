// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestVariables3Controller
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
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "Variables", ResourceVersion = 1)]
  public class TestVariables3Controller : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestVariableById.json", "Get a test variable by Id.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable GetTestVariableById(
      int testVariableId)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable) new TestVariableAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementVariableService>().GetTestVariableById(this.TestManagementRequestContext, testVariableId, this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpGet]
    [ClientLocationId("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C")]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GetTestVariables.json", "Get a list of test variables.", null, null)]
    public HttpResponseMessage GetTestVariables(string continuationToken = null)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestVariable> testVariables1 = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementVariableService>().GetTestVariables(this.TestManagementRequestContext, this.ProjectInfo.Name, skipRows, topToFetch, watermark);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestVariable> list = testVariables1 != null ? testVariables1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestVariable>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestVariable>) null;
      continuationToken = (string) null;
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable> testVariables = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable>();
      if (!list.IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestVariable>())
      {
        list.ForEach((Action<Microsoft.TeamFoundation.TestManagement.Server.TestVariable>) (variable => testVariables.Add((Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable) new TestVariableAdapter(this.TestManagementRequestContext, variable, (ProjectInfo) null))));
        if (testVariables != null && testVariables.Count >= topToFetch && testVariables[topToFetch - 1] != null)
        {
          continuationToken = Utils.GenerateContinuationToken(testVariables[topToFetch - 1].Id, topRemaining);
          testVariables.RemoveAt(topToFetch - 1);
        }
      }
      HttpResponseMessage response = this.GenerateResponse<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable>((IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable>) testVariables);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpPost]
    [ClientLocationId("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C")]
    [ClientExample("CreateTestVariable.json", "Create a new test variable.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable CreateTestVariable(
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable) new TestVariableAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementVariableService>().CreateTestVariable(this.TestManagementRequestContext, new TestVariableAdapter(testVariableCreateUpdateParameters).ToServerTestVariable(), this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpPatch]
    [ClientLocationId("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C")]
    [ClientExample("UpdateTestVariable.json", "Update a test variable.", null, null)]
    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable UpdateTestVariable(
      int testVariableId,
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters)
    {
      return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable) new TestVariableAdapter(this.TestManagementRequestContext, this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementVariableService>().UpdateTestVariable(this.TestManagementRequestContext, testVariableId, new TestVariableAdapter(testVariableCreateUpdateParameters).ToServerTestVariable(), this.ProjectInfo.Name), this.ProjectInfo);
    }

    [HttpDelete]
    [ClientLocationId("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C")]
    [ClientExample("DeleteTestVariable.json", "Delete a test variable.", null, null)]
    public void DeleteTestVariable(int testVariableId) => this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementVariableService>().DeleteTestVariable(this.TestManagementRequestContext, testVariableId, this.ProjectInfo.Name);
  }
}
