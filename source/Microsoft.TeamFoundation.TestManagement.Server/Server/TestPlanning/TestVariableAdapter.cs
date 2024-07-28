// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestVariableAdapter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  public class TestVariableAdapter : Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestVariable
  {
    public TestVariableAdapter(
      TestManagementRequestContext testManagementRequestContext,
      Microsoft.TeamFoundation.TestManagement.Server.TestVariable testVariable,
      ProjectInfo projectInfo)
    {
      if (testVariable == null)
        return;
      this.Description = testVariable.Description;
      this.Id = testVariable.Id;
      this.Name = testVariable.Name;
      this.Values = testVariable.Values;
      if (testManagementRequestContext == null || projectInfo == null)
        return;
      this.Project = new TeamProjectReference()
      {
        Name = projectInfo.Name,
        Id = projectInfo.Id,
        Url = testManagementRequestContext.ProjectServiceHelper?.GetProjectRepresentation(projectInfo.Name)?.Url
      };
    }

    public TestVariableAdapter(
      TestVariableCreateUpdateParameters testConfigurationVariableCreateUpdateParameters)
    {
      ArgumentUtility.CheckForNull<TestVariableCreateUpdateParameters>(testConfigurationVariableCreateUpdateParameters, "TestConfigurationVariableCreateUpdateParameters");
      this.Description = testConfigurationVariableCreateUpdateParameters.Description;
      this.Name = testConfigurationVariableCreateUpdateParameters.Name;
      this.Values = testConfigurationVariableCreateUpdateParameters.Values;
    }

    public Microsoft.TeamFoundation.TestManagement.Server.TestVariable ToServerTestVariable()
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestVariable serverTestVariable = new Microsoft.TeamFoundation.TestManagement.Server.TestVariable()
      {
        Description = this.Description,
        Id = this.Id,
        Name = this.Name
      };
      if (!this.Values.IsNullOrEmpty<string>())
        serverTestVariable.Values.AddRange((IEnumerable<string>) this.Values);
      return serverTestVariable;
    }
  }
}
