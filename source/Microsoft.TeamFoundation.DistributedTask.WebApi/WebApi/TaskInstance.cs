// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskInstance
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskInstance : TaskReference
  {
    private IDictionary<string, string> m_environment;
    [DataMember(EmitDefaultValue = false, Name = "Environment")]
    private IDictionary<string, string> m_serializedEnvironment;

    public TaskInstance() => this.Enabled = true;

    private TaskInstance(TaskInstance taskToBeCloned)
      : base((TaskReference) taskToBeCloned)
    {
      this.InstanceId = taskToBeCloned.InstanceId;
      this.DisplayName = taskToBeCloned.DisplayName;
      this.Enabled = taskToBeCloned.Enabled;
      this.Condition = taskToBeCloned.Condition;
      this.ContinueOnError = taskToBeCloned.ContinueOnError;
      this.AlwaysRun = taskToBeCloned.AlwaysRun;
      this.TimeoutInMinutes = taskToBeCloned.TimeoutInMinutes;
      this.RetryCountOnTaskFailure = taskToBeCloned.RetryCountOnTaskFailure;
      this.RefName = taskToBeCloned.RefName;
      if (taskToBeCloned.m_environment == null)
        return;
      this.m_environment = (IDictionary<string, string>) new Dictionary<string, string>(taskToBeCloned.m_environment, (IEqualityComparer<string>) StringComparer.Ordinal);
    }

    [DataMember]
    public Guid InstanceId { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public string Condition { get; set; }

    [DataMember]
    public bool ContinueOnError { get; set; }

    [DataMember]
    public bool AlwaysRun { get; set; }

    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [DataMember]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember]
    public string RefName { get; set; }

    public IDictionary<string, string> Environment
    {
      get
      {
        if (this.m_environment == null)
          this.m_environment = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return this.m_environment;
      }
    }

    public override TaskReference Clone() => (TaskReference) new TaskInstance(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedEnvironment, ref this.m_environment, (IEqualityComparer<string>) StringComparer.Ordinal, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_environment, ref this.m_serializedEnvironment);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedEnvironment = (IDictionary<string, string>) null;
  }
}
