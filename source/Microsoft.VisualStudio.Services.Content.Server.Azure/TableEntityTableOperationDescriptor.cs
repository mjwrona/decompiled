// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableEntityTableOperationDescriptor
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableEntityTableOperationDescriptor : TableOperationDescriptor
  {
    public readonly ITableEntity TableEntity;

    internal TableEntityTableOperationDescriptor(TableOperationType type, ITableEntity entity)
      : base(type)
    {
      this.TableEntity = entity;
    }

    public override string PartitionKey => this.TableEntity.PartitionKey;

    public override string RowKey => this.TableEntity.RowKey;
  }
}
