// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProcessParametersExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ProcessParametersExtensions
  {
    private const string c_ProcessParameterPrefix = "Parameters.";

    public static IDictionary<string, string> GetProcessParametersInputs(
      this ProcessParameters processParameters)
    {
      return processParameters == null ? (IDictionary<string, string>) new Dictionary<string, string>() : (IDictionary<string, string>) processParameters.Inputs.ToDictionary<TaskInputDefinitionBase, string, string>((Func<TaskInputDefinitionBase, string>) (v => "Parameters." + v.Name), (Func<TaskInputDefinitionBase, string>) (v => v.DefaultValue), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
