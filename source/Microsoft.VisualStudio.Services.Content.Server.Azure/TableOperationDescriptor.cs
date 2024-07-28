// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableOperationDescriptor
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class TableOperationDescriptor
  {
    public readonly TableOperationType OperationType;

    protected TableOperationDescriptor(TableOperationType type) => this.OperationType = type;

    public abstract string PartitionKey { get; }

    public abstract string RowKey { get; }

    public static TableOperationDescriptor Delete(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.Delete, entity);

    public static TableOperationDescriptor Insert(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.Insert, entity);

    public static TableOperationDescriptor InsertOrMerge(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.InsertOrMerge, entity);

    public static TableOperationDescriptor InsertOrReplace(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.InsertOrReplace, entity);

    public static TableOperationDescriptor Merge(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.Merge, entity);

    public static TableOperationDescriptor Replace(ITableEntity entity) => (TableOperationDescriptor) new TableEntityTableOperationDescriptor(TableOperationType.Replace, entity);

    public static TableOperationDescriptor Retrieve(string partitionKey, string rowKey) => (TableOperationDescriptor) new TableRowTableOperationDescriptor(TableOperationType.Retrieve, partitionKey, rowKey);
  }
}
