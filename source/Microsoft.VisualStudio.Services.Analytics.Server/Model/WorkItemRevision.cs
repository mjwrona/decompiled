// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItemRevision
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DisplayNameAnnotation("ENTITY_SET_NAME_WORK_ITEM_REVISIONS")]
  [DebuggerDisplay("{WorkItemId} : {Revision} : {Title}")]
  [DataContract]
  [Table("AnalyticsModel.vw_WorkItemRevision")]
  [ModelTableMapping("Model.WorkItem")]
  [ModelTableMapping("Model.WorkItemHistory")]
  public class WorkItemRevision : 
    WorkItemCommon,
    IHistoricalWorkItem,
    IIncremental,
    ICurrentProjectNavigate,
    ICurrentProjectScoped
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public override int WorkItemId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public override int? Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ReferenceName("System.RevisedDate")]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_REVISED_DATE", false)]
    public DateTimeOffset? RevisedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? RevisedDateSK { get; set; }

    [ForeignKey("PartitionId,RevisedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate RevisedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? DateSK { get; set; }

    [ForeignKey("PartitionId, DateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CalendarDate Date { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.WorkItemBoardLocation")]
    public ICollection<BoardLocation> BoardLocations { get; set; }

    [DataMember(IsRequired = false)]
    [QueryShapeAllowAnyAllAnnotation]
    [ModelTableMapping("Model.TeamToTeamField")]
    public ICollection<Team> Teams { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_CURRENT", false)]
    public virtual bool IsCurrent { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IS_LAST_REVISION_OF_DAY", false)]
    public bool IsLastRevisionOfDay { get; set; }

    [DatabaseHide(0, 19)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for client-side snapshot construction.")]
    public Period IsLastRevisionOfPeriod { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = true)]
    [ForeignKey("PartitionId, WorkItemId")]
    [InverseProperty("Revisions")]
    [NavigationRequiresPermissionCheck]
    public WorkItem WorkItem { get; set; }

    [IgnoreDataMember]
    [SuppressDataMemberCheck("CurrentProjectSK is only used for filter and should not be exposed")]
    public Guid? CurrentProjectSK { get; set; }

    [IgnoreDataMember]
    [ForeignKey("PartitionId, CurrentProjectSK")]
    public Project CurrentProject { get; set; }

    [ODataHide(0, 1)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.WorkItemProcess")]
    public ICollection<Process> Processes { get; set; }
  }
}
