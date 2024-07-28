// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolMaintenanceSchedule
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskAgentPoolMaintenanceSchedule
  {
    internal TaskAgentPoolMaintenanceSchedule() => this.DaysToBuild = TaskAgentPoolMaintenanceScheduleDays.None;

    private TaskAgentPoolMaintenanceSchedule(
      TaskAgentPoolMaintenanceSchedule maintenanceScheduleToBeCloned)
    {
      this.ScheduleJobId = maintenanceScheduleToBeCloned.ScheduleJobId;
      this.StartHours = maintenanceScheduleToBeCloned.StartHours;
      this.StartMinutes = maintenanceScheduleToBeCloned.StartMinutes;
      this.TimeZoneId = maintenanceScheduleToBeCloned.TimeZoneId;
      this.DaysToBuild = maintenanceScheduleToBeCloned.DaysToBuild;
    }

    [DataMember]
    public Guid ScheduleJobId { get; set; }

    [DataMember]
    public string TimeZoneId { get; set; }

    [DataMember]
    public int StartHours { get; set; }

    [DataMember]
    public int StartMinutes { get; set; }

    [DataMember]
    public TaskAgentPoolMaintenanceScheduleDays DaysToBuild { get; set; }

    public TaskAgentPoolMaintenanceSchedule Clone() => new TaskAgentPoolMaintenanceSchedule(this);
  }
}
