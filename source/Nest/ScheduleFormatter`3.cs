// Decompiled with JetBrains decompiler
// Type: Nest.ScheduleFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class ScheduleFormatter<TSchedule, TReadAsSchedule, TTime> : 
    IJsonFormatter<TSchedule>,
    IJsonFormatter
    where TSchedule : class, IEnumerable<TTime>
    where TReadAsSchedule : class, TSchedule
  {
    public TSchedule Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      IEnumerable<TTime> times;
      if (reader.GetCurrentJsonToken() != JsonToken.BeginArray)
        times = (IEnumerable<TTime>) new TTime[1]
        {
          formatterResolver.GetFormatter<TTime>().Deserialize(ref reader, formatterResolver)
        };
      else
        times = formatterResolver.GetFormatter<IEnumerable<TTime>>().Deserialize(ref reader, formatterResolver);
      return (TSchedule) typeof (TReadAsSchedule).CreateInstance((object) times);
    }

    public void Serialize(
      ref JsonWriter writer,
      TSchedule value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
        writer.WriteNull();
      else if (value.Count<TTime>() == 1)
        formatterResolver.GetFormatter<TTime>().Serialize(ref writer, value.First<TTime>(), formatterResolver);
      else
        formatterResolver.GetFormatter<IEnumerable<TTime>>().Serialize(ref writer, (IEnumerable<TTime>) value, formatterResolver);
    }
  }
}
