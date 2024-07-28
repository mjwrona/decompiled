// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DictionaryExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class DictionaryExtensionMethods
  {
    internal static TValue GetOrAdd<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
    {
      TValue orAdd;
      if (dictionary.TryGetValue(key, out orAdd))
        return orAdd;
      dictionary[key] = value;
      return value;
    }

    internal static TValue AddOrUpdate<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key,
      TValue addValue,
      Func<TKey, TValue, TValue> updateValueFactory)
    {
      TValue obj;
      if (dictionary.TryGetValue(key, out obj))
        addValue = updateValueFactory(key, obj);
      dictionary[key] = addValue;
      return addValue;
    }

    internal static bool TryRemove<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key,
      out TValue value)
    {
      value = default (TValue);
      dictionary.TryGetValue(key, out value);
      return dictionary.Remove(key);
    }
  }
}
