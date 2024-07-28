// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SqlTableEntity
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class SqlTableEntity : ITableEntityWithColumns
  {
    private const string DateTimeStringFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";

    public SqlTableEntity(ITableEntity entity)
    {
      this.Properties = entity.WriteEntity((OperationContext) null);
      this.RowKey = entity.RowKey;
      this.PartitionKey = entity.PartitionKey;
      this.ETag = entity.ETag;
      this.Entity = entity;
    }

    public SqlTableEntity(string pk, string rk)
    {
      this.PartitionKey = pk;
      this.RowKey = rk;
    }

    public SqlTableEntity(
      string pk,
      string rk,
      string etag,
      Dictionary<string, EntityProperty> properties)
    {
      this.PartitionKey = pk;
      this.RowKey = rk;
      this.Properties = (IDictionary<string, EntityProperty>) properties;
      this.ETag = etag;
    }

    public IDictionary<string, EntityProperty> Properties { get; private set; }

    public string PartitionKey { get; private set; }

    public string RowKey { get; private set; }

    public string ETag { get; private set; }

    public DateTimeOffset Timestamp => throw new NotSupportedException();

    internal ITableEntity Entity { get; private set; }

    public bool TryGetValue<T>(IColumnValue<T> columnValue, out IValue value) where T : IColumn => this.TryGetValue<T>(this.Properties, columnValue, out value);

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is SqlTableEntity sqlTableEntity && this.PartitionKey == sqlTableEntity.PartitionKey && this.RowKey == sqlTableEntity.RowKey;
    }

    public override int GetHashCode() => this.PartitionKey.GetHashCode() ^ this.RowKey.GetHashCode();

    public static string SerializeEntityProperty(EntityProperty ep)
    {
      switch (ep.PropertyType)
      {
        case EdmType.String:
          return ep.StringValue?.ToString();
        case EdmType.Binary:
        case EdmType.Double:
        case EdmType.Guid:
        case EdmType.Int32:
          throw new NotImplementedException(string.Format("Cannot set type of {0}", (object) ep.PropertyType));
        case EdmType.Boolean:
          bool? booleanValue = ep.BooleanValue;
          ref bool? local1 = ref booleanValue;
          return !local1.HasValue ? (string) null : local1.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case EdmType.DateTime:
          return !ep.DateTime.HasValue ? (string) null : SqlTableEntity.SerializeDateTime(ep.DateTime.Value);
        case EdmType.Int64:
          long? int64Value = ep.Int64Value;
          ref long? local2 = ref int64Value;
          return !local2.HasValue ? (string) null : local2.GetValueOrDefault().ToString();
        default:
          throw new InvalidOperationException(string.Format("unknown entity property type {0}", (object) ep.PropertyType));
      }
    }

    public static EntityProperty DeserializeEntityProperty(string str, EdmType type)
    {
      if (str == null)
        return (EntityProperty) null;
      switch (type)
      {
        case EdmType.String:
          return new EntityProperty(str);
        case EdmType.Binary:
          return new EntityProperty(Convert.FromBase64String(str));
        case EdmType.Boolean:
          return new EntityProperty(new bool?(bool.Parse(str)));
        case EdmType.DateTime:
          return new EntityProperty(new DateTime?(SqlTableEntity.DeserializeDateTime(str)));
        case EdmType.Double:
          return new EntityProperty(new double?(double.Parse(str)));
        case EdmType.Guid:
          return new EntityProperty(new Guid?(Guid.Parse(str)));
        case EdmType.Int32:
          return new EntityProperty(new int?(int.Parse(str)));
        case EdmType.Int64:
          return new EntityProperty(new long?(long.Parse(str)));
        default:
          throw new InvalidOperationException(string.Format("unknown entity property type {0}", (object) type));
      }
    }

    public static string SerializeDateTime(DateTime date) => date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", (IFormatProvider) CultureInfo.InvariantCulture);

    public static DateTime DeserializeDateTime(string dateTimeString) => DateTime.ParseExact(dateTimeString, "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
  }
}
