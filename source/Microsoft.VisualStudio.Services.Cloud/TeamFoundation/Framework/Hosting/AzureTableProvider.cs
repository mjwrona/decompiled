// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureTableProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  [Obsolete("Use AzureTableStorageProvider instead")]
  public class AzureTableProvider : IAzureTableProvider
  {
    private const string c_opsCommandPrefix = "AzureTable.Ops";
    private const string c_initializePrefix = "AzureTable.Initialize";
    private const string c_querySegmentedPrefix = "AzureTable.QS";
    private CloudStorageAccount m_cloudStorageAccount;
    private CloudTableClient m_cloudTableClient;
    private IDictionary<string, CloudTable> m_cloudTables;
    private ICommandFactory m_commandServiceFactory;
    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(20.0);

    public virtual void Initialize(
      IVssRequestContext requestContext,
      string storageAccountConnectionString,
      IEnumerable<string> tableNames,
      Func<bool> fallback = null,
      bool createIfNotExists = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tableNames, nameof (tableNames));
      try
      {
        this.m_cloudStorageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
      }
      catch (SystemException ex)
      {
        throw new InvalidOperationException("Storage account is not configured properly");
      }
      CommandPropertiesSetter defaultProperties = new CommandPropertiesSetter().WithExecutionTimeout(AzureTableProvider.s_defaultTimeout);
      this.m_commandServiceFactory = (ICommandFactory) new CommandServiceFactory(requestContext, (CommandKey) "AzureTable.Initialize", defaultProperties);
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(this.m_cloudStorageAccount.TableEndpoint);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.Expect100Continue = false;
      this.m_cloudTableClient = this.m_cloudStorageAccount.CreateCloudTableClient();
      this.m_cloudTableClient.DefaultRequestOptions = this.GetTableRequestOptions(requestContext);
      this.m_cloudTables = (IDictionary<string, CloudTable>) new Dictionary<string, CloudTable>();
      foreach (string tableName1 in tableNames)
      {
        string tableName = tableName1;
        Func<bool> run = (Func<bool>) (() =>
        {
          CloudTable cloudTable = this.GetCloudTable(tableName);
          return createIfNotExists && cloudTable.CreateIfNotExists();
        });
        this.ExecuteInCircuitBreaker<bool>(requestContext, "AzureTable.Initialize", AzureTableProvider.s_defaultTimeout, run, fallback);
      }
    }

    public virtual bool Exists(string tableName) => this.GetCloudTable(tableName).Exists();

    public virtual TableResult ExecuteTableOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation,
      Func<TableResult> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TableOperation>(tableOperation, nameof (tableOperation));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      requestContext.CheckWriteAccess();
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreaker<TableResult>(requestContext, "AzureTable.Ops" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<TableResult>) (() => cloudTable.Execute(tableOperation)), fallback);
    }

    public virtual Task<TableResult> ExecuteTableOperationAsync(
      string tableName,
      TableOperation tableOperation,
      Func<Task<TableResult>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
    {
      ArgumentUtility.CheckForNull<TableOperation>(tableOperation, nameof (tableOperation));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreakerAsync<TableResult>("AzureTable.Ops" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<Task<TableResult>>) (() => cloudTable.ExecuteAsync(tableOperation)), fallback);
    }

    public virtual Task<IList<TableResult>> ExecuteTableBatchOperationAsync(
      string tableName,
      TableBatchOperation tableOperation,
      Func<Task<IList<TableResult>>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      string commandKey = null)
    {
      ArgumentUtility.CheckForNull<TableBatchOperation>(tableOperation, nameof (tableOperation));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreakerAsync<IList<TableResult>>("AzureTable.Ops" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<Task<IList<TableResult>>>) (async () => (IList<TableResult>) await cloudTable.ExecuteBatchAsync(tableOperation)), fallback);
    }

    public virtual Task<T> RetrieveAsync<T>(
      string tableName,
      string partitionKey,
      string rowKey,
      Func<Task<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      ArgumentUtility.CheckStringForNullOrEmpty(partitionKey, nameof (partitionKey));
      ArgumentUtility.CheckStringForNullOrEmpty(rowKey, nameof (rowKey));
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreakerAsync<T>("AzureTable.Ops" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<Task<T>>) (async () => (T) (await cloudTable.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey)).ConfigureAwait(false)).Result), fallback);
    }

    public virtual IQueryable<T> CreateTableQueryable<T>(string tableName) where T : ITableEntity, new()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      return (IQueryable<T>) this.GetCloudTable(tableName).CreateQuery<T>();
    }

    public virtual IEnumerable<T> ExecuteTableQuery<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> tableQuery,
      Func<IEnumerable<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new()
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TableQuery<T>>(tableQuery, nameof (tableQuery));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      requestContext.CheckWriteAccess();
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreaker<IEnumerable<T>>(requestContext, "AzureTable.Q" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<IEnumerable<T>>) (() => cloudTable.ExecuteQuery<T>(tableQuery, (TableRequestOptions) null, (OperationContext) null)), fallback);
    }

    public virtual Task<IEnumerable<T>> ExecuteTableQueryAsync<T>(
      string tableName,
      TableQuery<T> tableQuery,
      Func<Task<IEnumerable<T>>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new()
    {
      ArgumentUtility.CheckForNull<TableQuery<T>>(tableQuery, nameof (tableQuery));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreakerAsync<IEnumerable<T>>("AzureTable.Ops" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<Task<IEnumerable<T>>>) (() => AzureTableProvider.ExecuteTableQueryAsync<T>(tableQuery, cloudTable)), fallback);
    }

    public virtual TableQuerySegment<T> ExecuteTableQuerySegmented<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> tableQuery,
      TableContinuationToken continuationToken,
      Func<TableQuerySegment<T>> fallback = null,
      TimeSpan? execIsolationThreadTimeout = null,
      [CallerMemberName] string commandKey = null)
      where T : ITableEntity, new()
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TableQuery<T>>(tableQuery, nameof (tableQuery));
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      requestContext.CheckWriteAccess();
      execIsolationThreadTimeout = new TimeSpan?(execIsolationThreadTimeout ?? AzureTableProvider.s_defaultTimeout);
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreaker<TableQuerySegment<T>>(requestContext, "AzureTable.QS" + (string.IsNullOrEmpty(commandKey) ? string.Empty : "." + commandKey), execIsolationThreadTimeout.Value, (Func<TableQuerySegment<T>>) (() => cloudTable.ExecuteQuerySegmented<T>(tableQuery, continuationToken, (TableRequestOptions) null, (OperationContext) null)), fallback);
    }

    private static async Task<IEnumerable<T>> ExecuteTableQueryAsync<T>(
      TableQuery<T> tableQuery,
      CloudTable cloudTable)
      where T : ITableEntity, new()
    {
      TableContinuationToken token = (TableContinuationToken) null;
      List<T> items = new List<T>();
      do
      {
        TableQuerySegment<T> collection = await cloudTable.ExecuteQuerySegmentedAsync<T>(tableQuery, token).ConfigureAwait(false);
        token = collection.ContinuationToken;
        items.AddRange((IEnumerable<T>) collection);
      }
      while (token != null);
      IEnumerable<T> objs = (IEnumerable<T>) items;
      items = (List<T>) null;
      return objs;
    }

    private T ExecuteInCircuitBreaker<T>(
      IVssRequestContext requestContext,
      string commandKeyPrefix,
      TimeSpan executionIsolationThreadTimeout,
      Func<T> run,
      Func<T> fallback = null)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKeyPrefix).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(executionIsolationThreadTimeout));
      return new CommandService<T>(requestContext, setter, run, fallback).Execute();
    }

    private async Task<T> ExecuteInCircuitBreakerAsync<T>(
      string commandKeyPrefix,
      TimeSpan executionIsolationThreadTimeout,
      Func<Task<T>> run,
      Func<Task<T>> fallback = null)
    {
      return await this.m_commandServiceFactory.CreateCommandAsync<T>(CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKeyPrefix).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(executionIsolationThreadTimeout)), run, fallback).Execute().ConfigureAwait(false);
    }

    private CloudTable GetCloudTable(string tableName)
    {
      if (this.m_cloudStorageAccount == null)
        return (CloudTable) null;
      CloudTable cloudTable;
      return this.m_cloudTables.TryGetValue(tableName, out cloudTable) ? cloudTable : (this.m_cloudTables[tableName] = this.m_cloudTableClient.GetTableReference(tableName));
    }

    private TableRequestOptions GetTableRequestOptions(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TableRequestOptions tableRequestOptions = new TableRequestOptions()
      {
        PayloadFormat = new TablePayloadFormat?(TablePayloadFormat.JsonNoMetadata)
      };
      if (requestContext.ServiceHost == null)
        tableRequestOptions.RetryPolicy = (IRetryPolicy) new ExponentialRetry();
      if (requestContext.ServiceHost.IsProduction)
      {
        tableRequestOptions.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromSeconds(2.0), 5);
      }
      else
      {
        tableRequestOptions.RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(2.0), 5);
        tableRequestOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(5.0));
      }
      return tableRequestOptions;
    }
  }
}
