// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.SourceNoDedupeFileContractV5
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class SourceNoDedupeFileContractV5 : SourceNoDedupeFileContractV4
  {
    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.SourceNoDedupeFileContractV5;

    public SourceNoDedupeFileContractV5()
    {
    }

    public SourceNoDedupeFileContractV5(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
    }

    public override IndexSettings GetCodeIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings codeIndexSettings = base.GetCodeIndexSettings(executionContext, numPrimaries, numReplicas, refreshInterval);
      codeIndexSettings.Analysis.Analyzers["contentanalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "contenttokenizer",
        Filter = (IEnumerable<string>) new string[2]
        {
          "customtokenfilter",
          "lowercase"
        }
      };
      codeIndexSettings.Analysis.Analyzers["reversetextanalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "wordtokenizer",
        Filter = (IEnumerable<string>) new string[3]
        {
          "reverse",
          "customtokenfilter",
          "lowercase"
        }
      };
      codeIndexSettings.Analysis.Analyzers["ngramanalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "ngramtokenizer",
        Filter = (IEnumerable<string>) new string[1]
        {
          "lowercase"
        }
      };
      return codeIndexSettings;
    }

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<SourceNoDedupeFileContractV5>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<SourceNoDedupeFileContractV5>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<SourceNoDedupeFileContractV5>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      return (IEnumerable<CodeFileContract>) searchIndex.BulkGetByQuery<SourceNoDedupeFileContractV5>(executionContext, request);
    }

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() || this.ShouldGetCodeSnippetWithHighlightHit(requestContext);

    protected bool ShouldGetCodeSnippetWithHighlightHit(IVssRequestContext requestContext) => requestContext.Items.ContainsKey("includeSnippetInCodeSearchKey") && requestContext.IsFeatureEnabled("Search.Server.Code.IncludeSnippetInHits");

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      string inputHighlighter = !this.ShouldGetCodeSnippetWithHighlightHit(requestContext) ? (!requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() ? "noPayloadCodeSearchHighlighter" : "noPayloadCodeSearchHighlighter_v2") : "noPayloadCodeSearchHighlighter_v3";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
    }

    protected override string GetOriginalContent(byte[] originalContent) => new TextEncoding().GetString(originalContent);
  }
}
