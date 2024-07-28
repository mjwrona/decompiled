// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ResourceGroupFilter
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ResourceGroupFilter
  {
    public ResourceGroupFilter()
    {
    }

    public ResourceGroupFilter(string tagName = null, string tagValue = null)
    {
      this.TagName = tagName;
      this.TagValue = tagValue;
    }

    [JsonProperty(PropertyName = "tagName")]
    public string TagName { get; set; }

    [JsonProperty(PropertyName = "tagValue")]
    public string TagValue { get; set; }
  }
}
