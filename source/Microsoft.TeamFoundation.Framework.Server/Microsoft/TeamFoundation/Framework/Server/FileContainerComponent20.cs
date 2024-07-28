// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent20
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent20 : FileContainerComponent19
  {
    internal override async Task<FileCleanupSegmentedFileStats> FileCleanupFilesWatermarkedAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false,
      int? previousFileId = null,
      int? previousDataspaceId = null)
    {
      FileContainerComponent20 containerComponent20 = this;
      int commandTimeout = 10800;
      containerComponent20.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearFiles2", commandTimeout);
      containerComponent20.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      containerComponent20.BindInt("@selectBatchSize", selectbatchsize);
      containerComponent20.BindInt("@fileContainerOwnerId", 10);
      containerComponent20.BindBoolean("@cleanupHashJoin", cleanupHashJoin);
      containerComponent20.BindNullableInt("@previousFileId", previousFileId);
      containerComponent20.BindNullableInt("@previousDataspaceId", previousDataspaceId);
      FileCleanupSegmentedFileStats segmentedFileStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent20.ExecuteReaderAsync(), containerComponent20.ProcedureName, containerComponent20.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedFileStats>((ObjectBinder<FileCleanupSegmentedFileStats>) containerComponent20.GetFileContainerCleanupSegmentedFilesBinder());
        segmentedFileStats = resultCollection.GetCurrent<FileCleanupSegmentedFileStats>().Items[0];
      }
      return segmentedFileStats;
    }

    internal virtual FileContainerCleanupSegmentedFilesBinder GetFileContainerCleanupSegmentedFilesBinder() => new FileContainerCleanupSegmentedFilesBinder();
  }
}
