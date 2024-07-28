// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class SnapshotDescriptor : 
    RequestDescriptorBase<SnapshotDescriptor, SnapshotRequestParameters, ISnapshotRequest>,
    ISnapshotRequest,
    IRequest<SnapshotRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotSnapshot;

    public SnapshotDescriptor(Name repository, Name snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected SnapshotDescriptor()
    {
    }

    Name ISnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Name ISnapshotRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    public SnapshotDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public SnapshotDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);

    bool? ISnapshotRequest.IgnoreUnavailable { get; set; }

    bool? ISnapshotRequest.IncludeGlobalState { get; set; }

    Nest.Indices ISnapshotRequest.Indices { get; set; }

    bool? ISnapshotRequest.Partial { get; set; }

    IDictionary<string, object> ISnapshotRequest.Metadata { get; set; }

    public SnapshotDescriptor Index(IndexName index) => this.Indices((Nest.Indices) index);

    public SnapshotDescriptor Index<T>() where T : class => this.Indices((Nest.Indices) typeof (T));

    public SnapshotDescriptor Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<ISnapshotRequest, Nest.Indices>) ((a, v) => a.Indices = v));

    public SnapshotDescriptor IgnoreUnavailable(bool? ignoreUnavailable = true) => this.Assign<bool?>(ignoreUnavailable, (Action<ISnapshotRequest, bool?>) ((a, v) => a.IgnoreUnavailable = v));

    public SnapshotDescriptor IncludeGlobalState(bool? includeGlobalState = true) => this.Assign<bool?>(includeGlobalState, (Action<ISnapshotRequest, bool?>) ((a, v) => a.IncludeGlobalState = v));

    public SnapshotDescriptor Partial(bool? partial = true) => this.Assign<bool?>(partial, (Action<ISnapshotRequest, bool?>) ((a, v) => a.Partial = v));

    public SnapshotDescriptor Metadata(IDictionary<string, object> metadata) => this.Assign<IDictionary<string, object>>(metadata, (Action<ISnapshotRequest, IDictionary<string, object>>) ((a, v) => a.Metadata = v));

    public SnapshotDescriptor Metadata(
      Func<FluentDictionary<string, object>, IDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, IDictionary<string, object>>>(selector, (Action<ISnapshotRequest, Func<FluentDictionary<string, object>, IDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
