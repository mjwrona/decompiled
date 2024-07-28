// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationExtensions
  {
    public static Microsoft.VisualStudio.Services.Organization.Client.Organization ToClient(
      this Microsoft.VisualStudio.Services.Organization.Organization x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.Organization) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.Organization()
      {
        Id = x.Id,
        Name = x.Name,
        TenantId = x.TenantId,
        Type = x.Type,
        Status = x.Status,
        PreferredRegion = x.PreferredRegion,
        PreferredGeography = x.PreferredGeography,
        IsActivated = x.IsActivated,
        Collections = (ICollection<Microsoft.VisualStudio.Services.Organization.Client.Collection>) x.Collections.ToClient(),
        DateCreated = x.DateCreated,
        LastUpdated = x.LastUpdated,
        Properties = x.Properties.ToPropertiesCollection()
      };
    }

    public static IList<OrganizationRef> ToRef(this ICollection<Microsoft.VisualStudio.Services.Organization.Organization> organizations) => organizations == null ? (IList<OrganizationRef>) null : (IList<OrganizationRef>) organizations.Select<Microsoft.VisualStudio.Services.Organization.Organization, OrganizationRef>((Func<Microsoft.VisualStudio.Services.Organization.Organization, OrganizationRef>) (x => x.ToRef())).Where<OrganizationRef>((Func<OrganizationRef, bool>) (x => x != null)).ToList<OrganizationRef>();

    public static IList<Microsoft.VisualStudio.Services.Organization.Organization> Filter(
      this ICollection<Microsoft.VisualStudio.Services.Organization.Organization> organizations,
      bool? isActivatedFilterQuery)
    {
      return organizations == null ? (IList<Microsoft.VisualStudio.Services.Organization.Organization>) null : (IList<Microsoft.VisualStudio.Services.Organization.Organization>) (isActivatedFilterQuery.HasValue ? organizations.Select<Microsoft.VisualStudio.Services.Organization.Organization, Microsoft.VisualStudio.Services.Organization.Organization>((Func<Microsoft.VisualStudio.Services.Organization.Organization, Microsoft.VisualStudio.Services.Organization.Organization>) (x => x.Filter(new bool?(isActivatedFilterQuery.Value)))) : (IEnumerable<Microsoft.VisualStudio.Services.Organization.Organization>) organizations).Where<Microsoft.VisualStudio.Services.Organization.Organization>((Func<Microsoft.VisualStudio.Services.Organization.Organization, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Organization.Organization>();
    }

    public static OrganizationRef ToRef(this Microsoft.VisualStudio.Services.Organization.Organization organization)
    {
      if (organization == null)
        return (OrganizationRef) null;
      return new OrganizationRef(organization.Id)
      {
        Name = organization.Name
      };
    }

    public static Microsoft.VisualStudio.Services.Organization.Organization Filter(
      this Microsoft.VisualStudio.Services.Organization.Organization organization,
      bool? isActivatedFilterQuery)
    {
      if (organization == null)
        return (Microsoft.VisualStudio.Services.Organization.Organization) null;
      if (!isActivatedFilterQuery.HasValue)
        return organization;
      int num1 = organization.IsActivated ? 1 : 0;
      bool? nullable = isActivatedFilterQuery;
      int num2 = nullable.GetValueOrDefault() ? 1 : 0;
      return !(num1 == num2 & nullable.HasValue) ? (Microsoft.VisualStudio.Services.Organization.Organization) null : organization;
    }
  }
}
