// Decompiled with JetBrains decompiler
// Type: Nest.IRelations
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (VerbatimDictionaryKeysFormatter<Relations, IRelations, RelationName, Children>))]
  public interface IRelations : 
    IIsADictionary<RelationName, Children>,
    IDictionary<RelationName, Children>,
    ICollection<KeyValuePair<RelationName, Children>>,
    IEnumerable<KeyValuePair<RelationName, Children>>,
    IEnumerable,
    IIsADictionary
  {
  }
}
