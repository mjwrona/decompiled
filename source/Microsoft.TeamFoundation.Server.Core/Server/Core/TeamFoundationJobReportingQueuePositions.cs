// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingQueuePositions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationJobReportingQueuePositions
  {
    public int QueuePosition { get; set; }

    public int StateTime { get; set; }

    public int Priority { get; set; }

    public Guid JobId { get; set; }

    public Guid HostId { get; set; }

    public DateTime QueueTime { get; set; }

    public string JobName { get; set; }

    public string HostName { get; set; }

    public override string ToString() => string.Format("QueuePosition: '{1}'{0}StateTime: '{2}'{0}Priority: '{3}'{0}JobId: '{4}'{0}JobSource: '{5}'{0}", (object) Environment.NewLine, (object) this.QueuePosition, (object) this.StateTime, (object) this.Priority, (object) this.JobId, (object) this.HostId);
  }
}
