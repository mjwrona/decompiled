// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Collection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public sealed class Collection : CollectionRef, IPropertyContainer, ICloneable
  {
    public Collection(Guid id)
      : base(id)
    {
      this.Properties = new PropertyBag();
    }

    [JsonProperty]
    public CollectionStatus Status { get; internal set; }

    [JsonProperty]
    public Guid Owner { get; internal set; }

    [JsonProperty]
    public Guid TenantId { get; internal set; }

    [JsonProperty]
    public DateTime DateCreated { get; internal set; }

    [JsonProperty]
    public DateTime LastUpdated { get; internal set; }

    [JsonProperty]
    public int Revision { get; internal set; }

    [JsonProperty]
    public string PreferredRegion { get; internal set; }

    [JsonProperty]
    public string PreferredGeography { get; internal set; }

    [JsonProperty]
    public PropertyBag Properties { get; internal set; }

    public Collection Clone()
    {
      Collection collection = new Collection(this.Id);
      collection.Name = this.Name;
      collection.Status = this.Status;
      collection.Owner = this.Owner;
      collection.TenantId = this.TenantId;
      collection.DateCreated = this.DateCreated;
      collection.LastUpdated = this.LastUpdated;
      collection.Revision = this.Revision;
      collection.PreferredRegion = this.PreferredRegion;
      collection.PreferredGeography = this.PreferredGeography;
      collection.Properties = this.Properties?.Clone();
      return collection;
    }

    object ICloneable.Clone() => (object) this.Clone();

    private bool Equals(Collection obj)
    {
      if (this == obj)
        return true;
      return object.Equals((object) this.Id, (object) obj.Id) && VssStringComparer.Hostname.Equals(this.Name, obj.Name) && object.Equals((object) this.Owner, (object) obj.Owner) && object.Equals((object) this.TenantId, (object) obj.TenantId) && object.Equals((object) this.Status, (object) obj.Status) && object.Equals((object) this.DateCreated, (object) obj.DateCreated) && object.Equals((object) this.LastUpdated, (object) obj.LastUpdated) && object.Equals((object) this.Revision, (object) obj.Revision) && object.Equals((object) this.PreferredRegion, (object) obj.PreferredRegion) && object.Equals((object) this.PreferredGeography, (object) obj.PreferredGeography) && object.Equals((object) this.Properties, (object) obj.Properties);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((Collection) obj);
    }

    public override int GetHashCode()
    {
      int num1 = (((((((1231 * 3037 + base.GetHashCode()) * 3037 + this.Owner.GetHashCode()) * 3037 + this.TenantId.GetHashCode()) * 3037 + this.Status.GetHashCode()) * 3037 + this.DateCreated.GetHashCode()) * 3037 + this.LastUpdated.GetHashCode()) * 3037 + this.Revision.GetHashCode()) * 3037;
      string preferredRegion = this.PreferredRegion;
      int hashCode1 = preferredRegion != null ? preferredRegion.GetHashCode() : 0;
      int num2 = (num1 + hashCode1) * 3037;
      string preferredGeography = this.PreferredGeography;
      int hashCode2 = preferredGeography != null ? preferredGeography.GetHashCode() : 0;
      int num3 = (num2 + hashCode2) * 3037;
      PropertyBag properties = this.Properties;
      int hashCode3 = properties != null ? properties.GetHashCode() : 0;
      return num3 + hashCode3;
    }
  }
}
