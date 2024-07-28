// Decompiled with JetBrains decompiler
// Type: Nest.CleanupRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CleanupRepositoryDescriptor : 
    RequestDescriptorBase<CleanupRepositoryDescriptor, CleanupRepositoryRequestParameters, ICleanupRepositoryRequest>,
    ICleanupRepositoryRequest,
    IRequest<CleanupRepositoryRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotCleanupRepository;

    public CleanupRepositoryDescriptor(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [SerializationConstructor]
    protected CleanupRepositoryDescriptor()
    {
    }

    Name ICleanupRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    public CleanupRepositoryDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CleanupRepositoryDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
