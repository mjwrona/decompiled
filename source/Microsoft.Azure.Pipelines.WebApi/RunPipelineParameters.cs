// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class RunPipelineParameters
  {
    [DataMember(Name = "TemplateParameters", EmitDefaultValue = false)]
    private Dictionary<string, string> m_templateParameters;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private Dictionary<string, Variable> m_variables;
    [DataMember(Name = "StagesToSkip", EmitDefaultValue = false)]
    private HashSet<string> m_stagesToSkip;

    [DataMember(EmitDefaultValue = false)]
    public RunResourcesParameters Resources { get; set; }

    public Dictionary<string, string> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }

    public Dictionary<string, Variable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, Variable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public ISet<string> StagesToSkip
    {
      get
      {
        if (this.m_stagesToSkip == null)
          this.m_stagesToSkip = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_stagesToSkip;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string YamlOverride { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool PreviewRun { get; set; }
  }
}
