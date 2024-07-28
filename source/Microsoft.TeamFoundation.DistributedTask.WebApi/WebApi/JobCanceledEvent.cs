// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobCanceledEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class JobCanceledEvent : JobEvent
  {
    internal JobCanceledEvent()
      : base("JobCanceled")
    {
    }

    public JobCanceledEvent(Guid jobId)
      : this(jobId, (string) null, new TimeSpan())
    {
    }

    public JobCanceledEvent(Guid jobId, string reason)
      : this(jobId, reason, new TimeSpan())
    {
    }

    public JobCanceledEvent(Guid jobId, string reason, TimeSpan timeout)
      : base("JobCanceled", jobId)
    {
      this.Reason = reason;
      this.Timeout = timeout;
    }

    [DataMember]
    public string Reason { get; set; }

    [DataMember]
    public TimeSpan Timeout { get; set; }
  }
}
