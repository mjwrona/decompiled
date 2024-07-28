// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetRequestFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class MultiGetRequestFormatter : IJsonFormatter<IMultiGetRequest>, IJsonFormatter
  {
    private static readonly IdFormatter IdFormatter = new IdFormatter();

    public IMultiGetRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    public void Serialize(
      ref JsonWriter writer,
      IMultiGetRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginObject();
      if (!(value != null ? new bool?(value.Documents.HasAny<IMultiGetOperation>()) : new bool?()).GetValueOrDefault(false))
      {
        writer.WriteEndObject();
      }
      else
      {
        List<IMultiGetOperation> list;
        if (value.Index != (IndexName) null)
        {
          IConnectionSettingsValues settings = formatterResolver.GetConnectionSettings();
          string resolvedIndex = value.Index.GetString((IConnectionConfigurationValues) settings);
          list = value.Documents.Select<IMultiGetOperation, IMultiGetOperation>((Func<IMultiGetOperation, IMultiGetOperation>) (d =>
          {
            if (d.Index == (IndexName) null || !string.Equals(resolvedIndex, d.Index.GetString((IConnectionConfigurationValues) settings)))
              return d;
            d.Index = (IndexName) null;
            return d;
          })).ToList<IMultiGetOperation>();
        }
        else
          list = value.Documents.ToList<IMultiGetOperation>();
        bool flag = list.All<IMultiGetOperation>((Func<IMultiGetOperation, bool>) (p => p.CanBeFlattened));
        writer.WritePropertyName(flag ? "ids" : "docs");
        IJsonFormatter<IMultiGetOperation> jsonFormatter = (IJsonFormatter<IMultiGetOperation>) null;
        if (!flag)
          jsonFormatter = formatterResolver.GetFormatter<IMultiGetOperation>();
        writer.WriteBeginArray();
        for (int index = 0; index < list.Count; ++index)
        {
          if (index > 0)
            writer.WriteValueSeparator();
          IMultiGetOperation multiGetOperation = list[index];
          if (flag)
            MultiGetRequestFormatter.IdFormatter.Serialize(ref writer, multiGetOperation.Id, formatterResolver);
          else
            jsonFormatter.Serialize(ref writer, multiGetOperation, formatterResolver);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
      }
    }
  }
}
