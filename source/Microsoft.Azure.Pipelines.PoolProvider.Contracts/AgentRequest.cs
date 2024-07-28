// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentRequest
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentRequest
  {
    [DataMember]
    public Guid AgentId { get; set; }

    [DataMember]
    public string AgentPool { get; set; }

    [DataMember]
    public Guid AccountId { get; set; }

    [DataMember]
    public Guid AgentCloudId { get; set; }

    [DataMember]
    public string AuthenticationToken { get; set; }

    [DataMember]
    public string UpdateRequestUrl { get; set; }

    [DataMember]
    public string AppendRequestMessageUrl { get; set; }

    [DataMember]
    public string GetAssociatedJobUrl { get; set; }

    [DataMember]
    public AgentConfiguration AgentConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public JObject AgentSpecification { get; set; }

    [DataMember]
    public bool IsScheduled { get; set; }

    [DataMember]
    public string ProjectType { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int? Timeout { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int? MaxParallelism { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public DateTime? ValidUntil { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid? SourceCorrelationId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public AgentRequestJob AssociatedJob { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public IList<string> Demands { get; set; }
  }
}
