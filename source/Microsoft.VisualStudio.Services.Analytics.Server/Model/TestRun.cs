// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestRun
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_RUNS")]
  [ODataHide(0, 2)]
  [DatabaseHide(0, 21)]
  [Table("AnalyticsModel.vw_TestRun")]
  [ModelTableMapping("Model.TestRun")]
  public class TestRun : IPartitionScoped, IProjectNavigate, IProjectScoped, IContinuation<int>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    public Project Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int TestRunSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RUN_ID", false)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TITLE", false)]
    public string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_AUTOMATED", false)]
    public bool? IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RUN_TYPE", false)]
    public Microsoft.VisualStudio.Services.Analytics.Model.TestRunType? TestRunType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WORKFLOW", false)]
    public SourceWorkflow? Workflow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 47)]
    public int? PipelineRunSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineRunSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [DatabaseHide(0, 47)]
    public PipelineRun PipelineRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 47)]
    [IgnoreDataMember]
    public int? PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [NavigationRequiresPermissionCheck]
    [DatabaseHide(0, 47)]
    public Pipeline Pipeline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? BranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,BranchSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [NavigationRequiresPermissionCheck]
    public Branch Branch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CompletedDateSK { get; set; }

    [ForeignKey("PartitionId,CompletedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate CompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_DATE", false)]
    public DateTimeOffset? CompletedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? StartedDateSK { get; set; }

    [ForeignKey("PartitionId,StartedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate StartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STARTED_DATE", false)]
    public DateTimeOffset? StartedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RUN_DURATION_SECONDS", false)]
    public Decimal? RunDurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS", false)]
    public Decimal? ResultDurationSeconds { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_COUNT", false)]
    public int? ResultCount { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_PASS_COUNT", false)]
    [Column("ResultOutcomePassedCount")]
    public int? ResultPassCount { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_FAIL_COUNT", false)]
    [Column("ResultOutcomeFailedCount")]
    public int? ResultFailCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NONE_COUNT", false)]
    [Column("ResultOutcomeNoneCount")]
    public int? ResultNoneCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT", false)]
    [Column("ResultOutcomeInconclusiveCount")]
    public int? ResultInconclusiveCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT", false)]
    [Column("ResultOutcomeTimeoutCount")]
    public int? ResultTimeoutCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT", false)]
    [Column("ResultOutcomeAbortedCount")]
    public int? ResultAbortedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT", false)]
    [Column("ResultOutcomeBlockedCount")]
    public int? ResultBlockedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT", false)]
    [Column("ResultOutcomeNotExecutedCount")]
    public int? ResultNotExecutedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_WARNING_COUNT", false)]
    [Column("ResultOutcomeWarningCount")]
    public int? ResultWarningCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_ERROR_COUNT", false)]
    [Column("ResultOutcomeErrorCount")]
    public int? ResultErrorCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT", false)]
    [Column("ResultOutcomeNotApplicableCount")]
    public int? ResultNotApplicableCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT", false)]
    [Column("ResultOutcomeNotImpactedCount")]
    public int? ResultNotImpactedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_HAS_DETAIL", false)]
    public bool HasDetail { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleaseId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleaseEnvironmentId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleasePipelineId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_STAGE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleaseStageId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 49)]
    public int? ResultFlakyCount { get; set; }
  }
}
