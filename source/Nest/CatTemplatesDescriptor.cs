// Decompiled with JetBrains decompiler
// Type: Nest.CatTemplatesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatTemplatesDescriptor : 
    RequestDescriptorBase<CatTemplatesDescriptor, CatTemplatesRequestParameters, ICatTemplatesRequest>,
    ICatTemplatesRequest,
    IRequest<CatTemplatesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatTemplates;

    public CatTemplatesDescriptor()
    {
    }

    public CatTemplatesDescriptor(Nest.Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Nest.Name ICatTemplatesRequest.Name => this.Self.RouteValues.Get<Nest.Name>("name");

    public CatTemplatesDescriptor Name(Nest.Name name) => this.Assign<Nest.Name>(name, (Action<ICatTemplatesRequest, Nest.Name>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public CatTemplatesDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatTemplatesDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatTemplatesDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatTemplatesDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatTemplatesDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatTemplatesDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatTemplatesDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
