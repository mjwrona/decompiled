// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryRow
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryRow : ITableEntityWithColumns
  {
    private static readonly OperationContext context = new OperationContext();
    public readonly SortedList<string, EntityProperty> Properties = new SortedList<string, EntityProperty>((IComparer<string>) StringComparer.Ordinal);
    public string Etag;

    public DateTimeOffset Timestamp { get; internal set; }

    public string PartitionKey { get; }

    public string RowKey { get; }

    public MemoryRow(string partitionKey, string rowKey)
    {
      this.PartitionKey = partitionKey;
      this.RowKey = rowKey;
    }

    public MemoryRow(ITableEntity entity)
    {
      this.PartitionKey = entity.PartitionKey;
      this.RowKey = entity.RowKey;
      foreach (KeyValuePair<string, EntityProperty> keyValuePair in (IEnumerable<KeyValuePair<string, EntityProperty>>) entity.WriteEntity(MemoryRow.context))
        this.Properties[keyValuePair.Key] = keyValuePair.Value;
    }

    public bool TryGetValue<T>(IColumnValue<T> columnValue, out IValue value) where T : IColumn => this.TryGetValue<T>((IDictionary<string, EntityProperty>) this.Properties, columnValue, out value);

    public override int GetHashCode()
    {
      int hashCode1 = this.Etag.GetHashCode();
      int num = hashCode1 << 1 | hashCode1 >> 31 & 1 | this.PartitionKey.GetHashCode();
      int hashCode2 = num << 1 | num >> 31 & 1 | this.RowKey.GetHashCode();
      foreach (KeyValuePair<string, EntityProperty> property in this.Properties)
      {
        hashCode2 = hashCode2 << 1 | hashCode2 >> 31 & 1;
        hashCode2 |= property.Key.GetHashCode();
        hashCode2 |= property.Value.GetHashCode();
      }
      return hashCode2;
    }
  }
}
