// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.IAzureTableProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  [Obsolete("Use IAzureTableStorageProvider instead")]
  public interface IAzureTableProvider
  {
    TableResult ExecuteTableOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation,
      Func<TableResult> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null);

    Task<TableResult> ExecuteTableOperationAsync(
      string tableName,
      TableOperation tableOperation,
      Func<Task<TableResult>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null);

    Task<IList<TableResult>> ExecuteTableBatchOperationAsync(
      string tableName,
      TableBatchOperation tableOperation,
      Func<Task<IList<TableResult>>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null);

    Task<T> RetrieveAsync<T>(
      string tableName,
      string partitionKey,
      string rowKey,
      Func<Task<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new();

    IQueryable<T> CreateTableQueryable<T>(string tableName) where T : ITableEntity, new();

    IEnumerable<T> ExecuteTableQuery<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> tableQuery,
      Func<IEnumerable<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new();

    Task<IEnumerable<T>> ExecuteTableQueryAsync<T>(
      string tableName,
      TableQuery<T> tableQuery,
      Func<Task<IEnumerable<T>>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new();

    TableQuerySegment<T> ExecuteTableQuerySegmented<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> tableQuery,
      TableContinuationToken continuationToken,
      Func<TableQuerySegment<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new();

    void Initialize(
      IVssRequestContext requestContext,
      string storageAccountConnectionString,
      IEnumerable<string> tableNames,
      Func<bool> fallback = null,
      bool createIfNotExists = true);

    bool Exists(string tableName);
  }
}
