// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL.ShardDetailsDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL
{
  internal class ShardDetailsDataAccess : SqlAzureDataAccess, IShardDetailsDataAccess
  {
    public void InsertShardDetails(
      IVssRequestContext requestContext,
      List<ShardDetails> shardDetailsList)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.InsertShardDetails(shardDetailsList);
    }

    public void InsertOrUpdateShardDetails(
      IVssRequestContext requestContext,
      List<ShardDetails> shardDetailsList)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.InsertOrUpdateShardDetails(shardDetailsList);
    }

    public virtual IList<ShardDetails> QueryShardDetailsForAnIndex(
      IVssRequestContext requestContext,
      string esCluster,
      IEntityType entityType,
      string indexName)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        return (IList<ShardDetails>) component.QueryShardDetailsForAnIndex(esCluster, entityType, indexName);
    }

    public void UpdateActualShardDetails(
      IVssRequestContext requestContext,
      IList<ShardDetailsActualInfo> shardDetailsActualInfoList)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.UpdateActualShardDetails(shardDetailsActualInfoList);
    }

    public void UpdateShardEstimations(
      IVssRequestContext requestContext,
      List<ShardEstimateChange> shardEstimateChangeList)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.UpdateShardEstimations(shardEstimateChangeList);
    }

    public void DeleteShardDetailsByIds(
      IVssRequestContext requestContext,
      List<int> shardDetailsIdList)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.DeleteShardDetailsByIds(shardDetailsIdList);
    }

    public IDictionary<string, IList<ShardDetails>> GetActiveShards(
      IVssRequestContext requestContext,
      string esClusterName,
      IEntityType entityType)
    {
      IDictionary<string, IList<ShardDetails>> activeShards = (IDictionary<string, IList<ShardDetails>>) new FriendlyDictionary<string, IList<ShardDetails>>();
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
      {
        foreach (ShardDetails activeShard in (IEnumerable<ShardDetails>) component.GetActiveShards(esClusterName, entityType))
        {
          string indexName = activeShard.IndexName;
          IList<ShardDetails> shardDetailsList;
          if (!activeShards.TryGetValue(indexName, out shardDetailsList))
          {
            shardDetailsList = (IList<ShardDetails>) new List<ShardDetails>();
            activeShards[indexName] = shardDetailsList;
          }
          shardDetailsList.Add(activeShard);
        }
        return activeShards;
      }
    }

    public void MarkShardsInactive(
      IVssRequestContext requestContext,
      string esClusterName,
      IEntityType entityType,
      string indexName)
    {
      using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
        component.MarkShardsInactive(esClusterName, entityType, indexName);
    }
  }
}
