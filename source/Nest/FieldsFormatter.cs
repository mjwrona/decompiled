// Decompiled with JetBrains decompiler
// Type: Nest.FieldsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class FieldsFormatter : IJsonFormatter<Fields>, IJsonFormatter
  {
    private static readonly FieldFormatter FieldFormatter = new FieldFormatter();

    public Fields Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginArray)
      {
        reader.ReadNextBlock();
        return (Fields) null;
      }
      int count = 0;
      List<Field> fieldNames = new List<Field>();
      while (reader.ReadIsInArray(ref count))
      {
        Field field = FieldsFormatter.FieldFormatter.Deserialize(ref reader, formatterResolver);
        if (field != (Field) null)
          fieldNames.Add(field);
      }
      return new Fields((IEnumerable<Field>) fieldNames);
    }

    public void Serialize(
      ref JsonWriter writer,
      Fields value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (Fields) null)
      {
        writer.WriteNull();
      }
      else
      {
        List<Field> listOfFields = value.ListOfFields;
        writer.WriteBeginArray();
        for (int index = 0; index < listOfFields.Count; ++index)
        {
          if (index > 0)
            writer.WriteValueSeparator();
          FieldsFormatter.FieldFormatter.Serialize(ref writer, listOfFields[index], formatterResolver);
        }
        writer.WriteEndArray();
      }
    }
  }
}
