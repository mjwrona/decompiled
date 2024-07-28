// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableEntityAdapter`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Table
{
  public class TableEntityAdapter<T> : TableEntity
  {
    public TableEntityAdapter()
    {
    }

    public TableEntityAdapter(T originalEntity) => this.OriginalEntity = originalEntity;

    public TableEntityAdapter(T originalEntity, string partitionKey, string rowKey)
      : base(partitionKey, rowKey)
    {
      this.OriginalEntity = originalEntity;
    }

    public T OriginalEntity { get; set; }

    public override void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      this.OriginalEntity = TableEntity.ConvertBack<T>(properties, operationContext);
    }

    public override IDictionary<string, EntityProperty> WriteEntity(
      OperationContext operationContext)
    {
      return TableEntity.Flatten((object) this.OriginalEntity, operationContext);
    }
  }
}
