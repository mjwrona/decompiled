// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSlicesController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Slices")]
  public class TestSlicesController : TestExecutionApiController
  {
    [TraceFilter(6200401, 6200420)]
    [HttpGet]
    public TestAutomationRunSlice GetSlice(int testAgentId)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200401, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestSlicesController:GetSlice");
      try
      {
        return this.TestExecutionService.GetSlice(this.TestExecutionRequestContext, testAgentId);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200420, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestSlicesController:GetSlice");
      }
    }

    [TraceFilter(6200421, 6200430)]
    [HttpPatch]
    public void UpdateSlice(TestAutomationRunSlice sliceDetails)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200421, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestSlicesController:UpdateSlice");
      try
      {
        this.TestExecutionService.UpdateSlice(this.TestExecutionRequestContext, sliceDetails);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200430, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestSlicesController:UpdateSlice");
      }
    }
  }
}
