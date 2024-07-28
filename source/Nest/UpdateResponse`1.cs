// Decompiled with JetBrains decompiler
// Type: Nest.UpdateResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class UpdateResponse<TDocument> : 
    WriteResponseBase,
    IUpdateResponse<TDocument>,
    IResponse,
    IElasticsearchResponse
    where TDocument : class
  {
    public override bool IsValid => base.IsValid && this.Result != Result.NotFound && this.Result != 0;

    [DataMember(Name = "get")]
    public IInlineGet<TDocument> Get { get; internal set; }
  }
}
