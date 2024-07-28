// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.VariablesUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class VariablesUtility
  {
    public static ConfigurationVariableValue Clone(this ConfigurationVariableValue variableValue)
    {
      if (variableValue == null)
        throw new ArgumentNullException(nameof (variableValue));
      return new ConfigurationVariableValue(variableValue.Value, variableValue.IsSecret, variableValue.AllowOverride);
    }

    public static IDictionary<string, ConfigurationVariableValue> ReplaceSecretVariablesWithNull(
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      Dictionary<string, ConfigurationVariableValue> dictionary = new Dictionary<string, ConfigurationVariableValue>();
      if (variables == null)
        return (IDictionary<string, ConfigurationVariableValue>) dictionary;
      foreach (KeyValuePair<string, ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) variables)
      {
        ConfigurationVariableValue configurationVariableValue = variable.Value.Clone();
        if (variable.Value.IsSecret)
          configurationVariableValue.Value = (string) null;
        dictionary[variable.Key] = configurationVariableValue;
      }
      return (IDictionary<string, ConfigurationVariableValue>) dictionary;
    }

    private static void FillWorker<T>(
      IDictionary<string, T> source,
      IDictionary<string, T> target,
      Action<IDictionary<string, T>, IDictionary<string, T>> handleSourceAndTarget)
    {
      if (target == null)
        throw new ArgumentNullException(nameof (target));
      if (source == null)
        return;
      handleSourceAndTarget(source, target);
    }

    private static void FillWorker<T>(
      IDictionary<string, T> source,
      IDictionary<string, T> target,
      Action<KeyValuePair<string, T>, IDictionary<string, T>> handleSourceKeyValuePair)
    {
      VariablesUtility.FillWorker<T>(source, target, (Action<IDictionary<string, T>, IDictionary<string, T>>) ((source2, target2) =>
      {
        foreach (KeyValuePair<string, T> keyValuePair in (IEnumerable<KeyValuePair<string, T>>) source2)
          handleSourceKeyValuePair(keyValuePair, target2);
      }));
    }

    public static void FillVariables(
      IDictionary<string, ConfigurationVariableValue> source,
      IDictionary<string, ConfigurationVariableValue> target)
    {
      VariablesUtility.FillWorker<ConfigurationVariableValue>(source, target, (Action<KeyValuePair<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>>) ((kvp, target2) => target2[kvp.Key] = kvp.Value.Clone()));
    }

    public static void FillSecrets(
      IDictionary<string, ConfigurationVariableValue> source,
      IDictionary<string, ConfigurationVariableValue> target)
    {
      VariablesUtility.FillWorker<ConfigurationVariableValue>(source, target, (Action<IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>>) ((source2, target2) =>
      {
        foreach (KeyValuePair<string, ConfigurationVariableValue> keyValuePair in source2.Where<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (x => x.Value.IsSecret)))
          target2[keyValuePair.Key] = keyValuePair.Value.Clone();
      }));
    }

    public static bool HasSecretsWithValues(
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      return variables.Any<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (s => s.Value != null && s.Value.IsSecret && !string.IsNullOrEmpty(s.Value.Value)));
    }

    public static bool HasSecrets(
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      return variables.Any<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (s => s.Value != null && s.Value.IsSecret));
    }
  }
}
