// Decompiled with JetBrains decompiler
// Type: Nest.DateMathFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class DateMathFormatter : IJsonFormatter<DateMath>, IJsonFormatter
  {
    public DateMath Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.String)
        return (DateMath) null;
      ArraySegment<byte> arraySegment = reader.ReadStringSegmentUnsafe();
      DateTime dateTime;
      return !arraySegment.ContainsDateMathSeparator() && arraySegment.IsDateTime(formatterResolver, out dateTime) ? (DateMath) DateMath.Anchored(dateTime) : DateMath.FromString(arraySegment.Utf8String());
    }

    public void Serialize(
      ref JsonWriter writer,
      DateMath value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteString(value.ToString());
    }
  }
}
