// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.StorageLogStatsProcessor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class StorageLogStatsProcessor : IStorageLogStatsProcessor
  {
    private readonly string storageAccountName;

    public StorageLogStatsProcessor(IVssRequestContext requestContext) => this.storageAccountName = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogStatsJobShard, false, (string) null);

    public AggregatedStorageLogStatsResult AggregateStorageLogTransactions(
      IVssRequestContext requestContext,
      IStorageAccountAdapter storageAccountAdapter,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string filterString)
    {
      AggregatedStorageLogStatsResult storageLogStatsResult = new AggregatedStorageLogStatsResult(startTime, endTime);
      foreach (StorageLogRecord storageLogRecord in this.GetLogRecordsByStorageAccount(requestContext, storageAccountAdapter, startTime, endTime, filterString).SelectMany<KeyValuePair<string, List<StorageLogRecord>>, StorageLogRecord>((Func<KeyValuePair<string, List<StorageLogRecord>>, IEnumerable<StorageLogRecord>>) (kvp => (IEnumerable<StorageLogRecord>) kvp.Value)))
      {
        string operationType = storageLogRecord.OperationType;
        double? nullable;
        if (storageLogRecord == null)
        {
          nullable = new double?();
        }
        else
        {
          TimeSpan? serverLatency = storageLogRecord.ServerLatency;
          ref TimeSpan? local = ref serverLatency;
          nullable = local.HasValue ? new double?(local.GetValueOrDefault().TotalMilliseconds) : new double?();
        }
        double durationInMilliseconds = nullable.Value;
        OperationAggregate aggregatedOperation;
        if (!storageLogStatsResult.AggregatedOperations.TryGetValue(operationType, out aggregatedOperation))
        {
          storageLogStatsResult.AggregatedOperations[operationType] = new OperationAggregate(operationType);
          aggregatedOperation = storageLogStatsResult.AggregatedOperations[operationType];
        }
        aggregatedOperation.RecordOperation(durationInMilliseconds);
      }
      return storageLogStatsResult;
    }

    public Dictionary<Guid, long?> AggregateEgressByHostId(
      IVssRequestContext requestContext,
      IStorageAccountAdapter storageAccountAdapter,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string filterString)
    {
      return this.AggregateEgressByHostIdInternal((IEnumerable<StorageLogRecord>) this.GetLogRecordsByStorageAccount(requestContext, storageAccountAdapter, startTime, endTime, filterString)[this.storageAccountName]);
    }

    public async Task ExportLogRecordsByStorageAccountAsync(
      IVssRequestContext requestContext,
      IStorageAccountAdapter accountAdapter,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string filterString,
      string directoryPath)
    {
      if (this.storageAccountName.IsNullOrEmpty<char>())
        throw new ArgumentException("Storage account is not set in the registry.");
      IEnumerable<string> logPrefixes = this.GetLogPrefixes(startTime, endTime);
      foreach (string populateStorageAccount in accountAdapter.PopulateStorageAccountList(requestContext))
      {
        if (accountAdapter.GetStorageAccountName(populateStorageAccount).Equals(this.storageAccountName))
        {
          List<IListBlobItem> logBlobItems = new List<IListBlobItem>();
          foreach (string prefix in logPrefixes)
            logBlobItems.AddRange(accountAdapter.GetCloudBlobs(populateStorageAccount, prefix));
          await this.WriteLogBlobsAsync(accountAdapter, (IEnumerable<IListBlobItem>) logBlobItems, startTime, endTime, filterString, directoryPath);
        }
      }
      logPrefixes = (IEnumerable<string>) null;
    }

    public Dictionary<string, List<StorageLogRecord>> GetLogRecordsByStorageAccount(
      IVssRequestContext requestContext,
      IStorageAccountAdapter accountAdapter,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string filterString)
    {
      Dictionary<string, List<StorageLogRecord>> byStorageAccount = new Dictionary<string, List<StorageLogRecord>>();
      IEnumerable<string> logPrefixes = this.GetLogPrefixes(startTime, endTime);
      foreach (string populateStorageAccount in accountAdapter.PopulateStorageAccountList(requestContext))
      {
        string storageAccountName = accountAdapter.GetStorageAccountName(populateStorageAccount);
        if (!this.storageAccountName.IsNullOrEmpty<char>() && !this.storageAccountName.Equals(storageAccountName))
        {
          if (byStorageAccount.ContainsKey(this.storageAccountName))
            break;
        }
        else
        {
          byStorageAccount[storageAccountName] = new List<StorageLogRecord>();
          List<IListBlobItem> logBlobItems = new List<IListBlobItem>();
          foreach (string prefix in logPrefixes)
            logBlobItems.AddRange(accountAdapter.GetCloudBlobs(populateStorageAccount, prefix));
          byStorageAccount[storageAccountName] = this.GetLogRecordsFromBlobItems(accountAdapter, (IEnumerable<IListBlobItem>) logBlobItems, startTime, endTime, filterString);
        }
      }
      return byStorageAccount;
    }

    private async Task WriteLogBlobsAsync(
      IStorageAccountAdapter storageAccountAdapter,
      IEnumerable<IListBlobItem> logBlobItems,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string fieldValue,
      string filePath)
    {
      foreach (IListBlobItem logBlobItem in logBlobItems)
      {
        if (!(logBlobItem is ICloudBlob cloudBlob) || !cloudBlob.Metadata.ContainsKey("StartTime") && cloudBlob.Metadata.ContainsKey("EndTime") && cloudBlob.Metadata.ContainsKey("LogType"))
          throw new InvalidOperationException("Unable to access LogBlob Metadata. Aborting.");
        DateTimeOffset universalTime1 = DateTimeOffset.Parse(cloudBlob.Metadata["StartTime"]).ToUniversalTime();
        DateTimeOffset universalTime2 = DateTimeOffset.Parse(cloudBlob.Metadata["EndTime"]).ToUniversalTime();
        DateTimeOffset dateTimeOffset = startTime;
        if (universalTime1 >= dateTimeOffset && universalTime2 <= endTime)
          await storageAccountAdapter.WriteLogBlobToDiskAsync(cloudBlob, fieldValue, filePath);
      }
    }

    private IEnumerable<string> GetLogPrefixes(DateTimeOffset startTime, DateTimeOffset endTime)
    {
      List<string> logPrefixes = new List<string>();
      for (int hour = startTime.Hour; hour <= endTime.Hour; ++hour)
        logPrefixes.Add(string.Format("{0}/{1}/{2:D2}/{3:D2}/{4:D2}00", (object) "blob", (object) startTime.Year, (object) startTime.Month, (object) startTime.Day, (object) hour));
      return (IEnumerable<string>) logPrefixes;
    }

    private List<StorageLogRecord> GetLogRecordsFromBlobItems(
      IStorageAccountAdapter storageAccountAdapter,
      IEnumerable<IListBlobItem> logBlobItems,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      string fieldValue)
    {
      List<StorageLogRecord> recordsFromBlobItems = new List<StorageLogRecord>();
      foreach (IListBlobItem logBlobItem in logBlobItems)
      {
        if (!(logBlobItem is ICloudBlob cloudBlob) || !cloudBlob.Metadata.ContainsKey("StartTime") && !cloudBlob.Metadata.ContainsKey("EndTime") && !cloudBlob.Metadata.ContainsKey("LogType"))
          throw new InvalidOperationException("Unable to access LogBlob Metadata. Aborting.");
        DateTimeOffset universalTime1 = DateTimeOffset.Parse(cloudBlob.Metadata["StartTime"]).ToUniversalTime();
        DateTimeOffset universalTime2 = DateTimeOffset.Parse(cloudBlob.Metadata["EndTime"]).ToUniversalTime();
        DateTimeOffset dateTimeOffset = startTime;
        if (universalTime1 >= dateTimeOffset && universalTime2 <= endTime)
        {
          IEnumerable<StorageLogRecord> logRecords = storageAccountAdapter.GetLogRecords(cloudBlob, fieldValue);
          if (!logRecords.IsNullOrEmpty<StorageLogRecord>())
            recordsFromBlobItems.AddRange(logRecords);
        }
      }
      return recordsFromBlobItems;
    }

    private Dictionary<Guid, long?> AggregateEgressByHostIdInternal(
      IEnumerable<StorageLogRecord> storageLogs)
    {
      Dictionary<Guid, long?> dictionary1 = new Dictionary<Guid, long?>();
      foreach (StorageLogRecord storageLog in storageLogs)
      {
        Guid idFromRequestUrl = storageLog.GetHostIdFromRequestUrl();
        if (idFromRequestUrl.CompareTo(Guid.Empty) != 0)
        {
          if (!dictionary1.TryGetValue(idFromRequestUrl, out long? _))
            dictionary1.Add(idFromRequestUrl, new long?(0L));
          Dictionary<Guid, long?> dictionary2 = dictionary1;
          Guid key1 = idFromRequestUrl;
          Dictionary<Guid, long?> dictionary3 = dictionary2;
          Guid key2 = key1;
          long? nullable1 = dictionary2[key1];
          long? responsePacketSize = storageLog.ResponsePacketSize;
          long? nullable2 = nullable1.HasValue & responsePacketSize.HasValue ? new long?(nullable1.GetValueOrDefault() + responsePacketSize.GetValueOrDefault()) : new long?();
          dictionary3[key2] = nullable2;
        }
      }
      return dictionary1;
    }
  }
}
