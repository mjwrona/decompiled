// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestManagementConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestManagementConfigurationHelper : ITestManagementConfigurationHelper
  {
    public TestConfiguration GetTestConfigurationById(
      TestExecutionRequestContext requestContext,
      TeamProjectReference projectReference,
      int configId)
    {
      return !string.IsNullOrWhiteSpace(projectReference.Name) ? requestContext.TestPlanningHttpClient.GetTestConfigurationByIdAsync(projectReference.Name, configId).Result : requestContext.TestPlanningHttpClient.GetTestConfigurationByIdAsync(projectReference.Id, configId).Result;
    }
  }
}
