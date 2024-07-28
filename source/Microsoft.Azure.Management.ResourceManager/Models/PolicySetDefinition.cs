// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.PolicySetDefinition
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonTransformation]
  public class PolicySetDefinition : IResource
  {
    public PolicySetDefinition()
    {
    }

    public PolicySetDefinition(
      IList<PolicyDefinitionReference> policyDefinitions,
      string policyType = null,
      string displayName = null,
      string description = null,
      object metadata = null,
      IDictionary<string, ParameterDefinitionsValue> parameters = null,
      IList<PolicyDefinitionGroup> policyDefinitionGroups = null,
      string id = null,
      string name = null,
      string type = null)
    {
      this.PolicyType = policyType;
      this.DisplayName = displayName;
      this.Description = description;
      this.Metadata = metadata;
      this.Parameters = parameters;
      this.PolicyDefinitions = policyDefinitions;
      this.PolicyDefinitionGroups = policyDefinitionGroups;
      this.Id = id;
      this.Name = name;
      this.Type = type;
    }

    [JsonProperty(PropertyName = "properties.policyType")]
    public string PolicyType { get; set; }

    [JsonProperty(PropertyName = "properties.displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "properties.description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "properties.metadata")]
    public object Metadata { get; set; }

    [JsonProperty(PropertyName = "properties.parameters")]
    public IDictionary<string, ParameterDefinitionsValue> Parameters { get; set; }

    [JsonProperty(PropertyName = "properties.policyDefinitions")]
    public IList<PolicyDefinitionReference> PolicyDefinitions { get; set; }

    [JsonProperty(PropertyName = "properties.policyDefinitionGroups")]
    public IList<PolicyDefinitionGroup> PolicyDefinitionGroups { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }

    public virtual void Validate()
    {
      if (this.PolicyDefinitions == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "PolicyDefinitions");
      if (this.PolicyDefinitions != null)
      {
        foreach (PolicyDefinitionReference policyDefinition in (IEnumerable<PolicyDefinitionReference>) this.PolicyDefinitions)
          policyDefinition?.Validate();
      }
      if (this.PolicyDefinitionGroups == null)
        return;
      foreach (PolicyDefinitionGroup policyDefinitionGroup in (IEnumerable<PolicyDefinitionGroup>) this.PolicyDefinitionGroups)
        policyDefinitionGroup?.Validate();
    }
  }
}
