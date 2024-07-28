// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SqlTableOperationDescriptor
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class SqlTableOperationDescriptor
  {
    public SqlTableOperationDescriptor(TableOperationDescriptor descriptor)
    {
      this.OperationType = descriptor.OperationType;
      switch (descriptor.OperationType)
      {
        case TableOperationType.Insert:
        case TableOperationType.Delete:
        case TableOperationType.Replace:
        case TableOperationType.Merge:
        case TableOperationType.InsertOrReplace:
        case TableOperationType.InsertOrMerge:
          this.TableEntity = new SqlTableEntity(((TableEntityTableOperationDescriptor) descriptor).TableEntity);
          break;
        case TableOperationType.Retrieve:
          TableRowTableOperationDescriptor operationDescriptor = (TableRowTableOperationDescriptor) descriptor;
          this.TableEntity = new SqlTableEntity(operationDescriptor.PartitionKey, operationDescriptor.RowKey);
          break;
      }
    }

    public TableOperationType OperationType { get; private set; }

    public SqlTableEntity TableEntity { get; private set; }
  }
}
