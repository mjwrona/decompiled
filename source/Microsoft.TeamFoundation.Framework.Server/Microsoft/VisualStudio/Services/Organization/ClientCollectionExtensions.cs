// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ClientCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class ClientCollectionExtensions
  {
    public static CollectionCreationContext ToCreationContext(this Microsoft.VisualStudio.Services.Organization.Client.Collection x)
    {
      if (x == null)
        return (CollectionCreationContext) null;
      return new CollectionCreationContext(x.Name, x.Data)
      {
        OwnerId = x.Owner,
        PreferredGeography = x.PreferredGeography,
        PreferredRegion = x.PreferredRegion
      };
    }

    public static IReadOnlyList<CollectionRef> ToRef(this ICollection<Microsoft.VisualStudio.Services.Organization.Client.Collection> collections) => collections == null ? (IReadOnlyList<CollectionRef>) null : (IReadOnlyList<CollectionRef>) collections.Select<Microsoft.VisualStudio.Services.Organization.Client.Collection, CollectionRef>((Func<Microsoft.VisualStudio.Services.Organization.Client.Collection, CollectionRef>) (x => x.ToRef())).Where<CollectionRef>((Func<CollectionRef, bool>) (x => x != null)).ToList<CollectionRef>();

    public static CollectionRef ToRef(this Microsoft.VisualStudio.Services.Organization.Client.Collection x)
    {
      if (x == null)
        return (CollectionRef) null;
      return new CollectionRef(x.Id) { Name = x.Name };
    }

    public static IList<Collection> ToServer(this ICollection<Microsoft.VisualStudio.Services.Organization.Client.Collection> collections) => collections == null ? (IList<Collection>) null : (IList<Collection>) collections.Select<Microsoft.VisualStudio.Services.Organization.Client.Collection, Collection>((Func<Microsoft.VisualStudio.Services.Organization.Client.Collection, Collection>) (x => x.ToServer())).Where<Collection>((Func<Collection, bool>) (x => x != null)).ToList<Collection>();

    public static Collection ToServer(this Microsoft.VisualStudio.Services.Organization.Client.Collection x)
    {
      if (x == null)
        return (Collection) null;
      Collection server = new Collection(x.Id);
      server.Name = x.Name;
      server.Status = x.Status;
      server.Owner = x.Owner;
      server.TenantId = x.TenantId;
      server.DateCreated = x.DateCreated;
      server.LastUpdated = x.LastUpdated;
      server.Revision = x.Revision;
      server.PreferredRegion = x.PreferredRegion;
      server.PreferredGeography = x.PreferredGeography;
      server.Properties = x.Properties == null ? new PropertyBag() : new PropertyBag((IDictionary<string, object>) x.Properties);
      return server;
    }
  }
}
