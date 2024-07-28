// Decompiled with JetBrains decompiler
// Type: Nest.InterfaceDictionaryFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  internal class InterfaceDictionaryFormatter<TKey, TValue, TDictionary> : 
    InterfaceDictionaryFormatterBase<TKey, TValue, TDictionary>
    where TDictionary : class, IDictionary<TKey, TValue>
  {
    public InterfaceDictionaryFormatter(
      TypeExtensions.ObjectActivator<TDictionary> activator,
      bool parameterlessCtor)
      : base(activator, parameterlessCtor)
    {
    }

    protected override IEnumerator<KeyValuePair<TKey, TValue>> GetSourceEnumerator(
      TDictionary source)
    {
      return source.GetEnumerator();
    }

    protected override TDictionary Complete(
      ref Dictionary<TKey, TValue> intermediateCollection)
    {
      TDictionary dictionary;
      if (this.ParameterlessCtor)
      {
        dictionary = this.Activator(Array.Empty<object>());
        foreach (KeyValuePair<TKey, TValue> keyValuePair in intermediateCollection)
          dictionary.Add(keyValuePair);
      }
      else
        dictionary = this.Activator(new object[1]
        {
          (object) intermediateCollection
        });
      return dictionary;
    }
  }
}
