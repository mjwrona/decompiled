// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers.ItemStoreTableImporter
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration;
using Microsoft.VisualStudio.Services.DataImport;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers
{
  public class ItemStoreTableImporter
  {
    private readonly int sourceDatabaseId;
    private readonly string experienceName;
    private readonly IPartitionKeyFilter partitionKeyFilter;
    private readonly string c_logHeader;

    public ItemStoreTableImporter(
      int sourceDatabaseId,
      string experienceName,
      IPartitionKeyFilter partitionKeyFilter)
    {
      this.sourceDatabaseId = sourceDatabaseId;
      this.experienceName = experienceName;
      this.partitionKeyFilter = partitionKeyFilter;
      this.c_logHeader = "[ItemStore Data Import (" + experienceName + ")]";
    }

    public ItemStoreTableImporter(int sourceDatabaseId, string experienceName)
      : this(sourceDatabaseId, experienceName, (IPartitionKeyFilter) new PassThroughFilter())
    {
    }

    public void ItemStoreDataTransfer(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      string str1 = "Failed";
      string str2 = string.Format("Data import id: {0}; target host: {1}", (object) servicingContext.GetDataImportId(), (object) requestContext.ServiceHost.InstanceId);
      try
      {
        servicingContext.LogInfo(this.c_logHeader + " Started. " + str2);
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        ImportSettings importSettings = this.CreateImportSettings(this.experienceName, servicingContext, requestContext, deploymentContext, this.sourceDatabaseId);
        ITableClientFactory tableClientFactory = importSettings.AzureTableUploader.GetAzureTableClientFactory(requestContext, deploymentContext, this.experienceName);
        this.TransferDataInPhases(requestContext, tableClientFactory, importSettings);
        str1 = "Succeeded";
      }
      finally
      {
        stopwatch.Stop();
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        servicingContext.LogInfo(string.Format("{0} {1} in {2} seconds. {3}", (object) this.c_logHeader, (object) str1, (object) totalSeconds, (object) str2));
      }
    }

    protected virtual ImportSettings CreateImportSettings(
      string experienceName,
      ServicingContext servicingContext,
      IVssRequestContext requestContext,
      IVssRequestContext deploymentContext,
      int sourceDatabaseId)
    {
      return (ImportSettings) new ItemStoreImportSettings(experienceName, (IServicingContext) servicingContext, requestContext, deploymentContext, sourceDatabaseId);
    }

    internal long TransferDataInPhases(
      IVssRequestContext requestContext,
      ITableClientFactory tgtTables,
      ImportSettings settings)
    {
      DataTransferStatistics stat = new DataTransferStatistics();
      ITFLogger logger = settings.Logger;
      while (!requestContext.CancellationToken.IsCancellationRequested)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<string> list = this.partitionKeyFilter.Filter((IEnumerable<string>) settings.ASTableRetriever.ListPrimaryKeys(settings.TransferCheckpoint, settings.WatermarkValue)).ToList<string>();
        if (list.Count != 0)
        {
          ActionBlock<DataTransferContext> dataCopyFlow = this.CreateDataCopyFlow(settings, stat, requestContext.CancellationToken);
          this.DispatchDataCopyTasks(dataCopyFlow, tgtTables, logger, list, requestContext.CancellationToken);
          dataCopyFlow.Complete();
          try
          {
            dataCopyFlow.Completion.GetAwaiter().GetResult();
          }
          catch (Exception ex) when (!(ex is ArtifactImportException))
          {
            ex.MarkAsFatalServicingOrchestrationException();
            throw;
          }
          settings.UpdateWatermark(requestContext, list.Last<string>());
          long totalPrimaryKeys = stat.TotalPrimaryKeys;
          long totalEntities = stat.TotalEntities;
          int keysFromLastBatch = stat.PrimaryKeysFromLastBatch;
          int entitiesFromLastBatch = stat.EntitiesFromLastBatch;
          stopwatch.Stop();
          double totalSeconds = stopwatch.Elapsed.TotalSeconds;
          string message = string.Format("{0} Copy status: total = {1}({2} entities); last batch = {3} ({4} entities in {5} seconds).", (object) this.c_logHeader, (object) totalPrimaryKeys, (object) totalEntities, (object) keysFromLastBatch, (object) entitiesFromLastBatch, (object) totalSeconds);
          logger.Info(message);
          stat.Reset();
        }
        else
          break;
      }
      return stat.TotalPrimaryKeys;
    }

    private int DispatchDataCopyTasks(
      ActionBlock<DataTransferContext> block,
      ITableClientFactory tgtTables,
      ITFLogger logger,
      List<string> pks,
      CancellationToken cancellationToken)
    {
      int num = 0;
      foreach (string pk in pks)
      {
        DataTransferContext input = new DataTransferContext()
        {
          TargetTable = tgtTables.GetTable(pk) ?? throw ArtifactImportException.Create(this.experienceName, "The table factory cannot find a table for the given PK: " + pk, true),
          PrimaryKey = pk
        };
        ++num;
        block.PostOrThrow<DataTransferContext>(input, cancellationToken);
      }
      return num;
    }

    private ActionBlock<DataTransferContext> CreateDataCopyFlow(
      ImportSettings settings,
      DataTransferStatistics stat,
      CancellationToken cancellationToken)
    {
      Func<DataTransferContext, Task> action = (Func<DataTransferContext, Task>) (async context => await this.CopyDataAsync(settings, context, stat));
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = settings.TransferParallelism;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      return NonSwallowingActionBlock.Create<DataTransferContext>(action, dataflowBlockOptions);
    }

    internal async Task CopyDataAsync(
      ImportSettings settings,
      DataTransferContext context,
      DataTransferStatistics stat)
    {
      string primaryKey = context.PrimaryKey;
      ITable targetTable = context.TargetTable;
      IEnumerable<SqlTableEntity> sqlTableEntities = settings.ASTableRetriever.RetrieveEntities(primaryKey);
      List<DynamicTableEntity> entityList = new List<DynamicTableEntity>();
      foreach (SqlTableEntity sqlTableEntity in sqlTableEntities)
      {
        DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
        dynamicTableEntity.PartitionKey = sqlTableEntity.PartitionKey;
        dynamicTableEntity.RowKey = sqlTableEntity.RowKey;
        dynamicTableEntity.ETag = "*";
        dynamicTableEntity.ReadEntity(sqlTableEntity.Properties, (OperationContext) null);
        entityList.Add(dynamicTableEntity);
      }
      stat.AddEntities(await settings.AzureTableUploader.UploadEntitiesAsync<DynamicTableEntity>(entityList, context));
    }
  }
}
