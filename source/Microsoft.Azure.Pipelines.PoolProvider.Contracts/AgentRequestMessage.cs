// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentRequestMessage
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentRequestMessage
  {
    public AgentRequestMessage()
    {
    }

    public AgentRequestMessage(string message, AgentRequestMessageVerbosity verbosity)
    {
      this.Message = message;
      this.Verbosity = verbosity;
    }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public AgentRequestMessageVerbosity Verbosity { get; set; }
  }
}
