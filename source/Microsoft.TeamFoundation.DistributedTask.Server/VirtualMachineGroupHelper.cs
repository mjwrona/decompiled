// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineGroupHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class VirtualMachineGroupHelper
  {
    public static IList<VirtualMachine> MapAgentsToMachines(
      IEnumerable<VirtualMachine> machines,
      IEnumerable<TaskAgent> agents,
      bool addMachinesForNewAgents = true,
      bool removeMachinesForDeletedAgents = true)
    {
      List<VirtualMachine> list = machines.ToList<VirtualMachine>();
      Dictionary<int, TaskAgent> dictionary = agents.ToDictionary<TaskAgent, int, TaskAgent>((Func<TaskAgent, int>) (x => x.Id), (Func<TaskAgent, TaskAgent>) (x => x));
      for (int index = list.Count - 1; index >= 0; --index)
      {
        int id = list[index].Agent.Id;
        TaskAgent taskAgent;
        if (dictionary.TryGetValue(id, out taskAgent))
        {
          list[index].Agent = taskAgent;
          dictionary.Remove(id);
        }
        else if (removeMachinesForDeletedAgents)
          list.RemoveAt(index);
      }
      if (addMachinesForNewAgents)
      {
        foreach (TaskAgent taskAgent in dictionary.Values)
          list.Add(new VirtualMachine()
          {
            Id = taskAgent.Id,
            Agent = taskAgent
          });
      }
      return (IList<VirtualMachine>) list;
    }
  }
}
