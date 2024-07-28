// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentGroupHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DeploymentGroupHelper
  {
    public static IList<DeploymentMachine> MapAgentsToMachines(
      IEnumerable<DeploymentMachine> machines,
      IEnumerable<TaskAgent> agents,
      bool addMachinesForNewAgents = true,
      bool removeMachinesForDeletedAgents = true)
    {
      List<DeploymentMachine> list = machines.ToList<DeploymentMachine>();
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
          list.Add(new DeploymentMachine()
          {
            Id = taskAgent.Id,
            Agent = taskAgent
          });
      }
      return (IList<DeploymentMachine>) list;
    }

    public static IList<DeploymentMachineChangedData> MapAgentsToMachinesChangedData(
      IEnumerable<DeploymentMachineChangedData> machines,
      IEnumerable<TaskAgent> agents,
      bool addMachinesForNewAgents = true,
      bool removeMachinesForDeletedAgents = true)
    {
      List<DeploymentMachineChangedData> list = machines.ToList<DeploymentMachineChangedData>();
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
        {
          List<DeploymentMachineChangedData> machineChangedDataList = list;
          DeploymentMachineChangedData machineChangedData = new DeploymentMachineChangedData();
          machineChangedData.Id = taskAgent.Id;
          machineChangedData.Agent = taskAgent;
          machineChangedDataList.Add(machineChangedData);
        }
      }
      return (IList<DeploymentMachineChangedData>) list;
    }

    public static IList<DeploymentMachine> convertToDeploymentMachines(
      IList<DeploymentMachineChangedData> deploymentMachinesChangedDataList)
    {
      List<DeploymentMachine> deploymentMachines = new List<DeploymentMachine>();
      foreach (DeploymentMachineChangedData machinesChangedData in (IEnumerable<DeploymentMachineChangedData>) deploymentMachinesChangedDataList)
        deploymentMachines.Add((DeploymentMachine) machinesChangedData);
      return (IList<DeploymentMachine>) deploymentMachines;
    }

    public static IList<DeploymentMachineChangedData> convertToDeploymentMachinesChangedDataList(
      IList<DeploymentMachine> deploymentMachines)
    {
      List<DeploymentMachineChangedData> machinesChangedDataList = new List<DeploymentMachineChangedData>();
      foreach (DeploymentMachine deploymentMachine in (IEnumerable<DeploymentMachine>) deploymentMachines)
        machinesChangedDataList.Add(new DeploymentMachineChangedData(deploymentMachine));
      return (IList<DeploymentMachineChangedData>) machinesChangedDataList;
    }
  }
}
