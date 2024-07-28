// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [JsonConverter(typeof (VariableGroupJsonConverter))]
  [DataContract]
  public class VariableGroup
  {
    [DataMember(EmitDefaultValue = false, Name = "Variables")]
    private Dictionary<string, VariableValue> m_variables;

    public VariableGroup()
    {
    }

    private VariableGroup(VariableGroup group)
    {
      this.Id = group.Id;
      this.Type = group.Type;
      this.Name = group.Name;
      this.Description = group.Description;
      this.ProviderData = group.ProviderData;
      this.CreatedBy = group.CreatedBy;
      this.CreatedOn = group.CreatedOn;
      this.ModifiedBy = group.ModifiedBy;
      this.ModifiedOn = group.ModifiedOn;
      this.IsShared = group.IsShared;
      this.Variables = (IDictionary<string, VariableValue>) group.Variables.ToDictionary<KeyValuePair<string, VariableValue>, string, VariableValue>((Func<KeyValuePair<string, VariableValue>, string>) (x => x.Key), (Func<KeyValuePair<string, VariableValue>, VariableValue>) (x => x.Value.Clone()));
      IList<VariableGroupProjectReference> projectReferences = group.VariableGroupProjectReferences;
      this.VariableGroupProjectReferences = projectReferences != null ? (IList<VariableGroupProjectReference>) projectReferences.Select<VariableGroupProjectReference, VariableGroupProjectReference>((Func<VariableGroupProjectReference, VariableGroupProjectReference>) (item => item.Clone())).ToList<VariableGroupProjectReference>() : (IList<VariableGroupProjectReference>) null;
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public VariableGroupProviderData ProviderData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsShared { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public IList<VariableGroupProjectReference> VariableGroupProjectReferences { get; set; }

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

    public VariableGroup Clone() => new VariableGroup(this);
  }
}
