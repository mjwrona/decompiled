// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestResult
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_TEST_RESULTS")]
  [ODataHide(0, 2)]
  [DatabaseHide(0, 21)]
  [Table("AnalyticsModel.vw_TestResult")]
  [ModelTableMapping("Model.TestResult")]
  public class TestResult : IPartitionScoped, IProjectNavigate, IProjectScoped, IContinuation<long>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [IgnoreDataMember]
    public long SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    public Project Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public long TestResultSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RESULT_ID", false)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? TestRunSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RUN_ID", false)]
    public int? TestRunId { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TestRunSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    public TestRun TestRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_OUTCOME", false)]
    [Column(TypeName = "int")]
    public TestOutcome? Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Required]
    public int? TestSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,TestSK")]
    [Required]
    [QueryShapeAllowFlattenAnnotationAttribute]
    public Test Test { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 47)]
    public int? PipelineRunSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineRunSK")]
    [QueryShapeAllowFlattenAnnotationAttribute]
    [DatabaseHide(0, 47)]
    public PipelineRun PipelineRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [IgnoreDataMember]
    [DatabaseHide(0, 47)]
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
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DURATION_SECONDS", false)]
    public Decimal? DurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_TEST_RUN_TYPE", false)]
    public Microsoft.VisualStudio.Services.Analytics.Model.TestRunType? TestRunType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_WORKFLOW", false)]
    public SourceWorkflow? Workflow { get; set; }

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

    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_FLAKY", false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 48)]
    public bool? IsFlaky { get; set; }
  }
}
