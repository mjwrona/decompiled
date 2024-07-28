// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AgentRequestData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  public class AgentRequestData
  {
    public long RequestId { get; set; }

    public int PoolId { get; set; }

    public Guid PlanId { get; set; }

    public Guid JobId { get; set; }

    public string JobName { get; set; }

    public DateTime QueueTime { get; set; }

    public DateTime? AssignTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? FinishTime { get; set; }

    public TaskResult? Result { get; set; }

    public int? PipelineId { get; set; }

    public Guid? ProjectGuid { get; set; }

    public string PipelineType { get; set; }

    public int? AgentId { get; set; }

    public DateTime LastUpdated { get; set; }
  }
}
