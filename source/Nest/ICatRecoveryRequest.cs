// Decompiled with JetBrains decompiler
// Type: Nest.ICatRecoveryRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("cat.recovery.json")]
  [InterfaceDataContract]
  public interface ICatRecoveryRequest : IRequest<CatRecoveryRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Indices Index { get; }
  }
}
