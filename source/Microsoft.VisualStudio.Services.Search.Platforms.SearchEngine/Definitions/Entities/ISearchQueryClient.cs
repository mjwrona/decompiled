// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ISearchQueryClient
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public interface ISearchQueryClient
  {
    EntitySearchPlatformResponse Search<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      IEnumerable<EntitySearchField> storedFields,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    ResultsCountPlatformResponse Count<T>(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    EntitySearchSuggestPlatformResponse Suggest<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract;

    long GetNumberOfHitsCount(
      IExpression filter,
      DocumentContractType fileContractType,
      IEnumerable<IndexInfo> indexInfo = null);

    IEnumerable<Dictionary<string, string>> GetDocMetadata(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      List<string> fields,
      IExpression filter,
      DocumentContractType fileContractType,
      int numberOfHitsInOneGo);
  }
}
