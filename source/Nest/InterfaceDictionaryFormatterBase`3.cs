// Decompiled with JetBrains decompiler
// Type: Nest.InterfaceDictionaryFormatterBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal abstract class InterfaceDictionaryFormatterBase<TKey, TValue, TDictionary> : 
    IJsonFormatter<TDictionary>,
    IJsonFormatter
    where TDictionary : class
  {
    protected readonly TypeExtensions.ObjectActivator<TDictionary> Activator;
    protected readonly bool ParameterlessCtor;

    public InterfaceDictionaryFormatterBase(
      TypeExtensions.ObjectActivator<TDictionary> activator,
      bool parameterlessCtor)
    {
      this.Activator = activator;
      this.ParameterlessCtor = parameterlessCtor;
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
          this.Add(ref dictionary, key, obj);
        }
      }
      else
      {
        while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
        {
          TKey key = (TKey) Convert.ChangeType((object) reader.ReadString(), typeof (TKey));
          reader.ReadIsNameSeparatorWithVerify();
          TValue obj = formatterWithVerify2.Deserialize(ref reader, formatterResolver);
          this.Add(ref dictionary, key, obj);
        }
      }
      return this.Complete(ref dictionary);
    }

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
              formatterWithVerify1.SerializeToPropertyName(ref writer, current1.Key, formatterResolver);
              writer.WriteNameSeparator();
              formatterWithVerify2.Serialize(ref writer, current1.Value, formatterResolver);
              while (sourceEnumerator.MoveNext())
              {
                writer.WriteValueSeparator();
                KeyValuePair<TKey, TValue> current2 = sourceEnumerator.Current;
                formatterWithVerify1.SerializeToPropertyName(ref writer, current2.Key, formatterResolver);
                writer.WriteNameSeparator();
                formatterWithVerify2.Serialize(ref writer, current2.Value, formatterResolver);
              }
            }
          }
          else if (sourceEnumerator.MoveNext())
          {
            KeyValuePair<TKey, TValue> current3 = sourceEnumerator.Current;
            writer.WriteString(current3.Key.ToString());
            writer.WriteNameSeparator();
            formatterWithVerify2.Serialize(ref writer, current3.Value, formatterResolver);
            while (sourceEnumerator.MoveNext())
            {
              writer.WriteValueSeparator();
              KeyValuePair<TKey, TValue> current4 = sourceEnumerator.Current;
              writer.WriteString(current4.Key.ToString());
              writer.WriteNameSeparator();
              formatterWithVerify2.Serialize(ref writer, current4.Value, formatterResolver);
            }
          }
        }
        writer.WriteEndObject();
      }
    }

    private Dictionary<TKey, TValue> Create() => new Dictionary<TKey, TValue>();

    private void Add(ref Dictionary<TKey, TValue> collection, TKey key, TValue value) => collection.Add(key, value);

    protected abstract IEnumerator<KeyValuePair<TKey, TValue>> GetSourceEnumerator(
      TDictionary source);

    protected abstract TDictionary Complete(
      ref Dictionary<TKey, TValue> intermediateCollection);
  }
}
