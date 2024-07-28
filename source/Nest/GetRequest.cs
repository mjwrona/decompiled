// Decompiled with JetBrains decompiler
// Type: Nest.GetRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetRequest : 
    PlainRequestBase<GetRequestParameters>,
    IGetRequest,
    IRequest<GetRequestParameters>,
    IRequest
  {
    protected IGetRequest Self => (IGetRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceGet;

    public GetRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected GetRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IGetRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id IGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

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

    public Fields StoredFields
    {
      get => this.Q<Fields>("stored_fields");
      set => this.Q("stored_fields", (object) value);
    }

    public long? Version
    {
      get => this.Q<long?>("version");
      set => this.Q("version", (object) value);
    }

    public Elasticsearch.Net.VersionType? VersionType
    {
      get => this.Q<Elasticsearch.Net.VersionType?>("version_type");
      set => this.Q("version_type", (object) value);
    }
  }
}
