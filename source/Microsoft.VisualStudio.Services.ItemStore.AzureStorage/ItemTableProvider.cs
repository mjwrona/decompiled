// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.ItemTableProvider
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public class ItemTableProvider : ItemProviderBase
  {
    private const string RawItemPropertyPrefix = "RawItem_";
    private const int RawItemMaxLengthCharacters = 30720;
    private readonly ITableClientFactory tableFactory;
    private readonly int concurrentGetItemRequestCount;
    private readonly List<string> defaultEntityFields = new List<string>()
    {
      "ETag",
      "PartitionKey",
      "RowKey",
      "ShardHint"
    };
    private readonly int defaultBoundedCapacity;

    public override bool IsReadOnly => this.tableFactory.IsReadOnly;

    public override bool RequiresVssRequestContext => this.tableFactory.RequiresVssRequestContext;

    public ItemTableProvider(
      ITableClientFactory itemTableProviderFactory,
      int concurrentGetItemRequestCount = 2)
    {
      this.tableFactory = itemTableProviderFactory;
      this.defaultBoundedCapacity = this.tableFactory.GetAllTables().Count<ITable>() * 1000;
      this.concurrentGetItemRequestCount = concurrentGetItemRequestCount;
    }

    public override async Task<bool> CompareSwapItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      StoredItem item)
    {
      ItemTableProvider itemTableProvider = this;
      Dictionary<ShardableLocator, StoredItem> items = new Dictionary<ShardableLocator, StoredItem>()
      {
        {
          path,
          item
        }
      };
      return (await itemTableProvider.CompareSwapItemsConcurrentIterator<StoredItem>(processor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) items, false).SingleAsync<KeyValuePair<ShardableLocator, bool>>(CancellationToken.None).ConfigureAwait(false)).Value;
    }

    public override IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> CompareSwapItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, T> items,
      bool atomicDirectory = false)
    {
      TableBatchOperationFactory batchFactory = new TableBatchOperationFactory();
      foreach (KeyValuePair<ShardableLocator, T> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, T>>) items)
      {
        ItemTableProvider.ItemEntity itemEntity = new ItemTableProvider.ItemEntity(keyValuePair.Key, (StoredItem) keyValuePair.Value);
        itemEntity.ETag = keyValuePair.Value.StorageETag;
        ItemTableProvider.ItemEntity entity = itemEntity;
        batchFactory.AddOperation(entity.ETag == null ? TableOperationDescriptor.Insert((ITableEntity) entity) : TableOperationDescriptor.Replace((ITableEntity) entity));
      }
      return new ConcurrentIteratorExceptionWrapper<ProcessedTableOperationResult>(new TableBatchOperationExecutor(batchFactory, this.tableFactory).ProcessAllBatchesConcurrentIterator(processor, atomicDirectory ? (ITableErrorPolicy) TableErrorPolicyInsertAbortOnAnyConflictPolicy.Instance : (ITableErrorPolicy) TableErrorPolicyInsertIgnoreConflictPolicy.Instance), TableItemExceptionMapper.Instance).Select<ProcessedTableOperationResult, KeyValuePair<ShardableLocator, bool>>((Func<ProcessedTableOperationResult, KeyValuePair<ShardableLocator, bool>>) (result =>
      {
        ItemTableProvider.ItemEntity result1 = (ItemTableProvider.ItemEntity) result.OperationResult.Result;
        ShardableLocator shardableLocator = result1.GetShardableLocator();
        items[shardableLocator].StorageETag = result1.ETag;
        return new KeyValuePair<ShardableLocator, bool>(shardableLocator, result.Success);
      }));
    }

    public override Task<T> GetItemAsync<T>(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
    {
      return latencyPreference == LatencyPreference.PreferLowLatency ? this.GetItemAsyncInternalConcurrent<T>(processor, path, this.concurrentGetItemRequestCount) : this.GetItemAsyncInternal<T>(processor, path);
    }

    public override IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
    {
      return this.GetEntitiesConcurrentIterator(processor, prefix, options, ItemTableProvider.EntityRetrievalOptions.IncludeItemData, new int?()).Select<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>((Func<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>) (entity => new KeyValuePair<ShardableLocator, T>(entity.GetShardableLocator(), entity.GetItem<T>())));
    }

    public override IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetResumableItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      ShardableLocator resumePath,
      PathOptions options,
      IteratorPartition partition,
      FilterOptions filterOptions,
      ShardableLocatorRange locatorRange)
    {
      return this.GetResumableEntitiesConcurrentIterator(processor, prefix, resumePath, options, ItemTableProvider.EntityRetrievalOptions.IncludeItemData, new int?(), partition, filterOptions, locatorRange).Select<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>((Func<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>) (entity => new KeyValuePair<ShardableLocator, T>(entity.GetShardableLocator(), entity.GetItem<T>())));
    }

    public override IConcurrentIterator<IEnumerable<KeyValuePair<ShardableLocator, T>>> GetItemPagesConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
    {
      return this.GetEntityPagesConcurrentIterator(processor, prefix, options, ItemTableProvider.EntityRetrievalOptions.IncludeItemData, new int?()).Select<IEnumerable<ItemTableProvider.ItemEntity>, IEnumerable<KeyValuePair<ShardableLocator, T>>>((Func<IEnumerable<ItemTableProvider.ItemEntity>, IEnumerable<KeyValuePair<ShardableLocator, T>>>) (entityPage => entityPage.Select<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>((Func<ItemTableProvider.ItemEntity, KeyValuePair<ShardableLocator, T>>) (entity => new KeyValuePair<ShardableLocator, T>(entity.GetShardableLocator(), entity.GetItem<T>())))));
    }

    public override IConcurrentIterator<ShardableLocator> GetPathsConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
    {
      return this.GetEntitiesConcurrentIterator(processor, prefix, options, ItemTableProvider.EntityRetrievalOptions.None, new int?()).Select<ItemTableProvider.ItemEntity, ShardableLocator>((Func<ItemTableProvider.ItemEntity, ShardableLocator>) (entity => entity.GetShardableLocator()));
    }

    public override async Task<bool> RemoveItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      string etag = null)
    {
      ItemTableProvider itemTableProvider = this;
      Dictionary<ShardableLocator, string> locatorsAndETags = new Dictionary<ShardableLocator, string>()
      {
        {
          path,
          etag
        }
      };
      return (await itemTableProvider.RemoveItemsConcurrentIterator(processor, (IReadOnlyDictionary<ShardableLocator, string>) locatorsAndETags).ToListAsync<KeyValuePair<ShardableLocator, bool>>(CancellationToken.None).ConfigureAwait(false)).Single<KeyValuePair<ShardableLocator, bool>>().Value;
    }

    public override IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> RemoveItemsConcurrentIterator(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, string> locatorsAndETags)
    {
      List<ItemTableProvider.ItemEntity> entitiesToRemove = new List<ItemTableProvider.ItemEntity>(locatorsAndETags.Count);
      Dictionary<ShardableLocator, bool> dictionary = new Dictionary<ShardableLocator, bool>();
      entitiesToRemove.AddRange(locatorsAndETags.Select<KeyValuePair<ShardableLocator, string>, ItemTableProvider.ItemEntity>((Func<KeyValuePair<ShardableLocator, string>, ItemTableProvider.ItemEntity>) (locatorsAndETag =>
      {
        return new ItemTableProvider.ItemEntity(locatorsAndETag.Key, (StoredItem) null)
        {
          ETag = locatorsAndETag.Value ?? "*"
        };
      })));
      return this.DeleteEntities(processor, entitiesToRemove).Select<KeyValuePair<ItemTableProvider.ItemEntity, bool>, KeyValuePair<ShardableLocator, bool>>((Func<KeyValuePair<ItemTableProvider.ItemEntity, bool>, KeyValuePair<ShardableLocator, bool>>) (deleteResult => new KeyValuePair<ShardableLocator, bool>(deleteResult.Key.GetShardableLocator(), deleteResult.Value)));
    }

    public string GetTableAccountName(string shardHint) => this.tableFactory.GetTable(TableKeyUtility.RowKeyFromLocator(new Locator(new string[1]
    {
      shardHint
    }))).StorageAccountName;

    protected override int? DefaultBoundedCapacity => new int?(this.defaultBoundedCapacity);

    private async Task<T> GetItemAsyncInternal<T>(
      VssRequestPump.Processor processor,
      ShardableLocator path)
      where T : StoredItem
    {
      IConcurrentIterator<ItemTableProvider.ItemEntity> entities = this.GetEntitiesConcurrentIterator(processor, path, PathOptions.Target, ItemTableProvider.EntityRetrievalOptions.IncludeItemData, new int?(1));
      T itemAsyncInternal = await entities.MoveNextAsync(CancellationToken.None).ConfigureAwait(false) ? entities.Current.GetItem<T>() : default (T);
      entities = (IConcurrentIterator<ItemTableProvider.ItemEntity>) null;
      return itemAsyncInternal;
    }

    private async Task<T> GetItemAsyncInternalConcurrent<T>(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      int concurrentRequestCount)
      where T : StoredItem
    {
      CancellationTokenSource cts = new CancellationTokenSource();
      Dictionary<Task<bool>, IConcurrentIterator<ItemTableProvider.ItemEntity>> taskToEntityEnumerator = new Dictionary<Task<bool>, IConcurrentIterator<ItemTableProvider.ItemEntity>>();
      for (int index = 0; index < concurrentRequestCount; ++index)
      {
        IConcurrentIterator<ItemTableProvider.ItemEntity> concurrentIterator = this.GetEntitiesConcurrentIterator(processor, path, PathOptions.Target, ItemTableProvider.EntityRetrievalOptions.IncludeItemData, new int?(1));
        taskToEntityEnumerator[concurrentIterator.MoveNextAsync(cts.Token)] = concurrentIterator;
      }
      Task<bool> firstFinishedTask = await Task.WhenAny<bool>((IEnumerable<Task<bool>>) taskToEntityEnumerator.Keys).ConfigureAwait(false);
      cts.Cancel();
      T internalConcurrent = await firstFinishedTask.ConfigureAwait(false) ? taskToEntityEnumerator[firstFinishedTask].Current.GetItem<T>() : default (T);
      cts = (CancellationTokenSource) null;
      taskToEntityEnumerator = (Dictionary<Task<bool>, IConcurrentIterator<ItemTableProvider.ItemEntity>>) null;
      firstFinishedTask = (Task<bool>) null;
      return internalConcurrent;
    }

    private static RangeFilter<T> GetRangeFilter<T>(
      T column,
      string keyValue,
      TableKeyUtility.EqualityType equalityType)
      where T : INonUserColumn
    {
      return new RangeFilter<T>(new RangeMinimumBoundary<T>((IColumnValue<T>) new ItemTableProvider.MinimumPathColumnValue<T>(column, keyValue, equalityType), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<T>((IColumnValue<T>) new ItemTableProvider.MaximumPathColumnValue<T>(column, keyValue, equalityType), RangeBoundaryType.Exclusive));
    }

    private static RangeFilter<T> GetExactMinimumRangeFilter<T>(
      T column,
      string minKeyValue,
      string maxKeyValue,
      TableKeyUtility.EqualityType equalityType)
      where T : INonUserColumn
    {
      return new RangeFilter<T>(new RangeMinimumBoundary<T>((IColumnValue<T>) new ItemTableProvider.ExactPathColumnValue<T>(column, minKeyValue), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<T>((IColumnValue<T>) new ItemTableProvider.MaximumPathColumnValue<T>(column, maxKeyValue, equalityType), RangeBoundaryType.Exclusive));
    }

    private static TableShardPrimaryKey DeterminePrimaryKeyForItem(ShardableLocator path)
    {
      Locator locator1 = path.Locator.GetParent();
      if ((object) locator1 == null)
        locator1 = path.Locator;
      Locator locator2 = locator1;
      return new TableShardPrimaryKey(new PartitionKey(TableKeyUtility.RowKeyFromLocator(locator2)), new RowKey(TableKeyUtility.RowKeyFromLocator(path.Locator)), locator2.Value);
    }

    private static TableShardPrimaryKey DeterminePrimaryKeyForFolder(ShardableLocator path)
    {
      string shard = TableKeyUtility.RowKeyFromLocator(path.Locator);
      return new TableShardPrimaryKey(new PartitionKey(shard), new RowKey(TableKeyUtility.RowKeyFromLocator(path.Locator)), shard);
    }

    private static Dictionary<string, EntityProperty> DetermineFilterKeysForItem(Item item)
    {
      Dictionary<string, EntityProperty> filterKeysForItem = (Dictionary<string, EntityProperty>) null;
      if (item is IFilterableItem filterableItem)
      {
        IEnumerable<KeyValuePair<string, string>> filterKeyValue = filterableItem.GetFilterKeyValue();
        if (filterKeyValue.Any<KeyValuePair<string, string>>())
        {
          filterKeysForItem = new Dictionary<string, EntityProperty>();
          foreach (KeyValuePair<string, string> keyValuePair in filterKeyValue)
          {
            string key = keyValuePair.Key;
            if (key.IndexOfAny(new char[3]{ ':', '-', '_' }) >= 0)
              throw new ArgumentException("Name contains illegal character in name: " + key);
            filterKeysForItem.Add(key, EntityProperty.GeneratePropertyForString(keyValuePair.Value));
          }
        }
      }
      return filterKeysForItem;
    }

    private IConcurrentIterator<KeyValuePair<ItemTableProvider.ItemEntity, bool>> DeleteEntities(
      VssRequestPump.Processor processor,
      List<ItemTableProvider.ItemEntity> entitiesToRemove)
    {
      TableBatchOperationFactory operationBatcher = new TableBatchOperationFactory();
      entitiesToRemove.ForEach((Action<ItemTableProvider.ItemEntity>) (entity => operationBatcher.AddOperation(TableOperationDescriptor.Delete((ITableEntity) entity))));
      return new TableBatchOperationExecutor(operationBatcher, this.tableFactory).ProcessAllBatchesConcurrentIterator(processor, (ITableErrorPolicy) TableErrorPolicyDeleteDefaultPolicy.Instance).Select<ProcessedTableOperationResult, KeyValuePair<ItemTableProvider.ItemEntity, bool>>((Func<ProcessedTableOperationResult, KeyValuePair<ItemTableProvider.ItemEntity, bool>>) (tableResult => new KeyValuePair<ItemTableProvider.ItemEntity, bool>((ItemTableProvider.ItemEntity) tableResult.OperationResult.Result, tableResult.Success)));
    }

    private IConcurrentIterator<IEnumerable<ItemTableProvider.ItemEntity>> GetEntityPagesConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options,
      ItemTableProvider.EntityRetrievalOptions retrievalOptions,
      int? maxResults)
    {
      return (IConcurrentIterator<IEnumerable<ItemTableProvider.ItemEntity>>) this.GetTableQueries(prefix, prefix, options, retrievalOptions, maxResults).Select<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<IReadOnlyList<ItemTableProvider.ItemEntity>>>((Func<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<IReadOnlyList<ItemTableProvider.ItemEntity>>>) (tableAndQuery => tableAndQuery.Table.QueryPagesConcurrentIterator<ItemTableProvider.ItemEntity>(processor, tableAndQuery.Query))).CollectUnordered<IReadOnlyList<ItemTableProvider.ItemEntity>>(this.DefaultBoundedCapacity, processor.CancellationToken);
    }

    private IConcurrentIterator<ItemTableProvider.ItemEntity> GetEntitiesConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options,
      ItemTableProvider.EntityRetrievalOptions retrievalOptions,
      int? maxResults)
    {
      return this.GetTableQueries(prefix, prefix, options, retrievalOptions, maxResults).Select<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<ItemTableProvider.ItemEntity>>((Func<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<ItemTableProvider.ItemEntity>>) (tableAndQuery => this.QueryConcurrentIterator(processor, tableAndQuery))).CollectUnordered<ItemTableProvider.ItemEntity>(this.DefaultBoundedCapacity, processor.CancellationToken);
    }

    private IConcurrentIterator<ItemTableProvider.ItemEntity> QueryConcurrentIterator(
      VssRequestPump.Processor processor,
      ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity> tableAndQuery)
    {
      return tableAndQuery.Table.QueryConcurrentIterator<ItemTableProvider.ItemEntity>(processor, tableAndQuery.Query);
    }

    private IConcurrentIterator<ItemTableProvider.ItemEntity> GetResumableEntitiesConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      ShardableLocator resumePath,
      PathOptions options,
      ItemTableProvider.EntityRetrievalOptions retrievalOptions,
      int? maxResults,
      IteratorPartition partition = null,
      FilterOptions filterOptions = null,
      ShardableLocatorRange range = null)
    {
      IEnumerable<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>> tableQueries = this.GetTableQueries(prefix, resumePath, options, retrievalOptions, maxResults, filterOptions, range);
      return (partition?.SelectIterators<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>>(tableQueries) ?? tableQueries).Select<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<ItemTableProvider.ItemEntity>>((Func<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, IConcurrentIterator<ItemTableProvider.ItemEntity>>) (tableAndQuery => this.QueryConcurrentIterator(processor, tableAndQuery))).CollectSortOrdered<ItemTableProvider.ItemEntity>(this.DefaultBoundedCapacity, (IComparer<ItemTableProvider.ItemEntity>) new ItemTableProvider.ItemEntityComparer(), processor.CancellationToken);
    }

    private IEnumerable<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>> GetTableQueries(
      ShardableLocator prefix,
      ShardableLocator resumePath,
      PathOptions options,
      ItemTableProvider.EntityRetrievalOptions retrievalOptions,
      int? maxResults,
      FilterOptions filterOptions = null,
      ShardableLocatorRange range = null)
    {
      List<string> columns = this.defaultEntityFields.ToList<string>();
      if (retrievalOptions.HasFlag((Enum) ItemTableProvider.EntityRetrievalOptions.IncludeItemData))
        columns.AddRange(Enumerable.Range(0, 21).Select<int, string>((Func<int, string>) (i => string.Format("RawItem_{0:D2}", (object) i))));
      if (filterOptions != null)
        columns.AddRange(filterOptions.Filters.Select<Microsoft.VisualStudio.Services.ItemStore.Server.Common.Filter, string>((Func<Microsoft.VisualStudio.Services.ItemStore.Server.Common.Filter, string>) (f => f.Name)));
      return this.GetTableQueries(prefix, resumePath, options, filterOptions, range).Select<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>>((Func<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>, ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>>) (tableQuery =>
      {
        tableQuery.Query.Columns = columns;
        if (maxResults.HasValue)
          tableQuery.Query.MaxRowsToTake = maxResults;
        return tableQuery;
      }));
    }

    private RangeFilter<PartitionKeyColumn> GetPartitionKeyRangeFilterForDeepChildren(
      string partitionKey)
    {
      return new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new ItemTableProvider.MinimumPathColumnValue<PartitionKeyColumn>(PartitionKeyColumn.Instance, TableKeyUtility.MustBeGreaterThanElement(partitionKey, TableKeyUtility.EqualityType.DeepChildren)), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new ItemTableProvider.MaximumPathColumnValue<PartitionKeyColumn>(PartitionKeyColumn.Instance, TableKeyUtility.MustBeLessThanElement(partitionKey, TableKeyUtility.EqualityType.ImmediateChildren)), RangeBoundaryType.Exclusive));
    }

    private IFilter<UserColumn> GetUserColumnFilter(FilterOptions filterOptions)
    {
      IFilter<UserColumn> userColumnFilter = (IFilter<UserColumn>) null;
      if (filterOptions != null)
      {
        ComparisonFilter<UserColumn>[] array = filterOptions.Filters.Select<Microsoft.VisualStudio.Services.ItemStore.Server.Common.Filter, ComparisonFilter<UserColumn>>((Func<Microsoft.VisualStudio.Services.ItemStore.Server.Common.Filter, ComparisonFilter<UserColumn>>) (f => new ComparisonFilter<UserColumn>((IColumnValue<UserColumn>) new FilterUserColumnValue(new UserColumn(f.Name), f.Value), ComparisonOperator.Equal))).ToArray<ComparisonFilter<UserColumn>>();
        if (array.Length == 1)
          userColumnFilter = (IFilter<UserColumn>) ((IEnumerable<ComparisonFilter<UserColumn>>) array).Single<ComparisonFilter<UserColumn>>();
        else if (array.Length > 1)
          userColumnFilter = (IFilter<UserColumn>) new BooleanFilter<UserColumn>(BooleanOperator.And, (IFilter<UserColumn>[]) array);
      }
      return userColumnFilter;
    }

    private IEnumerable<ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>> GetTableQueries(
      ShardableLocator path,
      ShardableLocator resumePath,
      PathOptions options,
      FilterOptions filterOptions,
      ShardableLocatorRange locatorRange)
    {
      IFilter<UserColumn> userColumnFilter = this.GetUserColumnFilter(filterOptions);
      if (options.HasFlag((Enum) PathOptions.Target))
      {
        TableShardPrimaryKey primaryKeyForItem = ItemTableProvider.DeterminePrimaryKeyForItem(path);
        yield return new ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>(this.tableFactory.GetTable(primaryKeyForItem.PartitionKey.Value), (Query<ItemTableProvider.ItemEntity>) new PointQuery<ItemTableProvider.ItemEntity>((StringColumnValue<PartitionKeyColumn>) new ItemTableProvider.ExactPathColumnValue<PartitionKeyColumn>(PartitionKeyColumn.Instance, primaryKeyForItem.PartitionKey.Value), (StringColumnValue<RowKeyColumn>) new ItemTableProvider.ExactPathColumnValue<RowKeyColumn>(RowKeyColumn.Instance, primaryKeyForItem.RowKey.Value), (IFilter<IUserColumn>) userColumnFilter));
      }
      ShardableLocatorRange range = locatorRange;
      RangeFilter<RowKeyColumn> rowKeyRangeFilter = range != null ? range.ToRangeFilter() : (RangeFilter<RowKeyColumn>) null;
      TableShardPrimaryKey primaryKeyForFolder = ItemTableProvider.DeterminePrimaryKeyForFolder(path);
      TableShardPrimaryKey resumingPrimaryKey = ItemTableProvider.DeterminePrimaryKeyForItem(resumePath);
      bool isResuming = string.CompareOrdinal(resumingPrimaryKey.RowKey.Value, primaryKeyForFolder.RowKey.Value) != 0;
      if (isResuming)
      {
        string strB1 = TableKeyUtility.MustBeGreaterThanElement(primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.DeepChildren);
        string strB2 = TableKeyUtility.MustBeLessThanElement(primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.ImmediateChildren);
        if (string.CompareOrdinal(resumingPrimaryKey.RowKey.Value, strB1) < 0 || string.CompareOrdinal(resumingPrimaryKey.RowKey.Value, strB2) > 0)
          throw new ArgumentException(string.Format("The resume path provided is invalid, ResumePath: {0}, FolderPath: {1}", (object) resumePath, (object) path));
      }
      if (options.HasFlag((Enum) PathOptions.ImmediateChildren))
      {
        RangeFilter<RowKeyColumn> rangeFilter = !isResuming ? ItemTableProvider.GetRangeFilter<RowKeyColumn>(RowKeyColumn.Instance, primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.ImmediateChildren) : ItemTableProvider.GetExactMinimumRangeFilter<RowKeyColumn>(RowKeyColumn.Instance, resumingPrimaryKey.RowKey.Value, primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.ImmediateChildren);
        if (rowKeyRangeFilter != null)
          rangeFilter = rangeFilter.Intersect(rowKeyRangeFilter);
        yield return new ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>(this.tableFactory.GetTable(primaryKeyForFolder.PartitionKey.Value), (Query<ItemTableProvider.ItemEntity>) new RowRangeQuery<ItemTableProvider.ItemEntity>((StringColumnValue<PartitionKeyColumn>) new ItemTableProvider.ExactPathColumnValue<PartitionKeyColumn>(PartitionKeyColumn.Instance, primaryKeyForFolder.PartitionKey.Value), rangeFilter, (IFilter<IUserColumn>) userColumnFilter));
      }
      if (options.HasFlag((Enum) PathOptions.DeepChildren))
      {
        RangeFilter<PartitionKeyColumn> partitionFilter = this.GetPartitionKeyRangeFilterForDeepChildren(primaryKeyForFolder.PartitionKey.Value);
        RangeFilter<RowKeyColumn> rangeFilter = ItemTableProvider.GetRangeFilter<RowKeyColumn>(RowKeyColumn.Instance, primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.DeepChildren);
        IFilter<INonUserColumn> nonUserColumnFilter = (IFilter<INonUserColumn>) null;
        bool flag = false;
        if (rowKeyRangeFilter != null)
          rangeFilter = rangeFilter.Intersect(rowKeyRangeFilter);
        if (isResuming)
        {
          if (string.CompareOrdinal(resumingPrimaryKey.RowKey.Value, TableKeyUtility.MustBeLessThanElement(primaryKeyForFolder.RowKey.Value, TableKeyUtility.EqualityType.DeepChildren)) > 0)
            flag = true;
          else
            nonUserColumnFilter = (IFilter<INonUserColumn>) new BooleanFilter<INonUserColumn>(BooleanOperator.Or, new IFilter<INonUserColumn>[2]
            {
              (IFilter<INonUserColumn>) new BooleanFilter<INonUserColumn>(BooleanOperator.And, new IFilter<INonUserColumn>[2]
              {
                (IFilter<INonUserColumn>) new PartitionKeyFilter(new EqualFilter<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(resumingPrimaryKey.PartitionKey.Value))),
                (IFilter<INonUserColumn>) new RowKeyFilter(new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new RowKeyColumnValue(resumingPrimaryKey.RowKey.Value), RangeBoundaryType.Exclusive), (RangeMaximumBoundary<RowKeyColumn>) null))
              }),
              (IFilter<INonUserColumn>) new PartitionKeyFilter(new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(resumingPrimaryKey.PartitionKey.Value), RangeBoundaryType.Exclusive), (RangeMaximumBoundary<PartitionKeyColumn>) null))
            });
        }
        if (!flag)
        {
          foreach (ITable allTable in this.tableFactory.GetAllTables())
            yield return new ItemTableProvider.TableAndQuery<ItemTableProvider.ItemEntity>(allTable, (Query<ItemTableProvider.ItemEntity>) new PartitionRangeRowRangeQuery<ItemTableProvider.ItemEntity>(partitionFilter, rangeFilter, nonUserColumnFilter, userColumnFilter));
        }
        partitionFilter = (RangeFilter<PartitionKeyColumn>) null;
        rangeFilter = (RangeFilter<RowKeyColumn>) null;
        nonUserColumnFilter = (IFilter<INonUserColumn>) null;
      }
    }

    private enum EntityRetrievalOptions
    {
      None,
      IncludeItemData,
    }

    private abstract class PathColumnValue<T> : StringColumnValue<T> where T : IColumn
    {
      private const string AllowedValueRegularExpressionString = "^\\.([\\{}~][a-zA-Z0-9_=\\+]*)*$";
      private static readonly Lazy<Regex> AllowedValueRegularExpression = new Lazy<Regex>((Func<Regex>) (() => new Regex("^\\.([\\{}~][a-zA-Z0-9_=\\+]*)*$", RegexOptions.Compiled)));

      protected PathColumnValue(string path)
        : base(path)
      {
        if (!ItemTableProvider.PathColumnValue<T>.AllowedValueRegularExpression.Value.IsMatch(path))
          throw new ArgumentException(string.Format("'{0}' does not match the regular expression '{1}'", (object) path, (object) "^\\.([\\{}~][a-zA-Z0-9_=\\+]*)*$"), nameof (path));
      }
    }

    private class MinimumPathColumnValue<T> : ItemTableProvider.PathColumnValue<T> where T : IColumn
    {
      private readonly T column;

      public MinimumPathColumnValue(
        T column,
        string path,
        TableKeyUtility.EqualityType equalityType)
        : base(TableKeyUtility.MustBeGreaterThanElement(path, equalityType))
      {
        this.column = column;
      }

      public MinimumPathColumnValue(T column, string value)
        : base(value)
      {
        this.column = column;
      }

      public override T Column => this.column;
    }

    private class MaximumPathColumnValue<T> : ItemTableProvider.PathColumnValue<T> where T : IColumn
    {
      private readonly T column;

      public MaximumPathColumnValue(
        T column,
        string path,
        TableKeyUtility.EqualityType equalityType)
        : base(TableKeyUtility.MustBeLessThanElement(path, equalityType))
      {
        this.column = column;
      }

      public MaximumPathColumnValue(T column, string value)
        : base(value)
      {
        this.column = column;
      }

      public override T Column => this.column;
    }

    private class ExactPathColumnValue<T> : ItemTableProvider.PathColumnValue<T> where T : IColumn
    {
      private readonly T column;

      public ExactPathColumnValue(T column, string path)
        : base(path)
      {
        this.column = column;
      }

      public override T Column => this.column;
    }

    private class TableAndQuery<T> where T : ITableEntity, new()
    {
      public readonly ITable Table;
      public readonly Query<T> Query;

      public TableAndQuery(ITable table, Query<T> query)
      {
        this.Table = table;
        this.Query = query;
      }

      public override string ToString() => string.Format("[{0}] {1}", (object) this.Table.StorageAccountName, (object) this.Query);
    }

    private class ItemEntity : TableEntity, ITableEntityWithColumns
    {
      private string shardHint;
      private string rawItem;
      private IDictionary<string, EntityProperty> cachedPropertyBag;
      private IDictionary<string, EntityProperty> filterProperties;

      public ItemEntity()
      {
      }

      public ItemEntity(ShardableLocator path, StoredItem item)
      {
        if (item != null)
        {
          this.rawItem = item.ToJson().ToString();
          this.ETag = item.StorageETag;
        }
        TableShardPrimaryKey primaryKeyForItem = ItemTableProvider.DeterminePrimaryKeyForItem(path);
        this.PartitionKey = primaryKeyForItem.PartitionKey.Value;
        this.RowKey = primaryKeyForItem.RowKey.Value;
        this.shardHint = path.ShardHint;
        this.filterProperties = (IDictionary<string, EntityProperty>) ItemTableProvider.DetermineFilterKeysForItem((Item) item);
      }

      public TItem GetItem<TItem>() where TItem : StoredItem
      {
        if (string.IsNullOrEmpty(this.rawItem))
          return default (TItem);
        TItem obj = Item.FromJson<TItem>(this.rawItem);
        obj.StorageETag = this.ETag;
        return obj;
      }

      public Locator GetLocator() => new Locator(new string[1]
      {
        TableKeyUtility.LocatorPathFromRowKey(this.RowKey)
      });

      public ShardableLocator GetShardableLocator() => new ShardableLocator(this.GetLocator(), this.shardHint);

      public override void ReadEntity(
        IDictionary<string, EntityProperty> properties,
        OperationContext operationContext)
      {
        IEnumerable<EntityProperty> source = properties.Select<KeyValuePair<string, EntityProperty>, KeyValuePair<int, EntityProperty>>((Func<KeyValuePair<string, EntityProperty>, KeyValuePair<int, EntityProperty>>) (pv =>
        {
          string key = pv.Key;
          if (!key.StartsWith("RawItem_"))
            return new KeyValuePair<int, EntityProperty>(-1, (EntityProperty) null);
          int result = 0;
          if (int.TryParse(key.Substring("RawItem_".Length), out result))
            return new KeyValuePair<int, EntityProperty>(result, pv.Value);
          throw new ArgumentOutOfRangeException("The raw item property doesn't have a valid name: " + key);
        })).Where<KeyValuePair<int, EntityProperty>>((Func<KeyValuePair<int, EntityProperty>, bool>) (pv => pv.Key != -1)).OrderBy<KeyValuePair<int, EntityProperty>, int>((Func<KeyValuePair<int, EntityProperty>, int>) (pv => pv.Key)).Select<KeyValuePair<int, EntityProperty>, EntityProperty>((Func<KeyValuePair<int, EntityProperty>, EntityProperty>) (pv => pv.Value));
        if (source.Any<EntityProperty>())
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (EntityProperty entityProperty in source)
            stringBuilder.Append(entityProperty.StringValue);
          if (!string.IsNullOrEmpty(stringBuilder.ToString()))
            this.rawItem = stringBuilder.ToString();
        }
        this.shardHint = properties["ShardHint"].StringValue;
        this.cachedPropertyBag = properties;
      }

      public override IDictionary<string, EntityProperty> WriteEntity(
        OperationContext operationContext)
      {
        IDictionary<string, EntityProperty> dictionary = (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>()
        {
          {
            "ShardHint",
            EntityProperty.GeneratePropertyForString(this.shardHint)
          }
        };
        string rawItem = this.rawItem;
        if (rawItem != null)
        {
          int num = 0;
          int startIndex = 0;
          int length1 = rawItem.Length;
          do
          {
            string key = string.Format("{0}{1:D2}", (object) "RawItem_", (object) num++);
            int length2 = Math.Min(30720, length1 - startIndex);
            string input = rawItem.Substring(startIndex, length2);
            startIndex += length2;
            dictionary[key] = EntityProperty.GeneratePropertyForString(input);
          }
          while (startIndex < length1);
        }
        if (this.filterProperties != null)
        {
          foreach (KeyValuePair<string, EntityProperty> filterProperty in (IEnumerable<KeyValuePair<string, EntityProperty>>) this.filterProperties)
            dictionary.Add(filterProperty);
        }
        this.cachedPropertyBag = dictionary;
        return dictionary;
      }

      public override string ToString() => "(" + this.PartitionKey + ", " + this.RowKey + ")";

      public bool TryGetValue<T>(IColumnValue<T> columnValue, out IValue value) where T : IColumn
      {
        this.cachedPropertyBag = this.cachedPropertyBag ?? this.WriteEntity((OperationContext) null);
        return this.TryGetValue<T>(this.cachedPropertyBag, columnValue, out value);
      }
    }

    private class ItemEntityComparer : IComparer<ItemTableProvider.ItemEntity>
    {
      public int Compare(ItemTableProvider.ItemEntity x, ItemTableProvider.ItemEntity y)
      {
        if (x == null && y == null)
          return 0;
        return x == null || y == null ? (x != null ? 1 : -1) : (x.PartitionKey == y.PartitionKey ? string.CompareOrdinal(x.RowKey, y.RowKey) : string.CompareOrdinal(x.PartitionKey, y.PartitionKey));
      }
    }
  }
}
