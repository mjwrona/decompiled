// Decompiled with JetBrains decompiler
// Type: Nest.IPostJobDataRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.post_data.json")]
  [JsonFormatter(typeof (PostJobDataFormatter))]
  public interface IPostJobDataRequest : IRequest<PostJobDataRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id JobId { get; }

    [IgnoreDataMember]
    IEnumerable<object> Data { get; set; }
  }
}
