// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RunDeploymentPhaseInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  internal class RunDeploymentPhaseInput
  {
    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public string PlanType { get; set; }

    [DataMember]
    public PipelineGraphNodeReference Stage { get; set; }

    [DataMember]
    public PipelineGraphNodeReference Phase { get; set; }

    [DataMember]
    public long RequestId { get; set; }

    [DataMember]
    public ProviderPhase ProviderPhase { get; set; }

    [DataMember]
    public DeploymentStrategyBase Strategy { get; set; }
  }
}
