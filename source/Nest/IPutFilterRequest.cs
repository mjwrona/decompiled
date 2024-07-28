// Decompiled with JetBrains decompiler
// Type: Nest.IPutFilterRequest
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
  [MapsApi("ml.put_filter")]
  public interface IPutFilterRequest : IRequest<PutFilterRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id FilterId { get; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "items")]
    IEnumerable<string> Items { get; set; }
  }
}
