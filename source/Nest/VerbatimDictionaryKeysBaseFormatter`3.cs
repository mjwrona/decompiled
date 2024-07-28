// Decompiled with JetBrains decompiler
// Type: Nest.VerbatimDictionaryKeysBaseFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal class VerbatimDictionaryKeysBaseFormatter<TDictionary, TKey, TValue> : 
    IJsonFormatter<TDictionary>,
    IJsonFormatter
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private readonly bool _keyIsField = typeof (TKey) == typeof (Field);
    private readonly bool _keyIsIndexName = typeof (TKey) == typeof (IndexName);
    private readonly bool _keyIsPropertyName = typeof (TKey) == typeof (PropertyName);
    private readonly bool _keyIsRelationName = typeof (TKey) == typeof (RelationName);
    private readonly bool _keyIsString = typeof (TKey) == typeof (string);

    public virtual TDictionary Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return formatterResolver.GetFormatter<TDictionary>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      TDictionary value,
      IJsonFormatterResolver formatterResolver)
    {
      IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs = (IEnumerable<KeyValuePair<TKey, TValue>>) value;
      if (keyValuePairs == null)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>(value.Count<KeyValuePair<TKey, TValue>>());
        foreach (KeyValuePair<TKey, TValue> entry in keyValuePairs)
        {
          if (!this.SkipValue(entry))
          {
            string key1;
            if (this._keyIsString)
            {
              TKey key2 = entry.Key;
              ref TKey local = ref key2;
              key1 = (object) local != null ? local.ToString() : (string) null;
            }
            else if (connectionSettings == null)
              key1 = Convert.ToString((object) entry.Key, (IFormatProvider) CultureInfo.InvariantCulture);
            else if (this._keyIsField)
            {
              Field key3 = (object) entry.Key as Field;
              key1 = connectionSettings.Inferrer.Field(key3);
            }
            else if (this._keyIsPropertyName)
            {
              PropertyName key4 = (object) entry.Key as PropertyName;
              IPropertyMapping propertyMapping;
              if (!(key4?.Property != (PropertyInfo) null) || !connectionSettings.PropertyMappings.TryGetValue((MemberInfo) key4.Property, out propertyMapping) || !propertyMapping.Ignore)
                key1 = connectionSettings.Inferrer.PropertyName(key4);
              else
                continue;
            }
            else if (this._keyIsIndexName)
            {
              IndexName key5 = (object) entry.Key as IndexName;
              key1 = connectionSettings.Inferrer.IndexName(key5);
            }
            else if (this._keyIsRelationName)
            {
              RelationName key6 = (object) entry.Key as RelationName;
              key1 = connectionSettings.Inferrer.RelationName(key6);
            }
            else
              key1 = Convert.ToString((object) entry.Key, (IFormatProvider) CultureInfo.InvariantCulture);
            if (key1 != null)
              dictionary[key1] = entry.Value;
          }
        }
        writer.WriteBeginObject();
        if (dictionary.Count > 0)
        {
          IJsonFormatter<TValue> formatter = formatterResolver.GetFormatter<TValue>();
          int num = 0;
          foreach (KeyValuePair<string, TValue> keyValuePair in dictionary)
          {
            if (num > 0)
              writer.WriteValueSeparator();
            writer.WritePropertyName(keyValuePair.Key);
            formatter.Serialize(ref writer, keyValuePair.Value, formatterResolver);
            ++num;
          }
        }
        writer.WriteEndObject();
      }
    }

    protected virtual bool SkipValue(KeyValuePair<TKey, TValue> entry) => (object) entry.Value == null;
  }
}
