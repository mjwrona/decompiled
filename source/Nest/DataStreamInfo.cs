// Decompiled with JetBrains decompiler
// Type: Nest.DataStreamInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DataStreamInfo
  {
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "timestamp_field")]
    public TimestampField TimestampField { get; internal set; }

    [DataMember(Name = "indices")]
    public IReadOnlyCollection<Index> Indices { get; internal set; }

    [DataMember(Name = "generation")]
    public long Generation { get; internal set; }

    [DataMember(Name = "status")]
    public Health Status { get; internal set; }

    [DataMember(Name = "template")]
    public string Template { get; internal set; }

    [DataMember(Name = "ilm_policy")]
    public string IlmPolicy { get; internal set; }
  }
}
