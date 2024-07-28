// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponent : SQLTable<ItemLevelFailureRecord>
  {
    private const string ServiceName = "Search_ItemLevelFailures";
    protected internal const int BatchCountForSelectRecords = 500;
    protected internal const int BatchCountForInsertRecords = 500;
    protected internal const int BatchCountForDeleteRecords = 500;
    protected internal const int TopCountForFailedItems = 500;
    protected internal StringComparer FilePathComparer = StringComparer.OrdinalIgnoreCase;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[15]
    {
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV2>(2, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV3>(3, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV4>(4, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV5>(5, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV6>(6, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV7>(7, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV8>(8, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV9>(9, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV10>(10, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV11>(11, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV12>(12, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV13>(13, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV14>(14, false),
      (IComponentCreator) new ComponentCreator<ItemLevelFailuresComponentV15>(15, false)
    }, "Search_ItemLevelFailures");
    protected const string s_traceArea = "Indexing Pipeline";
    protected const string s_traceLayer = "FileLevelFailuresComponent";
    private static readonly SqlMetaData[] s_itemFailureRecordTable = new SqlMetaData[5]
    {
      new SqlMetaData("Item1", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Item2", SqlDbType.Int),
      new SqlMetaData("Item3", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Item4", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Item5", SqlDbType.Xml)
    };
    private static readonly SqlMetaData[] s_fileFailureRecordTable = new SqlMetaData[5]
    {
      new SqlMetaData("FilePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("Stage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Reason", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Branch", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_fileFailureRecordTableV2 = new SqlMetaData[5]
    {
      new SqlMetaData("FilePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("Stage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Reason", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MetaData", SqlDbType.Xml)
    };

    public ItemLevelFailuresComponent()
      : base()
    {
    }

    internal ItemLevelFailuresComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public virtual void AddItemFailureRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> records)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<ItemLevelFailureRecord>())
        return;
      List<ItemLevelFailureRecord> list = records.ToList<ItemLevelFailureRecord>();
      int count1 = list.Count;
      int val1 = count1;
      for (int index = 0; index < count1; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count2 = Math.Min(val1, 500);
        IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count2);
        val1 -= count2;
        if (range.Count <= 0)
          break;
        this.PrepareStoredProcedure("Search.prc_AddItemLevelFailures");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindItemLevelFailureRecordsParameter("@itemLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) range);
        this.ExecuteNonQuery(false);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.AddFileFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      }
    }

    public virtual int MergeItemFailureRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> records,
      int maxAttemptCount)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetFailureRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      int count)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      int val1 = count > -1 ? count : throw new ArgumentException(nameof (count));
      List<ItemLevelFailureRecord> source = new List<ItemLevelFailureRecord>();
      for (int index = 0; index < count; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_RetrieveItemLevelFailures");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponent.ItemFailureRecordColumns());
          ObjectBinder<ItemLevelFailureRecord> current = resultCollection.GetCurrent<ItemLevelFailureRecord>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                levelFailureRecordList.AddRange((IEnumerable<ItemLevelFailureRecord>) current.Items);
            }
          }
        }
        val1 -= parameterValue;
        source.AddRange((IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList);
        if (val1 > 0 && levelFailureRecordList.Any<ItemLevelFailureRecord>())
        {
          startingId = source.Max<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, long>) (x => x.Id)) + 1L;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) nameof (ItemLevelFailuresComponent), (object) nameof (GetFailureRecords), (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<ItemLevelFailureRecord>) source;
    }

    public virtual IDictionary<int, int> GetCountOfRecordsByIndexingUnit()
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfFailures");
      IDictionary<int, int> recordsByIndexingUnit = (IDictionary<int, int>) new Dictionary<int, int>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<int, int>>((ObjectBinder<Tuple<int, int>>) new ItemLevelFailuresComponent.CountOfItemFailureRecordsByIndexingUnit());
        ObjectBinder<Tuple<int, int>> current = resultCollection.GetCurrent<Tuple<int, int>>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
            {
              foreach (Tuple<int, int> tuple in current.Items)
                recordsByIndexingUnit.Add(tuple.Item1, tuple.Item2);
            }
          }
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsByIndexingUnit took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return recordsByIndexingUnit;
    }

    public virtual IDictionary<int, int> GetCountOfRecordsByIndexingUnit(
      IEntityType entityType,
      int maxAttemptCount)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetFailedItems(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId = 0)
    {
      throw new NotImplementedException();
    }

    public virtual IList<ItemLevelFailureRecord> GetFailedItemIds(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId = 0)
    {
      throw new NotImplementedException();
    }

    public virtual int GetCountOfRecordsForIndexingUnit(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit) => throw new NotImplementedException();

    public virtual void RemoveSuccessfullyIndexedItemsFromFailedRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> successfullyIndexedRecords)
    {
      throw new NotImplementedException();
    }

    public virtual int GetCountOfRecordsWithMaxAttemptCount(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int maxAttemptCount)
    {
      return 0;
    }

    public virtual IDictionary<int, int> GetCountOfRecordsByIndexingUnit(int maxAttemptCount) => throw new NotImplementedException();

    public virtual int GetCountOfFailedItemsModifiedBefore(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits,
      int hoursToLookBack,
      int maxAttemptCount)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
      int indexingUnitId,
      int maxAttemptCount,
      int count = 500)
    {
      if (indexingUnitId <= 0)
        throw new ArgumentException(nameof (indexingUnitId));
      if (maxAttemptCount <= 0)
        throw new ArgumentException(nameof (maxAttemptCount));
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<ItemLevelFailureRecord> withMaxAttemptCount = new List<ItemLevelFailureRecord>();
      this.PrepareStoredProcedure("Search.prc_GetItemsWithMaxAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindInt("@attemptCount", maxAttemptCount);
      this.BindInt("@count", count);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponent.ItemFailureRecordAttemptCountMetadataColumns());
        ObjectBinder<ItemLevelFailureRecord> current = resultCollection.GetCurrent<ItemLevelFailureRecord>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
              withMaxAttemptCount.AddRange((IEnumerable<ItemLevelFailureRecord>) current.Items);
          }
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetItemsWithMaxAttemptCount took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return (IEnumerable<ItemLevelFailureRecord>) withMaxAttemptCount;
    }

    public virtual void DeleteRecordsById(int indexingUnitId, IList<long> idsToDelete)
    {
      if (indexingUnitId <= 0)
        throw new ArgumentException(nameof (indexingUnitId));
      if (idsToDelete == null)
        throw new ArgumentNullException(nameof (idsToDelete));
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<long> list = idsToDelete.ToList<long>();
      int num = list.Count<long>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        int count = Math.Min(val1, 500);
        IList<long> range = (IList<long>) list.GetRange(index, count);
        val1 -= count;
        if (range.Count <= 0)
          break;
        this.PrepareStoredProcedure("Search.prc_DeleteFailureRecordsById");
        this.BindInt("@indexingUnitId", indexingUnitId);
        this.BindIds("@idList", (IEnumerable<long>) range);
        this.ExecuteNonQuery(false);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.DeleteRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      }
    }

    public virtual void DeleteRecordsForIndexingUnit(int indexingUnitId)
    {
      if (indexingUnitId <= 0)
        throw new ArgumentException(nameof (indexingUnitId));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_DeleteItemLevelFailureRecordsForIndexingUnit");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.DeleteRecordsByIndexingUnit took {0}ms", (object) stopwatch.ElapsedMilliseconds));
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithRejectionCodes(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      List<RejectionCode> rejectionCodesList,
      long startingId = 0)
    {
      return (IEnumerable<ItemLevelFailureRecord>) new List<ItemLevelFailureRecord>();
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithoutRejectionCodes(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      List<RejectionCode> rejectionCodesList,
      long startingId = 0)
    {
      return this.GetFailedItems(indexingUnit, topCount, startingId);
    }

    public virtual int GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(
      int indexingUnitId,
      List<RejectionCode> rejectionCodesListToExclude,
      int hoursToLookBack)
    {
      return 0;
    }

    public virtual IDictionary<string, IDictionary<int, int>> GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(
      List<RejectionCode> rejectionCodesListToExclude,
      int hoursToLookBack)
    {
      return (IDictionary<string, IDictionary<int, int>>) new Dictionary<string, IDictionary<int, int>>();
    }

    public virtual List<ItemLevelFailureRecord> GetFailedItemsForABranch(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      string branchName,
      int topCount,
      long startingId = 0)
    {
      return new List<ItemLevelFailureRecord>();
    }

    protected virtual SqlParameter BindFileLevelFailureRecordsParameter(
      string parameterName,
      IEnumerable<ItemLevelFailureRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemLevelFailureRecord>();
      System.Func<ItemLevelFailureRecord, SqlDataRecord> selector = (System.Func<ItemLevelFailureRecord, SqlDataRecord>) (failureRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponent.s_fileFailureRecordTable);
        sqlDataRecord.SetString(0, failureRecord.Item);
        sqlDataRecord.SetInt32(1, failureRecord.AttemptCount);
        if (string.IsNullOrWhiteSpace(failureRecord.Stage))
          sqlDataRecord.SetDBNull(2);
        else
          sqlDataRecord.SetString(2, failureRecord.Stage);
        if (string.IsNullOrWhiteSpace(failureRecord.Reason))
          sqlDataRecord.SetDBNull(3);
        else
          sqlDataRecord.SetString(3, failureRecord.Reason);
        FileFailureMetadata metadata = failureRecord.Metadata as FileFailureMetadata;
        string str = string.Empty;
        if (metadata?.Branches != null && metadata.Branches.Count == 1)
          str = string.IsNullOrWhiteSpace(metadata.Branches[0]) ? string.Empty : metadata.Branches[0];
        sqlDataRecord.SetString(4, str);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_FileFailureRecord", rows.Select<ItemLevelFailureRecord, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindFileLevelFailureRecordsParameterContainingListOfBranches(
      string parameterName,
      IEnumerable<ItemLevelFailureRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemLevelFailureRecord>();
      System.Func<ItemLevelFailureRecord, SqlDataRecord> selector = (System.Func<ItemLevelFailureRecord, SqlDataRecord>) (failureRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponent.s_fileFailureRecordTableV2);
        sqlDataRecord.SetString(0, failureRecord.Item);
        sqlDataRecord.SetInt32(1, failureRecord.AttemptCount);
        if (string.IsNullOrWhiteSpace(failureRecord.Stage))
          sqlDataRecord.SetDBNull(2);
        else
          sqlDataRecord.SetString(2, failureRecord.Stage);
        if (string.IsNullOrWhiteSpace(failureRecord.Reason))
          sqlDataRecord.SetDBNull(3);
        else
          sqlDataRecord.SetString(3, failureRecord.Reason);
        if (failureRecord.Metadata == null)
          sqlDataRecord.SetDBNull(4);
        else
          sqlDataRecord.SetSqlXml(4, new SqlXml(SQLTable<ItemLevelFailureRecord>.ToStream((object) failureRecord.Metadata, typeof (FailureMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_FileFailureRecordV2", rows.Select<ItemLevelFailureRecord, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindItemLevelFailureRecordsParameter(
      string parameterName,
      IEnumerable<ItemLevelFailureRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemLevelFailureRecord>();
      System.Func<ItemLevelFailureRecord, SqlDataRecord> selector = (System.Func<ItemLevelFailureRecord, SqlDataRecord>) (failureRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponent.s_itemFailureRecordTable);
        sqlDataRecord.SetString(0, failureRecord.Item);
        sqlDataRecord.SetInt32(1, failureRecord.AttemptCount);
        if (string.IsNullOrWhiteSpace(failureRecord.Stage))
          sqlDataRecord.SetDBNull(2);
        else
          sqlDataRecord.SetString(2, failureRecord.Stage);
        if (string.IsNullOrWhiteSpace(failureRecord.Reason))
          sqlDataRecord.SetDBNull(3);
        else
          sqlDataRecord.SetString(3, failureRecord.Reason);
        if (failureRecord.Metadata == null)
          sqlDataRecord.SetDBNull(4);
        else
          sqlDataRecord.SetSqlXml(4, new SqlXml(SQLTable<ItemLevelFailureRecord>.ToStream((object) failureRecord.Metadata, typeof (FailureMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ItemFailureRecord", rows.Select<ItemLevelFailureRecord, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIds(string parameterName, IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SQLTable<ItemLevelFailureRecord>.BigIntTable);
        sqlDataRecord.SetValue(0, (object) entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_Int64Descriptor", rows.Select<long, SqlDataRecord>(selector));
    }

    protected SqlParameter BindTinyIntIds(string parameterName, IEnumerable<byte> rows)
    {
      rows = rows ?? Enumerable.Empty<byte>();
      System.Func<byte, SqlDataRecord> selector = (System.Func<byte, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SQLTable<ItemLevelFailureRecord>.TinyIntTable);
        sqlDataRecord.SetValue(0, (object) entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TinyIntTable", rows.Select<byte, SqlDataRecord>(selector));
    }

    internal IDictionary<string, List<ItemLevelFailureRecord>> GetBranchToItemLevelFailureRecordsMap(
      IEnumerable<ItemLevelFailureRecord> records)
    {
      IDictionary<string, List<ItemLevelFailureRecord>> failureRecordsMap = (IDictionary<string, List<ItemLevelFailureRecord>>) new Dictionary<string, List<ItemLevelFailureRecord>>();
      foreach (ItemLevelFailureRecord record in records)
      {
        Branches branches1 = record.Metadata is FileFailureMetadata metadata ? metadata.Branches : (Branches) null;
        if (branches1 != null && branches1.Count >= 1)
        {
          foreach (string str in (List<string>) branches1)
          {
            string key = string.IsNullOrWhiteSpace(str) ? string.Empty : str;
            List<ItemLevelFailureRecord> levelFailureRecordList1;
            if (!failureRecordsMap.TryGetValue(key, out levelFailureRecordList1))
            {
              levelFailureRecordList1 = new List<ItemLevelFailureRecord>();
              failureRecordsMap[key] = levelFailureRecordList1;
            }
            List<ItemLevelFailureRecord> levelFailureRecordList2 = levelFailureRecordList1;
            ItemLevelFailureRecord levelFailureRecord = new ItemLevelFailureRecord();
            levelFailureRecord.AttemptCount = record.AttemptCount;
            levelFailureRecord.Stage = record.Stage;
            levelFailureRecord.Item = record.Item;
            levelFailureRecord.Reason = record.Reason;
            FileFailureMetadata fileFailureMetadata = new FileFailureMetadata();
            Branches branches2 = new Branches();
            branches2.Add(key);
            fileFailureMetadata.Branches = branches2;
            levelFailureRecord.Metadata = (FailureMetadata) fileFailureMetadata;
            levelFailureRecordList2.Add(levelFailureRecord);
          }
        }
      }
      return failureRecordsMap;
    }

    internal IDictionary<string, ItemLevelFailureRecord> GetFileToFailureRecordsMap(
      IEnumerable<ItemLevelFailureRecord> records)
    {
      IDictionary<string, ItemLevelFailureRecord> failureRecordsMap = (IDictionary<string, ItemLevelFailureRecord>) new Dictionary<string, ItemLevelFailureRecord>((IEqualityComparer<string>) this.FilePathComparer);
      foreach (ItemLevelFailureRecord record in records)
      {
        if (!failureRecordsMap.TryGetValue(record.Item, out ItemLevelFailureRecord _))
          failureRecordsMap[record.Item] = record;
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082628, "Indexing Pipeline", "FileLevelFailuresComponent", "ItemLevelFailuresComponent received duplicate entry -> " + record.ToString());
      }
      return failureRecordsMap;
    }

    protected class ItemFailureRecordColumns : ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_stage = new SqlColumnBinder("Stage");
      private SqlColumnBinder m_reason = new SqlColumnBinder("Reason");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      private SqlColumnBinder m_rejectionCode = new SqlColumnBinder("RejectionCode");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        Item = this.m_item.GetString((IDataReader) this.Reader, false),
        AttemptCount = this.m_attemptCount.GetInt32((IDataReader) this.Reader),
        Stage = this.m_stage.GetString((IDataReader) this.Reader, true),
        Reason = this.m_reason.GetString((IDataReader) this.Reader, true),
        Metadata = (FailureMetadata) SQLTable<ItemLevelFailureRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (FailureMetadata)),
        RejectionCode = (RejectionCode) this.m_rejectionCode.GetByte((IDataReader) this.Reader)
      };
    }

    protected class FailedItemRecordColumns : ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        Item = this.m_item.GetString((IDataReader) this.Reader, false),
        Metadata = (FailureMetadata) SQLTable<ItemLevelFailureRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (FailureMetadata))
      };
    }

    protected class FailedItemRecordColumnsV2 : ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Item = this.m_item.GetString((IDataReader) this.Reader, false),
        AttemptCount = this.m_attemptCount.GetInt32((IDataReader) this.Reader),
        Metadata = (FailureMetadata) SQLTable<ItemLevelFailureRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (FailureMetadata))
      };
    }

    protected class ItemFailureRecordAttemptCountMetadataColumns : 
      ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        Item = this.m_item.GetString((IDataReader) this.Reader, false),
        AttemptCount = this.m_attemptCount.GetInt32((IDataReader) this.Reader),
        Metadata = (FailureMetadata) SQLTable<ItemLevelFailureRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (FailureMetadata))
      };
    }

    protected class CountOfItemFailureRecordsByIndexingUnit : ObjectBinder<Tuple<int, int>>
    {
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_count = new SqlColumnBinder("Count");

      protected override Tuple<int, int> Bind() => new Tuple<int, int>(this.m_indexingUnitId.GetInt32((IDataReader) this.Reader), this.m_count.GetInt32((IDataReader) this.Reader));
    }

    protected class CountOfRecordsGroupedByEntityAndIndexingUnit : 
      ObjectBinder<Tuple<string, int, int>>
    {
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_count = new SqlColumnBinder("Count");

      protected override Tuple<string, int, int> Bind() => new Tuple<string, int, int>(this.m_entityType.GetString((IDataReader) this.Reader, false), this.m_indexingUnitId.GetInt32((IDataReader) this.Reader), this.m_count.GetInt32((IDataReader) this.Reader));
    }
  }
}
