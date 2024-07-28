// Decompiled with JetBrains decompiler
// Type: Nest.InterfaceReadOnlyDictionaryFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  internal class InterfaceReadOnlyDictionaryFormatter<TKey, TValue, TDictionary> : 
    InterfaceDictionaryFormatterBase<TKey, TValue, TDictionary>
    where TDictionary : class, IReadOnlyDictionary<TKey, TValue>
  {
    public InterfaceReadOnlyDictionaryFormatter(
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
      return this.Activator(new object[1]
      {
        (object) intermediateCollection
      });
    }
  }
}
