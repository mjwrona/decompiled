// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent23
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent23 : FileContainerComponent22
  {
    internal override async Task<FileCleanupSegmentedContainerStats> FileCleanupContainersAsync(
      bool useSecondaryFileIdRange,
      int deleteBatchSize,
      int selectBatchSize,
      ulong lastRowVersion = 0)
    {
      FileContainerComponent23 containerComponent23 = this;
      int commandTimeout = 21600;
      containerComponent23.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearContainers", commandTimeout);
      containerComponent23.BindInt("@batchDeleteSize", deleteBatchSize);
      containerComponent23.BindInt("@batchSelectSize", selectBatchSize);
      containerComponent23.BindRowVersion("@activeTimestamp", lastRowVersion);
      FileCleanupSegmentedContainerStats segmentedContainerStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent23.ExecuteReaderAsync(), containerComponent23.ProcedureName, containerComponent23.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedContainerStats>((ObjectBinder<FileCleanupSegmentedContainerStats>) containerComponent23.GetFileContainerCleanupSegmentContainerBinderWithRowversion());
        segmentedContainerStats = resultCollection.GetCurrent<FileCleanupSegmentedContainerStats>().Items[0];
      }
      return segmentedContainerStats;
    }

    internal override sealed async Task<FileCleanupSegmentedFileStats> FileCleanupFilesWatermarkedAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false,
      int? previousFileId = null,
      int? previousDataspaceId = null)
    {
      return await this.FileCleanupFilesWatermarkedAsync(useSecondaryFileIdRange, selectbatchsize, previousFileId, previousDataspaceId);
    }

    internal virtual async Task<FileCleanupSegmentedFileStats> FileCleanupFilesWatermarkedAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      int? previousFileId = null,
      int? previousDataspaceId = null)
    {
      FileContainerComponent23 containerComponent23 = this;
      int commandTimeout = 21600;
      containerComponent23.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedClearFiles2", commandTimeout);
      containerComponent23.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      containerComponent23.BindInt("@selectBatchSize", selectbatchsize);
      containerComponent23.BindInt("@fileContainerOwnerId", 10);
      containerComponent23.BindNullableInt("@previousFileId", previousFileId);
      containerComponent23.BindNullableInt("@previousDataspaceId", previousDataspaceId);
      FileCleanupSegmentedFileStats segmentedFileStats;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await containerComponent23.ExecuteReaderAsync(), containerComponent23.ProcedureName, containerComponent23.RequestContext))
      {
        resultCollection.AddBinder<FileCleanupSegmentedFileStats>((ObjectBinder<FileCleanupSegmentedFileStats>) containerComponent23.GetFileContainerCleanupSegmentedFilesBinder());
        segmentedFileStats = resultCollection.GetCurrent<FileCleanupSegmentedFileStats>().Items[0];
      }
      return segmentedFileStats;
    }

    internal virtual FileContainerCleanupSegmentedContainerBinderWithRowversion GetFileContainerCleanupSegmentContainerBinderWithRowversion() => new FileContainerCleanupSegmentedContainerBinderWithRowversion();

    public override long AddContainer(
      Uri artifactUri,
      string securityToken,
      string name,
      string description,
      ContainerOptions options,
      Guid dataspaceIdentifier,
      string locatorPath)
    {
      this.TraceEnter(0, nameof (AddContainer));
      this.PrepareStoredProcedure("prc_CreateContainer");
      this.BindString("@artifactUri", artifactUri.ToString(), 512, false, SqlDbType.NVarChar);
      this.BindString("@securityToken", securityToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@name", name, 260, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 2048, true, SqlDbType.NVarChar);
      this.BindByte("@options", (byte) options);
      this.BindNullableGuid("@signingKeyId", Guid.Empty);
      this.BindGuid("@createdBy", this.Author);
      this.BindDataspace(dataspaceIdentifier);
      this.BindString("@locatorPath", locatorPath, 260, true, SqlDbType.NVarChar);
      long int64 = Convert.ToInt64(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
      this.TraceLeave(0, nameof (AddContainer));
      return int64;
    }
  }
}
