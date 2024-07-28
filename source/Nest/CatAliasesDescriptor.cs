// Decompiled with JetBrains decompiler
// Type: Nest.CatAliasesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatAliasesDescriptor : 
    RequestDescriptorBase<CatAliasesDescriptor, CatAliasesRequestParameters, ICatAliasesRequest>,
    ICatAliasesRequest,
    IRequest<CatAliasesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatAliases;

    public CatAliasesDescriptor()
    {
    }

    public CatAliasesDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Names ICatAliasesRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public CatAliasesDescriptor Name(Names name) => this.Assign<Names>(name, (Action<ICatAliasesRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public CatAliasesDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public CatAliasesDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatAliasesDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatAliasesDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatAliasesDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatAliasesDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatAliasesDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatAliasesDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
