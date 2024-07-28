// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.PoolProviderConfiguration
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class PoolProviderConfiguration
  {
    [DataMember]
    public string PoolProviderProtocolVersion { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string PoolProviderVersion { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string AcquireAgentUrl { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string ReleaseAgentUrl { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAgentDefinitionsUrl { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAgentRequestStatusUrl { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAccountParallelismEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int? AcquisitionTimeout { get; set; }

    public override string ToString() => string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}, {10}: {11}, {12}: {13}, {14}: {15}", (object) "PoolProviderProtocolVersion", (object) this.PoolProviderProtocolVersion, (object) "PoolProviderVersion", (object) this.PoolProviderVersion, (object) "AcquireAgentUrl", (object) this.AcquireAgentUrl, (object) "ReleaseAgentUrl", (object) this.ReleaseAgentUrl, (object) "GetAgentDefinitionsUrl", (object) this.GetAgentDefinitionsUrl, (object) "GetAgentRequestStatusUrl", (object) this.GetAgentRequestStatusUrl, (object) "GetAccountParallelismEndpoint", (object) this.GetAccountParallelismEndpoint, (object) "AcquisitionTimeout", (object) this.AcquisitionTimeout);
  }
}
