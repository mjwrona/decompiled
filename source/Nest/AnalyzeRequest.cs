// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AnalyzeRequest : 
    PlainRequestBase<AnalyzeRequestParameters>,
    IAnalyzeRequest,
    IRequest<AnalyzeRequestParameters>,
    IRequest
  {
    public AnalyzeRequest(IndexName indices, string textToAnalyze)
      : this(indices)
    {
      this.Text = (IEnumerable<string>) new string[1]
      {
        textToAnalyze
      };
    }

    public string Analyzer { get; set; }

    public IEnumerable<string> Attributes { get; set; }

    public AnalyzeCharFilters CharFilter { get; set; }

    public bool? Explain { get; set; }

    public Field Field { get; set; }

    public AnalyzeTokenFilters Filter { get; set; }

    public string Normalizer { get; set; }

    public IEnumerable<string> Text { get; set; }

    public Union<string, ITokenizer> Tokenizer { get; set; }

    protected IAnalyzeRequest Self => (IAnalyzeRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesAnalyze;

    public AnalyzeRequest()
    {
    }

    public AnalyzeRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    IndexName IAnalyzeRequest.Index => this.Self.RouteValues.Get<IndexName>("index");
  }
}
