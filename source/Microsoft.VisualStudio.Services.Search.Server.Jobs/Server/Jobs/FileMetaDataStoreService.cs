// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.FileMetaDataStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class FileMetaDataStoreService : IFileMetaDataStoreService
  {
    private IndexingExecutionContext m_executionContext;
    private StorageContext m_storageContext;
    internal List<BasicMetaDataStoreItem> m_feedSuccessfullItems;
    internal List<BasicMetaDataStoreItem> m_feedFailItems;
    internal Dictionary<FileAttributes, List<int>> m_fileAttributeToSuccessfullyFedItemsIndex;
    private long m_startingId;
    private int m_takeCount;
    private Dictionary<FileAttributes, TempFileMetadataRecord> m_tempRecordsDictionary;
    private string m_indexingUnitType;
    private DocumentContractType m_documentContractType;

    internal Dictionary<FileAttributes, TempFileMetadataRecord> TempRecordsDictionary => this.m_tempRecordsDictionary;

    public FileMetaDataStoreService(
      IndexingExecutionContext executionContext,
      StorageContext storageContext,
      CoreCrawlSpec crawlSpec)
    {
      this.m_executionContext = executionContext;
      this.m_storageContext = storageContext;
      if (crawlSpec is FileGroupCrawlSpec fileGroupCrawlSpec)
      {
        this.m_startingId = fileGroupCrawlSpec.StartingId;
        this.m_takeCount = fileGroupCrawlSpec.TakeCount;
      }
      this.m_feedSuccessfullItems = new List<BasicMetaDataStoreItem>();
      this.m_feedFailItems = new List<BasicMetaDataStoreItem>();
      this.m_indexingUnitType = executionContext.IndexingUnit.IndexingUnitType;
      this.m_documentContractType = executionContext.ProvisioningContext.ContractType;
      this.m_tempRecordsDictionary = new Dictionary<FileAttributes, TempFileMetadataRecord>();
      this.m_fileAttributeToSuccessfullyFedItemsIndex = new Dictionary<FileAttributes, List<int>>();
    }

    public void Run(ESIndexFeedResponseData esIndexFeedResponseData)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082621, "Indexing Pipeline", nameof (FileMetaDataStoreService), nameof (Run));
      try
      {
        this.ProcessFeedData(esIndexFeedResponseData);
        this.ConvertFailedFilesMetaDataStoreItemToTempFileMetaDataRecord();
        if (this.m_executionContext.ProvisioningContext.ContractType.IsDedupeFileContract())
        {
          Dictionary<MetaDataStoreUpdateType, List<FileMetadataRecord>> fileMetaDataRecord = this.ConvertMetaDataStoreItemToFileMetaDataRecord();
          if (fileMetaDataRecord != null && fileMetaDataRecord.Count > 0)
          {
            Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> tuple = this.UpdateFileMetadataStore(fileMetaDataRecord, this.m_executionContext.ProvisioningContext.ContractType);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No of ADD records failed to update in FileMetaDataStore {0},No of DELETE records failed to update in FileMetaDataStore {1}", (object) tuple.Item1.Count, (object) tuple.Item2.Count));
            if (tuple.Item1.Count > 0)
              this.ConvertFileMetaDataRecordToTempRecords(MetaDataStoreUpdateType.Add, tuple.Item1);
            if (tuple.Item2.Count > 0)
              this.ConvertFileMetaDataRecordToTempRecords(MetaDataStoreUpdateType.Delete, tuple.Item2);
          }
        }
        List<TempFileMetadataRecord> list = this.m_tempRecordsDictionary.Values.ToList<TempFileMetadataRecord>();
        if (list == null || list.Count < 0)
          return;
        if (this.IsContractSupportedForMerge() && this.m_executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsMergeTempRecordsSupportedInStore", true))
          this.UpdateTempMetaDataStoreUsingMerge(list);
        else
          this.UpdateTempMetaDataStore(list);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082624, "Indexing Pipeline", nameof (FileMetaDataStoreService), nameof (Run));
      }
    }

    internal virtual void ProcessFeedData(ESIndexFeedResponseData responseData)
    {
      List<BasicMetaDataStoreItem> collection1 = new List<BasicMetaDataStoreItem>();
      List<BasicMetaDataStoreItem> collection2 = new List<BasicMetaDataStoreItem>();
      responseData.ConstuctSuccessfulAndFailedItemsList(this.m_storageContext, ref collection1, ref collection2);
      this.m_feedSuccessfullItems.AddRange((IEnumerable<BasicMetaDataStoreItem>) collection1);
      this.m_feedFailItems.AddRange((IEnumerable<BasicMetaDataStoreItem>) collection2);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Successfully fed items : {0}, Failed to feed items : {1}", (object) this.m_feedSuccessfullItems.Count, (object) this.m_feedFailItems.Count));
    }

    internal virtual Dictionary<MetaDataStoreUpdateType, List<FileMetadataRecord>> ConvertMetaDataStoreItemToFileMetaDataRecord()
    {
      Dictionary<MetaDataStoreUpdateType, List<FileMetadataRecord>> fileMetaDataRecord = new Dictionary<MetaDataStoreUpdateType, List<FileMetadataRecord>>();
      Dictionary<FileAttributes, FileMetadataRecord> dictionary1 = new Dictionary<FileAttributes, FileMetadataRecord>();
      Dictionary<FileAttributes, FileMetadataRecord> dictionary2 = new Dictionary<FileAttributes, FileMetadataRecord>();
      for (int index = 0; index < this.m_feedSuccessfullItems.Count; ++index)
      {
        BasicMetaDataStoreItem feedSuccessfullItem = this.m_feedSuccessfullItems[index];
        FileAttributes key = new FileAttributes(feedSuccessfullItem.Path, this.m_indexingUnitType, this.m_documentContractType);
        BranchMetadata branchMetadata = new BranchMetadata();
        branchMetadata.Metadata = new ContentHashBranchListDictionary();
        Branches branches = new Branches();
        branches.AddRange(feedSuccessfullItem.BranchesInfo.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
        if (feedSuccessfullItem.ContentId.HexHash != null)
        {
          branchMetadata.Metadata.Add(feedSuccessfullItem.ContentId.HexHash, branches);
          FileMetadataRecord fileMetadataRecord1 = new FileMetadataRecord()
          {
            FilePath = feedSuccessfullItem.Path,
            BranchMetadata = branchMetadata
          };
          long? filePathId = feedSuccessfullItem.FilePathId;
          if (filePathId.HasValue)
          {
            FileMetadataRecord fileMetadataRecord2 = fileMetadataRecord1;
            filePathId = feedSuccessfullItem.FilePathId;
            long num = filePathId.Value;
            fileMetadataRecord2.Id = num;
          }
          if (feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.Add || feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.UpdateMetaData)
          {
            if (dictionary1.ContainsKey(key))
              dictionary1[key] = this.UpdateFileMetadataRecord((IMetaDataStoreItem) feedSuccessfullItem, dictionary1[key]);
            else
              dictionary1.Add(key, fileMetadataRecord1);
          }
          else if (feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.Delete)
          {
            if (dictionary2.ContainsKey(key))
              dictionary2[key] = this.UpdateFileMetadataRecord((IMetaDataStoreItem) feedSuccessfullItem, dictionary2[key]);
            else
              dictionary2.Add(key, fileMetadataRecord1);
          }
          List<int> intList;
          if (!this.m_fileAttributeToSuccessfullyFedItemsIndex.TryGetValue(key, out intList))
          {
            intList = new List<int>();
            this.m_fileAttributeToSuccessfullyFedItemsIndex[key] = intList;
          }
          intList.Add(index);
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed while creating filemetadata record. ContentId is null."));
      }
      fileMetaDataRecord.Add(MetaDataStoreUpdateType.Add, dictionary1.Values.ToList<FileMetadataRecord>());
      fileMetaDataRecord.Add(MetaDataStoreUpdateType.Delete, dictionary2.Values.ToList<FileMetadataRecord>());
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No of ADD filemetadata records : {0}, No of DELETE filemetadata records : {1}", (object) dictionary1.Count, (object) dictionary2.Count));
      return fileMetaDataRecord;
    }

    internal virtual FileMetadataRecord UpdateFileMetadataRecord(
      IMetaDataStoreItem item,
      FileMetadataRecord existingRecord)
    {
      if (existingRecord == null)
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UpdateFileMetadataRecord existingrecord variable is null"));
      FileMetadataRecord fileMetadataRecord = new FileMetadataRecord()
      {
        FilePath = existingRecord.FilePath,
        Id = existingRecord.Id,
        BranchMetadata = existingRecord.BranchMetadata
      };
      IEnumerable<string> collection = item.BranchesInfo.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName));
      if (fileMetadataRecord.BranchMetadata.Metadata.ContainsKey(item.ContentId.HexHash))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Filemetadata record already contains the contenthash. FileMetadata existing record: {0} Item: {1}", (object) existingRecord, (object) item));
        fileMetadataRecord.BranchMetadata.Metadata[item.ContentId.HexHash].AddRange(collection);
      }
      else
      {
        Branches branches = new Branches();
        branches.AddRange(collection);
        fileMetadataRecord.BranchMetadata.Metadata.Add(item.ContentId.HexHash, branches);
      }
      return fileMetadataRecord;
    }

    internal virtual void ConvertFailedFilesMetaDataStoreItemToTempFileMetaDataRecord()
    {
      foreach (BasicMetaDataStoreItem feedFailItem in this.m_feedFailItems)
      {
        FileAttributes key = new FileAttributes(feedFailItem.Path, this.m_indexingUnitType, this.m_documentContractType);
        if (this.m_tempRecordsDictionary.ContainsKey(key))
          this.m_tempRecordsDictionary[key] = this.UpdateTempMetadataRecord(feedFailItem, this.m_tempRecordsDictionary[key]);
        else
          this.m_tempRecordsDictionary.Add(key, this.CreateTempFileMetadataRecord(feedFailItem));
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No of TempMetaData records due to feed failures {0}", (object) this.m_tempRecordsDictionary.Count));
    }

    private TempFileMetadataRecord CreateTempFileMetadataRecord(BasicMetaDataStoreItem item)
    {
      TempFileMetadataRecord fileMetadataRecord = new TempFileMetadataRecord()
      {
        FileAttributes = new FileAttributes(item.Path, this.m_indexingUnitType, this.m_documentContractType),
        TemporaryBranchMetadata = new TemporaryBranchMetadata()
        {
          BranchMetadata = new UpdateContentDictionary()
        }
      };
      if (item.FilePathId.HasValue)
        fileMetadataRecord.FilePathId = item.FilePathId;
      List<BranchInfo> branchesInfo = item.BranchesInfo;
      UpdateTypeDictionary updateTypeDictionary = new UpdateTypeDictionary()
      {
        ContentSize = item.Size
      };
      updateTypeDictionary.Add(item.UpdateType, branchesInfo);
      UpdateContentDictionary contentDictionary = new UpdateContentDictionary();
      contentDictionary.Add(item.ContentId.HexHash, updateTypeDictionary);
      fileMetadataRecord.TemporaryBranchMetadata.BranchMetadata = contentDictionary;
      return fileMetadataRecord;
    }

    internal virtual TempFileMetadataRecord UpdateTempMetadataRecord(
      BasicMetaDataStoreItem item,
      TempFileMetadataRecord existingRecord)
    {
      UpdateTypeDictionary updateTypeDictionary1;
      if (existingRecord.TemporaryBranchMetadata.BranchMetadata.TryGetValue(item.ContentId.HexHash, out updateTypeDictionary1))
      {
        if (!updateTypeDictionary1.ContainsKey(item.UpdateType))
          updateTypeDictionary1.Add(item.UpdateType, item.BranchesInfo);
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Temp record for filepath {0} already contains the contenthash {1} and updatetype {2}", (object) item.Path, (object) item.ContentId.HexHash, (object) item.UpdateType))));
      }
      else
      {
        UpdateTypeDictionary updateTypeDictionary2 = new UpdateTypeDictionary()
        {
          ContentSize = item.Size
        };
        updateTypeDictionary2.Add(item.UpdateType, item.BranchesInfo);
        existingRecord.TemporaryBranchMetadata.BranchMetadata.Add(item.ContentId.HexHash, updateTypeDictionary2);
      }
      return existingRecord;
    }

    internal virtual void ConvertFileMetaDataRecordToTempRecords(
      MetaDataStoreUpdateType updateType,
      List<FileMetadataRecord> fileMetadataRecords)
    {
      List<int> intList = (List<int>) null;
      TempFileMetadataRecord existingRecord = (TempFileMetadataRecord) null;
      foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecords)
      {
        FileAttributes key = new FileAttributes(fileMetadataRecord.FilePath, this.m_indexingUnitType, this.m_documentContractType);
        if (this.m_fileAttributeToSuccessfullyFedItemsIndex.TryGetValue(key, out intList))
        {
          foreach (int index in intList)
          {
            BasicMetaDataStoreItem feedSuccessfullItem = this.m_feedSuccessfullItems[index];
            if (updateType == MetaDataStoreUpdateType.Add && (feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.Add || feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.UpdateMetaData) || updateType == MetaDataStoreUpdateType.Delete && feedSuccessfullItem.UpdateType == MetaDataStoreUpdateType.Delete)
            {
              if (this.m_tempRecordsDictionary.TryGetValue(key, out existingRecord))
                this.m_tempRecordsDictionary[key] = this.UpdateTempMetadataRecord(feedSuccessfullItem, existingRecord);
              else
                this.m_tempRecordsDictionary.Add(key, this.CreateTempFileMetadataRecord(feedSuccessfullItem));
            }
          }
        }
      }
    }

    internal virtual Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> UpdateFileMetadataStore(
      Dictionary<MetaDataStoreUpdateType, List<FileMetadataRecord>> metaDataRecords,
      DocumentContractType documentContractType)
    {
      try
      {
        return this.m_executionContext.FileMetadataStoreDataAccess.UpdateRecords(this.m_executionContext.RequestContext, (IEnumerable<FileMetadataRecord>) metaDataRecords.GetValueOrDefault<MetaDataStoreUpdateType, List<FileMetadataRecord>>(MetaDataStoreUpdateType.Add, new List<FileMetadataRecord>()), (IEnumerable<FileMetadataRecord>) metaDataRecords.GetValueOrDefault<MetaDataStoreUpdateType, List<FileMetadataRecord>>(MetaDataStoreUpdateType.Delete, new List<FileMetadataRecord>()), documentContractType);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed while adding records to the metaData store. Exception : {0} ", (object) ex.ToString()));
        return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(metaDataRecords.GetValueOrDefault<MetaDataStoreUpdateType, List<FileMetadataRecord>>(MetaDataStoreUpdateType.Add, new List<FileMetadataRecord>()), metaDataRecords.GetValueOrDefault<MetaDataStoreUpdateType, List<FileMetadataRecord>>(MetaDataStoreUpdateType.Delete, new List<FileMetadataRecord>()));
      }
    }

    internal virtual void UpdateTempMetaDataStore(List<TempFileMetadataRecord> tempDataRecords)
    {
      try
      {
        ITempFileMetadataStoreDataAccess metadataStoreDataAccess = this.m_executionContext.TempFileMetadataStoreDataAccess;
        if (tempDataRecords != null && tempDataRecords.Count > 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No of temp records to add {0}", (object) tempDataRecords.Count));
          IDictionary<FileAttributes, byte> existingAttemptCount = this.GetFileAttributeToExistingAttemptCount();
          foreach (TempFileMetadataRecord tempDataRecord in tempDataRecords)
          {
            byte num;
            if (existingAttemptCount.TryGetValue(tempDataRecord.FileAttributes, out num))
              tempDataRecord.AttemptCount = ++num;
          }
          int num1 = metadataStoreDataAccess.AddFileMetadataRecords(this.m_executionContext.RequestContext, (IEnumerable<TempFileMetadataRecord>) tempDataRecords);
          if (num1 != tempDataRecords.Count)
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Adding tempFileMetadata records failed, added: {0}, expectedToAdd: {1}", (object) num1, (object) tempDataRecords.Count)));
        }
        long endingId = this.m_startingId + (long) this.m_takeCount - 1L;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting temp records from stratingid:{0} to endingid:{1}", (object) this.m_startingId, (object) endingId));
        metadataStoreDataAccess.DeleteRecords(this.m_executionContext.RequestContext, this.m_startingId, endingId);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed while adding records to the temp store. Exception : {0} ", (object) ex.ToString()));
      }
    }

    internal virtual void UpdateTempMetaDataStoreUsingMerge(
      List<TempFileMetadataRecord> tempDataRecordsToUpdate)
    {
      IEnumerable<TempFileMetadataRecord> fileMetadataRecords = (IEnumerable<TempFileMetadataRecord>) null;
      try
      {
        ITempFileMetadataStoreDataAccess metadataStoreDataAccess = this.m_executionContext.TempFileMetadataStoreDataAccess;
        long endingId = this.m_startingId + (long) this.m_takeCount - 1L;
        if (tempDataRecordsToUpdate != null && tempDataRecordsToUpdate.Count > 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No of temp records to update {0}", (object) tempDataRecordsToUpdate.Count));
          fileMetadataRecords = metadataStoreDataAccess.GetRecordsByFilePath(this.m_executionContext.RequestContext, this.m_executionContext.IndexingUnit, tempDataRecordsToUpdate.Select<TempFileMetadataRecord, FileAttributes>((Func<TempFileMetadataRecord, FileAttributes>) (x => x.FileAttributes)), this.m_startingId, endingId, this.m_indexingUnitType, this.m_documentContractType);
          if (tempDataRecordsToUpdate.Count != fileMetadataRecords.Count<TempFileMetadataRecord>())
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to retrieve records from the temp store corresponding to failed records. Expected: {0}, Found: {1}", (object) tempDataRecordsToUpdate.Count, (object) fileMetadataRecords.Count<TempFileMetadataRecord>()));
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Retrieving tempFileMetadata records failed, Expected: {0}, Found: {1}", (object) tempDataRecordsToUpdate.Count, (object) fileMetadataRecords.Count<TempFileMetadataRecord>())));
          }
        }
        IEnumerable<long> idsNotToDelete = fileMetadataRecords != null ? fileMetadataRecords.Select<TempFileMetadataRecord, long>((Func<TempFileMetadataRecord, long>) (x => x.Id)) : (IEnumerable<long>) null;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082623, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting temp records from stratingid:{0} to endingid:{1}", (object) this.m_startingId, (object) endingId));
        metadataStoreDataAccess.DeleteRecordsExcludingSome(this.m_executionContext.RequestContext, this.m_executionContext.IndexingUnit, this.m_startingId, endingId, idsNotToDelete);
        IDictionary<FileAttributes, byte> existingAttemptCount = this.GetFileAttributeToExistingAttemptCount(fileMetadataRecords);
        foreach (TempFileMetadataRecord fileMetadataRecord in tempDataRecordsToUpdate)
        {
          byte num;
          if (existingAttemptCount.TryGetValue(fileMetadataRecord.FileAttributes, out num))
            fileMetadataRecord.AttemptCount = ++num;
        }
        metadataStoreDataAccess.UpdateRecordsByOverwrite(this.m_executionContext.RequestContext, this.m_executionContext.IndexingUnit, (IEnumerable<TempFileMetadataRecord>) tempDataRecordsToUpdate);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetaDataStoreService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed while merging records to the temp store. Exception : {0} ", (object) ex.ToString()));
      }
    }

    internal virtual IDictionary<FileAttributes, byte> GetFileAttributeToExistingAttemptCount()
    {
      ITempFileMetadataStoreDataAccess metadataStoreDataAccess = this.m_executionContext.TempFileMetadataStoreDataAccess;
      DocumentContractType contractType1 = this.m_executionContext.ProvisioningContext.ContractType;
      string indexingUnitType1 = this.m_executionContext.IndexingUnit.IndexingUnitType;
      IVssRequestContext requestContext = this.m_executionContext.RequestContext;
      long startingId = this.m_startingId;
      int takeCount = this.m_takeCount;
      string indexingUnitType2 = indexingUnitType1;
      int contractType2 = (int) contractType1;
      IEnumerable<TempFileMetadataRecord> nextBatchOfRecords = metadataStoreDataAccess.GetNextBatchOfRecords(requestContext, startingId, takeCount, indexingUnitType2, (DocumentContractType) contractType2);
      IDictionary<FileAttributes, byte> existingAttemptCount = (IDictionary<FileAttributes, byte>) new Dictionary<FileAttributes, byte>();
      foreach (TempFileMetadataRecord fileMetadataRecord in nextBatchOfRecords)
        existingAttemptCount[fileMetadataRecord.FileAttributes] = fileMetadataRecord.AttemptCount;
      return existingAttemptCount;
    }

    internal virtual IDictionary<FileAttributes, byte> GetFileAttributeToExistingAttemptCount(
      IEnumerable<TempFileMetadataRecord> records)
    {
      IDictionary<FileAttributes, byte> existingAttemptCount = (IDictionary<FileAttributes, byte>) new Dictionary<FileAttributes, byte>();
      foreach (TempFileMetadataRecord record in records)
        existingAttemptCount[record.FileAttributes] = record.AttemptCount;
      return existingAttemptCount;
    }

    internal virtual bool IsContractSupportedForMerge() => this.m_documentContractType == DocumentContractType.DedupeFileContractV4 || this.m_documentContractType == DocumentContractType.DedupeFileContractV5;
  }
}
