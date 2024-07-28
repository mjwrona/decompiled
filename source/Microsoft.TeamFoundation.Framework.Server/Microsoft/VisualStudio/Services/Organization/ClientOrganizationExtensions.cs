// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ClientOrganizationExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class ClientOrganizationExtensions
  {
    public static OrganizationCreationContext ToCreationContext(this Microsoft.VisualStudio.Services.Organization.Client.Organization x)
    {
      if (x == null)
        return (OrganizationCreationContext) null;
      return new OrganizationCreationContext(x.Name, string.IsNullOrEmpty(x.Name), x.Data)
      {
        CreatorId = x.CreatorId,
        PreferredRegion = x.PreferredRegion,
        PreferredGeography = x.PreferredGeography,
        PrimaryCollection = x.PrimaryCollection.ToCreationContext()
      };
    }

    public static OrganizationRef ToRef(this Microsoft.VisualStudio.Services.Organization.Client.Organization x)
    {
      if (x == null)
        return (OrganizationRef) null;
      return new OrganizationRef(x.Id) { Name = x.Name };
    }

    public static Microsoft.VisualStudio.Services.Organization.Organization ToServer(
      this Microsoft.VisualStudio.Services.Organization.Client.Organization x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Organization) null;
      Microsoft.VisualStudio.Services.Organization.Organization server = new Microsoft.VisualStudio.Services.Organization.Organization(x.Id);
      server.Name = x.Name;
      server.TenantId = x.TenantId;
      server.Type = x.Type;
      server.Status = x.Status;
      server.PreferredRegion = x.PreferredRegion;
      server.PreferredGeography = x.PreferredGeography;
      server.IsActivated = x.IsActivated;
      server.Collections = x.Collections.ToRef();
      server.DateCreated = x.DateCreated;
      server.LastUpdated = x.LastUpdated;
      server.Properties = x.Properties == null ? (PropertyBag) null : new PropertyBag((IDictionary<string, object>) x.Properties);
      return server;
    }
  }
}
