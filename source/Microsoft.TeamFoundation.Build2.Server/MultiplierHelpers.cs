// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.MultiplierHelpers
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class MultiplierHelpers
  {
    private static readonly char[] MultiplierValueSeparator = new char[1]
    {
      ','
    };

    public static Dictionary<string, IDictionary<string, string>> GetMatrix(
      this IVariableMultiplierExecutionOptions multiplierOptions,
      IDictionary<string, VariableValue> environment,
      bool isGatedTrigger = false)
    {
      Dictionary<string, IDictionary<string, string>> matrix = new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (multiplierOptions != null)
      {
        IDictionary<string, List<string>> multiplierValues = MultiplierHelpers.GetMultiplierValues(environment, (IReadOnlyList<string>) multiplierOptions.Multipliers);
        if (multiplierValues.Count > 0)
        {
          List<List<KeyValuePair<string, string>>> combinations = MultiplierHelpers.ComputeCombinations(multiplierValues);
          for (int index = 0; index < combinations.Count; ++index)
          {
            List<KeyValuePair<string, string>> source = combinations[index];
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> keyValuePair in source)
              dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            if (index > 0 & isGatedTrigger)
            {
              dictionary.Add("build.gated.runci", string.Empty);
              dictionary.Add("build.gated.shelvesetname", string.Empty);
            }
            string key = string.Join(BuildServerResources.MultipliedBuildJobValueSeparator(), source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Value)));
            matrix[key] = (IDictionary<string, string>) dictionary;
          }
        }
        else
        {
          string key = string.Join(BuildServerResources.MultipliedBuildJobValueSeparator(), (IEnumerable<string>) multiplierOptions.Multipliers);
          matrix[key] = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        }
      }
      return matrix;
    }

    public static IList<TaskOrchestrationJob> MultiplyJob(
      TaskOrchestrationJob sourceJob,
      IDictionary<string, List<string>> multiplierValues)
    {
      return MultiplierHelpers.MultiplyJobs((IList<TaskOrchestrationJob>) new TaskOrchestrationJob[1]
      {
        sourceJob
      }, multiplierValues);
    }

    public static IList<TaskOrchestrationJob> MultiplyJobs(
      IList<TaskOrchestrationJob> sourceJobs,
      IDictionary<string, List<string>> multiplierValues)
    {
      List<TaskOrchestrationJob> orchestrationJobList = new List<TaskOrchestrationJob>();
      int num = 0;
      foreach (List<KeyValuePair<string, string>> combination in MultiplierHelpers.ComputeCombinations(multiplierValues))
      {
        foreach (TaskOrchestrationJob sourceJob in (IEnumerable<TaskOrchestrationJob>) sourceJobs)
        {
          TaskOrchestrationJob orchestrationJob = sourceJob.Clone();
          orchestrationJob.InstanceId = Guid.NewGuid();
          orchestrationJob.Name = BuildServerResources.MultipliedBuildJobFormat((object) orchestrationJob.Name, (object) string.Join(BuildServerResources.MultipliedBuildJobValueSeparator(), combination.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Value))));
          orchestrationJob.RefName = MultiplierHelpers.GetJobRefName(orchestrationJob.Name, ++num);
          foreach (TaskInstance task in orchestrationJob.Tasks)
            task.InstanceId = Guid.NewGuid();
          foreach (KeyValuePair<string, string> keyValuePair in combination)
            orchestrationJob.Variables[keyValuePair.Key] = keyValuePair.Value;
          orchestrationJobList.Add(orchestrationJob);
        }
      }
      return (IList<TaskOrchestrationJob>) orchestrationJobList;
    }

    public static IEnumerable<TaskOrchestrationJob> MultiplyJob(
      TaskOrchestrationJob sourceJob,
      int count)
    {
      for (int i = 0; i < count; ++i)
      {
        int counter = i + 1;
        TaskOrchestrationJob orchestrationJob = sourceJob.Clone();
        orchestrationJob.InstanceId = Guid.NewGuid();
        orchestrationJob.Name = BuildServerResources.MultipliedBuildJobFormat((object) orchestrationJob.Name, (object) counter);
        orchestrationJob.RefName = MultiplierHelpers.GetJobRefName(orchestrationJob.Name, counter);
        foreach (TaskInstance task in orchestrationJob.Tasks)
          task.InstanceId = Guid.NewGuid();
        yield return orchestrationJob;
      }
    }

    public static IDictionary<string, List<string>> GetMultiplierValues(
      IDictionary<string, VariableValue> environment,
      IReadOnlyList<string> multiplierVariables)
    {
      Dictionary<string, List<string>> multiplierValues = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string key in multiplierVariables.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        VariableValue variableValue;
        if (!string.IsNullOrEmpty(key) && environment.TryGetValue(key, out variableValue) && variableValue != null && variableValue.Value != null && !variableValue.IsSecret)
        {
          List<string> list = ((IEnumerable<string>) variableValue.Value.Split(MultiplierHelpers.MultiplierValueSeparator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (value => value.Trim())).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
          if (list.Any<string>())
            multiplierValues.Add(key, list);
        }
      }
      return (IDictionary<string, List<string>>) multiplierValues;
    }

    private static List<List<KeyValuePair<string, string>>> ComputeCombinations(
      IDictionary<string, List<string>> multipliers)
    {
      List<List<KeyValuePair<string, string>>> results = new List<List<KeyValuePair<string, string>>>();
      KeyValuePair<string, string>[] workingSet = new KeyValuePair<string, string>[multipliers.Count];
      MultiplierHelpers.ComputeCombinations(multipliers, results, workingSet, multipliers.Keys.ToList<string>(), 0);
      return results;
    }

    private static void ComputeCombinations(
      IDictionary<string, List<string>> multipliers,
      List<List<KeyValuePair<string, string>>> results,
      KeyValuePair<string, string>[] workingSet,
      List<string> keys,
      int index)
    {
      string key = keys[index];
      foreach (string str in multipliers[key])
      {
        workingSet[index] = new KeyValuePair<string, string>(key, str);
        if (index == keys.Count - 1)
          results.Add(((IEnumerable<KeyValuePair<string, string>>) workingSet).ToList<KeyValuePair<string, string>>());
        else
          MultiplierHelpers.ComputeCombinations(multipliers, results, workingSet, keys, index + 1);
      }
    }

    private static string GetJobRefName(string jobName, int counter)
    {
      string empty = string.Empty;
      foreach (char ch in jobName.ToCharArray())
      {
        if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '_')
          empty += ch.ToString();
      }
      return string.Format("{0}_{1}", (object) empty, (object) counter);
    }
  }
}
