// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupParameters
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [JsonConverter(typeof (VariableGroupParametersJsonConverter))]
  [DataContract]
  public class VariableGroupParameters
  {
    [DataMember(EmitDefaultValue = false, Name = "Variables")]
    private Dictionary<string, VariableValue> m_variables;
    [DataMember(EmitDefaultValue = false, Name = "VariableGroupProjectReferences")]
    private IList<VariableGroupProjectReference> m_variableGroupProjectReferences;

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public VariableGroupProviderData ProviderData { get; set; }

    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
      set
      {
        if (value == null)
          this.m_variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        else
          this.m_variables = new Dictionary<string, VariableValue>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public IList<VariableGroupProjectReference> VariableGroupProjectReferences
    {
      get
      {
        if (this.m_variableGroupProjectReferences == null)
          this.m_variableGroupProjectReferences = (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>();
        return this.m_variableGroupProjectReferences;
      }
      set
      {
        if (value == null)
          this.m_variableGroupProjectReferences = (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>();
        else
          this.m_variableGroupProjectReferences = value;
      }
    }
  }
}
