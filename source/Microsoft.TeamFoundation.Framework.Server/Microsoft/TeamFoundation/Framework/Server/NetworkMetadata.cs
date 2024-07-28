// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NetworkMetadata
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  public class NetworkMetadata
  {
    [JsonProperty(PropertyName = "interface")]
    public List<NetworkMetadata.InterfaceMetadata> Interfaces { get; set; }

    [JsonObject(MemberSerialization.OptIn)]
    public class InterfaceMetadata
    {
      [JsonProperty(PropertyName = "ipv4")]
      public NetworkMetadata.IPv4Metadata IPv4 { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class IPv4Metadata
    {
      [JsonProperty(PropertyName = "ipAddress")]
      public List<NetworkMetadata.IPAddressMetadata> IPAddress { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class IPAddressMetadata
    {
      [JsonProperty(PropertyName = "privateIpAddress")]
      public string PrivateIpAddress { get; set; }

      [JsonProperty(PropertyName = "publicIpAddress")]
      public string PublicIpAddress { get; set; }
    }
  }
}
