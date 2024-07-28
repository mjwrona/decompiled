// Decompiled with JetBrains decompiler
// Type: Nest.IndicesPrivilegesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class IndicesPrivilegesFormatter : 
    IJsonFormatter<IReadOnlyCollection<ResourcePrivileges>>,
    IJsonFormatter
  {
    public IReadOnlyCollection<ResourcePrivileges> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return IndicesPrivilegesFormatter.ReadResourcePrivileges(ref reader, formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      IReadOnlyCollection<ResourcePrivileges> value,
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
        IJsonFormatter<IReadOnlyDictionary<string, bool>> formatter = formatterResolver.GetFormatter<IReadOnlyDictionary<string, bool>>();
        foreach (ResourcePrivileges resourcePrivileges in (IEnumerable<ResourcePrivileges>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(resourcePrivileges.Resource);
          formatter.Serialize(ref writer, resourcePrivileges.Privileges, formatterResolver);
          ++num;
        }
        writer.WriteEndObject();
      }
    }

    internal static IReadOnlyCollection<ResourcePrivileges> ReadResourcePrivileges(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IReadOnlyCollection<ResourcePrivileges>) null;
      List<ResourcePrivileges> resourcePrivilegesList = new List<ResourcePrivileges>();
      int count = 0;
      IJsonFormatter<IReadOnlyDictionary<string, bool>> formatter = formatterResolver.GetFormatter<IReadOnlyDictionary<string, bool>>();
      while (reader.ReadIsInObject(ref count))
      {
        string str = reader.ReadPropertyName();
        IReadOnlyDictionary<string, bool> readOnlyDictionary = formatter.Deserialize(ref reader, formatterResolver);
        resourcePrivilegesList.Add(new ResourcePrivileges()
        {
          Resource = str,
          Privileges = readOnlyDictionary
        });
      }
      return (IReadOnlyCollection<ResourcePrivileges>) resourcePrivilegesList;
    }
  }
}
