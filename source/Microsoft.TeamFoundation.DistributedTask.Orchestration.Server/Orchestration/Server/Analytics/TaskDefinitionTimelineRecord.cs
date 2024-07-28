// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskDefinitionTimelineRecord
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  [DataContract]
  public class TaskDefinitionTimelineRecord
  {
    [DataMember]
    public Guid ProjectGuid { get; set; }

    [DataMember]
    public string PipelineType { get; set; }

    [DataMember]
    public Guid TimelineRecordGuid { get; set; }

    [DataMember]
    public int TaskDefinitionReferenceId { get; set; }

    [DataMember]
    public int PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PlanFinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime PlanLastUpdated { get; set; }

    [DataMember]
    public byte DataSourceId { get; set; }
  }
}
