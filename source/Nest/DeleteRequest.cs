// Decompiled with JetBrains decompiler
// Type: Nest.DeleteRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteRequest : 
    PlainRequestBase<DeleteRequestParameters>,
    IDeleteRequest,
    IRequest<DeleteRequestParameters>,
    IRequest
  {
    protected IDeleteRequest Self => (IDeleteRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceDelete;

    public DeleteRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected DeleteRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IDeleteRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id IDeleteRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public long? IfPrimaryTerm
    {
      get => this.Q<long?>("if_primary_term");
      set => this.Q("if_primary_term", (object) value);
    }

    public long? IfSequenceNumber
    {
      get => this.Q<long?>("if_seq_no");
      set => this.Q("if_seq_no", (object) value);
    }

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
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

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
