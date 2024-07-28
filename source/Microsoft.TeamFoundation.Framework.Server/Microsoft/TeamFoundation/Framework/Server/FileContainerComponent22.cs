// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent22
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent22 : FileContainerComponent21
  {
    internal override async Task<FileCleanupSegmentedContainerStats> FileCleanupContainersAlternateAsync(
      bool useSecondaryFileIdRange,
      int deleteBatchSize,
      int selectBatchSize)
    {
      FileContainerComponent22 containerComponent22 = this;
      int commandTimeout = 21600;
      containerComponent22.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearContainers2", commandTimeout);
      containerComponent22.BindInt("@batchDeleteSize", deleteBatchSize);
      containerComponent22.BindInt("@batchSelectSize", selectBatchSize);
      FileCleanupSegmentedContainerStats segmentedContainerStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent22.ExecuteReaderAsync(), containerComponent22.ProcedureName, containerComponent22.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedContainerStats>((ObjectBinder<FileCleanupSegmentedContainerStats>) containerComponent22.GetFileContainerCleanupSegmentContainerBinder());
        segmentedContainerStats = resultCollection.GetCurrent<FileCleanupSegmentedContainerStats>().Items[0];
      }
      return segmentedContainerStats;
    }
  }
}
