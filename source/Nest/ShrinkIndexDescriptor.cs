// Decompiled with JetBrains decompiler
// Type: Nest.ShrinkIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ShrinkIndexDescriptor : 
    RequestDescriptorBase<ShrinkIndexDescriptor, ShrinkIndexRequestParameters, IShrinkIndexRequest>,
    IShrinkIndexRequest,
    IRequest<ShrinkIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesShrink;

    public ShrinkIndexDescriptor(IndexName index, IndexName target)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (target), (IUrlParameter) target)))
    {
    }

    [SerializationConstructor]
    protected ShrinkIndexDescriptor()
    {
    }

    IndexName IShrinkIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    IndexName IShrinkIndexRequest.Target => this.Self.RouteValues.Get<IndexName>("target");

    public ShrinkIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IShrinkIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ShrinkIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IShrinkIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public ShrinkIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ShrinkIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public ShrinkIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    IAliases IShrinkIndexRequest.Aliases { get; set; }

    IIndexSettings IShrinkIndexRequest.Settings { get; set; }

    public ShrinkIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<IShrinkIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public ShrinkIndexDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<IShrinkIndexRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
