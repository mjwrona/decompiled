// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.Compute
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Util;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class Compute
  {
    [JsonConstructor]
    public Compute(
      string vMId,
      string location,
      string sKU,
      string azEnvironment,
      string oSType,
      string vMSize)
    {
      this.Location = location;
      this.SKU = sKU;
      this.AzEnvironment = azEnvironment;
      this.OSType = oSType;
      this.VMSize = vMSize;
      this.VMId = "hashedVmId:" + HashingExtension.ComputeHash(vMId);
    }

    [JsonProperty(PropertyName = "location")]
    internal string Location { get; }

    [JsonProperty(PropertyName = "sku")]
    internal string SKU { get; }

    [JsonProperty(PropertyName = "azEnvironment")]
    internal string AzEnvironment { get; }

    [JsonProperty(PropertyName = "osType")]
    internal string OSType { get; }

    [JsonProperty(PropertyName = "vmSize")]
    internal string VMSize { get; }

    [JsonProperty(PropertyName = "vmId")]
    internal string VMId { get; }
  }
}
