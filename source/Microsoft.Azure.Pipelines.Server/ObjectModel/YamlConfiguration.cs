// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.YamlConfiguration
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  [DataContract]
  public class YamlConfiguration : PipelineConfiguration
  {
    private static readonly int CurrentVersion = 1;
    private Dictionary<string, Variable> m_variables;

    public YamlConfiguration()
      : base(ConfigurationType.Yaml, YamlConfiguration.CurrentVersion)
    {
    }

    [DataMember]
    public string Path { get; set; }

    public Repository Repository { get; set; }

    public IDictionary<string, Variable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, Variable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, Variable>) this.m_variables;
      }
    }
  }
}
