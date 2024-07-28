// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestResultDaily
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_RESULTS_DAILY")]
  [ODataHide(0, 2)]
  [Table("AnalyticsModel.vw_TestResultDaily")]
  [ModelTableMapping("Model.TestResultDaily")]
  public class TestResultDaily : 
    IPartitionScoped,
    IProjectNavigate,
    IProjectScoped,
    IContinuation<long>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [IgnoreDataMember]
    public long SkipToken { get; set; }

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
    public long TestResultDailySK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Required]
    public int? TestSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TestSK")]
    [Required]
    [QueryShapeAllowFlattenAnnotationAttribute]
    public Test Test { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [NavigationRequiresPermissionCheck]
    [DatabaseHide(0, 46)]
    public Pipeline Pipeline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? BranchSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,BranchSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [NavigationRequiresPermissionCheck]
    public Branch Branch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? DateSK { get; set; }

    [ForeignKey("PartitionId,DateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate Date { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS", false)]
    public Decimal? ResultDurationSeconds { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_COUNT", false)]
    public int? ResultCount { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_PASS_COUNT", false)]
    public int? ResultPassCount { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_FAIL_COUNT", false)]
    public int? ResultFailCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT", false)]
    [DatabaseHide(0, 48)]
    public int? ResultFlakyCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NONE_COUNT", false)]
    public int? ResultNoneCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT", false)]
    public int? ResultInconclusiveCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT", false)]
    public int? ResultTimeoutCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT", false)]
    public int? ResultAbortedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT", false)]
    public int? ResultBlockedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT", false)]
    public int? ResultNotExecutedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_WARNING_COUNT", false)]
    public int? ResultWarningCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_ERROR_COUNT", false)]
    public int? ResultErrorCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT", false)]
    public int? ResultNotApplicableCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT", false)]
    public int? ResultNotImpactedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RUN_TYPE", false)]
    public Microsoft.VisualStudio.Services.Analytics.Model.TestRunType? TestRunType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WORKFLOW", false)]
    public SourceWorkflow? Workflow { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleasePipelineId { get; set; }

    [LocalizedDisplayName("ENTITY_FIELD_NAME_RELEASE_STAGE_ID", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 46)]
    public int? ReleaseStageId { get; set; }
  }
}
