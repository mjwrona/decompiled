// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.PipelineJob
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
  [Table("AnalyticsModel.vw_PipelineJob")]
  [DatabaseHide(0, 42)]
  [ModelTableMapping("Model.PipelineJob")]
  public class PipelineJob : IPartitionScoped, IProjectScoped, IProjectNavigate
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [ModelTableMapping("Model.Project")]
    public Project Project { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [VisibleCrossProject]
    public int PipelineJobSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_ID", false)]
    [VisibleCrossProject]
    [DatabaseHide(0, 47)]
    public int? PipelineId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DatabaseHide(0, 47)]
    public int PipelineSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STAGE_NAME", false)]
    public string StageName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_NAME", false)]
    public string JobName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_FULL_JOB_NAME", false)]
    public string FullJobName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STRATEGY_ATTRIBUTES", false)]
    public string StrategyAttributes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_STAGE_IDENTIFIER", false)]
    [DatabaseHide(0, 48)]
    [Column("StageIdentifier")]
    public string StageId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_JOB_IDENTIFIER", false)]
    [DatabaseHide(0, 48)]
    [Column("JobIdentifier")]
    public string JobId { get; set; }
  }
}
