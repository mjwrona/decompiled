// Decompiled with JetBrains decompiler
// Type: Nest.CompositeKey
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (CompositeKeyFormatter))]
  public class CompositeKey : IsAReadOnlyDictionaryBase<string, object>
  {
    public CompositeKey(
      IReadOnlyDictionary<string, object> backingDictionary)
      : base(backingDictionary)
    {
    }

    public bool TryGetValue(string key, out string value) => this.TryGetValue<string>(key, out value);

    public bool TryGetValue(string key, out double value) => this.TryGetValue<double>(key, out value);

    public bool TryGetValue(string key, out int value) => this.TryGetValue<int>(key, out value);

    public bool TryGetValue(string key, out long value) => this.TryGetValue<long>(key, out value);

    public bool TryGetValue(string key, out DateTime value)
    {
      DateTimeOffset dateTimeOffset;
      if (this.TryGetValue(key, out dateTimeOffset))
      {
        value = dateTimeOffset.DateTime;
        return true;
      }
      value = new DateTime();
      return false;
    }

    public bool TryGetValue(string key, out DateTimeOffset value)
    {
      object obj;
      long result;
      if (!this.TryGetValue<object>(key, out obj) || !long.TryParse(obj.ToString(), out result))
      {
        value = new DateTimeOffset();
        return false;
      }
      value = DateTimeUtil.UnixEpoch.AddMilliseconds((double) result);
      return true;
    }

    private bool TryGetValue<TValue>(string key, out TValue value)
    {
      object obj;
      if (!this.BackingDictionary.TryGetValue(key, out obj))
      {
        value = default (TValue);
        return false;
      }
      try
      {
        value = (TValue) Convert.ChangeType(obj, typeof (TValue));
        return true;
      }
      catch
      {
        value = default (TValue);
        return false;
      }
    }
  }
}
