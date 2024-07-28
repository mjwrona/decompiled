// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IDataAccessFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IDataAccessFactory
  {
    ICustomRepositoryDataAccess GetCustomRepositoryDataAccess();

    IDisabledFilesDataAccess GetDisabledFilesDataAccess();

    IIndexingUnitDataAccess GetIndexingUnitDataAccess();

    ISharedIndexingPropertyDataAccess GetSharedIndexingPropertyDataAccess();

    IIndexingUnitChangeEventDataAccess GetIndexingUnitChangeEventDataAccess();

    IIndexingUnitChangeEventArchiveDataAccess GetIndexingUnitChangeEventArchiveAccess();

    IReindexingStatusDataAccess GetReindexingStatusDataAccess();

    IClassificationNodeDataAccess GetClassificationNodeDataAccess();

    ILockManager GetLockManager();

    ITempFileMetadataStoreDataAccess GetTempFileStoreDataAccess(IndexingUnit indexingUnit);

    IFileMetadataStoreDataAccess GetFileMetadataStoreDataAccess(IndexingUnit indexingUnit);

    IItemLevelFailureDataAccess GetItemLevelFailureDataAccess();

    IHealthStatusDataAccess GetHealthStatusDataAccess();

    IIndexingUnitWikisDataAccess GetIndexingUnitWikisDataAccess();

    IShardDetailsDataAccess GetShardDetailsDataAccess();

    IHealthStatusMonitoringDataAccess GetHealthStatusMonitoringDataAccess();

    IPackageContainerDataAccess GetPackageContainerDataAccess();
  }
}
