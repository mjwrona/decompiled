// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.PolicyDefinitionReference
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class PolicyDefinitionReference
  {
    public PolicyDefinitionReference()
    {
    }

    public PolicyDefinitionReference(
      string policyDefinitionId,
      IDictionary<string, ParameterValuesValue> parameters = null,
      string policyDefinitionReferenceId = null,
      IList<string> groupNames = null)
    {
      this.PolicyDefinitionId = policyDefinitionId;
      this.Parameters = parameters;
      this.PolicyDefinitionReferenceId = policyDefinitionReferenceId;
      this.GroupNames = groupNames;
    }

    [JsonProperty(PropertyName = "policyDefinitionId")]
    public string PolicyDefinitionId { get; set; }

    [JsonProperty(PropertyName = "parameters")]
    public IDictionary<string, ParameterValuesValue> Parameters { get; set; }

    [JsonProperty(PropertyName = "policyDefinitionReferenceId")]
    public string PolicyDefinitionReferenceId { get; set; }

    [JsonProperty(PropertyName = "groupNames")]
    public IList<string> GroupNames { get; set; }

    public virtual void Validate()
    {
      if (this.PolicyDefinitionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "PolicyDefinitionId");
    }
  }
}
