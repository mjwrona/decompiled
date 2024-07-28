// Decompiled with JetBrains decompiler
// Type: Nest.ShapeQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class ShapeQueryFormatter : IJsonFormatter<IShapeQuery>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "boost",
        0
      },
      {
        "_name",
        1
      },
      {
        "ignore_unmapped",
        2
      }
    };
    private static readonly AutomataDictionary ShapeFields = new AutomataDictionary()
    {
      {
        "shape",
        0
      },
      {
        "indexed_shape",
        1
      },
      {
        "relation",
        2
      }
    };

    public IShapeQuery Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IShapeQuery) null;
      int count1 = 0;
      string str1 = (string) null;
      double? nullable1 = new double?();
      string str2 = (string) null;
      bool? nullable2 = new bool?();
      IShapeQuery shapeQuery = (IShapeQuery) null;
      ShapeRelation? nullable3 = new ShapeRelation?();
      while (reader.ReadIsInObject(ref count1))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num1;
        if (ShapeQueryFormatter.Fields.TryGetValue(segment, out num1))
        {
          switch (num1)
          {
            case 0:
              nullable1 = new double?(reader.ReadDouble());
              continue;
            case 1:
              str2 = reader.ReadString();
              continue;
            case 2:
              nullable2 = new bool?(reader.ReadBoolean());
              continue;
            default:
              continue;
          }
        }
        else
        {
          str1 = segment.Utf8String();
          int count2 = 0;
          while (reader.ReadIsInObject(ref count2))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num2;
            if (ShapeQueryFormatter.ShapeFields.TryGetValue(bytes, out num2))
            {
              switch (num2)
              {
                case 0:
                  IJsonFormatter<IGeoShape> formatter1 = formatterResolver.GetFormatter<IGeoShape>();
                  shapeQuery = (IShapeQuery) new ShapeQuery()
                  {
                    Shape = formatter1.Deserialize(ref reader, formatterResolver)
                  };
                  continue;
                case 1:
                  IJsonFormatter<FieldLookup> formatter2 = formatterResolver.GetFormatter<FieldLookup>();
                  shapeQuery = (IShapeQuery) new ShapeQuery()
                  {
                    IndexedShape = (IFieldLookup) formatter2.Deserialize(ref reader, formatterResolver)
                  };
                  continue;
                case 2:
                  nullable3 = new ShapeRelation?(formatterResolver.GetFormatter<ShapeRelation>().Deserialize(ref reader, formatterResolver));
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      if (shapeQuery == null)
        return (IShapeQuery) null;
      shapeQuery.Boost = nullable1;
      shapeQuery.Name = str2;
      shapeQuery.Field = (Field) str1;
      shapeQuery.Relation = nullable3;
      shapeQuery.IgnoreUnmapped = nullable2;
      return shapeQuery;
    }

    public void Serialize(
      ref JsonWriter writer,
      IShapeQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }
  }
}
