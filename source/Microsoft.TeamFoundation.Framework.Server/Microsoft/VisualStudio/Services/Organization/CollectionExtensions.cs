// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class CollectionExtensions
  {
    public static Collection FilterIfDeleted(this Collection x) => x == null || x.Status == CollectionStatus.LogicallyDeleted || x.Status == CollectionStatus.MarkedForPhysicalDelete ? (Collection) null : x;

    public static IList<CollectionRef> ToRef(this ICollection<Collection> collections) => collections == null ? (IList<CollectionRef>) null : (IList<CollectionRef>) collections.Select<Collection, CollectionRef>((Func<Collection, CollectionRef>) (x => x.ToRef())).Where<CollectionRef>((Func<CollectionRef, bool>) (x => x != null)).ToList<CollectionRef>();

    public static CollectionRef ToRef(this Collection x)
    {
      if (x == null)
        return (CollectionRef) null;
      return new CollectionRef(x.Id) { Name = x.Name };
    }

    public static Microsoft.VisualStudio.Services.Organization.Client.Collection ToClient(
      this Collection x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.Collection) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.Collection()
      {
        Name = x.Name,
        Id = x.Id,
        Status = x.Status,
        Owner = x.Owner,
        TenantId = x.TenantId,
        DateCreated = x.DateCreated,
        LastUpdated = x.LastUpdated,
        Revision = x.Revision,
        PreferredRegion = x.PreferredRegion,
        PreferredGeography = x.PreferredGeography,
        Properties = x.Properties.ToPropertiesCollection()
      };
    }
  }
}
