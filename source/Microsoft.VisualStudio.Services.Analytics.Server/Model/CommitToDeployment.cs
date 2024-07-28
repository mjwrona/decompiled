// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.CommitToDeployment
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
  [ODataHide(0, 3)]
  [Table("AnalyticsModel.vw_CommitToDeployment")]
  [ModelTableMapping("Model.CommitToDeployment")]
  [DatabaseHide(0, 50)]
  [LocalizedDisplayName("ENTITY_SET_NAME_COMMIT_TO_DEPLOYMENT", false)]
  public class CommitToDeployment : 
    IPartitionScoped,
    IProjectScoped,
    IProjectNavigate,
    IContinuation<long>
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [IgnoreDataMember]
    public long SkipToken { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public long CommitToDeploymentSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? CommitDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMIT_DATE", false)]
    public DateTimeOffset? CommitDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,CommitDateSK")]
    public CalendarDate CommitOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PipelineSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE", false)]
    public Pipeline Pipeline { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_RUN_ID", false)]
    public int PipelineRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PipelineJobSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,PipelineJobSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_JOB", false)]
    [NavigationRequiresPermissionCheck]
    public PipelineJob PipelineJob { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int EnvironmentSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,EnvironmentSK")]
    [NavigationRequiresPermissionCheck]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_ENVIRONMENT", false)]
    public PipelineEnvironment PipelineEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMMIT_TO_ENVIRONMENT_DURATION_SECONDS", false)]
    public Decimal? CommitToEnvironmentDurationSeconds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? JobStartedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_STARTED_DATE", false)]
    public DateTimeOffset? JobStartedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,JobStartedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_STARTED_ON", false)]
    public CalendarDate JobStartedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? JobEndedDateSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_END_DATE", false)]
    public DateTimeOffset? JobEndedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,JobEndedDateSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_ENDED_ON", false)]
    public CalendarDate JobEndedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
