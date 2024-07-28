// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ReIndexPatchProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class ReIndexPatchProvider
  {
    private readonly int m_batchSize;

    protected CodeFileContract CodeFileContract { get; }

    protected ReIndexPatchProvider(CodeFileContract codeFileContract, int batchSize)
    {
      this.CodeFileContract = codeFileContract;
      this.m_batchSize = batchSize;
    }

    public virtual IEnumerable<CodeFileContract> GetDocuments(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      IExpression queryExpression = this.GetQueryExpression(indexingExecutionContext);
      BulkGetByQueryRequest request = new BulkGetByQueryRequest(indexingUnit.GetIndexInfo(), queryExpression, contractType, (IEnumerable<string>) this.GetFieldsToQuery(this.CodeFileContract), this.m_batchSize, TimeSpan.FromMinutes(5.0));
      ISearchIndex index = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(indexingExecutionContext.GetIndex());
      foreach (CodeFileContract document in this.CodeFileContract.BulkGetByQuery((ExecutionContext) indexingExecutionContext, index, request))
        yield return document;
    }

    public abstract IExpression GetQueryExpression(IndexingExecutionContext indexingExecutionContext);

    public abstract List<string> GetFieldsToQuery(CodeFileContract codeFileContract);
  }
}
