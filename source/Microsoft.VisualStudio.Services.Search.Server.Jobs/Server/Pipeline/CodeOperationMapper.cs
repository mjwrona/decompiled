// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CodeOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.V2Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.V2Jobs.Operations.Code.Repository;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (IOperationMapper))]
  public class CodeOperationMapper : IOperationMapper
  {
    public IEntityType SupportedEntityType => (IEntityType) CodeEntityType.GetInstance();

    public virtual int Weight => 0;

    public IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnit indexingUnit)
    {
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      string indexingUnitType = indexingUnit.IndexingUnitType;
      if (indexingUnitType != null)
      {
        switch (indexingUnitType.Length)
        {
          case 7:
            if (indexingUnitType == "Project")
            {
              switch (indexingUnitChangeEvent.ChangeType)
              {
                case "BeginEntityRename":
                  return (IRunnable<OperationResult>) new CodeProjectRenameOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "CompleteEntityRename":
                  return (IRunnable<OperationResult>) new ProjectUpdateFinalizeOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "Delete":
                  return (IRunnable<OperationResult>) new ProjectDeleteOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "UpdateIndexingUnitProperties":
                  return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "Patch":
                  return (IRunnable<OperationResult>) new CodeProjectPatchOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
              }
            }
            else
              break;
            break;
          case 10:
            if (indexingUnitType == "Collection")
            {
              string changeType = indexingUnitChangeEvent.ChangeType;
              if (changeType != null)
              {
                switch (changeType.Length)
                {
                  case 5:
                    if (changeType == "Patch")
                      return (IRunnable<OperationResult>) new CollectionCodePatchOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 13:
                    if (changeType == "CrawlMetadata")
                      return (IRunnable<OperationResult>) new CollectionCodeMetadataCrawlOperation(executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 14:
                    switch (changeType[0])
                    {
                      case 'B':
                        if (changeType == "BeginBulkIndex")
                          return (IRunnable<OperationResult>) new CollectionCodeBulkIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                      case 'U':
                        if (changeType == "UpdateMetadata")
                          return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                    }
                    break;
                  case 17:
                    if (changeType == "CompleteBulkIndex")
                      return (IRunnable<OperationResult>) new CollectionCodeFinalizeOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 23:
                    if (changeType == "ExtensionBeginUninstall")
                      return (IRunnable<OperationResult>) new CollectionExtensionBeginUninstallOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 24:
                    if (changeType == "DeleteDuplicateDocuments")
                      return (IRunnable<OperationResult>) new CollectionCodeDuplicateDocumentsDeletionOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 26:
                    if (changeType == "ExtensionFinalizeUninstall")
                      return (IRunnable<OperationResult>) new CollectionExtensionFinalizeUninstallOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 28:
                    if (changeType == "UpdateIndexingUnitProperties")
                      return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 29:
                    if (changeType == "GitRepositorySecurityAcesSync")
                      return (IRunnable<OperationResult>) new CollectionCodeGitRepositoryAcesSyncOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 39:
                    if (changeType == "UpdateSearchUrlInIndexingUnitProperties")
                      return (IRunnable<OperationResult>) new UpdateSearchUrlInIndexingPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                }
              }
              else
                break;
            }
            else
              break;
            break;
          case 14:
            if (indexingUnitType == "Git_Repository")
            {
              string changeType = indexingUnitChangeEvent.ChangeType;
              if (changeType != null)
              {
                switch (changeType.Length)
                {
                  case 5:
                    if (changeType == "Patch")
                      return (IRunnable<OperationResult>) new RepositoryCodePatchOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 6:
                    if (changeType == "Delete")
                      return (IRunnable<OperationResult>) new RepositoryDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 7:
                    if (changeType == "ReIndex")
                      return (IRunnable<OperationResult>) new GitRepositoryReIndexingOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 11:
                    switch (changeType[6])
                    {
                      case 'F':
                        if (changeType == "UpdateField")
                          return (IRunnable<OperationResult>) new CodeRepositoryUpdateFieldOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      case 'I':
                        if (changeType == "UpdateIndex")
                          break;
                        goto label_142;
                      default:
                        goto label_142;
                    }
                    break;
                  case 12:
                    if (changeType == "BranchDelete")
                      return (IRunnable<OperationResult>) new GitBranchDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 13:
                    if (changeType == "AssignRouting")
                      return (IRunnable<OperationResult>) new CodeEntityRoutingAssignmentOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 14:
                    switch (changeType[0])
                    {
                      case 'B':
                        if (changeType == "BeginBulkIndex")
                          break;
                        goto label_142;
                      case 'U':
                        if (changeType == "UpdateMetadata")
                          return (IRunnable<OperationResult>) new GitRepositoryCodeUpdateMetadataOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      default:
                        goto label_142;
                    }
                    break;
                  case 17:
                    switch (changeType[0])
                    {
                      case 'B':
                        if (changeType == "BeginEntityRename")
                          return (IRunnable<OperationResult>) new GitRepoRenameOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      case 'D':
                        if (changeType == "DeleteOrphanFiles")
                          return (IRunnable<OperationResult>) new DeleteOrphanGitFilesOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      default:
                        goto label_142;
                    }
                  case 20:
                    switch (changeType[8])
                    {
                      case 'B':
                        if (changeType == "CompleteBranchDelete")
                          return (IRunnable<OperationResult>) new CompleteGitBranchDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      case 'E':
                        if (changeType == "CompleteEntityRename")
                          return (IRunnable<OperationResult>) new GitRepoRenameFinalizeOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      default:
                        goto label_142;
                    }
                  case 24:
                    if (changeType == "DeleteDuplicateDocuments")
                      return (IRunnable<OperationResult>) new RepositoryCodeDuplicateDocumentsDeletionOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 28:
                    switch (changeType[0])
                    {
                      case 'C':
                        if (changeType == "CustomGitRepositoryBulkIndex")
                          return (IRunnable<OperationResult>) new GitRepositoryCodeCustomBulkIndexingOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      case 'U':
                        if (changeType == "UpdateIndexingUnitProperties")
                          return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      default:
                        goto label_142;
                    }
                  case 32:
                    if (changeType == "ResetBranchesInGitRepoAttributes")
                      return (IRunnable<OperationResult>) new CodeGitRepoResetBranchInfoOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  default:
                    goto label_142;
                }
                return (IRunnable<OperationResult>) new GitRepositoryCodeIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
              }
              break;
            }
            break;
          case 15:
            if (indexingUnitType == "TFVC_Repository")
            {
              string changeType = indexingUnitChangeEvent.ChangeType;
              if (changeType != null)
              {
                switch (changeType.Length)
                {
                  case 5:
                    if (changeType == "Patch")
                      return (IRunnable<OperationResult>) new RepositoryCodePatchOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 6:
                    if (changeType == "Delete")
                      return (IRunnable<OperationResult>) new RepositoryDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 7:
                    switch (changeType[0])
                    {
                      case 'D':
                        if (changeType == "Destroy")
                          return (IRunnable<OperationResult>) new TfvcDestroyOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                      case 'R':
                        if (changeType == "ReIndex")
                          return (IRunnable<OperationResult>) new TfvcRepositoryReIndexingOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                    }
                    break;
                  case 11:
                    switch (changeType[6])
                    {
                      case 'F':
                        if (changeType == "UpdateField")
                          return (IRunnable<OperationResult>) new TfvcRepositoryUpdateFieldOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                      case 'I':
                        if (changeType == "UpdateIndex")
                        {
                          TfvcContinuousIndexJobYieldData indexJobYieldData = ((TfvcCodeRepoIndexingProperties) indexingUnit.Properties).ContinuousIndexJobYieldData;
                          return (indexJobYieldData == null ? 0 : (indexJobYieldData.HasData() ? 1 : 0)) == 0 && executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsContinuationTokenSupportedForTfvcCICrawling", true, true) ? (IRunnable<OperationResult>) new TfvcRepositoryCodeBulkIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent) : (IRunnable<OperationResult>) new TfvcChangesetIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        }
                        break;
                    }
                    break;
                  case 13:
                    if (changeType == "AssignRouting")
                      return (IRunnable<OperationResult>) new CodeEntityRoutingAssignmentOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 14:
                    switch (changeType[0])
                    {
                      case 'B':
                        if (changeType == "BeginBulkIndex")
                          return (IRunnable<OperationResult>) new TfvcRepositoryCodeBulkIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                      case 'U':
                        if (changeType == "UpdateMetadata")
                          return (IRunnable<OperationResult>) new TfvcRepositoryUpdateMetadataOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        break;
                    }
                    break;
                  case 17:
                    if (changeType == "DeleteOrphanFiles")
                      return (IRunnable<OperationResult>) new DeleteOrphanTfvcFilesOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 24:
                    if (changeType == "DeleteDuplicateDocuments")
                      return (IRunnable<OperationResult>) new RepositoryCodeDuplicateDocumentsDeletionOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 28:
                    if (changeType == "UpdateIndexingUnitProperties")
                      return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                }
              }
              else
                break;
            }
            else
              break;
            break;
          case 16:
            if (indexingUnitType == "CustomRepository")
            {
              switch (indexingUnitChangeEvent.ChangeType)
              {
                case "BeginBulkIndex":
                  return executionContext.RepositoryIndexingUnit.IndexingUnitType == "CustomRepository" ? (IRunnable<OperationResult>) new CustomRepositoryIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent) : (IRunnable<OperationResult>) new GitRepositoryCodeIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "UpdateIndexingUnitProperties":
                  return (IRunnable<OperationResult>) new UpdateIndexingUnitPropertiesOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "AssignRouting":
                  return (IRunnable<OperationResult>) new CodeEntityRoutingAssignmentOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "DeleteDuplicateDocuments":
                  return (IRunnable<OperationResult>) new RepositoryCodeDuplicateDocumentsDeletionOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "Delete":
                  return (IRunnable<OperationResult>) new CustomRepositoryDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
              }
            }
            else
              break;
            break;
          case 18:
            if (indexingUnitType == "ScopedIndexingUnit")
            {
              string changeType = indexingUnitChangeEvent.ChangeType;
              if (changeType != null)
              {
                switch (changeType.Length)
                {
                  case 5:
                    if (changeType == "Patch")
                      return (IRunnable<OperationResult>) new RepositoryCodePatchOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 6:
                    if (changeType == "Delete")
                      return (IRunnable<OperationResult>) new ScopedIndexingUnitDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 11:
                    if (changeType == "UpdateIndex")
                      break;
                    goto label_142;
                  case 12:
                    if (changeType == "BranchDelete")
                      return (IRunnable<OperationResult>) new ScopedIndexingUnitBranchDeleteOperation((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 13:
                    if (changeType == "AssignRouting")
                      return (IRunnable<OperationResult>) new CodeEntityRoutingAssignmentOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_142;
                  case 14:
                    switch (changeType[0])
                    {
                      case 'B':
                        if (changeType == "BeginBulkIndex")
                          break;
                        goto label_142;
                      case 'U':
                        if (changeType == "UpdateMetadata")
                          return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                        goto label_142;
                      default:
                        goto label_142;
                    }
                    break;
                  default:
                    goto label_142;
                }
                return executionContext.RepositoryIndexingUnit.IndexingUnitType == "CustomRepository" ? (IRunnable<OperationResult>) new CustomRepositoryIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent) : (IRunnable<OperationResult>) new GitRepositoryCodeIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
              }
              break;
            }
            break;
          case 21:
            if (indexingUnitType == "TemporaryIndexingUnit")
            {
              switch (indexingUnitChangeEvent.ChangeType)
              {
                case "BeginBulkIndex":
                case "UpdateIndex":
                  return executionContext.RepositoryIndexingUnit.IndexingUnitType == "CustomRepository" ? (IRunnable<OperationResult>) new CustomRepositoryIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent) : (IRunnable<OperationResult>) new GitRepositoryCodeIndexOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
                case "Delete":
                  return (IRunnable<OperationResult>) new CodeTemporaryIndexingUnitDeleteOperation((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent);
              }
            }
            else
              break;
            break;
        }
      }
label_142:
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Operation class for Indexing Unit Kind: {0}, Indexing Unit Type: {1}, ChangeType: {2}", (object) indexingUnit.EntityType.Name, (object) indexingUnit.IndexingUnitType, (object) indexingUnitChangeEvent.ChangeType)));
    }
  }
}
