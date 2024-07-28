// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Pipeline
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_PIPELINES")]
  [ODataHide(0, 2)]
  [Table("AnalyticsModel.vw_Pipeline")]
  [DisableProjectFiltering]
  [ModelTableMapping("Model.BuildPipeline")]
  [DatabaseHide(0, 47)]
  public class Pipeline : IPartitionScoped, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId,ProjectSK")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PROJECT", false)]
    public Project Project { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [VisibleCrossProject]
    public int PipelineSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_ID", false)]
    [VisibleCrossProject]
    public int? PipelineId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_NAME", false)]
    [VisibleCrossProject]
    public string PipelineName { get; set; }

    [DatabaseHide(0, 38)]
    [ODataHideByServiceVersion(0, 38)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_VERSION", false)]
    public int? PipelineVersion { get; set; }

    [DatabaseHide(0, 38)]
    [ODataHideByServiceVersion(0, 38)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_PIPELINE_PROCESS_TYPE", false)]
    [Column(TypeName = "int")]
    public Microsoft.VisualStudio.Services.Analytics.Model.PipelineProcessType? PipelineProcessType { get; set; }

    [IgnoreDataMember]
    [DatabaseHide(0, 38)]
    [ODataHideByServiceVersion(0, 38)]
    public bool IsDeleted { get; set; }
  }
}
