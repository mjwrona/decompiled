// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent21
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent21 : FileContainerComponent20
  {
    internal override async Task<FileCleanupSegmentedArtifactStats> FileCleanupArtifactsWatermarkedIndexedAsync(
      int batchSize,
      DateTime lastArtifactCreation)
    {
      FileContainerComponent21 containerComponent21 = this;
      int commandTimeout = 10800;
      containerComponent21.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearArtifacts3", commandTimeout);
      containerComponent21.BindInt("@deleteBatchSize", batchSize);
      containerComponent21.BindDateTime2("@artifactDateWatermark", lastArtifactCreation);
      FileCleanupSegmentedArtifactStats segmentedArtifactStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent21.ExecuteReaderAsync(), containerComponent21.ProcedureName, containerComponent21.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedArtifactStats>((ObjectBinder<FileCleanupSegmentedArtifactStats>) containerComponent21.GetFileContainerCleanupSegmentedArtifactBinder());
        segmentedArtifactStats = resultCollection.GetCurrent<FileCleanupSegmentedArtifactStats>().Items[0];
      }
      return segmentedArtifactStats;
    }

    internal override sealed async Task<FileCleanupSegmentedStats> FileCleanupArtifactsAsync(
      int batchSize)
    {
      FileContainerComponent21 containerComponent21 = this;
      int commandTimeout = 21600;
      containerComponent21.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearArtifacts", commandTimeout);
      containerComponent21.BindInt("@deleteBatchSize", batchSize);
      FileCleanupSegmentedStats cleanupSegmentedStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent21.ExecuteReaderAsync(), containerComponent21.ProcedureName, containerComponent21.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedStats>((ObjectBinder<FileCleanupSegmentedStats>) containerComponent21.GetFileContainerCleanupSegmentBinder());
        cleanupSegmentedStats = resultCollection.GetCurrent<FileCleanupSegmentedStats>().Items[0];
      }
      return cleanupSegmentedStats;
    }
  }
}
