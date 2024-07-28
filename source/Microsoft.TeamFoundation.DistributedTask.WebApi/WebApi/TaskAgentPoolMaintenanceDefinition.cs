// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolMaintenanceDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentPoolMaintenanceDefinition
  {
    [DataMember(EmitDefaultValue = false, Name = "Options")]
    public TaskAgentPoolMaintenanceOptions m_options;
    [DataMember(EmitDefaultValue = false, Name = "RetentionPolicy")]
    private TaskAgentPoolMaintenanceRetentionPolicy m_retentionPolicy;
    [DataMember(EmitDefaultValue = false, Name = "ScheduleSetting")]
    private TaskAgentPoolMaintenanceSchedule m_scheduleSetting;

    internal TaskAgentPoolMaintenanceDefinition()
    {
    }

    private TaskAgentPoolMaintenanceDefinition(
      TaskAgentPoolMaintenanceDefinition maintenanceDefinitionToBeCloned)
    {
      this.Enabled = maintenanceDefinitionToBeCloned.Enabled;
      this.JobTimeoutInMinutes = maintenanceDefinitionToBeCloned.JobTimeoutInMinutes;
      this.MaxConcurrentAgentsPercentage = maintenanceDefinitionToBeCloned.MaxConcurrentAgentsPercentage;
      if (maintenanceDefinitionToBeCloned.Pool != null)
        this.Pool = new TaskAgentPoolReference()
        {
          Id = maintenanceDefinitionToBeCloned.Pool.Id,
          Name = maintenanceDefinitionToBeCloned.Pool.Name,
          Scope = maintenanceDefinitionToBeCloned.Pool.Scope,
          PoolType = maintenanceDefinitionToBeCloned.Pool.PoolType
        };
      this.m_options = maintenanceDefinitionToBeCloned.Options.Clone();
      this.m_retentionPolicy = maintenanceDefinitionToBeCloned.RetentionPolicy.Clone();
      this.m_scheduleSetting = maintenanceDefinitionToBeCloned.ScheduleSetting.Clone();
    }

    [DataMember]
    public int Id { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolReference Pool { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public int JobTimeoutInMinutes { get; set; }

    [DataMember]
    public int MaxConcurrentAgentsPercentage { get; set; }

    public TaskAgentPoolMaintenanceOptions Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new TaskAgentPoolMaintenanceOptions()
          {
            WorkingDirectoryExpirationInDays = 0
          };
        return this.m_options;
      }
      internal set => this.m_options = value;
    }

    public TaskAgentPoolMaintenanceRetentionPolicy RetentionPolicy
    {
      get
      {
        if (this.m_retentionPolicy == null)
          this.m_retentionPolicy = new TaskAgentPoolMaintenanceRetentionPolicy()
          {
            NumberOfHistoryRecordsToKeep = 1
          };
        return this.m_retentionPolicy;
      }
      internal set => this.m_retentionPolicy = value;
    }

    public TaskAgentPoolMaintenanceSchedule ScheduleSetting
    {
      get
      {
        if (this.m_scheduleSetting == null)
          this.m_scheduleSetting = new TaskAgentPoolMaintenanceSchedule()
          {
            DaysToBuild = TaskAgentPoolMaintenanceScheduleDays.None
          };
        return this.m_scheduleSetting;
      }
      internal set => this.m_scheduleSetting = value;
    }

    public TaskAgentPoolMaintenanceDefinition Clone() => new TaskAgentPoolMaintenanceDefinition(this);
  }
}
