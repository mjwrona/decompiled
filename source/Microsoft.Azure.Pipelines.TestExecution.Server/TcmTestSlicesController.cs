// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmTestSlicesController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "Slices")]
  public class TcmTestSlicesController : TcmTestExecutionApiController
  {
    [TraceFilter(6200401, 6200420)]
    [HttpGet]
    [ActionName("Slices")]
    [ClientLocationId("487BE43D-93C4-4B24-BA9A-D78B67370334")]
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
    [ActionName("Slices")]
    [ClientLocationId("487BE43D-93C4-4B24-BA9A-D78B67370334")]
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
