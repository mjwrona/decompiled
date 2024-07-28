// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolMaintenanceOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskAgentPoolMaintenanceOptions
  {
    internal TaskAgentPoolMaintenanceOptions()
    {
    }

    private TaskAgentPoolMaintenanceOptions(
      TaskAgentPoolMaintenanceOptions maintenanceOptionToBeCloned)
    {
      this.WorkingDirectoryExpirationInDays = maintenanceOptionToBeCloned.WorkingDirectoryExpirationInDays;
    }

    [DataMember]
    public int WorkingDirectoryExpirationInDays { get; set; }

    public TaskAgentPoolMaintenanceOptions Clone() => new TaskAgentPoolMaintenanceOptions(this);
  }
}
