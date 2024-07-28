// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentRequestResponse
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentRequestResponse
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public JObject AgentData { get; set; }

    [DataMember]
    public bool Accepted { get; set; }

    [DataMember]
    public AgentRequestMessage ResponseMessage { get; set; }
  }
}
