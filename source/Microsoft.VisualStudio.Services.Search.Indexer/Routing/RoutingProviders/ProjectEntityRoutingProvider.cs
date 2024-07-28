// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.ProjectEntityRoutingProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class ProjectEntityRoutingProvider : IRoutingDataProvider
  {
    private readonly string m_routingId;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) ProjectRepoEntityType.GetInstance();

    internal ProjectEntityRoutingProvider(string routing) => this.m_routingId = routing;

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item) => this.m_routingId;

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      throw new NotImplementedException();
    }

    public List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      throw new NotImplementedException();
    }
  }
}
