// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV10
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV10 : ItemLevelFailuresComponentV9
  {
    public ItemLevelFailuresComponentV10()
    {
    }

    internal ItemLevelFailuresComponentV10(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
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
            IDictionary<string, ItemLevelFailureRecord> failedItemsFromSql = this.GetExistingFailedItemsFromSQL(indexingUnit, (IList<string>) range.Select<ItemLevelFailureRecord, string>((Func<ItemLevelFailureRecord, string>) (x => x.Item)).ToList<string>());
            this.UpdateRecords(failureRecordsMap, failedItemsFromSql);
            this.PrepareStoredProcedure("Search.prc_UpsertFileLevelFailureRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindFileLevelFailureRecordsParameterContainingListOfBranches("@fileLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) failureRecordsMap.Values.ToList<ItemLevelFailureRecord>());
            this.ExecuteNonQuery(false);
            num += failureRecordsMap.Values.Where<ItemLevelFailureRecord>((Func<ItemLevelFailureRecord, bool>) (s => s.AttemptCount >= maxAttemptCount)).Count<ItemLevelFailureRecord>();
            stopwatch.Stop();
            Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeItemFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
          }
          else
            break;
        }
      }
      return num;
    }
  }
}
