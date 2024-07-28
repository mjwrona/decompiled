// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SqlDataConverter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class SqlDataConverter
  {
    internal static SqlOperationData FromOperationDescriptors(
      IEnumerable<SqlTableOperationDescriptor> descriptors)
    {
      List<SqlOperation> operations = new List<SqlOperation>();
      List<SqlEntity> entities = new List<SqlEntity>();
      List<SqlProperty> props = new List<SqlProperty>();
      IEnumerator<SqlTableOperationDescriptor> enumerator = descriptors.GetEnumerator();
      int i = 0;
      while (enumerator.MoveNext())
      {
        SqlTableOperationDescriptor current = enumerator.Current;
        SqlTableEntity tableEntity = current.TableEntity;
        operations.Add(new SqlOperation()
        {
          Idx = i,
          OperationTypeRaw = current.OperationType
        });
        List<SqlEntity> sqlEntityList = entities;
        SqlEntity sqlEntity = new SqlEntity();
        sqlEntity.Idx = i;
        sqlEntity.PartitionKey = tableEntity.PartitionKey;
        sqlEntity.RowKey = tableEntity.RowKey;
        sqlEntity.ETag = tableEntity.ETag;
        sqlEntity.AzureTableEntity = tableEntity.Entity;
        sqlEntityList.Add(sqlEntity);
        if (tableEntity.Properties != null)
        {
          IEnumerable<SqlProperty> collection = tableEntity.Properties.Select<KeyValuePair<string, EntityProperty>, Tuple<string, EdmType, string>>((Func<KeyValuePair<string, EntityProperty>, Tuple<string, EdmType, string>>) (kvp => new Tuple<string, EdmType, string>(kvp.Key, kvp.Value.PropertyType, SqlTableEntity.SerializeEntityProperty(kvp.Value)))).Where<Tuple<string, EdmType, string>>((Func<Tuple<string, EdmType, string>, bool>) (tuple => tuple.Item3 != null)).Select<Tuple<string, EdmType, string>, SqlProperty>((Func<Tuple<string, EdmType, string>, SqlProperty>) (tuple => new SqlProperty()
          {
            Idx = i,
            PropertyName = tuple.Item1,
            PropertyValue = tuple.Item3,
            PropertyType = (short) tuple.Item2
          }));
          props.AddRange(collection);
        }
        i++;
      }
      return new SqlOperationData(operations, entities, props);
    }

    internal static void UpdateETag(TableOperationDescriptor operation, string etag)
    {
      if (!(operation is TableEntityTableOperationDescriptor operationDescriptor))
        return;
      operationDescriptor.TableEntity.ETag = etag;
    }

    internal static IEnumerable<SqlTableEntity> FromTableData(SQLTableData tableData)
    {
      List<SqlTableEntity> sqlTableEntityList = new List<SqlTableEntity>();
      List<SqlEntity> sqlEntityList = tableData.Entities ?? new List<SqlEntity>();
      Dictionary<int, Dictionary<string, EntityProperty>> propDictByIdx = (tableData.Properties ?? new List<SqlProperty>()).GroupBy<SqlProperty, int>((Func<SqlProperty, int>) (prop => prop.Idx)).ToDictionary<IGrouping<int, SqlProperty>, int, Dictionary<string, EntityProperty>>((Func<IGrouping<int, SqlProperty>, int>) (grp => grp.Key), (Func<IGrouping<int, SqlProperty>, Dictionary<string, EntityProperty>>) (grp => grp.Where<SqlProperty>((Func<SqlProperty, bool>) (prop => prop.PropertyValue != null)).ToDictionary<SqlProperty, string, EntityProperty>((Func<SqlProperty, string>) (prop => prop.PropertyName), (Func<SqlProperty, EntityProperty>) (prop => SqlTableEntity.DeserializeEntityProperty(prop.PropertyValue, (EdmType) prop.PropertyType)))));
      foreach (SqlEntity sqlEntity in sqlEntityList)
      {
        int idx = sqlEntity.Idx;
        Dictionary<string, EntityProperty> properties = (Dictionary<string, EntityProperty>) null;
        propDictByIdx.TryGetValue(idx, out properties);
        if (properties == null)
          properties = new Dictionary<string, EntityProperty>();
        yield return new SqlTableEntity(sqlEntity.PartitionKey, sqlEntity.RowKey, sqlEntity.ETag, properties);
      }
    }
  }
}
