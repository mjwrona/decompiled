// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.WorkItemIndexUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal sealed class WorkItemIndexUpdater : AbstractIndexUpdater
  {
    private readonly IVssDateTimeProvider m_dateTimeProvider;
    private static readonly string s_changedDateContractFieldName = WorkItemIndexedField.FromWitField("System.ChangedDate", WorkItemContract.FieldType.DateTime).ContractFieldName;

    public WorkItemIndexUpdater(
      IndexingExecutionContext indexingExecutionContext,
      IndexSubScope subScope,
      ISearchIndex searchIndex,
      RouteLevel routingLevel,
      DocumentContractType docContractType)
      : base(indexingExecutionContext, subScope, searchIndex, routingLevel, docContractType)
    {
      this.m_dateTimeProvider = VssDateTimeProvider.DefaultProvider;
    }

    internal WorkItemIndexUpdater(
      IndexingExecutionContext indexingExecutionContext,
      IndexSubScope subScope,
      ISearchIndex searchIndex,
      RouteLevel routingLevel,
      DocumentContractType docContractType,
      IVssDateTimeProvider dateTimeProvider)
      : this(indexingExecutionContext, subScope, searchIndex, routingLevel, docContractType)
    {
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public override IndexOperationsResponse InsertBatch(
      IEnumerable<AbstractSearchDocumentContract> batch)
    {
      return this.SearchIndex.BulkScriptUpdateSync<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, new BulkScriptUpdateRequest<AbstractSearchDocumentContract>()
      {
        Batch = batch,
        IndexIdentity = this.SearchIndex.IndexIdentity,
        Routing = this.RoutingInfo,
        ContractType = this.DocumentContractType,
        ScriptName = "update_work_item",
        ShouldUpsert = true,
        ScriptedUpsert = true,
        GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
        {
          return contract is WorkItemContract workItemContract2 ? new FluentDictionary<string, object>()
          {
            {
              "isDiscussionOnly",
              (object) workItemContract2.IsDiscussionsOnlyDocument
            },
            {
              "doc",
              (object) workItemContract2
            }
          } : throw new InvalidOperationException("Accepts only WorkItemContract.");
        })
      });
    }

    public override IndexOperationsResponse DeleteDocuments(
      IEnumerable<AbstractSearchDocumentContract> batch)
    {
      return this.SearchIndex.BulkDelete<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, new BulkDeleteRequest<AbstractSearchDocumentContract>()
      {
        Batch = batch,
        IndexIdentity = this.SearchIndex.IndexIdentity,
        ContractType = this.DocumentContractType,
        Routing = this.RoutingInfo
      });
    }

    public override IndexOperationsResponse DeleteDocumentsByQuery(
      IExpression query,
      bool forceComplete,
      bool leniant = false)
    {
      return this.SearchIndex.BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>(query, this.DocumentContractType)
      {
        Lenient = leniant
      }, forceComplete);
    }

    public override void PublishTimeToIndex(
      IEnumerable<AbstractSearchDocumentContract> documents)
    {
      DateTime indexingCompletionTime = this.m_dateTimeProvider.UtcNow;
      IEnumerable<double> source = documents.Where<AbstractSearchDocumentContract>((Func<AbstractSearchDocumentContract, bool>) (doc => (doc as WorkItemContract).Fields.ContainsKey(WorkItemIndexUpdater.s_changedDateContractFieldName))).Select<AbstractSearchDocumentContract, double>((Func<AbstractSearchDocumentContract, double>) (doc => (indexingCompletionTime - (DateTime) (doc as WorkItemContract).Fields[WorkItemIndexUpdater.s_changedDateContractFieldName]).TotalSeconds));
      if (!source.Any<double>())
        return;
      Tracer.PublishKpi("AverageTimeToIndexWorkItemInSec", "Indexing Pipeline", source.Average());
    }
  }
}
