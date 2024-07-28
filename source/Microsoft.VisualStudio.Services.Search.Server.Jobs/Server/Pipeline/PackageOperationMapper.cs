// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.PackageOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (IOperationMapper))]
  public class PackageOperationMapper : IOperationMapper
  {
    public IEntityType SupportedEntityType => (IEntityType) PackageEntityType.GetInstance();

    public virtual int Weight => 0;

    public virtual IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnit indexingUnit)
    {
      if (indexingUnit.IndexingUnitType == "Collection")
      {
        string changeType = indexingUnitChangeEvent.ChangeType;
        if (changeType != null)
        {
          switch (changeType.Length)
          {
            case 5:
              if (changeType == "Patch")
                return (IRunnable<OperationResult>) new CollectionPackagePatchOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
              break;
            case 11:
              switch (changeType[0])
              {
                case 'F':
                  if (changeType == "FeedUpdates")
                    return (IRunnable<OperationResult>) new CollectionPackageUpdateFeedsOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                  break;
                case 'U':
                  if (changeType == "UpdateIndex")
                    return (IRunnable<OperationResult>) new CollectionPackageUpdateIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                  break;
              }
              break;
            case 12:
              if (changeType == "CleanUpFeeds")
                return (IRunnable<OperationResult>) new CollectionPackageCleanupFeedsOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
              break;
            case 14:
              switch (changeType[0])
              {
                case 'B':
                  if (changeType == "BeginBulkIndex")
                    return (IRunnable<OperationResult>) new CollectionPackageIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                  break;
                case 'U':
                  if (changeType == "UpdateMetadata")
                    return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
                  break;
              }
              break;
            case 17:
              if (changeType == "CompleteBulkIndex")
                return (IRunnable<OperationResult>) new CollectionPackageIndexFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
              break;
            case 20:
              if (changeType == "FeedSecurityAcesSync")
                return (IRunnable<OperationResult>) new CollectionPackageFeedSecurityAcesSyncOperation(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
              break;
          }
        }
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Operation class for Indexing Unit Kind: {0}, Indexing Unit Type: {1}, ChangeType: {2}", (object) indexingUnit.EntityType, (object) indexingUnit.IndexingUnitType, (object) indexingUnitChangeEvent.ChangeType)));
    }
  }
}
