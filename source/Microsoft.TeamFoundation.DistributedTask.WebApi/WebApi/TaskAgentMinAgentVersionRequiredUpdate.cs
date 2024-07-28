// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentMinAgentVersionRequiredUpdate
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentMinAgentVersionRequiredUpdate : TaskAgentUpdateReason
  {
    [JsonConstructor]
    internal TaskAgentMinAgentVersionRequiredUpdate()
      : base(TaskAgentUpdateReasonType.MinAgentVersionRequired)
    {
    }

    private TaskAgentMinAgentVersionRequiredUpdate(
      TaskAgentMinAgentVersionRequiredUpdate updateToBeCloned)
      : base(TaskAgentUpdateReasonType.MinAgentVersionRequired)
    {
      if (updateToBeCloned.MinAgentVersion != null)
        this.MinAgentVersion = updateToBeCloned.MinAgentVersion.Clone();
      if (updateToBeCloned.JobDefinition != null)
        this.JobDefinition = updateToBeCloned.JobDefinition.Clone();
      if (updateToBeCloned.JobOwner == null)
        return;
      this.JobOwner = updateToBeCloned.JobOwner.Clone();
    }

    [DataMember]
    public Demand MinAgentVersion { get; set; }

    [DataMember]
    public TaskOrchestrationOwner JobDefinition { get; set; }

    [DataMember]
    public TaskOrchestrationOwner JobOwner { get; set; }

    public TaskAgentMinAgentVersionRequiredUpdate Clone() => new TaskAgentMinAgentVersionRequiredUpdate(this);
  }
}
