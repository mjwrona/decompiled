// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class TableExtensions
  {
    public static Task IterateOverQueryAsync<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      Func<IReadOnlyList<TEntity>, bool> segmentCallback)
      where TEntity : ITableEntity, new()
    {
      return table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, Task<bool>>) (segment => Task.FromResult<bool>(segmentCallback(segment))));
    }

    public static async Task<List<TEntity>> ToListAsync<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query)
      where TEntity : ITableEntity, new()
    {
      List<TEntity> list = new List<TEntity>();
      await table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, bool>) (segment =>
      {
        list.AddRange((IEnumerable<TEntity>) segment);
        return true;
      })).ConfigureAwait(false);
      return list;
    }

    public static async Task IterateOverQueryAsync<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      Func<IReadOnlyList<TEntity>, Task<bool>> segmentCallback)
      where TEntity : ITableEntity, new()
    {
      int actualMaxToTake = query.MaxRowsToTake ?? int.MaxValue;
      int taken = 0;
      IResultSegment<TEntity> segment = (IResultSegment<TEntity>) null;
      TableRequestOptions tableRequestOptions = new TableRequestOptions()
      {
        MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(5.0)),
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(1.0), 5)
      };
      do
      {
        segment = await table.ExecuteQuerySegmentedAsync<TEntity>(processor, query, segment?.ContinuationToken, tableRequestOptions, processor.CreateTableContext()).ConfigureAwait(false);
        if (segment.TableExists)
        {
          taken += segment.Results.Count;
          if (await segmentCallback((IReadOnlyList<TEntity>) segment.Results).ConfigureAwait(false))
          {
            if (segment.ContinuationToken == null)
              goto label_10;
          }
          else
            goto label_3;
        }
        else
          goto label_6;
      }
      while (taken < actualMaxToTake);
      goto label_2;
label_6:
      segment = (IResultSegment<TEntity>) null;
      tableRequestOptions = (TableRequestOptions) null;
      return;
label_3:
      segment = (IResultSegment<TEntity>) null;
      tableRequestOptions = (TableRequestOptions) null;
      return;
label_10:
      segment = (IResultSegment<TEntity>) null;
      tableRequestOptions = (TableRequestOptions) null;
      return;
label_2:
      segment = (IResultSegment<TEntity>) null;
      tableRequestOptions = (TableRequestOptions) null;
    }

    public static async Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query)
      where TEntity : ITableEntity, new()
    {
      int count = 0;
      TEntity result = default (TEntity);
      await table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, Task<bool>>) (segment =>
      {
        count += segment.Count;
        if (count > 1)
          throw new ArgumentException("Enumerator had more than one item.");
        result = segment.SingleOrDefault<TEntity>();
        return Task.FromResult<bool>(true);
      })).ConfigureAwait(false);
      return result;
    }

    public static async Task<TEntity> QuerySingleAsync<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query)
      where TEntity : ITableEntity, new()
    {
      int count = 0;
      TEntity result = default (TEntity);
      await table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, Task<bool>>) (segment =>
      {
        count += segment.Count;
        if (count > 1)
          throw new ArgumentException("Enumerator had more than one item.");
        result = segment.SingleOrDefault<TEntity>();
        return Task.FromResult<bool>(true);
      })).ConfigureAwait(false);
      if (count == 0)
        throw new ArgumentException("Enumerator had no items.");
      return result;
    }

    public static IConcurrentIterator<IReadOnlyList<TEntity>> QueryPagesConcurrentIterator<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query)
      where TEntity : ITableEntity, new()
    {
      return (IConcurrentIterator<IReadOnlyList<TEntity>>) new ConcurrentIterator<IReadOnlyList<TEntity>>(new int?(2), processor.CancellationToken, (Func<TryAddValueAsyncFunc<IReadOnlyList<TEntity>>, CancellationToken, Task>) ((addItemAsync, cancellationToken) => table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, Task<bool>>) (async segment => await addItemAsync(segment).ConfigureAwait(false)))));
    }

    public static IConcurrentIterator<TEntity> QueryConcurrentIterator<TEntity>(
      this ITable table,
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      int boundedCapacity = -1)
      where TEntity : ITableEntity, new()
    {
      return (IConcurrentIterator<TEntity>) new ConcurrentIterator<TEntity>(new int?(new int?(boundedCapacity == -1 || boundedCapacity == 0 ? 2 * table.MaxSegmentSize : boundedCapacity).Value), processor.CancellationToken, (Func<TryAddValueAsyncFunc<TEntity>, CancellationToken, Task>) (async (addItemAsync, cancellationToken) => await table.IterateOverQueryAsync<TEntity>(processor, query, (Func<IReadOnlyList<TEntity>, Task<bool>>) (async segment =>
      {
        bool flag = true;
        foreach (TEntity valueToAdd in (IEnumerable<TEntity>) segment)
        {
          flag = await addItemAsync(valueToAdd).ConfigureAwait(false);
          if (!flag)
            break;
        }
        return flag;
      })).ConfigureAwait(false)));
    }

    public static void ValidateQueryResult<TEntity>(
      this List<TEntity> entities,
      Query<TEntity> query)
      where TEntity : ITableEntity, new()
    {
      foreach (TEntity entity1 in entities)
      {
        if (!(!(entity1 is ITableEntityWithColumns entity2) ? query.IsMatch<TEntity>((ITableEntityWithColumns) new TableExtensions.TableEntityWrapper((ITableEntity) entity1)) : query.IsMatch<TEntity>(entity2)))
          throw new InvalidOperationException(string.Format("Item returned from query does not match query. Item:{0},{1} Query:{2}", (object) ("Item:PartitionKey=" + entity1.PartitionKey + ",RowKey=" + entity1.RowKey), (object) entity1, (object) query));
      }
    }

    private readonly struct TableEntityWrapper : ITableEntityWithColumns
    {
      private readonly ITableEntity entity;

      public TableEntityWrapper(ITableEntity entity) => this.entity = entity;

      public string PartitionKey => this.entity.PartitionKey;

      public string RowKey => this.entity.RowKey;

      public DateTimeOffset Timestamp => this.entity.Timestamp;

      public bool TryGetValue<T>(IColumnValue<T> columnValue, out IValue value) where T : IColumn
      {
        if ((object) columnValue.Column is PartitionKeyColumn)
        {
          value = (IValue) new StringValue(this.entity.PartitionKey);
          return true;
        }
        if ((object) columnValue.Column is RowKeyColumn)
        {
          value = (IValue) new StringValue(this.entity.RowKey);
          return true;
        }
        if (!((object) columnValue.Column is TimestampColumn))
          return this.TryGetValue<T>(this.entity.WriteEntity(new OperationContext()), columnValue, out value);
        value = (IValue) new DateTimeValue(this.entity.Timestamp.UtcDateTime);
        return true;
      }
    }
  }
}
