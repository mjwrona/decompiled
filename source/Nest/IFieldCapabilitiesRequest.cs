// Decompiled with JetBrains decompiler
// Type: Nest.IFieldCapabilitiesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("field_caps.json")]
  public interface IFieldCapabilitiesRequest : IRequest<FieldCapabilitiesRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Indices Index { get; }

    [DataMember(Name = "index_filter")]
    QueryContainer IndexFilter { get; set; }
  }
}
