// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.FileMetadataStoreComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class FileMetadataStoreComponentV2 : FileMetadataStoreComponent
  {
    public FileMetadataStoreComponentV2()
    {
    }

    internal FileMetadataStoreComponentV2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public virtual void DeleteFileMetadataRecordsByRange(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      long endingId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      if (startingId > endingId)
        throw new ArgumentException(nameof (endingId));
      this.PrepareStoredProcedure("Search.prc_DeleteFileMetadataRecordsByRange");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindLong("@startingId", startingId);
      this.BindLong("@endingId", endingId);
      this.ExecuteNonQuery(false);
      Tracer.TraceInfo(1082626, "Indexing Pipeline", "FileMetadataStoreComponent", string.Format("FileMetadataStoreComponent.DeleteRecordsByRange took {0}ms", (object) stopwatch.ElapsedMilliseconds));
    }
  }
}
