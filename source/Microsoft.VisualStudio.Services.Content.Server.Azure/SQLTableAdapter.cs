// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SQLTableAdapter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class SQLTableAdapter : ITable
  {
    private int tableId;

    public SQLTableAdapter(string tableName)
    {
      this.Name = tableName;
      this.tableId = SQLTableAdapter.ConvertTableNameToId(tableName);
    }

    public string StorageAccountName => "SQLACCOUNTNAME";

    public int MaxSegmentSize => 1000;

    public string Name { get; private set; }

    public IRetryPolicy RetryPolicy
    {
      set
      {
      }
    }

    public static int ConvertTableNameToId(string tableName) => tableName.GetUTF8Bytes().CalculateBlockHash((IBlobHasher) VsoHash.Instance).GetHashCode();

    public Task<PreauthenticatedUri> GetRowSasUriAsync(
      VssRequestPump.Processor processor,
      string partition,
      string row)
    {
      throw new NotImplementedException();
    }

    public Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      return Task.FromResult<bool>(false);
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      throw new NotImplementedException();
    }

    public Task<TableOperationResult> ExecuteAsync(
      VssRequestPump.Processor processor,
      TableOperationDescriptor operation,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      return processor.ExecuteWorkAsync<TableOperationResult>((Func<IVssRequestContext, TableOperationResult>) (requestContext =>
      {
        SqlTableOperationDescriptor operationDescriptor2 = new SqlTableOperationDescriptor(operation);
        ITableEntity result = (ITableEntity) null;
        SqlOperationData opdata = SqlDataConverter.FromOperationDescriptors((IEnumerable<SqlTableOperationDescriptor>) new SqlTableOperationDescriptor[1]
        {
          operationDescriptor2
        });
        try
        {
          using (ASTableComponent asTableComponent = this.CreateASTableComponent(requestContext))
          {
            List<SqlOperationResult> source = asTableComponent.ExecuteBatch(opdata);
            if (operation is TableEntityTableOperationDescriptor operationDescriptor3)
            {
              result = operationDescriptor3.TableEntity;
              result.ETag = source.FirstOrDefault<SqlOperationResult>()?.ETag;
            }
          }
        }
        catch (ASTableException ex)
        {
          return new TableOperationResult(ex.HttpStatusCode, (object) null);
        }
        return new TableOperationResult(this.GetHttpSuccessCode(operation.OperationType), (object) result);
      }));
    }

    public Task<TableBatchOperationResult> ExecuteBatchAsync(
      VssRequestPump.Processor processor,
      TableBatchOperationDescriptor operation,
      TableRequestOptions options,
      OperationContext context)
    {
      if (operation.Select<TableOperationDescriptor, string>((Func<TableOperationDescriptor, string>) (op => op.RowKey)).Distinct<string>().Count<string>() != operation.Count)
        throw new ArgumentException("Duplicate rows are not allowed in a batch.");
      return processor.ExecuteWorkAsync<TableBatchOperationResult>((Func<IVssRequestContext, TableBatchOperationResult>) (requestContext =>
      {
        SqlOperationData opdata = SqlDataConverter.FromOperationDescriptors(operation.Select<TableOperationDescriptor, SqlTableOperationDescriptor>((Func<TableOperationDescriptor, SqlTableOperationDescriptor>) (op => new SqlTableOperationDescriptor(op))));
        try
        {
          List<SqlOperationResult> source;
          using (ASTableComponent asTableComponent = this.CreateASTableComponent(requestContext))
            source = asTableComponent.ExecuteBatch(opdata);
          Dictionary<int, string> dictionary = source != null ? source.ToDictionary<SqlOperationResult, int, string>((Func<SqlOperationResult, int>) (result => result.Idx), (Func<SqlOperationResult, string>) (result => result.ETag)) : new Dictionary<int, string>();
          List<TableOperationResult> results = new List<TableOperationResult>();
          for (int index = 0; index < opdata.Count; ++index)
          {
            TableOperationType operationTypeRaw = opdata.Operations[index].OperationTypeRaw;
            SqlEntity entity = opdata.Entities[index];
            ITableEntity azureTableEntity = entity.AzureTableEntity;
            if (azureTableEntity != null)
            {
              string str = (string) null;
              dictionary.TryGetValue(entity.Idx, out str);
              azureTableEntity.ETag = str;
            }
            results.Add(new TableOperationResult(this.GetHttpSuccessCode(operationTypeRaw), (object) azureTableEntity));
          }
          return TableBatchOperationResult.FromSuccess((IList<TableOperationResult>) results);
        }
        catch (ASTableException ex)
        {
          return TableBatchOperationResult.FromError(new int?(ex.ErrorIndex), operation[ex.ErrorIndex], "", ex.HttpStatusCode, (StorageException) null);
        }
      }));
    }

    public Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token)
      where TEntity : ITableEntity, new()
    {
      return processor.ExecuteWorkAsync<IResultSegment<TEntity>>((Func<IVssRequestContext, IResultSegment<TEntity>>) (requestContext =>
      {
        SQLTableData tableData = (SQLTableData) null;
        using (ASTableComponent asTableComponent = this.CreateASTableComponent(requestContext))
        {
          if (query.PartitionKeyMax == query.PartitionKeyMin && query.PartitionKeyMax != null && query.RowKeyMax == query.RowKeyMin && query.RowKeyMax != null)
          {
            tableData = asTableComponent.PointQuery(query.PartitionKeyMax, query.RowKeyMax);
          }
          else
          {
            int maxToTake = !query.MaxRowsToTake.HasValue || query.HasExtraFilters ? int.MaxValue : query.MaxRowsToTake.Value;
            tableData = asTableComponent.RangeQuery(query.PartitionKeyMax, query.PartitionKeyMin, query.RowKeyMax, query.RowKeyMin, maxToTake);
          }
        }
        IEnumerable<SqlTableEntity> source = query.filter.Evaluate<IColumn, SqlTableEntity>(SqlDataConverter.FromTableData(tableData));
        if (query.MaxRowsToTake.HasValue)
          source = source.Take<SqlTableEntity>(query.MaxRowsToTake.Value);
        List<TEntity> list = source.Select<SqlTableEntity, TEntity>((Func<SqlTableEntity, TEntity>) (sqlEntity =>
        {
          TEntity entity3 = new TEntity();
          ref TEntity local5 = ref entity3;
          TEntity entity4;
          if ((object) default (TEntity) == null)
          {
            entity4 = local5;
            local5 = ref entity4;
          }
          string partitionKey = sqlEntity.PartitionKey;
          local5.PartitionKey = partitionKey;
          ref TEntity local6 = ref entity3;
          entity4 = default (TEntity);
          if ((object) entity4 == null)
          {
            entity4 = local6;
            local6 = ref entity4;
          }
          string rowKey = sqlEntity.RowKey;
          local6.RowKey = rowKey;
          ref TEntity local7 = ref entity3;
          entity4 = default (TEntity);
          if ((object) entity4 == null)
          {
            entity4 = local7;
            local7 = ref entity4;
          }
          string etag = sqlEntity.ETag;
          local7.ETag = etag;
          ref TEntity local8 = ref entity3;
          entity4 = default (TEntity);
          if ((object) entity4 == null)
          {
            entity4 = local8;
            local8 = ref entity4;
          }
          IDictionary<string, EntityProperty> properties = sqlEntity.Properties;
          local8.ReadEntity(properties, (OperationContext) null);
          return entity3;
        })).ToList<TEntity>();
        list.ValidateQueryResult<TEntity>(query);
        return (IResultSegment<TEntity>) new SQLTableAdapter.SQLResultSegment<TEntity>(list);
      }));
    }

    public Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token,
      TableRequestOptions options,
      OperationContext context)
      where TEntity : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TEntity>(processor, query, token);
    }

    private ASTableComponent CreateASTableComponent(IVssRequestContext requestContext)
    {
      ASTableComponent component = requestContext.CreateComponent<ASTableComponent>();
      component.TableId = this.tableId;
      return component;
    }

    private HttpStatusCode GetHttpSuccessCode(TableOperationType opType)
    {
      switch (opType)
      {
        case TableOperationType.Insert:
        case TableOperationType.Delete:
        case TableOperationType.Replace:
        case TableOperationType.Merge:
        case TableOperationType.InsertOrReplace:
        case TableOperationType.InsertOrMerge:
          return HttpStatusCode.NoContent;
        case TableOperationType.Retrieve:
          return HttpStatusCode.OK;
        default:
          throw new InvalidOperationException("unknown opType " + opType.ToString());
      }
    }

    private sealed class SQLResultSegment<TEntity> : IResultSegment<TEntity> where TEntity : ITableEntity, new()
    {
      public bool TableExists => true;

      public List<TEntity> Results { get; }

      public ITableQueryContinuationToken ContinuationToken => (ITableQueryContinuationToken) null;

      public SQLResultSegment(List<TEntity> results) => this.Results = results;
    }
  }
}
