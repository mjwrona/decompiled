// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV14
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV14 : ItemLevelFailuresComponentV13
  {
    public ItemLevelFailuresComponentV14()
    {
    }

    internal ItemLevelFailuresComponentV14(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
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
      List<RejectionCode> retriableCodes = RejectionCodeExtension.GetRetriableCodes();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsWithMaxAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindInt("@attemptCount", maxAttemptCount);
      this.BindTinyIntIds("@rejectionCodes", (IEnumerable<byte>) retriableCodes.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
      int withMaxAttemptCount = (int) this.ExecuteScalar();
      stopwatch.Stop();
      Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsWithMaxAttemptCount took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return withMaxAttemptCount;
    }
  }
}
