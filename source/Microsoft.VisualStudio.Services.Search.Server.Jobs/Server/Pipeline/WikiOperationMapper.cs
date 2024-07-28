// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.WikiOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (IOperationMapper))]
  public class WikiOperationMapper : IOperationMapper
  {
    public IEntityType SupportedEntityType => (IEntityType) WikiEntityType.GetInstance();

    public virtual int Weight => 0;

    public virtual IRunnable<OperationResult> GetOperation(
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
                  return (IRunnable<OperationResult>) new CollectionWikiPatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 13:
                if (changeType1 == "CrawlMetadata")
                  return (IRunnable<OperationResult>) new CollectionWikiMetadataCrawlOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 14:
                switch (changeType1[0])
                {
                  case 'B':
                    if (changeType1 == "BeginBulkIndex")
                      return (IRunnable<OperationResult>) new CollectionWikiBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                  case 'U':
                    if (changeType1 == "UpdateMetadata")
                      return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    break;
                }
                break;
              case 17:
                if (changeType1 == "CompleteBulkIndex")
                  return (IRunnable<OperationResult>) new CollectionWikiFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 23:
                if (changeType1 == "ExtensionBeginUninstall")
                  return (IRunnable<OperationResult>) new CollectionExtensionBeginUninstallOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 26:
                if (changeType1 == "ExtensionFinalizeUninstall")
                  return (IRunnable<OperationResult>) new CollectionExtensionFinalizeUninstallOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
              case 29:
                if (changeType1 == "GitRepositorySecurityAcesSync")
                  return (IRunnable<OperationResult>) new CollectionCodeGitRepositoryAcesSyncOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                break;
            }
          }
          else
            break;
          break;
        case "Git_Repository":
          string changeType2 = indexingUnitChangeEvent.ChangeType;
          if (changeType2 != null)
          {
            switch (changeType2.Length)
            {
              case 5:
                if (changeType2 == "Patch")
                  return (IRunnable<OperationResult>) new WikiRepositoryPatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_48;
              case 6:
                if (changeType2 == "Delete")
                  return (IRunnable<OperationResult>) new RepositoryDeleteOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_48;
              case 11:
                switch (changeType2[6])
                {
                  case 'F':
                    if (changeType2 == "UpdateField")
                      return (IRunnable<OperationResult>) new WikiRepositoryUpdateFieldOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_48;
                  case 'I':
                    if (changeType2 == "UpdateIndex")
                      break;
                    goto label_48;
                  default:
                    goto label_48;
                }
                break;
              case 12:
                if (changeType2 == "BranchDelete")
                  return (IRunnable<OperationResult>) new GitBranchDeleteOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_48;
              case 13:
                if (changeType2 == "AssignRouting")
                  return (IRunnable<OperationResult>) new WikiEntityRoutingAssignmentOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_48;
              case 14:
                if (changeType2 == "BeginBulkIndex")
                  break;
                goto label_48;
              case 17:
                if (changeType2 == "BeginEntityRename")
                  return (IRunnable<OperationResult>) new WikiRepositoryRenameOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                goto label_48;
              case 20:
                switch (changeType2[8])
                {
                  case 'B':
                    if (changeType2 == "CompleteBranchDelete")
                      return (IRunnable<OperationResult>) new CompleteGitBranchDeleteOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_48;
                  case 'E':
                    if (changeType2 == "CompleteEntityRename")
                      return (IRunnable<OperationResult>) new GitRepoRenameFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                    goto label_48;
                  default:
                    goto label_48;
                }
              default:
                goto label_48;
            }
            return (IRunnable<OperationResult>) new WikiRepositoryBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          }
          break;
        case "Project":
          switch (indexingUnitChangeEvent.ChangeType)
          {
            case "BeginEntityRename":
              return (IRunnable<OperationResult>) new WikiProjectUpdateFieldOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
            case "CompleteEntityRename":
              return (IRunnable<OperationResult>) new WikiProjectUpdateFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
            case "Delete":
              return (IRunnable<OperationResult>) new ProjectDeleteOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
            case "Patch":
              return (IRunnable<OperationResult>) new WikiProjectPatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          }
          break;
      }
label_48:
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Operation class for Indexing Unit Kind: {0}, Indexing Unit Type: {1}, ChangeType: {2}", (object) indexingUnit.EntityType, (object) indexingUnit.IndexingUnitType, (object) indexingUnitChangeEvent.ChangeType)));
    }
  }
}
