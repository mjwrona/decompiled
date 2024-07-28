// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.CreateYamlPipelineConfigurationParameters
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class CreateYamlPipelineConfigurationParameters : CreatePipelineConfigurationParameters
  {
    [DataMember(Name = "variables", EmitDefaultValue = false)]
    private Dictionary<string, Variable> m_variables;

    public CreateYamlPipelineConfigurationParameters()
      : base(ConfigurationType.Yaml)
    {
    }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public CreateRepositoryParameters Repository { get; set; }

    public Dictionary<string, Variable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, Variable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }
  }
}
