// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV3 : ItemLevelFailuresComponentV2
  {
    public ItemLevelFailuresComponentV3()
    {
    }

    internal ItemLevelFailuresComponentV3(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override void RemoveSuccessfullyIndexedItemsFromFailedRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> successfullyIndexedRecords)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (successfullyIndexedRecords == null || !successfullyIndexedRecords.Any<ItemLevelFailureRecord>() || !(indexingUnit.EntityType.Name == "Code"))
        return;
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
            this.PrepareStoredProcedure("Search.prc_RemoveSuccessfullyIndexedItemsFromFailedRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindFileLevelFailureRecordsParameter("@successfullyIndexedRecords", (IEnumerable<ItemLevelFailureRecord>) range);
            this.ExecuteNonQuery(false);
            stopwatch.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeFailedRecordsWtihSuccessfullyIndexedItems took {0}ms", (object) stopwatch.ElapsedMilliseconds));
          }
          else
            break;
        }
      }
    }

    public override int GetCountOfRecordsWithMaxAttemptCount(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int maxAttemptCount)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (maxAttemptCount <= 0)
        throw new ArgumentException(nameof (maxAttemptCount));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsWithMaxAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindInt("@attemptCount", maxAttemptCount);
      int withMaxAttemptCount = (int) this.ExecuteScalar();
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsWithMaxAttemptCount took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return withMaxAttemptCount;
    }

    public override IDictionary<int, int> GetCountOfRecordsByIndexingUnit(int maxAttemptCount)
    {
      if (maxAttemptCount <= 0)
        throw new ArgumentException(nameof (maxAttemptCount));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsWithMaxAttemptCountPerIndexingUnit");
      this.BindInt("@attemptCount", maxAttemptCount);
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
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsByIndexingUnit took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return recordsByIndexingUnit;
    }
  }
}
