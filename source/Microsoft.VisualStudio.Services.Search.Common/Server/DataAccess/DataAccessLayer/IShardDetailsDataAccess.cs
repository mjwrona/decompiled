// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IShardDetailsDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IShardDetailsDataAccess
  {
    void InsertShardDetails(IVssRequestContext requestContext, List<ShardDetails> shardDetailsList);

    void InsertOrUpdateShardDetails(
      IVssRequestContext requestContext,
      List<ShardDetails> shardDetailsList);

    IList<ShardDetails> QueryShardDetailsForAnIndex(
      IVssRequestContext requestContext,
      string esCluster,
      IEntityType entityType,
      string indexName);

    void UpdateShardEstimations(
      IVssRequestContext requestContext,
      List<ShardEstimateChange> shardEstimateChangeList);

    void UpdateActualShardDetails(
      IVssRequestContext requestContext,
      IList<ShardDetailsActualInfo> shardDetailsActualInfoList);

    void DeleteShardDetailsByIds(IVssRequestContext requestContext, List<int> shardDetailsIdList);

    IDictionary<string, IList<ShardDetails>> GetActiveShards(
      IVssRequestContext requestContext,
      string esClusterName,
      IEntityType entityType);

    void MarkShardsInactive(
      IVssRequestContext requestContext,
      string esClusterName,
      IEntityType entityType,
      string indexName);
  }
}
