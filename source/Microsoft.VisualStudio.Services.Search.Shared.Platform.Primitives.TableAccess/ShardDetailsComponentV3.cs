// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ShardDetailsComponentV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ShardDetailsComponentV3 : ShardDetailsComponentV2
  {
    private static readonly SqlMetaData[] s_shardEstimateSqlTableEntityLookupTable = new SqlMetaData[13]
    {
      new SqlMetaData("EsCluster", SqlDbType.NVarChar, 100L),
      new SqlMetaData("EntityType", SqlDbType.TinyInt),
      new SqlMetaData("IndexName", SqlDbType.VarChar, (long) byte.MaxValue),
      new SqlMetaData("ShardId", SqlDbType.SmallInt),
      new SqlMetaData("ChangeInEstimatedDocCount", SqlDbType.Int),
      new SqlMetaData("ChangeInEstimatedDocCountGrowth", SqlDbType.Int),
      new SqlMetaData("ChangeInReservedDocCount", SqlDbType.Int),
      new SqlMetaData("ChangeInEstimatedSize", SqlDbType.BigInt),
      new SqlMetaData("ChangeInEstimatedSizeGrowth", SqlDbType.BigInt),
      new SqlMetaData("ChangeInReservedSpace", SqlDbType.BigInt),
      new SqlMetaData("OriginalEstimatedDocCount", SqlDbType.Int),
      new SqlMetaData("OriginalEstimatedDocCountGrowth", SqlDbType.Int),
      new SqlMetaData("OriginalReservedDocCount", SqlDbType.Int)
    };

    public ShardDetailsComponentV3()
    {
    }

    internal ShardDetailsComponentV3(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override List<DiffInShardEstimates> UpdateShardEstimations(
      List<ShardEstimateChange> shardEstimateChangeList)
    {
      this.ValidateNotNullOrEmptyList<ShardEstimateChange>(nameof (shardEstimateChangeList), (IList<ShardEstimateChange>) shardEstimateChangeList);
      this.ValidateShardEstimateChangeList(shardEstimateChangeList);
      this.PrepareStoredProcedure("Search.prc_UpdateShardEstimations");
      this.BindShardEstimatesLookupTable("@itemList", (IEnumerable<ShardEstimateChange>) shardEstimateChangeList);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DiffInShardEstimates>((ObjectBinder<DiffInShardEstimates>) new ShardDetailsComponentV3.DiffInShardEstimatesColumns(this.m_entityTypes));
        ObjectBinder<DiffInShardEstimates> current = resultCollection.GetCurrent<DiffInShardEstimates>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<DiffInShardEstimates>();
      }
    }

    private SqlParameter BindShardEstimatesLookupTable(
      string parameterName,
      IEnumerable<ShardEstimateChange> rows)
    {
      rows = rows ?? Enumerable.Empty<ShardEstimateChange>();
      System.Func<ShardEstimateChange, SqlDataRecord> selector = (System.Func<ShardEstimateChange, SqlDataRecord>) (shardEstimateChange =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ShardDetailsComponentV3.s_shardEstimateSqlTableEntityLookupTable);
        sqlDataRecord.SetString(0, shardEstimateChange.EsCluster);
        sqlDataRecord.SetByte(1, (byte) shardEstimateChange.EntityType.ID);
        sqlDataRecord.SetString(2, shardEstimateChange.IndexName);
        sqlDataRecord.SetInt16(3, shardEstimateChange.ShardId);
        sqlDataRecord.SetInt32(4, shardEstimateChange.ChangeInEstimatedDocCount);
        sqlDataRecord.SetInt32(5, shardEstimateChange.ChangeInEstimatedDocCountGrowth);
        sqlDataRecord.SetInt32(6, shardEstimateChange.ChangeInReservedDocCount);
        sqlDataRecord.SetInt64(7, shardEstimateChange.ChangeInEstimatedSize);
        sqlDataRecord.SetInt64(8, shardEstimateChange.ChangeInEstimatedSizeGrowth);
        sqlDataRecord.SetInt64(9, shardEstimateChange.ChangeInReservedSpace);
        sqlDataRecord.SetInt32(10, shardEstimateChange.OriginalEstimatedDocCount);
        sqlDataRecord.SetInt32(11, shardEstimateChange.OriginalEstimatedDocCountGrowth);
        sqlDataRecord.SetInt32(12, shardEstimateChange.OriginalReservedDocCount);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ShardEstimationsV3", rows.Select<ShardEstimateChange, SqlDataRecord>(selector));
    }

    protected class DiffInShardEstimatesColumns : ObjectBinder<DiffInShardEstimates>
    {
      private SqlColumnBinder m_esCluster = new SqlColumnBinder("esCluster");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("entityType");
      private SqlColumnBinder m_indexName = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_shardId = new SqlColumnBinder("ShardId");
      private SqlColumnBinder m_diffInEstimatedDocCount = new SqlColumnBinder("DiffInEstimatedDocCount");
      private SqlColumnBinder m_diffInEstimatedDocCountGrowth = new SqlColumnBinder("DiffInEstimatedDocCountGrowth");
      private SqlColumnBinder m_diffInReservedDocCount = new SqlColumnBinder("DiffInReservedDocCount");
      private IEnumerable<IEntityType> m_entityTypes;

      public DiffInShardEstimatesColumns(IEnumerable<IEntityType> entityTypes) => this.m_entityTypes = entityTypes;

      protected override DiffInShardEstimates Bind() => new DiffInShardEstimates(this.m_esCluster.GetString((IDataReader) this.Reader, false), EntityPluginsFactory.GetEntityType(this.m_entityTypes, (int) this.m_entityType.GetByte((IDataReader) this.Reader)), this.m_indexName.GetString((IDataReader) this.Reader, false), this.m_shardId.GetInt16((IDataReader) this.Reader), this.m_diffInEstimatedDocCount.GetInt32((IDataReader) this.Reader), this.m_diffInEstimatedDocCountGrowth.GetInt32((IDataReader) this.Reader), this.m_diffInReservedDocCount.GetInt32((IDataReader) this.Reader));
    }
  }
}
