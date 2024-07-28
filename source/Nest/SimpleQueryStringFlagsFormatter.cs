// Decompiled with JetBrains decompiler
// Type: Nest.SimpleQueryStringFlagsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class SimpleQueryStringFlagsFormatter : 
    IJsonFormatter<SimpleQueryStringFlags?>,
    IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      SimpleQueryStringFlags? value,
      IJsonFormatterResolver formatterResolver)
    {
      if (!value.HasValue)
      {
        writer.WriteNull();
      }
      else
      {
        SimpleQueryStringFlags queryStringFlags = value.Value;
        List<string> values = new List<string>(13);
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.All))
          values.Add("ALL");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.None))
          values.Add("NONE");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.And))
          values.Add("AND");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Or))
          values.Add("OR");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Not))
          values.Add("NOT");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Prefix))
          values.Add("PREFIX");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Phrase))
          values.Add("PHRASE");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Precedence))
          values.Add("PRECEDENCE");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Escape))
          values.Add("ESCAPE");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Whitespace))
          values.Add("WHITESPACE");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Fuzzy))
          values.Add("FUZZY");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Near))
          values.Add("NEAR");
        if (queryStringFlags.HasFlag((Enum) SimpleQueryStringFlags.Slop))
          values.Add("SLOP");
        string str = string.Join("|", (IEnumerable<string>) values);
        writer.WriteString(str);
      }
    }

    public SimpleQueryStringFlags? Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      string str = reader.ReadString();
      if (str == null)
        return new SimpleQueryStringFlags?();
      return new SimpleQueryStringFlags?(((IEnumerable<string>) str.Split('|')).Select<string, SimpleQueryStringFlags?>((Func<string, SimpleQueryStringFlags?>) (flag => flag.ToEnum<SimpleQueryStringFlags>())).Where<SimpleQueryStringFlags?>((Func<SimpleQueryStringFlags?, bool>) (s => s.HasValue)).Aggregate<SimpleQueryStringFlags?, SimpleQueryStringFlags>((SimpleQueryStringFlags) 0, (Func<SimpleQueryStringFlags, SimpleQueryStringFlags?, SimpleQueryStringFlags>) ((current, s) => current | s.Value)));
    }
  }
}
