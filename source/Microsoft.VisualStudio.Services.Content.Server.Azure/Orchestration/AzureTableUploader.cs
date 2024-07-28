// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration.AzureTableUploader
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration
{
  public abstract class AzureTableUploader
  {
    public readonly IRetryPolicy DefaultTableCreationRetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromMilliseconds(500.0), 5);
    private const int c_tableCreationRetries = 5;
    private ImportSettings m_settings;

    internal virtual string LogHeader => this.m_settings.TablePrefix;

    public AzureTableUploader(ImportSettings settings) => this.m_settings = settings;

    public virtual ITableClientFactory GetAzureTableClientFactory(
      IVssRequestContext requestContext,
      IVssRequestContext deploymentContext,
      string experienceName)
    {
      IEnumerable<StrongBoxConnectionString> connectionStrings = StorageAccountConfigurationFacade.ReadAllStorageAccounts(deploymentContext);
      string tableName = experienceName + requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString();
      ITableClientFactory tableFactory = this.CreateTableFactory(deploymentContext, connectionStrings, tableName, "ConsistentHashing", StorageAccountConfigurationFacade.GetTableLocationMode(deploymentContext));
      OperationContext context = new OperationContext()
      {
        ClientRequestID = requestContext.E2EId.ToString("D")
      };
      ActionBlock<ITable> actionBlock = NonSwallowingActionBlock.Create<ITable>((Func<ITable, Task>) (async table => await this.CreateTableIfNotExistsWithRetries(table, (TableRequestOptions) null, context)), new ExecutionDataflowBlockOptions()
      {
        MaxDegreeOfParallelism = 8
      });
      foreach (ITable allTable in tableFactory.GetAllTables())
        actionBlock.Post(allTable);
      actionBlock.Complete();
      actionBlock.Completion.GetAwaiter().GetResult();
      return tableFactory;
    }

    public virtual async Task<int> UploadEntitiesAsync<T>(
      List<T> entityList,
      DataTransferContext context)
      where T : ITableEntity
    {
      int tableBatchSize = this.m_settings.TableBatchSize;
      List<T>.Enumerator enumerator = entityList.GetEnumerator();
      bool hasNext;
      do
      {
        TableBatchOperationDescriptor batch = new TableBatchOperationDescriptor();
        int num = tableBatchSize;
        hasNext = false;
        for (; num > 0 && (hasNext = enumerator.MoveNext()); --num)
          batch.InsertOrReplace((ITableEntity) enumerator.Current);
        if (num < tableBatchSize)
          await this.UploadTableDataAsync(batch, context.TargetTable, context.PrimaryKey);
      }
      while (hasNext);
      int count = entityList.Count;
      enumerator = new List<T>.Enumerator();
      return count;
    }

    private async Task UploadTableDataAsync(
      TableBatchOperationDescriptor batch,
      ITable targetTable,
      string pk)
    {
      (await targetTable.ExecuteBatchAsync(this.m_settings.TargetProcessor, batch, this.m_settings.TableRequestOptions)).Match((Action<IList<TableOperationResult>>) (tor =>
      {
        if (tor != null && tor.Any<TableOperationResult>((Func<TableOperationResult, bool>) (r => r.HttpStatusCode >= HttpStatusCode.MultipleChoices)))
          throw ArtifactImportException.Create(this.LogHeader, string.Format("Saw at least one error when executing batch on {0} on table {1} in storage account {2}. First error code: {3}.", (object) pk, (object) targetTable.Name, (object) targetTable.StorageAccountName, (object) tor.First<TableOperationResult>((Func<TableOperationResult, bool>) (r => r.HttpStatusCode >= HttpStatusCode.MultipleChoices)).HttpStatusCode), false);
      }), (Action<TableBatchOperationResult.Error>) (err =>
      {
        throw ArtifactImportException.Create(this.LogHeader, "Saw an error when executing batch on " + pk + " on table " + targetTable.Name + " in storage account " + targetTable.StorageAccountName + ". Error code: " + err.ErrorCode + "; error message: " + err.Exception?.Message + ".", false);
      }));
    }

    private async Task CreateTableIfNotExistsWithRetries(
      ITable table,
      TableRequestOptions options,
      OperationContext context)
    {
      table.RetryPolicy = this.DefaultTableCreationRetryPolicy;
      int num = await table.CreateIfNotExistsAsync(this.m_settings.TargetProcessor, options, context) ? 1 : 0;
    }

    protected abstract ITableClientFactory CreateTableFactory(
      IVssRequestContext deploymentContext,
      IEnumerable<StrongBoxConnectionString> connectionStrings,
      string tableName,
      string shardingStrategy,
      LocationMode? locationMode);
  }
}
