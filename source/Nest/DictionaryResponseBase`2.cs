// Decompiled with JetBrains decompiler
// Type: Nest.DictionaryResponseBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public abstract class DictionaryResponseBase<TKey, TValue> : 
    ResponseBase,
    IDictionaryResponse<TKey, TValue>,
    IResponse,
    IElasticsearchResponse
  {
    [IgnoreDataMember]
    protected IDictionaryResponse<TKey, TValue> Self => (IDictionaryResponse<TKey, TValue>) this;

    IReadOnlyDictionary<TKey, TValue> IDictionaryResponse<TKey, TValue>.BackingDictionary { get; set; } = EmptyReadOnly<TKey, TValue>.Dictionary;
  }
}
