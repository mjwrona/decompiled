// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentQueue
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [KnownType(typeof (TaskAgentPool))]
  public class TaskAgentQueue
  {
    public TaskAgentQueue()
    {
    }

    private TaskAgentQueue(TaskAgentQueue queueToBeCloned)
    {
      this.Id = queueToBeCloned.Id;
      this.ProjectId = queueToBeCloned.ProjectId;
      this.Name = queueToBeCloned.Name;
      this.GroupScopeId = queueToBeCloned.GroupScopeId;
      this.Provisioned = queueToBeCloned.Provisioned;
      if (queueToBeCloned.Pool == null)
        return;
      this.Pool = queueToBeCloned.Pool.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolReference Pool { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public Guid GroupScopeId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public bool Provisioned { get; set; }

    public TaskAgentQueue Clone() => new TaskAgentQueue(this);
  }
}
