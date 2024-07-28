// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IPlatformIdentityServiceInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Identity.PlatformIdentityServiceWrapper, Microsoft.VisualStudio.Services.Identity")]
  internal interface IPlatformIdentityServiceInternal : IVssFrameworkService
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity BuildFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null,
      string directoryAlias = null,
      string domain = null);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      Guid identityIdGuid,
      string mailAddress = null);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int CleanupDescriptorChangeQueue(IVssRequestContext requestContext, int beforeDays);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool DeleteHistoricalIdentities(IVssRequestContext requestContext, IList<Guid> identityIds);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool TryUpgradeHistoricalIdentityToClaimsIdentity(
      IVssRequestContext applicationContext,
      Guid identityId,
      out Microsoft.VisualStudio.Services.Identity.Identity claimsIdentity);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAggregateIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aggregateIdentityDescriptors);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> groupIdentityDescriptors);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<Guid> groupIds);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int UpdateIdentityVsid(IVssRequestContext requestContext, Guid oldVsid, Guid newVsid);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int SendMajorDescriptorChangeNotification(IVssRequestContext requestContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsAncestorMemberships(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors,
      SequenceContext minSequenceContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IdentityScope GetScopeByMasterId(IVssRequestContext requestContext, Guid masterScopeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IdentityScope GetScopeByLocalId(IVssRequestContext requestContext, Guid localScopeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      SequenceContext minSequenceContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool bypassCache);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool bypassCache);

    [EditorBrowsable(EditorBrowsableState.Never)]
    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SocialDescriptor socialDescriptor);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void InvalidateMembershipsCache(
      IVssRequestContext requestContext,
      ICollection<MembershipChangeInfo> membershipChangeInfos);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void CheckForLeakedMasterIds(IVssRequestContext requestContext, IEnumerable<Guid> ids);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void RepairAccountNameCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string reason);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Guid> GetIdentityIdsByDomainId(IVssRequestContext requestContext, Guid domainId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Guid> GetUserIdentityIdsByDomain(IVssRequestContext requestContext, Guid? domain);

    [EditorBrowsable(EditorBrowsableState.Never)]
    ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext,
      Guid scopeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void CheckUserWritePermission(IVssRequestContext requestContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IList<Guid> ReadIdentityIdsInScope(IVssRequestContext requestContext, Guid scopeId);

    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsFromDatabase(
      IVssRequestContext requestContext,
      bool readInactive);
  }
}
