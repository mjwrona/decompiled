// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemUpdateFieldValues
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemUpdateFieldValues : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080669, "Indexing Pipeline", "IndexingOperation");

    public CollectionWorkItemUpdateFieldValues(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionWorkItemUpdateFieldValues.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IDictionary<WorkItemField, object> valueUpdateSpecs = ((WorkItemUpdateFieldValuesEventData) this.IndexingUnitChangeEvent.ChangeData).FieldValueUpdateSpecs;
      if (valueUpdateSpecs != null)
      {
        if (valueUpdateSpecs.Any<KeyValuePair<WorkItemField, object>>())
        {
          try
          {
            if (!this.UpdateFieldValuesInSearchIndex((IndexingExecutionContext) coreIndexingExecutionContext, valueUpdateSpecs).Success)
              throw new SearchServiceException("Failed to updated work item fields.");
            operationResult.Status = OperationStatus.Succeeded;
          }
          finally
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionWorkItemUpdateFieldValues.s_traceMetadata, nameof (RunOperation));
          }
          return operationResult;
        }
      }
      coreIndexingExecutionContext.Log.Append("No fields present in spec.");
      operationResult.Status = OperationStatus.Succeeded;
      return operationResult;
    }

    private IndexOperationsResponse UpdateFieldValuesInSearchIndex(
      IndexingExecutionContext indexingExecutionContext,
      IDictionary<WorkItemField, object> fieldValueUpdateSpecs)
    {
      IndexIdentity index1 = indexingExecutionContext.GetIndex();
      ISearchIndex index2 = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
      IndexInfo indexInfo = new IndexInfo()
      {
        IndexName = index1.Name
      };
      FriendlyDictionary<string, object> fieldValueSpecForPlatform = new FriendlyDictionary<string, object>();
      foreach (KeyValuePair<WorkItemField, object> fieldValueUpdateSpec in (IEnumerable<KeyValuePair<WorkItemField, object>>) fieldValueUpdateSpecs)
      {
        FriendlyDictionary<string, object> friendlyDictionary = new FriendlyDictionary<string, object>();
        friendlyDictionary["value"] = fieldValueUpdateSpec.Value;
        WorkItemIndexedField itemIndexedField = WorkItemIndexedField.FromWitField(fieldValueUpdateSpec.Key);
        List<string> stringList = new List<string>();
        if (itemIndexedField.IsCompositeFieldEligible)
          stringList.Add(itemIndexedField.CompositePlatformFieldName);
        if (itemIndexedField.IsEligibleForNonAnalyzedIndex)
          stringList.Add(itemIndexedField.NonAnalyzedPlatformFieldName);
        if (itemIndexedField.ShouldBeIndexedAsString)
          stringList.Add(itemIndexedField.AsStringIndexedField.PlatformFieldName);
        if (stringList.Count > 0)
          friendlyDictionary["sink_fields"] = (object) stringList;
        fieldValueSpecForPlatform[itemIndexedField.PlatformFieldName] = (object) friendlyDictionary;
      }
      BulkScriptUpdateByQueryRequest<WorkItemContract> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<WorkItemContract>();
      updateByQueryRequest.ContractType = indexingExecutionContext.ProvisioningContext.ContractType;
      updateByQueryRequest.ScriptName = "update_field_values";
      updateByQueryRequest.ShouldUpsert = false;
      updateByQueryRequest.IndexIdentity = index1;
      updateByQueryRequest.Routing = indexInfo.Routing;
      updateByQueryRequest.Query = (IExpression) new TermExpression("collectionId", Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, indexingExecutionContext.CollectionId.ToString().ToLowerInvariant());
      updateByQueryRequest.GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
      {
        return new FluentDictionary<string, object>()
        {
          ["fields"] = (object) fieldValueSpecForPlatform,
          ["throw_if_field_not_found"] = (object) false
        };
      });
      BulkScriptUpdateByQueryRequest<WorkItemContract> scriptUpdateByQueryRequest = updateByQueryRequest;
      IndexOperationsResponse operationsResponse = index2.BulkScriptUpdateByQuery<WorkItemContract>((ExecutionContext) indexingExecutionContext, scriptUpdateByQueryRequest);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}].", (object) "BulkScriptUpdateByQuery", (object) operationsResponse)));
      return operationsResponse;
    }
  }
}
