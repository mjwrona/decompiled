// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.DedupeFileContractV4
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
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
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class DedupeFileContractV4 : DedupeFileContractBase
  {
    private NoPayloadContractUtils noPayloadContract;

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
    public override DocumentContractType ContractType => DocumentContractType.DedupeFileContractV4;

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;

    public DedupeFileContractV4() => this.Initialize();

    public DedupeFileContractV4(ISearchQueryClient elasticClient)
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
        CodeContractField.CodeSearchFieldDesc.PathsBranchNameOriginal,
        CodeContractField.CodeSearchFieldDesc.FilePathOriginal,
        CodeContractField.CodeSearchFieldDesc.FileExtension,
        CodeContractField.CodeSearchFieldDesc.PathsChangeId,
        CodeContractField.CodeSearchFieldDesc.ContentId,
        CodeContractField.CodeSearchFieldDesc.VersionControlType
      }.Select<CodeContractField.CodeSearchFieldDesc, CodeContractField>((Func<CodeContractField.CodeSearchFieldDesc, CodeContractField>) (x => new CodeContractField(x))).ToList<CodeContractField>();
      this.noPayloadContract = new NoPayloadContractUtils(this.fields);
    }

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled();

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<DedupeFileContractV4>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<DedupeFileContractV4>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<DedupeFileContractV4>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV4>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV4>(this.ShouldGetFieldNameWithHighlightHit(requestContext)));
    }

    public override IndexOperationsResponse InsertBatch<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> indexRequest,
      ISearchIndex searchIndex)
    {
      string str1 = (string) null;
      string repositoryName = (string) null;
      string str2 = (string) null;
      List<DedupeFileContractV4> source1 = new List<DedupeFileContractV4>();
      List<DedupeFileContractV4> source2 = new List<DedupeFileContractV4>();
      FriendlyDictionary<string, List<DedupeFileContractV4>> friendlyDictionary = new FriendlyDictionary<string, List<DedupeFileContractV4>>();
      foreach (T obj in indexRequest.Batch)
      {
        if (!(obj is DedupeFileContractV4 dedupeFileContractV4))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (str1 == null)
        {
          str1 = dedupeFileContractV4.RepositoryId.ToLowerInvariant();
          repositoryName = dedupeFileContractV4.GetRepoNameOriginal();
          str2 = dedupeFileContractV4.VcType;
        }
        if (dedupeFileContractV4.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV4.Paths.BranchNameOriginal)
          {
            List<DedupeFileContractV4> dedupeFileContractV4List;
            if (!friendlyDictionary.TryGetValue(key, out dedupeFileContractV4List))
            {
              dedupeFileContractV4List = new List<DedupeFileContractV4>();
              friendlyDictionary.Add(key, dedupeFileContractV4List);
            }
            dedupeFileContractV4List.Add(dedupeFileContractV4);
          }
        }
        if (dedupeFileContractV4.UpdateType == MetaDataStoreUpdateType.Add || dedupeFileContractV4.UpdateType == MetaDataStoreUpdateType.Edit)
          source1.Add(dedupeFileContractV4);
        else if (dedupeFileContractV4.UpdateType == MetaDataStoreUpdateType.UpdateMetaData)
          source2.Add(dedupeFileContractV4);
      }
      IndexOperationsResponse deleteResponse = (IndexOperationsResponse) null;
      if (friendlyDictionary.Count > 0)
      {
        string currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/LargeRepoFoldersToUseSearchAndUpdate", defaultValue: string.Empty);
        List<string> source3 = new List<string>();
        if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        {
          string str3 = currentHostConfigValue;
          char[] separator = new char[1]{ ',' };
          foreach (string filepath in str3.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            string normalizedFolderPath = this.GetNormalizedFolderPath(filepath);
            source3.Add(normalizedFolderPath);
          }
        }
        DedupeFileContractV4 firstFile = friendlyDictionary[friendlyDictionary.Keys.First<string>()].First<DedupeFileContractV4>();
        if (executionContext.RequestContext.IsLargeRepository(repositoryName) && "Custom".Equals(str2, StringComparison.OrdinalIgnoreCase) && source3.Any<string>((Func<string, bool>) (x => firstFile.FilePath.StartsWith(x, StringComparison.Ordinal))))
        {
          FriendlyDictionary<string, IList<string>> branchNameOriginalToListOfFiles = new FriendlyDictionary<string, IList<string>>();
          HashSet<string> stringSet = new HashSet<string>();
          List<string> terms = new List<string>();
          foreach (KeyValuePair<string, List<DedupeFileContractV4>> keyValuePair in friendlyDictionary)
          {
            IList<string> list = (IList<string>) keyValuePair.Value.Select<DedupeFileContractV4, string>((Func<DedupeFileContractV4, string>) (x => x.FilePath)).ToList<string>();
            stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) list);
            terms.Add(keyValuePair.Key);
            branchNameOriginalToListOfFiles[keyValuePair.Key] = list;
          }
          BulkScriptUpdateByQueryRequest<DedupeFileContractV4> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV4>();
          updateByQueryRequest.ContractType = indexRequest.ContractType;
          updateByQueryRequest.ScriptName = "delete_dedupe_file_v2";
          updateByQueryRequest.ShouldUpsert = false;
          updateByQueryRequest.IndexIdentity = indexRequest.IndexIdentity;
          updateByQueryRequest.Routing = indexRequest.Routing;
          updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[3]
          {
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, (IEnumerable<string>) stringSet),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, str1),
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, (IEnumerable<string>) terms)
          });
          updateByQueryRequest.GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract => new FluentDictionary<string, object>()
          {
            {
              "allOriginalBranchesAndFilePaths",
              (object) branchNameOriginalToListOfFiles
            }
          });
          BulkScriptUpdateByQueryRequest<DedupeFileContractV4> scriptUpdateByQueryRequest = updateByQueryRequest;
          deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV4>(executionContext, scriptUpdateByQueryRequest);
        }
        else
        {
          foreach (KeyValuePair<string, List<DedupeFileContractV4>> keyValuePair in friendlyDictionary)
          {
            firstFile = keyValuePair.Value.First<DedupeFileContractV4>();
            string branchNameOriginal = keyValuePair.Key;
            int index = firstFile.Paths.BranchNameOriginal.IndexOf(branchNameOriginal);
            string branchName = firstFile.Paths.BranchName[index];
            string repoName = firstFile.RepoName;
            if (keyValuePair.Value.Any<DedupeFileContractV4>((Func<DedupeFileContractV4, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
              throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
            BulkScriptUpdateByQueryRequest<DedupeFileContractV4> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV4>();
            updateByQueryRequest.ContractType = indexRequest.ContractType;
            updateByQueryRequest.ScriptName = "delete_dedupe_file";
            updateByQueryRequest.ShouldUpsert = false;
            updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV4>) keyValuePair.Value;
            updateByQueryRequest.IndexIdentity = indexRequest.IndexIdentity;
            updateByQueryRequest.Routing = indexRequest.Routing;
            updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
            {
              (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV4, string>((Func<DedupeFileContractV4, string>) (fc => fc.FilePath))),
              (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchNameOriginal.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, branchNameOriginal),
              (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repoName),
              (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.ProjectName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, firstFile.ProjectName)
            });
            updateByQueryRequest.GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract => new FluentDictionary<string, object>()
            {
              {
                "oldBranchName",
                (object) branchName
              },
              {
                "oldBranchNameOriginal",
                (object) branchNameOriginal
              }
            });
            BulkScriptUpdateByQueryRequest<DedupeFileContractV4> scriptUpdateByQueryRequest = updateByQueryRequest;
            deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV4>(executionContext, scriptUpdateByQueryRequest);
          }
        }
      }
      bool forceReIndexContent = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/DedupeFileContractForceReIndexContent", true);
      BulkScriptUpdateRequest<DedupeFileContractV4> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<DedupeFileContractV4>()
      {
        IndexIdentity = indexRequest.IndexIdentity,
        Routing = indexRequest.Routing,
        ContractType = indexRequest.ContractType,
        ScriptName = "update_dedupe_file",
        GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
        {
          if (!(contract is DedupeFileContractV4 dedupeFileContractV4_2))
            throw new InvalidOperationException("Accepts only DedupeFileContract.");
          FluentDictionary<string, object> fluentDictionary = new FluentDictionary<string, object>()
          {
            {
              "newBranchName",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV4_2.Paths.BranchName)
            },
            {
              "newBranchNameOriginal",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV4_2.Paths.BranchNameOriginal)
            },
            {
              "newChangeId",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV4_2.Paths.ChangeId)
            },
            {
              "isDefaultBranch",
              (object) dedupeFileContractV4_2.IsDefaultBranch
            }
          };
          if (forceReIndexContent)
            fluentDictionary.Add("content", (object) dedupeFileContractV4_2.Content);
          return fluentDictionary;
        })
      };
      IndexOperationsResponse addResponse = (IndexOperationsResponse) null;
      if (source1.Any<DedupeFileContractV4>())
      {
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV4>) source1;
        bulkScriptUpdateRequest.ShouldUpsert = true;
        addResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV4>(executionContext, bulkScriptUpdateRequest);
      }
      IndexOperationsResponse updateResponse = (IndexOperationsResponse) null;
      if (source2.Any<DedupeFileContractV4>())
      {
        if (forceReIndexContent)
          throw new SearchServiceException("IgnoreMetadataStore flag is set, we shouldn't be creating records with UpdateMetadata.");
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV4>) source2;
        bulkScriptUpdateRequest.ShouldUpsert = false;
        updateResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV4>(executionContext, bulkScriptUpdateRequest);
      }
      return this.GenerateCombinedResponse(addResponse, updateResponse, deleteResponse);
    }

    public override IndexOperationsResponse DeleteDocuments<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> deleteRequest,
      ISearchIndex searchIndex)
    {
      List<DedupeFileContractV4> source1 = new List<DedupeFileContractV4>();
      Dictionary<string, List<DedupeFileContractV4>> source2 = new Dictionary<string, List<DedupeFileContractV4>>();
      foreach (T obj in deleteRequest.Batch)
      {
        if (!(obj is DedupeFileContractV4 dedupeFileContractV4))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (dedupeFileContractV4.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV4.Paths.BranchName)
          {
            List<DedupeFileContractV4> dedupeFileContractV4List;
            if (!source2.TryGetValue(key, out dedupeFileContractV4List))
            {
              dedupeFileContractV4List = new List<DedupeFileContractV4>();
              source2.Add(key, dedupeFileContractV4List);
            }
            dedupeFileContractV4List.Add(dedupeFileContractV4);
          }
        }
        else
          source1.Add(dedupeFileContractV4);
      }
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      IndexOperationsResponse operationsResponse2 = (IndexOperationsResponse) null;
      if (source2.Any<KeyValuePair<string, List<DedupeFileContractV4>>>())
      {
        foreach (KeyValuePair<string, List<DedupeFileContractV4>> keyValuePair in source2)
        {
          DedupeFileContractV4 dedupeFileContractV4 = keyValuePair.Value.First<DedupeFileContractV4>();
          string branchName = keyValuePair.Key;
          int index = dedupeFileContractV4.Paths.BranchName.IndexOf(branchName);
          string branchNameOriginal = dedupeFileContractV4.Paths.BranchNameOriginal[index];
          string repoName = dedupeFileContractV4.RepoName;
          if (keyValuePair.Value.Any<DedupeFileContractV4>((Func<DedupeFileContractV4, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
            throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
          BulkScriptUpdateByQueryRequest<DedupeFileContractV4> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV4>();
          updateByQueryRequest.ContractType = deleteRequest.ContractType;
          updateByQueryRequest.ScriptName = "delete_dedupe_file";
          updateByQueryRequest.ShouldUpsert = false;
          updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV4>) keyValuePair.Value;
          updateByQueryRequest.IndexIdentity = deleteRequest.IndexIdentity;
          updateByQueryRequest.Routing = deleteRequest.Routing;
          updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
          {
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV4, string>((Func<DedupeFileContractV4, string>) (fc => fc.FilePath))),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, branchName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repoName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.ProjectName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, dedupeFileContractV4.ProjectName)
          });
          updateByQueryRequest.GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract => new FluentDictionary<string, object>()
          {
            {
              "oldBranchName",
              (object) branchName
            },
            {
              "oldBranchNameOriginal",
              (object) branchNameOriginal
            }
          });
          BulkScriptUpdateByQueryRequest<DedupeFileContractV4> scriptUpdateByQueryRequest = updateByQueryRequest;
          operationsResponse1 = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV4>(executionContext, scriptUpdateByQueryRequest);
        }
      }
      if (source1.Any<DedupeFileContractV4>())
      {
        BulkScriptUpdateRequest<T> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<T>()
        {
          Batch = deleteRequest.Batch,
          IndexIdentity = deleteRequest.IndexIdentity,
          Routing = deleteRequest.Routing,
          ContractType = deleteRequest.ContractType,
          ScriptName = "delete_dedupe_file",
          ShouldUpsert = false,
          GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
          {
            if (!(contract is DedupeFileContractV4 dedupeFileContractV4_2))
              throw new InvalidOperationException("Accepts only DedupeFileContract.");
            return new FluentDictionary<string, object>()
            {
              {
                "oldBranchName",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV4_2.Paths.BranchName)
              },
              {
                "oldBranchNameOriginal",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV4_2.Paths.BranchNameOriginal)
              }
            };
          })
        };
        operationsResponse2 = searchIndex.BulkScriptUpdateSync<T>(executionContext, bulkScriptUpdateRequest);
      }
      if (operationsResponse2 == null || operationsResponse1 == null)
        return operationsResponse2 ?? operationsResponse1 ?? (IndexOperationsResponse) null;
      return new IndexOperationsResponse()
      {
        FailedItems = operationsResponse2.FailedItems.Concat<FailedItem>(operationsResponse1.FailedItems),
        FailedItemsCount = operationsResponse2.FailedItemsCount + operationsResponse1.FailedItemsCount,
        ItemsCount = operationsResponse2.ItemsCount + operationsResponse1.ItemsCount,
        Success = operationsResponse2.Success && operationsResponse1.Success
      };
    }

    public override IndexOperationsResponse DeleteDocumentsByQuery<T>(
      ExecutionContext executionContext,
      BulkDeleteByQueryRequest<T> bulkDeleteByQueryRequest,
      ISearchIndex searchIndex,
      bool forceComplete)
    {
      if (!(bulkDeleteByQueryRequest.Query is AndExpression))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unexpected structure of {0}.{1} found. Value is [{2}].", (object) nameof (bulkDeleteByQueryRequest), (object) "Query", (object) bulkDeleteByQueryRequest.Query)));
      bool flag = false;
      string branchesToDelete = string.Empty;
      string branchesNameOriginalToDelete = string.Empty;
      foreach (IExpression child in bulkDeleteByQueryRequest.Query.Children)
      {
        if (child.GetType() == typeof (TermsExpression))
        {
          TermsExpression termsExpression = child as TermsExpression;
          if (termsExpression.Type == "paths.branchNameOriginal")
          {
            flag = !flag ? true : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Request query contains multiple entries of {0}", (object) "paths.branchNameOriginal")));
            branchesNameOriginalToDelete = string.Join(",", termsExpression.Terms);
          }
        }
      }
      if (!flag)
        return searchIndex.BulkDeleteByQuery<T>(executionContext, bulkDeleteByQueryRequest, forceComplete);
      branchesToDelete = branchesNameOriginalToDelete.NormalizePath();
      BulkScriptUpdateByQueryRequest<DedupeFileContractV4> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV4>();
      updateByQueryRequest.Query = bulkDeleteByQueryRequest.Query;
      updateByQueryRequest.ContractType = bulkDeleteByQueryRequest.ContractType;
      updateByQueryRequest.ScriptName = "delete_dedupe_file";
      updateByQueryRequest.ShouldUpsert = false;
      updateByQueryRequest.GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract => new FluentDictionary<string, object>()
      {
        {
          "oldBranchName",
          (object) branchesToDelete
        },
        {
          "oldBranchNameOriginal",
          (object) branchesNameOriginalToDelete
        }
      });
      BulkScriptUpdateByQueryRequest<DedupeFileContractV4> scriptUpdateByQueryRequest = updateByQueryRequest;
      return searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV4>(executionContext, scriptUpdateByQueryRequest);
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

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      return (IEnumerable<CodeFileContract>) searchIndex.BulkGetByQuery<DedupeFileContractV4>(executionContext, request);
    }

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      string inputHighlighter = requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() ? "noPayloadCodeSearchHighlighter_v2" : "noPayloadCodeSearchHighlighter";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
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
      keywordProperty4.Index = new bool?(true);
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
      PropertyName name10 = (PropertyName) CodeContractField.CodeSearchFieldDesc.Paths.ElasticsearchFieldName();
      ObjectProperty objectProperty = new ObjectProperty();
      objectProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.Paths.ElasticsearchFieldName();
      Properties properties4 = new Properties();
      PropertyName name11 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      KeywordProperty keywordProperty10 = new KeywordProperty();
      keywordProperty10.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      keywordProperty10.Index = new bool?(false);
      keywordProperty10.Store = new bool?(true);
      keywordProperty10.DocValues = new bool?(false);
      properties4.Add(name11, (IProperty) keywordProperty10);
      PropertyName name12 = (PropertyName) "branchNameOriginal";
      KeywordProperty keywordProperty11 = new KeywordProperty();
      keywordProperty11.Name = (PropertyName) "branchNameOriginal";
      keywordProperty11.Index = new bool?(true);
      keywordProperty11.Store = new bool?(true);
      keywordProperty11.DocValues = new bool?(true);
      properties4.Add(name12, (IProperty) keywordProperty11);
      PropertyName name13 = (PropertyName) "branchName";
      KeywordProperty keywordProperty12 = new KeywordProperty();
      keywordProperty12.Name = (PropertyName) "branchName";
      keywordProperty12.Index = new bool?(true);
      keywordProperty12.Store = new bool?(false);
      keywordProperty12.DocValues = new bool?(true);
      properties4.Add(name13, (IProperty) keywordProperty12);
      objectProperty.Properties = (IProperties) properties4;
      properties1.Add(name10, (IProperty) objectProperty);
      PropertyName name14 = (PropertyName) CodeContractField.CodeSearchFieldDesc.LastChangeUtcTime.ElasticsearchFieldName();
      DateProperty dateProperty1 = new DateProperty();
      dateProperty1.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.LastChangeUtcTime.ElasticsearchFieldName();
      dateProperty1.Store = new bool?(true);
      properties1.Add(name14, (IProperty) dateProperty1);
      PropertyName name15 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      KeywordProperty keywordProperty13 = new KeywordProperty();
      keywordProperty13.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      keywordProperty13.Index = new bool?(false);
      keywordProperty13.Store = new bool?(true);
      keywordProperty13.DocValues = new bool?(false);
      properties1.Add(name15, (IProperty) keywordProperty13);
      PropertyName name16 = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      KeywordProperty keywordProperty14 = new KeywordProperty();
      keywordProperty14.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      keywordProperty14.Index = new bool?(true);
      keywordProperty14.Store = new bool?(true);
      keywordProperty14.DocValues = new bool?(false);
      properties1.Add(name16, (IProperty) keywordProperty14);
      PropertyName name17 = (PropertyName) "collectionName";
      KeywordProperty keywordProperty15 = new KeywordProperty();
      keywordProperty15.Name = (PropertyName) "collectionName";
      keywordProperty15.Index = new bool?(false);
      keywordProperty15.Store = new bool?(false);
      keywordProperty15.DocValues = new bool?(false);
      keywordProperty15.Normalizer = "lowerCaseNormalizer";
      Properties properties5 = new Properties();
      PropertyName key3 = (PropertyName) "raw";
      KeywordProperty keywordProperty16 = new KeywordProperty();
      keywordProperty16.Name = (PropertyName) "raw";
      keywordProperty16.Index = new bool?(false);
      keywordProperty16.Store = new bool?(true);
      keywordProperty16.DocValues = new bool?(true);
      properties5[key3] = (IProperty) keywordProperty16;
      keywordProperty15.Fields = (IProperties) properties5;
      properties1.Add(name17, (IProperty) keywordProperty15);
      PropertyName name18 = (PropertyName) "collectionId";
      KeywordProperty keywordProperty17 = new KeywordProperty();
      keywordProperty17.Name = (PropertyName) "collectionId";
      keywordProperty17.Index = new bool?(true);
      keywordProperty17.Store = new bool?(true);
      keywordProperty17.DocValues = new bool?(true);
      properties1.Add(name18, (IProperty) keywordProperty17);
      PropertyName name19 = (PropertyName) "indexedTimeStamp";
      DateProperty dateProperty2 = new DateProperty();
      dateProperty2.Name = (PropertyName) "indexedTimeStamp";
      dateProperty2.Format = "epoch_second";
      dateProperty2.Index = new bool?(true);
      dateProperty2.Store = new bool?(true);
      dateProperty2.DocValues = new bool?(false);
      Properties properties6 = new Properties();
      PropertyName key4 = (PropertyName) "raw";
      DateProperty dateProperty3 = new DateProperty();
      dateProperty3.Name = (PropertyName) "raw";
      dateProperty3.Format = "epoch_second";
      dateProperty3.Index = new bool?(false);
      dateProperty3.Store = new bool?(false);
      dateProperty3.DocValues = new bool?(true);
      properties6[key4] = (IProperty) dateProperty3;
      dateProperty2.Fields = (IProperties) properties6;
      properties1.Add(name19, (IProperty) dateProperty2);
      PropertyName name20 = (PropertyName) "projectName";
      KeywordProperty keywordProperty18 = new KeywordProperty();
      keywordProperty18.Name = (PropertyName) "projectName";
      keywordProperty18.Index = new bool?(true);
      keywordProperty18.Store = new bool?(false);
      keywordProperty18.DocValues = new bool?(false);
      keywordProperty18.Normalizer = "lowerCaseNormalizer";
      Properties properties7 = new Properties();
      PropertyName key5 = (PropertyName) "raw";
      KeywordProperty keywordProperty19 = new KeywordProperty();
      keywordProperty19.Name = (PropertyName) "raw";
      keywordProperty19.Index = new bool?(false);
      keywordProperty19.Store = new bool?(true);
      keywordProperty19.DocValues = new bool?(true);
      properties7[key5] = (IProperty) keywordProperty19;
      keywordProperty18.Fields = (IProperties) properties7;
      properties1.Add(name20, (IProperty) keywordProperty18);
      PropertyName name21 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      KeywordProperty keywordProperty20 = new KeywordProperty();
      keywordProperty20.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      keywordProperty20.Index = new bool?(true);
      keywordProperty20.Store = new bool?(true);
      keywordProperty20.DocValues = new bool?(true);
      properties1.Add(name21, (IProperty) keywordProperty20);
      PropertyName name22 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      KeywordProperty keywordProperty21 = new KeywordProperty();
      keywordProperty21.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      keywordProperty21.Index = new bool?(false);
      keywordProperty21.Store = new bool?(true);
      keywordProperty21.DocValues = new bool?(false);
      properties1.Add(name22, (IProperty) keywordProperty21);
      PropertyName name23 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      KeywordProperty keywordProperty22 = new KeywordProperty();
      keywordProperty22.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      keywordProperty22.Index = new bool?(true);
      keywordProperty22.Store = new bool?(false);
      keywordProperty22.DocValues = new bool?(true);
      keywordProperty22.Normalizer = "lowerCaseNormalizer";
      Properties properties8 = new Properties();
      PropertyName key6 = (PropertyName) "raw";
      KeywordProperty keywordProperty23 = new KeywordProperty();
      keywordProperty23.Name = (PropertyName) "raw";
      keywordProperty23.Index = new bool?(false);
      keywordProperty23.Store = new bool?(true);
      keywordProperty23.DocValues = new bool?(true);
      properties8[key6] = (IProperty) keywordProperty23;
      keywordProperty22.Fields = (IProperties) properties8;
      properties1.Add(name23, (IProperty) keywordProperty22);
      PropertyName name24 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      KeywordProperty keywordProperty24 = new KeywordProperty();
      keywordProperty24.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      keywordProperty24.Index = new bool?(true);
      keywordProperty24.Store = new bool?(true);
      keywordProperty24.DocValues = new bool?(true);
      properties1.Add(name24, (IProperty) keywordProperty24);
      PropertyName name25 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      BooleanProperty booleanProperty = new BooleanProperty();
      booleanProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      booleanProperty.Index = new bool?(true);
      booleanProperty.Store = new bool?(false);
      booleanProperty.DocValues = new bool?(false);
      properties1.Add(name25, (IProperty) booleanProperty);
      Properties properties9 = properties1;
      this.noPayloadContract.AddNonPayloadProperties(properties9, requestContext);
      return (ITypeMapping) new TypeMapping()
      {
        Meta = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          ["version"] = (object) indexVersion
        },
        Properties = (IProperties) properties9,
        RoutingField = (IRoutingField) new RoutingField()
        {
          Required = new bool?(true)
        }
      };
    }

    public override string GetSearchFieldForCodeElementFilterType(string tokenKind) => this.noPayloadContract.GetSearchFieldForCodeElementFilterType(tokenKind);

    public override bool IsOriginalContentAvailable(IVssRequestContext requestContext) => true;

    internal override string CreateTermQueryStringForDefaultType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (termExpression.Value.StartsWith("*", StringComparison.Ordinal) || termExpression.Value.StartsWith("?", StringComparison.Ordinal))
      {
        string str = ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value);
        string rewriteMethod = CodeFileContract.GetRewriteMethod(requestContext, this.ContractType);
        char[] charArray = str.ToCharArray();
        Array.Reverse((Array) charArray);
        return ElasticsearchQueryBuilder.BuildWildcardQuery(this.fields[CodeFileContract.CodeContractQueryableElement.Prefix].ElasticsearchFieldName, new string(charArray), rewriteMethod);
      }
      return termExpression.Value.Contains("?") || termExpression.Value.Contains("*") ? this.CreateTermQueryString(requestContext, termExpression, enableRanking, requestId) : this.CreateBoolShouldQueryStringWithCEFieldsBoosting(requestContext, termExpression);
    }

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

    internal string CreateBoolShouldQueryStringWithCEFieldsBoosting(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      string str = ElasticsearchQueryBuilder.BuildTermQuery(this.GetSearchFieldForType(termExpression.Type), termExpression.Value);
      List<string> childQueries = new List<string>();
      childQueries.Add(str);
      childQueries.AddRange((IEnumerable<string>) this.noPayloadContract.AddTermQueryOnCEFieldWithBoost(requestContext, termExpression));
      return ElasticsearchQueryBuilder.BuildBoolShouldQuery(childQueries);
    }

    internal string CreateBoolShouldQueryStringWithoutCEFieldsBoosting(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      return ElasticsearchQueryBuilder.BuildTermQuery(!termExpression.IsOfType("regex") || !requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramSearch", TeamFoundationHostType.ProjectCollection) ? this.GetSearchFieldForType(termExpression.Type) : "originalContent.trigram", termExpression.Value);
    }

    internal override string CreateCodeElementQueryString(
      string tokenType,
      string tokenValue,
      IEnumerable<int> codeTokenIds,
      bool enableRanking,
      string requestId,
      string rewriteMethod = "top_terms_boost_100")
    {
      return this.noPayloadContract.CreateCodeElementQueryString(tokenType, tokenValue, rewriteMethod);
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
      this.noPayloadContract.AddNonPayloadIndexSettings(codeIndexSettings, executionContext, "/Service/ALMSearch/Settings/IndexSortingFieldsInDedupeContractV4");
      return codeIndexSettings;
    }

    public override string CorrectFilePath(string filePath) => this.noPayloadContract.CorrectFilePath(filePath);

    public override int GetDocumentContractSize()
    {
      int documentContractSize = base.GetDocumentContractSize();
      if (this.OriginalContent != null)
        documentContractSize += Encoding.UTF8.GetByteCount(this.OriginalContent);
      return documentContractSize;
    }

    protected virtual Properties GetFieldsForOriginalContent(IVssRequestContext requestContext)
    {
      Properties forOriginalContent = new Properties();
      TextProperty textProperty1 = new TextProperty()
      {
        Analyzer = "reversetextanalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      };
      forOriginalContent.Add((PropertyName) "reverse", (IProperty) textProperty1);
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramIndexing", TeamFoundationHostType.Deployment))
      {
        TextProperty textProperty2 = new TextProperty();
        textProperty2.Analyzer = "ngramanalyzer";
        textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
        textProperty2.Norms = new bool?(false);
        textProperty2.Store = new bool?(false);
        textProperty2.Index = new bool?(true);
        TextProperty textProperty3 = textProperty2;
        forOriginalContent.Add((PropertyName) "trigram", (IProperty) textProperty3);
      }
      return forOriginalContent;
    }

    protected override string GetNormalizedFolderPath(string filepath) => this.CorrectFilePath(filepath) + "/";

    protected override string GetRepoNameOriginal() => this.RepoName;
  }
}
