// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityReader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal interface IIdentityReader
  {
    Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> identityIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults = true);

    IdentitySnapshot ReadIdentitySnapshotFromDatabase(
      IVssRequestContext requestContext,
      Guid scopeIdGuid,
      HashSet<Guid> excludedIdentities,
      bool readAllIdentities = false,
      bool readInactiveMemberships = false);

    List<Microsoft.VisualStudio.Services.Identity.Identity> ListApplicationGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid[] scopeIds,
      bool recurse,
      bool deleted,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters);
  }
}
