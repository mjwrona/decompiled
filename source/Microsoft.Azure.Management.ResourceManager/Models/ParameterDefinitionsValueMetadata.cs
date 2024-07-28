// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ParameterDefinitionsValueMetadata
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ParameterDefinitionsValueMetadata
  {
    public ParameterDefinitionsValueMetadata()
    {
    }

    public ParameterDefinitionsValueMetadata(
      IDictionary<string, object> additionalProperties = null,
      string displayName = null,
      string description = null)
    {
      this.AdditionalProperties = additionalProperties;
      this.DisplayName = displayName;
      this.Description = description;
    }

    [JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties { get; set; }

    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }
  }
}
