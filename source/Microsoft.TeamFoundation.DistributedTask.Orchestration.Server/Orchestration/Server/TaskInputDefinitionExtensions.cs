// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskInputDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskInputDefinitionExtensions
  {
    private const string c_taskInputTypePrefix = "connectedService:";

    public static bool IsEndpointInputVisible(
      this TaskInputDefinition serviceEndpointTypeInput,
      IDictionary<string, string> taskInputs)
    {
      bool flag = true;
      if (serviceEndpointTypeInput.IsEndpointTypeInput() && !string.IsNullOrEmpty(serviceEndpointTypeInput.VisibleRule))
      {
        VisibilityRule visibilityRule = TaskInputVisibilityRule.GetVisibilityRule(serviceEndpointTypeInput.VisibleRule);
        if (visibilityRule != null)
          flag = TaskInputVisibilityRule.GetVisibility(visibilityRule, taskInputs);
      }
      return flag;
    }

    public static bool IsEndpointTypeInput(this TaskInputDefinition input) => input.InputType != null && input.InputType.Contains("connectedService:");
  }
}
