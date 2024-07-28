// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class MultiGetRequest : 
    PlainRequestBase<MultiGetRequestParameters>,
    IMultiGetRequest,
    IRequest<MultiGetRequestParameters>,
    IRequest
  {
    protected override sealed void RequestDefaults(MultiGetRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new MultiGetResponseBuilder((IMultiGetRequest) this);

    public Fields StoredFields { get; set; }

    public IEnumerable<IMultiGetOperation> Documents { get; set; }

    protected IMultiGetRequest Self => (IMultiGetRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiGet;

    public MultiGetRequest()
    {
    }

    public MultiGetRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    IndexName IMultiGetRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public bool? Realtime
    {
      get => this.Q<bool?>("realtime");
      set => this.Q("realtime", (object) value);
    }

    public bool? Refresh
    {
      get => this.Q<bool?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public bool? SourceEnabled
    {
      get => this.Q<bool?>("_source");
      set => this.Q("_source", (object) value);
    }

    public Fields SourceExcludes
    {
      get => this.Q<Fields>("_source_excludes");
      set => this.Q("_source_excludes", (object) value);
    }

    public Fields SourceIncludes
    {
      get => this.Q<Fields>("_source_includes");
      set => this.Q("_source_includes", (object) value);
    }
  }
}
