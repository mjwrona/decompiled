// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.PolicyAssignment
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonTransformation]
  public class PolicyAssignment : IResource
  {
    public PolicyAssignment()
    {
    }

    public PolicyAssignment(
      string displayName = null,
      string policyDefinitionId = null,
      string scope = null,
      IList<string> notScopes = null,
      IDictionary<string, ParameterValuesValue> parameters = null,
      string description = null,
      object metadata = null,
      string enforcementMode = null,
      string id = null,
      string type = null,
      string name = null,
      PolicySku sku = null,
      string location = null,
      Identity identity = null)
    {
      this.DisplayName = displayName;
      this.PolicyDefinitionId = policyDefinitionId;
      this.Scope = scope;
      this.NotScopes = notScopes;
      this.Parameters = parameters;
      this.Description = description;
      this.Metadata = metadata;
      this.EnforcementMode = enforcementMode;
      this.Id = id;
      this.Type = type;
      this.Name = name;
      this.Sku = sku;
      this.Location = location;
      this.Identity = identity;
    }

    [JsonProperty(PropertyName = "properties.displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "properties.policyDefinitionId")]
    public string PolicyDefinitionId { get; set; }

    [JsonProperty(PropertyName = "properties.scope")]
    public string Scope { get; set; }

    [JsonProperty(PropertyName = "properties.notScopes")]
    public IList<string> NotScopes { get; set; }

    [JsonProperty(PropertyName = "properties.parameters")]
    public IDictionary<string, ParameterValuesValue> Parameters { get; set; }

    [JsonProperty(PropertyName = "properties.description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "properties.metadata")]
    public object Metadata { get; set; }

    [JsonProperty(PropertyName = "properties.enforcementMode")]
    public string EnforcementMode { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "sku")]
    public PolicySku Sku { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "identity")]
    public Identity Identity { get; set; }

    public virtual void Validate()
    {
      if (this.Sku == null)
        return;
      this.Sku.Validate();
    }
  }
}
