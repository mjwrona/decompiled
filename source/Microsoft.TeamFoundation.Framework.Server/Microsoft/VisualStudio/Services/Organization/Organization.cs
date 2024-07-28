// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Organization
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public sealed class Organization : OrganizationRef, IPropertyContainer, ICloneable
  {
    public Organization(Guid id)
      : base(id)
    {
      this.Properties = new PropertyBag();
    }

    [JsonProperty]
    public OrganizationType Type { get; internal set; }

    [JsonProperty]
    public OrganizationStatus Status { get; internal set; }

    [JsonProperty]
    public string PreferredRegion { get; internal set; }

    [JsonProperty]
    public string PreferredGeography { get; internal set; }

    [JsonProperty]
    public IReadOnlyList<CollectionRef> Collections { get; internal set; }

    [JsonProperty]
    public DateTime DateCreated { get; internal set; }

    [JsonProperty]
    public DateTime LastUpdated { get; internal set; }

    [JsonProperty]
    public Guid TenantId { get; internal set; }

    [JsonProperty]
    public bool IsActivated { get; internal set; }

    [JsonProperty]
    public PropertyBag Properties { get; internal set; }

    public Microsoft.VisualStudio.Services.Organization.Organization Clone()
    {
      Microsoft.VisualStudio.Services.Organization.Organization organization = new Microsoft.VisualStudio.Services.Organization.Organization(this.Id);
      organization.Name = this.Name;
      organization.Type = this.Type;
      organization.Status = this.Status;
      organization.DateCreated = this.DateCreated;
      organization.LastUpdated = this.LastUpdated;
      organization.TenantId = this.TenantId;
      organization.PreferredRegion = this.PreferredRegion;
      organization.PreferredGeography = this.PreferredGeography;
      organization.IsActivated = this.IsActivated;
      organization.Properties = this.Properties?.Clone();
      organization.Collections = this.Collections;
      return organization;
    }

    object ICloneable.Clone() => (object) this.Clone();

    private bool Equals(Microsoft.VisualStudio.Services.Organization.Organization obj)
    {
      if (this == obj)
        return true;
      return (!object.Equals((object) this.Id, (object) obj.Id) || !VssStringComparer.Hostname.Equals(this.Name, obj.Name) || !object.Equals((object) this.TenantId, (object) obj.TenantId) || !object.Equals((object) this.Type, (object) obj.Type) || !object.Equals((object) this.Status, (object) obj.Status) || !object.Equals((object) this.PreferredRegion, (object) obj.PreferredRegion) || !object.Equals((object) this.IsActivated, (object) obj.IsActivated) || !object.Equals((object) this.DateCreated, (object) obj.DateCreated) || !object.Equals((object) this.LastUpdated, (object) obj.LastUpdated) || !object.Equals((object) this.Properties, (object) obj.Properties) ? 0 : (object.Equals((object) this.PreferredGeography, (object) obj.PreferredGeography) ? 1 : 0)) != 0 && Microsoft.VisualStudio.Services.Organization.Organization.AreCollectionsEquals(this.Collections, obj.Collections);
    }

    private static bool AreCollectionsEquals(
      IReadOnlyList<CollectionRef> left,
      IReadOnlyList<CollectionRef> right)
    {
      if (left == right)
        return true;
      if (left == null || right == null || left.Count != right.Count)
        return false;
      List<CollectionRef> list1 = left.ToList<CollectionRef>();
      List<CollectionRef> list2 = right.ToList<CollectionRef>();
      for (int index = 0; index < left.Count; ++index)
      {
        if (!object.Equals((object) list1[index], (object) list2[index]))
          return false;
      }
      return true;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((Microsoft.VisualStudio.Services.Organization.Organization) obj);
    }

    public override int GetHashCode()
    {
      int num1 = ((((((1231 * 3037 + base.GetHashCode()) * 3037 + this.TenantId.GetHashCode()) * 3037 + this.Type.GetHashCode()) * 3037 + this.Status.GetHashCode()) * 3037 + this.DateCreated.GetHashCode()) * 3037 + this.LastUpdated.GetHashCode()) * 3037;
      string preferredRegion = this.PreferredRegion;
      int hashCode1 = preferredRegion != null ? preferredRegion.GetHashCode() : 0;
      int num2 = ((num1 + hashCode1) * 3037 + this.IsActivated.GetHashCode()) * 3037;
      PropertyBag properties = this.Properties;
      int hashCode2 = properties != null ? properties.GetHashCode() : 0;
      int num3 = (num2 + hashCode2) * 3037;
      string preferredGeography = this.PreferredGeography;
      int hashCode3 = preferredGeography != null ? preferredGeography.GetHashCode() : 0;
      int hashCode4 = num3 + hashCode3;
      if (this.Collections != null)
      {
        foreach (CollectionRef collection in (IEnumerable<CollectionRef>) this.Collections)
          hashCode4 = hashCode4 * 3037 + (collection != null ? collection.GetHashCode() : 0);
      }
      return hashCode4;
    }
  }
}
