// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ISearchPlatform
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public interface ISearchPlatform
  {
    IndexOperationsResponse CreateIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity,
      IndexSettings settings,
      ITypeMapping mappings);

    BulkAliasResponse CreateAliasPointingToMultipleIndices(
      ExecutionContext executionContext,
      List<Alias> aliasesRequest);

    BulkAliasResponse RemoveAlias(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity,
      AliasIdentity aliasIdentity);

    GetAliasResponse GetAliases(ExecutionContext executionContext, AliasIdentity aliasIdentity);

    bool AliasExists(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity,
      AliasIdentity aliasIdentity);

    BulkAliasResponse SwapAlias(
      ExecutionContext executionContext,
      List<Alias> aliasAddDescriptors,
      List<Alias> aliasRemoveDescriptors);

    bool IndexExists(ExecutionContext executionContext, IndexIdentity indexIdentity);

    ISearchIndex GetIndex(IndexIdentity indexIdentity);

    ISearchQueryClient GetSearchQueryClient();

    CatResponse<CatIndicesRecord> GetIndices(
      ExecutionContext executionContext,
      List<string> indices = null);

    IndexOperationsResponse DeleteIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity);

    int GetShardId(ExecutionContext executionContext, string indexName, string routingId);

    long DeleteAllDocuments(ExecutionContext executionContext, IExpression filter);

    EntitySearchSuggestPlatformResponse Suggest<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest searchQueryRequest,
      EntitySearchSuggestPlatformRequest searchSuggestRequest,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    EntitySearchPlatformResponse Search<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest searchQueryRequest,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    ResultsCountPlatformResponse Count<T>(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    IEnumerable<Dictionary<string, string>> GetDocMetadata(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      List<string> fields,
      IExpression filter,
      DocumentContractType fileContractType,
      int numberOfHitsInOneGo);

    List<Dictionary<string, List<string>>> GetDocMetadata(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      List<string> fields,
      IExpression filter,
      DocumentContractType fileContractType,
      int numberOfHitsInOneGo,
      out string nextScrollId,
      string scrollId = null);

    long GetNumberOfHitsCount(
      IExpression filter,
      DocumentContractType fileContractType,
      IEnumerable<IndexInfo> indexInfo = null);
  }
}
