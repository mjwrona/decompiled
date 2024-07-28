// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IVssIdentityRetrievalService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DefaultServiceImplementation(typeof (IdentityRetrievalService))]
  public interface IVssIdentityRetrievalService : IVssFrameworkService
  {
    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetMaterializedUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors);

    Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActor(
      IVssRequestContext requestContext,
      IdentityDescriptor actorDescriptor);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> groupDescriptors);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> memberDescriptors);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> userDescriptors);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> groupDescriptors);

    IReadOnlyDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<IdentityDescriptor> memberDescriptors);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> userIds);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> groupIds);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> memberIds);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalUsers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> userIds);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalGroups(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> groupIds);

    IReadOnlyDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetActiveOrHistoricalMembers(
      IVssRequestContext collectionOrOrganizationContext,
      IEnumerable<Guid> memberIds);
  }
}
