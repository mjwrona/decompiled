// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemBoardSnapshot
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [Table("AnalyticsModel.vw_WorkItemBoardSnapshot")]
  [ModelTableMapping("Model.WorkItemBoardLocation")]
  [ModelTableMapping("Model.BoardLocation")]
  [ModelTableMapping("Model.Date")]
  public class WorkItemBoardSnapshot : 
    WorkItemCommon,
    IHistoricalWorkItem,
    ISnapshot,
    ICurrentProjectNavigate,
    ICurrentProjectScoped
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public override int WorkItemId { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int DateSK { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DATE_VALUE", false)]
    [SuppressDateConsistencyCheck("Date navigation property exists.")]
    public virtual DateTimeOffset DateValue { get; set; }

    [ForeignKey("PartitionId, DateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate Date { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used mainly for filtering.")]
    public Period IsLastDayOfPeriod { get; set; }

    [Key]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int? BoardLocationSK { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ForeignKey("PartitionId, BoardLocationSK")]
    public BoardLocation BoardLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? TeamSK { get; set; }

    [DataMember(IsRequired = false)]
    [ForeignKey("PartitionId, TeamSK")]
    public Team Team { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COLUMN_ID", false)]
    public Guid ColumnId { get; set; }

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

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_BOARD_ID", false)]
    public Guid BoardId { get; set; }

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

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LANE_ID", false)]
    public Guid LaneId { get; set; }

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
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_CURRENT", false)]
    public bool? IsCurrent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_DONE", false)]
    public BoardColumnSplit Done { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_DEFAULT_LANE", false)]
    public bool IsDefaultLane { get; set; }

    [IgnoreDataMember]
    [SuppressDataMemberCheck("CurrentProjectSK is only used for filter and should not be exposed")]
    public Guid? CurrentProjectSK { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, CurrentProjectSK")]
    public Project CurrentProject { get; set; }
  }
}
