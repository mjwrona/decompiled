// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.GeometryValidationResult
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  [JsonObject(MemberSerialization.OptIn)]
  public class GeometryValidationResult
  {
    [DataMember(Name = "valid")]
    [JsonProperty("valid", Required = Required.Always, Order = 0)]
    public bool IsValid { get; private set; }

    [DataMember(Name = "reason")]
    [JsonProperty("reason", Order = 1)]
    public string Reason { get; private set; }
  }
}
