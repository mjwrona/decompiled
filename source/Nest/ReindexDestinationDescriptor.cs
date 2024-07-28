// Decompiled with JetBrains decompiler
// Type: Nest.ReindexDestinationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ReindexDestinationDescriptor : 
    DescriptorBase<ReindexDestinationDescriptor, IReindexDestination>,
    IReindexDestination
  {
    IndexName IReindexDestination.Index { get; set; }

    Elasticsearch.Net.OpType? IReindexDestination.OpType { get; set; }

    string IReindexDestination.Pipeline { get; set; }

    ReindexRouting IReindexDestination.Routing { get; set; }

    Elasticsearch.Net.VersionType? IReindexDestination.VersionType { get; set; }

    public ReindexDestinationDescriptor Routing(ReindexRouting routing) => this.Assign<ReindexRouting>(routing, (Action<IReindexDestination, ReindexRouting>) ((a, v) => a.Routing = v));

    public ReindexDestinationDescriptor Pipeline(string pipeline) => this.Assign<string>(pipeline, (Action<IReindexDestination, string>) ((a, v) => a.Pipeline = v));

    public ReindexDestinationDescriptor OpType(Elasticsearch.Net.OpType? opType) => this.Assign<Elasticsearch.Net.OpType?>(opType, (Action<IReindexDestination, Elasticsearch.Net.OpType?>) ((a, v) => a.OpType = v));

    public ReindexDestinationDescriptor VersionType(Elasticsearch.Net.VersionType? versionType) => this.Assign<Elasticsearch.Net.VersionType?>(versionType, (Action<IReindexDestination, Elasticsearch.Net.VersionType?>) ((a, v) => a.VersionType = v));

    public ReindexDestinationDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IReindexDestination, IndexName>) ((a, v) => a.Index = v));
  }
}
