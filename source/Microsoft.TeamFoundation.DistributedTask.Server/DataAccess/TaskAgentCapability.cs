// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentCapability
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentCapability
  {
    public int AgentId { get; set; }

    public TaskAgentCapabilityType CapabilityType { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }

    public bool IsWellKnownCapability() => this.Name.Equals(PipelineConstants.AgentName, StringComparison.OrdinalIgnoreCase) || this.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase);
  }
}
