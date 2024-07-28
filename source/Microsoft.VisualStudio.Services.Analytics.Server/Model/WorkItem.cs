// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.WorkItem
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [DisplayNameAnnotation("ENTITY_SET_NAME_WORK_ITEMS")]
  [DataContract]
  [Table("AnalyticsModel.vw_WorkItem")]
  [Expand(new string[] {"Parent", "Children"}, MaxDepth = 2)]
  [Page(MaxTop = 2147483647)]
  [ModelTableMapping("Model.WorkItem")]
  [ModelTableMapping("Model.WorkItemHistory")]
  public class WorkItem : WorkItemCommon, IIncremental
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [Key]
    public override int WorkItemId { get; set; }

    [DataMember(IsRequired = false)]
    [ModelTableMapping("Model.WorkItemBoardLocation")]
    public ICollection<BoardLocation> BoardLocations { get; set; }

    [DataMember(IsRequired = false)]
    [QueryShapeAllowAnyAllAnnotation]
    [ModelTableMapping("Model.TeamToTeamField")]
    public ICollection<Team> Teams { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_IN_PROGRESS_DATE", false)]
    public override DateTimeOffset? InProgressDate { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_COMPLETED_DATE", false)]
    public override DateTimeOffset? CompletedDate { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_LEAD_TIME_DAYS", false)]
    public override double? LeadTimeDays { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [LocalizedDisplayName("ENTITY_FIELD_NAME_CYCLE_TIME_DAYS", false)]
    public override double? CycleTimeDays { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public override int? InProgressDateSK { get; set; }

    [DatabaseHide(0, 0)]
    [ForeignKey("PartitionId,InProgressDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public override CalendarDate InProgressOn { get; set; }

    [DatabaseHide(0, 0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public override int? CompletedDateSK { get; set; }

    [DatabaseHide(0, 0)]
    [ForeignKey("PartitionId,CompletedDateSK")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public override CalendarDate CompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [SuppressDisplayName("Property used for incremental load.")]
    public DateTimeOffset? AnalyticsUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [InverseProperty("WorkItem")]
    [NotExpandable]
    [ModelTableMapping("Model.WorkItemHistory")]
    public ICollection<WorkItemRevision> Revisions { get; set; }

    [DataMember(IsRequired = false)]
    [InverseProperty("SourceWorkItem")]
    [ModelTableMapping("Model.WorkItemLinkHistory")]
    public ICollection<WorkItemLink> Links { get; set; }

    [DataMember(IsRequired = false)]
    [NavigationRequiresPermissionCheck]
    [Expand(MaxDepth = 1)]
    [ModelTableMapping("Model.WorkItemLinkHistory")]
    public ICollection<WorkItem> Children { get; set; }

    [ForeignKey("PartitionId, ParentWorkItemId")]
    [DataMember(IsRequired = false)]
    [NavigationRequiresPermissionCheck]
    [Expand(MaxDepth = 1)]
    public WorkItem Parent { get; set; }

    [ODataHide(0, 1)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [ModelTableMapping("Model.WorkItemProcess")]
    public ICollection<Process> Processes { get; set; }

    [DatabaseHide(0, 40)]
    [ODataHideByServiceVersion(0, 40)]
    [DataMember(IsRequired = false)]
    [NavigationRequiresPermissionCheck]
    [Expand(MaxDepth = 1)]
    [ODataHide(0, 2)]
    [ModelTableMapping("Model.WorkItemDescendantHistory")]
    public ICollection<WorkItem> Descendants { get; set; }

    [BoundFunction(3, 2147483647)]
    public IQueryable<Tag> PredictTags() => Enumerable.Range(1, 2).Select<int, Tag>((Func<int, Tag>) (i => new Tag()
    {
      TagId = new Guid?(Guid.NewGuid()),
      TagName = string.Format("Tag{0}", (object) i)
    })).AsQueryable<Tag>();

    [BoundFunction(3, 2147483647)]
    public double? Predict(PredictModel model) => new double?(-42.0);
  }
}
