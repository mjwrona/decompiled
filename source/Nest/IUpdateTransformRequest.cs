// Decompiled with JetBrains decompiler
// Type: Nest.IUpdateTransformRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("transform.update_transform.json")]
  public interface IUpdateTransformRequest : IRequest<UpdateTransformRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id TransformId { get; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "source")]
    ITransformSource Source { get; set; }

    [DataMember(Name = "dest")]
    ITransformDestination Destination { get; set; }

    [DataMember(Name = "frequency")]
    Time Frequency { get; set; }

    [DataMember(Name = "sync")]
    ITransformSyncContainer Sync { get; set; }

    [DataMember(Name = "settings")]
    ITransformSettings Settings { get; set; }
  }
}
