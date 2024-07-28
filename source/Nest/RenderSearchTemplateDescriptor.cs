// Decompiled with JetBrains decompiler
// Type: Nest.RenderSearchTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class RenderSearchTemplateDescriptor : 
    RequestDescriptorBase<RenderSearchTemplateDescriptor, RenderSearchTemplateRequestParameters, IRenderSearchTemplateRequest>,
    IRenderSearchTemplateRequest,
    IRequest<RenderSearchTemplateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceRenderSearchTemplate;

    public RenderSearchTemplateDescriptor()
    {
    }

    public RenderSearchTemplateDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    Nest.Id IRenderSearchTemplateRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public RenderSearchTemplateDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IRenderSearchTemplateRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    string IRenderSearchTemplateRequest.File { get; set; }

    Dictionary<string, object> IRenderSearchTemplateRequest.Params { get; set; }

    string IRenderSearchTemplateRequest.Source { get; set; }

    public RenderSearchTemplateDescriptor Source(string source) => this.Assign<string>(source, (Action<IRenderSearchTemplateRequest, string>) ((a, v) => a.Source = v));

    public RenderSearchTemplateDescriptor File(string file) => this.Assign<string>(file, (Action<IRenderSearchTemplateRequest, string>) ((a, v) => a.File = v));

    public RenderSearchTemplateDescriptor Params(Dictionary<string, object> scriptParams) => this.Assign<Dictionary<string, object>>(scriptParams, (Action<IRenderSearchTemplateRequest, Dictionary<string, object>>) ((a, v) => a.Params = v));

    public RenderSearchTemplateDescriptor Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsSelector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramsSelector, (Action<IRenderSearchTemplateRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Params = v != null ? (Dictionary<string, object>) v(new FluentDictionary<string, object>()) : (Dictionary<string, object>) null));
    }
  }
}
