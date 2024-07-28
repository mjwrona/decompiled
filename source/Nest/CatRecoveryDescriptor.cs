// Decompiled with JetBrains decompiler
// Type: Nest.CatRecoveryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatRecoveryDescriptor : 
    RequestDescriptorBase<CatRecoveryDescriptor, CatRecoveryRequestParameters, ICatRecoveryRequest>,
    ICatRecoveryRequest,
    IRequest<CatRecoveryRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatRecovery;

    public CatRecoveryDescriptor()
    {
    }

    public CatRecoveryDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICatRecoveryRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CatRecoveryDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICatRecoveryRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CatRecoveryDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICatRecoveryRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CatRecoveryDescriptor AllIndices() => this.Index(Indices.All);

    public CatRecoveryDescriptor ActiveOnly(bool? activeonly = true) => this.Qs("active_only", (object) activeonly);

    public CatRecoveryDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatRecoveryDescriptor Detailed(bool? detailed = true) => this.Qs(nameof (detailed), (object) detailed);

    public CatRecoveryDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatRecoveryDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatRecoveryDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatRecoveryDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatRecoveryDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatRecoveryDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
