// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV11
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
  public class ItemLevelFailuresComponentV11 : ItemLevelFailuresComponentV10
  {
    private static readonly SqlMetaData[] s_fileFailureRecordTableV3 = new SqlMetaData[6]
    {
      new SqlMetaData("FilePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("Stage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Reason", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MetaData", SqlDbType.Xml),
      new SqlMetaData("RejectionCode", SqlDbType.TinyInt)
    };

    public ItemLevelFailuresComponentV11()
    {
    }

    internal ItemLevelFailuresComponentV11(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithRejectionCodes(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      List<RejectionCode> rejectionCodesList,
      long startingId = 0)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      int val1 = topCount > -1 ? topCount : throw new ArgumentException(nameof (topCount));
      List<ItemLevelFailureRecord> withRejectionCodes = new List<ItemLevelFailureRecord>();
      for (int index = 0; index < topCount; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_RetrieveFailedItemsWithRejectionCodes");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        this.BindTinyIntIds("@rejectionCodes", (IEnumerable<byte>) rejectionCodesList.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
        List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponentV11.FailedItemRecordColumnsWithRejectionCode());
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
        withRejectionCodes.AddRange((IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList);
        if (val1 > 0 && levelFailureRecordList.Count >= parameterValue)
        {
          stopwatch.Stop();
          startingId = levelFailureRecordList.Max<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, long>) (x => x.Id)) + 1L;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) "ItemLevelFailuresComponent", (object) nameof (GetFailedItemsWithRejectionCodes), (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<ItemLevelFailureRecord>) withRejectionCodes;
    }

    public override IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithoutRejectionCodes(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      List<RejectionCode> rejectionCodesList,
      long startingId = 0)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      int val1 = topCount > -1 ? topCount : throw new ArgumentException(nameof (topCount));
      List<ItemLevelFailureRecord> withoutRejectionCodes = new List<ItemLevelFailureRecord>();
      for (int index = 0; index < topCount; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_RetrieveFailedItemsWithoutRejectionCodes");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        this.BindTinyIntIds("@rejectionCodes", (IEnumerable<byte>) rejectionCodesList.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
        List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponentV11.FailedItemRecordColumnsWithRejectionCode());
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
        withoutRejectionCodes.AddRange((IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList);
        if (val1 > 0 && levelFailureRecordList.Count >= parameterValue)
        {
          stopwatch.Stop();
          startingId = levelFailureRecordList.Max<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, long>) (x => x.Id)) + 1L;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) "ItemLevelFailuresComponent", (object) nameof (GetFailedItemsWithoutRejectionCodes), (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<ItemLevelFailureRecord>) withoutRejectionCodes;
    }

    public override void AddItemFailureRecords(
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
        this.BindFileLevelFailureRecordsParametersContainingRejectionCode("@itemLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) range);
        this.ExecuteNonQuery(false);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.AddFileFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      }
    }

    public override int MergeItemFailureRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> records,
      int maxAttemptCount)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<ItemLevelFailureRecord>())
        return 0;
      int num = 0;
      if (indexingUnit.EntityType.Name == "WorkItem")
      {
        num = base.MergeItemFailureRecords(indexingUnit, records, maxAttemptCount);
      }
      else
      {
        if (!(indexingUnit.EntityType.Name == "Code") && !(indexingUnit.EntityType.Name == "Wiki"))
          throw new NotSupportedException(string.Format("Entity type [{0}] is not supported.", (object) indexingUnit.EntityType));
        List<ItemLevelFailureRecord> list = records.ToList<ItemLevelFailureRecord>();
        int count1 = list.Count;
        int val1 = count1;
        for (int index = 0; index < count1; index += 500)
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          int count2 = Math.Min(val1, 500);
          IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count2);
          val1 -= count2;
          if (range.Count > 0)
          {
            this.SanitizeBranchNames(range);
            IDictionary<string, ItemLevelFailureRecord> failureRecordsMap = this.GetFileToFailureRecordsMap((IEnumerable<ItemLevelFailureRecord>) range);
            IDictionary<string, ItemLevelFailureRecord> failedItemsFromSql = this.GetExistingFailedItemsFromSQL(indexingUnit, (IList<string>) failureRecordsMap.Values.Select<ItemLevelFailureRecord, string>((System.Func<ItemLevelFailureRecord, string>) (x => x.Item)).ToList<string>());
            this.UpdateRecords(failureRecordsMap, failedItemsFromSql);
            this.PrepareStoredProcedure("Search.prc_UpsertFileLevelFailureRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindFileLevelFailureRecordsParametersContainingRejectionCode("@fileLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) failureRecordsMap.Values.ToList<ItemLevelFailureRecord>());
            this.ExecuteNonQuery(false);
            num += failureRecordsMap.Values.Where<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, bool>) (s => s.AttemptCount >= maxAttemptCount)).Count<ItemLevelFailureRecord>();
            stopwatch.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeItemFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
          }
          else
            break;
        }
      }
      return num;
    }

    protected SqlParameter BindFileLevelFailureRecordsParametersContainingRejectionCode(
      string parameterName,
      IEnumerable<ItemLevelFailureRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemLevelFailureRecord>();
      System.Func<ItemLevelFailureRecord, SqlDataRecord> selector = (System.Func<ItemLevelFailureRecord, SqlDataRecord>) (failureRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponentV11.s_fileFailureRecordTableV3);
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
        sqlDataRecord.SetSqlByte(5, (SqlByte) (byte) failureRecord.RejectionCode);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_FileFailureRecordV3", rows.Select<ItemLevelFailureRecord, SqlDataRecord>(selector));
    }

    protected class FailedItemRecordColumnsWithRejectionCode : ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      private SqlColumnBinder m_rejectionCode = new SqlColumnBinder("RejectionCode");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        Item = this.m_item.GetString((IDataReader) this.Reader, false),
        Metadata = (FailureMetadata) SQLTable<ItemLevelFailureRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (FailureMetadata)),
        RejectionCode = (RejectionCode) this.m_rejectionCode.GetByte((IDataReader) this.Reader)
      };
    }
  }
}
