// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ISearchIndex
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public interface ISearchIndex
  {
    IndexIdentity IndexIdentity { get; }

    IndexOperationsResponse BulkIndexSync<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> bulkIndexSyncRequest)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse BulkScriptUpdateSync<T>(
      ExecutionContext executionContext,
      BulkScriptUpdateRequest<T> bulkScriptUpdateRequest)
      where T : AbstractSearchDocumentContract;

    EntitySearchGetDocumentsResponse<T> GetDocuments<T>(
      ExecutionContext executionContext,
      EntitySearchGetDocumentsRequest request)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse BulkUpdateSync<T>(
      ExecutionContext executionContext,
      BulkUpdateRequest<T> bulkUpdateRequest)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse BulkDelete<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> bulkDeleteRequest)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse BulkDeleteByQuery<T>(
      ExecutionContext executionContext,
      BulkDeleteByQueryRequest<T> bulkDeleteByQueryRequest,
      bool forceComplete)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse BulkScriptUpdateByQuery<T>(
      ExecutionContext executionContext,
      BulkScriptUpdateByQueryRequest<T> scriptUpdateByQueryRequest)
      where T : AbstractSearchDocumentContract;

    IndexOperationsResponse Refresh(ExecutionContext executionContext);

    IIndexSettings GetSettings();

    bool UpdateSettings(ExecutionContext executionContext, string property, object newValue);

    HealthStatus GetHealth();

    int GetNumberOfCollections(DocumentContractType contractType);

    long GetIndexedDocumentCount();

    IEnumerable<DocumentContractType> GetDocumentContracts(ExecutionContext executionContext);

    IndexOperationsResponse BulkUpdateByQuery(
      ExecutionContext executionContext,
      BulkUpdateByQueryRequest request);

    IEnumerable<T> BulkGetByQuery<T>(
      ExecutionContext executionContext,
      BulkGetByQueryRequest bulkGetByQueryRequest)
      where T : AbstractSearchDocumentContract;

    IDictionary<string, object> GetMetadata();
  }
}
