// Decompiled with JetBrains decompiler
// Type: Nest.PutPipelineRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutPipelineRequest : 
    PlainRequestBase<PutPipelineRequestParameters>,
    IPutPipelineRequest,
    IPipeline,
    IRequest<PutPipelineRequestParameters>,
    IRequest
  {
    public string Description { get; set; }

    public IEnumerable<IProcessor> OnFailure { get; set; }

    public IEnumerable<IProcessor> Processors { get; set; }

    public long? Version { get; set; }

    protected IPutPipelineRequest Self => (IPutPipelineRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestPutPipeline;

    public PutPipelineRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected PutPipelineRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutPipelineRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public int? IfVersion
    {
      get => this.Q<int?>("if_version");
      set => this.Q("if_version", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
