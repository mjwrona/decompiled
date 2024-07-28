// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.BoardLocation
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
  [DisplayNameAnnotation("ENTITY_SET_NAME_BOARD_LOCATIONS")]
  [Table("AnalyticsModel.vw_BoardLocation")]
  [ModelTableMapping("Model.BoardLocation")]
  public class BoardLocation : IPartitionScoped, IProjectScoped, ITeamScoped
  {
    [IgnoreDataMember]
    public int PartitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ProjectSK { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public int BoardLocationSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COLUMN_ID", false)]
    public Guid? ColumnId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COLUMN_NAME", false)]
    public string ColumnName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COLUMN_ORDER", false)]
    public int? ColumnOrder { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COLUMN_ITEM_LIMIT", false)]
    public int? ColumnItemLimit { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_DONE", false)]
    public bool? IsDone { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BOARD_ID", false)]
    public Guid? BoardId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BOARD_CATEGORY_REFERENCE_NAME", false)]
    public string BoardCategoryReferenceName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BOARD_NAME", false)]
    public string BoardName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BOARD_LEVEL", false)]
    public int? BoardLevel { get; set; }

    [DatabaseHide(0, 23)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BACKLOG_TYPE", false)]
    public string BacklogType { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_BOARD_VISIBLE", false)]
    public bool IsBoardVisible { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LANE_ID", false)]
    public Guid? LaneId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LANE_NAME", false)]
    public string LaneName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LANE_ORDER", false)]
    public int? LaneOrder { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_COLUMN_SPLIT", false)]
    public bool? IsColumnSplit { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CHANGED_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset? ChangedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVISED_DATE", false)]
    [SuppressDateConsistencyCheck("SK column does not exists in the database at this point.")]
    public DateTimeOffset? RevisedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_CURRENT", false)]
    public bool? IsCurrent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DONE", false)]
    public BoardColumnSplit Done { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? TeamSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, TeamSK")]
    public Team Team { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    [DatabaseHide(0, 41)]
    [ODataHide(0, 2)]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_DEFAULT_LANE", false)]
    public bool IsDefaultLane { get; set; }

    [DataMember(IsRequired = false)]
    [ODataHide(0, 2)]
    [ModelTableMapping("Model.WorkItemBoardLocation")]
    public ICollection<WorkItem> WorkItems { get; set; }
  }
}
