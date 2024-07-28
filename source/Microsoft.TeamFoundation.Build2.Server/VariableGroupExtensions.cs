// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.VariableGroupExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class VariableGroupExtensions
  {
    public static VariableGroup ToBuildDefinitionVariableGroup(this Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup)
    {
      VariableGroup definitionVariableGroup = new VariableGroup()
      {
        Description = variableGroup.Description,
        Id = variableGroup.Id,
        Name = variableGroup.Name,
        Type = variableGroup.Type
      };
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variableGroup.Variables)
      {
        BuildDefinitionVariable definitionVariable = new BuildDefinitionVariable()
        {
          AllowOverride = false,
          Value = variable.Value.Value,
          IsSecret = variable.Value.IsSecret
        };
        definitionVariableGroup.Variables.Add(variable.Key, definitionVariable);
      }
      return definitionVariableGroup;
    }

    public static bool IsKeyVaultType(this Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup) => string.Equals(variableGroup.Type, "AzureKeyVault", StringComparison.OrdinalIgnoreCase);
  }
}
