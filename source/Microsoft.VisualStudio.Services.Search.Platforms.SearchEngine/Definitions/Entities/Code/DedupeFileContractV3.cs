// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.DedupeFileContractV3
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
  public class DedupeFileContractV3 : DedupeFileContractBase
  {
    private const string HighlighterName = "codesearch_v3";
    private const string HighlighterNameV4 = "codesearch_v4";

    public string OrganizationName { get; set; }

    public string OrganizationNameOriginal { get; set; }

    public string OrganizationId { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.DedupeFileContractV3;

    public DedupeFileContractV3() => this.Initialize();

    public DedupeFileContractV3(ISearchQueryClient elasticClient)
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
        CodeContractField.CodeSearchFieldDesc.PathsBranchNameOriginal,
        CodeContractField.CodeSearchFieldDesc.FilePathOriginal,
        CodeContractField.CodeSearchFieldDesc.FileExtension,
        CodeContractField.CodeSearchFieldDesc.PathsChangeId,
        CodeContractField.CodeSearchFieldDesc.ContentId,
        CodeContractField.CodeSearchFieldDesc.VersionControlType
      }.Select<CodeContractField.CodeSearchFieldDesc, CodeContractField>((Func<CodeContractField.CodeSearchFieldDesc, CodeContractField>) (x => new CodeContractField(x))).ToList<CodeContractField>();
    }

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      request.SearchFilters = this.CorrectSearchFilters(request as CodeSearchPlatformRequest);
      return this.SearchQueryClient.Search<DedupeFileContractV3>(requestContext, request, (IEnumerable<EntitySearchField>) this.GetSearchableFields(), EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      return this.SearchQueryClient.Count<DedupeFileContractV3>(requestContext, request, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      return this.SearchQueryClient.Suggest<DedupeFileContractV3>(requestContext, request, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Code"), (EntityIndexProvider<DedupeFileContractV3>) new CodeFileContract.CodeEntityQueryHandler<DedupeFileContractV3>(this.ShouldGetFieldNameWithHighlightHit(requestContext), this.ShouldParseFieldNameAsEnum(requestContext)));
    }

    public override IndexOperationsResponse InsertBatch<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> indexRequest,
      ISearchIndex searchIndex)
    {
      string str1 = (string) null;
      string repositoryName = (string) null;
      string str2 = (string) null;
      List<DedupeFileContractV3> source1 = new List<DedupeFileContractV3>();
      List<DedupeFileContractV3> source2 = new List<DedupeFileContractV3>();
      FriendlyDictionary<string, List<DedupeFileContractV3>> friendlyDictionary = new FriendlyDictionary<string, List<DedupeFileContractV3>>();
      foreach (T obj in indexRequest.Batch)
      {
        if (!(obj is DedupeFileContractV3 dedupeFileContractV3))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (str1 == null)
        {
          str1 = dedupeFileContractV3.RepositoryId.ToLowerInvariant();
          repositoryName = dedupeFileContractV3.GetRepoNameOriginal();
          str2 = dedupeFileContractV3.VcType;
        }
        if (dedupeFileContractV3.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV3.Paths.BranchNameOriginal)
          {
            List<DedupeFileContractV3> dedupeFileContractV3List;
            if (!friendlyDictionary.TryGetValue(key, out dedupeFileContractV3List))
            {
              dedupeFileContractV3List = new List<DedupeFileContractV3>();
              friendlyDictionary.Add(key, dedupeFileContractV3List);
            }
            dedupeFileContractV3List.Add(dedupeFileContractV3);
          }
        }
        if (dedupeFileContractV3.UpdateType == MetaDataStoreUpdateType.Add || dedupeFileContractV3.UpdateType == MetaDataStoreUpdateType.Edit)
          source1.Add(dedupeFileContractV3);
        else if (dedupeFileContractV3.UpdateType == MetaDataStoreUpdateType.UpdateMetaData)
          source2.Add(dedupeFileContractV3);
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
        DedupeFileContractV3 firstFile = friendlyDictionary[friendlyDictionary.Keys.First<string>()].First<DedupeFileContractV3>();
        if (executionContext.RequestContext.IsLargeRepository(repositoryName) && "Custom".Equals(str2, StringComparison.OrdinalIgnoreCase) && source3.Any<string>((Func<string, bool>) (x => firstFile.FilePath.StartsWith(x, StringComparison.Ordinal))))
        {
          FriendlyDictionary<string, IList<string>> branchNameOriginalToListOfFiles = new FriendlyDictionary<string, IList<string>>();
          HashSet<string> stringSet = new HashSet<string>();
          List<string> terms = new List<string>();
          foreach (KeyValuePair<string, List<DedupeFileContractV3>> keyValuePair in friendlyDictionary)
          {
            IList<string> list = (IList<string>) keyValuePair.Value.Select<DedupeFileContractV3, string>((Func<DedupeFileContractV3, string>) (x => x.FilePath)).ToList<string>();
            stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) list);
            terms.Add(keyValuePair.Key);
            branchNameOriginalToListOfFiles[keyValuePair.Key] = list;
          }
          BulkScriptUpdateByQueryRequest<DedupeFileContractV3> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV3>();
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
          BulkScriptUpdateByQueryRequest<DedupeFileContractV3> scriptUpdateByQueryRequest = updateByQueryRequest;
          deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV3>(executionContext, scriptUpdateByQueryRequest);
        }
        else
        {
          foreach (KeyValuePair<string, List<DedupeFileContractV3>> keyValuePair in friendlyDictionary)
          {
            firstFile = keyValuePair.Value.First<DedupeFileContractV3>();
            string branchNameOriginal = keyValuePair.Key;
            int index = firstFile.Paths.BranchNameOriginal.IndexOf(branchNameOriginal);
            string branchName = firstFile.Paths.BranchName[index];
            string repoName = firstFile.RepoName;
            if (keyValuePair.Value.Any<DedupeFileContractV3>((Func<DedupeFileContractV3, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
              throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
            BulkScriptUpdateByQueryRequest<DedupeFileContractV3> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV3>();
            updateByQueryRequest.ContractType = indexRequest.ContractType;
            updateByQueryRequest.ScriptName = "delete_dedupe_file";
            updateByQueryRequest.ShouldUpsert = false;
            updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV3>) keyValuePair.Value;
            updateByQueryRequest.IndexIdentity = indexRequest.IndexIdentity;
            updateByQueryRequest.Routing = indexRequest.Routing;
            updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
            {
              (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV3, string>((Func<DedupeFileContractV3, string>) (fc => fc.FilePath))),
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
            BulkScriptUpdateByQueryRequest<DedupeFileContractV3> scriptUpdateByQueryRequest = updateByQueryRequest;
            deleteResponse = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV3>(executionContext, scriptUpdateByQueryRequest);
          }
        }
      }
      bool forceReIndexContent = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/DedupeFileContractForceReIndexContent", true);
      BulkScriptUpdateRequest<DedupeFileContractV3> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<DedupeFileContractV3>()
      {
        IndexIdentity = indexRequest.IndexIdentity,
        Routing = indexRequest.Routing,
        ContractType = indexRequest.ContractType,
        ScriptName = "update_dedupe_file",
        GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
        {
          if (!(contract is DedupeFileContractV3 dedupeFileContractV3_2))
            throw new InvalidOperationException("Accepts only DedupeFileContract.");
          FluentDictionary<string, object> fluentDictionary = new FluentDictionary<string, object>()
          {
            {
              "newBranchName",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV3_2.Paths.BranchName)
            },
            {
              "newBranchNameOriginal",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV3_2.Paths.BranchNameOriginal)
            },
            {
              "newChangeId",
              (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV3_2.Paths.ChangeId)
            },
            {
              "isDefaultBranch",
              (object) dedupeFileContractV3_2.IsDefaultBranch
            }
          };
          if (forceReIndexContent)
            fluentDictionary.Add("content", (object) dedupeFileContractV3_2.Content);
          return fluentDictionary;
        })
      };
      IndexOperationsResponse addResponse = (IndexOperationsResponse) null;
      if (source1.Any<DedupeFileContractV3>())
      {
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV3>) source1;
        bulkScriptUpdateRequest.ShouldUpsert = true;
        addResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV3>(executionContext, bulkScriptUpdateRequest);
      }
      IndexOperationsResponse updateResponse = (IndexOperationsResponse) null;
      if (source2.Any<DedupeFileContractV3>())
      {
        if (forceReIndexContent)
          throw new SearchServiceException("IgnoreMetadataStore flag is set, we shouldn't be creating records with UpdateMetadata.");
        bulkScriptUpdateRequest.Batch = (IEnumerable<DedupeFileContractV3>) source2;
        bulkScriptUpdateRequest.ShouldUpsert = false;
        updateResponse = searchIndex.BulkScriptUpdateSync<DedupeFileContractV3>(executionContext, bulkScriptUpdateRequest);
      }
      return this.GenerateCombinedResponse(addResponse, updateResponse, deleteResponse);
    }

    public override IndexOperationsResponse DeleteDocuments<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> deleteRequest,
      ISearchIndex searchIndex)
    {
      List<DedupeFileContractV3> source1 = new List<DedupeFileContractV3>();
      Dictionary<string, List<DedupeFileContractV3>> source2 = new Dictionary<string, List<DedupeFileContractV3>>();
      foreach (T obj in deleteRequest.Batch)
      {
        if (!(obj is DedupeFileContractV3 dedupeFileContractV3))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (dedupeFileContractV3.SearchAndUpdate)
        {
          foreach (string key in dedupeFileContractV3.Paths.BranchName)
          {
            List<DedupeFileContractV3> dedupeFileContractV3List;
            if (!source2.TryGetValue(key, out dedupeFileContractV3List))
            {
              dedupeFileContractV3List = new List<DedupeFileContractV3>();
              source2.Add(key, dedupeFileContractV3List);
            }
            dedupeFileContractV3List.Add(dedupeFileContractV3);
          }
        }
        else
          source1.Add(dedupeFileContractV3);
      }
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      IndexOperationsResponse operationsResponse2 = (IndexOperationsResponse) null;
      if (source2.Any<KeyValuePair<string, List<DedupeFileContractV3>>>())
      {
        foreach (KeyValuePair<string, List<DedupeFileContractV3>> keyValuePair in source2)
        {
          DedupeFileContractV3 dedupeFileContractV3 = keyValuePair.Value.First<DedupeFileContractV3>();
          string branchName = keyValuePair.Key;
          int index = dedupeFileContractV3.Paths.BranchName.IndexOf(branchName);
          string branchNameOriginal = dedupeFileContractV3.Paths.BranchNameOriginal[index];
          string repoName = dedupeFileContractV3.RepoName;
          if (keyValuePair.Value.Any<DedupeFileContractV3>((Func<DedupeFileContractV3, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
            throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
          BulkScriptUpdateByQueryRequest<DedupeFileContractV3> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV3>();
          updateByQueryRequest.ContractType = deleteRequest.ContractType;
          updateByQueryRequest.ScriptName = "delete_dedupe_file";
          updateByQueryRequest.ShouldUpsert = false;
          updateByQueryRequest.Batch = (IEnumerable<DedupeFileContractV3>) keyValuePair.Value;
          updateByQueryRequest.IndexIdentity = deleteRequest.IndexIdentity;
          updateByQueryRequest.Routing = deleteRequest.Routing;
          updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
          {
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractV3, string>((Func<DedupeFileContractV3, string>) (fc => fc.FilePath))),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, branchName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repoName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.ProjectName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, dedupeFileContractV3.ProjectName)
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
          BulkScriptUpdateByQueryRequest<DedupeFileContractV3> scriptUpdateByQueryRequest = updateByQueryRequest;
          operationsResponse1 = searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV3>(executionContext, scriptUpdateByQueryRequest);
        }
      }
      if (source1.Any<DedupeFileContractV3>())
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
            if (!(contract is DedupeFileContractV3 dedupeFileContractV3_2))
              throw new InvalidOperationException("Accepts only DedupeFileContract.");
            return new FluentDictionary<string, object>()
            {
              {
                "oldBranchName",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV3_2.Paths.BranchName)
              },
              {
                "oldBranchNameOriginal",
                (object) string.Join(",", (IEnumerable<string>) dedupeFileContractV3_2.Paths.BranchNameOriginal)
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
      BulkScriptUpdateByQueryRequest<DedupeFileContractV3> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<DedupeFileContractV3>();
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
      BulkScriptUpdateByQueryRequest<DedupeFileContractV3> scriptUpdateByQueryRequest = updateByQueryRequest;
      return searchIndex.BulkScriptUpdateByQuery<DedupeFileContractV3>(executionContext, scriptUpdateByQueryRequest);
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
      this.FileExtensionId = new float?((float) RelevanceUtility.GetFileExtensionId(this.FileExtension));
      IMetaDataStoreItem metaDataStoreItem = (IMetaDataStoreItem) data;
      this.FilePath = metaDataStoreItem.Path.NormalizePath();
      this.FilePathOriginal = metaDataStoreItem.Path;
      this.Item = this.FilePathOriginal;
    }

    public override string GetHighlighter(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      string inputHighlighter = requestContext.IsCodesearchV4HighlighterFeatureEnabled() ? "codesearch_v4" : "codesearch_v3";
      return this.GetMappedHighlighter(requestContext, inputHighlighter);
    }

    internal override string CreateTermQueryStringForDefaultType(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      if (!termExpression.Value.StartsWith("*", StringComparison.Ordinal) && !termExpression.Value.StartsWith("?", StringComparison.Ordinal))
        return this.CreateTermQueryString(requestContext, termExpression, enableRanking, requestId);
      string str = ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(termExpression.Value);
      string rewriteMethod = CodeFileContract.GetRewriteMethod(requestContext, this.ContractType);
      char[] charArray = str.ToCharArray();
      Array.Reverse((Array) charArray);
      return ElasticsearchQueryBuilder.BuildWildcardQuery(this.fields[CodeFileContract.CodeContractQueryableElement.Prefix].ElasticsearchFieldName, new string(charArray), rewriteMethod);
    }

    public override int GetDocumentContractSize()
    {
      int documentContractSize = base.GetDocumentContractSize();
      if (this.OriginalContent != null)
        documentContractSize += Encoding.UTF8.GetByteCount(this.OriginalContent);
      return documentContractSize;
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

    protected internal override ITypeMapping GetMapping(
      IVssRequestContext requestContext,
      int indexVersion)
    {
      TypeMapping mapping = new TypeMapping();
      mapping.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
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
      PropertyName name9 = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtensionId.ElasticsearchFieldName();
      NumberProperty numberProperty = new NumberProperty(NumberType.Float);
      numberProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.FileExtensionId.ElasticsearchFieldName();
      numberProperty.Store = new bool?(true);
      properties1.Add(name9, (IProperty) numberProperty);
      PropertyName name10 = (PropertyName) CodeContractField.CodeSearchFieldDesc.Content.ElasticsearchFieldName();
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.Content.ElasticsearchFieldName();
      textProperty2.Analyzer = "codesearch";
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.Norms = new bool?(false);
      properties1.Add(name10, (IProperty) textProperty2);
      PropertyName name11 = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      TextProperty textProperty3 = new TextProperty();
      textProperty3.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.OriginalContent.ElasticsearchFieldName();
      textProperty3.Analyzer = "contentanalyzer";
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.Norms = new bool?(false);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "reverse"] = (IProperty) new TextProperty()
      {
        Analyzer = "reversetextanalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      };
      textProperty3.Fields = (IProperties) properties3;
      properties1.Add(name11, (IProperty) textProperty3);
      PropertyName name12 = (PropertyName) "organizationId";
      KeywordProperty keywordProperty9 = new KeywordProperty();
      keywordProperty9.Name = (PropertyName) "organizationId";
      keywordProperty9.Store = new bool?(true);
      properties1.Add(name12, (IProperty) keywordProperty9);
      PropertyName name13 = (PropertyName) "organizationNameOriginal";
      KeywordProperty keywordProperty10 = new KeywordProperty();
      keywordProperty10.Name = (PropertyName) "organizationNameOriginal";
      keywordProperty10.Store = new bool?(true);
      properties1.Add(name13, (IProperty) keywordProperty10);
      PropertyName name14 = (PropertyName) "organizationName";
      KeywordProperty keywordProperty11 = new KeywordProperty();
      keywordProperty11.Name = (PropertyName) "organizationName";
      properties1.Add(name14, (IProperty) keywordProperty11);
      PropertyName name15 = (PropertyName) CodeContractField.CodeSearchFieldDesc.Paths.ElasticsearchFieldName();
      ObjectProperty objectProperty = new ObjectProperty();
      objectProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.Paths.ElasticsearchFieldName();
      Properties properties4 = new Properties();
      PropertyName name16 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      KeywordProperty keywordProperty12 = new KeywordProperty();
      keywordProperty12.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ChangeId.ElasticsearchFieldName();
      keywordProperty12.Store = new bool?(true);
      properties4.Add(name16, (IProperty) keywordProperty12);
      PropertyName name17 = (PropertyName) "branchNameOriginal";
      KeywordProperty keywordProperty13 = new KeywordProperty();
      keywordProperty13.Name = (PropertyName) "branchNameOriginal";
      keywordProperty13.Store = new bool?(true);
      properties4.Add(name17, (IProperty) keywordProperty13);
      PropertyName name18 = (PropertyName) "branchName";
      KeywordProperty keywordProperty14 = new KeywordProperty();
      keywordProperty14.Name = (PropertyName) "branchName";
      properties4.Add(name18, (IProperty) keywordProperty14);
      objectProperty.Properties = (IProperties) properties4;
      properties1.Add(name15, (IProperty) objectProperty);
      PropertyName name19 = (PropertyName) CodeContractField.CodeSearchFieldDesc.LastChangeUtcTime.ElasticsearchFieldName();
      DateProperty dateProperty1 = new DateProperty();
      dateProperty1.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.LastChangeUtcTime.ElasticsearchFieldName();
      dateProperty1.Store = new bool?(true);
      properties1.Add(name19, (IProperty) dateProperty1);
      PropertyName name20 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      KeywordProperty keywordProperty15 = new KeywordProperty();
      keywordProperty15.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ContentId.ElasticsearchFieldName();
      keywordProperty15.Store = new bool?(true);
      properties1.Add(name20, (IProperty) keywordProperty15);
      PropertyName name21 = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      KeywordProperty keywordProperty16 = new KeywordProperty();
      keywordProperty16.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.VersionControlType.ElasticsearchFieldName();
      keywordProperty16.Store = new bool?(true);
      properties1.Add(name21, (IProperty) keywordProperty16);
      PropertyName name22 = (PropertyName) "collectionNameOriginal";
      KeywordProperty keywordProperty17 = new KeywordProperty();
      keywordProperty17.Name = (PropertyName) "collectionNameOriginal";
      keywordProperty17.Store = new bool?(true);
      properties1.Add(name22, (IProperty) keywordProperty17);
      PropertyName name23 = (PropertyName) "collectionName";
      KeywordProperty keywordProperty18 = new KeywordProperty();
      keywordProperty18.Name = (PropertyName) "collectionName";
      properties1.Add(name23, (IProperty) keywordProperty18);
      PropertyName name24 = (PropertyName) "collectionId";
      KeywordProperty keywordProperty19 = new KeywordProperty();
      keywordProperty19.Name = (PropertyName) "collectionId";
      keywordProperty19.Store = new bool?(true);
      properties1.Add(name24, (IProperty) keywordProperty19);
      PropertyName name25 = (PropertyName) "indexedTimeStamp";
      DateProperty dateProperty2 = new DateProperty();
      dateProperty2.Name = (PropertyName) "indexedTimeStamp";
      dateProperty2.Format = "epoch_second";
      dateProperty2.Store = new bool?(true);
      properties1.Add(name25, (IProperty) dateProperty2);
      PropertyName name26 = (PropertyName) "projectNameOriginal";
      KeywordProperty keywordProperty20 = new KeywordProperty();
      keywordProperty20.Name = (PropertyName) "projectNameOriginal";
      keywordProperty20.Store = new bool?(true);
      properties1.Add(name26, (IProperty) keywordProperty20);
      PropertyName name27 = (PropertyName) "projectName";
      KeywordProperty keywordProperty21 = new KeywordProperty();
      keywordProperty21.Name = (PropertyName) "projectName";
      properties1.Add(name27, (IProperty) keywordProperty21);
      PropertyName name28 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      KeywordProperty keywordProperty22 = new KeywordProperty();
      keywordProperty22.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName();
      keywordProperty22.Store = new bool?(true);
      properties1.Add(name28, (IProperty) keywordProperty22);
      PropertyName name29 = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      KeywordProperty keywordProperty23 = new KeywordProperty();
      keywordProperty23.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.ProjectInfo.ElasticsearchFieldName();
      keywordProperty23.Store = new bool?(true);
      properties1.Add(name29, (IProperty) keywordProperty23);
      PropertyName name30 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoNameOriginal.ElasticsearchFieldName();
      KeywordProperty keywordProperty24 = new KeywordProperty();
      keywordProperty24.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoNameOriginal.ElasticsearchFieldName();
      keywordProperty24.Store = new bool?(true);
      properties1.Add(name30, (IProperty) keywordProperty24);
      PropertyName name31 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      KeywordProperty keywordProperty25 = new KeywordProperty();
      keywordProperty25.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName();
      properties1.Add(name31, (IProperty) keywordProperty25);
      PropertyName name32 = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      KeywordProperty keywordProperty26 = new KeywordProperty();
      keywordProperty26.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.RepositoryId.ElasticsearchFieldName();
      keywordProperty26.Store = new bool?(true);
      properties1.Add(name32, (IProperty) keywordProperty26);
      PropertyName name33 = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      BooleanProperty booleanProperty = new BooleanProperty();
      booleanProperty.Name = (PropertyName) CodeContractField.CodeSearchFieldDesc.IsDefaultBranch.ElasticsearchFieldName();
      properties1.Add(name33, (IProperty) booleanProperty);
      mapping.Properties = (IProperties) properties1;
      return (ITypeMapping) mapping;
    }

    public override IEnumerable<CodeFileContract> BulkGetByQuery(
      ExecutionContext executionContext,
      ISearchIndex searchIndex,
      BulkGetByQueryRequest request)
    {
      return (IEnumerable<CodeFileContract>) searchIndex.BulkGetByQuery<DedupeFileContractV3>(executionContext, request);
    }

    protected override string GetNormalizedFolderPath(string filepath) => filepath.NormalizePath() + "\\";

    protected override string GetRepoNameOriginal() => this.RepoNameOriginal;

    protected override bool ShouldGetFieldNameWithHighlightHit(IVssRequestContext requestContext) => requestContext.IsCodesearchV4HighlighterFeatureEnabled();

    private bool ShouldParseFieldNameAsEnum(IVssRequestContext requestContext) => requestContext.IsCodesearchV4HighlighterFeatureEnabled();
  }
}
