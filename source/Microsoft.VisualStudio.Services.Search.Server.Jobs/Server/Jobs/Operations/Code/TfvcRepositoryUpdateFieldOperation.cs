// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.TfvcRepositoryUpdateFieldOperation
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class TfvcRepositoryUpdateFieldOperation : AbstractIndexingOperation
  {
    private CodeFileContract m_codeFileContract;

    public TfvcRepositoryUpdateFieldOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, (CodeFileContract) AbstractSearchDocumentContract.CreateContract(((IndexingExecutionContext) executionContext).ProvisioningContext.ContractType))
    {
    }

    internal TfvcRepositoryUpdateFieldOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      CodeFileContract codeFileContract)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_codeFileContract = codeFileContract;
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Tracer.TraceEnter(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) executionContext;
      try
      {
        int batchSize = this.GetBatchSize();
        DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
        IExpression queryExpression = this.GetQueryExpression(indexingExecutionContext);
        IndexInfo indexInfo = this.IndexingUnit.GetIndexInfo();
        if (indexInfo != null && !string.IsNullOrWhiteSpace(indexInfo.IndexName))
        {
          BulkGetByQueryRequest request = new BulkGetByQueryRequest(indexInfo, queryExpression, contractType, (IEnumerable<string>) this.GetFieldsToQuery(this.m_codeFileContract), batchSize, TimeSpan.FromMinutes(5.0));
          IndexIdentity index1 = indexingExecutionContext.GetIndex();
          ISearchIndex index2 = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
          IEnumerable<CodeFileContract> byQuery = this.m_codeFileContract.BulkGetByQuery((ExecutionContext) indexingExecutionContext, index2, request);
          int num = 0;
          if (byQuery != null)
          {
            List<CodeFileContract> codeFileContractList = new List<CodeFileContract>(batchSize);
            foreach (CodeFileContract document in byQuery)
            {
              this.UpdatePartialDocument(indexingExecutionContext, document);
              codeFileContractList.Add(document);
              if (codeFileContractList.Count == batchSize)
              {
                index2.BulkUpdateSync<CodeFileContract>((ExecutionContext) indexingExecutionContext, new BulkUpdateRequest<CodeFileContract>()
                {
                  ContractType = indexingExecutionContext.ProvisioningContext.ContractType,
                  Batch = (IEnumerable<CodeFileContract>) codeFileContractList,
                  IndexIdentity = indexingExecutionContext.GetIndex(),
                  Routing = indexingExecutionContext.IndexingUnit.GetIndexInfo().Routing,
                  ShouldUpsert = false
                });
                num += codeFileContractList.Count;
                codeFileContractList = new List<CodeFileContract>(batchSize);
              }
            }
            if (codeFileContractList.Count > 0)
            {
              index2.BulkUpdateSync<CodeFileContract>((ExecutionContext) indexingExecutionContext, new BulkUpdateRequest<CodeFileContract>()
              {
                ContractType = indexingExecutionContext.ProvisioningContext.ContractType,
                Batch = (IEnumerable<CodeFileContract>) codeFileContractList,
                IndexIdentity = indexingExecutionContext.GetIndex(),
                Routing = indexingExecutionContext.IndexingUnit.GetIndexInfo().Routing,
                ShouldUpsert = false
              });
              num += codeFileContractList.Count;
            }
          }
          indexingExecutionContext.IndexingUnit.Properties.Name = "$/" + indexingExecutionContext.ProjectIndexingUnit.Properties.Name;
          ((CodeRepoTFSAttributes) indexingExecutionContext.IndexingUnit.TFSEntityAttributes).RepositoryName = "$/" + indexingExecutionContext.ProjectIndexingUnit.Properties.Name;
          indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit);
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully updated {0} documents in TFVC Repository with the new name.", (object) num)));
        }
        else
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Index information is not available for the current indexing unit {0}, hence skipping", (object) this.IndexingUnit.IndexingUnitId)));
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Tracer.TraceLeave(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual IExpression GetQueryExpression(
      IndexingExecutionContext indexingExecutionContext)
    {
      return (IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryIndexingUnit.GetTfsEntityIdAsNormalizedString());
    }

    internal virtual List<string> GetFieldsToQuery(CodeFileContract codeFileContract) => new List<string>()
    {
      codeFileContract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.FilePath).ElasticsearchFieldName
    };

    internal virtual void UpdatePartialDocument(
      IndexingExecutionContext indexingExecutionContext,
      CodeFileContract document)
    {
      document.ProjectName = indexingExecutionContext.ProjectIndexingUnit.Properties.Name;
      document.RepoName = "$/" + document.ProjectName;
      string filePathOriginal = document.FilePathOriginal;
      if (filePathOriginal.StartsWith("$/", StringComparison.Ordinal))
      {
        int num = filePathOriginal.IndexOf(CommonConstants.DirectorySeparatorCharacter, "$/".Length);
        if (num > 0 && num != "$/".Length && num != filePathOriginal.Length - 1)
        {
          string str = filePathOriginal.Remove(0, num + 1);
          document.FilePathOriginal = "$/" + document.ProjectName + CommonConstants.DirectorySeparatorString + str;
          document.FilePath = document.CorrectFilePath(document.FilePathOriginal);
          if (!indexingExecutionContext.ProvisioningContext.ContractType.IsDedupeFileContract())
            return;
          ((DedupeFileContractBase) document).Paths = (FileGroup) null;
        }
        else
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} doesn't have a valid File Path.", (object) filePathOriginal)));
      }
      else
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} doesn't start with {1} even though it belongs to a TFVC Repository.", (object) filePathOriginal, (object) "$/")));
    }

    internal virtual int GetBatchSize() => 1000;
  }
}
