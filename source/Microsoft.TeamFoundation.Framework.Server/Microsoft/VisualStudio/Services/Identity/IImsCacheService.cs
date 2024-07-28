// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IImsCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.Cache;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DefaultServiceImplementation(typeof (ImsCacheService))]
  internal interface IImsCacheService : IVssFrameworkService
  {
    void Initialize(
      IVssRequestContext context,
      ImsOperation operationsToCacheLocally,
      ImsOperation operationsToCacheRemotely);

    void AddSupportForOperations(
      IVssRequestContext context,
      ImsOperation operationsToCacheLocally,
      ImsOperation operationsToCacheRemotely);

    Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<IdentityDescriptor> descriptorIds);

    Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<Guid> ids);

    IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayName);

    IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountName);

    void SetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> values);

    void SetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> values);

    void SetIdentitiesByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayName,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values);

    void SetIdentitiesByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountName,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values);

    Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> GetIdentitiesInScope(
      IVssRequestContext context,
      IEnumerable<Guid> scopeIds);

    void SetIdentitiesInScope(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>> values);

    void CreateSearchIndexByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap);

    void CreateSearchIndexByAppId(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap);

    void CreateSearchIndexByEmail(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap);

    void CreateSearchIndexByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap);

    void CreateSearchIndexByDomainAccountName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap);

    IEnumerable<IdentityId> SearchIdentityIdsByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayNamePrefix,
      out bool isStale);

    IEnumerable<IdentityId> SearchIdentityIdsByAppId(
      IVssRequestContext context,
      Guid scopeId,
      string displayNamePrefix,
      out bool isStale,
      out bool inputParameterError);

    IEnumerable<IdentityId> SearchIdentityIdsByEmail(
      IVssRequestContext context,
      Guid scopeId,
      string emailPrefix,
      out bool isStale);

    IEnumerable<IdentityId> SearchIdentityIdsByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountNamePrefix,
      out bool isStale);

    IEnumerable<IdentityId> SearchIdentityIdsByDomainAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string domainAccountNamePrefix,
      out bool isStale);

    IList<Guid> GetMruIdentityIds(IVssRequestContext context, Guid identityId, Guid containerId);

    void SetMruIdentityIds(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      List<Guid> values);

    void RemoveMruIdentityIds(IVssRequestContext context, Guid identityId, Guid containerId);

    Dictionary<Guid, ISet<IdentityId>> GetDescendants(
      IVssRequestContext context,
      IEnumerable<Guid> groupIds);

    bool ProcessChanges(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<MembershipChangeInfo> membershipChanges);

    bool ProcessChanges(IVssRequestContext context, Guid scopeId, IList<Guid> identityChanges);

    void ProcessChangesOnSearchCaches(
      IVssRequestContext context,
      Guid scopeId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> newlyAddedIdentities);
  }
}
