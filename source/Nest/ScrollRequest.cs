// Decompiled with JetBrains decompiler
// Type: Nest.ScrollRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ScrollRequest : 
    PlainRequestBase<ScrollRequestParameters>,
    IScrollRequest,
    IRequest<ScrollRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    protected IScrollRequest Self => (IScrollRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceScroll;

    public bool? TotalHitsAsInteger
    {
      get => this.Q<bool?>("rest_total_hits_as_int");
      set => this.Q("rest_total_hits_as_int", (object) value);
    }

    public ScrollRequest(string scrollId, Time scroll)
    {
      this.Scroll = scroll;
      this.ScrollId = scrollId;
    }

    public Time Scroll { get; set; }

    public string ScrollId { get; set; }

    Type ITypedSearchRequest.ClrType => (Type) null;
  }
}
