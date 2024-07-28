// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableErrorPolicyInsertPolicyBase
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class TableErrorPolicyInsertPolicyBase : ITableErrorPolicy
  {
    public virtual TableExceptionHandlingAction GetBatchResultAction(
      TableBatchOperationResult.Error error,
      VssRequestPump.Processor processor,
      ITable table)
    {
      if (!this.IsTableNotFoundStatus(error) || error.FailedOperation != null && !this.IsInsertOperation(error.FailedOperation))
        return this.GetBatchResultActionForCreatedTable(error, processor, table);
      this.TryCreateTableWithExponentialBackoff(processor, table, (Exception) error.Exception);
      return TableExceptionHandlingAction.Retry;
    }

    protected abstract TableExceptionHandlingAction GetBatchResultActionForCreatedTable(
      TableBatchOperationResult.Error error,
      VssRequestPump.Processor processor,
      ITable table);

    private void TryCreateTableWithExponentialBackoff(
      VssRequestPump.Processor processor,
      ITable table,
      Exception exception)
    {
      int millisecondsTimeout = 500;
label_1:
      try
      {
        TaskSafety.SyncResultOnThreadPool<bool>((Func<Task<bool>>) (() => table.CreateIfNotExistsAsync(processor, context: processor.CreateTableContext())));
      }
      catch (Exception ex)
      {
        Thread.Sleep(millisecondsTimeout);
        millisecondsTimeout *= 2;
        if (millisecondsTimeout >= 60000)
        {
          if (exception == null)
            throw new AggregateException("Initially encountered an exception caused by a missing table. But then failed to create the table despite multiple attempts.", ex);
          throw new AggregateException("Initially encountered an exception caused by a missing table. But then failed to create the table despite multiple attempts.", new Exception[2]
          {
            ex,
            exception
          });
        }
        goto label_1;
      }
    }

    private bool IsInsertOperation(TableOperationDescriptor operation) => operation.OperationType == TableOperationType.Insert || operation.OperationType == TableOperationType.InsertOrMerge || operation.OperationType == TableOperationType.InsertOrReplace;

    private bool IsTableNotFoundStatus(TableBatchOperationResult.Error error) => error.StatusCode == HttpStatusCode.NotFound && TableErrorCodeStrings.TableNotFound.Equals(error.ErrorCode);
  }
}
