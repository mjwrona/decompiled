// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Analytics.CloudAnalyticsClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Storage.Analytics
{
  public sealed class CloudAnalyticsClient
  {
    private CloudBlobClient blobClient;
    private CloudTableClient tableClient;

    internal string LogContainer { get; set; }

    public CloudAnalyticsClient(
      Microsoft.Azure.Storage.StorageUri blobStorageUri,
      Microsoft.Azure.Storage.StorageUri tableStorageUri,
      Microsoft.Azure.Storage.Auth.StorageCredentials credentials)
    {
      ArgumentUtility.CheckForNull<Microsoft.Azure.Storage.StorageUri>(blobStorageUri, nameof (blobStorageUri));
      ArgumentUtility.CheckForNull<Microsoft.Azure.Storage.StorageUri>(tableStorageUri, nameof (tableStorageUri));
      this.blobClient = new CloudBlobClient(blobStorageUri, credentials);
      this.tableClient = new CloudTableClient(new Microsoft.Azure.Cosmos.Table.StorageUri(tableStorageUri.PrimaryUri, tableStorageUri.SecondaryUri), credentials.ToTableCredentials());
      this.LogContainer = "$logs";
    }

    public void SetServerTimeout(TimeSpan timeout)
    {
      this.blobClient.DefaultRequestOptions.ServerTimeout = new TimeSpan?(timeout);
      this.tableClient.DefaultRequestOptions.ServerTimeout = new TimeSpan?(timeout);
    }

    public CloudBlobDirectory GetLogDirectory(StorageService service) => this.blobClient.GetContainerReference(this.LogContainer).GetDirectoryReference(service.ToString().ToLowerInvariant());

    public CloudTable GetHourMetricsTable(StorageService service) => this.GetHourMetricsTable(service, Microsoft.Azure.Cosmos.Table.StorageLocation.Primary);

    public CloudTable GetHourMetricsTable(StorageService service, Microsoft.Azure.Cosmos.Table.StorageLocation location)
    {
      switch (service)
      {
        case StorageService.Blob:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsHourPrimaryTransactionsBlob") : this.tableClient.GetTableReference("$MetricsHourSecondaryTransactionsBlob");
        case StorageService.Queue:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsHourPrimaryTransactionsQueue") : this.tableClient.GetTableReference("$MetricsHourSecondaryTransactionsQueue");
        case StorageService.Table:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsHourPrimaryTransactionsTable") : this.tableClient.GetTableReference("$MetricsHourSecondaryTransactionsTable");
        case StorageService.File:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsHourPrimaryTransactionsFile") : this.tableClient.GetTableReference("$MetricsHourSecondaryTransactionsFile");
        default:
          throw new ArgumentException(string.Format("Invalid storage service type {0}", (object) service));
      }
    }

    public CloudTable GetMinuteMetricsTable(StorageService service) => this.GetMinuteMetricsTable(service, Microsoft.Azure.Cosmos.Table.StorageLocation.Primary);

    public CloudTable GetMinuteMetricsTable(StorageService service, Microsoft.Azure.Cosmos.Table.StorageLocation location)
    {
      switch (service)
      {
        case StorageService.Blob:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsMinutePrimaryTransactionsBlob") : this.tableClient.GetTableReference("$MetricsMinuteSecondaryTransactionsBlob");
        case StorageService.Queue:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsMinutePrimaryTransactionsQueue") : this.tableClient.GetTableReference("$MetricsMinuteSecondaryTransactionsQueue");
        case StorageService.Table:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsMinutePrimaryTransactionsTable") : this.tableClient.GetTableReference("$MetricsMinuteSecondaryTransactionsTable");
        case StorageService.File:
          return location == Microsoft.Azure.Cosmos.Table.StorageLocation.Primary ? this.tableClient.GetTableReference("$MetricsMinutePrimaryTransactionsFile") : this.tableClient.GetTableReference("$MetricsMinuteSecondaryTransactionsFile");
        default:
          throw new ArgumentException(string.Format("Invalid storage service type {0}", (object) service));
      }
    }

    public CloudTable GetCapacityTable() => this.tableClient.GetTableReference("$MetricsCapacityBlob");

    public IEnumerable<ICloudBlob> ListLogs(StorageService service) => this.ListLogs(service, Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All, BlobListingDetails.None, (BlobRequestOptions) null, (Microsoft.Azure.Storage.OperationContext) null);

    public IEnumerable<ICloudBlob> ListLogs(
      StorageService service,
      Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations operations,
      BlobListingDetails details,
      BlobRequestOptions options,
      Microsoft.Azure.Storage.OperationContext operationContext)
    {
      BlobListingDetails blobListingDetails = BlobListingDetails.None;
      if (details.HasFlag((Enum) BlobListingDetails.Copy) || details.HasFlag((Enum) BlobListingDetails.Snapshots) || details.HasFlag((Enum) BlobListingDetails.UncommittedBlobs))
        throw new ArgumentException("Invalid blob listing details specified.");
      if (operations == Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.None)
        throw new ArgumentException("Invalid logging operations specified.");
      if (details.HasFlag((Enum) BlobListingDetails.Metadata) || !operations.HasFlag((Enum) Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All))
        blobListingDetails = BlobListingDetails.Metadata;
      return this.GetLogDirectory(service).ListBlobs(true, blobListingDetails, options, operationContext).Select<IListBlobItem, ICloudBlob>((Func<IListBlobItem, ICloudBlob>) (log => (ICloudBlob) log)).Where<ICloudBlob>((Func<ICloudBlob, bool>) (log => CloudAnalyticsClient.IsCorrectLogType(log, operations)));
    }

    public IEnumerable<ICloudBlob> ListLogs(
      StorageService service,
      DateTimeOffset startTime,
      DateTimeOffset? endTime)
    {
      return this.ListLogs(service, startTime, endTime, Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All, BlobListingDetails.None, (BlobRequestOptions) null, (Microsoft.Azure.Storage.OperationContext) null);
    }

    public IEnumerable<ICloudBlob> ListLogs(
      StorageService service,
      DateTimeOffset startTime,
      DateTimeOffset? endTime,
      Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations operations,
      BlobListingDetails details,
      BlobRequestOptions options,
      Microsoft.Azure.Storage.OperationContext operationContext)
    {
      CloudBlobDirectory logDirectory = this.GetLogDirectory(service);
      BlobListingDetails metadataDetails = details;
      DateTimeOffset universalTime = startTime.ToUniversalTime();
      DateTimeOffset dateCounter = new DateTimeOffset(universalTime.Ticks - universalTime.Ticks % 36000000000L, universalTime.Offset);
      DateTimeOffset? utcEndTime = new DateTimeOffset?();
      string endPrefix = (string) null;
      DateTimeOffset utcNow;
      if (endTime.HasValue)
      {
        utcNow = endTime.Value;
        utcEndTime = new DateTimeOffset?(utcNow.ToUniversalTime());
        string prefix = logDirectory.Prefix;
        utcNow = utcEndTime.Value;
        string str = utcNow.ToString("yyyy/MM/dd/HH", (IFormatProvider) CultureInfo.InvariantCulture);
        endPrefix = prefix + str;
        if (universalTime > utcEndTime.Value)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StartTime invalid. The start time '{0}' occurs after the end time '{1}'.", (object) startTime, (object) endTime.Value));
      }
      if (details.HasFlag((Enum) BlobListingDetails.Copy) || details.HasFlag((Enum) BlobListingDetails.Snapshots) || details.HasFlag((Enum) BlobListingDetails.UncommittedBlobs))
        throw new ArgumentException("Invalid blob listing details specified.");
      if (operations == Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.None)
        throw new ArgumentException("Invalid logging operations specified.");
      if (details.HasFlag((Enum) BlobListingDetails.Metadata) || !operations.HasFlag((Enum) Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All))
        metadataDetails = BlobListingDetails.Metadata;
      while (dateCounter.Hour > 0)
      {
        foreach (ICloudBlob listBlob in logDirectory.Container.ListBlobs(logDirectory.Prefix + dateCounter.ToString("yyyy/MM/dd/HH", (IFormatProvider) CultureInfo.InvariantCulture), true, metadataDetails, options, operationContext))
        {
          if (!utcEndTime.HasValue || string.Compare(listBlob.Parent.Prefix, endPrefix) <= 0)
          {
            if (CloudAnalyticsClient.IsCorrectLogType(listBlob, operations))
              yield return listBlob;
          }
          else
            yield break;
        }
        dateCounter = dateCounter.AddHours(1.0);
        DateTimeOffset dateTimeOffset1 = dateCounter;
        utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset dateTimeOffset2 = utcNow.AddHours(1.0);
        if (dateTimeOffset1 > dateTimeOffset2)
          yield break;
      }
      while (dateCounter.Day > 1)
      {
        foreach (ICloudBlob listBlob in logDirectory.Container.ListBlobs(logDirectory.Prefix + dateCounter.ToString("yyyy/MM/dd", (IFormatProvider) CultureInfo.InvariantCulture), true, metadataDetails, options, operationContext))
        {
          if (!utcEndTime.HasValue || string.Compare(listBlob.Parent.Prefix, endPrefix) <= 0)
          {
            if (CloudAnalyticsClient.IsCorrectLogType(listBlob, operations))
              yield return listBlob;
          }
          else
            yield break;
        }
        dateCounter = dateCounter.AddDays(1.0);
        DateTimeOffset dateTimeOffset3 = dateCounter;
        utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset dateTimeOffset4 = utcNow.AddHours(1.0);
        if (dateTimeOffset3 > dateTimeOffset4)
          yield break;
      }
      while (dateCounter.Month > 1)
      {
        foreach (ICloudBlob listBlob in logDirectory.Container.ListBlobs(logDirectory.Prefix + dateCounter.ToString("yyyy/MM", (IFormatProvider) CultureInfo.InvariantCulture), true, metadataDetails, options, operationContext))
        {
          if (!utcEndTime.HasValue || string.Compare(listBlob.Parent.Prefix, endPrefix) <= 0)
          {
            if (CloudAnalyticsClient.IsCorrectLogType(listBlob, operations))
              yield return listBlob;
          }
          else
            yield break;
        }
        dateCounter = dateCounter.AddMonths(1);
        DateTimeOffset dateTimeOffset5 = dateCounter;
        utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset dateTimeOffset6 = utcNow.AddHours(1.0);
        if (dateTimeOffset5 > dateTimeOffset6)
          yield break;
      }
label_36:
      foreach (ICloudBlob listBlob in logDirectory.Container.ListBlobs(logDirectory.Prefix + dateCounter.ToString("yyyy", (IFormatProvider) CultureInfo.InvariantCulture), true, metadataDetails, options, operationContext))
      {
        if (!utcEndTime.HasValue || string.Compare(listBlob.Parent.Prefix, endPrefix) <= 0)
        {
          if (CloudAnalyticsClient.IsCorrectLogType(listBlob, operations))
            yield return listBlob;
        }
        else
          yield break;
      }
      dateCounter = dateCounter.AddYears(1);
      DateTimeOffset dateTimeOffset7 = dateCounter;
      utcNow = DateTimeOffset.UtcNow;
      DateTimeOffset dateTimeOffset8 = utcNow.AddHours(1.0);
      if (!(dateTimeOffset7 > dateTimeOffset8))
        goto label_36;
    }

    public IEnumerable<LogRecord> ListLogRecords(StorageService service) => this.ListLogRecords(service, (BlobRequestOptions) null, (Microsoft.Azure.Storage.OperationContext) null);

    public IEnumerable<LogRecord> ListLogRecords(
      StorageService service,
      BlobRequestOptions options,
      Microsoft.Azure.Storage.OperationContext operationContext)
    {
      return CloudAnalyticsClient.ParseLogBlobs(this.ListLogs(service, Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All, BlobListingDetails.None, options, operationContext));
    }

    public IEnumerable<LogRecord> ListLogRecords(
      StorageService service,
      DateTimeOffset startTime,
      DateTimeOffset? endTime)
    {
      return this.ListLogRecords(service, startTime, endTime, (BlobRequestOptions) null, (Microsoft.Azure.Storage.OperationContext) null);
    }

    public IEnumerable<LogRecord> ListLogRecords(
      StorageService service,
      DateTimeOffset startTime,
      DateTimeOffset? endTime,
      BlobRequestOptions options,
      Microsoft.Azure.Storage.OperationContext operationContext)
    {
      return CloudAnalyticsClient.ParseLogBlobs(this.ListLogs(service, startTime, endTime, Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.All, BlobListingDetails.None, options, operationContext));
    }

    public static IEnumerable<LogRecord> ParseLogBlobs(IEnumerable<ICloudBlob> logBlobs) => logBlobs.SelectMany<ICloudBlob, LogRecord>(CloudAnalyticsClient.\u003C\u003EO.\u003C0\u003E__ParseLogBlob ?? (CloudAnalyticsClient.\u003C\u003EO.\u003C0\u003E__ParseLogBlob = new Func<ICloudBlob, IEnumerable<LogRecord>>(CloudAnalyticsClient.ParseLogBlob)));

    public static IEnumerable<LogRecord> ParseLogBlob(ICloudBlob logBlob)
    {
      using (Stream stream = ((CloudBlob) logBlob).OpenRead())
      {
        using (LogRecordStreamReader reader = new LogRecordStreamReader(stream, (int) stream.Length))
        {
          while (!reader.IsEndOfFile)
            yield return new LogRecord(reader);
        }
      }
    }

    public static IEnumerable<LogRecord> ParseLogStream(Stream stream)
    {
      LogRecordStreamReader reader = new LogRecordStreamReader(stream, (int) stream.Length);
      while (!reader.IsEndOfFile)
        yield return new LogRecord(reader);
    }

    public TableQuery<CapacityEntity> CreateCapacityQuery() => this.GetCapacityTable().CreateQuery<CapacityEntity>();

    public TableQuery<MetricsEntity> CreateHourMetricsQuery(
      StorageService service,
      Microsoft.Azure.Cosmos.Table.StorageLocation location)
    {
      return this.GetHourMetricsTable(service, location).CreateQuery<MetricsEntity>();
    }

    public TableQuery<MetricsEntity> CreateMinuteMetricsQuery(
      StorageService service,
      Microsoft.Azure.Cosmos.Table.StorageLocation location)
    {
      return this.GetMinuteMetricsTable(service, location).CreateQuery<MetricsEntity>();
    }

    internal static bool IsCorrectLogType(ICloudBlob logBlob, Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations operations)
    {
      string str;
      return !logBlob.Metadata.TryGetValue("LogType", out str) || operations.HasFlag((Enum) Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.Read) && str.Contains("read") || operations.HasFlag((Enum) Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.Write) && str.Contains("write") || operations.HasFlag((Enum) Microsoft.Azure.Storage.Shared.Protocol.LoggingOperations.Delete) && str.Contains("delete");
    }
  }
}
