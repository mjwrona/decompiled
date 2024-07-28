// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.TempFileMetadataStoreComponentV3
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
  internal class TempFileMetadataStoreComponentV3 : TempFileMetadataStoreComponentV2
  {
    public TempFileMetadataStoreComponentV3()
    {
    }

    internal TempFileMetadataStoreComponentV3(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<TempFileMetadataRecord> GetFilesWithMinAttemptCount(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      short minAttemptCount,
      string indexingUnitType,
      DocumentContractType contractType,
      int hours = 0)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (minAttemptCount <= (short) 0)
        throw new ArgumentException(nameof (minAttemptCount));
      List<TempFileMetadataRecord> withMinAttemptCount = new List<TempFileMetadataRecord>();
      this.PrepareStoredProcedure("Search.prc_GetFilesWithMinAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindShort("@attemptCount", minAttemptCount);
      this.BindInt("@hours", hours);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TempFileMetadataRecord>((ObjectBinder<TempFileMetadataRecord>) new TempFileMetadataStoreComponent.FilePathAndAttemptCount(indexingUnitType, contractType));
        ObjectBinder<TempFileMetadataRecord> current = resultCollection.GetCurrent<TempFileMetadataRecord>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
              withMinAttemptCount.AddRange((IEnumerable<TempFileMetadataRecord>) current.Items);
          }
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", "TempFileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.GetFilesWithMinAttemptCount took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return (IEnumerable<TempFileMetadataRecord>) withMinAttemptCount;
    }
  }
}
