// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal interface IIdentityCache
  {
    void InvalidateSequenceContext(IVssRequestContext requestContext);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor descriptor,
      QueryMembership queryMembership);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SubjectDescriptor subjectDescriptor,
      QueryMembership queryMembership);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor,
      QueryMembership queryMembership);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid identityId,
      QueryMembership queryMembership);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership);

    void UpdateIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    bool? IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      out long cacheStamp,
      out IdentityMembershipInfo membershipInfo);

    void UpdateParentMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    bool CompareAndSwapParentMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      long cacheStamp);

    bool ProcessChanges(
      IVssRequestContext requestContext,
      ICollection<Guid> descriptorChangeIds,
      ICollection<Guid> identityChangeIds,
      ICollection<Guid> groupChangeIds,
      ICollection<MembershipChangeInfo> membershipChanges,
      ICollection<Guid> groupScopeChangeIds,
      SequenceContext sequenceContext);

    void Sweep(IVssRequestContext requestContext);

    void Clear(IVssRequestContext requestContext);

    void Clear(IVssRequestContext requestContext, IdentityDomain hostDomain);

    void Unload(IVssRequestContext requestContext);

    void AddDomain(IVssRequestContext requestContext, IdentityDomain hostDomain);

    Guid GetOrAddScopeParent(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      Guid parentScopeId);

    bool TryReadScopeParent(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out Guid parentScopeId);

    bool TryGetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out IdentityScope scope);

    bool AddScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IdentityScope scope);

    bool TryGetSequenceContext(out SequenceContext sequenceContext);

    SequenceContext CompareAndSwapSequenceContextIfGreater(SequenceContext sequenceContext);
  }
}
