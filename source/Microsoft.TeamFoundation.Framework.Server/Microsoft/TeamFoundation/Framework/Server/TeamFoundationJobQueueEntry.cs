// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobQueueEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationJobQueueEntry : TeamFoundationJobExecutionEntry
  {
    public int Priority { get; set; }

    public int NextRun { get; set; }

    public TeamFoundationJobState State { get; set; }

    public DateTime StateChangeTime { get; set; }

    public TeamFoundationJobResult JobLastResult { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[JobSource: {0}, JobId: {1}, QueueTime: {2}, ExecutionStartTime: {3}, State: {4}, AgentId: {5}, QueuedReasons: {6}, Priority: {7}, NextRun: {8}, StateChangeTime: {9}]", (object) this.JobSource, (object) this.JobId, (object) this.QueueTime, (object) this.ExecutionStartTime, (object) this.State, (object) this.AgentId, (object) this.QueuedReasons, (object) this.Priority, (object) this.NextRun, (object) this.StateChangeTime);
  }
}
