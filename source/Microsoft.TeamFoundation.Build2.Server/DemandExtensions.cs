// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DemandExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class DemandExtensions
  {
    public static IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> ToDistributedTaskDemands(
      this IEnumerable<Demand> demands)
    {
      if (demands != null)
      {
        foreach (Demand demand in demands)
        {
          DemandEquals demandEquals = demand as DemandEquals;
          DemandExists demandExists = demand as DemandExists;
          if (demandEquals != null)
            yield return (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) new Microsoft.TeamFoundation.DistributedTask.WebApi.DemandEquals(demandEquals.Name, demandEquals.Value);
          else if (demandExists != null)
            yield return (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) new Microsoft.TeamFoundation.DistributedTask.WebApi.DemandExists(demandExists.Name);
        }
      }
    }

    public static IEnumerable<Demand> ToBuildTaskDemands(this IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demands)
    {
      if (demands != null)
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand in demands)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.DemandEquals demandEquals = demand as Microsoft.TeamFoundation.DistributedTask.WebApi.DemandEquals;
          Microsoft.TeamFoundation.DistributedTask.WebApi.DemandExists demandExists = demand as Microsoft.TeamFoundation.DistributedTask.WebApi.DemandExists;
          if (demandEquals != null)
            yield return (Demand) new DemandEquals(demandEquals.Name, demandEquals.Value);
          else if (demandExists != null)
            yield return (Demand) new DemandExists(demandExists.Name);
        }
      }
    }

    public static bool IsAgentMinimumVersionDemand(this Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand) => demand.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase);
  }
}
