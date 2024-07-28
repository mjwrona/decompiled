// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.VariableGroup
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
  public class VariableGroup : VariableGroupReference
  {
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, BuildDefinitionVariable> m_serializedVariables;
    private IDictionary<string, BuildDefinitionVariable> m_variables;

    public VariableGroup()
      : this((ISecuredObject) null)
    {
    }

    internal VariableGroup(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    public IDictionary<string, BuildDefinitionVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
      internal set => this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_serializedVariables, ref this.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_variables, ref this.m_serializedVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedVariables = (IDictionary<string, BuildDefinitionVariable>) null;
  }
}
