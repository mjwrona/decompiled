// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent18 : FileContainerComponent17
  {
    internal override sealed async Task<FileCleanupSegmentedStats> FileCleanupFilesAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false)
    {
      FileContainerComponent18 containerComponent18 = this;
      int commandTimeout = 10800;
      containerComponent18.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearFiles", commandTimeout);
      containerComponent18.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      containerComponent18.BindInt("@selectBatchSize", selectbatchsize);
      containerComponent18.BindInt("@fileContainerOwnerId", 10);
      containerComponent18.BindBoolean("@cleanupHashJoin", cleanupHashJoin);
      FileCleanupSegmentedStats cleanupSegmentedStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent18.ExecuteReaderAsync(), containerComponent18.ProcedureName, containerComponent18.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedStats>((ObjectBinder<FileCleanupSegmentedStats>) containerComponent18.GetFileContainerCleanupSegmentBinder());
        cleanupSegmentedStats = resultCollection.GetCurrent<FileCleanupSegmentedStats>().Items[0];
      }
      return cleanupSegmentedStats;
    }
  }
}
