// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [JsonConverter(typeof (VariableGroupJsonConverter))]
  [DataContract]
  public class VariableGroup : ReleaseManagementSecuredObject
  {
    [DataMember(EmitDefaultValue = false, Name = "Variables")]
    private Dictionary<string, VariableValue> variables;

    public VariableGroup() => this.variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public VariableGroup(VariableGroup group)
    {
      if (group == null)
        return;
      this.Id = group.Id;
      this.Type = group.Type;
      this.Name = group.Name;
      this.Description = group.Description;
      this.ProviderData = group.ProviderData;
      this.CreatedBy = group.CreatedBy;
      this.variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) group.Variables)
        this.variables[variable.Key] = variable.Value.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

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

    public IDictionary<string, VariableValue> Variables => (IDictionary<string, VariableValue>) this.variables;

    public VariableGroup Clone() => new VariableGroup(this);

    public override int GetHashCode() => this.Id.GetHashCode();

    public override bool Equals(object obj)
    {
      VariableGroup variableGroup2 = obj as VariableGroup;
      return variableGroup2 != null && this.Id == variableGroup2.Id && !(this.Name != variableGroup2.Name) && !(this.Type != variableGroup2.Type) && this.IsShared == variableGroup2.IsShared && this.Variables.Count == variableGroup2.Variables.Count && this.Variables.All<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (variable => variableGroup2.Variables.ContainsKey(variable.Key) && variableGroup2.Variables[variable.Key].Value == variable.Value.Value && variableGroup2.Variables[variable.Key].IsSecret == variable.Value.IsSecret));
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ProviderData?.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, VariableValue> variables = this.Variables;
      if (variables == null)
        return;
      variables.ForEach<KeyValuePair<string, VariableValue>>((Action<KeyValuePair<string, VariableValue>>) (i => i.Value?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
