// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmDistributedTestRunController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "DistributedTestRuns")]
  public class TcmDistributedTestRunController : TcmTestExecutionApiController
  {
    [TraceFilter(6200321, 6200220)]
    [HttpPatch]
    [ActionName("DistributedTestRuns")]
    [ClientLocationId("01627D68-DE6C-4E6F-B6B5-AFEBDCB81F19")]
    public DistributedTestRun UpdateTestRun(DistributedTestRun distributedTestRun)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200321, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "DistributedTestRunController:CreateTestRun");
      ArgumentUtility.CheckForNull<DistributedTestRun>(distributedTestRun, "Distributed Test Run");
      ArgumentUtility.CheckForNull<DistributedTestRunCreateModel>(distributedTestRun.DistributedTestRunCreateModel, "Run Create Model");
      ArgumentUtility.CheckForNull<string>(distributedTestRun.DistributedTestRunCreateModel.EnvironmentUrl, "Distributed Test Run Environment");
      try
      {
        IDistributedTestRunService service = this.TestExecutionRequestContext.RequestContext.GetService<IDistributedTestRunService>();
        this.TestExecutionRequestContext.RequestContext.GetService<IProjectService>();
        DistributedTestRun testRun = service.GetTestRun(this.TestExecutionRequestContext, this.ProjectId, distributedTestRun.DistributedTestRunCreateModel.EnvironmentUrl);
        return testRun.TestRunId > 0 ? testRun : service.CreateTestRun(this.TestExecutionRequestContext, this.ProjectId, distributedTestRun);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200330, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "DistributedTestRunController:CreateTestRun");
      }
    }
  }
}
