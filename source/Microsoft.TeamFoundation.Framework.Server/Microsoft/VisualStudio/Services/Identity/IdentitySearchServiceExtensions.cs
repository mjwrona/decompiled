// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentitySearchServiceExtensions
  {
    public static Microsoft.VisualStudio.Services.Identity.Identity FindActiveUser(
      this IVssIdentitySearchService identitySearchService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      return IdentitySearchServiceExtensions.FindItem(collectionOrOrganizationContext, new Func<IVssRequestContext, IdentitySearchFilter, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identitySearchService.FindActiveUsers), searchFilter, filterValue);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindActiveGroup(
      this IVssIdentitySearchService identitySearchService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      return IdentitySearchServiceExtensions.FindItem(collectionOrOrganizationContext, new Func<IVssRequestContext, IdentitySearchFilter, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identitySearchService.FindActiveGroups), searchFilter, filterValue);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindActiveMember(
      this IVssIdentitySearchService identitySearchService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      string identityType = null)
    {
      return IdentitySearchServiceExtensions.FindItem(collectionOrOrganizationContext, new Func<IVssRequestContext, IdentitySearchFilter, string, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identitySearchService.FindActiveMembers), searchFilter, filterValue, identityType);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindActiveOrHistoricalMember(
      this IVssIdentitySearchService identitySearchService,
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      string identityType = null)
    {
      return IdentitySearchServiceExtensions.FindItem(collectionOrOrganizationContext, new Func<IVssRequestContext, IdentitySearchFilter, string, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identitySearchService.FindActiveOrHistoricalMembers), searchFilter, filterValue, identityType);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity FindItem(
      IVssRequestContext collectionOrOrganizationContext,
      Func<IVssRequestContext, IdentitySearchFilter, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>> searchFunc,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = searchFunc(collectionOrOrganizationContext, searchFilter, filterValue);
      if (!(source is IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList))
        identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> matchingIdentities = identityList;
      switch (matchingIdentities.Count)
      {
        case 0:
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        case 1:
          return matchingIdentities[0];
        default:
          throw new MultipleIdentitiesFoundException(filterValue, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) matchingIdentities);
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity FindItem(
      IVssRequestContext collectionOrOrganizationContext,
      Func<IVssRequestContext, IdentitySearchFilter, string, string, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>> searchFunc,
      IdentitySearchFilter searchFilter,
      string filterValue,
      string identityType = null)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = searchFunc(collectionOrOrganizationContext, searchFilter, filterValue, identityType);
      if (!(source is IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList))
        identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> matchingIdentities = identityList;
      switch (matchingIdentities.Count)
      {
        case 0:
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        case 1:
          return matchingIdentities[0];
        default:
          throw new MultipleIdentitiesFoundException(filterValue, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) matchingIdentities);
      }
    }
  }
}
