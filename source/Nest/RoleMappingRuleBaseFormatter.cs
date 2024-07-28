// Decompiled with JetBrains decompiler
// Type: Nest.RoleMappingRuleBaseFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class RoleMappingRuleBaseFormatter : IJsonFormatter<RoleMappingRuleBase>, IJsonFormatter
  {
    private static readonly FieldRuleBaseFormatter FieldRuleBaseFormatter = new FieldRuleBaseFormatter();
    private static readonly AutomataDictionary Rules = new AutomataDictionary()
    {
      {
        "all",
        0
      },
      {
        "any",
        1
      },
      {
        "field",
        2
      },
      {
        "except",
        3
      }
    };
    private static readonly Nest.SingleOrEnumerableFormatter<RoleMappingRuleBase> SingleOrEnumerableFormatter = new Nest.SingleOrEnumerableFormatter<RoleMappingRuleBase>();

    public RoleMappingRuleBase Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (RoleMappingRuleBase) null;
      int count = 0;
      RoleMappingRuleBase roleMappingRuleBase = (RoleMappingRuleBase) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (RoleMappingRuleBaseFormatter.Rules.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              roleMappingRuleBase = (RoleMappingRuleBase) new AllRoleMappingRule(RoleMappingRuleBaseFormatter.SingleOrEnumerableFormatter.Deserialize(ref reader, formatterResolver));
              continue;
            case 1:
              roleMappingRuleBase = (RoleMappingRuleBase) new AnyRoleMappingRule(RoleMappingRuleBaseFormatter.SingleOrEnumerableFormatter.Deserialize(ref reader, formatterResolver));
              continue;
            case 2:
              roleMappingRuleBase = (RoleMappingRuleBase) new FieldRoleMappingRule(RoleMappingRuleBaseFormatter.FieldRuleBaseFormatter.Deserialize(ref reader, formatterResolver));
              continue;
            case 3:
              roleMappingRuleBase = (RoleMappingRuleBase) new ExceptRoleMappingRole(this.Deserialize(ref reader, formatterResolver));
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return roleMappingRuleBase;
    }

    public void Serialize(
      ref JsonWriter writer,
      RoleMappingRuleBase value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        if (value.AllRules != null)
        {
          writer.WritePropertyName("all");
          writer.WriteBeginArray();
          int num = 0;
          foreach (RoleMappingRuleBase allRule in value.AllRules)
          {
            if (num++ > 0)
              writer.WriteValueSeparator();
            this.Serialize(ref writer, allRule, formatterResolver);
          }
          writer.WriteEndArray();
        }
        else if (value.AnyRules != null)
        {
          writer.WritePropertyName("any");
          writer.WriteBeginArray();
          int num = 0;
          foreach (RoleMappingRuleBase anyRule in value.AnyRules)
          {
            if (num++ > 0)
              writer.WriteValueSeparator();
            this.Serialize(ref writer, anyRule, formatterResolver);
          }
          writer.WriteEndArray();
        }
        else if (value.ExceptRule != null)
        {
          writer.WritePropertyName("except");
          this.Serialize(ref writer, value.ExceptRule, formatterResolver);
        }
        else
        {
          writer.WritePropertyName("field");
          RoleMappingRuleBaseFormatter.FieldRuleBaseFormatter.Serialize(ref writer, value.FieldRule, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }
  }
}
