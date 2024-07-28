// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV2 : ItemLevelFailuresComponent
  {
    public ItemLevelFailuresComponentV2()
    {
    }

    internal ItemLevelFailuresComponentV2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override int GetCountOfRecordsForIndexingUnit(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCoutOfRecordsForIndexingUnit");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      int recordsForIndexingUnit = (int) this.ExecuteScalar();
      Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsByIndexingUnit took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return recordsForIndexingUnit;
    }
  }
}
