// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.SourceNoDedupeFileContractV4
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class SourceNoDedupeFileContractV4 : SourceNoDedupeFileContractBase
  {
    internal NoPayloadContractUtils noPayloadContract;

    [Keyword(Ignore = true)]
    public override string DocumentId { get; set; }

    [Keyword(Ignore = true)]
    public override string Item { get; set; }

    [Keyword(Ignore = true)]
    public override string CollectionNameOriginal { get; set; }

    [Keyword(Ignore = true)]
    public override string ProjectNameOriginal { get; set; }

    [Keyword(Ignore = true)]
    public override string RepoNameOriginal { get; set; }

    public bool? IsDefaultBranch { get; set; }

    [Nest.Text(Name = "class")]
    public string Class { get; set; }

    [Keyword(Name = "def")]
    public string Definition { get; set; }

    [Keyword(Name = "ref")]
    public string Reference { get; set; }

    [Keyword(Name = "method")]
    public string Method { get; set; }

    [Keyword(Name = "strlit")]
    public string StringLiteral { get; set; }

    [Keyword(Name = "enum")]
    public string Enum { get; set; }

    [Keyword(Name = "basetype")]
    public string BaseType { get; set; }

    [Keyword(Name = "decl")]
    public string Declaration { get; set; }

    [Keyword(Name = "namespace")]
    public string Namespace { get; set; }

    [Keyword(Name = "type")]
    public string Type { get; set; }

    [Keyword(Name = "interface")]
    public string Interface { get; set; }

    [Keyword(Name = "comment")]
    public string Comment { get; set; }

    [Keyword(Name = "macro")]
    public string Macro { get; set; }

    [Keyword(Name = "field")]
    public string Field { get; set; }

    [Keyword(Name = "language")]
    public string Language { get; set; }

    [Boolean(Name = "isDocumentDeletedInReIndexing")]
    public bool IsDocumentDeletedDuringReindexing { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.SourceNoDedupeFileContractV4;

    public SourceNoDedupeFileContractV4() => this.Initialize();

    public SourceNoDedupeFileContractV4(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.StoredFields = (IEnumerable<CodeContractField>) new List<CodeContractField.CodeSearchFieldDesc>()
      {
        CodeContractField.CodeSearchFieldDesc.CollectionNameRaw,
        CodeContractField.CodeSearchFieldDesc.ProjectNameRaw,
        CodeContractField.CodeSearchFieldDesc.RepoNameRaw,
        CodeContractField.CodeSearchFieldDesc.BranchNameOriginal,
        CodeContractField.CodeSearchFieldDesc.FilePathOriginal,
        CodeContractField.CodeSearchFieldDesc.FileExtension,
        CodeContractField.CodeSearchFieldDesc.ChangeId,
        CodeContractField.CodeSearchFieldDesc.ContentId,
        CodeContractField.CodeSearchFieldDesc.VersionControlType
      }.Select<CodeContractField.CodeSearchFieldDesc, CodeContractField>((Func<CodeContractField.CodeSearchFieldDesc, CodeContractField>) (x => new CodeContractField(x))).ToList<CodeContractField>();
      this.noPayloadContract = new NoPayloadContractUtils(this.fields);
    }

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<SourceNoDedupeFileContractV4>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<SourceNoDedupeFileContractV4>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<SourceNoDedupeFileContractV4>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<SourceNoDedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<SourceNoDedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;

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
      IMetaDataStoreItem metaDataStoreItem = data as IMetaDataStoreItem;
      if (metaDataStoreItem.UpdateType == MetaDataStoreUpdateType.Delete)
      {
        this.IsDocumentDeletedDuringReindexing = true;
        this.Content = this.noPayloadContract.GetEmptyParsedContent();
        this.OriginalContent = string.Empty;
      }
      Dictionary<string, byte[]> fields = parsedData.Fields;
      if (fields != null)
      {
        this.Class = this.noPayloadContract.GetTheFieldValueAsString("Class", fields);
        this.Definition = this.noPayloadContract.GetTheFieldValueAsString("Definition", fields);
        this.Reference = this.noPayloadContract.GetTheFieldValueAsString("Reference", fields);
        this.Method = this.noPayloadContract.GetTheFieldValueAsString("Method", fields);
        this.StringLiteral = this.noPayloadContract.GetTheFieldValueAsString("StringLiteral", fields);
        this.Enum = this.noPayloadContract.GetTheFieldValueAsString("Enum", fields);
        this.BaseType = this.noPayloadContract.GetTheFieldValueAsString("BaseType", fields);
        this.Declaration = this.noPayloadContract.GetTheFieldValueAsString("Declaration", fields);
        this.Namespace = this.noPayloadContract.GetTheFieldValueAsString("Namespace", fields);
        this.Type = this.noPayloadContract.GetTheFieldValueAsString("Type", fields);
        this.Interface = this.noPayloadContract.GetTheFieldValueAsString("Interface", fields);
        this.Comment = this.noPayloadContract.GetTheFieldValueAsString("Comment", fields);
        this.Macro = this.noPayloadContract.GetTheFieldValueAsString("Macro", fields);
        this.Field = this.noPayloadContract.GetTheFieldValueAsString("Field", fields);
      }
      this.Language = ProgrammingLanguages.GetProgrammingLanguage(this.FileExtension).GetProgrammingLanguageDisplayName();
      FileAttributes fileAttributes = new FileAttributes(metaDataStoreItem.Path, indexingUnit.IndexingUnitType, this.ContractType);
      this.FilePathOriginal = fileAttributes.OriginalFilePath;
      this.FilePath = this.CorrectFilePath(fileAttributes.NormalizedFilePath);
      this.Item = this.FilePathOriginal;
      this.CollectionName = metaDataStore["CollectionName"];
      this.ProjectName = metaDataStore["ProjectName"];
      this.RepoName = metaDataStore["RepoName"];
      this.IndexingVersion = settings.IndexingVersion.Major.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.CollectionNameOriginal = (string) null;
      this.ProjectNameOriginal = (string) null;
      this.RepoNameOriginal = (string) null;
    }

    protected override void SetIsDefaultBranch(bool isDefaultBranch) => this.IsDefaultBranch = new bool?(isDefaultBranch);

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled();

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      string inputHighlighter = requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() ? "noPayloadCodeSearchHighlighter_v2" : "noPayloadCodeSearchHighlighter";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
    }

    public override int GetDocumentContractSize()
    {
      int documentContractSize = base.GetDocumentContractSize();
      if (this.OriginalContent != null)
        documentContractSize += Encoding.UTF8.GetByteCount(this.OriginalContent);
      return documentContractSize;
    }

    protected internal override IEnumerable<string> CorrectPathFilter(
      IEnumerable<string> filterValues)
    {
      return filterValues.Select<string, string>(new Func<string, string>(((CodeFileContract) this).CorrectFilePath));
    }

    public override string CorrectFilePath(string filePath) => this.noPayloadContract.CorrectFilePath(filePath);

    public override bool IsOriginalContentAvailable(IVssRequestContext requestContext) => true;

    internal override string CreateTermQueryStringForRegexTrigramType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      return this.CreateBoolShouldQueryStringWithoutCEFieldsBoosting(requestContext, termExpression);
    }

    internal override string ConvertToTrigramPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (termExpression.Value.Length < 3)
        throw new NotSupportedException("Substring Search for less than 3 characters in trigram is not supported.");
      return ElasticsearchQueryBuilder.BuildMatchPhraseQuery("originalContent.trigram", termExpression.Value);
    }

    internal override string CreateTermQueryStringForDefaultType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      return termExpression.Value.Contains("?") || termExpression.Value.Contains("*") ? this.CreateTermQueryString(requestContext, termExpression, enableRanking, requestId) : this.CreateBoolShouldQueryStringWithCEFieldsBoosting(requestContext, termExpression);
    }

    internal string CreateBoolShouldQueryStringWithCEFieldsBoosting(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      string str = ElasticsearchQueryBuilder.BuildTermQuery(this.GetSearchFieldForType(termExpression.Type), termExpression.Value);
      List<string> childQueries = new List<string>();
      childQueries.Add(str);
      if (requestContext.IsFeatureEnabled("Search.Server.Code.NoPayloadCodeSearchHighlighterV2"))
      {
        foreach (string fieldName in NoPayloadContractUtils.KnownFieldsTypeForHighlightHit)
          childQueries.Add(ElasticsearchQueryBuilder.BuildTermQuery(fieldName, termExpression.Value));
      }
      childQueries.AddRange((IEnumerable<string>) this.noPayloadContract.AddTermQueryOnCEFieldWithBoost(requestContext, termExpression));
      return ElasticsearchQueryBuilder.BuildBoolShouldQuery(childQueries);
    }

    internal string CreateBoolShouldQueryStringWithoutCEFieldsBoosting(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      return ElasticsearchQueryBuilder.BuildTermQuery(!termExpression.IsOfType("regex") || !requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramSearch", TeamFoundationHostType.ProjectCollection) ? this.GetSearchFieldForType(termExpression.Type) : "originalContent.trigram", termExpression.Value);
    }

    public override string GetSearchFieldForCodeElementFilterType(string tokenKind) => this.noPayloadContract.GetSearchFieldForCodeElementFilterType(tokenKind);

    internal override string CreateCodeElementQueryString(
      string tokenKind,
      string tokenValue,
      IEnumerable<int> codeTokenIds,
      bool enableRanking,
      string requestId,
      string rewriteMethod = "top_terms_boost_100")
    {
      return this.noPayloadContract.CreateCodeElementQueryString(tokenKind, tokenValue, rewriteMethod);
    }

    internal override CodeAggregationBuilder GetAggregationBuilder(
      EntitySearchPlatformRequest searchPlatformRequest)
    {
      return (CodeAggregationBuilder) new NoPayloadCodeAggregationBuilder(searchPlatformRequest.ContractType, searchPlatformRequest.QueryParseTree, searchPlatformRequest.SearchFilters, searchPlatformRequest.Options.HasFlag((System.Enum) SearchOptions.Faceting));
    }

    internal override CodeFilterBuilder GetFilterBuilder(
      EntitySearchPlatformRequest searchPlatformRequest)
    {
      return new CodeFilterBuilder(searchPlatformRequest.QueryParseTree, searchPlatformRequest.ContractType, searchPlatformRequest.SearchFilters);
    }

    public override IndexSettings GetCodeIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings codeIndexSettings = base.GetCodeIndexSettings(executionContext, numPrimaries, numReplicas, refreshInterval);
      this.noPayloadContract.AddNonPayloadIndexSettings(codeIndexSettings, executionContext, "/Service/ALMSearch/Settings/IndexSortingFieldsInNoPayloadMapping");
      return codeIndexSettings;
    }

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      return (IEnumerable<CodeFileContract>) searchIndex.BulkGetByQuery<SourceNoDedupeFileContractV4>(executionContext, request);
    }

    protected internal override ITypeMapping GetMapping(
      IVssRequestContext requestContext,
      int indexVersion)
    {
      Properties properties1 = new Properties();
      PropertyName name1 = (PropertyName) "contractType";
      KeywordProperty keywordProperty1 = new KeywordProperty();
      keywordProperty1.Name = (PropertyName) "contractType";
      keywordProperty1.Index = new bool?(true);
      keywordProperty1.Store = new bool?(false);
      keywordProperty1.DocValues = new bool?(false);
      properties1.Add(name1, (IProperty) keywordProperty1);
      PropertyName name2 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IndexingVersion.ElasticsearchFieldName();
      KeywordProperty keywordProperty2 = new KeywordProperty();
      keywordProperty2.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IndexingVersion.ElasticsearchFieldName();
      keywordProperty2.Index = new bool?(true);
      keywordProperty2.Store = new bool?(false);
      keywordProperty2.DocValues = new bool?(false);
      properties1.Add(name2, (IProperty) keywordProperty2);
      PropertyName name3 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName();
      KeywordProperty keywordProperty3 = new KeywordProperty();
      keywordProperty3.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName();
      keywordProperty3.Index = new bool?(true);
      keywordProperty3.Store = new bool?(false);
      keywordProperty3.DocValues = new bool?(true);
      properties1.Add(name3, (IProperty) keywordProperty3);
      PropertyName name4 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathOriginal.ElasticsearchFieldName();
      KeywordProperty keywordProperty4 = new KeywordProperty();
      keywordProperty4.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathOriginal.ElasticsearchFieldName();
      keywordProperty4.Index = new bool?(false);
      keywordProperty4.Store = new bool?(true);
      keywordProperty4.DocValues = new bool?(false);
      properties1.Add(name4, (IProperty) keywordProperty4);
      PropertyName name5 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName();
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName();
      Properties properties2 = new Properties();
      PropertyName key1 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathRaw.ElasticsearchFieldName();
      KeywordProperty keywordProperty5 = new KeywordProperty();
      keywordProperty5.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FilePathRaw.ElasticsearchFieldName();
      keywordProperty5.Index = new bool?(false);
      keywordProperty5.Store = new bool?(false);
      keywordProperty5.DocValues = new bool?(true);
      properties2[key1] = (IProperty) keywordProperty5;
      textProperty1.Fields = (IProperties) properties2;
      textProperty1.Analyzer = "pathanalyzer";
      textProperty1.SearchAnalyzer = "keyword";
      textProperty1.Index = new bool?(true);
      textProperty1.Store = new bool?(false);
      properties1.Add(name5, (IProperty) textProperty1);
      PropertyName name6 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName();
      KeywordProperty keywordProperty6 = new KeywordProperty();
      keywordProperty6.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName();
      keywordProperty6.Index = new bool?(true);
      keywordProperty6.Store = new bool?(false);
      keywordProperty6.DocValues = new bool?(false);
      properties1.Add(name6, (IProperty) keywordProperty6);
      PropertyName name7 = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      textProperty2.Analyzer = "contentanalyzer";
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.Norms = new bool?(false);
      textProperty2.Index = new bool?(true);
      textProperty2.Store = new bool?(false);
      textProperty2.Fields = (IProperties) this.GetFieldsForOriginalContent(requestContext);
      properties1.Add(name7, (IProperty) textProperty2);
      PropertyName name8 = (PropertyName) "organizationId";
      KeywordProperty keywordProperty7 = new KeywordProperty();
      keywordProperty7.Name = (PropertyName) "organizationId";
      keywordProperty7.Index = new bool?(true);
      keywordProperty7.Store = new bool?(true);
      keywordProperty7.DocValues = new bool?(true);
      properties1.Add(name8, (IProperty) keywordProperty7);
      PropertyName name9 = (PropertyName) "organizationName";
      KeywordProperty keywordProperty8 = new KeywordProperty();
      keywordProperty8.Name = (PropertyName) "organizationName";
      keywordProperty8.Index = new bool?(false);
      keywordProperty8.Store = new bool?(false);
      keywordProperty8.DocValues = new bool?(false);
      keywordProperty8.Normalizer = "lowerCaseNormalizer";
      Properties properties3 = new Properties();
      PropertyName key2 = (PropertyName) "raw";
      KeywordProperty keywordProperty9 = new KeywordProperty();
      keywordProperty9.Name = (PropertyName) "raw";
      keywordProperty9.Index = new bool?(false);
      keywordProperty9.Store = new bool?(false);
      keywordProperty9.DocValues = new bool?(true);
      properties3[key2] = (IProperty) keywordProperty9;
      keywordProperty8.Fields = (IProperties) properties3;
      properties1.Add(name9, (IProperty) keywordProperty8);
      PropertyName name10 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      KeywordProperty keywordProperty10 = new KeywordProperty();
      keywordProperty10.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      keywordProperty10.Index = new bool?(false);
      keywordProperty10.Store = new bool?(true);
      keywordProperty10.DocValues = new bool?(false);
      properties1.Add(name10, (IProperty) keywordProperty10);
      PropertyName name11 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      KeywordProperty keywordProperty11 = new KeywordProperty();
      keywordProperty11.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      keywordProperty11.Index = new bool?(false);
      keywordProperty11.Store = new bool?(true);
      keywordProperty11.DocValues = new bool?(false);
      properties1.Add(name11, (IProperty) keywordProperty11);
      PropertyName name12 = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      KeywordProperty keywordProperty12 = new KeywordProperty();
      keywordProperty12.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      keywordProperty12.Index = new bool?(true);
      keywordProperty12.Store = new bool?(true);
      keywordProperty12.DocValues = new bool?(false);
      properties1.Add(name12, (IProperty) keywordProperty12);
      PropertyName name13 = (PropertyName) "collectionName";
      KeywordProperty keywordProperty13 = new KeywordProperty();
      keywordProperty13.Name = (PropertyName) "collectionName";
      keywordProperty13.Index = new bool?(false);
      keywordProperty13.Store = new bool?(false);
      keywordProperty13.DocValues = new bool?(false);
      keywordProperty13.Normalizer = "lowerCaseNormalizer";
      Properties properties4 = new Properties();
      PropertyName key3 = (PropertyName) "raw";
      KeywordProperty keywordProperty14 = new KeywordProperty();
      keywordProperty14.Name = (PropertyName) "raw";
      keywordProperty14.Index = new bool?(false);
      keywordProperty14.Store = new bool?(true);
      keywordProperty14.DocValues = new bool?(true);
      properties4[key3] = (IProperty) keywordProperty14;
      keywordProperty13.Fields = (IProperties) properties4;
      properties1.Add(name13, (IProperty) keywordProperty13);
      PropertyName name14 = (PropertyName) "collectionId";
      KeywordProperty keywordProperty15 = new KeywordProperty();
      keywordProperty15.Name = (PropertyName) "collectionId";
      keywordProperty15.Index = new bool?(true);
      keywordProperty15.Store = new bool?(true);
      keywordProperty15.DocValues = new bool?(true);
      properties1.Add(name14, (IProperty) keywordProperty15);
      PropertyName name15 = (PropertyName) "indexedTimeStamp";
      DateProperty dateProperty1 = new DateProperty();
      dateProperty1.Name = (PropertyName) "indexedTimeStamp";
      dateProperty1.Format = "epoch_second";
      dateProperty1.Index = new bool?(true);
      dateProperty1.Store = new bool?(true);
      dateProperty1.DocValues = new bool?(false);
      Properties properties5 = new Properties();
      PropertyName key4 = (PropertyName) "raw";
      DateProperty dateProperty2 = new DateProperty();
      dateProperty2.Name = (PropertyName) "raw";
      dateProperty2.Format = "epoch_second";
      dateProperty2.Index = new bool?(false);
      dateProperty2.Store = new bool?(false);
      dateProperty2.DocValues = new bool?(true);
      properties5[key4] = (IProperty) dateProperty2;
      dateProperty1.Fields = (IProperties) properties5;
      properties1.Add(name15, (IProperty) dateProperty1);
      PropertyName name16 = (PropertyName) "projectName";
      KeywordProperty keywordProperty16 = new KeywordProperty();
      keywordProperty16.Name = (PropertyName) "projectName";
      keywordProperty16.Index = new bool?(true);
      keywordProperty16.Store = new bool?(false);
      keywordProperty16.DocValues = new bool?(false);
      keywordProperty16.Normalizer = "lowerCaseNormalizer";
      Properties properties6 = new Properties();
      PropertyName key5 = (PropertyName) "raw";
      KeywordProperty keywordProperty17 = new KeywordProperty();
      keywordProperty17.Name = (PropertyName) "raw";
      keywordProperty17.Index = new bool?(false);
      keywordProperty17.Store = new bool?(true);
      keywordProperty17.DocValues = new bool?(true);
      properties6[key5] = (IProperty) keywordProperty17;
      keywordProperty16.Fields = (IProperties) properties6;
      properties1.Add(name16, (IProperty) keywordProperty16);
      PropertyName name17 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      KeywordProperty keywordProperty18 = new KeywordProperty();
      keywordProperty18.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      keywordProperty18.Index = new bool?(true);
      keywordProperty18.Store = new bool?(true);
      keywordProperty18.DocValues = new bool?(true);
      properties1.Add(name17, (IProperty) keywordProperty18);
      PropertyName name18 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      KeywordProperty keywordProperty19 = new KeywordProperty();
      keywordProperty19.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      keywordProperty19.Index = new bool?(false);
      keywordProperty19.Store = new bool?(true);
      keywordProperty19.DocValues = new bool?(false);
      properties1.Add(name18, (IProperty) keywordProperty19);
      PropertyName name19 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      KeywordProperty keywordProperty20 = new KeywordProperty();
      keywordProperty20.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      keywordProperty20.Index = new bool?(true);
      keywordProperty20.Store = new bool?(false);
      keywordProperty20.DocValues = new bool?(true);
      keywordProperty20.Normalizer = "lowerCaseNormalizer";
      Properties properties7 = new Properties();
      PropertyName key6 = (PropertyName) "raw";
      KeywordProperty keywordProperty21 = new KeywordProperty();
      keywordProperty21.Name = (PropertyName) "raw";
      keywordProperty21.Index = new bool?(false);
      keywordProperty21.Store = new bool?(true);
      keywordProperty21.DocValues = new bool?(true);
      properties7[key6] = (IProperty) keywordProperty21;
      keywordProperty20.Fields = (IProperties) properties7;
      properties1.Add(name19, (IProperty) keywordProperty20);
      PropertyName name20 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      KeywordProperty keywordProperty22 = new KeywordProperty();
      keywordProperty22.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      keywordProperty22.Index = new bool?(true);
      keywordProperty22.Store = new bool?(true);
      keywordProperty22.DocValues = new bool?(true);
      properties1.Add(name20, (IProperty) keywordProperty22);
      PropertyName name21 = (PropertyName) "branchNameOriginal";
      KeywordProperty keywordProperty23 = new KeywordProperty();
      keywordProperty23.Name = (PropertyName) "branchNameOriginal";
      keywordProperty23.Index = new bool?(true);
      keywordProperty23.Store = new bool?(true);
      keywordProperty23.DocValues = new bool?(true);
      properties1.Add(name21, (IProperty) keywordProperty23);
      PropertyName name22 = (PropertyName) "branchName";
      KeywordProperty keywordProperty24 = new KeywordProperty();
      keywordProperty24.Name = (PropertyName) "branchName";
      keywordProperty24.Index = new bool?(true);
      keywordProperty24.Store = new bool?(false);
      keywordProperty24.DocValues = new bool?(true);
      properties1.Add(name22, (IProperty) keywordProperty24);
      PropertyName name23 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      BooleanProperty booleanProperty = new BooleanProperty();
      booleanProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      booleanProperty.Index = new bool?(true);
      booleanProperty.Store = new bool?(false);
      booleanProperty.DocValues = new bool?(false);
      properties1.Add(name23, (IProperty) booleanProperty);
      Properties properties8 = properties1;
      this.noPayloadContract.AddNonPayloadProperties(properties8, requestContext);
      SourceFieldDescriptor sourceFieldDescriptor = new SourceFieldDescriptor().Enabled(new bool?(requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/SourceEnabledInNoPayloadMapping", TeamFoundationHostType.Deployment, true)));
      return (ITypeMapping) new TypeMapping()
      {
        Meta = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          ["version"] = (object) indexVersion
        },
        SourceField = (ISourceField) sourceFieldDescriptor,
        Properties = (IProperties) properties8,
        RoutingField = (IRoutingField) new RoutingField()
        {
          Required = new bool?(true)
        }
      };
    }

    protected virtual Properties GetFieldsForOriginalContent(IVssRequestContext requestContext)
    {
      if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramIndexing", TeamFoundationHostType.Deployment))
        return (Properties) null;
      Properties forOriginalContent = new Properties();
      PropertyName key = (PropertyName) "trigram";
      TextProperty textProperty = new TextProperty();
      textProperty.Analyzer = "ngramanalyzer";
      textProperty.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty.Norms = new bool?(false);
      textProperty.Store = new bool?(false);
      textProperty.Index = new bool?(true);
      forOriginalContent[key] = (IProperty) textProperty;
      return forOriginalContent;
    }
  }
}
