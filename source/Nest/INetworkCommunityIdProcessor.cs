// Decompiled with JetBrains decompiler
// Type: Nest.INetworkCommunityIdProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface INetworkCommunityIdProcessor : IProcessor
  {
    [DataMember(Name = "destination_ip")]
    Field DestinationIp { get; set; }

    [DataMember(Name = "destination_port")]
    Field DestinationPort { get; set; }

    [DataMember(Name = "iana_number")]
    Field IanaNumber { get; set; }

    [DataMember(Name = "icmp_type")]
    Field IcmpType { get; set; }

    [DataMember(Name = "icmp_code")]
    Field IcmpCode { get; set; }

    [DataMember(Name = "ignore_missing")]
    bool? IgnoreMissing { get; set; }

    [DataMember(Name = "seed")]
    int? Seed { get; set; }

    [DataMember(Name = "source_ip")]
    Field SourceIp { get; set; }

    [DataMember(Name = "source_port")]
    Field SourcePort { get; set; }

    [DataMember(Name = "target_field")]
    Field TargetField { get; set; }

    [DataMember(Name = "transport")]
    Field Transport { get; set; }
  }
}
