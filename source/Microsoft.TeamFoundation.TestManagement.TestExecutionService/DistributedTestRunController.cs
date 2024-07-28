// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DistributedTestRunController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "DistributedTestRuns")]
  public class DistributedTestRunController : TestExecutionApiController
  {
    [TraceFilter(6200321, 6200220)]
    [HttpPatch]
    public DistributedTestRun UpdateTestRun(string project, DistributedTestRun distributedTestRun)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200321, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "DistributedTestRunController:CreateTestRun");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(project, "Project Name");
      ArgumentUtility.CheckForNull<DistributedTestRun>(distributedTestRun, "Distributed Test Run");
      ArgumentUtility.CheckForNull<DistributedTestRunCreateModel>(distributedTestRun.DistributedTestRunCreateModel, "Run Create Model");
      ArgumentUtility.CheckForNull<string>(distributedTestRun.DistributedTestRunCreateModel.EnvironmentUrl, "Distributed Test Run Environment");
      try
      {
        IDistributedTestRunService service = this.TestExecutionRequestContext.RequestContext.GetService<IDistributedTestRunService>();
        Guid projectId = this.TestExecutionRequestContext.RequestContext.GetService<IProjectService>().GetProjectId(this.TestExecutionRequestContext.RequestContext, project);
        DistributedTestRun testRun = service.GetTestRun(this.TestExecutionRequestContext, projectId, distributedTestRun.DistributedTestRunCreateModel.EnvironmentUrl);
        return testRun.TestRunId > 0 ? testRun : service.CreateTestRun(this.TestExecutionRequestContext, projectId, distributedTestRun);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200330, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "DistributedTestRunController:CreateTestRun");
      }
    }
  }
}
