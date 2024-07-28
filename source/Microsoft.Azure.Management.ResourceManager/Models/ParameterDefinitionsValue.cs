// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ParameterDefinitionsValue
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ParameterDefinitionsValue
  {
    public ParameterDefinitionsValue()
    {
    }

    public ParameterDefinitionsValue(
      string type = null,
      IList<object> allowedValues = null,
      object defaultValue = null,
      ParameterDefinitionsValueMetadata metadata = null)
    {
      this.Type = type;
      this.AllowedValues = allowedValues;
      this.DefaultValue = defaultValue;
      this.Metadata = metadata;
    }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "allowedValues")]
    public IList<object> AllowedValues { get; set; }

    [JsonProperty(PropertyName = "defaultValue")]
    public object DefaultValue { get; set; }

    [JsonProperty(PropertyName = "metadata")]
    public ParameterDefinitionsValueMetadata Metadata { get; set; }
  }
}
