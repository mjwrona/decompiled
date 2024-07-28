// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.RepositoryUpdateFieldOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal abstract class RepositoryUpdateFieldOperation : AbstractIndexingOperation
  {
    public RepositoryUpdateFieldOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Tracer.TraceEnter(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      string indexingUnitType = (this.IndexingUnitChangeEvent.ChangeData as EntityRenameEventData).FieldUpdateEventIndexingUnitType;
      try
      {
        if (!this.UpdateEntityNameInSearchPlatformAtRepoLevel((IndexingExecutionContext) executionContext, resultMessage))
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Item: {0} Entity rename failed on search platform", (object) indexingUnitType)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Tracer.TraceLeave(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected virtual bool UpdateEntityNameInSearchPlatformAtRepoLevel(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      bool flag = true;
      string normalizedString = this.IndexingUnit.GetTfsEntityIdAsNormalizedString();
      string indexingUnitType = ((EntityRenameEventData) this.IndexingUnitChangeEvent.ChangeData).FieldUpdateEventIndexingUnitType;
      DocumentContractType contractType1 = executionContext.ProvisioningContext.ContractType;
      AbstractSearchDocumentContract partialUpdatedDocument = this.GetPartialUpdatedDocument(executionContext, indexingUnitType, contractType1);
      IndexIdentity index1 = executionContext.GetIndex();
      ISearchIndex index2 = executionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
      IndexInfo indexInfo1 = this.IndexingUnit.GetIndexInfo();
      if (!string.IsNullOrWhiteSpace(indexInfo1?.IndexName))
      {
        IndexInfo indexInfo2 = indexInfo1;
        int contractType2 = (int) contractType1;
        IExpression expression = (IExpression) new AndExpression(new IExpression[2]
        {
          (IExpression) new TermExpression("collectionId", Operator.Equals, executionContext.CollectionId.ToString().ToLowerInvariant()),
          (IExpression) new TermExpression("repositoryId", Operator.Equals, normalizedString)
        });
        AbstractSearchDocumentContract updatedPartialAbstractSearchDocument = partialUpdatedDocument;
        IExpression query = expression;
        BulkUpdateByQueryRequest request = new BulkUpdateByQueryRequest(indexInfo2, (DocumentContractType) contractType2, updatedPartialAbstractSearchDocument, query);
        IndexOperationsResponse operationsResponse = index2.BulkUpdateByQuery((ExecutionContext) executionContext, request);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "BulkUpdateByQuery", (object) operationsResponse)));
        flag = operationsResponse.Success;
      }
      else
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("{0} Skipping update request as there is no Index Information present in the repository. It will be fixed by periodic catch up", (object) "BulkUpdateByQuery")));
      return flag;
    }

    protected virtual IndexInfo SelectIndexInfo(
      IndexingExecutionContext executionContext,
      IEnumerable<IndexInfo> indexInfos)
    {
      IndexInfo indexInfo = indexInfos.First<IndexInfo>();
      if (indexInfos.Count<IndexInfo>() > 1)
        indexInfo = indexInfos.Where<IndexInfo>((Func<IndexInfo, bool>) (i => i.EntityName.Equals(executionContext.CollectionIndexingUnit.TFSEntityAttributes is CollectionAttributes entityAttributes ? entityAttributes.CollectionName : (string) null, StringComparison.OrdinalIgnoreCase))).First<IndexInfo>();
      return indexInfo;
    }

    protected internal abstract AbstractSearchDocumentContract GetPartialUpdatedDocument(
      IndexingExecutionContext executionContext,
      string indexingUnitType,
      DocumentContractType contractType);
  }
}
