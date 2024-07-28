// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.ContextBuilderFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class ContextBuilderFactory : AbstractContextBuilderFactory
  {
    public override IProviderContextBuilder GetProviderContextBuilder(DataType dataType)
    {
      switch (dataType)
      {
        case DataType.CollectionIndexingUnitData:
        case DataType.ProjectIndexingUnitData:
        case DataType.RepoIndexingUnitData:
        case DataType.ScopedIndexingUnitData:
          return (IProviderContextBuilder) new IUContextBuilder();
        case DataType.ESTermQuerydata:
        case DataType.GitRepoBranchLevelDocCount:
          return (IProviderContextBuilder) new ESContextBuilder();
        case DataType.IndexingUnitChangeEventsData:
        case DataType.IndexingUnitChangeEventStateData:
          return (IProviderContextBuilder) new IUCEContextBuilder();
        case DataType.FailedFilesCountData:
          return (IProviderContextBuilder) new FailedFilesContextBuilder();
        case DataType.JobQueueData:
          return (IProviderContextBuilder) new JobQueueContextBuilder();
        case DataType.ReindexingStatusData:
          return (IProviderContextBuilder) new ReindexingStatusContextBuilder();
        case DataType.ShardSizeData:
        case DataType.SearchIndexDocumentCountData:
          return (IProviderContextBuilder) new ESDeploymentContextBuilder();
        default:
          throw new NotSupportedException("Unsupported data type and context builder cannot be instantiated.");
      }
    }
  }
}
