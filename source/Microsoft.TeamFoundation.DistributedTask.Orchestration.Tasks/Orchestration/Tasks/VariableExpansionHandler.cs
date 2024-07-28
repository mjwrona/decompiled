// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.VariableExpansionHandler
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public static class VariableExpansionHandler
  {
    public static void ExpandVariableInDemand(
      Demand demand,
      IDictionary<string, string> jobVariables,
      IDictionary<string, string> planVariables)
    {
      string str1 = demand.Value;
      if (string.IsNullOrEmpty(str1))
        return;
      string str2 = VariableExpansionHandler.ExpandVariables(str1, jobVariables, planVariables);
      demand.Update(str2);
    }

    public static void ExpandVariablesInDemands(
      IList<Demand> demands,
      IDictionary<string, string> jobVariables,
      IDictionary<string, string> planVariables)
    {
      foreach (Demand demand in (IEnumerable<Demand>) demands)
        VariableExpansionHandler.ExpandVariableInDemand(demand, jobVariables, planVariables);
    }

    public static string ExpandVariables(
      string str,
      IDictionary<string, string> jobVariable,
      IDictionary<string, string> planVariable)
    {
      str = VariableUtility.ExpandVariables(str, jobVariable);
      str = VariableUtility.ExpandVariables(str, planVariable);
      return str;
    }
  }
}
