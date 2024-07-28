// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ITableEntityWithColumnsExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class ITableEntityWithColumnsExtensions
  {
    public static bool TryGetValue<T>(
      this ITableEntityWithColumns entity,
      IDictionary<string, EntityProperty> properties,
      IColumnValue<T> columnValue,
      out IValue value)
      where T : IColumn
    {
      T column = columnValue.Column;
      if ((object) column is PartitionKeyColumn)
      {
        value = (IValue) new StringValue(entity.PartitionKey);
        return true;
      }
      if ((object) column is RowKeyColumn)
      {
        value = (IValue) new StringValue(entity.RowKey);
        return true;
      }
      if ((object) column is TimestampColumn)
      {
        value = (IValue) new DateTimeValue(entity.Timestamp.UtcDateTime);
        return true;
      }
      EntityProperty entityProperty;
      if (!properties.TryGetValue(columnValue.Column.Name, out entityProperty))
      {
        value = (IValue) null;
        return false;
      }
      if (columnValue.Value.EdmType != entityProperty.PropertyType)
        throw new FormatException(string.Format("Expected EdmType '{0}', but found '{1}'.", (object) columnValue.Value.EdmType, (object) entityProperty.PropertyType));
      switch (entityProperty.PropertyType)
      {
        case EdmType.String:
          value = (IValue) new StringValue(entityProperty.StringValue);
          break;
        case EdmType.Boolean:
          value = (IValue) new BooleanValue(entityProperty.BooleanValue);
          break;
        case EdmType.DateTime:
          value = (IValue) new DateTimeValue(entityProperty.DateTime.Value);
          break;
        default:
          throw new NotImplementedException(string.Format("Cannot get type of {0}", (object) entityProperty.PropertyType));
      }
      return true;
    }
  }
}
