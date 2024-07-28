// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent17
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent17 : FileContainerComponent16
  {
    internal override Task<FileCleanupSegmentedContainerStats> FileCleanupContainersAsync(
      bool useSecondaryFileIdRange,
      int deleteBatchSize,
      int selectBatchSizeulong,
      ulong lastRowVersion = 0)
    {
      return Task.FromResult<FileCleanupSegmentedContainerStats>(new FileCleanupSegmentedContainerStats());
    }

    internal override async Task<FileCleanupSegmentedStats> FileCleanupFilesAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false)
    {
      FileContainerComponent17 containerComponent17 = this;
      int commandTimeout = 21600;
      containerComponent17.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearFiles", commandTimeout);
      containerComponent17.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      containerComponent17.BindInt("@selectBatchSize", selectbatchsize);
      containerComponent17.BindInt("@deleteBatchSize", 5000);
      containerComponent17.BindInt("@fileContainerOwnerId", 10);
      FileCleanupSegmentedStats cleanupSegmentedStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent17.ExecuteReaderAsync(), containerComponent17.ProcedureName, containerComponent17.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedStats>((ObjectBinder<FileCleanupSegmentedStats>) containerComponent17.GetFileContainerCleanupSegmentBinder());
        cleanupSegmentedStats = resultCollection.GetCurrent<FileCleanupSegmentedStats>().Items[0];
      }
      return cleanupSegmentedStats;
    }

    internal virtual FileContainerCleanupSegmentedBinder GetFileContainerCleanupSegmentBinder() => new FileContainerCleanupSegmentedBinder();

    internal virtual FileContainerCleanupSegmentedContainerBinder GetFileContainerCleanupSegmentContainerBinder() => new FileContainerCleanupSegmentedContainerBinder();
  }
}
