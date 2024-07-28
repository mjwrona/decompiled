// Decompiled with JetBrains decompiler
// Type: Nest.IStopDatafeedRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.stop_datafeed.json")]
  public interface IStopDatafeedRequest : IRequest<StopDatafeedRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id DatafeedId { get; }

    [DataMember(Name = "force")]
    bool? Force { get; set; }

    [DataMember(Name = "timeout")]
    Time Timeout { get; set; }
  }
}
