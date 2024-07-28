// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ITeamFoundationCatalogService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  [DefaultServiceImplementation(typeof (TeamFoundationCatalogService))]
  public interface ITeamFoundationCatalogService : IVssFrameworkService
  {
    CatalogTransactionContext CreateTransactionContext();

    List<CatalogNode> QueryCatalogDependents(
      IVssRequestContext requestContext,
      string path,
      CatalogQueryOptions queryOptions);

    List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter);

    List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter,
      IEnumerable<KeyValuePair<string, string>> propertyFilters);

    List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions);

    List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions);

    List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions);

    List<CatalogNode> QueryParents(
      IVssRequestContext requestContext,
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions);

    List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions);

    List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions);

    List<CatalogResource> QueryResourcesByType(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      CatalogQueryOptions queryOptions);

    CatalogResourceType QueryResourceType(
      IVssRequestContext requestContext,
      Guid resourceTypeIdentifier);

    List<CatalogResourceType> QueryResourceTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeIdentifiers);

    CatalogNode QueryRootNode(IVssRequestContext requestContext, CatalogTree tree);

    List<CatalogNode> QueryRootNodes(IVssRequestContext requestContext);

    List<CatalogResource> SaveTransactionContextChanges(
      IVssRequestContext requestContext,
      CatalogTransactionContext context,
      CatalogQueryOptions queryOptions,
      bool preview,
      out List<CatalogResource> deletedResources,
      out List<CatalogNode> deletedNodes);
  }
}
