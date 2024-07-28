// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityPropertiesCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal interface IIdentityPropertiesCache
  {
    void FilterUnchangedIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identityToFilter);

    bool EnrichIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identityToEnrich);

    void UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    bool UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IDictionary<Guid, Dictionary<string, object>> changedIdentityProperties);

    void Dispose(IVssRequestContext requestContext);

    void OnCachedIdentityEvicted(object sender, IdentityRemovedEventArgs args);

    IEnumerable<string> GetPrefetchedProperties();
  }
}
