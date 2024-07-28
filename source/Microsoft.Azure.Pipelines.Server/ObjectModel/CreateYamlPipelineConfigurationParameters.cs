// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.CreateYamlPipelineConfigurationParameters
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class CreateYamlPipelineConfigurationParameters : CreatePipelineConfigurationParameters
  {
    private Dictionary<string, Variable> m_variables;

    public CreateYamlPipelineConfigurationParameters()
      : base(ConfigurationType.Yaml)
    {
    }

    public string Path { get; set; }

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

    public override string GeneratePipelineName()
    {
      if (string.IsNullOrEmpty(this.Path))
        return string.Empty;
      return this.Path.Substring(this.Path.LastIndexOfAny(new char[2]
      {
        '\\',
        '/'
      }) + 1);
    }
  }
}
