// Decompiled with JetBrains decompiler
// Type: Nest.PutWatchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutWatchRequest : 
    PlainRequestBase<PutWatchRequestParameters>,
    IPutWatchRequest,
    IRequest<PutWatchRequestParameters>,
    IRequest
  {
    protected IPutWatchRequest Self => (IPutWatchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherPut;

    public PutWatchRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected PutWatchRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutWatchRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public bool? Active
    {
      get => this.Q<bool?>("active");
      set => this.Q("active", (object) value);
    }

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

    public long? Version
    {
      get => this.Q<long?>("version");
      set => this.Q("version", (object) value);
    }

    public Actions Actions { get; set; }

    public ConditionContainer Condition { get; set; }

    public InputContainer Input { get; set; }

    public IDictionary<string, object> Metadata { get; set; }

    public string ThrottlePeriod { get; set; }

    public TransformContainer Transform { get; set; }

    public TriggerContainer Trigger { get; set; }
  }
}
