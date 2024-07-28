// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.VariablesHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class VariablesHelper
  {
    public static void PopulateVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      IOrchestrationEnvironment environment,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups,
      IDictionary<string, BuildDefinitionVariable> buildDefinitionVariables,
      IDictionary<string, string> queueTimeVariables,
      IDictionary<string, string> wellKnownVariables,
      IDictionary<string, string> buildDefinitionSecretVariables,
      out List<TaskInstance> tasksToInject)
    {
      tasksToInject = new List<TaskInstance>();
      queueTimeVariables?.Remove(string.Empty);
      Dictionary<string, BuildDefinitionVariableWrapper> mergedVariables = new Dictionary<string, BuildDefinitionVariableWrapper>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (variableGroups != null && variableGroups.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) variableGroups)
        {
          foreach (KeyValuePair<string, VariableValue> keyValuePair in variableGroup.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value != null)))
          {
            BuildDefinitionVariableWrapper definitionVariableWrapper1;
            if (mergedVariables.TryGetValue(keyValuePair.Key, out definitionVariableWrapper1))
            {
              definitionVariableWrapper1.VariableGroupId = new int?(variableGroup.Id);
              definitionVariableWrapper1.DefinitionVariable.Value = keyValuePair.Value.Value;
              definitionVariableWrapper1.IsKeyVaultSecret = variableGroup.IsKeyVaultType();
            }
            else
            {
              BuildDefinitionVariableWrapper definitionVariableWrapper2 = new BuildDefinitionVariableWrapper(keyValuePair.Value)
              {
                IsKeyVaultSecret = variableGroup.IsKeyVaultType(),
                VariableGroupId = new int?(variableGroup.Id)
              };
              mergedVariables[keyValuePair.Key] = definitionVariableWrapper2;
            }
          }
        }
      }
      if (buildDefinitionVariables != null && buildDefinitionVariables.Count > 0)
      {
        foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in buildDefinitionVariables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (v => v.Value != null)))
        {
          string str;
          if (buildDefinitionSecretVariables != null && keyValuePair.Value.IsSecret && buildDefinitionSecretVariables.TryGetValue(keyValuePair.Key, out str))
          {
            mergedVariables[keyValuePair.Key] = new BuildDefinitionVariableWrapper(new VariableValue()
            {
              IsSecret = true,
              Value = str
            });
          }
          else
          {
            BuildDefinitionVariableWrapper definitionVariableWrapper;
            if (mergedVariables.TryGetValue(keyValuePair.Key, out definitionVariableWrapper))
            {
              definitionVariableWrapper.DefinitionVariable.Value = keyValuePair.Value.Value;
              if (definitionVariableWrapper.IsKeyVaultSecret)
              {
                definitionVariableWrapper.IsKeyVaultSecret = false;
                definitionVariableWrapper.DefinitionVariable.IsSecret = true;
              }
            }
            else
              mergedVariables[keyValuePair.Key] = new BuildDefinitionVariableWrapper(new VariableValue()
              {
                IsSecret = keyValuePair.Value.IsSecret,
                Value = keyValuePair.Value.Value
              });
          }
        }
      }
      if (queueTimeVariables != null && queueTimeVariables.Count > 0)
      {
        foreach (string key in queueTimeVariables.Keys.ToArray<string>())
        {
          BuildDefinitionVariableWrapper definitionVariableWrapper;
          if (mergedVariables.TryGetValue(key, out definitionVariableWrapper))
          {
            string b = definitionVariableWrapper.DefinitionVariable.Value;
            definitionVariableWrapper.DefinitionVariable.Value = queueTimeVariables[key];
            if (definitionVariableWrapper.DefinitionVariable.IsSecret || definitionVariableWrapper.IsKeyVaultSecret)
            {
              queueTimeVariables[key] = (string) null;
              definitionVariableWrapper.IsKeyVaultSecret = false;
              definitionVariableWrapper.DefinitionVariable.IsSecret = true;
            }
            else if (string.Equals(definitionVariableWrapper.DefinitionVariable.Value, b, StringComparison.Ordinal))
              queueTimeVariables.Remove(key);
          }
          else
            mergedVariables[key] = new BuildDefinitionVariableWrapper(new VariableValue()
            {
              IsSecret = false,
              Value = queueTimeVariables[key]
            });
        }
      }
      if (wellKnownVariables != null && wellKnownVariables.Count > 0)
      {
        foreach (KeyValuePair<string, string> wellKnownVariable in (IEnumerable<KeyValuePair<string, string>>) wellKnownVariables)
          mergedVariables[wellKnownVariable.Key] = new BuildDefinitionVariableWrapper(new VariableValue()
          {
            IsSecret = false,
            Value = wellKnownVariable.Value
          });
      }
      foreach (KeyValuePair<string, BuildDefinitionVariableWrapper> keyValuePair in mergedVariables)
      {
        VariableValue variableValue;
        if (environment.Variables.TryGetValue(keyValuePair.Key, out variableValue))
        {
          variableValue.Value = keyValuePair.Value.DefinitionVariable.Value;
          variableValue.IsSecret |= keyValuePair.Value.DefinitionVariable.IsSecret;
        }
        else
          environment.Variables[keyValuePair.Key] = new VariableValue(keyValuePair.Value.DefinitionVariable.Value, keyValuePair.Value.DefinitionVariable.IsSecret);
      }
      if (!(environment is PlanEnvironment))
        return;
      VariablesHelper.InjectContainerTasks(mergedVariables, variableGroups, out tasksToInject);
    }

    private static void InjectContainerTasks(
      Dictionary<string, BuildDefinitionVariableWrapper> mergedVariables,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups,
      out List<TaskInstance> legacyContainerTasksToInject)
    {
      legacyContainerTasksToInject = (List<TaskInstance>) null;
      Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
      foreach (KeyValuePair<string, BuildDefinitionVariableWrapper> keyValuePair in mergedVariables.Where<KeyValuePair<string, BuildDefinitionVariableWrapper>>((Func<KeyValuePair<string, BuildDefinitionVariableWrapper>, bool>) (x => x.Value.IsKeyVaultSecret && x.Value.VariableGroupId.HasValue)))
      {
        List<string> stringList;
        if (!dictionary.TryGetValue(keyValuePair.Value.VariableGroupId.Value, out stringList))
        {
          stringList = new List<string>();
          dictionary[keyValuePair.Value.VariableGroupId.Value] = stringList;
        }
        stringList.Add(keyValuePair.Key);
      }
      if (variableGroups == null || variableGroups.Count <= 0)
        return;
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup group in variableGroups.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (x => x.IsKeyVaultType())))
      {
        List<string> externalVariablesFilter;
        if (dictionary.TryGetValue(group.Id, out externalVariablesFilter))
        {
          IList<TaskInstance> variableGroupTasks = group.GetVariableGroupTasks((IList<string>) externalVariablesFilter);
          if (legacyContainerTasksToInject == null)
            legacyContainerTasksToInject = new List<TaskInstance>();
          legacyContainerTasksToInject.AddRange((IEnumerable<TaskInstance>) variableGroupTasks);
        }
      }
    }
  }
}
