// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskGroupUpdateParameter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskGroupUpdateParameter
  {
    [DataMember(Name = "Tasks")]
    private IList<TaskGroupStep> m_tasks;
    [DataMember(Name = "Inputs", EmitDefaultValue = false)]
    private List<TaskInputDefinition> m_inputs;
    [DataMember(Name = "RunsOn", EmitDefaultValue = false)]
    private List<string> m_serializedRunsOn;
    private List<string> m_runsOn;

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FriendlyName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ParentDefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string IconUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string InstanceNameFormat { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Category { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskVersion Version { get; set; }

    public IList<TaskGroupStep> Tasks
    {
      get
      {
        if (this.m_tasks == null)
          this.m_tasks = (IList<TaskGroupStep>) new List<TaskGroupStep>();
        return this.m_tasks;
      }
    }

    public IList<TaskInputDefinition> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new List<TaskInputDefinition>();
        return (IList<TaskInputDefinition>) this.m_inputs;
      }
    }

    public IList<string> RunsOn
    {
      get
      {
        if (this.m_runsOn == null)
          this.m_runsOn = new List<string>((IEnumerable<string>) TaskRunsOnConstants.DefaultValue);
        return (IList<string>) this.m_runsOn;
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_serializedRunsOn, ref this.m_runsOn, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_runsOn, ref this.m_serializedRunsOn);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedRunsOn = (List<string>) null;
  }
}
