// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogComponent3
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogComponent3 : CatalogComponent2
  {
    public CatalogComponent3() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override ResultCollection QueryCatalogNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<int> artifactIdFilters,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogNodes");
      this.BindCatalogPathSpecTable("@pathSpecItems", pathSpecs, internalPath);
      this.BindGuidTable("@resourceTypeFilter", resourceTypeFilters);
      this.BindInt32Table("@artifactIdFilter", artifactIdFilters);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogNodes", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    public override ResultCollection QueryCatalogResources(
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogResources");
      this.BindGuidTable("@identifier", resourceIdentifiers);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResources", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    public override ResultCollection QueryCatalogResourcesByTypeAndArtifactId(
      IEnumerable<Guid> resourceTypes,
      IEnumerable<int> artifactIds,
      CatalogQueryOptions queryOptions)
    {
      string internalPath = this.GetInternalPath();
      bool flag = (queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies;
      this.PrepareStoredProcedure("prc_QueryCatalogResourcesByType");
      this.BindGuidTable("@resourceType", resourceTypes);
      this.BindInt32Table("@artifactIdFilter", artifactIds);
      this.BindBoolean("@expandDependencies", flag);
      this.BindBoolean("@includeParents", (queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResourcesByType", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }

    protected override string GetInternalPath() => CatalogRoots.OrganizationalPath;
  }
}
