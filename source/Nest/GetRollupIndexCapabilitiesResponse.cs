// Decompiled with JetBrains decompiler
// Type: Nest.GetRollupIndexCapabilitiesResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (ResolvableDictionaryResponseFormatter<GetRollupIndexCapabilitiesResponse, IndexName, RollupIndexCapabilities>))]
  public class GetRollupIndexCapabilitiesResponse : 
    DictionaryResponseBase<IndexName, RollupIndexCapabilities>
  {
    public IReadOnlyDictionary<IndexName, RollupIndexCapabilities> Indices => this.Self.BackingDictionary;
  }
}
