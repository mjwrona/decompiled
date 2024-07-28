// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.TagDetails
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class TagDetails
  {
    public TagDetails()
    {
    }

    public TagDetails(string id = null, string tagName = null, TagCount count = null, IList<TagValue> values = null)
    {
      this.Id = id;
      this.TagName = tagName;
      this.Count = count;
      this.Values = values;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "tagName")]
    public string TagName { get; set; }

    [JsonProperty(PropertyName = "count")]
    public TagCount Count { get; set; }

    [JsonProperty(PropertyName = "values")]
    public IList<TagValue> Values { get; set; }
  }
}
