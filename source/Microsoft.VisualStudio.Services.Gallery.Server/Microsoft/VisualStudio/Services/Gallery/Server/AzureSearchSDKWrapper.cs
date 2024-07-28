// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchSDKWrapper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class AzureSearchSDKWrapper
  {
    private static readonly string s_area = "Gallery";
    private static readonly string s_layer = nameof (AzureSearchSDKWrapper);
    private string searchServiceName;
    private string indexName;
    private SearchServiceClient serviceAdminClient;
    private ISearchIndexClient indexClient;
    private SearchServiceClient serviceQueryClient;
    private ISearchIndexClient queryClient;

    internal AzureSearchSDKWrapper()
    {
    }

    public AzureSearchSDKWrapper(
      string searchServiceName,
      string indexName,
      string key,
      AzureClientMode mode)
    {
      this.searchServiceName = searchServiceName;
      this.indexName = indexName;
      if (mode != AzureClientMode.Admin)
      {
        if (mode != AzureClientMode.Query)
          return;
        this.serviceQueryClient = new SearchServiceClient(searchServiceName, new SearchCredentials(key));
        this.queryClient = IndexesGetClientExtensions.GetClient(this.serviceQueryClient.Indexes, indexName);
      }
      else
      {
        this.serviceAdminClient = new SearchServiceClient(searchServiceName, new SearchCredentials(key));
        this.indexClient = IndexesGetClientExtensions.GetClient(this.serviceAdminClient.Indexes, indexName);
      }
    }

    public void CreateIndex(Index indexDefinition)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating index on Search Service : {0}, IndexName: {1}", (object) this.searchServiceName, (object) indexDefinition.Name);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (CreateIndex), message);
      IndexesOperationsExtensions.Create(this.serviceAdminClient.Indexes, indexDefinition, (SearchRequestOptions) null);
    }

    public virtual void CreateOrUpdateIndex(Index indexDefinition)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating/Updating index on Search Service : {0}, IndexName: {1}", (object) this.searchServiceName, (object) indexDefinition.Name);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (CreateOrUpdateIndex), message);
      IndexesOperationsExtensions.CreateOrUpdate(this.serviceAdminClient.Indexes, indexDefinition, new bool?(), (SearchRequestOptions) null, (AccessCondition) null);
    }

    public virtual Index GetIndexDefinition()
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Getting index on Search Service : {0} for the Index: {1}", (object) this.searchServiceName, (object) this.indexName);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, "GetIndex", message);
      return IndexesOperationsExtensions.Get(this.serviceAdminClient.Indexes, this.indexName, (SearchRequestOptions) null);
    }

    public virtual void UploadSynonymMap(string synonymMapName, string synonymMapValue = "")
    {
      if (string.IsNullOrWhiteSpace(synonymMapName) || string.IsNullOrWhiteSpace(synonymMapValue))
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating synonym maps on Search Service : {0}, SynonymMapName: {1}", (object) this.searchServiceName, (object) synonymMapName);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (UploadSynonymMap), message);
      synonymMapValue = synonymMapValue.Replace("\\n", "\n");
      SynonymMap synonymMap = new SynonymMap()
      {
        Name = synonymMapName,
        Format = SynonymMapFormat.op_Implicit("solr"),
        Synonyms = synonymMapValue
      };
      try
      {
        SynonymMapsOperationsExtensions.CreateOrUpdate(this.serviceAdminClient.SynonymMaps, synonymMap, (SearchRequestOptions) null, (AccessCondition) null);
      }
      catch (CloudException ex)
      {
        TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (UploadSynonymMap), ex.Message);
      }
    }

    public virtual void RemoveSynonymMap(string synonymMapName)
    {
      if (!SynonymMapsOperationsExtensions.Exists(this.serviceAdminClient.SynonymMaps, synonymMapName, (SearchRequestOptions) null))
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Remove synonym maps on Search Service : {0}, SynonymMapName: {1}", (object) this.searchServiceName, (object) synonymMapName);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (RemoveSynonymMap), message);
      SynonymMapsOperationsExtensions.Delete(this.serviceAdminClient.SynonymMaps, synonymMapName, (SearchRequestOptions) null, (AccessCondition) null);
    }

    public virtual SynonymMap GetSynonymMap(string synonymMapName) => SynonymMapsOperationsExtensions.Get(this.serviceAdminClient.SynonymMaps, synonymMapName, (SearchRequestOptions) null);

    public void DeleteIndex()
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting index on Search Service : {0}, IndexName: {1}", (object) this.searchServiceName, (object) this.indexName);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (DeleteIndex), message);
      if (!IndexesOperationsExtensions.Exists(this.serviceAdminClient.Indexes, this.indexName, (SearchRequestOptions) null))
        return;
      IndexesOperationsExtensions.Delete(this.serviceAdminClient.Indexes, this.indexName, (SearchRequestOptions) null, (AccessCondition) null);
    }

    public void PopulateIndex(List<AzureIndexDocument> indexDocumentList)
    {
      try
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Indexing {0} extensions on Search Service : {1}", (object) indexDocumentList.Count, (object) this.searchServiceName);
        TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (PopulateIndex), message);
        this.PerformIndexOperation(indexDocumentList, AzureIndexAction.Upload, 0);
      }
      catch (IndexBatchException ex)
      {
        List<string> list = ex.IndexingResults.Where<IndexingResult>((Func<IndexingResult, bool>) (r => !r.Succeeded)).Select<IndexingResult, string>((Func<IndexingResult, string>) (r => r.Key)).ToList<string>();
        string str = string.Join(",", (IEnumerable<string>) list);
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Populate Index Failed for {0} extensions, Failed extensions Ids: {1} on Search Service : {2}", (object) list.Count, (object) str, (object) this.searchServiceName);
        TeamFoundationTracingService.TraceRaw(12060102, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (PopulateIndex), message);
        throw;
      }
    }

    public void DeleteEntries(List<AzureIndexDocument> indexDocumentList)
    {
      try
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting index data of {0} extensions from Search Service : {1}", (object) indexDocumentList.Count, (object) this.searchServiceName);
        TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (DeleteEntries), message);
        this.PerformIndexOperation(indexDocumentList, AzureIndexAction.Delete, 0);
      }
      catch (IndexBatchException ex)
      {
        List<string> list = ex.IndexingResults.Where<IndexingResult>((Func<IndexingResult, bool>) (r => !r.Succeeded)).Select<IndexingResult, string>((Func<IndexingResult, string>) (r => r.Key)).ToList<string>();
        string str = string.Join(",", (IEnumerable<string>) list);
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting index data Failed for {0} extensions, Failed extensions Ids: {1} on Search Service : {2}", (object) list.Count, (object) str, (object) this.searchServiceName);
        TeamFoundationTracingService.TraceRaw(12060102, TraceLevel.Error, AzureSearchSDKWrapper.s_area, nameof (DeleteEntries), message);
        throw;
      }
    }

    public virtual DocumentSearchResult<AzureIndexDocument> Search(
      string searchText,
      SearchParameters searchParams)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Searching extensions on Search Service : {0}, SearchText: {1}", (object) this.searchServiceName, (object) searchText);
      TeamFoundationTracingService.TraceRaw(12060110, TraceLevel.Info, AzureSearchSDKWrapper.s_area, nameof (Search), message);
      return DocumentsOperationsExtensions.Search<AzureIndexDocument>(this.queryClient.Documents, searchText, searchParams, (SearchRequestOptions) null);
    }

    private void PerformIndexOperation(
      List<AzureIndexDocument> indexDocumentList,
      AzureIndexAction action,
      int retryCount)
    {
      List<IndexAction<AzureIndexDocument>> indexActionList = new List<IndexAction<AzureIndexDocument>>();
      if (action == AzureIndexAction.Upload)
      {
        foreach (AzureIndexDocument indexDocument in indexDocumentList)
          indexActionList.Add(IndexAction.Upload<AzureIndexDocument>(indexDocument));
      }
      else
      {
        foreach (AzureIndexDocument indexDocument in indexDocumentList)
          indexActionList.Add(IndexAction.Delete<AzureIndexDocument>(indexDocument));
      }
      IndexBatch<AzureIndexDocument> indexBatch = IndexBatch.New<AzureIndexDocument>((IEnumerable<IndexAction<AzureIndexDocument>>) indexActionList);
      try
      {
        DocumentsOperationsExtensions.Index<AzureIndexDocument>(this.indexClient.Documents, indexBatch, (SearchRequestOptions) null);
      }
      catch (IndexBatchException ex)
      {
        if (retryCount < AzureSearchConstants.RequestRetryCount)
        {
          List<AzureIndexDocument> indexDocumentList1 = new List<AzureIndexDocument>();
          string str = string.Empty;
          foreach (IndexingResult indexingResult in (IEnumerable<IndexingResult>) ex.IndexingResults)
          {
            str = str + "," + indexingResult.Key;
            if (!indexingResult.Succeeded)
            {
              using (List<AzureIndexDocument>.Enumerator enumerator = indexDocumentList.GetEnumerator())
              {
                if (enumerator.MoveNext())
                {
                  AzureIndexDocument current = enumerator.Current;
                  indexDocumentList1.Add(current);
                }
              }
            }
          }
          string message = string.Format(" Index operation {0} failed for the extensions : {1}, Retry count : {2} on Search Service : {3}", (object) action.ToString(), (object) str, (object) retryCount, (object) this.searchServiceName);
          TeamFoundationTracingService.TraceRaw(12060102, TraceLevel.Warning, AzureSearchSDKWrapper.s_area, AzureSearchSDKWrapper.s_layer, message);
          this.PerformIndexOperation(indexDocumentList1, action, ++retryCount);
        }
        else
          throw;
      }
      catch (Exception ex)
      {
        if (retryCount < AzureSearchConstants.RequestRetryCount)
        {
          string message = string.Format(" Index operation {0} failed on Search Service : {1}, Retry count : {2}, Exception Type: {3}, Exception Message : {4}, Stack Trace : {5}", (object) action.ToString(), (object) this.searchServiceName, (object) retryCount, (object) ex.GetType().ToString(), (object) ex.Message, (object) ex.StackTrace);
          TeamFoundationTracingService.TraceRaw(12060102, TraceLevel.Warning, AzureSearchSDKWrapper.s_area, AzureSearchSDKWrapper.s_layer, message);
          this.PerformIndexOperation(indexDocumentList, action, ++retryCount);
        }
        else
          throw;
      }
    }
  }
}
