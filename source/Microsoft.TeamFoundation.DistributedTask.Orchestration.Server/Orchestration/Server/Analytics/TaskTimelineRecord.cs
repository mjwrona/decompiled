// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskTimelineRecord
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  [DataContract]
  public class TaskTimelineRecord
  {
    [DataMember]
    public Guid ProjectGuid { get; set; }

    [DataMember]
    public Guid TimelineRecordGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ParentRecordGuid { get; set; }

    [DataMember]
    public string PipelineType { get; set; }

    [DataMember]
    public int TimelineId { get; set; }

    [DataMember]
    public int PlanId { get; set; }

    [IgnoreDataMember]
    public Guid PlanGuidId { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RefName { get; set; }

    [DataMember]
    public int ChangeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Order { get; set; }

    [DataMember]
    public int State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string WorkerName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string LogPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? LogLineCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PlanFinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TaskDefinitionReferenceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime PlanLastUpdated { get; set; }

    [DataMember]
    public byte DataSourceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Attempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Identifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string JobIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string JobName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PhaseIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PhaseName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsJobFailureDueToTask { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public List<Issue> Issues { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public DateTime? AgentQueueTime { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public DateTime? AgentStartTime { get; set; }

    public string GetKey() => this.GenerateKeyWithGuid(new Guid?(this.TimelineRecordGuid));

    public string GetParentKey()
    {
      if (this.ParentRecordGuid.HasValue)
      {
        Guid? parentRecordGuid = this.ParentRecordGuid;
        Guid empty = Guid.Empty;
        if ((parentRecordGuid.HasValue ? (parentRecordGuid.HasValue ? (parentRecordGuid.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
          return this.GenerateKeyWithGuid(this.ParentRecordGuid);
      }
      return (string) null;
    }

    private string GenerateKeyWithGuid(Guid? guid) => string.Format("{0} {1} {2} {3} {4}", (object) this.PipelineType, (object) this.ProjectGuid, (object) this.PlanId, (object) this.TimelineId, (object) guid);
  }
}
