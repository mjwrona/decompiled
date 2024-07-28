// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureTableMigrator
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Table.AzureImpl;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class AzureTableMigrator
  {
    private const int c_totalRetry = 4;
    public const int ParallelismForPrefixes = 16;
    public const string DoSyncDeleteInternalOperationName = "DoSyncDeleteInternal";
    public const string DoCopyInternalOperationName = "DoCopyInternal";
    private readonly Regex c_tableIndexRegex = new Regex("^.*(\\d+)$", RegexOptions.RightToLeft);
    private readonly ITrackingKey c_globalCopyStartTimeKey = (ITrackingKey) new UncategoriedTrackingKey((string) null);
    private readonly TableRequestOptions tableRequestOptions;
    private readonly string vsoAreaPrefix;
    private readonly string overallPrefix;
    private readonly bool performTableCopy;
    private readonly bool performSyncDelete;
    private readonly bool isDistributedBySA;
    private readonly bool isDistributedByPrefix;
    private readonly TableMigrationConcurrencySetting csetting;
    private Dictionary<string, SourceTableStorage> filteredSourceTables;
    private Dictionary<string, SourceTableStorage> allSourceTables;
    private Dictionary<string, ITable> filteredTargetTables;
    private Dictionary<string, ITable> allTargetTables;
    private ConsistentHashShardManager<TablePhysicalNode> sourceShardManager;
    private ConsistentHashShardManager<TablePhysicalNode> targetShardManager;
    private DateTime? globalLastIncrementalStartTime;
    private ConcurrentDictionary<string, CopyStats> copyStatsDict;
    private ConcurrentDictionary<string, DeleteStats> deleteStatsDict;
    private TableLoader loader;
    private MigratingOperationSettings settings;
    private TableMigrationTimeTracker copyStartTimeTracker;
    private List<ITrackingKey> timeTrackingKeys;
    private ConcurrentDictionary<string, Dictionary<string, bool>> copyTaskCompletions;
    private bool isPremigration;
    private IEnumerable<string> prefixList;
    private bool isInnerPrefixParallelismSet;
    private IAzureTableMigratorCheckpointService checkpointService;
    private DateTime m_startTime;
    private bool canUseCheckpoints;
    private TableCopyDeleteCheckpointBehavior checkpointBehavior;
    private bool m_faulted;

    protected abstract ConcurrencyStrategy ConcurrencyStrategy { get; }

    public virtual MigratingOperationSettings MigratingOperationSettings
    {
      get
      {
        if (this.settings == null)
          this.settings = new MigratingOperationSettings();
        return this.settings;
      }
      set => this.settings = value;
    }

    protected virtual bool ShouldProcessPartitionKey(string pk) => true;

    protected virtual TableLoader GetTableLoader() => (TableLoader) new AzureTableLoader();

    protected virtual IEnumerable<string> GetPrefixes(IVssRequestContext deploymentContext) => (IEnumerable<string>) new string[1]
    {
      string.Empty
    };

    protected virtual StorageMigrationLogger CreateStorageMigrationLogger(
      ISqlConnectionInfo connectionInfo,
      StorageMigration sm)
    {
      return new StorageMigrationLogger(connectionInfo, sm);
    }

    protected internal abstract string GetShardName(
      ConsistentHashShardManager<TablePhysicalNode> manager,
      string partitionKey);

    protected AzureTableMigrator(string vsoAreaPrefix, TableMigrationConcurrencySetting setting)
      : this(vsoAreaPrefix, true, true, setting)
    {
    }

    protected AzureTableMigrator(string vsoAreaPrefix, string prefix = "")
      : this(vsoAreaPrefix, TableMigrationConcurrencySetting.ByOverallPrefix(prefix))
    {
    }

    protected AzureTableMigrator(
      string vsoAreaPrefix,
      bool tableCopy,
      bool syncDelete,
      string prefix = "")
      : this(vsoAreaPrefix, tableCopy, syncDelete, TableMigrationConcurrencySetting.ByOverallPrefix(prefix))
    {
    }

    protected AzureTableMigrator(
      string vsoAreaPrefix,
      bool tableCopy,
      bool syncDelete,
      TableMigrationConcurrencySetting setting)
    {
      this.tableRequestOptions = new TableRequestOptions()
      {
        MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(30.0)),
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(1.0), 10)
      };
      this.vsoAreaPrefix = vsoAreaPrefix;
      this.csetting = setting;
      this.overallPrefix = setting.OverallPrefix;
      if (setting.IsDistributed)
      {
        this.isDistributedBySA = setting.Type == TableMigrationJobLevelConcurrencyType.StorageAccount;
        this.isDistributedByPrefix = setting.Type == TableMigrationJobLevelConcurrencyType.Prefix;
      }
      this.performTableCopy = tableCopy;
      this.performSyncDelete = syncDelete;
      this.checkpointBehavior = TableCopyDeleteCheckpointBehavior.None;
    }

    public async Task IncrementalMigrateAsync(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer,
      bool isTest = false)
    {
      tracer = tracer.Enter(nameof (AzureTableMigrator), nameof (IncrementalMigrateAsync));
      this.checkpointService = deploymentContext.GetService<IAzureTableMigratorCheckpointService>();
      this.checkpointService.CheckToUseLogFile(deploymentContext, migrationEntry);
      string checkpointSwitch = this.checkpointService.GetTableCopyDeleteCheckpointSwitch(deploymentContext, migrationEntry, this.vsoAreaPrefix);
      this.canUseCheckpoints = !string.IsNullOrEmpty(checkpointSwitch) && checkpointSwitch.Equals("True", StringComparison.OrdinalIgnoreCase);
      int numAttempts = migrationEntry.StorageOnly ? 1 : 2;
      int attemptCount = 1;
      while (attemptCount <= numAttempts)
      {
        try
        {
          if (!this.canUseCheckpoints)
            this.checkpointBehavior = TableCopyDeleteCheckpointBehavior.None;
          else if (migrationEntry.StorageOnly || !migrationEntry.StorageOnly && attemptCount > 1)
            this.checkpointBehavior = TableCopyDeleteCheckpointBehavior.LoadAndSave;
          else if (!migrationEntry.StorageOnly)
            this.checkpointBehavior = TableCopyDeleteCheckpointBehavior.SaveOnly;
          tracer.Info(deploymentContext, 197500, (Func<string>) (() => string.Format("CheckpointMode={0};AttemptCount={1}", (object) this.checkpointBehavior, (object) attemptCount)));
          await this.IncrementalMigrateAsync(deploymentContext, migrationEntry, tracer, this.checkpointBehavior);
          break;
        }
        catch (Exception ex)
        {
          ++attemptCount;
          if (attemptCount > numAttempts)
          {
            throw;
          }
          else
          {
            tracer.Error(deploymentContext, 197500, (Func<string>) (() => string.Format("IncrementalMigrateAsync failed. Retrying. AttemptCount/NumAttempts: {0}/{1}", (object) attemptCount, (object) numAttempts)));
            if (!isTest)
            {
              int sleepTimeInSecs = 30000;
              tracer.Error(deploymentContext, 197500, (Func<string>) (() => string.Format("Sleep for {0} before next attempt", (object) sleepTimeInSecs)));
              Thread.Sleep(sleepTimeInSecs);
            }
          }
        }
      }
    }

    internal async Task IncrementalMigrateAsync(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer,
      TableCopyDeleteCheckpointBehavior checkpointBehavior)
    {
      tracer = tracer.Enter(nameof (AzureTableMigrator), nameof (IncrementalMigrateAsync));
      if (!this.performTableCopy && !this.performSyncDelete)
        throw new ArgumentException("A table migration job must specify at least one of the two operations: Table-Copy, Sync-Delete.");
      this.m_startTime = DateTime.UtcNow;
      bool initialized = false;
      bool succeeded = false;
      try
      {
        if (initialized = this.Initialize(deploymentContext, migrationEntry, tracer, checkpointBehavior))
        {
          await deploymentContext.PumpFromAsync((Func<VssRequestPump.Processor, Task>) (async processor => await Task.WhenAll(this.performTableCopy ? this.TableCopyAsync(processor, migrationEntry, tracer) : (Task) Task.FromResult<int>(0), this.performSyncDelete ? this.SyncDeleteAsync(processor, migrationEntry, tracer) : (Task) Task.FromResult<int>(0)).ConfigureAwait(true)));
          this.MarkMigrationAsComplete(deploymentContext, migrationEntry);
          succeeded = true;
          tracer.Info(deploymentContext, 197500, "Migration marked as complete");
        }
        else
        {
          string str = "No tables were found to copy.";
          tracer.Warn(deploymentContext, 197500, str);
          this.UpdateMigrationStatus(deploymentContext, migrationEntry, StorageMigrationStatus.Completed, str);
        }
      }
      catch (Exception ex)
      {
        string str = (string) null;
        ReadOnlyCollection<Exception> innerExceptions = ex is AggregateException aggregateException ? aggregateException.InnerExceptions : (ReadOnlyCollection<Exception>) null;
        if (innerExceptions != null)
        {
          foreach (Exception exception in innerExceptions)
          {
            if (!(exception is OperationCanceledException))
            {
              str = "(multiple exceptions; only showing the first)\n" + exception.ToString();
              break;
            }
          }
        }
        if (str == null)
          str = ex.ToString();
        this.UpdateMigrationStatus(deploymentContext, migrationEntry, StorageMigrationStatus.Failed, "Failed copying sharding tables. Error:\n" + str);
        throw;
      }
      finally
      {
        if (initialized)
        {
          Dictionary<string, SourceTableStorage>.KeyCollection keys = this.filteredSourceTables.Keys;
          if (succeeded)
          {
            if (this.isDistributedByPrefix || this.isDistributedBySA)
            {
              this.SaveCopyStartTimes(deploymentContext, migrationEntry);
            }
            else
            {
              this.checkpointService.SaveIncrementalCopyStartTime(deploymentContext, migrationEntry, this.m_startTime, this.vsoAreaPrefix, this.c_globalCopyStartTimeKey);
              this.checkpointService.DeleteIncrementalCopyStartTime(deploymentContext, migrationEntry, this.vsoAreaPrefix, (IEnumerable<ITrackingKey>) this.timeTrackingKeys);
            }
            if (!checkpointBehavior.Equals((object) TableCopyDeleteCheckpointBehavior.None))
            {
              this.checkpointService.DeletePartitionKeys(deploymentContext, migrationEntry, (IEnumerable<string>) keys, "DoCopyInternal", this.vsoAreaPrefix);
              this.checkpointService.DeletePartitionKeys(deploymentContext, migrationEntry, (IEnumerable<string>) this.filteredTargetTables.Keys, "DoSyncDeleteInternal", this.vsoAreaPrefix);
              tracer.Info(deploymentContext, 197500, "Removed checkpoints after successful migration.");
            }
          }
          else
          {
            this.SaveCopyStartTimes(deploymentContext, migrationEntry);
            if (!migrationEntry.StorageOnly && checkpointBehavior.Equals((object) TableCopyDeleteCheckpointBehavior.LoadAndSave))
            {
              this.checkpointService.DeletePartitionKeys(deploymentContext, migrationEntry, (IEnumerable<string>) keys, "DoCopyInternal", this.vsoAreaPrefix);
              this.checkpointService.DeletePartitionKeys(deploymentContext, migrationEntry, (IEnumerable<string>) this.filteredTargetTables.Keys, "DoSyncDeleteInternal", this.vsoAreaPrefix);
              tracer.Error(deploymentContext, 197500, "Removed checkpoints in failed migration.");
            }
          }
        }
      }
    }

    private void SaveCopyStartTimes(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry)
    {
      if (this.timeTrackingKeys == null)
        return;
      foreach (ITrackingKey timeTrackingKey in this.timeTrackingKeys)
      {
        DateTime? nullable = this.copyStartTimeTracker.Get(timeTrackingKey);
        if (nullable.HasValue)
          this.checkpointService.SaveIncrementalCopyStartTime(deploymentContext, migrationEntry, nullable.Value, this.vsoAreaPrefix, timeTrackingKey);
      }
    }

    internal async Task SyncDeleteAsync(
      VssRequestPump.Processor processor,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer)
    {
      OperationTracer jtracer = new OperationTracer(processor, tracer, "Table delete: ", this.GetOperationTracerHeaderInfix(), this.overallPrefix);
      jtracer.Info("Started.");
      Task completion = (Task) null;
      CancellationToken ct = await processor.ExecuteWorkAsync<CancellationToken>((Func<IVssRequestContext, CancellationToken>) (context => context.CancellationToken)).ConfigureAwait(true);
      switch (this.ConcurrencyStrategy)
      {
        case ConcurrencyStrategy.PerStorageAccount:
          Func<Tuple<string, LoggerWrapper>, Task> action1 = (Func<Tuple<string, LoggerWrapper>, Task>) (async tuple =>
          {
            string str = tuple.Item1;
            DeleteStats orAdd = this.deleteStatsDict.GetOrAdd(str, (Func<string, DeleteStats>) (_ => new DeleteStats()));
            await this.DoSyncDeleteInternalAsync(processor, migrationEntry, tracer, str, tuple.Item2, orAdd).ConfigureAwait(true);
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions1.MaxDegreeOfParallelism = this.filteredTargetTables.Values.Count;
          dataflowBlockOptions1.CancellationToken = ct;
          ActionBlock<Tuple<string, LoggerWrapper>> actionBlock1 = NonSwallowingActionBlock.Create<Tuple<string, LoggerWrapper>>(action1, dataflowBlockOptions1);
          StorageMigration[] srcAsArray1 = this.GetSourceStorageAccountArray();
          foreach (ITable table in this.filteredTargetTables.Values)
          {
            string targetAccountName = table.StorageAccountName;
            actionBlock1.PostOrThrow<Tuple<string, LoggerWrapper>>(Tuple.Create<string, LoggerWrapper>(targetAccountName, await processor.ExecuteWorkAsync<LoggerWrapper>((Func<IVssRequestContext, LoggerWrapper>) (context => this.CreateTargetLogger(context, srcAsArray, targetAccountName))).ConfigureAwait(true)), ct);
          }
          actionBlock1.Complete();
          jtracer.Info("Queued all storage accounts.");
          completion = actionBlock1.Completion;
          break;
        case ConcurrencyStrategy.PerStorageAccountAndPrefix:
          Func<Tuple<string, LoggerWrapper, string>, Task> action2 = (Func<Tuple<string, LoggerWrapper, string>, Task>) (async tuple =>
          {
            string str = tuple.Item1;
            DeleteStats orAdd = this.deleteStatsDict.GetOrAdd(str, (Func<string, DeleteStats>) (_ => new DeleteStats()));
            await this.DoSyncDeleteInternalAsync(processor, migrationEntry, tracer, str, tuple.Item2, orAdd, tuple.Item3).ConfigureAwait(true);
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions2.MaxDegreeOfParallelism = this.settings.SyncDelete_OuterParallelism;
          dataflowBlockOptions2.CancellationToken = ct;
          ActionBlock<Tuple<string, LoggerWrapper, string>> actionBlock2 = NonSwallowingActionBlock.Create<Tuple<string, LoggerWrapper, string>>(action2, dataflowBlockOptions2);
          StorageMigration[] srcAsArray2 = this.GetSourceStorageAccountArray();
          foreach (string prefix in this.prefixList)
          {
            foreach (ITable table in this.filteredTargetTables.Values)
            {
              string targetAccountName = table.StorageAccountName;
              actionBlock2.PostOrThrow<Tuple<string, LoggerWrapper, string>>(Tuple.Create<string, LoggerWrapper, string>(targetAccountName, await processor.ExecuteWorkAsync<LoggerWrapper>((Func<IVssRequestContext, LoggerWrapper>) (context => this.CreateTargetLogger(context, srcAsArray, targetAccountName))).ConfigureAwait(true), prefix), ct);
            }
          }
          actionBlock2.Complete();
          jtracer.Info("Queued all storage account/prefix combinations.");
          completion = actionBlock2.Completion;
          break;
      }
      await completion;
      jtracer.Info("Completed.");
      jtracer = (OperationTracer) null;
      completion = (Task) null;
      ct = new CancellationToken();
    }

    internal async Task TableCopyAsync(
      VssRequestPump.Processor processor,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer)
    {
      OperationTracer jtracer = new OperationTracer(processor, tracer, "Table copy: ", this.GetOperationTracerHeaderInfix(), this.overallPrefix);
      jtracer.Info("Started.");
      Task completion = (Task) null;
      int parallelismLevel = await processor.ExecuteWorkAsync<int>((Func<IVssRequestContext, int>) (context => this.checkpointService.GetTableCopyParallelism(context, migrationEntry, this.vsoAreaPrefix, this.settings))).ConfigureAwait(true);
      ISqlConnectionInfo connectionInfo = await processor.ExecuteWorkAsync<ISqlConnectionInfo>((Func<IVssRequestContext, ISqlConnectionInfo>) (context => context.FrameworkConnectionInfo)).ConfigureAwait(true);
      switch (this.ConcurrencyStrategy)
      {
        case ConcurrencyStrategy.PerStorageAccount:
          Func<SourceTableStorage, Task> action1 = (Func<SourceTableStorage, Task>) (async sct =>
          {
            StorageMigrationLogger storageMigrationLogger = new StorageMigrationLogger(connectionInfo, sct.StorageMigration);
            CopyStats orAdd = this.copyStatsDict.GetOrAdd(sct.StorageAccountName, (Func<string, CopyStats>) (_ => new CopyStats()));
            await this.DoCopyInternalAsync(processor, migrationEntry, tracer, orAdd, sct.StorageAccountName, sct.Table, new Action<string>(storageMigrationLogger.LogMessage)).ConfigureAwait(true);
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions1.MaxDegreeOfParallelism = this.filteredSourceTables.Values.Count;
          dataflowBlockOptions1.CancellationToken = processor.CancellationToken;
          ActionBlock<SourceTableStorage> targetBlock1 = NonSwallowingActionBlock.Create<SourceTableStorage>(action1, dataflowBlockOptions1);
          foreach (SourceTableStorage input in this.filteredSourceTables.Values)
            targetBlock1.PostOrThrow<SourceTableStorage>(input, processor.CancellationToken);
          targetBlock1.Complete();
          jtracer.Info("Queued all SA/prefix combinations.");
          completion = targetBlock1.Completion;
          break;
        case ConcurrencyStrategy.PerStorageAccountAndPrefix:
          Func<Tuple<SourceTableStorage, string>, Task> action2 = (Func<Tuple<SourceTableStorage, string>, Task>) (async sct =>
          {
            StorageMigrationLogger storageMigrationLogger = new StorageMigrationLogger(connectionInfo, sct.Item1.StorageMigration);
            string storageAccountName = sct.Item1.StorageAccountName;
            ITable table = sct.Item1.Table;
            string prefix = sct.Item2;
            CopyStats orAdd = this.copyStatsDict.GetOrAdd(storageAccountName, (Func<string, CopyStats>) (_ => new CopyStats()));
            await this.DoCopyInternalAsync(processor, migrationEntry, tracer, orAdd, storageAccountName, table, new Action<string>(storageMigrationLogger.LogMessage), prefix).ConfigureAwait(true);
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions2.MaxDegreeOfParallelism = parallelismLevel;
          dataflowBlockOptions2.CancellationToken = processor.CancellationToken;
          ActionBlock<Tuple<SourceTableStorage, string>> targetBlock2 = NonSwallowingActionBlock.Create<Tuple<SourceTableStorage, string>>(action2, dataflowBlockOptions2);
          foreach (string prefix in this.prefixList)
          {
            foreach (SourceTableStorage sourceTableStorage in this.filteredSourceTables.Values)
              targetBlock2.PostOrThrow<Tuple<SourceTableStorage, string>>(Tuple.Create<SourceTableStorage, string>(sourceTableStorage, prefix), processor.CancellationToken);
          }
          targetBlock2.Complete();
          jtracer.Info("Queued all SA/prefix combinations.");
          completion = targetBlock2.Completion;
          break;
      }
      await completion;
      jtracer.Info("Completed.");
      if (!this.isInnerPrefixParallelismSet)
      {
        jtracer = (OperationTracer) null;
        completion = (Task) null;
      }
      else
      {
        using (List<ITrackingKey>.Enumerator enumerator = this.timeTrackingKeys.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.copyStartTimeTracker.Put(enumerator.Current, new DateTime?(this.m_startTime));
          jtracer = (OperationTracer) null;
          completion = (Task) null;
        }
      }
    }

    internal bool Initialize(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer,
      TableCopyDeleteCheckpointBehavior checkpointBehavior = TableCopyDeleteCheckpointBehavior.None)
    {
      tracer = tracer.Enter(nameof (AzureTableMigrator), nameof (Initialize));
      this.loader = this.GetTableLoader();
      this.isPremigration = migrationEntry.StorageOnly;
      this.settings = this.MigratingOperationSettings;
      this.checkpointService = deploymentContext.GetService<IAzureTableMigratorCheckpointService>();
      string parallelismSwitch = this.checkpointService.GetPrefixParallelismSwitch(deploymentContext, migrationEntry, this.vsoAreaPrefix);
      if (this.ConcurrencyStrategy.Equals((object) ConcurrencyStrategy.PerStorageAccountAndPrefix) && (string.IsNullOrEmpty(parallelismSwitch) ? 0 : (parallelismSwitch.Equals("True", StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0)
      {
        this.isInnerPrefixParallelismSet = true;
        this.settings.ParallelismForPrefixes = 16;
        this.settings.PerStorageAccountAndPrefix_MaxParallelism = 32 * this.settings.ParallelismForPrefixes;
        this.settings.SyncDelete_OuterParallelism = 32 * this.settings.ParallelismForPrefixes;
      }
      if (this.canUseCheckpoints)
      {
        this.settings.CopyInternalQueue_MaxParallelism = 1;
        this.settings.SyncDelete_InnerParallelism = 1;
      }
      bool flag = false;
      foreach (ShardingInfo shardingInfo in migrationEntry.ShardingInfo)
      {
        if (shardingInfo.StorageType == StorageType.Blob || shardingInfo.StorageType == StorageType.Table)
        {
          if (shardingInfo.VirtualNodes != 128)
            throw new ArgumentException(string.Format("Expected {0}.{1} to be {2}, ", (object) "ShardingInfo", (object) "VirtualNodeCount", (object) shardingInfo.VirtualNodes) + string.Format("but it's {0} for: {1}", (object) 128, (object) shardingInfo.ToString()));
          flag = true;
          break;
        }
      }
      if (!flag)
        throw new ArgumentException("Expected ShardingInfo with Blob, but none were found for: " + migrationEntry.ToString());
      List<TablePhysicalNode> nodes = new List<TablePhysicalNode>();
      IEnumerable<StorageMigration> source = this.GetTableMigrations(migrationEntry);
      if (!source.Any<StorageMigration>())
        return false;
      string tableName = (string) null;
      this.allSourceTables = this.filteredSourceTables = new Dictionary<string, SourceTableStorage>();
      foreach (StorageMigration storage in source)
      {
        nodes.Add(new TablePhysicalNode((Func<ITable>) (() => (ITable) null), (storage.IsSharded ? storage.StorageAccountName : throw new InvalidOperationException("Expected StorageMigration.IsSharded to be true, but it's false for: " + storage.ToString())) ?? throw new ArgumentException("Expected StorageMigration.StorageAccountName to be specified, but it's null for: " + storage.ToString())));
        if (string.IsNullOrEmpty(storage.SasToken))
          throw new ArgumentException("Expected StorageMigration.SasToken to be specified, but it's empty for: " + storage.ToString());
        this.allSourceTables.Add(storage.StorageAccountName, new SourceTableStorage()
        {
          StorageMigration = storage,
          StorageAccountName = storage.StorageAccountName,
          Table = this.loader.LoadSource(deploymentContext, storage)
        });
        if (tableName == null)
          tableName = storage.Id;
        else if (tableName != storage.Id)
          throw new ArgumentException("Expected all table names across the shard to be the same (the first was " + tableName + "), but StorageMigration.Id was " + storage.Id + " for: " + storage.ToString());
      }
      if (tableName == null)
        throw new ArgumentException(string.Format("Expected table name to have been found among the {0} {1}s, but {2}.{3} was null for all of them.", (object) source.Count<StorageMigration>(), (object) "StorageMigration", (object) "StorageMigration", (object) "Id"));
      this.sourceShardManager = new ConsistentHashShardManager<TablePhysicalNode>((IEnumerable<TablePhysicalNode>) nodes, 128);
      this.allTargetTables = this.filteredTargetTables = this.LoadTargetTables(deploymentContext, tableName);
      this.targetShardManager = new ConsistentHashShardManager<TablePhysicalNode>((IEnumerable<TablePhysicalNode>) this.allTargetTables.Keys.Select<string, TablePhysicalNode>((Func<string, TablePhysicalNode>) (acctName => new TablePhysicalNode((Func<ITable>) (() => (ITable) null), acctName))).ToList<TablePhysicalNode>(), 128);
      if (this.isDistributedBySA)
      {
        source = (IEnumerable<StorageMigration>) ParallelMigrationUtil.GetGroup<StorageMigrationBacked>(source.Select<StorageMigration, StorageMigrationBacked>((Func<StorageMigration, StorageMigrationBacked>) (c => new StorageMigrationBacked(c))).ToList<StorageMigrationBacked>(), (IGroupIndexable) this.csetting, true).Select<StorageMigrationBacked, StorageMigration>((Func<StorageMigrationBacked, StorageMigration>) (ic => ic.StorageMigration)).ToList<StorageMigration>();
        HashSet<string> srcSet = source.Select<StorageMigration, string>((Func<StorageMigration, string>) (sm => sm.StorageAccountName)).ToHashSet<string>();
        this.filteredSourceTables = this.allSourceTables.Where<KeyValuePair<string, SourceTableStorage>>((Func<KeyValuePair<string, SourceTableStorage>, bool>) (kvp => srcSet.Contains(kvp.Key))).ToDictionary<KeyValuePair<string, SourceTableStorage>, string, SourceTableStorage>((Func<KeyValuePair<string, SourceTableStorage>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, SourceTableStorage>, SourceTableStorage>) (kvp => kvp.Value));
        this.filteredTargetTables = ParallelMigrationUtil.GetGroup<TableBackedStorageMigration>(this.allTargetTables.ToList<KeyValuePair<string, ITable>>().Select<KeyValuePair<string, ITable>, TableBackedStorageMigration>((Func<KeyValuePair<string, ITable>, TableBackedStorageMigration>) (t => new TableBackedStorageMigration(t.Key, t.Value))).ToList<TableBackedStorageMigration>(), (IGroupIndexable) this.csetting, true).ToDictionary<TableBackedStorageMigration, string, ITable>((Func<TableBackedStorageMigration, string>) (tbsm => tbsm.VsoArea), (Func<TableBackedStorageMigration, ITable>) (tbsm => tbsm.Table));
      }
      this.copyStatsDict = new ConcurrentDictionary<string, CopyStats>();
      this.deleteStatsDict = new ConcurrentDictionary<string, DeleteStats>();
      this.timeTrackingKeys = new List<ITrackingKey>();
      if (this.isDistributedByPrefix)
      {
        this.copyStartTimeTracker = new TableMigrationTimeTracker(TimeTrackingKeyType.Prefix);
        foreach (string prefix in this.GetPrefixes(deploymentContext))
          this.timeTrackingKeys.Add((ITrackingKey) new PrefixTrackingKey(this.overallPrefix + prefix));
      }
      else if (this.isDistributedBySA)
      {
        this.copyStartTimeTracker = new TableMigrationTimeTracker(TimeTrackingKeyType.StorageAccount);
        foreach (string key in this.filteredSourceTables.Keys)
          this.timeTrackingKeys.Add((ITrackingKey) new StorageAccountTrackingKey(key));
      }
      else
      {
        this.globalLastIncrementalStartTime = this.checkpointService.GetIncrementalCopyStartTime(deploymentContext, migrationEntry, this.vsoAreaPrefix, this.c_globalCopyStartTimeKey);
        this.copyStartTimeTracker = new TableMigrationTimeTracker(TimeTrackingKeyType.StorageAccount);
        foreach (StorageMigration storageMigration in source)
          this.timeTrackingKeys.Add((ITrackingKey) new StorageAccountTrackingKey(storageMigration.StorageAccountName));
      }
      foreach (ITrackingKey timeTrackingKey in this.timeTrackingKeys)
      {
        DateTime? dt = this.checkpointService.GetIncrementalCopyStartTime(deploymentContext, migrationEntry, this.vsoAreaPrefix, timeTrackingKey);
        if (!dt.HasValue)
          dt = this.globalLastIncrementalStartTime;
        this.copyStartTimeTracker.Put(timeTrackingKey, dt);
      }
      this.checkpointService.SetTableSyncDeleteParallelism(deploymentContext, migrationEntry, this.vsoAreaPrefix, this.settings);
      this.prefixList = this.GetPrefixList(deploymentContext);
      this.copyTaskCompletions = new ConcurrentDictionary<string, Dictionary<string, bool>>();
      foreach (string key in this.filteredSourceTables.Keys)
      {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        this.copyTaskCompletions.TryAdd(key, dictionary);
        foreach (string prefix in this.prefixList)
          dictionary.Add(prefix, false);
      }
      StringBuilder stringBuilder = new StringBuilder("Config: ");
      stringBuilder.Append("Checkpoints=");
      stringBuilder.Append(this.canUseCheckpoints ? "On" : "Off");
      stringBuilder.Append(";InnerPrefixParallelism=");
      stringBuilder.Append(this.isInnerPrefixParallelismSet ? "On" : "Off");
      string str;
      int num1;
      int num2;
      switch (this.ConcurrencyStrategy)
      {
        case ConcurrencyStrategy.PerStorageAccount:
          str = "AccountOnly";
          num1 = this.filteredSourceTables.Values.Count;
          num2 = this.filteredTargetTables.Values.Count;
          break;
        case ConcurrencyStrategy.PerStorageAccountAndPrefix:
          str = "AccountAndPrefix";
          num1 = this.settings.PerStorageAccountAndPrefix_MaxParallelism;
          num2 = this.settings.SyncDelete_OuterParallelism;
          break;
        default:
          throw new InvalidOperationException("Unrecognized concurrency strategy: " + this.ConcurrencyStrategy.ToString());
      }
      stringBuilder.Append(";ConcurrencyStrategy=");
      stringBuilder.Append(str);
      stringBuilder.Append(";CopyConcurrency(Outer/Inner)=");
      stringBuilder.Append(num1);
      stringBuilder.Append("/");
      stringBuilder.Append(this.settings.CopyInternalQueue_MaxParallelism);
      stringBuilder.Append(";DeleteConcurrency(Outer/Inner)=");
      stringBuilder.Append(num2);
      stringBuilder.Append("/");
      stringBuilder.Append(this.settings.SyncDelete_InnerParallelism);
      tracer.Info(deploymentContext, 197500, stringBuilder.ToString());
      return true;
    }

    protected virtual void MarkMigrationAsComplete(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry)
    {
      IDictionary<string, StorageMigration> dictionary;
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        dictionary = (IDictionary<string, StorageMigration>) component.QueryContainerMigrationsById(migrationEntry.MigrationId).Where<StorageMigration>(new Func<StorageMigration, bool>(this.IsShardTableWithRightPrefix)).ToDictionary<StorageMigration, string>((Func<StorageMigration, string>) (sm => sm.VsoArea));
      IEnumerable<StorageMigration> source = this.GetTableMigrations(migrationEntry);
      if (this.isDistributedBySA)
        source = source.Where<StorageMigration>((Func<StorageMigration, bool>) (sm => this.filteredSourceTables.ContainsKey(sm.StorageAccountName)));
      List<StorageMigration> list1 = source.ToList<StorageMigration>();
      DeleteStats delete = (DeleteStats) null;
      if (this.performSyncDelete)
      {
        delete = new DeleteStats();
        List<string> list2 = this.filteredTargetTables.Keys.ToList<string>();
        int count = list1.Count;
        if (count > 0)
        {
          foreach (string key in list2)
          {
            DeleteStats deleteStats;
            if (this.deleteStatsDict.TryGetValue(key, out deleteStats))
            {
              delete.totalProcessed += deleteStats.totalProcessed;
              delete.totalBatches += deleteStats.totalBatches;
              delete.totalDeleted += deleteStats.totalDeleted;
            }
          }
          delete = new DeleteStats()
          {
            totalProcessed = delete.totalProcessed / (long) count,
            totalBatches = delete.totalBatches / (long) count,
            totalDeleted = delete.totalDeleted / (long) count
          };
        }
      }
      foreach (StorageMigration container in list1)
      {
        string storageAccountName = container.StorageAccountName;
        TableMigrationResult tableMigrationResult = TableMigrationResult.ReadFromStatus(container, dictionary);
        CopyStats copyStats;
        tableMigrationResult.Merge(this.performTableCopy ? (this.copyStatsDict.TryGetValue(storageAccountName, out copyStats) ? copyStats : (CopyStats) null) : (CopyStats) null, delete);
        if (tableMigrationResult.Status != StorageMigrationStatus.Failed)
          tableMigrationResult.Status = StorageMigrationStatus.Completed;
        tableMigrationResult.UpdateDatabase(deploymentContext);
      }
    }

    protected virtual void UpdateMigrationStatus(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      StorageMigrationStatus status,
      string description)
    {
      IEnumerable<StorageMigration> source = this.GetTableMigrations(migrationEntry);
      if (this.isDistributedBySA && this.filteredSourceTables != null && this.filteredSourceTables.Count > 0)
        source = source.Where<StorageMigration>((Func<StorageMigration, bool>) (sm => this.filteredSourceTables.ContainsKey(sm.StorageAccountName)));
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        component.UpdateContainerMigrationStatus(source.ToList<StorageMigration>(), status, description);
    }

    protected virtual Dictionary<string, ITable> LoadTargetTables(
      IVssRequestContext deploymentContext,
      string tableName)
    {
      List<Tuple<string, ITable>> tupleList = new List<Tuple<string, ITable>>();
      using (IDisposableReadOnlyList<BlobMigrationExtension> extensions = deploymentContext.GetExtensions<BlobMigrationExtension>())
      {
        foreach (BlobMigrationExtension migrationExtension in (IEnumerable<BlobMigrationExtension>) extensions)
        {
          foreach (MigrateStorageInfo storageInfo in migrationExtension.GetStorageInfos(deploymentContext))
          {
            if (storageInfo.Name.StartsWith(this.vsoAreaPrefix))
            {
              ITable ct = this.loader.LoadTarget(deploymentContext, storageInfo, tableName);
              VssRequestPump.Processor processor = VssRequestPump.Processor.CreateWithoutRequestContext(deploymentContext.CancellationToken, deploymentContext.ActivityId, deploymentContext.E2EId, deploymentContext.UniqueIdentifier);
              AsyncPump.Run<bool>((Func<Task<bool>>) (() => ct.CreateIfNotExistsAsync(processor)));
              string accountNameFromVsoArea = this.ExtractAccountNameFromVSOArea(storageInfo.Name, this.vsoAreaPrefix);
              tupleList.Add(Tuple.Create<string, ITable>(accountNameFromVsoArea, ct));
            }
          }
        }
      }
      Dictionary<string, ITable> dictionary = new Dictionary<string, ITable>(tupleList.Count);
      foreach (Tuple<string, ITable> tuple in tupleList)
        dictionary[tuple.Item1] = tuple.Item2;
      return dictionary;
    }

    internal LoggerWrapper CreateTargetLogger(
      IVssRequestContext deploymentContext,
      StorageMigration[] srcAsArray,
      string targetAccountName)
    {
      try
      {
        int length = srcAsArray.Length;
        if (length > 0)
        {
          Match match = this.c_tableIndexRegex.Match(targetAccountName);
          if (match.Success)
          {
            int index = int.Parse(match.Groups[1].Value) % length;
            return new LoggerWrapper(new Action<string>(this.CreateStorageMigrationLogger(deploymentContext.FrameworkConnectionInfo, srcAsArray[index]).LogMessage), targetAccountName);
          }
        }
      }
      catch
      {
      }
      return new LoggerWrapper((Action<string>) (msg => { }));
    }

    internal StorageMigration[] GetSourceStorageAccountArray()
    {
      try
      {
        int count = this.filteredSourceTables.Count;
        int length = count + 1;
        StorageMigration[] sourceArray = new StorageMigration[length];
        foreach (SourceTableStorage sourceTableStorage in this.filteredSourceTables.Values)
        {
          Match match = this.c_tableIndexRegex.Match(sourceTableStorage.StorageAccountName);
          if (!match.Success)
            return Array.Empty<StorageMigration>();
          int index = int.Parse(match.Groups[1].Value);
          if (index < 0 || index >= length)
            return Array.Empty<StorageMigration>();
          if (sourceArray[index] != null)
            return Array.Empty<StorageMigration>();
          sourceArray[index] = sourceTableStorage.StorageMigration;
        }
        int sourceIndex = 0;
        for (int index = 0; index < length; ++index)
        {
          if (sourceArray[index] == null)
          {
            if (index == 0)
              sourceIndex = 1;
            else if (index < count)
              return Array.Empty<StorageMigration>();
          }
        }
        StorageMigration[] destinationArray = new StorageMigration[count];
        Array.Copy((Array) sourceArray, sourceIndex, (Array) destinationArray, 0, count);
        return destinationArray;
      }
      catch
      {
        return Array.Empty<StorageMigration>();
      }
    }

    private IEnumerable<string> GetPrefixList(IVssRequestContext context)
    {
      Random random = new Random();
      this.prefixList = this.GetPrefixes(context);
      this.prefixList = !this.settings.PerStorageAccountAndPrefix_Randomness ? (IEnumerable<string>) this.prefixList.ToList<string>() : this.prefixList.Select<string, Tuple<string, int>>((Func<string, Tuple<string, int>>) (prefix => Tuple.Create<string, int>(prefix, random.Next()))).OrderBy<Tuple<string, int>, int>((Func<Tuple<string, int>, int>) (t => t.Item2)).Select<Tuple<string, int>, string>((Func<Tuple<string, int>, string>) (t => t.Item1));
      return this.prefixList;
    }

    private bool CanTableCopyDeleteCheckpointBeSet(
      EntitiesProcessed entitiesProcessed,
      int entryCount)
    {
      entitiesProcessed.count += (long) entryCount;
      long num = entitiesProcessed.count >> this.settings.ExponentOfTwoForPartitionKeyCheckpoint;
      if (this.checkpointBehavior.Equals((object) TableCopyDeleteCheckpointBehavior.None) || num <= entitiesProcessed.quotient)
        return false;
      entitiesProcessed.quotient = num;
      return true;
    }

    private IEnumerable<StorageMigration> GetTableMigrations(TargetHostMigration migrationEntry) => migrationEntry.GetAzureTables().Where<StorageMigration>(new Func<StorageMigration, bool>(this.IsShardTableWithRightPrefix));

    public string ExtractAccountNameFromVSOArea(string vsoArea, string vsoAreaPrefix) => vsoArea.Substring(vsoAreaPrefix.Length + 1);

    private bool IsShardTableWithRightPrefix(StorageMigration sm) => sm.IsSharded && sm.StorageType == StorageType.Table && sm.VsoArea.StartsWith(this.vsoAreaPrefix);

    private string GetOperationTracerHeaderInfix()
    {
      if (this.isDistributedByPrefix)
        return "All";
      return !this.isDistributedBySA ? (string) null : string.Format("{0}/{1}", (object) this.csetting.GroupIndex, (object) this.csetting.TotalGroups);
    }

    private async Task DoSyncDeleteInternalAsync(
      VssRequestPump.Processor processor,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer,
      string targetAccountName,
      LoggerWrapper logger,
      DeleteStats stats,
      string prefix = "")
    {
      if (this.isDistributedByPrefix)
        prefix = this.overallPrefix + prefix;
      AccountPrefixTracer aptracer = new AccountPrefixTracer(processor, tracer, "Table delete: ", targetAccountName, prefix);
      aptracer.Info("Started.");
      ITable targetCloudTable = this.filteredTargetTables[targetAccountName];
      targetCloudTable.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromMilliseconds(1.0), 4);
      string name = targetCloudTable.Name;
      long totalAgg = 0;
      Func<PartitionDetailsForSyncDelete, Task> action = (Func<PartitionDetailsForSyncDelete, Task>) (async targetPartition =>
      {
        string partitionKey = targetPartition.PartitionKey;
        Query<TableEntity> query = (Query<TableEntity>) new RowRangeQuery<TableEntity>((StringColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(partitionKey), (RangeFilter<RowKeyColumn>) null);
        ITableQueryContinuationToken sourceContinuationToken = (ITableQueryContinuationToken) null;
        SourceTableStorage sourceTable = this.allSourceTables[this.GetShardName(this.sourceShardManager, partitionKey)];
        HashSet<string> rowsToDelete = targetPartition.RowsToDelete;
        while (!this.m_faulted)
        {
          IResultSegment<TableEntity> sourceResultSegment;
          try
          {
            sourceResultSegment = await sourceTable.Table.ExecuteQuerySegmentedAsync<TableEntity>(processor, query, sourceContinuationToken, this.tableRequestOptions);
          }
          catch (Exception ex)
          {
            sourceResultSegment = await sourceTable.Table.ExecuteQuerySegmentedAsync<TableEntity>(processor, query, sourceContinuationToken, this.tableRequestOptions);
          }
          sourceContinuationToken = sourceResultSegment.ContinuationToken;
          foreach (TableEntity result in sourceResultSegment.Results)
            rowsToDelete.Remove(result.RowKey);
          sourceResultSegment = (IResultSegment<TableEntity>) null;
          if (sourceContinuationToken == null)
          {
            if (!rowsToDelete.Any<string>())
            {
              query = (Query<TableEntity>) null;
              sourceContinuationToken = (ITableQueryContinuationToken) null;
              sourceTable = (SourceTableStorage) null;
              rowsToDelete = (HashSet<string>) null;
              return;
            }
            int num = 0;
            foreach (List<string> pageOfDeletes in rowsToDelete.GetPages<string>(50))
            {
              if (this.m_faulted)
                throw new OperationCanceledException("A previous exception has faulted the entire operation.");
              TableBatchOperationDescriptor batch = new TableBatchOperationDescriptor();
              foreach (string rowKey in pageOfDeletes)
                batch.Delete((ITableEntity) new TableEntity(partitionKey, rowKey)
                {
                  ETag = "*"
                });
              try
              {
                TableBatchOperationResult batchOperationResult = await targetCloudTable.ExecuteBatchAsync(processor, batch, this.tableRequestOptions);
                num += pageOfDeletes.Count;
                if (this.CanTableCopyDeleteCheckpointBeSet(targetPartition.EntitiesProcessed, batch.Count))
                  await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => this.checkpointService.SetPartitionKey(context, migrationEntry, targetAccountName, "DoSyncDeleteInternal", prefix, partitionKey, this.vsoAreaPrefix)));
                Interlocked.Add(ref stats.totalDeleted, (long) pageOfDeletes.Count);
              }
              catch (Exception ex1)
              {
                foreach (string rowKey in pageOfDeletes)
                {
                  try
                  {
                    TableOperationResult tableOperationResult = await targetCloudTable.ExecuteAsync(processor, TableOperationDescriptor.Delete((ITableEntity) new TableEntity(partitionKey, rowKey)
                    {
                      ETag = "*"
                    }), this.tableRequestOptions);
                    ++num;
                  }
                  catch (Exception ex2)
                  {
                  }
                }
              }
              batch = (TableBatchOperationDescriptor) null;
            }
            query = (Query<TableEntity>) null;
            sourceContinuationToken = (ITableQueryContinuationToken) null;
            sourceTable = (SourceTableStorage) null;
            rowsToDelete = (HashSet<string>) null;
            return;
          }
        }
        throw new OperationCanceledException("A previous exception has faulted the entire operation.");
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = this.settings.InternalActionBlockCapacity;
      dataflowBlockOptions.MaxDegreeOfParallelism = this.settings.SyncDelete_InnerParallelism;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.SingleProducerConstrained = true;
      ActionBlock<PartitionDetailsForSyncDelete> queue = NonSwallowingActionBlock.Create<PartitionDetailsForSyncDelete>(action, dataflowBlockOptions);
      string checkpoint = (string) null;
      if (this.checkpointBehavior.Equals((object) TableCopyDeleteCheckpointBehavior.LoadAndSave))
        checkpoint = await processor.ExecuteWorkAsync<string>((Func<IVssRequestContext, string>) (context => this.checkpointService.GetPartitionKey(context, migrationEntry, targetAccountName, "DoSyncDeleteInternal", prefix, this.vsoAreaPrefix)));
      Query<TableEntity> targetQuery = (Query<TableEntity>) new PartitionRangeRowRangeQuery<TableEntity>(this.GetPartitionKeyFilter(checkpoint, prefix), (RangeFilter<RowKeyColumn>) null);
      ITableQueryContinuationToken targetContinuationToken = (ITableQueryContinuationToken) null;
      PartitionDetailsForSyncDelete currentPartition = (PartitionDetailsForSyncDelete) null;
      Task<IResultSegment<TableEntity>> nextSegmentTask = targetCloudTable.ExecuteQuerySegmentedAsync<TableEntity>(processor, targetQuery, targetContinuationToken, this.tableRequestOptions);
      EntitiesProcessed entitiesProcessed = new EntitiesProcessed();
      do
      {
        IResultSegment<TableEntity> resultSegment = await nextSegmentTask;
        targetContinuationToken = resultSegment.ContinuationToken;
        nextSegmentTask = targetCloudTable.ExecuteQuerySegmentedAsync<TableEntity>(processor, targetQuery, targetContinuationToken, this.tableRequestOptions);
        foreach (TableEntity result in resultSegment.Results)
        {
          TableEntity targetRow = result;
          if (currentPartition == null)
            currentPartition = new PartitionDetailsForSyncDelete(targetRow.PartitionKey, new HashSet<string>(), entitiesProcessed);
          if (targetRow.PartitionKey == currentPartition.PartitionKey)
          {
            currentPartition.RowsToDelete.Add(targetRow.RowKey);
          }
          else
          {
            if (this.GetShardName(this.targetShardManager, currentPartition.PartitionKey) == targetAccountName)
            {
              await queue.SendOrThrowSingleBlockNetworkAsync<PartitionDetailsForSyncDelete>(currentPartition, processor.CancellationToken);
              Interlocked.Add(ref stats.totalProcessed, (long) currentPartition.RowsToDelete.Count);
              Interlocked.Increment(ref stats.totalBatches);
            }
            long num = stats.totalProcessed >> 14;
            if (num > totalAgg)
            {
              totalAgg = num;
              aptracer.InfoOnAccount(string.Format("Processed={0}({1} batches), Deleted={2}", (object) stats.totalProcessed, (object) stats.totalBatches, (object) stats.totalDeleted));
            }
            string partitionKey = targetRow.PartitionKey;
            HashSet<string> rowsToDelete = new HashSet<string>();
            rowsToDelete.Add(targetRow.RowKey);
            EntitiesProcessed entitiesProcessed1 = entitiesProcessed;
            currentPartition = new PartitionDetailsForSyncDelete(partitionKey, rowsToDelete, entitiesProcessed1);
            targetRow = (TableEntity) null;
          }
        }
      }
      while (!processor.CancellationToken.IsCancellationRequested && targetContinuationToken != null);
      if (!processor.CancellationToken.IsCancellationRequested && currentPartition != null && currentPartition.RowsToDelete.Any<string>())
        await queue.SendOrThrowSingleBlockNetworkAsync<PartitionDetailsForSyncDelete>(currentPartition, processor.CancellationToken);
      queue.Complete();
      await queue.Completion;
      logger.Info("Completed sync-deletion");
      aptracer.Info("Completed. " + string.Format("Processed={0}({1} batches), Deleted={2}", (object) stats.totalProcessed, (object) stats.totalBatches, (object) stats.totalDeleted));
      aptracer = (AccountPrefixTracer) null;
      queue = (ActionBlock<PartitionDetailsForSyncDelete>) null;
      targetQuery = (Query<TableEntity>) null;
      targetContinuationToken = (ITableQueryContinuationToken) null;
      currentPartition = (PartitionDetailsForSyncDelete) null;
      nextSegmentTask = (Task<IResultSegment<TableEntity>>) null;
      entitiesProcessed = (EntitiesProcessed) null;
    }

    protected virtual TimeSpan GetMaxClockSkewOffset() => TimeSpan.FromMinutes(5.0);

    private async Task DoCopyInternalAsync(
      VssRequestPump.Processor processor,
      TargetHostMigration migrationEntry,
      MigrationTracer tracer,
      CopyStats stats,
      string sourceAccountName,
      ITable cloudTable,
      Action<string> log,
      string prefix = "")
    {
      ITrackingKey key;
      if (this.isDistributedByPrefix)
      {
        prefix = this.overallPrefix + prefix;
        key = (ITrackingKey) new PrefixTrackingKey(prefix);
      }
      else
        key = (ITrackingKey) new StorageAccountTrackingKey(sourceAccountName);
      AccountPrefixTracer aptracer = new AccountPrefixTracer(processor, tracer, "Table copy: ", sourceAccountName, prefix);
      aptracer.Info("Started.");
      LoggerWrapper logger = new LoggerWrapper(log, this.isPremigration ? (string) null : sourceAccountName);
      cloudTable.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromMilliseconds(1.0), 4);
      string currentPartitonKey = "<dummy>";
      TableBatchOperationDescriptor batch = (TableBatchOperationDescriptor) null;
      long totalAgg = 0;
      string checkpoint = (string) null;
      if (this.checkpointBehavior.Equals((object) TableCopyDeleteCheckpointBehavior.LoadAndSave))
        checkpoint = await processor.ExecuteWorkAsync<string>((Func<IVssRequestContext, string>) (context => this.checkpointService.GetPartitionKey(context, migrationEntry, sourceAccountName, "DoCopyInternal", prefix, this.vsoAreaPrefix)));
      RangeFilter<PartitionKeyColumn> partitionKeyFilter = this.GetPartitionKeyFilter(checkpoint, prefix);
      DateTime? nullable = this.copyStartTimeTracker.Get(key);
      ComparisonFilter<TimestampColumn> nonUserColumnFilter = (ComparisonFilter<TimestampColumn>) null;
      if (nullable.HasValue)
        nonUserColumnFilter = new ComparisonFilter<TimestampColumn>((IColumnValue<TimestampColumn>) new TimestampColumnValue(nullable.Value.ToUniversalTime() - this.GetMaxClockSkewOffset()), ComparisonOperator.GreaterThanOrEqual);
      PartitionRangeRowRangeQuery<DynamicTableEntity> query = new PartitionRangeRowRangeQuery<DynamicTableEntity>(partitionKeyFilter, (RangeFilter<RowKeyColumn>) null, (IFilter<INonUserColumn>) nonUserColumnFilter);
      EntitiesProcessed entitiesProcessed = new EntitiesProcessed();
      Func<Tuple<string, TableBatchOperationDescriptor>, Task> action = (Func<Tuple<string, TableBatchOperationDescriptor>, Task>) (parKeyAndBatchOperation => this.CompleteCurrentBatchAsync(processor, migrationEntry, sourceAccountName, entitiesProcessed, parKeyAndBatchOperation.Item2, parKeyAndBatchOperation.Item1, stats, logger, prefix));
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = this.settings.CopyInternalQueue_MaxParallelism;
      dataflowBlockOptions.BoundedCapacity = this.settings.InternalActionBlockCapacity;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.SingleProducerConstrained = true;
      ActionBlock<Tuple<string, TableBatchOperationDescriptor>> queue = NonSwallowingActionBlock.Create<Tuple<string, TableBatchOperationDescriptor>>(action, dataflowBlockOptions);
      Task<IResultSegment<DynamicTableEntity>> nextSegmentTask = cloudTable.ExecuteQuerySegmentedAsync<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) query, (ITableQueryContinuationToken) null, this.tableRequestOptions);
      ITableQueryContinuationToken continuationToken;
      do
      {
        CancellationToken cancellationToken = processor.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();
        if (this.m_faulted)
          throw new OperationCanceledException("A previous exception has faulted the entire operation.");
        IResultSegment<DynamicTableEntity> resultSegment;
        try
        {
          resultSegment = await nextSegmentTask;
        }
        catch (OperationCanceledException ex)
        {
          throw;
        }
        catch (StorageException ex)
        {
          if (!ex.HasHttpStatus(HttpStatusCode.NotFound))
          {
            logger.Error(string.Format("Encountered an error when querying table. {0} retries were executed. Error: {1}", (object) 4, (object) ex.Message));
            this.m_faulted = true;
            throw;
          }
          else
            break;
        }
        catch (Exception ex)
        {
          logger.Error(string.Format("Encountered an error when querying table. {0} retries were executed. Error: {1}", (object) 4, (object) ex.Message));
          this.m_faulted = true;
          throw;
        }
        nextSegmentTask = cloudTable.ExecuteQuerySegmentedAsync<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) query, resultSegment.ContinuationToken, this.tableRequestOptions);
        foreach (DynamicTableEntity result in resultSegment.Results)
        {
          DynamicTableEntity entity = result;
          cancellationToken = processor.CancellationToken;
          cancellationToken.ThrowIfCancellationRequested();
          Interlocked.Increment(ref stats.totalProcessed);
          if (entity.PartitionKey == currentPartitonKey)
          {
            if (batch != null)
            {
              if (batch.Count < this.settings.BatchSizeForTableCopyAsync)
              {
                batch.InsertOrReplace((ITableEntity) entity);
                continue;
              }
            }
            else
              continue;
          }
          await queue.SendOrThrowSingleBlockNetworkAsync<Tuple<string, TableBatchOperationDescriptor>>(Tuple.Create<string, TableBatchOperationDescriptor>(currentPartitonKey, batch), processor.CancellationToken);
          batch = (TableBatchOperationDescriptor) null;
          currentPartitonKey = entity.PartitionKey;
          if (!this.ShouldProcessPartitionKey(currentPartitonKey))
          {
            logger.Warn("Ignoring table entry with PartitionKey " + entity.PartitionKey);
            Interlocked.Increment(ref stats.totalIgnored);
          }
          else
          {
            string shardName = this.GetShardName(this.sourceShardManager, currentPartitonKey);
            if (shardName != sourceAccountName)
            {
              logger.Warn("Blob with PK " + currentPartitonKey + " mismatches shard, Source: " + sourceAccountName + ", MappedShard: " + shardName);
              Interlocked.Increment(ref stats.totalIgnored);
            }
            else
            {
              batch = new TableBatchOperationDescriptor();
              batch.InsertOrReplace((ITableEntity) entity);
              entity = (DynamicTableEntity) null;
            }
          }
        }
        continuationToken = resultSegment.ContinuationToken;
        long num = stats.totalProcessed >> 14;
        if (num > totalAgg)
        {
          totalAgg = num;
          string msg = string.Format("Processed={0}({1} batches), Copied={2}, Failed={3}, Ignored={4}", (object) stats.totalProcessed, (object) stats.totalBatches, (object) stats.totalSuccess, (object) stats.totalFailure, (object) stats.totalIgnored);
          logger.Info(msg);
          aptracer.InfoOnAccount(msg);
        }
        resultSegment = (IResultSegment<DynamicTableEntity>) null;
      }
      while (continuationToken != null);
      processor.CancellationToken.ThrowIfCancellationRequested();
      await queue.SendOrThrowSingleBlockNetworkAsync<Tuple<string, TableBatchOperationDescriptor>>(Tuple.Create<string, TableBatchOperationDescriptor>(currentPartitonKey, batch), processor.CancellationToken);
      queue.Complete();
      await queue.Completion;
      if (!this.isInnerPrefixParallelismSet)
      {
        bool flag = false;
        switch (this.ConcurrencyStrategy)
        {
          case ConcurrencyStrategy.PerStorageAccount:
            flag = true;
            break;
          case ConcurrencyStrategy.PerStorageAccountAndPrefix:
            Dictionary<string, bool> dictionary;
            if (this.copyTaskCompletions.TryGetValue(sourceAccountName, out dictionary))
            {
              lock (this.copyTaskCompletions)
              {
                dictionary[prefix] = true;
                flag = dictionary.Values.All<bool>((Func<bool, bool>) (b => b));
                break;
              }
            }
            else
              break;
          default:
            throw new InvalidOperationException("Invalid ConcurrencyStrategy encountered");
        }
        if (flag)
          this.copyStartTimeTracker.Put(key, new DateTime?(this.m_startTime));
      }
      string str = string.Format("Processed={0}({1} batches), Copied={2}, Failed={3}, Ignored={4}", (object) stats.totalProcessed, (object) stats.totalBatches, (object) stats.totalSuccess, (object) stats.totalFailure, (object) stats.totalIgnored);
      logger.Info("(COMPLETED) " + str);
      aptracer.Info("Completed. " + str);
      key = (ITrackingKey) null;
      aptracer = (AccountPrefixTracer) null;
      currentPartitonKey = (string) null;
      batch = (TableBatchOperationDescriptor) null;
      query = (PartitionRangeRowRangeQuery<DynamicTableEntity>) null;
      queue = (ActionBlock<Tuple<string, TableBatchOperationDescriptor>>) null;
      nextSegmentTask = (Task<IResultSegment<DynamicTableEntity>>) null;
    }

    private async Task CompleteCurrentBatchAsync(
      VssRequestPump.Processor processor,
      TargetHostMigration migrationEntry,
      string sourceAccountName,
      EntitiesProcessed entProcessed,
      TableBatchOperationDescriptor batch,
      string partitonKey,
      CopyStats stats,
      LoggerWrapper logger,
      string prefix = "")
    {
      if (batch == null)
        ;
      else if (!batch.Any<TableOperationDescriptor>())
        ;
      else
      {
        ITable allTargetTable = this.allTargetTables[this.GetShardName(this.targetShardManager, partitonKey)];
        allTargetTable.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromMilliseconds(1.0), 4);
        try
        {
          (await allTargetTable.ExecuteBatchAsync(processor, batch, this.tableRequestOptions)).Match((Action<IList<TableOperationResult>>) (tor =>
          {
            if (tor != null && tor.Any<TableOperationResult>((Func<TableOperationResult, bool>) (r => r.HttpStatusCode >= HttpStatusCode.MultipleChoices)))
            {
              string str = string.Format("Saw at least one error when executing batch on {0}. First error code: {1}.", (object) partitonKey, (object) tor.First<TableOperationResult>((Func<TableOperationResult, bool>) (r => r.HttpStatusCode >= HttpStatusCode.MultipleChoices)).HttpStatusCode);
              logger.Error(str);
              this.m_faulted = true;
              Interlocked.Increment(ref stats.totalFailure);
              throw new Exception(str);
            }
            Interlocked.Increment(ref stats.totalBatches);
            Interlocked.Add(ref stats.totalSuccess, (long) batch.Count);
          }), (Action<TableBatchOperationResult.Error>) (err =>
          {
            string str = "Saw an error when executing batch on " + partitonKey + ". Error code: " + err.ErrorCode + "; error message: " + err.Exception?.Message + ".";
            logger.Error(str);
            this.m_faulted = true;
            Interlocked.Increment(ref stats.totalFailure);
            throw new Exception(str);
          }));
          if (this.CanTableCopyDeleteCheckpointBeSet(entProcessed, batch.Count))
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => this.checkpointService.SetPartitionKey(context, migrationEntry, sourceAccountName, "DoCopyInternal", prefix, partitonKey, this.vsoAreaPrefix)));
        }
        catch (Exception ex)
        {
          StorageException storageException = this.DrillToStorageException(ex);
          int num1;
          if (storageException == null)
          {
            num1 = 0;
          }
          else
          {
            int? httpStatusCode = storageException.RequestInformation?.HttpStatusCode;
            int num2 = 413;
            num1 = httpStatusCode.GetValueOrDefault() == num2 & httpStatusCode.HasValue ? 1 : 0;
          }
          if (num1 != 0)
          {
            int count = batch.Count;
            if (count <= 1)
              throw;
            int num3 = count / 2;
            TableBatchOperationDescriptor batch1 = new TableBatchOperationDescriptor(batch.GetRange(0, num3));
            TableBatchOperationDescriptor batch2 = new TableBatchOperationDescriptor(batch.GetRange(num3, count - num3));
            await Task.WhenAll(this.CompleteCurrentBatchAsync(processor, migrationEntry, sourceAccountName, entProcessed, batch1, partitonKey, stats, logger), this.CompleteCurrentBatchAsync(processor, migrationEntry, sourceAccountName, entProcessed, batch2, partitonKey, stats, logger));
          }
          else
            throw;
        }
      }
    }

    private StorageException DrillToStorageException(Exception e)
    {
      switch (e)
      {
        case StorageException storageException1:
          return storageException1;
        case AggregateException aggregateException:
          foreach (Exception innerException in aggregateException.InnerExceptions)
          {
            StorageException storageException = this.DrillToStorageException(innerException);
            if (storageException != null)
              return storageException;
          }
          return (StorageException) null;
        case null:
          return (StorageException) null;
        default:
          return this.DrillToStorageException(e.InnerException);
      }
    }

    private RangeFilter<PartitionKeyColumn> GetPartitionRangeQueryFilterForPrefix(
      string prefixFromCheckpoint,
      string prefix)
    {
      return new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(prefixFromCheckpoint), RangeBoundaryType.Inclusive), new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(prefix + "~"), RangeBoundaryType.Exclusive));
    }

    private RangeFilter<PartitionKeyColumn> GetPartitionKeyFilter(string checkpoint, string prefix) => !string.IsNullOrEmpty(checkpoint) ? this.GetPartitionRangeQueryFilterForPrefix(checkpoint, prefix) : this.GetPartitionRangeQueryFilterForPrefix(prefix, prefix);
  }
}
