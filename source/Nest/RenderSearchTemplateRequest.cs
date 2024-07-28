// Decompiled with JetBrains decompiler
// Type: Nest.RenderSearchTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RenderSearchTemplateRequest : 
    PlainRequestBase<RenderSearchTemplateRequestParameters>,
    IRenderSearchTemplateRequest,
    IRequest<RenderSearchTemplateRequestParameters>,
    IRequest
  {
    protected IRenderSearchTemplateRequest Self => (IRenderSearchTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceRenderSearchTemplate;

    public RenderSearchTemplateRequest()
    {
    }

    public RenderSearchTemplateRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    [IgnoreDataMember]
    Id IRenderSearchTemplateRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public string File { get; set; }

    public Dictionary<string, object> Params { get; set; }

    public string Source { get; set; }
  }
}
