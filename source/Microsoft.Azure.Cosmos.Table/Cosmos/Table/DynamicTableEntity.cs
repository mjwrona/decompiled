// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.DynamicTableEntity
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class DynamicTableEntity : ITableEntity
  {
    public DynamicTableEntity() => this.Properties = (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>();

    public DynamicTableEntity(string partitionKey, string rowKey)
      : this(partitionKey, rowKey, DateTimeOffset.MinValue, (string) null, (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>())
    {
    }

    public DynamicTableEntity(
      string partitionKey,
      string rowKey,
      string etag,
      IDictionary<string, EntityProperty> properties)
      : this(partitionKey, rowKey, DateTimeOffset.MinValue, etag, properties)
    {
    }

    internal DynamicTableEntity(
      string partitionKey,
      string rowKey,
      DateTimeOffset timestamp,
      string etag,
      IDictionary<string, EntityProperty> properties)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      this.PartitionKey = partitionKey;
      this.RowKey = rowKey;
      this.Timestamp = timestamp;
      this.ETag = etag;
      this.Properties = properties;
    }

    public IDictionary<string, EntityProperty> Properties { get; set; }

    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string ETag { get; set; }

    public EntityProperty this[string key]
    {
      get => this.Properties[key];
      set => this.Properties[key] = value;
    }

    public void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      this.Properties = properties;
    }

    public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) => this.Properties;
  }
}
