// Decompiled with JetBrains decompiler
// Type: Nest.GetRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using System;

namespace Nest
{
  public class GetRepositoryDescriptor : 
    RequestDescriptorBase<GetRepositoryDescriptor, GetRepositoryRequestParameters, IGetRepositoryRequest>,
    IGetRepositoryRequest,
    IRequest<GetRepositoryRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotGetRepository;

    public GetRepositoryDescriptor()
    {
    }

    public GetRepositoryDescriptor(Names repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    Names IGetRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Names>("repository");

    public GetRepositoryDescriptor RepositoryName(Names repository) => this.Assign<Names>(repository, (Action<IGetRepositoryRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (repository), (IUrlParameter) v)));

    public GetRepositoryDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetRepositoryDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
