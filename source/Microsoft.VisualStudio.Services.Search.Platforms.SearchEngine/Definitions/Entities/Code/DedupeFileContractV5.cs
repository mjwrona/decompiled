// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.DedupeFileContractV5
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
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class DedupeFileContractV5 : DedupeFileContractV4
  {
    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.DedupeFileContractV5;

    public DedupeFileContractV5()
    {
    }

    public DedupeFileContractV5(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
    }

    protected override Properties GetFieldsForOriginalContent(IVssRequestContext requestContext)
    {
      Properties forOriginalContent = new Properties();
      if (!this.IsNGramIndexingEnabled(requestContext))
        return base.GetFieldsForOriginalContent(requestContext);
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Analyzer = "fast_wildcard";
      textProperty1.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty1.Norms = new bool?(false);
      textProperty1.Store = new bool?(false);
      textProperty1.Index = new bool?(true);
      TextProperty textProperty2 = textProperty1;
      forOriginalContent.Add((PropertyName) "ngram", (IProperty) textProperty2);
      return forOriginalContent;
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
      if (this.IsNGramIndexingEnabled(executionContext.RequestContext))
      {
        this.ConfigureSettingsForIndexingNGrams(executionContext.RequestContext, codeIndexSettings);
      }
      else
      {
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
      }
      return codeIndexSettings;
    }

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<DedupeFileContractV5>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<DedupeFileContractV5>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<DedupeFileContractV5>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV5>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV5>(this.ShouldGetFieldNameWithHighlightHit(requestContext), false, this.ShouldGetCodeSnippetWithHighlightHit(requestContext)));
    }

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      return (IEnumerable<CodeFileContract>) searchIndex.BulkGetByQuery<DedupeFileContractV5>(executionContext, request);
    }

    public override IndexOperationsResponse InsertBatch<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> indexRequest,
      ISearchIndex searchIndex)
    {
      string str1 = (string) null;
      string repositoryName = (string) null;
      string str2 = (string) null;
      List<DedupeFileContractV5> source1 = new List<DedupeFileContractV5>();
      List<DedupeFileContractV5> source2 = new List<DedupeFileContractV5>();
      FriendlyDictionary<string, List<DedupeFileContractV5>> friendlyDictionary = new FriendlyDictionary<string, List<DedupeFileContractV5>>();
      foreach (T obj in indexRequest.Batch)
      {
        if (!(obj is DedupeFileContractV5 dedupeFileContractV5))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (str1 == null)
        {
          str1 = dedupeFileContractV5.RepositoryId.ToLowerInvariant();
          repositoryName = dedupeFileContractV5.GetRepoNameOriginal();
          str2 = dedupeFileContractV5.VcType;
        }
        if (dedupeFileContractV5.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV5.Paths.BranchNameOriginal)
          {
            List<DedupeFileContractV5> dedupeFileContractV5List;
            if (!friendlyDictionary.TryGetValue(key, out dedupeFileContractV5List))
            {
              dedupeFileContractV5List = new List<DedupeFileContractV5>();
              friendlyDictionary.Add(key, dedupeFileContractV5List);
            }
            dedupeFileContractV5List.Add(dedupeFileContractV5);
          }
        }
        if (dedupeFileContractV5.UpdateType == MetaDataStoreUpdateType.Add || dedupeFileContractV5.UpdateType == MetaDataStoreUpdateType.Edit)
          source1.Add(dedupeFileContractV5);
        else if (dedupeFileContractV5.UpdateType == MetaDataStoreUpdateType.UpdateMetaData)
          source2.Add(dedupeFileContractV5);
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
        DedupeFileContractV5 firstFile = friendlyDictionary[friendlyDictionary.Keys.First<string>()].First<DedupeFileContractV5>();
        if (executionContext.RequestContext.IsLargeRepository(repositoryName) && "Custom".Equals(str2, StringComparison.OrdinalIgnoreCase) && source3.Any<string>((Func<string, bool>) (x => firstFile.FilePath.StartsWith(x, StringComparison.Ordinal))))
        {
          FriendlyDictionary<string, IList<string>> branchNameOriginalToListOfFiles = new FriendlyDictionary<string, IList<string>>();
          HashSet<string> stringSet = new HashSet<string>();
          List<string> terms = new List<string>();
          foreach (KeyValuePair<string, List<DedupeFileContractV5>> keyValuePair in friendlyDictionary)
          {
            IList<string> list = (IList<string>) keyValuePair.Value.Select<DedupeFileContractV5, string>((Func<DedupeFileContractV5, string>) (x => x.FilePath)).ToList<string>();
            stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) list);
            terms.Add(keyValuePair.Key);
            branchNameOriginalToListOfFiles[keyValuePair.Key] = list;
          }
          BulkScriptUpdateByQueryRequest<DedupeFileContractV5> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV5>();
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
          BulkScriptUpdateByQueryRequest<DedupeFileContractV5> scriptUpdateByQueryRequest = updateByQueryRequest;
          deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV5>(executionContext, scriptUpdateByQueryRequest);
        }
        else
        {
          foreach (KeyValuePair<string, List<DedupeFileContractV5>> keyValuePair in friendlyDictionary)
          {
            firstFile = keyValuePair.Value.First<DedupeFileContractV5>();
            string branchNameOriginal = keyValuePair.Key;
            int index = firstFile.Paths.BranchNameOriginal.IndexOf(branchNameOriginal);
            string branchName = firstFile.Paths.BranchName[index];
            string repoName = firstFile.RepoName;
            if (keyValuePair.Value.Any<DedupeFileContractV5>((Func<DedupeFileContractV5, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
              throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
            BulkScriptUpdateByQueryRequest<DedupeFileContractV5> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV5>();
            updateByQueryRequest.ContractType = indexRequest.ContractType;
            updateByQueryRequest.ScriptName = "delete_dedupe_file";
            updateByQueryRequest.ShouldUpsert = false;
            updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV5>) keyValuePair.Value;
            updateByQueryRequest.IndexIdentity = indexRequest.IndexIdentity;
            updateByQueryRequest.Routing = indexRequest.Routing;
            updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
            {
              (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV5, string>((Func<DedupeFileContractV5, string>) (fc => fc.FilePath))),
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
            BulkScriptUpdateByQueryRequest<DedupeFileContractV5> scriptUpdateByQueryRequest = updateByQueryRequest;
            deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV5>(executionContext, scriptUpdateByQueryRequest);
          }
        }
      }
      bool forceReIndexContent = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/DedupeFileContractForceReIndexContent", true);
      BulkScriptUpdateRequest<DedupeFileContractV5> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<DedupeFileContractV5>()
      {
        IndexIdentity = indexRequest.IndexIdentity,
        Routing = indexRequest.Routing,
        ContractType = indexRequest.ContractType,
        ScriptName = "update_dedupe_file",
        GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
        {
          if (!(contract is DedupeFileContractV5 dedupeFileContractV5_2))
            throw new InvalidOperationException("Accepts only DedupeFileContract.");
          FluentDictionary<string, object> fluentDictionary = new FluentDictionary<string, object>()
          {
            {
              "newBranchName",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV5_2.Paths.BranchName)
            },
            {
              "newBranchNameOriginal",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV5_2.Paths.BranchNameOriginal)
            },
            {
              "newChangeId",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV5_2.Paths.ChangeId)
            },
            {
              "isDefaultBranch",
              (object) dedupeFileContractV5_2.IsDefaultBranch
            }
          };
          if (forceReIndexContent)
            fluentDictionary.Add("content", (object) dedupeFileContractV5_2.Content);
          return fluentDictionary;
        })
      };
      IndexOperationsResponse addResponse = (IndexOperationsResponse) null;
      if (source1.Any<DedupeFileContractV5>())
      {
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV5>) source1;
        bulkScriptUpdateRequest.ShouldUpsert = true;
        addResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV5>(executionContext, bulkScriptUpdateRequest);
      }
      IndexOperationsResponse updateResponse = (IndexOperationsResponse) null;
      if (source2.Any<DedupeFileContractV5>())
      {
        if (forceReIndexContent)
          throw new SearchServiceException("IgnoreMetadataStore flag is set, we shouldn't be creating records with UpdateMetadata.");
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV5>) source2;
        bulkScriptUpdateRequest.ShouldUpsert = false;
        updateResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV5>(executionContext, bulkScriptUpdateRequest);
      }
      return this.GenerateCombinedResponse(addResponse, updateResponse, deleteResponse);
    }

    public override IndexOperationsResponse DeleteDocuments<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> deleteRequest,
      ISearchIndex searchIndex)
    {
      List<DedupeFileContractV5> source1 = new List<DedupeFileContractV5>();
      Dictionary<string, List<DedupeFileContractV5>> source2 = new Dictionary<string, List<DedupeFileContractV5>>();
      foreach (T obj in deleteRequest.Batch)
      {
        if (!(obj is DedupeFileContractV5 dedupeFileContractV5))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (dedupeFileContractV5.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV5.Paths.BranchName)
          {
            List<DedupeFileContractV5> dedupeFileContractV5List;
            if (!source2.TryGetValue(key, out dedupeFileContractV5List))
            {
              dedupeFileContractV5List = new List<DedupeFileContractV5>();
              source2.Add(key, dedupeFileContractV5List);
            }
            dedupeFileContractV5List.Add(dedupeFileContractV5);
          }
        }
        else
          source1.Add(dedupeFileContractV5);
      }
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      IndexOperationsResponse operationsResponse2 = (IndexOperationsResponse) null;
      if (source2.Any<KeyValuePair<string, List<DedupeFileContractV5>>>())
      {
        foreach (KeyValuePair<string, List<DedupeFileContractV5>> keyValuePair in source2)
        {
          DedupeFileContractV5 dedupeFileContractV5 = keyValuePair.Value.First<DedupeFileContractV5>();
          string branchName = keyValuePair.Key;
          int index = dedupeFileContractV5.Paths.BranchName.IndexOf(branchName);
          string branchNameOriginal = dedupeFileContractV5.Paths.BranchNameOriginal[index];
          string repoName = dedupeFileContractV5.RepoName;
          if (keyValuePair.Value.Any<DedupeFileContractV5>((Func<DedupeFileContractV5, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
            throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
          BulkScriptUpdateByQueryRequest<DedupeFileContractV5> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV5>();
          updateByQueryRequest.ContractType = deleteRequest.ContractType;
          updateByQueryRequest.ScriptName = "delete_dedupe_file";
          updateByQueryRequest.ShouldUpsert = false;
          updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV5>) keyValuePair.Value;
          updateByQueryRequest.IndexIdentity = deleteRequest.IndexIdentity;
          updateByQueryRequest.Routing = deleteRequest.Routing;
          updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
          {
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV5, string>((Func<DedupeFileContractV5, string>) (fc => fc.FilePath))),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, branchName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repoName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.ProjectName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, dedupeFileContractV5.ProjectName)
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
          BulkScriptUpdateByQueryRequest<DedupeFileContractV5> scriptUpdateByQueryRequest = updateByQueryRequest;
          operationsResponse1 = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV5>(executionContext, scriptUpdateByQueryRequest);
        }
      }
      if (source1.Any<DedupeFileContractV5>())
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
            if (!(contract is DedupeFileContractV5 dedupeFileContractV5_2))
              throw new InvalidOperationException("Accepts only DedupeFileContract.");
            return new FluentDictionary<string, object>()
            {
              {
                "oldBranchName",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV5_2.Paths.BranchName)
              },
              {
                "oldBranchNameOriginal",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV5_2.Paths.BranchNameOriginal)
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
      BulkScriptUpdateByQueryRequest<DedupeFileContractV5> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV5>();
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
      BulkScriptUpdateByQueryRequest<DedupeFileContractV5> scriptUpdateByQueryRequest = updateByQueryRequest;
      return searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV5>(executionContext, scriptUpdateByQueryRequest);
    }

    internal override string CreateTermQueryStringForDefaultType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (!requestContext.IsQueryingNGramsEnabled())
        return base.CreateTermQueryStringForDefaultType(requestContext, termExpression, enableRanking, requestId);
      return termExpression.Value.Contains("?") || termExpression.Value.Contains("*") ? this.CreateTermQueryString(requestContext, termExpression, enableRanking, requestId) : this.CreateBoolShouldQueryStringWithCEFieldsBoosting(requestContext, termExpression);
    }

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() || this.ShouldGetCodeSnippetWithHighlightHit(requestContext);

    protected bool ShouldGetCodeSnippetWithHighlightHit(IVssRequestContext requestContext) => requestContext.Items.ContainsKey("includeSnippetInCodeSearchKey") && requestContext.IsFeatureEnabled("Search.Server.Code.IncludeSnippetInHits");

    public override bool IsNGramIndexingEnabled(IVssRequestContext requestContext) => requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCodeNGramsIndexing");

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      string inputHighlighter = !this.ShouldGetCodeSnippetWithHighlightHit(requestContext) ? (!requestContext.IsNoPayloadCodeSearchHighlighterV2FeatureEnabled() ? "noPayloadCodeSearchHighlighter" : "noPayloadCodeSearchHighlighter_v2") : "noPayloadCodeSearchHighlighter_v3";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
    }

    protected override string GetOriginalContent(byte[] originalContent) => new TextEncoding().GetString(originalContent);

    internal override string ConvertToTrigramPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (requestContext.IsQueryingNGramsEnabled())
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} field is not indexed. Query on {1} field instead.", (object) "originalContent.trigram", (object) "originalContent.ngram")));
      return base.ConvertToTrigramPhraseQueryString(requestContext, termExpression, enableRanking, requestId);
    }

    private void ConfigureSettingsForIndexingNGrams(
      IVssRequestContext requestContext,
      IndexSettings indexSettings)
    {
      int num1 = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CodeMinEdgeGramSizeForIndexing");
      int num2 = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CodeInlineGramSizeForIndexing");
      if (num1 <= 0 || num2 <= 0)
      {
        requestContext.SetConfigValue<int>("/Service/ALMSearch/Settings/CodeMinEdgeGramSizeForIndexing", num1 = 2);
        requestContext.SetConfigValue<int>("/Service/ALMSearch/Settings/CodeInlineGramSizeForIndexing", num2 = 3);
      }
      indexSettings.Analysis.TokenFilters["fast_wildcard"] = (ITokenFilter) new FastWildcardTokenFilter()
      {
        MinEdgeGram = new int?(num1),
        InlineGram = new int?(num2)
      };
      indexSettings.Analysis.Analyzers["fast_wildcard"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "wordtokenizer",
        Filter = (IEnumerable<string>) new string[2]
        {
          "lowercase",
          "fast_wildcard"
        }
      };
    }
  }
}
