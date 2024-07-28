// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ICatalogService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface ICatalogService
  {
    ReadOnlyCollection<CatalogResourceType> QueryResourceTypes(
      IEnumerable<Guid> resourceTypeIdentifiers);

    ReadOnlyCollection<CatalogResource> QueryResources(
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogResource> QueryResourcesByType(
      IEnumerable<Guid> resourceTypeIdentifiers,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogResource> QueryResources(
      IEnumerable<Guid> resourceTypeIdentifiers,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogNode> QueryUpTree(
      string path,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogNode> QueryParents(
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogNode> QueryNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogNode> QueryNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions);

    ReadOnlyCollection<CatalogNode> RootNodes { get; }

    CatalogNode QueryRootNode(CatalogTree tree);

    void SaveResource(CatalogResource resource);

    void SaveNode(CatalogNode node);

    void SaveDelete(CatalogNode node, bool recurse);

    void SaveMove(CatalogNode nodeToMove, CatalogNode newParent);

    CatalogChangeContext CreateChangeContext();

    ILocationService LocationService { get; }
  }
}
