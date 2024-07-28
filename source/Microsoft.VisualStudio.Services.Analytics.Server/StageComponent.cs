// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Transform;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class StageComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[69]
    {
      (IComponentCreator) new ComponentCreator<StageComponent>(1, true),
      (IComponentCreator) new ComponentCreator<StageComponent>(2),
      (IComponentCreator) new ComponentCreator<StageComponent3>(3),
      (IComponentCreator) new ComponentCreator<StageComponent3>(4),
      (IComponentCreator) new ComponentCreator<StageComponent3>(5),
      (IComponentCreator) new ComponentCreator<StageComponent10>(6),
      (IComponentCreator) new ComponentCreator<StageComponent10>(7),
      (IComponentCreator) new ComponentCreator<StageComponent10>(8),
      (IComponentCreator) new ComponentCreator<StageComponent10>(9),
      (IComponentCreator) new ComponentCreator<StageComponent10>(10),
      (IComponentCreator) new ComponentCreator<StageComponent10>(11),
      (IComponentCreator) new ComponentCreator<StageComponent12>(12),
      (IComponentCreator) new ComponentCreator<StageComponent12>(13),
      (IComponentCreator) new ComponentCreator<StageComponent12>(14),
      (IComponentCreator) new ComponentCreator<StageComponent12>(15),
      (IComponentCreator) new ComponentCreator<StageComponent12>(16),
      (IComponentCreator) new ComponentCreator<StageComponent12>(17),
      (IComponentCreator) new ComponentCreator<StageComponent18>(18),
      (IComponentCreator) new ComponentCreator<StageComponent19>(19),
      (IComponentCreator) new ComponentCreator<StageComponent19>(20),
      (IComponentCreator) new ComponentCreator<StageComponent21>(21),
      (IComponentCreator) new ComponentCreator<StageComponent21>(22),
      (IComponentCreator) new ComponentCreator<StageComponent23>(23),
      (IComponentCreator) new ComponentCreator<StageComponent23>(24),
      (IComponentCreator) new ComponentCreator<StageComponent25>(25),
      (IComponentCreator) new ComponentCreator<StageComponent26>(26),
      (IComponentCreator) new ComponentCreator<StageComponent26>(27),
      (IComponentCreator) new ComponentCreator<StageComponent26>(28),
      (IComponentCreator) new ComponentCreator<StageComponent29>(29),
      (IComponentCreator) new ComponentCreator<StageComponent29>(30),
      (IComponentCreator) new ComponentCreator<StageComponent29>(31),
      (IComponentCreator) new ComponentCreator<StageComponent32>(32),
      (IComponentCreator) new ComponentCreator<StageComponent32>(33),
      (IComponentCreator) new ComponentCreator<StageComponent32>(34),
      (IComponentCreator) new ComponentCreator<StageComponent32>(35),
      (IComponentCreator) new ComponentCreator<StageComponent32>(36),
      (IComponentCreator) new ComponentCreator<StageComponent32>(37),
      (IComponentCreator) new ComponentCreator<StageComponent32>(38),
      (IComponentCreator) new ComponentCreator<StageComponent32>(39),
      (IComponentCreator) new ComponentCreator<StageComponent33>(40),
      (IComponentCreator) new ComponentCreator<StageComponent33>(41),
      (IComponentCreator) new ComponentCreator<StageComponent33>(42),
      (IComponentCreator) new ComponentCreator<StageComponent43>(43),
      (IComponentCreator) new ComponentCreator<StageComponent43>(44),
      (IComponentCreator) new ComponentCreator<StageComponent43>(45),
      (IComponentCreator) new ComponentCreator<StageComponent43>(46),
      (IComponentCreator) new ComponentCreator<StageComponent47>(47),
      (IComponentCreator) new ComponentCreator<StageComponent47>(48),
      (IComponentCreator) new ComponentCreator<StageComponent47>(49),
      (IComponentCreator) new ComponentCreator<StageComponent47>(50),
      (IComponentCreator) new ComponentCreator<StageComponent47>(51),
      (IComponentCreator) new ComponentCreator<StageComponent47>(52),
      (IComponentCreator) new ComponentCreator<StageComponent47>(53),
      (IComponentCreator) new ComponentCreator<StageComponent47>(54),
      (IComponentCreator) new ComponentCreator<StageComponent47>(55),
      (IComponentCreator) new ComponentCreator<StageComponent56>(56),
      (IComponentCreator) new ComponentCreator<StageComponent57>(57),
      (IComponentCreator) new ComponentCreator<StageComponent57>(58),
      (IComponentCreator) new ComponentCreator<StageComponent57>(59),
      (IComponentCreator) new ComponentCreator<StageComponent57>(60),
      (IComponentCreator) new ComponentCreator<StageComponent57>(61),
      (IComponentCreator) new ComponentCreator<StageComponent57>(62),
      (IComponentCreator) new ComponentCreator<StageComponent57>(63),
      (IComponentCreator) new ComponentCreator<StageComponent57>(64),
      (IComponentCreator) new ComponentCreator<StageComponent57>(65),
      (IComponentCreator) new ComponentCreator<StageComponent57>(66),
      (IComponentCreator) new ComponentCreator<StageComponent57>(67),
      (IComponentCreator) new ComponentCreator<StageComponent57>(68),
      (IComponentCreator) new ComponentCreator<StageComponent57>(69)
    }, "StageService");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public StageComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) StageComponent.s_sqlExceptionFactories;

    public IDictionary<int, SqlMetaData[]> GetStagingSchemas(string table) => this.GetStagingSchemas(StageTypeDefinitions.GetStageTypeDefinition(table).Metadata, this.GetTableStageVersions(table));

    private IDictionary<int, SqlMetaData[]> GetStagingSchemas(
      StageFieldMetaData[] metaData,
      IEnumerable<int> stageVersions)
    {
      return (IDictionary<int, SqlMetaData[]>) stageVersions.ToDictionary<int, int, SqlMetaData[]>((System.Func<int, int>) (v => v), (System.Func<int, SqlMetaData[]>) (v => ((IEnumerable<StageFieldMetaData>) metaData).Where<StageFieldMetaData>((System.Func<StageFieldMetaData, bool>) (m => m.MinStageVersion <= v && (m.MaxStageVersion ?? int.MaxValue) >= v)).Select<StageFieldMetaData, SqlMetaData>((System.Func<StageFieldMetaData, SqlMetaData>) (m => m.SqlMetaData)).ToArray<SqlMetaData>()));
    }

    private int GetExtendedSchemaStageVersion() => this.GetTableStageVersions(StageTypeDefinitions.ExtendedFieldTableDefinition.StageVersions).Single<int>();

    public SqlMetaData[] GetStagingExtendedSchema() => this.GetStagingSchemas(StageTypeDefinitions.ExtendedFieldTableDefinition.Metadata, (IEnumerable<int>) new int[1]
    {
      this.GetExtendedSchemaStageVersion()
    }).Select<KeyValuePair<int, SqlMetaData[]>, SqlMetaData[]>((System.Func<KeyValuePair<int, SqlMetaData[]>, SqlMetaData[]>) (s => s.Value)).Single<SqlMetaData[]>();

    protected SqlMetaData[] GetLatestStagingSchema(string table) => this.GetStagingSchemas(table).OrderByDescending<KeyValuePair<int, SqlMetaData[]>, int>((System.Func<KeyValuePair<int, SqlMetaData[]>, int>) (s => s.Key)).Select<KeyValuePair<int, SqlMetaData[]>, SqlMetaData[]>((System.Func<KeyValuePair<int, SqlMetaData[]>, SqlMetaData[]>) (s => s.Value)).FirstOrDefault<SqlMetaData[]>();

    public IEnumerable<int> GetTableStageVersions(string table) => this.GetTableStageVersions(StageTypeDefinitions.GetStageTypeDefinition(table).StageVersions);

    private IEnumerable<int> GetTableStageVersions(StageVersion[] stageVersions) => ((IEnumerable<StageVersion>) stageVersions).Where<StageVersion>((System.Func<StageVersion, bool>) (m => m.MinServiceVersion <= this.Version && (m.MaxServiceVersion ?? int.MaxValue) >= this.Version)).Select<StageVersion, int>((System.Func<StageVersion, int>) (m => m.Version));

    protected string GetTableTypeName(string table, int stageVersion)
    {
      StageTypeDefinitions.StageTypeDefinition stageTypeDefinition = StageTypeDefinitions.GetStageTypeDefinition(table);
      return this.GetTableTypeName(table, stageVersion, stageTypeDefinition.TypeNameVersions);
    }

    private string GetTableTypeName(string table, int stageVersion, StageSqlTableType[] typeNames) => ((IEnumerable<StageSqlTableType>) typeNames).Where<StageSqlTableType>((System.Func<StageSqlTableType, bool>) (t => t.StageVersion == stageVersion)).Select<StageSqlTableType, string>((System.Func<StageSqlTableType, string>) (t => t.SqlTableTypeName)).SingleOrDefault<string>() ?? throw new InvalidOperationException(AnalyticsResources.UNKNOWN_SQL_TABLE_TYPE((object) table, (object) stageVersion));

    protected string GetExtendedTypeName() => this.GetTableTypeName("ExtendedField", this.GetExtendedSchemaStageVersion(), StageTypeDefinitions.ExtendedFieldTableDefinition.TypeNameVersions);

    protected string GetMergeStoredProcedure(string table, int stageVersion)
    {
      StageTypeDefinitions.StageTypeDefinition stageTypeDefinition = StageTypeDefinitions.GetStageTypeDefinition(table);
      return this.GetMergeStoredProcedure(table, stageVersion, stageTypeDefinition.TypeNameVersions);
    }

    private string GetMergeStoredProcedure(
      string table,
      int stageVersion,
      StageSqlTableType[] typeNames)
    {
      return ((IEnumerable<StageSqlTableType>) typeNames).Where<StageSqlTableType>((System.Func<StageSqlTableType, bool>) (t => t.StageVersion == stageVersion)).Select<StageSqlTableType, string>((System.Func<StageSqlTableType, string>) (t => t.MergeSprocName)).SingleOrDefault<string>() ?? throw new InvalidOperationException(AnalyticsResources.UNKNOWN_SQL_TABLE_TYPE((object) table, (object) stageVersion));
    }

    protected string GetExportStoredProcedure(string table)
    {
      StageTypeDefinitions.StageTypeDefinition stageTypeDefinition = StageTypeDefinitions.GetStageTypeDefinition(table);
      return !string.IsNullOrEmpty(stageTypeDefinition.ExportSprocName) ? stageTypeDefinition.ExportSprocName : throw new ArgumentOutOfRangeException(AnalyticsResources.NO_EXPORT_SPROC((object) nameof (table)));
    }

    protected string GetCleanupStreamStoredProcedure(string table) => StageTypeDefinitions.GetStageTypeDefinition(table).CleanupStreamSprocName;

    protected string GetDbTableName(string table) => StageTypeDefinitions.GetStageTypeDefinition(table).DbTableName;

    protected string GetDbExtendedTableName(string table) => StageTypeDefinitions.GetStageTypeDefinition(table).DbExtendedTableName;

    protected string GetDbCustomTableNamePattern(string table) => StageTypeDefinitions.GetStageTypeDefinition(table).DbCustomTableNamePattern;

    public List<StreamInfo> GetProviderShardStreamInfos(string table) => this.GetShardStreamInfos(table, new int?(), new int?());

    public List<StreamInfo> GetProviderShardStreamInfos(string table, int providerShardId) => this.GetShardStreamInfos(table, new int?(providerShardId), new int?());

    public virtual List<StreamInfo> GetShardStreamInfos(
      string table,
      int? providerShardId,
      int? streamId)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetStreamInfos");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableInt("@providerShardId", providerShardId);
      this.BindNullableInt("@streamId", streamId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StreamInfo>((ObjectBinder<StreamInfo>) new StreamInfoColumns());
        return resultCollection.GetCurrent<StreamInfo>().Items;
      }
    }

    protected virtual void SetWatermark(
      string table,
      int providerShardId,
      int streamId,
      string watermark,
      int? contentVersion)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_SetWatermark");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      this.BindString("@watermark", watermark, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void CreateStream(string table, int providerShardId, bool enabled, int priority)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CreateStream");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindBoolean("@enabled", enabled);
      this.BindInt("@priority", priority);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateStream(
      string table,
      int providerShardId,
      int streamId,
      bool enabled,
      bool current,
      int? priority,
      bool disposed,
      bool? keysOnly)
    {
      if (keysOnly.GetValueOrDefault())
        throw new NotSupportedException(AnalyticsResources.KEYS_ONLY_NOT_SUPPORTED());
      this.PrepareStoredProcedure("AnalyticsInternal.prc_UpdateStream");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      this.BindBoolean("@enabled", enabled);
      this.BindBoolean("@current", current);
      this.BindNullableInt("@priority", priority);
      this.ExecuteNonQuery();
    }

    public virtual void RecordTableOperation(
      string table,
      int? providerShardId,
      int? streamId,
      string tableOperation)
    {
      if (!((IEnumerable<string>) StageTableNames.All).Contains<string>(table) && !((IEnumerable<TransformDefinition>) TransformDefinitions.All).Select<TransformDefinition, string>((System.Func<TransformDefinition, string>) (t => t.TriggerTable)).Contains<string>(table))
        throw new ArgumentException(AnalyticsResources.UNKNOWN_TABLE_NAME((object) (table ?? " (null) ")));
      this.PrepareStoredProcedure("AnalyticsInternal.prc_RecordTableOperation");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindNullableInt("@providerShardId", providerShardId);
      this.BindNullableInt("@streamId", streamId);
      this.BindString("@operation", tableOperation, 10, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual void StageRecords(
      string table,
      int providerShardId,
      int streamId,
      IEnumerable<SqlDataRecord> records,
      IEnumerable<SqlDataRecord> extendedFields,
      bool replace,
      bool keysOnly,
      bool tableLoading,
      string watermark,
      int stageVersion,
      int? contentVersion,
      DateTime? providerSyncDate,
      out bool terminate)
    {
      terminate = false;
      if (keysOnly)
        throw new StageKeysOnlyNotSupportedException(table, providerShardId, streamId);
      string mergeStoredProcedure = this.GetMergeStoredProcedure(table, stageVersion);
      try
      {
        this.BeginTransaction(IsolationLevel.ReadCommitted);
        this.SetWatermark(table, providerShardId, streamId, watermark, contentVersion);
        this.PrepareStoredProcedure(mergeStoredProcedure);
        this.BindInt("@providerShardId", providerShardId);
        this.BindInt("@streamId", streamId);
        this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@operation", replace ? nameof (replace) : "merge", 10, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindBoolean("@replace", replace);
        this.BindTable("@records", this.GetTableTypeName(table, stageVersion), records);
        this.BindTable("@extendedFields", this.GetExtendedTypeName(), extendedFields);
        this.ExecuteNonQuery();
        this.CommitTransaction();
      }
      catch
      {
        this.RollbackTransaction();
        throw;
      }
    }

    public virtual void InvalidateProviderShard(
      string table,
      int providerShardId,
      IEnumerable<string> fieldNames = null,
      bool disableCurrentStream = false,
      bool keysOnly = false)
    {
      if (keysOnly)
        throw new NotSupportedException(AnalyticsResources.KEYS_ONLY_NOT_SUPPORTED());
      SqlMetaData[] latestStagingSchema = this.GetLatestStagingSchema(table);
      if (fieldNames != null && !((IEnumerable<SqlMetaData>) latestStagingSchema).Select<SqlMetaData, string>((System.Func<SqlMetaData, string>) (meta => meta.Name)).Intersect<string>(fieldNames, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase).Any<string>())
        return;
      this.BeginTransaction(IsolationLevel.ReadCommitted);
      List<StreamInfo> shardStreamInfos = this.GetProviderShardStreamInfos(table, providerShardId);
      foreach (StreamInfo streamInfo in shardStreamInfos.Where<StreamInfo>((System.Func<StreamInfo, bool>) (strm => !strm.Disposed)))
        this.UpdateStream(table, providerShardId, streamInfo.StreamId, streamInfo.Current, streamInfo.Current, new int?(), !streamInfo.Current, new bool?());
      if (shardStreamInfos.Any<StreamInfo>())
        this.CreateStream(table, providerShardId, true, 2);
      this.CommitTransaction();
    }

    public virtual void InvalidateTable(string table)
    {
      this.GetLatestStagingSchema(table);
      foreach (int providerShardId in this.GetProviderShardStreamInfos(table).Select<StreamInfo, int>((System.Func<StreamInfo, int>) (strm => strm.ProviderShardId)).Distinct<int>())
        this.InvalidateProviderShard(table, providerShardId);
    }

    public virtual void InvalidateAllTables() => throw new NotImplementedException(AnalyticsResources.METHOD_INTRODUCED_IN_VERSION((object) 3));

    public virtual void InvalidateTableAllPartitions(string table) => throw new NotImplementedException(AnalyticsResources.METHOD_INTRODUCED_IN_VERSION((object) 23));

    public virtual CleanupStreamResult CleanupStream(
      string table,
      int providerShardId,
      int streamId,
      int commandTimeoutSeconds)
    {
      throw new NotImplementedException(AnalyticsResources.METHOD_INTRODUCED_IN_VERSION((object) 17));
    }

    public virtual CleanupStreamResult CleanupOrphanedStreamData(string table, int commandTimeout) => new CleanupStreamResult()
    {
      Unsupported = true,
      Complete = true
    };

    public virtual CleanupStreamResult CleanupBatches(int retainBatchHistoryDays) => new CleanupStreamResult()
    {
      Unsupported = true,
      Complete = true
    };

    public virtual void SetStagingTableMaintenance(
      string table,
      bool enable,
      string maintenanceReason)
    {
      throw new NotImplementedException(AnalyticsResources.METHOD_INTRODUCED_IN_VERSION((object) 19));
    }

    public virtual string ExportRecords(string table, long? batchId) => throw new NotImplementedException(AnalyticsResources.METHOD_INTRODUCED_IN_VERSION((object) 28));
  }
}
