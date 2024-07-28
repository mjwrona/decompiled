// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EdmConverter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents.Interop.Common.Schema;
using Microsoft.Azure.Documents.Interop.Common.Schema.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class EdmConverter
  {
    public static void GetPropertiesFromDocument(
      IList<string> selectColumns,
      string serializedDocument,
      out IDictionary<string, EntityProperty> entityProperties,
      out IDictionary<string, EntityProperty> documentDBProperties)
    {
      entityProperties = (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>();
      documentDBProperties = (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>();
      using (ITableEntityReader tableEntityReader = (ITableEntityReader) new TableEntityReader(serializedDocument))
      {
        tableEntityReader.Start();
        while (tableEntityReader.MoveNext())
        {
          string currentName = tableEntityReader.CurrentName;
          EntityProperty property;
          switch (tableEntityReader.CurrentType)
          {
            case DataType.Guid:
              property = new EntityProperty(tableEntityReader.ReadGuid());
              break;
            case DataType.Double:
              property = new EntityProperty(tableEntityReader.ReadDouble());
              break;
            case DataType.String:
              property = new EntityProperty(tableEntityReader.ReadString());
              break;
            case DataType.Binary:
              property = new EntityProperty(tableEntityReader.ReadBinary());
              break;
            case DataType.Boolean:
              property = new EntityProperty(tableEntityReader.ReadBoolean());
              break;
            case DataType.DateTime:
              property = new EntityProperty(tableEntityReader.ReadDateTime());
              break;
            case DataType.Int32:
              property = new EntityProperty(tableEntityReader.ReadInt32());
              break;
            case DataType.Int64:
              property = new EntityProperty(tableEntityReader.ReadInt64());
              break;
            default:
              throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unexpected Edm type '{0}'", (object) (int) tableEntityReader.CurrentType));
          }
          if (!property.IsNull())
          {
            if (EdmSchemaMapping.IsDocumentDBProperty(currentName) || currentName == "$id" || currentName == "$pk")
              documentDBProperties.Add(currentName, property);
            else if (selectColumns == null || selectColumns.Count == 0 || selectColumns != null && selectColumns.Contains(currentName))
              entityProperties.Add(currentName, property);
          }
        }
        if (selectColumns != null)
        {
          foreach (string selectColumn in (IEnumerable<string>) selectColumns)
          {
            if (!entityProperties.ContainsKey(selectColumn))
              entityProperties.Add(selectColumn, EntityProperty.GeneratePropertyForString((string) null));
          }
        }
        tableEntityReader.End();
      }
    }

    public static void ValidateDocumentDBProperties(
      IDictionary<string, EntityProperty> documentDBProperties)
    {
      EntityProperty property1;
      documentDBProperties.TryGetValue("$pk", out property1);
      if (property1 != null && property1.IsNull())
        throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "PartitionKey cannot be null"));
      EntityProperty property2;
      documentDBProperties.TryGetValue("$id", out property2);
      if (property2 != null && property2.IsNull())
        throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Id cannot be null"));
      EntityProperty property3;
      documentDBProperties.TryGetValue("_etag", out property3);
      if (property3 != null && property3.IsNull())
        throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Etag cannot be null"));
      EntityProperty property4;
      documentDBProperties.TryGetValue("_ts", out property4);
      if (property4 != null && property4.IsNull())
        throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Timestamp cannot be null"));
    }
  }
}
