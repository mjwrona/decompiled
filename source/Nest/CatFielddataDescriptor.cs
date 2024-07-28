// Decompiled with JetBrains decompiler
// Type: Nest.CatFielddataDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Linq.Expressions;

namespace Nest
{
  public class CatFielddataDescriptor : 
    RequestDescriptorBase<CatFielddataDescriptor, CatFielddataRequestParameters, ICatFielddataRequest>,
    ICatFielddataRequest,
    IRequest<CatFielddataRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatFielddata;

    public CatFielddataDescriptor()
    {
    }

    public CatFielddataDescriptor(Nest.Fields fields)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (fields), (IUrlParameter) fields)))
    {
    }

    Nest.Fields ICatFielddataRequest.Fields => this.Self.RouteValues.Get<Nest.Fields>("fields");

    public CatFielddataDescriptor Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ICatFielddataRequest, Nest.Fields>) ((a, v) => a.RouteValues.Optional(nameof (fields), (IUrlParameter) v)));

    public CatFielddataDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) => this.Assign<Expression<Func<T, object>>[]>(fields, (Action<ICatFielddataRequest, Expression<Func<T, object>>[]>) ((a, v) => a.RouteValues.Optional(nameof (fields), (IUrlParameter) (Nest.Fields) (Expression[]) v)));

    public CatFielddataDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatFielddataDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatFielddataDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatFielddataDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatFielddataDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatFielddataDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatFielddataDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatFielddataDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
