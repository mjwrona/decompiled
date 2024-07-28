// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.JobInstance
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class JobInstance
  {
    [DataMember(Name = "Outputs")]
    private VariablesDictionary m_outputs;

    public JobInstance()
      : this(string.Empty)
    {
    }

    public JobInstance(string name)
      : this(name, 1)
    {
    }

    public JobInstance(string name, int attempt)
    {
      this.Name = name;
      this.Attempt = attempt;
    }

    public JobInstance(string name, TaskResult result)
      : this(name)
    {
      this.Result = new TaskResult?(result);
    }

    public JobInstance(Job job)
      : this(job, 1)
    {
    }

    public JobInstance(Job job, int attempt)
      : this(job.Name, attempt)
    {
      this.Definition = job;
      this.State = PipelineState.NotStarted;
    }

    [DataMember]
    public string Identifier { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public PipelineState State { get; set; }

    [DataMember]
    public TaskResult? Result { get; set; }

    [DataMember]
    public Job Definition { get; set; }

    [DataMember]
    public bool DeliveryFailed { get; set; }

    [DataMember]
    public int CheckRerunAttempt { get; set; }

    public IDictionary<string, VariableValue> Outputs
    {
      get
      {
        if (this.m_outputs == null)
          this.m_outputs = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_outputs;
      }
    }
  }
}
