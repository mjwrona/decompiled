// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExecutionControlOptionsController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "TestExecutionControlOptions")]
  public class TestExecutionControlOptionsController : TestExecutionApiController
  {
    private ITestExecutionControlOptionsService testExecutionControlOptionsService;

    [TraceFilter(6200375, 6200385)]
    [HttpGet]
    public TestExecutionControlOptions GetTestExecutionControlOptions(string envUrl)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200375, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestExecutionFlowController:GetTestExecutionControlOptions");
      try
      {
        return this.TFSTestExecutionControlOptionsService.GetTestExecutionControlOptions(this.TestExecutionRequestContext, envUrl);
      }
      catch (Exception ex)
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200376, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestExecutionFlowController:GetTestExecutionControlOptions: " + ex.Message);
        return new TestExecutionControlOptions()
        {
          UseTcmService = false
        };
      }
    }

    internal ITestExecutionControlOptionsService TFSTestExecutionControlOptionsService
    {
      get => this.testExecutionControlOptionsService ?? (this.testExecutionControlOptionsService = this.TestExecutionRequestContext.RequestContext.GetService<ITestExecutionControlOptionsService>());
      set => this.testExecutionControlOptionsService = value;
    }
  }
}
