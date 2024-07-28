// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV6
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV6 : ItemLevelFailuresComponentV5
  {
    public ItemLevelFailuresComponentV6()
    {
    }

    internal ItemLevelFailuresComponentV6(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IDictionary<int, int> GetCountOfRecordsByIndexingUnit(
      IEntityType entityType,
      int maxAttemptCount)
    {
      if (maxAttemptCount <= 0)
        throw new ArgumentException(nameof (maxAttemptCount));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsWithMaxAttemptCountPerIndexingUnitForEntityType");
      this.BindInt("@attemptCount", maxAttemptCount);
      this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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
