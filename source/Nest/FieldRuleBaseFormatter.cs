// Decompiled with JetBrains decompiler
// Type: Nest.FieldRuleBaseFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class FieldRuleBaseFormatter : IJsonFormatter<FieldRuleBase>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "username",
        0
      },
      {
        "dn",
        1
      },
      {
        "realm.name",
        2
      },
      {
        "groups",
        3
      }
    };
    private static readonly VerbatimDictionaryInterfaceKeysFormatter<string, object> Formatter = new VerbatimDictionaryInterfaceKeysFormatter<string, object>();

    public FieldRuleBase Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (FieldRuleBase) null;
      int count = 0;
      FieldRuleBase fieldRuleBase = (FieldRuleBase) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (FieldRuleBaseFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              fieldRuleBase = reader.GetCurrentJsonToken() != JsonToken.BeginArray ? (FieldRuleBase) new UsernameRule(reader.ReadString()) : (FieldRuleBase) new UsernameRule(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
              continue;
            case 1:
              fieldRuleBase = reader.GetCurrentJsonToken() != JsonToken.BeginArray ? (FieldRuleBase) new DistinguishedNameRule(reader.ReadString()) : (FieldRuleBase) new DistinguishedNameRule(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
              continue;
            case 2:
              fieldRuleBase = (FieldRuleBase) new RealmRule(reader.ReadString());
              continue;
            case 3:
              fieldRuleBase = (FieldRuleBase) new GroupsRule(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
              continue;
            default:
              continue;
          }
        }
        else
        {
          string str = segment.Utf8String();
          if (str.StartsWith("metadata."))
            fieldRuleBase = (FieldRuleBase) new MetadataRule(str.Replace("metadata.", string.Empty), formatterResolver.GetFormatter<object>().Deserialize(ref reader, formatterResolver));
        }
      }
      return fieldRuleBase;
    }

    public void Serialize(
      ref JsonWriter writer,
      FieldRuleBase value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        FieldRuleBaseFormatter.Formatter.Serialize(ref writer, (IDictionary<string, object>) value, formatterResolver);
    }
  }
}
