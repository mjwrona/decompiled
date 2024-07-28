// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ShardDetailsComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ShardDetailsComponentV2 : ShardDetailsComponent
  {
    public ShardDetailsComponentV2()
    {
    }

    internal ShardDetailsComponentV2(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override IList<ShardDetails> GetActiveShards(
      string esClusterName,
      IEntityType entityType)
    {
      this.ValidateNotNullOrEmptyString(nameof (esClusterName), esClusterName);
      List<ShardDetails> activeShards = new List<ShardDetails>();
      int parameterValue = -1;
      while (true)
      {
        this.PrepareStoredProcedure("Search.prc_GetActiveShardDetails");
        this.BindString("@esCluster", esClusterName, 100, false, SqlDbType.NVarChar);
        this.BindByte("@entityType", (byte) entityType.ID);
        this.BindInt("@count", 500);
        this.BindInt("@startingId", parameterValue);
        List<ShardDetails> shardDetailsList = new List<ShardDetails>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ShardDetails>((ObjectBinder<ShardDetails>) new ShardDetailsComponentV2.ShardDetailsColumnsV2(esClusterName, entityType));
          ObjectBinder<ShardDetails> current = resultCollection.GetCurrent<ShardDetails>();
          if (current?.Items != null && current.Items.Count > 0)
            shardDetailsList.AddRange((IEnumerable<ShardDetails>) current.Items);
          activeShards.AddRange((IEnumerable<ShardDetails>) shardDetailsList);
          if (shardDetailsList.Count >= 500)
            parameterValue = shardDetailsList.Max<ShardDetails>((System.Func<ShardDetails, int>) (x => x.Id)) + 1;
          else
            break;
        }
      }
      return (IList<ShardDetails>) activeShards;
    }

    public override void MarkShardsInactive(
      string esCluster,
      IEntityType entityType,
      string indexName)
    {
      this.ValidateNotNullOrEmptyString(nameof (esCluster), esCluster);
      this.ValidateNotNullOrEmptyString(nameof (indexName), indexName);
      this.PrepareStoredProcedure("Search.prc_MarkShardsInActive");
      this.BindString("@esCluster", esCluster, 100, false, SqlDbType.NVarChar);
      this.BindByte("@entityType", (byte) entityType.ID);
      this.BindString("@indexName", indexName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    protected class ShardDetailsColumnsV2 : ObjectBinder<ShardDetails>
    {
      private string m_clusterName;
      private IEntityType m_entityType;
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_indexName = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_shardId = new SqlColumnBinder("ShardId");
      private SqlColumnBinder m_estimatedDocCount = new SqlColumnBinder("EstimatedDocCount");
      private SqlColumnBinder m_estimatedDocCountGrowth = new SqlColumnBinder("EstimatedDocCountGrowth");
      private SqlColumnBinder m_reservedDocCount = new SqlColumnBinder("ReservedDocCount");

      public ShardDetailsColumnsV2(string esClusterName, IEntityType entityType)
      {
        this.m_clusterName = esClusterName;
        this.m_entityType = entityType;
      }

      protected override ShardDetails Bind() => new ShardDetails()
      {
        Id = this.m_id.GetInt32((IDataReader) this.Reader),
        EsClusterName = this.m_clusterName,
        EntityType = this.m_entityType,
        IndexName = this.m_indexName.GetString((IDataReader) this.Reader, false),
        ShardId = this.m_shardId.GetInt16((IDataReader) this.Reader),
        ActualDocCount = -1,
        EstimatedDocCount = this.m_estimatedDocCount.GetInt32((IDataReader) this.Reader),
        EstimatedDocCountGrowth = this.m_estimatedDocCountGrowth.GetInt32((IDataReader) this.Reader),
        ReservedDocCount = this.m_reservedDocCount.GetInt32((IDataReader) this.Reader),
        DeletedDocCount = -1,
        ActualSize = -1,
        EstimatedSize = -1,
        EstimatedSizeGrowth = -1,
        ReservedSpace = -1
      };
    }
  }
}
