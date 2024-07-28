// Decompiled with JetBrains decompiler
// Type: Nest.ResolvableReadOnlyDictionaryFormatter`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  internal class ResolvableReadOnlyDictionaryFormatter<TKey, TValue> : 
    ResolvableDictionaryFormatterBase<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>
    where TKey : IUrlParameter
  {
    protected override IReadOnlyDictionary<TKey, TValue> Create(
      IConnectionSettingsValues settings,
      Dictionary<TKey, TValue> dictionary)
    {
      return (IReadOnlyDictionary<TKey, TValue>) new ResolvableDictionaryProxy<TKey, TValue>((IConnectionConfigurationValues) settings, (IReadOnlyDictionary<TKey, TValue>) dictionary);
    }
  }
}
