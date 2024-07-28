// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableBatchOperationExecutor
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableBatchOperationExecutor
  {
    private static readonly ITableErrorPolicy DefaultErrorStrategy = (ITableErrorPolicy) TableErrorPolicyThrow.Instance;
    private static readonly Regex ErrorText = new Regex("Unexpected response code for operation : ([0-9]+)", RegexOptions.IgnoreCase);
    private readonly ITableClientFactory clientFactory;
    private readonly TableBatchOperationFactory batchFactory;

    public TableBatchOperationExecutor(
      TableBatchOperationFactory batchFactory,
      ITableClientFactory clientFactory)
    {
      this.batchFactory = batchFactory;
      this.clientFactory = clientFactory;
    }

    public IConcurrentIterator<ProcessedTableOperationResult> ProcessAllBatchesConcurrentIterator(
      VssRequestPump.Processor processor,
      ITableErrorPolicy errorStrategy = null)
    {
      return (IConcurrentIterator<ProcessedTableOperationResult>) new ConcurrentIterator<ProcessedTableOperationResult>(new int?(2000), processor.CancellationToken, (Func<TryAddValueAsyncFunc<ProcessedTableOperationResult>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) =>
      {
        bool continueReading = true;
        bool flag;
        do
        {
          int batchesRan = 0;
          Func<TableBatchOperationDescriptor, Task> action = (Func<TableBatchOperationDescriptor, Task>) (async batch =>
          {
            Interlocked.Increment(ref batchesRan);
            foreach (ProcessedTableOperationResult valueToAdd in (IEnumerable<ProcessedTableOperationResult>) await this.ProcessBatchAsync(processor, batch, errorStrategy).ConfigureAwait(false))
            {
              if (!await valueAdderAsync(valueToAdd).ConfigureAwait(false))
                continueReading = false;
            }
          });
          ActionBlock<TableBatchOperationDescriptor> batchQueue = NonSwallowingActionBlock.Create<TableBatchOperationDescriptor>(action, new ExecutionDataflowBlockOptions()
          {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
          });
          foreach (TableBatchOperationDescriptor input in this.batchFactory.GetOperationsBatch())
          {
            ++batchesRan;
            await batchQueue.SendOrThrowSingleBlockNetworkAsync<TableBatchOperationDescriptor>(input, cancellationToken).ConfigureAwait(false);
          }
          batchQueue.Complete();
          await batchQueue.Completion.ConfigureAwait(false);
          flag = batchesRan > 0;
          batchQueue = (ActionBlock<TableBatchOperationDescriptor>) null;
        }
        while (continueReading & flag);
      }));
    }

    internal async Task<IList<ProcessedTableOperationResult>> ProcessBatchAsync(
      VssRequestPump.Processor processor,
      TableBatchOperationDescriptor batchToExecute,
      ITableErrorPolicy errorPolicy = null)
    {
      List<ProcessedTableOperationResult> results = new List<ProcessedTableOperationResult>();
      if (errorPolicy == null)
        errorPolicy = TableBatchOperationExecutor.DefaultErrorStrategy;
      bool aborted = false;
      while (!aborted && batchToExecute.Any<TableOperationDescriptor>())
      {
        ITable table = this.clientFactory.GetTable(TableBatchOperationFactory.GetOperationPartitionKey(batchToExecute[0]));
        (await table.ExecuteBatchAsync(processor, batchToExecute, context: processor.CreateTableContext()).ConfigureAwait(false)).Match(closure_3 ?? (closure_3 = (Action<IList<TableOperationResult>>) (tableResults =>
        {
          results.AddRange(tableResults.Select<TableOperationResult, ProcessedTableOperationResult>((Func<TableOperationResult, ProcessedTableOperationResult>) (r => new ProcessedTableOperationResult(true, r))));
          batchToExecute.Clear();
        })), (Action<TableBatchOperationResult.Error>) (error =>
        {
          TableExceptionHandlingAction batchResultAction = errorPolicy.GetBatchResultAction(error, processor, table);
          switch (batchResultAction)
          {
            case TableExceptionHandlingAction.Retry:
              break;
            case TableExceptionHandlingAction.Abort:
              results.AddRange(batchToExecute.Select<TableOperationDescriptor, ProcessedTableOperationResult>((Func<TableOperationDescriptor, ProcessedTableOperationResult>) (operation => new ProcessedTableOperationResult(false, new TableOperationResult(error.StatusCode, (object) TableBatchOperationFactory.GetOperationEntity(operation))))));
              aborted = true;
              break;
            default:
              if (batchResultAction == TableExceptionHandlingAction.IgnoreFailed && error.FailedOperationIndex.HasValue)
              {
                int index = error.FailedOperationIndex.Value;
                results.Add(new ProcessedTableOperationResult(false, new TableOperationResult(error.StatusCode, (object) TableBatchOperationFactory.GetOperationEntity(batchToExecute[index]))));
                batchToExecute.RemoveAt(index);
                break;
              }
              if (batchResultAction != TableExceptionHandlingAction.IgnoreFailedAndRetryPartialBatch || !error.FailedOperationIndex.HasValue)
                throw new ExpandedTableStorageException(string.Format("HttpStatusCode '{0}' is to be thrown, or not handled by ExceptionPolicy '{1}'", (object) error.StatusCode, (object) errorPolicy.GetType().Name), error.Exception, (string) null);
              int index1 = error.FailedOperationIndex.Value;
              for (int index2 = batchToExecute.Count<TableOperationDescriptor>() - 1; index2 > index1; --index2)
              {
                this.batchFactory.AddOperation(batchToExecute[index2]);
                batchToExecute.RemoveAt(index2);
              }
              results.Add(new ProcessedTableOperationResult(false, new TableOperationResult(error.StatusCode, (object) TableBatchOperationFactory.GetOperationEntity(batchToExecute[index1]))));
              batchToExecute.RemoveAt(index1);
              break;
          }
        }));
      }
      return (IList<ProcessedTableOperationResult>) results;
    }
  }
}
