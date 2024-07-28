// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmTestCommandsController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "Commands")]
  public class TcmTestCommandsController : TcmTestExecutionApiController
  {
    private ITestCommandCatalogService _testCommandCatalogService;

    [TraceFilter(6200101, 6200120)]
    [HttpGet]
    [ActionName("Commands")]
    [ClientLocationId("DAA073EA-7F66-4025-949A-9A052695AB17")]
    public async Task<TestExecutionServiceCommand> GetCommandAsync(int testAgentId = -2147483648, long? commandId = null)
    {
      TcmTestCommandsController commandsController = this;
      commandsController.TestExecutionRequestContext.RequestContext.TraceEnter(6200101, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestMessagesController:GetTestExecutionMessage");
      TestExecutionServiceCommand commandAsync;
      try
      {
        commandAsync = await commandsController.TestCommandCatalogService.GetCommandAsync(commandsController.TestExecutionRequestContext, testAgentId, commandId);
      }
      finally
      {
        commandsController.TestExecutionRequestContext.RequestContext.TraceLeave(6200120, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestMessagesController:GetTestExecutionMessage");
      }
      return commandAsync;
    }

    internal ITestCommandCatalogService TestCommandCatalogService
    {
      get => this._testCommandCatalogService ?? (this._testCommandCatalogService = this.TfsRequestContext.GetService<ITestCommandCatalogService>());
      set => this._testCommandCatalogService = value;
    }
  }
}
