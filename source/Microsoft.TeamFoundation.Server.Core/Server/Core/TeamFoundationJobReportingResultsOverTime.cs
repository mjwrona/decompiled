// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingResultsOverTime
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationJobReportingResultsOverTime
  {
    public Guid JobId { get; set; }

    public int SucceededCount { get; set; }

    public int PartiallySucceededCount { get; set; }

    public int FailedCount { get; set; }

    public int StoppedCount { get; set; }

    public int KilledCount { get; set; }

    public int BlockedCount { get; set; }

    public int ExtensionNotFoundCount { get; set; }

    public int InactiveCount { get; set; }

    public int DisabledCount { get; set; }

    public int TotalCount { get; set; }

    public override string ToString() => string.Format("JobId: '{1}'{0}SucceededCount: '{2}'{0}PartiallySucceededCount: '{3}'{0}FailedCount: '{4}'{0}StoppedCount: '{5}'{0}KilledCount: '{6}'{0}BlockedCount: '{7}'{0}ExtensionNotFoundCount: '{8}'{0}InactiveCount: '{9}'{0}DisabledCount: '{10}'{0}TotalCount: '{11}'{0}", (object) Environment.NewLine, (object) this.JobId, (object) this.SucceededCount, (object) this.PartiallySucceededCount, (object) this.FailedCount, (object) this.StoppedCount, (object) this.KilledCount, (object) this.BlockedCount, (object) this.ExtensionNotFoundCount, (object) this.InactiveCount, (object) this.DisabledCount, (object) this.TotalCount);
  }
}
