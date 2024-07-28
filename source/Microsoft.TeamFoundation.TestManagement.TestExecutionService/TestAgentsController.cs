// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestAgentsController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Agents")]
  public class TestAgentsController : TestExecutionApiController
  {
    private ITfsTestAgentCatalogService _tfsTestAgentCatalogService;

    [TraceFilter(6200001, 6200040)]
    [HttpPost]
    public TestAgent CreateAgent(TestAgent testAgent)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200019, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:CreateAgent");
      try
      {
        return this.TfsTestAgentCatalogService.CreateAgent(this.TestExecutionRequestContext, testAgent);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200020, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:CreateAgent");
      }
    }

    [TraceFilter(6200041, 6200060)]
    [HttpGet]
    public TestAgent GetAgent(int id)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200041, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:GetAgent");
      try
      {
        return this.TfsTestAgentCatalogService.GetAgent(this.TestExecutionRequestContext, id);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200060, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:GetAgent");
      }
    }

    [TraceFilter(6200001, 6200040)]
    [HttpDelete]
    public void DeleteAgent(int id)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200022, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:DeleteAgent");
      try
      {
        this.TfsTestAgentCatalogService.UnRegisterAgent(this.TestExecutionRequestContext, id);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200040, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestAgentsController:DeleteAgent");
      }
    }

    internal ITfsTestAgentCatalogService TfsTestAgentCatalogService
    {
      get => this._tfsTestAgentCatalogService ?? (this._tfsTestAgentCatalogService = this.TfsRequestContext.GetService<ITfsTestAgentCatalogService>());
      set => this._tfsTestAgentCatalogService = value;
    }
  }
}
