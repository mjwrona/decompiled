// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ResourceUsage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class ResourceUsage
  {
    [DataMember(Name = "RunningRequests", EmitDefaultValue = false)]
    private IList<TaskAgentJobRequest> m_runningRequests;

    [DataMember]
    public ResourceLimit ResourceLimit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? UsedCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? UsedMinutes { get; set; }

    public IList<TaskAgentJobRequest> RunningRequests
    {
      get
      {
        if (this.m_runningRequests == null)
          this.m_runningRequests = (IList<TaskAgentJobRequest>) new List<TaskAgentJobRequest>();
        return this.m_runningRequests;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IList<TaskAgentJobRequest> runningRequests = this.m_runningRequests;
      if ((runningRequests != null ? (runningRequests.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_runningRequests = (IList<TaskAgentJobRequest>) null;
    }
  }
}
