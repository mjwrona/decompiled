// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.WorkflowCancellationData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class WorkflowCancellationData
  {
    public string AgentDisplayName { get; set; }

    public int ReservationId { get; set; }

    public int ReservedAgentId { get; set; }

    public string MessageQueueUrl { get; set; }

    public string EndpointUrl { get; set; }
  }
}
