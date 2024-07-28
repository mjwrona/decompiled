// Decompiled with JetBrains decompiler
// Type: Nest.RootNodeInfoResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class RootNodeInfoResponse : ResponseBase
  {
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "cluster_name")]
    public string ClusterName { get; internal set; }

    [DataMember(Name = "cluster_uuid")]
    public string ClusterUUID { get; internal set; }

    [DataMember(Name = "version")]
    public ElasticsearchVersionInfo Version { get; internal set; }

    [DataMember(Name = "tagline")]
    public string Tagline { get; internal set; }
  }
}
