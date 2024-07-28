// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.GeometryValidationResult
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents.Spatial
{
  [JsonObject(MemberSerialization.OptIn)]
  public class GeometryValidationResult
  {
    [JsonProperty("valid", Required = Required.Always, Order = 0)]
    public bool IsValid { get; private set; }

    [JsonProperty("reason", Order = 1)]
    public string Reason { get; private set; }
  }
}
