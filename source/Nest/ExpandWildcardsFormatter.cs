// Decompiled with JetBrains decompiler
// Type: Nest.ExpandWildcardsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class ExpandWildcardsFormatter : 
    IJsonFormatter<IEnumerable<ExpandWildcards>>,
    IJsonFormatter
  {
    public IEnumerable<ExpandWildcards> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.BeginArray)
        return formatterResolver.GetFormatter<IEnumerable<ExpandWildcards>>().Deserialize(ref reader, formatterResolver);
      return (IEnumerable<ExpandWildcards>) new ExpandWildcards[1]
      {
        formatterResolver.GetFormatter<ExpandWildcards>().Deserialize(ref reader, formatterResolver)
      };
    }

    public void Serialize(
      ref JsonWriter writer,
      IEnumerable<ExpandWildcards> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        ExpandWildcards[] array = value.ToArray<ExpandWildcards>();
        int length = array.Length;
        if (length <= 1)
        {
          if (length != 1)
            return;
          formatterResolver.GetFormatter<ExpandWildcards>().Serialize(ref writer, ((IEnumerable<ExpandWildcards>) array).First<ExpandWildcards>(), formatterResolver);
        }
        else
          formatterResolver.GetFormatter<IEnumerable<ExpandWildcards>>().Serialize(ref writer, (IEnumerable<ExpandWildcards>) array, formatterResolver);
      }
    }
  }
}
