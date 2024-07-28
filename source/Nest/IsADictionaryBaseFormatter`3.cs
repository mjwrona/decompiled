// Decompiled with JetBrains decompiler
// Type: Nest.IsADictionaryBaseFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class IsADictionaryBaseFormatter<TKey, TValue, TDictionary> : 
    IJsonFormatter<TDictionary>,
    IJsonFormatter
    where TDictionary : class, IIsADictionary<TKey, TValue>
  {
    private readonly TypeExtensions.ObjectActivator<TDictionary> _activator;
    private readonly bool _parameterlessCtor;

    public IsADictionaryBaseFormatter(
      TypeExtensions.ObjectActivator<TDictionary> activator,
      bool parameterlessCtor)
    {
      this._activator = activator;
      this._parameterlessCtor = parameterlessCtor;
    }

    protected Dictionary<TKey, TValue> Create() => new Dictionary<TKey, TValue>();

    protected void Add(ref Dictionary<TKey, TValue> collection, int index, TKey key, TValue value) => collection.Add(key, value);

    protected TDictionary Complete(
      ref Dictionary<TKey, TValue> intermediateCollection)
    {
      TDictionary dictionary;
      if (this._parameterlessCtor)
      {
        dictionary = this._activator(Array.Empty<object>());
        foreach (KeyValuePair<TKey, TValue> keyValuePair in intermediateCollection)
          dictionary.Add(keyValuePair);
      }
      else
        dictionary = this._activator(new object[1]
        {
          (object) intermediateCollection
        });
      return dictionary;
    }

    protected IEnumerator<KeyValuePair<TKey, TValue>> GetSourceEnumerator(TDictionary source) => source.GetEnumerator();

    public void Serialize(
      ref JsonWriter writer,
      TDictionary value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
      {
        writer.WriteNull();
      }
      else
      {
        Func<string, string> fieldNameInferrer = formatterResolver.GetConnectionSettings().DefaultFieldNameInferrer;
        IObjectPropertyNameFormatter<TKey> formatterWithVerify1 = formatterResolver.GetFormatterWithVerify<TKey>() as IObjectPropertyNameFormatter<TKey>;
        IJsonFormatter<TValue> formatterWithVerify2 = formatterResolver.GetFormatterWithVerify<TValue>();
        writer.WriteBeginObject();
        using (IEnumerator<KeyValuePair<TKey, TValue>> sourceEnumerator = this.GetSourceEnumerator(value))
        {
          if (formatterWithVerify1 != null)
          {
            if (sourceEnumerator.MoveNext())
            {
              KeyValuePair<TKey, TValue> current1 = sourceEnumerator.Current;
              JsonWriter writer1 = new JsonWriter();
              formatterWithVerify1.SerializeToPropertyName(ref writer1, current1.Key, formatterResolver);
              writer.WriteString(fieldNameInferrer(writer1.ToString()));
              writer.WriteNameSeparator();
              formatterWithVerify2.Serialize(ref writer, current1.Value, formatterResolver);
              while (sourceEnumerator.MoveNext())
              {
                writer.WriteValueSeparator();
                KeyValuePair<TKey, TValue> current2 = sourceEnumerator.Current;
                JsonWriter writer2 = new JsonWriter();
                formatterWithVerify1.SerializeToPropertyName(ref writer2, current2.Key, formatterResolver);
                writer.WriteString(fieldNameInferrer(writer2.ToString()));
                writer.WriteNameSeparator();
                formatterWithVerify2.Serialize(ref writer, current2.Value, formatterResolver);
              }
            }
          }
          else if (sourceEnumerator.MoveNext())
          {
            KeyValuePair<TKey, TValue> current3 = sourceEnumerator.Current;
            writer.WriteString(fieldNameInferrer(current3.Key.ToString()));
            writer.WriteNameSeparator();
            formatterWithVerify2.Serialize(ref writer, current3.Value, formatterResolver);
            while (sourceEnumerator.MoveNext())
            {
              writer.WriteValueSeparator();
              KeyValuePair<TKey, TValue> current4 = sourceEnumerator.Current;
              writer.WriteString(fieldNameInferrer(current4.Key.ToString()));
              writer.WriteNameSeparator();
              formatterWithVerify2.Serialize(ref writer, current4.Value, formatterResolver);
            }
          }
        }
        writer.WriteEndObject();
      }
    }

    public TDictionary Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return default (TDictionary);
      IObjectPropertyNameFormatter<TKey> formatterWithVerify1 = formatterResolver.GetFormatterWithVerify<TKey>() as IObjectPropertyNameFormatter<TKey>;
      IJsonFormatter<TValue> formatterWithVerify2 = formatterResolver.GetFormatterWithVerify<TValue>();
      reader.ReadIsBeginObjectWithVerify();
      Dictionary<TKey, TValue> dictionary = this.Create();
      int count = 0;
      if (formatterWithVerify1 != null)
      {
        while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
        {
          TKey key = formatterWithVerify1.DeserializeFromPropertyName(ref reader, formatterResolver);
          reader.ReadIsNameSeparatorWithVerify();
          TValue obj = formatterWithVerify2.Deserialize(ref reader, formatterResolver);
          this.Add(ref dictionary, count - 1, key, obj);
        }
      }
      else
      {
        while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
        {
          TKey key = (TKey) Convert.ChangeType((object) reader.ReadString(), typeof (TKey));
          reader.ReadIsNameSeparatorWithVerify();
          TValue obj = formatterWithVerify2.Deserialize(ref reader, formatterResolver);
          this.Add(ref dictionary, count - 1, key, obj);
        }
      }
      return this.Complete(ref dictionary);
    }
  }
}
