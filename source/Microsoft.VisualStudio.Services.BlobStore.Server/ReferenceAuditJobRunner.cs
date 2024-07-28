// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditJobRunner
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ReferenceAuditJobRunner
  {
    private static readonly IFilter<UserColumn> UserColumnScopeFilter = (IFilter<UserColumn>) new EqualFilter<UserColumn>((IColumnValue<UserColumn>) new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.ScopeIdColumnValue("symbol"));
    private static readonly TableQuery<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity> TableCrawlQuery = new PartitionScanRowRangeQuery<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>(AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.AllRowsFilter, userColumnFilter: ReferenceAuditJobRunner.UserColumnScopeFilter).CreateTableQuery();
    private readonly ReferenceAuditJobInfo jobInfo;
    private readonly IVssRequestContext requestContext;
    private readonly IEnumerable<StrongBoxConnectionString> storageAccountConnectionStrings;
    private readonly IReferenceAuditResultExporter resultExporter;
    private readonly bool isCpuThrottlingEnabled;
    private readonly Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer;
    private readonly CancellationToken cancellationToken;
    private readonly string metadataTableName;
    private readonly Guid serviceHostInstanceId;
    private readonly ConcurrentBag<string> storageHostNames = new ConcurrentBag<string>();

    public ReferenceAuditJobRunner(
      ReferenceAuditJobInfo jobInfo,
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxConnectionString> storageAccountConnectionStrings,
      IReferenceAuditResultExporter resultExporter,
      Guid serviceHostInstanceId,
      bool isCpuThrottlingEnabled,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      CancellationToken cancellationToken)
    {
      this.jobInfo = jobInfo;
      this.requestContext = requestContext;
      this.storageAccountConnectionStrings = storageAccountConnectionStrings;
      this.resultExporter = resultExporter;
      this.serviceHostInstanceId = serviceHostInstanceId;
      this.isCpuThrottlingEnabled = isCpuThrottlingEnabled;
      this.tracer = tracer;
      this.cancellationToken = cancellationToken;
      this.metadataTableName = "md" + serviceHostInstanceId.ConvertToAzureCompatibleString();
    }

    public async Task RunAsync()
    {
      ReferenceAuditJobRunner referenceAuditJobRunner = this;
      bool isRetryRequired;
      do
      {
        using (System.Timers.Timer periodicTraceTimer = new System.Timers.Timer(TimeSpan.FromMinutes(5.0).TotalMilliseconds)
        {
          AutoReset = true,
          Enabled = true
        })
        {
          // ISSUE: reference to a compiler-generated method
          periodicTraceTimer.Elapsed += new ElapsedEventHandler(referenceAuditJobRunner.\u003CRunAsync\u003Eb__13_1);
          isRetryRequired = false;
          try
          {
            // ISSUE: reference to a compiler-generated method
            await referenceAuditJobRunner.requestContext.ForkChildrenAsync<StrongBoxConnectionString, ReferenceAuditJobRunner.IReferenceAuditCrawlTaskService>(Math.Min(referenceAuditJobRunner.storageAccountConnectionStrings.Count<StrongBoxConnectionString>(), Environment.ProcessorCount), referenceAuditJobRunner.storageAccountConnectionStrings, new Func<IVssRequestContext, StrongBoxConnectionString, Task>(referenceAuditJobRunner.\u003CRunAsync\u003Eb__13_2)).ConfigureAwait(true);
            long num = referenceAuditJobRunner.jobInfo.CrawlStatuses.Select<KeyValuePair<string, ReferenceCrawlStatus>, long>((Func<KeyValuePair<string, ReferenceCrawlStatus>, long>) (kvp => kvp.Value.ReferenceCount)).Sum();
            referenceAuditJobRunner.tracer.TraceAlways(string.Format("Found {0} symbol requests in the table: {1}.", (object) num, (object) referenceAuditJobRunner.metadataTableName));
            referenceAuditJobRunner.RemoveSavedCrawlEntries();
          }
          catch (Exception ex) when (isRetryRequired = IsRetryRequired(ex))
          {
            referenceAuditJobRunner.tracer.TraceException(ex);
            referenceAuditJobRunner.jobInfo.ErrorDetails = JobHelper.GetNestedExceptionMessage(ex);
            await Task.Delay(JobHelper.RetryInterval).ConfigureAwait(true);
          }
        }
      }
      while (isRetryRequired);

      bool IsRetryRequired(Exception exception) => AsyncHttpRetryHelper.IsTransientException(exception, this.cancellationToken) && ++this.jobInfo.JobRetryCount < 10 && !this.cancellationToken.IsCancellationRequested;
    }

    private void UpdateReferenceAuditCrawlerStatus(
      IVssRequestContext requestContext,
      IVssRegistryService registrySvc,
      string storageHostName)
    {
      string json = registrySvc.GetValue(requestContext, (RegistryQuery) this.GetCrawlStatusRegistryPath(storageHostName), (string) null);
      if (string.IsNullOrEmpty(json))
        return;
      this.jobInfo.CrawlStatuses[storageHostName] = JsonUtilities.Deserialize<ReferenceCrawlStatus>(json);
    }

    private async Task CrawlTableAndPublishReferencesAsync(
      IVssRequestContext requestContext,
      StrongBoxConnectionString shardConnectionString)
    {
      CloudTable cloudTable;
      string storageHostName;
      IVssRegistryService registrySvc;
      ReferenceCrawlStatus crawlStatus;
      if (!this.TryGetCloudTable(shardConnectionString, out cloudTable))
      {
        cloudTable = (CloudTable) null;
        storageHostName = (string) null;
        registrySvc = (IVssRegistryService) null;
        crawlStatus = (ReferenceCrawlStatus) null;
      }
      else
      {
        storageHostName = cloudTable.StorageUri.PrimaryUri.Host;
        registrySvc = requestContext.GetService<IVssRegistryService>();
        this.UpdateReferenceAuditCrawlerStatus(requestContext, registrySvc, storageHostName);
        crawlStatus = this.jobInfo.CrawlStatuses.GetOrAdd(storageHostName, new ReferenceCrawlStatus());
        while (!crawlStatus.IsComplete)
        {
          TableQuerySegment<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity> result = await cloudTable.ExecuteQuerySegmentedAsync<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>(ReferenceAuditJobRunner.TableCrawlQuery, crawlStatus.CurrentContinuationToken).ConfigureAwait(true);
          await this.ThrottleIfNecessary();
          crawlStatus.ReferenceCount += (long) await this.PublishCrawledEntries(result);
          crawlStatus.CurrentContinuationToken = result.ContinuationToken;
          crawlStatus.IsComplete = crawlStatus.CurrentContinuationToken == null;
          registrySvc.SetValue<string>(requestContext, this.GetCrawlStatusRegistryPath(storageHostName), crawlStatus.Serialize<ReferenceCrawlStatus>());
          result = (TableQuerySegment<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>) null;
        }
        this.tracer.TraceAlways(string.Format("{0} symbol requests were found in {1} on account: {2}", (object) crawlStatus.ReferenceCount, (object) this.metadataTableName, (object) storageHostName));
        cloudTable = (CloudTable) null;
        storageHostName = (string) null;
        registrySvc = (IVssRegistryService) null;
        crawlStatus = (ReferenceCrawlStatus) null;
      }
    }

    private async Task<int> PublishCrawledEntries(
      TableQuerySegment<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity> result)
    {
      ReferenceAuditJobRunner referenceAuditJobRunner = this;
      // ISSUE: reference to a compiler-generated method
      IList<ReferenceAuditEntry> references = (IList<ReferenceAuditEntry>) result.Results.Where<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>((Func<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, bool>) (rowEntry => !string.IsNullOrWhiteSpace(rowEntry.ReferenceId))).Select<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, ReferenceAuditEntry>(new Func<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, ReferenceAuditEntry>(referenceAuditJobRunner.\u003CPublishCrawledEntries\u003Eb__16_1)).ToList<ReferenceAuditEntry>();
      await referenceAuditJobRunner.resultExporter.ExportResultsAsync((IEnumerable<ReferenceAuditEntry>) references, referenceAuditJobRunner.cancellationToken, referenceAuditJobRunner.tracer);
      int count = references.Count;
      references = (IList<ReferenceAuditEntry>) null;
      return count;
    }

    private bool TryGetCloudTable(
      StrongBoxConnectionString strongBoxConnectionString,
      out CloudTable cloudTable)
    {
      CloudStorageAccount account;
      if (!CloudStorageAccount.TryParse(strongBoxConnectionString.ConnectionString, out account))
        throw new Exception("Couldn't parse connection string from strong box. Aborting.");
      cloudTable = account.CreateCloudTableClient().GetTableReference(this.metadataTableName);
      if (cloudTable == null || !cloudTable.Exists())
      {
        this.tracer.TraceAlways("Couldn't locate table: " + this.metadataTableName + " in account: " + account.TableStorageUri.PrimaryUri.Host);
        return false;
      }
      this.storageHostNames.Add(cloudTable.StorageUri.PrimaryUri.Host);
      return true;
    }

    private async Task ThrottleIfNecessary()
    {
      if (!this.isCpuThrottlingEnabled)
        return;
      int num = await CpuThrottleHelper.Instance.Yield(this.jobInfo.CpuThreshold, this.cancellationToken).ConfigureAwait(true);
    }

    private void RemoveSavedCrawlEntries()
    {
      IVssRegistryService service = this.requestContext.GetService<IVssRegistryService>();
      foreach (string storageHostName in this.storageHostNames)
        service.DeleteEntries(this.requestContext, this.GetCrawlStatusRegistryPath(storageHostName));
    }

    private string GetCrawlStatusRegistryPath(string storageHostName) => "/Configuration/BlobStore/ReferenceAuditJob/CrawlStatus/" + storageHostName;

    [DefaultServiceImplementation(typeof (ReferenceAuditJobRunner.ReferenceAuditCrawlTaskService))]
    public interface IReferenceAuditCrawlTaskService : IVssTaskService, IVssFrameworkService
    {
    }

    private sealed class ReferenceAuditCrawlTaskService : 
      VssTaskService,
      ReferenceAuditJobRunner.IReferenceAuditCrawlTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }
  }
}
