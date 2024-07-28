// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.WorkItemOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (IOperationMapper))]
  public class WorkItemOperationMapper : IOperationMapper
  {
    public IEntityType SupportedEntityType => (IEntityType) WorkItemEntityType.GetInstance();

    public virtual int Weight => 0;

    public IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnit indexingUnit)
    {
      switch (indexingUnit.IndexingUnitType)
      {
        case "Collection":
          string changeType1 = indexingUnitChangeEvent.ChangeType;
          if (changeType1 != null)
          {
            switch (changeType1.Length)
            {
              case 5:
                if (changeType1 == "Patch")
                  return (IRunnable<OperationResult>) new CollectionWorkItemPatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 13:
                if (changeType1 == "CrawlMetadata")
                  return (IRunnable<OperationResult>) new CollectionWorkItemMetadataCrawlOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 14:
                switch (changeType1[0])
                {
                  case 'B':
                    if (changeType1 == "BeginBulkIndex")
                      return (IRunnable<OperationResult>) new CollectionWorkItemBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 'U':
                    if (changeType1 == "UpdateMetadata")
                      return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                }
                break;
              case 17:
                if (changeType1 == "CompleteBulkIndex")
                  return (IRunnable<OperationResult>) new CollectionWorkItemFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 23:
                if (changeType1 == "ExtensionBeginUninstall")
                  return (IRunnable<OperationResult>) new CollectionExtensionBeginUninstallOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 24:
                if (changeType1 == "AreaNodeSecurityAcesSync")
                  return (IRunnable<OperationResult>) new CollectionWorkItemAreaNodeSecurityAcesSyncOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 25:
                if (changeType1 == "UpdateWorkItemFieldValues")
                  return (IRunnable<OperationResult>) new CollectionWorkItemUpdateFieldValues((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 26:
                if (changeType1 == "ExtensionFinalizeUninstall")
                  return (IRunnable<OperationResult>) new CollectionExtensionFinalizeUninstallOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 28:
                if (changeType1 == "UpdateIndexingUnitProperties")
                  return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 39:
                if (changeType1 == "UpdateSearchUrlInIndexingUnitProperties")
                  return (IRunnable<OperationResult>) new UpdateSearchUrlInIndexingPropertiesOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
            }
          }
          else
            break;
          break;
        case "Project":
          string changeType2 = indexingUnitChangeEvent.ChangeType;
          if (changeType2 != null)
          {
            switch (changeType2.Length)
            {
              case 5:
                if (changeType2 == "Patch")
                  return (IRunnable<OperationResult>) new ProjectWorkItemPatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              case 6:
                if (changeType2 == "Delete")
                  return (IRunnable<OperationResult>) new ProjectWorkItemDeleteOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              case 7:
                if (changeType2 == "Destroy" && indexingUnitChangeEvent.ChangeData is WorkItemDestroyedEventData)
                  return (IRunnable<OperationResult>) new ProjectWorkItemDestroyOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              case 11:
                if (changeType2 == "UpdateIndex")
                  break;
                goto label_46;
              case 14:
                if (changeType2 == "BeginBulkIndex")
                  break;
                goto label_46;
              case 17:
                if (changeType2 == "BeginEntityRename")
                  return (IRunnable<OperationResult>) new ProjectWorkItemRenameOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              case 21:
                if (changeType2 == "AddClassificationNode")
                  goto label_43;
                else
                  goto label_46;
              case 24:
                if (changeType2 == "UpdateClassificationNode")
                  goto label_43;
                else
                  goto label_46;
              case 25:
                if (changeType2 == "SyncAllClassificationNode")
                  return (IRunnable<OperationResult>) new ProjectWorkItemSyncAllClassificationNodesOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              case 28:
                if (changeType2 == "UpdateIndexingUnitProperties")
                  return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_46;
              default:
                goto label_46;
            }
            return (IRunnable<OperationResult>) new ProjectWorkItemIndexingOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
label_43:
            return (IRunnable<OperationResult>) new ProjectWorkItemUpdateClassificationNodeOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          }
          break;
      }
label_46:
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Operation class for Indexing Unit Kind: {0}, Indexing Unit Type: {1}, ChangeType: {2}", (object) indexingUnit.EntityType, (object) indexingUnit.IndexingUnitType, (object) indexingUnitChangeEvent.ChangeType)));
    }
  }
}
