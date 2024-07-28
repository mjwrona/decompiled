// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [KnownType(typeof (JobAssignedEvent))]
  [KnownType(typeof (JobCanceledEvent))]
  [KnownType(typeof (JobCompletedEvent))]
  [KnownType(typeof (JobStartedEvent))]
  [KnownType(typeof (JobMetadataEvent))]
  [KnownType(typeof (TaskAssignedEvent))]
  [KnownType(typeof (TaskStartedEvent))]
  [KnownType(typeof (TaskCompletedEvent))]
  [KnownType(typeof (TaskLocalExecutionCompletedEvent))]
  [JsonConverter(typeof (JobEventJsonConverter))]
  public abstract class JobEvent
  {
    protected JobEvent(string name) => this.Name = name;

    protected JobEvent(string name, Guid jobId)
    {
      this.Name = name;
      this.JobId = jobId;
    }

    [DataMember]
    public string Name { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid JobId { get; set; }
  }
}
