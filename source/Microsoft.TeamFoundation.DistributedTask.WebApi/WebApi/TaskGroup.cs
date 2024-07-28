// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskGroup
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskGroup : TaskDefinition
  {
    [DataMember(Name = "Tasks")]
    private IList<TaskGroupStep> m_tasks;

    public TaskGroup() => this.DefinitionType = "metaTask";

    private TaskGroup(TaskGroup definition)
      : base((TaskDefinition) definition)
    {
      this.DefinitionType = "metaTask";
      this.Owner = definition.Owner;
      this.Revision = definition.Revision;
      this.CreatedOn = definition.CreatedOn;
      this.ModifiedOn = definition.ModifiedOn;
      this.Comment = definition.Comment;
      this.ParentDefinitionId = definition.ParentDefinitionId;
      if (definition.Tasks != null)
        this.Tasks = (IList<TaskGroupStep>) new List<TaskGroupStep>(definition.Tasks.Select<TaskGroupStep, TaskGroupStep>((Func<TaskGroupStep, TaskGroupStep>) (x => x.Clone())));
      if (definition.CreatedBy != null)
        this.CreatedBy = definition.CreatedBy.Clone();
      if (definition.ModifiedBy == null)
        return;
      this.ModifiedBy = definition.ModifiedBy.Clone();
    }

    public IList<TaskGroupStep> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = (IList<TaskGroupStep>) new List<TaskGroupStep>();
        return this.m_tasks;
      }
      set
      {
        if (value == null)
          this.m_tasks = (IList<TaskGroupStep>) new List<TaskGroupStep>();
        else
          this.m_tasks = value;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public DateTime ModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ParentDefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Deleted { get; set; }

    internal TaskGroup Clone() => new TaskGroup(this);
  }
}
