// Decompiled with JetBrains decompiler
// Type: Nest.SuggestDictionary`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  public class SuggestDictionary<T> : 
    IsAReadOnlyDictionaryBase<string, ISuggest<T>[]>,
    ISuggestDictionary<T>
    where T : class
  {
    [SerializationConstructor]
    public SuggestDictionary(
      IReadOnlyDictionary<string, ISuggest<T>[]> backingDictionary)
      : base(backingDictionary)
    {
    }

    public static SuggestDictionary<T> Default { get; } = new SuggestDictionary<T>(EmptyReadOnly<string, ISuggest<T>[]>.Dictionary);

    protected override string Sanitize(string key)
    {
      int num = key.IndexOf('#');
      return num <= -1 ? key : key.Substring(num + 1);
    }
  }
}
