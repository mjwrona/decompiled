// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ShardDetailsComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ShardDetailsComponent : SQLTable<ShardDetails>
  {
    protected internal const int BatchCount = 500;
    private const string ServiceName = "Search_ShardDetails";
    protected IEnumerable<IEntityType> m_entityTypes;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ShardDetailsComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ShardDetailsComponentV2>(2),
      (IComponentCreator) new ComponentCreator<ShardDetailsComponentV3>(3)
    }, "Search_ShardDetails");
    private static readonly SqlMetaData[] s_shardDetailsSqlTableEntityLookupTable = new SqlMetaData[13]
    {
      new SqlMetaData("EsCluster", SqlDbType.NVarChar, 100L),
      new SqlMetaData("EntityType", SqlDbType.TinyInt),
      new SqlMetaData("IndexName", SqlDbType.VarChar, (long) byte.MaxValue),
      new SqlMetaData("ShardId", SqlDbType.SmallInt),
      new SqlMetaData("ActualDocCount", SqlDbType.Int),
      new SqlMetaData("EstimatedDocCount", SqlDbType.Int),
      new SqlMetaData("EstimatedDocCountGrowth", SqlDbType.Int),
      new SqlMetaData("ReservedDocCount", SqlDbType.Int),
      new SqlMetaData("DeletedDocCount", SqlDbType.Int),
      new SqlMetaData("ActualSize", SqlDbType.BigInt),
      new SqlMetaData("EstimatedSize", SqlDbType.BigInt),
      new SqlMetaData("EstimatedSizeGrowth", SqlDbType.BigInt),
      new SqlMetaData("ReservedSpace", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] s_shardEstimateSqlTableEntityLookupTable = new SqlMetaData[10]
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
      new SqlMetaData("ChangeInReservedSpace", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] s_shardDetailsActualInfoSqlTableEntityLookupTable = new SqlMetaData[6]
    {
      new SqlMetaData("EsCluster", SqlDbType.NVarChar, 100L),
      new SqlMetaData("IndexName", SqlDbType.VarChar, (long) byte.MaxValue),
      new SqlMetaData("ShardId", SqlDbType.SmallInt),
      new SqlMetaData("ActualDocCount", SqlDbType.Int),
      new SqlMetaData("DeletedDocCount", SqlDbType.Int),
      new SqlMetaData("ActualSize", SqlDbType.BigInt)
    };

    public ShardDetailsComponent()
      : base(false)
    {
    }

    internal ShardDetailsComponent(string connectionString, IVssRequestContext requestContext)
      : base(false)
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public virtual void InsertShardDetails(List<ShardDetails> shardDetailsList)
    {
      this.ValidateNotNull<List<ShardDetails>>(nameof (shardDetailsList), shardDetailsList);
      if (!shardDetailsList.Any<ShardDetails>())
        return;
      this.ValidateShardDetailsList(shardDetailsList);
      int count1 = shardDetailsList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<ShardDetails> range = (IList<ShardDetails>) shardDetailsList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_AddShardDetails");
        this.BindShardDetailsLookupTable("@itemList", (IEnumerable<ShardDetails>) range);
        this.ExecuteNonQuery(false);
      }
    }

    public virtual void InsertOrUpdateShardDetails(List<ShardDetails> shardDetailsList)
    {
      this.ValidateNotNull<List<ShardDetails>>(nameof (shardDetailsList), shardDetailsList);
      if (!shardDetailsList.Any<ShardDetails>())
        return;
      this.ValidateShardDetailsList(shardDetailsList);
      int count1 = shardDetailsList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<ShardDetails> range = (IList<ShardDetails>) shardDetailsList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_AddOrUpdateShardDetails");
        this.BindShardDetailsLookupTable("@itemList", (IEnumerable<ShardDetails>) range);
        this.ExecuteNonQuery(false);
      }
    }

    public virtual List<ShardDetails> QueryShardDetailsForAnIndex(
      string esCluster,
      IEntityType entityType,
      string indexName)
    {
      this.ValidateNotNullOrEmptyString(nameof (esCluster), esCluster);
      this.ValidateNotNullOrEmptyString(nameof (indexName), indexName);
      this.PrepareStoredProcedure("Search.prc_QueryShardDetailsForIndex");
      this.BindString("@esCluster", esCluster, 100, false, SqlDbType.NVarChar);
      this.BindByte("@entityType", (byte) entityType.ID);
      this.BindString("@indexName", indexName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShardDetails>((ObjectBinder<ShardDetails>) new ShardDetailsComponent.ShardDetailsColumns(this.m_entityTypes));
        ObjectBinder<ShardDetails> current = resultCollection.GetCurrent<ShardDetails>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<ShardDetails>();
      }
    }

    public virtual List<DiffInShardEstimates> UpdateShardEstimations(
      List<ShardEstimateChange> shardEstimateChangeList)
    {
      this.ValidateNotNullOrEmptyList<ShardEstimateChange>(nameof (shardEstimateChangeList), (IList<ShardEstimateChange>) shardEstimateChangeList);
      this.ValidateShardEstimateChangeList(shardEstimateChangeList);
      this.PrepareStoredProcedure("Search.prc_UpdateShardEstimations");
      this.BindShardEstimatesLookupTable("@itemList", (IEnumerable<ShardEstimateChange>) shardEstimateChangeList);
      this.ExecuteNonQuery(false);
      return new List<DiffInShardEstimates>();
    }

    public virtual void UpdateActualShardDetails(
      IList<ShardDetailsActualInfo> shardDetailsActualInfoList)
    {
      this.ValidateNotNullOrEmptyList<ShardDetailsActualInfo>(nameof (shardDetailsActualInfoList), shardDetailsActualInfoList);
      this.ValidateShardDetailsActualInfoList(shardDetailsActualInfoList);
      this.PrepareStoredProcedure("Search.prc_UpdateActualShardInfo");
      this.BindShardDetailsActualInfoLookupTable("itemList", (IEnumerable<ShardDetailsActualInfo>) shardDetailsActualInfoList);
      this.ExecuteNonQuery(false);
    }

    public virtual void DeleteShardDetailsByIds(List<int> shardDetailsIdList)
    {
      this.ValidateNotNull<List<int>>(nameof (shardDetailsIdList), shardDetailsIdList);
      if (!shardDetailsIdList.Any<int>())
        return;
      shardDetailsIdList = shardDetailsIdList.Distinct<int>().ToList<int>();
      int count1 = shardDetailsIdList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<int> range = (IList<int>) shardDetailsIdList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_DeleteShardDetailsByIds");
        this.BindShardDetailsIdTable("itemList", range);
        this.ExecuteNonQuery(false);
      }
    }

    public virtual IList<ShardDetails> GetActiveShards(string esClusterName, IEntityType entityType) => (IList<ShardDetails>) new List<ShardDetails>();

    public virtual void MarkShardsInactive(
      string esCluster,
      IEntityType entityType,
      string indexName)
    {
    }

    private SqlParameter BindShardDetailsLookupTable(
      string parameterName,
      IEnumerable<ShardDetails> rows)
    {
      rows = rows ?? Enumerable.Empty<ShardDetails>();
      System.Func<ShardDetails, SqlDataRecord> selector = (System.Func<ShardDetails, SqlDataRecord>) (shardDetails =>
      {
        SqlDataRecord record = new SqlDataRecord(ShardDetailsComponent.s_shardDetailsSqlTableEntityLookupTable);
        record.SetString(0, shardDetails.EsClusterName.ToString());
        record.SetByte(1, (byte) shardDetails.EntityType.ID);
        record.SetString(2, shardDetails.IndexName.ToString());
        record.SetInt16(3, shardDetails.ShardId);
        record.SetInt32(4, shardDetails.ActualDocCount);
        record.SetNullableInt32(5, new int?(shardDetails.EstimatedDocCount));
        record.SetNullableInt32(6, new int?(shardDetails.EstimatedDocCountGrowth));
        record.SetNullableInt32(7, new int?(shardDetails.ReservedDocCount));
        record.SetInt32(8, shardDetails.DeletedDocCount);
        record.SetInt64(9, shardDetails.ActualSize);
        record.SetValue(10, (object) shardDetails.EstimatedSize);
        record.SetValue(11, (object) shardDetails.EstimatedSizeGrowth);
        record.SetValue(12, (object) shardDetails.ReservedSpace);
        return record;
      });
      return this.BindTable(parameterName, "Search.typ_ShardDetailsV2", rows.Select<ShardDetails, SqlDataRecord>(selector));
    }

    private SqlParameter BindShardEstimatesLookupTable(
      string parameterName,
      IEnumerable<ShardEstimateChange> rows)
    {
      rows = rows ?? Enumerable.Empty<ShardEstimateChange>();
      System.Func<ShardEstimateChange, SqlDataRecord> selector = (System.Func<ShardEstimateChange, SqlDataRecord>) (shardEstimateChange =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ShardDetailsComponent.s_shardEstimateSqlTableEntityLookupTable);
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
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ShardEstimationsV2", rows.Select<ShardEstimateChange, SqlDataRecord>(selector));
    }

    private SqlParameter BindShardDetailsActualInfoLookupTable(
      string parameterName,
      IEnumerable<ShardDetailsActualInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<ShardDetailsActualInfo>();
      System.Func<ShardDetailsActualInfo, SqlDataRecord> selector = (System.Func<ShardDetailsActualInfo, SqlDataRecord>) (shardDetailsActualInfo =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ShardDetailsComponent.s_shardDetailsActualInfoSqlTableEntityLookupTable);
        sqlDataRecord.SetString(0, shardDetailsActualInfo.EsCluster.ToString());
        sqlDataRecord.SetString(1, shardDetailsActualInfo.IndexName.ToString());
        sqlDataRecord.SetInt16(2, shardDetailsActualInfo.ShardId);
        sqlDataRecord.SetInt32(3, shardDetailsActualInfo.ActualDocCount);
        sqlDataRecord.SetInt32(4, shardDetailsActualInfo.DeletedDocCount);
        sqlDataRecord.SetValue(5, (object) shardDetailsActualInfo.ActualSize);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ActualShardInfoV2", rows.Select<ShardDetailsActualInfo, SqlDataRecord>(selector));
    }

    protected SqlParameter BindShardDetailsIdTable(string parameterName, IList<int> rows)
    {
      rows = rows ?? (IList<int>) new List<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (id =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(new SqlMetaData[1]
        {
          new SqlMetaData("Id", SqlDbType.Int)
        });
        sqlDataRecord.SetInt32(0, id);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Int32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    private void ValidateShardDetailsList(List<ShardDetails> shardDetailsList)
    {
      foreach (ShardDetails shardDetails in shardDetailsList)
      {
        this.ValidateNotNullOrEmptyString("EsClusterName", shardDetails.EsClusterName);
        this.ValidateNotNullOrEmptyString("IndexName", shardDetails.IndexName);
      }
    }

    internal void ValidateShardEstimateChangeList(List<ShardEstimateChange> shardEstimateChangeList)
    {
      foreach (ShardEstimateChange shardEstimateChange in shardEstimateChangeList)
      {
        this.ValidateNotNullOrEmptyString("EsCluster", shardEstimateChange.EsCluster);
        this.ValidateNotNullOrEmptyString("IndexName", shardEstimateChange.IndexName);
      }
    }

    internal void ValidateShardDetailsActualInfoList(
      IList<ShardDetailsActualInfo> shardDetailsActualInfoList)
    {
      foreach (ShardDetailsActualInfo detailsActualInfo in (IEnumerable<ShardDetailsActualInfo>) shardDetailsActualInfoList)
      {
        this.ValidateNotNullOrEmptyString("EsCluster", detailsActualInfo.EsCluster);
        this.ValidateNotNullOrEmptyString("IndexName", detailsActualInfo.IndexName);
      }
    }

    protected class ShardDetailsColumns : ObjectBinder<ShardDetails>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_esCluster = new SqlColumnBinder("esCluster");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("entityType");
      private SqlColumnBinder m_indexName = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_shardId = new SqlColumnBinder("ShardId");
      private SqlColumnBinder m_actualDocCount = new SqlColumnBinder("ActualDocCount");
      private SqlColumnBinder m_estimatedDocCount = new SqlColumnBinder("EstimatedDocCount");
      private SqlColumnBinder m_estimatedDocCountGrowth = new SqlColumnBinder("EstimatedDocCountGrowth");
      private SqlColumnBinder m_reservedDocCount = new SqlColumnBinder("ReservedDocCount");
      private SqlColumnBinder m_deletedDocCount = new SqlColumnBinder("DeletedDocCount");
      private SqlColumnBinder m_actualSize = new SqlColumnBinder("ActualSize");
      private SqlColumnBinder m_estimatedSize = new SqlColumnBinder("EstimatedSize");
      private SqlColumnBinder m_estimatedSizeGrowth = new SqlColumnBinder("EstimatedSizeGrowth");
      private SqlColumnBinder m_reservedSpace = new SqlColumnBinder("ReservedSpace");
      private SqlColumnBinder m_createdTimeStamp = new SqlColumnBinder("CreatedTimeStamp");
      private SqlColumnBinder m_lastModifiedTimeStamp = new SqlColumnBinder("LastModifiedTimeStamp");
      private IEnumerable<IEntityType> m_entityTypes;

      protected override ShardDetails Bind() => new ShardDetails()
      {
        Id = this.m_id.GetInt32((IDataReader) this.Reader),
        EsClusterName = this.m_esCluster.GetString((IDataReader) this.Reader, false),
        EntityType = EntityPluginsFactory.GetEntityType(this.m_entityTypes, (int) this.m_entityType.GetByte((IDataReader) this.Reader)),
        IndexName = this.m_indexName.GetString((IDataReader) this.Reader, false),
        ShardId = this.m_shardId.GetInt16((IDataReader) this.Reader),
        ActualDocCount = this.m_actualDocCount.GetInt32((IDataReader) this.Reader),
        EstimatedDocCount = this.m_estimatedDocCount.GetInt32((IDataReader) this.Reader),
        EstimatedDocCountGrowth = this.m_estimatedDocCountGrowth.GetInt32((IDataReader) this.Reader),
        ReservedDocCount = this.m_reservedDocCount.GetInt32((IDataReader) this.Reader),
        DeletedDocCount = this.m_deletedDocCount.GetInt32((IDataReader) this.Reader),
        ActualSize = this.m_actualSize.GetInt64((IDataReader) this.Reader),
        EstimatedSize = this.m_estimatedSize.GetInt64((IDataReader) this.Reader),
        EstimatedSizeGrowth = this.m_estimatedSizeGrowth.GetInt64((IDataReader) this.Reader),
        ReservedSpace = this.m_reservedSpace.GetInt64((IDataReader) this.Reader),
        CreatedTimeStamp = this.m_createdTimeStamp.GetDateTime((IDataReader) this.Reader),
        LastModifiedTimeStamp = this.m_lastModifiedTimeStamp.GetDateTime((IDataReader) this.Reader)
      };

      public ShardDetailsColumns(IEnumerable<IEntityType> entityTypes) => this.m_entityTypes = entityTypes;
    }
  }
}
