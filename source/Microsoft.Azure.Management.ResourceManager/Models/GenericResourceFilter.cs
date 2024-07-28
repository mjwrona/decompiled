// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.GenericResourceFilter
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class GenericResourceFilter
  {
    public GenericResourceFilter()
    {
    }

    public GenericResourceFilter(string resourceType = null, string tagname = null, string tagvalue = null)
    {
      this.ResourceType = resourceType;
      this.Tagname = tagname;
      this.Tagvalue = tagvalue;
    }

    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }

    [JsonProperty(PropertyName = "tagname")]
    public string Tagname { get; set; }

    [JsonProperty(PropertyName = "tagvalue")]
    public string Tagvalue { get; set; }
  }
}
