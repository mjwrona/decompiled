// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Area
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_AREAS")]
  [Table("AnalyticsModel.vw_Area")]
  [ModelTableMapping("Model.Area")]
  public class Area : IPartitionScoped, IProjectNavigate, IProjectScoped
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
    public Guid AreaSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_ID", false)]
    public Guid AreaId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_NAME", false)]
    public string AreaName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Legacy property that is useful only at query time.")]
    public int? Number { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_PATH", false)]
    public string AreaPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_1", false)]
    public string AreaLevel1 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_2", false)]
    public string AreaLevel2 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_3", false)]
    public string AreaLevel3 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_4", false)]
    public string AreaLevel4 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_5", false)]
    public string AreaLevel5 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_6", false)]
    public string AreaLevel6 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_7", false)]
    public string AreaLevel7 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_8", false)]
    public string AreaLevel8 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_9", false)]
    public string AreaLevel9 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_10", false)]
    public string AreaLevel10 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_11", false)]
    public string AreaLevel11 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_12", false)]
    public string AreaLevel12 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_13", false)]
    public string AreaLevel13 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_AREA_LEVEL_14", false)]
    public string AreaLevel14 { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DEPTH", false)]
    public int? Depth { get; set; }

    [IgnoreDataMember]
    public bool IsDeleted { get; set; }

    [IgnoreDataMember]
    public int SkipToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.TeamArea")]
    public ICollection<Team> Teams { get; set; }
  }
}
