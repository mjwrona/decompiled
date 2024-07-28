// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskResourceServiceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class DistributedTaskResourceServiceHelper
  {
    public static string GetAgentOSDescription(string userAgent)
    {
      if (!string.IsNullOrEmpty(userAgent))
      {
        int startIndex = userAgent.IndexOf("vstsagentcore-", StringComparison.OrdinalIgnoreCase);
        if (startIndex >= 0)
        {
          string str1 = userAgent.Substring(startIndex);
          if (str1.IndexOf("(") > 0)
          {
            string str2 = str1.Substring(str1.IndexOf("("));
            return str2.Remove(str2.Length - 1).Substring(1);
          }
        }
      }
      return (string) null;
    }

    public static string ConstructUnmatchedDemandsError(
      IList<Demand> demands,
      IList<Tuple<TaskAgent, IList<Demand>>> unmatchedAgents,
      string poolName)
    {
      string str = string.Join(", ", demands.Select<Demand, string>((Func<Demand, string>) (d => string.Format("{0}", (object) d))));
      Dictionary<Demand, HashSet<TaskAgent>> dictionary = new Dictionary<Demand, HashSet<TaskAgent>>();
      foreach (Tuple<TaskAgent, IList<Demand>> unmatchedAgent in (IEnumerable<Tuple<TaskAgent, IList<Demand>>>) unmatchedAgents)
      {
        foreach (Demand key in (IEnumerable<Demand>) unmatchedAgent.Item2)
        {
          HashSet<TaskAgent> taskAgentSet;
          if (dictionary.TryGetValue(key, out taskAgentSet))
          {
            taskAgentSet.Add(unmatchedAgent.Item1);
            dictionary[key] = taskAgentSet;
          }
          else
            dictionary.Add(key, new HashSet<TaskAgent>()
            {
              unmatchedAgent.Item1
            });
        }
      }
      foreach (Demand demand in (IEnumerable<Demand>) demands)
      {
        HashSet<TaskAgent> source;
        if (dictionary.TryGetValue(demand, out source) && source.Count<TaskAgent>() == unmatchedAgents.Count<Tuple<TaskAgent, IList<Demand>>>())
          return TaskResources.AgentNotFoundMatchingSingleDemand((object) poolName, (object) demand.Name, (object) str);
      }
      int index1 = 0;
      for (int index2 = demands.Count<Demand>(); index1 < index2; ++index1)
      {
        Demand demand1 = demands[index1];
        HashSet<TaskAgent> taskAgentSet1;
        if (dictionary.TryGetValue(demand1, out taskAgentSet1))
        {
          for (int index3 = index1 + 1; index3 < index2; ++index3)
          {
            Demand demand2 = demands[index3];
            HashSet<TaskAgent> taskAgentSet2;
            if (dictionary.TryGetValue(demand2, out taskAgentSet2))
            {
              bool flag = false;
              for (int index4 = 0; index4 < unmatchedAgents.Count && !flag; ++index4)
              {
                if (!taskAgentSet1.Contains(unmatchedAgents[index4].Item1) && !taskAgentSet2.Contains(unmatchedAgents[index4].Item1))
                  flag = true;
              }
              if (!flag)
                return TaskResources.AgentNotFoundMatchingDemandPair((object) poolName, (object) demand1.Name, (object) demand2.Name, (object) str);
            }
          }
        }
      }
      return TaskResources.AgentNotFoundMatchingDemands((object) poolName, (object) str);
    }
  }
}
