// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureCloudTableAdapter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureCloudTableAdapter : ITable
  {
    private readonly CloudTable cloudTable;

    public AzureCloudTableAdapter(CloudTable cloudTable) => this.cloudTable = cloudTable;

    public string Name => this.cloudTable.Name;

    public string StorageAccountName => StorageMigration.GetStorageAccountName(this.cloudTable.Uri.AbsoluteUri);

    public int MaxSegmentSize => 1000;

    public IRetryPolicy RetryPolicy
    {
      set => this.cloudTable.ServiceClient.DefaultRequestOptions.RetryPolicy = value;
    }

    public Task<PreauthenticatedUri> GetRowSasUriAsync(
      VssRequestPump.Processor processor,
      string partition,
      string row)
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        string sharedAccessSignature = this.cloudTable.GetSharedAccessSignature(new SharedAccessTablePolicy()
        {
          SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) DateTime.UtcNow.AddMinutes(60.0)),
          Permissions = SharedAccessTablePermissions.Query
        }, (string) null, partition, row, partition, row);
        return Task.FromResult<PreauthenticatedUri>(new PreauthenticatedUri(new Uri(string.Format("{0}(PartitionKey='{1}',RowKey='{2}'){3}", (object) this.cloudTable.Uri, (object) partition, (object) row, (object) sharedAccessSignature)), EdgeType.NotEdge));
      }
    }

    public Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options,
      OperationContext context)
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        context = processor.CreateTableContext(context);
        return this.cloudTable.CreateIfNotExistsAsync(options, context, processor.CancellationToken);
      }
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options,
      OperationContext context)
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        context = processor.CreateTableContext(context);
        return this.cloudTable.DeleteIfExistsAsync(options, context, processor.CancellationToken);
      }
    }

    public async Task<TableOperationResult> ExecuteAsync(
      VssRequestPump.Processor processor,
      TableOperationDescriptor operation,
      TableRequestOptions options,
      OperationContext context)
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        context = processor.CreateTableContext(context);
        try
        {
          TableResult tableResult = await this.cloudTable.ExecuteAsync(AzureCloudTableAdapter.TableOperationFactory.CreateTableOperation(operation), options, context, processor.CancellationToken).ConfigureAwait(false);
          return new TableOperationResult((HttpStatusCode) tableResult.HttpStatusCode, tableResult.Result);
        }
        catch (StorageException ex)
        {
          HttpStatusCode httpStatusCode = (HttpStatusCode) ex.RequestInformation.HttpStatusCode;
          if (this.ShouldReturnHttpStatusCode(httpStatusCode, operation.OperationType))
            return new TableOperationResult(httpStatusCode, (object) null);
          throw new ExpandedTableStorageException(string.Format("Operation of type {0} returned non-returnable status code: {1}", (object) operation.OperationType.ToString(), (object) httpStatusCode), ex, this.StorageAccountName);
        }
      }
    }

    public async Task<TableBatchOperationResult> ExecuteBatchAsync(
      VssRequestPump.Processor processor,
      TableBatchOperationDescriptor batchOperation,
      TableRequestOptions options,
      OperationContext context)
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        context = processor.CreateTableContext(context);
        try
        {
          return TableBatchOperationResult.FromSuccess((IList<TableOperationResult>) (await this.cloudTable.ExecuteBatchAsync(AzureCloudTableAdapter.TableOperationFactory.CreateTableBatchOperation(batchOperation), options, context, processor.CancellationToken).ConfigureAwait(false)).Select<TableResult, TableOperationResult>((Func<TableResult, TableOperationResult>) (tr => new TableOperationResult((HttpStatusCode) tr.HttpStatusCode, tr.Result))).Cast<TableOperationResult>().ToList<TableOperationResult>());
        }
        catch (StorageException ex)
        {
          int? failedOperationIndex = new int?();
          string errorCode = (string) null;
          int index = -1;
          if (ex.TryGetFailedOperationIndex(batchOperation, out index))
            failedOperationIndex = new int?(index);
          if (ex.RequestInformation.ExtendedErrorInformation != null)
            errorCode = ex.RequestInformation.ExtendedErrorInformation.ErrorCode;
          HttpStatusCode httpStatusCode = (HttpStatusCode) ex.RequestInformation.HttpStatusCode;
          if (failedOperationIndex.HasValue && this.ShouldReturnHttpStatusCode(httpStatusCode, batchOperation[failedOperationIndex.Value].OperationType))
            return TableBatchOperationResult.FromError(failedOperationIndex, batchOperation[failedOperationIndex.Value], errorCode, httpStatusCode, ex);
          throw new ExpandedTableStorageException(string.Format("Operation batch returned non-returnable status code: {0}", (object) httpStatusCode), ex, this.StorageAccountName);
        }
      }
    }

    public async Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token,
      TableRequestOptions options,
      OperationContext context1)
      where TEntity : ITableEntity, new()
    {
      using (new ActivityLogPerfTimer(processor, "TableStorage"))
      {
        context1 = processor.CreateTableContext(context1);
        TableContinuationToken continuationToken;
        if (token != null)
          continuationToken = new TableContinuationToken()
          {
            NextPartitionKey = token.NextPartitionKey,
            NextRowKey = token.NextRowKey
          };
        else
          continuationToken = (TableContinuationToken) null;
        TableContinuationToken azureToken = continuationToken;
        AsyncRetryPolicy asyncRetryPolicy = AsyncRetrySyntax.WaitAndRetryAsync(Policy.Handle<ExpandedStorageException>().Or<ExpandedTableStorageException>().Or<Exception>((Func<Exception, bool>) (ex => ex is StorageException storageException && storageException.RequestInformation.HttpStatusCode == 306)), 3, (Func<int, TimeSpan>) (retryAttempt => TimeSpan.FromSeconds(Math.Pow(5.0, (double) retryAttempt))), (Action<Exception, TimeSpan, Context>) ((exception, retryCount, context2) => processor.TraceExceptionAsync(ContentTracePoints.XStore.ExecuteQuerySegmentedAsync, exception)));
        try
        {
          TableQuerySegment<TEntity> tableQuerySegment = await ((AsyncPolicy) asyncRetryPolicy).ExecuteAsync<TableQuerySegment<TEntity>>((Func<Task<TableQuerySegment<TEntity>>>) (async () => await this.cloudTable.ExecuteQuerySegmentedAsync<TEntity>(query.CreateTableQuery(), azureToken, options, context1, processor.CancellationToken).ConfigureAwait(false))).ConfigureAwait(false);
          tableQuerySegment.Results.ValidateQueryResult<TEntity>(query);
          return (IResultSegment<TEntity>) new AzureCloudTableAdapter.ResultSegment<TEntity>(tableQuerySegment.Results, tableQuerySegment.ContinuationToken == null ? (ITableQueryContinuationToken) null : (ITableQueryContinuationToken) new AzureCloudTableAdapter.TableQueryContinuationToken(tableQuerySegment.ContinuationToken), true);
        }
        catch (StorageException ex)
        {
          if (ex.HasHttpStatus(HttpStatusCode.NotFound))
            return (IResultSegment<TEntity>) new AzureCloudTableAdapter.ResultSegment<TEntity>(new List<TEntity>(), (ITableQueryContinuationToken) null, false);
          throw new ExpandedTableStorageException(ex, this.StorageAccountName);
        }
      }
    }

    private bool ShouldReturnHttpStatusCode(HttpStatusCode httpCode, TableOperationType opType)
    {
      bool flag1;
      switch (opType)
      {
        case TableOperationType.Insert:
          if (httpCode <= HttpStatusCode.NoContent)
          {
            if (httpCode != HttpStatusCode.Created && httpCode != HttpStatusCode.NoContent)
              goto label_5;
          }
          else if (httpCode != HttpStatusCode.NotFound && httpCode != HttpStatusCode.Conflict && httpCode != HttpStatusCode.PreconditionFailed)
            goto label_5;
          bool flag2 = true;
          goto label_6;
label_5:
          flag2 = false;
label_6:
          flag1 = flag2;
          break;
        case TableOperationType.Delete:
        case TableOperationType.Replace:
        case TableOperationType.Merge:
        case TableOperationType.InsertOrReplace:
        case TableOperationType.InsertOrMerge:
          if (httpCode <= HttpStatusCode.NotFound)
          {
            if (httpCode != HttpStatusCode.NoContent && httpCode != HttpStatusCode.NotFound)
              goto label_17;
          }
          else if (httpCode != HttpStatusCode.Conflict && httpCode != HttpStatusCode.PreconditionFailed)
            goto label_17;
          bool flag3 = true;
          goto label_18;
label_17:
          flag3 = false;
label_18:
          flag1 = flag3;
          break;
        case TableOperationType.Retrieve:
          if (httpCode <= HttpStatusCode.NotFound)
          {
            if (httpCode != HttpStatusCode.OK && httpCode != HttpStatusCode.NotFound)
              goto label_11;
          }
          else if (httpCode != HttpStatusCode.Conflict && httpCode != HttpStatusCode.PreconditionFailed)
            goto label_11;
          bool flag4 = true;
          goto label_12;
label_11:
          flag4 = false;
label_12:
          flag1 = flag4;
          break;
        default:
          flag1 = false;
          break;
      }
      return flag1;
    }

    private sealed class TableQueryContinuationToken : ITableQueryContinuationToken
    {
      public string NextPartitionKey { get; }

      public string NextRowKey { get; }

      public TableQueryContinuationToken(TableContinuationToken token)
      {
        this.NextPartitionKey = token.NextPartitionKey;
        this.NextRowKey = token.NextRowKey;
      }
    }

    private sealed class ResultSegment<TEntity> : IResultSegment<TEntity> where TEntity : ITableEntity, new()
    {
      public bool TableExists { get; }

      public List<TEntity> Results { get; }

      public ITableQueryContinuationToken ContinuationToken { get; }

      public ResultSegment(
        List<TEntity> results,
        ITableQueryContinuationToken continuationToken,
        bool tableExists)
      {
        this.TableExists = tableExists;
        this.Results = results;
        this.ContinuationToken = continuationToken;
      }
    }

    private static class TableOperationFactory
    {
      public static TableOperation CreateTableOperation(TableOperationDescriptor descriptor)
      {
        TableOperation tableOperation;
        switch (descriptor.OperationType)
        {
          case TableOperationType.Insert:
            tableOperation = TableOperation.Insert(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.Delete:
            tableOperation = TableOperation.Delete(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.Replace:
            tableOperation = TableOperation.Replace(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.Merge:
            tableOperation = TableOperation.Merge(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.InsertOrReplace:
            tableOperation = TableOperation.InsertOrReplace(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.InsertOrMerge:
            tableOperation = TableOperation.InsertOrMerge(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
            break;
          case TableOperationType.Retrieve:
            tableOperation = TableOperation.Retrieve(descriptor.PartitionKey, descriptor.RowKey);
            break;
          default:
            tableOperation = (TableOperation) null;
            break;
        }
        return tableOperation;
      }

      public static TableBatchOperation CreateTableBatchOperation(
        TableBatchOperationDescriptor descriptor)
      {
        TableBatchOperation tableBatchOperation = new TableBatchOperation();
        foreach (TableOperationDescriptor descriptor1 in (List<TableOperationDescriptor>) descriptor)
          tableBatchOperation.Add(AzureCloudTableAdapter.TableOperationFactory.CreateTableOperation(descriptor1));
        return tableBatchOperation;
      }
    }
  }
}
