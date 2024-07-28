// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class VariableGroupUtility
  {
    private static Type keyVaultVariableType = typeof (AzureKeyVaultVariableValue);

    public static VariableValue Clone(this VariableValue value) => VariableGroupUtility.keyVaultVariableType == value.GetType() ? (VariableValue) new AzureKeyVaultVariableValue((AzureKeyVaultVariableValue) value) : new VariableValue(value);

    public static void PopulateVariablesAndProviderData(
      this VariableGroup group,
      string variablesJson,
      string providerDataJson)
    {
      switch (group.Type)
      {
        case "Vsts":
          if (variablesJson != null)
            group.Variables = JsonUtility.FromString<IDictionary<string, VariableValue>>(variablesJson);
          if (providerDataJson == null)
            break;
          group.ProviderData = JsonUtility.FromString<VariableGroupProviderData>(providerDataJson);
          break;
        case "AzureKeyVault":
          if (variablesJson != null)
          {
            IDictionary<string, AzureKeyVaultVariableValue> dictionary = JsonUtility.FromString<IDictionary<string, AzureKeyVaultVariableValue>>(variablesJson);
            if (dictionary != null)
            {
              foreach (KeyValuePair<string, AzureKeyVaultVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, AzureKeyVaultVariableValue>>) dictionary)
                group.Variables[keyValuePair.Key] = (VariableValue) keyValuePair.Value;
            }
          }
          if (providerDataJson == null)
            break;
          group.ProviderData = (VariableGroupProviderData) JsonUtility.FromString<AzureKeyVaultVariableGroupProviderData>(providerDataJson);
          break;
      }
    }

    public static void PopulateVariablesAndProviderData(
      this VariableGroupParameters variableGroupParameters,
      string variablesJson,
      string providerDataJson)
    {
      switch (variableGroupParameters.Type)
      {
        case "Vsts":
          if (variablesJson != null)
            variableGroupParameters.Variables = JsonUtility.FromString<IDictionary<string, VariableValue>>(variablesJson);
          if (providerDataJson == null)
            break;
          variableGroupParameters.ProviderData = JsonUtility.FromString<VariableGroupProviderData>(providerDataJson);
          break;
        case "AzureKeyVault":
          if (variablesJson != null)
          {
            IDictionary<string, AzureKeyVaultVariableValue> dictionary = JsonUtility.FromString<IDictionary<string, AzureKeyVaultVariableValue>>(variablesJson);
            if (dictionary != null)
            {
              foreach (KeyValuePair<string, AzureKeyVaultVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, AzureKeyVaultVariableValue>>) dictionary)
                variableGroupParameters.Variables[keyValuePair.Key] = (VariableValue) keyValuePair.Value;
            }
          }
          if (providerDataJson == null)
            break;
          variableGroupParameters.ProviderData = (VariableGroupProviderData) JsonUtility.FromString<AzureKeyVaultVariableGroupProviderData>(providerDataJson);
          break;
      }
    }

    public static IList<VariableGroup> CloneVariableGroups(IList<VariableGroup> source)
    {
      List<VariableGroup> variableGroupList = new List<VariableGroup>();
      if (source == null)
        return (IList<VariableGroup>) variableGroupList;
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) source)
      {
        if (variableGroup != null)
          variableGroupList.Add(variableGroup.Clone());
      }
      return (IList<VariableGroup>) variableGroupList;
    }

    public static IList<VariableGroup> ClearSecrets(IList<VariableGroup> variableGroups)
    {
      List<VariableGroup> variableGroupList = new List<VariableGroup>();
      if (variableGroups == null)
        return (IList<VariableGroup>) variableGroupList;
      foreach (VariableGroup variableGroup1 in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (variableGroup1 != null)
        {
          VariableGroup variableGroup2 = variableGroup1.Clone();
          foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variableGroup2.Variables)
          {
            if (variable.Value != null && variable.Value.IsSecret)
              variable.Value.Value = (string) null;
          }
          variableGroupList.Add(variableGroup2);
        }
      }
      return (IList<VariableGroup>) variableGroupList;
    }

    public static IDictionary<string, VariableValue> ClearSecrets(
      IDictionary<string, VariableValue> variables)
    {
      Dictionary<string, VariableValue> dictionary = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (variables == null)
        return (IDictionary<string, VariableValue>) dictionary;
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variables)
      {
        if (variable.Value != null)
        {
          VariableValue variableValue = variable.Value.Clone();
          if (variable.Value.IsSecret)
            variableValue.Value = (string) null;
          dictionary[variable.Key] = variableValue;
        }
      }
      return (IDictionary<string, VariableValue>) dictionary;
    }

    public static bool HasSecretWithValue(IList<VariableGroup> variableGroups)
    {
      if (variableGroups == null || variableGroups.Count == 0)
        return false;
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (variableGroup != null && VariableGroupUtility.HasSecretWithValue(variableGroup.Variables))
          return true;
      }
      return false;
    }

    public static bool HasSecretWithValue(IDictionary<string, VariableValue> variables) => variables != null && variables.Count != 0 && variables.Any<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (s => s.Value != null && s.Value.IsSecret && !string.IsNullOrEmpty(s.Value.Value)));

    public static bool HasSecret(IList<VariableGroup> variableGroups)
    {
      if (variableGroups == null || variableGroups.Count == 0)
        return false;
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) variableGroups)
      {
        if (variableGroup != null && VariableGroupUtility.HasSecret(variableGroup.Variables))
          return true;
      }
      return false;
    }

    public static bool HasSecret(IDictionary<string, VariableValue> variables) => variables != null && variables.Any<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value != null && v.Value.IsSecret));

    public static void FillSecrets(
      IList<VariableGroup> sourceGroups,
      IList<VariableGroup> targetGroups)
    {
      if (sourceGroups == null || sourceGroups.Count == 0)
        return;
      if (targetGroups == null)
        throw new ArgumentNullException(nameof (targetGroups));
      foreach (VariableGroup sourceGroup1 in (IEnumerable<VariableGroup>) sourceGroups)
      {
        VariableGroup sourceGroup = sourceGroup1;
        VariableGroup variableGroup = targetGroups.FirstOrDefault<VariableGroup>((Func<VariableGroup, bool>) (group => group.Id == sourceGroup.Id));
        if (variableGroup != null && sourceGroup.Variables != null && sourceGroup.Variables.Count != 0)
        {
          if (variableGroup.Variables == null)
            throw new ArgumentNullException("Variables");
          foreach (KeyValuePair<string, VariableValue> keyValuePair in sourceGroup.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => x.Value.IsSecret)))
            variableGroup.Variables[keyValuePair.Key] = keyValuePair.Value.Clone();
        }
      }
    }
  }
}
