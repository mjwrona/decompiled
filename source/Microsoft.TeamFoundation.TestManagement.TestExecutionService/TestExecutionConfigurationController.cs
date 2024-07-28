// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExecutionConfigurationController
// Assembly: Microsoft.TeamFoundation.TestManagement.TestExecutionService, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F1A78E22-A12E-4CA4-8586-1FF5AE0B1013
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.TestExecutionService.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "TestExecutionConfiguration")]
  public class TestExecutionConfigurationController : TestExecutionApiController
  {
    [TraceFilter(6200351, 6200361)]
    [HttpPost]
    public TestRunExecutionConfiguration GetRerunConfiguration(
      TestRunExecutionConfiguration testRunExecutionConfiguration)
    {
      this.TestExecutionRequestContext.RequestContext.TraceEnter(6200351, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestExecutionConfigurationController:GetRerunConfiguration");
      ArgumentUtility.CheckForNull<TestRunExecutionConfiguration>(testRunExecutionConfiguration, "Test Run Execution Configuration Model");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testRunExecutionConfiguration.ProjectName, "Project Name");
      ArgumentUtility.CheckGreaterThanZero((float) testRunExecutionConfiguration.TestRunId, "Test Run Id");
      try
      {
        return this.TestExecutionRequestContext.RequestContext.GetService<ITestRunExecutionConfigurationService>().GetRerunConfiguration(this.TestExecutionRequestContext, testRunExecutionConfiguration);
      }
      finally
      {
        this.TestExecutionRequestContext.RequestContext.TraceLeave(6200361, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "TestExecutionConfigurationController:GetRerunConfiguration");
      }
    }
  }
}
