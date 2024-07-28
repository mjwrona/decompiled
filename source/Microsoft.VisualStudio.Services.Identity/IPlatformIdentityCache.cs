// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IPlatformIdentityCache
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal interface IPlatformIdentityCache : IIdentityCache
  {
    IIdMapper GetIdMapper(IVssRequestContext requestContext, IdentityDomain hostDomain);

    IScopeMapper GetScopeMapper(IVssRequestContext requestContext, IdentityDomain hostDomain);

    void OnIdentityIdTranslationChanged(
      IVssRequestContext requestContext,
      IdentityIdTranslationChangeData identityIdTranslationChangeData);

    IdentityCache GetIdentityCacheByDomain(IdentityDomain hostDomain);

    bool TryReadScopeAncestorIds(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out HashSet<Guid> ancestorScopeIds);

    bool UpdateScopeAncestorIds(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      HashSet<Guid> ancestorScopeIds);

    IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors);

    IDictionary<IdentityDescriptor, IdentityMembershipInfo> ReadIdentityMembershipInfo(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      SequenceContext minSequenceContext);

    Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor);
  }
}
