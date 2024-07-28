// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV13
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV13 : ItemLevelFailuresComponentV12
  {
    public ItemLevelFailuresComponentV13()
    {
    }

    internal ItemLevelFailuresComponentV13(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
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
      List<RejectionCode> retriableCodes = RejectionCodeExtension.GetRetriableCodes();
      this.PrepareStoredProcedure("Search.prc_GetItemsWithMaxAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindInt("@attemptCount", maxAttemptCount);
      this.BindInt("@count", count);
      this.BindTinyIntIds("@rejectionCodes", (IEnumerable<byte>) retriableCodes.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
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
  }
}
