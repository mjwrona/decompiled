// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.AzureResourceBase
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Azure;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class AzureResourceBase : IResource
  {
    public AzureResourceBase()
    {
    }

    public AzureResourceBase(string id = null, string name = null, string type = null)
    {
      this.Id = id;
      this.Name = name;
      this.Type = type;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }
  }
}
