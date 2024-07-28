// Decompiled with JetBrains decompiler
// Type: Nest.DocumentSimulation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DocumentSimulation
  {
    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "_index")]
    public string Index { get; internal set; }

    [DataMember(Name = "_ingest")]
    public Ingest Ingest { get; internal set; }

    [DataMember(Name = "_parent")]
    public string Parent { get; internal set; }

    [DataMember(Name = "_routing")]
    public string Routing { get; internal set; }

    [DataMember(Name = "_source")]
    public ILazyDocument Source { get; internal set; }

    [DataMember(Name = "_type")]
    public string Type { get; internal set; }
  }
}
