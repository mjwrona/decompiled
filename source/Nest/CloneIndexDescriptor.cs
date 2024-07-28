// Decompiled with JetBrains decompiler
// Type: Nest.CloneIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CloneIndexDescriptor : 
    RequestDescriptorBase<CloneIndexDescriptor, CloneIndexRequestParameters, ICloneIndexRequest>,
    ICloneIndexRequest,
    IRequest<CloneIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesClone;

    public CloneIndexDescriptor(IndexName index, IndexName target)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (target), (IUrlParameter) target)))
    {
    }

    [SerializationConstructor]
    protected CloneIndexDescriptor()
    {
    }

    IndexName ICloneIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    IndexName ICloneIndexRequest.Target => this.Self.RouteValues.Get<IndexName>("target");

    public CloneIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ICloneIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public CloneIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICloneIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public CloneIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CloneIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public CloneIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    IAliases ICloneIndexRequest.Aliases { get; set; }

    IIndexSettings ICloneIndexRequest.Settings { get; set; }

    public CloneIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ICloneIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public CloneIndexDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<ICloneIndexRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
