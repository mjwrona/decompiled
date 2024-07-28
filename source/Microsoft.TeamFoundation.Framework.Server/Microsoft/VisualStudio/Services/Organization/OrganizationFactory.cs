// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationFactory
  {
    public static Microsoft.VisualStudio.Services.Organization.Organization Create(
      Guid id,
      string name,
      OrganizationType type,
      OrganizationStatus status,
      string preferredRegion,
      string preferredGeography,
      bool IsActivated,
      DateTime created,
      DateTime updated)
    {
      Microsoft.VisualStudio.Services.Organization.Organization organization = new Microsoft.VisualStudio.Services.Organization.Organization(id);
      organization.Name = name;
      organization.Type = type;
      organization.Status = status;
      organization.PreferredRegion = preferredRegion;
      organization.PreferredGeography = preferredGeography;
      organization.IsActivated = IsActivated;
      organization.DateCreated = created;
      organization.LastUpdated = updated;
      return organization;
    }

    public static Microsoft.VisualStudio.Services.Organization.Organization Create(
      Microsoft.VisualStudio.Services.Organization.Organization organization,
      Guid tenantId,
      ICollection<CollectionRef> collections,
      PropertyBag propertiesToSet)
    {
      ICollection<CollectionRef> source = organization.Collections == null ? (ICollection<CollectionRef>) null : (ICollection<CollectionRef>) new List<CollectionRef>((IEnumerable<CollectionRef>) organization.Collections);
      if (source == null)
        source = collections;
      else if (collections != null)
      {
        foreach (CollectionRef collection in (IEnumerable<CollectionRef>) collections)
          source.Add(collection);
      }
      Microsoft.VisualStudio.Services.Organization.Organization organization1 = new Microsoft.VisualStudio.Services.Organization.Organization(organization.Id);
      organization1.Name = organization.Name;
      organization1.Type = organization.Type;
      organization1.Status = organization.Status;
      organization1.PreferredRegion = organization.PreferredRegion;
      organization1.PreferredGeography = organization.PreferredGeography;
      organization1.IsActivated = organization.IsActivated;
      organization1.TenantId = tenantId;
      organization1.Collections = source != null ? (IReadOnlyList<CollectionRef>) source.Where<CollectionRef>((Func<CollectionRef, bool>) (y => y != null)).ToList<CollectionRef>() : (IReadOnlyList<CollectionRef>) null;
      organization1.DateCreated = organization.DateCreated;
      organization1.LastUpdated = organization.LastUpdated;
      organization1.Properties = propertiesToSet ?? new PropertyBag();
      return organization1;
    }
  }
}
