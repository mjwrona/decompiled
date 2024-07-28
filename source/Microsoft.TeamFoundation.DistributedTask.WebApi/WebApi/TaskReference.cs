// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskReference : ITaskDefinitionReference
  {
    private IDictionary<string, string> m_inputs;
    [DataMember(EmitDefaultValue = false, Name = "Inputs")]
    private IDictionary<string, string> m_serializedInputs;

    public TaskReference()
    {
    }

    protected TaskReference(TaskReference taskToBeCloned)
    {
      this.Id = taskToBeCloned.Id;
      this.Name = taskToBeCloned.Name;
      this.Version = taskToBeCloned.Version;
      if (taskToBeCloned.m_inputs == null)
        return;
      this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>(taskToBeCloned.m_inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Version { get; set; }

    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_inputs;
      }
    }

    public virtual TaskReference Clone() => new TaskReference(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedInputs, ref this.m_inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_inputs, ref this.m_serializedInputs);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedInputs = (IDictionary<string, string>) null;
  }
}
