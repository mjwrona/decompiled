// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.Server.TcmTestExecutionConfigurationController
// Assembly: Microsoft.Azure.Pipelines.TestExecution.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A35ABAC4-7A2F-41DF-9E6F-54457266EDD3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestExecution.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestExecution.Server
{
  [VersionedApiControllerCustomName(Area = "TestExecution", ResourceName = "TestExecutionConfiguration")]
  public class TcmTestExecutionConfigurationController : TcmTestExecutionApiController
  {
    [TraceFilter(6200351, 6200361)]
    [HttpPost]
    [ActionName("TestExecutionConfiguration")]
    [ClientLocationId("A7BA9848-DC97-4049-949D-0C1D03B4B412")]
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
