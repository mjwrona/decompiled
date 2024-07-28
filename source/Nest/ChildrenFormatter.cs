// Decompiled with JetBrains decompiler
// Type: Nest.ChildrenFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class ChildrenFormatter : IJsonFormatter<Children>, IJsonFormatter
  {
    public Children Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Children children = new Children();
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginArray:
          List<RelationName> collection = new List<RelationName>();
          int count = 0;
          while (reader.ReadIsInArray(ref count))
          {
            string str = reader.ReadString();
            collection.Add((RelationName) str);
          }
          children.AddRange((IEnumerable<RelationName>) collection);
          return children;
        case JsonToken.String:
          string str1 = reader.ReadString();
          children.Add((RelationName) str1);
          return children;
        default:
          reader.ReadNextBlock();
          return children;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Children value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Count == 0)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        List<IUrlParameter> list = value.Cast<IUrlParameter>().ToList<IUrlParameter>();
        if (list.Count == 1)
        {
          writer.WriteString(list[0].GetString((IConnectionConfigurationValues) connectionSettings));
        }
        else
        {
          writer.WriteBeginArray();
          for (int index = 0; index < list.Count; ++index)
          {
            if (index > 0)
              writer.WriteValueSeparator();
            IUrlParameter urlParameter = list[index];
            writer.WriteString(urlParameter.GetString((IConnectionConfigurationValues) connectionSettings));
          }
          writer.WriteEndArray();
        }
      }
    }
  }
}
