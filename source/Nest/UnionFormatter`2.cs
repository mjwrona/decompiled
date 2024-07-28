// Decompiled with JetBrains decompiler
// Type: Nest.UnionFormatter`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class UnionFormatter<TFirst, TSecond> : 
    IJsonFormatter<Union<TFirst, TSecond>>,
    IJsonFormatter
  {
    private readonly bool _attemptTSecondIfTFirstIsNull;

    public UnionFormatter() => this._attemptTSecondIfTFirstIsNull = false;

    public UnionFormatter(bool attemptTSecondIfTFirstIsNull) => this._attemptTSecondIfTFirstIsNull = attemptTSecondIfTFirstIsNull;

    public Union<TFirst, TSecond> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> segment = reader.ReadNextBlockSegment();
      TFirst v1;
      if (this.TryRead<TFirst>(ref segment, formatterResolver, out v1))
      {
        if ((object) v1 != null || !this._attemptTSecondIfTFirstIsNull)
          return (Union<TFirst, TSecond>) v1;
        TSecond v2;
        if (this.TryRead<TSecond>(ref segment, formatterResolver, out v2))
          return (Union<TFirst, TSecond>) v2;
      }
      else
      {
        TSecond v3;
        if (this.TryRead<TSecond>(ref segment, formatterResolver, out v3))
          return (Union<TFirst, TSecond>) v3;
      }
      return (Union<TFirst, TSecond>) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      Union<TFirst, TSecond> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Tag)
        {
          case 0:
            formatterResolver.GetFormatter<TFirst>().Serialize(ref writer, value.Item1, formatterResolver);
            break;
          case 1:
            formatterResolver.GetFormatter<TSecond>().Serialize(ref writer, value.Item2, formatterResolver);
            break;
          default:
            throw new Exception(string.Format("Unrecognized tag value: {0}", (object) value.Tag));
        }
      }
    }

    public bool TryRead<T>(
      ref ArraySegment<byte> segment,
      IJsonFormatterResolver formatterResolver,
      out T v)
    {
      JsonReader reader = new JsonReader(segment.Array, segment.Offset);
      try
      {
        IJsonFormatter<T> formatter = formatterResolver.GetFormatter<T>();
        v = formatter.Deserialize(ref reader, formatterResolver);
        return true;
      }
      catch
      {
        v = default (T);
        return false;
      }
    }
  }
}
