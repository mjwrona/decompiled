// Decompiled with JetBrains decompiler
// Type: Nest.IUpdateIndexSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("indices.put_settings.json")]
  [JsonFormatter(typeof (UpdateIndexSettingsRequestFormatter))]
  [InterfaceDataContract]
  public interface IUpdateIndexSettingsRequest : 
    IRequest<UpdateIndexSettingsRequestParameters>,
    IRequest
  {
    IDynamicIndexSettings IndexSettings { get; set; }

    [IgnoreDataMember]
    Indices Index { get; }
  }
}
