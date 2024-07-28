// Decompiled with JetBrains decompiler
// Type: Nest.ScrollDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ScrollDescriptor<TInferDocument> : 
    RequestDescriptorBase<ScrollDescriptor<TInferDocument>, ScrollRequestParameters, IScrollRequest>,
    IScrollRequest,
    IRequest<ScrollRequestParameters>,
    IRequest,
    ITypedSearchRequest
    where TInferDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceScroll;

    public ScrollDescriptor<TInferDocument> TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public ScrollDescriptor(Time scroll, string scrollId) => this.ScrollId(scrollId).Scroll(scroll);

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);

    Time IScrollRequest.Scroll { get; set; }

    string IScrollRequest.ScrollId { get; set; }

    public ScrollDescriptor<TInferDocument> Scroll(Time scroll) => this.Assign<Time>(scroll, (Action<IScrollRequest, Time>) ((a, v) => a.Scroll = v));

    public ScrollDescriptor<TInferDocument> ScrollId(string scrollId) => this.Assign<string>(scrollId, (Action<IScrollRequest, string>) ((a, v) => a.ScrollId = v));
  }
}
