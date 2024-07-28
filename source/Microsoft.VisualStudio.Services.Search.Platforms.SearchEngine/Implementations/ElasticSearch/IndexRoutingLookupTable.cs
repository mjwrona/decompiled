// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.IndexRoutingLookupTable
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch
{
  public class IndexRoutingLookupTable
  {
    private readonly string[] m_routingKeys;
    private readonly IDictionary<string, int> m_routingKeyShardMap;

    internal IndexRoutingLookupTable(int size)
    {
      this.m_routingKeys = new string[size];
      this.m_routingKeyShardMap = (IDictionary<string, int>) new Dictionary<string, int>(size);
    }

    internal string GetRoutingKey(int shardId) => this.m_routingKeys[shardId];

    internal void AddorUpdate(string routingKey, int shardId)
    {
      this.m_routingKeys[shardId] = routingKey;
      this.m_routingKeyShardMap.Add(routingKey, shardId);
    }

    internal int GetShardId(string routing) => this.m_routingKeyShardMap[routing];

    internal bool ContainsRoutingMapping(string routing) => this.m_routingKeyShardMap.ContainsKey(routing);
  }
}
