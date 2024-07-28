// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV7
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV7 : ItemLevelFailuresComponentV6
  {
    private static readonly SqlMetaData[] s_int32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.Int)
    };

    public ItemLevelFailuresComponentV7()
    {
    }

    internal ItemLevelFailuresComponentV7(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IList<ItemLevelFailureRecord> GetFailedItemIds(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId = 0)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      int val1 = topCount > -1 ? topCount : throw new ArgumentException(nameof (topCount));
      List<ItemLevelFailureRecord> failedItemIds = new List<ItemLevelFailureRecord>();
      for (int index = 0; index < topCount; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_RetrieveFailedItemIds");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponentV7.FailedItemIdRecordColumns());
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
        stopwatch.Stop();
        if (levelFailureRecordList.Count > 0)
        {
          failedItemIds.AddRange((IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList);
          startingId = levelFailureRecordList.Max<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, long>) (x => x.Id)) + 1L;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) "ItemLevelFailuresComponent", (object) "GetFailedItems", (object) stopwatch.ElapsedMilliseconds));
        if (val1 <= 0 || levelFailureRecordList.Count < parameterValue)
          break;
      }
      return (IList<ItemLevelFailureRecord>) failedItemIds;
    }

    public override int GetCountOfFailedItemsModifiedBefore(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits,
      int hoursToLookBack,
      int maxAttemptCount)
    {
      if (indexingUnits == null || !indexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        throw new ArgumentException(nameof (indexingUnits));
      if (hoursToLookBack < 0)
        throw new ArgumentException(nameof (hoursToLookBack));
      if (maxAttemptCount <= 0)
        throw new ArgumentException(nameof (maxAttemptCount));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfFailedItemsModifiedBefore");
      this.BindIndexingUnitIdTable("@indexingUnitIdList", indexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((System.Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId)));
      this.BindInt("@hoursBack", hoursToLookBack);
      this.BindInt("@attemptCount", maxAttemptCount);
      int itemsModifiedBefore = (int) this.ExecuteScalar();
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.prc_GetCountOfFailedItemsModifiedWithin took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return itemsModifiedBefore;
    }

    protected SqlParameter BindIndexingUnitIdTable(string parameterName, IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ItemLevelFailuresComponentV7.s_int32Table);
        sqlDataRecord.SetInt32(0, entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Int32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    protected class FailedItemIdRecordColumns : ObjectBinder<ItemLevelFailureRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_item = new SqlColumnBinder("Item");

      protected override ItemLevelFailureRecord Bind() => new ItemLevelFailureRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        Item = this.m_item.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
