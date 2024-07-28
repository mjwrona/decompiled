// Decompiled with JetBrains decompiler
// Type: Nest.IPutEnrichPolicyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.EnrichApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("enrich.put_policy")]
  [ReadAs(typeof (PutEnrichPolicyRequest))]
  public interface IPutEnrichPolicyRequest : IRequest<PutEnrichPolicyRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Name Name { get; }

    [DataMember(Name = "match")]
    IEnrichPolicy Match { get; set; }

    [DataMember(Name = "geo_match")]
    IEnrichPolicy GeoMatch { get; set; }
  }
}
