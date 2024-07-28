// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSchedule
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildSchedule
  {
    public int ScheduleId { get; }

    public string ScheduleName { get; }

    public ScheduleType ScheduleType { get; }

    public Guid ScheduleJobId { get; }

    public bool ScheduleOnlyWithChanges { get; }

    public bool Batch { get; }

    public int DefinitionId { get; }

    protected BuildSchedule(ScheduleType scheduleType) => this.ScheduleType = scheduleType;

    public BuildSchedule(
      int scheduleId,
      string scheduleName,
      ScheduleType scheduleType,
      Guid scheduleJobId,
      bool scheduleOnlyWithChanges,
      bool batch,
      int definitionId)
    {
      this.ScheduleId = scheduleId;
      this.ScheduleName = scheduleName;
      this.ScheduleType = scheduleType;
      this.ScheduleJobId = scheduleJobId;
      this.ScheduleOnlyWithChanges = scheduleOnlyWithChanges;
      this.Batch = batch;
      this.DefinitionId = definitionId;
    }
  }
}
