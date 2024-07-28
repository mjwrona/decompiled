// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class CollectionFactory
  {
    public static Collection Create(
      Guid id,
      string name,
      CollectionStatus status,
      Guid owner,
      Guid tenantId,
      DateTime created,
      DateTime updated,
      int revision,
      string preferredRegion,
      string preferredGeography)
    {
      Collection collection = new Collection(id);
      collection.Name = name;
      collection.Status = status;
      collection.Owner = owner;
      collection.TenantId = tenantId;
      collection.DateCreated = created;
      collection.LastUpdated = updated;
      collection.Revision = revision;
      collection.PreferredRegion = preferredRegion;
      collection.PreferredGeography = preferredGeography;
      return collection;
    }

    public static Collection Create(Collection collection, PropertyBag propertiesToSet)
    {
      Collection collection1 = new Collection(collection.Id);
      collection1.Name = collection.Name;
      collection1.Status = collection.Status;
      collection1.Owner = collection.Owner;
      collection1.TenantId = collection.TenantId;
      collection1.DateCreated = collection.DateCreated;
      collection1.LastUpdated = collection.LastUpdated;
      collection1.Revision = collection.Revision;
      collection1.PreferredRegion = collection.PreferredRegion;
      collection1.PreferredGeography = collection.PreferredGeography;
      collection1.Properties = propertiesToSet;
      return collection1;
    }

    public static CollectionRef CreateRef(Guid id, string name) => new CollectionRef(id)
    {
      Name = name
    };
  }
}
