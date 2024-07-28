// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  internal sealed class DataAccessFactory : IDataAccessFactory
  {
    [StaticSafe]
    private static readonly DataAccessFactory s_instance = new DataAccessFactory();

    private DataAccessFactory()
    {
    }

    internal static IDataAccessFactory GetInstance() => (IDataAccessFactory) DataAccessFactory.s_instance;

    public ICustomRepositoryDataAccess GetCustomRepositoryDataAccess() => (ICustomRepositoryDataAccess) new CustomRepositoryDataAccess();

    public IIndexingUnitDataAccess GetIndexingUnitDataAccess() => (IIndexingUnitDataAccess) new IndexingUnitDataAccess();

    public ISharedIndexingPropertyDataAccess GetSharedIndexingPropertyDataAccess() => (ISharedIndexingPropertyDataAccess) new SharedIndexingPropertyDataAccess();

    public IIndexingUnitChangeEventDataAccess GetIndexingUnitChangeEventDataAccess() => (IIndexingUnitChangeEventDataAccess) new IndexingUnitChangeEventDataAccess();

    public IDisabledFilesDataAccess GetDisabledFilesDataAccess() => (IDisabledFilesDataAccess) new DisabledFilesDataAccess();

    public IIndexingUnitChangeEventArchiveDataAccess GetIndexingUnitChangeEventArchiveAccess() => (IIndexingUnitChangeEventArchiveDataAccess) new IndexingUnitChangeEventArchiveAccess();

    public IReindexingStatusDataAccess GetReindexingStatusDataAccess() => (IReindexingStatusDataAccess) new ReindexingStatusDataAccess();

    public IClassificationNodeDataAccess GetClassificationNodeDataAccess() => (IClassificationNodeDataAccess) new ClassificationNodeDataAccess();

    public ILockManager GetLockManager() => (ILockManager) new Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.LockManager();

    public ITempFileMetadataStoreDataAccess GetTempFileStoreDataAccess(IndexingUnit indexingUnit) => (ITempFileMetadataStoreDataAccess) new TempFileMetadataStoreDataAccess(indexingUnit);

    public IFileMetadataStoreDataAccess GetFileMetadataStoreDataAccess(IndexingUnit indexingUnit) => (IFileMetadataStoreDataAccess) new FileMetadataStoreDataAccess(indexingUnit);

    public IItemLevelFailureDataAccess GetItemLevelFailureDataAccess() => (IItemLevelFailureDataAccess) new ItemLevelFailureDataAccess();

    public IHealthStatusDataAccess GetHealthStatusDataAccess() => (IHealthStatusDataAccess) new HealthStatusDataAccess();

    public IIndexingUnitWikisDataAccess GetIndexingUnitWikisDataAccess() => (IIndexingUnitWikisDataAccess) new IndexingUnitWikisDataAccess();

    public IShardDetailsDataAccess GetShardDetailsDataAccess() => (IShardDetailsDataAccess) new ShardDetailsDataAccess();

    public IHealthStatusMonitoringDataAccess GetHealthStatusMonitoringDataAccess() => (IHealthStatusMonitoringDataAccess) new HealthStatusMonitoringDataAccess();

    public IPackageContainerDataAccess GetPackageContainerDataAccess() => (IPackageContainerDataAccess) new PackageContainerDataAccess();
  }
}
