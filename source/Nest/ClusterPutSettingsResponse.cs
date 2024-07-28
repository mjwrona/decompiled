// Decompiled with JetBrains decompiler
// Type: Nest.ClusterPutSettingsResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterPutSettingsResponse : ResponseBase
  {
    [DataMember(Name = "acknowledged")]
    public bool Acknowledged { get; internal set; }

    [DataMember(Name = "persistent")]
    public IReadOnlyDictionary<string, object> Persistent { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;

    [DataMember(Name = "transient")]
    public IReadOnlyDictionary<string, object> Transient { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;
  }
}
