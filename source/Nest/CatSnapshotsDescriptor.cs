// Decompiled with JetBrains decompiler
// Type: Nest.CatSnapshotsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatSnapshotsDescriptor : 
    RequestDescriptorBase<CatSnapshotsDescriptor, CatSnapshotsRequestParameters, ICatSnapshotsRequest>,
    ICatSnapshotsRequest,
    IRequest<CatSnapshotsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatSnapshots;

    public CatSnapshotsDescriptor()
    {
    }

    public CatSnapshotsDescriptor(Names repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    Names ICatSnapshotsRequest.RepositoryName => this.Self.RouteValues.Get<Names>("repository");

    public CatSnapshotsDescriptor RepositoryName(Names repository) => this.Assign<Names>(repository, (Action<ICatSnapshotsRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (repository), (IUrlParameter) v)));

    public CatSnapshotsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatSnapshotsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatSnapshotsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatSnapshotsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public CatSnapshotsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatSnapshotsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatSnapshotsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
