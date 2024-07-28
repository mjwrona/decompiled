// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.DedupeFileContractBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public abstract class DedupeFileContractBase : CodeFileContract
  {
    public FileGroup Paths { get; set; }

    public bool? IsDefaultBranch { get; set; }

    public string OriginalContent { get; set; }

    [Keyword(Ignore = true)]
    public MetaDataStoreUpdateType UpdateType { get; set; }

    [Boolean(Ignore = true)]
    public bool SearchAndUpdate { get; set; }

    protected DedupeFileContractBase()
    {
      this.Paths = new FileGroup();
      this.Initialize();
    }

    protected DedupeFileContractBase(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
      this.Paths = new FileGroup();
      this.Initialize();
    }

    private void Initialize()
    {
      this.fields[CodeFileContract.CodeContractQueryableElement.BranchName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.PathsBranchName);
      this.fields[CodeFileContract.CodeContractQueryableElement.ChangeId] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.PathsChangeId);
      this.fields[CodeFileContract.CodeContractQueryableElement.Prefix] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.OriginalContentReverse);
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
      this.CollectionName = metaDataStore["NormalizedCollectionName"];
      this.CollectionNameOriginal = metaDataStore["CollectionName"];
      if (originalContent != null)
        this.OriginalContent = this.GetOriginalContent(originalContent);
      IMetaDataStoreItem metaDataStoreItem = data as IMetaDataStoreItem;
      this.UpdateType = metaDataStoreItem.UpdateType;
      this.SearchAndUpdate = true;
      bool currentHostConfigValue = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/LargeRepoUseSearchAndUpdate", true);
      if (requestContext.IsLargeRepository(this.RepoNameOriginal) && string.Equals(metaDataStore["VcType"], "Git", StringComparison.OrdinalIgnoreCase))
        this.SearchAndUpdate = currentHostConfigValue && (this.UpdateType == MetaDataStoreUpdateType.Add || this.UpdateType == MetaDataStoreUpdateType.UpdateMetaData);
      if (metaDataStoreItem.BranchesInfo != null && metaDataStoreItem.BranchesInfo.Count > 0)
      {
        bool flag = false;
        foreach (BranchInfo branchInfo in metaDataStoreItem.BranchesInfo)
        {
          string branchName = branchInfo.BranchName;
          string nameWithoutPrefix1 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/".NormalizePathWithoutTrimming(), branchName.NormalizePath());
          string nameWithoutPrefix2 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/", branchName);
          if (!flag && branchName.Equals(metaDataStore["DefaultBranchName"], StringComparison.Ordinal))
          {
            flag = true;
            this.Paths.BranchName.Insert(0, nameWithoutPrefix1);
            this.Paths.BranchNameOriginal.Insert(0, nameWithoutPrefix2);
            this.Paths.ChangeId.Insert(0, branchInfo.ChangeId.NormalizeString());
          }
          else
          {
            this.Paths.BranchName.Add(nameWithoutPrefix1);
            this.Paths.BranchNameOriginal.Add(nameWithoutPrefix2);
            this.Paths.ChangeId.Add(branchInfo.ChangeId.NormalizeString());
          }
        }
        this.IsDefaultBranch = new bool?(flag);
      }
      else
      {
        string nameWithoutPrefix3 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/".NormalizePathWithoutTrimming(), metaDataStore["NormalizedBranchName"]);
        string nameWithoutPrefix4 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/", metaDataStore["BranchName"]);
        this.Paths.BranchName.Add(nameWithoutPrefix3);
        this.Paths.BranchNameOriginal.Add(nameWithoutPrefix4);
        this.Paths.ChangeId.Add(metaDataStore["NormalizedLatestCommitIdKey"]);
        bool result;
        if (!bool.TryParse(metaDataStore["IsDefaultBranch"], out result))
          result = true;
        this.IsDefaultBranch = new bool?(result);
      }
    }

    internal override string ConvertToAdvancedPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      return ElasticsearchQueryBuilder.BuildMatchPhraseQuery(this.GetSearchFieldForAdvancedQuery(requestContext).ElasticsearchFieldName, termExpression.Value);
    }

    protected IndexOperationsResponse GenerateCombinedResponse(
      IndexOperationsResponse addResponse,
      IndexOperationsResponse updateResponse,
      IndexOperationsResponse deleteResponse)
    {
      IndexOperationsResponse combinedResponse;
      if (addResponse == null && updateResponse == null && deleteResponse == null)
        combinedResponse = new IndexOperationsResponse()
        {
          Success = false
        };
      else if (addResponse == null && updateResponse == null && deleteResponse != null)
        combinedResponse = deleteResponse;
      else if (addResponse == null && updateResponse != null && deleteResponse == null)
        combinedResponse = updateResponse;
      else if (addResponse != null && updateResponse == null && deleteResponse == null)
      {
        combinedResponse = addResponse;
      }
      else
      {
        bool flag = true;
        long num1 = 0;
        long num2 = 0;
        List<FailedItem> failedItemList = new List<FailedItem>();
        if (addResponse != null)
        {
          flag &= addResponse.Success;
          num1 += addResponse.ItemsCount;
          num2 += addResponse.FailedItemsCount;
          if (addResponse.FailedItems != null)
            failedItemList.AddRange(addResponse.FailedItems);
        }
        if (updateResponse != null)
        {
          flag &= updateResponse.Success;
          num1 += updateResponse.ItemsCount;
          num2 += updateResponse.FailedItemsCount;
          if (updateResponse.FailedItems != null)
            failedItemList.AddRange(updateResponse.FailedItems);
        }
        if (deleteResponse != null)
        {
          flag &= deleteResponse.Success;
          num2 += deleteResponse.FailedItemsCount;
          if (deleteResponse.FailedItems != null)
            failedItemList.AddRange(deleteResponse.FailedItems);
        }
        combinedResponse = new IndexOperationsResponse()
        {
          FailedItemsCount = num2,
          ItemsCount = num1,
          FailedItems = (IEnumerable<FailedItem>) failedItemList,
          Success = flag
        };
      }
      return combinedResponse;
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
      BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract>();
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
      BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> scriptUpdateByQueryRequest = updateByQueryRequest;
      return searchIndex.BulkScriptUpdateByQuery<AbstractSearchDocumentContract>(executionContext, scriptUpdateByQueryRequest);
    }

    public override IndexOperationsResponse InsertBatch<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> indexRequest,
      ISearchIndex searchIndex)
    {
      string str1 = (string) null;
      string repositoryName = (string) null;
      string str2 = (string) null;
      List<DedupeFileContractBase> source1 = new List<DedupeFileContractBase>();
      List<DedupeFileContractBase> source2 = new List<DedupeFileContractBase>();
      FriendlyDictionary<string, List<DedupeFileContractBase>> friendlyDictionary = new FriendlyDictionary<string, List<DedupeFileContractBase>>();
      foreach (T obj in indexRequest.Batch)
      {
        if (!(obj is DedupeFileContractBase fileContractBase))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (str1 == null)
        {
          str1 = fileContractBase.RepositoryId.ToLowerInvariant();
          repositoryName = fileContractBase.GetRepoNameOriginal();
          str2 = fileContractBase.VcType;
        }
        if (fileContractBase.SearchAndUpdate)
        {
          foreach (string key in fileContractBase.Paths.BranchNameOriginal)
          {
            List<DedupeFileContractBase> fileContractBaseList;
            if (!friendlyDictionary.TryGetValue(key, out fileContractBaseList))
            {
              fileContractBaseList = new List<DedupeFileContractBase>();
              friendlyDictionary.Add(key, fileContractBaseList);
            }
            fileContractBaseList.Add(fileContractBase);
          }
        }
        if (fileContractBase.UpdateType == MetaDataStoreUpdateType.Add || fileContractBase.UpdateType == MetaDataStoreUpdateType.Edit)
          source1.Add(fileContractBase);
        else if (fileContractBase.UpdateType == MetaDataStoreUpdateType.UpdateMetaData)
          source2.Add(fileContractBase);
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
        DedupeFileContractBase firstFile = friendlyDictionary[friendlyDictionary.Keys.First<string>()].First<DedupeFileContractBase>();
        if (executionContext.RequestContext.IsLargeRepository(repositoryName) && "Custom".Equals(str2, StringComparison.OrdinalIgnoreCase) && source3.Any<string>((Func<string, bool>) (x => firstFile.FilePath.StartsWith(x, StringComparison.Ordinal))))
        {
          FriendlyDictionary<string, IList<string>> branchNameOriginalToListOfFiles = new FriendlyDictionary<string, IList<string>>();
          HashSet<string> stringSet = new HashSet<string>();
          List<string> terms = new List<string>();
          foreach (KeyValuePair<string, List<DedupeFileContractBase>> keyValuePair in friendlyDictionary)
          {
            IList<string> list = (IList<string>) keyValuePair.Value.Select<DedupeFileContractBase, string>((Func<DedupeFileContractBase, string>) (x => x.FilePath)).ToList<string>();
            stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) list);
            terms.Add(keyValuePair.Key);
            branchNameOriginalToListOfFiles[keyValuePair.Key] = list;
          }
          BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract>();
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
          BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> scriptUpdateByQueryRequest = updateByQueryRequest;
          deleteResponse = searchIndex.BulkScriptUpdateByQuery<AbstractSearchDocumentContract>(executionContext, scriptUpdateByQueryRequest);
        }
        else
        {
          foreach (KeyValuePair<string, List<DedupeFileContractBase>> keyValuePair in friendlyDictionary)
          {
            firstFile = keyValuePair.Value.First<DedupeFileContractBase>();
            string branchNameOriginal = keyValuePair.Key;
            int index = firstFile.Paths.BranchNameOriginal.IndexOf(branchNameOriginal);
            string branchName = firstFile.Paths.BranchName[index];
            string repoName = firstFile.RepoName;
            if (keyValuePair.Value.Any<DedupeFileContractBase>((Func<DedupeFileContractBase, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
              throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
            BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract>();
            updateByQueryRequest.ContractType = indexRequest.ContractType;
            updateByQueryRequest.ScriptName = "delete_dedupe_file";
            updateByQueryRequest.ShouldUpsert = false;
            updateByQueryRequest.Batch = (IEnumerable<AbstractSearchDocumentContract>) keyValuePair.Value;
            updateByQueryRequest.IndexIdentity = indexRequest.IndexIdentity;
            updateByQueryRequest.Routing = indexRequest.Routing;
            updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
            {
              (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractBase, string>((Func<DedupeFileContractBase, string>) (fc => fc.FilePath))),
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
            BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> scriptUpdateByQueryRequest = updateByQueryRequest;
            deleteResponse = searchIndex.BulkScriptUpdateByQuery<AbstractSearchDocumentContract>(executionContext, scriptUpdateByQueryRequest);
          }
        }
      }
      bool forceReIndexContent = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/DedupeFileContractForceReIndexContent", true);
      BulkScriptUpdateRequest<AbstractSearchDocumentContract> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<AbstractSearchDocumentContract>()
      {
        IndexIdentity = indexRequest.IndexIdentity,
        Routing = indexRequest.Routing,
        ContractType = indexRequest.ContractType,
        ScriptName = "update_dedupe_file",
        GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
        {
          if (!(contract is DedupeFileContractBase fileContractBase2))
            throw new InvalidOperationException("Accepts only DedupeFileContract.");
          FluentDictionary<string, object> fluentDictionary = new FluentDictionary<string, object>()
          {
            {
              "newBranchName",
              (object) string.Join(",", (IEnumerable<string>) fileContractBase2.Paths.BranchName)
            },
            {
              "newBranchNameOriginal",
              (object) string.Join(",", (IEnumerable<string>) fileContractBase2.Paths.BranchNameOriginal)
            },
            {
              "newChangeId",
              (object) string.Join(",", (IEnumerable<string>) fileContractBase2.Paths.ChangeId)
            },
            {
              "isDefaultBranch",
              (object) fileContractBase2.IsDefaultBranch
            }
          };
          if (forceReIndexContent)
            fluentDictionary.Add("content", (object) fileContractBase2.Content);
          return fluentDictionary;
        })
      };
      IndexOperationsResponse addResponse = (IndexOperationsResponse) null;
      if (source1.Any<DedupeFileContractBase>())
      {
        bulkScriptUpdateRequest.Batch = (IEnumerable<AbstractSearchDocumentContract>) source1;
        bulkScriptUpdateRequest.ShouldUpsert = true;
        addResponse = searchIndex.BulkScriptUpdateSync<AbstractSearchDocumentContract>(executionContext, bulkScriptUpdateRequest);
      }
      IndexOperationsResponse updateResponse = (IndexOperationsResponse) null;
      if (source2.Any<DedupeFileContractBase>())
      {
        if (forceReIndexContent)
          throw new SearchServiceException("IgnoreMetadataStore flag is set, we shouldn't be creating records with UpdateMetadata.");
        bulkScriptUpdateRequest.Batch = (IEnumerable<AbstractSearchDocumentContract>) source2;
        bulkScriptUpdateRequest.ShouldUpsert = false;
        updateResponse = searchIndex.BulkScriptUpdateSync<AbstractSearchDocumentContract>(executionContext, bulkScriptUpdateRequest);
      }
      return this.GenerateCombinedResponse(addResponse, updateResponse, deleteResponse);
    }

    public override IndexOperationsResponse DeleteDocuments<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> deleteRequest,
      ISearchIndex searchIndex)
    {
      List<DedupeFileContractBase> source1 = new List<DedupeFileContractBase>();
      Dictionary<string, List<DedupeFileContractBase>> source2 = new Dictionary<string, List<DedupeFileContractBase>>();
      foreach (T obj in deleteRequest.Batch)
      {
        if (!(obj is DedupeFileContractBase fileContractBase))
          throw new ArgumentException("Only dedupe file contract batch is supported.");
        if (fileContractBase.SearchAndUpdate)
        {
          foreach (string key in fileContractBase.Paths.BranchName)
          {
            List<DedupeFileContractBase> fileContractBaseList;
            if (!source2.TryGetValue(key, out fileContractBaseList))
            {
              fileContractBaseList = new List<DedupeFileContractBase>();
              source2.Add(key, fileContractBaseList);
            }
            fileContractBaseList.Add(fileContractBase);
          }
        }
        else
          source1.Add(fileContractBase);
      }
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      IndexOperationsResponse operationsResponse2 = (IndexOperationsResponse) null;
      if (source2.Any<KeyValuePair<string, List<DedupeFileContractBase>>>())
      {
        foreach (KeyValuePair<string, List<DedupeFileContractBase>> keyValuePair in source2)
        {
          DedupeFileContractBase fileContractBase = keyValuePair.Value.First<DedupeFileContractBase>();
          string branchName = keyValuePair.Key;
          int index = fileContractBase.Paths.BranchName.IndexOf(branchName);
          string branchNameOriginal = fileContractBase.Paths.BranchNameOriginal[index];
          string repoName = fileContractBase.RepoName;
          if (keyValuePair.Value.Any<DedupeFileContractBase>((Func<DedupeFileContractBase, bool>) (file => !file.RepoName.Equals(repoName, StringComparison.Ordinal))))
            throw new ArgumentException("All the files in the request should belong to same project, repo and branch");
          BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> updateByQueryRequest = new BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract>();
          updateByQueryRequest.ContractType = deleteRequest.ContractType;
          updateByQueryRequest.ScriptName = "delete_dedupe_file";
          updateByQueryRequest.ShouldUpsert = false;
          updateByQueryRequest.Batch = (IEnumerable<AbstractSearchDocumentContract>) keyValuePair.Value;
          updateByQueryRequest.IndexIdentity = deleteRequest.IndexIdentity;
          updateByQueryRequest.Routing = deleteRequest.Routing;
          updateByQueryRequest.Query = (IExpression) new AndExpression(new IExpression[4]
          {
            (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, keyValuePair.Value.Select<DedupeFileContractBase, string>((Func<DedupeFileContractBase, string>) (fc => fc.FilePath))),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.PathsBranchName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, branchName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, repoName),
            (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.ProjectName.ElasticsearchFieldName(), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, fileContractBase.ProjectName)
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
          BulkScriptUpdateByQueryRequest<AbstractSearchDocumentContract> scriptUpdateByQueryRequest = updateByQueryRequest;
          operationsResponse1 = searchIndex.BulkScriptUpdateByQuery<AbstractSearchDocumentContract>(executionContext, scriptUpdateByQueryRequest);
        }
      }
      if (source1.Any<DedupeFileContractBase>())
      {
        BulkScriptUpdateRequest<AbstractSearchDocumentContract> bulkScriptUpdateRequest = new BulkScriptUpdateRequest<AbstractSearchDocumentContract>()
        {
          Batch = (IEnumerable<AbstractSearchDocumentContract>) deleteRequest.Batch,
          IndexIdentity = deleteRequest.IndexIdentity,
          Routing = deleteRequest.Routing,
          ContractType = deleteRequest.ContractType,
          ScriptName = "delete_dedupe_file",
          ShouldUpsert = false,
          GetParams = (Func<AbstractSearchDocumentContract, FluentDictionary<string, object>>) (contract =>
          {
            if (!(contract is DedupeFileContractBase fileContractBase2))
              throw new InvalidOperationException("Accepts only DedupeFileContract.");
            return new FluentDictionary<string, object>()
            {
              {
                "oldBranchName",
                (object) string.Join(",", (IEnumerable<string>) fileContractBase2.Paths.BranchName)
              },
              {
                "oldBranchNameOriginal",
                (object) string.Join(",", (IEnumerable<string>) fileContractBase2.Paths.BranchNameOriginal)
              }
            };
          })
        };
        operationsResponse2 = searchIndex.BulkScriptUpdateSync<AbstractSearchDocumentContract>(executionContext, bulkScriptUpdateRequest);
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

    protected virtual string GetOriginalContent(byte[] originalContent) => new TextEncoding().GetString(originalContent).ToLowerInvariant();

    protected abstract string GetNormalizedFolderPath(string filepath);

    protected abstract string GetRepoNameOriginal();
  }
}
