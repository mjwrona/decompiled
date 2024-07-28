// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.IOrganizationCatalogService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DefaultServiceImplementation(typeof (FrameworkOrganizationCatalogService))]
  public interface IOrganizationCatalogService : IVssFrameworkService
  {
    OrganizationRef CreateOrganization(
      IVssRequestContext context,
      OrganizationCreationContext creationContext);

    IEnumerable<OrganizationRef> GetOrganizations(
      IVssRequestContext context,
      OrganizationQueryContext queryContext);

    Microsoft.VisualStudio.Services.Organization.Organization GetOldestOrganizationByTenant(
      IVssRequestContext context,
      Guid tenantId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<Region> GetRegions(
      IVssRequestContext context,
      bool includeRegionsWithNoAvailableHosts = false);

    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<Geography> GetGeographies(IVssRequestContext context);

    IEnumerable<CollectionRef> GetCollections(
      IVssRequestContext context,
      CollectionQueryContext queryContext);
  }
}
