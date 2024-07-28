// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TaskAgentRequestSnapshot
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_TASKAGENTREQUESTSNAPSHOT")]
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_TaskAgentRequestInstantRunningCount")]
  [ModelTableMapping("Model.TaskAgentRequest")]
  [ModelTableMapping("Model.Date")]
  [ModelTableMapping("Internal.Time")]
  [DatabaseHide(0, 54)]
  public class TaskAgentRequestSnapshot : IPartitionScoped, IProjectNavigate, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int SamplingDateSK { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_SAMPLING_HOUR", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int SamplingHour { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SAMPLING_TIME", false)]
    [SuppressDateConsistencyCheck("Date navigation property exists.")]
    public virtual DateTimeOffset SamplingTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUED_DATE", false)]
    [DatabaseHide(0, 56)]
    public DateTimeOffset? QueuedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 56)]
    public int? QueuedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,QueuedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUED_ON", false)]
    [DatabaseHide(0, 56)]
    public CalendarDate QueuedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STARTED_DATE", false)]
    [DatabaseHide(0, 56)]
    public DateTimeOffset? StartedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 56)]
    public int? StartedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,StartedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STARTED_ON", false)]
    [DatabaseHide(0, 56)]
    public CalendarDate StartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_END_DATE", false)]
    [DatabaseHide(0, 56)]
    public DateTimeOffset? FinishedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 56)]
    public int? FinishedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,FinishedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_ENDED_ON", false)]
    [DatabaseHide(0, 56)]
    public CalendarDate FinishedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS", false)]
    [DatabaseHide(0, 56)]
    public Decimal? QueueDurationSeconds { get; set; }

    [DatabaseHide(0, 55)]
    [DataMember(IsRequired = false)]
    public Guid? ProjectSK { get; set; }

    [DatabaseHide(0, 55)]
    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [IgnoreDataMember]
    [DatabaseHide(0, 56)]
    public int? PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [NavigationRequiresPermissionCheck]
    [DatabaseHide(0, 56)]
    public Pipeline Pipeline { get; set; }

    [DatabaseHide(0, 55)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REQUEST_ID", false)]
    public long RequestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_POOL_ID", false)]
    public int? PoolId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_TYPE", false)]
    public string PipelineType { get; set; }

    [DatabaseHide(0, 56)]
    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_HOSTED", false)]
    public bool? IsHosted { get; set; }

    [DatabaseHide(0, 56)]
    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_RUNNING", false)]
    public bool? IsRunning { get; set; }

    [DatabaseHide(0, 56)]
    [DataMember(IsRequired = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_QUEUED", false)]
    public bool? IsQueued { get; set; }
  }
}
