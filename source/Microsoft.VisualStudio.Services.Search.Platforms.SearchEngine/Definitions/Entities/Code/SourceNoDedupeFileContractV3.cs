// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.SourceNoDedupeFileContractV3
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class SourceNoDedupeFileContractV3 : SourceNoDedupeFileContractBase
  {
    private const string HighlighterName = "codesearch_v3";
    private const string HighlighterNameV4 = "codesearch_v4";

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.SourceNoDedupeFileContractV3;

    public bool IsDefaultBranch { get; set; }

    public SourceNoDedupeFileContractV3() => this.Initialize();

    public SourceNoDedupeFileContractV3(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.fields[CodeFileContract.CodeContractQueryableElement.CollectionName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.CollectionName);
      this.fields[CodeFileContract.CodeContractQueryableElement.ProjectName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.ProjectName);
      this.fields[CodeFileContract.CodeContractQueryableElement.RepoName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.RepoName);
      this.StoredFields = (IEnumerable<CodeContractField>) new List<CodeContractField.CodeSearchFieldDesc>()
      {
        CodeContractField.CodeSearchFieldDesc.DocumentId,
        CodeContractField.CodeSearchFieldDesc.IndexingVersion,
        CodeContractField.CodeSearchFieldDesc.OrganizationNameOriginal,
        CodeContractField.CodeSearchFieldDesc.CollectionNameOriginal,
        CodeContractField.CodeSearchFieldDesc.ProjectNameOriginal,
        CodeContractField.CodeSearchFieldDesc.RepoNameOriginal,
        CodeContractField.CodeSearchFieldDesc.BranchNameOriginal,
        CodeContractField.CodeSearchFieldDesc.FilePathOriginal,
        CodeContractField.CodeSearchFieldDesc.FileExtension,
        CodeContractField.CodeSearchFieldDesc.ChangeId,
        CodeContractField.CodeSearchFieldDesc.ContentId,
        CodeContractField.CodeSearchFieldDesc.VersionControlType
      }.Select<CodeContractField.CodeSearchFieldDesc, CodeContractField>((Func<CodeContractField.CodeSearchFieldDesc, CodeContractField>) (x => new CodeContractField(x))).ToList<CodeContractField>();
    }

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<SourceNoDedupeFileContractV3>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<SourceNoDedupeFileContractV3>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<SourceNoDedupeFileContractV3>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    internal override CodeAggregationBuilder GetAggregationBuilder(
      EntitySearchPlatformRequest searchPlatformRequest)
    {
      return (CodeAggregationBuilder) new NoPayloadCodeAggregationBuilder(searchPlatformRequest.ContractType, searchPlatformRequest.QueryParseTree, searchPlatformRequest.SearchFilters, searchPlatformRequest.Options.HasFlag((Enum) SearchOptions.Faceting));
    }

    internal override CodeFilterBuilder GetFilterBuilder(
      EntitySearchPlatformRequest searchPlatformRequest)
    {
      return (CodeFilterBuilder) new CodeFilterBuilderV3(searchPlatformRequest.QueryParseTree, searchPlatformRequest.ContractType, searchPlatformRequest.SearchFilters);
    }

    public override int GetDocumentContractSize()
    {
      int documentContractSize = base.GetDocumentContractSize();
      if (this.OriginalContent != null)
        documentContractSize += Encoding.UTF8.GetByteCount(this.OriginalContent);
      return documentContractSize;
    }

    public override void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      base.PopulateFileContractDetails(requestContext, indexingUnit, data, metaDataStore, parsedData, settings, originalContent);
      this.FileExtensionId = new float?((float) RelevanceUtility.GetFileExtensionId(this.FileExtension));
      IMetaDataStoreItem metaDataStoreItem = (IMetaDataStoreItem) data;
      this.FilePath = metaDataStoreItem.Path.NormalizePath();
      this.FilePathOriginal = metaDataStoreItem.Path;
      this.Item = this.FilePathOriginal;
      this.DocumentId = this.GetDocumentId(metaDataStore, metaDataStoreItem);
    }

    public override string GetDocumentId(
      IMetaDataStore metaDataStore,
      IMetaDataStoreItem metaDataStoreItem)
    {
      if (metaDataStoreItem == null || metaDataStoreItem.BranchesInfo == null || metaDataStoreItem.BranchesInfo.Count != 1 || !string.Equals(this.VcType, "Custom", StringComparison.OrdinalIgnoreCase))
        return base.GetDocumentId(metaDataStore, metaDataStoreItem);
      return metaDataStore["RepoId"] + "@" + this.BranchName + "@" + metaDataStoreItem.Path;
    }

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string inputHighlighter = requestContext.IsCodesearchV4HighlighterFeatureEnabled() ? "codesearch_v4" : "codesearch_v3";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
    }

    internal override string CreateCodeElementQueryString(
      string tokenKind,
      string tokenValue,
      IEnumerable<int> codeTokenIds,
      bool enableRanking,
      string requestId,
      string rewriteMethod = "top_terms_boost_100")
    {
      return this.CreateCodeElementQueryString(tokenValue, codeTokenIds, enableRanking, requestId, rewriteMethod);
    }

    public override IndexSettings GetCodeIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings codeIndexSettings = base.GetCodeIndexSettings(executionContext, numPrimaries, numReplicas, refreshInterval);
      codeIndexSettings.Analysis.Tokenizers.Add("pathtokenizer", (ITokenizer) new PathHierarchyTokenizer()
      {
        Delimiter = new char?('\\')
      });
      return codeIndexSettings;
    }

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => false;

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} doesn't have _source enabled.", (object) this.GetType().Name)));
    }

    protected internal override ITypeMapping GetMapping(
      IVssRequestContext requestContext,
      int indexVersion)
    {
      Properties properties1 = new Properties();
      PropertyName name1 = (PropertyName) CodeContractField.CodeSearchFieldDesc.DocumentId.ElasticsearchFieldName();
      KeywordProperty keywordProperty1 = new KeywordProperty();
      keywordProperty1.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.DocumentId.ElasticsearchFieldName();
      keywordProperty1.Index = new bool?(false);
      keywordProperty1.Store = new bool?(false);
      properties1.Add(name1, (IProperty) keywordProperty1);
      PropertyName name2 = (PropertyName) "item";
      KeywordProperty keywordProperty2 = new KeywordProperty();
      keywordProperty2.Name = (PropertyName) "item";
      keywordProperty2.Index = new bool?(false);
      properties1.Add(name2, (IProperty) keywordProperty2);
      PropertyName name3 = (PropertyName) "contractType";
      KeywordProperty keywordProperty3 = new KeywordProperty();
      keywordProperty3.Name = (PropertyName) "contractType";
      properties1.Add(name3, (IProperty) keywordProperty3);
      PropertyName name4 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IndexingVersion.ElasticsearchFieldName();
      KeywordProperty keywordProperty4 = new KeywordProperty();
      keywordProperty4.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IndexingVersion.ElasticsearchFieldName();
      keywordProperty4.Store = new bool?(true);
      properties1.Add(name4, (IProperty) keywordProperty4);
      PropertyName name5 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName();
      KeywordProperty keywordProperty5 = new KeywordProperty();
      keywordProperty5.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName();
      properties1.Add(name5, (IProperty) keywordProperty5);
      PropertyName name6 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathOriginal.ElasticsearchFieldName();
      KeywordProperty keywordProperty6 = new KeywordProperty();
      keywordProperty6.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathOriginal.ElasticsearchFieldName();
      keywordProperty6.Store = new bool?(true);
      properties1.Add(name6, (IProperty) keywordProperty6);
      PropertyName name7 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName();
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName();
      Properties properties2 = new Properties();
      PropertyName key = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathRaw.ElasticsearchFieldName();
      KeywordProperty keywordProperty7 = new KeywordProperty();
      keywordProperty7.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathRaw.ElasticsearchFieldName();
      properties2[key] = (IProperty) keywordProperty7;
      textProperty1.Fields = (IProperties) properties2;
      textProperty1.Analyzer = "pathanalyzer";
      textProperty1.SearchAnalyzer = "keyword";
      properties1.Add(name7, (IProperty) textProperty1);
      PropertyName name8 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName();
      KeywordProperty keywordProperty8 = new KeywordProperty();
      keywordProperty8.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName();
      keywordProperty8.Store = new bool?(true);
      properties1.Add(name8, (IProperty) keywordProperty8);
      PropertyName name9 = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      textProperty2.Analyzer = "contentanalyzer";
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.Norms = new bool?(false);
      properties1.Add(name9, (IProperty) textProperty2);
      PropertyName name10 = (PropertyName) "organizationId";
      KeywordProperty keywordProperty9 = new KeywordProperty();
      keywordProperty9.Name = (PropertyName) "organizationId";
      keywordProperty9.Store = new bool?(true);
      properties1.Add(name10, (IProperty) keywordProperty9);
      PropertyName name11 = (PropertyName) "organizationNameOriginal";
      KeywordProperty keywordProperty10 = new KeywordProperty();
      keywordProperty10.Name = (PropertyName) "organizationNameOriginal";
      keywordProperty10.Store = new bool?(true);
      properties1.Add(name11, (IProperty) keywordProperty10);
      PropertyName name12 = (PropertyName) "organizationName";
      KeywordProperty keywordProperty11 = new KeywordProperty();
      keywordProperty11.Name = (PropertyName) "organizationName";
      properties1.Add(name12, (IProperty) keywordProperty11);
      PropertyName name13 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      KeywordProperty keywordProperty12 = new KeywordProperty();
      keywordProperty12.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      keywordProperty12.Store = new bool?(true);
      properties1.Add(name13, (IProperty) keywordProperty12);
      PropertyName name14 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      KeywordProperty keywordProperty13 = new KeywordProperty();
      keywordProperty13.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      keywordProperty13.Store = new bool?(true);
      properties1.Add(name14, (IProperty) keywordProperty13);
      PropertyName name15 = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      KeywordProperty keywordProperty14 = new KeywordProperty();
      keywordProperty14.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      keywordProperty14.Store = new bool?(true);
      properties1.Add(name15, (IProperty) keywordProperty14);
      PropertyName name16 = (PropertyName) "collectionNameOriginal";
      KeywordProperty keywordProperty15 = new KeywordProperty();
      keywordProperty15.Name = (PropertyName) "collectionNameOriginal";
      keywordProperty15.Store = new bool?(true);
      properties1.Add(name16, (IProperty) keywordProperty15);
      PropertyName name17 = (PropertyName) "collectionName";
      KeywordProperty keywordProperty16 = new KeywordProperty();
      keywordProperty16.Name = (PropertyName) "collectionName";
      properties1.Add(name17, (IProperty) keywordProperty16);
      PropertyName name18 = (PropertyName) "collectionId";
      KeywordProperty keywordProperty17 = new KeywordProperty();
      keywordProperty17.Name = (PropertyName) "collectionId";
      keywordProperty17.Store = new bool?(true);
      properties1.Add(name18, (IProperty) keywordProperty17);
      PropertyName name19 = (PropertyName) "indexedTimeStamp";
      DateProperty dateProperty = new DateProperty();
      dateProperty.Name = (PropertyName) "indexedTimeStamp";
      dateProperty.Format = "epoch_second";
      dateProperty.Store = new bool?(true);
      properties1.Add(name19, (IProperty) dateProperty);
      PropertyName name20 = (PropertyName) "projectNameOriginal";
      KeywordProperty keywordProperty18 = new KeywordProperty();
      keywordProperty18.Name = (PropertyName) "projectNameOriginal";
      keywordProperty18.Store = new bool?(true);
      properties1.Add(name20, (IProperty) keywordProperty18);
      PropertyName name21 = (PropertyName) "projectName";
      KeywordProperty keywordProperty19 = new KeywordProperty();
      keywordProperty19.Name = (PropertyName) "projectName";
      properties1.Add(name21, (IProperty) keywordProperty19);
      PropertyName name22 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      KeywordProperty keywordProperty20 = new KeywordProperty();
      keywordProperty20.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      keywordProperty20.Store = new bool?(true);
      properties1.Add(name22, (IProperty) keywordProperty20);
      PropertyName name23 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      KeywordProperty keywordProperty21 = new KeywordProperty();
      keywordProperty21.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      keywordProperty21.Store = new bool?(true);
      properties1.Add(name23, (IProperty) keywordProperty21);
      PropertyName name24 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoNameOriginal.ElasticsearchFieldName();
      KeywordProperty keywordProperty22 = new KeywordProperty();
      keywordProperty22.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoNameOriginal.ElasticsearchFieldName();
      keywordProperty22.Store = new bool?(true);
      properties1.Add(name24, (IProperty) keywordProperty22);
      PropertyName name25 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      KeywordProperty keywordProperty23 = new KeywordProperty();
      keywordProperty23.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      properties1.Add(name25, (IProperty) keywordProperty23);
      PropertyName name26 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      KeywordProperty keywordProperty24 = new KeywordProperty();
      keywordProperty24.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      keywordProperty24.Store = new bool?(true);
      properties1.Add(name26, (IProperty) keywordProperty24);
      PropertyName name27 = (PropertyName) "branchNameOriginal";
      KeywordProperty keywordProperty25 = new KeywordProperty();
      keywordProperty25.Name = (PropertyName) "branchNameOriginal";
      keywordProperty25.Store = new bool?(true);
      properties1.Add(name27, (IProperty) keywordProperty25);
      PropertyName name28 = (PropertyName) "branchName";
      KeywordProperty keywordProperty26 = new KeywordProperty();
      keywordProperty26.Name = (PropertyName) "branchName";
      properties1.Add(name28, (IProperty) keywordProperty26);
      PropertyName name29 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      BooleanProperty booleanProperty = new BooleanProperty();
      booleanProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      properties1.Add(name29, (IProperty) booleanProperty);
      Properties properties3 = properties1;
      Properties properties4 = properties3;
      PropertyName name30 = (PropertyName) CodeContractField.CodeSearchFieldDesc.Content.ElasticsearchFieldName();
      TextProperty textProperty3 = new TextProperty();
      textProperty3.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.Content.ElasticsearchFieldName();
      textProperty3.Analyzer = "codesearch";
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.Norms = new bool?(false);
      properties4.Add(name30, (IProperty) textProperty3);
      Properties properties5 = properties3;
      PropertyName name31 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtensionId.ElasticsearchFieldName();
      NumberProperty numberProperty = new NumberProperty(NumberType.Float);
      numberProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtensionId.ElasticsearchFieldName();
      numberProperty.Store = new bool?(true);
      properties5.Add(name31, (IProperty) numberProperty);
      return (ITypeMapping) new TypeMapping()
      {
        Meta = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          ["version"] = (object) indexVersion
        },
        SourceField = (ISourceField) new SourceFieldDescriptor().Enabled(new bool?(false)),
        Properties = (IProperties) properties3
      };
    }

    protected override void SetIsDefaultBranch(bool isDefaultBranch) => this.IsDefaultBranch = isDefaultBranch;

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsCodesearchV4HighlighterFeatureEnabled();

    private bool ShouldParseFieldNameAsEnum(IVssRequestContext requestContext) => requestContext.IsCodesearchV4HighlighterFeatureEnabled();
  }
}
