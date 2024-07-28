// Decompiled with JetBrains decompiler
// Type: Nest.ResolvableDictionaryFormatterBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal abstract class ResolvableDictionaryFormatterBase<TDictionary, TKey, TValue> : 
    IJsonFormatter<TDictionary>,
    IJsonFormatter
  {
    public TDictionary Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Dictionary<TKey, TValue> dictionary = formatterResolver.GetFormatter<Dictionary<TKey, TValue>>().Deserialize(ref reader, formatterResolver);
      return this.Create(formatterResolver.GetConnectionSettings(), dictionary);
    }

    public void Serialize(
      ref JsonWriter writer,
      TDictionary value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    protected abstract TDictionary Create(
      IConnectionSettingsValues settings,
      Dictionary<TKey, TValue> dictionary);
  }
}
