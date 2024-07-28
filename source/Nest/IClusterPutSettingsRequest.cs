// Decompiled with JetBrains decompiler
// Type: Nest.IClusterPutSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("cluster.put_settings.json")]
  [InterfaceDataContract]
  public interface IClusterPutSettingsRequest : 
    IRequest<ClusterPutSettingsRequestParameters>,
    IRequest
  {
    [DataMember(Name = "persistent")]
    IDictionary<string, object> Persistent { get; set; }

    [DataMember(Name = "transient")]
    IDictionary<string, object> Transient { get; set; }
  }
}
