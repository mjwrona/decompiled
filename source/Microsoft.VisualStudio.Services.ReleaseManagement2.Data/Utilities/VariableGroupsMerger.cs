// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.VariableGroupsMerger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class VariableGroupsMerger
  {
    public static IDictionary<string, MergedConfigurationVariableValue> MergeVariableGroups(
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> groups)
    {
      IDictionary<string, MergedConfigurationVariableValue> dictionary1 = (IDictionary<string, MergedConfigurationVariableValue>) new Dictionary<string, MergedConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (groups == null || !groups.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>())
        return dictionary1;
      List<IDictionary<string, MergedConfigurationVariableValue>> dictionaryList = new List<IDictionary<string, MergedConfigurationVariableValue>>();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup group in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) groups)
      {
        if (!group.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue>>())
        {
          Dictionary<string, MergedConfigurationVariableValue> dictionary2 = new Dictionary<string, MergedConfigurationVariableValue>();
          foreach (KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue>>) group.Variables)
            dictionary2[variable.Key] = new MergedConfigurationVariableValue(group.Id, !group.Type.Equals("Vsts", StringComparison.OrdinalIgnoreCase), variable.Value.Value, variable.Value.IsSecret);
          dictionaryList.Add((IDictionary<string, MergedConfigurationVariableValue>) dictionary2);
        }
      }
      IDictionary<string, MergedConfigurationVariableValue> source = DictionaryMerger.MergeDictionaries<string, MergedConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, MergedConfigurationVariableValue>>) dictionaryList);
      if (source != null && source.Any<KeyValuePair<string, MergedConfigurationVariableValue>>())
        dictionary1 = source;
      return dictionary1;
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> MergeVariableGroups(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiGroups)
    {
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> groups = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      if (webApiGroups != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup webApiGroup in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>) webApiGroups)
          groups.Add(VariableGroupConverter.ToServerVariableGroup(webApiGroup));
      }
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) VariableGroupsMerger.MergeVariableGroups((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) groups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }));
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> GetMergedGroupVariables(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiGroups)
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariableGroupsMerger.MergeVariableGroups(webApiGroups);
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>();
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) dictionary)
        mergedGroupVariables[keyValuePair.Key] = keyValuePair.Value.ToWebApiConfigurationVariableValue();
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) mergedGroupVariables;
    }

    public static MergedConfigurationVariableValue ToMergedConfigurationVariableValue(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return new MergedConfigurationVariableValue(0, false, value.Value, value.IsSecret);
    }
  }
}
