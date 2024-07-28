// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.DeleteOrphanGitFilesOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class DeleteOrphanGitFilesOperation : DeleteOrphanFilesOperation
  {
    public DeleteOrphanGitFilesOperation(
      CoreIndexingExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    protected override IndexOperationsResponse PerformBulkDelete(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      string repositoryId = this.IndexingUnit.TFSEntityId.ToString().NormalizeString();
      string entityName;
      string unitType;
      if (((CodeDeleteOrphanFilesEventData) this.IndexingUnitChangeEvent.ChangeData).OldEntityName != null)
      {
        entityName = ((CodeDeleteOrphanFilesEventData) this.IndexingUnitChangeEvent.ChangeData).OldEntityName;
        unitType = ((CodeDeleteOrphanFilesEventData) this.IndexingUnitChangeEvent.ChangeData).UnitType;
      }
      else
      {
        entityName = ((CodeDeleteOrphanFilesEventData) this.IndexingUnitChangeEvent.ChangeData).OldProjectName;
        unitType = "Project";
      }
      IExpression queryExpression = this.CreateIndexOperationsMetadata(repositoryId, entityName, unitType).GetQueryExpression();
      DocumentContractType contractType = executionContext.ProvisioningContext.ContractType;
      if (contractType.IsSourceNoDedupeFileContract())
        return new IndexOperations().PerformBulkDelete(executionContext, executionContext.GetIndex(), queryExpression);
      if (!contractType.IsDedupeFileContract())
        return (IndexOperationsResponse) null;
      return new IndexOperationsResponse()
      {
        Success = true
      };
    }
  }
}
