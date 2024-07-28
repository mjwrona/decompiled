// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DesignerSchedule
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class DesignerSchedule : BuildSchedule
  {
    public IReadOnlyList<string> BranchDetails { get; }

    public DesignerScheduleDetails ScheduleDetails { get; }

    public DesignerSchedule()
      : base(ScheduleType.Designer)
    {
    }

    public DesignerSchedule(
      int scheduleId,
      List<string> branchDetails,
      DesignerScheduleDetails scheduleDetails,
      Guid scheduleJobId,
      bool scheduleOnlyWithChanges,
      bool batch,
      int definitionId)
      : base(scheduleId, (string) null, ScheduleType.Designer, scheduleJobId, scheduleOnlyWithChanges, batch, definitionId)
    {
      this.BranchDetails = (IReadOnlyList<string>) branchDetails;
      this.ScheduleDetails = scheduleDetails;
    }
  }
}
