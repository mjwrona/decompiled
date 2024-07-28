// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitDetailsComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitDetailsComponent : SQLTable<IndexingUnitDetails>
  {
    protected internal const int BatchCountForQueries = 500;
    private const string ServiceName = "Search_IndexingUnitIndexingInformation";
    protected IEnumerable<Type> m_indexingPropertiesKnownTypes;
    protected IEnumerable<Type> m_tfsEntityAttributesKnownTypes;
    protected IEnumerable<IEntityType> m_entityTypes;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<IndexingUnitDetailsComponent>(1, true)
    }, "Search_IndexingUnitIndexingInformation");
    private static readonly SqlMetaData[] s_indexingUnitDetailsSqlTableEntityLookupTable = new SqlMetaData[18]
    {
      new SqlMetaData("IndexingUnitId", SqlDbType.Int),
      new SqlMetaData("TFSEntityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexingUnitType", SqlDbType.VarChar, 64L),
      new SqlMetaData("EntityTypeVarChar", SqlDbType.VarChar, 32L),
      new SqlMetaData("EstimatedInitialDocCount", SqlDbType.Int),
      new SqlMetaData("ActualInitialDocCount", SqlDbType.Int),
      new SqlMetaData("EstimatedDocCountGrowth", SqlDbType.Int),
      new SqlMetaData("ActualDocCountGrowth", SqlDbType.Int),
      new SqlMetaData("EstimatedInitialSize", SqlDbType.BigInt),
      new SqlMetaData("ActualInitialSize", SqlDbType.BigInt),
      new SqlMetaData("EstimatedSizeGrowth", SqlDbType.BigInt),
      new SqlMetaData("ActualSizeGrowth", SqlDbType.BigInt),
      new SqlMetaData("EntityType", SqlDbType.TinyInt),
      new SqlMetaData("EsCluster", SqlDbType.NVarChar, 100L),
      new SqlMetaData("IndexName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("RoutingIds", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ShardIds", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LastIndexedWatermark", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_indexingUnitSqlTableEntityLookupTable = new SqlMetaData[7]
    {
      new SqlMetaData("TFSEntityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexingUnitType", SqlDbType.VarChar, 64L),
      new SqlMetaData("EntityType", SqlDbType.VarChar, 32L),
      new SqlMetaData("TFSEntityAttributes", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ParentUnitID", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AssociatedJobId", SqlDbType.UniqueIdentifier)
    };

    public IndexingUnitDetailsComponent()
      : base()
    {
    }

    internal IndexingUnitDetailsComponent(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      DataContractKnownTypesPluginService service = context.GetService<DataContractKnownTypesPluginService>();
      this.m_indexingPropertiesKnownTypes = service.GetKnownTypes(typeof (IndexingProperties));
      this.m_tfsEntityAttributesKnownTypes = service.GetKnownTypes(typeof (TFSEntityAttributes));
      this.m_entityTypes = context.GetService<IEntityService>().GetEntityTypes();
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      DataContractKnownTypesPluginService service = context.GetService<DataContractKnownTypesPluginService>();
      this.m_indexingPropertiesKnownTypes = service.GetKnownTypes(typeof (IndexingProperties));
      this.m_tfsEntityAttributesKnownTypes = service.GetKnownTypes(typeof (TFSEntityAttributes));
      this.m_entityTypes = context.GetService<IEntityService>().GetEntityTypes();
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public void InsertIndexingUnitDetails(List<IndexingUnitDetails> indexingUnitDetailsList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnitDetails>(nameof (indexingUnitDetailsList), (IList<IndexingUnitDetails>) indexingUnitDetailsList);
      int count1 = indexingUnitDetailsList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<IndexingUnitDetails> range = (IList<IndexingUnitDetails>) indexingUnitDetailsList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_AddIndexingUnitIndexingInformation");
        this.BindIndexingUnitDetailsLookupTable("@itemList", (IEnumerable<IndexingUnitDetails>) range);
        this.ExecuteNonQuery(false);
      }
    }

    public IndexingUnitDetails QueryIndexingUnitDetails(int indexingUnitId)
    {
      this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitIndexingInformation");
      this.BindInt("@indexingUnitId", indexingUnitId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IndexingUnitDetails>((ObjectBinder<IndexingUnitDetails>) new IndexingUnitDetailsComponent.IndexingUnitDetailsColumns(indexingUnitId, this.m_entityTypes));
        ObjectBinder<IndexingUnitDetails> current = resultCollection.GetCurrent<IndexingUnitDetails>();
        return current?.Items != null && current.Items.Count == 1 ? current.Items[0] : (IndexingUnitDetails) null;
      }
    }

    public virtual IDictionary<int, IndexingUnitDetails> GetIndexingUnitDetails(
      IList<int> indexingUnitIds)
    {
      IDictionary<int, IndexingUnitDetails> indexingUnitDetails1 = (IDictionary<int, IndexingUnitDetails>) new Dictionary<int, IndexingUnitDetails>();
      if (indexingUnitIds == null)
        throw new ArgumentNullException(nameof (indexingUnitIds), "Input List can not be null.");
      if (indexingUnitIds.Count == 0)
        return indexingUnitDetails1;
      List<int> list = indexingUnitIds.Distinct<int>().ToList<int>();
      int count1 = list.Count;
      int val1 = count1;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<int> range = (IList<int>) list.GetRange(index, count2);
        val1 -= count2;
        if (range.Count > 0)
        {
          this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitIndexingInformationByIds");
          this.BindIndexingUnitIdTable("@indexingUnitIdList", range);
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<IndexingUnitDetails>((ObjectBinder<IndexingUnitDetails>) new IndexingUnitDetailsComponent.IndexingUnitDetailsColumnsWithMultipleIndexingUnits(this.m_entityTypes));
            ObjectBinder<IndexingUnitDetails> current = resultCollection.GetCurrent<IndexingUnitDetails>();
            if (current?.Items != null)
            {
              foreach (IndexingUnitDetails indexingUnitDetails2 in current.Items)
                indexingUnitDetails1.Add(indexingUnitDetails2.IndexingUnitId, indexingUnitDetails2);
            }
          }
        }
        else
          break;
      }
      return indexingUnitDetails1;
    }

    public virtual List<IndexingUnitDetails> GetIndexingUnitDetails(
      IEntityType entityType,
      string indexingUnitType)
    {
      int parameterValue = 0;
      List<IndexingUnitDetails> indexingUnitDetails = new List<IndexingUnitDetails>();
      bool flag = false;
      while (!flag)
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitIndexingInformationInRange");
        this.BindString("@indexingUnitType", indexingUnitType, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindInt("@startingId", parameterValue);
        this.BindInt("@count", 500);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitDetails>((ObjectBinder<IndexingUnitDetails>) new IndexingUnitDetailsComponent.IndexingUnitDetailsInRangeColumns(entityType));
          ObjectBinder<IndexingUnitDetails> current = resultCollection.GetCurrent<IndexingUnitDetails>();
          if (current?.Items != null && current.Items.Count > 0)
          {
            if (current.Items.Count < 500)
              flag = true;
            indexingUnitDetails.AddRange((IEnumerable<IndexingUnitDetails>) current.Items.ToList<IndexingUnitDetails>());
            parameterValue = indexingUnitDetails[indexingUnitDetails.Count - 1].IndexingUnitId;
          }
          else
            flag = true;
        }
      }
      return indexingUnitDetails;
    }

    public void UpdateInitialEstimates(
      int indexingUnitId,
      string lastIndexedWatermark,
      int estimatedInitialDocCount,
      int actualInitialDocCount,
      int estimatedDocCountGrowth,
      long estimatedInitialSize,
      long actualInitialSize,
      long estimatedSizeGrowth)
    {
      this.PrepareStoredProcedure("Search.prc_UpdateInitialEstimates");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindInt("@estimatedInitialDocCount", estimatedInitialDocCount);
      this.BindInt("@actualInitialDocCount", actualInitialDocCount);
      this.BindInt("@estimatedDocCountGrowth", estimatedDocCountGrowth);
      this.BindLong("@estimatedInitialSize", estimatedInitialSize);
      this.BindLong("@actualInitialSize", actualInitialSize);
      this.BindLong("@estimatedSizeGrowth", estimatedSizeGrowth);
      this.BindString("@lastIndexedWatermark", lastIndexedWatermark, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery(false);
    }

    public void UpdateIndexingUnitGrowthDetails(
      int indexingUnitId,
      string lastIndexedWatermark,
      int changeInActualDocCount,
      long changeInActualSize)
    {
      this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnitGrowthDetails");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindInt("@changeInActualDocCount", changeInActualDocCount);
      this.BindLong("@changeInActualSize", changeInActualSize);
      this.BindString("@lastIndexedWatermark", lastIndexedWatermark, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery(false);
    }

    public virtual void AddOrUpdateIndexingUnitInformation(
      List<IndexingUnit> indexingUnitList,
      List<IndexingUnitDetails> indexingUnitDetailsList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnitList), (IList<IndexingUnit>) indexingUnitList);
      this.ValidateNotNullOrEmptyList<IndexingUnitDetails>(nameof (indexingUnitDetailsList), (IList<IndexingUnitDetails>) indexingUnitDetailsList);
      int count1 = indexingUnitList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<IndexingUnit> range1 = (IList<IndexingUnit>) indexingUnitList.GetRange(index, count2);
        IList<IndexingUnitDetails> range2 = (IList<IndexingUnitDetails>) indexingUnitDetailsList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_AddOrUpdateIndexingUnitInformation");
        this.BindIndexingUnitDetailsLookupTable("@indexingUnitIndexingInformationList", (IEnumerable<IndexingUnitDetails>) range2);
        this.BindIndexingUnitLookupTable("@indexingUnitList", (IEnumerable<IndexingUnit>) range1);
        this.ExecuteNonQuery(false);
      }
    }

    public virtual void DeleteIndexingUnitDetailsByIds(List<int> indexingUnitIdList)
    {
      this.ValidateNotNull<List<int>>(nameof (indexingUnitIdList), indexingUnitIdList);
      int count1 = indexingUnitIdList.Count;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(500, count1 - index);
        IList<int> range = (IList<int>) indexingUnitIdList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitIndexingInformationByIds");
        this.BindIndexingUnitIdTable("@indexingUnitIdList", range);
        this.ExecuteNonQuery(false);
      }
    }

    private SqlParameter BindIndexingUnitDetailsLookupTable(
      string parameterName,
      IEnumerable<IndexingUnitDetails> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitDetails>();
      System.Func<IndexingUnitDetails, SqlDataRecord> selector = (System.Func<IndexingUnitDetails, SqlDataRecord>) (indexingUnitDetails =>
      {
        SqlDataRecord record = new SqlDataRecord(IndexingUnitDetailsComponent.s_indexingUnitDetailsSqlTableEntityLookupTable);
        record.SetInt32(0, indexingUnitDetails.IndexingUnitId);
        record.SetNullableGuid(1, indexingUnitDetails.TFSEntityId);
        record.SetString(2, indexingUnitDetails.IndexingUnitType_Extended);
        record.SetString(3, indexingUnitDetails.EntityType.Name.ToString());
        record.SetInt32(4, indexingUnitDetails.EstimatedInitialDocCount);
        record.SetInt32(5, indexingUnitDetails.ActualInitialDocCount);
        record.SetInt32(6, indexingUnitDetails.EstimatedDocCountGrowth);
        record.SetInt32(7, indexingUnitDetails.ActualDocCountGrowth);
        record.SetInt64(8, indexingUnitDetails.EstimatedInitialSize);
        record.SetInt64(9, indexingUnitDetails.ActualInitialSize);
        record.SetInt64(10, indexingUnitDetails.EstimatedSizeGrowth);
        record.SetInt64(11, indexingUnitDetails.ActualSizeGrowth);
        record.SetByte(12, (byte) indexingUnitDetails.EntityType.ID);
        record.SetString(13, indexingUnitDetails.ESClusterName);
        record.SetString(14, indexingUnitDetails.IndexName);
        if (string.IsNullOrWhiteSpace(indexingUnitDetails.RoutingIds))
          record.SetDBNull(15);
        else
          record.SetString(15, indexingUnitDetails.RoutingIds);
        if (string.IsNullOrWhiteSpace(indexingUnitDetails.ShardIds))
          record.SetDBNull(16);
        else
          record.SetString(16, indexingUnitDetails.ShardIds);
        if (string.IsNullOrWhiteSpace(indexingUnitDetails.LastIndexedWatermark))
          record.SetDBNull(17);
        else
          record.SetString(17, indexingUnitDetails.LastIndexedWatermark);
        return record;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitIndexingInformationDescriptor", rows.Select<IndexingUnitDetails, SqlDataRecord>(selector));
    }

    private SqlParameter BindIndexingUnitLookupTable(
      string parameterName,
      IEnumerable<IndexingUnit> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnit>();
      System.Func<IndexingUnit, SqlDataRecord> selector = (System.Func<IndexingUnit, SqlDataRecord>) (indexingUnit =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitDetailsComponent.s_indexingUnitSqlTableEntityLookupTable);
        sqlDataRecord.SetGuid(0, indexingUnit.TFSEntityId);
        sqlDataRecord.SetString(1, indexingUnit.IndexingUnitType_Extended);
        sqlDataRecord.SetString(2, indexingUnit.EntityType.Name);
        sqlDataRecord.SetString(3, SQLTable<IndexingUnitDetails>.ToString((object) indexingUnit.TFSEntityAttributes, typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes));
        sqlDataRecord.SetInt32(4, indexingUnit.ParentUnitId);
        sqlDataRecord.SetString(5, SQLTable<IndexingUnitDetails>.ToString((object) indexingUnit.Properties, typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes));
        if (indexingUnit.AssociatedJobId.HasValue)
          sqlDataRecord.SetGuid(6, indexingUnit.AssociatedJobId.Value);
        else
          sqlDataRecord.SetDBNull(6);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitDescriptor", rows.Select<IndexingUnit, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitIdTable(string parameterName, IList<int> rows)
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

    protected class IndexingUnitDetailsColumns : ObjectBinder<IndexingUnitDetails>
    {
      private SqlColumnBinder m_estimatedInitialDocCount = new SqlColumnBinder("EstimatedInitialDocCount");
      private SqlColumnBinder m_actualInitialDocCount = new SqlColumnBinder("ActualInitialDocCount");
      private SqlColumnBinder m_estimatedDocCountGrowth = new SqlColumnBinder("EstimatedDocCountGrowth");
      private SqlColumnBinder m_actualDocCountGrowth = new SqlColumnBinder("ActualDocCountGrowth");
      private SqlColumnBinder m_estimatedInitialSize = new SqlColumnBinder("EstimatedInitialSize");
      private SqlColumnBinder m_actualInitialSize = new SqlColumnBinder("ActualInitialSize");
      private SqlColumnBinder m_estimatedSizeGrowth = new SqlColumnBinder("EstimatedSizeGrowth");
      private SqlColumnBinder m_actualSizeGrowth = new SqlColumnBinder("ActualSizeGrowth");
      private SqlColumnBinder m_indexName = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_esCluster = new SqlColumnBinder("EsCluster");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private SqlColumnBinder m_routingIds = new SqlColumnBinder("RoutingIds");
      private SqlColumnBinder m_shardIds = new SqlColumnBinder("ShardIds");
      private SqlColumnBinder m_lastIndexedWatermark = new SqlColumnBinder("LastIndexedWatermark");
      private int m_indexingUnitId;
      private IEnumerable<IEntityType> m_entityTypes;

      protected override IndexingUnitDetails Bind()
      {
        int indexingUnitId = this.GetIndexingUnitId(this.Reader);
        int int32_1 = this.m_estimatedInitialDocCount.GetInt32((IDataReader) this.Reader);
        int int32_2 = this.m_actualInitialDocCount.GetInt32((IDataReader) this.Reader);
        int int32_3 = this.m_estimatedDocCountGrowth.GetInt32((IDataReader) this.Reader);
        int int32_4 = this.m_actualDocCountGrowth.GetInt32((IDataReader) this.Reader);
        long int64_1 = this.m_estimatedInitialSize.GetInt64((IDataReader) this.Reader);
        long int64_2 = this.m_actualInitialSize.GetInt64((IDataReader) this.Reader);
        long int64_3 = this.m_estimatedSizeGrowth.GetInt64((IDataReader) this.Reader);
        long int64_4 = this.m_actualSizeGrowth.GetInt64((IDataReader) this.Reader);
        string str1 = this.m_indexName.GetString((IDataReader) this.Reader, false);
        string str2 = this.m_esCluster.GetString((IDataReader) this.Reader, false);
        IEntityType entityType1 = EntityPluginsFactory.GetEntityType(this.m_entityTypes, (int) this.m_entityType.GetByte((IDataReader) this.Reader));
        string str3 = this.m_lastIndexedWatermark.GetString((IDataReader) this.Reader, true);
        string routingIds = this.m_routingIds.GetString((IDataReader) this.Reader, true);
        string shardIds = this.m_shardIds.GetString((IDataReader) this.Reader, true);
        int estimatedInitialDocCount = int32_1;
        int actualInitialDocCount = int32_2;
        int estimatedDocCountGrowth = int32_3;
        int actualDocCountGrowth = int32_4;
        long estimatedInitialSize = int64_1;
        long actualInitialSize = int64_2;
        long estimatedSizeGrowth = int64_3;
        long actualSizeGrowth = int64_4;
        string lastIndexedWatermark = str3;
        IEntityType entityType2 = entityType1;
        string indexName = str1;
        string esClusterName = str2;
        Guid? tfsEntityId = new Guid?();
        DateTime? createdTimeStamp = new DateTime?();
        DateTime? lastModifiedTimeStamp = new DateTime?();
        return new IndexingUnitDetails(indexingUnitId, routingIds, shardIds, estimatedInitialDocCount, actualInitialDocCount, estimatedDocCountGrowth, actualDocCountGrowth, estimatedInitialSize, actualInitialSize, estimatedSizeGrowth, actualSizeGrowth, lastIndexedWatermark, entityType2, indexName, esClusterName, tfsEntityId, createdTimeStamp: createdTimeStamp, lastModifiedTimeStamp: lastModifiedTimeStamp);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected virtual int GetIndexingUnitId(SqlDataReader reader) => this.m_indexingUnitId;

      public IndexingUnitDetailsColumns(int indexingUnitId, IEnumerable<IEntityType> m_entityTypes)
      {
        this.m_indexingUnitId = indexingUnitId;
        this.m_entityTypes = m_entityTypes;
      }
    }

    protected class IndexingUnitDetailsColumnsWithMultipleIndexingUnits : 
      IndexingUnitDetailsComponent.IndexingUnitDetailsColumns
    {
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");

      public IndexingUnitDetailsColumnsWithMultipleIndexingUnits(
        IEnumerable<IEntityType> m_entityTypes)
        : base(-1, m_entityTypes)
      {
      }

      protected override int GetIndexingUnitId(SqlDataReader reader) => this.m_indexingUnitId.GetInt32((IDataReader) reader);
    }

    protected class IndexingUnitDetailsInRangeColumns : ObjectBinder<IndexingUnitDetails>
    {
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_estimatedInitialDocCount = new SqlColumnBinder("EstimatedInitialDocCount");
      private SqlColumnBinder m_estimatedDocCountGrowth = new SqlColumnBinder("EstimatedDocCountGrowth");
      private SqlColumnBinder m_routingIds = new SqlColumnBinder("RoutingIds");
      private SqlColumnBinder m_shardIds = new SqlColumnBinder("ShardIds");
      private SqlColumnBinder m_indexName = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_esCluster = new SqlColumnBinder("EsCluster");
      private IEntityType m_entityType;

      protected override IndexingUnitDetails Bind()
      {
        int int32_1 = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader);
        int int32_2 = this.m_estimatedInitialDocCount.GetInt32((IDataReader) this.Reader);
        int int32_3 = this.m_estimatedDocCountGrowth.GetInt32((IDataReader) this.Reader);
        string routingIds = this.m_routingIds.GetString((IDataReader) this.Reader, true);
        string shardIds = this.m_shardIds.GetString((IDataReader) this.Reader, true);
        int estimatedInitialDocCount = int32_2;
        int estimatedDocCountGrowth = int32_3;
        IEntityType entityType = this.m_entityType;
        string indexName = this.m_indexName.GetString((IDataReader) this.Reader, false);
        string esClusterName = this.m_esCluster.GetString((IDataReader) this.Reader, false);
        Guid? tfsEntityId = new Guid?();
        DateTime? createdTimeStamp = new DateTime?();
        DateTime? lastModifiedTimeStamp = new DateTime?();
        return new IndexingUnitDetails(int32_1, routingIds, shardIds, estimatedInitialDocCount, -1, estimatedDocCountGrowth, -1, -1L, -1L, -1L, -1L, (string) null, entityType, indexName, esClusterName, tfsEntityId, createdTimeStamp: createdTimeStamp, lastModifiedTimeStamp: lastModifiedTimeStamp);
      }

      public IndexingUnitDetailsInRangeColumns(IEntityType entityType) => this.m_entityType = entityType;
    }
  }
}
