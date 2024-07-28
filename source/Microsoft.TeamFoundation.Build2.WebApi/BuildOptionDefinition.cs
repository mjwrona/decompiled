// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildOptionDefinition : BuildOptionDefinitionReference
  {
    private List<BuildOptionInputDefinition> m_inputs;
    private List<BuildOptionGroupDefinition> m_groups;

    public BuildOptionDefinition()
    {
    }

    internal BuildOptionDefinition(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public int Ordinal { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<BuildOptionInputDefinition> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new List<BuildOptionInputDefinition>();
        return (IList<BuildOptionInputDefinition>) this.m_inputs;
      }
      set => this.m_inputs = new List<BuildOptionInputDefinition>((IEnumerable<BuildOptionInputDefinition>) value);
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<BuildOptionGroupDefinition> Groups
    {
      get
      {
        if (this.m_groups == null)
          this.m_groups = new List<BuildOptionGroupDefinition>();
        return (IList<BuildOptionGroupDefinition>) this.m_groups;
      }
      set => this.m_groups = new List<BuildOptionGroupDefinition>((IEnumerable<BuildOptionGroupDefinition>) value);
    }
  }
}
