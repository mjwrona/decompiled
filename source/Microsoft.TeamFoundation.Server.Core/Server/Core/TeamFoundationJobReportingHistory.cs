// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingHistory
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationJobReportingHistory
  {
    public long HistoryId { get; set; }

    public Guid JobSource { get; set; }

    public Guid JobId { get; set; }

    public DateTime QueueTime { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Guid AgentId { get; set; }

    public TeamFoundationJobResult Result { get; set; }

    public string ResultMessage { get; set; }

    public TeamFoundationJobQueuedReasons QueuedReasons { get; set; }

    public int QueueFlags { get; set; }

    public int Priority { get; set; }

    public string QueueReasonsString => this.QueuedReasons.ToString();

    public TimeSpan RunDuration => this.EndTime - this.StartTime;

    public TimeSpan QueueDuration => this.StartTime - this.QueueTime;

    public string ResultString => this.Result.ToString();

    public override string ToString() => string.Format("HistoryId: '{1}'{0}JobSource: '{2}'{0}JobId: '{3}'{0}QueueTime: '{4}'{0}StartTime: '{5}'{0}EndTime: '{6}'{0}AgentId: '{7}'{0}Result: '{8}'{0}ResultMessage: '{9}'{0}QueuedReasons: '{10}'{0}QueueFlags: '{11}'{0}Priority: '{12}'{0}", (object) Environment.NewLine, (object) this.HistoryId, (object) this.JobSource, (object) this.JobId, (object) this.QueueTime, (object) this.StartTime, (object) this.EndTime, (object) this.AgentId, (object) this.Result, (object) this.ResultMessage, (object) this.QueuedReasons, (object) this.QueueFlags, (object) this.Priority);
  }
}
