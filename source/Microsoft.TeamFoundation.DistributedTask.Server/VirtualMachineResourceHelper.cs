// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineResourceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class VirtualMachineResourceHelper
  {
    public static IList<VirtualMachineResource> MapAgentsToVirtualMachines(
      IEnumerable<VirtualMachineResource> machines,
      IEnumerable<TaskAgent> agents)
    {
      List<VirtualMachineResource> list = machines.ToList<VirtualMachineResource>();
      Dictionary<int, TaskAgent> dictionary = agents.ToDictionary<TaskAgent, int, TaskAgent>((Func<TaskAgent, int>) (x => x.Id), (Func<TaskAgent, TaskAgent>) (x => x));
      foreach (VirtualMachineResource virtualMachineResource in list)
      {
        int id = virtualMachineResource.Agent.Id;
        TaskAgent taskAgent;
        if (dictionary.TryGetValue(id, out taskAgent))
          virtualMachineResource.Agent = taskAgent;
      }
      return (IList<VirtualMachineResource>) list;
    }
  }
}
