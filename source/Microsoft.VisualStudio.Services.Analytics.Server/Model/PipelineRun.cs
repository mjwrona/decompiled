// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.PipelineRun
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_PIPELINE_RUNS")]
  [ODataHide(0, 2)]
  [Table("AnalyticsModel.vw_PipelineRun")]
  [DisableProjectFiltering]
  [ModelTableMapping("Model.Build")]
  [DatabaseHide(0, 47)]
  public class PipelineRun : IPartitionScoped, IProjectScoped, IContinuation<int>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    [VisibleCrossProject]
    public int PipelineRunSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_ID", false)]
    [VisibleCrossProject]
    public int PipelineRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_ID", false)]
    [VisibleCrossProject]
    public int? PipelineId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE", false)]
    public Pipeline Pipeline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? BranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,BranchSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BRANCH", false)]
    public Branch Branch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RUN_NUMBER", false)]
    public string RunNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_NUMBER_REVISION", false)]
    public int? RunNumberRevision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RUN_REASON", false)]
    public PipelineRunReason? RunReason { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RUN_OUTCOME", false)]
    public PipelineRunOutcome? RunOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUED_DATE", false)]
    public DateTimeOffset? QueuedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? QueuedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,QueuedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUED_ON", false)]
    public CalendarDate QueuedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STARTED_DATE", false)]
    public DateTimeOffset? StartedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? StartedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,StartedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STARTED_ON", false)]
    public CalendarDate StartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_DATE", false)]
    public DateTimeOffset? CompletedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CompletedDateSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,CompletedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_ON", false)]
    public CalendarDate CompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RUN_DURATION_SECONDS", false)]
    public Decimal? RunDurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS", false)]
    public Decimal? QueueDurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TOTAL_DURATION_SECONDS", false)]
    public Decimal? TotalDurationSeconds { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_SUCCEEDED_COUNT", false)]
    public int? SucceededCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PARTIALLY_SUCCEEDED_COUNT", false)]
    public int? PartiallySucceededCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FAILED_COUNT", false)]
    public int? FailedCount { get; set; }

    [IgnoreDataMember]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CANCELED_COUNT", false)]
    public int? CanceledCount { get; set; }
  }
}
