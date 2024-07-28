// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchServiceErrorCode
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public enum SearchServiceErrorCode
  {
    Unknown,
    AliasExists,
    IndexExists,
    AliasNotExists,
    IndexNotExists,
    Invalidquery_Error,
    ElasticSearch_ResponseNull_Error,
    ElasticSearch_Connection_Error,
    ElasticSearch_InvalidResponse_Error,
    Repository_Not_Found,
    Repository_Access_Denied,
    Log_Punctuations_Misplaced,
    Log_Deserialization_Failed,
    Log_Serialization_Failed,
    UnexpectedError,
    MaxAcceptableFractionOfFailedItemsInCrawlerThresholdBreached,
    MismatchInItemsToDownloadCount,
    ElasticsearchBulkDeleteFailed,
    ElasticsearchBulkUpdateFailed,
    IndexingIndexNameDoesNotExist,
    ElasticSearch_Server_Error,
    ElasticSearch_ItemsResponse_Error,
    ElasticSearch_FilesRejectedAboveThreshold_Error,
    ElasticsearchClusterStateServiceError,
  }
}
