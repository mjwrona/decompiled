// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCommandsController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Commands")]
  public class TestCommandsController : TestExecutionApiController
  {
    private ITestCommandCatalogService _testCommandCatalogService;

    [TraceFilter(6200101, 6200120)]
    [HttpGet]
    public async Task<TestExecutionServiceCommand> GetCommandAsync(int testAgentId = -2147483648, long? commandId = null)
    {
      TestCommandsController commandsController = this;
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
