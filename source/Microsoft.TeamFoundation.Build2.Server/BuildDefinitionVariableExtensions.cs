// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariableExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildDefinitionVariableExtensions
  {
    public static VariableValue ToVariableValue(
      this BuildDefinitionVariable variable,
      string name,
      IDictionary<string, string> secretVariables = null)
    {
      VariableValue variableValue = new VariableValue()
      {
        IsSecret = variable.IsSecret,
        Value = variable.Value
      };
      string str;
      if (variableValue.IsSecret && secretVariables != null && secretVariables.TryGetValue(name, out str))
        variableValue.Value = str;
      return variableValue;
    }
  }
}
