// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class AnalyzeRepositoryDescriptor : 
    RequestDescriptorBase<AnalyzeRepositoryDescriptor, AnalyzeRepositoryRequestParameters, IAnalyzeRepositoryRequest>,
    IAnalyzeRepositoryRequest,
    IRequest<AnalyzeRepositoryRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotAnalyzeRepository;

    public AnalyzeRepositoryDescriptor(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [SerializationConstructor]
    protected AnalyzeRepositoryDescriptor()
    {
    }

    Name IAnalyzeRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    public AnalyzeRepositoryDescriptor BlobCount(long? blobcount) => this.Qs("blob_count", (object) blobcount);

    public AnalyzeRepositoryDescriptor Concurrency(long? concurrency) => this.Qs(nameof (concurrency), (object) concurrency);

    public AnalyzeRepositoryDescriptor Detailed(bool? detailed = true) => this.Qs(nameof (detailed), (object) detailed);

    public AnalyzeRepositoryDescriptor EarlyReadNodeCount(long? earlyreadnodecount) => this.Qs("early_read_node_count", (object) earlyreadnodecount);

    public AnalyzeRepositoryDescriptor MaxBlobSize(string maxblobsize) => this.Qs("max_blob_size", (object) maxblobsize);

    public AnalyzeRepositoryDescriptor MaxTotalDataSize(string maxtotaldatasize) => this.Qs("max_total_data_size", (object) maxtotaldatasize);

    public AnalyzeRepositoryDescriptor RareActionProbability(long? rareactionprobability) => this.Qs("rare_action_probability", (object) rareactionprobability);

    public AnalyzeRepositoryDescriptor RarelyAbortWrites(bool? rarelyabortwrites = true) => this.Qs("rarely_abort_writes", (object) rarelyabortwrites);

    public AnalyzeRepositoryDescriptor ReadNodeCount(long? readnodecount) => this.Qs("read_node_count", (object) readnodecount);

    public AnalyzeRepositoryDescriptor Seed(long? seed) => this.Qs(nameof (seed), (object) seed);

    public AnalyzeRepositoryDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
