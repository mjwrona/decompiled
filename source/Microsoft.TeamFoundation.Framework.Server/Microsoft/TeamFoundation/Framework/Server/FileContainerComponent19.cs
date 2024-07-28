// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent19
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent19 : FileContainerComponent18
  {
    internal override sealed async Task<FileCleanupSegmentedArtifactStats> FileCleanupArtifactsWatermarkedAsync(
      int batchSize,
      DateTime lastArtifactCreationDate)
    {
      FileContainerComponent19 containerComponent19 = this;
      int commandTimeout = 3600;
      containerComponent19.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearArtifacts2", commandTimeout);
      containerComponent19.BindInt("@deleteBatchSize", batchSize);
      containerComponent19.BindDateTime2("@artifactDateWatermark", lastArtifactCreationDate);
      FileCleanupSegmentedArtifactStats segmentedArtifactStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent19.ExecuteReaderAsync(), containerComponent19.ProcedureName, containerComponent19.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedArtifactStats>((ObjectBinder<FileCleanupSegmentedArtifactStats>) containerComponent19.GetFileContainerCleanupSegmentedArtifactBinder());
        segmentedArtifactStats = resultCollection.GetCurrent<FileCleanupSegmentedArtifactStats>().Items[0];
      }
      return segmentedArtifactStats;
    }

    internal virtual FileContainerCleanupSegmentedArtifactBinder GetFileContainerCleanupSegmentedArtifactBinder() => new FileContainerCleanupSegmentedArtifactBinder();
  }
}
