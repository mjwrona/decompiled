// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Schedule2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class Schedule2
  {
    public int ScheduleId { get; set; }

    public string ScheduleName { get; set; }

    public ScheduleType ScheduleType { get; set; }

    public List<string> BranchDetails { get; set; }

    public DesignerScheduleDetails ScheduleDetails { get; set; }

    public Guid ScheduleJobId { get; set; }

    public bool ScheduleOnlyWithChanges { get; set; }

    public bool Batch { get; set; }

    public int DefinitionId { get; set; }

    public Schedule2()
    {
    }

    public Schedule2(
      int scheduleId,
      string scheduleName,
      ScheduleType scheduleType,
      List<string> branchDetails,
      DesignerScheduleDetails scheduleDetails,
      Guid scheduleJobId,
      bool scheduleOnlyWithChanges,
      bool batch,
      int definitionId)
    {
      this.ScheduleId = scheduleId;
      this.ScheduleName = scheduleName;
      this.ScheduleType = scheduleType;
      this.BranchDetails = branchDetails;
      this.ScheduleDetails = scheduleDetails;
      this.ScheduleJobId = scheduleJobId;
      this.ScheduleOnlyWithChanges = scheduleOnlyWithChanges;
      this.Batch = batch;
      this.DefinitionId = definitionId;
    }
  }
}
