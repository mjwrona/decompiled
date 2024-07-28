// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.VariableGroup
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class VariableGroup
  {
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, BuildDefinitionVariable> m_serializedVariables;
    private IDictionary<string, BuildDefinitionVariable> m_variables;

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

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
      set => this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public VariableGroup Clone()
    {
      VariableGroup variableGroup = new VariableGroup();
      variableGroup.Id = this.Id;
      variableGroup.Type = this.Type;
      variableGroup.Name = this.Name;
      variableGroup.Description = this.Description;
      foreach (string key in (IEnumerable<string>) this.Variables.Keys)
        variableGroup.Variables.Add(key, this.Variables[key]);
      return variableGroup;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_serializedVariables, ref this.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_variables, ref this.m_serializedVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedVariables = (IDictionary<string, BuildDefinitionVariable>) null;
  }
}
