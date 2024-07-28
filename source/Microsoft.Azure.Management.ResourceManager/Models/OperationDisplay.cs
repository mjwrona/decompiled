// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.OperationDisplay
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class OperationDisplay
  {
    public OperationDisplay()
    {
    }

    public OperationDisplay(
      string provider = null,
      string resource = null,
      string operation = null,
      string description = null)
    {
      this.Provider = provider;
      this.Resource = resource;
      this.Operation = operation;
      this.Description = description;
    }

    [JsonProperty(PropertyName = "provider")]
    public string Provider { get; set; }

    [JsonProperty(PropertyName = "resource")]
    public string Resource { get; set; }

    [JsonProperty(PropertyName = "operation")]
    public string Operation { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }
  }
}
