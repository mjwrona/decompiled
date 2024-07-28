// Decompiled with JetBrains decompiler
// Type: Nest.ApplicationsPrivilegesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class ApplicationsPrivilegesFormatter : 
    IJsonFormatter<IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>>>,
    IJsonFormatter
  {
    private static readonly IndicesPrivilegesFormatter Formatter = new IndicesPrivilegesFormatter();

    public IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>>) null;
      Dictionary<string, IReadOnlyCollection<ResourcePrivileges>> dictionary = new Dictionary<string, IReadOnlyCollection<ResourcePrivileges>>();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        string key = reader.ReadPropertyName();
        IReadOnlyCollection<ResourcePrivileges> resourcePrivilegeses = ApplicationsPrivilegesFormatter.Formatter.Deserialize(ref reader, formatterResolver);
        dictionary.Add(key, resourcePrivilegeses);
      }
      return (IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>>) dictionary;
    }

    public void Serialize(
      ref JsonWriter writer,
      IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        int num = 0;
        foreach (KeyValuePair<string, IReadOnlyCollection<ResourcePrivileges>> keyValuePair in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<ResourcePrivileges>>>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(keyValuePair.Key);
          ApplicationsPrivilegesFormatter.Formatter.Serialize(ref writer, keyValuePair.Value, formatterResolver);
          ++num;
        }
        writer.WriteEndObject();
      }
    }
  }
}
