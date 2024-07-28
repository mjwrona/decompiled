// Decompiled with JetBrains decompiler
// Type: Nest.FuzzinessInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class FuzzinessInterfaceFormatter : IJsonFormatter<IFuzziness>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IFuzziness value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (value.Auto)
      {
        int? nullable = value.Low;
        if (nullable.HasValue)
        {
          nullable = value.High;
          if (nullable.HasValue)
          {
            writer.WriteString(string.Format("AUTO:{0},{1}", (object) value.Low, (object) value.High));
            return;
          }
        }
        writer.WriteString("AUTO");
      }
      else
      {
        int? editDistance = value.EditDistance;
        if (editDistance.HasValue)
        {
          ref JsonWriter local = ref writer;
          editDistance = value.EditDistance;
          int num = editDistance.Value;
          local.WriteInt32(num);
        }
        else
        {
          double? ratio = value.Ratio;
          if (ratio.HasValue)
          {
            ref JsonWriter local = ref writer;
            ratio = value.Ratio;
            double num = ratio.Value;
            local.WriteDouble(num);
          }
          else
            writer.WriteNull();
        }
      }
    }

    public IFuzziness Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => (IFuzziness) formatterResolver.GetFormatter<Fuzziness>().Deserialize(ref reader, formatterResolver);
  }
}
