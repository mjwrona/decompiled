// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.IndexerPipelineFlowHandlerCreator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (ICorePipelineFlowHandlerCreator))]
  public class IndexerPipelineFlowHandlerCreator : ICorePipelineFlowHandlerCreator
  {
    public CorePipelineFlowHandler GetPipelineFlowHandler(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      TraceMetaData traceMetaData)
    {
      switch (indexingUnit.EntityType.Name)
      {
        case "Code":
          switch (indexingUnit.IndexingUnitType)
          {
            case "Git_Repository":
              string changeType1 = indexingUnitChangeEvent.ChangeType;
              if (changeType1 == "BeginBulkIndex" || changeType1 == "CustomGitRepositoryBulkIndex" || changeType1 == "UpdateIndex")
                return (CorePipelineFlowHandler) new GitIndexPipelineFlowHandler(indexingUnit, traceMetaData);
              break;
            case "ScopedIndexingUnit":
              string changeType2 = indexingUnitChangeEvent.ChangeType;
              if (changeType2 == "BeginBulkIndex" || changeType2 == "CustomGitRepositoryBulkIndex" || changeType2 == "UpdateIndex")
              {
                switch (((IndexingExecutionContext) coreIndexingExecutionContext).RepositoryIndexingUnit.IndexingUnitType)
                {
                  case "Git_Repository":
                    return (CorePipelineFlowHandler) new GitIndexPipelineFlowHandler(indexingUnit, traceMetaData);
                  case "CustomRepository":
                    return (CorePipelineFlowHandler) new CustomRepositoryPipelineFlowHandler(indexingUnit, traceMetaData);
                }
              }
              else
                break;
              break;
            case "TFVC_Repository":
              TfvcCodeRepoIndexingProperties properties = indexingUnit.Properties as TfvcCodeRepoIndexingProperties;
              switch (indexingUnitChangeEvent.ChangeType)
              {
                case "BeginBulkIndex":
                  return (CorePipelineFlowHandler) new TfvcIndexPipelineFlowHandler(indexingUnit, traceMetaData);
                case "UpdateIndex":
                  return (properties.ContinuousIndexJobYieldData == null ? 0 : (properties.ContinuousIndexJobYieldData.HasData() ? 1 : 0)) == 0 && coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsContinuationTokenSupportedForTfvcCICrawling", true, true) ? (CorePipelineFlowHandler) new TfvcIndexPipelineFlowHandler(indexingUnit, traceMetaData) : (CorePipelineFlowHandler) new TimeBoxedTfvcCIPipelineFlowHandler(indexingUnit, traceMetaData);
              }
              break;
            case "CustomRepository":
              return (CorePipelineFlowHandler) new CustomRepositoryPipelineFlowHandler(indexingUnit, traceMetaData);
            default:
              if (indexingUnitChangeEvent.ChangeType == "Patch")
                return (CorePipelineFlowHandler) new RepositoryCodePatchPipelineFlowHandler(indexingUnit, traceMetaData);
              break;
          }
          break;
        case "Wiki":
          if (indexingUnit.IndexingUnitType == "Git_Repository")
          {
            string changeType3 = indexingUnitChangeEvent.ChangeType;
            if (changeType3 == "BeginBulkIndex" || changeType3 == "UpdateIndex")
              return (CorePipelineFlowHandler) new GitIndexPipelineFlowHandler(indexingUnit, traceMetaData);
            break;
          }
          if (indexingUnitChangeEvent.ChangeType == "Patch")
            return (CorePipelineFlowHandler) new RepositoryCodePatchPipelineFlowHandler(indexingUnit, traceMetaData);
          break;
      }
      return new CorePipelineFlowHandler();
    }
  }
}
