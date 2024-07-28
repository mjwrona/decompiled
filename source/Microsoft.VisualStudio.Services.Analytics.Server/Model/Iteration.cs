// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Iteration
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DataContract]
  [DisplayNameAnnotation("ENTITY_SET_NAME_ITERATIONS")]
  [Table("AnalyticsModel.vw_Iteration")]
  [ModelTableMapping("Model.Iteration")]
  public class Iteration : IPartitionScoped, IProjectNavigate, IProjectScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, ProjectSK")]
    public Project Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public Guid IterationSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_ID", false)]
    public Guid IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_NAME", false)]
    public string IterationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Legacy property that is useful only at query time.")]
    public int? Number { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_PATH", false)]
    public string IterationPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_START_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset? StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_END_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset? EndDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_1", false)]
    public string IterationLevel1 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_2", false)]
    public string IterationLevel2 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_3", false)]
    public string IterationLevel3 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_4", false)]
    public string IterationLevel4 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_5", false)]
    public string IterationLevel5 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_6", false)]
    public string IterationLevel6 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_7", false)]
    public string IterationLevel7 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_8", false)]
    public string IterationLevel8 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_9", false)]
    public string IterationLevel9 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_10", false)]
    public string IterationLevel10 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_11", false)]
    public string IterationLevel11 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_12", false)]
    public string IterationLevel12 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_13", false)]
    public string IterationLevel13 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_ITERATION_LEVEL_14", false)]
    public string IterationLevel14 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DEPTH", false)]
    public int Depth { get; set; }

    [IgnoreDataMember]
    public bool IsDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_ENDED", false)]
    public bool IsEnded { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.TeamIteration")]
    public ICollection<Team> Teams { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }
  }
}
