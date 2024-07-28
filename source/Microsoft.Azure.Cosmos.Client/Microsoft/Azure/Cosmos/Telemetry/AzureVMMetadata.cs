// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.AzureVMMetadata
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class AzureVMMetadata
  {
    public AzureVMMetadata(Compute compute) => this.Compute = compute;

    [JsonProperty(PropertyName = "compute")]
    internal Compute Compute { get; }
  }
}
