// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeFileContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public abstract class CodeFileContract : AbstractSearchDocumentContract
  {
    internal IDictionary<CodeFileContract.CodeContractQueryableElement, CodeContractField> fields;
    private IEnumerable<CodeFileContract.CodeContractQueryableElement> m_allowedTermExpressions;
    public const string PathTokenizerName = "pathtokenizer";
    private const string CodeSearchHighlighterNameV3 = "codesearch_v3";
    private const string CodeSearchHighlighterNameV4 = "codesearch_v4";
    private const string CodeSearchHighlighterNameV3ConstantScore = "codesearch_v3_constantscore";
    private const string CodeSearchHighlighterNameV4ConstantScore = "codesearch_v4_constantscore";
    protected const string CodeIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.code";
    [StaticSafe]
    public static readonly ISet<DocumentContractType> MultiBranchSupportedContracts = (ISet<DocumentContractType>) new HashSet<DocumentContractType>()
    {
      DocumentContractType.DedupeFileContractV3,
      DocumentContractType.DedupeFileContractV4,
      DocumentContractType.DedupeFileContractV5,
      DocumentContractType.SourceNoDedupeFileContractV3,
      DocumentContractType.SourceNoDedupeFileContractV4,
      DocumentContractType.SourceNoDedupeFileContractV5
    };
    [StaticSafe]
    public static readonly ISet<DocumentContractType> MultiBranchUnsupportedContracts = (ISet<DocumentContractType>) new HashSet<DocumentContractType>();
    [StaticSafe]
    private static readonly IDictionary<DocumentContractType, CodeFileContract> s_documentContractMapping = (IDictionary<DocumentContractType, CodeFileContract>) new FriendlyDictionary<DocumentContractType, CodeFileContract>()
    {
      [DocumentContractType.DedupeFileContractV3] = (CodeFileContract) new DedupeFileContractV3(),
      [DocumentContractType.DedupeFileContractV4] = (CodeFileContract) new DedupeFileContractV4(),
      [DocumentContractType.DedupeFileContractV5] = (CodeFileContract) new DedupeFileContractV5(),
      [DocumentContractType.SourceNoDedupeFileContractV3] = (CodeFileContract) new SourceNoDedupeFileContractV3(),
      [DocumentContractType.SourceNoDedupeFileContractV4] = (CodeFileContract) new SourceNoDedupeFileContractV4(),
      [DocumentContractType.SourceNoDedupeFileContractV5] = (CodeFileContract) new SourceNoDedupeFileContractV5()
    };
    [StaticSafe]
    private static readonly Dictionary<string, string> s_constantscoreHighlighterMapping = new Dictionary<string, string>()
    {
      {
        "noPayloadCodeSearchHighlighter",
        "noPayloadCodeSearchHighlighter_constantscore"
      },
      {
        "noPayloadCodeSearchHighlighter_v2",
        "noPayloadCodeSearchHighlighter_v2_constantscore"
      },
      {
        "noPayloadCodeSearchHighlighter_v3",
        "noPayloadCodeSearchHighlighter_v3_constantscore"
      },
      {
        "codesearch_v3",
        "codesearch_v3_constantscore"
      },
      {
        "codesearch_v4",
        "codesearch_v4_constantscore"
      }
    };
    [StaticSafe]
    private static readonly ISet<DocumentContractType> s_constantscoreDocumentContractTypeExceptionList = (ISet<DocumentContractType>) new HashSet<DocumentContractType>()
    {
      DocumentContractType.DedupeFileContractV3,
      DocumentContractType.SourceNoDedupeFileContractV3
    };
    public const string ElasticSearchDocIdField = "_id";
    public const string ElasticSearchScrollIdField = "_scroll_id";
    public const string UpdateScript = "update_dedupe_file";
    public const string DeleteFileScript = "delete_dedupe_file";
    public const string DeleteFileScriptV2 = "delete_dedupe_file_v2";
    public const string ScriptOldBranchNameParam = "oldBranchName";
    public const string ScriptOldBranchNameOriginalParam = "oldBranchNameOriginal";
    public const string ScriptNewBranchNameParam = "newBranchName";
    public const string ScriptNewBranchNameOriginalParam = "newBranchNameOriginal";
    public const string ScriptNewChangeIdParam = "newChangeId";
    public const string ScriptIsDefaultBranchParam = "isDefaultBranch";
    public const string ScriptAllOriginalBranchesAndFilePathsParam = "allOriginalBranchesAndFilePaths";
    public const string ContentFieldParam = "content";
    public const string ReverseTextAnalyzer = "reversetextanalyzer";
    public const string WordTokenizer = "wordtokenizer";
    public const string FieldNameSuffixReverse = "reverse";
    public const string FieldNameSuffixTrigram = "trigram";
    public const string FieldNameSuffixNGram = "ngram";
    private const string RepositoryFilterId = "repo";
    private const string ProjectFilterId = "proj";
    private const string PathFilterId = "path";
    private const string FileExtensionFilterId = "ext";
    private const string FileNameFilterId = "file";
    private const string BranchFilterId = "branch";
    private const string FileNameFilter = "fileName";
    private const string IsDefaultBranchFilterId = "isdefaultbranch";
    private const string RegexFilterId = "regex";
    protected const string BranchRefPrefix = "refs/heads/";

    protected internal IEnumerable<CodeContractField> StoredFields { get; set; }

    public static CodeFileContract CreateCodeContract(
      DocumentContractType contractType,
      ISearchPlatform searchPlatform)
    {
      switch (contractType)
      {
        case DocumentContractType.SourceNoDedupeFileContractV3:
          return (CodeFileContract) new SourceNoDedupeFileContractV3(searchPlatform.GetSearchQueryClient());
        case DocumentContractType.DedupeFileContractV3:
          return (CodeFileContract) new DedupeFileContractV3(searchPlatform.GetSearchQueryClient());
        case DocumentContractType.SourceNoDedupeFileContractV4:
          return (CodeFileContract) new SourceNoDedupeFileContractV4(searchPlatform.GetSearchQueryClient());
        case DocumentContractType.DedupeFileContractV4:
          return (CodeFileContract) new DedupeFileContractV4(searchPlatform.GetSearchQueryClient());
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return (CodeFileContract) new SourceNoDedupeFileContractV5(searchPlatform.GetSearchQueryClient());
        case DocumentContractType.DedupeFileContractV5:
          return (CodeFileContract) new DedupeFileContractV5(searchPlatform.GetSearchQueryClient());
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type '{0}'", (object) contractType)));
      }
    }

    protected IEnumerable<CodeContractField> GetSearchableFields()
    {
      if (this.SearchElements == null || !this.SearchElements.Any<CodeFileContract.CodeContractQueryableElement>())
        return this.StoredFields;
      HashSet<CodeContractField> hashSet = this.SearchElements.Select<CodeFileContract.CodeContractQueryableElement, CodeContractField>((Func<CodeFileContract.CodeContractQueryableElement, CodeContractField>) (x => this.fields[x])).ToHashSet<CodeContractField>();
      hashSet.UnionWith(this.StoredFields);
      return (IEnumerable<CodeContractField>) hashSet;
    }

    protected IDictionary<string, IEnumerable<string>> CorrectSearchFilters(
      CodeSearchPlatformRequest request)
    {
      IDictionary<string, IEnumerable<string>> dictionary = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      if (request.SearchFilters == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "SearchFilters")), nameof (request));
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.SearchFilters)
      {
        if (searchFilter.Key.Equals("PathFilters", StringComparison.OrdinalIgnoreCase))
        {
          IEnumerable<string> strings = this.CorrectPathFilter(searchFilter.Value);
          dictionary.Add(searchFilter.Key, strings);
        }
        else if (searchFilter.Key.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))
        {
          if (request.ScopeFiltersExpression.IsNullOrEmpty<IExpression>() || request.ScopeFiltersExpression.Children.Length == 0)
            dictionary.Add(searchFilter);
        }
        else if (DocumentContractTypeExtension.IsValidCodeDocumentContractType(this.ContractType))
        {
          if (searchFilter.Key == "AccountFilters")
            dictionary.Add("CollectionFilters", searchFilter.Value);
          else
            dictionary.Add(searchFilter);
        }
        else
          dictionary.Add(searchFilter);
      }
      return dictionary;
    }

    protected internal virtual IEnumerable<string> CorrectPathFilter(
      IEnumerable<string> filterValues)
    {
      return filterValues.Select<string, string>(new Func<string, string>(this.CorrectFilePath));
    }

    public virtual string CorrectFilePath(string filePath) => filePath.NormalizePath();

    protected CodeFileContract() => this.Initialize();

    internal CodeFileContract(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.fields = (IDictionary<CodeFileContract.CodeContractQueryableElement, CodeContractField>) new Dictionary<CodeFileContract.CodeContractQueryableElement, CodeContractField>();
      foreach (CodeContractField.CodeSearchFieldDesc field in CodeSearchFieldExtensions.GetCodeSearchFieldDescsValues().Where<CodeContractField.CodeSearchFieldDesc>((Func<CodeContractField.CodeSearchFieldDesc, bool>) (elem => elem.QueryableElement() != 0)))
        this.fields[field.QueryableElement()] = new CodeContractField(field);
      this.fields[CodeFileContract.CodeContractQueryableElement.Default] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.Content);
      this.fields[CodeFileContract.CodeContractQueryableElement.Advanced] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.OriginalContent);
      this.fields[CodeFileContract.CodeContractQueryableElement.Prefix] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.Content);
      this.fields[CodeFileContract.CodeContractQueryableElement.CodeElement] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.Content);
      this.m_allowedTermExpressions = (IEnumerable<CodeFileContract.CodeContractQueryableElement>) new List<CodeFileContract.CodeContractQueryableElement>()
      {
        CodeFileContract.CodeContractQueryableElement.Default,
        CodeFileContract.CodeContractQueryableElement.Advanced,
        CodeFileContract.CodeContractQueryableElement.Prefix,
        CodeFileContract.CodeContractQueryableElement.CodeElement,
        CodeFileContract.CodeContractQueryableElement.Account,
        CodeFileContract.CodeContractQueryableElement.CollectionName,
        CodeFileContract.CodeContractQueryableElement.CollectionId,
        CodeFileContract.CodeContractQueryableElement.FileName,
        CodeFileContract.CodeContractQueryableElement.FileExtension,
        CodeFileContract.CodeContractQueryableElement.FilePath,
        CodeFileContract.CodeContractQueryableElement.ProjectName,
        CodeFileContract.CodeContractQueryableElement.RepoName,
        CodeFileContract.CodeContractQueryableElement.BranchName,
        CodeFileContract.CodeContractQueryableElement.IsDefaultBranch,
        CodeFileContract.CodeContractQueryableElement.RepositoryId,
        CodeFileContract.CodeContractQueryableElement.RepoId,
        CodeFileContract.CodeContractQueryableElement.VersionControlType,
        CodeFileContract.CodeContractQueryableElement.Content,
        CodeFileContract.CodeContractQueryableElement.OriginalContent,
        CodeFileContract.CodeContractQueryableElement.OriginalContentReverse,
        CodeFileContract.CodeContractQueryableElement.ContentId
      };
    }

    public static CodeFileContract GetContractInstance(DocumentContractType contractType) => CodeFileContract.s_documentContractMapping[contractType];

    public override string DocumentId { get; set; }

    [Keyword(Name = "item")]
    public override string Item { get; set; }

    [Keyword(Ignore = true)]
    public override string Routing { get; set; }

    [Keyword(Ignore = true)]
    public override long? PreviousDocumentVersion
    {
      get => new long?();
      set => throw new NotImplementedException();
    }

    [Keyword(Ignore = true)]
    public override long CurrentDocumentVersion { get; set; }

    public string VcType { get; set; }

    public string IndexingVersion { get; set; }

    public IEnumerable<string> FileName { get; set; }

    public string FilePathOriginal { get; set; }

    public string FilePath { get; set; }

    public string FileExtension { get; set; }

    public float? FileExtensionId { get; set; }

    public virtual string Content { get; set; }

    public string ContentId { get; set; }

    [Keyword(Ignore = true)]
    public override string ParentDocumentId
    {
      get => (string) null;
      set => throw new NotImplementedException();
    }

    public virtual void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      IMetaDataStoreItem metaDataStoreItem = data as IMetaDataStoreItem;
      if (parsedData.Content != null)
      {
        char[] destinationArray = new char[parsedData.Content.Length];
        Array.Copy((Array) parsedData.Content, (Array) destinationArray, parsedData.Content.Length);
        this.Content = new string(destinationArray);
      }
      else
        this.Content = (string) null;
      this.VcType = metaDataStore["VcType"];
      this.CollectionName = metaDataStore["NormalizedCollectionName"];
      this.CollectionNameOriginal = metaDataStore["CollectionName"];
      this.CollectionId = metaDataStore["CollectionId"].ToLowerInvariant();
      this.ProjectName = metaDataStore["NormalizedProjectName"];
      this.ProjectNameOriginal = metaDataStore["ProjectName"];
      this.ProjectId = metaDataStore["ProjectId"].ToLowerInvariant();
      this.RepoName = metaDataStore["NormalizedRepoName"];
      this.RepoNameOriginal = metaDataStore["RepoName"];
      this.RepositoryId = metaDataStore["RepoId"].ToLowerInvariant();
      this.ContentId = metaDataStoreItem.ContentId.HexHash.NormalizeString();
      this.FileExtension = metaDataStoreItem.Extension.NormalizeFileExtension();
      this.FileName = FilenameTokenizer.GetNormalizedFileTokens(metaDataStoreItem.FileName);
      this.IndexingVersion = settings.IndexingVersion.ToString();
      this.DocumentId = this.GetDocumentId(metaDataStore, metaDataStoreItem);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string GetDocumentId(
      IMetaDataStore metaDataStore,
      IMetaDataStoreItem metaDataStoreItem)
    {
      return metaDataStoreItem.DocumentId;
    }

    public abstract string GetHighlighter(IVssRequestContext requestContext);

    public static string GetBranchNameWithoutPrefix(string branchRefPrefix, string branchName)
    {
      string nameWithoutPrefix = branchName;
      if (branchName.StartsWith(branchRefPrefix, StringComparison.OrdinalIgnoreCase))
        nameWithoutPrefix = nameWithoutPrefix.Substring(branchRefPrefix.Length);
      return nameWithoutPrefix;
    }

    public virtual IndexOperationsResponse InsertBatch<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> indexRequest,
      ISearchIndex searchIndex)
      where T : AbstractSearchDocumentContract
    {
      return searchIndex.BulkIndexSync<T>(executionContext, indexRequest);
    }

    public virtual IndexOperationsResponse DeleteDocuments<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> deleteRequest,
      ISearchIndex searchIndex)
      where T : AbstractSearchDocumentContract
    {
      return searchIndex.BulkDelete<T>(executionContext, deleteRequest);
    }

    public virtual IndexOperationsResponse DeleteDocumentsByQuery<T>(
      ExecutionContext executionContext,
      BulkDeleteByQueryRequest<T> bulkDeleteByQueryRequest,
      ISearchIndex searchIndex,
      bool forceComplete)
      where T : AbstractSearchDocumentContract
    {
      return searchIndex.BulkDeleteByQuery<T>(executionContext, bulkDeleteByQueryRequest, forceComplete);
    }

    public abstract IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request);

    public override string GetFieldNameForStoredField(string storedField) => storedField;

    public override string GetStoredFieldForFieldName(string field) => field;

    protected internal abstract ITypeMapping GetMapping(
      IVssRequestContext requestContext,
      int indexVersion);

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public CodeContractField GetField(
      CodeFileContract.CodeContractQueryableElement fieldDesc)
    {
      return this.fields[fieldDesc];
    }

    public string GetSearchFieldForProximityOperators() => this.fields[CodeFileContract.CodeContractQueryableElement.Default].ElasticsearchFieldName;

    public override string GetSearchFieldForType(string type)
    {
      if (type.Equals("*", StringComparison.OrdinalIgnoreCase))
        return this.fields[CodeFileContract.CodeContractQueryableElement.Default].ElasticsearchFieldName;
      CodeFileContract.CodeContractQueryableElement key = CodeSearchFieldExtensions.GetQueryableElementValues().FirstOrDefault<CodeFileContract.CodeContractQueryableElement>((Func<CodeFileContract.CodeContractQueryableElement, bool>) (v => v.InlineFilterName() == type));
      if (key == CodeFileContract.CodeContractQueryableElement.None)
        return type;
      if (!this.m_allowedTermExpressions.Contains<CodeFileContract.CodeContractQueryableElement>(key))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082281, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Unknown type in term expression [{0}]", (object) type)));
        return type;
      }
      CodeContractField codeContractField;
      if (this.fields.TryGetValue(key, out codeContractField))
        return codeContractField.ElasticsearchFieldName;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082281, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Unknown type in term expression [{0}]", (object) type)));
      return type;
    }

    public CodeContractField GetSearchStoredFieldForType(
      CodeFileContract.CodeContractQueryableElement type)
    {
      if (!this.m_allowedTermExpressions.Contains<CodeFileContract.CodeContractQueryableElement>(type))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082281, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Unknown type in term expression [{0}]", (object) type)));
        return this.fields[type];
      }
      CodeContractField storedFieldForType;
      if (!this.fields.TryGetValue(type, out storedFieldForType))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082281, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Unknown type in term expression [{0}]", (object) type)));
        return this.fields[type];
      }
      if (storedFieldForType.StoredToFieldDesc != CodeContractField.CodeSearchFieldDesc.None)
        return new CodeContractField(storedFieldForType.StoredToFieldDesc);
      if (storedFieldForType.IsStoredField)
        return storedFieldForType;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082281, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Unknown type in term expression [{0}]", (object) type)));
      return this.fields[type];
    }

    public CodeContractField GetSearchFieldForAdvancedQuery(IVssRequestContext requestContext) => !this.IsOriginalContentAvailable(requestContext) ? this.fields[CodeFileContract.CodeContractQueryableElement.Default] : this.fields[CodeFileContract.CodeContractQueryableElement.Advanced];

    public virtual string GetSearchFieldForCodeElementFilterType(string tokenKind) => throw new NotImplementedException("Not valid for anything other than V4 contracts");

    protected virtual bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => false;

    public override int GetSize() => this.Content != null ? this.Content.Length : this.GetDocumentContractSize();

    public virtual int GetDocumentContractSize() => this.Content != null ? Encoding.UTF8.GetByteCount(this.Content) : 0;

    public virtual bool IsOriginalContentAvailable(IVssRequestContext requestContext) => requestContext.IsFTSEnabled();

    public long GetBranchDocCount(
      Guid repositoryId,
      string branch,
      IEnumerable<IndexInfo> indexInfo = null)
    {
      return this.SearchQueryClient.GetNumberOfHitsCount((IExpression) new AndExpression(new IExpression[2]
      {
        (IExpression) new TermExpression(nameof (repositoryId), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repositoryId.ToString().ToLowerInvariant()),
        (IExpression) new TermExpression(this.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.BranchName).ElasticsearchFieldName, Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/", branch))
      }), this.ContractType, indexInfo);
    }

    public virtual long GetScopePathDocCount(
      IVssRequestContext requestContext,
      Guid repositoryId,
      string scopePath,
      IEnumerable<IndexInfo> indexInfo = null)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException("Scope path cannot be null. Use a repo count query in this scenario.");
      return this.SearchQueryClient.GetNumberOfHitsCount((IExpression) new AndExpression(new IExpression[2]
      {
        (IExpression) new TermExpression(nameof (repositoryId), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repositoryId.ToString().ToLowerInvariant()),
        (IExpression) new TermExpression(this.GetSearchFieldForType(CodeFileContract.CodeContractQueryableElement.FilePath.InlineFilterName()), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, this.CorrectFilePath(scopePath))
      }), this.ContractType, indexInfo);
    }

    public virtual long GetRepoDocCount(
      IVssRequestContext requestContext,
      Guid repositoryId,
      IEnumerable<IndexInfo> indexInfo = null)
    {
      return this.SearchQueryClient.GetNumberOfHitsCount((IExpression) new TermExpression(nameof (repositoryId), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repositoryId.ToString().ToLowerInvariant()), this.ContractType, indexInfo);
    }

    public static string GetFieldName(
      MetadataType metadataType,
      DocumentContractType documentContractType)
    {
      switch (metadataType)
      {
        case MetadataType.FilePath:
          return "filePathOriginal";
        case MetadataType.FileContentId:
          return "contentId";
        case MetadataType.BranchNames:
          return AbstractSearchDocumentContract.GetBranchNameOriginalFieldName(documentContractType);
        default:
          throw new ArgumentException("Field mapping of metadataType is not present.", nameof (metadataType));
      }
    }

    public static MetadataType GetMetadataType(string fieldName)
    {
      switch (fieldName)
      {
        case "filePathOriginal":
          return MetadataType.FilePath;
        case "contentId":
          return MetadataType.FileContentId;
        case "paths.branchNameOriginal":
        case "branchNameOriginal":
        case "branchName":
          return MetadataType.BranchNames;
        default:
          throw new ArgumentException("Invalid field name. Cannot convert to existing metadata form.", nameof (fieldName));
      }
    }

    public static string GetBranchNameFieldName(DocumentContractType documentContractType) => documentContractType == DocumentContractType.DedupeFileContractV3 || documentContractType == DocumentContractType.DedupeFileContractV4 || documentContractType == DocumentContractType.DedupeFileContractV5 ? "paths.branchName" : "branchName";

    public static string GetChangeIdFieldName(DocumentContractType documentContractType) => documentContractType == DocumentContractType.DedupeFileContractV3 || documentContractType == DocumentContractType.DedupeFileContractV4 || documentContractType == DocumentContractType.DedupeFileContractV5 ? "paths.changeId" : "changeId";

    public static string GetIndexedTimeStampFieldName(DocumentContractType documentContractType) => documentContractType.IsNoPayloadContract() ? "indexedTimeStamp.raw" : "indexedTimeStamp";

    public virtual string CollectionNameOriginal { get; set; }

    public override string CollectionName { get; set; }

    public override string CollectionId { get; set; }

    public override int IndexedTimeStamp { get; set; }

    public virtual string ProjectNameOriginal { get; set; }

    public string ProjectName { get; set; }

    public string ProjectId { get; set; }

    public virtual string RepoNameOriginal { get; set; }

    public string RepoName { get; set; }

    public string RepositoryId { get; set; }

    public virtual IndexSettings GetCodeIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings indexSettings = new IndexSettings();
      indexSettings.RefreshInterval = (Time) refreshInterval;
      indexSettings.NumberOfReplicas = new int?(numReplicas);
      indexSettings.NumberOfShards = new int?(numPrimaries);
      indexSettings.Analysis = (IAnalysis) new Analysis()
      {
        Analyzers = (IAnalyzers) new Analyzers()
        {
          {
            "pathanalyzer",
            (IAnalyzer) new CustomAnalyzer()
            {
              Tokenizer = "pathtokenizer"
            }
          },
          {
            "contentanalyzer",
            (IAnalyzer) new CustomAnalyzer()
            {
              Tokenizer = "contenttokenizer",
              Filter = (IEnumerable<string>) new string[1]
              {
                "customtokenfilter"
              }
            }
          },
          {
            "reversetextanalyzer",
            (IAnalyzer) new CustomAnalyzer()
            {
              Tokenizer = "wordtokenizer",
              Filter = (IEnumerable<string>) new string[2]
              {
                "reverse",
                "customtokenfilter"
              }
            }
          },
          {
            "ngramanalyzer",
            (IAnalyzer) new CustomAnalyzer()
            {
              Tokenizer = "ngramtokenizer"
            }
          }
        },
        Tokenizers = (ITokenizers) new Tokenizers()
        {
          {
            "contenttokenizer",
            (ITokenizer) new PatternTokenizer()
            {
              Pattern = "(\\w+)|([^\\w\\s]?)",
              Group = new int?(0)
            }
          },
          {
            "wordtokenizer",
            (ITokenizer) new PatternTokenizer()
            {
              Pattern = "(\\w+)",
              Group = new int?(0)
            }
          },
          {
            "ngramtokenizer",
            (ITokenizer) new NGramTokenizer()
            {
              MinGram = new int?(3),
              MaxGram = new int?(3),
              TokenChars = (IEnumerable<TokenChar>) new TokenChar[5]
              {
                TokenChar.Letter,
                TokenChar.Digit,
                TokenChar.Punctuation,
                TokenChar.Whitespace,
                TokenChar.Symbol
              }
            }
          }
        },
        TokenFilters = (ITokenFilters) new TokenFilters()
        {
          {
            "customtokenfilter",
            (ITokenFilter) new LengthTokenFilter()
            {
              Min = new int?(1),
              Max = new int?(20000)
            }
          }
        }
      };
      IndexSettings codeIndexSettings = indexSettings;
      if (!executionContext.RequestContext.ExecutionEnvironment.IsDevFabricDeployment && !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        codeIndexSettings.Add("index.routing.allocation.include.entity.code", (object) true);
      return codeIndexSettings;
    }

    public static ITypeMapping GetCodeIndexMappings(
      IVssRequestContext requestContext,
      DocumentContractType contractType,
      int indexVersion = 0)
    {
      if (CodeFileContract.GetContractInstance(contractType) == null)
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Contract type [{0}] is not supported for code entity.", (object) contractType)));
      return CodeFileContract.GetContractInstance(contractType).GetMapping(requestContext, indexVersion);
    }

    protected internal IEnumerable<CodeFileContract.CodeContractQueryableElement> GetQueryableElements() => this.m_allowedTermExpressions;

    public static string GetRewriteMethod(
      IVssRequestContext requestContext,
      DocumentContractType documentContractType)
    {
      return requestContext != null && !CodeFileContract.s_constantscoreDocumentContractTypeExceptionList.Contains(documentContractType) && requestContext.IsWildcardConstantScoreRewriteFeatureEnabled() ? "constant_score" : "top_terms_boost_100";
    }

    public static bool IsContractTypeSupportedByConstantScoreRewrite(
      DocumentContractType documentContractType)
    {
      return !CodeFileContract.s_constantscoreDocumentContractTypeExceptionList.Contains(documentContractType);
    }

    protected internal string GetMappedHighlighter(
      IVssRequestContext requestContext,
      string inputHighlighter)
    {
      if (!requestContext.IsWildcardConstantScoreRewriteFeatureEnabled() || CodeFileContract.s_constantscoreDocumentContractTypeExceptionList.Contains(this.ContractType))
        return inputHighlighter;
      string empty = string.Empty;
      if (CodeFileContract.s_constantscoreHighlighterMapping.TryGetValue(inputHighlighter, out empty))
        return empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1083155, "Search Engine", "DocumentContractTypeService", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The highlighter {0} does not have a mapped constant score highlighter, even though the constant score feature flag is turned on", (object) inputHighlighter));
      return inputHighlighter;
    }

    public IEnumerable<CodeFileContract.CodeContractQueryableElement> SearchElements { get; set; }

    public virtual bool IsNGramIndexingEnabled(IVssRequestContext requestContext) => false;

    internal abstract string CreateCodeElementQueryString(
      string tokenType,
      string tokenValue,
      IEnumerable<int> codeTokenIds,
      bool enableRanking,
      string requestId,
      string rewriteMethod = "top_terms_boost_100");

    internal abstract CodeAggregationBuilder GetAggregationBuilder(
      EntitySearchPlatformRequest searchPlatformRequest);

    internal abstract CodeFilterBuilder GetFilterBuilder(
      EntitySearchPlatformRequest searchPlatformRequest);

    internal virtual string CreateCodeElementQueryString(
      string tokenValue,
      IEnumerable<int> codeTokenIds,
      bool enableRanking,
      string requestId,
      string rewriteMethod = "top_terms_boost_100")
    {
      return this.GetCustomizedQueryString(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"{0}\": {{\r\n                        \"{1}\": \"{2}\",\r\n                        \"{3}\": {4},\r\n                        \"{5}\": [{6}],\r\n                        \"{7}\": \"{8}\"\r\n                    }}\r\n                }}", (object) "codeelement", (object) "field", (object) this.fields[CodeFileContract.CodeContractQueryableElement.CodeElement].ElasticsearchFieldName, (object) "value", (object) JsonConvert.SerializeObject((object) tokenValue), (object) "tokenKind", (object) string.Join<int>(", ", codeTokenIds), (object) "rewrite", (object) rewriteMethod), enableRanking, requestId);
    }

    internal virtual string CreateTermQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (!termExpression.Value.Contains("?") && !termExpression.Value.Contains("*"))
        return this.GetCustomizedQueryString(ElasticsearchQueryBuilder.BuildTermQuery(this.GetSearchFieldForType(termExpression.Type), termExpression.Value), enableRanking, requestId);
      string str = ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value);
      string rewriteMethod = CodeFileContract.GetRewriteMethod(requestContext, this.ContractType);
      string termQueryString;
      if (requestContext.IsQueryingNGramsEnabled())
        termQueryString = ElasticsearchQueryBuilder.BuildFastWildcardQuery("originalContent.ngram", str);
      else if ((termExpression.Value.StartsWith("*", StringComparison.Ordinal) || termExpression.Value.StartsWith("?", StringComparison.Ordinal)) && requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableCodePrefixWildcardSearch", true))
      {
        char[] charArray = ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value).ToCharArray();
        Array.Reverse((Array) charArray);
        termQueryString = ElasticsearchQueryBuilder.BuildWildcardQuery(this.fields[CodeFileContract.CodeContractQueryableElement.Prefix].ElasticsearchFieldName, new string(charArray), rewriteMethod);
      }
      else
        termQueryString = ElasticsearchQueryBuilder.BuildWildcardQuery(this.GetSearchFieldForAdvancedQuery(requestContext).ElasticsearchFieldName, str, rewriteMethod);
      if (!requestContext.IsFeatureEnabled("Search.Server.Code.NoPayloadCodeSearchHighlighterV2"))
        return termQueryString;
      List<string> childQueries = new List<string>();
      childQueries.Add(termQueryString);
      foreach (string fieldName in NoPayloadContractUtils.KnownFieldsTypeForHighlightHit)
        childQueries.Add(ElasticsearchQueryBuilder.BuildWildcardQuery(fieldName, str, "constant_score"));
      return ElasticsearchQueryBuilder.BuildBoolShouldQuery(childQueries);
    }

    internal virtual string CreateSpanQueryNearString(TermExpression termExpression, int slopValue)
    {
      if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Near)
        return ElasticsearchQueryBuilder.BuildSpanNearQuery(this.GetSearchFieldForProximityOperators(), termExpression.Type, termExpression.Value, slopValue);
      throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("NEAR operator is expected. Found: {0}", (object) termExpression.Operator)));
    }

    internal virtual string CreateSpanQueryBeforeString(
      TermExpression termExpression,
      int slopValue)
    {
      if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Before)
        return ElasticsearchQueryBuilder.BuildSpanBeforeQuery(this.GetSearchFieldForProximityOperators(), termExpression.Type, termExpression.Value, slopValue);
      throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("BEFORE operator is expected. Found: {0}", (object) termExpression.Operator)));
    }

    internal virtual string CreateSpanQueryAfterString(TermExpression termExpression, int slopValue)
    {
      if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.After)
        return ElasticsearchQueryBuilder.BuildSpanBeforeQuery(this.GetSearchFieldForProximityOperators(), termExpression.Value, termExpression.Type, slopValue);
      throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("AFTER operator is expected. Found: {0}", (object) termExpression.Operator)));
    }

    internal virtual string CreateTermQueryStringForRegexTrigramType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      throw new NotImplementedException();
    }

    internal virtual string CreateTermQueryStringForDefaultType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      return this.CreateTermQueryString(requestContext, termExpression, enableRanking, requestId);
    }

    internal virtual string CreateFilePathQuery(TermExpression termExpression, string requestId) => this.CreateFilteredQueryString(termExpression, requestId);

    internal virtual string CreateFilteredQueryString(
      TermExpression termExpression,
      string requestId)
    {
      string searchFieldForType = this.GetSearchFieldForType(termExpression.Type);
      return termExpression.Value.Contains("?") || termExpression.Value.Contains("*") ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\" : {{\r\n                                \"value\" : {2},\r\n                                \"{3}\" : \"{4}\",\r\n                                \"_name\": {5}\r\n                            }}\r\n                        }}\r\n                    }}", (object) "wildcard", (object) searchFieldForType, (object) JsonConvert.SerializeObject((object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value)), (object) "rewrite", (object) "constant_score", (object) JsonConvert.SerializeObject((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) searchFieldForType, (object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value)))) : (termExpression.IsOfType("file") ? (this.IsExactMatch(termExpression) ? this.ConvertToFileExactMatchQueryString(termExpression, searchFieldForType, 10000000.0) : CodeFileContract.ConvertToFunctionScoreQueryString(termExpression, searchFieldForType, 10000000.0)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\": {{\r\n                                \"{2}\" : {{\r\n                                    \"{3}\" : [{4}],\r\n                                    \"_name\" : {5}\r\n                                }}\r\n                            }}\r\n                        }}\r\n                    }}", (object) "bool", (object) "filter", (object) "terms", (object) searchFieldForType, (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) JsonConvert.SerializeObject((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) searchFieldForType, (object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value)))));
    }

    private bool IsExactMatch(TermExpression termExpression) => termExpression.Value.StartsWith("\"", StringComparison.Ordinal) && termExpression.Value.EndsWith("\"", StringComparison.Ordinal) && termExpression.Value.Length > 1;

    internal virtual string ConvertToPhraseQueryString(
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      string str = Regex.Replace(termExpression.Value, "[^\\w]", " ");
      return this.GetCustomizedQueryString(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"match_phrase\": {{\r\n                        \"{0}\": {{\r\n                            \"query\": {1},\r\n                            \"analyzer\" : \"{2}\"\r\n                        }}\r\n                    }}\r\n                }}", (object) this.GetSearchFieldForType(termExpression.Type), (object) JsonConvert.SerializeObject((object) str), (object) "whitespace"), enableRanking, requestId);
    }

    internal virtual string ConvertToAdvancedPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      throw new NotImplementedException();
    }

    internal virtual string ConvertToRegexpQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      return ElasticsearchQueryBuilder.BuildRegexpQuery(this.GetSearchFieldForAdvancedQuery(requestContext).ElasticsearchFieldName, ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value));
    }

    internal virtual string ConvertToTrigramPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      throw new NotImplementedException();
    }

    internal virtual string GetCustomizedQueryString(
      string originalQueryString,
      bool enableRanking,
      string requestId)
    {
      if (!enableRanking)
        return originalQueryString;
      if (string.IsNullOrWhiteSpace(requestId))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                            \"{0}\": {{\r\n                                \"{1}\": {2}\r\n                             }}\r\n                        }}", (object) "codesearch_score_query", (object) "query", (object) originalQueryString);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                            \"{0}\": {{\r\n                                \"{1}\": {2},\r\n                                \"{3}\": {4}\r\n                             }}\r\n                        }}", (object) "codesearch_score_query", (object) "query_id", (object) JsonConvert.SerializeObject((object) requestId), (object) "query", (object) originalQueryString);
    }

    internal static string ConvertToFunctionScoreQueryString(
      TermExpression termExpression,
      string fieldName,
      double scoreBoost)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"{0}\": {{\r\n                        \"boost\": {1},\r\n                        \"{2}\": {{\r\n                            \"{3}\" : {{\r\n                                \"{4}\" : {{\r\n                                \"value\" : {5},\r\n                                \"_name\" : {6}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        }}\r\n                }}", (object) "function_score", (object) scoreBoost, (object) "query", (object) "term", (object) fieldName, (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) JsonConvert.SerializeObject((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) fieldName, (object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value))));
    }

    internal string ConvertToFileExactMatchQueryString(
      TermExpression termExpression,
      string fieldName,
      double scoreBoost)
    {
      string text1 = ElasticsearchQueryBuilder.TrimDoubleQuotesPadding(termExpression.Value);
      string text2 = text1 + ".*";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"{0}\": {{\r\n                         \"{1}\": [\r\n                                {{\r\n                                \"{2}\": {{\r\n                                    \"boost\": {3},\r\n                                    \"{4}\": {{\r\n                                        \"{5}\" : {{\r\n                                            \"{6}\" : {{\r\n                                                \"value\" : {7},\r\n                                                \"_name\" : {8}\r\n                                           }}\r\n                                        }}\r\n                                     }}\r\n                                }}\r\n                            }}\r\n                        ],\r\n                        \"{9}\": [\r\n                           {{\r\n                                \"{10}\": {{\r\n                                    \"{6}\" : {{\r\n                                        \"value\" : {12},\r\n                                        \"rewrite\" : \"constant_score\" ,\r\n                                        \"_name\" : {11}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) "bool", (object) "must", (object) "function_score", (object) scoreBoost, (object) "query", (object) "term", (object) fieldName, (object) JsonConvert.SerializeObject((object) text1), (object) JsonConvert.SerializeObject((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) fieldName, (object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(text1))), (object) "must_not", (object) "wildcard", (object) JsonConvert.SerializeObject((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) fieldName, (object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(text2))), (object) JsonConvert.SerializeObject((object) text2));
    }

    internal virtual string CreateTermQueryStringWithBoost(
      TermExpression termExpression,
      bool enableRanking,
      string fieldName,
      string requestId,
      double boost)
    {
      return termExpression.Value.Contains("?") || termExpression.Value.Contains("*") ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\" : {{\r\n                                \"value\" : {2},\r\n                                \"{3}\" : \"{4}\",\r\n                                \"boost\" : {5}\r\n                            }}\r\n                        }}\r\n                    }}", (object) "wildcard", (object) fieldName, (object) JsonConvert.SerializeObject((object) ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value)), (object) "rewrite", (object) "top_terms_boost_100", (object) boost) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\" : {{\r\n                                \"value\" : {2},\r\n                                \"boost\" : {3}\r\n                            }}\r\n                        }}\r\n                    }}", (object) "term", (object) fieldName, (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) boost);
    }

    private static class StoredFieldNames
    {
      public const string RepositoryName = "repoName";
      public const string BranchName = "branchName";
      public const string FilePath = "filePath";
      public const string CommitId = "commitId";
      public const string ChangeId = "changeId";
      public const string ContentId = "contentId";
      public const string FileExtension = "fileExtension";
      public const string VersionControlType = "versionControlType";
      public const string DocumentId = "documentId";
    }

    public enum CodeContractQueryableElement
    {
      None,
      [CodeSearchFieldExtensions.InlineFilterName("*")] Default,
      Advanced,
      Prefix,
      CodeElement,
      [CodeSearchFieldExtensions.InlineFilterName("regex")] Regex,
      DocumentId,
      Account,
      CollectionName,
      [CodeSearchFieldExtensions.InlineFilterName("collectionId")] CollectionId,
      [CodeSearchFieldExtensions.InlineFilterName("proj")] ProjectName,
      ProjectId,
      [CodeSearchFieldExtensions.InlineFilterName("repo")] RepoName,
      RepositoryId,
      [CodeSearchFieldExtensions.InlineFilterName("repoId")] RepoId,
      [CodeSearchFieldExtensions.InlineFilterName("isdefaultbranch")] IsDefaultBranch,
      [CodeSearchFieldExtensions.InlineFilterName("branch")] BranchName,
      CommitId,
      [CodeSearchFieldExtensions.InlineFilterName("file")] FileName,
      [CodeSearchFieldExtensions.InlineFilterName("fileName")] FileName2,
      [CodeSearchFieldExtensions.InlineFilterName("ext")] FileExtension,
      [CodeSearchFieldExtensions.InlineFilterName("path")] FilePath,
      SortableFilePath,
      ChangeId,
      ContentId,
      LastChangeUtcTime,
      [CodeSearchFieldExtensions.InlineFilterName("vcType")] VersionControlType,
      [CodeSearchFieldExtensions.InlineFilterName("content")] Content,
      [CodeSearchFieldExtensions.InlineFilterName("originalContent")] OriginalContent,
      [CodeSearchFieldExtensions.InlineFilterName("originalContent.reverse")] OriginalContentReverse,
      [CodeSearchFieldExtensions.InlineFilterName("originalContent.trigram")] OriginalContentTrigram,
      [CodeSearchFieldExtensions.InlineFilterName("isDocumentDeletedInReIndexing")] IsDocumentDeletedInReIndexing,
      [CodeSearchFieldExtensions.InlineFilterName("indexedTimeStamp")] IndexedTimeStamp,
    }

    public class CodeSearchHit : SearchHit
    {
      public CodeSearchHit() => this.Matches = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>();

      public CodeSearchHit(
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>> matches,
        IDictionary<string, IEnumerable<string>> fields)
        : base(fields)
      {
        this.Matches = matches;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected override string GetFieldValue(string fieldName) => throw new InvalidOperationException("Not supported for code");

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected override string GetFieldValue(string fieldName, string defaultFieldValue) => throw new InvalidOperationException("Not supported for code");

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected override IEnumerable<string> GetFieldValues(string fieldName) => throw new InvalidOperationException("Not supported for code");

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected override IEnumerable<string> GetFieldValues(
        string fieldName,
        IEnumerable<string> defaultFieldValue)
      {
        throw new InvalidOperationException("Not supported for code");
      }

      public IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>> Matches { get; private set; }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected string GetFieldValue(CodeContractField field)
      {
        IEnumerable<string> source;
        if (!this.Fields.TryGetValue(field.StoredToFieldDesc.ElasticsearchFieldName(), out source))
          throw new SearchServiceException("Search platform response: " + field.ElasticsearchFieldName + " not found");
        return source.FirstOrDefault<string>();
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected string GetFieldValue(CodeContractField field, string defaultFieldValue)
      {
        IEnumerable<string> source;
        return !this.Fields.TryGetValue(field.StoredToFieldDesc.ElasticsearchFieldName(), out source) ? defaultFieldValue : source.FirstOrDefault<string>();
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected IEnumerable<string> GetFieldValues(CodeContractField field)
      {
        IEnumerable<string> fieldValues;
        if (!this.Fields.TryGetValue(field.StoredToFieldDesc.ElasticsearchFieldName(), out fieldValues))
          throw new SearchServiceException("Search platform response: " + field.ElasticsearchFieldName + " not found");
        return fieldValues;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      protected IEnumerable<string> GetFieldValues(
        CodeContractField field,
        IEnumerable<string> defaultFieldValue)
      {
        IEnumerable<string> strings;
        return !this.Fields.TryGetValue(field.StoredToFieldDesc.ElasticsearchFieldName(), out strings) ? defaultFieldValue : strings;
      }

      public static Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult CreateCodeResult(
        CodeFileContract contract,
        CodeFileContract.CodeSearchHit platformHit,
        string vcGit)
      {
        if (!DocumentContractTypeExtension.IsValidCodeDocumentContractType(contract.ContractType))
          throw new ArgumentException("Invalid code document contract type");
        string key = "content";
        platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.CollectionName));
        string fieldValue1 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.ProjectName));
        string fieldValue2 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.RepoName));
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> source1 = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>();
        using (IEnumerator<string> enumerator1 = platformHit.GetFieldValues(contract.GetField(CodeFileContract.CodeContractQueryableElement.BranchName)).GetEnumerator())
        {
          using (IEnumerator<string> enumerator2 = platformHit.GetFieldValues(contract.GetField(CodeFileContract.CodeContractQueryableElement.ChangeId)).GetEnumerator())
          {
            while (enumerator1.MoveNext())
            {
              if (enumerator2.MoveNext())
                source1.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version(enumerator1.Current, enumerator2.Current));
              else
                break;
            }
          }
        }
        string branchName = source1.First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>().BranchName;
        string changeId = source1.First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>().ChangeId;
        string fieldValue3 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.FilePath));
        string fieldValue4 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.ContentId));
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType vcType = (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType) Enum.Parse(typeof (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType), platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.VersionControlType), vcGit), true);
        int num1 = platformHit.Matches.ContainsKey(key) ? 1 : 0;
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> source2 = num1 != 0 ? platformHit.Matches[key] : (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
        int num2 = num1 != 0 ? source2.Count<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>() : 0;
        string project = fieldValue1;
        string repository = fieldValue2;
        string branch = branchName;
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> versions = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>) source1;
        int hitCount = num2;
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> hits = source2;
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>> matches = platformHit.Matches;
        string path = fieldValue3;
        return new Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult(FilePathUtils.GetFileName(fieldValue3), path, hitCount, hits, matches, "DefaultCollection", project, repository, (string) null, branch, versions, changeId, fieldValue4, vcType);
      }

      public static Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult CreateScrollCodeResult(
        CodeFileContract contract,
        CodeFileContract.CodeSearchHit platformHit,
        string vcGit)
      {
        string str = DocumentContractTypeExtension.IsValidCodeDocumentContractType(contract.ContractType) ? platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.ProjectName)) : throw new ArgumentException("Invalid code document contract type");
        string fieldValue1 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.RepoName));
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version> versionList = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>();
        using (IEnumerator<string> enumerator1 = platformHit.GetFieldValues(contract.GetField(CodeFileContract.CodeContractQueryableElement.BranchName)).GetEnumerator())
        {
          using (IEnumerator<string> enumerator2 = platformHit.GetFieldValues(contract.GetField(CodeFileContract.CodeContractQueryableElement.ChangeId)).GetEnumerator())
          {
            while (enumerator1.MoveNext())
            {
              if (enumerator2.MoveNext())
                versionList.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version(enumerator1.Current, enumerator1.Current));
              else
                break;
            }
          }
        }
        string fieldValue2 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.FilePath));
        string fieldValue3 = platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.ContentId));
        Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType versionControlType = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType) Enum.Parse(typeof (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType), platformHit.GetFieldValue(contract.GetField(CodeFileContract.CodeContractQueryableElement.VersionControlType), vcGit), true);
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult scrollCodeResult = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult()
        {
          Collection = new Collection()
          {
            Name = "DefaultCollection"
          },
          ContentId = fieldValue3,
          Project = new Project() { Name = str },
          Repository = new Repository()
          {
            Name = fieldValue1,
            Id = (string) null
          },
          Versions = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) versionList
        };
        scrollCodeResult.Repository.Type = versionControlType;
        scrollCodeResult.Path = fieldValue2;
        scrollCodeResult.Filename = FilePathUtils.GetFileName(fieldValue2);
        scrollCodeResult.ContentId = fieldValue3;
        scrollCodeResult.Matches = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>) platformHit.Matches.ToDictionary<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>) (x => x.Value.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>) (i => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit()
        {
          CharOffset = i.CharOffset,
          Length = i.Length,
          Type = i.Type
        }))));
        return scrollCodeResult;
      }
    }

    internal sealed class CodeEntityQueryHandler<T> : EntityIndexProvider<T> where T : class
    {
      private readonly bool m_returnFieldnameWithHighlight;
      private readonly bool m_parseFieldNameAsEnum;
      private readonly bool m_includeSnippet;
      [StaticSafe]
      private static readonly FriendlyDictionary<DocumentContractType, CodeFileContract> s_documentContractMapping = new FriendlyDictionary<DocumentContractType, CodeFileContract>()
      {
        {
          DocumentContractType.DedupeFileContractV5,
          (CodeFileContract) new DedupeFileContractV5()
        },
        {
          DocumentContractType.DedupeFileContractV4,
          (CodeFileContract) new DedupeFileContractV4()
        },
        {
          DocumentContractType.DedupeFileContractV3,
          (CodeFileContract) new DedupeFileContractV3()
        },
        {
          DocumentContractType.SourceNoDedupeFileContractV3,
          (CodeFileContract) new SourceNoDedupeFileContractV3()
        },
        {
          DocumentContractType.SourceNoDedupeFileContractV4,
          (CodeFileContract) new SourceNoDedupeFileContractV4()
        },
        {
          DocumentContractType.SourceNoDedupeFileContractV5,
          (CodeFileContract) new SourceNoDedupeFileContractV5()
        }
      };

      public CodeEntityQueryHandler()
      {
      }

      public CodeEntityQueryHandler(bool returnFieldnameWithHighlight)
        : this(returnFieldnameWithHighlight, false, false)
      {
      }

      public CodeEntityQueryHandler(bool returnFieldnameWithHighlight, bool parseFieldNameAsEnum)
        : this(returnFieldnameWithHighlight, parseFieldNameAsEnum, false)
      {
      }

      public CodeEntityQueryHandler(
        bool returnFieldnameWithHighlight,
        bool parseFieldNameAsEnum,
        bool includeSnippet)
      {
        this.m_returnFieldnameWithHighlight = returnFieldnameWithHighlight;
        this.m_parseFieldNameAsEnum = returnFieldnameWithHighlight & parseFieldNameAsEnum;
        this.m_includeSnippet = includeSnippet;
      }

      public override void BuildSearchComponents(
        IVssRequestContext requestContext,
        EntitySearchPlatformRequest request,
        string rawQueryString,
        string rawFilterString,
        out Func<QueryContainerDescriptor<T>, QueryContainer> query,
        out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
        out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
        out Func<HighlightDescriptor<T>, IHighlight> highlight,
        out Func<IVssRequestContext, SortDescriptor<T>> sort)
      {
        CodeSearchPlatformRequest searchPlatformRequest = (CodeSearchPlatformRequest) request;
        CodeFileContract documentContract = (CodeFileContract) request.DocumentContract;
        query = this.GetFilteredQuery(rawQueryString, rawFilterString);
        filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(documentContract.GetFilterBuilder(request).Filters<T>);
        aggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(documentContract.GetAggregationBuilder(request).Aggregates<T>);
        string elasticsearchFieldName = documentContract.GetSearchFieldForAdvancedQuery(requestContext).ElasticsearchFieldName;
        string highlighter = documentContract.GetHighlighter(requestContext);
        string str = elasticsearchFieldName;
        searchPlatformRequest.HighlightField = str;
        highlight = request.ScrollSize <= 0 || !requestContext.IsFeatureEnabled("Search.Server.ScrollSearchQuery") ? new CodeHighlightBuilder(highlighter, elasticsearchFieldName, request.Options.HasFlag((Enum) SearchOptions.Highlighting)).Highlight<T>() : new CodeHighlightBuilder(highlighter, elasticsearchFieldName, false).Highlight<T>();
        sort = new CodeSortBuilder(request.SortOptions).Sort<T>();
      }

      public override bool IsValidEntityAccess(EntitySearchPlatformRequest request) => base.IsValidEntityAccess((EntitySearchPlatformRequest) (request as CodeSearchPlatformRequest));

      public override void ValidateQueryRequest(EntitySearchPlatformRequest request)
      {
        CodeSearchPlatformRequest request1 = request as CodeSearchPlatformRequest;
        base.ValidateQueryRequest((EntitySearchPlatformRequest) request1);
        if (request1?.ScopeFiltersExpression == null)
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "ScopeFiltersExpression")), nameof (request));
      }

      public override EntitySearchPlatformResponse DefaultPlatformResponse(
        EntitySearchPlatformRequest request)
      {
        if (!(request is CodeSearchPlatformRequest searchPlatformRequest))
          throw new ArgumentException("Supported only for CodeSearchPlatformRequest objects");
        Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>();
        if (request.Options.HasFlag((Enum) SearchOptions.Faceting))
        {
          CodeFileContract.CodeEntityQueryHandler<T>.AddToSearchFacets("ProjectFilters", searchPlatformRequest?.SearchFilters, (Func<string, string>) (f => f), ref searchFacets);
          CodeFileContract.CodeEntityQueryHandler<T>.AddToSearchFacets("RepositoryFilters", searchPlatformRequest?.SearchFilters, (Func<string, string>) (f => f), ref searchFacets);
          CodeFileContract.CodeEntityQueryHandler<T>.AddToSearchFacets("CodeElementFilters", searchPlatformRequest?.SearchFilters, (Func<string, string>) (f => CodeSearchFilters.DisplayableCEFilterIdToNameMap[f]), ref searchFacets);
        }
        return (EntitySearchPlatformResponse) new CodeSearchPlatformResponse(searchPlatformRequest.DocumentContract as CodeFileContract, 0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) searchFacets);
      }

      public override int GetTotalResultCount(
        EntitySearchPlatformRequest request,
        ISearchResponse<T> elasticSearchResponse)
      {
        return (int) elasticSearchResponse.Total;
      }

      public override EntitySearchPlatformResponse PreparePlatformResponse(
        int responseCount,
        bool isTimedOut,
        List<SearchHit> searchResults,
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
      {
        throw new InvalidOperationException("Not being used anywhere, to be deprecated");
      }

      public override EntitySearchPlatformResponse PreparePlatformResponse(
        AbstractSearchDocumentContract docContract,
        int responseCount,
        bool isTimedOut,
        List<SearchHit> searchResults,
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
        string scrollId)
      {
        CodeFileContract contract = docContract as CodeFileContract;
        int totalMatches = responseCount;
        bool flag = isTimedOut;
        List<SearchHit> results = searchResults;
        string scrollId1 = scrollId;
        int num = flag ? 1 : 0;
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = searchFacets;
        return (EntitySearchPlatformResponse) new CodeSearchPlatformResponse(contract, totalMatches, (IList<SearchHit>) results, scrollId1, num != 0, facets);
      }

      public override IEnumerable<string> GetFieldNames(
        IEnumerable<string> storedFields,
        DocumentContractType contractType)
      {
        throw new InvalidOperationException("Not supported for code");
      }

      public override string GetStoredFieldNameForElasticsearchName(
        string field,
        DocumentContractType contractType)
      {
        return field;
      }

      public override string GetStoredFieldValue(
        string field,
        string fieldValue,
        DocumentContractType contractType)
      {
        return CodeFileContract.CodeEntityQueryHandler<T>.s_documentContractMapping.ContainsKey(contractType) ? CodeFileContract.CodeEntityQueryHandler<T>.s_documentContractMapping[contractType].GetStoredFieldValue(field, fieldValue) : fieldValue;
      }

      public override SearchHit GetSearchHit(
        IHit<T> hit,
        EntitySearchPlatformRequest request,
        ConcurrentBag<Exception> exceptions,
        ref bool isOperationSuccessful)
      {
        CodeSearchPlatformRequest codeSearchRequest = request as CodeSearchPlatformRequest;
        Dictionary<string, IEnumerable<string>> searchHitFields = this.GetSearchHitFields(hit, request, exceptions, ref isOperationSuccessful);
        if (!isOperationSuccessful)
          return (SearchHit) new CodeFileContract.CodeSearchHit();
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>> matches = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>();
        if (!request.Options.HasFlag((Enum) SearchOptions.Highlighting))
          return (SearchHit) new CodeFileContract.CodeSearchHit(matches, (IDictionary<string, IEnumerable<string>>) searchHitFields);
        if (hit.Highlight == null)
        {
          exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlights cannot be null"));
          isOperationSuccessful = false;
          return (SearchHit) new CodeFileContract.CodeSearchHit();
        }
        ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> hitOffsets = this.CalculateHitOffsets(hit, codeSearchRequest, ref exceptions, ref isOperationSuccessful);
        IList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> hitList = (IList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
        if (hit.MatchedQueries != null && hit.MatchedQueries.Any<string>((Func<string, bool>) (s => s.StartsWith(CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName(), StringComparison.Ordinal))))
        {
          Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit hit1 = new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit()
          {
            CharOffset = 0,
            Length = -1
          };
          hitList.Add(hit1);
        }
        matches.Add("content", (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) hitOffsets);
        matches.Add(CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName(), (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) hitList);
        return (SearchHit) new CodeFileContract.CodeSearchHit(matches, (IDictionary<string, IEnumerable<string>>) searchHitFields);
      }

      private ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> CalculateHitOffsets(
        IHit<T> hit,
        CodeSearchPlatformRequest codeSearchRequest,
        ref ConcurrentBag<Exception> exceptions,
        ref bool isOperationSuccessful)
      {
        isOperationSuccessful = true;
        exceptions = new ConcurrentBag<Exception>();
        ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> hitOffsets;
        if (codeSearchRequest.HighlightField != null)
        {
          if (hit.Highlight.ContainsKey(codeSearchRequest.HighlightField))
          {
            try
            {
              hitOffsets = !this.m_returnFieldnameWithHighlight ? CodeFileContract.CodeEntityQueryHandler<T>.GetHitOffsets(hit.Highlight[codeSearchRequest.HighlightField]) : CodeFileContract.CodeEntityQueryHandler<T>.GetHitOffsetsWithField(hit.Highlight[codeSearchRequest.HighlightField], this.m_parseFieldNameAsEnum, this.m_includeSnippet);
            }
            catch (Exception ex)
            {
              hitOffsets = (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
              exceptions.Add(ex);
              isOperationSuccessful = false;
            }
          }
          else
          {
            isOperationSuccessful = false;
            hitOffsets = (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
          }
        }
        else
          hitOffsets = (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
        return hitOffsets;
      }

      private static ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> GetHitOffsets(
        IReadOnlyCollection<string> highlight)
      {
        List<string> list = highlight.ToList<string>();
        int count = list.Count;
        HashSet<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> source = (count & 1) == 0 ? new HashSet<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>(count >> 1) : throw new SearchPlatformException("Number of entries in Elasticsearch response highlight list [" + count.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is odd. It is expected to be even (pairs of <start offset, length>).");
        for (int index = 0; index < count; index += 2)
        {
          int result1;
          if (!int.TryParse(list[index], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            throw new SearchPlatformException("Could not parse token start offset [" + list[index] + "] as an integer.");
          if (result1 < 0)
            throw new SearchPlatformException("Token start offset [" + result1.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be non-negative.");
          int result2;
          if (!int.TryParse(list[index + 1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
            throw new SearchPlatformException("Could not parse token length [" + list[index + 1] + "] as an integer.");
          if (result2 <= 0)
            throw new SearchPlatformException("Token length [" + result2.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be positive.");
          source.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit()
          {
            CharOffset = result1,
            Length = result2,
            Type = (string) null,
            CodeSnippet = (string) null,
            Line = 0,
            Column = 0
          });
        }
        return (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) source.ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
      }

      private static ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> GetHitOffsetsWithField(
        IReadOnlyCollection<string> highlight,
        bool parseFieldNameAsEnum = false,
        bool includeSnippet = false)
      {
        List<string> list = highlight.ToList<string>();
        int count = list.Count;
        int num = includeSnippet ? 6 : 3;
        string str1 = includeSnippet ? "<start offset, length, type, code snippet, line, column>" : "<start offset, length, type>";
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit> source = count % num == 0 ? new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>(count / num) : throw new SearchPlatformException("Number of entries in Elasticsearch response highlight list [" + count.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is not a multiple of " + num.ToString() + ". It is expected to be even (pairs of " + str1 + ").");
        for (int index = 0; index < count; index += num)
        {
          int result1;
          if (!int.TryParse(list[index], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            throw new SearchPlatformException("Could not parse token start offset [" + list[index] + "] as an integer.");
          if (result1 < 0)
            throw new SearchPlatformException("Token start offset [" + result1.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be non-negative.");
          int result2;
          if (!int.TryParse(list[index + 1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
            throw new SearchPlatformException("Could not parse token length [" + list[index + 1] + "] as an integer.");
          if (result2 <= 0)
            throw new SearchPlatformException("Token length [" + result2.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be positive.");
          string str2 = string.Empty;
          if (parseFieldNameAsEnum)
          {
            int result3;
            if (!int.TryParse(list[index + 2], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
              throw new SearchPlatformException("Could not parse field name [" + list[index + 2] + "] as an integer.");
            if (result3 < 0)
              throw new SearchPlatformException("Token type enum [" + result3.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be zero or positive.");
            foreach (KeyValuePair<string, CodeSearchFilters.CEFilterAttributes> ceFilterAttribute in (IEnumerable<KeyValuePair<string, CodeSearchFilters.CEFilterAttributes>>) CodeSearchFilters.CEFilterAttributeMap)
            {
              if (ceFilterAttribute.Value.TokenIds.Contains<int>(result3))
              {
                str2 = ceFilterAttribute.Key;
                break;
              }
            }
          }
          else
            str2 = list[index + 2];
          if (string.IsNullOrWhiteSpace(str2))
            throw new SearchPlatformException("Type is not populated correctly");
          string str3 = (string) null;
          int result4 = 0;
          int result5 = 0;
          if (includeSnippet)
          {
            str3 = list[index + 3];
            if (!int.TryParse(list[index + 4], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result4))
              throw new SearchPlatformException("Could not parse token start offset [" + list[index + 4] + "] as an integer.");
            if (result4 < 0)
              throw new SearchPlatformException("Line number of the code snippet  [" + result4.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be non-negative.");
            if (!int.TryParse(list[index + 5], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
              throw new SearchPlatformException("Could not parse token start offset [" + list[index + 5] + "] as an integer.");
            if (result5 < 0)
              throw new SearchPlatformException("Column of the match in code snippet [" + result5.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] is expected to be non-negative.");
          }
          source.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit()
          {
            CharOffset = result1,
            Length = result2,
            Type = str2,
            CodeSnippet = str3,
            Line = result4,
            Column = result5
          });
        }
        return (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) source.GroupBy(f => new
        {
          CharOffset = f.CharOffset,
          Length = f.Length
        }).ToList<IGrouping<\u003C\u003Ef__AnonymousType0<int, int>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>().Select<IGrouping<\u003C\u003Ef__AnonymousType0<int, int>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>(f => f.Count<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>() == 1 ? f.ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>().Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>) (ff =>
        {
          if (!ff.Type.ToString().Equals("originalContent", StringComparison.OrdinalIgnoreCase))
            return ff;
          return new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit()
          {
            CharOffset = ff.CharOffset,
            Length = ff.Length,
            Type = "content",
            CodeSnippet = ff.CodeSnippet,
            Line = ff.Line,
            Column = ff.Column
          };
        })).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>() : f.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, bool>) (ff => !ff.Type.ToString().Equals("originalContent", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>()).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>();
      }

      public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
        EntitySearchPlatformRequest request,
        ISearchResponse<T> elasticSearchResponse)
      {
        CodeSearchPlatformRequest request1 = request as CodeSearchPlatformRequest;
        bool flag1 = false;
        bool flag2 = false;
        FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        AggregateDictionary aggregations = elasticSearchResponse.Aggregations;
        if (aggregations != null)
        {
          try
          {
            KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> accountFacets = this.GetAccountFacets(request1, aggregations);
            if (accountFacets.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())
            {
              searchFacets.Add(accountFacets);
              flag1 = true;
            }
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082268, "Search Engine", "Search Engine", new SearchPlatformException("Failed to compute account aggregations.", ex).ToString());
          }
          try
          {
            KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> collectionFacets = this.GetCollectionFacets(request1, aggregations);
            if (collectionFacets.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())
            {
              searchFacets.Add(collectionFacets);
              flag2 = true;
            }
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082268, "Search Engine", "Search Engine", new SearchPlatformException("Failed to compute collection aggregations.", ex).ToString());
          }
          if (!flag1)
          {
            if (!flag2)
            {
              try
              {
                KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> projectFacets = this.GetProjectFacets(request1, aggregations);
                if (projectFacets.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())
                  searchFacets.Add(projectFacets);
              }
              catch (Exception ex)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082244, "Search Engine", "Search Engine", new SearchPlatformException("Failed to compute project aggregations.", ex).ToString());
              }
              try
              {
                KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> repositoryFacets = this.GetRepositoryFacets(request1, aggregations);
                if (repositoryFacets.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())
                  searchFacets.Add(repositoryFacets);
              }
              catch (Exception ex)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082245, "Search Engine", "Search Engine", new SearchPlatformException("Failed to compute repository aggregations.", ex).ToString());
              }
              try
              {
                KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> payloadAggregations = CodeFileContract.CodeEntityQueryHandler<T>.GetCodeElementFacetsFromPayloadAggregations(request1);
                if (payloadAggregations.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())
                  searchFacets.Add(payloadAggregations);
              }
              catch (Exception ex)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082246, "Search Engine", "Search Engine", new SearchPlatformException("Failed to compute code type aggregations.", ex).ToString());
              }
            }
          }
        }
        return this.PostFacetCleanup((EntitySearchPlatformRequest) request1, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) searchFacets, SearchQuery.ParentOf);
      }

      protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
        T sources,
        EntitySearchPlatformRequest request,
        ConcurrentBag<Exception> exceptions,
        ref bool isOperationSuccessful)
      {
        Dictionary<string, IEnumerable<string>> searchHitSources = new Dictionary<string, IEnumerable<string>>();
        if (sources is CodeFileContract codeFileContract)
        {
          if (!string.IsNullOrWhiteSpace(codeFileContract.CollectionNameOriginal))
            searchHitSources.Add("collectionName", (IEnumerable<string>) new List<string>()
            {
              codeFileContract.CollectionNameOriginal
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.ContentId))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.ContentId
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.DocumentId))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.DocumentId.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.DocumentId
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.FileExtension))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.FileExtension
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.FilePathOriginal))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.FilePathOriginal
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.ProjectNameOriginal))
            searchHitSources.Add("projectName", (IEnumerable<string>) new List<string>()
            {
              codeFileContract.ProjectNameOriginal
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.RepoNameOriginal))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.RepoNameOriginal
            });
          if (!string.IsNullOrWhiteSpace(codeFileContract.VcType))
            searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
            {
              codeFileContract.VcType
            });
          if (sources is DedupeFileContractBase fileContractBase && fileContractBase.Paths != null)
          {
            if (fileContractBase.Paths.BranchNameOriginal != null && fileContractBase.Paths.BranchNameOriginal.Any<string>())
              searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.BranchName.ElasticsearchFieldName(), (IEnumerable<string>) fileContractBase.Paths.BranchNameOriginal);
            if (fileContractBase.Paths.ChangeId != null && fileContractBase.Paths.ChangeId.Any<string>())
              searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName(), (IEnumerable<string>) fileContractBase.Paths.ChangeId);
          }
          if (sources is SourceNoDedupeFileContractV3 dedupeFileContractV3)
          {
            if (!string.IsNullOrWhiteSpace(dedupeFileContractV3.BranchNameOriginal))
              searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.BranchName.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
              {
                dedupeFileContractV3.BranchNameOriginal
              });
            if (!string.IsNullOrWhiteSpace(dedupeFileContractV3.ChangeId))
              searchHitSources.Add(CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName(), (IEnumerable<string>) new List<string>()
              {
                dedupeFileContractV3.ChangeId
              });
          }
        }
        return searchHitSources;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private static void AddToSearchFacets(
        string filterCategoryId,
        IDictionary<string, IEnumerable<string>> searchFilters,
        Func<string, string> filterIdToName,
        ref Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
      {
        IEnumerable<string> source;
        if (!searchFilters.TryGetValue(filterCategoryId, out source))
          return;
        searchFacets.Add(filterCategoryId, source.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (f =>
        {
          string id = f;
          return new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(filterIdToName(f), id, 0, true);
        })));
      }

      private KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetAccountFacets(
        CodeSearchPlatformRequest request,
        AggregateDictionary aggsDictionary)
      {
        return this.GetTermFacets(aggsDictionary, (EntitySearchPlatformRequest) request, "AccountFilters", new string[3]
        {
          "term_aggs",
          "filtered_account_aggs",
          "account_aggs"
        });
      }

      private KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetCollectionFacets(
        CodeSearchPlatformRequest request,
        AggregateDictionary aggsDictionary)
      {
        return this.GetTermFacets(aggsDictionary, (EntitySearchPlatformRequest) request, "CollectionFilters", new string[3]
        {
          "term_aggs",
          "filtered_collection_aggs",
          "collection_aggs"
        });
      }

      private KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetProjectFacets(
        CodeSearchPlatformRequest request,
        AggregateDictionary aggsDictionary)
      {
        return this.GetTermFacets(aggsDictionary, (EntitySearchPlatformRequest) request, "ProjectFilters", new string[3]
        {
          "term_aggs",
          "filtered_project_aggs",
          "project_aggs"
        });
      }

      private KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetRepositoryFacets(
        CodeSearchPlatformRequest request,
        AggregateDictionary aggsDictionary)
      {
        return this.GetTermFacets(aggsDictionary, (EntitySearchPlatformRequest) request, "RepositoryFilters", new string[3]
        {
          "term_aggs",
          "filtered_repository_aggs",
          "repository_aggs"
        });
      }

      private static KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetCodeElementFacetsFromPayloadAggregations(
        CodeSearchPlatformRequest request)
      {
        Dictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> facetMap = new Dictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return EntityIndexProvider<T>.PreserveFilterSelectionStateAndSort(request.SearchFilters, "CodeElementFilters", (IDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) facetMap, (Func<string, string>) (f => CodeSearchFilters.DisplayableCEFilterIdToNameMap[f]));
      }

      public override void BuildCountComponents(
        IVssRequestContext requestContext,
        ResultsCountPlatformRequest request,
        string rawQueryString,
        string rawFilterString,
        out Func<QueryContainerDescriptor<T>, QueryContainer> query)
      {
        query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      }

      public override IExpression BuildQueryFilterExpression(
        IVssRequestContext requestContext,
        IDictionary<string, IEnumerable<string>> searchFilters,
        IExpression queryParseTree,
        DocumentContractType contractType)
      {
        return new CodeFilterBuilder(queryParseTree, contractType, searchFilters).GetQueryFilterExpression();
      }

      public override string GetQueryRequestTimeout(IVssRequestContext requestContext) => requestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/SearchQueryRequestTimeout", true, "62s");

      public override void BuildSuggestComponents(
        IVssRequestContext requestContext,
        EntitySearchSuggestPlatformRequest suggestRequest,
        string rawFilterString,
        out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest)
      {
        suggest = new DefaultPhraseSuggesterBuilder().Suggest<T>();
      }
    }
  }
}
