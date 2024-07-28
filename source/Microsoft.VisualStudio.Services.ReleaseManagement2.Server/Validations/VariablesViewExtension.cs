// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.VariablesViewExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class VariablesViewExtension
  {
    public static bool Diff(VariablesView oldView, VariablesView newView, out VariablesView diff)
    {
      bool flag = false;
      diff = new VariablesView();
      if (oldView == null && newView == null)
        return false;
      if (oldView == null)
      {
        diff = newView;
        return true;
      }
      if (newView == null)
        return false;
      foreach (KeyValuePair<string, IDictionary<string, string>> environmentVariable in (IEnumerable<KeyValuePair<string, IDictionary<string, string>>>) oldView.MergedEnvironmentVariables)
      {
        string key = environmentVariable.Key;
        IDictionary<string, string> oldData;
        IDictionary<string, string> newData;
        if (oldView.MergedEnvironmentVariables.TryGetValue(key, out oldData) && newView.MergedEnvironmentVariables.TryGetValue(key, out newData))
        {
          IDictionary<string, string> source = VariablesViewExtension.DiffDictionary(oldData, newData);
          if (source.Any<KeyValuePair<string, string>>())
            diff.MergedEnvironmentVariables.Add(key, source);
        }
      }
      if (diff.MergedEnvironmentVariables.Any<KeyValuePair<string, IDictionary<string, string>>>())
        flag = true;
      return flag;
    }

    public static VariablesView GetVariablesView(ReleaseDefinition releaseDefinition)
    {
      VariablesView variablesView = new VariablesView();
      if (releaseDefinition == null || releaseDefinition.Environments == null)
        return variablesView;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        IDictionary<string, string> environmentVariables = ServiceEndpointVariablesHelper.GetMergedEnvironmentVariables(releaseDefinition, environment);
        variablesView.MergedEnvironmentVariables.Add(ServiceEndpointVariablesHelper.GetEnvironmentKey(environment), environmentVariables);
      }
      return variablesView;
    }

    public static VariablesView GetVariablesView(Release release)
    {
      VariablesView variablesView = new VariablesView();
      if (release == null || release.Environments == null)
        return variablesView;
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
      {
        IDictionary<string, string> environmentVariables = ServiceEndpointVariablesHelper.GetMergedEnvironmentVariables(release, environment);
        variablesView.MergedEnvironmentVariables.Add(ServiceEndpointVariablesHelper.GetEnvironmentKey(environment), environmentVariables);
      }
      return variablesView;
    }

    private static IDictionary<string, string> DiffDictionary(
      IDictionary<string, string> oldData,
      IDictionary<string, string> newData)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) oldData)
      {
        string key = keyValuePair.Key;
        string a = keyValuePair.Value;
        if (newData.ContainsKey(key))
        {
          string b = newData[key];
          if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
            dictionary.Add(key, b);
        }
      }
      return (IDictionary<string, string>) dictionary;
    }
  }
}
