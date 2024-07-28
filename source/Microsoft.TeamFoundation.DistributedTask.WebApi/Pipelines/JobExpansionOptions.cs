// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.JobExpansionOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public class JobExpansionOptions
  {
    public const int NoSpecifiedAttemptNumber = -1;
    private Dictionary<string, int> m_configurations;

    public JobExpansionOptions(ICollection<string> configurations) => this.AddConfigurations(configurations);

    internal JobExpansionOptions(IDictionary<string, int> configurations) => this.UpdateConfigurations(configurations);

    internal JobExpansionOptions(string configuration, int attemptNumber = -1)
    {
      if (string.IsNullOrEmpty(configuration))
        return;
      this.Configurations.Add(configuration, attemptNumber);
    }

    internal IDictionary<string, int> Configurations
    {
      get
      {
        if (this.m_configurations == null)
          this.m_configurations = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, int>) this.m_configurations;
      }
    }

    public bool IsIncluded(string configuration) => this.m_configurations == null || this.m_configurations.ContainsKey(configuration);

    public void AddConfigurations(ICollection<string> configurations)
    {
      if (configurations == null)
        return;
      IDictionary<string, int> configurations1 = this.Configurations;
      foreach (string configuration in (IEnumerable<string>) configurations)
      {
        if (!configurations1.ContainsKey(configuration))
          configurations1[configuration] = -1;
      }
    }

    public void UpdateConfigurations(IDictionary<string, int> configurations)
    {
      if (configurations == null)
        return;
      IDictionary<string, int> configurations1 = this.Configurations;
      foreach (KeyValuePair<string, int> configuration in (IEnumerable<KeyValuePair<string, int>>) configurations)
        configurations1[configuration.Key] = configuration.Value;
    }

    public int GetAttemptNumber(string configuration)
    {
      int num;
      return this.m_configurations != null && this.m_configurations.TryGetValue(configuration, out num) ? num : -1;
    }
  }
}
