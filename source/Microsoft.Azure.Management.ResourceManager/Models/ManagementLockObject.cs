// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ManagementLockObject
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
  public class ManagementLockObject : IResource
  {
    public ManagementLockObject()
    {
    }

    public ManagementLockObject(
      string level,
      string notes = null,
      IList<ManagementLockOwner> owners = null,
      string id = null,
      string type = null,
      string name = null)
    {
      this.Level = level;
      this.Notes = notes;
      this.Owners = owners;
      this.Id = id;
      this.Type = type;
      this.Name = name;
    }

    [JsonProperty(PropertyName = "properties.level")]
    public string Level { get; set; }

    [JsonProperty(PropertyName = "properties.notes")]
    public string Notes { get; set; }

    [JsonProperty(PropertyName = "properties.owners")]
    public IList<ManagementLockOwner> Owners { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    public virtual void Validate()
    {
      if (this.Level == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Level");
    }
  }
}
