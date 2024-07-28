// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityServiceWrapper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PlatformIdentityServiceWrapper : 
    UserIdentityServiceWrapper<PlatformIdentityService>,
    IPlatformIdentityServiceInternal,
    IVssFrameworkService
  {
    private IPlatformIdentityServiceInternal m_platformIdentityService;

    public Microsoft.VisualStudio.Services.Identity.Identity BuildFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null,
      string directoryAlias = null,
      string domain = null)
    {
      return this.GetPlatformIdentityService().BuildFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress, directoryAlias, domain);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      Guid identityIdGuid,
      string mailAddress = null)
    {
      return this.GetPlatformIdentityService().CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, identityIdGuid, mailAddress);
    }

    public int CleanupDescriptorChangeQueue(IVssRequestContext requestContext, int beforeDays) => this.GetPlatformIdentityService().CleanupDescriptorChangeQueue(requestContext, beforeDays);

    public int UpdateIdentityVsid(IVssRequestContext requestContext, Guid oldVsid, Guid newVsid) => this.GetPlatformIdentityService().UpdateIdentityVsid(requestContext, oldVsid, newVsid);

    public bool DeleteHistoricalIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      return this.GetPlatformIdentityService().DeleteHistoricalIdentities(requestContext, identityIds);
    }

    public IdentityScope GetScopeByMasterId(IVssRequestContext requestContext, Guid masterScopeId) => this.GetPlatformIdentityService().GetScopeByMasterId(requestContext, masterScopeId);

    public IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors)
    {
      return this.GetPlatformIdentityService().HasAadGroups(requestContext, aadGroupDescriptors);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsAncestorMemberships(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors,
      SequenceContext minSequenceContext)
    {
      return this.GetPlatformIdentityService().ReadAadGroupsAncestorMemberships(requestContext, aadGroupDescriptors, minSequenceContext);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAggregateIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aggregateIdentityDescriptors)
    {
      return this.GetPlatformIdentityService().ReadAggregateIdentitiesFromDatabase(requestContext, aggregateIdentityDescriptors);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> groupIdentityDescriptors)
    {
      return this.GetPlatformIdentityService().ReadGroupsFromDatabase(requestContext, groupIdentityDescriptors);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<Guid> groupIds)
    {
      return this.GetPlatformIdentityService().ReadGroupsFromDatabase(requestContext, groupIds);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      SequenceContext minSequenceContext)
    {
      return this.GetPlatformIdentityService().ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, minSequenceContext);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.GetPlatformIdentityService().ReadIdentitiesByScope(requestContext, scopeId, queryMembership, propertyNameFilters);
    }

    public int SendMajorDescriptorChangeNotification(IVssRequestContext requestContext) => this.GetPlatformIdentityService().SendMajorDescriptorChangeNotification(requestContext);

    public bool TryUpgradeHistoricalIdentityToClaimsIdentity(
      IVssRequestContext applicationContext,
      Guid identityId,
      out Microsoft.VisualStudio.Services.Identity.Identity claimsIdentity)
    {
      return this.GetPlatformIdentityService().TryUpgradeHistoricalIdentityToClaimsIdentity(applicationContext, identityId, out claimsIdentity);
    }

    public void RepairAccountNameCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string reason)
    {
      this.GetPlatformIdentityService().RepairAccountNameCollision(requestContext, identity, reason);
    }

    public IList<Guid> GetIdentityIdsByDomainId(IVssRequestContext requestContext, Guid domainId) => this.GetPlatformIdentityService().GetIdentityIdsByDomainId(requestContext, domainId);

    public IList<Guid> GetUserIdentityIdsByDomain(IVssRequestContext requestContext, Guid? domain) => this.GetPlatformIdentityService().GetUserIdentityIdsByDomain(requestContext, domain);

    public ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext,
      Guid scopeId)
    {
      return this.GetPlatformIdentityService().GetIdentityChanges(requestContext, sequenceContext, scopeId);
    }

    public IList<Guid> ReadIdentityIdsInScope(IVssRequestContext requestContext, Guid scopeId) => this.GetPlatformIdentityService().ReadIdentityIdsInScope(requestContext, scopeId);

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsFromDatabase(
      IVssRequestContext requestContext,
      bool readInactive)
    {
      return this.GetPlatformIdentityService().ReadAadGroupsFromDatabase(requestContext, readInactive);
    }

    public void CheckForLeakedMasterIds(IVssRequestContext requestContext, IEnumerable<Guid> ids) => this.GetPlatformIdentityService(false)?.CheckForLeakedMasterIds(requestContext, ids);

    public IdentityScope GetScopeByLocalId(IVssRequestContext requestContext, Guid localScopeId) => this.GetPlatformIdentityService(false)?.GetScopeByLocalId(requestContext, localScopeId);

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      IPlatformIdentityServiceInternal platformIdentityService = this.GetPlatformIdentityService(false);
      return platformIdentityService == null ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>() : platformIdentityService.ReadIdentitiesByDomainAndOid(requestContext, domain, externalId);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      IPlatformIdentityServiceInternal platformIdentityService = this.GetPlatformIdentityService(false);
      if (platformIdentityService != null)
        return platformIdentityService.ReadIdentityByDomainAndOid(requestContext, domain, externalId);
      if (string.IsNullOrEmpty(domain) || domain.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      SubjectDescriptor subjectDescriptor = AadIdentityHelper.GetAadUserSubjectDescriptor(new Guid(domain), externalId);
      return this.IdentityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        subjectDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SocialDescriptor socialDescriptor)
    {
      return this.GetPlatformIdentityService(false)?.ReadIdentity(requestContext, socialDescriptor);
    }

    public void CheckUserWritePermission(IVssRequestContext requestContext)
    {
      IPlatformIdentityServiceInternal platformIdentityService = this.GetPlatformIdentityService(false);
      if (platformIdentityService != null)
        platformIdentityService.CheckUserWritePermission(requestContext);
      else
        requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 4);
    }

    protected override IdentityService Initialize(IVssRequestContext requestContext)
    {
      IdentityService dentityService = base.Initialize(requestContext);
      this.m_platformIdentityService = dentityService as IPlatformIdentityServiceInternal;
      return dentityService;
    }

    private IPlatformIdentityServiceInternal GetPlatformIdentityService(bool throwIfNull = true)
    {
      IPlatformIdentityServiceInternal platformIdentityService = this.m_platformIdentityService;
      return !(platformIdentityService == null & throwIfNull) ? platformIdentityService : throw new NotSupportedException("This operation is not supported by the current identity service implementation.");
    }

    public void InvalidateMembershipsCache(
      IVssRequestContext requestContext,
      ICollection<MembershipChangeInfo> membershipChangeInfos)
    {
      requestContext.GetService<PlatformIdentityService>().InvalidateMembershipsCacheHelper(requestContext, membershipChangeInfos);
    }
  }
}
