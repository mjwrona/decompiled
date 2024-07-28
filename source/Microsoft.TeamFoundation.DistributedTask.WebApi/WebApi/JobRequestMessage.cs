// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobRequestMessage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [JsonConverter(typeof (JobRequestMessageJsonConverter))]
  public abstract class JobRequestMessage
  {
    protected JobRequestMessage(string messageType) => this.MessageType = messageType;

    protected JobRequestMessage(
      string messageType,
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      Guid jobId,
      string jobName,
      string jobRefName,
      JobEnvironment environment)
    {
      this.MessageType = messageType;
      this.Plan = plan;
      this.JobId = jobId;
      this.JobName = jobName;
      this.JobRefName = jobRefName;
      this.Timeline = timeline;
      this.Environment = environment;
    }

    [DataMember]
    public string MessageType { get; private set; }

    [DataMember]
    public TaskOrchestrationPlanReference Plan { get; private set; }

    [DataMember]
    public TimelineReference Timeline { get; private set; }

    [DataMember]
    public Guid JobId { get; private set; }

    [DataMember]
    public string JobName { get; private set; }

    [DataMember]
    public string JobRefName { get; private set; }

    [DataMember]
    public JobEnvironment Environment { get; private set; }
  }
}
