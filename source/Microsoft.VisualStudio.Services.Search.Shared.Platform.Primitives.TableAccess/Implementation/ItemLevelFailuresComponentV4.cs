// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV4
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV4 : ItemLevelFailuresComponentV3
  {
    private static readonly SqlMetaData[] s_workItemFailureRecordTable = new SqlMetaData[4]
    {
      new SqlMetaData("WorkItemId", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("Stage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Reason", SqlDbType.NVarChar, -1L)
    };

    public ItemLevelFailuresComponentV4()
    {
    }

    internal ItemLevelFailuresComponentV4(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override void RemoveSuccessfullyIndexedItemsFromFailedRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> successfullyIndexedRecords)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (successfullyIndexedRecords == null || !successfullyIndexedRecords.Any<ItemLevelFailureRecord>())
        return;
      if (indexingUnit.EntityType.Name == "Code" || indexingUnit.EntityType.Name == "Wiki")
      {
        foreach (KeyValuePair<string, List<ItemLevelFailureRecord>> levelFailureRecords in (IEnumerable<KeyValuePair<string, List<ItemLevelFailureRecord>>>) this.GetBranchToItemLevelFailureRecordsMap(successfullyIndexedRecords))
        {
          List<ItemLevelFailureRecord> source = levelFailureRecords.Value;
          int num = source.Count<ItemLevelFailureRecord>();
          int val1 = num;
          for (int index = 0; index < num; index += 500)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int count = Math.Min(val1, 500);
            IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) source.GetRange(index, count);
            val1 -= count;
            if (range.Count > 0)
            {
              this.PrepareStoredProcedure("Search.prc_RemoveSuccessfullyIndexedFilesFromFailedRecords");
              this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
              this.BindFileLevelFailureRecordsParameter("@successfullyIndexedRecords", (IEnumerable<ItemLevelFailureRecord>) range);
              this.ExecuteNonQuery(false);
              stopwatch.Stop();
              Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeFailedRecordsWtihSuccessfullyIndexedItems took {0}ms", (object) stopwatch.ElapsedMilliseconds));
            }
            else
              break;
          }
        }
      }
      else
      {
        if (!(indexingUnit.EntityType.Name == "WorkItem"))
          throw new NotSupportedException("Entity type [" + indexingUnit.EntityType.Name + "] is not supported.");
        List<ItemLevelFailureRecord> list = successfullyIndexedRecords.ToList<ItemLevelFailureRecord>();
        int count1 = list.Count;
        int val1 = count1;
        for (int index = 0; index < count1; index += 500)
        {
          int count2 = Math.Min(val1, 500);
          IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count2);
          val1 -= count2;
          if (range.Count <= 0)
            break;
          this.PrepareStoredProcedure("Search.prc_RemoveSuccessfullyIndexedWorkItemsFromFailedRecords");
          this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
          this.BindWorkItemLevelFailureRecordsParameter("@successfullyIndexedRecords", (IEnumerable<ItemLevelFailureRecord>) range);
          this.ExecuteNonQuery(false);
        }
      }
    }

    protected virtual SqlParameter BindWorkItemLevelFailureRecordsParameter(
      string parameterName,
      IEnumerable<ItemLevelFailureRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemLevelFailureRecord>();
      System.Func<ItemLevelFailureRecord, SqlDataRecord> selector = (System.Func<ItemLevelFailureRecord, SqlDataRecord>) (failureRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponentV4.s_workItemFailureRecordTable);
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
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_WorkItemFailureRecord", rows.Select<ItemLevelFailureRecord, SqlDataRecord>(selector));
    }
  }
}
