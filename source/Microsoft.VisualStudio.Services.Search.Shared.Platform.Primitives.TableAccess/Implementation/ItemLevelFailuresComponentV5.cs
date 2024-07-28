// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV5
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
  public class ItemLevelFailuresComponentV5 : ItemLevelFailuresComponentV4
  {
    public ItemLevelFailuresComponentV5()
    {
    }

    internal ItemLevelFailuresComponentV5(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<ItemLevelFailureRecord> GetFailedItems(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId = 0)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      int val1 = topCount > -1 ? topCount : throw new ArgumentException(nameof (topCount));
      List<ItemLevelFailureRecord> failedItems = new List<ItemLevelFailureRecord>();
      for (int index = 0; index < topCount; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_RetrieveFailedItems");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponent.FailedItemRecordColumns());
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
        failedItems.AddRange((IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList);
        if (val1 > 0 && levelFailureRecordList.Count >= parameterValue)
        {
          stopwatch.Stop();
          startingId = levelFailureRecordList.Max<ItemLevelFailureRecord>((System.Func<ItemLevelFailureRecord, long>) (x => x.Id)) + 1L;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) "ItemLevelFailuresComponent", (object) nameof (GetFailedItems), (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<ItemLevelFailureRecord>) failedItems;
    }
  }
}
