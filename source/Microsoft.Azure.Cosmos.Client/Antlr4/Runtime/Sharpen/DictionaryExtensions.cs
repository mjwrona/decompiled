// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.DictionaryExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Sharpen
{
  internal static class DictionaryExtensions
  {
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
    {
      TValue obj;
      return !dictionary.TryGetValue(key, out obj) ? default (TValue) : obj;
    }

    public static TValue Put<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
      where TValue : class
    {
      TValue obj;
      if (!dictionary.TryGetValue(key, out obj))
        obj = default (TValue);
      dictionary[key] = value;
      return obj;
    }
  }
}
