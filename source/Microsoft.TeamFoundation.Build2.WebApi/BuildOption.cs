// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildOption
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildOption : BaseSecuredObject
  {
    [DataMember(Name = "Inputs", EmitDefaultValue = false, Order = 2)]
    private Dictionary<string, string> m_inputs;

    public BuildOption()
    {
    }

    internal BuildOption(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = true, Order = 1, Name = "Definition")]
    public virtual BuildOptionDefinitionReference BuildOptionDefinition { get; set; }

    public virtual IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_inputs;
      }
      internal set => this.m_inputs = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = true)]
    public virtual bool Enabled { get; set; }
  }
}
