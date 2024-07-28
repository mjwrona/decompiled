// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.DataProviderFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class DataProviderFactory : AbstractDataProviderFactory
  {
    private readonly Dictionary<DataType, IDataProvider> m_typeToDataProviderMap = new Dictionary<DataType, IDataProvider>()
    {
      {
        DataType.CollectionIndexingUnitData,
        (IDataProvider) new CollectionIndexingUnitsDataProvider()
      },
      {
        DataType.ProjectIndexingUnitData,
        (IDataProvider) new ProjectIndexingUnitsDataProvider()
      },
      {
        DataType.RepoIndexingUnitData,
        (IDataProvider) new RepositoryIndexingUnitsDataProvider()
      },
      {
        DataType.FailedFilesCountData,
        (IDataProvider) new FailedFilesDataProvider()
      },
      {
        DataType.JobQueueData,
        (IDataProvider) new JobQueueDataProvider()
      },
      {
        DataType.ReindexingStatusData,
        (IDataProvider) new ReindexingStatusDataProvider()
      },
      {
        DataType.ESTermQuerydata,
        (IDataProvider) new ElasticSearchBaseDataProvider()
      },
      {
        DataType.IndexingUnitChangeEventsData,
        (IDataProvider) new IndexingUnitChangeEventsDataProvider()
      },
      {
        DataType.SearchIndexDocumentCountData,
        (IDataProvider) new EsIndexDocumentCountDataProvider()
      },
      {
        DataType.ShardSizeData,
        (IDataProvider) new ShardsSizeDataProvider()
      },
      {
        DataType.IndexingUnitChangeEventStateData,
        (IDataProvider) new IndexingUnitChangeEventStateDataProvider()
      },
      {
        DataType.ScopedIndexingUnitData,
        (IDataProvider) new ScopedIndexingUnitsDataProvider()
      },
      {
        DataType.GitRepoBranchLevelDocCount,
        (IDataProvider) new GitRepoBranchLevelDocumentCountDataProvider()
      }
    };

    public override List<IDataProvider> GetDataProviders(HashSet<DataType> dataTypes)
    {
      List<IDataProvider> dataProviders = new List<IDataProvider>();
      foreach (DataType dataType in dataTypes)
        dataProviders.Add(this.m_typeToDataProviderMap[dataType]);
      return dataProviders;
    }

    public override IDataProvider GetDataProvider(DataType dataType) => this.m_typeToDataProviderMap[dataType];
  }
}
