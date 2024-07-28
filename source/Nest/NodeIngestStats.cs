// Decompiled with JetBrains decompiler
// Type: Nest.NodeIngestStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodeIngestStats
  {
    [DataMember(Name = "pipelines")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, IngestStats>))]
    public IReadOnlyDictionary<string, IngestStats> Pipelines { get; internal set; } = EmptyReadOnly<string, IngestStats>.Dictionary;

    [DataMember(Name = "total")]
    public IngestStats Total { get; set; }
  }
}
