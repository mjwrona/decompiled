// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeRepositoryRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class AnalyzeRepositoryRequest : 
    PlainRequestBase<AnalyzeRepositoryRequestParameters>,
    IAnalyzeRepositoryRequest,
    IRequest<AnalyzeRepositoryRequestParameters>,
    IRequest
  {
    protected IAnalyzeRepositoryRequest Self => (IAnalyzeRepositoryRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotAnalyzeRepository;

    public AnalyzeRepositoryRequest(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [SerializationConstructor]
    protected AnalyzeRepositoryRequest()
    {
    }

    [IgnoreDataMember]
    Name IAnalyzeRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    public long? BlobCount
    {
      get => this.Q<long?>("blob_count");
      set => this.Q("blob_count", (object) value);
    }

    public long? Concurrency
    {
      get => this.Q<long?>("concurrency");
      set => this.Q("concurrency", (object) value);
    }

    public bool? Detailed
    {
      get => this.Q<bool?>("detailed");
      set => this.Q("detailed", (object) value);
    }

    public long? EarlyReadNodeCount
    {
      get => this.Q<long?>("early_read_node_count");
      set => this.Q("early_read_node_count", (object) value);
    }

    public string MaxBlobSize
    {
      get => this.Q<string>("max_blob_size");
      set => this.Q("max_blob_size", (object) value);
    }

    public string MaxTotalDataSize
    {
      get => this.Q<string>("max_total_data_size");
      set => this.Q("max_total_data_size", (object) value);
    }

    public long? RareActionProbability
    {
      get => this.Q<long?>("rare_action_probability");
      set => this.Q("rare_action_probability", (object) value);
    }

    public bool? RarelyAbortWrites
    {
      get => this.Q<bool?>("rarely_abort_writes");
      set => this.Q("rarely_abort_writes", (object) value);
    }

    public long? ReadNodeCount
    {
      get => this.Q<long?>("read_node_count");
      set => this.Q("read_node_count", (object) value);
    }

    public long? Seed
    {
      get => this.Q<long?>("seed");
      set => this.Q("seed", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
