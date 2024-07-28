// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentUpdateReasonData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskAgentUpdateReasonData
  {
    internal TaskAgentUpdateReasonData()
    {
    }

    internal TaskAgentUpdateReasonType Reason { get; set; }

    internal Guid ServiceOwner { get; set; }

    internal Guid HostId { get; set; }

    internal Guid ScopeId { get; set; }

    internal string PlanType { get; set; }

    internal TaskOrchestrationOwner DefinitionReference { get; set; }

    internal TaskOrchestrationOwner OwnerReference { get; set; }

    internal Demand MinAgentVersion { get; set; }
  }
}
