// Decompiled with JetBrains decompiler
// Type: Nest.JoinFieldFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class JoinFieldFormatter : IJsonFormatter<JoinField>, IJsonFormatter
  {
    public JoinField Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return new JoinField(new JoinField.Parent((RelationName) reader.ReadString()));
      int count = 0;
      Id parent = (Id) null;
      string name = (string) null;
      while (reader.ReadIsInObject(ref count))
      {
        switch (reader.ReadPropertyName())
        {
          case "parent":
            parent = formatterResolver.GetFormatter<Id>().Deserialize(ref reader, formatterResolver);
            continue;
          case "name":
            name = reader.ReadString();
            continue;
          default:
            continue;
        }
      }
      return !(parent != (Id) null) ? new JoinField(new JoinField.Parent((RelationName) name)) : new JoinField(new JoinField.Child((RelationName) name, parent));
    }

    public void Serialize(
      ref JsonWriter writer,
      JoinField value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Tag)
        {
          case 0:
            formatterResolver.GetFormatter<RelationName>().Serialize(ref writer, value.ParentOption.Name, formatterResolver);
            break;
          case 1:
            JoinField.Child childOption = value.ChildOption;
            writer.WriteBeginObject();
            writer.WritePropertyName("name");
            formatterResolver.GetFormatter<RelationName>().Serialize(ref writer, childOption.Name, formatterResolver);
            writer.WriteValueSeparator();
            writer.WritePropertyName("parent");
            string str = ((IUrlParameter) childOption.ParentId)?.GetString((IConnectionConfigurationValues) formatterResolver.GetConnectionSettings());
            writer.WriteString(str);
            writer.WriteEndObject();
            break;
        }
      }
    }
  }
}
