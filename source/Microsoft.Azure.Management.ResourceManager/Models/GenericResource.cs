// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.GenericResource
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class GenericResource : Resource
  {
    public GenericResource()
    {
    }

    public GenericResource(
      string id = null,
      string name = null,
      string type = null,
      string location = null,
      IDictionary<string, string> tags = null,
      Plan plan = null,
      object properties = null,
      string kind = null,
      string managedBy = null,
      Sku sku = null,
      Identity identity = null)
      : base(id, name, type, location, tags)
    {
      this.Plan = plan;
      this.Properties = properties;
      this.Kind = kind;
      this.ManagedBy = managedBy;
      this.Sku = sku;
      this.Identity = identity;
    }

    [JsonProperty(PropertyName = "plan")]
    public Plan Plan { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public object Properties { get; set; }

    [JsonProperty(PropertyName = "kind")]
    public string Kind { get; set; }

    [JsonProperty(PropertyName = "managedBy")]
    public string ManagedBy { get; set; }

    [JsonProperty(PropertyName = "sku")]
    public Sku Sku { get; set; }

    [JsonProperty(PropertyName = "identity")]
    public Identity Identity { get; set; }

    public virtual void Validate()
    {
      if (this.Kind != null && !Regex.IsMatch(this.Kind, "^[-\\w\\._,\\(\\)]+$"))
        throw new ValidationException(ValidationRules.Pattern, "Kind", (object) "^[-\\w\\._,\\(\\)]+$");
    }
  }
}
