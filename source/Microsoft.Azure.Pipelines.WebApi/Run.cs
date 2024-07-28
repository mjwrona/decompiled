// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Run
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class Run : RunReference
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private Dictionary<string, Variable> m_variables;
    [DataMember(Name = "TemplateParameters", EmitDefaultValue = false)]
    private Dictionary<string, object> m_templateParameters;

    [JsonConstructor]
    private Run()
    {
    }

    internal Run(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public PipelineReference Pipeline { get; set; }

    [DataMember]
    public RunState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public RunResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url => this.m_links.GetLink("self");

    [DataMember(EmitDefaultValue = false)]
    public RunResources Resources { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FinalYaml { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
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

    public Dictionary<string, object> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }
  }
}
