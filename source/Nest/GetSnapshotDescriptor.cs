// Decompiled with JetBrains decompiler
// Type: Nest.GetSnapshotDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetSnapshotDescriptor : 
    RequestDescriptorBase<GetSnapshotDescriptor, GetSnapshotRequestParameters, IGetSnapshotRequest>,
    IGetSnapshotRequest,
    IRequest<GetSnapshotRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotGet;

    public GetSnapshotDescriptor(Name repository, Names snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected GetSnapshotDescriptor()
    {
    }

    Name IGetSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Names IGetSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Names>("snapshot");

    public GetSnapshotDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetSnapshotDescriptor IncludeRepository(bool? includerepository = true) => this.Qs("include_repository", (object) includerepository);

    public GetSnapshotDescriptor IndexDetails(bool? indexdetails = true) => this.Qs("index_details", (object) indexdetails);

    public GetSnapshotDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public GetSnapshotDescriptor Verbose(bool? verbose = true) => this.Qs(nameof (verbose), (object) verbose);
  }
}
