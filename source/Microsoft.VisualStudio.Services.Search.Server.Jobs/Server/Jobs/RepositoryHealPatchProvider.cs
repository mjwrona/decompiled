// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.RepositoryHealPatchProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataCrawler.MultiBranchMetaData;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataProvider;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class RepositoryHealPatchProvider : IIndexPatchProvider
  {
    internal long TotalItemsInMultiBranchStoreForTFS;
    internal long TotalItemsInMultiBranchStoreForES;
    internal long TotalItemsSkippedForDelete;

    internal CodeCrawlSpec CodeCrawlSpec { get; set; }

    internal VersionControlType VcType { get; set; }

    internal FriendlyDictionary<string, string> LastIndexedChangeIdPerBranch { get; set; }

    internal FriendlyDictionary<string, DateTime> LastIndexedChangeUtcTimePerBranch { get; set; }

    internal DocMetaDataStoreFactory DocMetaDataStoreFactory { get; set; }

    public bool IsDuplicateDocumentsDetected { get; private set; }

    public RepositoryHealPatchProvider(CodeCrawlSpec codeCrawlSpec)
    {
      this.CodeCrawlSpec = codeCrawlSpec;
      this.VcType = this.CodeCrawlSpec.VcType;
      this.DocMetaDataStoreFactory = new DocMetaDataStoreFactory();
      this.IsDuplicateDocumentsDetected = false;
      this.SetLastIndexedChangeIdAndChangeTime();
    }

    public RepositoryHealPatchProvider()
    {
      this.DocMetaDataStoreFactory = new DocMetaDataStoreFactory();
      this.IsDuplicateDocumentsDetected = false;
    }

    public IEnumerable<PatchDescription> GetPatches(
      IndexingExecutionContext iexContext,
      string branchName,
      CodeCrawlSpec codeCrawlSpec,
      IndexingUnit indexingUnit)
    {
      this.CodeCrawlSpec = codeCrawlSpec;
      this.VcType = codeCrawlSpec.VcType;
      int thresholdForMaxDocs = iexContext.ServiceSettings.JobSettings.PatchInMemoryThresholdForMaxDocs;
      if (iexContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/EnableMultiBranchRepoHeal", true))
      {
        IEnumerable<KeyValuePair<string, MultiBranchDocMetaData>> metaData1 = ((IMultiBranchDocMetadataProvider) new TFSMultiBranchDocMetadataProvider(iexContext, this.CodeCrawlSpec)).GetMetaData();
        this.SetLastIndexedChangeIdAndChangeTime();
        IEnumerable<KeyValuePair<string, MultiBranchDocMetaData>> metaData2 = ((IMultiBranchDocMetadataProvider) this.GetESMultiBranchDocMetadataProvider(iexContext, indexingUnit)).GetMetaData();
        return this.GetDifferenceBetweenTFSvsES(metaData1.GetEnumerator(), metaData2.GetEnumerator(), iexContext);
      }
      IEnumerable<KeyValuePair<string, string>> metaData3 = this.DocMetaDataStoreFactory.GetTFSDocMetadataStore(iexContext, this.CodeCrawlSpec, true, thresholdForMaxDocs).GetMetaData();
      IEnumerable<KeyValuePair<string, string>> metaData4 = this.DocMetaDataStoreFactory.GetESDocMetadataStore(iexContext, this.CodeCrawlSpec, true, thresholdForMaxDocs).GetMetaData();
      Func<string, string, bool> comparator = this.GetComparator();
      return this.GetDifferenceBetweenTFSvsES(metaData3.GetEnumerator(), metaData4.GetEnumerator(), comparator);
    }

    public void PostPatchOperation(
      IndexingExecutionContext iexContext,
      string branchName,
      IEnumerable<PatchDescription> patchDescriptions)
    {
      iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Total items cralwed from multi branch store for TFS : {0}. Total items crawled from multi branch store for ES : {1}. Total items skipped for detete : {2}", (object) this.TotalItemsInMultiBranchStoreForTFS, (object) this.TotalItemsInMultiBranchStoreForES, (object) this.TotalItemsSkippedForDelete)));
    }

    internal Func<string, string, bool> GetComparator() => new Func<string, string, bool>(this.FileVersionComparator);

    public IEnumerable<PatchDescription> GetDifferenceBetweenTFSvsES(
      IEnumerator<KeyValuePair<string, MultiBranchDocMetaData>> readerTFS,
      IEnumerator<KeyValuePair<string, MultiBranchDocMetaData>> readerES)
    {
      return this.GetDifferenceBetweenTFSvsES(readerTFS, readerES, (IndexingExecutionContext) null);
    }

    public IEnumerable<PatchDescription> GetDifferenceBetweenTFSvsES(
      IEnumerator<KeyValuePair<string, MultiBranchDocMetaData>> readerTFS,
      IEnumerator<KeyValuePair<string, MultiBranchDocMetaData>> readerES,
      IndexingExecutionContext indexingExecutionContext)
    {
      bool movedESReader = readerTFS != null && readerES != null ? readerES.MoveNext() : throw new ArgumentNullException();
      bool movedTFSReader = readerTFS.MoveNext();
      if (movedTFSReader)
        ++this.TotalItemsInMultiBranchStoreForTFS;
      if (movedESReader)
        ++this.TotalItemsInMultiBranchStoreForES;
      List<PatchDescription> patchDescriptionList = new List<PatchDescription>();
      string tfsFilePath;
      MultiBranchDocMetaData tfsMetaData;
      KeyValuePair<string, PathHashContent> kvp;
      PathHashContent pathHashContent;
      KeyValuePair<string, MultiBranchDocMetaData> current;
      while (movedTFSReader & movedESReader)
      {
        current = readerES.Current;
        string esFilePath = current.Key;
        current = readerES.Current;
        MultiBranchDocMetaData esMetaData = current.Value;
        current = readerTFS.Current;
        tfsFilePath = current.Key;
        current = readerTFS.Current;
        tfsMetaData = current.Value;
        if (!esMetaData.FilePath.Equals(esFilePath))
          throw new ArgumentException("Inconsistent file paths in ESReader");
        if (!tfsMetaData.FilePath.Equals(tfsFilePath))
          throw new ArgumentException("Inconsistent file paths in TFSREader");
        int num = string.Compare(tfsFilePath, esFilePath, CrawlerHelpers.GitPathStringComparison);
        if (num == 0)
        {
          Dictionary<string, Tuple<string, long?>> toContentHashMap1 = this.GetBranchToContentHashMap(tfsMetaData);
          Dictionary<string, Tuple<string, long?>> toContentHashMap2 = this.GetBranchToContentHashMap(esMetaData);
          foreach (PatchDescription hashMapsAndGetPatch in this.MergeBranchToContentHashMapsAndGetPatches(tfsMetaData.FilePath, toContentHashMap1, toContentHashMap2, indexingExecutionContext))
            yield return hashMapsAndGetPatch;
          movedESReader = readerES.MoveNext();
          movedTFSReader = readerTFS.MoveNext();
          if (movedTFSReader)
          {
            string strA = tfsFilePath;
            current = readerTFS.Current;
            string key = current.Key;
            int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
            if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
            {
              object[] objArray = new object[3]
              {
                (object) nameof (readerTFS),
                (object) tfsFilePath,
                null
              };
              current = readerTFS.Current;
              objArray[2] = (object) current.Key;
              throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
            }
            ++this.TotalItemsInMultiBranchStoreForTFS;
          }
          if (movedESReader)
          {
            string strA = esFilePath;
            current = readerES.Current;
            string key = current.Key;
            int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
            if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
            {
              object[] objArray = new object[3]
              {
                (object) nameof (readerES),
                (object) esFilePath,
                null
              };
              current = readerES.Current;
              objArray[2] = (object) current.Key;
              throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
            }
            ++this.TotalItemsInMultiBranchStoreForES;
          }
        }
        else if (num < 0)
        {
          foreach (KeyValuePair<string, PathHashContent> keyValuePair in tfsMetaData.PathHashContent)
          {
            kvp = keyValuePair;
            pathHashContent = kvp.Value;
            foreach (string branch in pathHashContent.Branches)
              yield return PatchDescription.CreateAddPatchDescription(tfsMetaData.FilePath, branch, kvp.Key, this.VcType, this.LastIndexedChangeIdPerBranch[branch], this.LastIndexedChangeUtcTimePerBranch[branch], pathHashContent.FileSize);
            pathHashContent = (PathHashContent) null;
            kvp = new KeyValuePair<string, PathHashContent>();
          }
          movedTFSReader = readerTFS.MoveNext();
          if (movedTFSReader)
          {
            string strA = tfsFilePath;
            current = readerTFS.Current;
            string key = current.Key;
            int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
            if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
            {
              object[] objArray = new object[3]
              {
                (object) nameof (readerTFS),
                (object) tfsFilePath,
                null
              };
              current = readerTFS.Current;
              objArray[2] = (object) current.Key;
              throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
            }
            ++this.TotalItemsInMultiBranchStoreForTFS;
          }
        }
        else if (num > 0)
        {
          if (indexingExecutionContext != null && indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/DisableFileDeletionDuringRepoHeal"))
          {
            ++this.TotalItemsSkippedForDelete;
          }
          else
          {
            foreach (KeyValuePair<string, PathHashContent> keyValuePair in esMetaData.PathHashContent)
            {
              kvp = keyValuePair;
              foreach (string branch in kvp.Value.Branches)
                yield return PatchDescription.CreateDeletePatchDescription(esMetaData.FilePath, branch, kvp.Key, this.VcType, this.LastIndexedChangeIdPerBranch[branch], this.LastIndexedChangeUtcTimePerBranch[branch], kvp.Value.FileSize);
              kvp = new KeyValuePair<string, PathHashContent>();
            }
          }
          movedESReader = readerES.MoveNext();
          if (movedESReader)
          {
            string strA = esFilePath;
            current = readerES.Current;
            string key = current.Key;
            int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
            if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
            {
              object[] objArray = new object[3]
              {
                (object) nameof (readerES),
                (object) esFilePath,
                null
              };
              current = readerES.Current;
              objArray[2] = (object) current.Key;
              throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
            }
            ++this.TotalItemsInMultiBranchStoreForES;
          }
        }
        esFilePath = (string) null;
        esMetaData = (MultiBranchDocMetaData) null;
        tfsFilePath = (string) null;
        tfsMetaData = (MultiBranchDocMetaData) null;
      }
      while (movedESReader)
      {
        current = readerES.Current;
        tfsFilePath = current.Key;
        current = readerES.Current;
        tfsMetaData = current.Value;
        current = readerTFS.Current;
        MultiBranchDocMetaData branchDocMetaData = current.Value;
        if (!tfsMetaData.FilePath.Equals(tfsFilePath))
          throw new ArgumentException("Inconsistent file paths in ESReader");
        if (indexingExecutionContext != null && indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/DisableFileDeletionDuringRepoHeal"))
        {
          ++this.TotalItemsSkippedForDelete;
        }
        else
        {
          foreach (KeyValuePair<string, PathHashContent> keyValuePair in tfsMetaData.PathHashContent)
          {
            kvp = keyValuePair;
            foreach (string branch in kvp.Value.Branches)
              yield return PatchDescription.CreateDeletePatchDescription(tfsMetaData.FilePath, branch, kvp.Key, this.VcType, this.LastIndexedChangeIdPerBranch[branch], this.LastIndexedChangeUtcTimePerBranch[branch], kvp.Value.FileSize);
            kvp = new KeyValuePair<string, PathHashContent>();
          }
        }
        movedESReader = readerES.MoveNext();
        if (movedESReader)
        {
          string strA = tfsFilePath;
          current = readerES.Current;
          string key = current.Key;
          int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
          if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
          {
            object[] objArray = new object[3]
            {
              (object) nameof (readerES),
              (object) tfsFilePath,
              null
            };
            current = readerES.Current;
            objArray[2] = (object) current.Key;
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
          }
          ++this.TotalItemsInMultiBranchStoreForES;
        }
        tfsFilePath = (string) null;
        tfsMetaData = (MultiBranchDocMetaData) null;
      }
      while (movedTFSReader)
      {
        current = readerTFS.Current;
        tfsFilePath = current.Key;
        current = readerES.Current;
        MultiBranchDocMetaData branchDocMetaData = current.Value;
        current = readerTFS.Current;
        tfsMetaData = current.Value;
        if (!tfsMetaData.FilePath.Equals(tfsFilePath))
          throw new ArgumentException("Inconsistent file paths in ESReader");
        foreach (KeyValuePair<string, PathHashContent> keyValuePair in tfsMetaData.PathHashContent)
        {
          kvp = keyValuePair;
          pathHashContent = kvp.Value;
          foreach (string branch in pathHashContent.Branches)
            yield return PatchDescription.CreateAddPatchDescription(tfsMetaData.FilePath, branch, kvp.Key, this.VcType, this.LastIndexedChangeIdPerBranch[branch], this.LastIndexedChangeUtcTimePerBranch[branch], pathHashContent.FileSize);
          pathHashContent = (PathHashContent) null;
          kvp = new KeyValuePair<string, PathHashContent>();
        }
        movedTFSReader = readerTFS.MoveNext();
        if (movedTFSReader)
        {
          string strA = tfsFilePath;
          current = readerTFS.Current;
          string key = current.Key;
          int stringComparison = (int) CrawlerHelpers.GitPathStringComparison;
          if (string.Compare(strA, key, (StringComparison) stringComparison) > 0)
          {
            object[] objArray = new object[3]
            {
              (object) nameof (readerTFS),
              (object) tfsFilePath,
              null
            };
            current = readerTFS.Current;
            objArray[2] = (object) current.Key;
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryHealPatchProvider: input is not sorted as per expectation. {0}: previous path = {1}, current path = {2}", objArray)));
          }
          ++this.TotalItemsInMultiBranchStoreForTFS;
        }
        tfsFilePath = (string) null;
        tfsMetaData = (MultiBranchDocMetaData) null;
      }
    }

    private Dictionary<string, Tuple<string, long?>> GetBranchToContentHashMap(
      MultiBranchDocMetaData metaData)
    {
      Dictionary<string, Tuple<string, long?>> toContentHashMap = new Dictionary<string, Tuple<string, long?>>();
      foreach (KeyValuePair<string, PathHashContent> keyValuePair in metaData.PathHashContent)
      {
        foreach (string branch in keyValuePair.Value.Branches)
        {
          if (toContentHashMap.ContainsKey(branch))
            this.IsDuplicateDocumentsDetected = true;
          else
            toContentHashMap.Add(branch, new Tuple<string, long?>(keyValuePair.Key, keyValuePair.Value.FileSize));
        }
      }
      return toContentHashMap;
    }

    private List<PatchDescription> MergeBranchToContentHashMapsAndGetPatches(
      string filePath,
      Dictionary<string, Tuple<string, long?>> branchToContentHashMapTFS,
      Dictionary<string, Tuple<string, long?>> branchToContentHashMapES,
      IndexingExecutionContext indexingExecutionContext)
    {
      Dictionary<string, PatchDescription> dictionary = new Dictionary<string, PatchDescription>();
      foreach (KeyValuePair<string, Tuple<string, long?>> keyValuePair in branchToContentHashMapTFS)
        dictionary.Add(keyValuePair.Key, PatchDescription.CreateAddPatchDescription(filePath, keyValuePair.Key, keyValuePair.Value.Item1, this.VcType, this.LastIndexedChangeIdPerBranch[keyValuePair.Key], this.LastIndexedChangeUtcTimePerBranch[keyValuePair.Key], keyValuePair.Value.Item2));
      foreach (KeyValuePair<string, Tuple<string, long?>> keyValuePair in branchToContentHashMapES)
      {
        PatchDescription patchDescription;
        if (dictionary.TryGetValue(keyValuePair.Key, out patchDescription))
        {
          if (!patchDescription.ContentHash.Equals(keyValuePair.Value.Item1))
          {
            dictionary.Remove(keyValuePair.Key);
            dictionary.Add(keyValuePair.Key, PatchDescription.CreateEditPatchDescription(filePath, keyValuePair.Key, patchDescription.ContentHash, keyValuePair.Value.Item1, this.VcType, this.LastIndexedChangeIdPerBranch[keyValuePair.Key], this.LastIndexedChangeUtcTimePerBranch[keyValuePair.Key], patchDescription.ContentSize));
          }
          else
            dictionary.Remove(keyValuePair.Key);
        }
        else if (indexingExecutionContext != null && indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/DisableFileDeletionDuringRepoHeal"))
          ++this.TotalItemsSkippedForDelete;
        else
          dictionary.Add(keyValuePair.Key, PatchDescription.CreateDeletePatchDescription(filePath, keyValuePair.Key, keyValuePair.Value.Item1, this.VcType, this.LastIndexedChangeIdPerBranch[keyValuePair.Key], this.LastIndexedChangeUtcTimePerBranch[keyValuePair.Key], keyValuePair.Value.Item2));
      }
      return dictionary.Values.ToList<PatchDescription>();
    }

    internal IEnumerable<PatchDescription> GetDifferenceBetweenTFSvsES(
      IEnumerator<KeyValuePair<string, string>> readerTFS,
      IEnumerator<KeyValuePair<string, string>> readerES,
      Func<string, string, bool> fileVersionComparator)
    {
      bool movedESReader = readerES.MoveNext();
      bool movedTFSReader = readerTFS.MoveNext();
      while (movedTFSReader & movedESReader)
      {
        KeyValuePair<string, string> current1 = readerES.Current;
        KeyValuePair<string, string> current2 = readerTFS.Current;
        int num = string.Compare(current2.Key, current1.Key, CrawlerHelpers.GitPathStringComparison);
        if (num == 0)
        {
          if (!fileVersionComparator(current2.Value, current1.Value))
            yield return PatchDescription.CreatePathDescription(current2.Key, MetaDataStoreUpdateType.Add, this.CodeCrawlSpec.VcType, this.CodeCrawlSpec.LastIndexedChangeId, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
          movedESReader = readerES.MoveNext();
          movedTFSReader = readerTFS.MoveNext();
        }
        else if (num < 0)
        {
          yield return PatchDescription.CreatePathDescription(current2.Key, MetaDataStoreUpdateType.Add, this.CodeCrawlSpec.VcType, this.CodeCrawlSpec.LastIndexedChangeId, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
          movedTFSReader = readerTFS.MoveNext();
        }
        else if (num > 0)
        {
          yield return PatchDescription.CreatePathDescription(current1.Key, MetaDataStoreUpdateType.Delete, this.CodeCrawlSpec.VcType, this.CodeCrawlSpec.LastIndexedChangeId, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
          movedESReader = readerES.MoveNext();
        }
      }
      for (; movedESReader; movedESReader = readerES.MoveNext())
        yield return PatchDescription.CreatePathDescription(readerES.Current.Key, MetaDataStoreUpdateType.Delete, this.CodeCrawlSpec.VcType, this.CodeCrawlSpec.LastIndexedChangeId, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
      for (; movedTFSReader; movedTFSReader = readerTFS.MoveNext())
        yield return PatchDescription.CreatePathDescription(readerTFS.Current.Key, MetaDataStoreUpdateType.Add, this.CodeCrawlSpec.VcType, this.CodeCrawlSpec.LastIndexedChangeId, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
    }

    internal bool FileVersionComparator(string contentIdTFS, string contentIdES) => string.Compare(contentIdTFS, contentIdES, StringComparison.Ordinal) == 0;

    internal ESMultiBranchDocMetadataProvider GetESMultiBranchDocMetadataProvider(
      IndexingExecutionContext iexContext,
      IndexingUnit indexingUnit)
    {
      return indexingUnit.IndexingUnitType == "ScopedIndexingUnit" && iexContext.RepositoryIndexingUnit.IndexingUnitType == "Git_Repository" && indexingUnit.GetScopePathFromTFSAttributesIfScopedIUElseNull() == "/" ? (ESMultiBranchDocMetadataProvider) new ESMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit(iexContext, this.CodeCrawlSpec) : new ESMultiBranchDocMetadataProvider(iexContext, this.CodeCrawlSpec);
    }

    private void SetLastIndexedChangeIdAndChangeTime()
    {
      this.LastIndexedChangeIdPerBranch = new FriendlyDictionary<string, string>();
      this.LastIndexedChangeUtcTimePerBranch = new FriendlyDictionary<string, DateTime>();
      if (this.CodeCrawlSpec is CustomCrawlSpec)
      {
        CustomCrawlSpec codeCrawlSpec = this.CodeCrawlSpec as CustomCrawlSpec;
        if (codeCrawlSpec.BranchesInfo == null)
          return;
        foreach (CustomBranchInfo customBranchInfo in codeCrawlSpec.BranchesInfo)
        {
          this.LastIndexedChangeIdPerBranch.Add(customBranchInfo.BranchName, customBranchInfo.ChangeId);
          this.LastIndexedChangeUtcTimePerBranch.Add(customBranchInfo.BranchName, customBranchInfo.ChangeTime);
        }
      }
      else if (this.CodeCrawlSpec is GitIndexCrawlSpec)
      {
        GitIndexCrawlSpec codeCrawlSpec = this.CodeCrawlSpec as GitIndexCrawlSpec;
        if (((GitCrawlSpec) codeCrawlSpec).CurrentBranchesInfo == null)
          return;
        foreach (BranchInfo branchInfo in ((GitCrawlSpec) codeCrawlSpec).CurrentBranchesInfo)
        {
          this.LastIndexedChangeIdPerBranch.Add(branchInfo.BranchName, branchInfo.ChangeId);
          this.LastIndexedChangeUtcTimePerBranch.Add(branchInfo.BranchName, branchInfo.ChangeTime);
        }
      }
      else if (this.CodeCrawlSpec is TfvcIndexCrawlSpec)
      {
        this.LastIndexedChangeIdPerBranch.Add(string.Empty, this.CodeCrawlSpec.LastIndexedChangeId);
        this.LastIndexedChangeUtcTimePerBranch.Add(string.Empty, this.CodeCrawlSpec.LastIndexedChangeUtcTime);
      }
      else
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unexpected {0} encountered.", (object) "CodeCrawlSpec")));
    }
  }
}
