// Decompiled with JetBrains decompiler
// Type: Nest.OpenPointInTimeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class OpenPointInTimeDescriptor : 
    RequestDescriptorBase<OpenPointInTimeDescriptor, OpenPointInTimeRequestParameters, IOpenPointInTimeRequest>,
    IOpenPointInTimeRequest,
    IRequest<OpenPointInTimeRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceOpenPointInTime;

    public OpenPointInTimeDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected OpenPointInTimeDescriptor()
    {
    }

    Indices IOpenPointInTimeRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public OpenPointInTimeDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IOpenPointInTimeRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public OpenPointInTimeDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IOpenPointInTimeRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public OpenPointInTimeDescriptor AllIndices() => this.Index(Indices.All);

    public OpenPointInTimeDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public OpenPointInTimeDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public OpenPointInTimeDescriptor KeepAlive(string keepalive) => this.Qs("keep_alive", (object) keepalive);

    public OpenPointInTimeDescriptor Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public OpenPointInTimeDescriptor Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);
  }
}
