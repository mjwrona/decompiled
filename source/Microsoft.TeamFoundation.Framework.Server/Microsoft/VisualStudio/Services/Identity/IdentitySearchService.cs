// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class IdentitySearchService : IVssIdentitySearchService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindActiveUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      return this.FindItems(collectionOrOrganizationContext, searchFilter, filterValue, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item != null && item.IsActive && !item.IsContainer));
    }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindActiveGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      return this.FindItems(collectionOrOrganizationContext, searchFilter, filterValue, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item != null && item.IsActive && item.IsContainer));
    }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindActiveMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      string identityType = null)
    {
      return this.FindItems(collectionOrOrganizationContext, searchFilter, filterValue, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item =>
      {
        if (item == null || !item.IsActive)
          return false;
        return string.IsNullOrWhiteSpace(identityType) || item.Descriptor.IdentityType == identityType;
      }));
    }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindActiveOrHistoricalMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      string identityType = null)
    {
      return this.FindItemsAtDeploymentThenScopeBackDownToProperLevel(collectionOrOrganizationContext, searchFilter, filterValue, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => item != null && string.IsNullOrWhiteSpace(identityType) || item.Descriptor.IdentityType == identityType));
    }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindIdentitiesInSource(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFilter,
      string filterValue)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filterValue, nameof (filterValue));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, searchFilter, filterValue, QueryMembership.None, (IEnumerable<string>) null);
      return source is IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> identityList ? identityList : (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindItems(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> postSearchFilters)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filterValue, nameof (filterValue));
      postSearchFilters = postSearchFilters ?? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => true);
      IdentityService service = collectionOrOrganizationContext.GetService<IdentityService>();
      try
      {
        return (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(collectionOrOrganizationContext, searchFilter, filterValue, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>(postSearchFilters).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        return (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
    }

    private IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> FindItemsAtDeploymentThenScopeBackDownToProperLevel(
      IVssRequestContext collectionOrOrganizationContext,
      IdentitySearchFilter searchFilter,
      string filterValue,
      Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> postSearchFilters)
    {
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filterValue, nameof (filterValue));
      postSearchFilters = postSearchFilters ?? (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (item => true);
      IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity> items = this.FindItems(collectionOrOrganizationContext, searchFilter, filterValue, postSearchFilters);
      if (!(items is List<Microsoft.VisualStudio.Services.Identity.Identity> identityList))
        identityList = items.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> downToProperLevel = identityList;
      if (downToProperLevel.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x.Descriptor.IsTeamFoundationType())) && (searchFilter == IdentitySearchFilter.AccountName || searchFilter == IdentitySearchFilter.DisplayName || searchFilter == IdentitySearchFilter.General))
        return (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) downToProperLevel;
      HashSet<IdentityDescriptor> alreadyReadSet = new HashSet<IdentityDescriptor>(downToProperLevel.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (item => item.Descriptor)), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      TeamFoundationHostType hostType = collectionOrOrganizationContext.IsDeploymentFallbackIdentityReadAllowed() ? TeamFoundationHostType.Deployment : TeamFoundationHostType.Application;
      IVssRequestContext vssRequestContext = collectionOrOrganizationContext.To(hostType).Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IList<IdentityDescriptor> descriptors;
      try
      {
        descriptors = (IList<IdentityDescriptor>) service.ReadIdentities(vssRequestContext, searchFilter, filterValue, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>(postSearchFilters).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)).Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (descriptor => !alreadyReadSet.Contains(descriptor))).ToList<IdentityDescriptor>();
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        descriptors = (IList<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      }
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> second = collectionOrOrganizationContext.GetService<IdentityService>().ReadIdentities(collectionOrOrganizationContext, descriptors, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null));
      return (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) downToProperLevel.Concat<Microsoft.VisualStudio.Services.Identity.Identity>(second).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }
  }
}
