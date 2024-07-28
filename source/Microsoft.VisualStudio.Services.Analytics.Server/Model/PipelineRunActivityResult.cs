// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.PipelineRunActivityResult
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [ODataHide(0, 2)]
  [Table("AnalyticsModel.vw_PipelineRunActivityResult_v2")]
  [ModelTableMapping("Model.BuildTaskResult")]
  [DatabaseHide(0, 47)]
  [LocalizedDisplayName("ENTITY_SET_NAME_PIPELINE_RUN_ACTIVITY_RESULTS", false)]
  public class PipelineRunActivityResult : 
    IPartitionScoped,
    IProjectScoped,
    IProjectNavigate,
    IContinuation<long>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public long PipelineRunActivityResultSK { get; set; }

    [IgnoreDataMember]
    public long SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_ID", false)]
    public int? PipelineRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE", false)]
    public Pipeline Pipeline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineTaskSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineTaskSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_TASK", false)]
    public PipelineTask PipelineTask { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 42)]
    public int? PipelineJobSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineJobSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_JOB", false)]
    [NavigationRequiresPermissionCheck]
    [DatabaseHide(0, 42)]
    public PipelineJob PipelineJob { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? BranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,BranchSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BRANCH", false)]
    public Branch Branch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineRunQueuedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineRunQueuedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_QUEUED_ON", false)]
    public CalendarDate PipelineRunQueuedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineRunStartedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineRunStartedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_STARTED_ON", false)]
    public CalendarDate PipelineRunStartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineRunCompletedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineRunCompletedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_COMPLETED_ON", false)]
    public CalendarDate PipelineRunCompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ActivityStartedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ActivityStartedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY_STARTED_ON", false)]
    public CalendarDate ActivityStartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY_STARTED_DATE", false)]
    public DateTimeOffset? ActivityStartedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? ActivityCompletedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ActivityCompletedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_ON", false)]
    public CalendarDate ActivityCompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_DATE", false)]
    public DateTimeOffset? ActivityCompletedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TASK_DISPLAY_NAME", false)]
    public string TaskDisplayName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TASK_LOG_PATH", false)]
    public string TaskLogPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TASK_RESULT", false)]
    [Column(TypeName = "int")]
    public PipelineRunTaskOutcome? TaskOutcome { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUCCEEDED_COUNT", false)]
    public int? SucceededCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUCCEEDED_WITH_ISSUES_COUNT", false)]
    public int? SucceededWithIssuesCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FAILED_COUNT", false)]
    public int? FailedCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CANCELED_COUNT", false)]
    public int? CanceledCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SKIPPED_COUNT", false)]
    public int? SkippedCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ABANDONED_COUNT", false)]
    public int? AbandonedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ACTIVITY_DURATION_SECONDS", false)]
    public Decimal? ActivityDurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Column(TypeName = "int")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_OUTCOME", false)]
    public Microsoft.VisualStudio.Services.Analytics.Model.PipelineRunOutcome? PipelineRunOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Column(TypeName = "int")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_ACTIVITY_TYPE", false)]
    [DatabaseHide(0, 48)]
    public PipelineActivityType? ActivityType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELATIVE_START_TIME_JOB_SECONDS", false)]
    [DatabaseHide(0, 48)]
    public int? RelativeStartTimeFromJobSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELATIVE_START_TIME_STAGE_SECONDS", false)]
    [DatabaseHide(0, 48)]
    public int? RelativeStartTimeFromStageSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELATIVE_START_TIME_RUN_SECONDS", false)]
    [DatabaseHide(0, 48)]
    public int? RelativeStartTimeFromRunSeconds { get; set; }

    [IgnoreDataMember]
    public int? PlanId { get; set; }

    [IgnoreDataMember]
    public int? TimelineId { get; set; }

    [IgnoreDataMember]
    public Guid? TimelineRecordId { get; set; }
  }
}
