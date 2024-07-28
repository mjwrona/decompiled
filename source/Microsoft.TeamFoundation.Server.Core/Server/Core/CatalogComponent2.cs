// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogComponent2
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
  internal class CatalogComponent2 : CatalogComponent
  {
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
      this.BindString("@organizationalRoot", internalPath, CatalogConstants.MaximumPathLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@infrastructureRoot", CatalogRoots.InfrastructurePath, CatalogConstants.MaximumPathLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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
      this.BindString("@organizationalRoot", internalPath, CatalogConstants.MaximumPathLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@infrastructureRoot", CatalogRoots.InfrastructurePath, CatalogConstants.MaximumPathLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryCatalogResourcesByType", this.RequestContext);
      resultCollection.AddBinder<CatalogResource>((ObjectBinder<CatalogResource>) new CatalogResourceColumns());
      resultCollection.AddBinder<CatalogServiceReference>((ObjectBinder<CatalogServiceReference>) new CatalogServiceReferenceColumns());
      resultCollection.AddBinder<CatalogNode>((ObjectBinder<CatalogNode>) new CatalogNodeColumns(flag, internalPath));
      if (flag)
        resultCollection.AddBinder<CatalogNodeDependency>((ObjectBinder<CatalogNodeDependency>) new CatalogNodeDependencyColumns(internalPath));
      return resultCollection;
    }
  }
}
