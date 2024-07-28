// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ITable
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public interface ITable
  {
    string Name { get; }

    string StorageAccountName { get; }

    IRetryPolicy RetryPolicy { set; }

    int MaxSegmentSize { get; }

    Task<PreauthenticatedUri> GetRowSasUriAsync(
      VssRequestPump.Processor processor,
      string partition,
      string row);

    Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null);

    Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null);

    Task<TableOperationResult> ExecuteAsync(
      VssRequestPump.Processor processor,
      TableOperationDescriptor operation,
      TableRequestOptions options = null,
      OperationContext context = null);

    Task<TableBatchOperationResult> ExecuteBatchAsync(
      VssRequestPump.Processor processor,
      TableBatchOperationDescriptor operation,
      TableRequestOptions options = null,
      OperationContext context = null);

    Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token,
      TableRequestOptions options = null,
      OperationContext context = null)
      where TEntity : ITableEntity, new();
  }
}
