// Decompiled with JetBrains decompiler
// Type: Nest.CatThreadPoolDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatThreadPoolDescriptor : 
    RequestDescriptorBase<CatThreadPoolDescriptor, CatThreadPoolRequestParameters, ICatThreadPoolRequest>,
    ICatThreadPoolRequest,
    IRequest<CatThreadPoolRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatThreadPool;

    public CatThreadPoolDescriptor()
    {
    }

    public CatThreadPoolDescriptor(Names threadPoolPatterns)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("thread_pool_patterns", (IUrlParameter) threadPoolPatterns)))
    {
    }

    Names ICatThreadPoolRequest.ThreadPoolPatterns => this.Self.RouteValues.Get<Names>("thread_pool_patterns");

    public CatThreadPoolDescriptor ThreadPoolPatterns(Names threadPoolPatterns) => this.Assign<Names>(threadPoolPatterns, (Action<ICatThreadPoolRequest, Names>) ((a, v) => a.RouteValues.Optional("thread_pool_patterns", (IUrlParameter) v)));

    public CatThreadPoolDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatThreadPoolDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatThreadPoolDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatThreadPoolDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatThreadPoolDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.7.0, reason: Setting this value has no effect and will be removed from the specification.")]
    public CatThreadPoolDescriptor Size(Elasticsearch.Net.Size? size) => this.Qs(nameof (size), (object) size);

    public CatThreadPoolDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatThreadPoolDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
