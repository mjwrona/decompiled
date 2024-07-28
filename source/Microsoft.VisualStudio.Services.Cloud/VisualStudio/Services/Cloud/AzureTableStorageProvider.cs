// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureTableStorageProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Utilities.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureTableStorageProvider : IAzureTableStorageProvider
  {
    public static readonly int MaxTableOperationsPerBatch = 100;
    private const string OperationCouldNotBeCompletedInTimeMessage = "Operation could not be completed within the specified time.";
    private const string RequestTimeoutExceptionTypeName = "Microsoft.Azure.Documents.RequestTimeoutException";
    private const string RequestRateTooLargeMessage = "The request rate is too large. Please retry after sometime.";
    private SortedList<string, CloudTable> m_cloudTables;
    private string m_storageAccountName;
    private int m_retryCount;
    private string m_connectionString;
    private StorageMigration m_sourceMigration;
    private IEnumerable<string> m_tableNames;
    private const int c_defaultNotificationThreshold = 5000;
    private const int c_defaultCircuitBreakerTimeoutInSeconds = 2;
    private const int c_defaultExecutionTimeoutInSeconds = 4;
    private const double c_defaultServerTimeoutInSeconds = 1.5;
    private const int c_defaultRetryCount = 0;
    private static int s_notificationThreshold = 5000;
    private static TimeSpan s_circuitBreakerTimeout = TimeSpan.FromSeconds(2.0);
    private const string s_registrySettingRootPath = "/Service/AzureTableStorageProvider/";
    private const string s_notificationRegistrySettingPath = "/Service/AzureTableStorageProvider/SlowWarningThreshold";
    internal const string s_circuitBreakerTimeoutRegistrySettingPath = "/Service/AzureTableStorageProvider/CircuitBreakerTimeoutInSeconds";
    internal const string s_executionTimeoutRegistrySettingPath = "/Service/AzureTableStorageProvider/ExecutionTimeoutInSeconds";
    internal const string s_serverTimeoutRegistrySettingPath = "/Service/AzureTableStorageProvider/ServerTimeoutInSeconds";
    internal const string s_retryCountRegistrySettingPath = "/Service/AzureTableStorageProvider/RetryCount";
    private const string c_area = "AzureTableStorageProvider";
    private const string c_layer = "AzureTableStorageProvider";

    private IAzureStorageTablesQueueMessagesUtilService AzureStorageTablesQueueMessagesUtil { get; set; }

    public string TableEndpoint { get; private set; }

    protected CloudTableClient TableClient { get; private set; }

    public AzureTableStorageProvider(
      IVssRequestContext requestContext,
      string connectionString,
      IEnumerable<string> tableNamesPrefixes)
    {
      requestContext.TraceEnter(10003300, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ".ctor");
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
        this.Initialize(requestContext, connectionString, (StorageMigration) null, tableNamesPrefixes);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10003302, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10003301, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ".ctor");
      }
    }

    public AzureTableStorageProvider(
      IVssRequestContext requestContext,
      StorageMigration sourceMigration,
      IEnumerable<string> tableNames)
    {
      requestContext.TraceEnter(10003303, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ".ctor");
      try
      {
        ArgumentUtility.CheckForNull<StorageMigration>(sourceMigration, nameof (sourceMigration));
        this.Initialize(requestContext, (string) null, sourceMigration, tableNames);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10003305, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10003304, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ".ctor");
      }
    }

    internal virtual void Initialize(
      IVssRequestContext requestContext,
      string connectionString,
      StorageMigration sourceMigration,
      IEnumerable<string> tableNames)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tableNames, nameof (tableNames));
      foreach (string tableName in tableNames)
        ArgumentUtility.CheckStringForNullOrEmpty(tableName, "tableName");
      if (string.IsNullOrEmpty(connectionString) == (sourceMigration == null))
        throw new ArgumentException("Only one, connectionString or sourceMigration, and not both, needs to be provided.");
      bool flag = !string.IsNullOrEmpty(connectionString);
      this.m_connectionString = connectionString;
      this.m_sourceMigration = sourceMigration;
      this.m_tableNames = tableNames;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      RegistryEntryCollection registryEntries = vssRequestContext.GetService<IVssRegistryService>().ReadEntries(vssRequestContext, (RegistryQuery) "/Service/AzureTableStorageProvider/*");
      this.AzureStorageTablesQueueMessagesUtil = vssRequestContext.GetService<IAzureStorageTablesQueueMessagesUtilService>();
      AzureTableStorageProvider.s_notificationThreshold = registryEntries.GetValueFromPath<int>("/Service/AzureTableStorageProvider/SlowWarningThreshold", 5000);
      requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Threshold now at {0} milliseconds.", (object) AzureTableStorageProvider.s_notificationThreshold);
      AzureTableStorageProvider.s_circuitBreakerTimeout = TimeSpan.FromSeconds((double) registryEntries.GetValueFromPath<int>("/Service/AzureTableStorageProvider/CircuitBreakerTimeoutInSeconds", 2));
      requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Circuit breaker timeout now at {0} seconds.", (object) AzureTableStorageProvider.s_circuitBreakerTimeout.TotalSeconds);
      this.m_retryCount = registryEntries.GetValueFromPath<int>("/Service/AzureTableStorageProvider/RetryCount", 0);
      requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Default retry count now at {0}.", (object) this.m_retryCount);
      CloudStorageAccount storageAccount = !flag ? new CloudStorageAccount(new StorageCredentials(sourceMigration.SasToken), sourceMigration.StorageAccountName, (string) null, true) : CloudStorageAccount.Parse(connectionString);
      this.TableEndpoint = storageAccount.TableEndpoint.AbsoluteUri;
      this.m_storageAccountName = storageAccount.Credentials.AccountName;
      this.TableClient = this.CreateTableClient(requestContext, storageAccount, registryEntries);
      this.m_cloudTables = new SortedList<string, CloudTable>();
      List<CloudTable> cloudTableList = new List<CloudTable>();
      foreach (string tableName in tableNames)
      {
        if (flag)
          cloudTableList.AddRange((IEnumerable<CloudTable>) this.TableClient.ListTables(tableName).ToList<CloudTable>());
        else
          cloudTableList.Add(this.TableClient.GetTableReference(tableName));
      }
      lock (this.m_cloudTables)
      {
        foreach (CloudTable cloudTable in cloudTableList)
        {
          if (!this.m_cloudTables.ContainsKey(cloudTable.Name))
            this.m_cloudTables.Add(cloudTable.Name, cloudTable);
        }
      }
    }

    private CloudTableClient CreateTableClient(
      IVssRequestContext requestContext,
      CloudStorageAccount storageAccount,
      RegistryEntryCollection registryEntries)
    {
      TableClientConfiguration configuration = (TableClientConfiguration) null;
      if (AzureConnectionStringUtility.IsPremiumTableStorage(storageAccount.TableStorageUri.PrimaryUri))
      {
        bool valueFromPath1 = registryEntries.GetValueFromPath<bool>("UseDirectMode", true);
        requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Direct connection mode set to {0}.", (object) valueFromPath1);
        string valueFromPath2 = registryEntries.GetValueFromPath<string>("PreferredLocations", "");
        requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Preferred regions set to {0}.", (object) valueFromPath2);
        string valueFromPath3 = registryEntries.GetValueFromPath<string>("UserAgent", string.Join<ProductInfoHeaderValue>("|", (IEnumerable<ProductInfoHeaderValue>) UserAgentUtility.GetDefaultRestUserAgent()));
        requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "UserAgent suffix set to {0}.", (object) valueFromPath3);
        configuration = new TableClientConfiguration();
        CosmosExecutorConfiguration executorConfiguration = configuration.CosmosExecutorConfiguration;
        executorConfiguration.UseConnectionModeDirect = valueFromPath1;
        if (!string.IsNullOrEmpty(valueFromPath2))
          executorConfiguration.CurrentRegion = valueFromPath2.Split(new char[1]
          {
            ';'
          }, StringSplitOptions.None)[0];
        executorConfiguration.UserAgentSuffix = AzureTableStorageProvider.NormalizeUserAgentString(valueFromPath3);
      }
      int valueFromPath4 = registryEntries.GetValueFromPath<int>("/Service/AzureTableStorageProvider/ExecutionTimeoutInSeconds", 4);
      requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Execution timeout now at {0} seconds.", (object) valueFromPath4);
      double valueFromPath5 = registryEntries.GetValueFromPath<double>("/Service/AzureTableStorageProvider/ServerTimeoutInSeconds", 1.5);
      requestContext.Trace(10003322, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), "Server timeout now at {0} seconds.", (object) valueFromPath5);
      CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient(configuration);
      cloudTableClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromSeconds((double) valueFromPath4));
      cloudTableClient.DefaultRequestOptions.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(valueFromPath5));
      cloudTableClient.DefaultRequestOptions.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromSeconds(1.0), 5);
      return cloudTableClient;
    }

    public string FindNextTableName(IVssRequestContext requestContext, string tableName) => this.GetRelativeTableName(tableName, 1);

    public string FindPreviousTableName(IVssRequestContext requestContext, string tableName) => this.GetRelativeTableName(tableName, -1);

    public bool CreateIfNotExists(IVssRequestContext requestContext, string tableName)
    {
      CloudTable cloudTable = this.GetCloudTable(tableName, true);
      return this.ExecuteInCircuitBreaker<bool>(requestContext, nameof (CreateIfNotExists), (Func<AzureTableStorageProvider.Tracer, bool>) (_ => cloudTable.CreateIfNotExists(this.GetAdminOperationRequestOptions())), true);
    }

    public void DeleteIfExists(IVssRequestContext requestContext, string tableName)
    {
      try
      {
        CloudTable cloudTable = this.GetCloudTable(tableName);
        this.ExecuteInCircuitBreaker<bool>(requestContext, nameof (DeleteIfExists), (Func<AzureTableStorageProvider.Tracer, bool>) (_ => cloudTable.DeleteIfExists(this.GetAdminOperationRequestOptions())), true);
      }
      catch (TableDoesNotExistException ex)
      {
      }
    }

    public void CreateSharedAccessPolicy(
      IVssRequestContext requestContext,
      string tableName,
      string policyName,
      DateTime expiryDate,
      SharedAccessTablePermissions permissions)
    {
      requestContext.TraceEnter(10003312, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (CreateSharedAccessPolicy));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(policyName, nameof (policyName));
        requestContext.CheckWriteAccess();
        TablePermissions tablePermissions = this.GetTablePermissions(requestContext, tableName);
        if (tablePermissions.SharedAccessPolicies.Any<KeyValuePair<string, SharedAccessTablePolicy>>((Func<KeyValuePair<string, SharedAccessTablePolicy>, bool>) (policy => string.Equals(policy.Key, policyName, StringComparison.OrdinalIgnoreCase))))
          throw new ArgumentException(string.Format("The policy name '{0}' is already in use.  Each policy must have a unique name.", (object) policyName), nameof (policyName));
        tablePermissions.SharedAccessPolicies.Add(policyName, new SharedAccessTablePolicy()
        {
          SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) expiryDate),
          Permissions = permissions
        });
        this.SetTablePermissions(requestContext, tableName, tablePermissions);
      }
      finally
      {
        requestContext.TraceLeave(10003313, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (CreateSharedAccessPolicy));
      }
    }

    public SharedAccessTablePolicies GetSharedAccessPolicies(
      IVssRequestContext requestContext,
      string tableName)
    {
      requestContext.TraceEnter(10003323, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (GetSharedAccessPolicies));
      try
      {
        return this.GetTablePermissions(requestContext, tableName).SharedAccessPolicies;
      }
      finally
      {
        requestContext.TraceLeave(10003324, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (GetSharedAccessPolicies));
      }
    }

    public void DeleteSharedAccessPolicies(
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<string> policyNames)
    {
      requestContext.TraceEnter(10003318, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (DeleteSharedAccessPolicies));
      try
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) policyNames, nameof (policyNames));
        foreach (string policyName in policyNames)
          ArgumentUtility.CheckStringForNullOrEmpty(policyName, "policyName");
        requestContext.CheckWriteAccess();
        TablePermissions tablePermissions = this.GetTablePermissions(requestContext, tableName);
        foreach (string policyName in policyNames)
        {
          if (!tablePermissions.SharedAccessPolicies.Remove(policyName))
            throw new ArgumentException(string.Format("Policy name '{0}' does not exist on this table.", (object) policyName), "policyName");
        }
        this.SetTablePermissions(requestContext, tableName, tablePermissions);
      }
      finally
      {
        requestContext.TraceLeave(10003319, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (DeleteSharedAccessPolicies));
      }
    }

    public string GetSasTokenForPolicy(
      IVssRequestContext requestContext,
      string tableName,
      string policyName)
    {
      requestContext.TraceEnter(10003314, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (GetSasTokenForPolicy));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(policyName, nameof (policyName));
        if (!this.GetTablePermissions(requestContext, tableName).SharedAccessPolicies.Any<KeyValuePair<string, SharedAccessTablePolicy>>((Func<KeyValuePair<string, SharedAccessTablePolicy>, bool>) (policy => string.Equals(policy.Key, policyName, StringComparison.OrdinalIgnoreCase))))
          throw new ArgumentException("Policy name does not exist on this table.", nameof (policyName));
        CloudTable cloudTable = this.GetCloudTable(tableName);
        return this.ExecuteInCircuitBreaker<string>(requestContext, "GetSharedAccessSignature", (Func<AzureTableStorageProvider.Tracer, string>) (_ => cloudTable.GetSharedAccessSignature(new SharedAccessTablePolicy(), policyName)));
      }
      finally
      {
        requestContext.TraceLeave(10003315, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (GetSasTokenForPolicy));
      }
    }

    public void UpdateConnectionString(
      IVssRequestContext requestContext,
      string storageAccountConnectionString)
    {
      requestContext.CheckWriteAccess();
      this.TableClient.Credentials.UpdateKey(CloudStorageAccount.Parse(storageAccountConnectionString).Credentials.Key);
    }

    public IList<T> QueryTable<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> query,
      ref TableContinuationToken continuationToken)
      where T : ITableEntity, new()
    {
      requestContext.TraceEnter(10003325, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (QueryTable));
      try
      {
        ArgumentUtility.CheckForNull<TableQuery<T>>(query, nameof (query));
        CloudTable cloudTable = this.GetCloudTable(tableName);
        TableContinuationToken resumeFrom = continuationToken;
        TableQuerySegment<T> tableQuerySegment1 = this.ExecuteInCircuitBreaker<TableQuerySegment<T>>(requestContext, "ExecuteQuery", (Func<AzureTableStorageProvider.Tracer, TableQuerySegment<T>>) (tracer =>
        {
          TableQuerySegment<T> tableQuerySegment2 = cloudTable.ExecuteQuerySegmented<T>(query, resumeFrom, (TableRequestOptions) null, (OperationContext) null);
          tracer.SetRowCount(tableQuerySegment2.Results.Count);
          return tableQuerySegment2;
        }));
        continuationToken = tableQuerySegment1.ContinuationToken;
        return (IList<T>) tableQuerySegment1.Results;
      }
      finally
      {
        requestContext.TraceLeave(10003326, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (QueryTable));
      }
    }

    public TableResult ExecuteTableOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation)
    {
      requestContext.TraceEnter(10003345, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteTableOperation));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
        ArgumentUtility.CheckForNull<TableOperation>(tableOperation, nameof (tableOperation));
        CloudTable cloudTable = this.GetCloudTable(tableName);
        string storageAccountName = AzureTableStorageProvider.GetStorageAccountName(cloudTable.Uri);
        TableReplicationStatus? nullable1 = this.AzureStorageTablesQueueMessagesUtil.PublishTableOperationMessage(requestContext, storageAccountName, tableName, tableOperation);
        if (nullable1.HasValue)
        {
          TableReplicationStatus? nullable2 = nullable1;
          TableReplicationStatus replicationStatus1 = TableReplicationStatus.Replicating;
          if (!(nullable2.GetValueOrDefault() == replicationStatus1 & nullable2.HasValue))
          {
            nullable2 = nullable1;
            TableReplicationStatus replicationStatus2 = TableReplicationStatus.FailingOver;
            if (!(nullable2.GetValueOrDefault() == replicationStatus2 & nullable2.HasValue))
            {
              this.Initialize(requestContext, this.m_connectionString, this.m_sourceMigration, this.m_tableNames);
              cloudTable = this.GetCloudTable(tableName);
            }
          }
        }
        string actionName = AzureTableStorageProvider.GetOperationType(tableOperation).ToString();
        return this.ExecuteInCircuitBreaker<TableResult>(requestContext, actionName, (Func<AzureTableStorageProvider.Tracer, TableResult>) (_ => cloudTable.Execute(tableOperation)));
      }
      finally
      {
        requestContext.TraceLeave(10003346, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteTableOperation));
      }
    }

    public IList<TableResult> ExecuteBatchOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableBatchOperation batchOperation)
    {
      requestContext.TraceEnter(10003347, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteBatchOperation));
      try
      {
        if (batchOperation == null || batchOperation.Count == 0)
          return (IList<TableResult>) new List<TableResult>();
        CloudTable cloudTable = this.GetCloudTable(tableName);
        string storageAccountName = AzureTableStorageProvider.GetStorageAccountName(cloudTable.Uri);
        TableReplicationStatus? nullable1 = this.AzureStorageTablesQueueMessagesUtil.PublishTableBatchOperationMessage(requestContext, storageAccountName, tableName, batchOperation);
        if (nullable1.HasValue)
        {
          TableReplicationStatus? nullable2 = nullable1;
          TableReplicationStatus replicationStatus1 = TableReplicationStatus.Replicating;
          if (!(nullable2.GetValueOrDefault() == replicationStatus1 & nullable2.HasValue))
          {
            nullable2 = nullable1;
            TableReplicationStatus replicationStatus2 = TableReplicationStatus.FailingOver;
            if (!(nullable2.GetValueOrDefault() == replicationStatus2 & nullable2.HasValue))
            {
              this.Initialize(requestContext, this.m_connectionString, this.m_sourceMigration, this.m_tableNames);
              cloudTable = this.GetCloudTable(tableName);
            }
          }
        }
        return (IList<TableResult>) this.ExecuteInCircuitBreaker<TableBatchResult>(requestContext, "ExecuteBatch", (Func<AzureTableStorageProvider.Tracer, TableBatchResult>) (tracer =>
        {
          tracer.SetRowCount(batchOperation.Count);
          return cloudTable.ExecuteBatch(batchOperation);
        }));
      }
      finally
      {
        requestContext.TraceLeave(10003348, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteBatchOperation));
      }
    }

    public Task<TableResult> ExecuteTableOperationAsync(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation)
    {
      requestContext.TraceEnter(10003351, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteTableOperationAsync));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
        ArgumentUtility.CheckForNull<TableOperation>(tableOperation, nameof (tableOperation));
        CloudTable cloudTable = this.GetCloudTable(tableName);
        string actionName = AzureTableStorageProvider.GetOperationType(tableOperation).ToString();
        return this.ExecuteInCircuitBreakerAsync<TableResult>(requestContext, actionName, (Func<AzureTableStorageProvider.Tracer, Task<TableResult>>) (_ => cloudTable.ExecuteAsync(tableOperation, requestContext.CancellationToken)));
      }
      finally
      {
        requestContext.TraceLeave(10003352, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), nameof (ExecuteTableOperationAsync));
      }
    }

    protected virtual CloudTable GetCloudTable(string tableName, bool allowAdd = false)
    {
      lock (this.m_cloudTables)
      {
        CloudTable tableReference;
        if (!this.m_cloudTables.TryGetValue(tableName, out tableReference))
        {
          if (!allowAdd)
            throw new TableDoesNotExistException(tableName);
          tableReference = this.TableClient.GetTableReference(tableName);
          this.m_cloudTables.Add(tableName, tableReference);
        }
        return tableReference;
      }
    }

    private TableRequestOptions GetAdminOperationRequestOptions()
    {
      TableRequestOptions operationRequestOptions = new TableRequestOptions(this.TableClient.DefaultRequestOptions);
      operationRequestOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMilliseconds(operationRequestOptions.MaximumExecutionTime.Value.TotalMilliseconds * 3.0));
      operationRequestOptions.ServerTimeout = new TimeSpan?(TimeSpan.FromMilliseconds(operationRequestOptions.ServerTimeout.Value.TotalMilliseconds * 3.0));
      return operationRequestOptions;
    }

    private string GetRelativeTableName(string tableName, int offset)
    {
      lock (this.m_cloudTables)
      {
        int num = this.m_cloudTables.IndexOfKey(tableName);
        if (num == -1)
          throw new TableDoesNotExistException(tableName);
        return num + offset < 0 || num + offset >= this.m_cloudTables.Count ? (string) null : this.m_cloudTables.Keys[num + offset];
      }
    }

    private string FormatCommandKey(string operationType) => "AzureTableStorageProvider." + operationType + "-" + this.m_storageAccountName;

    private TablePermissions GetTablePermissions(
      IVssRequestContext requestContext,
      string tableName)
    {
      CloudTable cloudTable = this.GetCloudTable(tableName);
      return this.ExecuteInCircuitBreaker<TablePermissions>(requestContext, "GetPermissions", (Func<AzureTableStorageProvider.Tracer, TablePermissions>) (_ => cloudTable.GetPermissions()));
    }

    private void SetTablePermissions(
      IVssRequestContext requestContext,
      string tableName,
      TablePermissions tablePermissions)
    {
      CloudTable cloudTable = this.GetCloudTable(tableName);
      this.ExecuteInCircuitBreaker(requestContext, "SetPermissions", (Action<AzureTableStorageProvider.Tracer>) (_ => cloudTable.SetPermissions(tablePermissions)));
    }

    private T ExecuteInCircuitBreaker<T>(
      IVssRequestContext requestContext,
      string actionName,
      Func<AzureTableStorageProvider.Tracer, T> action,
      bool adminOperation = false)
    {
      using (AzureTableStorageProvider.Tracer tracer = new AzureTableStorageProvider.Tracer(requestContext, this.TableEndpoint, actionName))
      {
        TimeSpan timeSpan = AzureTableStorageProvider.s_circuitBreakerTimeout;
        if (adminOperation)
          timeSpan = TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds * 3.0);
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) this.FormatCommandKey(actionName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(timeSpan));
        return new CommandService<T>(requestContext, setter, (Func<T>) (() => Execute(tracer))).Execute();
      }

      T Execute(AzureTableStorageProvider.Tracer tracer)
      {
        for (int retryCount = this.m_retryCount; retryCount >= 0; --retryCount)
        {
          try
          {
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.TableStorage))
              return action(tracer);
          }
          catch (StorageException ex) when (retryCount > 0 && AzureTableStorageProvider.IsTimeoutException(ex))
          {
            requestContext.TraceException(10003327, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), (Exception) ex);
            requestContext.Trace(10003328, TraceLevel.Error, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), this.TableEndpoint + ":" + actionName);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10003329, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), ex);
            requestContext.Trace(10003330, TraceLevel.Error, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), this.TableEndpoint + ":" + actionName);
            throw;
          }
        }
        throw new InvalidOperationException("Shouldn't get here. Satisfy the compiler.");
      }
    }

    private static bool IsTimeoutException(StorageException ex)
    {
      if (!(ex.InnerException is TimeoutException) && !string.Equals(ex.InnerException?.GetType().FullName, "Microsoft.Azure.Documents.RequestTimeoutException") && !string.Equals(ex.Message, "The request rate is too large. Please retry after sometime.", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ex.RequestInformation?.ExtendedErrorInformation?.ErrorCode, StorageErrorCodeStrings.OperationTimedOut, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ex.RequestInformation?.ExtendedErrorInformation?.ErrorCode, StorageErrorCodeStrings.ServerBusy, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ex.RequestInformation?.HttpStatusMessage, "Operation could not be completed within the specified time.", StringComparison.InvariantCultureIgnoreCase))
      {
        RequestResult requestInformation = ex.RequestInformation;
        if ((requestInformation != null ? (requestInformation.HttpStatusCode.CompareTo((object) HttpStatusCode.InternalServerError) == 0 ? 1 : 0) : 0) == 0)
          return false;
      }
      return true;
    }

    private void ExecuteInCircuitBreaker(
      IVssRequestContext requestContext,
      string actionName,
      Action<AzureTableStorageProvider.Tracer> action)
    {
      Func<AzureTableStorageProvider.Tracer, int> action1 = (Func<AzureTableStorageProvider.Tracer, int>) (tracer =>
      {
        action(tracer);
        return 0;
      });
      this.ExecuteInCircuitBreaker<int>(requestContext, actionName, action1);
    }

    private Task<T> ExecuteInCircuitBreakerAsync<T>(
      IVssRequestContext requestContext,
      string actionName,
      Func<AzureTableStorageProvider.Tracer, Task<T>> action)
    {
      using (AzureTableStorageProvider.Tracer tracer = new AzureTableStorageProvider.Tracer(requestContext, this.TableEndpoint, actionName))
      {
        CommandSetter commandSetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) this.FormatCommandKey(actionName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(AzureTableStorageProvider.s_circuitBreakerTimeout));
        return new CommandServiceFactory(requestContext, commandSetter.CommandKey, commandSetter.CommandPropertiesDefaults).CreateCommandAsync<T>(commandSetter, (Func<Task<T>>) (() => ExecuteAsync(tracer)), (Func<Task<T>>) null, false).Execute();
      }

      async Task<T> ExecuteAsync(AzureTableStorageProvider.Tracer tracer)
      {
        T obj;
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.TableStorage))
          obj = await action(tracer);
        return obj;
      }
    }

    private static TableOperationType GetOperationType(TableOperation operation) => operation.OperationType;

    private static string NormalizeUserAgentString(string userAgent)
    {
      char[] charArray = userAgent.ToCharArray();
      for (int index = 0; index < charArray.Length; ++index)
      {
        if (!AzureTableStorageProvider.IsValidUserAgentChar(charArray[index]))
          charArray[index] = '-';
      }
      return new string(charArray);
    }

    private static bool IsValidUserAgentChar(char c) => c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == ' ' || c == '.' || c == '-' || c == '_' || c == '/' || c == '|' || c == '!' || c == '~' || c == '#' || c == '$' || c == '&' || c == '+' || c == '*' || c == '\'';

    public static string GetStorageAccountName(Uri uri) => uri.Host.Substring(0, uri.Host.IndexOf('.'));

    private struct Tracer : IDisposable
    {
      private PerformanceTimer m_perfTimer;
      private bool m_isDisposed;
      private string m_accountName;
      private string m_methodName;
      private int m_rowCount;
      private IVssRequestContext m_requestContext;

      public Tracer(
        IVssRequestContext requestContext,
        string accountName,
        string actionName,
        int rowCount = 0)
      {
        this.m_accountName = accountName;
        this.m_methodName = actionName;
        this.m_rowCount = rowCount;
        this.m_requestContext = requestContext;
        VssPerformanceEventSource.Log.WindowsAzureStorageStart(requestContext.UniqueIdentifier, accountName, actionName);
        this.m_isDisposed = false;
        this.m_perfTimer = PerformanceTimer.StartMeasure(requestContext, "TableStorage");
      }

      public void Dispose()
      {
        if (this.m_isDisposed)
          return;
        this.m_perfTimer.End();
        long duration = this.m_perfTimer.Duration / 10000L;
        VssPerformanceEventSource.Log.WindowsAzureStorageStop(this.m_requestContext.UniqueIdentifier, this.m_accountName, this.m_methodName, duration);
        this.TraceTimes(this.m_requestContext, this.m_methodName, duration, this.m_rowCount);
        this.m_perfTimer.Dispose();
        this.m_isDisposed = true;
      }

      public void SetRowCount(int rowCount) => this.m_rowCount = rowCount;

      private void TraceTimes(
        IVssRequestContext requestContext,
        string methodName,
        long duration,
        int rowCount = 0)
      {
        if (duration > (long) AzureTableStorageProvider.s_notificationThreshold)
          requestContext.TraceConditionally(10003320, TraceLevel.Warning, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), new Func<string>(FormatMessage));
        else
          requestContext.TraceConditionally(10003321, TraceLevel.Info, nameof (AzureTableStorageProvider), nameof (AzureTableStorageProvider), new Func<string>(FormatMessage));

        string FormatMessage()
        {
          string str = string.Format("Table storage operation '{0}' took {1} ms", (object) methodName, (object) duration);
          if (rowCount > 0)
            str += string.Format(" with {0} rows.", (object) rowCount);
          return str;
        }
      }
    }
  }
}
