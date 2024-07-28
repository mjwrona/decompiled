// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV8
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
  public class ItemLevelFailuresComponentV8 : ItemLevelFailuresComponentV7
  {
    public ItemLevelFailuresComponentV8()
    {
    }

    internal ItemLevelFailuresComponentV8(string connectionString, int partitionId)
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
      int num1 = 0;
      if (indexingUnit.EntityType.Name == "Code" || indexingUnit.EntityType.Name == "Wiki")
      {
        foreach (KeyValuePair<string, List<ItemLevelFailureRecord>> levelFailureRecords in (IEnumerable<KeyValuePair<string, List<ItemLevelFailureRecord>>>) this.GetBranchToItemLevelFailureRecordsMap(records))
        {
          List<ItemLevelFailureRecord> levelFailureRecordList = levelFailureRecords.Value;
          int count1 = levelFailureRecordList.Count;
          int val1 = count1;
          for (int index = 0; index < count1; index += 500)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int count2 = Math.Min(val1, 500);
            IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) levelFailureRecordList.GetRange(index, count2);
            val1 -= count2;
            if (range.Count > 0)
            {
              this.PrepareStoredProcedure("Search.prc_MergeFileLevelFailures");
              this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
              this.BindFileLevelFailureRecordsParameter("@fileLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) range);
              this.BindInt("@maxFailureThreshold", maxAttemptCount);
              int num2 = (int) this.ExecuteScalar();
              num1 += num2;
              stopwatch.Stop();
              Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeItemFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
            }
            else
              break;
          }
        }
      }
      else
      {
        if (!(indexingUnit.EntityType.Name == "WorkItem"))
          throw new NotSupportedException(string.Format("Entity type [{0}] is not supported.", (object) indexingUnit.EntityType));
        List<ItemLevelFailureRecord> list = records.ToList<ItemLevelFailureRecord>();
        int count3 = list.Count;
        int val1 = count3;
        for (int index = 0; index < count3; index += 500)
        {
          int count4 = Math.Min(val1, 500);
          IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count4);
          val1 -= count4;
          if (range.Count > 0)
          {
            this.PrepareStoredProcedure("Search.prc_MergeWorkItemLevelFailures");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindWorkItemLevelFailureRecordsParameter("@workItemLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) range);
            this.BindInt("@maxFailureThreshold", maxAttemptCount);
            int num3 = (int) this.ExecuteScalar();
            num1 += num3;
          }
          else
            break;
        }
      }
      return num1;
    }
  }
}
