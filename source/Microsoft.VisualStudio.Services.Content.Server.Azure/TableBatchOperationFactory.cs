// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableBatchOperationFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableBatchOperationFactory
  {
    private const int AzureTableBatchOverheadSize = 164;
    private const int AzureTableMaximumBatchByteCount = 4194304;
    private const int AzureTableMaximumBatchOperationCount = 100;
    private int maximumBatchOperationCount;
    private object syncLock = new object();

    public TableBatchOperationFactory(int maximumBatchSize = 100)
    {
      this.MaximumBatchOperationCount = maximumBatchSize;
      this.OperationsByPartition = new Dictionary<string, List<TableOperationDescriptor>>();
    }

    public int MaximumBatchOperationCount
    {
      get => this.maximumBatchOperationCount;
      set => this.maximumBatchOperationCount = value > 0 && value <= 100 ? value : throw new ArgumentOutOfRangeException(nameof (value), Resources.MaximumBatchSizeArgumentExceptionMessage((object) 1, (object) 100));
    }

    internal Dictionary<string, List<TableOperationDescriptor>> OperationsByPartition { get; private set; }

    public static string GetOperationPartitionKey(TableOperationDescriptor operation) => operation.OperationType == TableOperationType.Retrieve ? operation.PartitionKey : ((TableEntityTableOperationDescriptor) operation).TableEntity.PartitionKey;

    public void AddOperation(TableOperationDescriptor operation)
    {
      string operationPartitionKey = TableBatchOperationFactory.GetOperationPartitionKey(operation);
      lock (this.syncLock)
      {
        if (!this.OperationsByPartition.ContainsKey(operationPartitionKey))
          this.OperationsByPartition[operationPartitionKey] = new List<TableOperationDescriptor>();
        this.OperationsByPartition[operationPartitionKey].Add(operation);
      }
    }

    public void AddOperations(IEnumerable<TableOperationDescriptor> operations)
    {
      foreach (TableOperationDescriptor operation in operations)
        this.AddOperation(operation);
    }

    public IEnumerable<TableBatchOperationDescriptor> GetOperationsBatch()
    {
      for (TableBatchOperationDescriptor operationBatch = this.GetOperationBatch(); operationBatch.Count != 0; operationBatch = this.GetOperationBatch())
        yield return operationBatch;
    }

    public TableBatchOperationDescriptor GetOperationBatch(string partition = null)
    {
      if (partition == string.Empty)
        throw new ArgumentOutOfRangeException(nameof (partition), Resources.PartitionOutOfRangeExceptionMessage());
      TableBatchOperationDescriptor operationBatch = new TableBatchOperationDescriptor();
      lock (this.syncLock)
      {
        string key;
        if (partition == null)
        {
          key = this.SelectAnyPartition();
          if (key == null)
            return new TableBatchOperationDescriptor();
        }
        else
          key = partition;
        List<TableOperationDescriptor> list;
        if (!TableBatchOperationFactory.IsBatchableOperation(this.OperationsByPartition[key][0]))
        {
          list = this.OperationsByPartition[key].Take<TableOperationDescriptor>(1).ToList<TableOperationDescriptor>();
          this.OperationsByPartition[key].RemoveAt(0);
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          list = this.OperationsByPartition[key].Where<TableOperationDescriptor>(TableBatchOperationFactory.\u003C\u003EO.\u003C0\u003E__IsBatchableOperation ?? (TableBatchOperationFactory.\u003C\u003EO.\u003C0\u003E__IsBatchableOperation = new Func<TableOperationDescriptor, bool>(TableBatchOperationFactory.IsBatchableOperation))).GroupBy<TableOperationDescriptor, string>(TableBatchOperationFactory.\u003C\u003EO.\u003C1\u003E__GetOperationRowKey ?? (TableBatchOperationFactory.\u003C\u003EO.\u003C1\u003E__GetOperationRowKey = new Func<TableOperationDescriptor, string>(TableBatchOperationFactory.GetOperationRowKey))).Select<IGrouping<string, TableOperationDescriptor>, TableOperationDescriptor>((Func<IGrouping<string, TableOperationDescriptor>, TableOperationDescriptor>) (group => group.First<TableOperationDescriptor>())).Take<TableOperationDescriptor>(this.MaximumBatchOperationCount).ToList<TableOperationDescriptor>();
          this.OperationsByPartition[key] = this.OperationsByPartition[key].Except<TableOperationDescriptor>((IEnumerable<TableOperationDescriptor>) list).ToList<TableOperationDescriptor>();
        }
        int num = 164;
        while (num < 4194304 && list.Any<TableOperationDescriptor>())
        {
          TableOperationDescriptor operation = list.First<TableOperationDescriptor>();
          num += TableBatchOperationFactory.GetOperationSize(operation);
          if (num < 4194304)
          {
            operationBatch.Add(operation);
            list.Remove(operation);
          }
        }
        if (list.Any<TableOperationDescriptor>())
          this.OperationsByPartition[key].AddRange((IEnumerable<TableOperationDescriptor>) list);
        else if (!this.OperationsByPartition[key].Any<TableOperationDescriptor>())
          this.OperationsByPartition.Remove(key);
        return operationBatch;
      }
    }

    internal static ITableEntity GetOperationEntity(TableOperationDescriptor operation) => operation.OperationType != TableOperationType.Retrieve ? ((TableEntityTableOperationDescriptor) operation).TableEntity : (ITableEntity) null;

    internal static string GetOperationRowKey(TableOperationDescriptor operation) => operation.OperationType != TableOperationType.Retrieve ? ((TableEntityTableOperationDescriptor) operation).TableEntity.RowKey : operation.RowKey;

    internal static int GetOperationSize(TableOperationDescriptor operation)
    {
      ITableEntity operationEntity = TableBatchOperationFactory.GetOperationEntity(operation);
      JObject jobject;
      if (operationEntity != null)
        jobject = JObject.FromObject((object) operationEntity);
      else
        jobject = JObject.FromObject((object) new Dictionary<string, string>()
        {
          {
            "partitionKey",
            TableBatchOperationFactory.GetOperationPartitionKey(operation)
          },
          {
            "rowKey",
            TableBatchOperationFactory.GetOperationRowKey(operation)
          }
        });
      return jobject.ToString().Length;
    }

    internal static bool IsBatchableOperation(TableOperationDescriptor operation) => operation.OperationType != TableOperationType.Retrieve;

    private static T2 GetProperty<T1, T2>(T1 containingClass, string propertyName) where T1 : class => (T2) typeof (T1).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) containingClass);

    private string SelectAnyPartition() => this.OperationsByPartition.Keys.Any<string>() ? this.OperationsByPartition.Keys.First<string>() : (string) null;
  }
}
