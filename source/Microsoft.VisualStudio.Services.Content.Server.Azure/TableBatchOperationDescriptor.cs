// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableBatchOperationDescriptor
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableBatchOperationDescriptor : List<TableOperationDescriptor>
  {
    public TableBatchOperationDescriptor()
    {
    }

    public TableBatchOperationDescriptor(List<TableOperationDescriptor> descriptors)
      : base((IEnumerable<TableOperationDescriptor>) descriptors)
    {
    }

    public void Delete(ITableEntity entity) => this.Add(TableOperationDescriptor.Delete(entity));

    public void Insert(ITableEntity entity) => this.Add(TableOperationDescriptor.Insert(entity));

    public void InsertOrMerge(ITableEntity entity) => this.Add(TableOperationDescriptor.InsertOrMerge(entity));

    public void InsertOrReplace(ITableEntity entity) => this.Add(TableOperationDescriptor.InsertOrReplace(entity));

    public void Merge(ITableEntity entity) => this.Add(TableOperationDescriptor.Merge(entity));

    public void Replace(ITableEntity entity) => this.Add(TableOperationDescriptor.Replace(entity));

    public void Retrieve(string partitionKey, string rowKey) => this.Add(TableOperationDescriptor.Retrieve(partitionKey, rowKey));
  }
}
