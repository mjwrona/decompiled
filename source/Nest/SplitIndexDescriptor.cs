// Decompiled with JetBrains decompiler
// Type: Nest.SplitIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class SplitIndexDescriptor : 
    RequestDescriptorBase<SplitIndexDescriptor, SplitIndexRequestParameters, ISplitIndexRequest>,
    ISplitIndexRequest,
    IRequest<SplitIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesSplit;

    public SplitIndexDescriptor(IndexName index, IndexName target)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (target), (IUrlParameter) target)))
    {
    }

    [SerializationConstructor]
    protected SplitIndexDescriptor()
    {
    }

    IndexName ISplitIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    IndexName ISplitIndexRequest.Target => this.Self.RouteValues.Get<IndexName>("target");

    public SplitIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ISplitIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public SplitIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISplitIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public SplitIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public SplitIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public SplitIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    IAliases ISplitIndexRequest.Aliases { get; set; }

    IIndexSettings ISplitIndexRequest.Settings { get; set; }

    public SplitIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ISplitIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public SplitIndexDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<ISplitIndexRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
