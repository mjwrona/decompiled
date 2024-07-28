// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmTestAgentsController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "Agents")]
  public class TcmTestAgentsController : TcmTestExecutionApiController
  {
    private ITfsTestAgentCatalogService _tfsTestAgentCatalogService;

    [TraceFilter(6200001, 6200040)]
    [HttpPost]
    [ActionName("Agents")]
    [ClientLocationId("9782642A-22B0-42BC-8EC1-1F4725AA20C2")]
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
    [ActionName("Agents")]
    [ClientLocationId("9782642A-22B0-42BC-8EC1-1F4725AA20C2")]
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
    [ActionName("Agents")]
    [ClientLocationId("9782642A-22B0-42BC-8EC1-1F4725AA20C2")]
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
