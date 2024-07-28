// Decompiled with JetBrains decompiler
// Type: Nest.ExplainResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ExplainResponse<TDocument> : 
    ResponseBase,
    IExplainResponse<TDocument>,
    IResponse,
    IElasticsearchResponse
    where TDocument : class
  {
    [DataMember(Name = "explanation")]
    public ExplanationDetail Explanation { get; internal set; }

    [DataMember(Name = "get")]
    public IInlineGet<TDocument> Get { get; internal set; }

    [DataMember(Name = "matched")]
    public bool Matched { get; internal set; }
  }
}
